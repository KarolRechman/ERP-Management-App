using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NewOPAL.Models
{
    public class Table
    {
        public Table()
        {

        }

        public Table(dynamic data)
        {
            Columns = GetColumns(data[0]);
            Items = GetItems(data);
        }

        public List<Column> Columns { get; set; }
        public List<ExpandoObject> Items { get; set; }
        public TableStyles Styles { get; set; }
        public Options TableOptions { get; set; }

        public class Options
        {
            public bool HasCheckboxes { get; set; }
            public bool Hasbuttons { get; set; }
            public bool IsEditable { get; set; }
        }

        private List<Column> GetColumns(dynamic data)
        {
            List<Column> columns = new List<Column>();
            int number = 1;
            for (var i = 0; i < 1; i++)
            {
                foreach (var pair in (IDictionary<string, object>)data)
                {
                    bool hidden;
                    if (pair.Key.ToLower().Contains("id"))
                    {
                        hidden = true;
                    }
                    else
                    {
                        hidden = false;
                    }

                    Column column = new Column()
                    {
                        Id = number,
                        Name = pair.Key,
                        Hidden = hidden,
                    };
                    number = number + 1;
                    columns.Add(column);
                }
            }
            return columns;
        }

        private List<ExpandoObject> GetItems(dynamic data)
        {
            dynamic Items = new List<ExpandoObject>();
            foreach (IDictionary<string, object> row in data)
            {
                Items.Add(CreateDynamicObject(row));
            }

            return Items;
        }

        private ExpandoObject CreateDynamicObject(IDictionary<string, object> row)
        {
            dynamic obj = new ExpandoObject();
            foreach (var pair in row)
            {
                ((IDictionary<string, object>)obj)[pair.Key] = pair.Value.ToString();
            }

            return obj;
        }
    }

    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Hidden { get; set; }
    }

    public class TableStyles
    {
        public string DivClass { get; set; }
        public string TableClass { get; set; }
        public string TheadClass { get; set; }
        public string TheadTrClass { get; set; }
        public string ThClass { get; set; }
        public string TbodyClass { get; set; }
        public string TbodyTrClass { get; set; }
        public string TbodyTdClass { get; set; }
    }
}
