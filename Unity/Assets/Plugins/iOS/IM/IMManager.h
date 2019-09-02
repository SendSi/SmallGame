//
// Created by douzifly on 8/8/16.
//

#import <Foundation/Foundation.h>
#import <ImSDK/ImSDK.h>
#import <IMMessageExt/IMMessageExt.h>

@protocol IMManagerDelegate <NSObject>

- (void)onReceiveSystemMessage:(TIMMessage *)message;
- (void)onReceiveGroupMessage:(TIMMessage *)message;

@end

@interface IMManager : NSObject <TIMMessageListener>
{
}

@property (nonatomic, assign) BOOL isLogin;
+(instancetype) sharedInstance;

@property(nonatomic, assign) id<IMManagerDelegate> delegate;

+ (TIMMessage *)newTextMessage:(NSString *) text;
+ (TIMMessage *)newVoiceMessage:(NSString *) path duration:(NSTimeInterval) duration;

/*
 * 设置HISD初始化环境
 */
- (void)setupWithUserID:(NSString *)userID appID:(int)appID token:(NSString *)token systemSender:(NSString *)systemSender;

- (void)logout;

- (void)openConversationWithOppositeId:(NSString *)oppositeId;
- (void) closeConversation;

- (BOOL)sendMessage:(TIMMessage *)message;

- (int)getLoginStatu;

- (void)joinGroup:(NSString *)groupId success:(void(^)(BOOL success))success;

@end
