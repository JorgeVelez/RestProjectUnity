<?php
$debug = true;
$ServerScriptCalled = 1;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Handle communication with the game and execute commands depending to action specified
//	This script handle every single communication the game wants to make
//	This is the only accessible link from the outside (except ValidateAccountByURL.php and ValidateIPByURL.php -> accessed by link)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


include_once 'Includes/Functions.php'; 	
include_once 'Includes/miniCrypto.php';
// Include protection functions and tools functions
include_once 'Includes/Session.php'; 					// Begin client session (or return to index if not session ID received)
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database
if(!isset($NotInGame) || !$NotInGame==1)


// ACTION
if(isset($_POST['Action'])) { $ACTION = $_POST['Action']; }
else if(isset($_GET['Action'])) { $ACTION = $_GET['Action']; }
else { end_script('Server: No action has been received.'); }

//----------------------------------: AUTHENTIFICATION :--------------------------------------------//
// Check session information automatically for you													//
//include_once './Includes/Framework/CheckAuthentification.php';										//
//--------------------------------------------------------------------------------------------------//


if($ACTION == "GetData") { include_once 'Includes/Actions/GetData.php'; }
else if($ACTION == "GetBusqueda") { include_once 'Includes/Api/GetBusqueda.php'; }
/*************************** ACTIONS ZONE END ******************************/



// If the action hasn't been declared here we will inform you
//else if($debug) { echo('Warning : action ->'.$ACTION.'<- not set in Server.php script'); }
// Close connection to database properly
end_script('');

?>