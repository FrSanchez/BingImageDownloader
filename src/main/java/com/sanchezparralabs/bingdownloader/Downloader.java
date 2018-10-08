/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.io.InputStream;
import java.net.HttpURLConnection;
import java.time.Duration;
import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.io.IOUtils;
import org.apache.commons.lang.StringUtils;

/***
 * Main orchestrator for the download
 * @author francisco.sanchez
 *
 */
public class Downloader implements Callback {

    private Set<String> locales = null;
    private boolean initialized = false;

    public Downloader() {
        try {
            enumerateCountries();
        } catch (Exception e) {
            System.err.println("Can't instantiate downloader");
            e.printStackTrace(System.err);
        }
    }

    public void enumerateCountries() throws Exception {
        HtmlReader.loadFromUrl("http://www.bing.com/account/general?FORM=O2HV46", this, Duration.ofSeconds(2));
    }

    @Override
    public void onError(InputStream inputStream, HttpURLConnection connection) throws Exception {
        Map<String, List<String>> headers = connection.getHeaderFields();
        System.err.println(headers);
        String theString = IOUtils.toString(inputStream, "UTF-8");
        System.err.println(theString);
        locales = null;
    }

    @Override
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception {
        locales = new HashSet<>();
        Map<String, List<String>> headers = connection.getHeaderFields();
        System.out.println(headers);
        String charset = StringUtils.substringAfter(connection.getContentType(), "charset=");
        if (charset == null) {
            charset = "UTF-8";
        }
        String theString = IOUtils.toString(inputStream, charset.toUpperCase());
        Pattern p = Pattern.compile("(mkt=([a-zA-Z\\-]*))");
        Matcher m = p.matcher(theString);
        while (m.find()) {
            if (m.groupCount() == 2) {
                locales.add(m.group(2));
            }
            System.out.println(m.group(0));
        }
        initialized = true;
    }

    public boolean isInitialized() {
        return initialized;
    }

    public void batchDownload() {
        for (String locale : locales) {
            download(locale);
        }

    }

    private void download(String locale) {
        System.out.println("Downlading image from " + locale);
        String url = String.format("http://www.bing.com?scope=web&setmkt=%s", locale);
        int retry = 3;
        while (retry > 0) {
            try {
                retry--;
                HtmlReader.loadFromUrl(url, new BingPageHandler(url), Duration.ofSeconds(10));
                return;
            } catch (Exception e) {
                System.err.println(e.getMessage());
                e.printStackTrace(System.err);
                if (retry > 0) {
                    System.err.println("Will retry again");
                }
            }
        }
    }
}
