#region License and Information

/*****
*
* BMPLoader.cs
* 
* This is a simple implementation of a BMP file loader for Unity3D.
* Formats it should support are:
*  - 1 bit monochrome indexed
*  - 2-8 bit indexed
*  - 16 / 24 / 32 bit color (including "BI_BITFIELDS")
*  - RLE-4 and RLE-8 support has been added.
* 
* Unless the type is "BI_ALPHABITFIELDS" the loader does not interpret alpha
* values by default, however you can set the "ReadPaletteAlpha" setting to
* true to interpret the 4th (usually "00") value as alpha for indexed images.
* You can also set "ForceAlphaReadWhenPossible" to true so it will interpret
* the "left over" bits as alpha if there are any. It will also force to read
* alpha from a palette if it's an indexed image, just like "ReadPaletteAlpha".
* 
* It's not tested well to the bone, so there might be some errors somewhere.
* However I tested it with 4 different images created with MS Paint
* (1bit, 4bit, 8bit, 24bit) as those are the only formats supported.
* 
* 2017.02.05 - first version 
* 2017.03.06 - Added RLE4 / RLE8 support
* 
* Copyright (c) 2017 Markus Göbel (Bunny83)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to
* deal in the Software without restriction, including without limitation the
* rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
* sell copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
* IN THE SOFTWARE.
* 
*****/

#endregion License and Information

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RGF.Game.BMP
{
	public class BMPLoader
	{
		private const ushort MAGIC = 0x4D42; // "BM" little endian

		private string fileName;
		public bool forceAlphaReadWhenPossible = false;

		public bool readPaletteAlpha = false;

		public BMPImage LoadBMP(string aFileName)
		{
			fileName = aFileName;

			using FileStream file = File.OpenRead(aFileName);
			return LoadBMP_Internal(file);
		}

		public BMPImage LoadBMP(byte[] aData, string path = default)
		{
			fileName = path;

			using MemoryStream stream = new MemoryStream(aData);
			return LoadBMP_Internal(stream);
		}

		private BMPImage LoadBMP_Internal(Stream aData)
		{
			using BinaryReader reader = new BinaryReader(aData);
			return LoadBMP_Internal(reader);
		}

		private BMPImage LoadBMP_Internal(BinaryReader aReader)
		{
			BMPImage bmp = new BMPImage();
			if (!ReadFileHeader(aReader, ref bmp.header))
			{
				Debug.LogWarning($"Not a BMP file : {fileName}");
				return null;
			}

			if (!ReadInfoHeader(aReader, ref bmp.info))
			{
				Debug.LogError("Unsupported header format");
				return null;
			}

			if (bmp.info.compressionMethod != BMPComressionMode.BI_RGB
			    && bmp.info.compressionMethod != BMPComressionMode.BI_BITFIELDS
			    && bmp.info.compressionMethod != BMPComressionMode.BI_ALPHABITFIELDS
			    && bmp.info.compressionMethod != BMPComressionMode.BI_RLE4
			    && bmp.info.compressionMethod != BMPComressionMode.BI_RLE8
			)
			{
				Debug.LogError("Unsupported image format: " + bmp.info.compressionMethod);
				return null;
			}

			long offset = 14 + bmp.info.size;
			aReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			if (bmp.info.nBitsPerPixel < 24)
			{
				bmp.rMask = 0x00007C00;
				bmp.gMask = 0x000003E0;
				bmp.bMask = 0x0000001F;
			}

			if (bmp.info.compressionMethod == BMPComressionMode.BI_BITFIELDS ||
			    bmp.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
			{
				bmp.rMask = aReader.ReadUInt32();
				bmp.gMask = aReader.ReadUInt32();
				bmp.bMask = aReader.ReadUInt32();
			}

			if (forceAlphaReadWhenPossible)
			{
				bmp.aMask = GetMask(bmp.info.nBitsPerPixel) ^ (bmp.rMask | bmp.gMask | bmp.bMask);
			}

			if (bmp.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
			{
				bmp.aMask = aReader.ReadUInt32();
			}

			if (bmp.info.nPaletteColors > 0 || bmp.info.nBitsPerPixel <= 8)
			{
				bmp.palette = ReadPalette(aReader, bmp, readPaletteAlpha || forceAlphaReadWhenPossible);
			}


			aReader.BaseStream.Seek(bmp.header.offset, SeekOrigin.Begin);
			bool uncompressed = bmp.info.compressionMethod == BMPComressionMode.BI_RGB ||
			                    bmp.info.compressionMethod == BMPComressionMode.BI_BITFIELDS ||
			                    bmp.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS;
			if (bmp.info.nBitsPerPixel == 32 && uncompressed)
			{
				Read32BitImage(aReader, bmp);
			}
			else if (bmp.info.nBitsPerPixel == 24 && uncompressed)
			{
				Read24BitImage(aReader, bmp);
			}
			else if (bmp.info.nBitsPerPixel == 16 && uncompressed)
			{
				Read16BitImage(aReader, bmp);
			}
			else if (bmp.info.compressionMethod == BMPComressionMode.BI_RLE4 && bmp.info.nBitsPerPixel == 4 &&
			         bmp.palette != null)
			{
				ReadIndexedImageRLE4(aReader, bmp);
			}
			else if (bmp.info.compressionMethod == BMPComressionMode.BI_RLE8 && bmp.info.nBitsPerPixel == 8 &&
			         bmp.palette != null)
			{
				ReadIndexedImageRLE8(aReader, bmp);
			}
			else if (uncompressed && bmp.info.nBitsPerPixel <= 8 && bmp.palette != null)
			{
				ReadIndexedImage(aReader, bmp);
			}
			else
			{
				Debug.LogError("Unsupported file format: " + bmp.info.compressionMethod + " BPP: " +
				               bmp.info.nBitsPerPixel);
				return null;
			}

			return bmp;
		}


		private static void Read32BitImage(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			Color32[] data = bmp.imageData = new Color32[w * h];
			if (aReader.BaseStream.Position + w * h * 4 > aReader.BaseStream.Length)
			{
				Debug.LogError("Unexpected end of file.");
				return;
			}

			int shiftR = GetShiftCount(bmp.rMask);
			int shiftG = GetShiftCount(bmp.gMask);
			int shiftB = GetShiftCount(bmp.bMask);
			int shiftA = GetShiftCount(bmp.aMask);
			byte a = 255;
			for (int i = 0; i < data.Length; i++)
			{
				uint v = aReader.ReadUInt32();
				byte r = (byte)((v & bmp.rMask) >> shiftR);
				byte g = (byte)((v & bmp.gMask) >> shiftG);
				byte b = (byte)((v & bmp.bMask) >> shiftB);
				if (bmp.bMask != 0)
				{
					a = (byte)((v & bmp.aMask) >> shiftA);
				}

				data[i] = new Color32(r, g, b, a);
			}
		}


		private static void Read24BitImage(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			int rowLength = (24 * w + 31) / 32 * 4;
			int count = rowLength * h;
			int pad = rowLength - w * 3;
			Color32[] data = bmp.imageData = new Color32[w * h];
			if (aReader.BaseStream.Position + count > aReader.BaseStream.Length)
			{
				Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + count) +
				               " bytes, expected " + aReader.BaseStream.Length + " bytes)");
				return;
			}

			int shiftR = GetShiftCount(bmp.rMask);
			int shiftG = GetShiftCount(bmp.gMask);
			int shiftB = GetShiftCount(bmp.bMask);
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					uint v = aReader.ReadByte() | ((uint)aReader.ReadByte() << 8) | ((uint)aReader.ReadByte() << 16);
					byte r = (byte)((v & bmp.rMask) >> shiftR);
					byte g = (byte)((v & bmp.gMask) >> shiftG);
					byte b = (byte)((v & bmp.bMask) >> shiftB);
					data[x + y * w] = new Color32(r, g, b, 255);
				}

				for (int i = 0; i < pad; i++)
				{
					aReader.ReadByte();
				}
			}
		}

		private static void Read16BitImage(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			int rowLength = (16 * w + 31) / 32 * 4;
			int count = rowLength * h;
			int pad = rowLength - w * 2;
			Color32[] data = bmp.imageData = new Color32[w * h];
			if (aReader.BaseStream.Position + count > aReader.BaseStream.Length)
			{
				Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + count) +
				               " bytes, expected " + aReader.BaseStream.Length + " bytes)");
				return;
			}

			int shiftR = GetShiftCount(bmp.rMask);
			int shiftG = GetShiftCount(bmp.gMask);
			int shiftB = GetShiftCount(bmp.bMask);
			int shiftA = GetShiftCount(bmp.aMask);
			byte a = 255;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					uint v = aReader.ReadByte() | ((uint)aReader.ReadByte() << 8);
					byte r = (byte)((v & bmp.rMask) >> shiftR);
					byte g = (byte)((v & bmp.gMask) >> shiftG);
					byte b = (byte)((v & bmp.bMask) >> shiftB);
					if (bmp.aMask != 0)
					{
						a = (byte)((v & bmp.aMask) >> shiftA);
					}

					data[x + y * w] = new Color32(r, g, b, a);
				}

				for (int i = 0; i < pad; i++)
				{
					aReader.ReadByte();
				}
			}
		}

		private static void ReadIndexedImage(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			int bitCount = bmp.info.nBitsPerPixel;
			int rowLength = (bitCount * w + 31) / 32 * 4;
			int count = rowLength * h;
			int pad = rowLength - (w * bitCount + 7) / 8;
			Color32[] data = bmp.imageData = new Color32[w * h];
			if (aReader.BaseStream.Position + count > aReader.BaseStream.Length)
			{
				Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + count) +
				               " bytes, expected " + aReader.BaseStream.Length + " bytes)");
				return;
			}

			BitStreamReader bitReader = new BitStreamReader(aReader);
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					int v = (int)bitReader.ReadBits(bitCount);
					if (v >= bmp.palette.Count)
					{
						Debug.LogError("Indexed bitmap has indices greater than it's color palette");
						return;
					}

					data[x + y * w] = bmp.palette[v];
				}

				bitReader.Flush();
				for (int i = 0; i < pad; i++)
				{
					aReader.ReadByte();
				}
			}
		}

		private static void ReadIndexedImageRLE4(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			Color32[] data = bmp.imageData = new Color32[w * h];
			int x = 0;
			int y = 0;
			int yOffset = 0;
			while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1)
			{
				int count = aReader.ReadByte();
				byte d = aReader.ReadByte();
				if (count > 0)
				{
					for (int i = count / 2; i > 0; i--)
					{
						data[x++ + yOffset] = bmp.palette[(d >> 4) & 0x0F];
						data[x++ + yOffset] = bmp.palette[d & 0x0F];
					}

					if ((count & 0x01) > 0)
					{
						data[x++ + yOffset] = bmp.palette[(d >> 4) & 0x0F];
					}
				}
				else
				{
					if (d == 0)
					{
						x = 0;
						y += 1;
						yOffset = y * w;
					}
					else if (d == 1)
					{
						break;
					}
					else if (d == 2)
					{
						x += aReader.ReadByte();
						y += aReader.ReadByte();
						yOffset = y * w;
					}
					else
					{
						for (int i = d / 2; i > 0; i--)
						{
							byte d2 = aReader.ReadByte();
							data[x++ + yOffset] = bmp.palette[(d2 >> 4) & 0x0F];
							data[x++ + yOffset] = bmp.palette[d2 & 0x0F];
						}

						if ((d & 0x01) > 0)
						{
							data[x++ + yOffset] = bmp.palette[(aReader.ReadByte() >> 4) & 0x0F];
						}

						if ((((d - 1) / 2) & 1) == 0)
						{
							aReader.ReadByte(); // padding (word alignment)
						}
					}
				}
			}
		}

		private static void ReadIndexedImageRLE8(BinaryReader aReader, BMPImage bmp)
		{
			int w = Mathf.Abs(bmp.info.width);
			int h = Mathf.Abs(bmp.info.height);
			Color32[] data = bmp.imageData = new Color32[w * h];
			int x = 0;
			int y = 0;
			int yOffset = 0;
			while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1)
			{
				int count = aReader.ReadByte();
				byte d = aReader.ReadByte();
				if (count > 0)
				{
					for (int i = count; i > 0; i--)
					{
						data[x++ + yOffset] = bmp.palette[d];
					}
				}
				else
				{
					if (d == 0)
					{
						x = 0;
						y += 1;
						yOffset = y * w;
					}
					else if (d == 1)
					{
						break;
					}
					else if (d == 2)
					{
						x += aReader.ReadByte();
						y += aReader.ReadByte();
						yOffset = y * w;
					}
					else
					{
						for (int i = d; i > 0; i--)
						{
							data[x++ + yOffset] = bmp.palette[aReader.ReadByte()];
						}

						if ((d & 0x01) > 0)
						{
							aReader.ReadByte(); // padding (word alignment)
						}
					}
				}
			}
		}

		private static int GetShiftCount(uint mask)
		{
			for (int i = 0; i < 32; i++)
			{
				if ((mask & 0x01) > 0)
				{
					return i;
				}

				mask >>= 1;
			}

			return -1;
		}

		private static uint GetMask(int bitCount)
		{
			uint mask = 0;
			for (int i = 0; i < bitCount; i++)
			{
				mask <<= 1;
				mask |= 0x01;
			}

			return mask;
		}

		private bool ReadFileHeader(BinaryReader aReader, ref BMPFileHeader aFileHeader)
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			try
			{
#endif
				aFileHeader.magic = aReader.ReadUInt16();
				if (aFileHeader.magic != MAGIC)
				{
					return false;
				}

				aFileHeader.filesize = aReader.ReadUInt32();
				aFileHeader.reserved = aReader.ReadUInt32();
				aFileHeader.offset = aReader.ReadUInt32();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			}
			catch (Exception e)
			{
				Debug.LogWarning($"{fileName}\n{e}");
				return false;
			}
#endif
			return true;
		}

		private static bool ReadInfoHeader(BinaryReader aReader, ref BitmapInfoHeader aHeader)
		{
			aHeader.size = aReader.ReadUInt32();
			if (aHeader.size < 40)
			{
				return false;
			}

			aHeader.width = aReader.ReadInt32();
			aHeader.height = aReader.ReadInt32();
			aHeader.nColorPlanes = aReader.ReadUInt16();
			aHeader.nBitsPerPixel = aReader.ReadUInt16();
			aHeader.compressionMethod = (BMPComressionMode)aReader.ReadInt32();
			aHeader.rawImageSize = aReader.ReadUInt32();
			aHeader.xPPM = aReader.ReadInt32();
			aHeader.yPPM = aReader.ReadInt32();
			aHeader.nPaletteColors = aReader.ReadUInt32();
			aHeader.nImportantColors = aReader.ReadUInt32();
			int pad = (int)aHeader.size - 40;
			if (pad > 0)
			{
				aReader.ReadBytes(pad);
			}

			return true;
		}

		public static List<Color32> ReadPalette(BinaryReader aReader, BMPImage aBmp, bool aReadAlpha)
		{
			uint count = aBmp.info.nPaletteColors;
			if (count == 0u)
			{
				count = 1u << aBmp.info.nBitsPerPixel;
			}

			List<Color32> palette = new List<Color32>((int)count);
			for (int i = 0; i < count; i++)
			{
				byte b = aReader.ReadByte();
				byte g = aReader.ReadByte();
				byte r = aReader.ReadByte();
				byte a = aReader.ReadByte();
				if (!aReadAlpha)
				{
					a = 255;
				}

				palette.Add(new Color32(r, g, b, a));
			}

			return palette;
		}
	}
}