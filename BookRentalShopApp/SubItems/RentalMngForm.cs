using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace BookRentalShopApp.SubItems
{
    public partial class RentalMngForm : MetroForm
    {
        readonly string strTblName = "rentalTbl";

        BaseMode MyMode = BaseMode.NONE;
        public RentalMngForm()
        {
            InitializeComponent();
        }

        private void DivMngForm_Load(object sender, EventArgs e)
        {

            UpdateComboMember();
            UpdateCombobook();
            UpdateData();
            InitControls();
        }

        private void UpdateCombobook()
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
            {
                string strQuery = $"SELECT b.Idx, " +
                                   "       b.Names, " +
                                   "       (SELECT Names FROM divtbl WHERE Division = b.Division) AS Division " +
                                   "  FROM booksTbl AS b; "; // 쿼리문 
                conn.Open(); // 연결시작
                MySqlCommand cmd = new MySqlCommand(strQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();


                Dictionary<string, string> temps = new Dictionary<string, string>();
                temps.Add("선택", "");

                while (reader.Read())
                {
                    temps.Add($"[{reader[2]}] {reader[1]}", $"{reader[0]}");
                }

                //콤보박스 
                CboBooks.DataSource = new BindingSource(temps, null);
                CboBooks.DisplayMember = "Key";
                CboBooks.ValueMember = "Value";
                //CboMembers.SelectedIndex = -1;

            }
        }

        private void UpdateComboMember()
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
            {
                string strQuery = $"SELECT Idx,Names FROM memberTbl"; // 쿼리문 
                conn.Open(); // 연결시작
                MySqlCommand cmd = new MySqlCommand(strQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();


                Dictionary<string, string> temps = new Dictionary<string, string>();
                temps.Add("선택", "");

                while (reader.Read())
                {
                    temps.Add(reader[1].ToString(), reader[0].ToString());
                }

                //콤보박스 
                CboMembers.DataSource = new BindingSource(temps, null);
                CboMembers.DisplayMember = "Key";
                CboMembers.ValueMember = "Value";
                //CboMembers.SelectedIndex = -1;

            }

        }

        private void UpdateData()
        {
            //DB 연결
            using (MySqlConnection con = new MySqlConnection(Commons.CONNSTR))
            {
                string strQuery = $"SELECT r.idx AS '대여번호', " +
                                   "       m.Names AS '대여회원', " +
                                   "       b.Names AS '대여책제목', " +
                                   "       b.ISBN, " +
                                   "       r.rentalDate AS '대여일', " +
                                   "       r.returnDate AS '반납일', " +
                                   "       r.memberIdx, " +
                                   "       r.bookIdx " +
                                   "  FROM rentaltbl AS r " +
                                   " INNER JOIN membertbl AS m " +
                                   "    ON r.memberIdx = m.Idx " +
                                   " INNER JOIN bookstbl AS b " +
                                   "    ON r.bookIdx = b.Idx";

                con.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds, strTblName);

                GrdRentalbTbl.DataSource = ds;
                GrdRentalbTbl.DataMember = strTblName;
            }

            SetColumnHeaders();

        }

        private void SetColumnHeaders()
        {
            DataGridViewColumn column;

            column = GrdRentalbTbl.Columns[0];
            column.Width = 80;
            column.HeaderText = "대여번호";

            column = GrdRentalbTbl.Columns[1];
            column.Width = 120;
            column.HeaderText = "대여회원";

            column = GrdRentalbTbl.Columns[2];
            column.Width = 120;
            column.HeaderText = "대여 책 제목";

            column = GrdRentalbTbl.Columns[3];
            column.Width = 120;
            column.HeaderText = "ISBM";

            column = GrdRentalbTbl.Columns[4];
            column.Width = 90;
            //column.HeaderText = "대여일";

            column = GrdRentalbTbl.Columns[5];
            column.Width = 90;
            //column.HeaderText = "반납일";

            column = GrdRentalbTbl.Columns[6];
            column.Visible = false; //memberIdx
            column = GrdRentalbTbl.Columns[7];
            column.Visible = false; //bookIdx

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
            TxtIdx.Text = string.Empty;
            CboMembers.SelectedIndex = CboBooks.SelectedIndex = 0;

            //대여일 초기화
            DtpRentalDate.CustomFormat = "yyyy-MM-dd";
            DtpRentalDate.Format = DateTimePickerFormat.Custom;
            DtpRentalDate.Value = DateTime.Now;

            //반납일 초기화
            DtpReturnDate.CustomFormat = " "; //빈 값 넣기
            DtpReturnDate.Format = DateTimePickerFormat.Custom;

            CboBooks.SelectedIndex = 0; // 선택
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
            //if (CboBooks.SelectedIndex < 1 || string.IsNullOrEmpty(TxtNames.Text) || string.IsNullOrEmpty(TxtMobile.Text))
            //{
            //    MetroMessageBox.Show(this, "빈 값은 넣을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
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
                        cmd.CommandText = "UPDATE rentaltbl " +
                                          "   SET " +
                                          "       memberIdx = @memberIdx, " +
                                          "       bookIdx = @bookIdx, " +
                                          "       rentalDate = @rentalDate, " +
                                          "       returnDate = @returnDate " +
                                          " WHERE Idx = @Idx"; //Update문 작성

                    }
                    else if (MyMode == BaseMode.INSERT)
                    {
                        cmd.CommandText = "INSERT INTO rentaltbl " +
                                          " ( " +
                                          "    memberIdx, " +
                                          "    bookIdx, " +
                                          "    rentalDate, " +
                                          "    returnDate " +
                                          " ) " +
                                          " VALUES " +
                                          " ( " +
                                          "    @memberIdx, " +
                                          "    @bookIdx, " +
                                          "    @rentalDate, " +
                                          "    @returnDate " +
                                          " ) ";
                                          

                    }


                    //파라미터 설정

                    //이름
                    MySqlParameter parammemberIdx = new MySqlParameter("@memberIdx", MySqlDbType.Int32);
                    parammemberIdx.Value = CboMembers.SelectedValue;
                    cmd.Parameters.Add(parammemberIdx);

                    MySqlParameter parambookIdx = new MySqlParameter("@bookIdx", MySqlDbType.Int32);
                    parambookIdx.Value = CboBooks.SelectedValue;
                    cmd.Parameters.Add(parambookIdx);

                    MySqlParameter paramrentalDate = new MySqlParameter("@rentalDate", MySqlDbType.Date);
                    paramrentalDate.Value = DtpRentalDate.Value;
                    cmd.Parameters.Add(paramrentalDate);
                   
                    MySqlParameter paramreturnDate = new MySqlParameter("@returnDate", MySqlDbType.Date);
                    if (MyMode == BaseMode.INSERT)
                    {
                        paramreturnDate.Value = null;
                    }
                    else
                        paramreturnDate.Value = DtpReturnDate.Value;
                        
                    cmd.Parameters.Add(paramreturnDate);




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
                DataGridViewRow data = GrdRentalbTbl.Rows[e.RowIndex];//선택한 행의 데이터 들고옴
                TxtIdx.Text = data.Cells[0].Value.ToString();       //TxtBox에 출력
                CboMembers.SelectedValue = data.Cells[6].Value.ToString();
                CboBooks.SelectedValue = data.Cells[7].Value.ToString();

                DtpRentalDate.Value = DateTime.Parse(data.Cells[4].Value.ToString());

                if (!string.IsNullOrEmpty(data.Cells[5].Value.ToString()))
                {
                    DtpReturnDate.CustomFormat = "yyyy-MM-dd";
                    DtpReturnDate.Format = DateTimePickerFormat.Custom;
                    DtpReturnDate.Value = DateTime.Parse(data.Cells[5].Value.ToString());
                }
                else
                {
                    DtpReturnDate.CustomFormat = " "; //빈 값 넣기
                    DtpReturnDate.Format = DateTimePickerFormat.Custom;
                }

                TxtIdx.ReadOnly = true; //PK가 들어가는 텍스트는 수정 안함!!
                MyMode = BaseMode.UPDATE; // 수정 모드 변경
            }


        }

        private void DtpReturnDate_ValueChanged(object sender, EventArgs e)
        {
            DtpReturnDate.CustomFormat = "yyyy-MM-dd";
            DtpReturnDate.Format = DateTimePickerFormat.Custom;
        }

        private void BtnExcelExprot_Click(object sender, EventArgs e) // 엑셀로 변환
        {
            IWorkbook workbook = new HSSFWorkbook();//xls // XSSFWorkbook(); xlsx
            ISheet sheet1 = workbook.CreateSheet("sheet1");
            sheet1.CreateRow(0).CreateCell(0).SetCellValue("Rental Book Data");
            int x = 1;

            DataSet ds = GrdRentalbTbl.DataSource as DataSet;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i);
                for (int j = 0; j < ds.Tables[0].Rows[0].ItemArray.Length; j++)
                {
                    if(j == 4 || j == 5)
                    {
                        var value = string.IsNullOrEmpty(ds.Tables[0].Rows[i].ItemArray[j].ToString()) ? "" : ds.Tables[0].Rows[i].ItemArray[j].ToString().Substring(0, 10);
                        row.CreateCell(j).SetCellValue(value);
                    }
                    else if(j>5)
                    {
                        break;
                    }
                    else
                    {
                        row.CreateCell(j).SetCellValue(ds.Tables[0].Rows[i].ItemArray[j].ToString());
                    }
                }
            }

            FileStream file = File.Create(Environment.CurrentDirectory + $@"\export.xls"); //xlsx //xls
            workbook.Write(file);
            file.Close();

            MessageBox.Show("Export done!!");
        }
    }
}
