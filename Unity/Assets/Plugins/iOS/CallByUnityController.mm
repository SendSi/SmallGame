#import "CallByUnityController.h"
#import "AppDelegateListener.h"
#import <CoreLocation/CoreLocation.h>
#import "PruneImgViewController.h"
#import "InAppPurchaseManager.h"
#import "IMManager.h"
#import "AudioRecorder.h"
#import "AudioPlayer.h"
#import "Ntalker.h"

#if defined(__cplusplus)
extern"C"{
#endif
	extern void UnitySendMessage(const char *,const char *,const char *);
	extern NSString* _CreateNSString (const char* string);
#if defined(__cplusplus)
}
#endif

enum NativeCallType
{
    GetSafeArea = 1,            //获取异形屏设备信息
    AwakeByURL = 2,             //URL唤醒
    GetGPS = 3,                 //获取GPS定位
    TakeHeadImage = 4,          //获取头像图片
    InAppPurchase = 5,          //iOS内购
    
    IMMes = 100,                //牌局的聊天信息
    IMOpearteMes = 101,          //IM操作信息
    IMServerMes = 200,          //服务器发送来的信息
};

@interface CallByUnityController()<CLLocationManagerDelegate,PruneImgViewControllerDelegate,InAppPurchaseManagerDelegate,AudioRecorderDelegate,IMManagerDelegate>{
    CLLocationManager * _locMgr;
    AudioRecorder *_audioRecorder;
    NSTimeInterval _recordTime;
}

@end

@implementation CallByUnityController
//打开相册选择照片时响应的方法
-(void) imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
	//NSLog(@"完成照片选择");
    
	//关闭相册
    [picker dismissViewControllerAnimated:YES completion:^{
        UIImage *image = [info objectForKey:UIImagePickerControllerOriginalImage];
        //prune image
        PruneImgViewController *vc = [[PruneImgViewController alloc] initWithNibName:@"PruneImgViewController"
                                                                            delegate:self
                                                                               image:image];
        UINavigationController *nav = [[UINavigationController alloc]initWithRootViewController:vc];
        [self presentViewController:nav animated:YES completion:nil];
    }];
}


#pragma mark - PruneImgViewControllerDelegate
- (void)pruneImgViewController:(PruneImgViewController *)controller didFinishWithImage:(UIImage *)image {
    UIImage* scaleImage = [self scaleImage:image];
    
    //获取保存路径
    NSString *imagePath = [self GetSavaPath:@"temp.jpg"];
    //得到 image 然后用你的函数传回 u3d
    NSData *imgData;
    if(UIImagePNGRepresentation(scaleImage) == nil)
    {
        imgData = UIImageJPEGRepresentation(image,1.0);
    }
    else
    {
        imgData = UIImagePNGRepresentation(scaleImage);
    }
    [imgData writeToFile:imagePath atomically:YES];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)TakeHeadImage, imagePath];
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
    
    [self dismissViewControllerAnimated:YES completion:^{
        [self.view removeFromSuperview];
    }];
}

-(UIImage*)scaleImage:(UIImage*)image
{
    UIGraphicsBeginImageContext(CGSizeMake(320, 320));
    [image drawInRect:CGRectMake(0, 0, 320, 320)];
    UIImage *scaledImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return scaledImage;
}

//获取保存路径
-(NSString *)GetSavaPath:(NSString *) filename{
    NSArray *pathArray = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *docPath = [pathArray objectAtIndex:0];
    
    return  [docPath stringByAppendingPathComponent:filename];
}

-(void) SavaFileToDoc:(UIImage *)image path:(NSString *)path{
	
}

//打开相册后点击取消的响应
-(void) imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
	//NSLog(@"--imagePickerControllerDidCancel !!");
	[self dismissViewControllerAnimated:YES completion:nil];
}
-(void)OpenTarget:(UIImagePickerControllerSourceType)type{
	UIImagePickerController *picker;
	picker = [[UIImagePickerController alloc] init];

	picker.delegate = self;
	picker.allowsEditing = NO;
	picker.sourceType = type;

	[self presentViewController:picker animated:YES completion:nil];
}

#pragma mark - 获取SafeArea
-(void)getDeviceSafeArea
{
    UIWindow *window = [UIApplication sharedApplication].keyWindow;
    UIEdgeInsets safeInsets;
    if (@available(iOS 11.0, *)) {
        safeInsets = window.safeAreaInsets;
    } else {
        safeInsets = UIEdgeInsetsZero;
    }
    if (safeInsets.top == 0) {
        safeInsets.top = [[UIApplication sharedApplication] statusBarFrame].size.height;
    }
    NSString *safeAreaJson = [NSString stringWithFormat:@"{\\\"top\\\":%d,\\\"bottom\\\":%d,\\\"left\\\":%d,\\\"right\\\":%d,\\\"width\\\":%d}",(int)safeInsets.top,(int)safeInsets.bottom,(int)safeInsets.left,(int)safeInsets.right,(int)window.bounds.size.width];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)GetSafeArea, safeAreaJson];
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
}

#pragma mark - URL唤醒
- (void)initAwakeByURLObserver{
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleAwakeByURL:) name:kUnityOnOpenURL object:nil];
    
}

- (void)handleAwakeByURL:(NSNotification *)notification{
    NSDictionary *info = notification.userInfo;
    NSURL *url = info[@"url"];
    if (url && [url isKindOfClass:[NSURL class]]) {
        NSString *urlStr = url.absoluteString;
        if ([urlStr containsString:@"crazypoker://"]) {
            NSString *paramas = [urlStr substringFromIndex:[urlStr rangeOfString:@"crazypoker://"].length];
            NSLog(@"urlparams = %@",paramas);
            NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)AwakeByURL, paramas];
            UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
        }
    }
}

#pragma mark - 获取GPS定位信息
- (void)GetCurrentLocation{
    if (!_locMgr) {
        _locMgr = [[CLLocationManager alloc] init];
        [_locMgr setDesiredAccuracy:kCLLocationAccuracyBest];
        _locMgr.delegate = self;
    }
    [_locMgr requestWhenInUseAuthorization];
    [_locMgr startUpdatingLocation];
}

- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    int status = 1;
    if ([error code] == kCLErrorDenied)
    {
        status = 1;
    }else{
        status = 2;
    }
    NSString *gpsInfoJson = [NSString stringWithFormat:@"{\\\"latitude\\\":%f,\\\"longitude\\\":%f,\\\"locationName\\\":\\\"%@\\\",\\\"status\\\":%d}",0.f,0.f,@"",status];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)GetGPS, gpsInfoJson];
    NSLog(@"aaa=%@",resultContent);
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
}


- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray<CLLocation *> *)locations
{
    CLLocation *location = [locations lastObject];
    CLLocationCoordinate2D coordinate = location.coordinate;
    
    [manager stopUpdatingLocation];
    
    
    CLGeocoder *geocoder = [[CLGeocoder alloc]init];
    [geocoder reverseGeocodeLocation:location completionHandler:^(NSArray<CLPlacemark *> *_Nullable placemarks, NSError * _Nullable error) {
        NSString *locationName = @"";
        if (placemarks.count) {
            CLPlacemark *place = placemarks.firstObject;
            NSLog(@"");
            locationName = [NSString stringWithFormat:@"%@，%@%@%@%@%@%@",place.name,place.country,place.administrativeArea,place.locality,place.subLocality,place.thoroughfare,place.subThoroughfare];
        }
        
        NSString *gpsInfoJson = [NSString stringWithFormat:@"{\\\"latitude\\\":%f,\\\"longitude\\\":%f,\\\"locationName\\\":\\\"%@\\\",\\\"status\\\":%d}",coordinate.latitude,coordinate.longitude,locationName,0];
        NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)GetGPS, gpsInfoJson];
        UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
    }];
}

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)status
{
    
}

#pragma mark - 内购
- (void)InAppPurchase:(NSString *)productID orderID:(NSString *)orderID{
    if (productID && [productID isKindOfClass:[NSString class]] && orderID && [orderID isKindOfClass:[NSString class]]){
        InAppPurchaseManager *manager = [InAppPurchaseManager sharedManager];
        [manager onInitialized];
        [manager setDelegate:self];
        [manager buy:productID orderId:orderID];
    }

}

- (void)onPaymentPurchasing
{
}

- (void)onPaymentDeferred
{
}

- (void)onPaymentPurchased:(InAppPurchasePayment *)payment
{
    [[InAppPurchaseManager sharedManager] onDestroyed];
    //支付成功，回调Unity较验
    NSString *receiptStr = [[NSString alloc]initWithData:[payment receiptData] encoding:NSUTF8StringEncoding];
    NSString *purchaseInfoJson = [NSString stringWithFormat:@"{\\\"status\\\":%d,\\\"productId\\\":\\\"%@\\\",\\\"orderId\\\":\\\"%@\\\",\\\"transactionId\\\":\\\"%@\\\",\\\"receiptData\\\":\\\"%@\\\"}",0,payment.productId,payment.orderId,payment.transactionId,receiptStr];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)InAppPurchase, purchaseInfoJson];
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
}

- (void)onPaymentFailed:(PaymentError)error
{
    [[InAppPurchaseManager sharedManager] onDestroyed];
    NSString *purchaseInfoJson = [NSString stringWithFormat:@"{\\\"status\\\":%lu,\\\"productId\\\":\\\"%@\\\",\\\"orderId\\\":\\\"%@\\\",\\\"transactionId\\\":\\\"%@\\\",\\\"receiptData\\\":\\\"%@\\\"}",(unsigned long)error,@"",@"",@"",@""];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)InAppPurchase, purchaseInfoJson];
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
}

#pragma mark - 录音相关
- (void)StartRecord{
    // 录音
    if (_audioRecorder == nil){
        _audioRecorder = [[AudioRecorder alloc] init];
        _audioRecorder.saveFolder = NSTemporaryDirectory();
        _audioRecorder.delegate = self;
    }
    [_audioRecorder startRecord:^(BOOL started, BOOL isNoPermission) {
        if (isNoPermission) {
            UIAlertController *alertVC = [UIAlertController alertControllerWithTitle:nil message:@"请到设置打开录音权限" preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"好的" style:UIAlertActionStyleCancel handler:^(UIAlertAction *action) {
            }];
            [alertVC addAction:cancelAction];
            [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:alertVC animated:YES completion:nil];
        }
    }];
}

- (long)stopRecord {
    if (_audioRecorder != nil) {
        [_audioRecorder stopRecord];
        if (_recordTime < _audioRecorder.minDuration) {
            return 0;
        }
        return _recordTime * 1000;
    } else {
        return 0;
    }
}

- (void)cancelRecord {
    if (_audioRecorder != nil) {
        [_audioRecorder cancelRecord];
    }
}

/**
 * 录音状态更新
 * @param recordedTime 已经录制时间 秒为单位
 * @param rate 声音大小频率 0.2 ~ 1.0 之间
 */
- (void)recorderUpdate:(NSTimeInterval)recordedTime AndRate:(CGFloat)rate{
    _recordTime = recordedTime;
}
/**
 * 录制完成
 * @param recorder
 * @param flag  是否成功
 * @param path  路径
 * @param duration 长度秒数
 */
- (void)recorderFinished:(AVAudioRecorder *)recorder successfuly:(BOOL) flag path:(NSURL *)path duration:(NSTimeInterval) duration{
    [[IMManager sharedInstance] sendMessage:[IMManager newVoiceMessage:path.path duration:duration]];
}

- (void)recorderErrorOccur:(AVAudioRecorder *)recorder error:(NSError *) error{
    
}

- (void)recorderFinishedButDurationTooShort{
    
}

//IM相关
#pragma mark - IMManagerDelegate
- (void)onReceiveSystemMessage:(TIMMessage *)message{
    TIMElem * elem = [message getElem:0];
    if ([elem isKindOfClass:[TIMTextElem class]]) {
        TIMTextElem *textElem = (TIMTextElem *)elem;
        NSString *mesContent = textElem.text;
        mesContent = [mesContent stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
        NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)IMServerMes, mesContent];
        UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
        NSLog(@"aaa=%@",resultContent);
    }
}
- (void)onReceiveGroupMessage:(TIMMessage *)message{
    NSString *sender = message.sender;
    
    TIMElem * elem = [message getElem:0];
    if ([elem isKindOfClass:[TIMSoundElem class]]) {
        // voice message
        TIMSoundElem *soundElem = (TIMSoundElem *)elem;
        long duration = soundElem.second * 1000;
        
        //播放语音
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES);
        NSString *cache = [paths objectAtIndex:0];
        NSString *audioDir = [NSString stringWithFormat:@"%@/sound",cache];
        BOOL isDir = FALSE;
        BOOL isDirExist = [[NSFileManager defaultManager] fileExistsAtPath:audioDir isDirectory:&isDir];
        if (!(isDir && isDirExist))
        {
            BOOL isCreateDir = [[NSFileManager defaultManager] createDirectoryAtPath:audioDir withIntermediateDirectories:YES attributes:nil error:nil];
            if (!isCreateDir) {
                return;
            }
        }
        NSString *path = [NSString stringWithFormat:@"%@/%@",audioDir,soundElem.uuid];
        if ([[NSFileManager defaultManager] fileExistsAtPath:path]) {
            NSLog(@"AudioPlayer imVoice in local: %@", path);
            [self SendUnityIMMessage:sender mesType:1 mesContent:path duration:duration];
        }
        else {
            [soundElem getSound:path succ:^{
                [self SendUnityIMMessage:sender mesType:1 mesContent:path duration:duration];
            } fail:^(int code, NSString *msg) {
                
            }];
        }
        
    }else if ([elem isKindOfClass:[TIMTextElem class]]) {
        // text message
        TIMTextElem *textElem = (TIMTextElem *)elem;
        [self SendUnityIMMessage:sender mesType:0 mesContent:textElem.text duration:0];
    }
    

}

- (void)SendUnityIMMessage:(NSString *)sender mesType:(int)mesType mesContent:(NSString *)mesContent duration:(long)duration{
    mesContent = [mesContent stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\\\\\""];
    NSString *imInfoJson = [NSString stringWithFormat:@"{\\\"sender\\\":\\\"%@\\\",\\\"mesType\\\":%d,\\\"mesContent\\\":\\\"%@\\\",\\\"duration\\\":%ld}",sender,mesType,mesContent,duration];
    NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)IMMes, imInfoJson];
    UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
}

- (void)saveImageToAlbum:(UIImage *)image{
    UIImageWriteToSavedPhotosAlbum(image, self, @selector(image:didFinishSavingWithError:contextInfo:), nil);
}

-(void)image:(UIImage *)image didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo {
    NSString *msg = nil ;
    if(error){
        msg = @"保存图片失败" ;
    }else{
        msg = @"保存图片成功" ;
    }
    UIAlertController *alertVC = [UIAlertController alertControllerWithTitle:nil message:msg preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"好的" style:UIAlertActionStyleCancel handler:^(UIAlertAction *action) {
    }];
    [alertVC addAction:cancelAction];
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:alertVC animated:YES completion:nil];
}

@end

#if defined(__cplusplus)
extern "C"{
#endif

	static CallByUnityController *iosCallByUnityController;

	//字符串转化的工具函数

	NSString* _CreateNSString (const char* string)
	{
	    if(string)
			return [NSString stringWithUTF8String: string];
	    else
			return [NSString stringWithUTF8String: " "];
	}
	
	//打开照片
	void _OpenPhotoLibrary()
	{
		if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary])
		{
			if(iosCallByUnityController == NULL)
			{
				iosCallByUnityController = [[CallByUnityController alloc] init];
			}
			UIViewController *vc = UnityGetGLViewController();
			[vc.view addSubview : iosCallByUnityController.view];

			[iosCallByUnityController OpenTarget:UIImagePickerControllerSourceTypePhotoLibrary];
		}
		else
		{
			//UnitySendMessage("AndroidCallBack","PhotoGalleryHandler",(@"").UTF8String);
		}
    }

	//打开相册
	void _OpenPhotoAblums()
	{
	   if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum])
	   {
			if(iosCallByUnityController == NULL)
			{
				iosCallByUnityController = [[CallByUnityController alloc] init];
			}
			UIViewController *vc = UnityGetGLViewController();
			[vc.view addSubview: iosCallByUnityController.view];

			[iosCallByUnityController OpenTarget:UIImagePickerControllerSourceTypeSavedPhotosAlbum];
	   }
	   else	
	   {
			_OpenPhotoLibrary();
	   }
	}
    
    //打开相机
    void _OpenCamera()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            if(iosCallByUnityController == NULL)
            {
                iosCallByUnityController = [[CallByUnityController alloc] init];
            }
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: iosCallByUnityController.view];
            
            [iosCallByUnityController OpenTarget:UIImagePickerControllerSourceTypeCamera];
        }
        else
        {
            _OpenPhotoLibrary();
        }
    }
	
	void CallByUnity(const char *jsonContent)
	{
		if(jsonContent)
		{
			NSString *pauseAdJson = [[NSString alloc] initWithUTF8String:jsonContent];
            NSLog(@"jsonContent=%@",pauseAdJson);
			NSData *jsonData = [pauseAdJson dataUsingEncoding:NSUTF8StringEncoding];
			
			NSError *err;
			
			NSDictionary *jsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
			
			if(err)
			{
				NSLog(@"json解析失败: %@",err);
			}
			else
			{
				if(iosCallByUnityController == NULL)
				{
					iosCallByUnityController = [[CallByUnityController alloc] init];
				}
				NSString *type = [jsonDict objectForKey:@"type"];
				NativeCallType calltype = (NativeCallType)[type intValue];
				id content = [jsonDict objectForKey:@"content"];
                NSLog(@"%@",content);

                
				switch(calltype){
					case GetSafeArea:
						[iosCallByUnityController getDeviceSafeArea];
						break;
                    case AwakeByURL:
                        [iosCallByUnityController initAwakeByURLObserver];
                        break;
                    case GetGPS:
                        [iosCallByUnityController GetCurrentLocation];
                        break;
                    case TakeHeadImage:
                        if ([(NSString *)content isEqualToString:@"1"]) {
                            //拍照
                            _OpenCamera();
                        } else {
                            //相册
                            _OpenPhotoLibrary();
                        }
                        break;
                    case InAppPurchase:
                        NSDictionary *purchaseDic = (NSDictionary *)content;
                        [iosCallByUnityController InAppPurchase:purchaseDic[@"productID"] orderID:purchaseDic[@"orderID"]];
                        break;
					
				}
			}

		}
	}
    
    void IMLogin(const char *userID, const char *token, int appID, const char *systemSender){
        NSString *userIDStr = [[NSString alloc] initWithUTF8String:userID];
        NSString *tokenStr = [[NSString alloc] initWithUTF8String:token];
        NSString *systemSenderStr = [[NSString alloc] initWithUTF8String:systemSender];
        if(iosCallByUnityController == NULL)
        {
            iosCallByUnityController = [[CallByUnityController alloc] init];
        }
        [IMManager sharedInstance].delegate = iosCallByUnityController;
        [[IMManager sharedInstance] setupWithUserID:userIDStr appID:appID token:tokenStr systemSender:systemSenderStr];
    }
    
    void IMLoginOut(){
        [[IMManager sharedInstance] logout];
    }
    
    bool StartRecord(){
        if(iosCallByUnityController == NULL)
        {
            iosCallByUnityController = [[CallByUnityController alloc] init];
        }
        [iosCallByUnityController StartRecord];
        return true;
    }
    
    long StopRecord(bool isSend){
        if(iosCallByUnityController == NULL)
        {
            iosCallByUnityController = [[CallByUnityController alloc] init];
        }
        if (isSend) {
            return [iosCallByUnityController stopRecord];
        } else {
            [iosCallByUnityController cancelRecord];
            return 0;
        }
        
    }
    
    void PlayVoice(const char *path){
        NSString *pathStr = [[NSString alloc] initWithUTF8String:path];
        [[AudioPlayer sharedInstance] play:pathStr];
    }
    
    void StopVoice(){
        [[AudioPlayer sharedInstance] stopPlay];
    }
    
    bool IsPlayingVoice(){
        return [[AudioPlayer sharedInstance] isPlayering];
    }
    
    void SetVolume(float volume){
        [[AudioPlayer sharedInstance] setMute:volume == 0];
    }
    
    void IMSendMessage(const char *str){
        NSString *msgStr = [[NSString alloc] initWithUTF8String:str];
        [[IMManager sharedInstance] sendMessage:[IMManager newTextMessage:msgStr]];
    }
    
    void SetConversation(const char *groupId){
        NSString *groupIdStr = [[NSString alloc] initWithUTF8String:groupId];
        [[IMManager sharedInstance] openConversationWithOppositeId:groupIdStr];
    }
    
    void ClearConversation(){
        [[IMManager sharedInstance] closeConversation];
    }
    
    int GetImState(){
        return [[IMManager sharedInstance] getLoginStatu];
    }
    
    void ApplyJoinGroup(const char *groupId){
        NSString *groupIdStr = [[NSString alloc] initWithUTF8String:groupId];
        [[IMManager sharedInstance] joinGroup:groupIdStr success:^(BOOL success) {
            if (success) {
                NSString *imInfoJson = [NSString stringWithFormat:@"{\\\"opearteType\\\":%d}",0];
                NSString *resultContent = [NSString stringWithFormat:@"{\"type\":%d,\"content\":\"%@\"}", (int)IMOpearteMes, imInfoJson];
                UnitySendMessage([@"NativeManager" UTF8String], [@"NativeCallUnity" UTF8String], [resultContent UTF8String]);
            }
        }];
    }
    
    void UpdateShowStatusBar(bool show){
        NSString *key = @"statusBar";
        UIApplication *object = [UIApplication sharedApplication];
        UIView *statusBar = [[UIView alloc]init];
        if ([object respondsToSelector:NSSelectorFromString(key)]){
            statusBar = [object valueForKey:key];
        }
        statusBar.hidden = !show;
    }
    
    void SaveImageToNative(const void *bytes, long length){
        NSData *picData = [[NSData alloc]initWithBytes:bytes length:length];
        UIImage *image = [UIImage imageWithData:picData];
        if(iosCallByUnityController == NULL)
        {
            iosCallByUnityController = [[CallByUnityController alloc] init];
        }
        [iosCallByUnityController saveImageToAlbum:image];
    }
    
    void KeFuInit(const char *siteid, const char *sdkkey){
        NSString *siteidStr = [[NSString alloc] initWithUTF8String:siteid];
        NSString *sdkkeyStr = [[NSString alloc] initWithUTF8String:sdkkey];
        [[NTalker standardIntegration] initSDKWithSiteid:siteidStr andSDKKey:sdkkeyStr];
    }
    
    int KeFuLogin(const char *userId, const char *userName){
        NSString *userIdStr = [[NSString alloc] initWithUTF8String:userId];
        NSString *userNameStr = [[NSString alloc] initWithUTF8String:userName];
        return (int)[[NTalker standardIntegration] loginWithUserid:userIdStr andUsername:userNameStr andUserLevel:@"0"];
    }
    
    void KeFuLoginOut(){
        [[NTalker standardIntegration] logout];
    }
    
    int StartChat(const char *settingid, const char *groupName, const char *strbody){
        NSString *settingidStr = [[NSString alloc] initWithUTF8String:settingid];
        NSString *groupNameStr = [[NSString alloc] initWithUTF8String:groupName];
        NSString *strbodyStr = [[NSString alloc] initWithUTF8String:strbody];
        strbodyStr = [strbodyStr stringByReplacingOccurrencesOfString:@"\r" withString:@""];
        NSData *jsonData = [strbodyStr dataUsingEncoding:NSUTF8StringEncoding];
        NSLog(@"%@",strbodyStr);
        NSError *err;
        NSDictionary *jsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
        if(err)
        {
            NSLog(@"json解析失败: %@",err);
            NTalkerChatViewController *chat = [[NTalker standardIntegration] startChatWithSettingId:settingidStr];
            chat.title = groupNameStr;
            UINavigationController *nav = [[UINavigationController alloc] initWithRootViewController:chat];
            chat.pushOrPresent = NO;
            [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:nav animated:YES completion:nil];
        }
        else
        {
            NTalkerChatViewController *chat = [[NTalker standardIntegration] startChatWithSettingId:settingidStr];
            chat.title = groupNameStr;
            chat.erpParams = jsonDict[@"erpParam"];
            chat.pageTitle = jsonDict[@"startPageTitle"];
            chat.pageURLString = jsonDict[@"startPageUrl"];
            chat.kefuId = jsonDict[@"kfuid"];
            chat.kefuName = jsonDict[@"kfname"];
            NSString *sendMsg = jsonDict[@"sendMsg"];

            UINavigationController *nav = [[UINavigationController alloc] initWithRootViewController:chat];
            chat.pushOrPresent = NO;
            [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:nav animated:YES completion:^{
                if (sendMsg && [sendMsg isKindOfClass:[NSString class]] && sendMsg.length) {
                    [NTalkerChatViewController sendExtendTextMessage:sendMsg];
                }
            }];

        }

        return 0;
    }

#if defined(__cplusplus)
}
#endif


