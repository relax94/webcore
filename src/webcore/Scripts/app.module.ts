import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent }  from './app';
//import { routing,
//    appRoutingProviders }  from './app.routing';

import { StopsList }    from './stopslist';
import {PageOneComponent} from './pageone'
import { HttpModule} from '@angular/http';


@NgModule({
    imports: [BrowserModule, /*routing,*/ HttpModule],
    declarations: [AppComponent, StopsList, PageOneComponent],
    bootstrap: [AppComponent],
    providers: [/*appRoutingProviders*/]
})
export class AppModule { }