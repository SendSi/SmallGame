//
//  PruneImgViewController.h
//  DownJoyTexas-pokes
//
//  Created by baidu on 16/3/21.
//
//

#ifndef PruneImgViewController_h
#define PruneImgViewController_h

#import <UIKit/UIKit.h>

@class PruneImgViewController;
@protocol PruneImgViewControllerDelegate <NSObject>

- (void)pruneImgViewController:(PruneImgViewController *)controller didFinishWithImage:(UIImage *)image;

@end

@interface PruneImgViewController : UIViewController
{
    UIImageView *imageView;
    int radius;
@private
    UIImage *_image;
}
@property (assign, nonatomic) id<PruneImgViewControllerDelegate> delegate;

- (id)initWithNibName:(NSString *)nibNameOrNil
             delegate:(id)delegateParam
                image:(UIImage *)image;


@end


#endif /* PruneImgViewController_h */
