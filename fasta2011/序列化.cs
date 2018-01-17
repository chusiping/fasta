using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace fasta2011
{
    public class ObjectToBin
    {
        public static void Serialize<T>(T o, string filePath)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, o);
                stream.Flush();
                stream.Close();
            }
            catch (Exception) { }
        }
        public static void Serialize<T>(T o)
        {
            Serialize(o, System.Environment.CurrentDirectory + "\\Para.Bin");
        }
        public static T DeSerialize<T>()
        {
            return DeSerialize<T>(System.Environment.CurrentDirectory + "\\Para.Bin");
        }

        public static T DeSerialize<T>(string filePath)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream destream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                T o = (T)formatter.Deserialize(destream);
                destream.Flush();
                destream.Close();
                return o;
            }
            catch (Exception)
            {
            }
            return default(T);
        }
    }
}
