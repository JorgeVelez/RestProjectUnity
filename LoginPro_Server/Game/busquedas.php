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

  if (isset($_GET['id'])) {

    // retrive params
    $id = $_GET['id'];

    // call db
    $query = "SELECT ".$_SESSION['BusquedasTable'].".id,
                      ".$_SESSION['BusquedasTable'].".nombre,
                      ".$_SESSION['BusquedasTable'].".grupo,
                      ".$_SESSION['BusquedasTable'].".avatar,
                      ".$_SESSION['BusquedasTable'].".lugar,
                      ".$_SESSION['BusquedasTable'].".creation_date,
                      ".$_SESSION['BusquedasTable'].".activa,
                      ".$_SESSION['GruposTable'].".id AS grupoId,
                      ".$_SESSION['GruposTable'].".name AS grupoName,
                      ".$_SESSION['GruposTable'].".avatar AS grupoAvatar,
                      ".$_SESSION['GruposTable'].".color AS grupoColor
              FROM ".$_SESSION['BusquedasTable']."
        			LEFT JOIN ".$_SESSION['GruposTable']." ON ".$_SESSION['GruposTable'].".id = ".$_SESSION['BusquedasTable'].".grupo
              WHERE ".$_SESSION['BusquedasTable'].".id = :id";
    $parameters = array(':id' => $id);
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
       echo json_encode(array( 'error' => 'Invalid ID' ));
    }

  } else {



    // find where parameters

    $where = " WHERE 1=1";

    if (isset($_GET['query'])) $query = $_GET['query'];
    if ($query) {
    	$where .= " AND ".$_SESSION['BusquedasTable'].".nombre like '%".$query."%'";
    }

    if (isset($_GET['group'])) $groupId = $_GET['group'];
    if ($groupId) {
    	$where .= " AND ".$_SESSION['BusquedasTable'].".grupo='".$groupId."'";
    }


    // count total
    $total = 0;
    $queryCount = "SELECT COUNT(id) as count
              FROM ".$_SESSION['BusquedasTable'].$where;
    $parametersCount = array();
    $stmtCount = ExecuteQuery($queryCount, $parametersCount);
    $itemsCount = $stmtCount->fetch(PDO::FETCH_ASSOC);
    if ($itemsCount && $itemsCount['count']) $total = $itemsCount['count'];





    // Find list

    if (isset($_GET['limit'])) $limit = $_GET['limit'];
    else $limit = 25;

    if (isset($_GET['skip'])) $skip = $_GET['skip'];
    else $skip = 0;
    if (!($skip >= 0)) $skip = 0;

    $limiter = " LIMIT " . $skip . "," . $limit;

    $order = " ORDER BY ".$_SESSION['BusquedasTable'].".creation_date DESC";

    $query = "SELECT
                    ".$_SESSION['BusquedasTable'].".id,
                    ".$_SESSION['BusquedasTable'].".nombre,
                    ".$_SESSION['BusquedasTable'].".grupo,
                    ".$_SESSION['BusquedasTable'].".avatar,
                    ".$_SESSION['BusquedasTable'].".lugar,
                    ".$_SESSION['BusquedasTable'].".creation_date,
                    ".$_SESSION['BusquedasTable'].".activa,
                    ".$_SESSION['GruposTable'].".id AS grupoId,
                    ".$_SESSION['GruposTable'].".name AS grupoName,
                    ".$_SESSION['GruposTable'].".avatar AS grupoAvatar,
                    ".$_SESSION['GruposTable'].".color AS grupoColor
              FROM ".$_SESSION['BusquedasTable']."
        			LEFT JOIN ".$_SESSION['GruposTable']." ON ".$_SESSION['GruposTable'].".id = ".$_SESSION['BusquedasTable'].".grupo"
              .$where.$order.$limiter;
    $stmt = ExecuteQuery($query);

    $data = array();
    while($item = $stmt->fetch(PDO::FETCH_ASSOC)){
      $data[]=$item;
    }

    $res = array(
      'data' => $data,
      'skip' => $skip,
      'limit' => $limit,
      'total' => $total
    );
    $myJSON=json_encode($res);

    header("HTTP/1.1 200 OK");
    echo $myJSON;
  }
}

close();
?>
