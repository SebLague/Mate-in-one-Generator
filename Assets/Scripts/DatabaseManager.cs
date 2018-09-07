using UnityEngine;
using System.Collections;
using System.IO;

public class DatabaseManager : MonoBehaviour {

	// Write puzzle to file (position is the game position - white to move and mate, finalMove is black's previous move that resulted in this position, and difficulty is a range from 0-100)
	public static string GetPuzzleFEN(Square[,] position, Coord finalMoveFrom, Coord finalMoveTo, LegalMoves legal) {
		string puzzleFEN = "";
		int skipCount = 0;

		Coord epSquare = new Coord();
		bool isEP = false;

		for (int ranks = 7; ranks >= 0; ranks --) {
			for (int files = 0; files < 8; files ++) {
				if (position[files,ranks].piece == GameBoard.Pieces.Empty) {
					skipCount ++;
				}
				else {
					if (skipCount > 0) {
						puzzleFEN += skipCount.ToString();
						skipCount = 0;
					}
					puzzleFEN += PieceToNotation(position[files,ranks].piece, position[files,ranks].pieceColour);

					if (position[files,ranks].isEnPassantSquare) {
						isEP = true;
						epSquare = new Coord(files,ranks);
					}
				}
			}
			if (skipCount > 0) {
				puzzleFEN += skipCount.ToString();
				skipCount = 0;
			}
			if (ranks !=0)
				puzzleFEN += "/";
		}
		puzzleFEN += " w ";

		if (legal.whiteCanCastleKingside)
			puzzleFEN += "K";
		if (legal.whiteCanCastleQueenside)
			puzzleFEN += "Q";
		if (legal.blackCanCastleKingside)
			puzzleFEN += "k";
		if (legal.blackCanCastleQueenside)
			puzzleFEN += "q";

		if (!legal.whiteCanCastleKingside && !legal.whiteCanCastleQueenside && !legal.blackCanCastleKingside && !legal.blackCanCastleQueenside) {
			puzzleFEN += "-";
		}

		if (isEP) {
			puzzleFEN += " " + "abcdefgh"[epSquare.x] + (epSquare.y+1) + " ";
		}
		else {
			puzzleFEN += " - ";
		}

		puzzleFEN += "[" + finalMoveFrom.x +"" + finalMoveFrom.y +"" + finalMoveTo.x + "" + finalMoveTo.y + "]";

		return puzzleFEN;
	}

	static string PieceToNotation(GameBoard.Pieces piece, GameBoard.Colour colour) {
		string notation = "";
		switch (piece) {
		case GameBoard.Pieces.Pawn:
			notation = "P";
			break;
		case GameBoard.Pieces.Rook:
			notation = "R";
			break;
		case GameBoard.Pieces.Knight:
			notation = "N";
			break;
		case GameBoard.Pieces.Bishop:
			notation = "B";
			break;
		case GameBoard.Pieces.Queen:
			notation = "Q";
			break;
		case GameBoard.Pieces.King:
			notation = "K";
			break;
		}

		if (colour == GameBoard.Colour.Black) {
			notation = notation.ToLower();
		}
		return notation;
	}
}
