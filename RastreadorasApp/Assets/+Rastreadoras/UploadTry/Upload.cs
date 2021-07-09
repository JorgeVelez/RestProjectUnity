using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;
using System;
using UnityEngine.Networking;
 
// a very simplistic level upload and random name generator script
 
public class Upload : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("UploadLevel");
		 //StartCoroutine("Upload2");
    }

    IEnumerator UploadLevel()  
    {  
        WWWForm form = new WWWForm();
        form.AddField("action", "level upload");
        form.AddField("file","file");

		string filePath = "/video.mp4";
		//filePath = "/image.psd";
		//filePath = "/audio2.wav";
        filePath = "/file.zip";
		filePath = Application.dataPath + filePath;

		byte[] bytes = { };

        if (File.Exists(filePath))
        {
            bytes = File.ReadAllBytes(filePath);
            Debug.Log("si existe");
        }
        //media = Convert.ToBase64String(bytes);
        form.AddBinaryData ( "file", bytes, "file.zip");
		//form.AddBinaryData ( "file", bytes, "audio2.wav","audio/wav");
		//form.AddBinaryData ( "file", bytes, "image.psd","image/psd");
		//form.AddBinaryData ( "file", levelData, fileName,"text/xml");
 
        WWW w = new WWW("http://www.thisisnotanumber.org/uploads/Upload.php",form);
 
        yield return w;
        if (w.error != null)
        {
            print (w.error );    
        }else{
            print ("exito" ); 
        }
    }
}
 
 