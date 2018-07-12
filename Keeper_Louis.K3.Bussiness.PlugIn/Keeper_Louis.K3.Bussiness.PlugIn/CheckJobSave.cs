using Kingdee.BOS.Core.DynamicForm.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using System.ComponentModel;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.App.Data;

namespace Keeper_Louis.K3.Bussiness.PlugIn
{
    [Description("盘点作业表创建状态时按照物料进行平均单价赋值")]
    public class CheckJobSave:AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            //平均单价等于当前月份1号-账存日期之间该库存组织下的采购入库单物料的平均单价
            if (e.DataEntitys!=null&&e.DataEntitys.Count<DynamicObject>()>0)
            {
                foreach (DynamicObject item in e.DataEntitys)
                {

                    DateTime dt = Convert.ToDateTime(item["BackUpDate"]);
                    long orgId = Convert.ToInt64(item["StockOrgId_Id"]);
                    DynamicObjectCollection doc = item["StkCountInputEntry"] as DynamicObjectCollection;
                    foreach (DynamicObject dot in doc)
                    {
                        long fentryid = Convert.ToInt64(dot["Id"]);
                        long materialId = Convert.ToInt64(dot["MaterialId_Id"]);
                        string strSql = string.Format(@"/*dialect*/select sum(c.FAMOUNT)/sum(b.FREALQTY) from t_STK_InStock a inner join t_stk_instockentry b on a.FID = b.FID inner join T_STK_INSTOCKENTRY_F c on a.FID = c.FID where a.FSTOCKORGID = {0} and b.FMATERIALID = {1} and a.FDATE >= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0) and a.FDATE <{2}", orgId,materialId, dt.ToShortDateString());
                        double result = DBUtils.ExecuteScalar<Double>(this.Context,strSql,0.0,null);
                        if (result!=0.0)
                        {
                            string updateSql = string.Format(@"/*dialect*/update T_STK_STKCOUNTINPUTENTRY set F_LHR_PRICE = {0} where FENTRYID = {1}",result,fentryid);
                            DBUtils.Execute(this.Context,updateSql);
                        }
                    }
                }
            }
        }
    }
}
