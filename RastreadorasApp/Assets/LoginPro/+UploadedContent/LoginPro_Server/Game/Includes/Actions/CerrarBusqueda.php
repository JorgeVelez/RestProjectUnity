<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// Datas
$busquedaId = $datas[0];
$activa = '0';

// Set the role
$query = "UPDATE ".$_SESSION['BusquedasTable']." SET activa=:activa, idCerroBusqueda=:cerroId WHERE id = :busquedaId";
$parameters = array(':activa' => $activa,':busquedaId' => $busquedaId,':cerroId' => $account['id']);
$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Búsqueda cerrada".$role;
$total['response']      = "true";

sendAndFinish(json_encode($total));

?>