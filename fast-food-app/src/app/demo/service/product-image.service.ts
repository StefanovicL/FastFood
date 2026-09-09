import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ProductImageService {
  private readonly basePath = 'assets/demo/images/burgers/';
  private readonly placeholder = 'assets/demo/images/burgers/Placeholder.jpg';

  getStoredImageUrl(storedFileName?: string | null): string {
    if (!storedFileName) 
      return this.placeholder;

    return this.basePath + storedFileName;
  }
}
