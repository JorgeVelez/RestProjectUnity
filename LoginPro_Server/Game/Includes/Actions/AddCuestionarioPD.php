<?php


$a1 = $datas[0];
$a2 = $datas[1];
$a3 = $datas[2];

$nombreCompleto = $datas[3];
$sexo = $datas[4];
$date = date_create_from_format('d/m/Y', $datas[5]);
$fechaNacimiento = $date->format('Y-m-d H-i-s');

$estadoNacimiento = $datas[6];
$municipioNacimiento = $datas[7];
$localidadNacimiento = $datas[8];

$estadoResidencia = $datas[9];
$municipioResidencia = $datas[10];
$localidadResidencia = $datas[11];

$ocupacion = $datas[12];
$estadoCivil = $datas[13];

$estadoDesaparicionPD=$datas[14] ;
$municipioDesaparicionPD =$datas[15];
$localidadDesaparicionPD=$datas[16];

$fechaDesapTx = date_create_from_format('d/m/Y', $datas[17]);
$fechaDesap = $fechaDesapTx->format('Y-m-d H-i-s');

$fechaDenunciaTx = date_create_from_format('d/m/Y', $datas[18]);
$fechaDenuncia = $fechaDenunciaTx->format('Y-m-d H-i-s');

$lugarDenunciaPD= $datas[19];

$query = "INSERT INTO ".$_SESSION['CuestionarioPDTable']." (userID,a1,a2,foto,nombreCompleto,sexo, fechaNacimiento,estadoNacimiento,municipioNacimiento, localidadNacimiento, estadoResidencia, municipioResidencia, localidadResidencia, ocupacion, estadoCivil, 
estadoDesaparicion,
municipioDesaparicion, 
localidadDesaparicion,
fechaDesaparicion,
fechaDenuncia,
lugarDenuncia,
creation_date) VALUES (:userID,:a1,:a2,:a3,:nombreCompleto,:sexo, :fechaNacimiento,:estadoNacimiento,:municipioNacimiento, :localidadNacimiento, :estadoResidencia, :municipioResidencia, :localidadResidencia, :ocupacion, :estadoCivil, 
:estadoDesaparicionPD,
:municipioDesaparicionPD, 
:localidadDesaparicionPD,
:fechaDesapPD ,
:fechaDenunciaPD ,
:lugarDenunciaPD
,NOW())";
$parameters = array(':userID' => $account['id'],':a1' => $a1,':a2' => $a2,':a3' => $a3,':nombreCompleto' => $nombreCompleto,':sexo' => $sexo,':fechaNacimiento' => $fechaNacimiento,':estadoNacimiento' => $estadoNacimiento,':municipioNacimiento' => $municipioNacimiento,':localidadNacimiento' => $localidadNacimiento,':estadoResidencia' => $estadoResidencia,':municipioResidencia' => $municipioResidencia,':localidadResidencia' => $localidadResidencia,':ocupacion' => $ocupacion,':estadoCivil' => $estadoCivil,
':estadoDesaparicionPD'=> $estadoDesaparicionPD, 
':municipioDesaparicionPD'=> $municipioDesaparicionPD, 
':localidadDesaparicionPD'=> $localidadDesaparicionPD,
':fechaDesapPD' => $fechaDesap,
':fechaDenunciaPD' => $fechaDenuncia,
':lugarDenunciaPD'=> $lugarDenunciaPD);
$stmt = ExecuteQuery($query, $parameters);

$query = "UPDATE ".$_SESSION['AccountTable']." SET cuestionario = :validado WHERE id = :id";
	$parameters = array(':id' => $account['id'],':validado' => 1 );
	$stmt = ExecuteQuery($query, $parameters);

$total['titulo']     = "Cuestionario Creado";
$total['response']      = "true";
        
sendAndFinish(json_encode($total));

?>