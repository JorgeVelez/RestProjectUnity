<?php

include_once 'funciones.php'; 
include_once 'Includes/Functions.php'; 	
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

$tokenReceived = $_SERVER['HTTP_AUTHORIZATION'];
$id = $_GET['id'];	

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');
	
if(!checkAuthenticationFetch($tokenReceived)){
    header("HTTP/1.1 401 Unauthorized");
  echo json_encode(array( 'error' => 'Session invalida' ));
   close();
}
if (isset($_GET['id'])) {
    $query = "SELECT id, nombre, grupo, avatar as avatar_busqueda, lugar, creation_date, activa FROM ".$_SESSION['BusquedasTable']." WHERE id = :id";
    $parameters = array(':id' => $id);
    $stmt = ExecuteQuery($query, $parameters);

    $items = $stmt->fetch(PDO::FETCH_ASSOC);

    if(isset($items['id'])){
      // item founded, send it
      header("HTTP/1.1 200 OK");
      $myJSON=json_encode($items);
      echo $_GET['callback']."(".$myJSON.");";
    } else {
       // item not found, respond with 404
       header("HTTP/1.1 404 Not Found");
       echo json_encode(array( 'error' => 'Invalid ID' ));
    }

  }

close();
?>