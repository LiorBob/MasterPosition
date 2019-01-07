using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;





namespace MasterPosition
{
    public partial class FrmMasterPosition : Form
    {
        // Gets auto search URLs from text file  
        
        private string[] autoSearchURLs = File.ReadAllLines(Application.StartupPath + "\\autoSearchURLs.txt");
        private CheckBox readCompanyDescription;




        public FrmMasterPosition()
        {
            InitializeComponent();
            GenerateButtons();

            this.MinimumSize = new Size(270, 210);          // so we can't decrease form size too much
        }



        private void FrmMasterPosition_Resize(object sender, EventArgs e)
        {
            Controls.Clear();                   // Removing the existing buttons before creating them again
            GenerateButtons();
        }




        // Generates the buttons  with their background images and tags (image name is the actual link)

        private void GenerateButtons()
        {
            readCompanyDescription = new CheckBox
            {
                Location = new Point(15, 10),
                Size = new Size(350, 30),
                Text = "Read companies description on buttons",
                ForeColor = Color.Blue
            };

            this.Controls.Add(readCompanyDescription);




            int x = 30;
            int y = 80;




            // Gets all bmp files  ordered by their creation time, from the oldest to the newest

            List<string> allCompaniesPhotosNames = Directory.GetFiles(Application.StartupPath, "*.bmp", SearchOption.TopDirectoryOnly).OrderBy(f => new FileInfo(f).CreationTime).ToList();


            foreach (string companyPhotoName in allCompaniesPhotosNames)
            {
                Image companyPhoto = Image.FromFile(companyPhotoName);


                Button companyButton = new Button
                {
                    Location = new Point(x, y),
                    Size = companyPhoto.Size,
                    BackgroundImage = companyPhoto,
                    Tag = Path.GetFileNameWithoutExtension(companyPhotoName)
                };

                companyButton.Click += new System.EventHandler(this.Button_Click);
                companyButton.MouseHover += new System.EventHandler(this.Button_MouseHover);

                this.Controls.Add(companyButton);





                // For each button we add a checkbox for auto search option

                CheckBox autoSearchCheckBox = new CheckBox
                {
                    Location = new Point(companyButton.Location.X - 15, companyButton.Location.Y - 10 + companyButton.Height / 2),
                    Size = new Size(20, 20),
                    Tag = companyButton.Tag
                };


                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(autoSearchCheckBox, "Auto Search");

                this.Controls.Add(autoSearchCheckBox);





                x += companyButton.Width + 50;                 // Sets the location of the next button (same row)


                if (x >= this.Width)
                {
                    x = 30;
                    y += companyButton.Height + 50;            // Sets the location of the next button (next row)
                }
            }
        }




        // Actually opens the required link (the company site)

        private void Button_Click(object sender, EventArgs e)
        {
            Button activatedButton = (Button)sender;

            string companyLinkToOpen = activatedButton.Tag.ToString();




            // Checks the state of the checkbox associated with the activated (clicked) button

            CheckBox autoSearchCheckBoxForActivatedButton = Controls.OfType<CheckBox>().Where(c => c.Tag == activatedButton.Tag).First();



            if (!autoSearchCheckBoxForActivatedButton.Checked)              // CheckBox is OFF
            {
                Process.Start(companyLinkToOpen);
            }

            else                                                            // CheckBox is ON: get the auto search URL associated with companyLinkToOpen  from autoSearchURLs string array defined above
            {
                string autoSearchCompanyLinkToOpen = autoSearchURLs.Where(x => x.StartsWith(companyLinkToOpen)).First();

                Process.Start(autoSearchCompanyLinkToOpen);
            }
        }






        // When we hover on a button with the mouse - play company description sound file

        private void Button_MouseHover(object sender, EventArgs e)
        {
            if (readCompanyDescription.Checked)
            {
                Button activatedButton = (Button)sender;

                string company = activatedButton.Tag.ToString();
                string companySoundFile = Application.StartupPath + "\\" + company + ".wav";


                System.Media.SoundPlayer companySoundDescription = new System.Media.SoundPlayer(companySoundFile);
                companySoundDescription.Play();
            }
        }

    }

}
