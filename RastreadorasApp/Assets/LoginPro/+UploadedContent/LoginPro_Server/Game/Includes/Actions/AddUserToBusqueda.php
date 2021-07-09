<?php
if(!isset($ServerScriptCalled)) { exit(0); }

// Check data presence of the new
$idUsuario = $datas[0];
$idBusqueda = $datas[1];

$query = "INSERT INTO ".$_SESSION['BusquedasAsignacionUsuariosTable']." (idUsuario,idBusqueda,creation_date, rol, creator_id, activa) VALUES (:idUsuario,:idBusqueda,NOW(),'Usuario',:account_id, '1')";
	$parameters = array(':idUsuario' => $idUsuario, ':idBusqueda' => $idBusqueda, ':account_id' => USER_ID);
	$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario añadido a busqueda";
$total['response']      = "true";

sendAndFinish(json_encode($total));
?>
