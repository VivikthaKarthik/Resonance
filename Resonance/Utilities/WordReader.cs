using DocumentFormat.OpenXml.Packaging;
using ResoClassAPI.DTOs;
using ResoClassAPI.Utilities.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ResoClassAPI.Utilities
{
    public class WordReader : IWordReader
    {
        private readonly IAwsHandler awsHandler;
        public WordReader(IAwsHandler _awsHandler)
        {
            awsHandler = _awsHandler;
        }

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
                                else if (text != null && text.Text.StartsWith("Option1"))
                                    currentElement = CurrentElement.FirstAnswer;
                                else if (text != null && text.Text.StartsWith("Option2"))
                                    currentElement = CurrentElement.SecondAnswer;
                                else if (text != null && text.Text.StartsWith("Option3"))
                                    currentElement = CurrentElement.ThirdAnswer;
                                else if (text != null && text.Text.StartsWith("Option4"))
                                    currentElement = CurrentElement.FourthAnswer;
                                else if (text != null && text.Text.StartsWith("CorrectAnswer"))
                                    currentElement = CurrentElement.CorrectAnswer;

                                if (text != null && !string.IsNullOrEmpty(text.Text) && text.Text.Trim() != "Question" &&
                                    text.Text.Trim() != "Option1" && text.Text.Trim() != "Option2" && text.Text.Trim() != "Option3"
                                     && text.Text.Trim() != "Option4" && text.Text.Trim() != "CorrectAnswer")
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
                                            string name = GetImageName(currentElement, currentQuestion);
                                            currentQuestion = await AddImage(currentElement, ms.ToArray(), name, currentQuestion);
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
                AddAnswer(currentElement, currentQuestion, decodedText);
            }
            else if (currentElement == CurrentElement.CorrectAnswer)
            {
                currentQuestion.CorrectAnswer = decodedText;
            }
        }

        private string GetImageName(CurrentElement currentElement, QuestionsDto currentQuestion)
        {
            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            if(currentElement == CurrentElement.Question)
                return "ResoImage_" + currentElement.ToString() + "_" + (!string.IsNullOrEmpty(currentQuestion.Question) ? currentQuestion.Question.Split("<img").Length : 1);
            else if (currentElement == CurrentElement.FirstAnswer)
                return "ResoImage_" + currentElement.ToString() + "_" + (!string.IsNullOrEmpty(currentQuestion.FirstAnswer) ? currentQuestion.FirstAnswer.Split("<img").Length : 1);
            else if (currentElement == CurrentElement.SecondAnswer)
                return "ResoImage_" + currentElement.ToString() + "_" + (!string.IsNullOrEmpty(currentQuestion.SecondAnswer) ? currentQuestion.SecondAnswer.Split("<img").Length : 1);
            else if (currentElement == CurrentElement.ThirdAnswer)
                return "ResoImage_" + currentElement.ToString() + "_" + (!string.IsNullOrEmpty(currentQuestion.ThirdAnswer) ? currentQuestion.ThirdAnswer.Split("<img").Length : 1);
            else 
                return "ResoImage_" + currentElement.ToString() + "_" + (!string.IsNullOrEmpty(currentQuestion.FourthAnswer) ? currentQuestion.FourthAnswer.Split("<img").Length : 1);
        }

        private async Task<QuestionsDto> AddImage(CurrentElement currentElement, byte[] imageArray, string name, QuestionsDto currentQuestion)
        {
            var bucketName = "lessonvids";
            var folderPath = "QuestionAndAnswerImages";

            string fileUrl = await awsHandler.UploadImage(imageArray, name, bucketName, folderPath);
            string imageTag = string.Format(" <img src='{0}' class='{1}' /> ", fileUrl, currentElement.ToString() + "Image");

            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            if (currentElement == CurrentElement.Question)
                currentQuestion.Question += imageTag;
            else
                AddAnswer(currentElement, currentQuestion, imageTag);
            return currentQuestion; 
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
            if (string.IsNullOrEmpty(currentQuestion.FirstAnswer))
                return false;
            if (string.IsNullOrEmpty(currentQuestion.SecondAnswer))
                return false;
            if (string.IsNullOrEmpty(currentQuestion.ThirdAnswer))
                return false;
            if (string.IsNullOrEmpty(currentQuestion.FourthAnswer))
                return false;
            if (string.IsNullOrEmpty(currentQuestion.CorrectAnswer))
                return false;

            return true;
        }

        private void AddAnswer(CurrentElement currentElement, QuestionsDto question, string answerText)
        {
            switch (currentElement)
            {
                case CurrentElement.FirstAnswer:
                    question.FirstAnswer += answerText;
                    break;
                case CurrentElement.SecondAnswer:
                    question.SecondAnswer += answerText;
                    break;
                case CurrentElement.ThirdAnswer:
                    question.ThirdAnswer += answerText;
                    break;
                case CurrentElement.FourthAnswer:
                    question.FourthAnswer += answerText;
                    break;
            }
        }

    }
}
