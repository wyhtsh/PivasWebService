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
    public class SyncPatientService
    {
        private const string responseMsgCode = "PIVS0061";
        private static string err = "";

        #region 同步病人信息
        public static string savePatient(string jsonStr)
        {
            string key = "pcode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            //添加表字段
            dt.Columns.Add("pcode");
            dt.Columns.Add("pname");
            dt.Columns.Add("sex");
            dt.Columns.Add("ward");
            dt.Columns.Add("bed");
            dt.Columns.Add("state");
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
                dt.Rows.Add(new object[] { row["pcode"].ToString(), row["pname"].ToString(), row["sex"].ToString(), row["ward"].ToString(), row["bed"].ToString(), row["state"].ToString(), row["idupdate"].ToString() });
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
                            sql.Append("insert into [Patient]([PatCode],[PatName],[Sex],[WardCode],[BedNo],[PatStatus],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["pcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ward"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["bed"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            break;
                        case "update":
                            sql.Append("update [Patient] set");
                            sql.Append(" [Patient].[PatName] = '" + dt.Rows[i]["pname"].ToString() + "'");
                            sql.Append(",[Patient].[Sex] = '" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",[Patient].[WardCode] = '" + dt.Rows[i]["ward"].ToString() + "'");
                            sql.Append(",[Patient].[BedNo] = '" + dt.Rows[i]["bed"].ToString() + "'");
                            sql.Append(",[Patient].[PatStatus] = '" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",[Patient].[flag] = 0");
                            sql.Append(" where [Patient].[PatCode] = '" + dt.Rows[i]["pcode"].ToString() + "'");
                            break;
                        case "delete":
                            sql.Append("delete from [Patient]");
                            sql.Append(" where [Patient].[PatCode] = '" + dt.Rows[i]["pcode"].ToString() + "'");
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
                            sql.Append("insert into [Patient]([PatCode],[PatName],[Sex],[WardCode],[BedNo],[PatStatus],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["pcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ward"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["bed"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            n = dbhelp.addAndUpdate(sql.ToString());
                        }
                        else
                        {
                            CommonUtil.isCount(dt.Rows[i]["idupdate"].ToString().ToLower());
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
                        string updateSql = "update [Patient] set flag = 1 where [Patient].[PatCode] = '" + dt.Rows[j]["pcode"].ToString() + "'";
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
            writeContent.writeOutLog("Patient_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}