using System.Collections.Generic;
using UnityEngine;

namespace RGF.Game.BMP
{
	public class BMPImage
	{
		public uint aMask = 0x00000000;
		public uint bMask = 0x000000FF;
		public uint gMask = 0x0000FF00;
		public BMPFileHeader header;
		public Color32[] imageData;
		public BitmapInfoHeader info;
		public List<Color32> palette;
		public uint rMask = 0x00FF0000;

		public Texture2D ToTexture2D()
		{
			Texture2D tex = new Texture2D(info.absWidth, info.absHeight);
			tex.SetPixels32(imageData);
			tex.Apply();
			return tex;
		}
	}
}