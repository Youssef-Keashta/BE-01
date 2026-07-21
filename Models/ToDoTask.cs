namespace BE_01.Models
{
    public class ToDoTask
    {
        private static int _nextId = 1;
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Done { get; set; } = false;
        public ToDoTask(string title)
        {
            this.Id = _nextId++;
            this.Title = title;
            this.Done = false;
        }
    }
}
