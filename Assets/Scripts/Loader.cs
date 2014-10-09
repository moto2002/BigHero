using UnityEngine;
using System.Collections;
using System.IO;

public class Loader  {
	public static string GetText(string fileName){

		if(Application.platform == RuntimePlatform.Android){
			FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/" + fileName + ".json" );

			
			Debug.Log("kkkk " +  Application.persistentDataPath + "/" + fileName + ".json" );
			Debug.Log("kkkk " +  fileInfo.Exists);
			if(fileInfo.Exists == true){
				StreamReader sr = fileInfo.OpenText();
				
				string text = "";
				string read = "";
				while ((read = sr.ReadLine()) != null){
					text += read;
				}
				Debug.Log("kkkk " +  text);
				return text;
			}
		}


		TextAsset textAsset = Resources.Load<TextAsset>(fileName);
		return textAsset.text;
	}
}
