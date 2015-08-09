using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PivasWebService
{
    /// <summary>
    /// {0}errorcode;{1}errMsg;
    /// </summary>
    public class PrintError
    {
        /// <summary>
        /// 错误报告
        /// </summary>
        /// <param name="Ex"></param>
        /// <returns></returns>
        public static List<string> printErrMsg(string Ex)
        {
            int info = 0;
            List<string> msgs = new List<string>();
            string errcode ="EPVS0000";
            string errMsg ="操作成功";
            try
            {

                info = int.Parse(Ex);
            }
            catch
            {
                info = 2;
            }
            if (info != 0)
            {
                switch (info)
                {
                    case 1:
                        errcode = "EPVS0001";
                        errMsg = "JSON字符串格式错误";
                        break;
                    case 2:
                        errcode = "EPVS0002";
                        errMsg = "数据操作异常";
                        break;
                    case 3:
                        errcode = "EPVS0003";
                        errMsg = "idupdate 不是规范值";
                        break;
                    case 4:
                        errcode = "EPVS0004";
                        errMsg = "数据插入失败";
                        break;
                    case 5:
                        errcode = "EPVS0005";
                        errMsg = "数据修改失败";
                        break;
                    case 6:
                        errcode = "EPVS0006";
                        errMsg = "数据删除失败，重复删除或表中无此数据";
                        break;
                    case 7:
                        errcode = "EPVS0007";
                        errMsg = "其他异常信息";
                        break;
                }
            }
            msgs.Add(errcode);
            msgs.Add(errMsg);
            if (2 == info)
            {
                msgs.Add(Ex);
            }
            else
            {
                msgs.Add(errMsg);
            }
            return msgs;
        }
    }
}