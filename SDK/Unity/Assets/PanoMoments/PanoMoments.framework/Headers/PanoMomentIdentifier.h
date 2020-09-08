//
//  PanoMomentIdentifier.h
//  PanoMoments
//
//  Created by Yusuf Olokoba on 4/14/19.
//  Copyright © 2019 Moment Capture Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef NS_ENUM (NSUInteger, PanoMomentQuality) {
    SD = 100,
    HD = 101,
    UHD = 102,
};

@interface PanoMomentIdentifier : NSObject
/*!
 * Create an identifier
 */
+ (instancetype) identifierWithPublicApiKey:(NSString*) publicApiKey identifierWithPrivateApiKey:(NSString*) privateApiKey momentID:(NSString*) momentID andQuality:(PanoMomentQuality) quality;

/*!
 * User Public API Key
 */
@property (readonly) NSString* publicApiKey;

/*!
 * User Private API Key
 */
@property (readonly) NSString* privateApiKey;

/*!
 * Moment ID
 */
@property (readonly) NSString* momentID;

/*!
 * Quality
 */
@property (readonly) PanoMomentQuality quality;
@end
