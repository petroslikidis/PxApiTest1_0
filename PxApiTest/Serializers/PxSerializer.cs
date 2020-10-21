using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace PCAxis.Api.Serializers
{
    /// <summary>
    /// PX file serializer
    /// </summary>
    public class PxSerializer : IWebSerializer
    {
        #region IWebSerializer Members


        public string ContentType { get => "application/octet-stream; charset=" + System.Text.Encoding.Default.WebName; }

        public byte[] Serialize(PCAxis.Paxiom.PXModel model)
        {
            PCAxis.Paxiom.IPXModelStreamSerializer serializer = new PCAxis.Paxiom.PXFileSerializer();

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(model, stream);
                stream.Flush();
                return stream.ToArray();
            }
        }

        #endregion
    }
}