using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Listeler için eklendi

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(Image))]
public class MiniGridVisualizer : MonoBehaviour
{
	[Header("Grid Ayarları")]
	[Range(3, 8)]
	public int gridSize = 4;
	public float spacing = 5f;
	public int padding = 10;

	[Header("Görsel Ayarları")]
	[Tooltip("Bu modda kaç tane kutu renkli/dolu görünsün?")]
	public int filledCellCount = 5; // İSTEDİĞİN ÖZELLİK BURADA

	[Header("Referanslar")]
	public GameObject miniCellPrefab; // İçinde TextMeshProUGUI olan prefab

	void Start()
	{
		GenerateMiniGrid();
	}

	[ContextMenu("Gridi Olustur (Editörde Test İçin)")]
	public void GenerateMiniGrid()
	{
		ColorUtility.TryParseHtmlString("#BBADA0", out Color boardColor);
		ColorUtility.TryParseHtmlString("#CDC1B4", out Color emptyCellColor);

		// Ana arka planı boya
		Image bgImage = GetComponent<Image>();
		if (bgImage != null) bgImage.color = boardColor;

		// Düzeni kur
		GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = gridSize;
		grid.spacing = new Vector2(spacing, spacing);
		grid.padding = new RectOffset(padding, padding, padding, padding);
		grid.childAlignment = TextAnchor.MiddleCenter;

		// Eski hücreleri temizle
		foreach (Transform child in transform)
		{
			if (Application.isPlaying) Destroy(child.gameObject);
			else DestroyImmediate(child.gameObject);
		}

		// Boyutları hesapla
		RectTransform rectTransform = GetComponent<RectTransform>();
		Canvas.ForceUpdateCanvases();

		float width = rectTransform.rect.width > 0 ? rectTransform.rect.width : 200f;
		float totalSpacing = (spacing * (gridSize - 1)) + (padding * 2);
		float cellSize = (width - totalSpacing) / gridSize;
		grid.cellSize = new Vector2(cellSize, cellSize);

		// --- DOLU KUTULARI BELİRLEME MANTIĞI ---
		int totalCells = gridSize * gridSize;

		// Eğer grid'in alabileceğinden fazla sayı girersen, program çökmesin diye otomatik sınırlar (Örn: 3x3 için max 9 yapar)
		int safeFilledCount = Mathf.Clamp(filledCellCount, 0, totalCells);

		// 1'den toplam hücre sayısına kadar bir torba oluşturuyoruz
		List<int> availableIndices = new List<int>();
		for (int i = 0; i < totalCells; i++) availableIndices.Add(i);

		// Torbadan 'safeFilledCount' kadar rastgele şanslı numara (hücre indeksi) çekiyoruz
		List<int> chosenIndices = new List<int>();
		for (int i = 0; i < safeFilledCount; i++)
		{
			int r = Random.Range(0, availableIndices.Count);
			chosenIndices.Add(availableIndices[r]);
			availableIndices.RemoveAt(r); // Aynı kutu iki kere seçilmesin diye torbadan çıkarıyoruz
		}
		// ---------------------------------------

		// Hücreleri üret ve boya
		int[] possibleValues = { 2, 4, 8, 16, 32, 64 };

		for (int i = 0; i < totalCells; i++)
		{
			GameObject cellObj = Instantiate(miniCellPrefab, transform);
			cellObj.name = $"MiniCell_{i}";

			Image cellBg = cellObj.GetComponent<Image>();
			TextMeshProUGUI cellText = cellObj.GetComponentInChildren<TextMeshProUGUI>();

			// Eğer şu anki ürettiğimiz hücre, çektiğimiz şanslı numaralardan biriyse içini doldur
			if (chosenIndices.Contains(i))
			{
				int randomValue = possibleValues[Random.Range(0, possibleValues.Length)];

				// TileManager'dan rengi çek
				if (TileManager.Instance != null && TileManager.Instance.tileColors != null)
				{
					foreach (var colorData in TileManager.Instance.tileColors)
					{
						if (colorData.value == randomValue)
						{
							if (cellBg != null) cellBg.color = colorData.backgroundColor;
							if (cellText != null)
							{
								cellText.text = randomValue.ToString();
								cellText.color = colorData.textColor;
							}
							break;
						}
					}
				}
			}
			else // Şanslı değilse boş bırak
			{
				if (cellBg != null) cellBg.color = emptyCellColor;
				if (cellText != null) cellText.text = "";
			}
		}
	}
}