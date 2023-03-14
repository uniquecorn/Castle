using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Castle.Core.TimeTools
{
    public static class CastleTime
    {
        public static System.DateTime Now => simulatedTime ? simulatedCastleTime : System.DateTime.Now;
        public static bool simulatedTime;
        public static System.DateTime simulatedCastleTime;
        public static int Today => (int)Now.ToOADate();
        public static int DayInt => GetDayInt(Now);
        public static int WeekInt => GetWeekInt(Now);
        public static readonly string[] Hosts = {"pool.ntp.org", "time.nist.gov","time.google.com"};
        private const int DefaultPort = 123;
        private const int TimeoutMilliseconds = 1000;
        private const byte NTPDataLength = 48;
        private static byte[] NTPData = new byte[NTPDataLength];
        private const byte OffTransmitTimestamp = 40;
        public static System.DateTime LastKnownLegitTimeUTC;
        public static System.DateTime LegitTimeUTC => LastKnownLegitTimeUTC.Add(stopWatch?.Elapsed ?? System.TimeSpan.Zero);
        public static System.DateTime LegitTime => LegitTimeUTC.ToLocalTime();
        private static Stopwatch stopWatch;
        public static TimeState State;
        public enum TimeState
        {
            NeverFetched,
            HasLastKnown,
            Fetched
        }
        public static void SetSimulatedTime(System.DateTime simTime)
        {
            simulatedCastleTime = simTime;
            simulatedTime = true;
        }
        public static System.DateTime GetDay() => GetDay(Now);
        public static System.DateTime GetDay(System.DateTime dateTime) => dateTime.TimeOfDay.TotalHours < Settings.Instance.HourOfDayStart ? dateTime.Date.AddDays(-1) : dateTime.Date;
        public static int GetDayInt(System.DateTime dateTime) => (int)GetDay(dateTime).ToOADate();
        public static System.DateTime GetWeek() => GetWeek(Now);
        public static System.DateTime GetWeek(System.DateTime dateTime)
        {
            var day = GetDay(dateTime);
            return day.AddDays(-((int)day.DayOfWeek - 1));
        }
        public static int GetWeekInt(System.DateTime dateTime)=>(int)GetWeek(dateTime).ToOADate();
        public static async UniTask<bool> TryGetNetworkTime()
        {
            for (var i = 0; i < Hosts.Length; i++)
            {
                if (await FetchNetworkTime(Hosts[i]))
                {
                    break;
                }
            }
            return State == TimeState.Fetched;
        }
        static async UniTask<bool> FetchNetworkTime(string host)
        {
            var hostEntry = await Dns.GetHostEntryAsync(host);
            foreach (var address in hostEntry.AddressList)
            {
                if (address != null)
                {
                    var ipEndPoint = new IPEndPoint(address, DefaultPort);
                    var socket = new Socket(address.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
                    {
                        ReceiveTimeout = TimeoutMilliseconds
                    };
                    try
                    {
                        await socket.ConnectAsync(ipEndPoint);
                        if (socket.Connected)
                        {
                            NTPData[0] = 0x1B;
                            // Initialize all other fields with 0
                            for (int i = 1; i < NTPDataLength; i++)
                            {
                                NTPData[i] = 0;
                            }
                            var sentBytes = await socket.SendAsync(NTPData, SocketFlags.None);
                            Debug.Log("Sent socket bytes:" + sentBytes);
                            var receivedBytes = await socket.ReceiveAsync(NTPData, SocketFlags.None);
                            Debug.Log("Received socket bytes:" + receivedBytes);
                            LastKnownLegitTimeUTC = ComputeDate(GetMilliSeconds(OffTransmitTimestamp));
                            Debug.Log("UTC Time:" + LastKnownLegitTimeUTC);
                            Debug.Log("Local Time:"+LastKnownLegitTimeUTC.ToLocalTime());
                            if (stopWatch == null)
                            {
                                stopWatch = new Stopwatch();
                                stopWatch.Start();
                            }
                            else
                            {
                                stopWatch.Restart();
                            }
                            State = TimeState.Fetched;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(address.AddressFamily);
                        Debug.LogException(e);
                    }
                    finally
                    {
                        socket.Close();
                    }
                    if (State == TimeState.Fetched) break;
                }
            }
            return State == TimeState.Fetched;
        }
        private static System.DateTime ComputeDate(ulong milliseconds)
        {
            var span = System.TimeSpan.FromMilliseconds((double)milliseconds);
            var time = new System.DateTime(1900, 1, 1).Add(span);
            time.Add(System.TimeSpan.FromTicks(System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).Ticks));
            return time;
        }
        private static ulong GetMilliSeconds(byte offset)
        {
            ulong intpart = 0, fractpart = 0;
            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + NTPData[offset + i];
            }
            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + NTPData[offset + i];
            }
            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }
        public static void LoadLastKnownLegitTime(System.DateTime dateTime)
        {
            State = TimeState.HasLastKnown;
            LastKnownLegitTimeUTC = dateTime;
            if (stopWatch == null)
            {
                stopWatch = new Stopwatch();
                stopWatch.Start();
            }
            else
            {
                stopWatch.Restart();
            }
        }
    }
}
