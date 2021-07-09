<?php
$decryptedData = read_unsafe($_POST["EncryptedInfo"]);		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);

// Protect against injection for the message and encode in base64 the screenshot so all injection characters are encoded
$id = $datas[0];
$nombre = $datas[1];
$nombre = preg_replace('/[\x00-\x1F\x80-\xFF]/', '', $nombre);
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/'.$nombre;
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/serve.php?file='.$nombre;


//create_cropped_thumbnail('media/'.$nombre, 550, 300, 'media/'.$nombreNuevo);

$query = "UPDATE ".$_SESSION['AccountTable']." SET avatar = :file WHERE id = :id";
$parameters = array(':file' => $nombreParaBase,':id' => $id);
$stmt = ExecuteQuery($query, $parameters);

// Get the file back and send it to the client
$query = "SELECT avatar FROM ".$_SESSION['AccountTable']." WHERE id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);
$row = $stmt->fetch();

// SUCCESS
if(isset($row['avatar']))
{
	$fileData = $row["avatar"];
	$serverDatas = array($fileData);
	sendArrayAndFinish($serverDatas);
}

end_script("Error al actualizar imagen.");

?>