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
                                {
                                    if (currentQuestion.Question.StartsWith("<Text") && !currentQuestion.Question.EndsWith("</Text>"))
                                        currentQuestion.Question += "</Text>";
                                    currentElement = CurrentElement.FirstAnswer;
                                }
                                else if (text != null && text.Text.StartsWith("Option2"))
                                {
                                    if(currentQuestion.FirstAnswer.StartsWith("<Text") && !currentQuestion.FirstAnswer.EndsWith("</Text>"))
                                        currentQuestion.FirstAnswer += "</Text>";
                                    currentElement = CurrentElement.SecondAnswer;
                                }
                                else if (text != null && text.Text.StartsWith("Option3"))
                                {
                                    if (currentQuestion.SecondAnswer.StartsWith("<Text") && !currentQuestion.SecondAnswer.EndsWith("</Text>"))
                                        currentQuestion.SecondAnswer += "</Text>";
                                    currentElement = CurrentElement.ThirdAnswer;
                                }
                                else if (text != null && text.Text.StartsWith("Option4"))
                                {
                                    if (currentQuestion.ThirdAnswer.StartsWith("<Text") && !currentQuestion.ThirdAnswer.EndsWith("</Text>"))
                                        currentQuestion.ThirdAnswer += "</Text>";
                                    currentElement = CurrentElement.FourthAnswer;
                                }
                                else if (text != null && text.Text.StartsWith("CorrectAnswer"))
                                {
                                    if (currentQuestion.FourthAnswer.StartsWith("<Text") && !currentQuestion.FourthAnswer.EndsWith("</Text>"))
                                        currentQuestion.FourthAnswer += "</Text>";
                                    currentElement = CurrentElement.CorrectAnswer;
                                }

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
                currentQuestion = new QuestionsDto { Question = "<Text style={styles.questionText}>" + decodedText };
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
                currentQuestion.CorrectAnswer = "<Text>" + decodedText + "</Text>";
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
            string imageTag = string.Empty;

            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            if (currentElement == CurrentElement.Question)
            {
                imageTag = "<Image style={styles.questionImage} source={{ uri: " + fileUrl + "}} />";
                if (!string.IsNullOrEmpty(currentQuestion.Question))
                    currentQuestion.Question += "</Text>" + imageTag;
                else
                    currentQuestion.Question += imageTag;
            }
            else
            {
                imageTag = "<Image style={styles.answerImage} source={{ uri: " + fileUrl + "}} />";
                AddAnswer(currentElement, currentQuestion, imageTag);
            }
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
                    if(string.IsNullOrEmpty(question.FirstAnswer) && !answerText.StartsWith("<Image"))
                        question.FirstAnswer = "<Text>";
                    question.FirstAnswer += answerText;
                    break;
                case CurrentElement.SecondAnswer:
                    if (string.IsNullOrEmpty(question.SecondAnswer) && !answerText.StartsWith("<Image"))
                        question.SecondAnswer = "<Text>";
                    question.SecondAnswer += answerText;
                    break;
                case CurrentElement.ThirdAnswer:
                    if (string.IsNullOrEmpty(question.ThirdAnswer) && !answerText.StartsWith("<Image"))
                        question.ThirdAnswer = "<Text>";
                    question.ThirdAnswer += answerText;
                    break;
                case CurrentElement.FourthAnswer:
                    if (string.IsNullOrEmpty(question.FourthAnswer) && !answerText.StartsWith("<Image"))
                        question.FourthAnswer = "<Text>";
                    question.FourthAnswer += answerText;
                    break;
            }
        }

    }
}
