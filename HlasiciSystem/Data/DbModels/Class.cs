using Data.APIModels;
using Data.Mapper;
using Npgsql.Replication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.DbModels
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public static class ClassExtensions
    {
        public static Class ToClass(this IApplicationMapper mapper, CreateClass createClass)
        {
            return new()
            {
                Id = new Guid(),
                Name = createClass.Name
            };
        }
    }
}