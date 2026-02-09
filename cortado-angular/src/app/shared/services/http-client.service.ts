import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface RequestOptions {
  headers?: HttpHeaders | { [header: string]: string | string[] };
  params?:
    | HttpParams
    | { [param: string]: string | number | boolean | ReadonlyArray<string | number | boolean> };
  responseType?: 'json' | 'text' | 'blob';
}

@Injectable({
  providedIn: 'root',
})
export class HttpClientService {
  public static API_BASE = '/api';
  constructor(private http: HttpClient) {}

  public get$ = <T>(url: string, queryParams?: object, options = {}) =>
    this.http.get<T>(
      `${HttpClientService.API_BASE}${url}${this.serializeQueryParams(queryParams)}`,
      { ...options, withCredentials: true }
    );

  public getWithResponse$ = <T = any>(url: string, queryParams?: object) =>
    this.http.get<T>(
      `${HttpClientService.API_BASE}${url}${this.serializeQueryParams(queryParams)}`,
      { observe: 'response' }
    );

  public post$ = <T, U>(url: string, requestBody: U, options = {}) =>
    this.http.post<T>(`${HttpClientService.API_BASE}${url}`, requestBody, {
      ...options,
      withCredentials: true,
    });

  public put$ = <T, U>(url: string, requestBody: U, options = {}) =>
    this.http.put<T>(`${HttpClientService.API_BASE}${url}`, requestBody, {
      ...options,
      withCredentials: true,
    });

  public patch$ = <T = any>(url: string, requestBody: any, options = {}) =>
    this.http.patch<T>(`${HttpClientService.API_BASE}${url}`, requestBody, {
      ...options,
      withCredentials: true,
    });

  public delete$ = <T>(url: string, queryParams?: object, options = {}) =>
    this.http.delete<T>(
      `${HttpClientService.API_BASE}${url}${this.serializeQueryParams(queryParams)}`,
      { ...options, withCredentials: true }
    );
  private serializeQueryParams = (params: any) => {
    if (!params) {
      return '';
    }
    const validatedParams: any = {};
    Object.keys(params).forEach((key) => {
      if (params.keys !== undefined) {
        validatedParams[key] = params.keys;
      }
    });
    return `?${new URLSearchParams(validatedParams).toString().replace(/%2C/g, ',')}`;
  };
}
