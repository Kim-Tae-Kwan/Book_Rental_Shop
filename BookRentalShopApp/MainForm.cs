using System;
using System.Windows.Forms;
using BookRentalShopApp.SubItems;
using MetroFramework;
using MetroFramework.Forms;

namespace BookRentalShopApp
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.ShowDialog();

            LbLUserID.Text = $"LOGIN : {Commons.USERID}";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MetroMessageBox.Show(this, "종류하시겠습니까?", "종료", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result ==DialogResult.Yes)//프로그램 종료
            {
                e.Cancel = false;
                Environment.Exit(0);
            }
            else // 프로그램 종료 안함
            {
                e.Cancel = true;
            }
        }

        private void MnuItemCodeMng_Click(object sender, EventArgs e)
        {
            DivMngForm form = new DivMngForm();
            ShowFormControl(form,"코드 관리");
        }

        private void MnuItemExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuItemBooksMng_Click(object sender, EventArgs e)
        {
            BookDBMngForm form = new BookDBMngForm();
            ShowFormControl(form,"도서 관리");
        }

        private void ShowFormControl(Form form,string title) // Division,Book  Form 생성하는 함수
        {
            form.MdiParent = this;
            form.Text = title;
            form.Dock = DockStyle.Fill;
            form.Show();
            form.WindowState = FormWindowState.Maximized;
        }

        private void MnuItemMembMng_Click(object sender, EventArgs e)
        {
            MembMngForm form = new MembMngForm();
            ShowFormControl(form, "회원 관리");
        }

        private void MnuItemRentalMng_Click(object sender, EventArgs e)
        {
            RentalMngForm form = new RentalMngForm();
            ShowFormControl(form, "대여 관리");
        }

        private void MnuItemUserMng_Click(object sender, EventArgs e)
        {

        }
    }
}
