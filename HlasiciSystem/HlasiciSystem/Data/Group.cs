using System.ComponentModel.DataAnnotations;

namespace HlasiciSystem.Data
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(MetaData.NameLength)]
        public string Name { get; set; }

        public bool IsActive { get; set; }



        public static class MetaData
        {
            public const int NameLength = 255;
        }
    }
}
