<?php
$debug = true;
$ServerScriptCalled = 1;

include_once 'Includes/Functions.php';
include_once 'Includes/miniCrypto.php';
// Include protection functions and tools functions
include_once 'Includes/Session.php'; 					// Begin client session (or return to index if not session ID received)
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database
if(!isset($NotInGame) || !$NotInGame==1)

// would be better retrive token in header authorization key
// require rule in .htaccess :
// RewriteEngine On
// RewriteRule .* - [e=HTTP_AUTHORIZATION:%{HTTP:Authorization}]
// then we can access to token as authorization 
    
$tokenKey=(int)decrypt($_SERVER['HTTP_AUTHORIZATION']);
//$tokenKey=(int)decrypt($_GET['tokenKey']);
//if(isset($_POST['tokenKey'])) { $tokenKey=(int)decrypt($_POST['tokenKey']); }
//$tokenKey=decrypt("j6R7bW9ypiJPAQ/M/9CRrv1sUaYK/vPPWlZhN/wFpQudKaaW/DIRUU56V/aiXD1D2y2uWkWVbYiZuUhkcouaVg==");
$milliseconds = round(microtime(true) * 1000);
$transcurrido=round($milliseconds / 1000)-round($tokenKey / 1000);


header("Access-Control-Allow-Headers: *");
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With");
header('Content-Type: application/json');


$transcurrido = 200; // for pruebas only !!!!

//if($transcurrido > 300) { end_script('Session invalida.'); }
if ($transcurrido > 300) {
  // session invalida, return error
  header("HTTP/1.1 401 Unauthorized");
  echo json_encode(array( 'error' => 'Session invalida' ));
} else {

  // session is ok, we can find and send data

  if (isset($_GET['id'])) {

    // ITEM WAITED

    // retrive params
    $id = $_GET['id'];

    // call db
    $query = "SELECT id, nombre, grupo, avatar as avatar_busqueda, lugar, creation_date, activa FROM ".$_SESSION['BusquedasTable']." WHERE id = :id";
    $parameters = array(':id' => $id);
    $stmt = ExecuteQuery($query, $parameters);

    $items = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($items[0]) {
      // item founded, send it
      header("HTTP/1.1 200 OK");
      $myJSON=json_encode($items[0]);
      echo $_GET['callback']."(".$myJSON.");";
    } else {
       // item not found, respond with 404
       header("HTTP/1.1 404 Not Found");
       echo json_encode(array( 'error' => 'Invalid ID' ));
    }

  } else {

    // LIST WAITED

    // retrive params
    if (isset($_GET['query'])) $query = $_GET['query'];
    if (isset($_GET['group'])) $groupId = $_GET['group'];
    if (isset($_GET['user'])) $userId = $_GET['user'];

    // cur page
    if (isset($_GET['page'])) $page = $_GET['page'];
    else $page = 0;

    // how many items by page ?
    if (isset($_GET['items'])) $itemByPage = $_GET['items'];
    else $itemByPage = 25;

    $where = "WHERE 1=1";

		if ($query) {
			$where .= " AND ".$_SESSION['BusquedasTable'].".nombre like '%".$query."%'";
		}

		if ($groupId) {
			$where .= " AND ".$_SESSION['BusquedasTable'].".grupo='".$groupId."'";
		}

		if ($userId) {
      // how can we do here?
			// $where .= " AND ".$_SESSION['BusquedasTable'].".grupo='".$userId."'";
		}

    // count total
    $total = 0;
    $queryCount = "SELECT COUNT(id) as count
              FROM ".$_SESSION['BusquedasTable']."
              ".$where;
    $parametersCount = array();
    $stmtCount = ExecuteQuery($queryCount, $parametersCount);
    $itemsCount = $stmtCount->fetch(PDO::FETCH_ASSOC);
    if ($itemsCount && $itemsCount[0]) $total = $itemsCount[0]['count'];

    // get total page
    $countPageTotal = $total / $itemByPage;
    // if current page more than total page, cur = total
    if ($page > $countPageTotal) $page = $countPageTotal;

    // then find init & max for the limit
    $ini = $page * $itemByPage;
    $max = $itemByPage;
    $limit = " LIMIT " . $ini . "," . $max;

    $order = " ORDER BY ".$_SESSION['BusquedasTable'].".creation_date DESC";

    // now call db
    $query = "SELECT id, nombre, grupo, avatar as avatar_busqueda, lugar, creation_date, activa
              FROM ".$_SESSION['BusquedasTable']
              .$where
              .$order
              .$limit;
    $parameters = array();
    $stmt = ExecuteQuery($query, $parameters);

    $items = $stmt->fetch(PDO::FETCH_ASSOC);

    $res = array(
      'list' => $items,
      'pages' => $countPageTotal
    );

    // item founded, send it
    header("HTTP/1.1 200 OK");
    $myJSON=json_encode($doc);
    echo $_GET['callback']."(".$myJSON.");";

  }

}



// close
$_SESSION['databaseConnection'] = null;
die();
exit(0);





// If the action hasn't been declared here we will inform you
//else if($debug) { echo('Warning : action ->'.$ACTION.'<- not set in Server.php script'); }
// Close connection to database properly
end_script('');

?>