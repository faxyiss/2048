using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	public enum GameState { Playing, GameOver, Won }
	public GameState State { get; private set; } // Oyunun anlık durumu

	[Header("Oyun Ayarları")]
	[Range(3, 8)]
	public int gridSize = 4;
	public BoardSystem boardSystem { get; private set; }

	private bool isAnimating = false; // INPUT KİLİDİ

	void Awake()
	{
		if (Instance != null && Instance != this) Destroy(gameObject);
		else Instance = this;
	}

	void Start()
	{
		StartNewGame();
	}

	void OnEnable() { InputManager.OnSwipeDetected += HandleSwipe; }
	void OnDisable() { InputManager.OnSwipeDetected -= HandleSwipe; }

	public void StartNewGame()
	{
		State = GameState.Playing;
		GridManager.Instance.GenerateGrid();
		Canvas.ForceUpdateCanvases();

		boardSystem = new BoardSystem(gridSize);
		TileManager.Instance.InitializeGrid(gridSize);

		if (boardSystem.SpawnRandomTile(out int x1, out int y1, out int val1))
			TileManager.Instance.SpawnTile(x1, y1, val1);

		if (boardSystem.SpawnRandomTile(out int x2, out int y2, out int val2))
			TileManager.Instance.SpawnTile(x2, y2, val2);
	}

	// async void kullanıyoruz çünkü UniTask ile bekleyeceğiz
	private async void HandleSwipe(InputManager.SwipeDirection direction)
	{
		// Eğer animasyon oynuyorsa VEYA oyun bittiyse, input alma!
		if (isAnimating || State != GameState.Playing) return;

		List<MoveData> receipt = boardSystem.MoveAndGetReceipt(direction);

		if (receipt.Count > 0)
		{
			isAnimating = true;

			try // RİSKLİ BÖLGEYİ KORUMAYA ALIYORUZ
			{
				// 1. Görsel animasyonların bitmesini bekle
				await TileManager.Instance.AnimateMovesAsync(receipt);

				// 2. KAZANDI MI KONTROLÜ
				if (boardSystem.CheckWinCondition())
				{
					State = GameState.Won;
					Debug.Log($"<color=yellow>TEBRİKLER! 2048'e Ulaştın! Skor: {boardSystem.Score}</color>");
				}
				else
				{
					// 3. Oyun devam ediyorsa yeni sayıyı doğur
					if (boardSystem.SpawnRandomTile(out int x, out int y, out int val))
					{
						TileManager.Instance.SpawnTile(x, y, val);
					}

					// 4. YENİ SAYI DOĞDUKTAN SONRA KAYBETTİ Mİ KONTROLÜ
					if (boardSystem.CheckGameOver())
					{
						State = GameState.GameOver;
						Debug.Log($"<color=red>OYUN BİTTİ! Yapacak hamle kalmadı. Skor: {boardSystem.Score}</color>");
					}
				}
			}
			finally
			{
				// HATA ÇIKSA BİLE BU KİLİT KESİNLİKLE AÇILACAK
				isAnimating = false;
			}
		}
	}
}