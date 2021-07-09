<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// Datas
$mail = $datas[0];
$username = $datas[0];
$password = $datas[1];
$nombreCompleto = $datas[2];
$currentUsername=USERNAME;

$account = getAccount($currentUsername);
	
	// Information to update
	if($mail!="" && $mail != $account['mail'])
	{
		if(checkMailExists($mail)) { end_script('Correo existente.'); } // Check if email address doesn't already exist
	}
	if($username!="" && $username != $account['username'])
	{
		if(checkUsernameExists($username)) { end_script('This username is already in use.'); } // Check if username doesn't already exist
	}
	if($password!="")
	{
		$salt = generateRandomString(50);
		$passwordHash = hashPassword($password,$salt);
	}

// Set the role
$query = "UPDATE ".$_SESSION['AccountTable']." SET mail = :mail, username = :username, password = :password,nombre_completo = :nombre_completo, salt = :salt WHERE id = :id";
$parameters = array(':mail' => $mail,':username' => $username,':password' => $passwordHash,':nombre_completo' => $nombreCompleto,':salt' => $salt,':id' => $_SESSION['user_id']);
$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario actualizado";
$total['response']      = "true";

sendAndFinish(json_encode($total));

?>