/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.time.Duration;
import java.util.*;
import java.util.function.Consumer;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

/***
 * Main orchestrator for the download
 * @author francisco.sanchez
 *
 */
public class Downloader  {

    private Set<String> allUrls = null;
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
        allUrls = new HashSet<>();
        Document settingsForm = Jsoup.connect("http://www.bing.com/account/general?FORM=O2HV46").get();
        Elements elements = settingsForm.getElementsByAttribute("data-priority");
        elements.forEach(new Consumer<Element>() {
            @Override
            public void accept(Element element) {
                if (element.tagName() == "li") {
                    String href = element.childNode(0).attr("href");
                    allUrls.add(href);
                }
            }
        });
        initialized = true;
//        HtmlReader.loadFromUrl("http://www.bing.com/account/general?FORM=O2HV46", this, Duration.ofSeconds(2));
    }

    public boolean isInitialized() {
        return initialized;
    }

    public void batchDownload() {
        for (String url : allUrls) {
            download(url);
        }

    }

    private void download(String url) {
        System.out.println("Downlading image from " + url);
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
