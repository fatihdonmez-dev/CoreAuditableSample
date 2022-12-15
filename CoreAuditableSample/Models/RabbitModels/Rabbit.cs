namespace CoreAuditableSample.Models.RabbitModels
{
    public class Rabbit
    {
        public enum QueueList
        {
            MainPageCreator = 1,
            //CacheCleaner = 2,
            VideoOperation = 3,
            ImageTrigger = 4,
            InProgressOperations = 5,
            CropSound = 6,
            ConvertM4a = 7
        }

        public class InProgressOperationObject
        {
            /// <summary>
            /// DB seçimi için
            /// </summary>
            public Guid WebSiteId { get; set; }
            /// <summary>
            /// "ArticleUpdate", "VideoUpdate", ...
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// "ArticleId", "VideoId", ...
            /// </summary>
            public Guid RelationId { get; set; }
            /// <summary>
            /// Geri bildirim göndermek için kullanılır
            /// </summary>
            public Guid UserId { get; set; }
            /// <summary>
            /// Aktarılmak istenen diğer objeler içindir; kategori idleri, tarih, vs.
            /// </summary>
            public string ExtraObjectJson { get; set; }
        }
        public class CacheListExtra
        {
            public List<string> RequestUrlList;
            public List<string> UrlList;
            public List<string> CDNUrlList;
            public List<string> DataCacheList;
            public string VirtualPath;
            public string Host;
            /// <summary>
            /// DataCache keylerinin ters hallerinin de düşürülmesini sağlar
            /// </summary>
            public bool DeleteReverseKeys;
        }
    }
}
