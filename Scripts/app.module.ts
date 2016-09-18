import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent }  from './app';
import { routing,
    appRoutingProviders }  from './app.routing';

import { StopsList }    from './stopslist';
import {PageOneComponent} from './pageone'


@NgModule({
    imports: [BrowserModule, routing],
    declarations: [AppComponent, StopsList, PageOneComponent],
    bootstrap: [AppComponent],
    providers: [appRoutingProviders]
})
export class AppModule { }