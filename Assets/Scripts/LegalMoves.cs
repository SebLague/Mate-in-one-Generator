using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LegalMoves : MonoBehaviour {

	List<Move> legalMoves = new List<Move>();

	BitBoard[,] pawnBoardWhite = new BitBoard[8,8];
	BitBoard[,] pawnBoardBlack = new BitBoard[8,8];
	BitBoard[,] rookBoard = new BitBoard[8,8];
	BitBoard[,] knightBoard = new BitBoard[8,8];
	BitBoard[,] bishopBoard = new BitBoard[8,8];
	BitBoard[,] queenBoard = new BitBoard[8,8];
	BitBoard[,] kingBoard = new BitBoard[8,8];


	Coord[] rookDirections;
	Coord[] bishopDirections;
	Coord[] queenDirections;
	Coord[] kingDirections;
	Coord[] knightDirections;

	public bool whiteCanCastleKingside = true;
	public bool whiteCanCastleQueenside = true;
	public bool blackCanCastleKingside = true;
	public bool blackCanCastleQueenside = true;

	int gameIndex;
	float moveIndex;

	void Awake() {
		SetPieceDirections();
		SetBitBoards();
	}

	void SetPieceDirections() {
		// Rook
		rookDirections = new Coord[4];
		rookDirections[0] = new Coord(0,1);
		rookDirections[1] = new Coord(0,-1);
		rookDirections[2] = new Coord(1,0);
		rookDirections[3] = new Coord(-1,0);

		// Bishop
		bishopDirections = new Coord[4];
		bishopDirections[0] = new Coord(1,1);
		bishopDirections[1] = new Coord(-1,-1);
		bishopDirections[2] = new Coord(1,-1);
		bishopDirections[3] = new Coord(-1,1);

		// Queen
		queenDirections = new Coord[8];
		for (int i = 0; i < 8; i ++) {
			if (i < 4)
				queenDirections[i] = rookDirections[i];
			else 
				queenDirections[i] = bishopDirections[i-4];
		}

		// King
		kingDirections = queenDirections;

		// Knight
		knightDirections = new Coord[8];
		knightDirections[0] = new Coord(-2,1);
		knightDirections[1] = new Coord(-2,-1);
		knightDirections[2] = new Coord(-1,2);
		knightDirections[3] = new Coord(-1,-2);
		knightDirections[4] = new Coord(2,1);
		knightDirections[5] = new Coord(2,-1);
		knightDirections[6] = new Coord(1,2);
		knightDirections[7] = new Coord(1,-2);
	}

	public void NewGame() {
		blackCanCastleKingside = true;
		whiteCanCastleKingside = true;
		blackCanCastleQueenside = true;
		whiteCanCastleQueenside = true;
		gameIndex ++;
		moveIndex = 0;
	}

	public void NoCastling() {
		blackCanCastleKingside = false;
		whiteCanCastleKingside = false;
		blackCanCastleQueenside = false;
		whiteCanCastleQueenside = false;
	}

	void SetBitBoards() {
		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				pawnBoardWhite[files,ranks] = new BitBoard(8);
				pawnBoardBlack[files,ranks] = new BitBoard(8);
				rookBoard[files,ranks] = new BitBoard(8);
				knightBoard[files,ranks] = new BitBoard(8);
				bishopBoard[files,ranks] = new BitBoard(8);
				queenBoard[files,ranks] = new BitBoard(8);
				kingBoard[files,ranks] = new BitBoard(8);

				Coord currentCoord = new Coord(files,ranks);
				// rook and queen
				foreach (Coord c in rookDirections) {
					for (int i =0; i < 8; i ++) {
						queenBoard[files,ranks].Set(currentCoord + c * i);
						if (!rookBoard[files,ranks].Set(currentCoord + c * i))
							break;
					}
				}

				// knight
				foreach (Coord c in knightDirections) {
					knightBoard[files,ranks].Set(currentCoord + c);
				}

				// bishop and queen
				foreach (Coord c in bishopDirections) {
					for (int i =0; i < 8; i ++) {
						queenBoard[files,ranks].Set(currentCoord + c * i);
						if (!bishopBoard[files,ranks].Set(currentCoord + c * i))
							break;
					}
				}

				// king
				foreach (Coord c in kingDirections) {
					kingBoard[files,ranks].Set(currentCoord + c);
				}
				kingBoard[files,ranks].Set(currentCoord-new Coord(1,0));
				kingBoard[files,ranks].Set(currentCoord-new Coord(-1,0));

				// pawn
				pawnBoardWhite[files,ranks].Set (currentCoord + new Coord(0,1));
				pawnBoardWhite[files,ranks].Set (currentCoord + new Coord(-1,1));
				pawnBoardWhite[files,ranks].Set (currentCoord + new Coord(1,1));
				if (ranks == 1)
					pawnBoardWhite[files,ranks].Set (currentCoord + new Coord(0,2));

				pawnBoardBlack[files,ranks].Set (currentCoord + new Coord(0,-1));
				pawnBoardBlack[files,ranks].Set (currentCoord + new Coord(-1,-1));
				pawnBoardBlack[files,ranks].Set (currentCoord + new Coord(1,-1));
				if (ranks == 6)
					pawnBoardBlack[files,ranks].Set (currentCoord + new Coord(0,-2));
			}
		}
	}
	

	public List<Move> GetAllLegalMoves(Square[,] board, GameBoard.Colour myColour) {
		List<Move> allMoves = new List<Move>();
		foreach (Square s in board) {
			if (s.piece != GameBoard.Pieces.Empty && s.pieceColour == myColour) {
				BitBoard[,] myBitboard = new BitBoard[8,8];
				switch (s.piece) {
				case GameBoard.Pieces.Pawn:
					myBitboard = (myColour == GameBoard.Colour.White)?pawnBoardWhite:pawnBoardBlack;
					break;
				case GameBoard.Pieces.Rook:
					myBitboard = rookBoard;
					break;
				case GameBoard.Pieces.Knight:
					myBitboard = knightBoard;
					break;
				case GameBoard.Pieces.Bishop:
					myBitboard = bishopBoard;
					break;
				case GameBoard.Pieces.Queen:
					myBitboard = queenBoard;
					break;
				case GameBoard.Pieces.King:
					myBitboard = kingBoard;
					break;
				}


				for (int ranks = 0; ranks < 8; ranks ++) {
					for (int files = 0; files < 8; files ++) {
					
						if (myBitboard[s.coordinates.x,s.coordinates.y].Get(new Coord(files,ranks))) {

							Move m = new Move(s,board[files,ranks]);

							if (IsLegalMove(m,board)) {
								allMoves.Add(m);
							}
						}
					}
				}
			}

		}

		return allMoves;
	}

	public bool IsLegalMove(Move move, Square[,] board, bool debugMode = false) {
		Square[,] analysisBoard = new Square[8,8];

		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				analysisBoard[files,ranks] = new Square(board[files,ranks].coordinates,board[files,ranks].piece,board[files,ranks].pieceColour);
				analysisBoard[files,ranks].isEnPassantSquare = board[files,ranks].isEnPassantSquare;
			}
		}


		GameBoard.Pieces pieceType = move.moveFrom.piece;
		GameBoard.Colour friendlyColour = move.moveFrom.pieceColour;

		Coord toCoord = move.moveTo.coordinates;
		Coord fromCoord = move.moveFrom.coordinates;
		Square targetSquare = analysisBoard[toCoord.x,toCoord.y];
		GameBoard.Pieces capturePiece = targetSquare.piece;

		if (debugMode) {
//			print (pieceType + " from " + fromCoord.x +";"+fromCoord.y + " to " + toCoord.x + ";" + toCoord.y + "  " );
		}

		// Do not allow capture of friendly pieces OR of kings
		if (targetSquare.pieceColour == friendlyColour && targetSquare.piece != GameBoard.Pieces.Empty || targetSquare.piece == GameBoard.Pieces.King) {
			if (debugMode)
				print ("Trying to capture friendly piece / king");
			return false;
		}


		// apply suggested move to analysis board (whether legal or not)
		analysisBoard[toCoord.x,toCoord.y].piece = analysisBoard[fromCoord.x,fromCoord.y].piece;
		analysisBoard[toCoord.x,toCoord.y].pieceColour = friendlyColour;

		analysisBoard[fromCoord.x,fromCoord.y].piece = GameBoard.Pieces.Empty;
		Coord epCoord = new Coord();
		GameBoard.Pieces epRemovedPiece = GameBoard.Pieces.Empty;
		bool isEP = false;
		if (analysisBoard[toCoord.x,toCoord.y].isEnPassantSquare && toCoord.x != fromCoord.x && pieceType == GameBoard.Pieces.Pawn) {
			isEP = true;
			int epDir = (toCoord.y == 5)?-1:1;
			epCoord = new Coord(toCoord.x,toCoord.y + epDir);
			epRemovedPiece = analysisBoard[epCoord.x,epCoord.y].piece;
			analysisBoard[epCoord.x,epCoord.y].piece = GameBoard.Pieces.Empty;
		}


		Coord deltaMove = toCoord-fromCoord;

		// Check if resulting position is legal (is king in check)
		Coord friendlyKingCoord = new Coord();
		foreach (Square s in analysisBoard) {
			if (s.pieceColour == friendlyColour && s.piece == GameBoard.Pieces.King) {
				friendlyKingCoord = s.coordinates;
				break;
			}
		}

		Coord offsetFromMyKing = fromCoord - friendlyKingCoord;
		bool pinned = false;
		if (SquareUnderAttack(friendlyKingCoord,analysisBoard, friendlyColour)) {
			if (debugMode) {
				print ("Piece is pinned");
			}
			pinned = true;
		}
		if (isEP)
			analysisBoard[epCoord.x,epCoord.y].piece = epRemovedPiece;

		if (pinned)
			return false;


		// check if move direction is allowed by this piece
		// reset analysis board
		analysisBoard[fromCoord.x,fromCoord.y].piece = analysisBoard[toCoord.x,toCoord.y].piece;
		analysisBoard[fromCoord.x,fromCoord.y].pieceColour = friendlyColour;
		analysisBoard[toCoord.x,toCoord.y].piece = capturePiece;
		analysisBoard[toCoord.x,toCoord.y].pieceColour = (friendlyColour == GameBoard.Colour.White)?GameBoard.Colour.Black:GameBoard.Colour.White;

	

		if (pieceType == GameBoard.Pieces.Knight) { // Check if knight move is legal
			foreach (Coord c in knightDirections) {
				if (fromCoord + c == toCoord) {
					return true;
				}
			}
			return false;
		}
		else if (pieceType == GameBoard.Pieces.Pawn) {
			if (toCoord.y == 0 || toCoord.y == 7) { // is promoting if reached first/eigth rank
				move.isPromotion = true;


				if(move.promotionPiece == GameBoard.Pieces.Empty) {
					move.promotionPiece = GameBoard.Pieces.Queen;
					foreach (Coord c in knightDirections) {
						Coord knightAttackRange = toCoord + c;
						if (CoordInRange(knightAttackRange)) {
							if (analysisBoard[knightAttackRange.x,knightAttackRange.y].piece == GameBoard.Pieces.King && analysisBoard[knightAttackRange.x,knightAttackRange.y].pieceColour != friendlyColour) {
								move.promotionPiece = GameBoard.Pieces.Knight;
							
							}
						}
					}
				}
			}
			int pawnMoveDir = (friendlyColour == GameBoard.Colour.White)?1:-1;
			if (deltaMove.x == 0 && Mathf.Sign(deltaMove.y) == pawnMoveDir  && analysisBoard[toCoord.x,toCoord.y].piece == GameBoard.Pieces.Empty) { // pawn is advancing in correct direction and landing on empty square
				if (Mathf.Abs(deltaMove.y) == 1) // single step forward
					return true;
				else if (Mathf.Abs(deltaMove.y) == 2) { // two steps forward
					if ((fromCoord.y == 1 && pawnMoveDir == 1) || (fromCoord.y == 6 && pawnMoveDir == -1)) { // is on starting square
						if (analysisBoard[fromCoord.x,fromCoord.y + pawnMoveDir].piece == GameBoard.Pieces.Empty) { // not jumping over any pieces
							move.createEnPassant = true;
							move.enPassantSquare = new Coord(fromCoord.x,fromCoord.y + pawnMoveDir); // set en passant square

							return true;
						}
					}
				}
			}
			if (Mathf.Sign(deltaMove.y) == pawnMoveDir && Mathf.Abs(deltaMove.y) == 1 && Mathf.Abs(deltaMove.x) == 1) { // capturing in correct direction
				if (analysisBoard[toCoord.x,toCoord.y].isEnPassantSquare) { // capturing ep
					move.isEnPassantCapture = true;
					move.enPassantSquare = analysisBoard[toCoord.x,toCoord.y].coordinates;
					move.enPassantCaptureSquare = analysisBoard[fromCoord.x + deltaMove.x,fromCoord.y].coordinates;
					return true;
				}
				if (capturePiece != GameBoard.Pieces.Empty) { // is capturing enemy piece
					return true;
				}
			}
			return false;
		}
		else if (pieceType == GameBoard.Pieces.King) {
			if (Mathf.Abs(deltaMove.x) <= 1 && Mathf.Abs(deltaMove.y) <= 1) {
				// Castling is henceforth illegal as king has moved
				if (friendlyColour == GameBoard.Colour.White) {
					whiteCanCastleKingside = false;
					whiteCanCastleQueenside = false;
				}
				else {
					blackCanCastleKingside = false;
					blackCanCastleQueenside = false;
				}
				return true;
			}

			if (deltaMove.y == 0 && Mathf.Abs(deltaMove.x) == 2) { // castling
				int castleDir = (int)Mathf.Sign(deltaMove.x);

				bool canCastle = false;
				if (castleDir == -1) {
					canCastle = (friendlyColour == GameBoard.Colour.White)?whiteCanCastleQueenside:blackCanCastleQueenside;
				}
				else if (castleDir == 1) {
					canCastle = (friendlyColour == GameBoard.Colour.White)?whiteCanCastleKingside:blackCanCastleKingside;
				}

				if (!canCastle) {
					print (analysisBoard[fromCoord.x,fromCoord.y].pieceColour + "  " + blackCanCastleQueenside);
					return false;
				}

				Coord passingSquare = new Coord(fromCoord.x + castleDir,fromCoord.y);
				int min = (castleDir == 1)?5:1;
				int max = (castleDir == 1)?7:4;
				for (int i = min; i < max; i ++) {
					if (analysisBoard[i,toCoord.y].piece != GameBoard.Pieces.Empty) { // cant castle through occupied squares
						return false;
					}
				}
				if (!SquareUnderAttack(passingSquare,analysisBoard,friendlyColour)) { //cant castle through check
					move.isCastling = true;
					move.rookStartSquare = new Coord((castleDir == -1)?0:7, fromCoord.y);
					move.rookTargetSquare = new Coord(fromCoord.x + castleDir, fromCoord.y);
					if (friendlyColour == GameBoard.Colour.White) {
						whiteCanCastleKingside = false;
						whiteCanCastleQueenside = false;
					}
					else {
						blackCanCastleKingside = false;
						blackCanCastleQueenside= false;
					}
					return true;
				}
				else {
					return false;
				}

			}
		}

	
		Coord moveDir = ClampCoordToDir(deltaMove);
		int moveLength = Mathf.Max(Mathf.Abs(deltaMove.x),Mathf.Abs(deltaMove.y));
		if ((deltaMove.x == 0 || deltaMove.y == 0) && (pieceType == GameBoard.Pieces.Rook || pieceType == GameBoard.Pieces.Queen)) { // horizontal/vertical for Rook or Queen
			for (int i = 1; i < moveLength; i ++) {
				Coord c = fromCoord + moveDir * i;
				if (analysisBoard[c.x,c.y].piece != GameBoard.Pieces.Empty) { // moving through a piece
					return false;
				}
			}

			if (pieceType == GameBoard.Pieces.Rook) {
				if (move.moveFrom.coordinates == new Coord(0,0))
					whiteCanCastleQueenside = false;
				if (move.moveFrom.coordinates == new Coord(7,0))
					whiteCanCastleKingside = false;
				if (move.moveFrom.coordinates == new Coord(0,7))
					blackCanCastleQueenside = false;
				if (move.moveFrom.coordinates == new Coord(7,7))
					blackCanCastleKingside = false;
			}
			return true;
		}
	

		if (Mathf.Abs(deltaMove.x) == Mathf.Abs(deltaMove.y) && (pieceType == GameBoard.Pieces.Bishop || pieceType == GameBoard.Pieces.Queen)) { // diagonal for bishop or Queen
		
			for (int i = 1; i < moveLength; i ++) {
				Coord c = fromCoord + moveDir * i;
				if (analysisBoard[c.x,c.y].piece != GameBoard.Pieces.Empty) { // moving through a piece
					if (debugMode)
						print ("Moving diagonally through a piece");
					return false;
				}
			}
			return true;
		}



		if (debugMode)
			print ("Returned false by default");
		return false;
	}

	Coord ClampCoordToDir(Coord a) {
		int x = (int)(Mathf.Clamp01(Mathf.Abs(a.x)) * Mathf.Sign(a.x));
		int y = (int)(Mathf.Clamp01(Mathf.Abs(a.y)) * Mathf.Sign(a.y));
		return new Coord(x,y);
	}

	bool CoordInRange(Coord coord) {
		if (coord.x < 0 || coord.x >= 8 || coord.y < 0 || coord.y >= 8)
			return false;
		return true;
	}

	Square GetSquare(Square[,] board, Coord c) {
		return board[c.x,c.y];
	}

	bool SquareUnderAttack (Coord coord, Square[,] analysisBoard, GameBoard.Colour friendlyColour) {
		// work outwards from friendly king to find attacking pieces
		foreach (Coord dir in rookDirections) { // check horizontal/vertical
			for (int i =1; i <8; i ++) {
				Coord c = coord + dir * i;
				if (CoordInRange(c)) {
					Square checkSquare = GetSquare(analysisBoard,c);
					if (checkSquare.piece != GameBoard.Pieces.Empty) {
						if (checkSquare.pieceColour == friendlyColour) // Direction blocked by a friendly piece
							break;
						else { // enemy piece in this direction
							if (checkSquare.piece == GameBoard.Pieces.Rook || checkSquare.piece == GameBoard.Pieces.Queen || (i == 1 && checkSquare.piece == GameBoard.Pieces.King)) { // Is a piece that attacks horizontally/vertically
								return true; // King is still in check, thus move is not legal
							}
							else { // enemy piece in this direction, but not one that attacks in this direction, thus blocking any further attacks in this direction
								break;
							}
						}
					}
				}
				else { // end of board, check next direction
					break;
				}
			}
		}
		
		foreach (Coord dir in bishopDirections) { // check diagonals
			for (int i =1; i <8; i ++) {
				Coord c = coord + dir * i;
				if (CoordInRange(c)) {
					Square checkSquare = GetSquare(analysisBoard,c);
					if (checkSquare.piece != GameBoard.Pieces.Empty) {
						if (checkSquare.pieceColour == friendlyColour) // Direction blocked by a friendly piece
							break;
						else { // enemy piece in this direction
							if (checkSquare.piece == GameBoard.Pieces.Bishop || checkSquare.piece == GameBoard.Pieces.Queen || (i == 1 && checkSquare.piece == GameBoard.Pieces.King)) { // Is a piece that attacks diagonally
								return true; // King is still in check, thus move is not legal
							}
							else { // enemy piece in this direction, but not one that attacks in this direction, thus blocking any further attacks in this direction
								break;
							}
						}
					}
				}
				else { // end of board, check next direction
					break;
				}
			}
		}

		// Check knight attacks
		foreach (Coord knightOffset in knightDirections) {
			Coord c = coord + knightOffset;
			if (CoordInRange(c)) {
				Square s = GetSquare(analysisBoard,c);
				if (s.piece == GameBoard.Pieces.Knight && s.pieceColour != friendlyColour) { // square is attacked by enemy knight
					return true;
				}
			}
		}


		// Check pawn attacks
		int pawnAttackDir = (friendlyColour == GameBoard.Colour.White)?-1:1;
		for (int i = -1; i <= 1; i +=2) {
			Coord pawnAttackCoord = coord + new Coord(i,-pawnAttackDir);
			if (CoordInRange(pawnAttackCoord)) {
				Square s = GetSquare(analysisBoard,pawnAttackCoord);
				if (s.piece == GameBoard.Pieces.Pawn && s.pieceColour != friendlyColour) { // square attacked by enemy pawn
					return true;
				}
			 
			}
		}

		return false;
	}

	public Move GetMoveFromNotation(string pieceType, string targetSquare, string disambiguation, string promotionPieceString, Square[,] analysisBoard, GameBoard.Colour colourToMove, string originalNotation = "") {

		bool debugMode = (originalNotation == "xxx");
		Coord toCoord = new Coord("abcdefgh".IndexOf(targetSquare[0]), int.Parse(targetSquare[1].ToString())-1);
		GameBoard.Pieces movePiece = GameBoard.Pieces.Empty;
		GameBoard.Pieces promotePiece = GameBoard.Pieces.Empty;
		moveIndex ++;

		#region piece type
		switch (pieceType) {
		case "P":
			movePiece = GameBoard.Pieces.Pawn;
			break;
		case "R":
			movePiece = GameBoard.Pieces.Rook;
			break;
		case "N":
			movePiece = GameBoard.Pieces.Knight;
			break;
		case "B":
			movePiece = GameBoard.Pieces.Bishop;
			break;
		case "Q":
			movePiece = GameBoard.Pieces.Queen;
			break;
		case "K":
			movePiece = GameBoard.Pieces.King;
			break;
		}
		#endregion

		if (promotionPieceString != "") {
			switch (promotionPieceString) {
			case "R":
				promotePiece = GameBoard.Pieces.Rook;
				break;
			case "N":
				promotePiece = GameBoard.Pieces.Knight;
				break;
			case "B":
				promotePiece = GameBoard.Pieces.Bishop;
				break;
			case "Q":
				promotePiece = GameBoard.Pieces.Queen;
				break;
			}
		}


		// if more than one piece is able to reach the target square, e.g. Rfd1 (where f is the disambiguation)
		if (disambiguation != "") {
		

			bool findRank = false; // true if given file
			int givenValue = 0;
			if ("abcdefgh".Contains(disambiguation)) { // given file
				givenValue = "abcdefgh".IndexOf(disambiguation);
				findRank = true;
			}
			if ("12345678".Contains(disambiguation)) { // given rank
				int givenRank = "12345678".IndexOf(disambiguation);
				if (findRank) {
					Move m = new Move(analysisBoard[givenValue,givenRank],analysisBoard[toCoord.x,toCoord.y]);
					m.promotionPiece = promotePiece;
					return m;
				}
				givenValue = givenRank;
			}

			for (int i =0; i <8; i ++) {
				Coord checkCoord = new Coord((findRank)?givenValue:i,(findRank)?i:givenValue);

				if (analysisBoard[checkCoord.x,checkCoord.y].piece == movePiece && analysisBoard[checkCoord.x,checkCoord.y].pieceColour == colourToMove) {
					Move proposedMove = new Move(analysisBoard[checkCoord.x,checkCoord.y],analysisBoard[toCoord.x,toCoord.y]);
					proposedMove.promotionPiece = promotePiece;
					if (IsLegalMove(proposedMove,analysisBoard,debugMode)) {
						return proposedMove;
					}
				}
			}
		}
		else { // Only one piece can reach this square
			Coord moveFromCoords;


			foreach (Square s in analysisBoard) {
				if (s.piece == movePiece && s.pieceColour == colourToMove) {
					Move proposedMove = new Move(s,analysisBoard[toCoord.x,toCoord.y]);
					proposedMove.promotionPiece = promotePiece;
					if (IsLegalMove(proposedMove,analysisBoard,debugMode)) {
						return proposedMove;
					}
				
				}
			}
		}
		print ("Move illegal " + originalNotation + "  " + gameIndex + "  " + Mathf.CeilToInt(moveIndex/2f) );
		return null;
	}

	public static string CoordToString(Coord c) {
		return "(" + c.x + ";" + c.y+")";
	}

	public bool IsOnBoard(Coord c) {
		if (c.x < 0 || c.x >= 8 || c.y < 0 || c.y >= 8)
			return false;
		return true;
	}

	public bool Mated(Square[,] board, GameBoard.Colour matedColour) {

		Move[] allMoves = GetAllLegalMoves(board, matedColour).ToArray();
		if (allMoves.Length == 0) {
			foreach (Square s in board) {
				if (s.piece == GameBoard.Pieces.King && s.pieceColour == matedColour) {
					return SquareUnderAttack(s.coordinates,board,matedColour);
				}
			}
		}

		return false;
	}
	
}


public struct Coord {
	public int x;
	public int y;

	public Coord (int xP, int yP) {
		x = xP;
		y = yP;
	}

	public static Coord operator +(Coord a, Coord b) {
		return new Coord(a.x + b.x, a.y + b.y);
	}

	public static Coord operator -(Coord a, Coord b) {
		return new Coord(a.x - b.x, a.y - b.y);
	}

	public static Coord operator *(Coord a, int b) {
		return new Coord(a.x *b, a.y *b);
	}

	public static bool operator ==(Coord a, Coord b) {
		return (a.x == b.x && a.y == b.y);
	}
	public static bool operator !=(Coord a, Coord b) {
		return !(a==b);
	}
}

public class Move {

	public Square moveFrom;
	public Square moveTo;

	public bool isCastling;
	public bool createEnPassant;
	public bool isEnPassantCapture;
	public bool isPromotion;

	public Coord rookStartSquare;
	public Coord rookTargetSquare;
	public Coord enPassantSquare;
	public Coord enPassantCaptureSquare;
	public GameBoard.Pieces promotionPiece = GameBoard.Pieces.Empty;
	
	public Move (Square moveFromP, Square moveToP) {
		moveFrom = moveFromP;
		moveTo = moveToP;
	}
}




public struct BitBoard {
	bool[,] board;

	public BitBoard(int size) {
		board = new bool[8,8];
	}

	public bool Set(Coord c) {
		if (c.x < 0 || c.x >= 8 || c.y < 0 || c.y >= 8)
			return false;
		board[c.x,c.y] = true;
		return true;
	}

	public bool Get(Coord c) {
		if (c.x < 0 || c.x >= 8 || c.y < 0 || c.y >= 8)
			return false;
		return board[c.x,c.y];
	}



}
