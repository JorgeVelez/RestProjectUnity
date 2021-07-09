<?php

include_once 'funciones.php'; 
include_once 'Includes/Functions.php'; 	
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

//$tokenReceived = $_POST["token"];	

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
//header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');
	
//if(!checkAuthenticationFetch($tokenReceived)){
  //  header("HTTP/1.1 401 Unauthorized");
 // echo json_encode(array( 'error' => 'Session invalida' ));
  // close();
//}
$query = "SELECT id, nombre, grupo, avatar as avatar_busqueda, lugar, creation_date, activa FROM ".$_SESSION['BusquedasTable'];
$stmt = ExecuteQuery($query);

$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC)){
    $data[]=$item;
}              
$myJSON=json_encode($data);
header("HTTP/1.1 200 OK");
echo $_GET['callback']."(".$myJSON.");";

close();
?>