<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// Datas
$userID = $datas[0];
$role = $datas[1];

// Set the role
$query = "UPDATE ".$_SESSION['GruposAsignacionUsuariosTable']." SET rol=:role WHERE idUsuario = :userID";
$parameters = array(':role' => $role,':userID' => $userID);
$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario con rol".$role;
$total['response']      = "true";

sendAndFinish(json_encode($total));

?>