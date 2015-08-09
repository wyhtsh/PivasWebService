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
    public class SyncDrugListService
    {
        private const string responseMsgCode = "PIVS0081";

        private static string err = "";
        #region 同步药单信息

        public static string saveUseDrugList(string jsonStr)
        {
            string key = "groupno";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            #region 添加表字段
            //添加表字段 
            dt.Columns.Add("groupno");
            dt.Columns.Add("recipeid");
            dt.Columns.Add("schedule");
            dt.Columns.Add("druglistid");
            dt.Columns.Add("drugfreq");
            dt.Columns.Add("drugcode");
            dt.Columns.Add("drugname");
            dt.Columns.Add("quantity");
            dt.Columns.Add("quantityunit");
            dt.Columns.Add("occdt");
            dt.Columns.Add("insertdt");
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
                dt.Rows.Add(new object[] { row["groupno"].ToString(), row["recipeid"].ToString(), row["schedule"].ToString(), row["druglistid"].ToString(), row["drugfreq"].ToString(), row["drugcode"].ToString(), row["drugname"].ToString(), row["quantity"].ToString(), row["quantityunit"].ToString(), row["occdt"].ToString(), row["insertdt"].ToString(), row["idupdate"].ToString() });
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
                            sql.Append("insert into [UseDrugListTemp]([GroupNo],[RecipeID],[Schedule],[DrugListID],[DrugFreq],[DrugCode],[DrugName],[Quantity],[QuantityUnit],[OccDT],[InsertDT],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["groupno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["schedule"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["druglistid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugfreq"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantityunit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["occdt"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["InsertDT"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            #endregion
                            break;
                        case "update":
                            #region 修改
                            sql.Append("update [UseDrugListTemp] set");
                            sql.Append(" [UseDrugListTemp].[GroupNo] = '" + dt.Rows[i]["groupno"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[RecipeID] = '" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[Schedule] = '" + dt.Rows[i]["schedule"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[DrugFreq] = '" + dt.Rows[i]["drugfreq"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[DrugCode] = '" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[DrugName] = '" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[Quantity] = '" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[QuantityUnit] = '" + dt.Rows[i]["quantityunit"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[OccDT] = '" + dt.Rows[i]["occdt"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[InsertDT] = '" + dt.Rows[i]["InsertDT"].ToString() + "'");
                            sql.Append(",[UseDrugListTemp].[flag] = 0");
                            sql.Append(" where [UseDrugListTemp].[DrugListID] = '" + dt.Rows[i]["druglistid"].ToString() + "'");
                            #endregion
                            break;
                        case "delete":
                            sql.Append("delete from [UseDrugListTemp]");
                            sql.Append(" where [UseDrugListTemp].[DrugListID] = '" + dt.Rows[i]["druglistid"].ToString() + "'");
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
                            sql.Append("insert into [UseDrugListTemp]([GroupNo],[RecipeID],[Schedule],[DrugListID],[DrugFreq],[DrugCode],[DrugName],[Quantity],[QuantityUnit],[OccDT],[InsertDT],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["groupno"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["recipeid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["schedule"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["druglistid"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugfreq"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugcode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantityunit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["occdt"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["InsertDT"].ToString() + "'");
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
                        string updateSql = "update [UseDrugListTemp] set flag = 1 where [UseDrugListTemp].[DrugListID] = '" + dt.Rows[j]["druglistid"].ToString() + "'";
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
                if (rows[0].ContainsKey(key) && rows[0].ContainsKey("recipeid") && rows[0].ContainsKey("schedule"))
                {
                    val = rows[0][key].ToString() + "\",\"recipeid\":\"" + rows[0]["recipeid"].ToString() + "\",\"schedule\":\"" + rows[0]["schedule"].ToString();
                }
            }
            p = MakeHeaders.makePayLoad(key, val, msgs);
            if (0 != info)
            {
                err = msgs[0];
            }
            string result = MakeHeaders.responseHeader(reqMsgSerial, responseMsgCode, err, p);
            writeContent.writeOutLog("DrugList_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}