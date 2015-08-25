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
    public class Sys_RoleDAL
    {

        public DataTable Get(int id)
        {
            string strSql = "select roleId,roleName,description,Convert(varchar(20),creationDate,120) as creationDate,Convert(varchar(20),modifiedDate,120) as modifiedDate from Sys_Role where roleId=" + id.ToString();

            return SqlHelper.Query(strSql).Tables[0];
        }

        public int Update(int id,string description)
        {
            string strSql = "update Sys_Role set description='" + description
               + "' where roleId=" + id.ToString();
            return SqlHelper.ExecuteSql(strSql);
        }

        public int Delete(int id)
        {
            string strSql = "exec Sys_DeleteRole " + id.ToString();
            return SqlHelper.ExecuteSql(strSql);
        }

        public int Add(string roleName, string description)
        {
            string strSql = "insert into Sys_Role(rolename,description) values('" + roleName+ "','" + description + "')";
            return SqlHelper.ExecuteSql(strSql);
        }

        public DataTable GetPage(int page, int pageSize, ref int totalRowCount, string conditions)
        {
            SqlParameter[] sps = new SqlParameter[9];
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i] = new SqlParameter();
            }

            sps[0].ParameterName = "@strTable";
            sps[0].SqlDbType = SqlDbType.VarChar;
            sps[0].Value = "Sys_Role";

            sps[1].ParameterName = "@strColumn";
            sps[1].SqlDbType = SqlDbType.VarChar;
            sps[1].Value = "roleId";

            sps[2].ParameterName = "@intColType";
            sps[2].SqlDbType = SqlDbType.Int;
            sps[2].Value = 0;

            sps[3].ParameterName = "@intOrder";
            sps[3].SqlDbType = SqlDbType.Bit;
            sps[3].Value = 0;

            sps[4].ParameterName = "@strColumnlist";
            sps[4].SqlDbType = SqlDbType.VarChar;
            sps[4].Value = "roleId,roleName,description,Convert(varchar(20),creationDate,120) as creationDate,Convert(varchar(20),modifiedDate,120) as modifiedDate";

            sps[5].ParameterName = "@intPageSize";
            sps[5].SqlDbType = SqlDbType.Int;
            sps[5].Value = pageSize;



            sps[6].ParameterName = "@intPageNum";
            sps[6].SqlDbType = SqlDbType.Int;
            sps[6].Value = page;



            sps[7].ParameterName = "@strWhere";
            sps[7].SqlDbType = SqlDbType.VarChar;
            sps[7].Value = "roleName like '%" + conditions + "%'";


            sps[8].ParameterName = "@totalRowCount";
            sps[8].SqlDbType = SqlDbType.Int;
            sps[8].Direction = ParameterDirection.Output;

            DataTable dtResult;

            dtResult = SqlHelper.RunProcedure("sp_page", sps, "Sys_Role").Tables[0];


            totalRowCount = Convert.ToInt32(sps[8].Value);


            return dtResult;
        }

        public bool ExistsRoleName(string roleName)
        {
            string strSql = "select count(*) from Sys_Role where rolename='" + roleName + "'";

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

        public bool SaveRolePermission(int roleId, DataTable dtMenus, DataTable dtButtons)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();

                string strInsertMenu = "insert into Tmp_RoleMenus(Id,MenuId) select top 0 '' as Id,0 as MenuId ";
                foreach (DataRow dr in dtMenus.Rows)
                {
                    strInsertMenu += " union all select '" + guid + "' as Id," + dr["menuId"] + " as MenuId ";
                }


                string strInsertButton = "insert into Tmp_RoleButtons(Id,MenuId,ButtonId,IsChecked) select top 0 '' as Id,0 as MenuId,0 as ButtonId,'' as isChecked ";

                foreach (DataRow dr in dtButtons.Rows)
                {
                    strInsertButton += " union all select '" + guid + "' as Id, " + dr["menuId"] + "as MenuId, " + dr["buttonId"] + "as ButtonId, '" + dr["ischecked"] + "' as IsChecked ";
                }

                string strSp = "Sys_SetRoleToPermission @roleId=" + roleId + ",@guid='" + guid + "'";

                ArrayList alSql = new ArrayList();
                alSql.Add(strInsertMenu);
                alSql.Add(strInsertButton);
                alSql.Add(strSp);

                SqlHelper.ExecuteSqlTran(alSql);

                return true;
            }
            catch (Exception e)
            {
                return false;
                throw (e);
            }

        }


        public DataTable GetAllRoles()
        {
            string strSql = "select * from Sys_Role";

            return SqlHelper.Query(strSql).Tables[0];
        }

    }
}
