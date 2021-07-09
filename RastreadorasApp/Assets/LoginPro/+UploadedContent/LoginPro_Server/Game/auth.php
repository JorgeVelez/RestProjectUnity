<?php

// error_reporting(E_ALL);
// ini_set("display_errors", 1);

header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

/*
PARAMS

email
password

*/


if (isset($_POST['email'])) {
  $email = $_POST['email']; //trim(mysql_real_escape_string(htmlspecialchars($_POST['email'])));
} else {
  header("HTTP/1.1 400 Bad Request");
  echo json_encode(array( 'error' => 'Email required' ));
  exit;
}

if (isset($_POST['password'])) {
  $password = $_POST['password']; //trim(mysql_real_escape_string(htmlspecialchars($_POST['password'])));
} else {
  header("HTTP/1.1 400 Bad Request");
  echo json_encode(array( 'error' => 'Password required' ));
  exit;
}

// $email = 'rr@example.com';
// $password = '12345';

function randomString( $length ) {
    $chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    return substr(str_shuffle($chars),0,$length);
}

if ($email == 'rr@example.com' && $password == '12345') {

  $userSession = randomString(32);
  $userId = 3;
  $userName = "Robert Polson";

  if (session_status() == PHP_SESSION_NONE) {
      session_start();
  }

  $_SESSION["user_id"] = $userId; //encrypt($userId, $secretKey);
  $_SESSION["user_session"] = $userSession; //encrypt($userSession, $secretKey);

  header("HTTP/1.1 200 OK");
  echo json_encode(array(
    'token' => $userSession,
    'user' => array(
      'id' => $userId,
      'name' => $userName
    )
  ));
} else {
  header("HTTP/1.1 400 Bad Request");
  echo json_encode(array( 'error' => 'Bad login' ));
  exit;
}

?>
