using D.APIModels;
using Data.APIModels;
using Data.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DbModels
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid TeacherId { get; set; }
        public User User { get; set; }
    }

    public static class GroupExtensions
    {
        public static Group ToGroup(this IApplicationMapper mapper, CreateGroup createGroup)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = createGroup.Name,
                IsActive = false
            };
        }
    }
}
