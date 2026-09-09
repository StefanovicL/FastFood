import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IngredientsComponent } from './ingredients.component';

@NgModule({
	imports: [RouterModule.forChild([
		{ path: '', component: IngredientsComponent }
	])],
	exports: [RouterModule]
})
export class IngredientsRoutingModule { }
