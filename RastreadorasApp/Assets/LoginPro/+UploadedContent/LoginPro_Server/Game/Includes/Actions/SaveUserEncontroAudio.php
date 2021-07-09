<?php
$decryptedData = read_unsafe($_POST["EncryptedInfo"]);		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);

// Protect against injection for the message and encode in base64 the screenshot so all injection characters are encoded
$id = $datas[0];
$nombre = $datas[1];
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/'.$nombre;


$query = "UPDATE ".$_SESSION['AccountTable']." SET estado_busqueda = :file WHERE id = :id";
$parameters = array(':file' => $nombreParaBase,':id' => $id);
$stmt = ExecuteQuery($query, $parameters);



$total['titulo']     = "Estatus cambiado";
$total['response']      = "true";
        
sendAndFinish(json_encode($total));

?>