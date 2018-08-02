using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data_Inspector.Models
{

    public class LoadViewModel
    {
        [Required]
        [Display(Name = "File")]
        public string File { get; set; }

    }

}
