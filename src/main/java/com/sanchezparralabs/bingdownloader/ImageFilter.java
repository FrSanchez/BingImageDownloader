package com.sanchezparralabs.bingdownloader;

import java.io.File;
import java.io.FileFilter;

/**
 * A class that implements the Java FileFilter interface.
 */
public class ImageFilter implements FileFilter
{
    private final String[] okFileExtensions = new String[] { "jpg", "png", "gif" };

    public boolean accept(File file)
    {
        for (String extension : okFileExtensions)
        {
            if (file.getName().toLowerCase().endsWith(extension))
            {
                return true;
            }
        }
        return false;
    }
}