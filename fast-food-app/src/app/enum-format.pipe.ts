import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumFormat'
})
export class EnumFormatPipe implements PipeTransform {

  transform(value: string | number | null | undefined): string {
    if (value === null || value === undefined) return '';

    // Ako je broj → samo vrati kao string
    if (typeof value === 'number') {
      return value.toString();
    }

    let formatted = value;

    // snake_case i kebab-case → space
    formatted = formatted.replace(/[_-]+/g, ' ');

    // camelCase / PascalCase → space
    formatted = formatted.replace(/([a-z])([A-Z])/g, '$1 $2');

    // ALLCAPS spojeno (DOUBLEDOUBLE) → Double Double
    formatted = formatted.replace(/([A-Z])([A-Z][a-z])/g, '$1 $2');

    // višestruki razmaci → jedan
    formatted = formatted.replace(/\s+/g, ' ').trim();

    // Capitalize svaku reč
    formatted = formatted
      .toLowerCase()
      .split(' ')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');

    return formatted;
  }
}