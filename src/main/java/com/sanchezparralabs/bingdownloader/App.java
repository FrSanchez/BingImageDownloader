/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.List;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.CommandLineParser;
import org.apache.commons.cli.DefaultParser;
import org.apache.commons.cli.Options;
import org.apache.commons.cli.ParseException;

import org.apache.log4j.BasicConfigurator;

/**
 * Main entry point
 * @author francisco.sanchez
 *
 */
public class App
{
//	\[(.*?)\]
//	background-image: url(/th?id=OHR.LittleBlueHeron_EN-US0980028207_1920x1080.jpg&rf=LaDigue_1920x1080.jpg); opacity: ;
    public static final String imageUrlPattern = "background-image: url\\((?<url>[^\\)]+)\\)";
    public static final String imageNameRegex = "(?<name>[a-zA-Z0-9]+)_(?<locale>[a-zA-Z\\-]{3,5})*(?<suffix>.*)_(?<sizex>[0-9]+)x(?<sizey>[0-9]+)\\.(?<ext>[^&]*)";
    public static final String imageNameRegex2 = "(?<name>[a-zA-Z0-9_]+)\\.(?<ext>gif|jpg|png)";

    private static String directory = "";
    
    public static String Directory() {
        return directory;
    }

    public static void main(String[] args) throws ParseException, UnsupportedEncodingException
    {
        BasicConfigurator.configure();
        // create Options object
        Options options = new Options();
        options.addOption("d", "directory", true, "Folder to save the downloaded files, will use the binary folder by default");

        CommandLineParser parser = new DefaultParser();
        CommandLine cmd = parser.parse(options, args);

        directory = cmd.getOptionValue("d");
        if (directory == null) {
            directory = URLDecoder.decode(App.class.getProtectionDomain().getCodeSource().getLocation().getPath(), "UTF-8");
            if (directory.endsWith(".jar")) {
                directory = directory.substring(0, directory.lastIndexOf('/') + 1);
            }
        }
        
        Downloader downloader = new Downloader();
        if (downloader.isInitialized()) {
            downloader.batchDownload();
        } else {
            System.err.println("Bye bye");
            System.exit(255);
        }

        System.out.println("Directory: " + directory);
        Duplicates dup = new Duplicates();
        List<File> dups = dup.findDuplicates(directory);
        for (File f : dups) {
            System.out.println("Deleting " + f);
            if (f.exists() && !f.isDirectory()) {
                try {
                    f.delete();
                } catch (Exception e) {
                    System.err.println(e.getMessage());
                    e.printStackTrace(System.err);
                }
            }
        }
    }

}
