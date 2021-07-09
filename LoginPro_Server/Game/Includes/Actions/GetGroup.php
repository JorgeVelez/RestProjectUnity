<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// LET'S BEGIN :
// Notice that $_SESSION['GamingTable'] is set in the script 'ServerSettings.php', if you want to add other tables: add them in 'Server.php' in the ******* TABLES ZONE ********
$id = $datas[0];
$query = "SELECT id, name,avatar, location, color,creation_date FROM ".$_SESSION['GruposTable']." WHERE id = :id";
$parameters = array(':id' => $id);
$stmt = ExecuteQuery($query, $parameters);


$queryAdmin = "SELECT id, rol FROM ".$_SESSION['GruposAsignacionUsuariosTable']." WHERE idUsuario=:account_id AND idGrupo=:idGrupo";
$parametersAdmin = array(':account_id' => $account['id'], ':idGrupo' => $id);
$stmtAdmin = ExecuteQuery($queryAdmin, $parametersAdmin);  
$itemAdmin = $stmtAdmin->fetch(PDO::FETCH_ASSOC);
$esAdmin="-1";
if(isset($itemAdmin['id']))
    $esAdmin=$itemAdmin['rol'];

$total = array();

$total['titulo']     = "grupo";
$total['response']      = "true";
$total['resultados']      = "1";
        
$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC))
{
    $queryUsers = "SELECT ".$_SESSION['GruposAsignacionUsuariosTable'].".rol AS rol, ".$_SESSION['AccountTable'].".nombre_completo AS nombre_completo,".$_SESSION['AccountTable'].".avatar AS avatar, ".$_SESSION['AccountTable'].".id AS id FROM ".$_SESSION['AccountTable']." JOIN ".$_SESSION['GruposAsignacionUsuariosTable']." ON ".$_SESSION['GruposAsignacionUsuariosTable'].".idUsuario = ".$_SESSION['AccountTable'].".id WHERE ".$_SESSION['GruposAsignacionUsuariosTable'].".idGrupo = :idUser ORDER BY FIELD(rol,'SuperAdministrador','Administrador','Usuario'), nombre_completo";  
	$parametersUsers = array(':idUser' => $item['id']);
    $stmtUsers = ExecuteQuery($queryUsers, $parametersUsers);
    
    $dataUsers = array();
    while($userGrupo = $stmtUsers->fetch(PDO::FETCH_ASSOC))
    {
        $dataUsers[]=$userGrupo;
    }
    $item['esAdmin']=$esAdmin;
     $item['users']=$dataUsers;
    $data[]=$item;
}

$total['data'] =$data;                

sendAndFinish(json_encode($total));

?>
