using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.APIModels
{
    public class Users
    {
        [Required]
        public List<string> Ids { get; set; }
    }
}
