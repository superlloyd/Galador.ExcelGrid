using NReco.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galador.WPF.ExcelGrid
{
    partial class ExcelModel
    {
        public void ToCsv(StringWriter writer)
        {
            var cw = new CsvWriter(writer);
            foreach (var row in rows)
            {
                for (int i = 0; i < ColumnCount; i++)
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

        public static ExcelModel FromCsv(StringReader reader)
        {
            var model = new ExcelModel();
            model.InitializeFromCsv(reader, false);
            return model;
        }
        public static ExcelModel FromCsv(string csv) => FromCsv(new StringReader(csv));

        public void InitializeFromCsv(StringReader reader) => InitializeFromCsv(reader, true);
        public void InitializeFromCsv(string csv) => InitializeFromCsv(new StringReader(csv), true);

        private void InitializeFromCsv(StringReader reader, bool clear)
        {
            if (clear)
                Clear();

            var cr = new CsvReader(reader);
            var first = true;
            while (cr.Read())
            {
                if (first)
                {
                    first = false;
                    ColumnCount = cr.FieldsCount;
                }
                var row = this.AddRow();
                for (int i = 0; i < cr.FieldsCount && i < this.ColumnCount; i++)
                    row.Set(i, cr[i]);
            }
        }
    }
}
