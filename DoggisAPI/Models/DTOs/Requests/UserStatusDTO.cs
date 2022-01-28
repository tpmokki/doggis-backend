using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoggisAPI.Models.DTOs.Requests
{
    public class UserStatusDTO
    {
        [Required]
        public string Status { get; set; }
    }
}
