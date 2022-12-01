using System.ComponentModel.DataAnnotations;

namespace Data.APIModels
{
    public class CreateGroup
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
