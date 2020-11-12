using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OzgulOguz.Challenges
{
    public delegate void CycleDelegate(ElevatorRuntime runtime);

    public class ElevatorRuntime
    {
        public event EventHandler Cycle;

        public int NumberOfFloors = 6;
        public int LoadCapacity = 300;
        public int NumberOfCycles = 1000000;

        public List<Person> PeopleInHalls = new List<Person>();
        public List<Person> PeopleOnBoard = new List<Person>();
        public List<Person> PeopleServed = new List<Person>();
        public int[] Calls; // 0: both inactive, 1: up active, 2: down active, 3: both active
        public int[] Gotos; // 1: active, 0: inactive
        public int DoorStatus = 0;
        public int Direction = 0; // -1: down, 0: stationary, 1: up
        public int IntendedDirection = 0; // -1: down, 0: stationary, 1: up
        public int CurrentFloor = 0;
        public int CurrentLoad = 0;
        public int FloorsTravelled = 0;
        public int DoorOpenCloses = 0;
        public int CycleDelay = 0;
        public int CurrentCycle = 0;
        public string ExecutablePath;

        public enum RunModes
        {
            Competition,
            Random,
            UI
        }

        public RunModes RunMode = RunModes.UI;

        private bool isPaused = false;

        public class Person
        {
            public int CycleCreated;
            public int CycleGotOn;
            public int CycleGotOff;

            public int StartFloor;
            public int TargetFloor;

            public int Weight;
        }

        private static Random randomNumberGenerator = new Random();
        private static double RD() { return randomNumberGenerator.NextDouble(); }
        private static int RN(int n) { return randomNumberGenerator.Next(n); }

        private static void Fail(string message)
        {
            throw new Exception(message);
        }

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        public void Run(DefaultAlgorithm algorithm)
        {
            StreamReader inputFileReader = null;
            string inputFileLine = null;
            int inputFileCycle = 0;

            if (RunMode == RunModes.Competition)
            {
                string inputFileName = ExecutablePath + "\\_" + NumberOfFloors + "_" + LoadCapacity + "_input.txt";
                if (!File.Exists(inputFileName))
                {
                    Fail("input file " + inputFileName + " not found");
                }
                inputFileReader = new StreamReader(inputFileName);
                inputFileLine = inputFileReader.ReadLine();
                inputFileCycle = int.Parse(inputFileLine.Split(' ')[0]);
            }

            Calls = new int[NumberOfFloors];
            Gotos = new int[NumberOfFloors];

            int topFloor = NumberOfFloors - 1;
            int groundFloor = 0;

            Input input = new Input() { WriteLog = RunMode != RunModes.Competition };
            Output output = new Output() { WriteLog = RunMode != RunModes.Competition };

            Task.Run(() => { algorithm.Run(input, output); });

            input.WriteLine("F#:" + NumberOfFloors + " L#:" + LoadCapacity);

            while (CurrentCycle++ < NumberOfCycles)
            {
                while (isPaused) Thread.Sleep(10);

                if (Cycle != null) Cycle(this, null);

                if (CurrentCycle % 10000 == 0) Console.Write(".");
                string action = output.ReadLine();

                switch (action)
                {
                    case "ST": // -> Stop. Elevator is going up
                        if (Direction == 0) Fail("ST while stationary");
                        Direction = 0;
                        break;
                    case "G+": // -> Go up
                        if (Direction != 0) Fail("G+ while moving");
                        if (DoorStatus == 1) Fail("G+ while door is open");
                        if (CurrentFloor == topFloor) Fail("G+ while on top floor");
                        Direction = 1;
                        break;
                    case "G-": // -> Go down
                        if (Direction != 0) Fail("G- while moving");
                        if (DoorStatus == 1) Fail("G- while door is open");
                        if (CurrentFloor == groundFloor) Fail("G- while on ground floor");
                        Direction = -1;
                        break;
                    case "O+": // -> Open door, elevator is going up
                        if (Direction != 0) Fail("O+ while moving");
                        if (DoorStatus == 1) Fail("O+ while door is open");
                        if (CurrentFloor == topFloor) Fail("O+ while on top floor");
                        DoorStatus = 1;
                        IntendedDirection = 1;
                        DoorOpenCloses++;
                        break;
                    case "O-": // -> Open door, elevator is going down
                        if (Direction != 0) Fail("O- while moving");
                        if (DoorStatus == 1) Fail("O- while door is open");
                        DoorStatus = 1;
                        IntendedDirection = 2;
                        DoorOpenCloses++;
                        break;
                    case "O.": // -> Open door, elevator is going down
                        if (Direction != 0) Fail("O. while moving");
                        if (DoorStatus == 1) Fail("O. while door is open");
                        if (Calls[CurrentFloor] != 0) Fail("O. while at least one call button is activated");
                        DoorStatus = 1;
                        IntendedDirection = 0;
                        DoorOpenCloses++;
                        break;
                    case "CL": // Close door
                        if (Direction != 0) Fail("CL while moving");
                        if (DoorStatus != 1) Fail("CL while door is open");
                        DoorStatus = 0;
                        DoorOpenCloses++;

                        break;
                    case "--": // -> No action
                        break;
                    default:
                        Fail("Unknown action " + action);
                        break;
                }

                if (RunMode != RunModes.Competition)
                {
                    Console.Write("Calls: ");
                    foreach (int callState in Calls) Console.Write("{0}", callState);
                    Console.Write(" Gotos: ");
                    foreach (int gotoState in Gotos) Console.Write("{0}", gotoState);
                    Console.WriteLine();
                }

                if (RunMode == RunModes.Random)
                {
                    // Add a new person with 10% probability
                    if (RD() < 0.10d)
                    {
                        Person person = new Person();
                        person.CycleCreated = CurrentCycle;
                        person.StartFloor = RN(NumberOfFloors);
                        person.TargetFloor = RN(NumberOfFloors);
                        while (person.TargetFloor == person.StartFloor) person.TargetFloor = RN(NumberOfFloors);
                        person.Weight = 45 + RN(46) + RN(46);
                        PeopleInHalls.Add(person);
                        //Console.WriteLine("{0} {1} {2} {3}", currentCycle, person.StartFloor, person.TargetFloor, person.Weight);
                    }
                }
                else if (RunMode == RunModes.Competition && inputFileCycle == CurrentCycle)
                {
                    Person person = new Person();
                    string[] inputFileInfo = inputFileLine.Split(' ');
                    person.CycleCreated = int.Parse(inputFileInfo[0]);
                    person.StartFloor = int.Parse(inputFileInfo[1]);
                    person.TargetFloor = int.Parse(inputFileInfo[2]);
                    person.Weight = int.Parse(inputFileInfo[3]);
                    PeopleInHalls.Add(person);
                    inputFileLine = inputFileReader.ReadLine();
                    if (inputFileLine != "")
                    {
                        inputFileCycle = int.Parse(inputFileLine.Split(' ')[0]);
                    }
                }

                // Check compulsory actions
                if (Direction == -1 && CurrentFloor == groundFloor) Fail(action + " while going down and on ground floor.");
                if (Direction == 1 && CurrentFloor == topFloor) Fail(action + " while going up and on top floor.");

                StringBuilder eventMessages = new StringBuilder();

                // move the elevator
                CurrentFloor += Direction;

                if (Direction != 0)
                {
                    eventMessages.AppendFormat(" FL:{0}", CurrentFloor);
                    FloorsTravelled++;
                }

                // deactivate buttons
                if (DoorStatus == 1)
                {
                    if (IntendedDirection != 0)
                    {
                        string upOrDown = IntendedDirection == 1 ? "U" : "D";

                        if ((Calls[CurrentFloor] & IntendedDirection) > 0)
                        {
                            Calls[CurrentFloor] -= IntendedDirection;
                            eventMessages.AppendFormat(" {0}-:{1}", upOrDown, CurrentFloor);
                        }
                    }
                    if (Gotos[CurrentFloor] == 1)
                    {
                        eventMessages.AppendFormat(" G-:{0}", CurrentFloor);
                        Gotos[CurrentFloor] = 0;
                    }
                }

                List<Person> peopleToMove = new List<Person>();

                // Let people out if on desired floor and the door is open
                if (DoorStatus == 1)
                {
                    foreach (Person person in PeopleOnBoard)
                    {
                        if (person.TargetFloor == CurrentFloor)
                        {
                            peopleToMove.Add(person);
                        }
                    }
                    foreach (Person person in peopleToMove)
                    {
                        person.CycleGotOff = CurrentCycle;
                        PeopleOnBoard.Remove(person);
                        PeopleServed.Add(person);
                        eventMessages.AppendFormat(" L-:{0}", person.Weight);
                    }
                    peopleToMove.Clear();
                }

                // Let people in if on called floor and the door is open
                if (DoorStatus == 1)
                {
                    foreach (Person person in PeopleInHalls)
                    {
                        if (person.StartFloor == CurrentFloor)
                        {
                            int personIntendedDirection = person.TargetFloor > person.StartFloor ? 1 : 2;
                            if (personIntendedDirection == IntendedDirection)
                            {
                                if (CurrentLoad + person.Weight <= LoadCapacity)
                                {
                                    peopleToMove.Add(person);
                                }
                            }
                        }
                    }
                    foreach (Person person in peopleToMove)
                    {
                        person.CycleGotOn = CurrentCycle;
                        PeopleInHalls.Remove(person);
                        PeopleOnBoard.Add(person);
                        eventMessages.AppendFormat(" L+:{0}", person.Weight);
                    }
                    peopleToMove.Clear();
                }

                // Let people in the halls call the elevator
                foreach (Person person in PeopleInHalls)
                {
                    int upOrDown = person.TargetFloor > person.StartFloor ? 1 : 2;
                    if ((Calls[person.StartFloor] & upOrDown) == 0)
                    {
                        eventMessages.AppendFormat(" {0}+:{1}", upOrDown == 1 ? "U" : "D", person.StartFloor);
                        Calls[person.StartFloor] |= upOrDown;
                    }
                }

                // Let people on board press the button for the desired floor 
                foreach (Person person in PeopleOnBoard)
                {
                    if ((Gotos[person.TargetFloor]) == 0)
                    {
                        eventMessages.AppendFormat(" G+:{0}", person.TargetFloor);
                        Gotos[person.TargetFloor] = 1;
                    }
                }

                if (CycleDelay > 0)
                {
                    Thread.Sleep(CycleDelay);
                }
                if (eventMessages.Length == 0) eventMessages.Append("--:0");

                input.WriteLine(eventMessages.ToString().TrimStart());
            }

            if (RunMode == RunModes.Competition)
            {
                Report(inputFileReader);
            }

            if (inputFileReader != null)
            {
                inputFileReader.Close();
                inputFileReader.Dispose();
            }

        }

        public void Report(StreamReader inputFileReader)
        {
            double avgWaitTime = 0;
            foreach (Person person in PeopleServed)
            {
                avgWaitTime += person.CycleGotOn - person.CycleCreated;
            }
            avgWaitTime /= PeopleServed.Count;

            double avgExtendedTravelTime = 0;
            foreach (Person person in PeopleServed)
            {
                // -1 because of the door opening on the target floor
                avgExtendedTravelTime += (person.CycleGotOff - person.CycleGotOn - 1) - Math.Abs(person.TargetFloor - person.StartFloor);
            }
            avgExtendedTravelTime /= PeopleServed.Count;

            int originalFloorsTravelled = int.Parse(inputFileReader.ReadLine());
            int originalDoorOpenCloses = int.Parse(inputFileReader.ReadLine());
            int originalPeopleServed = int.Parse(inputFileReader.ReadLine());
            double originalAvgWaitTime = double.Parse(inputFileReader.ReadLine(), CultureInfo.InvariantCulture);
            double originalAvgExtendedTravelTime = double.Parse(inputFileReader.ReadLine(), CultureInfo.InvariantCulture);
            int originalEnergyExpenditure = int.Parse(inputFileReader.ReadLine());

            int energyExpenditure = FloorsTravelled * 5 + DoorOpenCloses * 1;

            Console.WriteLine(
@"
                              ORIGINAL ALG.    YOUR ALGORITHM   IMPROVEMENT
----------------------------  ---------------  ---------------  -----------
Floors Travelled            : {0,15}{6,15}{12,11}
Door Open/Close             : {1,15}{7,15}{13,11}
People Served               : {2,15}{8,15}{14,11}
Avg Wait Time               : {3,15:0.0000}{9,15:0.0000}{15,11:0.0000}
Avg Extended Travel Time    : {4,15:0.0000}{10,15:0.0000}{16,11:0.0000}
Energy Expenditure          : {5,15}{11,15}{17,11}",
                    originalFloorsTravelled,
                    originalDoorOpenCloses,
                    originalPeopleServed,
                    originalAvgWaitTime,
                    originalAvgExtendedTravelTime,
                    originalEnergyExpenditure,
                    FloorsTravelled,
                    DoorOpenCloses,
                    PeopleServed.Count,
                    avgWaitTime,
                    avgExtendedTravelTime,
                    energyExpenditure,
                    FloorsTravelled - originalFloorsTravelled,
                    DoorOpenCloses - originalDoorOpenCloses,
                    PeopleServed.Count - originalPeopleServed,
                    originalAvgWaitTime - avgWaitTime,
                    originalAvgExtendedTravelTime - avgExtendedTravelTime,
                    originalEnergyExpenditure - energyExpenditure
             );
        }
    }
}
