<?php
if(!isset($ServerScriptCalled)) { exit(0); }

$idBusqueda = $datas[0];
$idGrupo = $datas[1];

$query = "SELECT id, nombre_completo, avatar FROM ".$_SESSION['AccountTable']." WHERE idGrupo = :idGrupo AND id IN (SELECT idUsuario as id FROM ".$_SESSION['GruposAsignacionUsuariosTable'].") AND id NOT IN (SELECT idUsuario as id FROM ".$_SESSION['BusquedasAsignacionUsuariosTable']." WHERE idBusqueda = :idBusqueda AND activa = :activa) AND id != :account_id AND role != :role ORDER BY nombre_completo";

$parametersUsers = array(':idBusqueda' => $idBusqueda, ':idGrupo' => $idGrupo, ':account_id' => USER_ID, ':activa' => '1', ':role' => 'Admin');
$stmt = ExecuteQuery($query, $parametersUsers);



$total = array();

$total['titulo']     = "non busqueda usuarios";
$total['response']      = "true";
$total['resultados']      = sizeof($result);
        
$data = array();

while($user = $stmt->fetch(PDO::FETCH_ASSOC))
{
    $data[]=$user;
}

$total['data'] =$data;                

sendAndFinish(json_encode($total));

?>
