"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GET_CHECKS = '[Monitor] Get checks';
exports.SET_CHECKS = '[Monitor] Set checks';
exports.UPDATE_CHECK = '[Monitor] Update check';
exports.RUN_CHECK = '[Monitor] Run check';
exports.ORDER_BY = '[Monitor] Order by';
exports.SHOW_OK_STATUS_CHECKS = '[Monitor] Show Ok status';
exports.NOOP = '[Monitor] Noop action';
var GetChecks = /** @class */ (function () {
    function GetChecks(payload) {
        this.payload = payload;
        this.type = exports.GET_CHECKS;
    }
    return GetChecks;
}());
exports.GetChecks = GetChecks;
var SetChecks = /** @class */ (function () {
    function SetChecks(payload) {
        this.payload = payload;
        this.type = exports.SET_CHECKS;
    }
    return SetChecks;
}());
exports.SetChecks = SetChecks;
var UpdateCheck = /** @class */ (function () {
    function UpdateCheck(payload) {
        this.payload = payload;
        this.type = exports.UPDATE_CHECK;
    }
    return UpdateCheck;
}());
exports.UpdateCheck = UpdateCheck;
var RunCheck = /** @class */ (function () {
    function RunCheck(payload) {
        this.payload = payload;
        this.type = exports.RUN_CHECK;
    }
    return RunCheck;
}());
exports.RunCheck = RunCheck;
var OrderBy = /** @class */ (function () {
    function OrderBy(payload) {
        this.payload = payload;
        this.type = exports.ORDER_BY;
    }
    return OrderBy;
}());
exports.OrderBy = OrderBy;
var ShowOkStatusChecks = /** @class */ (function () {
    function ShowOkStatusChecks(payload) {
        this.payload = payload;
        this.type = exports.SHOW_OK_STATUS_CHECKS;
    }
    return ShowOkStatusChecks;
}());
exports.ShowOkStatusChecks = ShowOkStatusChecks;
var Noop = /** @class */ (function () {
    function Noop() {
        this.type = exports.NOOP;
    }
    return Noop;
}());
exports.Noop = Noop;
//# sourceMappingURL=monitor.actions.js.map