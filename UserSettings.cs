﻿using System.Globalization;
using System.IO;
using System.Xml;

namespace MusicBeePlugin
{
    internal class UserSettings
    {
        public static string SettingsFilePath { get; set; }
        public static string SettingsFileName { get; set; }
        public static int ListeningPort { get; set; }
        private const string PortNumber = "portNumber";

        /// <summary>
        /// Writes an XML node.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="name">Name of the node.</param>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
        private static void WriteNodeValue(XmlDocument document, string name, string value)
        {
            XmlNode node = document.SelectSingleNode("//" + name);
            if (node == null)
            {
                XmlElement pattern = document.CreateElement(name);
                XmlNode root = document.DocumentElement;
                pattern.InnerText = value;
                if (root != null) root.AppendChild(pattern);
            }
            else
            {
                node.InnerText = value;
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <remarks></remarks>
        public static void SaveSettings(string application)
        {
            if (!Directory.Exists(SettingsFileName + "mb_remote"))
                Directory.CreateDirectory(SettingsFilePath + "mb_remote");
            if (!File.Exists(SettingsFilePath + SettingsFileName))
                CreateEmptySettingsFile(application);
            XmlDocument document = new XmlDocument();
            document.Load(SettingsFilePath + SettingsFileName);
            WriteApplicationSetting(document);
            document.Save(SettingsFilePath + SettingsFileName);
        }

        private static void CreateEmptySettingsFile(string application)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
            XmlElement root = document.CreateElement(application);
            document.InsertBefore(declaration, document.DocumentElement);
            document.AppendChild(root);
            document.Save(SettingsFilePath + SettingsFileName);
        }

        private static void WriteApplicationSetting(XmlDocument document)
        {
            if (ListeningPort > 0 && ListeningPort < 65535)
            WriteNodeValue(document,PortNumber,ListeningPort.ToString(CultureInfo.InvariantCulture));
        }

        private static string ReadNodeValue(XmlNode document, string name)
        {
            XmlNode node = document.SelectSingleNode("//" + name);
            return node != null ? node.InnerText : string.Empty;
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <remarks></remarks>
        public static void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath + SettingsFileName))
            {
                ListeningPort = 3000;
            }
            else
            {
                XmlDocument document = new XmlDocument();
                document.Load(SettingsFilePath + SettingsFileName);
                int listeningPort;
                ListeningPort = int.TryParse(ReadNodeValue(document, PortNumber), out listeningPort) ? listeningPort : 3000;
            }
        }
    }
}