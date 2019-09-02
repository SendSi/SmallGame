//
//  PruneImgViewController.m
//  DownJoyTexas-pokes
//
//  Created by baidu on 16/3/21.
//
//

#import "PruneImgViewController.h"

#define SCREEN_WIDTH [[UIScreen mainScreen] bounds].size.width
#define SCREEN_HEIGHT [[UIScreen mainScreen] bounds].size.height

@implementation PruneImgViewController
- (id)initWithNibName:(NSString *)nibNameOrNil
             delegate:(id)delegateParam
                image:(UIImage *)image
{
    self = [super initWithNibName:nibNameOrNil bundle:nil];
    if (self) {
        _delegate = delegateParam;
        _image = [image copy];
        imageView = [[UIImageView alloc] init];
        radius = 80;  //radius of the circle
    }
    return self;
}

-(void)onClickDone:(id)sender{
    [self.delegate pruneImgViewController:self didFinishWithImage:[self circleImage]];
}

- (void)viewDidLoad {
    [super viewDidLoad];
    self.view.clipsToBounds = YES;
    
    UIBarButtonItem *rightItem = [[UIBarButtonItem alloc]initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(onClickDone:)];
    self.navigationItem.rightBarButtonItem = rightItem;
}

- (void)addMask{
    UIView * _maskView = [[UIView  alloc] init];
    imageView.image = _image;
    imageView.contentMode  =  UIViewContentModeScaleAspectFit;

    CGRect rect = self.view.frame;
    [self.view setBackgroundColor:[UIColor blackColor]];
    imageView.frame = self.view.frame;
    [imageView setUserInteractionEnabled:YES];
    [imageView setMultipleTouchEnabled:YES];
    UIPanGestureRecognizer * panGestureRecognizer = [[UIPanGestureRecognizer alloc] initWithTarget:self
                                                                                            action:@selector(doHandlePanAction:)];
    UIPinchGestureRecognizer *pinchGestureRecognizer = [[UIPinchGestureRecognizer alloc] initWithTarget:self action:@selector(pinchView:)];
    [imageView addGestureRecognizer:pinchGestureRecognizer];
    [imageView addGestureRecognizer:panGestureRecognizer];
    
    [_maskView setUserInteractionEnabled:NO];
    [_maskView setMultipleTouchEnabled:NO];

    [self.view addSubview:imageView];
    
    [_maskView setFrame:CGRectMake(0,0,SCREEN_WIDTH,SCREEN_HEIGHT)];
    [_maskView setBackgroundColor:[UIColor colorWithWhite:0 alpha:0.5]];
    [self.view addSubview:_maskView];
    
    //create path
    UIBezierPath *path = [UIBezierPath bezierPathWithRect:CGRectMake(0,0,SCREEN_WIDTH,SCREEN_HEIGHT)];
    // MARK: circlePath
    [path appendPath:[UIBezierPath bezierPathWithArcCenter:CGPointMake(SCREEN_WIDTH / 2, rect.origin.y+rect.size.height/2) radius:80 startAngle:0 endAngle:2*M_PI clockwise:NO]];
    CAShapeLayer *shapeLayer = [CAShapeLayer layer];
    shapeLayer.path = path.CGPath;
    [_maskView.layer setMask:shapeLayer];
}

- (void) pinchView:(UIPinchGestureRecognizer *)pinchGestureRecognizer
{
    UIView *view = pinchGestureRecognizer.view;
    if (pinchGestureRecognizer.state == UIGestureRecognizerStateBegan || pinchGestureRecognizer.state == UIGestureRecognizerStateChanged) {
        CGFloat scale = pinchGestureRecognizer.scale;
        if (scale < 1) {
            CGSize realSizeOld = [self getRealSize];
            CGSize realSize;
            realSize.height = realSizeOld.height * pinchGestureRecognizer.scale;
            realSize.width = realSizeOld.width * pinchGestureRecognizer.scale;
            CGFloat scaleX=0,scaleY=0;
            bool bConflictX=false;
            bool bConflictY=false;
            if(view.center.x > self.view.center.x){//right
                if((view.center.x-realSize.width/2) > (self.view.frame.size.width/2 - radius)){
                    scaleX = (view.center.x - (self.view.center.x - radius))/(realSizeOld.width/2);
                    bConflictX = true;
                }
            }
            else{//left
                if((view.center.x+realSize.width/2) <  (radius+self.view.frame.size.width/2)){
                    scaleX = (self.view.center.x - (view.center.x - radius))/(realSizeOld.width/2);
                    bConflictX = true;
                }
            }
            if (view.center.y < self.view.center.y) { //up
                if((view.center.y+realSize.height/2) < (self.view.frame.size.height/2 +radius)){
                    scaleY = (self.view.center.y-view.center.y+radius)/(realSizeOld.height/2);
                    bConflictY = true;
                }
            }
            else{//down
                if((view.center.y-realSize.height/2) > (self.view.frame.size.height/2 - radius)){
                    scaleY = (view.center.y-self.view.center.y+radius)/(realSizeOld.height/2);
                    bConflictY = true;
                }
            }
            if (bConflictX && bConflictY) {
                if(scaleX > scaleY){
                    scale = scaleY;
                }
                else{
                    scale = scaleX;
                }
            }
            else{
                if(bConflictX){
                    scale = scaleX;
                }
                if (bConflictY) {
                    scale = scaleY;
                }
            }
        }
        view.transform = CGAffineTransformScale(view.transform, scale, scale);
        pinchGestureRecognizer.scale = 1;
    }
}

- (void) doHandlePanAction:(UIPanGestureRecognizer *)paramSender{
    CGSize realSize = [self getRealSize];
    CGPoint point = [paramSender translationInView:self.view];
    if((paramSender.view.center.x+point.x+realSize.width/2) <  (radius+self.view.frame.size.width/2)){
        point.x = (radius+self.view.frame.size.width/2) - (paramSender.view.center.x+realSize.width/2);
    }
    if((paramSender.view.center.x+point.x-realSize.width/2) > (self.view.frame.size.width/2 - radius)){
        point.x = (paramSender.view.center.x-realSize.width/2) - (self.view.frame.size.width/2 - radius);
    }
    if((paramSender.view.center.y+point.y-realSize.height/2) > (self.view.frame.size.height/2 - radius)){
        point.y = (self.view.frame.size.height/2 - radius) - (paramSender.view.center.y-realSize.height/2);
    }
    if((paramSender.view.center.y+point.y+realSize.height/2) < (self.view.frame.size.height/2 +radius)){
        point.y = (self.view.frame.size.height/2 +radius) - (paramSender.view.center.y+realSize.height/2) ;
    }
    paramSender.view.center = CGPointMake(paramSender.view.center.x + point.x, paramSender.view.center.y + point.y);
    [paramSender setTranslation:CGPointMake(0, 0) inView:self.view];
}

- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    [self addMask];
}

- (CGSize)getRealSize{
    CGSize pixelSize = imageView.image.size;
    CGSize imgViewSize = imageView.frame.size;
    CGFloat widthRatio = imgViewSize.width/pixelSize.width;
    CGFloat heightRatio = imgViewSize.height/pixelSize.height;
    CGFloat realWidth,realHeight;
    if(widthRatio > heightRatio){
        realHeight = imgViewSize.height;
        realWidth = heightRatio * pixelSize.width;
    }
    else{
        realHeight = widthRatio * pixelSize.height;
        realWidth = imgViewSize.width;
    }
    return CGSizeMake(realWidth, realHeight);
}

- (UIImage *)fixOrientation:(UIImage*) image {
    if (image.imageOrientation == UIImageOrientationUp) return image;
    CGAffineTransform transform = CGAffineTransformIdentity;
    switch (image.imageOrientation) {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, image.size.width, image.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, image.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, image.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    switch (image.imageOrientation) {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, image.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, image.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    CGContextRef ctx = CGBitmapContextCreate(NULL, image.size.width, image.size.height,
                                             CGImageGetBitsPerComponent(image.CGImage), 0,
                                             CGImageGetColorSpace(image.CGImage),
                                             CGImageGetBitmapInfo(image.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (image.imageOrientation) {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            CGContextDrawImage(ctx, CGRectMake(0,0,image.size.height,image.size.width), image.CGImage);
            break;
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,image.size.width,image.size.height), image.CGImage);
            break;
    }
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img;
}


-(UIImage*) circleImage{
    CGSize realSize = [self getRealSize];
    CGRect rectFrame = self.view.frame;
    CGRect rectImageView = imageView.frame;
    CGSize pixelImg = imageView.image.size;
    CGFloat circleX = (SCREEN_WIDTH/2) -rectImageView.origin.x-(rectImageView.size.width-realSize.width)/2;
    CGFloat circleY = (rectFrame.origin.y+SCREEN_HEIGHT/2)-rectImageView.origin.y-(rectImageView.size.height-realSize.height)/2;
    double ratioWidth = circleX/realSize.width;
    double ratioHeight = circleY/realSize.height;
    double ratioRadius = radius/realSize.width;
    CGRect rect = CGRectMake(ratioWidth*pixelImg.width-pixelImg.width*ratioRadius,ratioHeight*pixelImg.height-pixelImg.width*ratioRadius,2*pixelImg.width*ratioRadius,2*pixelImg.width*ratioRadius);
    CGImageRef imageRef = CGImageCreateWithImageInRect([[self fixOrientation:imageView.image] CGImage], rect);
    UIImage *retImg = [UIImage imageWithCGImage:imageRef scale:1 orientation:UIImageOrientationUp];
    CGImageRelease(imageRef);
    return retImg;
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
}
@end
