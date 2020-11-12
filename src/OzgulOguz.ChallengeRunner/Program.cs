using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OzgulOguz.Challenges;

namespace OzgulOguz.ChallengeRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            ElevatorRuntime runtime = new ElevatorRuntime();
            runtime.ExecutablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\"));

            if (args != null && args.Length > 0 && args[0] == "--run")
            {
                runtime.RunMode = ElevatorRuntime.RunModes.Competition;
                runtime.Run(new DefaultAlgorithm());
            }
            else if (args != null && args.Length > 0 && args[0] == "--rnd")
            {
                Application.EnableVisualStyles();
                runtime.RunMode = ElevatorRuntime.RunModes.Random;
                runtime.CycleDelay = 250;
                Task.Run(() => { runtime.Run(new DefaultAlgorithm()); });
                Application.Run(new Building(runtime));
            }
            else
            {
                Application.EnableVisualStyles();
                runtime.RunMode = ElevatorRuntime.RunModes.UI;
                runtime.CycleDelay = 1000;
                Task.Run(() => { runtime.Run(new DefaultAlgorithm()); });
                Application.Run(new Building(runtime));
            }
        }
    }
}
