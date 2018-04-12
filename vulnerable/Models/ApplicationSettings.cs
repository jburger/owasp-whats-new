using System.Collections.Generic;

namespace vulnerable.Models {
    public class ApplicationSettings {
        public List<User> Users { get; set; }
        public string GraylogHost { get; set; }
        public int GraylogPort { get; set; }
    }
}