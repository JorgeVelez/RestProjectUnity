using System.Net;
using System.IO;
using System.Text;
using System;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

    public class FTPClient {

        // The hostname or IP address of the FTP server
        private string _remoteHost;

        // The remote username
        private string _remoteUser;

        // Password for the remote user
	private string _remotePass;

	public float totalBytesRead=0;
	
	private Thread downloadThread;
	private Thread uploadThread;

	private long fileSizeVar;

	private float PorcentajeBajado=0;
	private float 	porcentajeSubido=0;

	private FtpWebRequest Downloadrequest=null;

	public bool isDownloading=false;

	public event FTPClient.onComplete onCompleteEvent;
	public delegate void onComplete(string url, long fileSizeVar);

	public event FTPClient.onError onErrorEvent;
	public delegate void onError(string msj);

        public FTPClient(string remoteHost, string remoteUser, string remotePassword) {
            _remoteHost = remoteHost;
            _remoteUser = remoteUser;
            _remotePass = remotePassword;

		if(Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath+"/documentos/")))
			DeleteDirectory(Path.GetDirectoryName(Application.persistentDataPath+"/documentos/"));

		Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath+"/documentos/"));

		//request.Method = "NLST";
        }

        public string DirectoryListing(string folder) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + folder);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string result = string.Empty; 

            while (!reader.EndOfStream) {
                result += reader.ReadLine() + Environment.NewLine;
            }

            reader.Close();
            response.Close();
            return result;
        }

		

	public void StartDownloadHandler(string file, string destination){
		downloadThread =  new Thread(() => FileSize(file,destination));
		downloadThread.Start ();
		
	}

	public void StartUploadHandler(string file, string destination){
		uploadThread =  new Thread(() => Upload(file,destination));
		uploadThread.Start ();
		
	}

	public void Upload (string FullPathFilename,string folderAndNameOnServer)
	{
		var file = new FileInfo(FullPathFilename);
		//var address = new Uri("ftp://" + server + "/" + Path.Combine(initialPath, file.Name));
		var request = FtpWebRequest.Create(_remoteHost + "/"+folderAndNameOnServer) as FtpWebRequest;

		request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
		request.KeepAlive = false;
		//request.EnableSsl=true;
		request.Method = WebRequestMethods.Ftp.UploadFile;
		request.UseBinary = true;
		request.ContentLength = file.Length;
		var bufferLength = 2048;
		var buffer = new byte[bufferLength];
		var contentLength = 0;
		var fs = file.OpenRead();
		int totalReadBytesCount = 0;

		try {
			var stream = request.GetRequestStream();
			contentLength = fs.Read(buffer, 0, bufferLength);
			while (contentLength != 0) {
				stream.Write(buffer, 0, contentLength);
				contentLength = fs.Read(buffer, 0, bufferLength);
                porcentajeSubido = (float)totalReadBytesCount/ (float)fs.Length;
				if(porcentajeSubido>1)porcentajeSubido=1;
				totalReadBytesCount += bufferLength;
			}

			stream.Close();
			fs.Close();
			porcentajeSubido=0;

			onCompleteEvent ("true", 0);
		} catch (Exception e) {
			onErrorEvent ("Error FTP:"+e.Message);
			return;
		}

	}

	public void FileSize(string file, string destination) {
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
		request.Method = WebRequestMethods.Ftp.GetFileSize;
		request.Credentials = new NetworkCredential(_remoteUser, _remotePass);

		try{
			using (FtpWebResponse response =(FtpWebResponse)request.GetResponse()){
				response.Close();

				fileSizeVar = response.ContentLength;
				DownloadFtpFile (file,destination);
				return ;
			}
		}
		catch (Exception ex){
			if (onErrorEvent != null)
				onErrorEvent ("Error FTPfs:"+ex.Message);
				return;
		}
	}

	public void abortDownloadRequest(){
		if (isDownloading) {
			Downloadrequest.Abort ();
			isDownloading = false;

			PorcentajeBajado=0;
			totalBytesRead = 0;
		}
	}

	public void DownloadFtpFile(string file, string savePath){
		totalBytesRead = 0;

		Downloadrequest = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
		Downloadrequest.Method = WebRequestMethods.Ftp.DownloadFile;
		Downloadrequest.Credentials = new NetworkCredential(_remoteUser, _remotePass);
		//request.UseBinary = true;
		isDownloading = true;

		try{
			using (FtpWebResponse response = (FtpWebResponse)Downloadrequest.GetResponse ()){
				Stream rs = response.GetResponseStream ();

				byte[] buffer = new byte[2048];
				int bytesRead = rs.Read(buffer, 0, buffer.Length);

				FileStream ws = new FileStream (savePath, FileMode.Create);

				totalBytesRead += bytesRead;

				while (bytesRead > 0){
					ws.Write(buffer, 0, bytesRead);
					bytesRead = rs.Read(buffer, 0, buffer.Length);
					totalBytesRead += bytesRead;
					PorcentajeBajado=totalBytesRead/fileSizeVar;
					if(porcentajeSubido>1)porcentajeSubido=1;
				}
				response.Close();
				ws.Close ();
				rs.Close();


				if (response == null) {
					fileSizeVar = 0;
					savePath = "null";
				}

				isDownloading = false;
				if (onCompleteEvent != null)
					onCompleteEvent (savePath, fileSizeVar);
				PorcentajeBajado=0;
				totalBytesRead = 0;

				return ;
			}
		}
		catch (Exception ex){
			if (ex.Message.Contains("aborted"))
				return;
			
			if (onErrorEvent != null)
				onErrorEvent ("Error FTP:"+ex.Message);
			return;
		}
	}

	public float getPorcentajeBajado(){
		return PorcentajeBajado;
	}

	public float getPorcentajeSubido(){
		return porcentajeSubido;
	}

	public static void DeleteDirectory(string path)
	{
		foreach (string directory in Directory.GetDirectories(path))
		{
			DeleteDirectory(directory);
		}

		try
		{
			Directory.Delete(path, true);
		}
		catch (IOException) 
		{
			Directory.Delete(path, true);
		}
		catch (UnauthorizedAccessException)
		{
			Directory.Delete(path, true);
		}
	}

	/*public void DownloadFtpFileAsync(string file, string savePath)
	{
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
		request.Method = WebRequestMethods.Ftp.DownloadFile;
		request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
		//request.UseBinary = true;
		//request.Proxy = true;
		//request.UsePassive = true;
		//request.KeepAlive = t;
		
	}



	public void DownloadFS(string file, string destination) {
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
		request.Method = WebRequestMethods.Ftp.DownloadFile;
		request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
		request.UseBinary = true;

		FtpWebResponse response = (FtpWebResponse)request.GetResponse();
		Stream responseStream = response.GetResponseStream();
		StreamReader reader = new StreamReader(responseStream);
		
		StreamWriter writer = new StreamWriter(destination);
		writer.Write(reader.ReadToEnd());
		
		writer.Close();
		reader.Close();
		response.Close();
	}

	public string DownloadToString(string file, string destination) {
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
		request.Method = WebRequestMethods.Ftp.DownloadFile;
		request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
		FtpWebResponse response = (FtpWebResponse)request.GetResponse();
		Stream responseStream = response.GetResponseStream();
		StreamReader reader = new StreamReader(responseStream);
		
		string result = reader.ReadToEnd(); 

		StreamWriter writer = new StreamWriter(destination);
		writer.Write(result);
		writer.Close();
		reader.Close();
		response.Close();
		return result;
	}



        public void UploadFile(string FullPathFilename) {
            string filename = Path.GetFileName(FullPathFilename);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + filename);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_remoteUser, _remotePass);

            StreamReader sourceStream = new StreamReader(FullPathFilename);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());

            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            response.Close();
            requestStream.Close();
            sourceStream.Close();
        }
*/


    }	
