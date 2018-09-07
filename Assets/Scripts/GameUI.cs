using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	public RectTransform gamePanel;
	public RectTransform scorePanel;
	public RectTransform menuPanel;
	float changeSpeed = 2;

	RectTransform activePanel;
	RectTransform panelOld;

	float displayPercent;
	bool changingDisplay;
	public static GameUI instance;

	void Awake() {
		instance = this;
		activePanel = menuPanel;
		gamePanel.gameObject.SetActive(false);
		menuPanel.gameObject.SetActive(true);
		scorePanel.gameObject.SetActive(true);
		//print (Screen.width * .27f);
	}

	public void Highscores() {
		Application.LoadLevel("Highscores");
	}

	public void Options() {

	}


	public void ShowGame() {
		ShowPanel(gamePanel);
	}

	public void ShowMenu() {
		ShowPanel(menuPanel);
	}

	void ShowPanel(RectTransform panel) {
		if (activePanel) {
			panelOld = activePanel;

		}

		activePanel = panel;
		activePanel.gameObject.SetActive(true);

		changingDisplay = true;
		displayPercent = 0;
	}

	// Update is called once per frame
	void Update () {
		if (changingDisplay) {
			displayPercent += Time.deltaTime * changeSpeed;
			if (panelOld) {
				panelOld.anchoredPosition = Vector2.Lerp(Vector2.zero,Vector2.right * (1-panelOld.anchorMin.x) * Screen.width,displayPercent);
			}
			activePanel.anchoredPosition = Vector2.Lerp(-Vector2.right * (1-activePanel.anchorMin.x) * Screen.width, Vector2.zero,displayPercent);

			if (displayPercent > 1) {
				changingDisplay = false;
			}
		}
	}
}
