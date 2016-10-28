package com.sanchezparralabs.bingdownloader;

import java.io.File;
import java.security.InvalidParameterException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.commons.lang.StringUtils;

public class Duplicates {
    public List<File> findDuplicates(String folder) {
        List<File> dups = new ArrayList<File>();
        Map<String, DupInfo> dupFiles = new HashMap<>();
        File f = new File(folder);
        if (f.exists() && f.isDirectory()) {
            for (File file : f.listFiles(new ImageFilter())) {
                String[] parsed = StringUtils.split(file.getName(), "_.");
                if (parsed.length > 2) {
                    if (!dupFiles.containsKey(parsed[0])) {
                        dupFiles.put(parsed[0], new DupInfo(parsed[0]));
                    }
                    dupFiles.get(parsed[0]).addFile(file, parsed[1]);
                }
            }
        } else {
            throw new InvalidParameterException(folder + " is not a valid folder");
        }
        
        for( DupInfo entry : dupFiles.values()) {
            File tbd = entry.getFileToDelete();
            if (tbd != null) {
                dups.add(tbd);
            }
        }
        return dups;
    }
}
