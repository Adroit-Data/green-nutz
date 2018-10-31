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

                if (source.StartsWith("\""))
                {
                    
                    int end = source.IndexOf("\"" + separator);
                    if (end < 0)
                    {
                        values.Add(source.Substring(0));
                        source = "";
                    }
                    else
                    {
                        values.Add(source.Substring(0, end));
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