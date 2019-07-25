import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of, Observable } from 'rxjs';
import { map, mergeMap, switchMap, catchError } from 'rxjs/operators';
import * as monitorActions from '../actions/monitor.actions';
import { MonitorService } from '../services/monitor.service';
import { Action } from '@ngrx/store';

@Injectable()
export class MonitorEffects {

  @Effect()
  getChecks$: Observable<Action> = this.actions$.pipe(
    ofType(monitorActions.GET_CHECKS),
    map(action => action as monitorActions.GetChecks),
    mergeMap((action: monitorActions.GetChecks) => {
      return this._monitorService.getChecks(action.payload.environment).pipe(map(checks => {
        return new monitorActions.SetChecks({ checks: checks });
      }));
    })
  );

  @Effect()
  runCheck$: Observable<Action>  = this.actions$.pipe(
    ofType(monitorActions.RUN_CHECK),
    map((action: monitorActions.RunCheck) => {      
      this._monitorService.runManualCheck(action.payload.checkType);
      return new monitorActions.Noop();
    })
  );

  constructor(
    private actions$: Actions,
    private _monitorService: MonitorService,
  ) { }
}
