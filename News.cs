using System;
using System.Collections.Generic;
using System.Text;

namespace TestBOTForPGTU
{
    public class News
    {
        public class NewsView
        {

            private static NewsView instance;
            public static NewsView Instance()
            {
                if (instance == null)
                {
                    instance = new NewsView();
                }
                return instance;
            }
            public static String PrintNews()
            {
                String tmpads = "TEst NEWS";
                return tmpads;
            }
            public string Annotation { get; set; }
            public int AnnouncementId { get; set; }
            public string Author { get; set; }
            public DateTime DateBegin { get; set; }
            public DateTime DateEnd { get; set; }
            public string Details { get; set; }
            public int DictAnnouncementTypeId { get; set; }
            public string Title { get; set; }

        }    
    }
}
