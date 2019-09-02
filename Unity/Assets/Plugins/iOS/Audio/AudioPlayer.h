//
// Created by douzifly on 8/18/16.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <ImSDK/ImSDK.h>

@interface AudioPlayer : NSObject<AVAudioPlayerDelegate>

+(instancetype) sharedInstance;

@property (nonatomic, assign) BOOL mute;

- (void)play:(NSString *)path;

- (void)stopPlay;

- (BOOL)isPlayering;

@end
