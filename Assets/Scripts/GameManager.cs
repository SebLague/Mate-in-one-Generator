using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	float timePerPuzzle = 10;
	float timeRemaining;
	int lastSecondTime;

	public Text timerUI;
	public Text scoreUI;
	public Text localBestUI;
	public Text worldBestUI;
	public AudioClip[] tickLoud;
	public AudioClip[] tickSoft;
	public AudioClip fail;
	public AudioClip move;
	public AudioClip checkMate;

	int localBest;
	int worldBest;


	public LayerMask pieceMask;

	GameBoard board;
	[HideInInspector]
	public GameBoard.Colour colourToMove = GameBoard.Colour.White;
	Camera cam;

	bool pieceSelected;
	Piece selectedSquare;

	Transform selectedPiece;
	Vector3 mouseDragStartPos;
	bool isDragging;
	bool waitingForAI;

	AI ai;
	LegalMoves legal;
	PuzzleManager puzzles;
	bool inGameMode;

	int difficulty = 1;
	int puzzleCompletionCount;
	int score;
	bool gameOver;

	bool freezeTimer;
	bool playing;

	void Awake() {

		localBest = PlayerPrefs.GetInt("localbest",0);
		localBestUI.text = localBest + "";
		worldBest = PlayerPrefs.GetInt("worldbest",localBest);
		worldBestUI.text = worldBest + "";

		board = GetComponent<GameBoard>();
		puzzles = GetComponent<PuzzleManager>();
		legal = GetComponent<LegalMoves>();
		ai = GetComponent<AI>();
		cam = Camera.main;
	
		inGameMode = !DeveloperMode.InDevMode();


	}

	void Start() {
		Highscores.instance.highscoresRetrieved += UpdateHighscore;
		Highscores.instance.RefreshHighscores();
		StartCoroutine("RefreshHighscores");
	}

	IEnumerator RefreshHighscores() {
		while (true) {

			yield return new WaitForSeconds(60);
			Highscores.instance.RefreshHighscores();
		}
	}

	void UpdateHighscore(bool success) {
		worldBest = Highscores.instance.highscore;
		worldBestUI.text = worldBest + "";
		PlayerPrefs.SetInt("worldbest",worldBest);
	}

	public void PlayInput() {
		if (inGameMode) {
			gameOver = false;
			GameUI.instance.ShowGame();

			timeRemaining = timePerPuzzle;
			lastSecondTime = (int)timeRemaining;
			scoreUI.text = "0";
			playing = true;
			LoadNextPuzzle();
		}
	}



	void PuzzleCompleted() {
		puzzleCompletionCount ++;
		difficulty = Mathf.FloorToInt((float)puzzleCompletionCount/5f);

		timeRemaining = timePerPuzzle;
		timeRemaining = Mathf.Clamp(timeRemaining,0,10);
		lastSecondTime = Mathf.CeilToInt(timeRemaining);

		score += 1;
		scoreUI.text = score + "";

		LoadNextPuzzle(difficulty);
	}

	void LoadNextPuzzle(int level = 1) {

		board.LoadPuzzle(puzzles.GetNextPuzzle());
		colourToMove = GameBoard.Colour.White;
		Invoke("MakeBlacksMove",.5f);
	}

	void MakeBlacksMove() {
		board.MakeBlacksMove();
		AudioManager.PlaySoundEffect(move);
		freezeTimer = false;
	}

	void AIResponse() {
		Move aiResponse = ai.GetMove(board.analysisBoard,GameBoard.Colour.Black);
		board.MakeMove(aiResponse.moveFrom.coordinates,aiResponse.moveTo.coordinates);
		AudioManager.PlaySoundEffect(move);
	}

	void Lose() {
		Highscores.instance.UploadNewScore(score);
		if (score > localBest) {

			localBest = score;
			PlayerPrefs.SetInt("localbest",localBest);
			localBestUI.text = score +"";
			if (localBest > worldBest) {
				worldBestUI.text = localBest + "";
			}
		}
		score = 0;
		playing = false;
		gameOver = true;
		AudioManager.PlaySoundEffect(fail);
		Reset();
		GameUI.instance.ShowMenu();
	}

	void Update() {
		if (!playing)
			return;

		if (inGameMode) {
			if (!freezeTimer)
				timeRemaining -= Time.deltaTime;
			timeRemaining = Mathf.Clamp(timeRemaining,0,int.MaxValue);
			int seconds = Mathf.CeilToInt(timeRemaining);
			timerUI.text = seconds + "";
			if (seconds != lastSecondTime) {
				lastSecondTime = seconds;
				AudioManager.PlaySoundEffect((lastSecondTime%2 == 0)?tickLoud:tickSoft,.25f);
			}
			if (timeRemaining == 0 && !gameOver) {
				Lose();
			}
		}

		if (colourToMove == GameBoard.Colour.White && !gameOver) {
		bool mouseDown = Input.GetMouseButtonDown(0);
			bool mouseUp = Input.GetMouseButtonUp(0);
			Vector3 mousePos = Input.mousePosition;

			if (mouseDown || mouseUp) {
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				Debug.DrawRay(ray.origin,ray.direction * 20,Color.green,5);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, pieceMask)) {

					if (mouseDown) {
						if (pieceSelected) { // new square clicked while piece already selected - move piece
							MakeMove(hit.collider.gameObject.GetComponent<Piece>());
						}
						else { // Select new piece
							isDragging = false;
							selectedSquare = hit.collider.gameObject.GetComponent<Piece>();

							if (selectedSquare.pieceColour == colourToMove && selectedSquare.piece != GameBoard.Pieces.Empty) {
								pieceSelected = true;
								mouseDragStartPos = Input.mousePosition;
								selectedPiece = selectedSquare.transform.GetChild(0);
							}
						}

					}
					else if (mouseUp && isDragging) {
						if (pieceSelected) { // select piece has been dropped - move it
							MakeMove(hit.collider.gameObject.GetComponent<Piece>());
						}
					}
				}
				else { // mouse off board, reset
					Reset();
				}
			
			}

			if (Input.GetMouseButton(0)) {
				if (pieceSelected) {

					if (isDragging) { // drag the selected piece around
						Vector3 targetPos = cam.ScreenToWorldPoint(Input.mousePosition) - Vector3.forward * cam.transform.position.z/2;
						selectedPiece.position = Vector3.MoveTowards(selectedPiece.position,targetPos, 3);
					}
					else { // start dragging if mouse has moved more than n pixels
						if (Mathf.Abs(mousePos.x - mouseDragStartPos.x) + Mathf.Abs(mousePos.y - mouseDragStartPos.y) > 5) {
							isDragging = true;
						}
					}
				}
			}
		}
	}


	void MakeMove(Piece targetSquare) {
	//	Move proposedMove = new Move(, targetSquare);
		if (board.MakeMove(selectedSquare.coordinates,targetSquare.coordinates)) {
			AudioManager.PlaySoundEffect(move);
			colourToMove = GameBoard.Colour.Black;
			if (legal.Mated(board.analysisBoard,GameBoard.Colour.Black)) {
				if (inGameMode)
					freezeTimer = true;
					Invoke("PuzzleCompleted",.4f);
			}
			else {

				Invoke ("Lose",.5f);
				Invoke("AIResponse",.5f);
			}
			
			//NextMove();
		}
		pieceSelected = false;
		selectedPiece.localPosition = Vector3.zero;
		isDragging = false;
	}

	void Reset() {
		pieceSelected = false;
		if (selectedPiece) {
			selectedPiece.localPosition = Vector3.zero;
		}
		isDragging = false;
	}

	
}
