using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;

namespace BookRentalShopApp.SubItems
{
    public partial class MembMngForm : MetroForm
    {
        readonly string strTblName = "memberTbl";

        BaseMode MyMode = BaseMode.NONE;
        public MembMngForm()
        {
            InitializeComponent();
        }

        private void DivMngForm_Load(object sender, EventArgs e)
        {

            UpdateComboboxDivision();
            UpdateData();
            InitControls();
        }

        private void UpdateComboboxDivision()
        {
            Dictionary<string, string> temps = new Dictionary<string, string>();
            temps.Add("선택","");
            temps.Add("A", "A");
            temps.Add("B", "B");
            temps.Add("C", "C");
            temps.Add("D", "D");


            //콤보박스 
            CboLevel.DataSource = new BindingSource(temps, null);
            CboLevel.DisplayMember = "Key"; //화면 보이는 글자 ex) 로맨스 판타지
            CboLevel.ValueMember = "Value"; // 실제 값 ex) B001,B002
          //CboDivision.SelectedIndex = -1;
        }

        private void UpdateData()
        {
            //DB 연결
            using (MySqlConnection con = new MySqlConnection(Commons.CONNSTR))
            {
                string strQuery = $"SELECT * FROM membertbl;";

                con.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds, strTblName);

                GrdMembTbl.DataSource = ds;
                GrdMembTbl.DataMember = strTblName;
            }

            SetColumnHeaders();

        }

        private void SetColumnHeaders()
        {
            DataGridViewColumn column;

            column = GrdMembTbl.Columns[0];
            column.Width = 100;
            column.HeaderText = "번호";

            column = GrdMembTbl.Columns[1];
            column.Width = 120;
            column.HeaderText = "이름";

            column = GrdMembTbl.Columns[2];
            column.Width = 120;
            column.HeaderText = "등급";

            column = GrdMembTbl.Columns[3];
            column.Width = 100;
            column.HeaderText = "주소";

            column = GrdMembTbl.Columns[4];
            column.Width = 200;
            column.HeaderText = "전화 번호";

            column = GrdMembTbl.Columns[5];
            column.Width = 150;
            column.HeaderText = "Email";

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MyMode != BaseMode.UPDATE)
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
            TxtIdx.Text = TxtNames.Text = TxtAddr.Text = TxtMobile.Text = TxtEmail.Text = string.Empty;
            CboLevel.SelectedIndex = 0; // 선택
            TxtIdx.Focus();
            MyMode = BaseMode.NONE;



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
            InitControls();
            MyMode = BaseMode.INSERT; // 신규입력 모드
        }

        /// <summary>
        /// DB 업데이트 및 입력
        /// </summary>
        private void SaveData()
        {
            #region Null 체크
            //빈값 비교 NULL 체크
            if (CboLevel.SelectedIndex < 1 || string.IsNullOrEmpty(TxtNames.Text) || string.IsNullOrEmpty(TxtMobile.Text))
            {
                MetroMessageBox.Show(this, "빈 값은 넣을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion
            #region 모드 체크
            if (MyMode == BaseMode.NONE)
            {
                MetroMessageBox.Show(this, "신규등록 시 신규 버튼을 눌러주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR)) //using 사용시 conn.close() 안씀
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    if (MyMode == BaseMode.UPDATE)
                    {
                        cmd.CommandText = "UPDATE membertbl " +
                                          "   SET " +
                                          "       Names  = @Names, " +
                                          "       Levels = @Levels, " +
                                          "       Addr   = @Addr, " +
                                          "       Mobile = @Mobile, " +
                                          "       Email  = @Email " +
                                          "   WHERE Idx  = @Idx"; //Update문 작성
                    }
                    else if (MyMode == BaseMode.INSERT)
                    {
                        cmd.CommandText = "INSERT INTO membertbl " +
                                          "            ( " +
                                          "            Names, " +
                                          "            Levels, " +
                                          "            Addr, " +
                                          "            Mobile, " +
                                          "            Email " +
                                          "            ) " +
                                          " VALUES " +
                                          "            ( " +
                                          "            @Names, " +
                                          "            @Levels, " +
                                          "            @Addr, " +
                                          "            @Mobile, " +
                                          "            @Email)";

                    }


                    //파라미터 설정

                    //이름
                    MySqlParameter paramNames = new MySqlParameter("@Names", MySqlDbType.VarChar, 45)
                    {
                        Value = TxtNames.Text
                    };
                    cmd.Parameters.Add(paramNames);

                    //등급
                    MySqlParameter paramLevels = new MySqlParameter("@Levels", MySqlDbType.VarChar)
                    {
                        Value = CboLevel.SelectedValue
                    };
                    cmd.Parameters.Add(paramLevels);

                    //주소
                    MySqlParameter paramAddr = new MySqlParameter("@Addr", MySqlDbType.VarChar, 100)
                    {
                        Value = TxtAddr.Text
                    };
                    cmd.Parameters.Add(paramAddr);

                    //전화번호
                    MySqlParameter paramMobile = new MySqlParameter("@Mobile", MySqlDbType.VarChar, 13)
                    {
                        Value = TxtMobile.Text
                    };
                    cmd.Parameters.Add(paramMobile);

                    //Email
                    MySqlParameter paramEmail = new MySqlParameter("@Email", MySqlDbType.VarChar, 50)
                    {
                        Value = TxtEmail.Text
                    };
                    cmd.Parameters.Add(paramEmail);

                    if (MyMode == BaseMode.UPDATE)
                    {
                        //Idx PK
                        MySqlParameter paramIdx = new MySqlParameter("@Idx", MySqlDbType.Int32)
                        {
                            Value = TxtIdx.Text
                        };
                        cmd.Parameters.Add(paramIdx);
                    }


                    //else if(MyMode == BaseMode.DELETE)
                    //{
                    //    MySqlParameter paramDivison = new MySqlParameter("@Division", MySqlDbType.VarChar);
                    //    paramDivison.Value = TxtNum.Text;
                    //    cmd.Parameters.Add(paramDivison);
                    //}



                    var result = cmd.ExecuteNonQuery(); //쿼리문 실행.




                    if (MyMode == BaseMode.INSERT)
                    {
                        MetroMessageBox.Show(this, $"{result}건이 신규입력되었습니다.", "신규입력");
                    }
                    else if (MyMode == BaseMode.UPDATE)
                    {
                        MetroMessageBox.Show(this, $"{result}건이 수정되었습니다.", "수정");
                    }
                    else if (MyMode == BaseMode.DELETE)
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
                TxtIdx.Focus();
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
            if (e.RowIndex > -1)
            {
                //Grid 셀 중 Rows 데이터 들고옴
                DataGridViewRow data = GrdMembTbl.Rows[e.RowIndex];//선택한 행의 데이터 들고옴
                TxtIdx.Text = data.Cells[0].Value.ToString();       //TxtBox에 출력
                TxtNames.Text = data.Cells[1].Value.ToString();    //TxtBox에 출력

                //콤보박스
                //로맨스,추리등 디스플레이되는 글자로 인덱스 찾기
                CboLevel.SelectedIndex = CboLevel.FindString(data.Cells[2].Value.ToString());

                //코드값을 그대로 할당
                //CboLevel.SelectedValue = data.Cells[2].Value.ToString();

                TxtAddr.Text = data.Cells[3].Value.ToString();      //TxtBox에 출력
                TxtMobile.Text = data.Cells[4].Value.ToString();     //TxtBox에 출력
                TxtEmail.Text = data.Cells[5].Value.ToString();     //TxtBox에 출력

                TxtIdx.ReadOnly = true; //PK가 들어가는 텍스트는 수정 안함!!
                MyMode = BaseMode.UPDATE; // 수정 모드 변경
            }


        }
    }
}
