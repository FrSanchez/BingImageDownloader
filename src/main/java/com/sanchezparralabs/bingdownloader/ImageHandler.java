/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */
package com.sanchezparralabs.bingdownloader;

import java.io.File;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.nio.file.FileAlreadyExistsException;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.io.IOUtils;
import org.apache.logging.log4j.*;

/***
 * Class to save the file locally
 * 
 * @author francisco.sanchez
 *
 */
public class ImageHandler implements Callback {

    private static final Logger logger = LogManager.getLogger(ImageHandler.class);
    
    private String originalFilename = null;
    private File targetFile = null;
    private final String path;

    public ImageHandler(String url, String path) throws FileAlreadyExistsException {
        Pattern p = Pattern.compile(App.imageNameRegex);
        Matcher m = p.matcher(path);
        this.path = path;
        if (m.find()) {
            originalFilename = String.format("%s_%sx%s.%s", m.group(1), m.group(4), m.group(5), m.group(6));
        } else {
            p = Pattern.compile(App.imageNameRegex2);
            m = p.matcher(path);
            if (m.find()) {
                // The name doesn't have localization or size, just the image
                originalFilename = String.format("%s.%s", m.group(1), m.group(2));
            }
        }
        if (null != originalFilename) {
            logger.info(originalFilename);
            targetFile = new File(App.Directory() + "/" + originalFilename);
            if (targetFile.exists()) {
                throw new FileAlreadyExistsException(targetFile.getName());
            }
        }
    }

    @Override
    public void onError(InputStream inputStream, HttpURLConnection connection) throws Exception {
        Map<String, List<String>> headers = connection.getHeaderFields();
        System.err.println(path);
        System.err.println(headers);
        if (inputStream != null) {
            String theString = IOUtils.toString(inputStream, "UTF-8");
            System.err.println(theString);
        }
    }

    @Override
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception {
        System.out.println("Ignoring");
        if (targetFile != null && !targetFile.exists()) {
        	System.out.println(String.format("Saving %s", targetFile));
            Files.copy(inputStream, targetFile.toPath(), StandardCopyOption.REPLACE_EXISTING);
        }
    }

}
