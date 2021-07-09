using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
//using System.Diagnostics;

public class ftptry : MonoBehaviour {

	FTPClient ftpc;

	public InputField tx;

	public InputField servidor;
	public InputField user;
	public InputField pass;

	public InputField file;
	public Text segcaja;

	private string servidorString;
	private string userString;
	private string passString;

	private string fileString="20151214210105.jpg";

	public Image img;

	void Start () {



		tx.text = "Starting\n";

		servidor.GetComponent<InputField> ().text="ftp://192.168.0.22:21/";
		user.GetComponent<InputField> ().text="docuware";
		pass.GetComponent<InputField> ().text="Admin01!";

		servidor.GetComponent<InputField> ().text="ftp://127.0.0.1:21/";
		user.GetComponent<InputField> ().text="test";
		pass.GetComponent<InputField> ().text="aaaaaaaa";

		user.GetComponent<InputField> ().text="jorgevelez";
		pass.GetComponent<InputField> ().text="marmil981";

		servidor.GetComponent<InputField> ().text="ftp://s112763.gridserver.com:21/";
		user.GetComponent<InputField> ().text="acervo@thisisnotanumber.org";
		pass.GetComponent<InputField> ().text="@Marzo1981";

		servidorString = servidor.GetComponent<InputField> ().text;
		userString = user.GetComponent<InputField> ().text;
		passString = pass.GetComponent<InputField> ().text;
		
		ftpc = new FTPClient (servidorString, userString, passString);
	}

	void Update () {
		//if (ftpc != null && ftpc.getDownloadedBytes () > 0 && ftpc.getDownloadedBytes () != ftpc.filesSize) {
			//Debug.Log (ftpc.getDownloadedBytes ());
			//Debug.Log ( ftpc.getDownloadedBytes () == ftpc.filesSize);

		//}
	}

	public void ListDir () {
		
		try{
			servidorString = servidor.GetComponent<InputField> ().text+"documentos/";
			userString = user.GetComponent<InputField> ().text;
			passString = pass.GetComponent<InputField> ().text;
			
			ftpc = new FTPClient (servidorString, userString, passString);
			
			tx.text = "dir list\n";
			tx.text += servidorString+" - "+ userString+" - "+  passString+"\n";
			tx.text +=ftpc.DirectoryListing ("")+"\n";
			segcaja.text=tx.text;
			Debug.Log(tx.text);
		}catch(Exception  e){
			tx.text +=e.Message+"\n";
		}
	}

	public void FileSize () {
		
		try{
			servidorString = servidor.GetComponent<InputField> ().text;
			userString = user.GetComponent<InputField> ().text;
			passString = pass.GetComponent<InputField> ().text;
			
			ftpc = new FTPClient (servidorString, userString, passString);

			fileString = file.GetComponent<InputField> ().text;
			
			tx.text = "dir list "+fileString+"\n";
			tx.text += servidorString+" - "+ userString+" - "+  passString+"\n";
			//tx.text +=ftpc.FileSize (fileString)+"\n";
		}catch(Exception  e){
			tx.text +="error dir list\n";
			Debug.Log(e.Message);
			tx.text +=e.Message+"\n";
		}
	}




	public void DownloadBase64 () {
		try{


			fileString = file.GetComponent<InputField> ().text;
			tx.text = "download "+fileString +" \n";
			tx.text += servidorString+" - "+ userString+" - "+  passString+"\n";
			//string responseImg=ftpc.DownloadToString(fileString, "builds/"+fileString);
			//tx.text +="chars "+responseImg.Length+" - "+"\n";
			ftpc.StartDownloadHandler(fileString, "builds/"+fileString);
			//ftpc.DownloadFtpFile(fileString, "builds/"+fileString);
			/*Texture2D text = new Texture2D(1,1);
			text.LoadImage(Convert.FromBase64String(responseImg));
			Sprite spr=Sprite.Create(text, new Rect(0,0,text.width, text.height), new Vector2(.5f, .5f));
			img.sprite=spr;*/

			tx.text = "complete \n";
		}catch(Exception  e){
			tx.text +="error download\n";
			Debug.Log(e.Message);
			if (e.Message.Contains("File unavailable"))
			tx.text +=e.Message+"\n";
		}
	}

}
