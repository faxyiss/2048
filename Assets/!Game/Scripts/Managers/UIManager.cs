using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("Hiyerarşi Referansları (Menü)")]
	public TextMeshProUGUI scoreNumberText; // Hiyerarşideki: Score_Number
	public TextMeshProUGUI playButtonText;  // Hiyerarşideki: Button -> Text (TMP)

	[Header("Panel Referansları (Geçişler İçin)")]
	public GameObject menuPanel;
	public GameObject gamePanel;
	public GameObject gameOverPanel; // Bunu sonradan eklersin

	// Hangi butonun seçili kaldığını hafızada tutmak için
	private RectTransform lastSelectedButton;

	void Awake()
	{
		if (Instance != null && Instance != this) Destroy(gameObject);
		else Instance = this;
	}

	// --- SEÇİM VE GÜNCELLEME SİSTEMİ ---

	// GameManager, bir moda (Selection) tıklandığında bu metodu çağıracak
	public void UpdateMenuSelection(int gridSize, RectTransform clickedButton, int currentHighScore)
	{
		// 1. Play butonunun yazısını güncelle (Örn: 4x4 OYNA)
		if (playButtonText != null)
		{
			playButtonText.text = $"{gridSize}x{gridSize} Play";

			// Dikkat çeksin diye yazıya ufak bir zıplama veriyoruz
			playButtonText.transform.DOKill(true);
			playButtonText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1);
		}

		// 2. Yukarıdaki High Score numarasını seçilen moda göre güncelle
		if (scoreNumberText != null)
		{
			scoreNumberText.text = currentHighScore.ToString();
		}

		// 3. Tıklanan butona (Selection) animasyon ver
		if (clickedButton != null)
		{
			// Eğer daha önceden seçilmiş bir buton varsa, onu eski (1x1) haline küçült
			if (lastSelectedButton != null && lastSelectedButton != clickedButton)
			{
				lastSelectedButton.DOKill(true);
				lastSelectedButton.DOScale(Vector3.one, 0.2f);
			}

			// Yeni seçilen butona şık bir zıplama ver ve %5 daha büyük kalmasını sağla
			clickedButton.DOKill(true);
			clickedButton.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1).OnComplete(() =>
			{
				clickedButton.DOScale(Vector3.one * 1.05f, 0.1f);
			});

			// Hafızayı güncelle
			lastSelectedButton = clickedButton;
		}
	}

	// --- BASİT PANEL GEÇİŞLERİ ---

	public void ShowMenu()
	{
		menuPanel.SetActive(true);
		if (gamePanel != null) gamePanel.SetActive(false);
		if (gameOverPanel != null) gameOverPanel.SetActive(false);
	}

	public void ShowGamePanel()
	{
		menuPanel.SetActive(false);
		if (gameOverPanel != null) gameOverPanel.SetActive(false);
		if (gamePanel != null) gamePanel.SetActive(true);
	}
}