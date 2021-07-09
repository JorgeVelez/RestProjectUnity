<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// Datas
$name = $datas[0];
$color = $datas[1];
$location = $datas[2];
$idGrupo = $datas[3];

// Set the role
$query = "UPDATE ".$_SESSION['GruposTable']." SET name=:name,color=:color, location=:location WHERE id = :idGrupo";
$parameters = array(':name' => $name,':color' => $color,':location' => $location,':idGrupo' => $idGrupo);
$stmt = ExecuteQuery($query, $parameters);

$total = array();
$total['titulo']     = "Grupo actualizado";
$total['response']      = $idGrupo;
$total['name']      = $name;

sendAndFinish(json_encode($total));

?>