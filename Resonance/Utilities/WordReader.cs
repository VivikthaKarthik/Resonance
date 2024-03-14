using DocumentFormat.OpenXml.Packaging;
using ResoClassAPI.DTOs;
using ResoClassAPI.Utilities.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ResoClassAPI.Utilities
{
    public class WordReader : IWordReader
    {
        private readonly IAwsHandler awsHandler;
        private const string QuestionIndicator = "Question";
        private const string FirstAnswerIndicator = "Option1";
        private const string SecondAnswerIndicator = "Option2";
        private const string ThirdAnswerIndicator = "Option3";
        private const string FourthAnswerIndicator = "Option4";
        private const string CorrectAnswerIndicator = "CorrectAnswer";
        private const string DifficultyLevelIndicator = "DifficultyLevel";
        private const string ImageNameAppender = "ResoImage_";
        private const string S3BucketFolderName = "QuestionAndAnswerImages";


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

                                if (text != null && text.Text.TrimStart().StartsWith(QuestionIndicator))
                                    currentElement = CurrentElement.Question;
                                else if (text != null && text.Text.TrimStart().StartsWith(FirstAnswerIndicator))
                                {
                                    if (!currentQuestion.Question.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag) && !currentQuestion.Question.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag))
                                        currentQuestion.Question += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag;
                                    currentElement = CurrentElement.FirstAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith(SecondAnswerIndicator))
                                {
                                    if(!currentQuestion.FirstAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag) && !currentQuestion.FirstAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag))
                                        currentQuestion.FirstAnswer += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag;
                                    currentElement = CurrentElement.SecondAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith(ThirdAnswerIndicator))
                                {
                                    if (!currentQuestion.SecondAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag) && !currentQuestion.SecondAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag))
                                        currentQuestion.SecondAnswer += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag;
                                    currentElement = CurrentElement.ThirdAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith(FourthAnswerIndicator))
                                {
                                    if (!currentQuestion.ThirdAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag) && !currentQuestion.ThirdAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag))
                                        currentQuestion.ThirdAnswer += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag;
                                    currentElement = CurrentElement.FourthAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith(CorrectAnswerIndicator))
                                {
                                    if (!currentQuestion.FourthAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag) && !currentQuestion.FourthAnswer.TrimEnd().EndsWith(QuestionAndAnswerTags.TextClosingTag))
                                        currentQuestion.FourthAnswer += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag;
                                    currentElement = CurrentElement.CorrectAnswer;
                                }
                                else if (text != null && text.Text.TrimStart().StartsWith(DifficultyLevelIndicator))
                                {
                                    currentElement = CurrentElement.DifficultyLevel;
                                }

                                if (text != null && !string.IsNullOrEmpty(text.Text) && text.Text.Trim() != QuestionIndicator &&
                                    text.Text.Trim() != FirstAnswerIndicator && text.Text.Trim() != SecondAnswerIndicator && text.Text.Trim() != ThirdAnswerIndicator
                                     && text.Text.Trim() != FourthAnswerIndicator && text.Text.Trim() != CorrectAnswerIndicator && text.Text.Trim() != DifficultyLevelIndicator)
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
                currentQuestion = new QuestionsDto { Question = QuestionAndAnswerTags.QuestionTextOpeningTag + decodedText };
            }
            else if (!IsAnswer(currentElement) && currentElement != CurrentElement.CorrectAnswer && currentElement != CurrentElement.DifficultyLevel)
            {
                if (currentQuestion.Question.TrimEnd().EndsWith(QuestionAndAnswerTags.ImageClosingTag))
                    currentQuestion.Question += QuestionAndAnswerTags.QuestionTextOpeningTag + decodedText;
                currentQuestion.Question += decodedText;
            }
            else if (currentElement != CurrentElement.CorrectAnswer && currentElement != CurrentElement.DifficultyLevel)
            {
                AddAnswer(currentElement, currentQuestion, decodedText, false);
            }
            else if (currentElement == CurrentElement.CorrectAnswer && decodedText != string.Empty)
            {
                currentQuestion.CorrectAnswer += decodedText.Trim();
            }
            else if (currentElement == CurrentElement.DifficultyLevel && decodedText != string.Empty)
            {
                currentQuestion.DifficultyLevel = decodedText.Trim();
            }
        }

        private string GetImageName(CurrentElement currentElement, QuestionsDto currentQuestion)
        {
            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            return ImageNameAppender + Guid.NewGuid();
        }

        private async Task<QuestionsDto> AddImage(CurrentElement currentElement, byte[] imageArray, string name, QuestionsDto currentQuestion)
        {
            string fileUrl = await awsHandler.UploadImage(imageArray, S3BucketFolderName, name);

            if (currentQuestion == null)
                currentQuestion = new QuestionsDto();

            if (currentElement == CurrentElement.Question)
            {
                string imageTag = QuestionAndAnswerTags.QuestionImageOpeningTag + fileUrl + QuestionAndAnswerTags.ImageClosingTag + QuestionAndAnswerTags.NewLineTag;
                if (!string.IsNullOrEmpty(currentQuestion.Question))
                    currentQuestion.Question += QuestionAndAnswerTags.TextClosingTag + QuestionAndAnswerTags.NewLineTag + imageTag;
                else
                    currentQuestion.Question += imageTag;
            }
            else
            {
                string imageTag = QuestionAndAnswerTags.AnswerImageOpeningTag + fileUrl + QuestionAndAnswerTags.ImageClosingTag + QuestionAndAnswerTags.NewLineTag;
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
                        question.FirstAnswer  = QuestionAndAnswerTags.AnswerTextOpeningTag;
                    question.FirstAnswer += answerText;
                    break;
                case CurrentElement.SecondAnswer:
                    if (string.IsNullOrEmpty(question.SecondAnswer) && !isImage)
                        question.SecondAnswer = QuestionAndAnswerTags.AnswerTextOpeningTag;
                    question.SecondAnswer += answerText;
                    break;
                case CurrentElement.ThirdAnswer:
                    if (string.IsNullOrEmpty(question.ThirdAnswer) && !isImage)
                        question.ThirdAnswer = QuestionAndAnswerTags.AnswerTextOpeningTag;
                    question.ThirdAnswer += answerText;
                    break;
                case CurrentElement.FourthAnswer:
                    if (string.IsNullOrEmpty(question.FourthAnswer) && !isImage)
                        question.FourthAnswer = QuestionAndAnswerTags.AnswerTextOpeningTag;
                    question.FourthAnswer += answerText;
                    break;
            }
        }

    }
}
