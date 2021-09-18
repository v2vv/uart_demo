using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private long RxCount = 0;
        SerialPort serialPort1 = new SerialPort();
        public Form1()
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
            InitializeComponent();
        }

        #region 初始化Form
        private void Form1_Load(object sender, EventArgs e)
        {
            // pictureBox1.Image = Properties.Resources.off;

            ////查询当前有用的串口号
            //SerialPort.GetPortNames();

            //string[] ports = SerialPort.GetPortNames();
            //foreach (string port in ports)
            //{
            //    cb_com.Items.Add(port);
            //}


            serialPort1.BaudRate = 115200;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = (StopBits)1;

            //迭代所有的波特率
            string[] tab_Baud = new string[] { "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200", "128000", "256000" };
            foreach (string str in tab_Baud)
            {
                comboBox2.Items.Add(str);
            }

            //迭代所有的数据位
            string[] tab_data = new string[] { "5", "6", "7", "8" };
            foreach (string str in tab_data)
            {
                comboBox3.Items.Add(str);
            }

            //迭代所有的停止位
            string[] tab_stop = new string[] { "1", "2" };
            foreach (string str in tab_stop)
            {
                comboBox4.Items.Add(str);
            }

            comboBox2.Text = "115200";
            comboBox3.Text = "8";
            comboBox4.Text = "1";

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//添加事件
            

        }
        #endregion

        #region 接收数据
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!checkBox2.Checked)//没有勾选hex时候，按照字符串方式读取
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                richTextBox1.AppendText(str);    //添加内容
                RxCount += str.Length;


            }
            else
            {
                byte data;
                data = (byte)serialPort1.ReadByte();
                string str = Convert.ToString(data, 16).ToUpper();
                richTextBox1.AppendText((str.Length == 1 ? "0" + str : str) + " ");//空位补"0"
                RxCount += str.Length;
            }
            label_ReceiveCount.Text = RxCount.ToString();
        }
        #endregion

        #region 打开/关闭串口
        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt32(comboBox3.Text);
                    serialPort1.StopBits = (StopBits)Convert.ToInt32(comboBox4.Text);
                    serialPort1.Open();
                    changeButtonTextAndPicture();

                    //string time = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
                    //richTextBox1.AppendText("-----\r\n" + "system_date：" + time + "\r\n-----");
                }
                catch
                {
                    MessageBox.Show("端口打开失败", "错误");

                }
            }
            else
            {
                try
                {
                    serialPort1.Close();
                    changeButtonTextAndPicture();
                }
                catch
                {
                    MessageBox.Show("端口关闭失败", "错误");

                }
            }

            // changeButtonTextAndPicture();

        }
        #endregion

        #region 根据串口状态切换按键名称和指示灯图片
        private void changeButtonTextAndPicture()
        {
            if (serialPort1.IsOpen)
            {
                // button1.Image = Properties.Resources.on;
                button1.Text = "关闭串口";
                comboBox1.Enabled = false;
            }
            else
            {
                //button1.Image = Properties.Resources.off;
                comboBox1.Enabled = true;
                button1.Text = "打开串口";
            }
        }
        #endregion



        #region 清空接收计数器和接收显示区域
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            RxCount = 0;
            label_ReceiveCount.Text = RxCount.ToString();
        }
        #endregion


        #region com的下拉菜单展开时候自动搜索当前设备管理器
        private void cb_com_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();       //清空
            //查询当前有用的串口号
            SerialPort.GetPortNames();

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }
        #endregion
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private void button3_Click(object sender, EventArgs e)
        {
            
            //richTextBox1.SaveFile("txtName", RichTextBoxStreamType.PlainText);
            if (this.richTextBox1.Text == "")
                return;
            //saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string FileName = this.saveFileDialog1.FileName;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && FileName.Length > 0)
            {
                // Save the contents of the RichTextBox into the file.
                richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                MessageBox.Show("文件已成功保存");
            }
        }

    }
}


// 20210918 使用数据接受函数，并增加跨线程访问
