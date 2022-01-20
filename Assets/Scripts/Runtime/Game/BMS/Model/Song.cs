using System;
using System.Collections.Generic;

namespace RGF.Game.BMS.Model
{
	/// <summary>
	/// Track contains sheets of various level with identical song.
	/// 트랙은 하나의 곡으로 작성된 다양한 난이도의 악보들을 포함합니다.
	/// </summary>
	[Serializable]
	public sealed class Song
	{
		public string songName = default;
		public string directory = default;

		public List<Track> tracks = new List<Track>(8);

		public Song() { }
	}
}