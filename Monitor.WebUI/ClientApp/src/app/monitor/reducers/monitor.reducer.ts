import { createSelector } from '@ngrx/store';
import * as monitor from "../actions/monitor.actions";
import { Check } from '../models/check.model';
import { EnvironmentsEnum } from '../models/environments.enum';
import { CheckFilters } from '../models/check-filter.model';
import { StatusesEnum } from '../models/statuses.enum';

export interface State {
  checks: Check[],
  allChecks: Check[],
  lastSortProperty: string,
  sortOrder: string,
  filters: CheckFilters
}

const initialState: State = {
  checks: [],
  allChecks: [],
  lastSortProperty: '',
  sortOrder: 'ASC',
  filters: new CheckFilters()
}

export function monitorReducer(state = initialState, action: monitor.Actions): State {

  switch (action.type) {

    case monitor.RUN_CHECK: {
      const existing = state.checks.find(c => c.type == action.payload.checkType);
      if (existing) {
        existing.loading = true;
      }

      return { ...state };
    }

    case monitor.SET_CHECKS: {
      const newList: Check[] = mergeArrays(action.payload.checks, state.checks);

      return {
        ...state,
        checks: newList,
        allChecks: newList
      }
    }

    case monitor.UPDATE_CHECK: {
      upsertCheck(action.payload.check, state.checks);

      return { ...state };
    }

    case monitor.ORDER_BY: {
      const field = action.payload.field;
      let sortOrder = 'ASC';
      if (state.lastSortProperty === field) {
        sortOrder = (state.sortOrder === 'ASC') ? 'DESC' : 'ASC';
      }

      let orderedChecks = state.checks.sort((check1, check2) => {
        let result = getOrder(check1, check2, field);
        return (sortOrder == 'ASC') ? (result) : (result * -1);
      });

      return {
        ...state,
        checks: orderedChecks,
        lastSortProperty: field,
        sortOrder: sortOrder
      }    
    }

    case monitor.SHOW_OK_STATUS_CHECKS: {
        return {
          ...state,
          filters: {
            ...state.filters,
            showOkStatus: action.payload
          }
        }
    }

    case monitor.APPLY_FILTERS: {
        return {
          ...state,        
          checks: getFilteredChecks(state.allChecks, state.filters)
        }
    }    

    default: {
      return state;
    }

  }
}

function mergeArrays(source: Check[], target: Check[]): Check[] {

  if (!target.length) {
    return source || [];
  } 

  let newChecksList: Check[] = [...target];

  source.forEach(check => {
    upsertCheck(check, newChecksList);
  });
  
  return newChecksList;
}

function upsertCheck(check: Check, checksList: Check[]): Check[] {

  const existing = checksList.find(c => c.type == check.type);

  if (existing) {
    for (var attrname in check) {
      existing[attrname] = check[attrname];
    }
    existing.loading = false;
  } else {
    checksList.push(check);
  }

  return checksList;
}

function getOrder(check1: Check, check2: Check, propertyName: string): number {

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

function convertToEnvironmentId(environment: string): number {
  switch (environment) {
    case "prod":
      return EnvironmentsEnum.Prod;
    case "beta":
      return EnvironmentsEnum.Beta;
    default:
      return null;
  }
} 

function getFilteredChecks(fullList: Check[], filters: CheckFilters): Check[] {
  if (filters.showOkStatus) {
    return fullList;
  }
  return fullList.filter(x => x.status != StatusesEnum.OK);
}

export const getProdChecks = (state: State) =>
  state.checks.filter(x => x.environmentId == EnvironmentsEnum.Prod);

export const getBetaChecks = (state: State) =>
  state.checks.filter(x => x.environmentId == EnvironmentsEnum.Beta);

export const getChecksForEnvironment = (state: State, environment: string) =>
  state.checks.filter(x => x.environmentId == convertToEnvironmentId(environment));
