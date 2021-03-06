/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */
package com.sanchezparralabs.bingdownloader;

import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URLDecoder;
import java.nio.file.FileAlreadyExistsException;
import java.time.Duration;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.io.IOUtils;
import org.apache.commons.lang.StringUtils;

/***
 * Downloads the bing page for every country
 * 
 * @author francisco.sanchez
 *
 */
public class BingPageHandler implements Callback {

    // private String url;

    public BingPageHandler(String url) {
        // this.url = url;
    }

    @Override
    public void onError(InputStream inputStream, HttpURLConnection connection) throws Exception {
        Map<String, List<String>> headers = connection.getHeaderFields();
        System.err.println(headers);
        String theString = IOUtils.toString(inputStream, "UTF-8");
        System.err.println(theString);
    }

    @Override
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception {
        String charset = StringUtils.substringAfter(connection.getContentType(), "charset=");
        if (charset == null) {
            charset = "UTF-8";
        }
        // String theString = IOUtils.toString(inputStream, charset.toUpperCase());
        Pattern p = Pattern.compile(App.imageUrlPattern);
        for (String line : IOUtils.readLines(inputStream, charset)) {
            Matcher m = p.matcher(line);
            while (m.find()) {
                if (m.groupCount() >= 1) {
                    try {
                        String path = StringUtils.replace(m.group(1), "\\u0026", "&");
                        String imgUrl = String.format("http://www.bing.com%s", path);
                        if (path.startsWith("http")) {
                            imgUrl = path;
                        }
                        HtmlReader.loadFromUrl(imgUrl, new ImageHandler(imgUrl, path), Duration.ofSeconds(5));
                    } catch (FileAlreadyExistsException fe) {
                        System.err.println("Duplicate " + fe.getMessage());
                    }
                }
            }
        }
    }
}
