import { Action } from '@ngrx/store';
import { Check } from '../models/check.model';

export const GET_CHECKS = '[Monitor] Get checks';
export const SET_CHECKS = '[Monitor] Set checks';
export const UPDATE_CHECK = '[Monitor] Update check';
export const RUN_CHECK = '[Monitor] Run check';
export const ORDER_BY = '[Monitor] Order by';
export const SHOW_OK_STATUS_CHECKS = '[Monitor] Show Ok status';
export const APPLY_FILTERS = '[Monitor] Apply filters'
export const NOOP = '[Monitor] Noop action';

export class GetChecks implements Action {
  readonly type = GET_CHECKS;

  constructor(public payload: { environment: string }) { }
}

export class SetChecks implements Action {
  readonly type = SET_CHECKS;

  constructor(public payload: { checks: Check[] }) { }
}

export class UpdateCheck implements Action {
  readonly type = UPDATE_CHECK;

  constructor(public payload: { check: Check }) { }
}

export class RunCheck implements Action {
  readonly type = RUN_CHECK;

  constructor(public payload: { checkType: number }) { }
}

export class OrderBy implements Action {
  readonly type = ORDER_BY;

  constructor(public payload: { field: string }) { }
}

export class ShowOkStatusChecks implements Action {
  readonly type = SHOW_OK_STATUS_CHECKS;

  constructor(public payload: boolean) { }
}

export class ApplyFilters implements Action {
  readonly type = APPLY_FILTERS;

  constructor() { }
}

export class Noop implements Action {
  readonly type = NOOP;

  constructor() { }
}


export type Actions =
  GetChecks
  | SetChecks
  | UpdateCheck
  | RunCheck
  | OrderBy
  | ShowOkStatusChecks
  | ApplyFilters
  | Noop
  ;

