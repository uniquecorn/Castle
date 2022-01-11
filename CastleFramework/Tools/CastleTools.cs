using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Castle.Tools
{
    public static class CastleTools
    {
        public static int VersionNum
        {
            get
            {
                var lines = Application.version.Split('.');
                if (lines.Length == 2)
                {
                    return int.Parse(lines[0]) * 10000 + int.Parse(lines[1]) * 100;
                }
                var (MajorVersion, MinorVersion, PatchVersion) = (int.Parse(lines[0]), int.Parse(lines[1]), int.Parse(lines[2]));
                return MajorVersion * 10000 + MinorVersion * 100 + PatchVersion;
            }
        }
        public static readonly string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static readonly string[] Vowels = { "A", "E", "I", "O", "U"};
        public static string RandomLetter => Letters.RandomValue();
        public static Rect ScreenBounds => new(0,0, Screen.width,Screen.height);
        public static string RandomString(int length = 5)
        {
            string r ="";
            for (var i = 0; i < length; i++)
            {
                if (CoinToss)
                {
                    if (CoinToss)
                    {
                        r += RandomLetter.ToLower();
                    }
                    else
                    {
                        r += RandomLetter;
                    }
                }
                else
                {
                    r += Random.Range(0, 10);
                }
            }

            return r;
        }
        public static bool CoinToss => Random.value < 0.5f;
        public static T RandomObject<T>(IList<T> assets) => assets[Random.Range(0, assets.Count)];
        public static bool RandomObject<T>(IList<T> arr, System.Func<T, bool> filter,out T chosen)
        {
            var r = RandomNumEnumerable(arr.Count);
            var startingPos = Random.Range(0, arr.Count);
            chosen = arr[0];
            foreach (var i in r)
            {
                chosen = arr.LoopFrom(startingPos, i);
                if(filter(chosen)) return true;
            }
            return false;
        }
        public static int FloorRange(float value, int length, bool clamped = false) => clamped
            ? Mathf.Clamp(Mathf.FloorToInt(value * (length - 1)), 0, length - 1)
            : Mathf.FloorToInt(value * (length - 1)) % length;
        public static int ClampRange(float value, int length, bool clamped = false) => clamped
            ? Mathf.Clamp(Mathf.RoundToInt(value * (length - 1)), 0, length - 1)
            : Mathf.RoundToInt(value * (length - 1)) % length;
    
        public static string GetScrambledDeviceID()
        {
            var sysDeviceId = new List<char>(SystemInfo.deviceUniqueIdentifier);
            var UserID = "";
            var i = 0;
            while (sysDeviceId.Count > 0)
            {
                if (sysDeviceId[i % sysDeviceId.Count] != '-') UserID += sysDeviceId[i % sysDeviceId.Count];
                sysDeviceId.RemoveAt(i % sysDeviceId.Count);
                i++;
            }
            return UserID;
        }
        public static void AddToArray<T>(ref T[] array, T variable,bool noDuplicate = false)
        {
            if (noDuplicate && array.Contains(variable)) return;
            var arr = array.ToList();
            arr.Add(variable);
            array = arr.ToArray();
        }
        public static void RemoveFromArray<T>(ref T[] array, T variable)
        {
            var arr = array.ToList();
            arr.Remove(variable);
            array = arr.ToArray();
        }
    
        public static void CopyStringToClipboard(string s)
        {
            var te = new TextEditor
            {
                text = s
            };
            te.SelectAll();
            te.Copy();
        }
        public static IEnumerable<int> RandomNumEnumerable(int length) => Enumerable.Range(0, length).Shuffle();
        #region Vectors
        public static Vector2 CenterOfVectors(params Vector2[] vectors)
        {
            Vector2 sum = Vector2.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }
            foreach (Vector2 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }
        public static Vector3 CenterOfVectors(params Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }
            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }
        #endregion
        #region Math
        public static float InverseLerp(float start, float end, float val)
        {
            if (val <= start)
            {
                return 0;
            }
            if (val >= end)
            {
                return 1;
            }
            return (val - start) / (end - start);
        }
        public static float InverseLerpSmooth(float start, float end, float val) => Mathf.SmoothStep(0, 1, InverseLerp(start, end, val));
        #endregion
        #region Strings
        public static string SlugKey(string s, bool lowercase = true)
        {
            if (lowercase) s = s.ToLower();
            s = StripWSP(s);
            return s;
        }
        public static string StripPunctuation(string value) => new(value.Where(c => !char.IsPunctuation(c)).ToArray());
        public static string StripWhitespace(string value) => new(value.Where(c => !char.IsWhiteSpace(c)).ToArray());
        public static string StripWSP(string value)=> new(value.Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).ToArray());
        public static string StripCharacters(string value, char[] chars) => chars.Aggregate(value, StripCharacter);
        public static string StripCharacter(string value, char _char) => new(value.Where(c => c != _char).ToArray());
        public static string Shorten(string value, int delete) => value[..^delete];
        public static string AddNumberToString(string value, int number=1)
        {
            var valueStripped = value.Split('_');
            if (int.TryParse(valueStripped[^1], out var daNumber))
            {
                return value.Shorten(valueStripped[^1].Length) + (daNumber + number);
            }
            return value;
        }
        #endregion
        #region IO
        public static string ReadTextFile(string sFileName)
        {
            //Debug.Log("Reading " + sFileName);

            //Check to see if the filename specified exists, if not try adding '.txt', otherwise fail
            var sFileNameFound = "";
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
                Debug.LogError("Could not find file '" + sFileName + "'.");
                return null;
            }

            StreamReader sr;
            try
            {
                sr = new StreamReader(sFileNameFound);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Something went wrong with read.  " + e.Message);
                return null;
            }

            var fileContents = sr.ReadToEnd();
            sr.Close();
            return fileContents;
        }
        public static void WriteTextFile(string sFilePathAndName, string sTextContents)
        {
            var sw = new StreamWriter(sFilePathAndName);
            sw.WriteLine(sTextContents);
            sw.Flush();
            sw.Close();
        }
        public static bool RunSh(string path, params string[] arguments)
        {
            var terminalCommand = path;
            if (arguments != null)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    //if (i > 0) 
                        terminalCommand += " ";
                    
                    terminalCommand += arguments[i];
                }
            }
#if UNITY_EDITOR_OSX
            //Process.Start(@"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal");
            //return true;
            //Process.Start("/bin/zsh");
            //return true;
            var stderr = new StringBuilder();
            var stdout = new StringBuilder();
            var StartInfo = new ProcessStartInfo()
            {
                FileName = "/bin/zsh",
                Arguments = terminalCommand,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
                //CreateNoWindow = false,
                //WindowStyle = ProcessWindowStyle.Normal
            };
            var shellProc = new Process
            {
                StartInfo = StartInfo
            };
            using (var process = Process.Start(StartInfo))
            {
                stdout.AppendLine(process.StandardOutput.ReadToEnd());
                stderr.AppendLine(process.StandardError.ReadToEnd());
                Debug.Log(stdout.ToString());
                Debug.Log(stderr.ToString());
            }
            
            return true;
#elif UNITY_EDITOR_WIN
        Process proc = new Process();
        proc.StartInfo = new ProcessStartInfo(@"C:\Program Files\Git\git-bash.exe", terminalCommand);
        return proc.Start();
#endif
        }
        #endregion
    }
}
