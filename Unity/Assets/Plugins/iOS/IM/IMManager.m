//
// Created by douzifly on 8/8/16.
//

#import "IMManager.h"

#define kSdkAccountType @"792"

static IMManager *_instance = nil;

@implementation IMManager {
    TIMConversation *_conversation;
    NSString *_userID;
    NSString *_token;
    NSString *_systemSender;
    NSString *_bigGroupID;
    int _appID;
    NSString *_groupID;
}

+ (TIMMessage *)newTextMessage:(NSString *) text {
    TIMMessage *message = [[TIMMessage alloc]init];
    TIMTextElem *elem = [[TIMTextElem alloc]init];
    elem.text = text;
    [message addElem:elem];
    return message;
}

+ (TIMMessage *)newVoiceMessage:(NSString *) path duration:(NSTimeInterval) duration {
    TIMMessage *message = [[TIMMessage alloc]init];
    TIMSoundElem *elem = [[TIMSoundElem alloc]init];
    elem.path = path;
    elem.second = duration;
    [message addElem:elem];
    return message;
}

#pragma mark SINGLETON

+ (instancetype)sharedInstance {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[IMManager alloc] init];
    });

    return _instance;
}

- (instancetype) init {
    self = [super init];
    if (self) {
        _bigGroupID = @"";
    }
    return self;
}

#pragma mark Init


- (void)setupWithUserID:(NSString *)userID appID:(int)appID token:(NSString *)token systemSender:(NSString *)systemSender {
    
    TIMManager * manager = [TIMManager sharedInstance];
    
    [manager setEnv:0];
    
    TIMSdkConfig *config = [[TIMSdkConfig alloc] init];
    config.sdkAppId = appID;
    config.accountType = kSdkAccountType;
    config.disableCrashReport = NO;
    [manager initSdk:config];
    
    TIMUserConfig *userConfig = [[TIMUserConfig alloc] init];
    userConfig.disableRecentContactNotify = YES;
    
    [manager setUserConfig:userConfig];
    
    _userID = userID;
    _appID = appID;
    _token = token;
    _systemSender = systemSender;
    
    [self login];
}



#pragma mark Login

- (void)login {
    TIMLoginParam * login_param = [[TIMLoginParam alloc ]init];
    // identifier为用户名，userSig 为用户登录凭证
    // appidAt3rd 在私有帐号情况下，填写与sdkAppId 一样
    login_param.identifier = _userID;
    login_param.userSig = _token;
    login_param.appidAt3rd = [NSString stringWithFormat:@"%d",_appID];
    
    [[TIMManager sharedInstance] login: login_param succ:^(){
        NSLog(@"Login Succ");
        NSLog(@"===aaanewmsg=====");
        self.isLogin = YES;
        [[TIMManager sharedInstance] addMessageListener:self];
    } fail:^(int code, NSString * err) {
        self.isLogin = NO;
        NSLog(@"Login Failed: %d->%@", code, err);
    }];
}

- (void)logout {
    self.isLogin = NO;
    [[TIMManager sharedInstance] logout:^() {
        NSLog(@"logout succ");
    } fail:^(int code, NSString * err) {
        NSLog(@"logout fail: code=%d err=%@", code, err);
    }];
}


- (int)getLoginStatu{
    // 0-未登录 , 1-登录中, 2已登录
    TIMLoginStatus status = [[TIMManager sharedInstance] getLoginStatus];
    if (status == TIM_STATUS_LOGINED) {
        return 2;
    } else if (status == TIM_STATUS_LOGINING){
        return 1;
    } else {
        return 0;
    }
}

#pragma mark CHAT

- (void)joinGroup:(NSString *)groupId success:(void(^)(BOOL success))success{
    //加入超大广播群聊
    _bigGroupID = groupId;
    [[TIMGroupManager sharedInstance] joinGroup:groupId msg:@"Apply Join Group" succ:^{
        success(YES);
    } fail:^(int code, NSString *msg) {
        success(NO);
    }];
}

- (void)openConversationWithOppositeId:(NSString *)oppositeId{
    _groupID = oppositeId;
    _conversation = [[TIMManager sharedInstance] getConversation:TIM_GROUP receiver:oppositeId];
}

- (void) closeConversation
{
    _conversation = nil;
    _groupID = nil;
}

- (BOOL)sendMessage:(TIMMessage *)message
{
    if (message == nil || _conversation == nil) {
        return NO;
    }
    BOOL isLogin = [IMManager sharedInstance].isLogin;
    if (!isLogin) {
        return NO;
    }
    [_conversation sendMessage:message succ:^{
        [self onNewMessage:@[message]];
        NSLog(@"Send Success");
    } fail:^(int code, NSString *msg) {
        NSLog(@"Send Flaid");
        [self login];
    }];
    return YES;
}


#pragma mark TIMMessageListener
- (void)onNewMessage:(NSArray *)msgs {
    for (TIMMessage *message in msgs) {
        int cnt = [message elemCount];
        if (cnt>0) {
            if ([message.sender isEqualToString:_systemSender] || [message.sender isEqualToString:_bigGroupID]){
                //系统消息
                [self.delegate onReceiveSystemMessage:message];
            } else {
                //房间群聊
                if (_groupID && [message.getConversation.getReceiver isEqualToString:_groupID]) {
                    
                    TIMElem * elem = [message getElem:0];
                    if ([elem isKindOfClass:[TIMSoundElem class]]) {
                        // voice message
                        if (!message.isSelf) { //自己发送的语音不播放
                            [self.delegate onReceiveGroupMessage:message];
                        }
                    }else if ([elem isKindOfClass:[TIMTextElem class]]) {
                        // text message
                        [self.delegate onReceiveGroupMessage:message];
                    }
                }
            }
        }
    }
    
}
@end
