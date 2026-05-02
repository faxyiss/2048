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

	[Header("Oyun İçi Skor Referansları")]
	public TextMeshProUGUI scoreValueText;    // Hiyerarşideki: ScoreValueText
	public TextMeshProUGUI maxScoreValueText; // Hiyerarşideki: MaxScoreValueText

	[Header("Game Over Referansları")]
	public CanvasGroup gameOverCanvasGroup;
	public TextMeshProUGUI gameOverResultLabelText; // "SCORE" veya "NEW BEST!" yazacak kısım
	public TextMeshProUGUI gameOverScoreValueText;  // Skoru gösterecek kısım
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
	public void UpdateGameScores(int currentScore, int bestScore)
	{
		// Mevcut skor değiştiyse zıplama efekti ver[cite: 1]
		if (scoreValueText.text != currentScore.ToString())
		{
			scoreValueText.text = currentScore.ToString();
			scoreValueText.transform.DOKill(true);
			// Sayı arttığında kutunun hafifçe şişip inmesi (Punch Scale)[cite: 1]
			scoreValueText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
		}

		// En yüksek skoru yazdır
		if (maxScoreValueText != null)
		{
			maxScoreValueText.text = bestScore.ToString();
		}
	}
	// --- BASİT PANEL GEÇİŞLERİ ---

	public void ShowMenu()
	{
		menuPanel.SetActive(true);
		if (gamePanel != null) gamePanel.SetActive(false);
		if (gameOverPanel != null) gameOverPanel.SetActive(false);

		AdManager.Instance.HideBanner();
	}

	public void ShowGamePanel()
	{
		menuPanel.SetActive(false);
		if (gameOverPanel != null) gameOverPanel.SetActive(false);
		if (gamePanel != null) gamePanel.SetActive(true);

		AdManager.Instance.ShowBanner();
	}

	public void ShowGameOver(int score, bool isNewBest)
	{
		// Paneli görünür ama şeffaf ve biraz küçük başlat
		gameOverCanvasGroup.gameObject.SetActive(true);
		gameOverCanvasGroup.alpha = 0f;
		gameOverCanvasGroup.blocksRaycasts = true; // Arkaya tıklamayı engeller
		gameOverCanvasGroup.transform.localScale = Vector3.one * 0.8f;

		// İçeriği güncelle
		gameOverResultLabelText.text = isNewBest ? "NEW BEST!" : "SCORE";
		gameOverScoreValueText.text = score.ToString();

		// DOTween ile yavaşça belirme ve büyüme animasyonu
		gameOverCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
		gameOverCanvasGroup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
	}

	public void HideGameOver()
	{
		// Menüye veya yeni oyuna geçerken paneli animasyonla kapat
		if (gameOverCanvasGroup.gameObject.activeSelf)
		{
			gameOverCanvasGroup.blocksRaycasts = false;
			gameOverCanvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
			{
				gameOverCanvasGroup.gameObject.SetActive(false);
			});
		}
	}

	public void PlayButtonPressEffect(RectTransform buttonTransform)
	{
		// Önce çalışan bir animasyon varsa durdur (üst üste binmesin)
		buttonTransform.DOKill(true);

		// Butonu %10 oranında anlık küçültüp geri zıplat (Punch Scale)
		buttonTransform.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.2f, 10, 1);
	}
}