using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace FourPanelComicMaker
{
    class ConfigClass
    {
        
        /*XmlDocument xmlDoc;
        string xmlFilePath;       

        public ConfigClass(string xmlFilePath)
        {
            this.xmlFilePath = xmlFilePath;
            xmlDoc.Load(xmlFilePath);
        }

        public void Create(string name, string innerText)
        {            
            var root = xmlDoc.DocumentElement;
            XmlNode newNode = xmlDoc.CreateNode("element", name, "");
            newNode.InnerText = innerText;
            root.AppendChild(newNode);
            xmlDoc.Save(xmlFilePath);
        }

        public string getXmlValue(string key)
        {
            XmlNode node = xmlDoc.SelectSingleNode(key);
            return node.InnerText;
        }

        public string SetXmlValue(string key)
        {
            XmlNode node = xmlDoc.SelectSingleNode(key);
            node["DBID"].InnerText = DBID.ToString();   //修改值
            xmlDoc.Save("blog.xml");

        }*/
        static Configuration config = ConfigurationManager.OpenExeConfiguration(Path.GetFileName(Application.ExecutablePath));

        public static string GetValue(string key)
        {
            return config.AppSettings.Settings[key].Value;
        }
        public static void SetValue(string key, string value)
        {
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");            
        }
        
    }
}
