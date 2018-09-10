using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Data_Inspector.Models
{

    public class LoadViewModel
    {
        [Required]
        [Display(Name = "File")]
        public string File { get; set; }

        public string DetectFileType(string headerrow)
        {
            bool csv;
            string filetype;

            //does the header row have commas?                                
            int commacount = 0;
            foreach (char c in headerrow)
                if (c == ',') commacount++;
            //does the header row have text qualifiers? i.e. "
            int qualifiercount = 0;
            foreach (char c in headerrow)
                if (c == '"') qualifiercount++;
            //work out if it's a csv structure
            if (2 * commacount == qualifiercount || 2 * commacount + 2 == qualifiercount)
            {
                csv = true;
                // this is for csv ","
               // seperator = "\",\"";
            }
            else if (qualifiercount == 0 && commacount > 0)
            {
                csv = true;
                // this is for csv ,
               // seperator = ",";
            }
            else
            {
                csv = false;
                // this is for csv ,
               // seperator = ",";
            }

            if (csv == true)
            {
                filetype = "csv";
            } else
            {
                filetype = "notvalid";
            }

            return filetype;
        }

        public char DetectDelimeter(string source)
        {
            char delimiter = '?';
            int commasCounter = 0;
            int semicolonCounter = 0;
            int colonCounter = 0;

            foreach (char c in source)
            {
                if (c == ',')
                    commasCounter++;

                else if (c == ';')
                    semicolonCounter++;

                else if (c == ':')
                    colonCounter++;
            }

            DelimiterObject[] array =
                {
                    new DelimiterObject { Name = "commas", value = commasCounter, delimiter = ','},
                    new DelimiterObject { Name = "semicolon", value = semicolonCounter, delimiter = ';'},
                    new DelimiterObject { Name = "colon", value = colonCounter, delimiter = ':'}
                };

            int highest = 0;

            foreach (DelimiterObject c in array)
            {
                if (c.value > highest)
                {
                    highest = c.value;
                    delimiter = c.delimiter;
                }
            }

            return delimiter;

        }

        public string GenerateCreateTableSql(string[] fields, string loadid)
        {
            string sql;

            sql = "CREATE TABLE table_load_" + loadid + " (";
            foreach (string item in fields)
            {
                string field = item.Replace("\"", "");
                sql = sql + " " + field + " nvarchar(max),";
            }
            sql = sql + ");";


            return sql;
        }

        public string GenerateInsertInToTableSql(string[] data, string loadid)
        {
            string sql;

            sql = "INSERT INTO table_load_" + loadid + " VALUES (";
            foreach (string item in data)
            {
                string row = item.Replace("\"", "");
                sql = sql + "'" + row + "',";
            }
            sql = sql.TrimEnd(',');
            sql = sql + ");";


            return sql;
        }

    }

    


}
