# BingImageDownloader
**Author:** Francisco Sanchez: francisco.sanchez@outlook.com

Will download the daily bing image from all the supported locales

This is a short Java project that will download today's Bing Wallpaper to the current folder or the specified one.

It can be used in a cronjob to run daily and download the wallpaper every day.

The wallpaper location is retrieved from the main page html code by using a regular expression and the stripped from some unique number - that way it can be de-duped.

The program browses all the supported locales from Bing and searches every one of them to get different wallpapers. It will ignore any duplicates.

## Dependencies
- Apache commons-cli
- Apache commons-lang
- Apache commons-io
- Google math parser

## Compilation
The project uses maven
```bash
mvn compile
```

## Execution
```bash
java -jar target/BingDownloader-1.0.jar
```

## TODO
- [ ] Move all the hardcoded strings to a central class
- [ ] Add more unit tests to the logic
