using DbSource;
using Microsoft.Extensions.Options;
using PCAxis.Menu;
using PCAxis.Menu.Implementations;
using PCAxis.Paxiom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace PxDatasource
{
    public class PxFileSource : IDatasource
    {

        private readonly PxFileSourceSettings _settings;

        public PxFileSource(IOptions<PxFileSourceSettings> settingsOption)
        {
            _settings = settingsOption.Value;
        }

        public string DatabaseId => _settings.Database;

        public IEnumerable<string> Languages => _settings.Languages;

        public IPXModelBuilder GetBuilder(string language, string path)
        {
            var builder = new PCAxis.Paxiom.PXFileBuilder();
            builder.SetPath(System.IO.Path.Combine($@"wwwroot{Path.DirectorySeparatorChar}db{Path.DirectorySeparatorChar}{ DatabaseId}{Path.DirectorySeparatorChar}", path));
            return builder;
        }

        public Item GetMenu(string db, string language, string path)
        {
            string menuXMLPath = $@"wwwroot{Path.DirectorySeparatorChar}db{Path.DirectorySeparatorChar}{DatabaseId}{Path.DirectorySeparatorChar}menu.xml";
            XDocument doc = XDocument.Load(menuXMLPath);
            XmlMenu menu = new XmlMenu(doc, language,
            m => { m.AlterItemBeforeStorage = item => { item.ID.Selection = System.IO.Path.GetFileName(item.ID.Selection); }; });

            ItemSelection cid = string.IsNullOrEmpty(path) ? new ItemSelection() : new ItemSelection(System.IO.Path.GetDirectoryName(db + Path.DirectorySeparatorChar + path), System.IO.Path.GetFileName(path));
            menu.SetCurrentItemBySelection(cid.Menu, cid.Selection);

            // Check if menu level has been found or not
            if (cid.Menu != "START" && cid.Selection != "START")
            {
                if (menu.CurrentItem.ID.Menu == "" && menu.CurrentItem.ID.Selection == db)
                {
                    //Menu level has not been found in database
                    return new PxMenuItem(null);
                }
                else if (((path.ToLower().Contains(".px")) && (!path.Contains(Path.DirectorySeparatorChar.ToString()))) &&
                          ((menu.CurrentItem.ID.Menu.Contains(Path.DirectorySeparatorChar.ToString()) && menu.CurrentItem.ID.Selection == path)))
                {
                    // 1. We have a .PX-file with no path (no \) specified in nodeID
                    // 2. SetCurrentItemBySelection however has found the path to the table and put this in CurrentItem.ID.Selection

                    // We need to return an empty PxMenuItem in this case and instead find the table via the search functionality.
                    // If we do not do this we will get an error in BuildForPresentation later on because of we do not have the cotrrect path to the table...
                    return new PxMenuItem(null);
                }
            }

            return menu.CurrentItem;
        }
    }
}
