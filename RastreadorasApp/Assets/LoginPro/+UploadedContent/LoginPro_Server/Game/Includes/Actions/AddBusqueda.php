<?php
if(!isset($ServerScriptCalled)) { exit(0); }

// Check data presence of the new
$nombre = $datas[0];
$location = $datas[1];
$grupo = $datas[2];

$query = "INSERT INTO ".$_SESSION['BusquedasTable']." (lugar,nombre,creation_date, grupo,activa, creator_id) VALUES (:lugar,:nombre,NOW(),:grupo, '1',:creator)";
	$parameters = array(':lugar' => $location, ':grupo' => $grupo, ':nombre' => $nombre, ':creator' => $account['id']);
	$stmt = ExecuteQuery($query, $parameters);

$idUsuario = $account['id'];
$idBusqueda = $_SESSION['databaseConnection'] ->lastInsertId();

if($account['role']!="Administrador"){
$query = "INSERT INTO ".$_SESSION['BusquedasAsignacionUsuariosTable']." (idUsuario,idBusqueda,creation_date, rol, creator_id, activa) VALUES (:idUsuario,:idBusqueda,NOW(),'Administrador',:account_id, '1')";
	$parameters = array(':idUsuario' => $idUsuario, ':idBusqueda' => $idBusqueda, ':account_id' => $idUsuario);
	$stmt = ExecuteQuery($query, $parameters);
}
$total = array();
$total['titulo']     = "Busqueda creada";
$total['response']      = "true";

sendAndFinish(json_encode($total));
?>
