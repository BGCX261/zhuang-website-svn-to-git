using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Zhuang.Utility.Common.DataBase;
using System.Collections;

namespace Zhuang.DAL
{
    public class Sys_UserDAL
    {
        public DataTable GetPage(int page, int pageSize, ref int totalRowCount, string conditions)
        {
            SqlParameter[] sps = new SqlParameter[9];
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i] = new SqlParameter();
            }

            sps[0].ParameterName = "@strTable";
            sps[0].SqlDbType = SqlDbType.VarChar;
            sps[0].Value = "Sys_User";

            sps[1].ParameterName = "@strColumn";
            sps[1].SqlDbType = SqlDbType.VarChar;
            sps[1].Value = "userid";

            sps[2].ParameterName = "@intColType";
            sps[2].SqlDbType = SqlDbType.Int;
            sps[2].Value = 0;

            sps[3].ParameterName = "@intOrder";
            sps[3].SqlDbType = SqlDbType.Bit;
            sps[3].Value = 0;

            sps[4].ParameterName = "@strColumnlist";
            sps[4].SqlDbType = SqlDbType.VarChar;
            sps[4].Value = "userid,userName,password,name,gender,address,birthday";

            sps[5].ParameterName = "@intPageSize";
            sps[5].SqlDbType = SqlDbType.Int;
            sps[5].Value = pageSize;



            sps[6].ParameterName = "@intPageNum";
            sps[6].SqlDbType = SqlDbType.Int;
            sps[6].Value = page;



            sps[7].ParameterName = "@strWhere";
            sps[7].SqlDbType = SqlDbType.VarChar;
            sps[7].Value = "userName like '%" + conditions + "%' or name like '%" + conditions + "' or address like '%" + conditions + "%' or gender like '%" + conditions + "%'";


            sps[8].ParameterName = "@totalRowCount";
            sps[8].SqlDbType = SqlDbType.Int;
            sps[8].Direction = ParameterDirection.Output;

            DataTable dtResult;

            dtResult = SqlHelper.RunProcedure("sp_page", sps, "Sys_User").Tables[0];


            totalRowCount = Convert.ToInt32(sps[8].Value);

            return dtResult;
        }

        public DataTable Get(int id)
        {
            string strSql = "select userid,userName,password,name,gender,address,birthday from Sys_User where userid=" + id;

            DataTable dt = SqlHelper.Query(strSql).Tables[0];

            return dt;
        }

        public int Update(int id,string password,string name,string gender,string address,string birthday)
        {
            string strSql = "update Sys_User set password='" + password + "',name='" + name + "',gender='"
                + gender + "',address='" + address + "',birthday='" + birthday + "'"
                + " where userid=" + id.ToString();
            return SqlHelper.ExecuteSql(strSql);
        }

        public int Delete(int id)
        {
            string strSql = "delete sys_user where userid=" + id.ToString();
            return SqlHelper.ExecuteSql(strSql);
        }

        public int Add(string userName,string password,string name,string gender,string address,string birthday) 
        {
            string strSql = "insert into Sys_User(username,password,name,gender,address,birthday) values('" + userName + "','" + password + "','" + name + "','"
                      + gender + "','" + address + "','" + birthday + "')";
            return SqlHelper.ExecuteSql(strSql);
        }

        public bool ExistsUserName(string userName)
        {
            string strSql = "select count(*) from Sys_User where username='" + userName+ "'";

            string result = SqlHelper.Query(strSql).Tables[0].Rows[0][0].ToString();

            if (result == "0")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public DataTable GetLoginUser(string username, string password)
        {
            DataTable dt = SqlHelper.Query("exec Sys_GetLoginUser '" + username + "','" + password + "'").Tables[0];
            return dt;
        }

        public DataTable GetUserMenus(string UserId)
        {
            return SqlHelper.Query("exec Sys_GetUserMenus '" + UserId + "'").Tables[0];
        }

        public DataTable GetUserRoles(int UserId)
        {
            return SqlHelper.Query("exec Liger_GetUserRoles "+UserId.ToString()).Tables[0];
        }


        public void SaveUserRoles(int UserId, DataTable dtUserRoles)
        {
            try
            {
                string strSql1 = " delete Sys_UserToRole where UserId="+UserId.ToString();

                string strSql2 = " insert into Sys_UserToRole(UserId,RoleId) ";
                strSql2 = strSql2 + " select top 0 0 as UserId,0 as RoleId ";


                foreach (DataRow dr in dtUserRoles.Rows)
                {
                    strSql2 = strSql2 + " union all ";
                    strSql2 = strSql2 + " select "+ UserId.ToString()+", "+dr["RoleId"].ToString();

                }

                ArrayList alSql = new ArrayList();

                alSql.Add(strSql1);
                alSql.Add(strSql2);

                SqlHelper.ExecuteSqlTran(alSql);

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
