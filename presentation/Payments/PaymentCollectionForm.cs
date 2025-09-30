using iTextSharp.text;
using iTextSharp.text.pdf;
using MicroFinance_Loan.presentation.Dashboards;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.presentation.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Payments
{
    public partial class PaymentCollectionForm : BaseForm
    {
        private Loan currentLoan;
        private List<Loan> allLoans;
        public PaymentCollectionForm()
        {
            InitializeComponent();
            LoadMemberLoans();
            string[] paymentMethods = { "Cash", "Check", "Mobile Banking", "Bank Transfer" };
            comboBox1.DataSource = paymentMethods;
        }

        private void PaymentCollectionForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadLoanInfo()
        {

            RepaymentService repaymentService = new RepaymentService();
            decimal outstanding = repaymentService.GetOutstandingBalance(currentLoan.LoanID);
            string loanInfo = $"Member: {currentLoan.MemberName}  \n" +
                             $"Approved Amount: ৳ {currentLoan.ApprovedAmount:N0} \n" +
                             $"Interest Rate: {currentLoan.InterestRate}% \n" +
                             $"Duration: {currentLoan.DurationMonths} months \n" +
                             $"Outstanding Balance: ৳ {outstanding:N0}\n";

            label4.Text = loanInfo;
        }

        private void LoadDuePayments()
        {
            try
            {
                string query = @"SELECT InstallmentNumber, DueDate, PrincipalAmount, 
                            InterestAmount, TotalAmount
                            FROM LoanSchedule 
                            WHERE LoanID = @LoanID AND IsPaid = 0 
                            ORDER BY InstallmentNumber";

                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LoanID", currentLoan.LoanID);
                    conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading due payments: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidatePaymentInputs()
        {
            if (!decimal.TryParse(textBox2.Text, out decimal amount) || amount <= 0)
            {
                ShowMessage("Please enter a valid payment amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (dateTimePicker1.Value.Date != DateTime.Now.Date)
            {
                ShowMessage("You can only collect payments for today.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }


            return true;
        }

        private void ClearPaymentFields()
        {
            textBox2.Clear();
            dateTimePicker1.Value = DateTime.Now;
            comboBox1.SelectedIndex = 0;
            richTextBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an installment to pay.", "No Selection");
                    return;
                }

                if (!ValidatePaymentInputs())
                    return;

                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                int installmentNumber = Convert.ToInt32(selectedRow.Cells["InstallmentNumber"].Value);

                string totalAmountStr = selectedRow.Cells["TotalAmount"].Value.ToString().Replace("৳", "").Replace(",", "").Trim();
                decimal amountDue = Convert.ToDecimal(totalAmountStr);

                Repayment payment = new Repayment
                {
                    LoanID = currentLoan.LoanID,
                    InstallmentNumber = installmentNumber,
                    AmountDue = amountDue,
                    AmountPaid = Convert.ToDecimal(textBox2.Text),
                    PaymentDate = dateTimePicker1.Value,
                    DueDate = DateTime.Parse(selectedRow.Cells["DueDate"].Value.ToString()),
                    PaymentMethod = comboBox1.Text,
                    CollectedBy = CurrentUser.UserID,
                    Notes = richTextBox1.Text.Trim()
                };

                RepaymentService repaymentService = new RepaymentService();
                if (repaymentService.RecordPayment(payment))
                {
                    ShowMessage("Payment recorded successfully!", "Success");
                    LoadDuePayments();
                    LoadLoanInfo();
                    ClearPaymentFields();
                    button1.Enabled = true;

                    button2.Tag = payment;   // store the payment object for later
                    button2.Enabled = true;
                }
                else
                {
                    ShowMessage("Failed to record payment.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error recording payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OfficerDashboard officerDashboard = new OfficerDashboard();
            officerDashboard.Show();
            this.Close();
        }

        private void LoadMemberLoans()
        {
            try
            {
                LoanRepository loanRepo = new LoanRepository();
                allLoans = loanRepo.GetAll()
                    .Where(l => l.Status == "Approved" || l.Status == "Disbursed")
                    .ToList();

                var loanDisplayList = allLoans.Select(l => new
                {
                    LoanID = l.LoanID,
                    DisplayText = $"Loan #{l.LoanID} - {l.PolicyType} (৳ {l.ApprovedAmount:N0})"
                }).ToList();

                comboBox2.DataSource = loanDisplayList;
                comboBox2.DisplayMember = "DisplayText";
                comboBox2.ValueMember = "LoanID";

                if (comboBox2.Items.Count > 0)
                    comboBox2.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading loans: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedItem != null && int.TryParse(comboBox2.SelectedValue.ToString(), out int selectedLoanID))
                {
                    currentLoan = allLoans.FirstOrDefault(l => l.LoanID == selectedLoanID);

                    if (currentLoan == null)
                    {
                        ShowMessage("Loan not found.", "Not Found");
                        return;
                    }

                    if (currentLoan.Status != "Approved" && currentLoan.Status != "Disbursed")
                    {
                        ShowMessage("This loan is not in a payable status.", "Invalid Status");
                        return;
                    }

                    LoadLoanInfo();
                    LoadDuePayments();
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching loan: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button2.Tag is Repayment payment)
            {
                GenerateReceipt(payment, currentLoan);
            }
            else
            {
                ShowMessage("No payment selected for receipt.", "Error");
            }
        }

        private void GenerateReceipt(Repayment payment, Loan loan)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Receipt_Loan_{loan.LoanID}_Installment_{payment.InstallmentNumber}.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Document doc = new Document(PageSize.A5, 30, 30, 20, 20);
                    PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                    Chunk c = new Chunk("Payment Receipt\n\n", titleFont);
                    Paragraph title = new Paragraph(c);

                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);

                    Font normalFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL);
                    
                    Paragraph details = new Paragraph(
                        $"Receipt No: {Guid.NewGuid().ToString().Substring(0, 8)}\n" +
                        $"Date: {payment.PaymentDate:dd MMM yyyy}\n\n" +
                        $"Member Name: {loan.MemberName}\n" +
                        $"Loan ID: {loan.LoanID}\n" +
                        $"Installment No: {payment.InstallmentNumber}\n" +
                        $"Payment Method: {payment.PaymentMethod}\n\n" +
                        $"Amount Due: ৳ {payment.AmountDue:N2}\n" +
                        $"Amount Paid: ৳ {payment.AmountPaid:N2}\n" +
                        $"Late Fee: ৳ {(payment.LateFee > 0 ? payment.LateFee : 0):N2}\n\n" +
                        $"Collected By (Officer ID): {payment.CollectedBy}\n" +
                        $"Notes: {payment.Notes}\n",
                        normalFont
                    );
                    doc.Add(details);

                    Paragraph footer = new Paragraph("\nThank you for your payment!", normalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(footer);

                    doc.Close();

                    MessageBox.Show("Receipt generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        }
}

