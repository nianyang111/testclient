#import "Clipboard.h"
@implementation Clipboard
//将文本复制到IOS剪贴板
- (void)objc_copyTextToClipboard : (NSString*)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}
@end

extern "C" {
    static Clipboard *iosClipboard;
    
    void _copyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList] ;
        
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
        
        [iosClipboard objc_copyTextToClipboard: text];
    }
    
    float getiOSBatteryLevel()
    {
        [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
        
        return [[UIDevice currentDevice] batteryLevel];
        
    }
    
    void openUrl(const char *textList)
    {
        NSString *secondStringOBJ = [NSString stringWithCString:textList encoding:NSUTF8StringEncoding];
        secondStringOBJ=[secondStringOBJ stringByReplacingOccurrencesOfString:@"\""  withString:@""];
        NSLog(@"secondStringOBJ = %@",secondStringOBJ);
        NSURL *cleanUrl=[NSURL URLWithString:[NSString stringWithFormat:@"%@",secondStringOBJ]];
        NSLog(@"Yang hongzhi cleanUrl %@",cleanUrl);
        NSLog(@"%@", [[UIApplication sharedApplication] canOpenURL:cleanUrl] ? @"true" : @"false");
        [[UIApplication sharedApplication] openURL:cleanUrl];
    }
    
    
}
