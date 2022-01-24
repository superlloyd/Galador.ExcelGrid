using NReco.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrid.GridModelExtensions
{
    partial class GridModel
    {
        public void ToCsv(StringWriter writer)
        {
            var cw = new CsvWriter(writer);
            foreach (var column in this.Columns)
                cw.WriteField(column);
            cw.NextRecord();
            foreach (var row in rows)
            {
                for (int i = 0; i < Columns.Count; i++)
                    cw.WriteField(row[i]);
                cw.NextRecord();
            }
        }
        public string ToCsv()
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
                ToCsv(writer);
            return sb.ToString();
        }

        public static GridModel FromCsv(StringReader reader)
        {
            var model = new GridModel();
            model.InitializeFromCsv(reader, false);
            return model;
        }
        public static GridModel FromCsv(string csv) => FromCsv(new StringReader(csv));

        public void InitializeFromCsv(StringReader reader) => InitializeFromCsv(reader, true);
        public void InitializeFromCsv(string csv) => InitializeFromCsv(new StringReader(csv), true);

        private void InitializeFromCsv(StringReader reader, bool clear)
        {
            if (clear)
            {
                Clear();
                Columns.Clear();
            }

            var cr = new CsvReader(reader);
            bool first = true;
            while (cr.Read())
            {
                if (first)
                {
                    first = false;
                    for (int i = 0; i < cr.FieldsCount; i++)
                        this.Columns.Add(cr[i]);
                }
                else
                {
                    var row = this.AddRow();
                    for (int i = 0; i < cr.FieldsCount && i < this.Columns.Count; i++)
                        row[i] = cr[i];
                }
            }
        }
    }
}
