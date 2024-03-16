// We need this for parsing the JSON, unless you use an alternative.
// You will need SimpleJSON if you don't use alternatives.
// It can be gotten hither. http://wiki.unity3d.com/index.php/SimpleJSON
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System;
using Unity.Collections;
using UnityEngine.Networking;

public class Translate : MonoBehaviour
{
	private static Translate instance;

    private void Awake()
    {
		if (instance != null)
		{
			DestroyImmediate(this.transform.root.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public static void TranstaleText(string input, Action<string> OnComplete, bool UseAuto, string source, string target)
    {
		Process(input, delegate(string r) { 
			OnComplete(r);
		}, UseAuto, source, target);
	}

	private static void Process(string sourceText, Action<string> result, bool useAuto, string sourceLang = "-1", string targetLang = "-1")
	{
		// We use Auto by default to determine if google can figure it out.. sometimes it can't.
		if (sourceLang == "-1")
		{
			sourceLang = useAuto ? "auto" : (Localization.CurrentLanguageType != LanguageType.English ? "en" : "ru");
		}

		if (targetLang == "-1")
		{
			targetLang = (Localization.CurrentLanguageType == LanguageType.English ? "en" : "ru");
		}

		// Construct the url using our variables and googles api.
		string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
			+ sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

		// Put together our unity bits for the web call.
		var www = UnityWebRequest.Get(url);
		// Now we actually make the call and wait for it to finish.
		var r = www.SendWebRequest();

		r.completed += delegate
		{
			// Check to see if it's done.
			if (www.isDone)
			{
				// Check to see if we don't have any errors.
				if (string.IsNullOrEmpty(www.error))
				{
					// Parse the response using JSON.
					var N = JSONNode.Parse(www.downloadHandler.text);

					string translatedText = "";
					// Dig through and take apart the text to get to the good stuff.
					for (int i = 0; i < N[0].Count; i++)
					{
						translatedText += N[0][i][0];
					}

					//Debug.Log(N.Count);
					//Debug.Log(N[0].Count);
					//Debug.Log(N[0][0].Count);
					// This is purely for debugging in the Editor to see if it's the word you wanted.
					result(translatedText);

					//Debug.Log("Translated: " + sourceText + " from " + sourceLang + " to " + targetLang + " with result " + translatedText + " (url: " + url + " )");
				}
			}

			www.Dispose();
		};
	}
}