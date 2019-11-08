# RemoveCommentsFromJsonFile
## GifViewer
A single page Web application that can remove all comments from uploaed JSON file.

## Development Enviroment
- Windows 10 + Visual Studio 2019 community
- Or MacOS + Visual Studio 2019 community for Mac.
- ASP.Net Core 3.0. System.Text.Json is used.
- For ASP.Net core 2.x see [here](https://github.com/XMTei/RemoveCommentsFromJsonFile/tree/5fcd910f141554fc0bca452bf6082a4a946d595d). Newtonsoft.Json was used in this version, and use RegularExpressions to remove all the coments from Json files.
- [W3.css](https://www.w3schools.com/w3css/default.asp) for responsiveness
- C#,Javascript

## Setup a Web Server
- Azure App Service windows server is used to test this project.[here](https://removecommentsfromjsonfile.azurewebsites.net/) is the web page.
- I believe Linux + ASP.NET core can be used, but need to test. 

## Use of browsers
- IE,Edge,Chrome,FireFox on Windows.
- Safari,Chrome on MacOS.
- Safari,Chrome,FireFox on iOS.
- Chrome,FireFox on Android.

## Features
- Remove all comments from uploaded JSON file and save it to download folder.
- Multi-language UI.
- Since System.Text.Json (.Net core 3.0) does not support dynamic, a Extension was made to help us to have a dynamic object.
