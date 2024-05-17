import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/models/product';
import { FastFoodService } from 'src/app/services/fastfood.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {
  title = 'FastFood';
  products: Product[] = [];

  constructor(private fastFoodService: FastFoodService) { }

  ngOnInit(): void {
    this.fastFoodService.getProducts()
    .subscribe((result: Product[]) => (this.products = result));
  }

}
