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
    public class SyncPrescriptionService
    {
        private const string responseMsgCode = "PIVS0071";
        private static string err = "";

        #region 同步医嘱信息

        public static string savePrescription(string jsonStr)
        {
            string key = "ccode";
            string val = "无法读取属性";
            int info = 0;
            err = "";
            DataTable dt = new DataTable();


            #region 添加字段名

            dt.Columns.Add("act_order_no");
            dt.Columns.Add("parent_no");
            dt.Columns.Add("ward_code");
            dt.Columns.Add("wardname");
            dt.Columns.Add("bed_no");
            dt.Columns.Add("hospital_no");
            dt.Columns.Add("inpatient_no");
            dt.Columns.Add("patientname");
            dt.Columns.Add("sex");
            dt.Columns.Add("age");
            dt.Columns.Add("ageunit");
            dt.Columns.Add("birthday");
            dt.Columns.Add("avdp");
            dt.Columns.Add("doctor");
            dt.Columns.Add("doctor_name");
            dt.Columns.Add("drawer");
            dt.Columns.Add("drawername");
            dt.Columns.Add("freq_code");
            dt.Columns.Add("charge_code");
            dt.Columns.Add("drugname");
            dt.Columns.Add("specification");
            dt.Columns.Add("dose");
            dt.Columns.Add("dose_unit");
            dt.Columns.Add("quantity");
            dt.Columns.Add("pack_unit");
            dt.Columns.Add("drug_company");
            dt.Columns.Add("start_time");
            dt.Columns.Add("end_time");
            dt.Columns.Add("remark");
            dt.Columns.Add("selfbuy");
            dt.Columns.Add("tpn");
            dt.Columns.Add("state");
            dt.Columns.Add("idupdate");
            dt.Columns.Add("reportdate");
            dt.Columns.Add("last_perform_date_time");

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
                #region 获取传入数据
                dt.Rows.Add(new object[] { 
                    row["act_order_no"].ToString(), 
                    row["parent_no"].ToString(), 
                    row["ward_code"].ToString(), 
                    row["wardname"].ToString(), 
                    row["bed_no"].ToString(), 
                    row["hospital_no"].ToString(), 
                    row["inpatient_no"].ToString(), 
                    row["patientname"].ToString(), 
                    row["sex"].ToString(), 
                    row["age"].ToString(), 
                    row["ageunit"].ToString(), 
                    row["birthday"].ToString(),
                    row["avdp"].ToString(),
                    row["doctor"].ToString(),
                    row["doctor_name"].ToString(),
                    "",//row["drawer"].ToString(), 
                    "",//row["drawername"].ToString(),
                    row["freq_code"].ToString(),
                    row["charge_code"].ToString(),
                    row["drugname"].ToString(),
                    row["specification"].ToString(), 
                    row["dose"].ToString(), 
                    row["dose_unit"].ToString(),
                    row["qantity"].ToString(),
                    row["pack_unit"].ToString(),
                    row["drug_company"].ToString(), 
                    row["start_time"].ToString(), 
                    row["end_time"].ToString(), 
                    row["remark"].ToString(), 
                    row["selfbuy"].ToString(), 
                    row["tpn"].ToString(), 
                    row["state"].ToString(), 
                    row["idupdate"].ToString(),
                    row["reportdate"].ToString()});
                #endregion
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
                            #region 插入SQL
                            sql.Append("insert into [RecipeSync]([RecipeID],[RecipeNo],[GroupNo],[WardCode],[WardName]");
                            sql.Append(",[BedNo],[HospitalNo],[PatientCode],[PatientName],[sex],[age],[ageunit]");
                            sql.Append(",[Birthday],[avdp],[DoctorCode],[DoctorName],[DrawerCode],[DrawerName]");
                            sql.Append(",[FreqCode],[ChargeCode],[DrugName],[specification],[dose],[dose_unit],[quantity]");
                            sql.Append(",[pack_unit],[DrugCompany],[InceptDT],[StartTime],[EndTime],[remark],[SelfBuy],[TPN],[state],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["inpatient_no"].ToString() + "Y" + dt.Rows[i]["parent_no"].ToString() + "Z" + dt.Rows[i]["act_order_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["act_order_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["parent_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ward_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["wardname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["bed_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["hospital_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["inpatient_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["patientname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["age"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ageunit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["birthday"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["avdp"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["doctor"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["doctor_name"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drawer"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drawername"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["freq_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["charge_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["specification"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dose"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dose_unit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pack_unit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drug_company"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["reportdate"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["start_time"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["end_time"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["remark"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["selfbuy"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["tpn"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            sql.AppendLine("");
                            #endregion
                            break;
                        case "update":
                            #region 修改SQL
                            sql.Append("update [RecipeSync] set");
                            sql.Append(" [RecipeSync].[GroupNo]='" + dt.Rows[i]["parent_no"].ToString() + "'");
                            sql.Append(",[RecipeSync].[WardCode]='" + dt.Rows[i]["ward_code"].ToString() + "'");
                            sql.Append(",[RecipeSync].[WardName]='" + dt.Rows[i]["wardname"].ToString() + "'");
                            sql.Append(",[RecipeSync].[BedNo]='" + dt.Rows[i]["bed_no"].ToString() + "'");
                            sql.Append(",[RecipeSync].[HospitalNo]='" + dt.Rows[i]["hospital_no"].ToString() + "'");
                            sql.Append(",[RecipeSync].[PatientCode]='" + dt.Rows[i]["inpatient_no"].ToString() + "'");
                            sql.Append(",[RecipeSync].[PatientName]='" + dt.Rows[i]["patientname"].ToString() + "'");
                            sql.Append(",[RecipeSync].[sex]='" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",[RecipeSync].[age]='" + dt.Rows[i]["age"].ToString() + "'");
                            sql.Append(",[RecipeSync].[ageunit]='" + dt.Rows[i]["ageunit"].ToString() + "'");
                            sql.Append(",[RecipeSync].[Birthday]='" + dt.Rows[i]["birthday"].ToString() + "'");
                            sql.Append(",[RecipeSync].[avdp]='" + dt.Rows[i]["avdp"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DoctorCode]='" + dt.Rows[i]["doctor"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DoctorName]='" + dt.Rows[i]["doctor_name"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DrawerCode]='" + dt.Rows[i]["drawer"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DrawerName]='" + dt.Rows[i]["drawername"].ToString() + "'");
                            sql.Append(",[RecipeSync].[FreqCode]='" + dt.Rows[i]["freq_code"].ToString() + "'");
                            sql.Append(",[RecipeSync].[ChargeCode]='" + dt.Rows[i]["charge_code"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DrugName]='" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",[RecipeSync].[specification]='" + dt.Rows[i]["specification"].ToString() + "'");
                            sql.Append(",[RecipeSync].[dose]='" + dt.Rows[i]["dose"].ToString() + "'");
                            sql.Append(",[RecipeSync].[dose_unit]='" + dt.Rows[i]["dose_unit"].ToString() + "'");
                            sql.Append(",[RecipeSync].[quantity]='" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",[RecipeSync].[pack_unit]='" + dt.Rows[i]["pack_unit"].ToString() + "'");
                            sql.Append(",[RecipeSync].[DrugCompany]='" + dt.Rows[i]["drug_company"].ToString() + "'");
                            sql.Append(",[RecipeSync].[StartTime]='" + dt.Rows[i]["start_time"].ToString() + "'");
                            sql.Append(",[RecipeSync].[EndTime]='" + dt.Rows[i]["end_time"].ToString() + "'");
                            sql.Append(",[RecipeSync].[remark]='" + dt.Rows[i]["remark"].ToString() + "'");
                            sql.Append(",[RecipeSync].[SelfBuy]='" + dt.Rows[i]["selfbuy"].ToString() + "'");
                            sql.Append(",[RecipeSync].[TPN]='" + dt.Rows[i]["tpn"].ToString() + "'");
                            sql.Append(",[RecipeSync].[state]='" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",[RecipeSync].[flag] = 0");
                            sql.Append(" where [RecipeSync].[RecipeID] = '" + dt.Rows[i]["inpatient_no"].ToString() + "Y" + dt.Rows[i]["parent_no"].ToString() + "Z" + dt.Rows[i]["act_order_no"].ToString() + "'");
                            #endregion
                            break;
                        case "delete":
                            sql.Append("delete from [RecipeSync]");
                            sql.Append(" where [RecipeSync].[RecipeID] = '" + dt.Rows[i]["inpatient_no"].ToString() + "Y" + dt.Rows[i]["parent_no"].ToString() + "Z" + dt.Rows[i]["act_order_no"].ToString() + "'");
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
                            #region 插入SQL
                            sql.Append("insert into [RecipeSync]([RecipeID],[RecipeNo],[GroupNo],[WardCode],[WardName]");
                            sql.Append(",[BedNo],[HospitalNo],[PatientCode],[PatientName],[sex],[age],[ageunit]");
                            sql.Append(",[Birthday],[avdp],[DoctorCode],[DoctorName],[DrawerCode],[DrawerName]");
                            sql.Append(",[FreqCode],[ChargeCode],[DrugName],[specification],[dose],[dose_unit],[quantity]");
                            sql.Append(",[pack_unit],[DrugCompany],[InceptDT],[StartTime],[EndTime],[remark],[SelfBuy],[TPN],[state],[flag]) values(");
                            sql.Append("'" + dt.Rows[i]["inpatient_no"].ToString() + "Y" + dt.Rows[i]["parent_no"].ToString() + "Z" + dt.Rows[i]["act_order_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["act_order_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["parent_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ward_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["wardname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["bed_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["hospital_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["inpatient_no"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["patientname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["sex"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["age"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["ageunit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["birthday"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["avdp"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["doctor"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["doctor_name"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drawer"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drawername"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["freq_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["charge_code"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drugname"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["specification"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dose"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["dose_unit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["quantity"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["pack_unit"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["drug_company"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["reportdate"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["start_time"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["end_time"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["remark"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["selfbuy"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["tpn"].ToString() + "'");
                            sql.Append(",'" + dt.Rows[i]["state"].ToString() + "'");
                            sql.Append(",0");
                            sql.Append(")");
                            sql.AppendLine("");
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
                        string updateSql = "update [RecipeSync] set flag = 1 where [RecipeSync].[RecipeNo] = '" + dt.Rows[j]["act_order_no"].ToString() + "'";
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
                writeContent.writeText(key + DateTime.Now.ToString("HHmmss") + msgs[0], msgs[0] + "\r\n" + msgs[1] + "\r\n" + ex.ToString() + "\r\nUUID:" + reqMsgSerial);
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
            writeContent.writeOutLog("Recipe_" + DateTime.Now.ToString("HHmmss"), "HISLog", result);
            return result;
        }
        #endregion
    }
}