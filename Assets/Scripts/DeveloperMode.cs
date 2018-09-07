using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class DeveloperMode : MonoBehaviour {

	public bool inDevMode;
	static DeveloperMode instance;
	public Text devDifficultyText;
	int puzzleIndex;
	GameBoard board;
	LegalMoves legal;
	string puzzleFen;
	Move blacksLastMove;
	string selectedDifficulty;
	List<List<Move>> allMoves;

	public GameObject devUI;
	public GameObject gameUI;
	bool firstLine = true;

	void Awake() {
		instance = this;
		board = GetComponent<GameBoard>();
		legal = GetComponent<LegalMoves>();

		devUI.SetActive(inDevMode);
		gameUI.SetActive(!inDevMode);
	}

	public static bool InDevMode() {
		return instance.inDevMode;
	}

	public void SetMoves(List<List<Move>> moves) {
		allMoves = moves;

		NextPuzzle();
	}

	void NextPuzzle() {
		board.ResetBoardToStarting();
		legal.NewGame();

		//print (puzzleIndex);

		//print ("Move Count: " +allMoves[puzzleIndex].Count);
		foreach (Move m in allMoves[puzzleIndex]) {
			board.MakeMove(m.moveFrom.coordinates,m.moveTo.coordinates);
		}
		bool mated = false;
		bool flip = false;
		if (legal.Mated(board.analysisBoard,GameBoard.Colour.Black)) {
			mated = true;
		}
		else if (legal.Mated(board.analysisBoard, GameBoard.Colour.White)) {
			mated = true;
			flip = true;
		}


		if (mated) {
			blacksLastMove = allMoves[puzzleIndex][allMoves[puzzleIndex].Count-2];
			
			board.ResetBoardToStarting();
			for (int i = 0; i < allMoves[puzzleIndex].Count - 2; i ++) {
				Move m = allMoves[puzzleIndex][i];
				board.MakeMove(m.moveFrom.coordinates,m.moveTo.coordinates);
			}

			if (flip) {
				for (int ranks = 0; ranks < 4; ranks ++) {
					for (int files = 0; files < 8; files ++) {
						GameBoard.Pieces tempPiece = board.analysisBoard[files,ranks].piece;
						GameBoard.Colour tempColour = board.analysisBoard[files,ranks].pieceColour;
						bool tempEp = board.analysisBoard[files,ranks].isEnPassantSquare;

						board.analysisBoard[files,ranks].piece = board.analysisBoard[7-files,7-ranks].piece;
						board.analysisBoard[files,ranks].pieceColour = (board.analysisBoard[7-files,7-ranks].pieceColour == GameBoard.Colour.White)?GameBoard.Colour.Black:GameBoard.Colour.White;
						board.analysisBoard[files,ranks].isEnPassantSquare = board.analysisBoard[7-files,7-ranks].isEnPassantSquare;
						board.analysisBoard[7-files,7-ranks].piece = tempPiece;
						board.analysisBoard[7-files,7-ranks].pieceColour = (tempColour == GameBoard.Colour.White)?GameBoard.Colour.Black:GameBoard.Colour.White;
						board.analysisBoard[7-files,7-ranks].isEnPassantSquare = tempEp;
					}
				}
				blacksLastMove.moveFrom.coordinates = new Coord(7-blacksLastMove.moveFrom.coordinates.x,7-blacksLastMove.moveFrom.coordinates.y);
				blacksLastMove.moveTo.coordinates = new Coord(7-blacksLastMove.moveTo.coordinates.x,7-blacksLastMove.moveTo.coordinates.y);
			}

			puzzleFen = DatabaseManager.GetPuzzleFEN(board.analysisBoard,blacksLastMove.moveFrom.coordinates,blacksLastMove.moveTo.coordinates,legal);
			board.SetToAnalysisBoard();
			board.MakeMove(blacksLastMove.moveFrom.coordinates,blacksLastMove.moveTo.coordinates);
			board.SetToAnalysisBoard();
		}
		else {
			SetPuzzle(false);
		}

	}

	public void SetPuzzle(bool accept) {
		if (accept) {
		

			FileStream fs = new FileStream("Assets/Puzzles/Puzzles.txt", FileMode.Append,FileAccess.Write);
			StreamWriter writer = new StreamWriter(fs);
			if (firstLine) {
				//writer.WriteLine("");
				firstLine = false;
			}
			writer.WriteLine(puzzleFen);
			writer.Close();
			//print (puzzleFen);
			//print (board.analysisBoard + "  " + blacksLastMove.moveFrom.coordinates + " " + blacksLastMove.moveTo.coordinates + "  " + legal);
		}

		puzzleIndex ++;
		if (puzzleIndex < allMoves.Count) {
			NextPuzzle();
		}
		else {
			board.ResetBoardToStarting();
		}
	}

	public void SetDifficultyText(float t) {
		selectedDifficulty =  (int)t + "";
		devDifficultyText.text = "0" +selectedDifficulty;
	}

}
