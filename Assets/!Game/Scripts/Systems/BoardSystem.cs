using System.Collections.Generic;
using UnityEngine;

public class BoardSystem
{
	public int Size { get; private set; }
	public int[,] Matrix { get; private set; }
	public int Score { get; private set; }

	public BoardSystem(int size)
	{
		Size = size;
		Matrix = new int[size, size];
		Score = 0;
		ClearBoard();
	}

	public void ClearBoard()
	{
		for (int x = 0; x < Size; x++)
			for (int y = 0; y < Size; y++)
				Matrix[x, y] = 0;
	}

	public bool SpawnRandomTile(out int spawnX, out int spawnY, out int spawnValue)
	{
		spawnX = -1; spawnY = -1; spawnValue = 0;
		List<Vector2Int> emptyCells = new List<Vector2Int>();

		for (int x = 0; x < Size; x++)
			for (int y = 0; y < Size; y++)
				if (Matrix[x, y] == 0) emptyCells.Add(new Vector2Int(x, y));

		if (emptyCells.Count == 0) return false;

		int randomIndex = Random.Range(0, emptyCells.Count);
		Vector2Int cell = emptyCells[randomIndex];
		int newNumber = Random.value < 0.9f ? 2 : 4;

		Matrix[cell.x, cell.y] = newNumber;
		spawnX = cell.x; spawnY = cell.y; spawnValue = newNumber;

		return true;
	}

	// YENİ VE GELİŞMİŞ KAYDIRMA SİSTEMİ
	public List<MoveData> MoveAndGetReceipt(InputManager.SwipeDirection direction)
	{
		List<MoveData> moves = new List<MoveData>();
		bool[,] merged = new bool[Size, Size];

		// EKSEN DÜZELTMESİ (Satır ve Sütun mantığına göre uyarlandı)
		int dirX = 0; // X artık Satır (Yukarı/Aşağı)
		int dirY = 0; // Y artık Sütun (Sağ/Sol)

		if (direction == InputManager.SwipeDirection.Up) dirX = -1;
		else if (direction == InputManager.SwipeDirection.Down) dirX = 1;
		else if (direction == InputManager.SwipeDirection.Left) dirY = -1;
		else if (direction == InputManager.SwipeDirection.Right) dirY = 1;

		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				// Aşağı veya Sağa kaydırırken matrisi sondan okumalıyız ki bloklar üst üste binmesin
				int x = (dirX == 1) ? Size - 1 - i : i;
				int y = (dirY == 1) ? Size - 1 - j : j;

				if (Matrix[x, y] == 0) continue;

				int currentX = x, currentY = y;
				int nextX = currentX + dirX, nextY = currentY + dirY;

				// Bloğu gidebildiği kadar ileri it
				while (nextX >= 0 && nextX < Size && nextY >= 0 && nextY < Size)
				{
					if (Matrix[nextX, nextY] == 0)
					{
						currentX = nextX; currentY = nextY; // Boşluk var, ilerle
					}
					else if (Matrix[nextX, nextY] == Matrix[x, y] && !merged[nextX, nextY])
					{
						currentX = nextX; currentY = nextY; // Eşit sayı var, üstüne çık ve dur
						break;
					}
					else break; // Farklı sayı var, burada dur

					nextX = currentX + dirX; nextY = currentY + dirY;
				}

				if (currentX != x || currentY != y)
				{
					int value = Matrix[x, y];
					Matrix[x, y] = 0;

					if (Matrix[currentX, currentY] == value)
					{
						// Birleşme (Merge) gerçekleşti
						Matrix[currentX, currentY] *= 2;
						Score += Matrix[currentX, currentY];
						merged[currentX, currentY] = true;
						moves.Add(new MoveData { FromX = x, FromY = y, ToX = currentX, ToY = currentY, IsMerge = true, NewValue = Matrix[currentX, currentY] });
					}
					else
					{
						// Sadece boşluğa kaydı
						Matrix[currentX, currentY] = value;
						moves.Add(new MoveData { FromX = x, FromY = y, ToX = currentX, ToY = currentY, IsMerge = false, NewValue = value });
					}
				}
			}
		}
		return moves;
	}
	public bool CheckWinCondition()
	{
		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				if (Matrix[x, y] == 2048) return true; // Kazandı!
			}
		}
		return false;
	}

	// Hamle kaldı mı kontrolü
	public bool CheckGameOver()
	{
		// 1. Önce boş yer var mı diye bak. Varsa oyun kesinlikle bitmemiştir.
		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				if (Matrix[x, y] == 0) return false;
			}
		}

		// 2. Boş yer yoksa, yan yana veya alt alta aynı sayı var mı diye bak.
		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				int current = Matrix[x, y];

				// Sağındakiyle aynı mı?
				if (x < Size - 1 && Matrix[x + 1, y] == current) return false;

				// Üstündekiyle aynı mı?
				if (y < Size - 1 && Matrix[x, y + 1] == current) return false;
			}
		}

		// Ne boş yer var, ne de birleşebilecek blok... Oyun bitti.
		return true;
	}

	public void PrintBoard() { /* Mevcut yazdırma kodun aynen kalabilir */ }
}