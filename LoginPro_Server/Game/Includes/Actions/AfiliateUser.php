<?php
if(!isset($ServerScriptCalled)) { exit(0); }

// Check data presence of the new
$idUsuario = $datas[0];
$idGrupo = $datas[1];
$rol = $datas[2];

$query = "INSERT INTO ".$_SESSION['GruposAsignacionUsuariosTable']." (idUsuario,idGrupo,creation_date, rol) VALUES (:idUsuario,:idGrupo,NOW(),:rol)";
	$parameters = array(':idUsuario' => $idUsuario, ':idGrupo' => $idGrupo, ':rol' => $rol);
	$stmt = ExecuteQuery($query, $parameters);

$query = "UPDATE ".$_SESSION['AccountTable']." SET idGrupo = :idGrupo, validated=:validated WHERE id = :id";
	$parameters = array(':idGrupo' => $idGrupo,':id' => $idUsuario,':validated' => 1 );
	$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Usuario añadido";
$total['response']      = "true";

sendAndFinish(json_encode($total));
?>
