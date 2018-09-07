using UnityEngine;
using System.Collections;

public class DrawBoard : MonoBehaviour {

	public Transform squareLight;
	public Transform squareDark;

	public Theme[] themes;
	public int themeIndex;
	Theme myTheme;
	public Sprite noPiece;
	public Sprite[] whitePieces;
	public Sprite[] blackPieces;
	public SpriteRenderer[,] squares;

	SpriteRenderer[,] pieceSpriteBoard;

	void Awake() {
		myTheme = themes[themeIndex];
		pieceSpriteBoard = new SpriteRenderer[8,8];
		squares = new SpriteRenderer[8,8];

		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				Vector3 localPos = Vector3.right * (-3.5f + files) + Vector3.up * (-3.5f + ranks);
			
				Transform newSquare = Instantiate(((files + ranks) % 2 == 0)?squareLight: squareDark) as Transform;
				newSquare.parent = transform;
				newSquare.localPosition = localPos;
				newSquare.localScale = Vector3.one;
				newSquare.name = files + "  " + ranks;
				squares[files,ranks] = newSquare.GetComponent<SpriteRenderer>();
				pieceSpriteBoard[files,ranks] = newSquare.GetChild(0).GetComponent<SpriteRenderer>();
			}
		}
		ColourSquares();
	}

	public void RedrawBoard(Piece[,] board) {
		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				pieceSpriteBoard[files,ranks].sprite = GetPiece(board[files,ranks]);
			}
		}
		ColourSquares();
	}

	public void DrawMove(Coord moveFrom, Coord moveTo) {
		ColourSquares();

		Color colFrom = ((moveFrom.x+moveFrom.y) %2 == 0)?myTheme.darkSelected:myTheme.lightSelected;
		Color colTo = ((moveTo.x+moveTo.y) %2 == 0)?myTheme.darkSelected:myTheme.lightSelected;

		colFrom = myTheme.lightSelected;
		colTo = myTheme.darkSelected;

		squares[moveFrom.x,moveFrom.y].color = colFrom;
		squares[moveTo.x,moveTo.y].color = colTo;

	}

	void ColourSquares() {
		for (int ranks = 0; ranks < 8; ranks ++) {
			for (int files = 0; files < 8; files ++) {
				Color col = ((files+ranks) %2 == 0)?myTheme.darkSquare:myTheme.lightSquare;
				squares[files,ranks].color =col;
			}
		}
		//squares[0,0].color = myTheme.darkSquare;
		//squares[1,0].color = myTheme.lightSquare;
	}

	public Piece GetSquare(int x, int y) {
		return pieceSpriteBoard[x,y].transform.parent.gameObject.GetComponent<Piece>();
	}

	Sprite GetPiece(Piece piece) {
		if (piece.piece == GameBoard.Pieces.Empty)
			return noPiece;
		else if (piece.pieceColour == GameBoard.Colour.White)
			return whitePieces[(int)piece.piece];
		else
			return blackPieces[(int)piece.piece];
	}
}

[System.Serializable]
public class Theme { 
	public Color lightSquare;
	public Color darkSquare;
	public Color lightSelected;
	public Color darkSelected;
}
