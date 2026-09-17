import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { Ingredient } from 'src/app/demo/api/models';
import { IngredientController } from 'src/app/services/fastfood.service';
import { eUnitType } from 'src/app/enums/eUnitType';

@Component({
  templateUrl: './ingredients.component.html',
  providers: [MessageService]
})
export class IngredientsComponent implements OnInit {

  ingredientDialog: boolean = false;
  deleteIngredientDialog: boolean = false;

  ingredients: Ingredient[] = [];
  ingredient: Ingredient = {};
  submitted: boolean = false;

  unitOptions: SelectItem[] = [];

  constructor(
    private messageService: MessageService,
    private ingredientController: IngredientController
  ) { }

  ngOnInit() {
    this.initializeIngredients();

    this.unitOptions = (Object.keys(eUnitType) as Array<keyof typeof eUnitType>)
      .filter(k => isNaN(Number(k)))
      .map(k => ({ label: k, value: eUnitType[k] }));
  }

  initializeIngredients() {
    this.ingredientController.GetAllIngredients().subscribe({
      next: (result) => {
        this.ingredients = result ?? [];
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Neuspešno', detail: 'Greška pri učitavanju zaliha', life: 3000 });
      }
    });
  }

  unitLabel(unit?: eUnitType): string {
    switch (unit) {
      case eUnitType.g: return 'g';
      case eUnitType.ml: return 'ml';
      case eUnitType.kom: return 'kom';
      default: return '';
    }
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  openNew() {
    this.ingredient = { unit: eUnitType.g, quantity: 0 };
    this.submitted = false;
    this.ingredientDialog = true;
  }

  editIngredient(ingredient: Ingredient) {
    this.ingredient = { ...ingredient };
    this.ingredientDialog = true;
  }

  hideDialog() {
    this.ingredientDialog = false;
    this.submitted = false;
  }

  deleteIngredient(ingredient: Ingredient) {
    this.ingredient = { ...ingredient };
    this.deleteIngredientDialog = true;
  }

  confirmDelete() {
    this.deleteIngredientDialog = false;

    const id = this.ingredient.id;
    if (!id)
        return;

    this.ingredientController.DeleteIngredient(id).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Uspešno', detail: 'Namirnica je obrisana', life: 3000 });
        this.initializeIngredients();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Neuspešno', detail: 'Greška pri brisanju namirnice', life: 3000 });
      }
    });

    this.ingredient = {};
  }

  saveIngredient() {
    this.submitted = true;

    if (!this.ingredient.name || this.ingredient.unit == null) {
      this.messageService.add({ severity: 'warn', summary: 'Nedostaju podaci', detail: 'Naziv i jedinica su obavezni', life: 3000 });
      return;
    }

    const isUpdate = !!this.ingredient.id;
    const req$ = isUpdate
      ? this.ingredientController.UpdateIngredient(this.ingredient)
      : this.ingredientController.CreateIngredient(this.ingredient);

    req$.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Uspešno',
          detail: isUpdate ? 'Namirnica je izmenjena' : 'Namirnica je kreirana',
          life: 3000
        });

        this.ingredientDialog = false;
        this.initializeIngredients();
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Neuspešno',
          detail: isUpdate ? 'Greška pri izmeni namirnice' : 'Greška pri kreiranju namirnice',
          life: 3000
        });
      }
    });
  }
}
