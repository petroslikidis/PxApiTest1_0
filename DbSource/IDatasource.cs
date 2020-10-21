using PCAxis.Menu;
using PCAxis.Paxiom;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbSource
{
    public interface IDatasource
    {
        string DatabaseId { get; }
        IEnumerable<string> Languages { get; }
        Item GetMenu(string db, string language, string path);
        IPXModelBuilder GetBuilder(string language, string tablePath);
    }
}
