<?php
// Copyright (C) 2010  Juan Valencia <jvalenciae@jveweb.net>
// All rights reserved.
//
// Redistribution and use of this script, with or without modification, is
// permitted provided that the following conditions are met:
//
// 1. Redistributions of this script must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
// EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// create_cropped_thumbnail()
// $image_path - The relative path to an existent image from the script
// $thumb_width - The width of the thumbnail
// $thumb_height - the height of the thumbnail
// $prefix - a prefix to be added to the beginning of the name of the original
//    file in order to obtain the name that will be given to the thumbnail

function create_cropped_thumbnail($image_path, $thumb_width, $thumb_height, $prefix) {

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
            $source_image = imagecreatefromjpeg($image_path);
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
            imagejpeg($target_image, $prefix . "_" . $image_path);
            break;
        case "gif":
            imagegif($target_image, $prefix . "_" . $image_path);
            break;
        case "png":
            imagepng($target_image, $prefix . "_" . $image_path);
            break;
        default:
            exit(1);
            break;
    }

    imagedestroy($target_image);
    imagedestroy($source_image);
}
?>
