<?php
if(!isset($ServerScriptCalled)) { exit(0); }

// Check data presence of the new
$idUsuario = $datas[0];
$idBusqueda = $datas[1];

$query = "DELETE FROM ".$_SESSION['BusquedasAsignacionUsuariosTable']." WHERE idUsuario = :account_id AND idBusqueda = :idBusqueda";

$parameters = array(':account_id' => $idUsuario, ':idBusqueda' => $idBusqueda);
	$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario desligado de busqueda";
$total['response']      = "true";

sendAndFinish(json_encode($total));
?>
