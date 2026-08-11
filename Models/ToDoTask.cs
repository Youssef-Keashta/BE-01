namespace BE_01.Models
{
    public class ToDoTask
    {
        private static int _nextId = 1;
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Done { get; set; } = false;
        public ToDoTask() {}

        public static ToDoTask FromDatabase(int id, string title, bool done)
        {
            return new ToDoTask { Id = id, Title = title, Done = done };
        }
    }
}
