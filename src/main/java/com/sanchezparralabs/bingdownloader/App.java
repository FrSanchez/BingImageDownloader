package com.sanchezparralabs.bingdownloader;

import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.net.URLDecoder;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.CommandLineParser;
import org.apache.commons.cli.DefaultParser;
import org.apache.commons.cli.Options;
import org.apache.commons.cli.ParseException;

/**
 * Hello world!
 *
 */
public class App
{
    private static String directory = "";

    public static void main(String[] args) throws ParseException, UnsupportedEncodingException
    {
        // create Options object
        Options options = new Options();
        options.addOption("d", "directory", false, "Folder to save the downloaded files, will use the binary folder by default");

        CommandLineParser parser = new DefaultParser();
        CommandLine cmd = parser.parse(options, args);
        
        directory = cmd.getOptionValue("d");
        if (directory == null) {
            directory = URLDecoder.decode( App.class.getProtectionDomain().getCodeSource().getLocation().getPath(), "UTF-8");
            if (directory.endsWith(".jar")) {
                directory = directory.substring(0, directory.lastIndexOf('/') + 1);
            }
        }
        
        System.out.println("Directory: " + directory);
    }
}
