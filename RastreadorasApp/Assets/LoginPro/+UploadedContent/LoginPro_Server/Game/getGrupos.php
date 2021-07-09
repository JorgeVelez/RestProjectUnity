<?php

include_once 'funciones.php'; 
include_once 'Includes/Functions.php'; 	
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

$tokenReceived = $_POST["token"];	
if (isset($_GET['page'])) $page = $_GET['page'];
else $page = 0;

if (isset($_GET['items'])) $itemByPage = $_GET['items'];
else $itemByPage = 25;

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');
	
if(!checkAuthenticationFetch($tokenReceived)){
    header("HTTP/1.1 401 Unauthorized");
  echo json_encode(array( 'error' => 'Session invalida' ));
   close();
}
$query = "SELECT id, name,avatar, location, color,creation_date FROM ".$_SESSION['GruposTable'];
$stmt = ExecuteQuery($query);

$data = array();

while($item = $stmt->fetch(PDO::FETCH_ASSOC)){
    $data[]=$item;
}              
$myJSON=json_encode($data);

echo $_GET['callback']."(".$myJSON.");";

close();
?>