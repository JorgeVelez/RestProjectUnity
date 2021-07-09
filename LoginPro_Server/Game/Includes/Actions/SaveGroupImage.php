<?php

// A screenshot has been sent so a lot of character could be truncated by anti injection system : let's do it manually
$decryptedData = read_unsafe($_POST["EncryptedInfo"]);		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);

$id = $datas[0];
$nombre = $datas[1];
$nombre = preg_replace('/[\x00-\x1F\x80-\xFF]/', '', $nombre);
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/'.$nombre;
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/serve.php?file='.$nombre;


$query = "UPDATE ".$_SESSION['GruposTable']." SET avatar = :file WHERE id = :id";
$parameters = array(':file' => $nombreParaBase,':id' => $id);
$stmt = ExecuteQuery($query, $parameters);

$query = "SELECT avatar FROM ".$_SESSION['GruposTable']." WHERE id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);
$row = $stmt->fetch();

if(isset($row['avatar']))
{
	$fileData = $row["avatar"];
	$serverDatas = array($fileData);
	sendArrayAndFinish($serverDatas);
}

end_script("Error al añadir imagen.");

?>