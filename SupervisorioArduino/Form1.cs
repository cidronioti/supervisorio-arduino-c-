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

namespace SupervisorioArduino
{
    public partial class Form1 : Form
    {

        delegate void funcaoRecepcao();

        public Form1()
        {
            InitializeComponent();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            funcaoRecepcao recepcaodelegate = new funcaoRecepcao(RecepcaoSerial);
            Invoke(recepcaodelegate);
        }

        String chtxt = null, str = null;
        long tempox = 0;
        bool flagTIME = false;

        public void RecepcaoSerial() {
            chtxt += serialPort1.ReadExisting();
            textBoxSerial.Text += chtxt;
            str += chtxt;
            chtxt = null;
            //01234567
            //Texto [AtU1ON] ou [AtU1OF]
            //Texto [S10000]   o S é de sensor e o numero 1 é relativo ao sensor 1 informação vinda do arduino
            //Texto [TIMEOK]

            if (textBoxSerial.Text.Length > 776)
            {
                textBoxSerial.Clear();
            }

            if (str.Substring(0, 1).Equals("["))
            {
                if (str.Length >= 8)
                {
                    if (str.Substring(1, 1).Equals("A") &&
                       str.Substring(2, 1).Equals("t") &&
                       str.Substring(3, 1).Equals("U") &&
                       str.Substring(5, 1).Equals("O") &&
                       str.Substring(7, 1).Equals("]"))
                    {
                        if (str.Substring(6, 1).Equals("N"))
                        {
                            switch (str.Substring(4, 1))
                            {
                                case "1": pictureBoxLampada.Image = SupervisorioArduino.Properties.Resources.ligth_on;
                                    labelStatusLampada.Text = "Acesa";
                                    labelStatusLampada.BackColor = Color.Blue;
                                    break;
                                case "2": pictureBoxVentilador.Image = SupervisorioArduino.Properties.Resources.ventilador_ligado;
                                          labelStatusVentilador.Text = "Ligado";
                                          labelStatusVentilador.BackColor = Color.Blue;
                                    break;
                                case "3": pictureBoxClimatizador.Image = SupervisorioArduino.Properties.Resources.ar_condicionado_ligado;
                                          labelStatusClimatizador.Text = "Ligado";
                                          labelStatusClimatizador.BackColor = Color.Blue;
                                    break;
                            }
                        }
                        else
                        {
                            if (str.Substring(6, 1).Equals("F"))
                            {
                                switch (str.Substring(4, 1))
                                {
                                    case "1": pictureBoxLampada.Image = SupervisorioArduino.Properties.Resources.ligth_off;
                                        labelStatusLampada.Text = "Acesa";
                                        labelStatusLampada.BackColor = Color.BlanchedAlmond;
                                        break;
                                    case "2": pictureBoxVentilador.Image = SupervisorioArduino.Properties.Resources.ventilador_desligado1;
                                        labelStatusVentilador.Text = "Ligado";
                                        labelStatusVentilador.BackColor = Color.BlanchedAlmond;
                                        break;
                                    case "3": pictureBoxClimatizador.Image = SupervisorioArduino.Properties.Resources.ar_condicionado_desligado1;
                                        labelStatusClimatizador.Text = "Ligado";
                                        labelStatusClimatizador.BackColor = Color.BlanchedAlmond;
                                        break;
                                }
                            }
                        }

                        str = null;
                    }
                    else
                    {
                        if (str.Substring(1, 1).Equals("S") && str.Substring(7, 1).Equals("]"))
                        {
                            if (str.Substring(2, 1).Equals("1"))
                            {
                                labelTemperatura.Text = str.Substring(3, 4);
                                progressBarTemperatura.Value = int.Parse(str.Substring(3, 4));

                                if (chartSensores.Series[0].Points.Count > 8)
                                {
                                    chartSensores.Series[0].Points.RemoveAt(0);
                                    chartSensores.Update();
                                }

                                chartSensores.Series[0].Points.AddXY(tempox, int.Parse(str.Substring(3, 4)));


                            }
                            else
                            {
                                if (str.Substring(2, 1).Equals("2"))
                                {
                                    labelUmidade.Text = str.Substring(3, 4);
                                    progressBarUmidade.Value = int.Parse(str.Substring(3, 4));

                                    if (chartSensores.Series[1].Points.Count > 8)
                                    {
                                        chartSensores.Series[1].Points.RemoveAt(0);
                                        chartSensores.Update();
                                    }

                                    chartSensores.Series[1].Points.AddXY(tempox, int.Parse(str.Substring(3, 4)));
                                }
                            }

                            tempox++;
                            str = null;
                        }//
                        else
                        {
                            if (str.Substring(1, 1).Equals("T") &&
                                  str.Substring(2, 1).Equals("I") &&
                                  str.Substring(3, 1).Equals("M") &&
                                  str.Substring(4, 1).Equals("E") &&
                                  str.Substring(5, 1).Equals("O") &&
                                  str.Substring(6, 1).Equals("K") &&
                                  str.Substring(7, 1).Equals("]"))
                            {
                                flagTIME = false;
                                str = null;
                            }
                        }
                    }

                    str = null;
                }
            }
            else
            {
                str = null;
            }
        }

        private void buttonSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Conf_port
            String[] valorPort = SerialPort.GetPortNames();
            for (int i = 0; i < valorPort.Length; i++ ) {
                comboBoxPorta.Items.Add(valorPort[i]);
            }
            comboBoxPorta.Text = "COM0";
            #endregion

            #region Conf_Baud
            int[] valorBaud = { 2400, 4800, 9600, 19200, 57600, 115200};
            for (int i = 0; i < valorBaud.Length; i++) {
                comboBoxBaud.Items.Add(valorBaud[i]);
            }
            comboBoxBaud.Text = "9600";
            #endregion

            #region Conf_Data
            comboBoxData.Items.Add("7");
            comboBoxData.Items.Add("8");
            comboBoxData.Text = "8";
            #endregion

            #region Conf_Stop
            comboBoxStop.Items.Add("None");
            comboBoxStop.Items.Add("One");
            comboBoxStop.Items.Add("two");
            comboBoxStop.Text = "One";
            #endregion

            #region Conf_Parity
            comboBoxParity.Items.Add("NONE");
            comboBoxParity.Items.Add("EVEN");
            comboBoxParity.Items.Add("000");
            comboBoxParity.Items.Add("MARK");
            comboBoxParity.Items.Add("SPACE");
            comboBoxParity.Text = "NONE";
            #endregion
        }

        private void buttonConecta_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
            else {
                serialPort1.PortName = comboBoxPorta.Text;
                serialPort1.BaudRate = int.Parse(comboBoxBaud.Text);
                serialPort1.DataBits = int.Parse(comboBoxData.Text);
                serialPort1.StopBits = (StopBits)(comboBoxStop.SelectedIndex);
                serialPort1.Parity = (Parity)(comboBoxParity.SelectedIndex);
            }
            try
            {
                serialPort1.Open();

                buttonConecta.Enabled = false;
                buttonDesconecta.Enabled = true;
                comboBoxBaud.Enabled = false;
                comboBoxData.Enabled = false;
                comboBoxParity.Enabled = false;
                comboBoxPorta.Enabled = false;
                comboBoxStop.Enabled = false;
                buttonSair.Enabled = false;
                labelMsg.Text = "Conectado";
                labelMsg.ForeColor = Color.Green;
                pictureBox1.Image = SupervisorioArduino.Properties.Resources.connect;
            }catch{
                MessageBox.Show("Falha na Comunicação Serial!");
            }
        }

        private void buttonDesconecta_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();

                buttonConecta.Enabled = true;
                buttonDesconecta.Enabled = false;
                comboBoxBaud.Enabled = true;
                comboBoxData.Enabled = true;
                comboBoxParity.Enabled = true;
                comboBoxPorta.Enabled = true;
                comboBoxStop.Enabled = true;
                buttonSair.Enabled = true;
                labelMsg.Text = "Conectado";
                labelMsg.ForeColor = Color.Black;
                pictureBox1.Image = SupervisorioArduino.Properties.Resources.disconnect;
            }
            catch
            {
                MessageBox.Show("Falha na Comunicação Serial!");
            }
        }

        private void buttonEnviar_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                if (checkBox1.Checked)
                {
                    serialPort1.Write(textBoxEnviar.Text + "\r");
                }
                else
                {
                    serialPort1.Write(textBoxEnviar.Text);
                }

                textBoxEnviar.Text = null;
            }
            else
            {
                MessageBox.Show("Erro de comunicação com a porta!!");
            }
        }

        private void buttonLimpar_Click(object sender, EventArgs e)
        {
            textBoxSerial.Text = null;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
