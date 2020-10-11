using System.Security.Cryptography;
using System.Text;

namespace BookRentalShopApp
{
    public enum BaseMode
    {
        NONE,   //기본 상태
        INSERT, //입력 상태
        UPDATE, //수정 상태
        DELETE, //
        SELECT  //
    }
    public class Commons
    {
        public static string USERID = string.Empty;


        //공통 DB연결 문자열
        public static readonly string CONNSTR = "Data Source=localhost;port=3306;Database=bookrentalshop;Uid=root;Password=mysql_p@ssw0rd";
        
        /// <summary>
        /// MD5 암호화 메소드
        /// </summary>
        /// <param name="md5hash">해시 키값</param>
        /// <param name="input">평문 문자열</param>
        /// <returns>암호화된 문자열</returns>
        public static string GetMD5Hash(MD5 md5hash,string input)
        {
            byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(input)); // byte 로 형변환 후 ComputeHash에 입력.
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }
        

    }
}
