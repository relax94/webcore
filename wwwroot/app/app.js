"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var stopslist_1 = require('./stopslist');
var AppComponent = (function () {
    function AppComponent() {
        this.Title = 'THIS IS ANGULAR2 Boooo !';
        this.setMap();
    }
    AppComponent.prototype.setMap = function () {
        if (window.hasOwnProperty('travelMap')) {
            var map = window['travelMap'].createMap({
                stops: [
                    {
                        lat: 33.748995,
                        long: -84.387982,
                        info: "Atlanta11, Georgia - Departed Jun 3, 2014"
                    },
                    {
                        lat: 48.856614,
                        long: 2.352222,
                        info: "Paris, France - Jun 4-24, 2014"
                    },
                    {
                        lat: 50.850000,
                        long: 4.350000,
                        info: "Brussels, Belgium - Jun 25-27, 2014"
                    }
                ],
                selector: "#map"
            });
        }
        else
            console.log('travelMap is undefined');
    };
    AppComponent = __decorate([
        core_1.Component({
            directives: [stopslist_1.StopsList],
            selector: 'my-app',
            template: "<p>{{Title}}</p> <!-- Routed views go here -->\n<router-outlet></router-outlet>"
        }), 
        __metadata('design:paramtypes', [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
