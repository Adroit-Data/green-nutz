//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data_Inspector.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoadedFile
    {
        public System.Guid LoadedFileID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public System.DateTime FileImportDate { get; set; }
        public string UserID { get; set; }
    }
}
