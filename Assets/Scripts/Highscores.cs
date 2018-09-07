using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highscores : MonoBehaviour {

	public bool sortHighToLow;
	
	// Database access information: http://dreamlo.com/lb/8klfu7qqQkuIQPy3KRFyeg7KyCrXlT6EeqkQK2RT5q5g
	public const string  privateCode = "8klfu7qqQkuIQPy3KRFyeg7KyCrXlT6EeqkQK2RT5q5g";
	public const string publicCode = "548df09c6e51b61b3c641d2a";
	public const string webURL = "http://dreamlo.com/lb/";
	
	public static Highscores instance;
	public Score[] highscoresList;

	public delegate void HighscoresRetrieved(bool downloadSuccessful);
	public HighscoresRetrieved highscoresRetrieved;

	string uniqueID;
	public int highscore;

	string username;

	void Awake() {
		instance = this;
		uniqueID = SystemInfo.deviceUniqueIdentifier;
		username = PlayerPrefs.GetString("name");
		if (string.IsNullOrEmpty(username)) {
			username = "Anonymous";
		}
	}

	/*
	 * Uploading
	 */

	public void UploadNewScore(int score) {;
		StartCoroutine(UploadScore(username, score));
	}
	
	// Upload score to server and refresh
	IEnumerator UploadScore(string _username, int score){
		WWW www = new WWW(Highscores.webURL + Highscores.privateCode + "/add-pipe/" + WWW.EscapeURL(_username + "=" + uniqueID) + "/" + score + "/0/" + GenerateCode(_username,score));
		yield return www;

		// Highscores successfully fetched
		if (string.IsNullOrEmpty(www.error)) {
			FormatHighscores(www.text);
		}
		else { // An error occurred
			if (highscoresRetrieved != null)
				highscoresRetrieved(false);
		}


	}


	/*
	 * Downloading
	 */

	public void RefreshHighscores() {
		StartCoroutine("DownloadScoresFromServer");
	}
	
	IEnumerator DownloadScoresFromServer()
	{
		// If sorting low to high entire database must be downloaded, otherwise just top entries can be fetched
		WWW www = new WWW(webURL +  publicCode  + "/pipe/");
		yield return www;
		// Highscores successfully fetched
		if (string.IsNullOrEmpty(www.error)) {
			FormatHighscores(www.text);
		}
		else { // An error occurred
			if (highscoresRetrieved != null)
				highscoresRetrieved(false);
		}
		
	}

	// Format highscore text received from server
	void FormatHighscores(string textStream) {
		string[] rows = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries); // Separate entries into rows
		List<Score> tempList = new List<Score>();

		// Go through each highscore entry
		for (int i = 0; i <rows.Length; i ++) {
			string[] rowInfo = rows[i].Split(new char[] {'|'});


			string[] id = rowInfo[0].Split(new char[] {'='});
			string username = id[0].Replace('+', ' ');
			int score = int.Parse(rowInfo[1]);
			string code = rowInfo[3];

			if (IsValid(username,score,code)) {
				tempList.Add(new Score(username,score));
			}
		}

		highscoresList = tempList.ToArray();

		if (highscoresList.Length > 0) {
			highscore = highscoresList[0].score;
		}


		// Retrieved Highscores
		if (highscoresRetrieved != null)
			highscoresRetrieved(true);
	}

	public Score[] SortLowToHigh(Score[] array) {
		System.Array.Sort(array,(x,y) => x.score.CompareTo(y.score));
		return array;
	}


	string GenerateCode(string username, int score) {
		username = username.ToLower();

		// mangle name
		string mangledName = "";
		int nameCharIndex = 0;
		int offsetIndex = 0;
		string realAlphabet = "abcdefghijklmnopqrstuvwxyz+ ";
		string codeAlphabet = "axdshgiqurytbnlzxciuhmnoireyoi12lskdfj88xcxzlk04kndfg7kjnc0zxckn123lkndsg8";
		for (int i = 0; i < 6; i ++) {
			offsetIndex += score;
			nameCharIndex += score;
			nameCharIndex %= username.Length;

			if (realAlphabet.Contains(username[nameCharIndex].ToString())) {
				int codeAlphabetIndex = realAlphabet.IndexOf(username[nameCharIndex]) + offsetIndex;
				codeAlphabetIndex %= codeAlphabet.Length;
				mangledName += codeAlphabet[codeAlphabetIndex];

			}
		}

		return mangledName;
	}

	bool IsValid(string username, int score, string code) {
		return GenerateCode(username,score) == code;
	}
}




public struct Score{
	public string username;
	public int score;

	public Score(string _usrn, int _score) {
		username = _usrn;
		score = _score;
	}
}
