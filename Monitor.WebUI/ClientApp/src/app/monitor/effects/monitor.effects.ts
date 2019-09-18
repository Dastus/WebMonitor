import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of, Observable } from 'rxjs';
import { map, mergeMap, switchMap, catchError } from 'rxjs/operators';
import * as monitorActions from '../actions/monitor.actions';
import { MonitorService } from '../services/monitor.service';
import { Action } from '@ngrx/store';
import { Check } from '../models/check.model';

@Injectable()
export class MonitorEffects {

  @Effect()
  getChecks$: Observable<Action> = this.actions$.ofType(monitorActions.GET_CHECKS).pipe(
      switchMap((action: monitorActions.GetChecks)=> {
        return this._monitorService.getChecks(action.payload.environment).pipe(
          switchMap((checks: Check[]) => [
            new monitorActions.SetChecks({ checks: checks }),
            new monitorActions.ApplyFilters()
          ])
        )
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
