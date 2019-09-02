#import <Foundation/Foundation.h>
#import "InAppPurchasePayment.h"

@implementation InAppPurchasePayment

- (instancetype)initWithParams:(NSString *)productId
                       orderId:(NSString *)orderId
                 transactionId:(NSString *)transactionId
                   receiptData:(NSData *)receiptData
                   receiptTime:(int)receiptTime
{
    @autoreleasepool {
        if (productId == nil || [productId length] == 0
            || orderId == nil || [orderId length] == 0
            || transactionId == nil || [transactionId length] == 0
            || receiptData == nil || [receiptData length] == 0) {
            // TODO(ARC): [self release]
            return nil;
        }
        self = [super init];
        if (self == nil) {
            NSAssert(NO, @"");
            return nil;
        }
        self->_productId = [productId copy];
        self->_orderId = [orderId copy];
        self->_transactionId = [transactionId copy];
        self->_receiptData = [receiptData copy];
        self->_receiptTime = receiptTime;
        return self;
    }
}

@end
