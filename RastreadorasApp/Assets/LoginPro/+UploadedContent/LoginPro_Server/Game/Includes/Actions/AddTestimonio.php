<?php



//$creator_id = $datas[0];
$idBusqueda = $datas[0];
$titulo = $datas[1];
$donde_estas = $datas[2];
$notas = $datas[3];
$lat = $datas[4];
$lon = $datas[5];
$media= $datas[6]; 
$nombre = $datas[7];
$user_creation_date = $datas[8];
$nombre = preg_replace('/[\x00-\x1F\x80-\xFF]/', '', $nombre);
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/'.$nombre;
$nombreParaBase = 'http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/serve.php?file='.$nombre;

//$getCon =file_put_contents('media/'.$nombre,$media);
if($nombre=="")
$nombreParaBase="";

$query = "INSERT INTO ".$_SESSION['TestimoniosTable']." (creator_id,idBusqueda,titulo,donde_estas,notas,coordenadas, media,lat,lon,user_creation_date, creation_date) VALUES (:creator_id,:idBusqueda,:titulo,:donde_estas,:notas,:coordenadas,:media,:lat,:lon,:user_creation_date,NOW())";
$parameters = array(':creator_id' => USER_ID,':idBusqueda' => $idBusqueda,':titulo' => $titulo,':donde_estas' => $donde_estas,':notas' => $notas,':coordenadas' => $lat.",".$lon,':media' => $nombreParaBase,':lat' => $lat,':lon' => $lon,':user_creation_date' => $user_creation_date);
$stmt = ExecuteQuery($query, $parameters);

$total['titulo']     = "Testimonio Creado";
$total['response']      = "true";
$total['nombre']      = $nombre;
$total['byts']      = $getCon;
        
sendAndFinish(json_encode($total));

?>