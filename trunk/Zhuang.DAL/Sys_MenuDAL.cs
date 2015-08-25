using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Zhuang.Utility.Common.DataBase;

namespace Zhuang.DAL
{
    public class Sys_MenuDAL
    {

        public DataTable GetMenus_System()
        {
            return GetMenus("View_Sys_Menu_System");
        }

        public DataTable GetMenus(string tableName)
        {
            SqlParameter[] sps = new SqlParameter[6];
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i] = new SqlParameter();
            }

            sps[0].ParameterName = "@parentId";
            sps[0].SqlDbType = SqlDbType.Int;
            sps[0].Value = -1;

            sps[1].ParameterName = "@tableName";
            sps[1].SqlDbType = SqlDbType.VarChar;
            sps[1].Value = tableName;

            sps[2].ParameterName = "@idFieldName";
            sps[2].SqlDbType = SqlDbType.VarChar;
            sps[2].Value = "menuId";

            sps[3].ParameterName = "@parentIdFieldName";
            sps[3].SqlDbType = SqlDbType.VarChar;
            sps[3].Value = "parentId";


            string selectFields = @"menuId,parentId,menuUrl as url,menuName as text,case isexpand when 'Y' then 'true' else 'false' end as isexpand";
            sps[4].ParameterName = "@selectFields";
            sps[4].SqlDbType = SqlDbType.VarChar;
            sps[4].Value = selectFields;

            sps[5].ParameterName = "@orderBy";
            sps[5].SqlDbType = SqlDbType.VarChar;
            sps[5].Value = " order by orderid ";



            DataTable dt = SqlHelper.RunProcedure("sp_getTree", sps, tableName).Tables[0];

            return dt;
        }

        public DataTable GetUserMenus(string userId)
        {
            string strSql = "exec Liger_GetUserMenus '" + userId + "'";
            DataTable dt = SqlHelper.Query(strSql).Tables[0];
            return dt;
        }

        public DataTable GetMenuButtons(string roleId, string menuId)
        {
            string strSql = "exec liger_getRoleButtons " + roleId + "," + menuId;
            return SqlHelper.Query(strSql).Tables[0];
        }

        public DataTable GetRoleMenus(int roleId)
        {
            string strSql = "exec liger_getRoleMenus " + roleId.ToString();

            DataTable dt = SqlHelper.Query(strSql).Tables[0];

            return dt;

        }
    }
}
