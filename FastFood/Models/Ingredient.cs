﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FastFood.Models
{
    public partial class Ingredient
    {
        public int IngredientID { get; set; }
        public int ProductID { get; set; }
        public int StockID { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Stock Stock { get; set; }
    }
}