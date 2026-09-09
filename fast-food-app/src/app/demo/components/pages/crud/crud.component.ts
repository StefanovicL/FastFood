import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MessageService, SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { Ingredient, Product, ProductVariant, ProductVariantIngredient } from 'src/app/demo/api/models';
import { IngredientController, ProductController } from 'src/app/services/fastfood.service';
import { eProductSize } from 'src/app/enums/eProductSize';
import { eUnitType } from 'src/app/enums/eUnitType';
import { ProductImageService } from 'src/app/demo/service/product-image.service';
import { environment } from 'src/environments/environment';

@Component({
  templateUrl: './crud.component.html',
  providers: [MessageService]
})
export class CrudComponent implements OnInit {
  productDialog = false;
  deleteProductDialog = false;
  deleteProductsDialog = false;
  products: Product[] = [];
  product: Product = {};
  selectedProducts: Product[] = [];
  submitted = false;
  cols: any[] = [];
  rowsPerPageOptions = [5, 10, 20];
  productSizeOptions: SelectItem[] = [];
  activeVariantSize = 'Single';
  selectedVariantByProductId: Record<number, number> = {};
  pendingProductVariantId: number | undefined;
  ingredients: Ingredient[] = [];
  selectedImageFile: File | null = null;
  imagePreviewUrl: string | null = null;

  constructor(
    private messageService: MessageService,
    private productController: ProductController,
    private ingredientController: IngredientController,
    public productImageService: ProductImageService,
    private http: HttpClient
  ) { }

  ngOnInit() {
    this.initializeProducts();
    this.initializeIngredients();
    this.productSizeOptions = (Object.keys(eProductSize) as Array<keyof typeof eProductSize>)
      .filter(k => isNaN(Number(k)))
      .map(k => ({ label: k === 'DoubleDouble' ? 'Double Double' : k, value: k }));
    this.cols = [{ field: 'name', header: 'Name' }, { field: 'description', header: 'Description' }];
  }

  initializeIngredients() {
    this.ingredientController.GetAllIngredients().subscribe(result => this.ingredients = result ?? []);
  }

  initializeProducts() {
    this.productController.GetAllProducts().subscribe(result => {
      this.products = result ?? [];
      for (const product of this.products) {
        const variant = product.productVariants?.[0];
        if (product.id && variant?.id) this.selectedVariantByProductId[product.id] = variant.id;
      }
    });
  }

  private ensureProductVariants() {
    if (!this.product.productVariants) this.product.productVariants = [];
  }

  private getOrCreateVariant(sizeKey: string): ProductVariant {
    this.ensureProductVariants();
    let variant = this.product.productVariants!.find(v => this.normalizeSizeKey(v.size) === sizeKey);
    if (!variant) {
      variant = { size: eProductSize[sizeKey as keyof typeof eProductSize], price: 0, productVariantIngredients: [] };
      this.product.productVariants!.push(variant);
    }
    if (!variant.productVariantIngredients) variant.productVariantIngredients = [];
    return variant;
  }

  get activeVariant(): ProductVariant { return this.getOrCreateVariant(this.activeVariantSize); }

  getSelectedVariant(product: Product): ProductVariant | undefined {
    const selectedId = product.id ? this.selectedVariantByProductId[product.id] : undefined;
    return product.productVariants?.find(v => v.id === selectedId) ?? product.productVariants?.[0];
  }

  private getSelectedVariantId(product: Product): number | undefined { return this.getSelectedVariant(product)?.id; }

  normalizeSizeKey(size?: string | number | null): string {
    if (size == null) return '';
    return typeof size === 'string' ? size : eProductSize[size] ?? '';
  }

  getProductSizeOptions(product: Product): SelectItem[] {
    const seen = new Set<string>();
    return (product.productVariants ?? []).reduce((options: SelectItem[], variant) => {
      const key = this.normalizeSizeKey(variant.size);
      if (key && !seen.has(key)) {
        seen.add(key);
        options.push({ label: key === 'DoubleDouble' ? 'Double Double' : key, value: key });
      }
      return options;
    }, []);
  }

  getVariantSize(product: Product): string { return this.normalizeSizeKey(this.getSelectedVariant(product)?.size); }

  onRowSizeChange(sizeKey: string, product: Product) {
    const next = product.productVariants?.find(v => this.normalizeSizeKey(v.size) === sizeKey);
    if (product.id && next?.id) this.selectedVariantByProductId[product.id] = next.id;
  }

  hasOutOfStockIngredient(product: Product): boolean {
    const variant = (product.productVariants ?? []).find(v => this.normalizeSizeKey(v.size) === 'Single') ?? product.productVariants?.[0];
    return !!variant && (variant.productVariantIngredients ?? []).some(pvi => (pvi.ingredient?.quantity ?? 0) === 0);
  }

  getIngredientQuantityForActiveVariant(id: number): number {
    return this.activeVariant.productVariantIngredients?.find(x => this.getIngredientFk(x) === id)?.quantity ?? 0;
  }

  setIngredientQuantityForActiveVariant(id: number, quantity: number) {
    const variant = this.activeVariant;
    const existing = variant.productVariantIngredients!.find(x => this.getIngredientFk(x) === id);
    if (!quantity || quantity <= 0) {
      if (existing) variant.productVariantIngredients = variant.productVariantIngredients!.filter(x => this.getIngredientFk(x) !== id);
    } else if (existing) existing.quantity = quantity;
    else variant.productVariantIngredients!.push({ ingredient_FK: id, quantity });
  }

  openNew() {
    this.product = { name: '', description: '', productVariants: [] };
    this.activeVariantSize = 'Single';
    this.getOrCreateVariant(this.activeVariantSize);
    this.selectedImageFile = null;
    this.imagePreviewUrl = null;
    this.submitted = false;
    this.productDialog = true;
  }

  editProduct(product: Product) {
    this.product = {
      ...product,
      productVariants: (product.productVariants ?? []).map(v => ({
        ...v,
        productVariantIngredients: (v.productVariantIngredients ?? []).map(i => ({ ...i, ingredient_FK: this.getIngredientFk(i) }))
      }))
    };
    this.activeVariantSize = this.normalizeSizeKey(this.getSelectedVariant(product)?.size) || this.normalizeSizeKey(this.product.productVariants?.[0]?.size) || 'Single';
    this.getOrCreateVariant(this.activeVariantSize);
    this.selectedImageFile = null;
    this.imagePreviewUrl = null;
    this.productDialog = true;
  }

  triggerImageUpload() { document.getElementById('productImageInput')?.click(); }

  onImageFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.selectedImageFile = file;
    const reader = new FileReader();
    reader.onload = () => this.imagePreviewUrl = reader.result as string;
    reader.readAsDataURL(file);
  }

  deleteSelectedProducts() { this.deleteProductsDialog = true; }

  deleteProduct(product: Product) {
    this.deleteProductDialog = true;
    this.product = { ...product };
    this.pendingProductVariantId = this.getSelectedVariantId(product);
  }

  confirmDeleteSelected() {
    this.deleteProductsDialog = false;
    const ids = this.selectedProducts.map(p => this.getSelectedVariantId(p)).filter((id): id is number => !!id);
    if (!ids.length) { this.selectedProducts = []; return; }
    this.productController.BulkDeleteProductVariants(ids).subscribe({
      next: () => { this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Products Deleted', life: 3000 }); this.selectedProducts = []; this.initializeProducts(); },
      error: () => this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING PRODUCT DELETION', life: 3000 })
    });
  }

  confirmDelete() {
    this.deleteProductDialog = false;
    const id = this.pendingProductVariantId;
    if (!id) {
      this.messageService.add({ severity: 'warn', summary: 'Unable to delete', detail: 'The selected product variant was not found.', life: 3000 });
      return;
    }
    this.productController.DeleteProductVariant(id).subscribe({
      next: () => { this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Product Deleted', life: 3000 }); this.initializeProducts(); },
      error: () => this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING PRODUCT DELETION', life: 3000 })
    });
    this.product = {};
    this.pendingProductVariantId = undefined;
  }

  hideDialog() { this.productDialog = false; this.submitted = false; }
  onActiveVariantSizeChange(sizeKey: string) { this.activeVariantSize = sizeKey; this.getOrCreateVariant(sizeKey); }
  setActiveVariantPrice(price: number) { this.activeVariant.price = price ?? 0; }

  saveProduct() {
    this.submitted = true;
    if (!this.product.name || !this.product.description) {
      this.messageService.add({ severity: 'warn', summary: 'Missing', detail: 'Name and description are required', life: 3000 });
      return;
    }
    this.product.productVariants = (this.product.productVariants ?? []).filter(v => (v.price ?? 0) > 0 || (v.productVariantIngredients?.length ?? 0) > 0);
    if (!this.product.productVariants.length) {
      this.messageService.add({ severity: 'warn', summary: 'Missing', detail: 'At least one variant with price or ingredients is required', life: 3000 });
      return;
    }
    const isUpdate = !!this.product.id;
    const request$ = isUpdate ? this.productController.UpdateProduct(this.toProductPayload(this.product)) : this.productController.CreateProduct(this.toProductPayload(this.product));
    request$.subscribe({
      next: saved => {
        const productId = saved?.id ?? this.product.id;
        const finish = () => {
          this.messageService.add({ severity: 'success', summary: 'Successful', detail: isUpdate ? 'Product Updated' : 'Product Created', life: 3000 });
          this.productDialog = false;
          this.selectedImageFile = null;
          this.imagePreviewUrl = null;
          this.initializeProducts();
        };
        if (this.selectedImageFile && productId) {
          const formData = new FormData();
          formData.append('id', String(productId));
          formData.append('file', this.selectedImageFile);
          this.http.post(`${environment.apiUrl}/Product/UploadProductImage`, formData).subscribe({ next: finish, error: () => { this.messageService.add({ severity: 'warn', summary: 'Image upload failed', detail: 'Product saved but image could not be uploaded.', life: 4000 }); finish(); } });
        } else finish();
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: isUpdate ? 'ERROR DURING PRODUCT UPDATE' : 'ERROR DURING PRODUCT CREATION', life: 3000 })
    });
  }

  unitLabel(unit?: eUnitType): string {
    switch (unit) { case eUnitType.g: return 'g'; case eUnitType.ml: return 'ml'; case eUnitType.kom: return 'kom'; default: return ''; }
  }

  getVariantPrice(product: Product): number | null { return this.getSelectedVariant(product)?.price ?? null; }
  getProductImage(product: Product): string { return this.productImageService.getStoredImageUrl(this.getSelectedVariant(product)?.storedFileName); }
  getCurrentProductImage(): string { return this.productImageService.getStoredImageUrl(this.activeVariant.storedFileName); }
  private getIngredientFk(pvi: ProductVariantIngredient): number | undefined { return pvi.ingredient_FK ?? pvi.ingredient?.id; }

  private toProductPayload(product: Product): Product {
    return {
      id: product.id,
      name: product.name,
      description: product.description,
      productVariants: (product.productVariants ?? []).map(v => ({
        id: v.id, product_FK: v.product_FK, size: v.size, price: v.price, originalFileName: v.originalFileName, storedFileName: v.storedFileName,
        productVariantIngredients: (v.productVariantIngredients ?? []).map(i => ({ id: i.id, productVariant_FK: i.productVariant_FK, ingredient_FK: this.getIngredientFk(i), quantity: i.quantity })).filter(i => !!i.ingredient_FK)
      }))
    };
  }

  trackByFn(index: number, item: any) { return item?.id ?? index; }
  onGlobalFilter(table: Table, event: Event) { table.filterGlobal((event.target as HTMLInputElement).value, 'contains'); }
}
