using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;

public class EDITOR_FileOpenInDefaultApp: EditorWindow {

	[MenuItem("Assets/Open in System Default Application %#o")]
	private static void GetSelectFile(){
		
		if(Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0){
			List<string> fileList = new List<string>();
     
			foreach(var files in Selection.assetGUIDs){
				var path = AssetDatabase.GUIDToAssetPath(files);
				fileList.Add(path);
			}

			foreach (string directory in fileList) {
				Command(Application.dataPath + directory.Substring(6, directory.Length - 6));
                //UnityEngine.Debug.Log(Application.dataPath + directory.Substring(6, directory.Length - 6));   //[Debug] Show file path
                //UnityEngine.Debug.Log(Application.platform);  //[Debug] Show Editor platform
            }
		}
	}

    static string Command(string cmd) {
        var p = new Process();
        string output = "";

        switch (Application.platform) {
            case RuntimePlatform.WindowsEditor:
                p.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
                p.StartInfo.Arguments = "/c start " + cmd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                break;
            case RuntimePlatform.OSXEditor:
                p.StartInfo.FileName = "/bin/bash";
                p.StartInfo.Arguments = "-c \" " + "open " + cmd + " \"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                break;
            default:
                UnityEngine.Debug.LogError("Can not identify the OS on which the editor is running");
                break;
        }

        return output;
    }
}