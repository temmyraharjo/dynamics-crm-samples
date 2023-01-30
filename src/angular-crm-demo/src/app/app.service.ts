import { Injectable } from '@angular/core';
import * as WebAPI from "xrm-webapi";

@Injectable({
  providedIn: 'root'
})
export class AppService {
  private config = new WebAPI.WebApiConfig("9.2");
  retrieveMultiple(entitySet: string,  queryString?: string, queryOptions?: WebAPI.QueryOptions) {
    return WebAPI.retrieveMultiple(this.config, entitySet, queryString, queryOptions);
  }
}
