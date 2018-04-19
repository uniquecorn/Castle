namespace Castle
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Linq;

	public class Tools : MonoBehaviour
	{
		public static string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		public static T RandomObject<T>(T[] assets)
		{
			return assets[Random.Range(0, assets.Length)];
		}
		public static GameObject InstantiateRandom(GameObject[] assets)
		{
			return Instantiate(RandomObject(assets), Vector3.zero, Quaternion.identity);
		}
		public static Texture2D LoadTGA(string fileName)
		{
			using (var imageFile = File.OpenRead(fileName))
			{
				return LoadTGA(imageFile);
			}
		}
		public static Texture2D LoadTGA(Stream TGAStream)
		{
			using (BinaryReader r = new BinaryReader(TGAStream))
			{
				// Skip some header info we don't care about.
				// Even if we did care, we have to move the stream seek point to the beginning,
				// as the previous method in the workflow left it at the end.
				r.BaseStream.Seek(12, SeekOrigin.Begin);

				short width = r.ReadInt16();
				short height = r.ReadInt16();
				int bitDepth = r.ReadByte();

				// Skip a byte of header information we don't care about.
				r.BaseStream.Seek(1, SeekOrigin.Current);

				Texture2D tex = new Texture2D(width, height);
				Color32[] pulledColors = new Color32[width * height];

				if (bitDepth == 32)
				{
					for (int i = 0; i < width * height; i++)
					{
						byte red = r.ReadByte();
						byte green = r.ReadByte();
						byte blue = r.ReadByte();
						byte alpha = r.ReadByte();

						pulledColors[i] = new Color32(blue, green, red, alpha);
					}
				}
				else if (bitDepth == 24)
				{
					for (int i = 0; i < width * height; i++)
					{
						byte red = r.ReadByte();
						byte green = r.ReadByte();
						byte blue = r.ReadByte();

						pulledColors[i] = new Color32(blue, green, red, 1);
					}
				}
				else
				{
					throw new System.Exception("TGA texture had non 32/24 bit depth.");
				}

				tex.SetPixels32(pulledColors);
				tex.Apply();
				return tex;
			}
		}
		public static System.TimeSpan CalculateHoursDifference(string datetime1, string datetime2)
		{
			//print(System.DateTime.Now.ToString());
			System.DateTime date1 = System.DateTime.Parse(datetime1);
			System.DateTime date2 = System.DateTime.Parse(datetime2);

			//print(date2.CompareTo(date1));

			return date2.Subtract(date1);
		}
		public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volume = 1)
		{
			GameObject tempGO = new GameObject("TempAudio"); // create the temp object
			tempGO.transform.position = pos; // set its position
			AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
			aSource.clip = clip; // define the clip
								 // set other aSource properties here, if desired

			aSource.Play(); // start the sound
			aSource.volume = volume;
			aSource.spatialBlend = 1;
			Destroy(tempGO, clip.length); // destroy object after clip duration
			return aSource; // return the AudioSource reference
		}
		public static AudioSource PlayClip2D(AudioClip clip, float volume = 1, float pitch = 1)
		{
			GameObject tempGO = new GameObject("TempAudio"); // create the temp object
			tempGO.transform.position = Vector2.zero; // set its position
			AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
			aSource.clip = clip; // define the clip
								 // set other aSource properties here, if desired

			aSource.Play(); // start the sound
			aSource.volume = volume;
			aSource.pitch = pitch;
			aSource.spatialBlend = 1;
			Destroy(tempGO, clip.length); // destroy object after clip duration
			return aSource; // return the AudioSource reference
		}
		public static bool RandomBoolean()
		{
			return Random.value < 0.5f;
		}
		public static Quaternion RandomRotation()
		{
			float _rot = Random.value;
			if (_rot < 0.25f)
			{
				return Quaternion.identity;
			}
			else if (_rot < 0.5f)
			{
				return Quaternion.Euler(0, 90, 0);
			}
			else if (_rot < 0.75f)
			{
				return Quaternion.Euler(0, 180, 0);
			}
			else
			{
				return Quaternion.Euler(0, 270, 0);
			}
		}
		/// <summary>
		/// i = 90 degrees
		/// </summary>
		/// <param name="i">rotation index</param>
		/// <returns></returns>
		public static Quaternion KeyRotations(int i)
		{
			return Quaternion.Euler(0, i * 90, 0);
		}
		public static Vector3 RandomVector3(float randomness = 1)
		{
			return new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
		}
		public static Vector3 RandomVector3(float randomnessX, float randomnessY, float randomnessZ)
		{
			return new Vector3(Random.Range(-randomnessX, randomnessX), Random.Range(-randomnessY, randomnessY), Random.Range(-randomnessZ, randomnessZ));
		}
		public static Vector2 RandomVector2(float randomness = 1)
		{
			return new Vector2(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
		}
		public static float LerpSnap(float a, float b, float t, int snapSensitivity = 10)
		{
			float value = Mathf.Lerp(a, b, t);
			if (Mathf.Abs(value - b) <= (Mathf.Abs(b - a) / snapSensitivity))
			{
				value = b;
			}
			return value;
		}
		public static Vector2 LerpSnap2(Vector2 a, Vector2 b, float t, int snapSensitivity = 10)
		{
			return new Vector2(LerpSnap(a.x, b.x, t, snapSensitivity), LerpSnap(a.y, b.y, t, snapSensitivity));
		}
		public static Vector3 LerpSnap3(Vector3 a, Vector3 b, float t, int snapSensitivity = 10)
		{
			return new Vector3(LerpSnap(a.x, b.x, t, snapSensitivity), LerpSnap(a.y, b.y, t, snapSensitivity), LerpSnap(a.z, b.z, t, snapSensitivity));
		}
		public static float FreeLerp(float a, float b, float t)
		{
			return ((1.0f - t) * a) + (t * b);
		}
		public static Vector2 FreeLerp(Vector2 a, Vector2 b, float t)
		{
			return new Vector2(FreeLerp(a.x, b.x, t), FreeLerp(a.y, b.y, t));
		}
		public static Vector3 FreeLerp(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(FreeLerp(a.x, b.x, t), FreeLerp(a.y, b.y, t), FreeLerp(a.z, b.z, t));
		}
		public static Vector2 Vector2FromAngle(float a)
		{
			a *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
		}
		public static Color RandomColor(float _alpha = 1)
		{
			float total = 2.0f;
			float _r, _g, _b;
			_r = Random.Range(0.0f, 1.0f);
			total -= _r;
			_g = Mathf.Min(Random.Range(0.0f, total), 1.0f);
			total -= _g;
			_b = total;
			Color result = new Color()
			{
				a = _alpha,
				r = _r,
				g = _g,
				b = _b
			};
			return result;
		}
		public static void RunSh(string path, string[] arguments = null)
		{
			string terminalPath = "sh";
			string terminalCommand = path;
			if (arguments != null)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					terminalCommand += " " + arguments[i];
				}
			}
			System.Diagnostics.Process.Start(terminalPath, terminalCommand);
		}
		public static string StripPunctuation(string value)
		{
			return new string(value.Where(c => !char.IsPunctuation(c)).ToArray());
		}
		public static string StripWhitespace(string value)
		{
			return new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray());
		}
		public static string StripCharacters(string value, char[] chars)
		{
			string stripped = value;
			foreach (char _char in chars)
			{
				stripped = StripCharacter(stripped, _char);
			}
			return stripped;
		}
		public static string StripCharacter(string value, char _char)
		{
			return new string(value.Where(c => (c != _char)).ToArray());
		}
		public static string AddNumberToString(string value, int number)
		{
			string[] valueStripped = value.Split('_');
			int daNumber;
			if (int.TryParse(valueStripped[valueStripped.Length - 1], out daNumber))
			{
				return valueStripped[0] + "_" + (daNumber + 1);
			}
			else
			{
				return value;
			}
		}
		public static string RandomLetter()
		{
			int i = Random.Range(0, 26);
			return letters[i];
		}
		public static float Hermite(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value * value * (3.0f - (2.0f * value)));
		}
		public static float Sinerp(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
		}
		public static float Coserp(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
		}
		public static float Berp(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + (2.5f * value * value * value))) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}
		public static float SmoothStep(float x, float min, float max)
		{
			x = Mathf.Clamp(x, min, max);
			float v1 = (x - min) / (max - min);
			float v2 = (x - min) / (max - min);
			return -2 * v1 * v1 * v1 + 3 * v2 * v2;
		}
		public static float GetYRotFromVec(Vector3 v1, Vector3 v2)
		{
			float _r = Mathf.Atan2(v2.x - v1.x, v1.z - v2.z);
			float _d = (_r / Mathf.PI) * 180;

			return _d;
		}
		public static Vector3 Midpoint(Vector3 start, Vector3 end)
		{
			return Vector3.Lerp(start, end, 0.5f);
		}
		public static float Lerp(float start, float end, float value)
		{
			return ((1.0f - value) * start) + (value * end);
		}
		public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
			float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
			return lineStart + (closestPoint * lineDirection);
		}
		public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 fullDirection = lineEnd - lineStart;
			Vector3 lineDirection = Vector3.Normalize(fullDirection);
			float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
			return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
		}
		public static float Bounce(float x)
		{
			return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
		}
		// test for value that is near specified float (due to floating point inprecision)
		// all thanks to Opless for this!
		public static bool Approx(float val, float about, float range)
		{
			return ((Mathf.Abs(val - about) < range));
		}
		// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
		// compares the square of the distance to the square of the range as this 
		// avoids calculating a square root which is much slower than squaring the range
		public static bool Approx(Vector3 val, Vector3 about, float range)
		{
			return ((val - about).sqrMagnitude < range * range);
		}
		/*
		  * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
		  * This is useful when interpolating eulerAngles and the object
		  * crosses the 0/360 boundary.  The standard Lerp function causes the object
		  * to rotate in the wrong direction and looks stupid. Clerp fixes that.
		  */
		public static float Clerp(float start, float end, float value)
		{
			float min = 0.0f;
			float max = 360.0f;
			//half the distance between min and max
			float half = Mathf.Abs((max - min) / 2.0f);
			float retval = 0.0f;
			float diff = 0.0f;

			if ((end - start) < -half)
			{
				diff = ((max - start) + end) * value;
				retval = start + diff;
			}
			else if ((end - start) > half)
			{
				diff = -((max - end) + start) * value;
				retval = start + diff;
			}
			else retval = start + ((end - start) * value);

			// Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
			return retval;
		}
		public static string ReadTextFile(string sFileName)
		{
			//Debug.Log("Reading " + sFileName);

			//Check to see if the filename specified exists, if not try adding '.txt', otherwise fail
			string sFileNameFound = "";
			if (File.Exists(sFileName))
			{
				//Debug.Log("Reading '" + sFileName + "'.");
				sFileNameFound = sFileName; //file found
			}
			else if (File.Exists(sFileName + ".txt"))
			{
				sFileNameFound = sFileName + ".txt";
			}
			else
			{
				UnityEngine.Debug.Log("Could not find file '" + sFileName + "'.");
				return null;
			}

			StreamReader sr;
			try
			{
				sr = new StreamReader(sFileNameFound);
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogWarning("Something went wrong with read.  " + e.Message);
				return null;
			}

			string fileContents = sr.ReadToEnd();
			sr.Close();

			return fileContents;
		}
		public static string FindNonConflictingDir(string basePath, string extension, int num = 0)
		{
			if (num == 0)
			{
				if (File.Exists(basePath + extension))
				{
					return FindNonConflictingDir(basePath, extension, num + 1);
				}
				else
				{
					return (basePath + extension);
				}
			}
			else
			{
				if (File.Exists(basePath + num + extension))
				{
					return FindNonConflictingDir(basePath, extension, num + 1);
				}
				else
				{
					return (basePath + num + extension);
				}
			}
		}
		public static Vector3 Vec3RepZ(Vector3 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}
		public static void WriteTextFile(string sFilePathAndName, string sTextContents)
		{
			StreamWriter sw = new StreamWriter(sFilePathAndName);
			sw.WriteLine(sTextContents);
			sw.Flush();
			sw.Close();
		}
	}
}