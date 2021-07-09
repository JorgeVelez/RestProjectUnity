<?php



$nombre = $datas[0];
$location = $datas[1];

$query = "INSERT INTO ".$_SESSION['GruposTable']." (name,location,creation_date, color) VALUES (:name,:location,NOW(), '2299E1')";
$parameters = array(':name' => $nombre,':location' => $location);
$stmt = ExecuteQuery($query, $parameters);

$total['titulo']     = "Grupo Creado";
$total['response']      = "true";
        
sendAndFinish(json_encode($total));

?>