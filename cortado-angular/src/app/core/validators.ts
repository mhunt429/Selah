import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';

export const passwordMatchValidator: ValidatorFn = (
  group: AbstractControl
): ValidationErrors | null => {
  const password = group.get('password')?.value;
  const passwordConfirmation = group.get('passwordConfirmation')?.value;
  return password === passwordConfirmation ? null : { passwordsMismatch: true };
};
