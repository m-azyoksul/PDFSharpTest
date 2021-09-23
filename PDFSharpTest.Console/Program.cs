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
        // Define constants
        // TODO: Abstract away
        const int pageW = 595;
        const int pageH = 842;

        static void Main(string[] args)
        {
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

                // Add graphic
                var graphics = XGraphics.FromPdfPage(page);
                var textFormatter = new XTextFormatter(graphics);

                // Draw top margin
                graphics.DrawRectangle(XPens.Blue,
                    0,
                    0,
                    page.Width,
                    d.TopMargin);

                // Draw bottom margin
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
                columnXs.ForEach(columnX =>
                    {
                        graphics.DrawRectangle(XPens.Black,
                            columnX,
                            d.TopMargin,
                            d.ColumnW,
                            d.ColumnH);
                    });

                var font = new XFont(d.Font, d.FontSize, XFontStyle.Regular);

                for (int i = 0; i < testPage.TestColumns.Count; i++)
                {
                    textFormatter.DrawString("1.", font, XBrushes.Black, new XRect(columnXs[i], d.TopMargin, 20, 0), XStringFormats.TopLeft);
                    textFormatter.DrawString(testPage.TestColumns[i].McQuestions[0].Prompt, font, XBrushes.Black, new XRect(columnXs[i] + 20, d.TopMargin, d.ColumnW - 20, 100), XStringFormats.TopLeft);
                    
                    var imagePath = "image.png";
                    graphics.DrawImage(XImage.FromFile(imagePath), columnXs[i] + 1, d.TopMargin + 100, d.ColumnW - 2, d.ColumnW - 2);
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
                    ColumnH = 680,
                    ColumnW = 255,
                    TopMargin = 100,
                    SideMargin = 28,
                    BiasedMarginLeft = 0,
                    Font = "Verdana",
                    FontSize = 13,
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
                            QuestionOptions = Enumerable.Range(0, 2).Select(_ => new QuestionOptionModel
                            {
                                Prompt = "Question prompt",
                            }).ToList()
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
            };
        }
    }
}
