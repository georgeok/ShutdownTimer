using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Form1 : Form
    {
        private DateTime shutdownDate = DateTime.Now;
        private bool timerRunning = false;
        private double totalSeconds = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_shutdown_Click(object sender, EventArgs e)
        {
            startTurnOff();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            disableControls(false);
            timerRunning = false;
            updateRemainingLabel();
            this.numericUpDown1.Focus();
        }

        private void disableControls(bool yesNo)
        {
            btn_shutdown.Enabled = !yesNo;
            numericUpDown1.Enabled = !yesNo;
        }

        private void startTurnOff()
        {
            Properties.Settings.Default.LastTimer = Convert.ToInt32(numericUpDown1.Value);
            Properties.Settings.Default.Save();
            disableControls(true);
            double minutes = Convert.ToDouble(this.numericUpDown1.Value);
            this.shutdownDate = DateTime.Now.Add(TimeSpan.FromMinutes(minutes));
            timerRunning = true;
            totalSeconds = this.shutdownDate.Subtract(DateTime.Now).TotalSeconds;
        }

        private void stopTurnOff()
        {
            timerRunning = false;
            // Standby
            //Application.SetSuspendState(PowerState.Suspend, true, true);
            Process.Start("shutdown", "/s /t 0");  // shutdown
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Text = "Shutdown Timer - " + DateTime.Now.ToString("HH:mm:ss");
            if (timerRunning)
            {
                var diff = this.shutdownDate.Subtract(DateTime.Now);
                this.lbl_remaining.Text = String.Format("{0:0} min {1:0} sec",
                    Math.Floor(diff.TotalSeconds / 60),
                    diff.TotalSeconds % 60
                    );
                toolStripProgressBar1.Value = (int)((totalSeconds - diff.TotalSeconds) / totalSeconds * 100);
                if (diff.TotalSeconds < 1)
                {
                    stopTurnOff();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1000;
            timer.Start();
            numericUpDown1.Value = Properties.Settings.Default.LastTimer;
            updateRemainingLabel();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            updateRemainingLabel();
        }

        private void updateRemainingLabel()
        {
            double minutes = Convert.ToDouble(this.numericUpDown1.Value);
            this.lbl_remaining.Text = String.Format("{0:0} min {1:0} sec", minutes, 0);
            toolStripProgressBar1.Value = 100;
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                startTurnOff();
            }
        }
    }
}
