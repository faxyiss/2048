#if UNITY_EDITOR
using UnityEngine;

public class EditorWinCheat : MonoBehaviour
{
	void Start()
	{
		// Build'e girmemesi için objeyi etiketliyoruz
		gameObject.tag = "EditorOnly";
	}

	// OnGUI, Canvas'a ihtiyaç duymadan ekrana direkt UI çizen eski ama harika bir test aracıdır
	void OnGUI()
	{
		// Ekranın sağ üst köşesine (Genişlikten 120 piksel geri, Yukarıdan 20 piksel aşağı) bir alan açıyoruz
		GUILayout.BeginArea(new Rect(Screen.width - 120, 20, 100, 100));

		// Arka arkaya iki buton çizdiriyoruz
		if (GUILayout.Button("FORCE WIN", GUILayout.Height(40)))
		{
			ForceWin();
		}

		if (GUILayout.Button("FORCE LOSE", GUILayout.Height(40)))
		{
			ForceLose();
		}

		GUILayout.EndArea();
	}

	private void ForceWin()
	{
		Debug.Log("<color=magenta><b>[HİLE AKTİF]</b></color> Oyun zorla kazanıldı!");
		UIManager.Instance.ShowGameWon();
	}

	private void ForceLose()
	{
		Debug.Log("<color=red><b>[HİLE AKTİF]</b></color> Oyun zorla kaybedildi!");
		UIManager.Instance.ShowGameOver(100, false); // Örnek skor 1500
	}
}
#endif