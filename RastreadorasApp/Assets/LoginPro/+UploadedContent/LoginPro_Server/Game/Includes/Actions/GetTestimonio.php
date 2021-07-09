<?php
if(!isset($ServerScriptCalled)) { exit(0); }

$id = $datas[0];

$query = "SELECT ".$_SESSION['AccountTable'].".nombre_completo AS nombre_completo,".$_SESSION['AccountTable'].".avatar AS avatar_usuario,".$_SESSION['AccountTable'].".id AS id, creator_id, ".$_SESSION['TestimoniosTable'].".creation_date, user_creation_date, titulo, donde_estas, lat, lon, notas, media FROM ".$_SESSION['TestimoniosTable']." JOIN ".$_SESSION['AccountTable']." ON ".$_SESSION['TestimoniosTable'].".creator_id = ".$_SESSION['AccountTable'].".id WHERE ".$_SESSION['TestimoniosTable'].".id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);

$total = array();

$total['titulo']     = "Detalle testimonio";
$total['response']      = "true";
$total['resultados']      = "1";
        
$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC))
{
   //$item['media']= base64_decode($item['media']);
    $data[]=$item;
}

$total['testimonios'] =$data;                

sendAndFinish(json_encode($total));

?>
