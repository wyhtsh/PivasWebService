using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using FileWriteLib;

namespace PivasWebService
{
    public class CommonUtil
    {
        private static int info = 0;
        #region JSON转换
        /// <summary>
        /// JSON转换
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="reqMsgSerial">发送方UUID</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> JsonToDataTable(string Str, out string reqMsgSerial)
        {
            info = 0;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            Dictionary<string, object> rows = new Dictionary<string, object>();
            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            reqMsgSerial = "";
            try
            {

                jsonData = (Dictionary<string, object>)jss.DeserializeObject(Str);

                //header信息
                Dictionary<string, object> headers = (Dictionary<string, object>)jsonData["headers"];

                reqMsgSerial = headers["reqMsgSerial"].ToString();
                //传入同步数据
                Dictionary<string, object> payload = (Dictionary<string, object>)jsonData["payload"];
                rows = (Dictionary<string, object>)payload["request"];
                //若传入脚本中存在item则为数组
                if (rows.ContainsKey("item") == true)
                {
                    var items = (object[])rows["item"];
                    if (items.Length != 0)
                    {
                        foreach (var item in items)
                        {
                            Dictionary<string, object> rowsTmp = rows;
                            rowsTmp.Remove("item");
                            Dictionary<string, object> dic = (Dictionary<string, object>)item;
                            foreach (var key in dic.Keys)
                            {
                                rowsTmp.Add(key, dic[key]);
                            }
                            datas.Add(rowsTmp);
                        }
                    }
                }
                else
                {
                    datas.Add(rows);
                }
            }
            catch
            {
                info = 1;
                throw new Exception(info.ToString());
            }

            return datas;
        }
        #endregion

        /// <summary>
        /// 数据操作错误处理
        /// </summary>
        /// <param name="idupdate">操作方式（insert|update|delete）</param>
        public static void isCount(string idupdate)
        {
            info = 0;
            switch (idupdate)
            {
                case "insert":
                    info = 4;
                    break;
                case "update":
                    info = 5;
                    break;
                case "delete":
                    info = 6;
                    break;
            }
            List<string> msgs = new List<string>();
            msgs = PrintError.printErrMsg(info.ToString());
            writeContent.writeText(DateTime.Now.ToString("HHmmss") + msgs[0], msgs[0] + "\r\n" + msgs[1]);
            throw new Exception(info.ToString());
        }
    }
}