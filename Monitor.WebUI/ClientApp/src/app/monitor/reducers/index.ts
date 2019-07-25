import { createSelector, createFeatureSelector } from '@ngrx/store';

import * as fromMonitor from './monitor.reducer';
import * as fromRoot from '../../redux-store/';
import { EnvironmentsEnum } from '../models/environments.enum';

export interface MonitorState {
  monitor: fromMonitor.State;
}

export interface State extends fromRoot.State {
  'monitor': MonitorState;
}

export const monitorReducer = {
  monitor: fromMonitor.monitorReducer
};

export const getMonitorMainState = createFeatureSelector<MonitorState>('monitor');

export const getMonitorState = createSelector(getMonitorMainState, (state: MonitorState) => state.monitor);
export const getProdChecks = createSelector(getMonitorMainState, (state: MonitorState) =>
  state.monitor.checks.filter(x => x.environmentId == EnvironmentsEnum.Prod));
export const getBetaChecks = createSelector(getMonitorMainState, (state: MonitorState) =>
  state.monitor.checks.filter(x => x.environmentId == EnvironmentsEnum.Beta));

export const getChecksForEnvironment = createSelector(getMonitorMainState, (state: MonitorState, environment: string) =>
  state.monitor.checks.filter(x => x.environmentId == convertToEnvironmentId(environment)));

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
