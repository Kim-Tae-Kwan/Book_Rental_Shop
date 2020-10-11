using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;

namespace BookRentalShopApp.SubItems
{
    public partial class LoginForm : MetroForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void metroButton2_Click(object sender, EventArgs e)
        {

        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            LoginProcess();
        }

        private void LoginProcess()
        {
            // null 값 전처리
            if(string.IsNullOrEmpty(TxtUserID.Text) || string.IsNullOrEmpty(TxtPassword.Text))
            {
                MetroMessageBox.Show(this,"아이디,패스워드를 입력하세요!!", "로그인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtUserID.Focus();
                return;
            }

            //DB처리
            string resultID = string.Empty; // ""
            try
            {
                using (MySqlConnection con = new MySqlConnection(Commons.CONNSTR))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(); //데이터베이스 명령 인스턴스
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT userID FROM userTBL " + //쿼리문, 공백 포함 주의!!
                                      " WHERE userID = @userID " +    //@ 파라미터 사용
                                      "  AND Password = @Password ";
                    
                    //파라미터 생성
                    MySqlParameter paramUserID = new MySqlParameter("@userID", MySqlDbType.VarChar, 12);
                    paramUserID.Value = TxtUserID.Text.Trim();

                    MySqlParameter paramPassword = new MySqlParameter("@Password", MySqlDbType.VarChar);

                    var md5Hash = MD5.Create();
                    var cryptoPassword = Commons.GetMD5Hash(md5Hash, TxtPassword.Text.Trim());
                    paramPassword.Value = cryptoPassword;

                    cmd.Parameters.Add(paramUserID);
                    cmd.Parameters.Add(paramPassword);
                    
                    //
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if(!reader.HasRows)
                    {
                        MetroMessageBox.Show(this, "아이디나 패스워드를 정확히 입력하세요", "로그인 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        TxtPassword.Text = TxtUserID.Text = string.Empty;
                        TxtUserID.Focus();
                        return;
                    }
                    else 
                    {
                        resultID = reader["userID"] != null ? reader["userID"].ToString() : string.Empty;
                        Commons.USERID = resultID; //200720 12:30 추가
                        MetroMessageBox.Show(this, $"{resultID} 로그인성공");
                    }


                }

            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, $"DB접속에러 : {ex.Message}", "로그인 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

             if(string.IsNullOrEmpty(resultID))
            {
                MetroMessageBox.Show(this, "아이디나 패스워드를 정확히 입력하세요", "로그인 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TxtPassword.Text = TxtUserID.Text = string.Empty;
                TxtUserID.Focus();
                return;
            }
            else
            {
                this.Close();
            }

        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);// 0:에러없이 끝
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Enter 입력시 OK버튼 입력.
            if (e.KeyChar == 13)
            {
                BtnOk_Click(sender, new EventArgs());
            }
        }

        private void TxtUserID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)//Enter 입력시
            {
                TxtPassword.Focus();
            }
        }
    }
}
