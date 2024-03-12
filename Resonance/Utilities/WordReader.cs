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

                                if (text != null && text.Text.TrimStart().StartsWith("Question"))
                                    currentElement = CurrentElement.Question;
                                else if (text != null && text.Text.TrimStart().StartsWith("Option1"))
                                {
                                    if (!currentQuestion.Question.TrimEnd().EndsWith("}} />") && !currentQuestion.Question.TrimEnd().EndsWith("</Text>"))
                                        currentQuestion.Question += "</Text>\n";
                                    currentElement = CurrentElement.FirstAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith("Option2"))
                                {
                                    if(!currentQuestion.FirstAnswer.TrimEnd().EndsWith("}} />") && !currentQuestion.FirstAnswer.TrimEnd().EndsWith("</Text>"))
                                        currentQuestion.FirstAnswer += "</Text>\n";
                                    currentElement = CurrentElement.SecondAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith("Option3"))
                                {
                                    if (!currentQuestion.SecondAnswer.TrimEnd().EndsWith("}} />") && !currentQuestion.SecondAnswer.TrimEnd().EndsWith("</Text>"))
                                        currentQuestion.SecondAnswer += "</Text>\n";
                                    currentElement = CurrentElement.ThirdAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith("Option4"))
                                {
                                    if (!currentQuestion.ThirdAnswer.TrimEnd().EndsWith("}} />") && !currentQuestion.ThirdAnswer.TrimEnd().EndsWith("</Text>"))
                                        currentQuestion.ThirdAnswer += "</Text>\n";
                                    currentElement = CurrentElement.FourthAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith("CorrectAnswer"))
                                {
                                    if (!currentQuestion.FourthAnswer.TrimEnd().EndsWith("}} />") && !currentQuestion.FourthAnswer.TrimEnd().EndsWith("</Text>"))
                                        currentQuestion.FourthAnswer += "</Text>\n";
                                    currentElement = CurrentElement.CorrectAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith("DifficultyLevel"))
                                {
                                    currentElement = CurrentElement.DifficultyLevel;
                                }

                                if (text != null && !string.IsNullOrEmpty(text.Text) && text.Text.Trim() != "Question" &&
                                    text.Text.Trim() != "Option1" && text.Text.Trim() != "Option2" && text.Text.Trim() != "Option3"
                                     && text.Text.Trim() != "Option4" && text.Text.Trim() != "CorrectAnswer" && text.Text.Trim() != "DifficultyLevel")
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
            else if (!IsAnswer(currentElement) && currentElement != CurrentElement.CorrectAnswer && currentElement != CurrentElement.DifficultyLevel)
            {
                if (currentQuestion.Question.TrimEnd().EndsWith("}} />"))
                    currentQuestion.Question += "<Text style={styles.questionText}>" + decodedText;
                currentQuestion.Question += decodedText;
            }
            else if (currentElement != CurrentElement.CorrectAnswer && currentElement != CurrentElement.DifficultyLevel)
            {
                AddAnswer(currentElement, currentQuestion, decodedText, false);
            }
            else if (currentElement == CurrentElement.CorrectAnswer && decodedText != "")
            {
                currentQuestion.CorrectAnswer += decodedText.Trim();
            }
            else if (currentElement == CurrentElement.DifficultyLevel && decodedText != "")
            {
                currentQuestion.DifficultyLevel = decodedText.Trim();
            }
        }

        private string GetImageName(CurrentElement currentElement, QuestionsDto currentQuestion)
        {
            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            return "ResoImage_" + currentElement.ToString() + "_" + Guid.NewGuid();
        }

        private async Task<QuestionsDto> AddImage(CurrentElement currentElement, byte[] imageArray, string name, QuestionsDto currentQuestion)
        {
            string fileUrl = await awsHandler.UploadImage(imageArray, name);

            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            if (currentElement == CurrentElement.Question)
            {
                string imageTag = "<Image style={styles.questionImage} source={{ uri: " + fileUrl + "}} />\n";
                if (!string.IsNullOrEmpty(currentQuestion.Question))
                    currentQuestion.Question += "</Text>\n" + imageTag;
                else
                    currentQuestion.Question += imageTag;
            }
            else
            {
                string imageTag = "<Image style={styles.answerImage} source={{ uri: " + fileUrl + "}} />\n";
                AddAnswer(currentElement, currentQuestion, imageTag, true);
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
            if (string.IsNullOrEmpty(currentQuestion.DifficultyLevel))
                return false;

            return true;
        }

        private void AddAnswer(CurrentElement currentElement, QuestionsDto question, string answerText, bool isImage)
        {
            switch (currentElement)
            {
                case CurrentElement.FirstAnswer:
                    if(string.IsNullOrEmpty(question.FirstAnswer) && !isImage)
                        question.FirstAnswer = "<Text>";
                    question.FirstAnswer += answerText;
                    break;
                case CurrentElement.SecondAnswer:
                    if (string.IsNullOrEmpty(question.SecondAnswer) && !isImage)
                        question.SecondAnswer = "<Text>";
                    question.SecondAnswer += answerText;
                    break;
                case CurrentElement.ThirdAnswer:
                    if (string.IsNullOrEmpty(question.ThirdAnswer) && !isImage)
                        question.ThirdAnswer = "<Text>";
                    question.ThirdAnswer += answerText;
                    break;
                case CurrentElement.FourthAnswer:
                    if (string.IsNullOrEmpty(question.FourthAnswer) && !isImage)
                        question.FourthAnswer = "<Text>";
                    question.FourthAnswer += answerText;
                    break;
            }
        }

    }
}
