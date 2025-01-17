﻿using FastFood.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FastFoodContext _context;
        
        public ProductController(FastFoodContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<List<Product>>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(await _context.Products.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<Product>>> UpdateProduct(Product product)
        {
            var dbProduct = await _context.Products.FindAsync(product.ProductID);
            if (dbProduct == null)
                return BadRequest("Proizvod ne postoji.");

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.Size = product.Size;

            await _context.SaveChangesAsync();

            return Ok(await _context.Products.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Product>>> DeleteProduct(int id)
        {
            var dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null)
                return BadRequest("Proizvod ne postoji.");

            _context.Products.Remove(dbProduct);
            await _context.SaveChangesAsync();

            return Ok(await _context.Products.ToListAsync());
        }
    }
}
