using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace BluetoothFastConfig
{
    public partial class Form1 : Form
    {
        int TrueBAUD = 0;
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("1200");
            comboBox1.Items.Add("2400");
            comboBox1.Items.Add("4800");
            comboBox1.Items.Add("9600");
            comboBox1.Items.Add("19200");
            comboBox1.Items.Add("38400");
            comboBox1.Items.Add("57600");
            comboBox1.Items.Add("115200");
            comboBox1.Items.Add("230400");
            comboBox1.Items.Add("460800");
            comboBox1.Items.Add("921600");
            comboBox1.Items.Add("1382400");
            comboBox1.SelectedIndex = 3;

            comboBox2.Items.Add("请选择");
            comboBox2.SelectedIndex = 0;
            IsPortOpen();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.Text = "程序将尝试所有波特率";
                textBox1.Enabled = false;
            }
            else
            {
                textBox1.Text = "9600";
                textBox1.Enabled = true;
            }
        }
        private void L(String s,int color)
        {
            if (color == 0) richTextBox1.SelectionColor = Color.Black;
            if (color == 1) richTextBox1.SelectionColor = Color.Blue;
            if (color == 2) richTextBox1.SelectionColor = Color.Green;
            if (color == 3) richTextBox1.SelectionColor = Color.Red;
            richTextBox1.AppendText("[" + DateTime.Now.ToString("hh:mm:ss") + "] " + s + "\r\n");
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ScrollToCaret(); 
        }

        public bool IsPortOpen()
        {
            bool _available = false;
            SerialPort _tempPort;
            String[] Portname = SerialPort.GetPortNames();

            foreach (string str in Portname)
            {
                try
                {
                    _tempPort = new SerialPort(str);
                    _tempPort.Open();

                    if (_tempPort.IsOpen)
                    {
                        comboBox2.Items.Add(str);
                        _tempPort.Close();
                        _available = true;
                    }
                }
                catch (Exception ex)
                {
                    L("串口枚举失败，因为"+ex.Message,3);
                    _available = false;
                }
            }
            comboBox2.SelectedItem = 1;
            if (_available==false) L("没有找到串口设备或其他程序正在使用串口",3);
            L("串口枚举完成", 1);
            return _available;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsPortOpen();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int c = '\0';
            string input = "";
            if (comboBox2.SelectedItem.ToString() == "请选择")
            {
                L("你还没有选择串口",3);
                return;
            }
            if (checkBox1.Checked == false)
            {
                if (serialPort1.IsOpen) serialPort1.Close();
                try
                {
                    serialPort1.PortName = comboBox2.SelectedItem.ToString();
                    if (TrueBAUD == 0)
                    {
                        serialPort1.BaudRate = int.Parse(textBox1.Text);
                    }
                    else
                    {
                        serialPort1.BaudRate = TrueBAUD;
                        TrueBAUD = 0;
                    }
                    serialPort1.DataBits = 8;
                    serialPort1.Parity = System.IO.Ports.Parity.None;
                    serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    serialPort1.ReadTimeout = 1000;
                    serialPort1.Open();
                }
                catch (Exception ex) { L(ex.Message,3); return; }
                L("串口打开成功",0);


                L("发送:AT",1);
                try{serialPort1.Write("AT");}
                catch (Exception ex){L(ex.Message,3);return;}

                try
                {
                    input = "";
                    while(true)
                    {
                        c = serialPort1.ReadChar();
                        input = input + ((char)(c));
                    }
                }
                catch (Exception ex)
                {
                    L("收到:"+input,2);
                    if (input == "OK")
                    {
                        L("连接正常", 0);
                    }
                    else
                    {
                        L("连接失败，请检查波特率或选择不知道波特率", 3);
                        return;
                    }
                }


                L("发送:AT+NAME"+textBox3.Text,1);
                try { serialPort1.Write("AT+NAME" + textBox3.Text); }
                catch (Exception ex) { L(ex.Message,3); return; }
                try
                {
                    input = "";
                    while (true)
                    {
                        c = serialPort1.ReadChar();
                        input = input + ((char)(c));
                    }
                }
                catch (Exception ex)
                {
                    L("收到:" + input,2);
                    if (input.StartsWith("OKsetname"))
                    {
                        L("蓝牙名字设置完成", 0);
                    }
                    else
                    {
                        L("蓝牙名字设置失败", 3);
                    }
                }



                L("发送:AT+PIN"+textBox4.Text,1);
                try { serialPort1.Write("AT+PIN" + textBox4.Text); }
                catch (Exception ex) { L(ex.Message,3); return; }
                try
                {
                    input = "";
                    while (true)
                    {
                        c = serialPort1.ReadChar();
                        input = input + ((char)(c));
                    }
                }
                catch (Exception ex)
                {
                    L("收到:" + input,2);
                    if (input.StartsWith("OKsetPIN"))
                    {
                        L("配对密码设置完成", 0);
                    }
                    else
                    {
                        L("配对密码设置失败", 3);
                    }
                }


                try 
                {
                    if (comboBox1.SelectedItem.ToString() == "1200"){ string s = "AT+BAUD1"; serialPort1.Write(s); L("发送:"+s,1); }
                    if (comboBox1.SelectedItem.ToString() == "2400") { string s = "AT+BAUD2"; serialPort1.Write(s); L("发送:" + s,1); }
                    if (comboBox1.SelectedItem.ToString() == "4800") { string s = "AT+BAUD3"; serialPort1.Write(s); L("发送:" + s,1); }
                    if (comboBox1.SelectedItem.ToString() == "9600") { string s = "AT+BAUD4"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "19200") { string s = "AT+BAUD5"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "38400") { string s = "AT+BAUD6"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "57600") { string s = "AT+BAUD7"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "115200") { string s = "AT+BAUD8"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "230400") { string s = "AT+BAUD9"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "460800") { string s = "AT+BAUDA"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "921600") { string s = "AT+BAUDB"; serialPort1.Write(s); L("发送:" + s, 1); }
                    if (comboBox1.SelectedItem.ToString() == "1382400") { string s = "AT+BAUDC"; serialPort1.Write(s); L("发送:" + s, 1); }
                }
                catch (Exception ex) { L(ex.Message, 3); return; }

                try
                {
                    input = "";
                    while (true)
                    {
                        c = serialPort1.ReadChar();
                        input = input + ((char)(c));
                    }
                }
                catch (Exception ex)
                {
                    L("收到:" + input,2);
                    if (input.StartsWith("OK"))
                    {
                        L("波特率设置完成", 0);
                    }
                    else
                    {
                        L("波特率设置失败", 3);
                    }
                }
                L("全部操作完成",0);
            }
            else
            {
                int[] BAUD = new int[]{1200,2400,4800,
                                        9600,19200,38400,
                                        57600,115200,230400,
                                        460800,921600,1382400};

                if (serialPort1.IsOpen) serialPort1.Close();
                serialPort1.PortName = comboBox2.SelectedItem.ToString();
                serialPort1.DataBits = 8;
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.ReadTimeout = 1000;

                int i = 0;
                for (; i < 12; i++)
                {
                    L("尝试"+BAUD[i]+"波特率", 0);
                    try
                    {
                        if (serialPort1.IsOpen) serialPort1.Close();
                        serialPort1.BaudRate =BAUD[i];
                        serialPort1.Open();
                    }
                    catch (Exception ex) { L(ex.Message, 3); return; }

                    try { serialPort1.Write("AT"); }
                    catch (Exception ex) { L(ex.Message, 3); return; }
                    try
                    {
                        input = "";
                        while (true)
                        {
                            c = serialPort1.ReadChar();
                            input = input + ((char)(c));
                        }
                    }
                    catch (Exception ex)
                    {
                        //L("收到:" + input, 2);
                        if (input == "OK")
                        {
                            L("波特率是" + BAUD[i], 2);
                            break;
                        }
                        else
                        {
                            L("波特率不是" + BAUD[i], 3);
                        }
                    }
                }

                if (i == 12)
                {
                    L("未能找到这个蓝牙模块的波特率",3);
                }
                else
                {
                    textBox1.Text = BAUD[i] + "";
                    checkBox1.Checked = false;
                    TrueBAUD = BAUD[i];
                    button1.PerformClick();
                }

            }
            serialPort1.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            L("帮助", 0);
            L("本程序可以用来快速配置蓝牙模块，即使不知道模块的波特率。",0);
            L("在两个模块没有配对上（指示灯闪烁）时，进行配置。",3);
            L("只需要将主从模块的密码和波特率配置相同，再次上电即可自动配对。", 0);
            L("主模块不能配置名字（配置主模块名字会失败）但不影响配对",0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            L("关于", 1);
            L("版本:1.0",0);
            L("作者:James Murray",0);
            L("源代码:https://github.com/JamesMurrayBIT/BluetoothFastConfig", 0);
        }
    }

}
