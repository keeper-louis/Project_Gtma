using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Orm.DataEntity;

namespace Keeper_Louis.K3.Bussiness.PlugIn
{
    [Description("盘点录入界面数据录入")]
    public class CheckNotes:AbstractDynamicFormPlugIn
    {
        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            if (e.Key.ToUpperInvariant().Equals("F_LHR_CONFIRM"))
            {
                CheckNotesReturnInfo returnInfo = new CheckNotesReturnInfo();
                DynamicObject materialObject = this.Model.GetValue("F_LHR_MATERIAL") as DynamicObject;
                returnInfo.Qty = Convert.ToDecimal(this.Model.GetValue("F_LHR_Qty"));
                returnInfo.Materialid = Convert.ToInt64(materialObject["Id"]);
                this.View.ReturnToParentWindow(new FormResult(returnInfo));
                this.View.Close();
            }
        }
        
    }
}
