/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */ 
package com.sanchezparralabs.bingdownloader;

import java.io.File;
import java.util.*;

import org.apache.commons.lang.StringUtils;

import com.google.code.mathparser.MathParser;
import com.google.code.mathparser.MathParserFactory;
import com.google.code.mathparser.factories.exception.impl.InvalidCharacterException;
import com.google.code.mathparser.parser.calculation.Result;

/**
 * Holds info for duplicate files
 * @author francisco.sanchez
 *
 */
public class DupInfo {
    private String prefix;
    private final List<File> files;
    private final List<String> resolutions;

    public DupInfo(String prefix) {
        this.prefix = prefix;
        files = new ArrayList<>(2);
        resolutions = new ArrayList<String>(2);
    }

    public File getFileToDelete() {
        if (size() > 1) {
            String res = findSmallestResolution();
            if (res == null) {
                return null;
            }
            File answer = null;
            for (File f : files) {
                if (StringUtils.contains(f.getName(), res)) {
                    answer = f;
                }
            }
            return answer;
        } else {
            return null;
        }
    }

    private String findSmallestResolution() {
        MathParser parser = MathParserFactory.create();
        Map<Result, String> map = new HashMap<>();
        for (String res : resolutions) {
            try {
                map.put(parser.calculate(res.replace('x', '*')), res);
            } catch (InvalidCharacterException e) {
                System.err.println(String.format("[%s] %s :: %s", prefix, res, e.getMessage()));
            }
        }
        Result min = null;
        if (map.size() <2) {
            return null;
        }
        for (Result r : map.keySet()) {
            if (min == null) {
                min = r;
            } else {
                if (min.doubleValue() > r.doubleValue()) {
                    min = r;
                }
            }
        }

        return map.get(min);

    }

    public String getPrefix() {
        return prefix;
    }

    public void setPrefix(String prefix) {
        this.prefix = prefix;
    }

    public List<File> getFiles() {
        return files;
    }

    public List<String> getResolutions() {
        return resolutions;
    }

    public void addFile(File file, String parsed) {
        files.add(file);
        resolutions.add(parsed);
    }

    @Override
    public String toString() {
        return String.format("%s %s", prefix, files.toString());
    }

    public int size() {
        return files.size();
    }
}
