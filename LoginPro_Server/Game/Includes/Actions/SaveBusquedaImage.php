<?php
$decryptedData = read_unsafe($_POST["EncryptedInfo"]);		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);

// Protect against injection for the message and encode in base64 the screenshot so all injection characters are encoded
$id = $datas[0];
$nombre = $datas[1];
$nombre = preg_replace('/[\x00-\x1F\x80-\xFF]/', '', $nombre);
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/'.db_utf8_convert($nombre);
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/serve.php?file='.db_utf8_convert($nombre);

$query = "UPDATE ".$_SESSION['BusquedasTable']." SET avatar = :file WHERE id = :id";
$parameters = array(':file' => $nombreParaBase,':id' => $id);
$stmt = ExecuteQuery($query, $parameters);

// Get the file back and send it to the client
$query = "SELECT avatar FROM ".$_SESSION['BusquedasTable']." WHERE id = :id";
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

end_script("Cannot get savegame back.");

function db_utf8_convert($str)
{
    $convmap = array(0x80, 0x10ffff, 0, 0xffffff);
    return preg_replace('/\x{EF}\x{BF}\x{BD}/u', '', mb_encode_numericentity($str, $convmap, "UTF-8"));
}

?>