@ interface Clipboard : NSObject

extern "C"
{
    /*  compare the namelist with system processes  */
    void _copyTextToClipboard(const char *textList);
    
    float getiOSBatteryLevel();
    
    void openUrl(const char *textList);
}

@end
