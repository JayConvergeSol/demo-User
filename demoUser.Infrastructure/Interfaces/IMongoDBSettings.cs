using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Infrastructure.Interfaces
{
    public interface IMongoDBSettings
    {
        public string? ConnectionURI { get; set; } 
        public string? DatabaseName { get; set; }
        public string? CollectionName { get; set; }
    }
}
