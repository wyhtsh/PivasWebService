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
    public class SyncBDRecordService
    {
        private const string responseMsgCode = "PIVS0111";

        private static string err = "";


        #region 同步病人退药单
        public static string saveBDRecord(string jsonStr)
        {
            string key = "pcode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            #region 添加表字段
            dt.Columns.Add("pcode");
            dt.Columns.Add("wcode");
            dt.Columns.Add("recipeid");
            dt.Columns.Add("groupno");
            dt.Columns.Add("drugcode");
            dt.Columns.Add("quantity");
            dt.Columns.Add("backdt");
            dt.Columns.Add("backer");
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
                dt.Rows.Add(new object[] { row["pcode"].ToString(), row["wcode"].ToString(), row["recipeid"].ToString(), row["groupno"].ToString(), row["drugcode"].ToString(), row["quantity"].ToString(), row["backdt"].ToString(), row["backer"].ToString(), row["idupdate"].ToString() });
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
                            #region 插入
                            sql.Append("insert into [BDRecord]([ID],[pcode],[wcode],[RecipeID],[GroupNo],[DrugCode]");
                            sql.Append(",[Quantity],[Backdt],[Backer],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["pcode"].ToString() + "Y" + dt.Rows[i]["groupno"].ToString() + "Z" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["wcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["groupno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["backdt"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["backer"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            #endregion
                            break;
                        case "update":
                            #region 修改
                            sql.Append("update [BDRecord] set");
                            sql.Append(" [BDRecord].[DrugCode]='" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",[BDRecord].[wcode]='" + dt.Rows[i]["wcode"].ToString() + "'");
                            sql.Append(",[BDRecord].[Quantity]='" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",[BDRecord].[Backdt]='" + dt.Rows[i]["backdt"].ToString() + "'");
                            sql.Append(",[BDRecord].[Backer]='" + dt.Rows[i]["backer"].ToString() + "'");
                            sql.Append(",[BDRecord].[flag] = 0");
                            sql.Append(" where [BDRecord].[ID]='" + dt.Rows[i]["pcode"].ToString() + "Y" + dt.Rows[i]["groupno"].ToString() + "Z" + dt.Rows[i]["recipeid"].ToString() + "'");
                            #endregion
                            break;
                        case "delete":
                            #region 删除
                            sql.Append("delete from [BDRecord]");
                            sql.Append(" where [BDRecord].[ID]='" + dt.Rows[i]["pcode"].ToString() + "Y" + dt.Rows[i]["groupno"].ToString() + "Z" + dt.Rows[i]["recipeid"].ToString() + "'");
                            #endregion
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
                            #region 插入
                            sql.Append("insert into [BDRecord]([ID],[pcode],[wcode],[RecipeID],[GroupNo],[DrugCode]");
                            sql.Append(",[Quantity],[Backdt],[Backer],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["pcode"].ToString() + "Y" + dt.Rows[i]["groupno"].ToString() + "Z" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["wcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["groupno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["backdt"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["backer"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            #endregion
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
                        StringBuilder updateSql = new StringBuilder();
                    updateSql.Append("update [BDRecord] set flag = 1 where [BDRecord].[drugcode] = '" + dt.Rows[j]["drugcode"].ToString() + "'");
                    updateSql.Append(" and [BDRecord].[pcode]='" + dt.Rows[j]["pcode"].ToString() + "'");
                    updateSql.Append(" and [BDRecord].[wcode]='" + dt.Rows[j]["wcode"].ToString() + "'");
                    updateSql.Append(" and [BDRecord].[RecipeID]='" + dt.Rows[j]["recipeid"].ToString() + "'");
                    updateSql.Append(" and [BDRecord].[GroupNo]='" + dt.Rows[j]["groupno"].ToString() + "'");
                    updateSql.Append(" and [BDRecord].[DrugCode]='" + dt.Rows[j]["drugcode"].ToString() + "'");
                        countUpdate += dbhelp.addAndUpdate(updateSql.ToString());
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
                if (rows[0].ContainsKey(key) && rows[0].ContainsKey("wcode") && rows[0].ContainsKey("recipeid"))
                {
                    val = rows[0][key].ToString() + ",\"wcode\":\"" + rows[0]["wcode"].ToString() + "\",\"recipeid\":\"" + rows[0]["recipeid"].ToString();
                }
            }
            p = MakeHeaders.makePayLoad(key, val, msgs);
            if (0 != info)
            {
                err = msgs[0];
            }
            string result = MakeHeaders.responseHeader(reqMsgSerial, responseMsgCode, err, p);
            writeContent.writeOutLog("BDRecord_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}