using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.APIModels
{
    public class UpdateGroup
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
