namespace IOGKFExams.Server.Helpers
{
    using IOGKFExams.Server.Models;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public class ExamPdfDocument : IDocument
    {
        private readonly ExamPdfModel _exam;
        private readonly bool _includeAnswerKey;

        public ExamPdfDocument(
            ExamPdfModel exam,
            bool includeAnswerKey = false)
        {
            _exam = exam;
            _includeAnswerKey = includeAnswerKey;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"{_exam.ExamTitle} - {_exam.ExamId}",
                Author = "IOGKF"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);

                page.Margin(0.5f, Unit.Inch);

                page.DefaultTextStyle(x =>
                    x.FontSize(11));

                page.Header()
                    .Element(ComposeHeader);

                page.Content()
                    .PaddingVertical(15)
                    .Element(ComposeContent);

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .AlignCenter()
                    .Text("IOGKF Black Belt Examination")
                    .FontSize(18)
                    .Bold();

                if (_includeAnswerKey)
                {
                    column.Item()
                        .AlignCenter()
                        .Text("EXAMINER ANSWER KEY")
                        .FontSize(14)
                        .Bold();
                }

                column.Item()
                    .PaddingTop(10)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Candidate: {_exam.StudentName}");

                        row.RelativeItem()
                            .AlignRight()
                            .Text($"Exam ID: {_exam.ExamId}");
                    });

                column.Item()
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Rank Grading To: {_exam.Rank}");

                        row.RelativeItem()
                            .AlignRight()
                            .Text($"Language: {_exam.Language}");
                    });

                column.Item()
                    .PaddingTop(5)
                    .LineHorizontal(1);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(8);

                column.Item()
                    .Text(
                        "Instructions: Select the best answer for each question. " +
                        "Mark only one answer unless otherwise instructed.")
                    .Italic();

                column.Item()
                    .PaddingBottom(5)
                    .LineHorizontal(0.5f);

                foreach (var question in _exam.Questions)
                {
                    column.Item()
                        .PaddingTop(8)
                        .Text($"{question.QuestionNumber}. {question.Question}")
                        .Bold();

                    if (question.Image != null)
                    {
                        column.Item()
                            .PaddingVertical(5)
                            .AlignCenter()
                            .MaxHeight(250)
                            .Image(question.Image)
                            .FitArea();
                    }

                    for (int i = 0; i < question.Answers.Count; i++)
                    {
                        var answer = question.Answers[i];

                        var answerLetter =
                            ((char)('A' + i)).ToString();

                        column.Item()
                            .PaddingLeft(20)
                            .PaddingTop(3)
                            .Row(row =>
                            {
                                row.ConstantItem(35)
                                    .Text($"{answerLetter}. [ ]");

                                row.RelativeItem()
                                    .Text(text =>
                                    {
                                        text.Span(answer.Answer);

                                        if (_includeAnswerKey &&
                                            answer.IsCorrect)
                                        {
                                            text.Span("  ✓ CORRECT")
                                                .Bold();
                                        }
                                    });
                            });
                    }
                }
            });
        }
    }
}
