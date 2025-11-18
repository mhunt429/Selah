// import { inject } from '@angular/core';
// import { HttpInterceptorFn } from '@angular/common/http';
// import { AuthService } from '../services/auth.service';

// export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
//   const authService = inject(AuthService);
//   const token = authService.getAccessToken();

//   if (token) {
//     req = req.clone({
//       setHeaders: {
//         x_api_token: token,
//       },
//     });
//   }

//   return next(req);
// };
