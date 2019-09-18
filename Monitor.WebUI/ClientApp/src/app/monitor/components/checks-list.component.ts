import { Component, OnInit, OnDestroy } from '@angular/core';
import { Check } from '../models/check.model';
import { MonitorService } from '../services/monitor.service';
import { Observable ,  Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { Store, select } from '@ngrx/store';
import * as fromMonitorStore from '../reducers';
import * as actions from '../actions/monitor.actions';

@Component({
  selector: 'checks-list',
  templateUrl: './checks-list.component.html',
  styles: ['div[class^="col-"] { font-weight: bold; color: white; background-color: #9BA59B}; padding-bottom: 12px;']
})
export class ChecksListComponent implements OnInit, OnDestroy {

  public checks$: Observable<Check[]>;
  public displayOkChecks: boolean = false; 
  private _destroy$: Subject<boolean> = new Subject<boolean>();
  private environment: string;

  constructor(
    private _monitorService: MonitorService,
    private _activatedRoute: ActivatedRoute,
    private _store: Store<fromMonitorStore.State>,
  ) { }

  ngOnInit() {
    this._activatedRoute.params.pipe(takeUntil(this._destroy$)).subscribe(
      params => {
        this.environment = params['environment'];

        this.checks$ = this._store.select(fromMonitorStore.getChecksForEnvironment, this.environment);

        this._store.dispatch(new actions.GetChecks({ environment: this.environment }));
      });
    //this.checks$.pipe(takeUntil(this._destroy$)).subscribe(envChecks => { console.log({ envChecks }) });
  }

  ngOnDestroy() {
    this._monitorService.stopListen();
    this._destroy$.next(true);
    this._destroy$.unsubscribe();
  }

  public orderBy(param: string): void {
    this._store.dispatch(new actions.OrderBy({ field: param }));
  }

  public runManualCheck(check: Check): void {
    this._store.dispatch(new actions.RunCheck({ checkType: check.type }));
  }

  public onOkChecksChange(): void {
    this._store.dispatch(new actions.ShowOkStatusChecks(this.displayOkChecks));
    this._store.dispatch(new actions.ApplyFilters());
  }
}
