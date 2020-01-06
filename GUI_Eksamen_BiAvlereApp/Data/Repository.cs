using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUI_Eksamen_BiAvlereApp.Classes;

namespace GUI_Eksamen_BiAvlereApp.Data
{
    class Repository
    {
        internal static void ReadFile(string fileName, out ObservableCollection<VarroaCount> varroacounts)
        {
            // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<VarroaCount>));
            TextReader reader = new StreamReader(fileName);
            // Deserialize all the VarroaCounts.
            varroacounts = (ObservableCollection<VarroaCount>)serializer.Deserialize(reader);
            reader.Close();
        }

        internal static void SaveFile(string fileName, ObservableCollection<VarroaCount> varroacounts)
        {
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<VarroaCount>));
            TextWriter writer = new StreamWriter(fileName);
            // Serialize all the VarroaCounts.
            serializer.Serialize(writer, varroacounts);
            writer.Close();
        }
    }
}
