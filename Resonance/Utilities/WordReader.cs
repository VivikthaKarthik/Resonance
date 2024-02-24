using DocumentFormat.OpenXml.Packaging;
using ResoClassAPI.Controllers;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Utilities.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ResoClassAPI.Utilities
{
    public class WordReader : IWordReader
    {
        public async Task<List<QuestionsDto>> ProcessDocument(IFormFile document)
        {
            List<QuestionsDto> questions = new List<QuestionsDto>();
            QuestionsDto currentQuestion = null;
            CurrentElement currentElement = CurrentElement.Question;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await document.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, false))
                {
                    var paragraphs = wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>();

                    foreach (var paragraph in paragraphs)
                    {
                        var runs = paragraph.Elements<Run>().ToList();

                        if (runs.Any())
                        {
                            foreach (var run in runs)
                            {
                                var text = run.GetFirstChild<Text>();

                                if (text != null && text.Text.StartsWith("Question"))
                                    currentElement = CurrentElement.Question;
                                else if (text != null && text.Text.StartsWith("Answer1"))
                                    currentElement = CurrentElement.FirstAnswer;
                                else if (text != null && text.Text.StartsWith("Answer2"))
                                    currentElement = CurrentElement.SecondAnswer;
                                else if (text != null && text.Text.StartsWith("Answer3"))
                                    currentElement = CurrentElement.ThirdAnswer;
                                else if (text != null && text.Text.StartsWith("Answer4"))
                                    currentElement = CurrentElement.FourthAnswer;
                                else if (text != null && text.Text.StartsWith("CorrectAnswer"))
                                    currentElement = CurrentElement.CorrectAnswer;

                                if (text != null && !string.IsNullOrEmpty(text.Text) && text.Text.Trim() != "Question" &&
                                    text.Text.Trim() != "Answer1" && text.Text.Trim() != "Answer2" && text.Text.Trim() != "Answer3"
                                     && text.Text.Trim() != "Answer4" && text.Text.Trim() != "CorrectAnswer")
                                {
                                    // Explicitly set the encoding to UTF-8
                                    string decodedText = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(text.Text));

                                    if (decodedText != null)
                                        AddText(currentElement, decodedText, ref currentQuestion);
                                }

                                var drawing = run.GetFirstChild<Drawing>();

                                if (drawing != null)
                                {
                                    var blip = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();

                                    if (blip != null)
                                    {
                                        var imagePart = (ImagePart)wordDoc.MainDocumentPart.GetPartById(blip.Embed);
                                        var imageStream = imagePart.GetStream();

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            imageStream.CopyTo(ms);
                                            AddImage(currentElement, ms.ToArray(), ref currentQuestion);
                                        }
                                    }
                                }
                            }
                        }

                        if (currentQuestion != null && IsQuestionCompleted(currentQuestion))
                        {
                            questions.Add(currentQuestion);
                            currentQuestion = null;
                        }
                    }
                }
            }

            return questions;
        }

        private void AddText(CurrentElement currentElement, string decodedText, ref QuestionsDto currentQuestion)
        {
            if (currentQuestion == null)
            {
                currentQuestion = new QuestionsDto { Question = decodedText };
            }
            else if (!IsAnswer(currentElement) && currentElement != CurrentElement.CorrectAnswer)
            {
                currentQuestion.Question += decodedText;
            }
            else if (currentElement != CurrentElement.CorrectAnswer)
            {
                AddAnswer(currentElement, currentQuestion, decodedText, null);
            }
            else if (currentElement == CurrentElement.CorrectAnswer)
            {
                currentQuestion.CorrectAnswer = decodedText;
            }
        }

        private void AddImage(CurrentElement currentElement, byte[] image, ref QuestionsDto currentQuestion)
        {
            //if (currentQuestion == null && image != null && image.Length > 0)
            //{
            //    currentQuestion = new Question { QuestionText = decodedText.Substring(3, decodedText.Length - 4) };
            //}
            //else 

            AddAnswer(currentElement, currentQuestion, null, image);
        }


        private bool IsAnswer(CurrentElement currentElement)
        {
            if (currentElement == CurrentElement.FirstAnswer || currentElement == CurrentElement.SecondAnswer ||
                currentElement == CurrentElement.ThirdAnswer || currentElement == CurrentElement.FourthAnswer)
            {
                return true;
            }
            return false;
        }

        private bool IsQuestionCompleted(QuestionsDto currentQuestion)
        {
            if (currentQuestion == null || string.IsNullOrEmpty(currentQuestion.Question))
                return false;
            if (currentQuestion.FirstAnswer == null ||
                (string.IsNullOrEmpty(currentQuestion.FirstAnswer.Text) && currentQuestion.FirstAnswer.Image == null))
                return false;
            if (currentQuestion.SecondAnswer == null ||
                (string.IsNullOrEmpty(currentQuestion.SecondAnswer.Text) && currentQuestion.SecondAnswer.Image == null))
                return false;
            if (currentQuestion.ThirdAnswer == null ||
                (string.IsNullOrEmpty(currentQuestion.ThirdAnswer.Text) && currentQuestion.ThirdAnswer.Image == null))
                return false;
            if (currentQuestion.FourthAnswer == null ||
                (string.IsNullOrEmpty(currentQuestion.FourthAnswer.Text) && currentQuestion.FourthAnswer.Image == null))
                return false;
            if (currentQuestion.CorrectAnswer == null || string.IsNullOrEmpty(currentQuestion.CorrectAnswer))
                return false;

            return true;
        }

        private void AddAnswer(CurrentElement currentElement, QuestionsDto question, string answerText, byte[] image)
        {
            switch (currentElement)
            {
                case CurrentElement.FirstAnswer:
                    question.FirstAnswer = new AnswerDto(answerText, image);
                    break;
                case CurrentElement.SecondAnswer:
                    question.SecondAnswer = new AnswerDto(answerText, image);
                    break;
                case CurrentElement.ThirdAnswer:
                    question.ThirdAnswer = new AnswerDto(answerText, image);
                    break;
                case CurrentElement.FourthAnswer:
                    question.FourthAnswer = new AnswerDto(answerText, image);
                    break;
            }
        }

    }
}
