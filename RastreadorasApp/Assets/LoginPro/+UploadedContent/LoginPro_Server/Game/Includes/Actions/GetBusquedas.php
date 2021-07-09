<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

$query = "SELECT ".
    $_SESSION['GruposTable'].".name AS nombre_grupo,".
 $_SESSION['GruposTable'].".color AS color_grupo,".
    $_SESSION['GruposTable'].".id AS id_grupo,".
    $_SESSION['BusquedasTable'].".id, ".
    $_SESSION['BusquedasTable'].".grupo, ".
    $_SESSION['BusquedasTable'].".avatar, ".
    $_SESSION['BusquedasTable'].".activa, ".
    $_SESSION['BusquedasTable'].".creation_date,nombre, lugar FROM ".$_SESSION['BusquedasTable']."
JOIN ".$_SESSION['GruposTable']." 
    ON ".$_SESSION['BusquedasTable'].".grupo = ".$_SESSION['GruposTable'].".id 
WHERE ".$_SESSION['BusquedasTable'].".id IN (SELECT idBusqueda FROM ".$_SESSION['BusquedasAsignacionUsuariosTable']." WHERE idUsuario=:account_id)";

$parameters = array(':account_id' => $account['id']);

if($account['role']=="Administrador"){
   $query = "SELECT ".
    $_SESSION['GruposTable'].".name AS nombre_grupo,".
 $_SESSION['GruposTable'].".color AS color_grupo,".
    $_SESSION['GruposTable'].".id AS id_grupo,".
    $_SESSION['BusquedasTable'].".id, ".
    $_SESSION['BusquedasTable'].".grupo, ".
    $_SESSION['BusquedasTable'].".avatar, ".
    $_SESSION['BusquedasTable'].".activa, ".
    $_SESSION['BusquedasTable'].".creation_date,nombre, lugar FROM ".$_SESSION['BusquedasTable']."
JOIN ".$_SESSION['GruposTable']." 
    ON ".$_SESSION['BusquedasTable'].".grupo = ".$_SESSION['GruposTable'].".id";
    
    $stmt = ExecuteQuery($query);  
}else{
  $stmt = ExecuteQuery($query, $parameters);  
}

$total = array();

$total['titulo']     = "get Busquedas";
$total['response']      = "true";
        
$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC))
{
    $data[]=$item;
}
$total['resultados']      = sizeof($data);
$total['data'] =$data;                

sendAndFinish(json_encode($total));

?>
