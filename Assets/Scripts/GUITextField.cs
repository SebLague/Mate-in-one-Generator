﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUITextField : MonoBehaviour {

	public string legalChars = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public int maxLength = 10;


	private string myString;
	[HideInInspector]
	public Text nameField;
	private bool textModified;

	private bool holdingDelete;
	private float deleteHoldTime;
	private float nextDeleteTime;

	public void Awake ()
	{
		nameField = GetComponent<Text>();
		myString = PlayerPrefs.GetString("name");
		nameField.text = myString;
	}

	void Update() {
		if (Input.anyKeyDown) {

			// Backspace
			if (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)) 
				Backspace();

			char[] inputChars = Input.inputString.ToCharArray();
			string legalInput = "";

			foreach (char c in inputChars) {
				if (legalChars.Contains(c.ToString())) {
					if (c == ' ') { // Do not allow string to begin with a space, or for two or more spaces to be added in a row
						if (myString.Length > 0) {
							if (myString[myString.Length-1] != ' ') {
								legalInput += c;
								textModified = true;
							}
						}
					}
					else {
						legalInput += c;
						textModified = true;
					}

				}
			}

			myString += legalInput;
				if (myString.Length > maxLength)
			myString = myString.Substring(0,maxLength);
		}

		// Delete button held down
		if (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)) {
			deleteHoldTime += Time.deltaTime;
			//print (deleteHoldTime);
			if (deleteHoldTime > .5f && Time.time > nextDeleteTime) {
				nextDeleteTime = Time.time + .075f;
				Backspace();
			}
		}
		else {
			deleteHoldTime = 0;
		}

		// Apply modified text
		if (textModified) {
			textModified = false;

			nameField.text =myString;
			PlayerPrefs.SetString("name",myString);
		}
	}

	public string GetUsername() {
		return myString;
	}


	private void Backspace() {
		if (myString.Length >= 1) {
			myString = myString.Substring(0,myString.Length-1);
			textModified = true;
		}
	}
	
}
