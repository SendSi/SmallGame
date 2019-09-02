//
// Created by douzifly on 8/18/16.
//

#import "AudioPlayer.h"

static AudioPlayer *_instance = nil;

@implementation AudioPlayer {
    AVAudioPlayer *_player;
    
    ///是否正在录音
    BOOL isRecording;
}

#pragma mark SINGLETON

+ (instancetype)sharedInstance {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[AudioPlayer alloc] init];
    });
    
    return _instance;
}

- (instancetype)init {
    
    if (self = [super init]) {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryAmbient error:nil];
            [[AVAudioSession sharedInstance] setActive:YES error:nil];
        });
    }
    
    return self;
}


- (void)setMute:(BOOL)mute {
    _mute = mute;
}

- (void)play:(NSString *)path {
    NSLog(@"AudioPlayer play %@:", path);
    NSError *error = nil;

    _player = [[AVAudioPlayer alloc] initWithContentsOfURL:[NSURL fileURLWithPath:path]
                                              fileTypeHint:AVFileTypeAppleM4A
                                                     error:&error];
    if (_player) {
        [_player setVolume:_mute ? 0 : 1];
    }
    
    _player.delegate = self;
    BOOL ret = [_player prepareToPlay];
    ret = [_player play];
}

- (void)stopPlay {
    if (_player != nil) {
        [_player stop];
    }
}


- (void)audioPlayerDecodeErrorDidOccur:(AVAudioPlayer *)player error:(NSError *__nullable)error {
    NSLog(@"audioPlayerDecodeErrorDidOccur: %@", error);
}

- (void)audioPlayerDidFinishPlaying:(AVAudioPlayer *)player successfully:(BOOL)flag {
    NSLog(@"audioPlayerDidFinishPlaying successfully: %d", flag);
}

- (BOOL)isPlayering{
    if (_player != nil) {
        return [_player isPlaying];
    }
    return NO;
}

@end
