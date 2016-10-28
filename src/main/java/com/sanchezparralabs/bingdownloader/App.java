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
        try {
            downloader.enumerateCountries();
        } catch (Exception e1) {
            // TODO Auto-generated catch block
            e1.printStackTrace();
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
