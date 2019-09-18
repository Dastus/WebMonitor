import { Injectable } from '@angular/core';
import { Observable ,  of } from 'rxjs';
import { map } from 'rxjs/operators';
import { Check } from '../models/check.model';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { EnvironmentsEnum } from '../models/environments.enum';

import { Store, select } from '@ngrx/store';
import * as fromMonitorStore from '../reducers';
import * as actions from '../actions/monitor.actions';

//TODO : move to config
const API_URL = "https://localhost:44320/";

@Injectable({
  providedIn: 'root',
})
export class MonitorService {

  private hubConnection: HubConnection;

  constructor(
    private _http: HttpClient,
    private _store: Store<fromMonitorStore.State>,
  ) { }
  
  public getChecks(environment: string): Observable<Check[]> {
    const environmentId = this.convertToEnvironmentId(environment);

    const options = {
      params: { environmentId: environmentId.toString() }
    }

    return this._http.get(API_URL + 'api/monitor/get-checks', options).pipe(map((data: Check[]) => {        
        this.startListening();
        return data;
      }));   
  }

  public runManualCheck(checkType: number): void {
    const parameters = { checkType: checkType };

    const options = {
      params: new HttpParams().set("checkType", checkType.toString())
    }   

    this._http.get(API_URL + 'api/monitor/manual-check', options).subscribe();

  }

  public stopListen(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }    
  }

  private startListening(): void {
    if (this.hubConnection) {
      return;
    }  

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(API_URL + 'notify')
      .build();

    this.hubConnection.on("BroadcastChecks", (data: Check[]) => {
      console.log({ data });
      if (data && data.length) {
        data.forEach(check => {
          this._store.dispatch(new actions.UpdateCheck({ check: check }));
          this._store.dispatch(new actions.ApplyFilters());
        })
      }
    });        

    this.hubConnection
      .start()
      .then(() => { console.log('Connection started!')})
      .catch(err => console.log('Error while establishing connection'));
  }

  //public orderBy(param: string): void {
  //  if (!this.checks.length) {
  //    return;
  //  }
 
  //  let propertyName = this.orderByPropMapping[param];
    
  //  if (this.lastSortProperty === propertyName) {
  //    this.sortOrder = (this.sortOrder === 'ASC') ? 'DESC' : 'ASC';
  //  } else {
  //    this.sortOrder = 'ASC';
  //  }

  //  this.checks = this.checks.sort((check1, check2)=>{
  //    let result = this.getOrder(check1, check2, propertyName);
  //    return (this.sortOrder == 'ASC') ? (result) : (result * -1);
  //  });

  //  this.lastSortProperty = propertyName;
  //}

  private convertToEnvironmentId(environment: string): number {
    switch (environment) {
      case "prod":
        return EnvironmentsEnum.Prod;
      case "beta":
        return EnvironmentsEnum.Beta;
      default:
        return null;
    }      
  } 

  getOrder(check1: Check, check2: Check, propertyName: string): number {
    
    if (check1[propertyName] == null || check2[propertyName] == null) {
      if (check1[propertyName] == null && check2[propertyName] == null) {
        return 0;
      }
      return (check1[propertyName]) ? 1 : -1;        
    }
    if (check1[propertyName] == check2[propertyName]) {
      return 0;
    }
    return (check1[propertyName] > check2[propertyName]) ? 1 : -1;
  }

  orderByPropMapping = {
    id: 'id',
    host: 'host',
    lastRun: 'lastCheckTime',
    inCurrent: 'duration',
    status: 'status',
    service: 'service',
    statusInfo: 'statusInfo',
    description: 'description',
    priority: 'priority'
 };
}
