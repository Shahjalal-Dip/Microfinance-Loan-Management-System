using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microfinance_Loan_Management_System.Presentation.Authentication
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            BrandNameLable.Text = AppSettings.ApplicationName;
            versionLable.Text = $"Version {AppSettings.Version}";
            companyLable.Text = $"© {DateTime.Now.Year} {AppSettings.Company}, All Rights Reserved.";
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(1);
            parcentageLable.Text = "Loading... "+ progressBar1.Value.ToString() + "%";
        if (progressBar1.Value < 100)
        {
            if (progressBar1.Value <= 30)
                statusLable.Text = "Initializing database connection...";
            else if (progressBar1.Value <= 60)
                statusLable.Text = "Loading application modules...";
            else if (progressBar1.Value <= 90)
                statusLable.Text = "Preparing user interface...";
            else
                statusLable.Text = "Ready!";
        }
        else
        {
            timer1.Stop();
            
            if (!DatabaseConnection.TestConnection())
            {
                MessageBox.Show("Database connection failed. Please check your connection settings.", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();

            }
    }

        private void parcentageLable_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

        }
    }
}
