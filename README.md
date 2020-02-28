# BingImageDownloader
A command line utility that downloads the current bing image from all the supported locales.

This is a short Java project that will download today's Bing Wallpaper to the current folder or the specified one.

It can be used in a cronjob to run daily and download the wallpaper every day.

The wallpaper location is retrieved from the main page html code by using a regular expression and the stripped from some unique number - that way it can be de-duped.

The program browses all the supported locales from Bing and searches every one of them to get different wallpapers. It will ignore any duplicates.

## Getting started
To get your own copy, clone the current repository.
You are going to need:
* A working installation of JDK 8 or higher
* maven

## Compilation
The project is configured with maven, therefore the following line will give you a full jar file under the target folder:

```bash
mvn package
```


## Execution
After you compile, you can execute it from the command line with the following command:

```bash
java -jar target/BingDownloader-1.0.jar
```

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning.

## Authors

* **Francisco Sanchez** - *Initial work* - [GitHub](https://github.com/FrSanchez)


## License

This project is licensed under the GNU v3 License - see the [LICENSE](LICENSE) file for details


## TODO
- [ ] Move all the hardcoded strings to a central class
- [ ] Add more unit tests to the logic
