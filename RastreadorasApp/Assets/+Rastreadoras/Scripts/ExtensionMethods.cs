using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
 using System.Collections.Generic;

public static class ExtensionMethods
{

	public enum Rotation { Left, Right, HalfCircle }
public static void Rotate(this Texture2D texture, Rotation rotation)
     {
         Color32[] originalPixels = texture.GetPixels32();
         IEnumerable<Color32> rotatedPixels;
 
         if (rotation == Rotation.HalfCircle){
             rotatedPixels = originalPixels.Reverse();
         }else
         {    
             var firstRowPixelIndeces = Enumerable.Range(0, texture.height).Select(i => i * texture.width).Reverse().ToArray();
             rotatedPixels = Enumerable.Repeat(firstRowPixelIndeces, texture.width).SelectMany(
                 (frpi, rowIndex) => frpi.Select(i => originalPixels[i + rowIndex])
             );
 
             if (rotation == Rotation.Right)
                 rotatedPixels = rotatedPixels.Reverse();
         }
 
         texture.SetPixels32( rotatedPixels.ToArray() );
         texture.Apply();
     }

     public static Transform Clear(this Transform transform)
     {
         foreach (Transform child in transform) {
             GameObject.Destroy(child.gameObject);
         }
         return transform;
     }

	 public static Transform HideChildren(this Transform transform)
     {
         foreach (Transform child in transform) {
             child.gameObject.SetActive(false);
         }
         return transform;
     }

	public static string ToTitleCase(this string stringToConvert)
	{
		string[] palabras = stringToConvert.Split(" "[0]);
		string output="";
		for (int i  = 0; i < palabras.Length; i++){
			string firstChar = palabras[i][0].ToString();
			string palabra = firstChar.ToUpper() + palabras[i].Substring(1);
			output += (palabra) + " ";
		}
		return output ;

	}
 
 public static float Remap (this float value, float from1, float to1, float from2, float to2) {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}


		public const string MatchEmailPattern =
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,9})$";


		public static bool IsEmail(string email)
		{
			if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
			else return false;
		}
	
}