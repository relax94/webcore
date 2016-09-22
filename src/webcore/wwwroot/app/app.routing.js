"use strict";
var stopslist_1 = require('./stopslist');
var pageone_1 = require('./pageone');
var appRoutes = [
    { path: '', component: stopslist_1.StopsList },
    //{
    //    path: 'heroes',
    //    component: HeroListComponent,
    //    data: {
    //        title: 'Heroes List'
    //    }
    //},
    { path: 'pageone', component: pageone_1.PageOneComponent },
    { path: 'stoplist', component: stopslist_1.StopsList },
];
exports.appRoutingProviders = [];
//export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes); 
//# sourceMappingURL=app.routing.js.map