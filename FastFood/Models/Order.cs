﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FastFood.Models
{
    public partial class Order
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public DateTime OrderReceivedDateTime { get; set; }
        public DateTime OrderDeliveredDateTime { get; set; }
        public int State { get; set; }

        public virtual Product Product { get; set; }
    }
}