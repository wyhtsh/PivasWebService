using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;

namespace PivasWebService
{
    /// <summary>
    /// pivasSync 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://192.168.100.233/pivasws/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class pivasSync : System.Web.Services.WebService
    {
        public string msg { get; set; }

        //public string[] responseMsgCode = new string[] { "PIVS0011", "PIVS0021", "PIVS0031", "PIVS0041", "PIVS0051", "PIVS0061", "PIVS0071", "PIVS0081", "PIVS0091", "PIVS0111" };

        [WebMethod(Description = "同步药品目录")]
        public string syncDDrug(string json)
        {
            msg = SyncDrugService.saveDrug(json);
            return msg;
        }

        [WebMethod(Description = "同步病区")]
        public string syncDWard(string json)
        {
            msg = SyncWardService.saveWard(json);
            return msg;
        }

        [WebMethod(Description = "同步员工信息")]
        public string syncDEmployee(string json)
        {
            msg = SyncEmployeeService.saveEmployee(json);
            return msg;
        }

        [WebMethod(Description = "同步剂量单位")]
        public string syncDMetric(string json)
        {
            msg = SyncMetricService.saveDMetric(json);
            return msg;
        }

        [WebMethod(Description = "同步频次")]
        public string syncDFreq(string json)
        {
            msg = SyncFreqService.saveFreq(json);
            return msg;
        }

        [WebMethod(Description = "同步病人")]
        public string syncPatient(string json)
        {
            msg = SyncPatientService.savePatient(json);
            return msg;
        }

        [WebMethod(Description = "同步医嘱信息")]
        public string syncPrescription(string json)
        {
            msg = SyncPrescriptionService.savePrescription(json);
            return msg;
        }

        [WebMethod(Description = "同步药单信息")]
        public string syncUseDrugList(string json)
        {
            msg = SyncDrugListService.saveUseDrugList(json);
            return msg;
        }

        [WebMethod(Description = "同步静配中心药品库存")]
        public string syncQuantity(string json)
        {
            msg = SyncQuantityService.saveQuantity(json);
            return msg;
        }

        [WebMethod(Description = "同步病人退药单")]
        public string syncBDRecord(string json)
        {
            msg = SyncBDRecordService.saveBDRecord(json);
            return msg;
        }

    }
}
