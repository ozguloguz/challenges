using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OzgulOguz.Challenges;

namespace OzgulOguz.ChallengeRunner
{
    public partial class Building : Form
    {
        private ElevatorRuntime runtime;
        private Timer timer = new Timer();
        private Image personImage = Image.FromFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\")) + "\\person.png");

        private static Random randomNumberGenerator = new Random();
        private static double RD() { return randomNumberGenerator.NextDouble(); }
        private static int RN(int n) { return randomNumberGenerator.Next(n); }

        public Building(ElevatorRuntime runtime)
        {
            this.runtime = runtime;

            InitializeComponent();
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;

            Timer t = new Timer() { Enabled = true, Interval = 100 };
            t.Tick += t_Tick;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        Point mouseDownPoint = Point.Empty;
        Point mouseCurrentPoint = Point.Empty;

        void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDownPoint = e.Location;
            }
            else
            {
                runtime.IsPaused = !runtime.IsPaused;
                pictureBox1.Invalidate();
            }
        }

        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && mouseDownPoint != Point.Empty)
            {
                mouseCurrentPoint = e.Location;
                pictureBox1.Invalidate();
            }
            else
            {
                mouseDownPoint = Point.Empty;
                mouseCurrentPoint = Point.Empty;
            }
        }

        void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            float height = pictureBox1.Height;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && mouseDownPoint != Point.Empty)
            {
                startFloor.Text = Convert.ToInt32(Math.Floor((height - mouseDownPoint.Y) / (height / runtime.NumberOfFloors))).ToString();
                targetFloor.Text = Convert.ToInt32(Math.Floor((height - e.Location.Y) / (height / runtime.NumberOfFloors))).ToString();
                weight.Text = (45 + RN(46) + RN(46)).ToString();
                button1_Click(null, null);
            }
            mouseDownPoint = Point.Empty;
            mouseCurrentPoint = Point.Empty;
        }

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            canvas.Clear(Color.White);

            for (int i = 0; i < runtime.NumberOfFloors; i++) DrawFloor(i, canvas);
        }

        private void DrawFloor(int floor, Graphics canvas)
        {
            // Floor base
            float width = pictureBox1.Width;
            float height = pictureBox1.Height;
            float floorHeight = (float)pictureBox1.Height / runtime.NumberOfFloors;
            float floorY = (height * 0.98f) - (floor * floorHeight);

            // Current floor
            if (runtime.CurrentFloor == floor)
            {
                canvas.FillRectangle(Brushes.LightGreen, 0, floorY - floorHeight + 1, width, floorHeight);
            }

            // Floor base
            canvas.DrawLine(Pens.Black, 0, floorY, width, floorY);

            // Elevator door frame
            float doorFrameWidth = width * 0.20f;
            float doorFrameHeight = floorHeight * 0.75f;

            if (runtime.CurrentFloor != floor || runtime.DoorStatus == 0)
            {
                canvas.FillRectangle(Brushes.LightGray, width * 0.9f - doorFrameWidth, floorY - doorFrameHeight, doorFrameWidth, doorFrameHeight);
                canvas.DrawLine(Pens.Black, width * 0.9f - doorFrameWidth / 2, floorY - doorFrameHeight, width * 0.9f - doorFrameWidth / 2, floorY);
            }
            else
            {
                canvas.FillRectangle(Brushes.Yellow, width * 0.9f - doorFrameWidth, floorY - doorFrameHeight, doorFrameWidth, doorFrameHeight);
                canvas.FillRectangle(Brushes.LightGray, width * 0.9f - doorFrameWidth, floorY - doorFrameHeight, width * 0.02f, doorFrameHeight);
                canvas.DrawLine(Pens.Black, width * 0.9f - doorFrameWidth + width * 0.02f + 1, floorY - doorFrameHeight, width * 0.9f - doorFrameWidth + width * 0.02f + 1, floorY);
                canvas.FillRectangle(Brushes.LightGray, width * 0.9f - width * 0.02f, floorY - doorFrameHeight, width * 0.02f, doorFrameHeight);
                canvas.DrawLine(Pens.Black, width * 0.9f - width * 0.02f, floorY - doorFrameHeight, width * 0.9f - width * 0.02f, floorY);
            }

            canvas.DrawRectangle(Pens.Black, width * 0.9f - doorFrameWidth, floorY - doorFrameHeight, doorFrameWidth, doorFrameHeight);

            // Elevator Call buttons

            Brush colorUp = (runtime.Calls[floor] & 1) == 1 ? Brushes.Red : Brushes.Gray;
            Brush colorDown = (runtime.Calls[floor] & 2) == 2 ? Brushes.Red : Brushes.Gray;

            canvas.FillRectangle(colorUp, width * 0.9f - doorFrameWidth - width * 0.04f, floorY - floorHeight / 2, width * 0.02f, width * 0.02f);
            canvas.FillRectangle(colorDown, width * 0.9f - doorFrameWidth - width * 0.04f, floorY - floorHeight / 2 + width * 0.025f, width * 0.02f, width * 0.02f);

            // Floor buttons
            Brush floorButtonColor = runtime.Gotos[floor] == 1 ? Brushes.Red : Brushes.Gray;
            canvas.FillRectangle(floorButtonColor, width * 0.93f, floorY - floorHeight / 2, width * 0.04f, width * 0.04f);

            // People in halls
            float p = 2;
            foreach (ElevatorRuntime.Person person in runtime.PeopleInHalls.ToArray())
            {
                if (person.StartFloor == floor)
                {
                    canvas.DrawImage
                    (
                        personImage,
                        width * 0.9f - doorFrameWidth - p * floorHeight * 0.3f,
                        floorY - floorHeight * 0.5f,
                        floorHeight * 0.3f,
                        floorHeight * 0.45f
                    );
                    canvas.DrawString(person.TargetFloor.ToString(), Font, Brushes.Black, width * 0.9f - doorFrameWidth - p * floorHeight * 0.26f, floorY - floorHeight * 0.8f, StringFormat.GenericDefault);

                    p++;
                }
            }

            float fullWidth = pictureBox1.Width;

            if (runtime.IsPaused)
            {
                canvas.FillRectangle(Brushes.Black, fullWidth / 2f - fullWidth * 0.15f, height / 2f - height * 0.1f, fullWidth * 0.1f, height * 0.2f);
                canvas.FillRectangle(Brushes.Black, fullWidth / 2f + fullWidth * 0.05f, height / 2f - height * 0.1f, fullWidth * 0.1f, height * 0.2f);
            }

            if (mouseDownPoint != Point.Empty)
            {
                canvas.DrawLine(Pens.Red, mouseDownPoint, mouseCurrentPoint);
            }

            canvas.DrawString("Cycle: " + runtime.CurrentCycle, Font, Brushes.Black, 0, 0, StringFormat.GenericDefault);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int startFloorParsed = int.Parse(startFloor.Text);
                int targetFloorParsed = int.Parse(targetFloor.Text);
                int weightParsed = int.Parse(weight.Text);

                if (startFloorParsed < 0 || targetFloorParsed < 0 || startFloorParsed >= runtime.NumberOfFloors || targetFloorParsed >= runtime.NumberOfFloors)
                {
                    MessageBox.Show("Start and Target floors should be between 0 and " + (runtime.NumberOfFloors - 1));
                    return;
                }

                if (startFloorParsed == targetFloorParsed)
                {
                    MessageBox.Show("Start floor cannot be the same as target floor");
                    return;
                }

                runtime.PeopleInHalls.Add
                (
                    new ElevatorRuntime.Person()
                    {
                        StartFloor = startFloorParsed,
                        TargetFloor = targetFloorParsed,
                        Weight = weightParsed
                    }
                );

                // runtime.Calls[startFloorParsed] |= targetFloorParsed > startFloorParsed ? 1 : 2;

                startFloor.Text = targetFloor.Text = weight.Text = "";
                pictureBox1.Invalidate();
            }
            catch (Exception inputValidationError)
            {
                MessageBox.Show(inputValidationError.ToString());
            }
        }

    }
}
