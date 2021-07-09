<?php
if(!isset($ServerScriptCalled)) { exit(0); }

$query = "SELECT id, nombre_completo, avatar FROM ".$_SESSION['AccountTable']." WHERE current_game = :game_id AND role != :role";
$parameters = array(':game_id' => GAME_ID, ':role' => 'Admin');
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
$total['administrador'] ="true";   

sendAndFinish(json_encode($total));

?>
