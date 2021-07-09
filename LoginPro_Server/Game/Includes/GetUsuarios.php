<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }


// LET'S BEGIN :
// Notice that $_SESSION['GamingTable'] is set in the script 'ServerSettings.php', if you want to add other tables: add them in 'Server.php' in the ******* TABLES ZONE ********
$query = "SELECT * FROM ".$_SESSION['AccountTable'];
$stmt = ExecuteQuery($query);

$datasToSend = array();
$noFriendFound = true;
foreach($stmt as $row)
{
    // If it's the first achievement of the list : message = reports found
	if($noFriendFound)
	{
		$noFriendFound = false;
		$datasToSend[] = "Friend list received.";
	}
	
	$datasToSend[] = $row["nombre_completo"];
	$datasToSend[] = $row["id"];
	$datasToSend[] = $row["avatar"];
	
	// IMPORTANT ! In order to send just a message, use the function sendAndFinish("The message you want to send") (See the PHP script just besides called "SendData.php")
	// IMPORTANT ! If you want to send MULTIPLE DATAS from the server to the game use sendArrayAndFinish, like that :
	
}
// If no achievement exists : message = no achievement
if($noFriendFound)
{
	$datasToSend[] = "You have no users currently.";
}

sendArrayAndFinish($datasToSend);

?>
