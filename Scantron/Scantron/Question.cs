// Question.cs
//
// Property of the Kansas State University IT Help Desk
// Written by: William McCreight, Caleb Schweer, and Joseph Webster
// 
// An extensive explanation of the reasoning behind the architecture of this program can be found on the github 
// repository: https://github.com/prometheus1994/scantron-dev/wiki
//
// This class is used to create Question objects for the student responses & exam key.
// https://github.com/prometheus1994/scantron-dev/wiki/Question.cs

namespace Scantron
{
    class Question
    {
        // A char array that holds the answer a student gives or the answer on the answer key.
        private string answer = "";
        // How many points this question is worth.
        private float points;
        // Holds if this questions can be graded for partial credit on multiple answer questions.
        private bool partial_credit;

        public string DarkestBubble;
        
        public string Answer
        {
            get
            {
                return answer;
            }
        }

        public float Points
        {
            get
            {
                return points;
            }
        }

        public bool PartialCredit
        {
            get
            {
                return partial_credit;
            }
        }

        public Question(string answer, float points, bool partial_credit)
        {
            this.answer = answer;
            this.points = points;
            this.partial_credit = partial_credit;
        }

        /// <summary>
        /// Method for grading this question. https://community.canvaslms.com/docs/DOC-6674-understanding-multiple-answers-questions
        /// </summary>
        /// <param name="answer_key">Answer key to grade this questions against.</param>
        public void Grade(Question answer_key)
        {
            if (answer_key.PartialCredit)
            {
                float total_answers = 0;
                float correct_answers = 0;
                float incorrect_answers = 0;

                for (int i = 0; i < 5; i++)
                {
                    if (answer_key.answer[i] != ' ')
                    {
                        total_answers++;

                        if (answer[i] == ' ')
                        {
                            // No points are deducted.
                        }
                        else if (answer[i] == answer_key.answer[i])
                        {
                            correct_answers++;
                        } 
                        else
                        {
                            incorrect_answers++;
                        }
                    }
                    
                    if(answer_key.answer[i] == ' ')
                    {
                        if (answer[i] != ' ')
                        {
                            incorrect_answers++;
                        }
                    }
                }
                
                points = ((1 / total_answers) * correct_answers - (1 /total_answers) * incorrect_answers ) * answer_key.points;

                if (points < 0 || float.IsNaN(points))
                {
                    points = 0;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (answer[i] != answer_key.Answer[i])
                    {
                        points = 0;
                        return;
                    }
                }

                points = answer_key.Points;
                return;
            }
        }
    }
}