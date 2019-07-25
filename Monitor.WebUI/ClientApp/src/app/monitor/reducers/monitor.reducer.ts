import { createSelector } from '@ngrx/store';
import * as monitor from "../actions/monitor.actions";
import { Check } from '../models/check.model';
import { EnvironmentsEnum } from '../models/environments.enum';

export interface State {
  checks: Check[];
  lastSortProperty: string,
  sortOrder: string
}

export const initialState: State = {
  checks: [],
  lastSortProperty: '',
  sortOrder: 'ASC'
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
        checks: newList
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
