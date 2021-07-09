<?php
//header("Location: http://thisisnotanumber.org/rastreadoras/LoginPro_Server/Game/media/video_testimonio_1529093034.mp4");
//die();
include 'videostream.php';

$file= $_GET["file"];

$ext = strtolower(pathinfo($file, PATHINFO_EXTENSION));

switch ($ext)
{
    case "mp4":
    $stream1 = new VideoStream($file);
    $stream1->start();
    exit;
    break;
    case "mov":
$stream1 = new VideoStream($file);
$stream1->start();
break;
case "png":
$fp = fopen($file, 'r');
header("Content-Type: image/png");
header("Content-Length: ".filesize($file));
fpassthru($fp);
fclose($fp);
break;
case "jpg":
$fp = fopen($file, 'r');
header("Content-Type: image/jpg");
header("Content-Length: ".filesize($file));
fpassthru($fp);
fclose($fp);
break;
case "jpeg":
$fp = fopen($file, 'r');
header("Content-Type: image/jpg");
header("Content-Length: ".filesize($file));
fpassthru($fp);
fclose($fp);
break;
}

//readfile($file);

exit;

?>