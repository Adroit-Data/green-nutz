using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data_Inspector.Models
{
    public class MySplit
    {

        public List<string> mySplit(string source, char separator) {
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
                    if (end < 0)
                    {
                        char lastQuot;
                        char.TryParse(quotChar.TrimEnd(separator), out lastQuot);
                        values.Add(source.Substring(1).TrimEnd(lastQuot));
                        source = "";
                    }
                    else
                    {
                        values.Add(source.Substring(1, end-1));
                        source = source.Substring(end + 2);
                    }
                        
                }
                else
                {
                    int end = source.IndexOf(separator);
                    if (end < 0)
                    {
                        values.Add(source.Substring(0));
                        source = "";
                    }
                    else
                    {
                        values.Add(source.Substring(0, end));
                        source = source.Substring(end + 1);
                    }
                }
            }
            
            return values;
        }
    }
}