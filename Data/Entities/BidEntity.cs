﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class BidEntity : BaseEntity
    {
        [Key]
        public DateTime Timestamp { get; set; }
        public float Price { get; set; }
        public int Volume { get; set; }
    }
}
