//
// Created by douzifly on 8/17/16.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AudioToolbox/AudioServices.h>

typedef void (^CallbackBlock)(BOOL successStar, BOOL isNoPermission);

@protocol AudioRecorderDelegate <NSObject>

/**
 * 录音状态更新
 * @param recordedTime 已经录制时间 秒为单位
 * @param rate 声音大小频率 0.2 ~ 1.0 之间
 */
- (void)recorderUpdate:(NSTimeInterval)recordedTime AndRate:(CGFloat)rate;
/**
 * 录制完成
 * @param recorder
 * @param flag  是否成功
 * @param path  路径
 * @param duration 长度秒数
 */
- (void)recorderFinished:(AVAudioRecorder *)recorder successfuly:(BOOL) flag path:(NSURL *)path duration:(NSTimeInterval) duration;

- (void)recorderErrorOccur:(AVAudioRecorder *)recorder error:(NSError *) error;

- (void)recorderFinishedButDurationTooShort;

@end

@interface AudioRecorder : NSObject <AVAudioRecorderDelegate>

@property(nonatomic, copy) NSString *saveFolder;
@property(nonatomic, assign) id<AudioRecorderDelegate> delegate;
@property(nonatomic, assign) NSUInteger minDuration;

- (void)startRecord:(CallbackBlock)callback;

- (void)stopRecord;
- (void)cancelRecord;

@end
