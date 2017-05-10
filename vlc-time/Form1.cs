using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;

namespace vlc_time
{
    public partial class Form1 : Form
    {
        public string URL = "";
        public int remain;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label7.Text = "";
        }

 
        private void button1_Click(object sender, EventArgs e)
        {
            URL = @"http://" + textBox1.Text.Trim() + ":" + textBox2.Text.Trim() + "/requests/status.xml";
            //MessageBox.Show(URL);
            timer1.Enabled = true;
            timer2.Enabled = true;
            tabControl1.SelectTab(1);
            if(textBox4.Text.Trim().Length > 0) this.Text = textBox4.Text.Trim();
            timer1.Interval = Convert.ToInt32(numericUpDown1.Value) * 1000;
        }

        void getRemain()
        {
            string responseText = "";
            var encoding = ASCIIEncoding.ASCII;
            try {
                WebRequest request = WebRequest.Create(URL);
                var credential = Convert.ToBase64String(Encoding.Default.GetBytes(":" + textBox3.Text.Trim()));
                request.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                //request.Credentials = GetCredential();
                //request.PreAuthenticate = true;
                request.ContentType = "application/xml";
                WebResponse response = request.GetResponse();
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                    //MessageBox.Show(responseText);
                }
            } catch
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                writeLED(0);
                label7.Text = "";
                MessageBox.Show("ERROR: VLC Not Running?");
                tabControl1.SelectTab(0);
            }
            if (responseText.Length > 16)
            {
                //MessageBox.Show(responseText);
                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(responseText);
                    XmlNodeList elemList = xml.GetElementsByTagName("state");
                    string state = elemList[0].InnerXml.ToString();
                    label7.Text = state;
                    if (state == "paused" || state == "stopped") timer2.Enabled = false;
                    else timer2.Enabled = true;

                    elemList = xml.GetElementsByTagName("time");
                    int time = Convert.ToInt32(elemList[0].InnerXml);
                    elemList = xml.GetElementsByTagName("length");
                    int lenght = Convert.ToInt32(elemList[0].InnerXml);
                    remain = lenght - time;
                }
                catch
                {
                    writeLED(0);
                    timer1.Enabled = false;
                    timer2.Enabled = false;
                    MessageBox.Show("ERROR Loading XML");
                    tabControl1.SelectTab(0);
                }
            } else
            {
                timer2.Enabled = false;
                label7.Text = "";
            }
        }

        private void writeLED(int num)
        {
            if(num > 356400)
            {
                MessageBox.Show("Error: Remain to big! (> 99h)");
            } else
            {
                string tmp = num.ToString();
                TimeSpan t = TimeSpan.FromSeconds(num);
                string min = string.Format("{1:D2}",t.Hours,t.Minutes,t.Seconds);
                string sec = string.Format("{2:D2}", t.Hours, t.Minutes, t.Seconds);
                sevenSegment1.Value = min.Substring(0, 1);
                sevenSegment2.Value = min.Substring(1, 1);
                sevenSegment3.Value = sec.Substring(0, 1);
                sevenSegment4.Value = sec.Substring(1, 1);
                // change color
                if( num < 30)
                {
                    sevenSegment1.ColorLight = Color.Red;
                    sevenSegment2.ColorLight = Color.Red;
                    sevenSegment3.ColorLight = Color.Red;
                    sevenSegment4.ColorLight = Color.Red;
                    label5.ForeColor = Color.Black;
                } else if( num < 60 )
                {
                    sevenSegment1.ColorLight = Color.OrangeRed;
                    sevenSegment2.ColorLight = Color.OrangeRed;
                    sevenSegment3.ColorLight = Color.OrangeRed;
                    sevenSegment4.ColorLight = Color.OrangeRed;
                    label5.ForeColor = Color.Black;
                } else if(num < 120)
                {
                    sevenSegment1.ColorLight = Color.Orange;
                    sevenSegment2.ColorLight = Color.Orange;
                    sevenSegment3.ColorLight = Color.Orange;
                    sevenSegment4.ColorLight = Color.Orange;
                    label5.ForeColor = Color.Orange;
                } else
                {
                    sevenSegment1.ColorLight = Color.GreenYellow;
                    sevenSegment2.ColorLight = Color.GreenYellow;
                    sevenSegment3.ColorLight = Color.GreenYellow;
                    sevenSegment4.ColorLight = Color.GreenYellow;
                    label5.ForeColor = Color.GreenYellow;
                }
                // gray out unused
                if (sevenSegment1.Value == "0")
                {
                    sevenSegment1.ColorLight = Color.Black;
                    if (sevenSegment2.Value == "0")
                    {
                        sevenSegment2.ColorLight = Color.Black;
                        if (sevenSegment3.Value == "0") sevenSegment3.ColorLight = Color.Black;
                    }
                }
                
                
 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getRemain();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void ConfChanges(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            writeLED(0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Update Timer
            remain = remain - 1;
            if (remain < 0) remain = 0;
            writeLED(remain);
         }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/solariz/vlcLEDtimer/");
        }
    }
}
