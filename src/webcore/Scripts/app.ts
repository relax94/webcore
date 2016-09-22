import {Component} from '@angular/core';
import {StopsList} from './stopslist';
import {Http} from '@angular/http'

@Component({
    directives: [StopsList],
    selector: 'my-app',
    templateUrl: './app/templates/app.html'
})



export class AppComponent {
    Title: string;
    $http: Http;
    videos: Array<any>;

    constructor(http : Http) {
        this.Title = 'THIS IS ANGULAR2 Boooo !';
     //   this.setMap();
        this.$http = http;
        this.testRequest();
    }


    testRequest() {
        this.$http.get('/api/videos/get').subscribe((response) => {
            this.videos = response.json();
            console.log('videos get', response.json());
        });
        
    }

    uploadFile(event) {
        console.log('click: upload file');
        var file = document.getElementById("droppedFile")['files'][0];
        var uploadProgress = document.getElementById('upload-progress');
        var uploadValue = document.getElementById('upload-value');
        var self = this;
        var upload = new window['tus'].Upload(file,
            {
                endpoint: "http://localhost:4000/files/",
                resume: false,
                metadata: {
                    filename: file.name
                },
                onError: function (error) {
                    console.log("Failed because: " + error);
                },
                onProgress: function (bytesUploaded, bytesTotal) {
                    var percentage = (bytesUploaded / bytesTotal * 100).toFixed(2);
                    console.log(bytesUploaded, bytesTotal, percentage + "%");
                    uploadProgress.style.width = percentage + '%';
                    uploadValue.innerText = percentage + '%';
                },
                onSuccess: function () {
                    console.log("Download %s from %s", upload.file.name, upload.url);
                    uploadProgress.classList.add('progress-bar-success');
                    uploadProgress.style.width = '0%';
                    console.log(upload);
                    var videoVM = { href: upload.url, fileExtension: 'pdf', fileName: upload.file.name };
                    self.$http.post("/api/videos/insert", videoVM).subscribe((response) => {
                        if (response.ok) {
                         //   alert("success : ");
                            self.videos.push(videoVM);
                        }
                        else
                            alert(response.statusText);
                    });
                   
                }
            });

        // Start the upload
        upload.start();
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