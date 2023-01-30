import { Component } from '@angular/core';
import {AppService} from "./app.service";
import {RetrieveMultipleResponse} from "xrm-webapi";
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  data: {start: Date, end: Date, name: string, qty: number, unitPrice:  number, total: number}[] = [];
  constructor(private appService: AppService) { }
  ngOnInit() {
    let context = Xrm.Utility.getGlobalContext();
    console.log(`Logged in user: ${context.userSettings.userName}..`);
    const id = this.getParameterByName("data") || '';
    console.log(`Params detected ${id}..`);
    const query = "?$select=ins_actualend,ins_actualstart,_ins_contact_value,ins_name,ins_qty,ins_total,ins_unitprice" +
      (id ? `&$filter=_ins_contact_value eq ${id}`: "");
    this.appService.retrieveMultiple("ins_calculates", query)
      .then(result => this.loadTable(result));
  }

  getParameterByName(name: string, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
      results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
  }

  loadTable(result: RetrieveMultipleResponse) {
    this.data = [];
    for (const entity of result.value){
      const ins_actualEnd = entity["ins_actualend"]; // Date Time
      const ins_actualStart = entity["ins_actualstart"]; // Date Time
      const ins_name = entity["ins_name"]; // Text
      const ins_qty = entity["ins_qty"]; // Whole Number
      const ins_total = entity["ins_total"]; // Currency
      const ins_unitPrice = entity["ins_unitprice"]; // Currency
      this.data.push({
        name: ins_name,
        start: ins_actualStart,
        end: ins_actualEnd,
        qty: ins_qty,
        unitPrice: ins_unitPrice,
        total: ins_total
      });
    }
  }
}
