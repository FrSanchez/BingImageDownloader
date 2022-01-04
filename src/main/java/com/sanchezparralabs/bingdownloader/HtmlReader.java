/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.net.HttpURLConnection;
import java.net.InetSocketAddress;
import java.net.MalformedURLException;
import java.net.Proxy;
import java.net.URL;
import java.security.InvalidParameterException;
import java.time.Duration;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;

/**
 * Simple wrapper to read from http connection with a proxy
 * @author: francisco.sanchez
 */
public class HtmlReader {

    private static final Logger logger = Logger.getLogger(HtmlReader.class);
    private static volatile Proxy proxy = null;

    private static void setProxy() throws MalformedURLException {
        proxy = Proxy.NO_PROXY;
        String envProxy = System.getenv("HTTP_PROXY");
        if (envProxy != null && !StringUtils.isEmpty(envProxy)) {
            URL url = new URL(envProxy);
            proxy = new Proxy(Proxy.Type.HTTP, new InetSocketAddress(url.getHost(), url.getPort()));
        } else {
        }
    }

    public static int loadFromUrl(String url, Callback callback, Duration timeout) throws Exception {
        if (callback == null) {
            throw new InvalidParameterException("callback can't be null");
        }
        if (proxy == null) {
            setProxy();
        }
        if (url.contains("platform.bing.com")) {
            return 0;
        }
        logger.info(url);
        HttpURLConnection connection = (HttpURLConnection) new URL(url).openConnection(proxy);
        if (timeout != null) {
            connection.setConnectTimeout(1000);
            connection.setReadTimeout((int) timeout.toMillis());
        }
        connection.setRequestProperty("Accept", "*/* ");
        int status = connection.getResponseCode();
        logger.info("Status : " + status);
        boolean validstatus = status >= 200 && status < 300;
        
        if (validstatus) {
            callback.onSuccess(connection.getInputStream(), connection);
        } else {
            callback.onError(connection.getErrorStream(), connection);
        }
        connection.disconnect();
        return status;
    }

}
