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
    public class SyncDrugService
    {
        private const string responseMsgCode = "PIVS0011";
        private static string err = "";

        #region 同步药品目录
        public static string saveDrug(string jsonStr)
        {
            string key = "ccode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();

            //添加表字段
            dt.Columns.Add("ccode");
            dt.Columns.Add("cname");
            dt.Columns.Add("spec");
            dt.Columns.Add("dos");
            dt.Columns.Add("dosu");
            dt.Columns.Add("spell");
            dt.Columns.Add("cap");
            dt.Columns.Add("capu");
            dt.Columns.Add("pishi");
            dt.Columns.Add("portno");
            dt.Columns.Add("idupdate");

            string reqMsgSerial = "";//发送方报文流水号
            List<string> msgs = PrintError.printErrMsg(info.ToString());
            List<Dictionary<string, object>> rows = new List<Dictionary<string,object>>();

            try
            {
                rows = MakeJson.JsonToDataTable(jsonStr, out reqMsgSerial);
                for (int m = 0; m < rows.Count; m++)
                {
                    dt.Rows.Add(new object[] 
                    { rows[m]["ccode"].ToString(), 
                        rows[m]["cname"].ToString(),
                        rows[m]["spec"].ToString(),
                        rows[m]["dos"].ToString(),
                        rows[m]["dosu"].ToString(),
                        rows[m]["spell"].ToString(),
                        rows[m]["cap"].ToString(),
                        rows[m]["capu"].ToString(),
                        rows[m]["pishi"].ToString(),
                        rows[m]["portno"].ToString(),
                        rows[m]["idupdate"].ToString() });
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
                            sql.Append("insert into [DDrug]([DrugCode],[DrugName],[Spec],[Dosage],[DosageUnit]");
                            sql.Append(",[SpellCode],[Capacity],[CapacityUnit],[PiShi],[PortNo],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["ccode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["cname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["spec"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dos"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dosu"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["spell"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["cap"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["capu"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pishi"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["portno"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            break;
                        case "update":
                            sql.Append("update [DDrug] set");
                            sql.Append(" [DrugName] = '" + dt.Rows[i]["cname"].ToString() + "'");
                            sql.Append(",[Spec] = '" + dt.Rows[i]["spec"].ToString() + "'");
                            sql.Append(",[Dosage] = '" + dt.Rows[i]["dos"].ToString() + "'");
                            sql.Append(",[DosageUnit] = '" + dt.Rows[i]["dosu"].ToString() + "'");
                            sql.Append(",[SpellCode] = '" + dt.Rows[i]["spell"].ToString() + "'");
                            sql.Append(",[Capacity] = '" + dt.Rows[i]["cap"].ToString() + "'");
                            sql.Append(",[CapacityUnit] = '" + dt.Rows[i]["capu"].ToString() + "'");
                            sql.Append(",[PiShi] = '" + dt.Rows[i]["pishi"].ToString() + "'");
                            sql.Append(",[PortNo] = '" + dt.Rows[i]["portno"].ToString() + "'");
                            sql.Append(",[flag] = 0");
                            sql.Append(" where [DDrug].[DrugCode] = '" + dt.Rows[i]["ccode"].ToString() + "'");
                            break;
                        case "delete":
                            sql.Append("delete from [DDrug]");
                            sql.Append(" where [DDrug].[DrugCode] = '" + dt.Rows[i]["ccode"].ToString() + "'");
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
                            sql.Append("insert into [DDrug]([DrugCode],[DrugName],[Spec],[Dosage],[DosageUnit]");
                            sql.Append(",[SpellCode],[Capacity],[CapacityUnit],[PiShi],[PortNo],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["ccode"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["cname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["spec"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dos"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dosu"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["spell"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["cap"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["capu"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pishi"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["portno"].ToString() + "'");
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
                        string updateSql = "update [DDrug] set flag = 1 where [DDrug].[DrugCode] = '" + dt.Rows[j]["ccode"].ToString() + "'";
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
                //确认错误类型
                msgs = PrintError.printErrMsg(ex.Message);
                writeContent.writeText(DateTime.Now.ToString("HHmmss") + msgs[0], msgs[0] + "\r\n" + msgs[1] + "\r\n" + ex.ToString() + "\r\nUUID:" + reqMsgSerial);
            }
            string p;
            
            if (0 != rows.Count)
            {
                if(rows[0].ContainsKey(key))
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
            writeContent.writeOutLog("Drug_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}