using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Data;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading;


namespace Data_Inspector.Models
{

    public class LoadViewModel
    {
        [Required]
        [Display(Name = "File")]
        public string File { get; set; }
        public int perCent { get; set; }

        public int setPerCent(int value)
        {
            return this.perCent + value;
        }
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
            }
            else
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

        public string GenerateCreateTableSql(List<string> fields, string loadid)
        {
            string sql;

            sql = "CREATE TABLE table_load_" + loadid + " (DIRowID varchar(50) not null,";

            foreach (string item in fields)
            {
                //string field = item.Replace("\"", ""); commented out as this removes "" within "".The outer "" are now handled within func mySplit() in model MySplit.
                sql = sql + " [" + item + "] nvarchar(max),"; // add square brackets to prevent SQL key words from interrupting assigning datatypes if they are part of the column name.
            }
            sql = sql.TrimEnd(',');
            sql = sql + ");";


            return sql;
        }

        public string GenerateInsertInToTableSql(List<string> data, string loadid)
        {
            string sql;

            sql = "INSERT INTO table_load_" + loadid + " VALUES (newid(),";
            foreach (string item in data)
            {
                //string row = item.Replace("\"", "");  commented out as this removes "" within "". The outer "" are now handled within func mySplit() in model MySplit.
                sql = sql + "'" + item + "',";
            }
            sql = sql.TrimEnd(',');
            sql = sql + ");";


            return sql;
        }

        public void SetupColumnsDataTypes(List<string> fields, string loadid)
        {
            string fieldsql;
            Type dataType;
            string sqlDataType;
            List<string> sqlDataTypeList = new List<string>();

            foreach (string item in fields)
            {
                string field = item.Replace("\"", "");

                //identify columns datatype using function GetColumnType() based on first 50 records
                fieldsql = "SELECT TOP(50) " + field + " FROM table_load_" + loadid + " WHERE " + field + " IS NOT NULL";
                using (var newTableCtx = new LoadedFiles())
                {
                    var values = newTableCtx.Database.SqlQuery<string>(fieldsql).ToList();
                    //building sqlDataTypeList based on function GetColumnType() results
                    dataType = GetColumnType(values);
                    if (dataType.ToString() == "System.String")
                    {
                        sqlDataType = "varchar";
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
                    else if (dataType.ToString() == "System.Double") //later on is converting value back to nvarchar when consists character "," 
                    {                                                //We will have to implement a tool for the user to then define if char"," represents separator for decimal places or if
                        sqlDataType = "float";                       //is used to separate for example thousands (1,000) or millions(1,000,000)            
                    }
                    else if (dataType.ToString() == "System.DateTime")
                    {
                        sqlDataType = "datetime";
                    }
                    else
                    {
                        sqlDataType = "nvarchar";
                    }

                    sqlDataTypeList.Add(sqlDataType);

                }

            }
            // setting appropriate Data Types and Size for each column
            for (int x = 0; x < sqlDataTypeList.Count; x++)
            {
                using (var newTableCtx = new LoadedFiles())
                {
                    int alterRowIdDataType = newTableCtx.Database.ExecuteSqlCommand("ALTER TABLE table_load_" + loadid + " ALTER COLUMN DIRowID uniqueidentifier NOT NULL");
                    //unifying Date Format (yyyy-mm-dd) to be able to alter column and set as "datetime" data type. If column has invalid date it will be set as '1753-01-01' to pointed out wrong value and differentiate from not populated(NULL) - we must somehow catch this and reported 
                    if (sqlDataTypeList[x] == "datetime")
                    {
                        int unifyDates = newTableCtx.Database.ExecuteSqlCommand("Update table_load_" + loadid + " Set " + fields[x] + " = COALESCE( TRY_CONVERT(DATE, " + fields[x] + ", 103), TRY_CONVERT(DATE, " + fields[x] + ", 102), TRY_CONVERT(DATE, " + fields[x] + ", 101));");
                        int alterColumnsDataType = newTableCtx.Database.ExecuteSqlCommand("ALTER TABLE table_load_" + loadid + " ALTER COLUMN " + fields[x] + " " + sqlDataTypeList[x] + ";");
                    }
                    else
                    {
                        try
                        {
                            if (sqlDataTypeList[x] != "varchar" || sqlDataTypeList[x] != "nvarchar")
                            {
                                int alterColumnsDataType = newTableCtx.Database.ExecuteSqlCommand("ALTER TABLE table_load_" + loadid + " ALTER COLUMN " + fields[x] + " " + sqlDataTypeList[x] + ";");
                            }
                        }
                        catch
                        {
                            if (sqlDataTypeList[x] != "varchar" || sqlDataTypeList[x] != "nvarchar")
                            {
                                sqlDataTypeList[x] = "nvarchar";
                            }
                            string sql = "select max(len(" + fields[x] + ")) from table_load_" + loadid + ";";
                            var size = newTableCtx.Database.SqlQuery<Int64>(sql).ToList();
                            int alterColumnsTypeSize = newTableCtx.Database.ExecuteSqlCommand("ALTER TABLE table_load_" + loadid + " ALTER COLUMN " + fields[x] + " " + sqlDataTypeList[x] + "(" + size[0] + ");");

                        }
                    }
                }

            }

        }

        enum dataType
        {
            System_DateTime = 0, // datetime
            System_String = 1, // varchar
            System_Boolean = 2, // bit
            System_Int32 = 3, // int
            System_Int64 = 4, // bigint
            System_Double = 5 // float

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
            if (maxLevel == 3 && Convert.ToInt32(strMinValue) == 0 && Convert.ToInt32(strMaxValue) == 1)
            {
                T = Type.GetType("System.Boolean");
            }

            if (maxLevel != 5) colSize = -1;


            return T;
        }
        public void loadFile(string path, string fileName, string loadid)
        {
           // int lineCount = System.IO.File.ReadLines(path).Count();
            using (var streamReader = System.IO.File.OpenText(path))
            {

                MySplit split = new MySplit();
                string source = streamReader.ReadLine();

                //confirm filetype and detect seperator
                LoadViewModel LoadView = new LoadViewModel();
                string fileType = LoadView.DetectFileType(source);
                char seperator = LoadView.DetectDelimeter(source);                           

                //Bulk Load
                List<string> fields = split.mySplit(source, seperator);

                string sql;

                string sqlproofloadid = loadid.Replace('-', '_');

                sql = LoadView.GenerateCreateTableSql(fields, sqlproofloadid);

                string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                var Conn = new SqlConnection(ConnStr);
                var CreateTable = new SqlCommand(sql, Conn);
                Conn.Open();
                CreateTable.ExecuteNonQuery();

                var tempDataTable = new DataTable();
                tempDataTable.Columns.Add(new DataColumn("DIRowID"));
                for (int i = 0; i < fields.Count; i++)
                {
                    tempDataTable.Columns.Add(new DataColumn());
                }


                while (!streamReader.EndOfStream)
                {
                    DataRow row = tempDataTable.NewRow();
                    source = streamReader.ReadLine();

                    List<string> values = new List<string>();
                    values.Add(Guid.NewGuid().ToString());
                    values.AddRange(split.mySplit(source, seperator));
                    row.ItemArray = values.ToArray();
                    tempDataTable.Rows.Add(row);

                    //int currentProgress = tempDataTable.Rows.Count;
                    //perCent = (currentProgress / lineCount) * 100;

                    //sql = LoadView.GenerateInsertInToTableSql(values, sqlproofloadid);

                    //ViewBag.Message = sql;
                    //using (var newTableCtx = new LoadedFiles())
                    //{
                    //    int noOfRecordsInserted = newTableCtx.Database.ExecuteSqlCommand(sql);
                    //}
                }

                streamReader.Close();

                var bc = new SqlBulkCopy(Conn, SqlBulkCopyOptions.TableLock, null)
                {
                    DestinationTableName = "table_load_" + sqlproofloadid,
                    BatchSize = tempDataTable.Rows.Count
                };

                //Conn.Open();
                bc.WriteToServer(tempDataTable);
                Conn.Close();
                bc.Close();

                // identifying data types and altering table columns
                LoadView.SetupColumnsDataTypes(fields, sqlproofloadid);



            }


        }

        public string loadFileInfo(string path, string fileName, string userID)
        {
            int lineCount = System.IO.File.ReadLines(path).Count();
            using (var streamReader = System.IO.File.OpenText(path))
            {
                string source = streamReader.ReadLine();
                //confirm filetype and detect seperator
                LoadViewModel LoadView = new LoadViewModel();
                string fileType = LoadView.DetectFileType(source);
                char seperator = LoadView.DetectDelimeter(source);

                string loadid;

                LoadedFiles ctx = new LoadedFiles();
                LoadedFile loadedfile = new LoadedFile { LoadedFileID = Guid.NewGuid(), FileName = fileName, FileType = fileType, FileImportDate = DateTime.Now, UserID = userID, LineCount = lineCount };
                ctx.DBLoadedFiles.Add(loadedfile);
                ctx.SaveChanges();

                loadid = loadedfile.LoadedFileID.ToString();

                return loadid;

            }

        }

        public string filename(Guid id)
        {
            using (var context = new LoadedFiles())
            {
                // Query for all blogs with names starting with B
                var user = context.DBLoadedFiles.Find(id);

                string file = user.FileName;

                return file;

            }

        }

        public void progressupdate(Guid id)
        {
            using (var context = new LoadedFiles())
            {

                var result = context.DBLoadedFiles.SingleOrDefault(b => b.LoadedFileID == id);
                if (result != null)
                {
                    result.Progress = 100;
                    context.SaveChanges();
                }


            }

        }


        public List<Guid> unprocessedids()
        {

            List<Guid> list = new List<Guid>();

            using (var context = new LoadedFiles())
            {
                var unprocessedfiles = context.DBLoadedFiles.Where(b => b.Progress == null);

                foreach (var row in unprocessedfiles)
                {
                    list.Add(row.LoadedFileID);
                }

            }

            return list;

        }


    }
}

