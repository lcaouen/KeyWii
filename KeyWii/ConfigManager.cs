using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace KeyWii
{
    class ConfigManager
    {
        public List<KeyConfig> m_lstKeyConfig = new List<KeyConfig>();
        public List<String> m_lstProfileNames = new List<String>();

        public String m_strXmlFileName = "";
        public String m_strProfile = "Défaut";

        private XmlDocument m_XmlDoc = new XmlDocument();


        public ConfigManager(String in_strXmlFileName)
        {
            m_strXmlFileName = in_strXmlFileName;

            try
            {
                FileStream fs = new FileStream(m_strXmlFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_XmlDoc.Load(fs);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            
            LoadProfileNames();
        }

        public void Load(int in_nMotes)
        {
            for (int iMote = 0; iMote < in_nMotes; iMote++)
            {
                if (iMote >= m_lstKeyConfig.Count) m_lstKeyConfig.Add(new KeyConfig());
                Load(iMote, m_strProfile);
            }
        }

        public void Save()
        {
            if (m_strProfile == "") return;
            for (int iKey = 0; iKey < m_lstKeyConfig.Count; iKey++)
            {
                Save(iKey, m_strProfile);
            }
        }

        private void ReadKey(XmlNodeList in_NodeWiimote, String in_strKey, Key in_Key)
        {
            if (in_Key == null) return;
            if (in_NodeWiimote == null) return;

            in_Key.m_iKeyVal = 0;
            in_Key.m_strKeyName = "";
            in_Key.m_bKeyState = false;

            XmlNodeList nodeKey = in_NodeWiimote[0].SelectNodes(in_strKey);
            if (nodeKey == null || nodeKey.Count <= 0)
            {
                Debug.WriteLine("noeud introuvable : " + in_strKey);
                return;
            }

            XmlNodeList nodeName = nodeKey[0].SelectNodes("NAME");
            if (nodeName != null && nodeName.Count > 0)
            {
                in_Key.m_strKeyName = nodeName[0].InnerText;
            }

            XmlNodeList nodeValue = nodeKey[0].SelectNodes("VALUE");
            if (nodeValue != null && nodeValue.Count > 0)
            {
                try
                {
                    in_Key.m_iKeyVal = ushort.Parse(nodeValue[0].InnerText);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        private void WriteKey(XmlNodeList in_NodeWiimote, String in_strKey, Key in_Key)
        {
            if (in_Key == null) return;
            if (in_NodeWiimote == null) return;

            XmlElement elem = m_XmlDoc.CreateElement(in_strKey);
            in_NodeWiimote[0].AppendChild(elem);

            XmlNodeList nodeKey = in_NodeWiimote[0].SelectNodes(in_strKey);
            if (nodeKey == null || nodeKey.Count <= 0)
            {
                Debug.WriteLine("noeud introuvable : " + in_strKey);
                return;
            }

            elem = m_XmlDoc.CreateElement("NAME");
            elem.InnerText = in_Key.m_strKeyName;
            nodeKey[0].AppendChild(elem);

            elem = m_XmlDoc.CreateElement("VALUE");
            elem.InnerText = in_Key.m_iKeyVal.ToString();
            nodeKey[0].AppendChild(elem);
        }

        private void Load(int in_iMote, String in_strProfile)
        {
            try
            {
                XmlNode root = m_XmlDoc.DocumentElement;
                String strPath = "//WIIMOTECONFIG/PROFILE[@Name='" + in_strProfile + "']";
                XmlNodeList xmlnodes = root.SelectNodes(strPath);
                if (xmlnodes == null || xmlnodes.Count <= 0)
                {
                    Debug.WriteLine("noeud introuvable : " + strPath);
                    return;
                }

                strPath = "WiiMote" + in_iMote.ToString();
                XmlNodeList nodeWiiMote = xmlnodes[0].SelectNodes(strPath);
                if (nodeWiiMote == null || nodeWiiMote.Count <= 0)
                {
                    Debug.WriteLine("noeud introuvable : " + strPath);
                    return;
                }

                KeyConfig keyConfig = m_lstKeyConfig[in_iMote];
                XmlElement elem = (XmlElement)nodeWiiMote[0];
                String strMouse = elem.GetAttribute("Mouse");
                if (strMouse != null && strMouse != "")
                    keyConfig.bMouse = bool.Parse(strMouse);
                String strJoy = elem.GetAttribute("Joystick");
                if (strJoy != null && strJoy != "")
                    keyConfig.bJoystick = bool.Parse(strJoy);
                ReadKey(nodeWiiMote, "A", keyConfig.A);
                ReadKey(nodeWiiMote, "B", keyConfig.B);
                ReadKey(nodeWiiMote, "Minus", keyConfig.Minus);
                ReadKey(nodeWiiMote, "Home", keyConfig.Home);
                ReadKey(nodeWiiMote, "Plus", keyConfig.Plus);
                ReadKey(nodeWiiMote, "One", keyConfig.One);
                ReadKey(nodeWiiMote, "Up", keyConfig.Up);
                ReadKey(nodeWiiMote, "Down", keyConfig.Down);
                ReadKey(nodeWiiMote, "Left", keyConfig.Left);
                ReadKey(nodeWiiMote, "Right", keyConfig.Right);
                ReadKey(nodeWiiMote, "C", keyConfig.C);
                ReadKey(nodeWiiMote, "Z", keyConfig.Z);
                ReadKey(nodeWiiMote, "NunchukUp", keyConfig.NunchukUp);
                ReadKey(nodeWiiMote, "NunchukDown", keyConfig.NunchukDown);
                ReadKey(nodeWiiMote, "NunchukLeft", keyConfig.NunchukLeft);
                ReadKey(nodeWiiMote, "NunchukRight", keyConfig.NunchukRight);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void Save(int in_iMote, String in_strProfile)
        {
            XmlNode root = m_XmlDoc.DocumentElement;
            XmlElement elem = null;
            XmlAttribute attribute = null;

            if (root == null)
            {
                m_XmlDoc.LoadXml("<WIIMOTECONFIG></WIIMOTECONFIG>");
                root = m_XmlDoc.DocumentElement;
            }

            String strPath = "//WIIMOTECONFIG/PROFILE[@Name='" + in_strProfile + "']";
            XmlNodeList xmlnodes = root.SelectNodes(strPath);
            if (xmlnodes == null || xmlnodes.Count <= 0)
            {
                elem = m_XmlDoc.CreateElement("PROFILE");
                
                attribute = m_XmlDoc.CreateAttribute("Name");
                attribute.InnerText = in_strProfile;
                elem.Attributes.Append(attribute);
                root.AppendChild(elem);

                xmlnodes = root.SelectNodes(strPath);
                if (xmlnodes == null || xmlnodes.Count <= 0) return;
            }

            strPath = "WiiMote" + in_iMote.ToString();
            XmlNodeList nodeWiiMote = xmlnodes[0].SelectNodes(strPath);
            if (nodeWiiMote != null && nodeWiiMote.Count > 0)
            {
                nodeWiiMote[0].RemoveAll();
            }
            else
            {
                elem = m_XmlDoc.CreateElement(strPath);
                xmlnodes[0].AppendChild(elem);
                nodeWiiMote = xmlnodes[0].SelectNodes(strPath);
                if (nodeWiiMote == null || nodeWiiMote.Count <= 0) return;
            }

            KeyConfig keyConfig = m_lstKeyConfig[in_iMote];

            attribute = m_XmlDoc.CreateAttribute("Mouse");
            attribute.InnerText = keyConfig.bMouse.ToString();
            nodeWiiMote[0].Attributes.Append(attribute);

            attribute = m_XmlDoc.CreateAttribute("Joystick");
            attribute.InnerText = keyConfig.bJoystick.ToString();
            nodeWiiMote[0].Attributes.Append(attribute);

            WriteKey(nodeWiiMote, "A", keyConfig.A);
            WriteKey(nodeWiiMote, "B", keyConfig.B);
            WriteKey(nodeWiiMote, "Minus", keyConfig.Minus);
            WriteKey(nodeWiiMote, "Home", keyConfig.Home);
            WriteKey(nodeWiiMote, "Plus", keyConfig.Plus);
            WriteKey(nodeWiiMote, "One", keyConfig.One);
            WriteKey(nodeWiiMote, "Up", keyConfig.Up);
            WriteKey(nodeWiiMote, "Down", keyConfig.Down);
            WriteKey(nodeWiiMote, "Left", keyConfig.Left);
            WriteKey(nodeWiiMote, "Right", keyConfig.Right);
            WriteKey(nodeWiiMote, "C", keyConfig.C);
            WriteKey(nodeWiiMote, "Z", keyConfig.Z);
            WriteKey(nodeWiiMote, "NunchukUp", keyConfig.NunchukUp);
            WriteKey(nodeWiiMote, "NunchukDown", keyConfig.NunchukDown);
            WriteKey(nodeWiiMote, "NunchukLeft", keyConfig.NunchukLeft);
            WriteKey(nodeWiiMote, "NunchukRight", keyConfig.NunchukRight);

            m_XmlDoc.Save(m_strXmlFileName);
        }

        private void LoadProfileNames()
        {
            XmlNode root = m_XmlDoc.DocumentElement;
            if (root == null) return;
            
            String strPath = "//WIIMOTECONFIG/PROFILE";
            XmlNodeList xmlNodes = root.SelectNodes(strPath);
            if (xmlNodes == null) return;
            int nNodes = xmlNodes.Count;
            for (int iNode = 0; iNode < nNodes; iNode++)
            {
                XmlElement elem = (XmlElement)xmlNodes[iNode];
                String strProfile = elem.GetAttribute("Name");
                if (strProfile != null && strProfile != "")
                    m_lstProfileNames.Add(strProfile);
            }
        }
    }
}
