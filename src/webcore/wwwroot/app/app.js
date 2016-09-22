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
var http_1 = require('@angular/http');
var AppComponent = (function () {
    function AppComponent(http) {
        this.Title = 'THIS IS ANGULAR2 Boooo !';
        //   this.setMap();
        this.$http = http;
        this.testRequest();
    }
    AppComponent.prototype.testRequest = function () {
        var _this = this;
        this.$http.get('/api/videos/get').subscribe(function (response) {
            _this.videos = response.json();
            console.log('videos get', response.json());
        });
    };
    AppComponent.prototype.uploadFile = function (event) {
        console.log('click: upload file');
        var file = document.getElementById("droppedFile")['files'][0];
        var uploadProgress = document.getElementById('upload-progress');
        var uploadValue = document.getElementById('upload-value');
        var self = this;
        var upload = new window['tus'].Upload(file, {
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
                self.$http.post("/api/videos/insert", videoVM).subscribe(function (response) {
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
    };
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
            templateUrl: './app/templates/app.html'
        }), 
        __metadata('design:paramtypes', [http_1.Http])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.js.map