Create Or ALter Procedure GetRandomQuestions
@Id BIGINT,
@Type NVARCHAR(20),
@TotalQuestions INT,
@LevelId BIGINT
AS
BEGIN

	Declare @Level NVARCHAR(10);	
	Declare @DifficultyLevel NVARCHAR(10);
	
	Select @Level = [Name] From AssessmentLevel Where Id = @LevelId;

	IF(@Level = 'LEVEL 1' OR @Level = 'LEVEL 2' OR @Level = 'LEVEL 3')
	BEGIN
		If(@Level = 'LEVEL 1')
			SET @DifficultyLevel = 'Easy';
		ELSE If(@Level = 'LEVEL 2')
			SET @DifficultyLevel = 'Medium';
		ELSE If(@Level = 'LEVEL 3')
			SET @DifficultyLevel = 'Difficult';

		IF(@Type = 'Chapter')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE ChapterId = @Id
				AND IsPreviousYearQuestion = 0
				AND DifficultyLevel = @DifficultyLevel
		    ORDER BY NEWID();
		END
		ELSE IF(@Type = 'Topic')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE TopicId = @Id
				AND IsPreviousYearQuestion = 0
				AND DifficultyLevel = @DifficultyLevel
		    ORDER BY NEWID();
		END
		ELSE IF(@Type = 'SubTopic')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE SubTopicId = @Id
				AND IsPreviousYearQuestion = 0
				AND DifficultyLevel = @DifficultyLevel
		    ORDER BY NEWID();
		END
	END
	ELSE
	BEGIN
		IF(@Type = 'Chapter')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE ChapterId = @Id
				AND IsPreviousYearQuestion = 1
		    ORDER BY NEWID();
		END
		ELSE IF(@Type = 'Topic')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE TopicId = @Id
				AND IsPreviousYearQuestion = 1
		    ORDER BY NEWID();
		END
		ELSE IF(@Type = 'SubTopic')
		BEGIN
		 SELECT TOP (@TotalQuestions) *
		    FROM vwQuestionBank
			WHERE SubTopicId = @Id
				AND IsPreviousYearQuestion = 1
		    ORDER BY NEWID();
		END
	END
END
GO