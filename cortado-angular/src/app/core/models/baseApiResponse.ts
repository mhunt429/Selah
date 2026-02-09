export type BaseApiResponse<T> = {
  statusCode: number;
  data: T;
  errors: string[];
};
