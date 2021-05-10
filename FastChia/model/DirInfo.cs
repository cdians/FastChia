namespace FastChia.model
{
    public class DirInfo
    {
        public DirInfo(string name, int count)
        {
            this.name = name;
            this.count = count;
        }

        public string name { get; set; }

        public int count { get; set; }
    }
}
