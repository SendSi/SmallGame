#ifndef InAppPurchasePayment_h
#define InAppPurchasePayment_h

@interface InAppPurchasePayment : NSObject
@property(nonatomic, copy, readonly) NSString *productId;
@property(nonatomic, copy, readonly) NSString *orderId;
@property(nonatomic, copy, readonly) NSString *transactionId;
@property(nonatomic, copy, readonly) NSData *receiptData;
@property(nonatomic, assign) int receiptTime;
- (instancetype)initWithParams:(NSString *)productId
                       orderId:(NSString *)orderId
                 transactionId:(NSString *)transactionId
                   receiptData:(NSData *)receiptData
                   receiptTime:(int )receiptTime;
@end

#endif /* InAppPurchasePayment_h */
