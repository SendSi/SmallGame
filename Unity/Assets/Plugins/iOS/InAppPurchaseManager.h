#ifndef InAppPurchaseManager_h
#define InAppPurchaseManager_h

#import <StoreKit/StoreKit.h>
#import "InAppPurchasePayment.h"

typedef NS_ENUM(NSUInteger, PaymentError)
{
    PaymentErrorDefault = 1,
    PaymentErrorNetwork = 2,
    PaymentErrorCancelled = 3,
    PaymentErrorWaitForLastPayment = 4,
    PaymentErrorInAppPurchaseNotEnabled = 5,
    PaymentErrorRetrieveProductInformation = 6,
    PaymentErrorSubmitPayment = 7,
    PaymentErrorWaitForDelivery = 8,
};

@protocol InAppPurchaseManagerDelegate <NSObject>
@required
- (void)onPaymentPurchasing;
- (void)onPaymentDeferred;
- (void)onPaymentPurchased:(InAppPurchasePayment *)payment;
- (void)onPaymentFailed:(PaymentError)error;
@end

@interface InAppPurchaseManager : NSObject <
    SKPaymentTransactionObserver,
    SKProductsRequestDelegate>
{
@private
    NSUInteger _userId;
    InAppPurchasePayment *_payment;
    SKProductsRequest *_productsRequest;
}
@property(nonatomic, weak) id<InAppPurchaseManagerDelegate> delegate;
@property(nonatomic, copy, readonly) NSString *productId;
@property(nonatomic, copy, readonly) NSString *orderId;
+ (InAppPurchaseManager *)sharedManager;
- (void)onInitialized;
- (void)onDestroyed;
- (void)buy:(NSString *)productId
    orderId:(NSString *)orderId;
@end

#endif /* InAppPurchaseManager_h */
