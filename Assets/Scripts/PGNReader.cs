using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PGNReader : MonoBehaviour {

	List<List<string>> moveNotation = new List<List<string>>();
	List<List<Move>> allMoves = new List<List<Move>>();
	string pgn;
	LegalMoves legalMoves;
	GameBoard board;
	DeveloperMode devMode;

	void Start() {
		if (DeveloperMode.InDevMode()) {
			legalMoves = GetComponent<LegalMoves>();
			board = GetComponent<GameBoard>();
			devMode = GetComponent<DeveloperMode>();

			StreamReader reader = new StreamReader("Assets/Puzzles/PGN.txt");
			CustomMethods.MeasureExecutionTime(false);
			pgn = reader.ReadToEnd();
			CustomMethods.MeasureExecutionTime(true);
			reader.Close();
			ReadPGN(pgn);
		}
	}

	int gameIndex = 0;
	int moveIndex = 0;

	void Update() {
		/*
		if (Input.GetMouseButtonDown(0)) {
			if (moveIndex < allMoves[gameIndex].Count) {
				board.MakeMove(allMoves[gameIndex][moveIndex].moveFrom.coordinates, allMoves[gameIndex][moveIndex].moveTo.coordinates);
				moveIndex++;
			}
		}
		if (Input.GetMouseButtonDown(1)) {
			legalMoves.NewGame();
			moveIndex = 0;
			board.ResetBoardToStarting();
		}
	*/
	
	}

	public void ReadPGN(string pgn) {

		bool readingExtraInfo = false;
		bool readingGame = false;
		bool readingMove = false;
		bool readingWhiteMove = false;
		string currentMoveString = "";
		string moveLetters = "abcdefghRNBQKO";

		List<string> currentGameMoveNotations = new List<string>();

		// Separate moves into strings
		for (int i = 0; i < pgn.Length; i ++) {
			char c = pgn[i];
			if (c == '[') {
				readingGame = false;
				readingExtraInfo = true;
			}
			if (c == ']')
				readingExtraInfo = false;

			if (readingGame) {
				if (!readingMove && c == '-') { // Game over (result token, e.g. 0-1)
					moveNotation.Add(currentGameMoveNotations);
					currentGameMoveNotations = new List<string>();
					readingGame = false;
					readingMove = false;

				}
				if (moveLetters.Contains(c.ToString())) {
					readingMove = true;
				}
				if (c == ' ' || c == '.' || c == '\n' || c=='+' || c == '#') {
					if (currentMoveString.Length >= 2)
						currentGameMoveNotations.Add(currentMoveString);

					readingMove = false;

					currentMoveString = "";
				}
				if (readingMove) {
					currentMoveString += c.ToString();
				}

			}
			else if (c == '1' && !readingExtraInfo) {
				readingGame = true;
				readingWhiteMove = true;
			}
		}

		for (int gameIndex = 0; gameIndex < moveNotation.Count; gameIndex ++) {
	
			// Convert move notation to moves
			GameBoard.Colour colourToMove = GameBoard.Colour.White;
			List<Move> currentGameMoves = new List<Move>();
			//print (moveNotation[gameIndex].Count);
			foreach (string m in moveNotation[gameIndex]) {
			
				//print ( "   ASDAS " + m);
				string pieceType = ""; // e.g RNBQK
				string disambiguation = ""; // (e.g. Rfd1 (f is disambiguation between two possible rooks)
				string targetSquare = ""; // e.g e4
				bool promoting = false;
				string promotionPiece = "";

				string pieceSymbols = "RNBQK";
				string coordinateSymbols = "abcdefgh";
				string coordinateNumbers = "12345678";

				if (m == "O-O" || m == "O-O-O") { // Castling
					//print ("CASS");
					pieceType = "K";
					targetSquare = (m == "O-O")?"g":"c";
					targetSquare += (colourToMove == GameBoard.Colour.White)?"1":"8";
				}
				else {
					for (int i =0; i < m.Length; i ++) { // Iterate through each char in move string
						char c = m[i];

						if (c == '=')
							promoting = true;

						if (GameBoard.IsUpper(c.ToString()) && pieceSymbols.Contains(c.ToString())) {
							if (promoting)
								promotionPiece = c.ToString();
							else
								pieceType = c.ToString();
						}
						else {
							if (coordinateSymbols.Contains(c.ToString())) {
								if (targetSquare != "") {
									disambiguation = targetSquare;
								}
								targetSquare = c.ToString();
							}
							else if (coordinateNumbers.Contains(c.ToString())) {
								targetSquare += c.ToString();
							}
						}

					}
				}
				if (pieceType == "")
					pieceType = "P";
			//	print (m);

				Move newMove = legalMoves.GetMoveFromNotation(pieceType, targetSquare, disambiguation, promotionPiece,board.analysisBoard,colourToMove,m);

				currentGameMoves.Add(newMove);
				if (newMove != null) {
					board.UpdateAnalysisBoard(newMove);
				}
				colourToMove = (colourToMove == GameBoard.Colour.White)?GameBoard.Colour.Black:GameBoard.Colour.White;

			}
			allMoves.Add(currentGameMoves);
			board.ResetBoardToStarting();
			legalMoves.NewGame();
		}
		devMode.SetMoves(allMoves);
	}
}
