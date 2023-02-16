using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vivoautotestwifi.Pages.Wifi
{
    public class WaveFormInfo
    {
        public string Argument;
        public string Rate;

        public string RatebitIndex;
        public string Wlanmode;
        public string RateBW;

        public int SleepTime;

        public bool IsMIMO;
        public bool IsLDPC;
        public string WaveformName;
    }

    public class WaveFromIC
    {
        public string ICtype;
        public bool IsLDPC;
        public List<WaveFormInfo> WaveFormInfos;
    }

    public class WiFiIQWaveForm
    {
        public static List<WaveFromIC> WaveFromICs;

        public static void LoadWaveFromInfo()
        {
            using (StreamReader sr=new StreamReader(@"MainTestQueue\WiFi\config\WaveformSetting.xml",Encoding.UTF8,true))
            {
                WaveFromICs=Class2XML.ToClass<List<WaveFromIC>>(sr.ReadToEnd());
                sr.Close();
            }
        }

        public static void SaveWaveFromInfo()
        {
            using (StreamWriter sw = new StreamWriter(@"MainTestQueue\WiFi\config\WaveformSetting.xml", false, Encoding.UTF8))
            {
                sw.WriteLine(Class2XML.ToXml(WaveFromICs));
                sw.Close();
            }
        }
    }

    /// <summary>
    /// 将实体对象转换成XML
    /// </summary>
    public static class Class2XML
    {
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj">实体对象</param>
        public static string ToXml<T>(T obj)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                XmlSerializer xmlSer = new XmlSerializer(typeof(T));
                xmlSer.Serialize(stream, obj);

                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("将实体对象转换成XML异常", ex);
            }
        }

        /// <summary>
        /// 将XML转换成实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="strXML">XML</param>
        public static T ToClass<T>(string strXML) where T : class
        {
            try
            {
                T t;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(strXML))
                {
                    t = serializer.Deserialize(sr) as T;
                    sr.Close();
                }


                return t;
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成实体对象异常", ex);
            }
        }
    }
}
