using iTextSharp.text;
using iTextSharp.text.pdf;
using MicroFinance_Loan.presentation.Dashboards;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.presentation.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MicroFinance_Loan.presentation.Payments
{
    public partial class MakePaymentForm : BaseForm
    {
        private List<Loan> memberLoans;
        private Loan selectedLoan;
        public MakePaymentForm()
        {
            InitializeComponent();
            LoadMemberLoans();
        }

        private void MakePaymentForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadMemberLoans()
        {
            try
            {
                LoanService loanService = new LoanService();
                memberLoans = loanService.GetLoansByMember(CurrentUser.UserID)
                    .Where(l => l.Status == "Approved" || l.Status == "Disbursed")
                    .ToList();

                var loanDisplayList = memberLoans.Select(l => new
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
            if (comboBox2.SelectedItem != null && int.TryParse(comboBox2.SelectedValue.ToString(), out int selectedLoanID))
            {
                selectedLoan = memberLoans.FirstOrDefault(l => l.LoanID == selectedLoanID);

                if (selectedLoan != null)
                {
                    LoadPaymentDetails();
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }

        private void LoadPaymentDetails()
        {
            try
            {
                string query = @"SELECT TOP 1 InstallmentNumber, DueDate, TotalAmount 
                            FROM LoanSchedule 
                            WHERE LoanID = @LoanID AND IsPaid = 0 
                            ORDER BY InstallmentNumber";

                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LoanID", selectedLoan.LoanID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal dueAmount = Convert.ToDecimal(reader["TotalAmount"]);
                        DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);

                        label8.Text = $"৳ {dueAmount:N0}";
                        label11.Text = dueDate.ToString("dd/MM/yyyy");
                        label9.Text = $"৳ {selectedLoan.GetMonthlyEMI():N0}";
                        textBox1.Text = dueAmount.ToString();

                        if (dueDate < DateTime.Now.Date)
                        {
                            label11.ForeColor = Color.Red;
                        }
                        else
                        {
                            label11.ForeColor = Color.Black;
                        }
                    }
                }

                LoadPaymentSchedule();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payment details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadPaymentSchedule()
        {
            try
            {
                string query = @"SELECT InstallmentNumber, DueDate, TotalAmount, IsPaid  
                     FROM LoanSchedule  
                     WHERE LoanID = @LoanID  
                     ORDER BY InstallmentNumber";

                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LoanID", selectedLoan.LoanID);
                    conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dt.Columns.Add("Status", typeof(string));
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
                        bool isPaid = Convert.ToBoolean(row["IsPaid"]);

                        string status = isPaid ? "Paid" : (dueDate < DateTime.Now.Date ? "Overdue" : "Due");
                        row["Status"] = status;
                    }

                    dataGridView1.DataSource = dt;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Status"].Value != null)
                        {
                            string status = row.Cells["Status"].Value.ToString();
                            if (status == "Paid")
                                row.DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218);
                            else if (status == "Overdue")
                                row.DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payment schedule: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an installment to pay.", "No Selection");
                    return;
                }

                if (!ValidatePayment())
                    return;

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int installmentNumber = Convert.ToInt32(selectedRow.Cells["InstallmentNumber"].Value);

                string totalAmountStr = selectedRow.Cells["TotalAmount"].Value.ToString().Replace("৳", "").Replace(",", "").Trim();
                decimal amountDue = Convert.ToDecimal(totalAmountStr);

                Repayment payment = new Repayment
                {
                    LoanID = selectedLoan.LoanID,
                    InstallmentNumber = installmentNumber,
                    AmountDue = amountDue,
                    AmountPaid = Convert.ToDecimal(textBox1.Text),
                    PaymentDate = DateTime.Now,
                    DueDate = DateTime.Parse(selectedRow.Cells["DueDate"].Value.ToString()),
                    PaymentMethod = "self-service",
                    Notes = "Payment made via Member Portal"
                };

                RepaymentService repaymentService = new RepaymentService();
                if (repaymentService.RecordPayment(payment))
                {
                    ShowMessage("Payment recorded successfully!", "Success");
                    button1.Tag = payment;   // store the payment object for later
                    button1.Enabled = true;
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

        private void button2_Click(object sender, EventArgs e)
        {
        //    if (selectedLoan != null)
        //    {
        //        LoanDetailsForm detailsForm = new LoanDetailsForm(selectedLoan.LoanID);
        //        detailsForm.ShowDialog();
        //    }
        }

        private bool ValidatePayment()
        {
            if (selectedLoan == null)
            {
                ShowMessage("Please select a loan.", "No Selection");
                return false;
            }

            if (!decimal.TryParse(textBox1.Text, out decimal amount) || amount <= 0)
            {
                ShowMessage("Please enter a valid payment amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MemberDashboard dashboard = new MemberDashboard();
            dashboard.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button1.Tag is Repayment payment)
            {
                GenerateReceipt(payment, selectedLoan);
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

                    iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                    Chunk c = new Chunk("Payment Receipt\n\n", titleFont);
                    Paragraph title = new Paragraph(c);

                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);

                    iTextSharp.text.Font normalFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL);

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
