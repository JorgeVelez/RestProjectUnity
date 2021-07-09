<?php

include_once 'funciones.php';
include_once 'Includes/Functions.php';
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

$tokenReceived = $_GET["token"];
// $tokenReceived = $_POST["token"];
// $authReceived = $_SERVER['HTTP_AUTHORIZATION'];
// $tokenReceived="123";

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
// header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');
//
if(!checkAuthenticationFetch($tokenReceived)){
  header("HTTP/1.1 401 Unauthorized");
  echo json_encode(array( 'error' => 'Session invalida' ));
  close();
} else {

  if (isset($_GET['user'])) {

    // retrive params
    $userId = $_GET['user'];

    // call db
    $query = "SELECT
      id,
      a1,
      a2,
      userID,
      creation_date,
      a3,
      nombreCompleto,
      sexo,
      fechaNacimiento,
      estadoNacimiento,
      municipioNacimiento,
      localidadNacimiento,
      estadoResidencia,
      municipioResidencia,
      localidadResidencia,
      ocupacion,
      estadoCivil,
      parentezcoDesaparecido,
      familiarDesaparecido
     FROM ".$_SESSION['CuestionarioTable']." WHERE userID = :id";
    $parameters = array(':id' => $userId);
    $stmt = ExecuteQuery($query, $parameters);

    $items = array();
    while($item = $stmt->fetch(PDO::FETCH_ASSOC)){
      $items[]=$item;
    }

    if ($items[0]) {
      // item founded, send it
      header("HTTP/1.1 200 OK");
      $myJSON=json_encode($items[0]);
      echo $myJSON;
    } else {
       // item not found, respond with 404
       header("HTTP/1.1 404 Not Found");
       echo json_encode(array( 'error' => 'Invalid user ID' ));
    }
  } else {
     // item not found, respond with 404
     header("HTTP/1.1 404 Not Found");
     echo json_encode(array( 'error' => 'Missing user ID' ));
  }
}

close();
?>
