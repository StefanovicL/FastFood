import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { DataView } from 'primeng/dataview';
import { Product, ProductVariant, Ingredient, CartItem } from 'src/app/demo/api/models';
import { ProductImageService } from 'src/app/demo/service/product-image.service';
import { eProductSize } from 'src/app/enums/eProductSize';
import { ProductController } from 'src/app/services/fastfood.service';

@Component({ templateUrl: './listdemo.component.html', styleUrls: ['./listdemo.component.scss'], providers: [MessageService] })
export class ListDemoComponent implements OnInit {
  products: Product[] = [];
  productsToDisplay: Product[] = [];
  sortOptions: SelectItem[] = [];
  sortOrder = 0;
  sortField = '';
  showCart = false;
  cart: CartItem[] = [];
  selectedVariantByProductId: Record<number, number> = {};
  productSizeOptions: SelectItem[] = [];
  selectedProductSize: keyof typeof eProductSize = 'Single';

  constructor(private productController: ProductController, private messageService: MessageService, private productImageService: ProductImageService) { }

  ngOnInit() {
    this.productController.GetAllProducts().subscribe(result => {
      this.products = result ?? [];
      for (const product of this.products) {
        const variant = this.getVariantBySize(product, this.selectedProductSize) ?? product.productVariants?.[0];
        if (product.id && variant?.id) this.selectedVariantByProductId[product.id] = variant.id;
      }
      const hasSingles = this.products.some(product => this.getVariantBySize(product, 'Single'));
      this.productsToDisplay = hasSingles ? this.products.filter(product => this.getVariantBySize(product, 'Single')) : [...this.products];
    });
    this.productSizeOptions = (Object.keys(eProductSize) as Array<keyof typeof eProductSize>).filter(k => isNaN(Number(k))).map(k => ({ label: k === 'DoubleDouble' ? 'Double Double' : k, value: k }));
    this.sortOptions = [{ label: 'Price High to Low', value: '!variantPrice' }, { label: 'Price Low to High', value: 'variantPrice' }];
  }

  getSelectedVariant(product: Product): ProductVariant | undefined {
    const selectedId = product.id ? this.selectedVariantByProductId[product.id] : undefined;
    return product.productVariants?.find(v => v.id === selectedId) ?? product.productVariants?.[0];
  }
  getVariantBySize(product: Product, sizeKey: string): ProductVariant | undefined { return product.productVariants?.find(v => String(v.size) === String(sizeKey) || String(eProductSize[v.size as number]) === sizeKey); }
  getVariantPrice(product: Product): number { return this.getSelectedVariant(product)?.price ?? 0; }
  getVariantSize(product: Product): string { const size = this.getSelectedVariant(product)?.size; return typeof size === 'number' ? eProductSize[size] : String(size ?? ''); }
  getVariantIngredients(product: Product): Ingredient[] { return (this.getSelectedVariant(product)?.productVariantIngredients ?? []).map(pvi => pvi.ingredient).filter((x): x is Ingredient => !!x); }
  hasOutOfStockIngredient(product: Product): boolean { return this.getVariantIngredients(product).some(i => (i.quantity ?? 0) === 0); }
  onSizeChange(selectedSize: string, product: Product) { const next = this.getVariantBySize(product, selectedSize); if (product.id && next?.id) this.selectedVariantByProductId[product.id] = next.id; }

  addToCart(product: Product) {
    const variant = this.getSelectedVariant(product);
    if (!product.id || !variant?.id) return;
    const existing = this.cart.find(item => item.productVariantId === variant.id);
    if (existing) existing.quantity += 1;
    else this.cart.push({ productId: product.id, productName: product.name ?? '', productDescription: product.description, productVariantId: variant.id, size: variant.size ?? 0, price: variant.price ?? 0, quantity: 1 });
    this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Product Added To Cart', life: 3000 });
  }
  toggleView() { this.showCart = true; }
  onSortChange(event: any) { const value = event.value as string; this.sortOrder = value.startsWith('!') ? -1 : 1; this.sortField = value.replace('!', ''); this.applySort(); }
  onFilter(dataView: DataView, event: Event) { dataView.filter((event.target as HTMLInputElement).value); }
  applySort() { if (this.sortField === 'variantPrice') this.productsToDisplay = [...this.productsToDisplay].sort((a, b) => (this.getVariantPrice(a) - this.getVariantPrice(b)) * this.sortOrder); }
  getProductImage(product: Product): string { return this.productImageService.getStoredImageUrl(this.getSelectedVariant(product)?.storedFileName); }
}
