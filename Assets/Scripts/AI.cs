using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {

	LegalMoves legalMoves;
	GameBoard board;


	void Awake() {
		legalMoves = GetComponent<LegalMoves>();
		board = GetComponent<GameBoard>();
	}
	

	public Move GetMove(Square[,] analysisBoard, GameBoard.Colour myColour) {
		List<Move> allMoves = legalMoves.GetAllLegalMoves(analysisBoard,myColour);
		
		if (allMoves.Count > 0) {

			Move m = allMoves[Random.Range(0,allMoves.Count)];
			return m;
			//board.MakeComputerMove(move);
		}
		else {
			print ("AI MATED");
		}
		return null;
	}
}
