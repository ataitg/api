using Data.APIModels;
using Data.Mapper;
using Data.VModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("User")]
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

        public static GroupVm ToGroupVm(this IApplicationMapper mapper, Group group)
        {
            return new()
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                IsActive = group.IsActive
            };
        }

        public static UpdateGroup ToUpdateGroup(this IApplicationMapper mapper, Group group)
        {
            return new()
            {
                Name = group.Name,
                IsActive = group.IsActive
            };
        }

        public static Group ToGroup(this IApplicationMapper mapper, UpdateGroup updateGroup, Group group)
        {
            group.IsActive = updateGroup.IsActive;
            group.Name = updateGroup.Name;
            return group;
        }

    }
}
