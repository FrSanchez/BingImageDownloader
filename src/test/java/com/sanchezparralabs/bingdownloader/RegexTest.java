package com.sanchezparralabs.bingdownloader;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.junit.Test;

import junit.framework.TestCase;

public class RegexTest extends TestCase {
    
    public static final String imageUrlPattern = "(g_img=\\{url\\: \"(?<url>.*)\",id)";
    
    @Test
    public void testImageUrlPatternTest() throws Exception {
        String theString = "";
        Pattern p = Pattern.compile(imageUrlPattern);
        Matcher m = p.matcher(theString);
    }
}
