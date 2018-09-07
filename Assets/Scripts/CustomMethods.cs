using System.Collections;
using System.Diagnostics;
using UnityEngine;


public static class CustomMethods {
	
	
	// Methods concerning random output
	#region Random
	
	/// Returns an array of random numbers from a specified range without any duplicates.
	public static int[] GetRandomNumbersWithoutDuplicates(int randomNumberCount, int minValue, int maxValue) {
		// Nonsensical input:
		if (minValue>maxValue)
			UnityEngine.Debug.LogError("Given minValue is greater than given maxValue");
		else if (randomNumberCount > maxValue-minValue)
			UnityEngine.Debug.LogError("More unique numbers requested than are available in the given range");
		
		// Create an array of integers from minValue to maxValue
		int[] numberArray = new int[maxValue-minValue];
		for (int i = 0; i <numberArray.Length; i ++)
			numberArray[i] = minValue + i;
		
		// Shuffle array
		numberArray = ShuffleArray<int>(numberArray);
		int[] shuffledNumberArray = new int[randomNumberCount];
		// Return number of random integers requested from the shuffled array
		for (int i = 0; i < shuffledNumberArray.Length; i++)
			shuffledNumberArray[i] = numberArray[i];
		
		return shuffledNumberArray;
	}
	#endregion
	
	
	// Sort arrays
	#region Sorting
	/// Sort int/float array from highest to lowest value
	public static int[] SortHighToLow(int[] array) {
		System.Array.Sort(array,(x,y) => x.CompareTo(y));
		return array;
	}
	public static float[] SortHighToLow(float[] array) {
		System.Array.Sort(array,(x,y) => x.CompareTo(y));
		return array;
	}
	
	/// Sort int/float array from lowest to highest value
	public static int[] SortLowToHigh(int[] array) {
		System.Array.Sort(array,(x,y) => y.CompareTo(x));
		return array;
	}
	public static float[] SortLowToHigh(float[] array) {
		System.Array.Sort(array,(x,y) => y.CompareTo(x));
		return array;
	}
	
	
	/// Randomly shuffle array
	/// Type must be specified e.g. ShuffleArray<int>(foo);
	public static T[] ShuffleArray<T>(T[] array) {
		
		int elementsRemainingToShuffle = array.Length;
		int randomIndex = 0;
		
		while (elementsRemainingToShuffle > 1) {
			
			// Choose a random element from array
			randomIndex = Random.Range(0,elementsRemainingToShuffle);
			T chosenElement = array[randomIndex];
			
			// Swap the randomly chosen element with the last unshuffled element in the array
			elementsRemainingToShuffle --;
			array[randomIndex] = array[elementsRemainingToShuffle];
			array[elementsRemainingToShuffle] = chosenElement;
		}
		
		return array;
	}
	#endregion
	
	// Diagnostics
	#region Diagnostics
	private static Stopwatch stopwatch;
	
	/// Measures the execution time of a piece of code.
	/// Call MeasureExecutionTime(false) prior to running the code,
	/// and MeasureExecutionTime(true) afterwards to print the result.
	public static void MeasureExecutionTime(bool finish, string debugText = "Execution Time:") {
		// Create stopwatch
		if (stopwatch == null)
			stopwatch = new Stopwatch();
		
		// If finished timing, print result
		if (finish) {
			stopwatch.Stop();
			UnityEngine.Debug.Log(debugText + " " + stopwatch.ElapsedMilliseconds + " milliseconds");
		}
		// Start the timer
		else {
			stopwatch.Start();
		}	
	}
	
	/// Concatenates and prints input of multiple types
	/// e.g. Print("Values:", foo, bar);
	public static void Print(params object[] input) {
		string processedText = "";
		foreach (object o in input) {
			processedText += o.ToString() + " ";
		}
		UnityEngine.Debug.Log(processedText);
	}
	
	#endregion
}
