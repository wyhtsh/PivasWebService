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
    public class SyncQuantityService
    {
        private const string responseMsgCode = "PIVS0091";

        private static string err = "";

        #region 同步药品库存
        public static string saveQuantity(string jsonStr)
        {
            string key = "drugcode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            #region 添加表字段
            dt.Columns.Add("drugcode");
            dt.Columns.Add("quantity");
            dt.Columns.Add("idupdate");
            #endregion

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
                dt.Rows.Add(new object[] { row["drugcode"].ToString(), row["quantity"].ToString(), row["idupdate"].ToString() });
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
                            sql.Append("insert into [Quantity]([drugcode],[quantity],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            break;
                        case "update":
                            sql.Append("update [Quantity] set");
                            sql.Append(" [Quantity].[quantity] = '" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",[Quantity].[flag] = 0");
                            sql.Append(" where [Quantity].[drugcode] = '" + dt.Rows[i]["drugcode"].ToString() + "'");
                            break;
                        case "delete":
                            sql.Append("delete from [Quantity]");
                            sql.Append(" where [Quantity].[drugcode] = '" + dt.Rows[i]["drugcode"].ToString() + "'");
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
                            sql.Append("insert into [Quantity]([drugcode],[quantity],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
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
                        string updateSql = "update [Quantity] set flag = 1 where [Quantity].[drugcode] = '" + dt.Rows[j]["drugcode"].ToString() + "'";
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
            writeContent.writeOutLog("Quantity_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}