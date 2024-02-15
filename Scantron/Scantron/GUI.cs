// GUI.cs
//
// Property of the Kansas State University IT Help Desk
// Written by: William McCreight, Caleb Schweer, and Joseph Webster
// comment for push
//
// An extensive explanation of the reasoning behind the architecture of this program can be found on the github 
// repository: https://github.com/prometheus1994/scantron-dev/wiki
//
// This class handles GUI changing methods.
// https://github.com/prometheus1994/scantron-dev/wiki/GUI.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Scantron
{
    public class GUI
    {
        private Grader grader;

        # region Controls
        // GUI objects that we need data from.
        private Form scantron_form;
        private Label uxAnswerKeyInstructionLabel;
        private Label uxScanInstructionLabel;
        private Label uxGradeInstructionLabel;
        private Panel uxStudentResponsePanel;
        private ComboBox uxStudentSelector;
        private TabControl uxAnswerKeyTabControl;
        private TextBox uxExamNameTextBox;
        private NumericUpDown uxNumberOfQuestionsNumericUpDown;
        private NumericUpDown uxNumberOfVersionsNumericUpDown;
        private NumericUpDown uxAllQuestionPointsNumericUpDown;
        private CheckBox uxAllPartialCreditCheckBox;
        private Button uxPreviousButton;
        private Button uxNextButton;
        private Label uxVersionLabel;
        private Label uxScoreLabel;
        private DataGridView uxCardListDataGridView;
        //private TextBox uxStatusTextBox;
        private TabControl uxMainTabControl;
        private TabPage uxStartTabPage;
        private TabPage uxAnswerKeyTabPage;
        private TabPage uxScanTabPage;
        private TabPage uxGradeTabPage;
        private TabPage uxCreateFileTabPage;
        private Label uxStartInstructionLabel;
        private Label uxCreateFileInstructionLabel;
        private CheckBox uxGradingWithThisProgramCheckbox;
        private TextBox uxKeyMessageCenterTextBox;
        private TextBox uxScanMessageCenterTextBox;
        private TextBox uxGradeMessageCenterTextBox;
        #endregion

        // Holds the raw card data from the Scantron.
        private List<string> raw_cards = new List<string>();

        // Answer key variables.
        private int number_of_versions = 0;
        private int number_of_questions = 0;

        // Scanner communicator.
        private Scanner scanner;

        public GUI(Form scantron_form)
        {
            this.scantron_form = scantron_form;
            scanner = new Scanner(this);
            grader = new Grader(this);

            InitializeControls();
            InitializeDefaultValues();
            InitializeInstructionText();
        }

        /// <summary>
        /// Displays messages via Message Box.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        public void DisplayMessage(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Finds all controls used in GUI.cs and initializes them.
        /// </summary>
        private void InitializeControls()
        {
            uxAnswerKeyInstructionLabel = (Label) scantron_form.Controls.Find("uxAnswerKeyInstructionLabel", true)[0];
            uxScanInstructionLabel = (Label) scantron_form.Controls.Find("uxScanInstructionLabel", true)[0];
            uxGradeInstructionLabel = (Label) scantron_form.Controls.Find("uxGradeInstructionLabel", true)[0];
            uxStudentResponsePanel = (Panel) scantron_form.Controls.Find("uxStudentResponsePanel", true)[0];
            uxStudentSelector = (ComboBox) scantron_form.Controls.Find("uxStudentSelectorComboBox", true)[0];
            uxAnswerKeyTabControl = (TabControl) scantron_form.Controls.Find("uxAnswerKeyTabControl", true)[0];
            uxExamNameTextBox = (TextBox) scantron_form.Controls.Find("uxExamNameTextBox", true)[0];
            uxNumberOfQuestionsNumericUpDown = (NumericUpDown) scantron_form.Controls.Find("uxNumberOfQuestionsNumericUpDown", true)[0];
            uxNumberOfVersionsNumericUpDown = (NumericUpDown) scantron_form.Controls.Find("uxNumberOfVersionsNumericUpDown", true)[0];
            uxAllQuestionPointsNumericUpDown = (NumericUpDown) scantron_form.Controls.Find("uxAllQuestionPointsNumericUpDown", true)[0];
            uxAllPartialCreditCheckBox = (CheckBox) scantron_form.Controls.Find("uxAllPartialCreditCheckBox", true)[0];
            uxPreviousButton = (Button) scantron_form.Controls.Find("uxPreviousButton", true)[0];
            uxNextButton = (Button) scantron_form.Controls.Find("uxNextButton", true)[0];
            uxVersionLabel = (Label) scantron_form.Controls.Find("uxVersionLabel", true)[0];
            uxScoreLabel = (Label) scantron_form.Controls.Find("uxScoreLabel", true)[0];
            uxCardListDataGridView = (DataGridView) scantron_form.Controls.Find("uxCardListDataGridView", true)[0];
            uxMainTabControl = (TabControl) scantron_form.Controls.Find("uxMainTabControl", true)[0];
            uxStartTabPage = (TabPage) scantron_form.Controls.Find("uxStartTabPage", true)[0];
            uxAnswerKeyTabPage = (TabPage) scantron_form.Controls.Find("uxAnswerKeyTabPage", true)[0];
            uxScanTabPage = (TabPage) scantron_form.Controls.Find("uxScanTabPage", true)[0];
            uxGradeTabPage = (TabPage) scantron_form.Controls.Find("uxGradeTabPage", true)[0];
            uxCreateFileTabPage = (TabPage) scantron_form.Controls.Find("uxCreateFileTabPage", true)[0];
            uxStartInstructionLabel = (Label) scantron_form.Controls.Find("uxStartInstructionLabel", true)[0];
            uxCreateFileInstructionLabel = (Label) scantron_form.Controls.Find("uxCreateFileInstructionLabel", true)[0];
            uxGradingWithThisProgramCheckbox = (CheckBox) scantron_form.Controls.Find("uxGradingWithThisProgramCheckBox", true)[0];
            uxKeyMessageCenterTextBox = (TextBox) scantron_form.Controls.Find("uxKeyMessageCenterTextBox", true)[0];
            uxScanMessageCenterTextBox = (TextBox)scantron_form.Controls.Find("uxScanMessageCenterTextBox", true)[0];
            uxGradeMessageCenterTextBox = (TextBox) scantron_form.Controls.Find("uxGradeMessageCenterTextBox", true)[0];        
        }

        /// <summary>
        /// Updates main instruction boxes with text.
        /// </summary>
        private void InitializeInstructionText()
        {
            uxStartInstructionLabel.Text = "Welcome to the new Scantron program!"
                                                     + Environment.NewLine + Environment.NewLine +
                                                    "1. Set your Scantron cards in the tray by following the pictures to the right." + Environment.NewLine + Environment.NewLine +
                                                    "2. Click Reset." + Environment.NewLine + Environment.NewLine +
                                                    "3. Enter the the exam name, number of versions, and number of questions." + Environment.NewLine + Environment.NewLine +
                                                    "4. If you want your students to see their responses online, uncheck \"Grading within this program\".";

            uxAnswerKeyInstructionLabel.Text =      "1. Enter how many points each question is worth and which ones are given partial credit." + Environment.NewLine + Environment.NewLine +
                                                    "2. Create the answer key by scanning in cards with the correct answers for each version, or by checking boxes in the form." + Environment.NewLine + Environment.NewLine +
                                                    "3. Click Create Answer Key, then click Continue.";

            uxScanInstructionLabel.Text =           "1. Click Ready, then press the Start button on the Scantron machine." + Environment.NewLine + Environment.NewLine +
                                                    "2. Once the machine is done scanning, click Done." + Environment.NewLine + Environment.NewLine +
                                                    "3. You can edits WIDs, test versions, and sheet numbers." + Environment.NewLine + Environment.NewLine +
                                                    "4. Click Save Changes, then click Continue.";

            uxGradeInstructionLabel.Text =          "1. Click Grade Students." + Environment.NewLine + Environment.NewLine +
                                                    "2. Questions in green were given full points, questions in orange were given partial credit, and questions in red were given 0 points." + Environment.NewLine + Environment.NewLine +
                                                    "3. When you are done reviewing, click Continue.";

            uxCreateFileInstructionLabel.Text =     "1. Click the Gradebook button if you have graded within this program, otherwise click one of the Scantron Tool options." + Environment.NewLine + Environment.NewLine +
                                                    "2. If your exam has questions that have more than one answer, click the Multiple Answer button. The Single Answer option can only handle 1 answer per question." + Environment.NewLine + Environment.NewLine +
                                                    "3. The Gradebook method with give you a .csv file. Go to your course, go to Grades, then click import and select the file to upload it." + Environment.NewLine + Environment.NewLine +
                                                    "4. The Canvas Scantron tool has a separate set of instructions here: https://public.online.k-state.edu/tools/scantron/index.html" + Environment.NewLine + Environment.NewLine +
                                                    "5. Once you have your file saved, click the Finish button to clear all your data for the next person." + Environment.NewLine + Environment.NewLine +
                                                    "If you have any feedback, please email scantron@ksu.edu and we will attempt to incorporate it.";
        }

        /// <summary>
        /// Initializes all necessary values upon booting the program and resetting it. 
        /// </summary>
        private void InitializeDefaultValues()
        {
            number_of_versions = 1;
            number_of_questions = 50;
            uxExamNameTextBox.Text = "Exam";
            uxNumberOfVersionsNumericUpDown.Value = 1;
            uxNumberOfQuestionsNumericUpDown.Value = 50;
            uxGradingWithThisProgramCheckbox.Checked = false;
            //uxAnswerKeyTabControl.Enabled = true;
            uxAllQuestionPointsNumericUpDown.Value = 1;
            uxAllPartialCreditCheckBox.Checked = false;
            uxCardListDataGridView.Rows.Clear();
            uxStudentSelector.Items.Clear();
            uxStudentSelector.Text = "";
            uxVersionLabel.Text = "Version: ";
            uxScoreLabel.Text = "Score: ";
            uxNextButton.Enabled = false;
            uxPreviousButton.Enabled = false;

            uxAnswerKeyTabPage.Controls.Find("uxScanAnswerKeyButton", true)[0].BackColor = SystemColors.Control;
            uxAnswerKeyTabPage.Controls.Find("uxDoneScanningButton", true)[0].BackColor = SystemColors.Control;

            uxScanTabPage.Controls.Find("uxReadyButton", true)[0].BackColor = SystemColors.Control;
            uxScanTabPage.Controls.Find("uxDoneButton", true)[0].BackColor = SystemColors.Control;

            uxKeyMessageCenterTextBox.Hide();
            uxScanMessageCenterTextBox.Hide();
            uxGradeMessageCenterTextBox.Hide();
            //uxKeyMessageCenterTextBox.Show();
            //uxScanMessageCenterTextBox.Show();
            //uxGradeMessageCenterTextBox.Show();
        }

        /// <summary>
        /// Reset program to initial state.
        /// </summary>
        public void Reset()
        {
            Panel panel;

            raw_cards.Clear();
            grader.Cards.Clear();
            grader.Students.Clear();
            grader.PartialWIDs.Clear();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 250; j++)
                {
                    panel = (Panel)uxAnswerKeyTabControl.TabPages[i].Controls[j];
                    panel.Hide();
                }
            }
            for (int i = 0; i < 250; i++)
            {
                panel = (Panel)uxStudentResponsePanel.Controls[i];
                panel.Hide();
            }

            InitializeDefaultValues();

            DisplayMessage("Data has been reset!");
        }

        /// <summary>
        /// Change the current tab to the Answer Key tab or the Scan tab based on workflow.
        /// </summary>
        public void StartContinue()
        {
            if (uxGradingWithThisProgramCheckbox.Checked)
            {
                UpdateAnswerForm();
                uxMainTabControl.SelectTab("uxAnswerKeyTabPage");
            }
            else
            {
                AnswerKeyContinue();
            }
        }

        /// <summary>
        /// Create the answer key form to be filled out by the instructor.
        /// </summary>
        /// <param name="tabpage">Current version being created.</param>
        public void InstantiateAnswerKeyForm(TabPage tabpage)
        {
            for (int j = 0; j < 250; j++)
            {
                Panel question_panel = new Panel
                {
                    BackColor = Color.MediumPurple,
                    Location = new Point(3, 3 + 26 * j),
                    Size = new Size(385, 22),
                    Visible = false
                };

                for (int k = 0; k < 5; k++)
                {
                    CheckBox checkbox = new CheckBox
                    {
                        Location = new Point(38 + 39 * k, 3),
                        Size = new Size(33, 17),
                        Text = ((char)(k + 65)).ToString()
                    };
                    question_panel.Controls.Add(checkbox); // Checkboxes are added first so they are indices 0-4.
                }

                NumericUpDown points_updown = new NumericUpDown
                {
                    Location = new Point(228, 1),
                    DecimalPlaces = 2,
                    Size = new Size(58, 20),
                    Value = 1
                };

                CheckBox partial_credit_checkbox = new CheckBox
                {
                    Location = new Point(290, 3),
                    Size = new Size(100, 17),
                    Text = "Partial Credit"
                };
                // size(30, 13)
                Label question_label = new Label
                {
                    Location = new Point(3, 3),
                    Size = new Size(70, 13),
                    Text = "Q. " + (j + 1)
                };

                question_panel.Controls.Add(points_updown); // Index 5
                question_panel.Controls.Add(partial_credit_checkbox); // Index 6
                question_panel.Controls.Add(question_label); // Index 7

                tabpage.Controls.Add(question_panel);
            }
        }

        /// <summary>
        /// Update the number of test version from the NumericUpDown.
        /// </summary>
        public void UpdateNumberOfVersions()
        {
            number_of_versions = (int)uxNumberOfVersionsNumericUpDown.Value;
            UpdateAnswerForm();
        }

        /// <summary>
        /// Update the number of questions from the NumericUpDown.
        /// </summary>
        public void UpdateNumberOfQuestions()
        {
            number_of_questions = (int)uxNumberOfQuestionsNumericUpDown.Value;
            UpdateAnswerForm();
        }

        /// <summary>
        /// Updates points for all questions in the exam.
        /// </summary>
        public void UpdateAllQuestionPoints()
        {
            foreach (TabPage tabpage in uxAnswerKeyTabControl.TabPages)
            {
                foreach (Control control in tabpage.Controls)
                {
                    NumericUpDown updown = (NumericUpDown)control.Controls[5];
                    updown.Value = uxAllQuestionPointsNumericUpDown.Value;
                }
            }
        }

        /// <summary>
        /// Updates partial credit for all questions in the exam.
        /// </summary>
        public void UpdateAllPartialCredit()
        {
            foreach (TabPage tabpage in uxAnswerKeyTabControl.TabPages)
            {
                foreach (Control control in tabpage.Controls)
                {
                    CheckBox checkbox = (CheckBox)control.Controls[6];
                    checkbox.Checked = uxAllPartialCreditCheckBox.Checked;
                }
            }
        }

        /// <summary>
        /// Create the answer key form with the specified number of questions and versions.
        /// </summary>
        public void UpdateAnswerForm()
        {
            CheckBox checkbox;

            foreach (TabPage tabpage in uxAnswerKeyTabControl.TabPages)
            {
                foreach (Panel panel in tabpage.Controls)
                {
                    //panel.Hide();

                    for (int i = 0; i < 5; i++)
                    {
                        checkbox = (CheckBox)panel.Controls[i];
                        checkbox.Checked = false;
                    }

                    checkbox = (CheckBox)panel.Controls[6];
                    checkbox.Checked = false;
                }
            }

            uxAllQuestionPointsNumericUpDown.Value = 1;
            uxAllPartialCreditCheckBox.Checked = false;

            int number_of_versions = (int)uxNumberOfVersionsNumericUpDown.Value;
            int number_of_questions = (int)uxNumberOfQuestionsNumericUpDown.Value;
            
            TabPage answer_key_tabpage;

            for (int i = 0; i < number_of_versions; i++)
            {
                answer_key_tabpage = uxAnswerKeyTabControl.TabPages[i];

                for (int j = 0; j < number_of_questions; j++)
                {
                    answer_key_tabpage.Controls[j].Show();
                }
            }
        }

        /// <summary>
        /// Use a Scantron Card for the answer key.
        /// </summary>
        public void UseScantronCard()
        {
            try
            {
                scanner.Scan();
                UpdateScanButtonColor((Button)uxAnswerKeyTabPage.Controls.Find("uxScanAnswerKeyButton", true)[0], Color.Green);
            }
            catch (IOException)
            {
                UpdateScanButtonColor((Button)uxAnswerKeyTabPage.Controls.Find("uxScanAnswerKeyButton", true)[0], Color.Red);
                DisplayMessage("Scantron machine is not connected to computer by port COM1.");
                return;
            }
        }

        /// <summary>
        /// Close serial port and fill out the answer key form based on scanned cards.
        /// </summary>
        public void DoneScanning()
        {
            try
            {
                scanner.Stop();
                
                raw_cards = scanner.RawCards;
                grader.CreateCards(raw_cards);

                TabPage tabpage;
                Panel panel;
                CheckBox checkbox;
                int start_index;

                foreach (Card card in grader.Cards)
                {
                    tabpage = uxAnswerKeyTabControl.TabPages[card.TestVersion - 1];
                    start_index = card.Response.Count * (card.SheetNumber - 1);

                    for (int i = 0; i < card.Response.Count; i++)
                    {
                        panel = (Panel)tabpage.Controls[i + start_index];

                        for (int j = 0; j < 5; j++)
                        {
                            checkbox = (CheckBox)panel.Controls[j];
                            if (card.Response[i].Answer[j] != ' ')
                            {
                                checkbox.Checked = true;
                            }
                            else
                            {
                                checkbox.Checked = false;
                            }
                        }
                    }
                }
                UpdateScanButtonColor((Button)uxAnswerKeyTabPage.Controls.Find("uxScanAnswerKeyButton", true)[0], SystemColors.Control);
            }
            catch (Exception)
            {
                //UpdateScanButtonColor((Button)uxAnswerKeyTabPage.Controls.Find("uxDoneScanningButton", true)[0], Color.Red);
                DisplayMessage("Something went wrong. Do not click Done Scanning while the machine is running. " +
                                "Stop the machine, click Scan Answer Key, and begin scanning again.");
            }
        }

        private void UpdateScanButtonColor(Button button, Color color)
        {
            button.BackColor = color;
        }

        /// <summary>
        /// Create the answer key from the filled out answer key form.
        /// </summary>
        /// <returns>True if successful.</returns>
        public void CreateAnswerKey()
        {
            if (uxExamNameTextBox.Text == "")
            {
                DisplayMessage("Go to the Start tab and enter a name for the exam.");
                return;
            }

            if (number_of_versions == 0 || number_of_questions == 0)
            {
                DisplayMessage("You have not specified the number of versions and questions. Go back to the Start tab and follow the instructions.");
                return;
            }

            grader.AnswerKey = new Dictionary<int, List<Question>>();

            Panel question_panel;
            CheckBox checkbox;
            NumericUpDown points_updown;

            string answer = "";
            float points = 0;
            bool partial_credit = false;

            for (int i = 0; i < number_of_versions; i++)
            {
                grader.AnswerKey.Add(i, new List<Question>());

                for (int j = 0; j < number_of_questions; j++)
                {
                    answer = "";

                    question_panel = (Panel)uxAnswerKeyTabControl.TabPages[i].Controls[j];

                    // This loop cycles through the first 5 controls in the current question panel, which are the checkboes for A-E.
                    for (int k = 0; k < 5; k++)
                    {
                        checkbox = (CheckBox)question_panel.Controls[k];
                        if (checkbox.Checked)
                        {
                            answer += k + 1;
                        }
                        else
                        {
                            answer += " ";
                        }
                    }

                    points_updown = (NumericUpDown)question_panel.Controls[5];
                    points = (float)points_updown.Value;

                    // Checks the current question panel's partial credit checkbox.
                    checkbox = (CheckBox)question_panel.Controls[6];
                    if (checkbox.Checked)
                    {
                        partial_credit = true;
                    }
                    else
                    {
                        partial_credit = false;
                    }

                    grader.AnswerKey[i].Add(new Question(answer, points, partial_credit));
                }
            }

            DisplayMessage("Answer Key successfully created!");
        }

        /// <summary>
        /// Change the current tab to the Scan tab.
        /// </summary>
        public void AnswerKeyContinue()
        {
            uxMainTabControl.SelectTab("uxScanTabPage");
        }

        /// <summary>
        /// Opens the serial port and prepares to receive data from the Scantron machine.
        /// Ref: https://msdn.microsoft.com/en-us/library/system.io.ports.serialport.open(v=vs.110).aspx
        /// </summary>
        public void Ready()
        {
            if(uxExamNameTextBox.Text == "")
            {
                DisplayMessage("Go to the Start Tab and enter a name for the exam.");
                return;
            }

            if(number_of_versions == 0 || number_of_questions == 0)
            {
                DisplayMessage("You have not specified the number of versions and questions. Go back to the Start tab and follow the instructions.");
                return;
            }

            try
            {
                scanner.Scan();
                UpdateScanButtonColor((Button)uxScanTabPage.Controls.Find("uxReadyButton", true)[0], Color.Green);
            }
            catch(IOException)
            {
                UpdateScanButtonColor((Button)uxScanTabPage.Controls.Find("uxReadyButton", true)[0], Color.Red);
                DisplayMessage("Scantron machine is not connected to computer by port COM1.");
                return;
            }
        }

        /// <summary>
        /// Close the serial port and update the card list.
        /// </summary>
        public void Done()
        {
            try
            {
                scanner.Stop();
                raw_cards = scanner.RawCards;
                grader.CreateCards(raw_cards);
                UpdateCardList();
                UpdateScanButtonColor((Button)uxScanTabPage.Controls.Find("uxReadyButton", true)[0], SystemColors.Control);
            }
            catch (Exception)
            {
                UpdateScanButtonColor((Button)uxScanTabPage.Controls.Find("uxReadyButton", true)[0], Color.Red);
                DisplayMessage("Something went wrong. Do not click Done while the machine is running. " +
                                "Stop the machine, click Ready, and begin scanning again.");
            }
        }

        /// <summary>
        /// Save changes made to WIDs, test versions, and sheet numbers on cards list.
        /// </summary>
        public void SaveChanges()
        {
            if (grader.Cards.Count < 1)
            {
                DisplayMessage("No cards found. Please follow the instructions on this page from the beginning.");
                return;
            }
            UpdateScanButtonColor((Button)uxScanTabPage.Controls.Find("uxDoneButton", true)[0], SystemColors.Control);
            Card card;
            string wid;
            int test_version;
            int sheet_number;

            for(int i = 0; i < grader.Cards.Count; i++)
            {
                card = grader.Cards[i];
                wid = (string)uxCardListDataGridView.Rows[i].Cells[1].Value;
                test_version = Convert.ToInt32(uxCardListDataGridView.Rows[i].Cells[2].Value);
                sheet_number = Convert.ToInt32(uxCardListDataGridView.Rows[i].Cells[3].Value);

                if(wid.Length == 9)
                {
                    card.WID = wid;
                }

                if(test_version > 0 && test_version < 4)
                {
                    card.TestVersion = test_version;
                }

                if(sheet_number > 0 && sheet_number < 6)
                {
                    card.SheetNumber = sheet_number;
                }
            }

            UpdateCardList();
            grader.CreateStudents();
            DisplayMessage("Changes saved!");
        }

        /// <summary>
        /// Update the Card List DataGridView.
        /// </summary>
        private void UpdateCardList()
        {
            uxCardListDataGridView.Rows.Clear();
            Card card;

            for(int i = 0; i < grader.Cards.Count; i++)
            {
                card = grader.Cards[i];
                uxCardListDataGridView.Rows.Add(i, card.WID, card.TestVersion, card.SheetNumber);

                if(card.WID.Length != 9 || card.WID[0] != '8')
                {
                    uxCardListDataGridView.Rows[i].Cells[1].Style.BackColor = Color.Red;
                }

                if(card.TestVersion < 1 || card.TestVersion > 3 || card.TestVersion > number_of_versions)
                {
                    uxCardListDataGridView.Rows[i].Cells[2].Style.BackColor = Color.Red;
                }

                if(card.SheetNumber < 1 || card.SheetNumber > 5)
                {
                    uxCardListDataGridView.Rows[i].Cells[3].Style.BackColor = Color.Red;
                }
            }
        }

        /// <summary>
        /// Change the current tab to the Grade tab or the Create File tab based on workflow.
        /// </summary>
        public void ScanContinue()
        {
            if (uxGradingWithThisProgramCheckbox.Checked)
            {
                uxMainTabControl.SelectTab("uxGradeTabPage");
            }
            else
            {
                GradeContinue();
            }
        }

        /// <summary>
        /// Fill raw cards list with test data.
        /// </summary>
        public void TestData()
        {
            raw_cards = new List<string>();
            // Test Data.
            raw_cards.Add("b0F00F0FF#F0#DF00#\\Fb#T0#\\Fa0F00F0FF#F0#DF00#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa0E#R0#\\Fb#T0#\\Fa000D#P0#\\Fb#T0#\\Fa00C#Q0#\\Fb#T0#\\Fa#D0D#O0#\\Fb#T0#\\Fa#F0E#M0#\\Fb#T0#\\Fa#I0D#J0#\\Fb#T0#\\FaD#S0#\\Fb#T0#\\Fa#I0C#J0#\\Fa#I0C#J0#\\Fb#T0#\\Fb#T0#\\Fa#D0E#E0E#I0#\\Fb#T0#\\Fa000F000F#L0#\\Fb#T0#\\Fa00F#J0E#F0#\\Fb#T0#\\Fa0D#R0#\\FaD#S0#\\Fb#T0#\\Fb#T0#\\Fa#D0D#D0F#D0C#E0#\\Fb#T0#\\Fa000F#D0F#D0E#F0#\\Fb#T0#\\Fa00E#D0F#D0E#G0#\\Fb#T0#\\Fa0E#D0F#D0C#H0#\\FaE#D0D#D0F#I0#\\Fb#T0#\\Fb#T0#\\Fa#D0E#D0F#D0B#E0#\\Fb#T0#\\Fa000D#D0E#D0D#F0#\\Fb#T0#\\Fa00D#D0F#D0E#G0#\\Fb#T0#\\Fa0D#D0F#D0F#H0#\\FaD#D0F#D0F#I0#\\Fb#T0#\\Fb#T0#\\Fa#D0E#D0E#D0E#E0#\\Fb#T0#\\Fa000F#D0F#D0E#F0#\\Fb#T0#\\Fa00E#D0F#D0E0005000#\\Fb#T0#\\Fa0E#D0E#D0D#D06000#\\FaE#D0E#D0D#I0#\\F$");
            raw_cards.Add("b0F00F0FF#F0#DF00#\\Fb#T0#\\Fa0F00F0FF#F0#DF00#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa0F#R0#\\Fb#T0#\\Fa000F#P0#\\Fb#T0#\\Fa00E#Q0#\\Fb#T0#\\Fa#D0E#O0#\\Fb#T0#\\Fa#F0F#M0#\\Fb#T0#\\Fa#I0E#J0#\\Fb#T0#\\FaE#S0#\\Fb#T0#\\Fa#I0E#J0#\\Fa#I0E#J0#\\Fb#T0#\\FaD0F0F#E0F#I0#\\Fb#T0#\\Fa000E#P0#\\Fb#T0#\\Fa0F#E0F#E0E#F0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa00D#G0F00D#F0#\\Fb#T0#\\FaD#D0C0F0E0D#H0#\\Fb#T0#\\Fa0D0D#D0E#K0#\\Fb#T0#\\Fb#T0#\\Fb#T0#\\Fa#F0F#G0D#E0#\\Fa#D0D#G0E#G0#\\Fb#T0#\\Fb#T0#\\Fa0F#G0D#J0#\\Fb#T0#\\Fa#G0E#L0#\\Fb#T0#\\Fa#E0F#F0C0C#E0#\\Fb#T0#\\Fa#D0F0C0D0D00C#F0#\\FaF0EF#G0C#H0#\\Fb#T0#\\Fb#T0#\\FaE#F0E#H07000#\\Fb#T0#\\Fa0C#D0D0D#E0C#E0#\\Fb#T0#\\Fa00D00C000D00F#G0#\\Fb#T0#\\Fa#D0C#E0E#I0#\\Fa000C#G0E0E004000#\\F$");
            raw_cards.Add("b0F00F0FF#F0#DF00#\\Fb#T0#\\Fa0F00F0FF#F0#DF00#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa0F#R0#\\Fb#T0#\\Fa000E#P0#\\Fb#T0#\\Fa00E#Q0#\\Fb#T0#\\Fa#D0E#O0#\\Fb#T0#\\Fa#F0D#M0#\\Fb#T0#\\Fa#I0D#J0#\\Fb#T0#\\FaD#S0#\\Fb#T0#\\Fa#I0E#J0#\\Fa#I0D#J0#\\Fb#T0#\\Fb#T0#\\FaFBFFF00F00E#I0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#M0F#F0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#J0EEEFE#E0#\\Fb#T0#\\Fa#E0DFFFD#J0#\\Fb#T0#\\FaEEDEE#O0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#E0FFFEF#J0#\\Fb#T0#\\FaFFFDE#O0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#J0FEEFF#E0#\\Fb#T0#\\Fb#T0#\\Fa#P07000#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#J0DDFFC03000#\\Fb#T0#\\Fa#E0FFDFD#J0#\\FaF#DE#O0#\\F$");
            raw_cards.Add("b0F00F0FF#F0#DF00#\\Fb#T0#\\Fa0F00F0FF#F0#DF00#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa0E#R0#\\Fb#T0#\\Fa00E#Q0#\\Fb#T0#\\Fa#F0E#M0#\\Fb#T0#\\Fa000D#P0#\\Fb#T0#\\Fa#E0F#N0#\\Fb#T0#\\FaB#S0#\\Fb#T0#\\Fa#G0F#L0#\\Fb#T0#\\Fa#I0D#J0#\\Fa#H0D#K0#\\Fb#T0#\\Fb#T0#\\Fa#G0E#L0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#J0F00F#F0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#Q0300#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#S03#\\F$");
            raw_cards.Add("b0F00F0FF#F0#DF00#\\Fb#T0#\\Fa0F00F0FF#F0#DF00#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa0E#R0#\\Fb#T0#\\Fa00D#Q0#\\Fb#T0#\\Fa#F0E#M0#\\Fb#T0#\\Fa000D#P0#\\Fb#T0#\\Fa#E0D#N0#\\Fb#T0#\\FaB#S0#\\Fb#T0#\\Fa#G0F#L0#\\Fb#T0#\\Fa#I0E#J0#\\Fa#H0E#K0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#G0F00F00F#F0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\Fb#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fb#T0#\\Fa#T0#\\Fa#T0#\\F$");
            
            grader.CreateCards(raw_cards);
            UpdateCardList();
        }

        /// <summary>
        /// Create the controls for displaing a chosen student's answers and make them invisible.
        /// </summary>
        public void InstantiateStudentDisplay()
        {
            for (int i = 0; i < 250; i++)
            {
                Panel question_panel = new Panel
                {
                    Location = new Point(3, 3 + 26 * i),
                    Size = new Size(376, 22),
                    Visible = false
                };

                for (int j = 0; j < 5; j++)
                {
                    CheckBox checkbox = new CheckBox
                    {
                        Enabled = false,
                        Location = new Point(73 + 39 * j, 3),
                        Size = new Size(33, 17),
                        Text = ((char)(j + 65)).ToString()
                    };

                    question_panel.Controls.Add(checkbox); // Checkboxes are added first so their indices are 0-4.
                }

                Label points_label = new Label
                {
                    Location = new Point(268, 3),
                    Size = new Size(45, 13),
                    Text = "Points: 0"
                };

                Label question_label = new Label
                {
                    Location = new Point(3, 3),
                    Size = new Size(45, 13),
                    Text = "Q. " + (i + 1)
                };

                question_panel.Controls.Add(points_label); // Index 5
                question_panel.Controls.Add(question_label); // Index 6

                uxStudentResponsePanel.Controls.Add(question_panel);
            }
        }

        /// <summary>
        /// Grade the students.
        /// </summary>
        public void GradeStudents()
        {
            if(uxExamNameTextBox.Text == "")
            {
                DisplayMessage("Go to the Start Tab and enter a name for the exam.");
                return;
            }

            if(number_of_versions == 0 || number_of_questions == 0)
            {
                DisplayMessage("You have not specified the number of versions and questions. Go back to the Start tab and follow the instructions.");
                return;
            }

            if(grader.Students.Count < 1)
            {
                DisplayMessage("No students found. You  need to save changes on the Scan tab.");
                return;
            }

            if(uxGradingWithThisProgramCheckbox.Checked && grader.AnswerKey.Count < 1)
            {
                DisplayMessage("If you're grading with this program you need to make an answer key in the Answer Key tab." +
                                "If you're not grading with this program, go back to the Start tab and uncheck the box.");
                return;
            }

            grader.GradeStudents(uxExamNameTextBox.Text);
            uxStudentSelector.Items.Clear();
            foreach (Student student in grader.Students)
            {
                uxStudentSelector.Items.Add(student.WID);
            }

            uxNextButton.Enabled = true;
            uxPreviousButton.Enabled = true;
            NextStudent(); // Displays the first student in the index
        }

        /// <summary>
        /// Display the selected student's response in uxStudentResponsePanel.
        /// </summary>
        /// <param name="student">Selected student.</param>
        private void DisplayStudent(Student student)
        {
            if (uxStudentSelector.SelectedIndex == 0)
            {
                uxPreviousButton.Enabled = false;
            }
            else
            {
                uxPreviousButton.Enabled = true;
            }

            if (uxStudentSelector.SelectedIndex == uxStudentSelector.Items.Count - 1)
            {
                uxNextButton.Enabled = false;
            }
            else
            {
                uxNextButton.Enabled = true;
            }

            int test_version = student.TestVersion;
            uxVersionLabel.Text = "Version: " + test_version;
            uxScoreLabel.Text = "Score: " + student.Score();
            Panel question_panel;
            CheckBox response_checkbox;
            CheckBox answer_key_checkbox;

            foreach (Control control in uxStudentResponsePanel.Controls)
            {
                control.Hide();
            }

            if (test_version > grader.AnswerKey.Keys.Count || test_version <= 0)
            {
                // Stop the next for loop from throwing an error about the student's test version not existing.
                // User is notified elsewhere.
                return;
            }

            for (int i = 0; i < Math.Min(grader.AnswerKey[test_version - 1].Count, student.Response.Count); i++)
            {
                question_panel = (Panel)uxStudentResponsePanel.Controls[i];
                question_panel.Show();

                if (student.Response[i].Points == grader.AnswerKey[test_version - 1][i].Points)
                {
                    question_panel.BackColor = Color.Green;
                }
                else if (student.Response[i].Points == 0)
                {
                    question_panel.BackColor = Color.Red;
                }
                else
                {
                    question_panel.BackColor = Color.Orange;
                }

                for (int j = 0; j < 5; j++)
                {
                    response_checkbox = (CheckBox)question_panel.Controls[j];
                    answer_key_checkbox = (CheckBox)uxAnswerKeyTabControl.TabPages[test_version - 1].Controls[i].Controls[j];

                    if (student.Response[i].Answer[j] == ' ')
                    {
                        response_checkbox.Checked = false;
                    }
                    else
                    {
                        response_checkbox.Checked = true;
                    }
                }

                question_panel.Controls[5].Text = "Points: " + student.Response[i].Points.ToString("0.00");
            }
        }

        /// <summary>
        /// Populates the student answer panel with question panels that show the selected student's response.
        /// </summary>
        public void SelectStudent()
        {
            DisplayStudent(grader.Students.Find(student => student.WID == uxStudentSelector.Text));
        }

        /// <summary>
        /// Displays the previous student's responses in the uxStudentResponsePanel.
        /// </summary>
        public void PreviousStudent()
        {
            string wid = (string)uxStudentSelector.Items[uxStudentSelector.SelectedIndex - 1];
            uxStudentSelector.SelectedItem = wid;
        }

        /// <summary>
        /// Displays the next student's responses in the uxStudentResponsePanel.
        /// </summary>
        public void NextStudent()
        {
            string wid = (string)uxStudentSelector.Items[uxStudentSelector.SelectedIndex + 1];
            uxStudentSelector.SelectedItem = wid;
        }

        /// <summary>
        /// Change the current tab to the Create File tab.
        /// </summary>
        public void GradeContinue()
        {
            uxMainTabControl.SelectTab("uxCreateFileTabPage");
        }

        /// <summary>
        /// Write the file to be uploaded to the Canvas gradebook.
        /// </summary>
        public void WriteFile(string type)
        {
            string file = "";
            string filter = "txt files (*.txt)|*.txt";

            if (type == "gradebook")
            {
                file = grader.GradebookFile();
                filter = "csv files (*.csv)|*.csv";
            }
            if (type == "single")
            {
                file = grader.SingleAnswerFile();
            }
            if (type == "multiple")
            {
                file = grader.MultipleAnswerFile();
            }

            // Then we have to start a file dialog to save the string to a file.
            SaveFileDialog uxSaveFileDialog = new SaveFileDialog
            {
                // the name given by the user at the beginning.
                FileName = uxExamNameTextBox.Text,

                // Could be used to select the default directory ex. "C:\Users\Public\Desktop".
                InitialDirectory = "c:\\desktop",
                // Filter is the default file extensions seen by the user.
                Filter = filter,
                // FilterIndex sets what the user initially sees ex: 2nd index of the filter is ".txt".
                FilterIndex = 1
            };

            if (uxSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = uxSaveFileDialog.FileName;
                // Stores the location of the file we want to save; use filenames for multiple.
                if (path.Equals(""))
                {
                    MessageBox.Show("You must enter a filename and select\n" +
                                    "a file path for the exam record!");
                    throw new IOException();
                }
                else
                {
                    // "using" opens and close the StreamWriter.
                    using (StreamWriter file_generator = new StreamWriter(path))
                    {
                        // Adds everything in the 'file' given to the streamwriter.
                        file_generator.Write(file);
                    }
                    MessageBox.Show("Student responses have been successfully recorded!");
                }
            }
            /*  //probably won't need this, but I have regretted deleting things before.
            else
            {
                MessageBox.Show("An error occured while trying to save,\n" +
                                "The format for filenames should not include\n" +
                                "slashes, parentheticals, or symbols\n" +
                                "Please reload the hopper and ensure the\n" +
                                "cards are not stuck together, backwards,\n" +
                                "or reversed. ");
                //throw new IOException();
            }
            */
        }

        /// <summary>
        /// Change current tab to Start tab and reset data.
        /// </summary>
        public void Finish()
        {
            uxMainTabControl.SelectTab(uxStartTabPage);
            Reset();
        }
    }
}
