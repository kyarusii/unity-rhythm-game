using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGF.Game.BMS.Object;
using UnityEngine;
using Decoder = RGF.Game.Utility.Decoder;
using Enum = RGF.Game.Common.Enum;
using Random = System.Random;

namespace RGF.Game.BMS.Model
{
	/// <summary>
	/// BMS 헤더 (곡 데이터)
	/// 데피니션
	/// </summary>
	public sealed class Track : IComparable<Track>, IEquatable<Track>
	{
		#region HEADER

		public string artist = string.Empty;
		public string backBmp = string.Empty;
		public string banner = string.Empty;
		public string genre = string.Empty;
		public int lnObj = default;
		public Enum.Lntype lnType = default;
		public int player = default;

		public int playerLevel = default;
		public string preview = string.Empty;
		public int rank = default;
		public string stageFile = string.Empty;

		public Texture2D stageTexture = default;
		public string subtitle = string.Empty;
		public string title = string.Empty;

		public string RootPath { get; }
		public string FilePath { get; }
		public string SongName { get; set; }

		public float total = 400;
		public double bpm = default;

		public readonly Dictionary<int, string> wavNoteMap = new Dictionary<int, string>();
		public readonly Dictionary<string, double> bpms = new Dictionary<string, double>();
		public readonly Dictionary<string, double> stopDurations = new Dictionary<string, double>();

		public readonly Dictionary<string, string> bgVideoTable = new Dictionary<string, string>();
		private readonly Dictionary<string, string> backgroundImage = new Dictionary<string, string>();

		#endregion

		#region BODY

		public int noteCount = 0;
		public int barCount = 0;

		private readonly List<ChangeBGObject> changeBgObjs = new List<ChangeBGObject>();
		private readonly List<NoteObject> noteObjs = new List<NoteObject>();
		private readonly List<BpmObject> bpmObjs = new List<BpmObject>();
		private readonly List<StopObject> stopObjs = new List<StopObject>();

		public readonly Dictionary<int, double> beatTable = new Dictionary<int, double>();
		public readonly Lane[] lanes = new Lane[9];

		public List<BpmObject> GetBpms()
		{
			return bpmObjs.ToList();
		}

		public List<StopObject> GetStops()
		{
			return stopObjs.ToList();
		}

		public List<NoteObject> GetNotes()
		{
			return noteObjs.ToList();
		}

		public List<ChangeBGObject> GetChangeBGs()
		{
			return changeBgObjs.ToList();
		}

		#endregion

		public Track(string filePath)
		{
			FilePath = filePath;
			RootPath = Directory.GetParent(FilePath)?.FullName;

			for (int i = 0; i < 9; ++i)
			{
				lanes[i] = new Lane();
			}
		}

		public bool ReadAll()
		{
			StreamReader reader = new StreamReader(FilePath, Encoding.GetEncoding(932));

			bool hasError = false;
			bool inHeader = false;
			bool inBody = false;

			bool isInIfBrace = false;
			bool isIfVaild = false;
			int random = 1;

			int LNBits = 0;
			Random rand = new Random();

			string s;
			while ((s = reader.ReadLine()) != null)
			{
				try
				{
					if (s.Length <= 3)
					{
						continue;
					}

					if (s.Contains("*---------------------- HEADER FIELD"))
					{
						inHeader = true;
					}

					if (inHeader)
					{
						ParseLine(s);

						if (HeaderUtility.IsHeaderEnd(s))
						{
							inHeader = false;
							inBody = true;
							continue;
						}
					}

					if (inBody)
					{
						if (!s.StartsWith("#", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}

						if (s.StartsWith("#random", StringComparison.OrdinalIgnoreCase))
						{
							random = rand.Next(1, int.Parse(s.Substring(8)) + 1);
							continue;
						}

						if (s.StartsWith("#if", StringComparison.OrdinalIgnoreCase))
						{
							isInIfBrace = true;
							if (s[4] - '0' == random)
							{
								isIfVaild = true;
							}

							continue;
						}

						if (s.StartsWith("#endif", StringComparison.OrdinalIgnoreCase))
						{
							isInIfBrace = false;
							isIfVaild = false;
							continue;
						}

						if (isInIfBrace && !isIfVaild)
						{
							continue;
						}

						if (!int.TryParse(s.Substring(1, 3), out int bar))
						{
							continue;
						}

						if (barCount < bar)
						{
							barCount = bar;
						}

						switch (s[4])
						{
							case '0':
							{
								int beatLength = (s.Length - 7) / 2;
								switch (s[5])
								{
									case '1':
									{
										for (int i = 7; i < s.Length - 1; i += 2)
										{
											int beat = (i - 7) / 2;
											int keySound = Decoder.Decode36(s.Substring(i, 2));

											if (keySound != 0)
											{
												AddBGSound(bar, beat, beatLength, keySound);
											}
										}

										break;
									}
									case '2':
									{
										double beat = double.Parse(s.Substring(7));
										AddNewBeat(bar, beat);

										break;
									}
									case '3':
									{
										for (int i = 7; i < s.Length - 1; i += 2)
										{
											int beat = (i - 7) / 2;
											double bpm = int.Parse(s.Substring(i, 2), NumberStyles.HexNumber);

											if (bpm != 0)
											{
												AddBPM(bar, beat, beatLength, bpm);
											}
										}

										break;
									}
									case '4':
									{
										for (int i = 7; i < s.Length - 1; i += 2)
										{
											int beat = (i - 7) / 2;
											string key = s.Substring(i, 2);

											if (string.CompareOrdinal(key, "00") != 0)
											{
												if (bgVideoTable.ContainsKey(key))
												{
													AddBGAChange(bar, beat, beatLength, key);
												}
												else
												{
													AddBGAChange(bar, beat, beatLength, key, true);
												}
											}
										}

										break;
									}
									case '8':
									{
										for (int i = 7; i < s.Length - 1; i += 2)
										{
											int beat = (i - 7) / 2;
											string key = s.Substring(i, 2);
											if (string.Compare(key, "00", StringComparison.Ordinal) != 0)
											{
												AddBPM(bar, beat, beatLength, bpms[key]);
											}
										}

										break;
									}
									case '9':
									{
										for (int i = 7; i < s.Length - 1; i += 2)
										{
											int beat = (i - 7) / 2;
											string sub = s.Substring(i, 2);
											if (string.CompareOrdinal(sub, "00") != 0)
											{
												AddStop(bar, beat, beatLength, sub);
											}
										}

										break;
									}
								}

								break;
							}
							case '1':
							case '5':
							{
								int line, beatLength;
								line = s[5] - '1';
								beatLength = (s.Length - 7) / 2;

								for (int i = 7; i < s.Length - 1; i += 2)
								{
									int beat = (i - 7) / 2;

									int keySound = Decoder.Decode36(s.Substring(i, 2));
									if (keySound != 0)
									{
										if (s[4] == '5')
										{
											if ((LNBits & (1 << line)) != 0)
											{
												AddNote(line, bar, beat, beatLength, -1, 1);
												LNBits &= ~(1 << line); //erase bit
												continue;
											}

											LNBits |= 1 << line; //write bit
										}

										if (lnType.HasFlag(Enum.Lntype.LNOBJ) && keySound == lnObj)
										{
											AddNote(line, bar, beat, beatLength, keySound, 1);
										}
										else
										{
											AddNote(line, bar, beat, beatLength, keySound, 0);
										}
									}
								}

								break;
							}
							case 'D':
							case 'E':
							{
								int line, beatLength;
								line = s[5] - '1';
								beatLength = (s.Length - 7) / 2;

								for (int i = 7; i < s.Length - 1; i += 2)
								{
									int beat = (i - 7) / 2;

									int keySound = Decoder.Decode36(s.Substring(i, 2));
									if (keySound != 0)
									{
										AddNote(line, bar, beat, beatLength, keySound, -1);
									}
								}

								break;
							}
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning($"{nameof(Track)} Read Error : {e.Message}, {e.StackTrace}");
					hasError = true;
					break;
				}
			}

			GetBeatsAndTimings(bpm);

			if (hasError)
			{
				return false;
			}
			else { }

			return true;
		}


		public async Task<bool> ReadAsync()
		{
			StreamReader reader = new StreamReader(FilePath, Encoding.GetEncoding(932));

			bool hasError = false;
			bool inHeader = false;

			string s;
			while ((s = await reader.ReadLineAsync()) != null)
			{
				if (s.Length <= 3)
				{
					continue;
				}

				if (s.Contains("*---------------------- HEADER FIELD"))
				{
					inHeader = true;
				}

				if (!inHeader)
				{
					continue;
				}

				try
				{
					ParseLine(s);

					if (HeaderUtility.IsHeaderEnd(s))
					{
						inHeader = false;
						break;
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning($"{nameof(Track)} Read Error : {e.Message}, {e.StackTrace}");
					hasError = true;
					break;
				}
			}

			if (hasError)
			{
				return false;
			}

			return true;
		}

		private void ParseLine(string s)
		{
			if (HeaderUtility.IsPlayerLevel(s))
			{
				int.TryParse(s.Substring(11), out playerLevel);
				return;
			}

			if (HeaderUtility.IsStageFile(s))
			{
				stageFile = s.Substring(11);
				return;
			}

			if (HeaderUtility.IsSubTitle(s))
			{
				subtitle = s.Substring(10).Trim('[', ']');
				return;
			}

			if (HeaderUtility.IsPreview(s))
			{
				preview = s.Substring(9, s.Length - 13);
				return;
			}

			if (HeaderUtility.IsBackBmp(s))
			{
				backBmp = s.Substring(9);
				return;
			}

			if (HeaderUtility.IsPlayer(s))
			{
				int.TryParse(s.Substring(8), out player);
				return;
			}

			if (HeaderUtility.IsArtist(s))
			{
				artist = s.Substring(8);
				return;
			}

			if (HeaderUtility.IsBanner(s))
			{
				banner = s.Substring(8);
				return;
			}

			if (HeaderUtility.IsLnType(s))
			{
				int.TryParse(s.Substring(8), out int type);
				lnType |= (Enum.Lntype)(1 << type);
				return;
			}

			if (HeaderUtility.IsGenre(s))
			{
				genre = s.Substring(7);
				return;
			}

			if (HeaderUtility.IsTitle(s))
			{
				title = s.Substring(7);

				if (!string.IsNullOrEmpty(title))
				{
					int subTitleIndex = title.LastIndexOf('[');
					if (subTitleIndex > 0)
					{
						SongName = title.Remove(subTitleIndex);
						subtitle = title.Substring(subTitleIndex).Trim('[', ']');
					}
					else
					{
						SongName = title;
					}
				}

				return;
			}

			if (HeaderUtility.IsTotal(s))
			{
				float.TryParse(s.Substring(7), out total);
				return;
			}

			if (HeaderUtility.IsRank(s))
			{
				int.TryParse(s.Substring(6), out rank);
				return;
			}

			if (HeaderUtility.IsBPM(s))
			{
				if (s.IndexOf(' ') == 4)
				{
					double.TryParse(s.Substring(5), out bpm);
				}
				else
				{
					string key = s.Substring(4, 2);
					double bpmNote = double.Parse(s.Substring(7));

					bpms.Add(key, bpmNote);
				}

				return;
			}

			if (HeaderUtility.IsBMP(s))
			{
				string key = s.Substring(4, 2);
				string extend = s.Substring(s.Length - 3, 3);
				string Path = s.Substring(7, s.Length - 7);

				if (string.Compare(extend, "mpg", StringComparison.OrdinalIgnoreCase) == 0)
				{
					bgVideoTable.Add(key, Path);
				}
				else if (string.Compare(extend, "bmp", StringComparison.OrdinalIgnoreCase) == 0)
				{
					backgroundImage.Add(key, Path);
				}
				else if (string.Compare(extend, "png", StringComparison.OrdinalIgnoreCase) == 0)
				{
					backgroundImage.Add(key, Path);
				}

				return;
			}

			if (HeaderUtility.IsStop(s))
			{
				if (s.IndexOf(' ') == 7)
				{
					string key = s.Substring(5, 2);
					double stopDuration = int.Parse(s.Substring(8)) / 192.0f;

					stopDurations[key] = stopDuration;
				}

				return;
			}

			if (HeaderUtility.IsWav(s))
			{
				int key = Decoder.Decode36(s.Substring(4, 2));
				string path = s.Substring(7, s.Length - 11);

				wavNoteMap.Add(key, path);

				return;
			}

			if (HeaderUtility.IsLnObj(s))
			{
				lnObj = Decoder.Decode36(s.Substring(7, 2));
				lnType |= Enum.Lntype.LNOBJ;
			}
		}

		/// <summary>
		/// 내부적으로 Sort() 에서 사용됨.
		/// 낮은 난이도부터 높은 순으로 정렬.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(Track other)
		{
			if (playerLevel > other.playerLevel)
			{
				return 1;
			}

			if (playerLevel == other.playerLevel)
			{
				return 0;
			}

			return -1;
		}

		public bool Equals(Track other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return artist == other.artist && backBmp == other.backBmp && banner == other.banner &&
			       genre == other.genre && rank == other.rank && stageFile == other.stageFile &&
			       subtitle == other.subtitle && title == other.title && total.Equals(other.total) &&
			       bpm.Equals(other.bpm) && RootPath == other.RootPath && FilePath == other.FilePath &&
			       SongName == other.SongName;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is Track other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = artist != null ? artist.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (backBmp != null ? backBmp.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (banner != null ? banner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (genre != null ? genre.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ rank;
				hashCode = (hashCode * 397) ^ (stageFile != null ? stageFile.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (subtitle != null ? subtitle.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (title != null ? title.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ total.GetHashCode();
				hashCode = (hashCode * 397) ^ bpm.GetHashCode();
				hashCode = (hashCode * 397) ^ (RootPath != null ? RootPath.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SongName != null ? SongName.GetHashCode() : 0);
				return hashCode;
			}
		}

		#region BODY

		private void AddBGAChange(int bar, double beat, double beatLength, string key, bool isPic = false)
		{
			changeBgObjs.Add(new ChangeBGObject(bar, key, beat, beatLength, isPic));
		}

		private void AddNote(int line, int bar, double beat, double beatLength, int keySound, int mineFlag)
		{
			if (mineFlag == -1)
			{
				lanes[line].mineList.Add(new NoteObject(bar, keySound, beat, beatLength, mineFlag));
			}
			else
			{
				++noteCount;
				lanes[line].noteList.Add(new NoteObject(bar, keySound, beat, beatLength, mineFlag));
			}
		}

		private void AddBGSound(int bar, double beat, double beatLength, int keySound)
		{
			noteObjs.Add(new NoteObject(bar, keySound, beat, beatLength, 0));
		}

		private void AddNewBeat(int bar, double beatC)
		{
			beatTable.Add(bar, beatC);
		}

		private void AddBPM(int bar, double beat, double beatLength, double bpm)
		{
			bpmObjs.Add(new BpmObject(bar, bpm, beat, beatLength));
		}

		private void AddStop(int bar, double beat, double beatLength, string key)
		{
			stopObjs.Add(new StopObject(bar, key, beat, beatLength));
		}

		private double GetBeat(int bar)
		{
			return beatTable.ContainsKey(bar) ? beatTable[bar] : 1.0;
		}

		private void GetBeatsAndTimings(double bpm)
		{
			foreach (BpmObject b in bpmObjs)
			{
				b.CalculateBeat(GetPreviousBarBeatSum(b.bar), GetBeat(b.bar));
			}

			bpmObjs.Sort();

			if (bpmObjs.Count == 0 || bpmObjs.Count > 0 && bpmObjs[bpmObjs.Count - 1].beat != 0)
			{
				AddBPM(0, 0, 1, bpm);
			}

			bpmObjs[bpmObjs.Count - 1].timing = 0;
			for (int i = bpmObjs.Count - 2; i > -1; --i)
			{
				bpmObjs[i].timing = bpmObjs[i + 1].timing +
				                    (bpmObjs[i].beat - bpmObjs[i + 1].beat) / (bpmObjs[i + 1].Bpm / 60);
			}

			foreach (StopObject s in stopObjs)
			{
				s.CalculateBeat(GetPreviousBarBeatSum(s.bar), GetBeat(s.bar));
				s.timing = GetTimingInSecond(s);
			}

			stopObjs.Sort();

			foreach (ChangeBGObject c in changeBgObjs)
			{
				c.CalculateBeat(GetPreviousBarBeatSum(c.bar), GetBeat(c.bar));
				c.timing = GetTimingInSecond(c);
				int idx = stopObjs.Count - 1;
				double sum = 0;
				while (idx > 0 && c.beat > stopObjs[--idx].beat)
				{
					sum += stopDurations[stopObjs[idx].Key] / GetBpm(stopObjs[idx].beat) * 240;
				}

				c.timing += sum;
			}

			changeBgObjs.Sort();

			CalCulateTimingsInListExtension(noteObjs);

			foreach (Lane l in lanes)
			{
				CalCulateTimingsInListExtension(l.noteList);
				CalCulateTimingsInListExtension(l.mineList);
			}
		}

		private void CalCulateTimingsInListExtension(List<NoteObject> list)
		{
			foreach (NoteObject n in list)
			{
				n.CalculateBeat(GetPreviousBarBeatSum(n.bar), GetBeat(n.bar));
				n.timing = GetTimingInSecond(n);
				int idx = stopObjs.Count;
				double sum = 0;
				while (idx > 0 && n.beat > stopObjs[--idx].beat)
				{
					sum += stopDurations[stopObjs[idx].Key] / GetBpm(stopObjs[idx].beat) * 240;
				}

				n.timing += sum;
			}

			list.Sort();
		}

		private double GetBpm(double beat)
		{
			if (bpmObjs.Count == 1)
			{
				return bpmObjs[0].Bpm;
			}

			int idx = bpmObjs.Count - 1;
			while (idx > 0 && beat >= bpmObjs[--idx].beat) { }

			return bpmObjs[idx + 1].Bpm;
		}

		private double GetTimingInSecond(ObjectBase obj)
		{
			double timing = 0;
			int i;
			for (i = bpmObjs.Count - 1; i > 0 && obj.beat > bpmObjs[i - 1].beat; --i)
			{
				timing += (bpmObjs[i - 1].beat - bpmObjs[i].beat) / bpmObjs[i].Bpm * 60;
			}

			timing += (obj.beat - bpmObjs[i].beat) / bpmObjs[i].Bpm * 60;
			return timing;
		}

		public double GetPreviousBarBeatSum(int bar)
		{
			double sum = 0;
			for (int i = 0; i < bar; ++i)
			{
				sum += 4.0 * GetBeat(i);
			}

			return sum;
		}

		#endregion
	}
}