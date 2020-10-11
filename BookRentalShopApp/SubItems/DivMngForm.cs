using System;
using System.Data;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;


namespace BookRentalShopApp.SubItems
{
    public partial class DivMngForm : MetroForm
    {
        readonly string strTblName = "divTbl";

        BaseMode MyMode=BaseMode.NONE;
        public DivMngForm()
        {
            InitializeComponent();
        }

        private void DivMngForm_Load(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            using (MySqlConnection con = new MySqlConnection(Commons.CONNSTR))
            {
                string strQuery = $"SELECT Division,Names FROM {strTblName}";
                con.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds, strTblName);

                GrdDivTbl.DataSource = ds;
                GrdDivTbl.DataMember = strTblName;
            }

            SetColumnHeaders();

        }

        private void SetColumnHeaders()
        {
            DataGridViewColumn column;

            column = GrdDivTbl.Columns[0];
            column.Width = 100;
            column.HeaderText = "구분코드";

            column = GrdDivTbl.Columns[1];
            column.Width = 150;
            column.HeaderText = "이름";
            
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if(MyMode != BaseMode.UPDATE)
            {
                MetroMessageBox.Show(this, "삭제할 데이터를 선택하세요!!", "알림");
                return;
            }
            MyMode = BaseMode.DELETE;
            SaveData();
            InitControls();
        }

        private void InitControls()
        {
            TxtDivison.Text = TxtNames.Text = String.Empty;
            TxtDivison.Focus();
        }
        #region 삭제 메소드 주석
        /*
        private void DeleteProcess()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR)) //using 사용시 conn.close() 안씀
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM divtbl " +
                                      "  WHERE Divison = @Division";
                    MySqlParameter paramDivison = new MySqlParameter("@Division", MySqlDbType.VarChar);
                    paramDivison.Value = TxtDivison.Text;
                    cmd.Parameters.Add(paramDivison);

                    cmd.ExecuteNonQuery();
                }
                    
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, $"에러 발생 {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                TxtDivison.Focus();
                UpdateData();
            }

        }
        */
        #endregion

        private void BtnNew_Click(object sender, EventArgs e)
        {
            TxtDivison.Text = TxtNames.Text = string.Empty;
            TxtDivison.ReadOnly = false;

            TxtDivison.Focus();
            MyMode = BaseMode.INSERT; // 신규입력 모드
        }

        /// <summary>
        /// DB 업데이트 및 입력
        /// </summary>
        private void SaveData()
        {
            if (string.IsNullOrEmpty(TxtDivison.Text) || string.IsNullOrEmpty(TxtNames.Text))
            {
                MetroMessageBox.Show(this, "빈 값은 넣을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(MyMode == BaseMode.NONE)
            {
                MetroMessageBox.Show(this, "신규등록 시 신규 버튼을 눌러주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR)) //using 사용시 conn.close() 안씀
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    if(MyMode == BaseMode.UPDATE)
                    {
                        cmd.CommandText = "UPDATE divtbl " +
                                          "  SET Names = @Names " +
                                          " WHERE Division = @Division"; //Update문 작성
                    }
                    else if(MyMode == BaseMode.INSERT)
                    {
                        cmd.CommandText = "INSERT INTO "+
                                          " divtbl (Division,Names) "+
                                          " VALUES(@Division, @Names); ";
                    }
                    else if(MyMode == BaseMode.DELETE)
                    {
                        cmd.CommandText = "DELETE FROM divtbl " +
                                      "  WHERE Division = @Division";
                    }

                    if(MyMode == BaseMode.INSERT || MyMode == BaseMode.UPDATE)
                    {
                        MySqlParameter paramNames = new MySqlParameter("@Names", MySqlDbType.VarChar, 45); // 파라미터 설정
                        paramNames.Value = TxtNames.Text;
                        cmd.Parameters.Add(paramNames); //@Names <=== paramNames.Value

                        MySqlParameter paramDivision = new MySqlParameter("@Division", MySqlDbType.VarChar, 4);
                        paramDivision.Value = TxtDivison.Text;
                        cmd.Parameters.Add(paramDivision); //@Division <=== paramDivision.Value
                    }
                    else if(MyMode == BaseMode.DELETE)
                    {
                        MySqlParameter paramDivison = new MySqlParameter("@Division", MySqlDbType.VarChar);
                        paramDivison.Value = TxtDivison.Text;
                        cmd.Parameters.Add(paramDivison);
                    }

                    var result = cmd.ExecuteNonQuery(); //쿼리문 실행.

                    if(MyMode == BaseMode.INSERT)
                    {
                        MetroMessageBox.Show(this, $"{result}건이 신규입력되었습니다.", "신규입력");
                    }
                    else if(MyMode == BaseMode.UPDATE)
                    {
                        MetroMessageBox.Show(this, $"{result}건이 수정되었습니다.", "수정");
                    }
                    else if(MyMode == BaseMode.DELETE)
                    {
                        MetroMessageBox.Show(this, $"{result}건이 삭제되었습니다.", "삭제");
                    }
                }



            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, $"에러 발생 {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                TxtDivison.Focus();
                UpdateData();
            }
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            InitControls();
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {

        }

        private void GrdDivTbl_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1)
            {
                //Grid 셀 중 Rows 데이터 들고옴
                DataGridViewRow data = GrdDivTbl.Rows[e.RowIndex];//선택한 행의 데이터 들고옴
                TxtDivison.Text = data.Cells[0].Value.ToString();//행의 0번째 열 데이터
                TxtNames.Text = data.Cells[1].Value.ToString();//행의 1번째 열 데이터

                TxtDivison.ReadOnly = true; //PK가 들어가는 텍스트는 수정 안함!!

                MyMode = BaseMode.UPDATE; // 수정 모드 변경
            }


        }
    }
}
