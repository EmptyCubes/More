namespace More.Engine.Model
{
    public class CompilerErrorModel
    {
        public int Column { get; set; }

        public string ErrorText { get; set; }

        public int Line { get; set; }

        public CompilerErrorModel()
        {
        }

        public CompilerErrorModel(string errorText, int line, int column)
        {
            ErrorText = errorText;
            Line = line;
            Column = column;
        }
    }
}