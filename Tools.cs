using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Castle
{
    public static class Tools
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
        public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        public static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        public static readonly int MainTex = Shader.PropertyToID("_MainTex");
        public static readonly int MainTexST = Shader.PropertyToID("_MainTex_ST");
        private static MaterialPropertyBlock block;
        public static MaterialPropertyBlock Block
        {
            get
            {
                if (block == null) block = new MaterialPropertyBlock();
                return block;
            }
        }
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
        public static bool TryGetUniqueID(out string uniqueID, System.Func<string, bool> checkDuplicateID)
        {
            var id = System.Guid.NewGuid().ToString();

            var duplicateId = checkDuplicateID(id);
            var iteration = 0;
            while (duplicateId && iteration < 128)
            {
                id = System.Guid.NewGuid().ToString();
                duplicateId = checkDuplicateID(id);
                iteration++;
            }

            uniqueID = id;
            return !duplicateId;
        }
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
            var te = new TextEditor {text = s};
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

        public static string Slug(string s) => StripWSP(s).ToLower();
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
            var sFileNameFound = "";
            if (File.Exists(sFileName))
            {
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
                Debug.LogException(e);
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
        public static void StoreLongPlayerPrefs(string key, long value) => Store64BitValue(key,System.BitConverter.GetBytes(value));
        public static void StoreDoublePlayerPrefs(string key, double value) => Store64BitValue(key,System.BitConverter.GetBytes(value));
        public static bool FetchLongPlayerPrefs(string key, out double value)
        {
            if (Fetch64BitValue(key, out var bytes))
            {
                value = System.BitConverter.ToInt64(bytes, 0);
                return true;
            }
            value = default;
            return false;
        }
        public static bool FetchDoublePlayerPrefs(string key, out double value)
        {
            if (Fetch64BitValue(key, out var bytes))
            {
                value = System.BitConverter.ToDouble(bytes, 0);
                return true;
            }
            value = default;
            return false;
        }
        public static void Store64BitValue(string key, byte[] bytes)
        {
            PlayerPrefs.SetInt(key + "_low", System.BitConverter.ToInt32(bytes, 0));
            PlayerPrefs.SetInt(key + "_high", System.BitConverter.ToInt32(bytes, 4));
        }
        public static bool Fetch64BitValue(string key, out byte[] bytes)
        {
            if (PlayerPrefs.HasKey(key + "_low") && PlayerPrefs.HasKey(key + "_high"))
            {
                bytes = new byte[8];
                System.Array.Copy(System.BitConverter.GetBytes(PlayerPrefs.GetInt(key + "_low")), bytes, 4);
                System.Array.Copy(System.BitConverter.GetBytes(PlayerPrefs.GetInt(key + "_high")), 0, bytes, 4,
                    4);
                return true;
            }
            bytes = default;
            return false;
        }
        #endregion
        #region Rendering
        public static void SetColor(Color color, params Renderer[] renderers)
        {
            renderers.SetProperty(propertyBlock => propertyBlock.SetColor(BaseColor, color));
        }
        public static void ApplyMaterialProperties(System.Action<MaterialPropertyBlock> property,
            params Renderer[] renderers)
        {
            renderers.SetProperty(property);
        }
        #endregion
        #region Editor
        #if UNITY_EDITOR
        public static string EscapeCharsQuote(string input)
        {
            input = FileUtil.GetPhysicalPath(input);
            if (input.IndexOf('"') == -1)
                return "\"" + input + "\"";
            return input.IndexOf('\'') == -1 ? "'" + input + "'" : (string) null;
        }
        public static System.Type GetTypeFromProperty(SerializedProperty property)
        {
            // first, lets get the Type of component this serialized property is part of...
            var parentComponentType = property.serializedObject.targetObject.GetType();
            // ... then, using reflection well get the raw field info of the property this
            // SerializedProperty represents...
            var fieldInfo = parentComponentType.GetField(property.propertyPath);
            // ... using that we can return the raw .net type!
            return fieldInfo.FieldType;
        }
        public static bool RunSh(string path, params string[] arguments)
        {
            var terminalCommand = EscapeCharsQuote(path);
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    terminalCommand += " " + arg;
                }
            }
#if UNITY_EDITOR_OSX
            var stderr = new StringBuilder();
            var stdout = new StringBuilder();
            var StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = terminalCommand,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var shellProc = new System.Diagnostics.Process
            {
                StartInfo = StartInfo
            };
            using (var process = System.Diagnostics.Process.Start(StartInfo))
            {
                stdout.AppendLine(process.StandardOutput.ReadToEnd());
                stderr.AppendLine(process.StandardError.ReadToEnd());
                Debug.Log(stdout.ToString());
                Debug.LogError(stderr.ToString());
            }
            return true;
#elif UNITY_EDITOR_WIN
        var proc = new System.Diagnostics.Process();
        proc.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\Program Files\Git\git-bash.exe", terminalCommand);
        return proc.Start();
#else
return false;
#endif
        }
        #endif
        #endregion
    }
}