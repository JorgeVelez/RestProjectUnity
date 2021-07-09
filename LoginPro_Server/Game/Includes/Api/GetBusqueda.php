<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

if(isset($_POST['Action'])) { $id = $_POST['idBusqueda']; }
else if(isset($_GET['Action'])) { $id = $_GET['idBusqueda']; }

$tokenKey=(int)decrypt($_GET['tokenKey']);
if(isset($_POST['tokenKey'])) { $tokenKey=(int)decrypt($_POST['tokenKey']); }
//$tokenKey=decrypt("j6R7bW9ypiJPAQ/M/9CRrv1sUaYK/vPPWlZhN/wFpQudKaaW/DIRUU56V/aiXD1D2y2uWkWVbYiZuUhkcouaVg==");
$milliseconds = round(microtime(true) * 1000);
$transcurrido=round($milliseconds / 1000)-round($tokenKey / 1000);

//if($transcurrido >300) { end_script('Session invalida.'); }	

$query = "SELECT id, nombre,grupo,avatar as avatar_busqueda, lugar, creation_date, activa FROM ".$_SESSION['BusquedasTable']." WHERE id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);

$total = array();

//$total['titulo']     = "Búsqueda api";
//$total['response']      = "true";
//$milliseconds = round(microtime(true) * 1000);


$busqueda = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC))
{    
    $queryTestimonios = "SELECT ".$_SESSION['TestimoniosTable'].".titulo AS titulo,"
        .$_SESSION['TestimoniosTable'].".creation_date AS creation_date, ".
        $_SESSION['TestimoniosTable'].".id AS id, ".
        $_SESSION['TestimoniosTable'].".donde_estas AS lugar, ".
        $_SESSION['TestimoniosTable'].".coordenadas AS coordenadas, ".
        $_SESSION['TestimoniosTable'].".lat AS lat, ".
        $_SESSION['TestimoniosTable'].".lon AS lon, ".
        $_SESSION['TestimoniosTable'].".notas AS notas, ".
        //$_SESSION['TestimoniosTable'].".media AS media, ".
        $_SESSION['MediaTable'].".file_name AS media, ".
        $_SESSION['AccountTable'].".nombre_completo AS nombre_usuario,".
        $_SESSION['AccountTable'].".avatar AS avatar_usuario, ".
        $_SESSION['AccountTable'].".id AS idUser FROM ".$_SESSION['AccountTable'].
        " JOIN ".$_SESSION['TestimoniosTable'].
        " ON ".$_SESSION['TestimoniosTable'].".creator_id = ".$_SESSION['AccountTable'].".id  
         JOIN ".$_SESSION['MediaTable'].
        " ON ".$_SESSION['TestimoniosTable'].".media = ".$_SESSION['MediaTable'].".id  
        WHERE idBusqueda = :iddeBusqueda";
      
	$parametersTestimonios = array(':iddeBusqueda' => $item['id']);
    $stmtTestimonios = ExecuteQuery($queryTestimonios, $parametersTestimonios);
    
    $dataTestimonios = array();
    while($testimonio = $stmtTestimonios->fetch(PDO::FETCH_ASSOC))
    {
        $testimonio['media']="http://thisisnotanumber.org/rastreadoras/media/".$testimonio['media'];
        $testimonio['avatar_usuario']="http://thisisnotanumber.org/rastreadoras/media/".$testimonio['avatar_usuario'];
        $dataTestimonios[]=$testimonio;
    }
     $item['testimonios']=$dataTestimonios;
    $item['transcurrido']      = $transcurrido;
    $busqueda[]=$item;
}

$total['busqueda'] =$busqueda;                

 header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');
    
	 $myJSON=json_encode($busqueda);
echo $_GET['callback']."(".$myJSON.");";
/*echo json_encode(array(
'name' => 'my',
'title' => 'test'
));*/
	$_SESSION['databaseConnection'] = null;
	die();
	exit(0);

?>
