using System.Collections.Generic;

namespace PDFSharpTest.Console.Models
{
    public class TestModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TestDecorationModel TestDecoration { get; set; }
        public List<TestPageModel> TestPages { get; set; }
    }

    public class TestPageModel
    {
        public string Id { get; set; }
        public List<TestColumnModel> TestColumns { get; set; }
    }

    public class TestColumnModel
    {
        public string Id { get; set; }
        public List<McQuestionModel> McQuestions { get; set; }
    }

    public class McQuestionModel
    {
        public string Id { get; set; }
        public string Prompt { get; set; }
        public float Height { get; set; }

        public int NumberOfOptions { get; set; }
        public int CorrectOption { get; set; }
        public int OptionsPerRow { get; set; }
        public List<QuestionOptionModel> QuestionOptions { get; set; }
    }

    public class QuestionOptionModel
    {
        public string Id { get; set; }
        public string Prompt { get; set; }
    }

    public class TestDecorationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        #region Page Layout Parameters
        public int ColumnNumber { get; set; }
        public float TopMargin { get; set; }
        public float SideMargin { get; set; }
        public float ColumnW { get; set; }
        public float ColumnH { get; set; }
        public float QuestionOrderAreaWidth { get; set; }
        public float BiasedMarginLeft { get; set; }
        public float MinWhiteSpaceBetweenQuestions { get; set; }
        public bool LeaveSpaceToColumnEnd { get; set; }
        #endregion

        #region Font Style
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        #endregion

        #region Question Number Style
        public int QuestionNumberFontSize { get; set; }
        public string QuestionNumberColor { get; set; }
        public string QuestionNumberAppendix { get; set; }
        #endregion

        #region Option Style
        public OptionStyle OptionStyle { get; set; }
        public string OptionAppendix { get; set; }
        #endregion

        public string Background { get; set; }

        public string AnswerSheet { get; set; }
    }
    public enum OptionStyle
    {
        /// <summary>
        /// A, B, C, D, E...
        /// </summary>
        UppercaseLetters,

        /// <summary>
        /// a, b, c, d, e...
        /// </summary>
        LowercaseLetters,

        /// <summary>
        /// 1, 2, 3, 4, 5...
        /// </summary>
        NumbersFrom1,

        /// <summary>
        /// I, II, III, IV, V...
        /// </summary>
        RomanNumerals,

        /// <summary>
        /// True, False.
        /// This can only be used if the number of options is 2
        /// </summary>
        TrueFalse,

        /// <summary>
        /// ☐, ☐, ☐, ☐, ☐..
        /// </summary>
        CheckBox,
    }
}
