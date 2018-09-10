namespace Data_Inspector.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LoadedFile
    {
        [Key]
        [Column(Order = 0)]
        public Guid LoadedFileID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string FileName { get; set; }

        [StringLength(10)]
        public string FileType { get; set; }

        public DateTime FileImportDate { get; set; }

    }
}
