"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var operators_1 = require("rxjs/operators");
var signalr_1 = require("@aspnet/signalr");
var http_1 = require("@angular/common/http");
var environments_enum_1 = require("../models/environments.enum");
var actions = require("../actions/monitor.actions");
//TODO : move to config
var API_URL = "https://localhost:44320/";
var MonitorService = /** @class */ (function () {
    function MonitorService(_http, _store) {
        this._http = _http;
        this._store = _store;
        this.orderByPropMapping = {
            id: 'id',
            host: 'host',
            lastRun: 'lastCheckTime',
            inCurrent: 'duration',
            status: 'status',
            service: 'service',
            statusInfo: 'statusInfo',
            description: 'description',
            priority: 'priority'
        };
    }
    MonitorService.prototype.getChecks = function (environment) {
        var _this = this;
        var environmentId = this.convertToEnvironmentId(environment);
        var options = {
            params: { environmentId: environmentId.toString() }
        };
        return this._http.get(API_URL + 'api/monitor/get-checks', options).pipe(operators_1.map(function (data) {
            _this.startListening();
            return data;
        }));
    };
    MonitorService.prototype.runManualCheck = function (checkType) {
        var parameters = { checkType: checkType };
        var options = {
            params: new http_1.HttpParams().set("checkType", checkType.toString())
        };
        this._http.get(API_URL + 'api/monitor/manual-check', options).subscribe();
    };
    MonitorService.prototype.stopListen = function () {
        if (this.hubConnection) {
            this.hubConnection.stop();
        }
    };
    MonitorService.prototype.startListening = function () {
        var _this = this;
        if (this.hubConnection) {
            return;
        }
        this.hubConnection = new signalr_1.HubConnectionBuilder()
            .withUrl(API_URL + 'notify')
            .build();
        this.hubConnection.on("BroadcastChecks", function (data) {
            console.log({ data: data });
            if (data && data.length) {
                data.forEach(function (check) {
                    _this._store.dispatch(new actions.UpdateCheck({ check: check }));
                });
            }
        });
        this.hubConnection
            .start()
            .then(function () { console.log('Connection started!'); })
            .catch(function (err) { return console.log('Error while establishing connection'); });
    };
    //public orderBy(param: string): void {
    //  if (!this.checks.length) {
    //    return;
    //  }
    //  let propertyName = this.orderByPropMapping[param];
    //  if (this.lastSortProperty === propertyName) {
    //    this.sortOrder = (this.sortOrder === 'ASC') ? 'DESC' : 'ASC';
    //  } else {
    //    this.sortOrder = 'ASC';
    //  }
    //  this.checks = this.checks.sort((check1, check2)=>{
    //    let result = this.getOrder(check1, check2, propertyName);
    //    return (this.sortOrder == 'ASC') ? (result) : (result * -1);
    //  });
    //  this.lastSortProperty = propertyName;
    //}
    MonitorService.prototype.convertToEnvironmentId = function (environment) {
        switch (environment) {
            case "prod":
                return environments_enum_1.EnvironmentsEnum.Prod;
            case "beta":
                return environments_enum_1.EnvironmentsEnum.Beta;
            default:
                return null;
        }
    };
    MonitorService.prototype.getOrder = function (check1, check2, propertyName) {
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
    };
    MonitorService = __decorate([
        core_1.Injectable({
            providedIn: 'root',
        })
    ], MonitorService);
    return MonitorService;
}());
exports.MonitorService = MonitorService;
//# sourceMappingURL=monitor.service.js.map