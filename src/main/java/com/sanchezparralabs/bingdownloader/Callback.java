/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.io.InputStream;
import java.net.HttpURLConnection;

/**
 * Simple interface to create a callback from http call
 * @author francisco.sanchez
 *
 */
public interface Callback {
    public void onError(InputStream inputStream, HttpURLConnection connection) throws Exception;
    
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception;
}
