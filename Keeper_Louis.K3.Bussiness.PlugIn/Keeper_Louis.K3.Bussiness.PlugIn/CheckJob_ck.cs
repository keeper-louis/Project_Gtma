using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;

namespace Keeper_Louis.K3.Bussiness.PlugIn
{
    [Description("盘点作业表增加盘点按钮（批量盘点）")]
    public class CheckJob_ck:AbstractDynamicFormPlugIn
    {
        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            DynamicObject o = (DynamicObject)this.Model.DataObject;

            long id = Convert.ToInt64(o["Id"]);
            if (e.Key.ToUpperInvariant().Equals("F_LHR_CHECK"))
            {
                DynamicFormShowParameter showParam = new DynamicFormShowParameter();
                showParam.FormId = "LHR_CHECKDATANOTES";
                this.View.ShowForm(showParam, new Action<FormResult>((FormResult) =>
                {
                    if (FormResult != null&&FormResult.ReturnData != null)
                    {
                        //获取物料所在盘点表所在index
                        
                        long materialId = ((CheckNotesReturnInfo)FormResult.ReturnData).Materialid;
                        string strSql = string.Format(@"/*dialect*/select b.FSEQ from T_STK_STKCOUNTINPUT a inner join T_STK_STKCOUNTINPUTENTRY b on a.FID = b.FID where a.FID = {0} and b.FMATERIALID ={1}",id,materialId);
                        int result = DBUtils.ExecuteScalar<int>(this.Context, strSql, -1, null);
                        if (result!=-1)
                        {
                            this.Model.SetValue("FCountQty", ((CheckNotesReturnInfo)FormResult.ReturnData).Qty, result-1);
                        }
                        
                    }
                }));
            }
        }
        public override void DataChanged(DataChangedEventArgs e)
        {
            base.DataChanged(e);
            if (e.Field.Key.ToUpperInvariant().Equals("FCOUNTQTY"))
            {
                //盈亏金额 = 盘盈-盘亏*平均单价
                this.Model.SetValue("F_LHR_AMOUNT", Convert.ToDecimal(Convert.ToDecimal(Convert.ToDecimal(this.Model.GetValue("FGAINQTY", e.Row)) - Convert.ToDecimal(this.Model.GetValue("FLOSSQTY", e.Row))) * Convert.ToDecimal(this.Model.GetValue("F_LHR_PRICE", e.Row))), e.Row);
            }
            if (e.Field.Key.ToUpperInvariant().Equals("F_LHR_PRICE"))
            {
                //盈亏金额 = 盘盈-盘亏*平均单价
                this.Model.SetValue("F_LHR_AMOUNT", Convert.ToDecimal(Convert.ToDecimal(Convert.ToDecimal(this.Model.GetValue("FGAINQTY", e.Row)) - Convert.ToDecimal(this.Model.GetValue("FLOSSQTY", e.Row))) * Convert.ToDecimal(this.Model.GetValue("F_LHR_PRICE", e.Row))), e.Row);
            }
        }
    }
}
