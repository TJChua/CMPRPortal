using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects;
using CMPRPortal.Module.BusinessObjects.Maintenance;
using CMPRPortal.Module.BusinessObjects.PO;
using CMPRPortal.Module.BusinessObjects.Search_Screen;
using CMPRPortal.Module.BusinessObjects.View;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace CMPRPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PurchaseOrderControllers : ViewController
    {
        GeneralControllers genCon;
        public PurchaseOrderControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.PRInquiry.Active.SetItemValue("Enabled", false);
            this.SubmitPO.Active.SetItemValue("Enabled", false);
            this.CancelPO.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "PurchaseOrders_DetailView")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitPO.Active.SetItemValue("Enabled", true);
                    this.CancelPO.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitPO.Active.SetItemValue("Enabled", false);
                    this.CancelPO.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.PRInquiry.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.PRInquiry.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.SubmitPO.Active.SetItemValue("Enabled", false);
                this.CancelPO.Active.SetItemValue("Enabled", false);
                this.PRInquiry.Active.SetItemValue("Enabled", false);
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }
        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        private void PRInquiry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void PRInquiry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PurchaseOrders trx = (PurchaseOrders)View.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseOrders po = os.FindObject<PurchaseOrders>(new BinaryOperator("Oid", trx.Oid));

            IObjectSpace inqos = Application.CreateObjectSpace();
            PRInquiry addnew = inqos.CreateObject<PRInquiry>();

            DetailView dv = Application.CreateDetailView(inqos, "PRInquiry_DetailView_PO", true, addnew);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((PRInquiry)dv.CurrentObject).DocNum = po.DocNum;

            if (po.Entity != null)
            {
                ((PRInquiry)dv.CurrentObject).Entity = ((PRInquiry)dv.CurrentObject).Session.GetObjectByKey<Entity>
                    (po.Entity.Oid);
            }
            if (po.Department != null)
            {
                ((PRInquiry)dv.CurrentObject).Department = ((PRInquiry)dv.CurrentObject).Session.GetObjectByKey<vwDepartment>
                    (po.Department.DepartmentCode);
            }
            
            ((PRInquiry)dv.CurrentObject).ExpectedDeliveryDate = DateTime.Today;

            inqos.CommitChanges();
            inqos.Refresh();

            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.DialogController.AcceptAction.ActionMeaning = ActionMeaning.Unknown;
            e.Maximized = true;

            e.View = dv;
        }

        private void SubmitPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrders selectedObject = (PurchaseOrders)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Submitted;
            PurchaseOrderDocTrail ds = ObjectSpace.CreateObject<PurchaseOrderDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseOrderDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace pos = Application.CreateObjectSpace();
            PurchaseOrders ptrx = pos.FindObject<PurchaseOrders>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(pos, ptrx, ViewEditMode.View);
            showMsg("Successful", "Submit Done.", InformationType.Success);
        }

        private void SubmitPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrders selectedObject = (PurchaseOrders)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            PurchaseOrderDocTrail ds = ObjectSpace.CreateObject<PurchaseOrderDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseOrderDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            foreach (PurchaseOrderDetails dtl in selectedObject.PurchaseOrderDetails)
            {
                if (dtl.BaseDoc != 0)
                {
                    genCon.UpdPR(dtl.BaseDoc, dtl.BaseID, "Cancel", ObjectSpace, dtl.Quantity);
                }
            }

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }
    }
}
