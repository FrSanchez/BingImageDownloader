package com.sanchezparralabs.bingdownloader;

import java.io.InputStream;
import java.net.HttpURLConnection;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.io.IOUtils;
import org.apache.commons.lang.StringUtils;

public class BingPageHandler implements Callback {

    public static final String imageUrlPattern = "(g_img={url\\: \"(?<url>.*)\",id)";
    
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
        String theString = IOUtils.toString(inputStream, charset.toUpperCase());
        Pattern p = Pattern.compile(imageUrlPattern);
        Matcher m = p.matcher(theString);
        while (m.find()) {
            System.out.println(m.group(0));
            if (m.group("url")!=null) {
                
            }
        }
    }

}
