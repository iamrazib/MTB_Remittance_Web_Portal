<!--

/*
Configure menu styles below
NOTE: To edit the link colors, go to the STYLE tags and edit the ssm2Items colors
*/
YOffset=150; // no quotes!!
XOffset=0;
staticYOffset=30; // no quotes!!
slideSpeed=20 // no quotes!!
waitTime=100; // no quotes!! this sets the time the menu stays out for after the mouse goes off it.
menuBGColor="black";
menuIsStatic="yes"; //this sets whether menu should stay static on the screen
menuWidth=150; // Must be a multiple of 10! no quotes!!
menuCols=2;
hdrFontFamily="verdana";
hdrFontSize="2";
hdrFontColor="white";
hdrBGColor="#170088";
hdrAlign="left";
hdrVAlign="center";
hdrHeight="15";
linkFontFamily="Verdana";
linkFontSize="2";
linkBGColor="white";
linkOverBGColor="#FFFF99";
linkTarget="_top";
linkAlign="Left";
barBGColor="#444444";
barFontFamily="Verdana";
barFontSize="2";
barFontColor="white";
barVAlign="center";
barWidth=20; // no quotes!!
barText="SIDE CAPTION"; // <IMG> tag supported. Put exact html for an image to show.

///////////////////////////

// ssmItems[...]=[name, link, target, colspan, endrow?] - leave 'link' and 'target' blank to make a header

 ssmItems[0] = ["<big><font face='Arial'>Slid In Menu Items</font></big>", ""]
    ssmItems[1] = ["C-Sharp Home", "http://www.c-sharpcorner.com/"]
    ssmItems[2] = ["Videos", "http://www.c-sharpcorner.com/Videos/"]
    ssmItems[3] = ["Advertise", "http://www.c-sharpcorner.com/Media/ContactUs.aspx"]
    ssmItems[4] = ["Certification", "http://www.c-sharpcorner.com/Exam/Default.aspx"]
    ssmItems[5] = ["Downloads", "http://www.c-sharpcorner.com/Downloads/"]
    ssmItems[6] = ["Blogs", "http://www.c-sharpcorner.com/Blogs/"]
    ssmItems[7] = ["InterViews", "http://www.c-sharpcorner.com/Interviews/"]
    ssmItems[8] = ["Jobs", "http://www.c-sharpcorner.com/Jobs/"]
    ssmItems[9] = ["Forums", "http://www.c-sharpcorner.com/Forums/"]
    ssmItems[10] = ["Dotnet Heaven", "http://www.dotnetheaven.com/"]

buildMenu();

//-->