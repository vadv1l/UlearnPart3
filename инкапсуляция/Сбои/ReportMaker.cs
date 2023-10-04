using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{

    public class Common
    {
        /*public static int IsFailureSerious(int failureType)
        {
            if (failureType%2==0) return 1;
            return 0;
        }*/


        public static int Earlier(object[] v, int day, int month, int year)
        {
            int vYear = (int)v[2];
            int vMonth = (int)v[1];
            int vDay = (int)v[0];
            if (vYear < year) return 1;
            if (vYear > year) return 0;
            if (vMonth < month) return 1;
            if (vMonth > month) return 0;
            if (vDay < day) return 1;
            return 0;
        }
    }

    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {

            var problematicDevices = new HashSet<int>();
            for (int i = 0; i < failureTypes.Length; i++)
                if (Common.IsFailureSerious(failureTypes[i]) && Common.Earlier(times[i], day, month, year)==1)
                    problematicDevices.Add(deviceId[i]);

            var result = new List<string>();
            foreach (var device in devices)
                if (problematicDevices.Contains((int)device["DeviceId"]))
                    result.Add(device["Name"] as string);

            return result;
        }
        
        public class Device
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override bool Equals(object obj)
            {
                if (obj is Device device)
                {
                    return device.Id == Id && device.Name == Name;
                }
                else return false;
            }
        }
        enum FailureType
        {
            UnexpectedShutDown = 0,
            ShortNonResponding = 1,
            HardwareFailures = 2,
            ConnectionProblems = 3
        }
        public class Failure
        {
            public FailureType FailureType { get; set; }
            public bool IsFailureSerious(){return (int) FailureType%2==0; }

        }
        public static List<string> FindDevicesFailedBeforeData(DateTime data, Failure[]failures,)
        {
            
        }
           
    }
}
