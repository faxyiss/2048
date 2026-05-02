using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

// Renk verilerini Unity Inspector'da görmek için bu yapıyı oluşturuyoruz
[System.Serializable]
public struct TileColorData
{
	public int value;
	public Color backgroundColor;
	public Color textColor;
}

public class TileManager : MonoBehaviour
{
	public static TileManager Instance { get; private set; }

	[Header("Referanslar")]
	public GameObject tilePrefab;
	public Transform tileContainer;

	[Header("Renk Ayarları")]
	public TileColorData[] tileColors; // Tüm renklerin tutulduğu dizi

	private Tile[,] activeTiles;

	void Awake()
	{
		if (Instance != null && Instance != this) Destroy(gameObject);
		else Instance = this;
	}

	public void InitializeGrid(int size)
	{
		activeTiles = new Tile[size, size];
	}

	// İstenen sayıya ait renk verisini bulan yardımcı metot
	private TileColorData GetColorData(int value)
	{
		// 1. Liste boşsa hata fırlatma, varsayılan bir renk dön
		if (tileColors == null || tileColors.Length == 0)
		{
			Debug.LogWarning("TileManager: Renk listesi boş! Lütfen Inspector'dan renk ekleyin.");
			return new TileColorData { value = 0, backgroundColor = Color.gray, textColor = Color.white };
		}

		foreach (var colorData in tileColors)
		{
			if (colorData.value == value) return colorData;
		}

		// 2. Sayı bulunamadıysa en son tanımlı rengi güvenle dön
		return tileColors[tileColors.Length - 1];
	}

	public void SpawnTile(int x, int y, int value)
	{
		int size = GameManager.Instance.gridSize;
		int index = (x * size) + y;
		Transform cellTransform = GridManager.Instance.transform.GetChild(index);

		GameObject newTileObj = Instantiate(tilePrefab, tileContainer);
		newTileObj.transform.position = cellTransform.position;
		newTileObj.name = $"Tile_{x}_{y}";

		RectTransform tileRect = newTileObj.GetComponent<RectTransform>();
		tileRect.sizeDelta = GridManager.Instance.CellSize;

		Tile tileScript = newTileObj.GetComponent<Tile>();

		// RENK VE DEĞER ATAMASI
		TileColorData colorData = GetColorData(value);
		tileScript.SetState(value, colorData.backgroundColor, colorData.textColor);

		newTileObj.transform.localScale = Vector3.zero;
		newTileObj.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

		activeTiles[x, y] = tileScript;
	}

	public async UniTask AnimateMovesAsync(List<MoveData> moves)
	{
		int size = GameManager.Instance.gridSize;
		List<UniTask> animationTasks = new List<UniTask>();
		Tile[,] nextTiles = new Tile[size, size];
		List<GameObject> tilesToDestroy = new List<GameObject>();

		foreach (var move in moves)
		{
			Tile tileToMove = activeTiles[move.FromX, move.FromY];
			if (tileToMove == null) continue;

			int targetIndex = (move.ToX * size) + move.ToY;
			Vector3 targetPos = GridManager.Instance.transform.GetChild(targetIndex).position;

			animationTasks.Add(tileToMove.MoveToAsync(targetPos));

			if (move.IsMerge)
			{
				tilesToDestroy.Add(tileToMove.gameObject);

				Tile baseTile = nextTiles[move.ToX, move.ToY];
				if (baseTile == null) baseTile = activeTiles[move.ToX, move.ToY];

				animationTasks.Add(UpdateMergedTileAsync(baseTile, move.NewValue));
			}
			else
			{
				nextTiles[move.ToX, move.ToY] = tileToMove;
			}
		}

		await UniTask.WhenAll(animationTasks);

		foreach (var obj in tilesToDestroy)
		{
			if (obj != null) Destroy(obj);
		}

		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (nextTiles[x, y] == null && activeTiles[x, y] != null)
				{
					bool hasMoved = moves.Exists(m => m.FromX == x && m.FromY == y);
					if (!hasMoved) nextTiles[x, y] = activeTiles[x, y];
				}
			}
		}

		activeTiles = nextTiles;
	}

	public void ClearAllTiles()
	{
		// tileContainer (taşların ebeveyni) içindeki tüm çocukları bul ve sil
		foreach (Transform child in tileContainer)
		{
			Destroy(child.gameObject);
		}
	}

	private async UniTask UpdateMergedTileAsync(Tile baseTile, int newValue)
	{
		await UniTask.Delay(System.TimeSpan.FromSeconds(0.15f));

		if (baseTile != null)
		{
			// BİRLEŞME SONRASI YENİ RENGİ ATAMA
			TileColorData colorData = GetColorData(newValue);
			baseTile.SetState(newValue, colorData.backgroundColor, colorData.textColor);

			baseTile.PlayMergeAnimationAsync().Forget();
		}
	}

	[ContextMenu("Setup Classic Colors")]
	public void SetupClassicColors()
	{
		tileColors = new TileColorData[]
		{
            // 2: Daha soft bir krem
            new TileColorData { value = 2,    backgroundColor = HexToColor("#EEE4DA"), textColor = HexToColor("#776E65") },
            // 4: Daha sıcak, sarımtırak ve belirgin bir ton
            new TileColorData { value = 4,    backgroundColor = HexToColor("#EDE0C8"), textColor = HexToColor("#776E65") },
			new TileColorData { value = 8,    backgroundColor = HexToColor("#F2B179"), textColor = HexToColor("#FFFFFF") },
            // 16: Daha koyu Turuncu
            new TileColorData { value = 16,   backgroundColor = HexToColor("#F59563"), textColor = HexToColor("#FFFFFF") },
            // 32: Mercan Kırmızısı
            new TileColorData { value = 32,   backgroundColor = HexToColor("#F67C5F"), textColor = HexToColor("#FFFFFF") },
            // ... devamı aynı kalabilir
            new TileColorData { value = 64,   backgroundColor = HexToColor("#F65E3B"), textColor = HexToColor("#FFFFFF") },
			new TileColorData { value = 128,  backgroundColor = HexToColor("#EDCF72"), textColor = HexToColor("#FFFFFF") },
			new TileColorData { value = 256,  backgroundColor = HexToColor("#EDCC61"), textColor = HexToColor("#FFFFFF") },
			new TileColorData { value = 512,  backgroundColor = HexToColor("#EDC850"), textColor = HexToColor("#FFFFFF") },
			new TileColorData { value = 1024, backgroundColor = HexToColor("#EDC53F"), textColor = HexToColor("#FFFFFF") },
			new TileColorData { value = 2048, backgroundColor = HexToColor("#EDC22E"), textColor = HexToColor("#FFFFFF") }
		};

		Debug.Log("TileManager: Klasik 2048 renkleri başarıyla yüklendi!");
	}

	// Yardımcı Hex dönüştürücü
	private Color HexToColor(string hex)
	{
		if (ColorUtility.TryParseHtmlString(hex, out Color color))
			return color;
		return Color.white;
	}
}