using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using DBHelper;
using FileWriteLib;

namespace PivasWebService
{
    public class SyncEmployeeService
    {
        private const string responseMsgCode = "PIVS0031";
        private static string err = "";

        #region 同步员工信息
        /// <summary>
        /// "ecode" : "编码",
        ///	"eno" : "工号",
        ///	"ename" : "姓名",
        ///	"idupdate" : "insert"
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string saveEmployee(string jsonStr)
        {
            string key = "ecode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            //添加表字段
            dt.Columns.Add("ecode");
            dt.Columns.Add("eno");
            dt.Columns.Add("ename");
            dt.Columns.Add("idupdate");

            string reqMsgSerial = "";//发送方报文流水号
            List<string> msgs = PrintError.printErrMsg(info.ToString());

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            try
            {
                rows = MakeJson.JsonToDataTable(jsonStr, out reqMsgSerial);
                //HIS所加字段判断同步的操作类型{insert|update|delete}
                foreach (var row in rows)
                {
                //Dictionary<string, object> data = (Dictionary<string, object>)row;
                dt.Rows.Add(new object[] { row["ecode"].ToString(), row["eno"].ToString(), row["ename"].ToString(), row["idupdate"].ToString() });
                }

                DBHelp dbhelp = new DBHelp("sqlConnection");
                //统计数据执行条数
                int count = 0;
                int countUpdate = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder sql = new StringBuilder();
                    switch (dt.Rows[i]["idupdate"].ToString().ToLower())
                    {
                        case "insert":
                            sql.Append("insert into [DEmployee]([AccountID],[DEmployeeCode],[DEmployeeName],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["ecode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["eno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ename"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            break;
                        case "update":
                            sql.Append("update [DEmployee] set");
                            sql.Append(" [DEmployee].[DEmployeeCode] = '" + dt.Rows[i]["eno"].ToString() + "'");
                            sql.Append(",[DEmployee].[DEmployeeName] = '" + dt.Rows[i]["ename"].ToString() + "'");
                            sql.Append(",[DEmployee].[flag] = 0");
                            sql.Append(" where [DEmployee].[AccountID] = '" + dt.Rows[i]["ecode"].ToString() + "'");
                            break;
                        case "delete":
                            sql.Append("delete from [DEmployee]");
                            sql.Append(" where [DEmployee].[AccountID] = '" + dt.Rows[i]["ecode"].ToString() + "'");
                            break;
                        default:
                            info = 3;
                            throw new Exception(info.ToString());
                    }
                    int n = dbhelp.addAndUpdate(sql.ToString());
                    if (n == 0)
                    {
                        if (dt.Rows[i]["idupdate"].ToString().ToLower() == "update")
                        {
                            sql.Append("insert into [DEmployee]([AccountID],[DEmployeeCode],[DEmployeeName],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["ecode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["eno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ename"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            n = dbhelp.addAndUpdate(sql.ToString());
                        }
                        else
                        {
                            CommonUtil.isCount(dt.Rows[i]["idupdate"].ToString());
                        }
                    }
                    count += n;
                }
                if (count == dt.Rows.Count)
                {
                    //标识同步数据完成
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (dt.Rows[j]["idupdate"].ToString().ToLower() == "delete")
                        {
                            countUpdate += 1;
                            break;
                        }
                        string updateSql = "update [DEmployee] set flag = 1 where [DEmployee].[AccountID] = '" + dt.Rows[j]["ecode"].ToString() + "'";
                        countUpdate += dbhelp.addAndUpdate(updateSql);
                    }
                }
                else
                {
                    info = 7;
                    throw new Exception(info.ToString());
                }
            }
            catch (Exception ex)
            {
                msgs = PrintError.printErrMsg(ex.Message);
                writeContent.writeText(DateTime.Now.ToString("HHmmss") + msgs[0], msgs[0] + "\r\n" + msgs[1] + "\r\n" + ex.ToString() + "\r\nUUID:" + reqMsgSerial);
            }
            string p;
            
            if (0 != rows.Count)
            {
                if (rows[0].ContainsKey(key))
                {
                    val = rows[0][key].ToString();
                }
            }
            p = MakeHeaders.makePayLoad(key, val, msgs);
            if (0 != info)
            {
                err = msgs[0];
            }
            string result = MakeHeaders.responseHeader(reqMsgSerial, responseMsgCode, err, p);
            writeContent.writeOutLog("Emp_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}