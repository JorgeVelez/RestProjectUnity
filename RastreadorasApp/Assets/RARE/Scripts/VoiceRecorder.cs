using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class VoiceRecorder : MonoBehaviour {

	public AudioSource jungleLoop;
	public AudioSource currentAsrc;
	private bool isRecording = false;
	public GameObject popUp;
	private int micRecordNum = 0;
	private int audioListenerRecordNum = 0;
	private int exportClipRecordNum = 0;
	private int recordNum = 0;
	private List<AudioClip> myClips = new List<AudioClip> ();
	private bool isplaying;
	//text information
	public Text info;
	public Text time;
	//sliders
	//public Slider playbackSli;
	//buttons
	public GameObject micRecordButton;
	public GameObject playRecordingButton;
	//waveform variables
	private float tracklength = 0.0f;
	private float currentTrackTime = 0.0f;
	private float MicTime;
	private bool PlayHeadTouch;

	public GameObject circle;
	public GameObject panel;
	Action<string> methodForSuccess;
	public Button btOK;
	public Button btBack;
	string clipName;

	void Start(){
		micRecordButton.transform.Find("stop").gameObject.SetActive(false);
		micRecordButton.transform.Find("record").gameObject.SetActive(true);

		playRecordingButton.transform.Find("stop").gameObject.SetActive(false);
		playRecordingButton.transform.Find("play").gameObject.SetActive(true);
		playRecordingButton.gameObject.SetActive(false);

		btOK.onClick.AddListener(closeUp);
		btBack.onClick.AddListener(close);

		btOK.gameObject.SetActive(false);
	}

	void Update(){
		//if recording
		if (isRecording == true)
		{
			MicTime  += Time.deltaTime;
			string minutes = Mathf.Floor(MicTime / 60).ToString("0");
			string seconds = (MicTime % 60).ToString("00");
			time.text = minutes + ":" + seconds;
			//Debug.Log(RARE.Instance.volumeRMS*100);
			//circle.SetActive(true);
			circle.transform.localScale=new Vector3(1+RARE.Instance.volumeRMS, 1+RARE.Instance.volumeRMS, 1+RARE.Instance.volumeRMS);
		}  else {
			MicTime = 0;
			circle.transform.localScale=new Vector3(1,1,1);
			//circle.SetActive(false);
		}
		if (currentAsrc.clip != null && currentAsrc.isPlaying) {
			currentTrackTime = (float)currentAsrc.time;
			string minutes = Mathf.Floor(tracklength / 60).ToString("0");
			string seconds = (tracklength % 60).ToString("00");
			string tminutes = Mathf.Floor(currentTrackTime / 60).ToString("0");
			string tseconds = (currentTrackTime % 60).ToString("00");
			time.text = tminutes + ":" + tseconds + " / " + minutes + ":" + seconds;
		}else{
			if(isplaying){
				info.text = "";
				playRecordingButton.transform.Find("stop").gameObject.SetActive(false);
				playRecordingButton.transform.Find("play").gameObject.SetActive(true);
				isplaying = false;
			}
		}
	
		if (currentAsrc.clip == null) {
			return;
		}

		if (isplaying && PlayHeadTouch == false) { //updates the playhead
			//playbackSli.value = currentAsrc.timeSamples * currentAsrc.clip.channels;
		} 
	}

	public void openRecorder(Action<string> _methodForSuccess){
		panel.SetActive(true);
		methodForSuccess=_methodForSuccess;

		micRecordButton.transform.Find("stop").gameObject.SetActive(false);
			micRecordButton.transform.Find("record").gameObject.SetActive(true);
			playRecordingButton.gameObject.SetActive(false);
			btOK.gameObject.SetActive(false);

		info.text = "Listo para comenzar grabación.";
		time.text = "";
	}

	public void closeUp(){
		methodForSuccess(clipName);
		panel.SetActive(false);

		if (currentAsrc.isPlaying)
			currentAsrc.Pause();
	}

	public void close(){
		panel.SetActive(false);

		if (!isRecording) {
			MicStartStop();
		}

		if (currentAsrc.isPlaying)
			currentAsrc.Pause();
	}

	public void MicStartStop(){
		if (isRecording) {
			playRecordingButton.SetActive(true);
			RARE.Instance.StopMicRecording (CheckFileName("MicRecording"), ClipLoaded);
			recordNum++;
			isRecording = false;
			info.text = "Terminó grabación.";
			micRecordButton.transform.Find("stop").gameObject.SetActive(false);
			micRecordButton.transform.Find("record").gameObject.SetActive(true);
			btOK.gameObject.SetActive(true);
			playRecordingButton.gameObject.SetActive(true);
		} else {
			if (currentAsrc.isPlaying) {
				PlayStopRecording();
			}
			playRecordingButton.SetActive(false);
			info.text = "Grabando...";
			isRecording = true;
			RARE.Instance.StartMicRecording (599);
			micRecordButton.transform.Find("stop").gameObject.SetActive(true);
			micRecordButton.transform.Find("record").gameObject.SetActive(false);
			playRecordingButton.gameObject.SetActive(false);
			btOK.gameObject.SetActive(false);
		}
	}

	public void PlayStopRecording(){
		if (currentAsrc.isPlaying) {
			currentAsrc.Pause();
			info.text = "";
			playRecordingButton.transform.Find("stop").gameObject.SetActive(false);
			playRecordingButton.transform.Find("play").gameObject.SetActive(true);
			isplaying = false;

		} else {			
			currentAsrc.Play();
			info.text = "Oyendo grabación...";
			playRecordingButton.transform.Find("stop").gameObject.SetActive(true);
			playRecordingButton.transform.Find("play").gameObject.SetActive(false);
			isplaying = true;
		}
	}

	public void ClipLoaded(AudioClip myClip, string _clipName = null){
			Debug.Log("clipName: "+_clipName);
			clipName=_clipName;
			myClip.name = "untitled";
		currentAsrc.clip = myClip;		
		info.text = "Grabación concluida.";
		tracklength = (float)((myClip.length));
	}

	public string CheckFileName(string input){
		if (File.Exists (Application.persistentDataPath +"/"+ input + "(" + 1 + ").wav")) {
			int x = 2;
			while (File.Exists (Application.persistentDataPath +"/"+ input + "(" + x + ").wav")) {
				x++;
			}
			return input + "(" + x + ")";
		} else if (File.Exists (Application.persistentDataPath +"/"+ input + ".wav")) {
			return input + "(1)";
		} else {
			return input;
		}
	}

}
