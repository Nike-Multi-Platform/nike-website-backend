﻿namespace nike_website_backend.Dtos
{
    public class UserOrderItemDTO
    {
        public int? UserOrderId { get; set; }
        public int? ProductSizeid { get; set; }
        public int? Amount { get; set; }
        public string? Thumbnail { get; set; }
        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public decimal? Price { get; set; }

    }
}
