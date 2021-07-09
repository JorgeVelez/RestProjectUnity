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

updateToken($SID, $username);
updateActivitylogin($username);
updateConnectionDatelogin($username);

echo $SID;
}else{
	echo "false";
}
close();


?>