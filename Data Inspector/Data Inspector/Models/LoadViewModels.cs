using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Data;

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

            sql = "CREATE TABLE table_load_" + loadid + " (DIRowID uniqueidentifier not null,";

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

            sql = "INSERT INTO table_load_" + loadid + " VALUES (newid(),";
            foreach (string item in data)
            {
                string row = item.Replace("\"", "");
                sql = sql + "'" + row + "',";
            }
            sql = sql.TrimEnd(',');
            sql = sql + ");";


            return sql;
        }

        public void SetupColumnsDataTypes(string[] fields, string loadid)
        {
            string fieldsql;
            Type dataType;
            string sqlDataType;
            List<string>sqlDataTypeList = new List<string>();

            foreach (string item in fields)
            {
                string field = item.Replace("\"", "");

                //identify datatype using function GetColumnType() 
                fieldsql = "SELECT " + field + " FROM table_load_" + loadid + " WHERE " + field + " IS NOT NULL";
                using (var newTableCtx = new LoadedFiles())
                {
                    var values = newTableCtx.Database.SqlQuery<string>(fieldsql).ToList();
                    //building sqlDataTypeList based on function GetColumnType() results
                    dataType = GetColumnType(values);
                    if (dataType.ToString() == "System.String")
                    {
                        sqlDataType = "varchar (max)";
                    }
                    else if (dataType.ToString() == "System.Boolean")
                    {
                        sqlDataType = "bit";
                    }
                    else if (dataType.ToString() == "System.Int32")
                    {
                        sqlDataType = "int";
                    }
                    else if (dataType.ToString() == "System.Int64")
                    {
                        sqlDataType = "bigint";
                    }
                    else if (dataType.ToString() == "System.Double")
                    {
                        sqlDataType = "float (50)";
                    }
                    else if (dataType.ToString() == "System.DateTime")
                    {
                        sqlDataType = "datetime";
                    }
                    else
                    {
                        sqlDataType = "nvarchar (max)";
                    }

                    sqlDataTypeList.Add(sqlDataType);

                }             
                
            }
            // setting appropriate Data Types for each column
            for (int x=0; x<sqlDataTypeList.Count; x++)
            {
                using (var newTableCtx = new LoadedFiles())
                {   //unifying Date Format (yyyy-mm-dd) to be able to alter column and set as "datetime" data type. If column has invalid date it will be set as '1753-01-01' to pointed out wrong value and differentiate from not populated(NULL) - we must somehow catch this and reported 
                    if (sqlDataTypeList[x] == "datetime")
                    {
                    int unifyDates = newTableCtx.Database.ExecuteSqlCommand("Update table_load_" + loadid + " Set " + fields[x] + " = COALESCE( TRY_CONVERT(DATE, " + fields[x] + ", 103), TRY_CONVERT(DATE, " + fields[x] + ", 102), TRY_CONVERT(DATE, " + fields[x] + ", 101), TRY_CONVERT(DATE, '1753-01-01', 102));");
                    }

                    int alterColumnsDataType = newTableCtx.Database.ExecuteSqlCommand("ALTER TABLE table_load_" + loadid + " ALTER COLUMN "+fields[x]+" "+sqlDataTypeList[x]+";");
                }
            }
   
        }

        enum dataType
        {
            System_String = 0, // varchar
            System_Boolean = 1, // bit
            System_Int32 = 2, // int
            System_Int64 = 3, // bigint
            System_Double = 4, // float
            System_DateTime = 5 // datetime
            //above mappings have been done acordingly to "SQL Server Data Type Mappings"  https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings

        }

        private dataType ParseString(string str)
        {

            bool boolValue;
            Int32 intValue;
            Int64 bigintValue;
            double doubleValue;
            DateTime dateValue;
            

            // Place checks higher in if-else statement to give higher priority to type.

            if (bool.TryParse(str, out boolValue))
                return dataType.System_Boolean;
            else if (Int32.TryParse(str, out intValue))
                return dataType.System_Int32;
            else if (Int64.TryParse(str, out bigintValue))
                return dataType.System_Int64;
            else if (double.TryParse(str, out doubleValue))
                return dataType.System_Double;
            else if (DateTime.TryParse(str, out dateValue))
                return dataType.System_DateTime;
            else return dataType.System_String;

        }

        /// <summary>
        /// Gets the datatype for the Datacolumn column
        /// </summary>
        /// <returns></returns>
        public Type GetColumnType(List<string> values)
        {
            Type T;

            int colSize;

            //get smallest and largest values
            string strMinValue = values.Min();
            int minValueLevel = (int)ParseString(strMinValue);

            string strMaxValue = values.Max();
            int maxValueLevel = (int)ParseString(strMaxValue);
            colSize = strMaxValue.Length;

            //get max typelevel of first n to 50 rows
            int sampleSize = values.Count(); //Math.Max(values.Count(), 50);
            int maxLevel = Math.Max(minValueLevel, maxValueLevel);

            for (int i = 0; i < sampleSize; i++)
            {
                //maxLevel = Math.Max((int)ParseString(values[i].ToString()), maxLevel);
                maxLevel = Math.Max((int)ParseString(values[i].ToString()), maxLevel);
            }

            string enumCheck = ((dataType)maxLevel).ToString();
            T = Type.GetType(enumCheck.Replace('_', '.'));

         

            //if typelevel = int32 check for bit only data & cast to bool
            if (maxLevel == 1 && Convert.ToInt32(strMinValue) == 0 && Convert.ToInt32(strMaxValue) == 1)
            {
                T = Type.GetType("System.Boolean");
            }

            if (maxLevel != 5) colSize = -1;


            return T;
        }

    }


}

