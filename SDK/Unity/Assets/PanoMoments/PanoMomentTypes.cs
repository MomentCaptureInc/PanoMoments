/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments {

    public struct Identifier {
		public readonly string public_api_key;
		public readonly string private_api_key;
        public readonly string momentID;
        public readonly Quality quality;

		public Identifier (string public_api_key, string private_api_key, string momentID, Quality quality = Quality.SD) {
			this.public_api_key = public_api_key;
			this.private_api_key = private_api_key;
            this.momentID = momentID;
            this.quality = quality;
        }
    }

    public enum Quality : int {
        SD = 100,
        HD = 101,
        UHD = 102
    }
}