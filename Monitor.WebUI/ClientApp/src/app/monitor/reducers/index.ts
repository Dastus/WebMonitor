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

export const getProdChecks = createSelector(getMonitorState, fromMonitor.getProdChecks);
export const getBetaChecks = createSelector(getMonitorState, fromMonitor.getBetaChecks);
export const getChecksForEnvironment = createSelector(getMonitorState, fromMonitor.getChecksForEnvironment);
