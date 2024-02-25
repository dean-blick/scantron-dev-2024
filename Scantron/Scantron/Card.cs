// Card.cs
//
// Property of the Kansas State University IT Help Desk
// Written by: William McCreight, Caleb Schweer, and Joseph Webster
// 
// An extensive explanation of the reasoning behind the architecture of this program can be found on the github 
// repository: https://github.com/prometheus1994/scantron-dev/wiki
//
// This class is used for creating card objects from the raw scantron data.
// https://github.com/prometheus1994/scantron-dev/wiki/Card.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scantron
{
    class Card
    {
        // Stores the raw data stream initially read in from the scantron machine.
        private string raw_card_data;
        // The student's WID.
        private string wid;
        // Stores whether the grant permission bubble is filled.
        private string grant_permission = "-";
        // Stores which of the three test version bubbles is filled.
        private int test_version;
        // Stores which of the five sheet number bubbles is filled.
        private int sheet_number;
        // Stores the answer bubbles formatted to be written to the output file correctly. For more information refer 
        // to the github repository.
        private List<Question> response = new List<Question>();

        // Symbols selected in the Scantron machine's configuration.
        string front_of_card_symbol = "a";
        string back_of_card_symbol = "b";
        string compression_symbol = "#";
        
        public int TestVersion
        {
            get
            {
                if (test_version == 0) // Student did not fill out a test version.
                {
                    return 1;
                }
                else
                {
                    return test_version;
                }
            }
            set
            {
                test_version = value;
            }
        }
        
        public string WID
        {
            get
            {
                return wid;
            }
            set
            {
                wid = value;
            }
        }
        
        public int SheetNumber
        {
            get
            {
                if (sheet_number == 0) // Student did not fill out a sheet number.
                {
                    return 1;
                }
                else
                {
                    return sheet_number;
                }
            }
            set
            {
                sheet_number = value;
            }
        }
        
        public List<Question> Response
        {
            get
            {
                return response;
            }
        }

        // Card constructor. Translates the raw data and assigns it to the appropriate fields.
        public Card(string raw_card_data)
        {
            this.raw_card_data = raw_card_data;
            RemoveBackSide();
            Uncompress();
            Format();
            TranslateData();
        }

        /// <summary>
        /// Both sides of a scantron card are scanned. This removes the useless back side data, each line of which is 
        /// denoted by a "b" in the raw data. An "a" denotes a front side line.
        /// </summary>
        private void RemoveBackSide()
        {
            int start;
            int length;

            // As long as any b's are in the raw data, this loop removes any data from and including that b until it 
            // hits an a.
            while (raw_card_data.Contains(back_of_card_symbol))
            {
                start = raw_card_data.IndexOf(back_of_card_symbol);

                if (raw_card_data.IndexOf(front_of_card_symbol, start) != -1)
                {
                    length = raw_card_data.IndexOf(front_of_card_symbol, start) - start;
                }
                else
                {
                    length = raw_card_data.Length - start;
                }

                raw_card_data = raw_card_data.Remove(start, length);
            }
        }

        /// <summary>
        /// Looks for the compression character, "#", and uncompresses the characer after it. For more information on 
        /// scantron compress, refer to the github repository.
        /// </summary>
        private void Uncompress()
        {
            int hashtag_location;
            char amount_character;
            char character;
            int amount;
            string uncompressed_string;

            // As long as there is a # in the raw data, this loop replaces the data with its uncompressed form.
            while (raw_card_data.Contains(compression_symbol))
            {
                uncompressed_string = "";
                hashtag_location = raw_card_data.IndexOf(compression_symbol);
                amount_character = raw_card_data[hashtag_location + 1];
                character = raw_card_data[hashtag_location + 2];
                amount = (int)amount_character - 64;

                uncompressed_string = uncompressed_string.PadRight(amount, character);

                raw_card_data = raw_card_data.Replace(compression_symbol + amount_character + 
                                                        character, uncompressed_string);
            }
        }

        /// <summary>
        /// The empty space that a scantron card does not occupy when it goes beneath the scanner is read in as black
        /// marks. This method removes that data and trims down the parts of the scantron card that do not contain 
        /// bubbles. This turns it into an array of strings that directly correspond to the scantron card itself.
        /// </summary>
        private void Format()
        {
            int i;
            List<string> card_lines = new List<string>();
            char[] splitter = new char[] {front_of_card_symbol[0]};
            card_lines = raw_card_data.Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            // The first two lines read are above the bubbles on the scantron card. This removes them.
            card_lines.RemoveAt(0);
            card_lines.RemoveAt(0);

            // Trims useless space to the right of the WID section.
            for (i = 0; i < 9; i++)
            {
                card_lines[i] = card_lines[i].Substring(0, 10);
            }

            // Trims useless space to the right of the miscellaneous options and first five questions. 
            card_lines[9]   = card_lines[9].Substring(0, 11);
            card_lines[10]  = card_lines[10].Substring(0, 8);
            card_lines[11]  = card_lines[11].Substring(0, 14);
            card_lines[12]  = card_lines[12].Substring(0, 8);
            card_lines[13]  = card_lines[13].Substring(0, 11);

            // Trims the space to the right of quetions 6-50.
            for (i = 14; i < card_lines.Count; i++)
            {
                card_lines[i] = card_lines[i].Substring(0, 15);
            }

            raw_card_data = string.Join(",", card_lines);
        }

        /// <summary>
        /// This method takes the uncompressed, formatted data and assigns the appropriate data to each student field.
        /// </summary>
        private void TranslateData()
        {
            // This list splits up each line of bubbles on the scantron card.
            List<string> card_lines = new List<string>();
            char[] splitter = new char[] {','};
            card_lines = raw_card_data.Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Read in the WID bubbles. The relevant lines are reversed because from left to right the bubbles read 
            // from 9 to 0, but their indices are 0 to 9 in their respective strings. Reversing lets Array.IndexOf 
            // do its job correctly. If no bubble is dark enough for a given digit, a dash is used.
            for (int i = 0; i < 9; i++)
            {
                char[] line = card_lines[i].Reverse().ToArray();
                char max = line.Max();

                if (max > 54)
                {
                    wid += Array.IndexOf(line, max);
                }
                else
                {
                    wid += "-";
                }
            }
            
            // Checks the grant permission bubble.
            if (card_lines[11][13] > 6)
            {
                grant_permission = "1";
            }

            // Checks the test version bubbles.
            int test_version_one    = card_lines[9][10];
            int test_version_two    = card_lines[11][10];
            int test_version_three  = card_lines[13][10];

            test_version = GetDarkestBubble(test_version_one, test_version_two, test_version_three);

            // Checks the answer sheet bubbles.
            int sheet_number_one    = card_lines[9][7];
            int sheet_number_two    = card_lines[10][7];
            int sheet_number_three  = card_lines[11][7];
            int sheet_number_four   = card_lines[12][7];
            int sheet_number_five   = card_lines[13][7];

            sheet_number = GetDarkestBubble(sheet_number_one, sheet_number_two, sheet_number_three, 
                sheet_number_four, sheet_number_five);

            // Checks the answer bubbles.
            int count = 0;
            string answer = "";

            // These for loops are set up so that they read all 50 questions in order, which makes the indexing 
            // difficult. This page details these loops https://github.com/prometheus1994/scantron-dev/wiki/Student.cs.
            for (int i = 9; i < 29; i += 5)
            {
                if (i < 14)
                {
                    for (int j = 4; j >= 0; j--)
                    {
                        answer = "";
                        Tuple<String, Char> DarkestValues = new Tuple<String, Char>("1", '0');
                        
                        for (int k = 0; k < 5; k++)
                        {
                            if (card_lines[i + k][j] > DarkestValues.Item2)
                            {
                                DarkestValues = new Tuple<string, char>((k + 1).ToString(), card_lines[i + k][j]);
                            }
                            if (card_lines[i + k][j] > 54)
                            {
                                answer += k + 1;
                            }
                            else
                            {
                                answer += ' ';
                            }
                        }

                        Question q = new Question(answer, 0, false);
                        q.DarkestBubble = DarkestValues.Item1;
                        response.Add(q);
                        count++;
                    }
                }
                else
                {
                    for (int j = 14; j >= 0; j--)
                    {
                        answer = "";
                        Tuple<String, Char> DarkestValues = new Tuple<String, Char>("1", '0');

                        for (int k = 0; k < 5; k++)
                        {
                            if (card_lines[i + k][j] > DarkestValues.Item2)
                            {
                                DarkestValues = new Tuple<string, char>((k + 1).ToString(), card_lines[i + k][j]);
                            }
                            if (card_lines[i + k][j] > 54)
                            {
                                answer += k + 1;
                            }
                            else
                            {
                                answer += ' ';
                            }
                        }

                        Question q = new Question(answer, 0, false);
                        q.DarkestBubble = DarkestValues.Item1;
                        response.Add(q);
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns which bubble from a group of three is the darkest. Darkness is given by the scantron machine on a scale of 0 to F.
        /// </summary>
        /// <param name="a">First bubble's darkness.</param>
        /// <param name="b">Second bubble's darkness.</param>
        /// <param name="c">Third bubble's darkness.</param>
        /// <returns>Which bubble is darkest. Defaults to 0.</returns>
        private int GetDarkestBubble(int a, int b, int c)
        {
            ///returns 1 even if a bubble is mostly erased.
            if (a < 54 && b < 54 && c < 54) { return 1; }
            if (a > b && a >c)
            {
                return 1;
            }
            if (b > a && b > c)
            {
                return 2;
            }
            if (c > a && c > b)
            {
                return 3;
            }

            return 0;
        }

        /// <summary>
        /// Returns which bubble from a group of five is the darkest. Darkness is given by the scantron machine on a scale of 0 to F.
        /// </summary>
        /// <param name="a">First bubble's darkness.</param>
        /// <param name="b">Second bubble's darkness.</param>
        /// <param name="c">Third bubble's darkness.</param>
        /// <param name="d">Fourth bubble's darkness.</param>
        /// <param name="e">Fifth bubble's darkness.</param>
        /// <returns>Which bubble is darkest. Defaults to 0.</returns>
        private int GetDarkestBubble(int a, int b, int c, int d, int e)
        {
            ///returns 1 even if a bubble is mostly erased.
            if(a < 54 && b < 54 && c < 54 && d < 54 && e < 54) { return 1; }
            if (a > b && a > c && a > d && a > e)
            {
                return 1;
            }
            if (b > a && b > c && b > d && b > e)
            {
                return 2;
            }
            if (c > a && c > b && c > d && c > e)
            {
                return 3;
            }
            if (d > a && d > b && d > c && d > e)
            {
                return 4;
            }
            if (e > a && e > b && e > c && e > d)
            {
                return 5;
            }

            return 0;
        }
        
        /// <summary>
        /// Format the card as a string for us in a single answer only file.
        /// </summary>
        /// <returns>The card's data as a string.</returns>
        public string ToSingleAnswerString()
        {
            string card_info = "";
            string answer = "";
            string version;
            string sheet;

            if (test_version == 0)
            {
                version = "-";
            }
            else
            {
                version = test_version.ToString();
            }

            if (sheet_number == 0)
            {
                sheet = "-";
            }
            else
            {
                sheet = sheet_number.ToString();
            }

            card_info = wid + ", " + version + sheet + grant_permission + "--,   '";

            for (int i = 0; i < response.Count; i++)
            {
                answer = response[i].Answer;
                answer = answer.Trim();

                if (answer.Length == 1)
                {
                    card_info += answer;
                }
                else if (answer.Length == 0)
                {
                    card_info += "-";
                }
                else
                {
                    card_info += response[i].DarkestBubble;
                }
            }

            card_info += "'\r\n";

            return card_info;
        }

        /// <summary>
        /// Format the card as a string for use in a multiple answer compatible file.
        /// </summary>
        /// <returns>Card's data as a string.</returns>
        public string ToMultipleAnswerString()
        {
            string card_info = "";
            string version;
            string sheet;

            if (test_version == 0)
            {
                version = "-";
            }
            else
            {
                version = test_version.ToString();
            }

            if (sheet_number == 0)
            {
                sheet = "-";
            }
            else
            {
                sheet = sheet_number.ToString();
            }

            // Row 5
            card_info += wid + ", " + version + sheet + grant_permission + "--,5, '";

            for (int j = 0; j < response.Count; j++)
            {
                card_info += response[j].Answer[4];
            }

            card_info += "'\r\n";

            // Rows 4, 3, 2, 1
            for (int i = 3; i >= 0; i--)
            {
                card_info += "         ,      " + ',' + (i + 1) + ", '" ;

                for (int j = 0; j < response.Count; j++)
                {
                    card_info += response[j].Answer[i];
                }

                card_info += "'\r\n";
            }

            return card_info;
        }
    }
}
