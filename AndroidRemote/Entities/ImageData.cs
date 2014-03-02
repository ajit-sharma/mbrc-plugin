namespace MusicBeePlugin.AndroidRemote.Entities
{
    /// <summary>
    /// Class ImageData. Represents an image that will get transferred through the socket.
    /// </summary>
    class ImageData
    {
        private string _coverhash;
        private string _image;
        private string _album_id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageData"/> class.
        /// </summary>
        /// <param name="coverhash">The sha1 hash of the image</param>
        /// <param name="image">The base64 encoded image</param>
        public ImageData(string coverhash, string image)
        {
            _coverhash = coverhash;
            _image = image;
        }

        /// <summary>
        /// Gets or sets the SHA1 hash of the cover
        /// </summary>
        /// <value>SHA1 hash</value>
        public string coverhash
        {
            get { return _coverhash; }
            set { _coverhash = value; }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>Base64 encoded image data</value>
        public string image
        {
            get { return _image; }
            set { _image = value; }
        }

        /// <summary>
        /// Gets or sets the album_id.
        /// </summary>
        /// <value>The album_id.</value>
        public string album_id
        {
            get { return _album_id; }
            set { _album_id = value; }
        }
    }
}
