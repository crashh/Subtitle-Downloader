# Subtitle-Downloader
Scans a chosen directory, and lists all contents. Can then retrieve subtitles for a selected entry by hitting search.

It will attempt to find the correct subtitles based on the filename. It is possible to customize these parameters before searching.

##Images

![](https://raw.githubusercontent.com/crashh/Subtitle-Downloader/master/example.png)

##Installation

###Install latest build
In the folder 'Latest Build', run the file 'SubtitleDownloader.application'. This will install the program as a desktop app, which can then be run by hitting the windows key and typing 'SubtitleDownloader'.

Windows will complain about the unknown certificate, thus you will have to hit advanced and "install anyway" during the installation.

Alternatively, the program can be downloaded from [here](http://crashh.me:8082/clickonce/subtitledownloader/publish.htm).

###Compile
Clone the repository and compile the source-code, preferrably using visual studio. In case i forgot, it is nessecary to remove/add a new test-certificate for the signing of the files, this can be done by going to Project->Properties->Signing. 

##Uninstall
Remove the application using windows 'add or remove program' like you would any other installation.
