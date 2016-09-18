import {Component} from '@angular/core';
import {StopsList} from './stopslist';

@Component({
    directives: [StopsList],
    selector: 'my-app',
    template: `<p>{{Title}}</p> <!-- Routed views go here -->
<router-outlet></router-outlet>`
})



export class AppComponent {
    Title: string;

    constructor() {
        this.Title = 'THIS IS ANGULAR2 Boooo !';
        this.setMap();
    }

    setMap() {
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
        }

}