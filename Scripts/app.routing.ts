import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {StopsList} from './stopslist'
import {PageOneComponent} from './pageone'

const appRoutes: Routes = [
    { path: '', component: StopsList },
    //{
    //    path: 'heroes',
    //    component: HeroListComponent,
    //    data: {
    //        title: 'Heroes List'
    //    }
    //},
    { path: 'pageone', component: PageOneComponent },
    { path: 'stoplist', component: StopsList },
    //{ path: '**', component: PageNotFoundComponent }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);