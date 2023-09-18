using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Core.Entities
{
    public class userOTP
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OTP { get; set; }
        public string CreationTime { get; set; }
        public string ExpirationTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public string UpdatedOn { get; set; }

    }
}
