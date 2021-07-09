<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


$query = "SELECT id, name, location,avatar,color, creation_date FROM ".$_SESSION['GruposTable']." WHERE ".$_SESSION['GruposTable'].".id IN (SELECT idGrupo FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idUsuario=:account_id)";
$parameters = array(':account_id' => $account['id']);

if($account['role']=="Administrador"){
   $query = "SELECT id, name, location, avatar,color ,creation_date FROM ".$_SESSION['GruposTable']; 
    $stmt = ExecuteQuery($query);  
}else{
  $stmt = ExecuteQuery($query, $parameters);  
}

$total = array();

$total['titulo']     = "grupos";
$total['response']      = "true";

        
$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC))
{
    $data[]=$item;
}
$total['resultados']      = sizeof($data);
$total['query']      = $query." - ".USER_ID;
$total['data'] =$data;                

sendAndFinish(json_encode($total));

?>
