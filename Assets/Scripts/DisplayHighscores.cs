using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Highscores))]
public class DisplayHighscores : MonoBehaviour {

	public static int displayCount = 10;

	public Text[] highscoreText;


	void Awake() {
		displayCount = highscoreText.Length;
	}

	void Start() {
	
		for (int i =0; i <displayCount; i ++) {
			highscoreText[i].text = (i+1) + ". ";
		}
	
		/*
		Highscores.instance.UploadNewScore("Toby And",2);
		Highscores.instance.UploadNewScore("Matt Ago",12);
		Highscores.instance.UploadNewScore("Brian",31);


	;

*/
		Highscores.instance.highscoresRetrieved = OnHighscoresFetched;
		RefreshHighscores();
	}

	public void RefreshHighscores() {
		Highscores.instance.RefreshHighscores();
	}


	// Highscores have been fetched
	void OnHighscoresFetched(bool downloadSuccessful) {
	
		if (downloadSuccessful) {
			for (int i =0; i <displayCount; i ++) {
				if (i<Highscores.instance.highscoresList.Length) {
					highscoreText[i].text = (i+1) + ". " + Highscores.instance.highscoresList[i].username + ": " + Highscores.instance.highscoresList[i].score;

				}
			}
		}
		else {
			print ("Could not connect to highscore server");
		}
		Invoke("RefreshHighscores",30);
	}

	string RandomString() {
		string s = "";
		string a = "abcdefghijklmnopqrstuvwxyz";
		for (int i =0; i < 8; i ++) {
			s += a[Random.Range(0,a.Length)];
		}
		return s;
	}

}
