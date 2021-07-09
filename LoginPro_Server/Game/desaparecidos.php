<?php

include_once 'funciones.php';
include_once 'Includes/Functions.php';
include_once 'Includes/ServerSettings.php'; 			// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database

$tokenReceived = $_GET["token"];
// $tokenReceived = $_POST["token"];
// $tokenReceived = $_SERVER['HTTP_AUTHORIZATION'];

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
    $query = "SELECT
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".id,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".a1,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".a2,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".userID,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".creation_date,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".nombreCompleto,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".sexo,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".fechaNacimiento,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".estadoNacimiento,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".municipioNacimiento,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".localidadNacimiento,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".estadoResidencia,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".municipioResidencia,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".localidadResidencia,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".ocupacion,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".estadoCivil,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".hijos,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".estadoDesaparicion,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".municipioDesaparicion,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".localidadDesaparicion,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".fechaDesaparicion,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".fechaDenuncia,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".lugarDenuncia,
                ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".foto,

                ".$_SESSION['AccountTable'].".id AS USER_id,
                ".$_SESSION['AccountTable'].".nombre_completo AS USER_nombre_completo,
                ".$_SESSION['AccountTable'].".avatar AS USER_avatar,
                ".$_SESSION['AccountTable'].".idGrupo,

                ".$_SESSION['GruposTable'].".id AS grupoId,
                ".$_SESSION['GruposTable'].".name AS grupoName,
                ".$_SESSION['GruposTable'].".avatar AS grupoAvatar,
                ".$_SESSION['GruposTable'].".color AS grupoColor

              FROM ".$_SESSION['CuestionarioFamiliarDesaparecidoTable']."
        			LEFT JOIN ".$_SESSION['AccountTable']." ON ".$_SESSION['AccountTable'].".id = ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".userID
        			LEFT JOIN ".$_SESSION['GruposTable']." ON ".$_SESSION['GruposTable'].".id = ".$_SESSION['AccountTable'].".idGrupo
              WHERE ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".id = :id";
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
    	$where .= " AND ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".nombreCompleto like '%".$query."%'";
    }

    if (isset($_GET['user'])) $userId = $_GET['user'];
    if ($userId) {
    	$where .= " AND ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".userID='".$userId."'";
    }

    if (isset($_GET['group'])) $groupId = $_GET['group'];
    if ($groupId) {
    	$where .= " AND ".$_SESSION['GruposTable'].".id='".$groupId."'";
    }

    if (isset($_GET['limit'])) $limit = $_GET['limit'];
    else $limit = 25;

    if (isset($_GET['skip'])) $skip = $_GET['skip'];
    else $skip = 0;
    if (!($skip >= 0)) $skip = 0;

    $limiter = " LIMIT " . $skip . "," . $limit;

    $order = " ORDER BY ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".nombreCompleto ASC";

    $joins = " LEFT JOIN ".$_SESSION['AccountTable']." ON ".$_SESSION['AccountTable'].".id = ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".userID
      LEFT JOIN ".$_SESSION['GruposTable']." ON ".$_SESSION['GruposTable'].".id = ".$_SESSION['AccountTable'].".idGrupo";

    // count total
    $total = 0;
    $queryCount = "SELECT COUNT(".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".id) as count
              FROM ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].$joins.$where;
    $parametersCount = array();
    $stmtCount = ExecuteQuery($queryCount, $parametersCount);
    $itemsCount = $stmtCount->fetch(PDO::FETCH_ASSOC);
    if ($itemsCount && $itemsCount['count']) $total = $itemsCount['count'];


    // list
    $query = "SELECT
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".id,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".userID,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".nombreCompleto,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".sexo,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".fechaDesaparicion,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".fechaDenuncia,
          ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].".foto,

          ".$_SESSION['AccountTable'].".id AS USER_id,
          ".$_SESSION['AccountTable'].".nombre_completo AS USER_nombre_completo,
          ".$_SESSION['AccountTable'].".avatar AS USER_avatar,
          ".$_SESSION['AccountTable'].".idGrupo AS USER_idGrupo,

          ".$_SESSION['GruposTable'].".id AS grupoId,
          ".$_SESSION['GruposTable'].".name AS grupoName,
          ".$_SESSION['GruposTable'].".avatar AS grupoAvatar,
          ".$_SESSION['GruposTable'].".color AS grupoColor

        FROM ".$_SESSION['CuestionarioFamiliarDesaparecidoTable'].
        $joins.$where.$order.$limiter;



    $stmt = ExecuteQuery($query);

    $data = array();
    while($item = $stmt->fetch(PDO::FETCH_ASSOC)){
      $data[]=$item;
    }

    $res = array(
      'data' => $data,
      'skip' => $skip,
      'limit' => $limit,
      'total' => $total,
      'sql' => $sql
    );
    $myJSON=json_encode($res);

    header("HTTP/1.1 200 OK");
    echo $myJSON;

  }

}

close();
?>
