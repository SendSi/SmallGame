#import <Foundation/Foundation.h>
#import "InAppPurchaseManager.h"
#import "InAppPurchasePayment.h"

#define ValidateTime 10000

@implementation InAppPurchaseManager

+ (InAppPurchaseManager *)sharedManager
{
    static InAppPurchaseManager *manager = nil;
    static dispatch_once_t pred;
    dispatch_once(&pred, ^{
        manager = [[[self class]alloc]init];
    });
    return manager;
}

- (void)onInitialized
{
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
}

- (void)onDestroyed
{
    [self reset];
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

- (void)buy:(NSString *)productId
    orderId:(NSString *)orderId
{
    if ([self hasPayment]) {
        [self notifyPaymentFailed:PaymentErrorWaitForLastPayment];
        return;
    }
    if (![SKPaymentQueue canMakePayments]) {
        [self notifyPaymentFailed:PaymentErrorInAppPurchaseNotEnabled];
        return;
    }
    
    _orderId = orderId;
    _productId = productId;
    [self retrieveProductInformation];
}


#pragma mark - SKPaymentTransactionObserver

// Sent when the transaction array has changed (additions or state changes).  Client should check state of transactions and finish as appropriate.
- (void)paymentQueue:(SKPaymentQueue *)queue
 updatedTransactions:(NSArray<SKPaymentTransaction *> *)transactions
{
    for (SKPaymentTransaction *transaction in transactions) {
        if (transaction == nil) {
            continue;
        }
        __weak SKPayment *payment = [transaction payment];
        BOOL isCurrent = [self isCurrentPayment:payment];
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchasing:
                if (isCurrent) {
                    [self notifyPaymentPurchasing];
                    break;
                }
            case SKPaymentTransactionStateDeferred:
                if (isCurrent) {
                    [self notifyPaymentDeferred];
                }
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self completeTransaction:transaction];
                break;
            default:
                break;
        }
    }
}

// Sent when transactions are removed from the queue (via finishTransaction:).
- (void)paymentQueue:(SKPaymentQueue *)queue
 removedTransactions:(NSArray<SKPaymentTransaction *> *)transactions
{
}

// Sent when an error is encountered while adding transactions from the user's purchase history back to the queue.
- (void)paymentQueue:(SKPaymentQueue *)queue
restoreCompletedTransactionsFailedWithError:(NSError *)error
{
}

// Sent when all transactions from the user's purchase history have successfully been added back to the queue.
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
}

// Sent when the download state has changed.
- (void)paymentQueue:(SKPaymentQueue *)queue
    updatedDownloads:(NSArray<SKDownload *> *)downloads
{
}

#pragma mark - SKProductsRequestDelegate

// Sent immediately before -requestDidFinish:
- (void)productsRequest:(SKProductsRequest *)request
     didReceiveResponse:(SKProductsResponse *)response
{
    if (request == nil || request != _productsRequest) {
        return;
    }
    if (response == nil
        || [response products] == nil
        || [[response products]count] != 1) {
        [self notifyPaymentFailed:PaymentErrorRetrieveProductInformation];
        return;
    }
    _productsRequest = nil;
    SKProduct *product = [[response products]objectAtIndex:0];
    if (product == nil
        || ![[product productIdentifier]isEqualToString:_productId]) {
        [self notifyPaymentFailed:PaymentErrorRetrieveProductInformation];
        return;
    }
    [self submitPaymentRequest:product];
}

#pragma mark - private

- (instancetype)init
{
    self = [super init];
    if (self == nil) {
        return nil;
    }
    [self reset];
    return self;
}

- (void)reset
{
    _userId = 0;
    _productId = nil;
    _orderId = nil;
    _payment = nil;
    if (_productsRequest != nil) {
        [_productsRequest setDelegate:nil];
        [_productsRequest cancel];
        _productsRequest = nil;
    }
}

- (BOOL)hasPayment
{
    return _userId != 0
    || _productId != nil
    || _orderId != nil
    || _payment != nil
    || _productsRequest != nil;
}

- (BOOL)isCurrentPayment:(SKPayment *)payment
{
    return [[payment productIdentifier]isEqualToString:_productId]
    && [[payment applicationUsername]isEqualToString:_orderId];
}


- (void)retrieveProductInformation
{
    _productsRequest = [[SKProductsRequest alloc]initWithProductIdentifiers:[NSSet setWithObject:_productId]];
    if (_productsRequest == nil) {
        [self notifyPaymentFailed:PaymentErrorRetrieveProductInformation];
        return;
    }
    [_productsRequest setDelegate:self];
    [_productsRequest start];
}

- (void)submitPaymentRequest:(SKProduct *)product
{
    if (product == nil
        || ![[product productIdentifier]isEqualToString:_productId]
        || _orderId == nil || [_orderId length] == 0) {
        [self notifyPaymentFailed:PaymentErrorRetrieveProductInformation];
        return;
    }
    SKMutablePayment *payment = [SKMutablePayment paymentWithProduct:product];
    if (payment == nil) {
        [self notifyPaymentFailed:PaymentErrorSubmitPayment];
        return;
    }
    [payment setApplicationUsername:_orderId];
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction
{
    if (transaction == nil) {
        return;
    }
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    if ([self isCurrentPayment:[transaction payment]]) {
        [self notifyPaymentFailed:PaymentErrorCancelled];
    }
}

- (void)completeTransaction:(SKPaymentTransaction *)transaction
{
    if (transaction == nil) {
        return;
    }
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
    NSString *productId = [[transaction payment]productIdentifier];
    NSString *orderId = [[transaction payment]applicationUsername];
    NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData *receiptData = [NSData dataWithContentsOfURL:receiptURL];
    NSData *base64EncodedReceiptData = [receiptData base64EncodedDataWithOptions:0];
    if (productId == nil || [productId length] == 0
        || orderId == nil || [orderId length] == 0
        || [transaction transactionIdentifier] == nil || [[transaction transactionIdentifier] length] == 0
        || receiptURL == nil || receiptData == nil || base64EncodedReceiptData == nil) {
        if ([self isCurrentPayment:[transaction payment]]) {
            [self notifyPaymentFailed:PaymentErrorDefault];
        }
        return;
    }
    InAppPurchasePayment *payment = [[InAppPurchasePayment alloc]initWithParams:productId
                                                                        orderId:orderId
                                                                  transactionId:[transaction transactionIdentifier]
                                                                    receiptData:base64EncodedReceiptData
                                                                    receiptTime:ValidateTime];
    if (payment == nil) {
        if ([self isCurrentPayment:[transaction payment]]) {
            [self notifyPaymentFailed:PaymentErrorDefault];
        }
        return;
    }
    if ([self isCurrentPayment:[transaction payment]]) {
        _payment = payment;
        [self notifyPaymentPurchased];
    }
}


- (void)notifyPaymentPurchasing
{
    if ([self delegate]) {
        [[self delegate]onPaymentPurchasing];
    }
}

- (void)notifyPaymentDeferred
{
    if ([self delegate]) {
        [[self delegate]onPaymentDeferred];
    }
}

- (void)notifyPaymentPurchased
{
    if ([self delegate]) {
        [[self delegate]onPaymentPurchased:_payment];
    }
    [self reset];
}

- (void)notifyPaymentFailed:(PaymentError)error
{
    if ([self delegate]) {
        [[self delegate]onPaymentFailed:error];
    }
    [self reset];
}

@end
