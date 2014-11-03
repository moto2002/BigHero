using UnityEngine;
using System.Collections;
using System.IO;

public class Loader  {
	public static string GetText(string fileName){

		if(Application.platform == RuntimePlatform.Android){
			FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/" + fileName + ".json" );

			if(fileInfo.Exists == true){
				StreamReader sr = fileInfo.OpenText();
				
				string text = "";
				string read = "";
				while ((read = sr.ReadLine()) != null){
					text += read;
				}

				return text;
			}
		}


		TextAsset textAsset = Resources.Load<TextAsset>(fileName);
		return textAsset.text;
	}
}
