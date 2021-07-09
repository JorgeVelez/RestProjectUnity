<?php

// include_once 'funciones.php';
// include_once 'Includes/Functions.php';
// include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
// include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

// $tokenReceived = $_POST["token"];
// $tokenReceived=(int)decrypt($_SERVER['HTTP_AUTHORIZATION']);

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
// header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');


header("HTTP/1.1 200 OK");
// echo $myJSON;
// echo $_GET['callback']."(".$myJSON.");";

$list = array(
  array(
    'id' => 1,
    'name' => 'Busqueda 1',
    'place' => 'Sinaloa',
    'group_id' => 1,
    'group_name' => 'Grupo 1'
  ),
  array(
    'id' => 2,
    'name' => 'Busqueda 2',
    'place' => 'Veracruz',
    'group_id' => 1,
    'group_name' => 'Grupo 1'
  ),
  array(
    'id' => 3,
    'name' => 'Busqueda 3',
    'place' => 'Veracruz',
    'group_id' => 1,
    'group_name' => 'Grupo 1'
  )
);

echo json_encode(array(
  'list' => $list,
  'pages' => 12,
  'token' => $tokenReceived
));

// close();
?>
