"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var store_1 = require("@ngrx/store");
var fromMonitor = require("./monitor.reducer");
var environments_enum_1 = require("../models/environments.enum");
exports.monitorReducer = {
    monitor: fromMonitor.monitorReducer
};
exports.getMonitorMainState = store_1.createFeatureSelector('monitor');
exports.getMonitorState = store_1.createSelector(exports.getMonitorMainState, function (state) { return state.monitor; });
exports.getProdChecks = store_1.createSelector(exports.getMonitorMainState, function (state) {
    return state.monitor.checks.filter(function (x) { return x.environmentId == environments_enum_1.EnvironmentsEnum.Prod; });
});
exports.getBetaChecks = store_1.createSelector(exports.getMonitorMainState, function (state) {
    return state.monitor.checks.filter(function (x) { return x.environmentId == environments_enum_1.EnvironmentsEnum.Beta; });
});
exports.getChecksForEnvironment = store_1.createSelector(exports.getMonitorMainState, function (state, environment) {
    return state.monitor.checks.filter(function (x) { return x.environmentId == convertToEnvironmentId(environment); });
});
function convertToEnvironmentId(environment) {
    switch (environment) {
        case "prod":
            return environments_enum_1.EnvironmentsEnum.Prod;
        case "beta":
            return environments_enum_1.EnvironmentsEnum.Beta;
        default:
            return null;
    }
}
//# sourceMappingURL=index.js.map