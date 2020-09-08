//
//  PanoMoment.h
//  Panomoments
//
//  Created by Yusuf Olokoba on 1/14/19.
//  Copyright © 2019 Moment Capture Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import "PanoMomentIdentifier.h"

@class PanoMoment;

typedef void (^ReadyHandler) (PanoMoment* _Nonnull, int width, int height);
typedef void (^LoadedHandler) (PanoMoment* _Nonnull);
typedef void (^FrameHandler) (PanoMoment* _Nonnull, int, id<MTLTexture> _Nonnull);
typedef void (^ErrorHandler) (PanoMoment* _Nonnull, NSError* _Nonnull);

@interface PanoMoment : NSObject
/*!
 * Create a PanoMoment
 */
- (nonnull instancetype) initWithIdentifier:(nonnull PanoMomentIdentifier*) identifier readyHandler:(nonnull ReadyHandler) readyHandler loadedHandler:(nullable LoadedHandler) loadedHandler frameHandler:(nonnull FrameHandler) frameHandler andErrorHandler:(nonnull ErrorHandler) errorHandler;

/*!
 * Dispose the PanoMoment and teardown resources
 */
- (void) dispose;

/*!
 * Render a frame from the PanoMoment
 */
- (void) render:(int) frame;

/*!
 * Number of frames in the PanoMoment
 */
@property (readonly) int frameCount;

/*!
 * Metadata for this PanoMoment
 */
@property (readonly) NSDictionary* _Nullable metadata;
@end
