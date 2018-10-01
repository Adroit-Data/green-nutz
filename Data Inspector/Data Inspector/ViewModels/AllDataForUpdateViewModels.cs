using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data_Inspector.Models;

namespace Data_Inspector.ViewModels
{
    public class AllDataForUpdateViewModels 
    {
        public string tableName { get; set; }
        public string rowId { get; set; }
        public List<DataForUpdate> DataForUpdate { get; set; }
        // don't know why this wasn't uploaded to git hub at the first time
    }
}