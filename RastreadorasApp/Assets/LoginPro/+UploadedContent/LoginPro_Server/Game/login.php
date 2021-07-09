<?php
$debug = true;
$ServerScriptCalled = 1;

$_POST["NotConnectedYet"]=true;
include_once 'funciones.php'; 	
include_once 'Includes/Functions.php'; 
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

$username = $_POST["username"];	
$password = $_POST["password"];	

$connection_granted = passwordVerificationlogin($username, $password);

if($connection_granted){
$SID=generateRandomToken(128);
//$SID="123";


$info=updateToken($SID, $username);
updateActivitylogin($username);
updateConnectionDatelogin($username);

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');

$data = array();
$data['token']=$SID;
$data['data']=$info;
header("HTTP/1.1 200 OK");
echo json_encode($data);
}else{
	header("HTTP/1.1 404 Not Found");
    echo json_encode(array( 'error' => 'credenciales invalidas' ));
}
close();


?>