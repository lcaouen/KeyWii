using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using WiimoteLib;
using Hardware;

namespace KeyWii
{
    public partial class FormKeyWii : Form
    {
        const float m_fNunchukDelta = 0.1F;

        private bool m_bClose = false;

        private HardwareEmulator m_HE = null;
        private ConfigManager m_configManager = null;

        private List<Wiimote> m_lstMotes = new List<Wiimote>();

        private Graphics m_graphic = null;
        private Bitmap m_bitmap = new Bitmap(256, 192, PixelFormat.Format24bppRgb);

       
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void UpdateExtensionChangedDelegate(WiimoteExtensionChangedEventArgs args);


        public FormKeyWii()
        {
            InitializeComponent();
        }

        private void FormKeyWii_Load(object sender, EventArgs e)
        {
            String path = Environment.GetCommandLineArgs()[0];
            String strDirectory = path.Substring(0, path.LastIndexOf('\\')) + "\\";
            m_configManager = new ConfigManager(strDirectory + "KeyWii.xml");

            m_graphic = Graphics.FromImage(m_bitmap);

            Wiimote.GetConnectedWiimotes(m_lstMotes);

            if (m_lstMotes.Count > 0)
            {
                m_lstMotes[0].WiimoteChanged += new WiimoteChangedEventHandler(wm_WiimoteChanged0);
                m_lstMotes[0].WiimoteExtensionChanged += new WiimoteExtensionChangedEventHandler(wm_WiimoteExtensionChanged0);
            }
            if (m_lstMotes.Count > 1)
            {
                m_lstMotes[1].WiimoteChanged += new WiimoteChangedEventHandler(wm_WiimoteChanged1);
                m_lstMotes[1].WiimoteExtensionChanged += new WiimoteExtensionChangedEventHandler(wm_WiimoteExtensionChanged1);
            }
            if (m_lstMotes.Count > 2)
            {
                m_lstMotes[2].WiimoteChanged += new WiimoteChangedEventHandler(wm_WiimoteChanged2);
                m_lstMotes[2].WiimoteExtensionChanged += new WiimoteExtensionChangedEventHandler(wm_WiimoteExtensionChanged2);
            }
            if (m_lstMotes.Count > 3)
            {
                m_lstMotes[3].WiimoteChanged += new WiimoteChangedEventHandler(wm_WiimoteChanged3);
                m_lstMotes[3].WiimoteExtensionChanged += new WiimoteExtensionChangedEventHandler(wm_WiimoteExtensionChanged3);
            }

            for (int iMote = 0; iMote < m_lstMotes.Count; iMote++)
            {
                comboBoxWiimotes.Items.Add("Wiimote" + iMote.ToString());
                comboBoxMotes.Items.Add("Wiimote" + iMote.ToString());
                m_lstMotes[iMote].SetReportType(Wiimote.InputReport.IRAccel, true);
                m_lstMotes[iMote].SetLEDs(iMote == 0 ? true : false, iMote == 1 ? true : false, iMote == 2 ? true : false, iMote == 3 ? true : false);
            }

            m_HE = new HardwareEmulator(m_lstMotes.Count);
            UpdateProfiles();

            if (comboBoxProfiles.Items.Count > 0) comboBoxProfiles.SelectedIndex = 0;
            if (comboBoxWiimotes.Items.Count > 0) comboBoxWiimotes.SelectedIndex = 0;
            if (comboBoxMotes.Items.Count > 0) comboBoxMotes.SelectedIndex = 0;
        }

        private void FormKeyWii_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown)
            {
                if (m_bClose)
                {
                    e.Cancel = false;
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                    e.Cancel = true;
                }
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
            }

        }

        private void configuerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                tabControl.SelectTab(0);
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                tabControl.SelectTab(1);
            }
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_configManager.Save();

            if (m_lstMotes != null)
            {
                for (int iMote = 0; iMote < m_lstMotes.Count; iMote++)
                {
                    m_lstMotes[iMote].Disconnect();
                }
            }

            m_bClose = true;
            this.Close();
        }

        // Wiimote0
        private void wm_WiimoteChanged0(object sender, WiimoteChangedEventArgs args)
        {
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteState0), args);
        }

        private void wm_WiimoteExtensionChanged0(object sender, WiimoteExtensionChangedEventArgs args)
        {
            BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged0), args);
        }

        private void UpdateWiimoteState0(WiimoteChangedEventArgs args)
        {
            SetWiimoteState(0, args.WiimoteState);
        }

        private void UpdateExtensionChanged0(WiimoteExtensionChangedEventArgs args)
        {
            chkExtension.Text = args.ExtensionType.ToString();
            chkExtension.Checked = args.Inserted;

            SetExtensionChanged(0, args.Inserted);
        }

        // Wiimote1
        private void wm_WiimoteChanged1(object sender, WiimoteChangedEventArgs args)
        {
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteState1), args);
        }

        private void wm_WiimoteExtensionChanged1(object sender, WiimoteExtensionChangedEventArgs args)
        {
            BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged1), args);
        }

        private void UpdateWiimoteState1(WiimoteChangedEventArgs args)
        {
            SetWiimoteState(1, args.WiimoteState);
        }

        private void UpdateExtensionChanged1(WiimoteExtensionChangedEventArgs args)
        {
            chkExtension.Text = args.ExtensionType.ToString();
            chkExtension.Checked = args.Inserted;

            SetExtensionChanged(1, args.Inserted);
        }

        // Wiimote2
        private void wm_WiimoteChanged2(object sender, WiimoteChangedEventArgs args)
        {
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteState2), args);
        }

        private void wm_WiimoteExtensionChanged2(object sender, WiimoteExtensionChangedEventArgs args)
        {
            BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged2), args);
        }

        private void UpdateWiimoteState2(WiimoteChangedEventArgs args)
        {
            SetWiimoteState(2, args.WiimoteState);
        }

        private void UpdateExtensionChanged2(WiimoteExtensionChangedEventArgs args)
        {
            chkExtension.Text = args.ExtensionType.ToString();
            chkExtension.Checked = args.Inserted;

            SetExtensionChanged(2, args.Inserted);
        }

        // Wiimote3
        private void wm_WiimoteChanged3(object sender, WiimoteChangedEventArgs args)
        {
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteState3), args);
        }

        private void wm_WiimoteExtensionChanged3(object sender, WiimoteExtensionChangedEventArgs args)
        {
            BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged3), args);
        }

        private void UpdateWiimoteState3(WiimoteChangedEventArgs args)
        {
            SetWiimoteState(3, args.WiimoteState);
        }

        private void UpdateExtensionChanged3(WiimoteExtensionChangedEventArgs args)
        {
            chkExtension.Text = args.ExtensionType.ToString();
            chkExtension.Checked = args.Inserted;

            SetExtensionChanged(3, args.Inserted);
        }

        private void SetExtensionChanged(int in_iMote, bool in_bInserted)
        {
            if (m_lstMotes == null) return;
            if (in_iMote < 0 || in_iMote >= m_lstMotes.Count) return;

            if (in_bInserted)
                m_lstMotes[in_iMote].SetReportType(Wiimote.InputReport.IRExtensionAccel, true);
            else
                m_lstMotes[in_iMote].SetReportType(Wiimote.InputReport.IRAccel, true);
        }

        private void SetWiimoteState(int in_iMote, WiimoteState in_ws)
        {
            if (this.WindowState == FormWindowState.Normal && tabControl.SelectedIndex == 1)
            {
                if (in_iMote != comboBoxWiimotes.SelectedIndex) return;

                clbButtons.SetItemChecked(0, in_ws.ButtonState.A);
                clbButtons.SetItemChecked(1, in_ws.ButtonState.B);
                clbButtons.SetItemChecked(2, in_ws.ButtonState.Minus);
                clbButtons.SetItemChecked(3, in_ws.ButtonState.Home);
                clbButtons.SetItemChecked(4, in_ws.ButtonState.Plus);
                clbButtons.SetItemChecked(5, in_ws.ButtonState.One);
                clbButtons.SetItemChecked(6, in_ws.ButtonState.Two);
                clbButtons.SetItemChecked(7, in_ws.ButtonState.Up);
                clbButtons.SetItemChecked(8, in_ws.ButtonState.Down);
                clbButtons.SetItemChecked(9, in_ws.ButtonState.Left);
                clbButtons.SetItemChecked(10, in_ws.ButtonState.Right);
                clbButtons.SetItemChecked(11, in_ws.NunchukState.C);
                clbButtons.SetItemChecked(12, in_ws.NunchukState.Z);

                lblX.Text = in_ws.AccelState.X.ToString();
                lblY.Text = in_ws.AccelState.Y.ToString();
                lblZ.Text = in_ws.AccelState.Z.ToString();

                if (in_ws.ExtensionType == ExtensionType.Nunchuk)
                {
                    lblChukX.Text = in_ws.NunchukState.AccelState.X.ToString();
                    lblChukY.Text = in_ws.NunchukState.AccelState.Y.ToString();
                    lblChukZ.Text = in_ws.NunchukState.AccelState.Z.ToString();

                    lblChukJoyX.Text = in_ws.NunchukState.X.ToString();
                    lblChukJoyY.Text = in_ws.NunchukState.Y.ToString();
                }

                if (in_ws.IRState.Found1)
                {
                    lblIR1.Text = in_ws.IRState.X1.ToString() + ", " + in_ws.IRState.Y1.ToString() + ", " + in_ws.IRState.Size1;
                    lblIR1Raw.Text = in_ws.IRState.RawX1.ToString() + ", " + in_ws.IRState.RawY1.ToString();
                }
                if (in_ws.IRState.Found2)
                {
                    lblIR2.Text = in_ws.IRState.X2.ToString() + ", " + in_ws.IRState.Y2.ToString() + ", " + in_ws.IRState.Size2;
                    lblIR2Raw.Text = in_ws.IRState.RawX2.ToString() + ", " + in_ws.IRState.RawY2.ToString();
                }
                if (in_ws.IRState.Found3)
                {
                    lblIR3.Text = in_ws.IRState.X3.ToString() + ", " + in_ws.IRState.Y3.ToString() + ", " + in_ws.IRState.Size3;
                    lblIR3Raw.Text = in_ws.IRState.RawX3.ToString() + ", " + in_ws.IRState.RawY3.ToString();
                }
                if (in_ws.IRState.Found4)
                {
                    lblIR4.Text = in_ws.IRState.X4.ToString() + ", " + in_ws.IRState.Y4.ToString() + ", " + in_ws.IRState.Size4;
                    lblIR4Raw.Text = in_ws.IRState.RawX4.ToString() + ", " + in_ws.IRState.RawY4.ToString();
                }

                chkFound1.Checked = in_ws.IRState.Found1;
                chkFound2.Checked = in_ws.IRState.Found2;
                chkFound3.Checked = in_ws.IRState.Found3;
                chkFound4.Checked = in_ws.IRState.Found4;

                pbBattery.Value = (in_ws.Battery > 0xc8 ? 0xc8 : (int)in_ws.Battery);
                float f = (((100.0f * 48.0f * (float)(in_ws.Battery / 48.0f))) / 192.0f);
                lblBattery.Text = f.ToString("F") + "%";

                m_graphic.Clear(Color.Black);
                if (in_ws.IRState.Found1)
                    m_graphic.DrawEllipse(new Pen(Color.Red, 3), (int)(in_ws.IRState.RawX1 / 4), (int)(in_ws.IRState.RawY1 / 4), in_ws.IRState.Size1 + 1, in_ws.IRState.Size1 + 1);
                if (in_ws.IRState.Found2)
                    m_graphic.DrawEllipse(new Pen(Color.Blue, 3), (int)(in_ws.IRState.RawX2 / 4), (int)(in_ws.IRState.RawY2 / 4), in_ws.IRState.Size2 + 1, in_ws.IRState.Size2 + 1);
                if (in_ws.IRState.Found3)
                    m_graphic.DrawEllipse(new Pen(Color.Yellow, 3), (int)(in_ws.IRState.RawX3 / 4), (int)(in_ws.IRState.RawY3 / 4), in_ws.IRState.Size3 + 1, in_ws.IRState.Size3 + 1);
                if (in_ws.IRState.Found4)
                    m_graphic.DrawEllipse(new Pen(Color.Orange, 3), (int)(in_ws.IRState.RawX4 / 4), (int)(in_ws.IRState.RawY4 / 4), in_ws.IRState.Size4 + 1, in_ws.IRState.Size4 + 1);
                if (in_ws.IRState.Found1 && in_ws.IRState.Found2)
                    m_graphic.DrawEllipse(new Pen(Color.Green), (int)(in_ws.IRState.RawMidX / 4), (int)(in_ws.IRState.RawMidY / 4), 2, 2);
                pbIR.Image = m_bitmap;
            }
            else
            {
                if (in_iMote < 0 || in_iMote >= m_configManager.m_lstKeyConfig.Count) return;

                KeyConfig keyConfig = m_configManager.m_lstKeyConfig[in_iMote];

                if (keyConfig.bJoystick)
                {
                    m_HE.ResetJoy();
                    
                    m_HE.SetJoyState(keyConfig.A, in_ws.ButtonState.A);
                    m_HE.SetJoyState(keyConfig.B, in_ws.ButtonState.B);
                    m_HE.SetJoyState(keyConfig.Minus, in_ws.ButtonState.Minus);
                    m_HE.SetJoyState(keyConfig.Home, in_ws.ButtonState.Home);
                    m_HE.SetJoyState(keyConfig.Plus, in_ws.ButtonState.Plus);
                    m_HE.SetJoyState(keyConfig.One, in_ws.ButtonState.One);
                    m_HE.SetJoyState(keyConfig.Two, in_ws.ButtonState.Two);
                    m_HE.SetJoyState(keyConfig.Up, in_ws.ButtonState.Up);
                    m_HE.SetJoyState(keyConfig.Down, in_ws.ButtonState.Down);
                    m_HE.SetJoyState(keyConfig.Left, in_ws.ButtonState.Left);
                    m_HE.SetJoyState(keyConfig.Right, in_ws.ButtonState.Right);
                    m_HE.SetJoyState(keyConfig.C, in_ws.NunchukState.C);
                    m_HE.SetJoyState(keyConfig.Z, in_ws.NunchukState.Z);
                }

                else
                {
                    SendKey(keyConfig.A, in_ws.ButtonState.A);
                    SendKey(keyConfig.B, in_ws.ButtonState.B);
                    SendKey(keyConfig.Minus, in_ws.ButtonState.Minus);
                    SendKey(keyConfig.Home, in_ws.ButtonState.Home);
                    SendKey(keyConfig.Plus, in_ws.ButtonState.Plus);
                    SendKey(keyConfig.One, in_ws.ButtonState.One);
                    SendKey(keyConfig.Two, in_ws.ButtonState.Two);
                    SendKey(keyConfig.Up, in_ws.ButtonState.Up);
                    SendKey(keyConfig.Down, in_ws.ButtonState.Down);
                    SendKey(keyConfig.Left, in_ws.ButtonState.Left);
                    SendKey(keyConfig.Right, in_ws.ButtonState.Right);
                    SendKey(keyConfig.C, in_ws.NunchukState.C);
                    SendKey(keyConfig.Z, in_ws.NunchukState.Z);
                }

                int dx = 0;
                int dy = 0;
                int nFound = 0;
                if (in_ws.IRState.Found1)
                {
                    dx += (int)((in_ws.IRState.X1 - 0.5) * 50.0);
                    dy += (int)(-(in_ws.IRState.Y1 - 0.5) * 50.0);
                    nFound++;
                }
                if (in_ws.IRState.Found2)
                {
                    dx += (int)((in_ws.IRState.X2 - 0.5) * 50.0);
                    dy += (int)(-(in_ws.IRState.Y2 - 0.5) * 50.0);
                    nFound++;
                }
                if (in_ws.IRState.Found1)
                {
                    dx += (int)((in_ws.IRState.X1 - 0.5) * 50.0);
                    dy += (int)(-(in_ws.IRState.Y1 - 0.5) * 50.0);
                    nFound++;
                }
                if (in_ws.IRState.Found1)
                {
                    dx += (int)((in_ws.IRState.X1 - 0.5) * 50.0);
                    dy += (int)(-(in_ws.IRState.Y1 - 0.5) * 50.0);
                    nFound++;
                }
                if (keyConfig.bMouse == true && nFound > 0) m_HE.MoveMouse(dx / nFound, dy / nFound);

                if (in_ws.ExtensionType == ExtensionType.Nunchuk)
                {
                    if (keyConfig.bJoystick)
                    {
                        m_HE.SetJoyState(keyConfig.NunchukLeft, in_ws.NunchukState.X);
                        m_HE.SetJoyState(keyConfig.NunchukUp, -in_ws.NunchukState.Y);
                    }
                    else
                    {
                        SendKey(keyConfig.NunchukRight, in_ws.NunchukState.X > m_fNunchukDelta);
                        SendKey(keyConfig.NunchukLeft, in_ws.NunchukState.X < -m_fNunchukDelta);
                        SendKey(keyConfig.NunchukUp, in_ws.NunchukState.Y > m_fNunchukDelta);
                        SendKey(keyConfig.NunchukDown, in_ws.NunchukState.Y < -m_fNunchukDelta);
                    }
                }
                if (keyConfig.bJoystick) m_HE.SendJoy();
            }
        }

        private void SendKey(Key in_key, bool in_bState)
        {
            if (in_bState == false && in_bState == in_key.m_bKeyState) return;
            m_HE.SendKey(in_key.m_iKeyVal, in_bState);
            in_key.m_bKeyState = in_bState;
        }

        private void chkRumble_CheckedChanged(object sender, EventArgs e)
        {
            if (m_lstMotes == null) return;
            int iSel = comboBoxWiimotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_lstMotes.Count) return;
            m_lstMotes[iSel].SetRumble(chkRumble.Checked);
        }

        private void chkLED1_CheckedChanged(object sender, EventArgs e)
        {
            if (m_lstMotes == null) return;
            int iSel = comboBoxWiimotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_lstMotes.Count) return;
            m_lstMotes[iSel].SetLEDs(chkLED1.Checked, chkLED2.Checked, chkLED3.Checked, chkLED4.Checked);
        }

        private void UpdateKeyCode(Key in_key, Keys in_KeyCode, TextBox in_Textbox)
        {
            in_key.m_iKeyVal = m_HE.KeyCodeToScanCode(in_KeyCode);

            KeysConverter kc = new KeysConverter();
            in_key.m_strKeyName = kc.ConvertToString(in_KeyCode);
            in_Textbox.Text = in_key.m_strKeyName;
        }

        private void textBoxA_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].A, e.KeyCode, textBoxA);
        }

        private void textBoxB_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].B, e.KeyCode, textBoxB);
        }

        private void textBoxMoins_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Minus, e.KeyCode, textBoxMoins);
        }

        private void textBoxHome_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Home, e.KeyCode, textBoxHome);
        }

        private void textBoxPlus_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Plus, e.KeyCode, textBoxPlus);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].One, e.KeyCode, textBox1);
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Two, e.KeyCode, textBox2);
        }

        private void textBoxHaut_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Up, e.KeyCode, textBoxHaut);
        }

        private void textBoxBas_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Down, e.KeyCode, textBoxBas);
        }

        private void textBoxGauche_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Left, e.KeyCode, textBoxGauche);
        }

        private void textBoxDroite_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Right, e.KeyCode, textBoxDroite);
        }

        private void textBoxC_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].C, e.KeyCode, textBoxC);
        }

        private void textBoxZ_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].Z, e.KeyCode, textBoxZ);
        }

        private void textBoxNunchukHaut_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].NunchukUp, e.KeyCode, textBoxNunchukHaut);
        }

        private void textBoxNunchukBas_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].NunchukDown, e.KeyCode, textBoxNunchukBas);
        }

        private void textBoxNunchukGauche_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].NunchukLeft, e.KeyCode, textBoxNunchukGauche);
        }

        private void textBoxNunchukDroite_KeyUp(object sender, KeyEventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            UpdateKeyCode(m_configManager.m_lstKeyConfig[iSel].NunchukRight, e.KeyCode, textBoxNunchukDroite);
        }

        private void checkBoxMouse_CheckedChanged(object sender, EventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            m_configManager.m_lstKeyConfig[iSel].bMouse = checkBoxMouse.Checked;
        }

        private void checkBoxJoystick_CheckedChanged(object sender, EventArgs e)
        {
            int iSel = comboBoxMotes.SelectedIndex;
            if (iSel < 0 || iSel >= m_configManager.m_lstKeyConfig.Count) return;
            m_configManager.m_lstKeyConfig[iSel].bJoystick = checkBoxJoystick.Checked;
        }

        private void comboBoxMotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem itemClicked = (ToolStripMenuItem)sender;
            int iSel = comboBoxProfiles.Items.IndexOf(itemClicked.Text);
            if (iSel != -1) comboBoxProfiles.SelectedIndex = iSel;
        }

        private void comboBoxProfiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (!comboBoxProfiles.Items.Contains(comboBoxProfiles.Text))
                {
                    m_configManager.m_lstProfileNames.Add(comboBoxProfiles.Text);
                    UpdateProfiles();
                    LoadConfig();
                }
            }
        }

        private void LoadConfig()
        {
            ToolStripMenuItem menuItem = null;

            menuItem = FindMenu(m_configManager.m_strProfile);
            if (menuItem != null) menuItem.Checked = false;

            m_configManager.Save();

            m_configManager.m_strProfile = comboBoxProfiles.Text;

            menuItem = FindMenu(m_configManager.m_strProfile);
            if (menuItem != null) menuItem.Checked = true;

            m_configManager.Load(m_lstMotes.Count);
            
            UpdateKeyConfig(comboBoxMotes.SelectedIndex);
        }

        private ToolStripMenuItem FindMenu(String in_strProfile)
        {
            ToolStripMenuItem menuItem = null;
            int nMenus = profilsToolStripMenuItem.DropDownItems.Count;
            for (int iMenu = 0; iMenu < nMenus; iMenu++)
            {
                menuItem = (ToolStripMenuItem)profilsToolStripMenuItem.DropDownItems[iMenu];
                if (menuItem != null && menuItem.Text == in_strProfile) return menuItem;
            }

            return null;
        }

        private void UpdateProfiles()
        {
            profilsToolStripMenuItem.DropDownItems.Clear();
            comboBoxProfiles.Items.Clear();
            int nProfiles = m_configManager.m_lstProfileNames.Count;
            for (int iProfile = 0; iProfile < nProfiles; iProfile++)
            {
                String strProfile = m_configManager.m_lstProfileNames[iProfile];
                ToolStripMenuItem newItem = new ToolStripMenuItem(strProfile);
                if (iProfile == 0) newItem.Checked = true;
                newItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
                profilsToolStripMenuItem.DropDownItems.Add(newItem);

                comboBoxProfiles.Items.Add(strProfile);
                if (iProfile == 0) m_configManager.m_strProfile = strProfile;
            }
        }

        private void UpdateKeyConfig(int in_iMote)
        {
            if (in_iMote < 0 || in_iMote >= m_configManager.m_lstKeyConfig.Count) return;

            KeyConfig keyConfig = m_configManager.m_lstKeyConfig[in_iMote];

            textBoxA.Text = keyConfig.A.m_strKeyName;
            textBoxB.Text = keyConfig.B.m_strKeyName;
            textBoxMoins.Text = keyConfig.Minus.m_strKeyName;
            textBoxHome.Text = keyConfig.Home.m_strKeyName;
            textBoxPlus.Text = keyConfig.Plus.m_strKeyName;
            textBox1.Text = keyConfig.One.m_strKeyName;
            textBox2.Text = keyConfig.Two.m_strKeyName;
            textBoxHaut.Text = keyConfig.Up.m_strKeyName;
            textBoxBas.Text = keyConfig.Down.m_strKeyName;
            textBoxGauche.Text = keyConfig.Left.m_strKeyName;
            textBoxDroite.Text = keyConfig.Right.m_strKeyName;
            textBoxC.Text = keyConfig.C.m_strKeyName;
            textBoxZ.Text = keyConfig.Z.m_strKeyName;
            textBoxNunchukHaut.Text = keyConfig.NunchukUp.m_strKeyName;
            textBoxNunchukBas.Text = keyConfig.NunchukDown.m_strKeyName;
            textBoxNunchukGauche.Text = keyConfig.NunchukLeft.m_strKeyName;
            textBoxNunchukDroite.Text = keyConfig.NunchukRight.m_strKeyName;
            
            checkBoxMouse.Checked = keyConfig.bMouse;
            checkBoxJoystick.Checked = keyConfig.bJoystick;
        }
    }
}