using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;

namespace Data_Inspector.Models
{
    public class MySplit
    {

        
        public List<string> mySplit(string source, char separator) { // this function replaces empty "" fields with NULL values so that later on during Altering table and assigning int data type for the column, empty spaces will not result in being populated with "0" only with Nulls 

            List<string> values = new List<string>();

            while (source.Count() != 0)
            {
     
                string quotChar;
                if (source.StartsWith("\""))
                {
                    quotChar = "\"";
                }
                else if (source.StartsWith("'"))
                {
                    quotChar = "'";
                }
                else
                {
                    quotChar = null;
                }

                if (quotChar != null)
                {
                    quotChar += separator;
                    int end = source.IndexOf(quotChar);
                    if (end < 0) // happening at the end of the string when separator doesn't exist because it is end of line e.g.("value" insted of "value",)
                    {
                        char lastQuot;
                        char.TryParse(quotChar.TrimEnd(separator), out lastQuot);
                        if (source.Substring(1).TrimEnd(lastQuot).Count() != 0)
                        {
                            values.Add(source.Substring(1).TrimEnd(lastQuot));
                        }
                        else
                        {
                            values.Add(null);
                        }
                        source = "";
                    }
                    else
                    {
                        if (source.Substring(1, end - 1).Count() != 0)
                        {
                            values.Add(source.Substring(1, end - 1));
                        }
                        else
                        {
                            values.Add(null);
                        }
                        source = source.Substring(end + 2);
                    }
                        
                }
                else
                {
                    int end = source.IndexOf(separator);
                    if (end < 0)
                    {
                        if (source.Substring(0).Count() != 0)
                        {
                            values.Add(source.Substring(0));
                        }
                        else
                        {
                            values.Add(null);
                        }
                        source = "";
                    }
                    else
                    {
                        if (source.Substring(0, end).Count() != 0)
                        {
                            values.Add(source.Substring(0, end));
                        }
                        else
                        {
                            values.Add(null);
                        }
                        source = source.Substring(end + 1);
                    }
                }
            }
            
            return values;
        }
    }
}