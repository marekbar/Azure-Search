using System.Collections.Generic;

namespace Azure_Search
{
    public static class Samples
    {
        public static List<Cat> GenerateSampleCats()
        {
            return new List<Cat>()
                       {
                           new Cat()
                               {
                                   Id = "1",
                                   Name = "Fisker",
                                   Description = "lazy"
                               },
                           new Cat()
                               {
                                   Id = "2",
                                   Name = "White",
                                   Description = ""
                               },
                           new Cat()
                               {
                                   Id = "3",
                                   Name = "Blackie"
                               },
                           new Cat()
                               {
                                   Id = "4",
                                   Name = "Miaoow",
                                   Description = "shoot"
                               }
                       };
        }
    }
}
