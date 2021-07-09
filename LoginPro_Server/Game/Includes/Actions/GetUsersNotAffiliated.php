<?php
if(!isset($ServerScriptCalled)) { exit(0); }

$idGrupo = $datas[0];

$query = "SELECT id, nombre_completo, avatar, idGrupo  FROM ".$_SESSION['AccountTable']." WHERE idGrupo = :idGrupo2 AND id NOT IN (SELECT idUsuario as id FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idGrupo= :idGrupo) AND id != :account_id AND role != :role ORDER BY nombre_completo";
$parameters = array(':account_id' => USER_ID,':idGrupo2' => $idGrupo,':idGrupo' => $idGrupo, ':role' => 'Administrador');
$stmt = ExecuteQuery($query, $parameters);

$total = array();

$total['titulo']     = "usuarios";
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
