using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PuzzleManager : MonoBehaviour {

	public TextAsset fenText;

	string[] puzzles;
	int puzzleIndex;

	void Awake() {
	
		if (!DeveloperMode.InDevMode()) {
			/*
			List<string> puzzlesList = new List<string>();
			//puzzles = new string[puzzleDifficultyCount][];
			//for (int i = 0; i < puzzleDifficultyCount; i ++)
				//puzzlesList[i] = new List<string>();

			StreamReader reader = new StreamReader("Assets/Puzzles/Puzzles.txt");
			while (reader.Peek() >= 0) {
				string line = reader.ReadLine();
				puzzlesList.Add(line);
			}
			puzzles = CustomMethods.ShuffleArray<string>(puzzlesList.ToArray());
	
			reader.Close();
			*/
			puzzles = fenText.text.Split(new char[] {'\n'},System.StringSplitOptions.RemoveEmptyEntries);
			puzzles = CustomMethods.ShuffleArray<string>(puzzles);


		}
	}

	public string GetNextPuzzle() {

		//print ( puzzles[currentPuzzleDifficulty].Length);
		string newFEN = puzzles[puzzleIndex];
		puzzleIndex ++;
		puzzleIndex %= puzzles.Length;
		return newFEN;

	}
}
