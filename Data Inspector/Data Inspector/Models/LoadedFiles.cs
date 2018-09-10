namespace Data_Inspector.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LoadedFiles : DbContext
    {
        public LoadedFiles()
            : base("name=LoadedFiles")
        {
        }

        public virtual DbSet<LoadedFile> DBLoadedFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoadedFile>()
                .Property(e => e.FileName)
                .IsUnicode(false);

            modelBuilder.Entity<LoadedFile>()
                .Property(e => e.FileType)
                .IsUnicode(false);
        }
    }
}
 