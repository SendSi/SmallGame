//
// Created by douzifly on 8/17/16.
//

#import "AudioRecorder.h"

#define MIN_DURATION 2 // 2seconds

@implementation AudioRecorder {
    AVAudioRecorder *_audioRecorder;
    NSTimer *_meterTimer;
}

@synthesize saveFolder = _saveFolder;

- (instancetype) init {
    self = [super init];
    if (self) {
        _minDuration = MIN_DURATION;
    }
    return self;
}

/**
 * 生成录音文件名
 * @return
 */
- (NSString *)genFileName {
    if (_saveFolder == nil || [_saveFolder isEqualToString:@""]) {
        return @"";
    }

    return [NSString stringWithFormat:@"%@%d.m4a", _saveFolder, (int) [[NSDate date] timeIntervalSince1970]];
}

- (void)startRecord:(CallbackBlock)callback {
    NSString *fileName = [self genFileName];
    NSLog(@"startRecord %@", fileName);
    if ([fileName isEqualToString:@""]) {
        callback(NO, NO);
        return;
    }
    AVAudioSession *session = [AVAudioSession sharedInstance];
    [session requestRecordPermission:^(BOOL grant) {
        if (grant) {
            NSLog(@"Recording permission has been granted");
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord error:nil];
            [[AVAudioSession sharedInstance] setActive:true error:nil];
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord error: nil];
            
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker error:nil];
            
            NSDictionary *settings = [[NSDictionary alloc] initWithObjectsAndKeys:
                    [NSNumber numberWithInt:kAudioFormatMPEG4AAC], AVFormatIDKey,
                    [NSNumber numberWithInt:16000],AVSampleRateKey,
                    [NSNumber numberWithInt:1],AVNumberOfChannelsKey,
                    [NSNumber numberWithInt:16],AVLinearPCMBitDepthKey,
                    [NSNumber numberWithBool:NO],AVLinearPCMIsBigEndianKey,
                    [NSNumber numberWithBool:NO],AVLinearPCMIsFloatKey,
                            nil];

            NSError *error = nil;
            _audioRecorder = [[AVAudioRecorder alloc] initWithURL:[NSURL fileURLWithPath:fileName]
                                                         settings:settings
                                                            error:&error];

            if (error != nil) {
                NSLog(@"init AVAudioRecorder failed: %@", error);
                callback(NO, NO);
                return;
            }
            _audioRecorder.delegate = self;
            _audioRecorder.meteringEnabled = YES;

            BOOL prepared = [_audioRecorder prepareToRecord];
            if (!prepared) {
                NSLog(@"Recording prepare failed");
                callback(NO, NO);
                return;
            }
            NSLog(@"Recording start");
            [_audioRecorder record];
            callback(YES, NO);
            NSLog(@"timer start");
            // start timer
            _meterTimer = [NSTimer scheduledTimerWithTimeInterval:0.1
                                                           target:self
                                                         selector:@selector(updateTimeAndMeter)
                                                         userInfo:nil
                                                          repeats:YES];

        } else {
            NSLog(@"Recording permission has been denied");
            callback(NO, YES);
        }
    }];

}

- (void)updateTimeAndMeter{
    NSLog(@"updateTimeAndMeter record recorder:%@, time: %f", _audioRecorder, _audioRecorder.currentTime);
    if (_audioRecorder == nil) {
        return;
    }
    [_audioRecorder updateMeters];
    CGFloat ALPHA = 0.05;
//    NSLog(@"peakPower:%f", [_audioRecorder peakPowerForChannel:0]);
    // 0 ~ 0.06 | 0.06 ~ 0.13 | 0.13 ~ 0.20 | 0.20 ~  0.27 | 0.27 +
    double peakPower = pow(10, (ALPHA * [_audioRecorder peakPowerForChannel:0]));
    [self.delegate recorderUpdate:_audioRecorder.currentTime AndRate:(CGFloat)peakPower];
}

- (void)stopRecord {
    NSLog(@"stopRecord %@", _audioRecorder);
    if (_audioRecorder != nil) {
        [_audioRecorder stop];
    }
    [self releaseTimer];
}

- (void)cancelRecord {
    NSLog(@"cancelRecord %@", _audioRecorder);
    if (_audioRecorder != nil) {
        _audioRecorder.delegate = nil;
        [_audioRecorder stop];
        BOOL ret = [_audioRecorder deleteRecording];
        NSLog(@"cancelRecord deleted: %d", ret);
    }
    [self releaseTimer];
}

- (void)releaseAudiorecorder {
    if (_audioRecorder != nil) {
        NSLog(@"release audio recorder");
        _audioRecorder = nil;
    }
}

- (void) releaseTimer {
    if (_meterTimer != nil) {
        [_meterTimer invalidate];
        _meterTimer = nil;
    }
}

- (void)dealloc {
    [self releaseAudiorecorder];
    [self releaseTimer];
}

#pragma mark AVAudioRecorderDelegate
- (void)audioRecorderDidFinishRecording:(AVAudioRecorder *)recorder successfully:(BOOL)flag {
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryAmbient error:nil];
        [[AVAudioSession sharedInstance] setActive:YES error:nil];
    });

    if (_audioRecorder == nil) {
        return;
    }
    AVURLAsset *asset = [AVURLAsset URLAssetWithURL:recorder.url options:nil];
    NSTimeInterval durationInSeconds =  (int)ceil(CMTimeGetSeconds(asset.duration));

    if (durationInSeconds < _minDuration) {
        BOOL ret = [_audioRecorder deleteRecording];
        NSLog(@"voice record duration too short: %f deleted:%d", durationInSeconds, ret);
        // delete voice file
        [self.delegate recorderFinishedButDurationTooShort];
        return;
    }

    NSLog(@"record finished success:%d duration:%d", flag, durationInSeconds);
    [self.delegate recorderFinished:recorder
                        successfuly:flag
                               path:recorder.url duration:durationInSeconds];
}

- (void)audioRecorderEncodeErrorDidOccur:(AVAudioRecorder *)recorder error:(NSError *__nullable)error {
    NSLog(@"record encode error %@", error);
//    AVAudioSession *session = [AVAudioSession sharedInstance];
//    [session setActive:NO error:nil];
    [self.delegate recorderErrorOccur:recorder error:error];
}

@end
