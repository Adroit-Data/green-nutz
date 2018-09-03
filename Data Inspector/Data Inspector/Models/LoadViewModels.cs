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

        public string DetectSeperator(string headerrow)
        {
            string seperator;

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
                seperator = "\",\"";
            }
            else if (qualifiercount == 0 && commacount > 0)
            {
                seperator = ",";
            }
            else
            {                
                seperator = ",";
            }
            return seperator;
            ;
        }

    }

    


}
