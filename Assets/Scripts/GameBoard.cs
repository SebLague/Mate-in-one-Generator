using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour {

	public enum Pieces {Pawn,Rook,Knight,Bishop,Queen,King,Empty};
	public enum Colour {Black, White};


	Piece[,] board;
	public Square[,] analysisBoard;
	DrawBoard drawBoard;
	LegalMoves legalMoves;
	GameManager manager;
	AI ai;
	string startFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

	bool isPuzzle;
	Coord blacksPuzzleMoveFrom;
	Coord blacksPuzzleMoveTo;

	void Awake() {
		legalMoves = GetComponent<LegalMoves>();
		manager = GetComponent<GameManager>();
		drawBoard = GetComponent<DrawBoard>();
		ai = GetComponent<AI>();
		analysisBoard = new Square[8,8];

		board = new Piece[8,8];

		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				board[files,ranks] = drawBoard.GetSquare(files,ranks);
				board[files,ranks].piece = Pieces.Empty;
				board[files,ranks].coordinates = new Coord(files, ranks);
			}
		}

	

		SetupFromFEN(startFEN);

	}

	public void SetupFromFEN(string fen) {
		//fen = "r2q3r/ppp2Bpp/8/2B1k2n/4P3/3P4/PPP3PP/R2b1RK1 w - - [5544]";
		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				board[files,ranks] = drawBoard.GetSquare(files,ranks);
				board[files,ranks].piece = Pieces.Empty;
				board[files,ranks].coordinates = new Coord(files, ranks);
			}
		}

		string numbers = "12345678";
		int fenIndex = 0;

		for (int ranks = 7; ranks >=0; ranks --) {
			for (int files = 0; files < 8; files ++) {

				char fenChar = fen[fenIndex];
				fenIndex ++;
				if (fenChar == '/') {
					fenChar = fen[fenIndex];
					fenIndex ++;
				}

				if (numbers.Contains(fenChar.ToString())) {
					files += int.Parse(fenChar.ToString())-1;
				}
				else {
					board[files,ranks].piece = NotationToPiece(fenChar);
					board[files,ranks].pieceColour = (IsUpper(fenChar +""))?Colour.White:Colour.Black;
				}
			}
		}


		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				analysisBoard[files,ranks] = new Square(board[files,ranks].coordinates,board[files,ranks].piece,board[files,ranks].pieceColour);
			}
		}

		string remainingFen = fen.Substring(fen.IndexOf(" "));
		GameBoard.Colour colToMove = GameBoard.Colour.White;
		int fenSection = 0;
		bool sectionActive = false;
		bool isEnPassant = false;
		Coord enPassantSquare = new Coord();

		bool hasBlacksLastMove = false;
		Coord blackLastMoveFrom = new Coord();
		Coord blackLastMoveTo = new Coord();
		int[] blacksLastMoveCoord = new int[4];
		int blacksLastMoveIndex =0;

		legalMoves.NoCastling();
		foreach (char c in remainingFen) {
			if (fenSection == 0) {
				if (c== 'w' || c == 'b') { // colour to move
					fenSection ++;
				}
				if (c=='b')
					colToMove = Colour.Black;
			}
			else if (fenSection == 1) { // castling rights
				if (c == '-') {
					fenSection ++;
				}
				else {

					if ("kKqQ".Contains(c+""))
						sectionActive = true;
					if (c == 'K')
						legalMoves.whiteCanCastleKingside = true;
					if (c == 'k')
						legalMoves.blackCanCastleKingside = true;
					if (c == 'Q')
						legalMoves.whiteCanCastleQueenside = true;
					if (c == 'q')
						legalMoves.blackCanCastleQueenside = true;
					if (c == ' ' && sectionActive) {
						sectionActive = false;
						fenSection++;
					}
				}
			}
			else if (fenSection == 2) { // en passant square
				if (c == '-')
					fenSection++;
				else {
					if ("abcdefgh".Contains(c+"")) {
						sectionActive = true;
						isEnPassant = true;
						enPassantSquare = new Coord("abcdefgh"[c],0);
					}
					if ("12345678".Contains(c+"")) {
						enPassantSquare = new Coord(enPassantSquare.x,"12345678".IndexOf(c));
					}
					if (c == ' ' && sectionActive) {
						sectionActive = false;
						fenSection ++;
					}
				}
			}
			else if (fenSection == 3) { // puzzle 'black's last move' coords
				if (c == '[') {
					sectionActive = true;
				}
				if (c == ']') {
					sectionActive = false;
				}
				if (sectionActive) {
					if ("01234567".Contains(c+"")) {
						hasBlacksLastMove = true;

						blacksLastMoveCoord[blacksLastMoveIndex] = int.Parse(c + "");

						blacksLastMoveIndex ++;
					}
				}
			}
		}

		if (isEnPassant) {
			analysisBoard[enPassantSquare.x,enPassantSquare.y].isEnPassantSquare = true;
		}
		manager.colourToMove = colToMove;

		isPuzzle = hasBlacksLastMove;

		if (hasBlacksLastMove) {
			blacksPuzzleMoveFrom = new Coord(blacksLastMoveCoord[0],blacksLastMoveCoord[1]);
			blacksPuzzleMoveTo = new Coord(blacksLastMoveCoord[2],blacksLastMoveCoord[3]);

			//Invoke("MakeBlacksMove", .5f);
			//MakeBlacksMove();
		}


		drawBoard.RedrawBoard(board);

	}

	public void SetToAnalysisBoard() {
		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				board[files,ranks].piece = analysisBoard[files,ranks].piece;
				board[files,ranks].pieceColour = analysisBoard[files,ranks].pieceColour;
			}
		}

		drawBoard.RedrawBoard(board);
	}

	public void MakeBlacksMove() {
		MakeMove(blacksPuzzleMoveFrom,blacksPuzzleMoveTo);
	}

	public void LoadPuzzle(string puzzleFEN) {
		SetupFromFEN(puzzleFEN);
	}

	public void MakeComputerMove(Colour aiColour) {

		Move aiMove = ai.GetMove(analysisBoard,aiColour);
		if (aiMove == null) {

		}
		else {
			MakeMove(aiMove.moveFrom.coordinates,aiMove.moveTo.coordinates);
		}
	}

	public void ResetBoardToStarting() {
		SetupFromFEN(startFEN);
	}

	public void UpdateAnalysisBoard(Move move) {



		analysisBoard[move.moveTo.coordinates.x,move.moveTo.coordinates.y].piece = move.moveFrom.piece;

		analysisBoard[move.moveTo.coordinates.x,move.moveTo.coordinates.y].pieceColour = move.moveFrom.pieceColour;
		analysisBoard[move.moveFrom.coordinates.x,move.moveFrom.coordinates.y].piece = Pieces.Empty;

		bool specialMove = false;
		if (move.isCastling) {
			specialMove = true;
			analysisBoard[move.rookStartSquare.x,move.rookStartSquare.y].piece = Pieces.Empty;
			analysisBoard[move.rookTargetSquare.x,move.rookTargetSquare.y].piece = Pieces.Rook;
			analysisBoard[move.rookTargetSquare.x,move.rookTargetSquare.y].pieceColour = move.moveFrom.pieceColour;
		}
		if (move.createEnPassant) {
			specialMove = true;
			analysisBoard[move.enPassantSquare.x,move.enPassantSquare.y].isEnPassantSquare = true;
		}
		if (move.isEnPassantCapture) {
			specialMove = true;
			analysisBoard[move.enPassantCaptureSquare.x,move.enPassantCaptureSquare.y].piece = Pieces.Empty;
			analysisBoard[move.enPassantSquare.x,move.enPassantSquare.y].isEnPassantSquare = false;
		}
		if (move.isPromotion) {
			specialMove = true;

			print (move.isPromotion);
			analysisBoard[move.moveTo.coordinates.x,move.moveTo.coordinates.y].piece = move.promotionPiece;

		}

		foreach (Square s in analysisBoard) {
			s.UpdateEP();
		}


	}


	public bool MakeMove(Coord from, Coord to) {

		//print (from.x + " " + from.y + "    "+ to.x + " " + to.y);
		Move move = new Move(analysisBoard[from.x,from.y], analysisBoard[to.x,to.y]);


		if (legalMoves.IsLegalMove(move, analysisBoard)) {
			board[move.moveTo.coordinates.x,move.moveTo.coordinates.y].piece = move.moveFrom.piece;
			board[move.moveTo.coordinates.x,move.moveTo.coordinates.y].pieceColour = move.moveFrom.pieceColour;
			board[move.moveFrom.coordinates.x,move.moveFrom.coordinates.y].piece = Pieces.Empty;

			if (move.isCastling) {
				board[move.rookStartSquare.x,move.rookStartSquare.y].piece = Pieces.Empty;
				board[move.rookTargetSquare.x,move.rookTargetSquare.y].piece = Pieces.Rook;
				board[move.rookTargetSquare.x,move.rookTargetSquare.y].pieceColour = move.moveFrom.pieceColour;
			}
			if (move.createEnPassant) {
				analysisBoard[move.enPassantSquare.x,move.enPassantSquare.y].isEnPassantSquare = true;
			}
			if (move.isEnPassantCapture) {
				board[move.enPassantCaptureSquare.x,move.enPassantCaptureSquare.y].piece = Pieces.Empty;
				analysisBoard[move.enPassantSquare.x,move.enPassantSquare.y].isEnPassantSquare = false;
			}
			if (move.isPromotion) {
				board[move.moveTo.coordinates.x,move.moveTo.coordinates.y].piece = move.promotionPiece;
				//print ("PROMOTE");
			}
			drawBoard.RedrawBoard(board);
			drawBoard.DrawMove(from,to);
			
			for (int ranks = 0; ranks < 8; ranks ++) {
				for (int files = 0; files < 8; files ++) {
					bool isEP = analysisBoard[files,ranks].isEnPassantSquare;
					bool epLife = analysisBoard[files,ranks].destroyEP;
					analysisBoard[files,ranks] = new Square(board[files,ranks].coordinates,board[files,ranks].piece,board[files,ranks].pieceColour);
					analysisBoard[files,ranks].isEnPassantSquare = isEP;
					analysisBoard[files,ranks].destroyEP = epLife;
					analysisBoard[files,ranks].UpdateEP();
				}
			}

			return true;
		}

		return false;
	}

	public Pieces NotationToPiece(char notation) {
		string n = notation.ToString().ToLower();
		string pieceNotation = "prnbqk";
		if (pieceNotation.Contains(n))
			return (Pieces)pieceNotation.IndexOf(n);
		else
			return Pieces.Empty;

	}

	public static bool IsUpper(string text) {
		string upperText = text.ToUpper();
		return text == upperText;
	}

}

public class Square {
	public bool underAttack;
	public bool isEnPassantSquare;
	
	public GameBoard.Pieces piece;
	public GameBoard.Colour pieceColour;
	public Coord coordinates;
	public bool destroyEP;

	public Square(Coord coords, GameBoard.Pieces pieceP, GameBoard.Colour pieceColourP) {
		coordinates = coords;
		piece = pieceP;
		pieceColour = pieceColourP;
		underAttack = false;
		isEnPassantSquare = false;
		//destroyEP = false;
	}
	


	public void UpdateEP() {
		if (isEnPassantSquare) {
			if (destroyEP) {
				isEnPassantSquare = false;
				destroyEP = false;
			}
			destroyEP = true;
		}

	}
}


