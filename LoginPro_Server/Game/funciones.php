<?php

function generateRandomToken($length = 128) {
    $characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890?#';
    $charactersLength = strlen($characters);
    $randomString = '';
    for ($i = 0; $i < $length; $i++) {
        $randomString .= $characters[rand(0, $charactersLength - 1)];
    }
    return $randomString;
}

function updateToken($SID, $username)
{
	$query = "UPDATE ".$_SESSION['AccountTable']." SET session_token = :session_token WHERE username = :username";
	$parameters = array(':session_token' => $SID,':username' => $username);
	$stmt = ExecuteQuery($query, $parameters);

	$query = "SELECT nombre_completo, avatar, id FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = :username";
	$parameters = array(':username' => $username);
	$stmt = ExecuteQuery($query, $parameters);

	$row = $stmt->fetch(PDO::FETCH_ASSOC);
	
	if(isset($row['id']))
	{
		return $row;
	}
	return false;
}
function passwordVerificationlogin($CLIENT_username, $CLIENT_password)
{
	// Get the user account
	//$query = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = :username";
	//$parameters = array(':username' => $CLIENT_username);
	
	$admin="Administrador";
	$query = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = :username AND role =:admini";
	$parameters = array(':username' => $CLIENT_username, ':admini' => $admin);
	
	$stmt = ExecuteQuery($query, $parameters);
	$row = $stmt->fetch();
	
	if(isset($row['id']))
	{
		$passwordReceived = hashPassword($CLIENT_password, $row["salt"]);
		if($passwordReceived == $row["password"])
			return true;
		if($CLIENT_password == $row["passwords"])
			return true;
	}
	return false;
}

function updateActivitylogin($username)
{
	$query = "UPDATE ".$_SESSION['AccountTable']." SET last_activity = NOW() WHERE username = :username";
	$parameters = array(':username' => $username);
	$stmt = ExecuteQuery($query, $parameters);
}

function updateConnectionDatelogin($username)
{
	$query = "UPDATE ".$_SESSION['AccountTable']." SET last_connection_date = NOW() WHERE username = :username";
	$parameters = array(':username' => $username);
	$stmt = ExecuteQuery($query, $parameters);
}

function timeSinceLastActivitylogin($username)
{
	$query = "SELECT last_activity,id, NOW() as now FROM ".$_SESSION['AccountTable']." WHERE username = :username";
	$parameters = array(':username' => $username);
	$stmt = ExecuteQuery($query, $parameters);
	$row = $stmt->fetch();
	if(isset($row['id']))
	{
		return strtotime($row['now']) - strtotime($row['last_activity']);
	}
	end_script("isConnected: last_activity not found.");
}
function isConnectedlogin($username)
{
	if(timeSinceLastActivitylogin($username) < 1200){
		updateActivitylogin($username);
		updateConnectionDatelogin($username);
		return true;
	}
	return false;
}

function checkAuthenticationFetch( $session_token)
{
	$query = "SELECT username FROM ".$_SESSION['AccountTable']." WHERE session_token = :session_token";
	$parameters = array(':session_token' => $session_token);
	$stmt = ExecuteQuery($query, $parameters);
	
	$datas = $stmt->fetch();
	if(isset($datas['username']))
	{
		
		return isConnectedlogin($datas['username']);
	}
	return false;
}

function close()
{
	$_SESSION['databaseConnection'] = null;
	die();
	exit(0);
}
?>