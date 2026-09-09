import { eOrderState } from "src/app/enums/eOrderState";
import { eProductSize } from "src/app/enums/eProductSize";
import { eUnitType } from "src/app/enums/eUnitType";

export interface Ingredient {
  id?: number;
  name?: string;
  quantity?: number;
  unit?: eUnitType;
  productVariantIngredients?: ProductVariantIngredient[];
}

export interface Product {
  id?: number;
  name?: string;
  description?: string;
  productVariants?: ProductVariant[];
}

export interface ProductVariant {
  id?: number;
  product_FK?: number;
  size?: eProductSize;
  price?: number;
  originalFileName?: string;
  storedFileName?: string;
  productName?: string;
  product?: Product;
  productVariantIngredients?: ProductVariantIngredient[];
  productVariantOrders?: ProductVariantOrder[];
}

export interface ProductVariantIngredient {
  id?: number;
  productVariant_FK?: number;
  ingredient_FK?: number;
  quantity?: number;
  productVariant?: ProductVariant;
  ingredient?: Ingredient;
}

export interface Order {
  id?: number;
  orderReceivedDateTime?: string; // Date
  orderReadyDateTime?: string; // Date
  orderDeliveredDateTime?: string; // Date
  orderCancelledDateTime?: string; // Date
  state?: eOrderState;
  totalPrice?: number;
  orderNumber?: number;
  productVariantOrders?: ProductVariantOrder[];
}

export interface ProductVariantOrder {
  id?: number;
  productVariant_FK?: number;
  order_FK?: number;
  quantity?: number;
  unitPrice?: number;
  order?: Order;
  productVariant?: ProductVariant;
}

export interface User {
  id?: number;
  username?: string;
  password?: string;
  roleId?: number;
  userRoles?: UserRole[];
}

export interface Role {
  id?: number;
  name?: string;
  userRoles?: UserRole[];
}

export interface UserRole {
  id?: number;
  user_FK?: number;
  role_FK?: number;
  user?: User;
  role?: Role;
}

export interface CartItem {
  productId: number;
  productName: string;
  productDescription?: string;
  productVariantId: number;
  size: number;   // enum int
  price: number;
  quantity: number;
  storedFileName?: string;
}
