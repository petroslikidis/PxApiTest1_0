using PCAxis.Menu;
using PCAxis.Paxiom;
using PCAxis.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxApiTest
{
    public static class LegacyExtensions
    {
        /// <summary>
        /// Returns the coded type indicating that the item  t(able), h(eadline) or l(ink)/folder 
        /// </summary>
        /// <param name="menuItem">the item </param>
        /// <returns>the coded type</returns>
        public static string GetMetaListType(this Item menuItem)
        {
            if (menuItem is TableLink)
            {
                return "t";
            }
            else if (menuItem is Headline)
            {
                return "h";
            }
            else
            {
                return "l";
            }
        }

        public static IEnumerable<MetaList> GetMetaList(this PxMenuItem item)
        {
            return item.SubItems.Select(i => new MetaList
            {
                Id = i.ID.Selection.Replace('\\', '/'),
                Text = i.Text,
                Type = i.GetMetaListType(),
                Updated = i is TableLink ? (((TableLink)i).Published) : null
            });
        }

        public static MetaTable GetTableMeta(this IPXModelBuilder builder)
        {
            builder.DoNotApplyCurrentValueSet = true;  // DoNotApplyCurrentValueSet means the "client that made the request" is an api(, not a GUI) so that
                                                       // CNMM2.4 property DefaultInGUI (for Valueset/grouping) should not be used  
            builder.BuildForSelection();

            return new MetaTable
            {
                Title = builder.Model.Meta.Title,
                Variables = builder.Model.Meta.Variables.Select(variable => new MetaVariable
                {
                    Code = variable.Code,
                    Text = variable.Name,
                    Elimination = variable.Elimination,
                    Time = variable.IsTime,
                    Map = string.IsNullOrEmpty(variable.Map) ? null : variable.Map,
                    Values = variable.Values.Select(value => value.Code).ToArray(),
                    ValueTexts = variable.Values.Select(value => value.Value).ToArray()
                }).ToArray()
            };
        }

        public static List<PCAxis.Paxiom.Selection> BuildSelections(this PCAxis.Paxiom.IPXModelBuilder builder, TableQuery tableQuery)
        {
            //Check to see that the variable exists
            int c = builder.Model.Meta.Variables.Where(var => tableQuery.Query.Select(q => q.Code).Contains(var.Code)).ToArray().Length;
            if (tableQuery.Query.Length > c) throw new ArgumentException("Variable is not defined");

            var selections = new List<PCAxis.Paxiom.Selection>();
            foreach (var variable in builder.Model.Meta.Variables)
            {

                PCAxis.Paxiom.Selection selection = new PCAxis.Paxiom.Selection(variable.Code);
                var query = tableQuery.Query.SingleOrDefault(q => q.Code == variable.Code);
                if (query != null)
                {
                    // Process filters
                    if (query.Selection.Filter.ToLower() == "all") // All
                    {
                        selection = QueryHelper.SelectAll(variable, query);
                    }
                    else if (query.Selection.Filter.ToLower() == "top") // Top
                    {
                        selection = QueryHelper.SelectTop(variable, query);
                    }
                    else if (PCAxis.Query.QueryHelper.IsAggregation(query.Selection.Filter))
                    {
                        selection = QueryHelper.SelectAggregation(variable, query, builder);
                    }
                    else if (query.Selection.Filter.StartsWith("vs:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        selection = QueryHelper.SelectValueSet(variable, query, builder);
                    }
                    else
                    {
                        // Assume item
                        selection = QueryHelper.SelectItem(variable, query);
                    }
                }
                else
                {
                    // Variable not specified in query
                    if (!variable.Elimination)
                        selection = PCAxis.Paxiom.Selection.SelectAll(variable);
                }

                if (selection != null)
                {
                    selections.Add(selection);
                }
                else
                {
                    //The user as requested an non vaild selection
                    return null;
                }
            }

            if (!ValidateSelection(builder, selections)) return null;

            return selections;
        }

        public static bool ValidateSelection(PCAxis.Paxiom.IPXModelBuilder builder, List<PCAxis.Paxiom.Selection> selections)
        {
            //Check that all mandatory variables has at least one value selected

            var mandatoryVariables = builder.Model.Meta.Variables.Where(v => !v.Elimination);

            foreach (var v in mandatoryVariables)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == v.Code);
                if (selection == null) return false;
                if (selection.ValueCodes.Count == 0) return false;
            }

            return true;

        }

    }
}
