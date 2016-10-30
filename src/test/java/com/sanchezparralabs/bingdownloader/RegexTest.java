/*                                                                                                                            
 * Copyright, 2016, Sanchez Parra Labs
 * All Rights Reserved
 */
package com.sanchezparralabs.bingdownloader;

import java.util.regex.Pattern;

import junit.framework.TestCase;

import org.junit.Test;

public class RegexTest extends TestCase {

    @Test
    // Make sure the image pattern will compile
    public void testImageUrlPatternTest() throws Exception {
        Pattern p = Pattern.compile(App.imageUrlPattern);
        p.matcher("");
    }

    @Test
    public void test() throws Exception {
        Pattern p = Pattern.compile(App.imageNameRegex);
        p.matcher("");
    }
}
