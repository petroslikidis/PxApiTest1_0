using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace PCAxis.Api.Serializers
{
    /// <summary>
    /// Interface for table response serializers
    /// </summary>
    interface IWebSerializer
    {
        byte[] Serialize(Paxiom.PXModel model);
        string ContentType  { get; }
    }
}