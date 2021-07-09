<?php 
if ($_POST)
{
    if ( isset ($_POST['file']) )
    {
        if(!isset($_FILES) || isset($HTTP_POST_FILES))
        {
            $_FILES = $HTTP_POST_FILES;
        }          
        if ($_FILES['file']['error'] === UPLOAD_ERR_OK)
        {
            if ($_FILES['file']['name'] !== "")
            {
                //if ($_FILES['file']['type'] === 'text/xml')
                {
                    $nombre =$_FILES['file']['name'];
                    $uploadfile =  '/nfs/c07/h02/mnt/112763/domains/thisisnotanumber.org/html/rastreadoras/LoginPro_Server/Game/media/' . $nombre;
                    
                    move_uploaded_file($_FILES['file']['tmp_name'], $uploadfile);  
                    
                    if($_POST['width']!=""){
                        create_cropped_thumbnail('media/'.$nombre, (int)$_POST['width'], (int)$_POST['height'], 'media/'.$nombre); 
                    }
                    
                    echo $nombre;        
                }
                        
            }
                
        }
    }
   
}

function createRotIm($image_path){
    $image= imagecreatefromstring(file_get_contents($image_path));
    $exif = exif_read_data($image_path);
    if(!empty($exif['Orientation'])) {
        switch($exif['Orientation']) {
            case 8:
                $image = imagerotate($image,90,0);
                break;
            case 3:
                $image = imagerotate($image,180,0);
                break;
            case 6:
                $image = imagerotate($image,-90,0);
                break;
        }
    }
    return $image;
}

function create_cropped_thumbnail($image_path, $thumb_width, $thumb_height, $imageout_path) {

    if (!(is_integer($thumb_width) && $thumb_width > 0) && !($thumb_width === "*")) {
        echo "The width is invalid";
        exit(1);
    }

    if (!(is_integer($thumb_height) && $thumb_height > 0) && !($thumb_height === "*")) {
        echo "The height is invalid";
        exit(1);
    }

    $extension = pathinfo($image_path, PATHINFO_EXTENSION);
    switch ($extension) {
        case "jpg":
		case "jpeg":
            $source_image= createRotIm($image_path);
            break;
		case "gif":
            $source_image = imagecreatefromgif($image_path);
            break;
		case "png":
            $source_image = imagecreatefrompng($image_path);
            break;
        default:
            exit(1);
            break;
    }

    $source_width = imageSX($source_image);
    $source_height = imageSY($source_image);

    if (($source_width / $source_height) == ($thumb_width / $thumb_height)) {
        $source_x = 0;
        $source_y = 0;
    }

    if (($source_width / $source_height) > ($thumb_width / $thumb_height)) {
        $source_y = 0;
        $temp_width = $source_height * $thumb_width / $thumb_height;
        $source_x = ($source_width - $temp_width) / 2;
        $source_width = $temp_width;
    }

    if (($source_width / $source_height) < ($thumb_width / $thumb_height)) {
        $source_x = 0;
        $temp_height = $source_width * $thumb_height / $thumb_width;
        $source_y = ($source_height - $temp_height) / 2;
        $source_height = $temp_height;
    }

    $target_image = ImageCreateTrueColor($thumb_width, $thumb_height);

    imagecopyresampled($target_image, $source_image, 0, 0, $source_x, $source_y, $thumb_width, $thumb_height, $source_width, $source_height);

    switch ($extension) {
        case "jpg":
        case "jpeg":
            imagejpeg($target_image, $imageout_path);
            break;
        case "gif":
            imagegif($target_image, $imageout_path);
            break;
        case "png":
            imagepng($target_image, $imageout_path);
            break;
        default:
            exit(1);
            break;
    }

    imagedestroy($target_image);
    imagedestroy($source_image);
}
?>
 