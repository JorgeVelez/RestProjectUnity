<?php
if(!isset($ServerScriptCalled)) { exit(0); }

// Check data presence of the new
$idUsuario = $datas[0];

$query = "DELETE FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idUsuario = :account_id";
$parameters = array(':account_id' => $idUsuario);
	$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario desligado";
$total['response']      = "true";

sendAndFinish(json_encode($total));
?>
