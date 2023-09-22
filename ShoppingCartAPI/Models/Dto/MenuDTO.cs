﻿using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models
{
    public class MenuDTO
    {
        public int MenuId { get; set; }

        [Required]
        public string MenuName { get; set; }

        [Required]
        public int? ParentId { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
