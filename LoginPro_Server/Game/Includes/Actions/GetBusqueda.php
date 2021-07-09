<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// LET'S BEGIN :
// Notice that $_SESSION['GamingTable'] is set in the script 'ServerSettings.php', if you want to add other tables: add them in 'Server.php' in the ******* TABLES ZONE ********
$id = $datas[0];
$query = "SELECT id, nombre,grupo,avatar, lugar, creation_date, activa, grupo FROM ".$_SESSION['BusquedasTable']." WHERE id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);
$item = $stmt->fetch(PDO::FETCH_ASSOC);

$total = array();

$total['titulo']     = "Búsqueda";
$total['response']      = "true";
$total['resultados']      = "1";
$milliseconds = round(microtime(true) * 1000);
$total['tokenKey']      =  encrypt($milliseconds);  
        
$data = array();

$queryUsers = "SELECT ".$_SESSION['BusquedasAsignacionUsuariosTable'].".rol AS rol, ".$_SESSION['AccountTable'].".nombre_completo AS nombre_completo,".$_SESSION['AccountTable'].".avatar AS avatar, ".$_SESSION['AccountTable'].".id AS id FROM ".$_SESSION['AccountTable']." JOIN ".$_SESSION['BusquedasAsignacionUsuariosTable']." ON ".$_SESSION['BusquedasAsignacionUsuariosTable'].".idUsuario = ".$_SESSION['AccountTable'].".id WHERE idBusqueda = :idUser AND activa = '1' ORDER BY nombre_completo";
$parametersUsers = array(':idUser' => $item['id']);
$stmtUsers = ExecuteQuery($queryUsers, $parametersUsers);

$dataUsers = array();
while($userGrupo = $stmtUsers->fetch(PDO::FETCH_ASSOC))
{
    $queryRol = "SELECT rol FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idUsuario=:account_id AND idGrupo=:idGrupo";
    $parametersRol = array(':account_id' => $userGrupo['id'], ':idGrupo' => $item['grupo']);
    $stmtRol = ExecuteQuery($queryRol, $parametersRol);
    $row = $stmtRol->fetch(PDO::FETCH_ASSOC);
    $userGrupo['rol_enGrupo']=$row['rol'];

    $dataUsers[]=$userGrupo;
}
    $item['users']=$dataUsers;
//$data[]=$item;

$queryTestimonios = "SELECT ".$_SESSION['TestimoniosTable'].".titulo AS titulo,".$_SESSION['TestimoniosTable'].".creation_date AS creation_date,".$_SESSION['TestimoniosTable'].".user_creation_date AS user_creation_date, ".$_SESSION['TestimoniosTable'].".id AS id, ".$_SESSION['TestimoniosTable'].".media AS media, ".$_SESSION['AccountTable'].".nombre_completo AS nombre_completo,".$_SESSION['AccountTable'].".avatar AS avatar_usuario, ".$_SESSION['AccountTable'].".id AS idUser FROM ".$_SESSION['AccountTable']." JOIN ".$_SESSION['TestimoniosTable']." ON ".$_SESSION['TestimoniosTable'].".creator_id = ".$_SESSION['AccountTable'].".id  WHERE idBusqueda = :iddeBusqueda ORDER BY user_creation_date";
    
$parametersTestimonios = array(':iddeBusqueda' => $item['id']);
$stmtTestimonios = ExecuteQuery($queryTestimonios, $parametersTestimonios);

$dataTestimonios = array();
while($testimonio = $stmtTestimonios->fetch(PDO::FETCH_ASSOC))
{
    $dataTestimonios[]=$testimonio;
}

$item['testimonios']=$dataTestimonios;

$queryAdmin = "SELECT rol FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idUsuario=:account_id AND idGrupo=:idGrupo";
$parametersAdmin = array(':account_id' => $account['id'], ':idGrupo' => $item['grupo']);
$stmtAdmin = ExecuteQuery($queryAdmin, $parametersAdmin);  
$itemAdmin = $stmtAdmin->fetch(PDO::FETCH_ASSOC);
$item['rol_enGrupo']="SuperAdministrador";
if(isset($itemAdmin['rol']))
    $item['rol_enGrupo']=$itemAdmin['rol'];

$data[]=$item;

$total['data'] =$data;                

sendAndFinish(json_encode($total));

?>
