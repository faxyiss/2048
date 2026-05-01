using UnityEngine;
using UnityEngine.UI; // Renk değişimi için UI kütüphanesini ekledik
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Tile : MonoBehaviour
{
	[Header("Görsel Referanslar")]
	public Image backgroundImage; // Bloğun arka planı
	public TextMeshProUGUI valueText; // İçindeki yazı

	// Artık sadece sayıyı değil, renkleri de dışarıdan alıyor
	public void SetState(int value, Color bgColor, Color txtColor)
	{
		valueText.text = value.ToString();
		backgroundImage.color = bgColor;
		valueText.color = txtColor;
	}

	public async UniTask MoveToAsync(Vector3 targetPos)
	{
		// .AsyncWaitForCompletion() eklendi
		await transform.DOMove(targetPos, 0.15f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
	}

	public async UniTask PlayMergeAnimationAsync()
	{
		// .AsyncWaitForCompletion() eklendi
		await transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 0, 0).AsyncWaitForCompletion();
	}
}