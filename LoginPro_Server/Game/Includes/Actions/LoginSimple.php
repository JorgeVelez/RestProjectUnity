<?php
$debug = true;
$ServerScriptCalled = 1;

include_once 'Includes/Functions.php'; 	
include_once 'Includes/miniCrypto.php';
$_POST["NotConnectedYet"]=true;
include_once 'Includes/Session.php'; 					// Begin client session (or return to index if not session ID received)
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

if(isset($_POST['Action'])) { $ACTION = $_POST['Action']; }
else { end_script('Server: No action has been received.'); }

$decryptedData = $_POST["EncryptedInfo"];		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);

$username = $datas[0];
$password = $datas[1];
$SID=$datas[2];

//checkAuthentification($datas[0], $datas[1], "Rastreadoras", "", $SID);	
$connection_granted = 1;

echo $SID;
$_SESSION['databaseConnection'] = null;
	// Ensure the end of the current script
	die();
	exit(0);

/*
if($connection_granted == 1)	// If the connection is granted
{
	// SUCCESS
	$_SESSION['connected'] = true;
	$_SESSION['username'] = $username;
	//$_SESSION['role'] = $account['role'];
	$_SESSION['user_id'] = getUsernameID($username);
	$_SESSION['user_ip'] = $_SERVER['REMOTE_ADDR'];
	
	$registrationDate = new DateTime($account['creation_date'], new DateTimeZone('Europe/Paris'));
	
	// Get gaming of the player
	//$gaming = getGaming($account['current_game'], $account['id']);
    
	$serverDatas = array(
		"Connection granted.",
		$SID,
		$account['role'],
		$account['mail'],
		$registrationDate->format('Y-m-d H:i:s'),
		//$previousConnectionDate->format('Y-m-d H:i:s'),
		//$gaming['minutes_played'],
       
        
	);
	
	echo json_encode($serverDatas);
}
*/
//end_script("Credenciales incorrectas.");

?>