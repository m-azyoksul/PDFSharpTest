using System;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PDFSharpTest.Console.Models;

namespace PDFSharpTest.Console
{
    class Program
    {
        /// <summary>
        /// Page width value that PDFSharp uses by default.
        /// Test decoration calculates dimensions using PDFSharp defaults as well.
        /// TODO: Abstract away
        /// </summary>
        const float pageW = 595f;

        /// <summary>
        /// Page height value that PDFSharp uses by default.
        /// Test decoration calculates dimensions using PDFSharp defaults as well.
        /// TODO: Abstract away
        /// </summary>
        const float pageH = 842f;

        static void Main(string[] args)
        {
            var debug = true;

            // Get the test
            var test = GetATestModel();

            // Validate: There are pages to print
            if (test.TestPages == null || test.TestPages.Count == 0) return;

            // Shorthand notation for test decoration
            var d = test.TestDecoration;

            // Calculate middle margin
            var middleMargin = (pageW - test.TestPages[0].TestColumns.Count * d.ColumnW - 2 * d.SideMargin) / (test.TestPages[0].TestColumns.Count - 1);

            // Validate: Middle margin is not negative
            if (middleMargin < 0) return;

            // Validate: Bottom margin is not negative
            if (pageH - d.TopMargin - d.ColumnH < 0) return;

            // Register encoding provider.
            // This is necessary for the PDFSharp to work properly.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Create doc
            var document = new PdfDocument();

            // For each page
            test.TestPages.ForEach(testPage =>
            {
                // Add page
                var page = document.AddPage();

                // Define necessary stuff
                var graphics = XGraphics.FromPdfPage(page);
                var textFormatter = new XTextFormatter(graphics);

                // Text properties
                var questionOrderFont = new XFont(d.FontFamily, d.QuestionNumberFontSize, XFontStyle.Regular);
                var questionOrderColor = XBrushes.MediumPurple;
                var font = new XFont(d.FontFamily, d.FontSize, XFontStyle.Regular);
                var textColor = XBrushes.Black;

                // Draw top margin
                if (debug)
                    graphics.DrawRectangle(XPens.Blue,
                    0,
                    0,
                    page.Width,
                    d.TopMargin);

                // Draw bottom margin
                if (debug)
                    graphics.DrawRectangle(XPens.Blue,
                    0,
                    d.TopMargin + d.ColumnH,
                    page.Width,
                    page.Height - (d.TopMargin + d.ColumnH));

                // Calculate X coordinates of columns
                var columnXs = testPage.TestColumns
                    .Select((tc, index) => d.BiasedMarginLeft + d.SideMargin + index * (d.ColumnW + middleMargin))
                    .ToList();

                // Draw columns
                if (debug)
                    columnXs.ForEach(columnX =>
                    {
                        graphics.DrawRectangle(XPens.Orange,
                            columnX,
                            d.TopMargin,
                            d.ColumnW,
                            d.ColumnH);
                    });

                // Draw middle line
                graphics.DrawLine(XPens.Black,
                    pageW / 2,
                    d.TopMargin,
                    pageW / 2,
                    d.TopMargin + d.ColumnH);

                //XBrush brush = new XLinearGradientBrush(new XPoint(0, 0), new XPoint(50, 20), XColors.Blue, XColors.Green);
                //graphics.DrawRectangle(XPens.Black,
                //    brush,
                //    pageW / 2,
                //    d.TopMargin,
                //    .25,
                //    d.ColumnH);

                // For each column
                for (int i = 0; i < testPage.TestColumns.Count; i++)
                {
                    for (int j = 0; j < testPage.TestColumns[i].McQuestions.Count; j++)
                    {
                        // Type question order
                        var questionOrderRect = new XRect(
                            columnXs[i],
                            d.TopMargin + j * 200,
                            d.QuestionOrderAreaWidth,
                            200);
                        textFormatter.DrawString($"{j + 1}{d.QuestionNumberAppendix}", questionOrderFont, questionOrderColor, questionOrderRect, XStringFormats.TopLeft);

                        // Type question prompt
                        var questionPromptRect = new XRect(
                            columnXs[i] + d.QuestionOrderAreaWidth,
                            d.TopMargin + j * 200,
                            d.ColumnW - d.QuestionOrderAreaWidth,
                            100);
                        textFormatter.DrawString(testPage.TestColumns[i].McQuestions[j].Prompt, font, textColor, questionPromptRect, XStringFormats.TopLeft);

                        for (int k = 0; k < testPage.TestColumns[i].McQuestions[j].QuestionOptions.Count; k++)
                        {
                            // Type question option prompt
                            var optionPromptRect = new XRect(
                                columnXs[i] + d.QuestionOrderAreaWidth,
                                d.TopMargin + j * 200 + 100 + k * 20,
                                d.ColumnW - d.QuestionOrderAreaWidth,
                                20);
                            var optionText = $"{OptionPretext(d.OptionStyle, k)}{d.OptionAppendix} {testPage.TestColumns[i].McQuestions[j].QuestionOptions[k].Prompt}";
                            textFormatter.DrawString(optionText, font, textColor, optionPromptRect, XStringFormats.TopLeft);
                        }
                    }

                    //var imagePath = "image.png";
                    //graphics.DrawImage(XImage.FromFile(imagePath), columnXs[i] + 1, d.TopMargin + 100, d.ColumnW - 2, d.ColumnW - 2);
                }
            });

            // Save PDF
            document.Save("HelloWorld.pdf");
        }
        private static TestModel GetATestModel()
        {
            return new TestModel
            {
                Name = "Test 1",
                TestDecoration = new TestDecorationModel
                {
                    Name = "Awesome Decorations",
                    ColumnNumber = 2,
                    TopMargin = 100,
                    SideMargin = 40,
                    ColumnW = 235,
                    ColumnH = 680,
                    QuestionOrderAreaWidth = 20,
                    BiasedMarginLeft = 0,

                    FontFamily = "Verdana",
                    FontSize = 13,

                    QuestionNumberFontSize = 13,
                    QuestionNumberColor = "000000",
                    QuestionNumberAppendix = ".",

                    OptionStyle = OptionStyle.RomanNumerals,
                    OptionAppendix = ")",
                },
                TestPages = Enumerable.Range(0, 3).Select(_ => new TestPageModel
                {
                    TestColumns = Enumerable.Range(0, 2).Select(_ => new TestColumnModel
                    {
                        McQuestions = Enumerable.Range(0, 2).Select(_ => new McQuestionModel
                        {
                            Prompt = "Alice is a very smart girl. One day, Alice wanted to eat apples but she didn't have any apples. She decided to buy some apples. Alice looked for an apple shop.",
                            Height = 0,
                            NumberOfOptions = 5,
                            CorrectOption = 0,
                            OptionsPerRow = 5,
                            QuestionOptions = Enumerable.Range(0, 4).Select(optionOrder => new QuestionOptionModel
                            {
                                Prompt = $"Option {optionOrder + 1}",
                            }).ToList()
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
            };
        }

        private static string OptionPretext(OptionStyle style, int optionIndex)
        {
            return style switch
            {
                OptionStyle.UppercaseLetters => optionIndex switch
                {
                    0 => "A",
                    1 => "B",
                    2 => "C",
                    3 => "D",
                    4 => "E",
                    5 => "F",
                    6 => "G",
                    7 => "H",
                    8 => "I",
                    9 => "J",
                    _ => throw new ArgumentOutOfRangeException(nameof(optionIndex), optionIndex, null),
                },
                OptionStyle.LowercaseLetters => optionIndex switch
                {
                    0 => "a",
                    1 => "b",
                    2 => "c",
                    3 => "d",
                    4 => "e",
                    5 => "f",
                    6 => "g",
                    7 => "h",
                    8 => "i",
                    9 => "j",
                    _ => throw new ArgumentOutOfRangeException(nameof(optionIndex), optionIndex, null),
                },
                OptionStyle.NumbersFrom1 => (optionIndex + 1).ToString(),
                OptionStyle.RomanNumerals => optionIndex switch
                {
                    0 => "I",
                    1 => "II",
                    2 => "III",
                    3 => "IV",
                    4 => "V",
                    5 => "VI",
                    6 => "VII",
                    7 => "VIII",
                    8 => "IX",
                    9 => "X",
                    _ => throw new ArgumentOutOfRangeException(nameof(optionIndex), optionIndex, null),
                },
                OptionStyle.TrueFalse => optionIndex % 2 == 0 ? "True" : "False",
                OptionStyle.CheckBox => "☐",
                _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
            };
        }
    }
}
