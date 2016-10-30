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
import org.apache.log4j.Logger;

/***
 * Class to save the file locally
 * @author francisco.sanchez
 *
 */
public class ImageHandler implements Callback {

    private Logger logger = Logger.getLogger(ImageHandler.class);
    private String originalFilename = null;
    private File targetFile = null;

    public static final String imageNameRegex = "(?<name>[a-zA-Z0-9]+)_(?<locale>[a-zA-Z\\-]{3,5})*(?<suffix>.*)_(?<sizex>[0-9]+)x(?<sizey>[0-9]+)\\.(?<ext>.*)";

    public ImageHandler(String url, String path) throws FileAlreadyExistsException {
        Pattern p = Pattern.compile(imageNameRegex);
        Matcher m = p.matcher(path);

        if (m.find()) {
            originalFilename = String.format("%s_%sx%s.%s", m.group(1), m.group(4), m.group(5), m.group(6));
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
        System.err.println(headers);
        String theString = IOUtils.toString(inputStream, "UTF-8");
        System.err.println(theString);
    }

    @Override
    public void onSuccess(InputStream inputStream, HttpURLConnection connection) throws Exception {
        System.out.println("Ignoring");
        if (targetFile != null && !targetFile.exists()) {
            Files.copy(inputStream, targetFile.toPath(), StandardCopyOption.REPLACE_EXISTING);
        }
    }

}
