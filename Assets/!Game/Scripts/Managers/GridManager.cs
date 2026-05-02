using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class GridManager : MonoBehaviour
{
	public static GridManager Instance { get; private set; }

	[Header("Grid Ayarları")]
	
	public GameObject cellPrefab;
	public float spacing = 15f;
	public Vector2 CellSize { get; private set; }
	private int CurrentSize
	{
		get
		{
			// Oyun çalışıyorsa Singleton üzerinden, en performanslı şekilde çek
			if (Application.isPlaying && GameManager.Instance != null)
				return GameManager.Instance.gridSize;

			// Editörde (oyun kapalıyken) "Generate Grid" yaparsak sahneden bulup çek
			GameManager gm = FindObjectOfType<GameManager>();
			return gm != null ? gm.gridSize : 4; // Bulamazsa varsayılan 4 döndür
		}
	}
	// UI Bileşenleri
	private GridLayoutGroup gridLayout;
	private RectTransform rectTransform;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		gridLayout = GetComponent<GridLayoutGroup>();
		rectTransform = GetComponent<RectTransform>();
	}

	
	[ContextMenu("Generate Grid")]
	public void GenerateGrid()
	{
		if (gridLayout == null || rectTransform == null)
		{
			gridLayout = GetComponent<GridLayoutGroup>();
			rectTransform = GetComponent<RectTransform>();
		}

		ClearGrid();
		SetupLayout();
		SpawnCells();
	}

	private void SetupLayout()
	{

		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

	
		gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayout.constraintCount = CurrentSize;
		gridLayout.spacing = new Vector2(spacing, spacing);
		gridLayout.childAlignment = TextAnchor.MiddleCenter;

		
		float totalWidth = rectTransform.rect.width;

	
		if (totalWidth <= 0) totalWidth = 1000f;

		float paddingSum = gridLayout.padding.left + gridLayout.padding.right;
		float totalSpacing = spacing * (CurrentSize - 1);

		float rawCellSize = (totalWidth - paddingSum - totalSpacing) / CurrentSize;
		float finalCellSize = rawCellSize - 0.5f;

		gridLayout.cellSize = new Vector2(finalCellSize, finalCellSize);
		CellSize = gridLayout.cellSize;
	}

	private void SpawnCells()
	{
		int totalCells = CurrentSize * CurrentSize;
		for (int i = 0; i < totalCells; i++)
		{
			GameObject newCell = Instantiate(cellPrefab, transform);

			newCell.name = $"Cell_{i / CurrentSize}_{i % CurrentSize}";
		}
	}

	private void ClearGrid()
	{
		foreach (Transform child in transform)
		{
			if (Application.isPlaying)
			{
				Destroy(child.gameObject);
			}
			else
			{
				DestroyImmediate(child.gameObject);
			}
		}
	}
}