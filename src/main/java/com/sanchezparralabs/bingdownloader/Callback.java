package com.sanchezparralabs.bingdownloader;

import java.io.InputStream;
import java.net.HttpURLConnection;

public interface Callback {
    public void onError(InputStream inputStream, HttpURLConnection connection) throws Exception;
    
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception;
}
