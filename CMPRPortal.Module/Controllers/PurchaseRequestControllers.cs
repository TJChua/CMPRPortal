using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects;
using CMPRPortal.Module.BusinessObjects.Maintenance;
using CMPRPortal.Module.BusinessObjects.PR;
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
    public partial class PurchaseRequestControllers : ViewController
    {
        GeneralControllers genCon;
        public PurchaseRequestControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.SubmitPR.Active.SetItemValue("Enabled", false);
            this.CancelPR.Active.SetItemValue("Enabled", false);
            this.DuplicatePR.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "PurchaseRequests_DetailView")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitPR.Active.SetItemValue("Enabled", true);
                    this.CancelPR.Active.SetItemValue("Enabled", true);
                    this.DuplicatePR.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitPR.Active.SetItemValue("Enabled", false);
                    this.CancelPR.Active.SetItemValue("Enabled", false);
                    this.DuplicatePR.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.SubmitPR.Active.SetItemValue("Enabled", false);
                this.CancelPR.Active.SetItemValue("Enabled", false);
                this.DuplicatePR.Active.SetItemValue("Enabled", false);
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

        private void SubmitPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());

            selectedObject.Status = DocStatus.Submitted;
            PurchaseRequestDocTrail ds = ObjectSpace.CreateObject<PurchaseRequestDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseRequestDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            #region Get approval
            List<string> ToEmails = new List<string>();
            string emailbody = "";
            string emailsubject = "";
            string emailaddress = "";
            Guid emailuser;
            DateTime emailtime = DateTime.Now;

            string getapproval = "EXEC sp_GetApproval '" + selectedObject.CreateUser.Oid + "', '" + selectedObject.Oid + "', 'PurchaseRequests'";
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand(getapproval, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetString(1) != "")
                {
                    emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                           reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                           System.Environment.NewLine + System.Environment.NewLine;

                    emailsubject = "Purchase Requests Approval";
                    emailaddress = reader.GetString(1);
                    emailuser = reader.GetGuid(0);

                    ToEmails.Add(emailaddress);
                }
            }
            conn.Close();

            if (ToEmails.Count > 0)
            {
                if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                {
                }
            }

            #endregion

            IObjectSpace pos = Application.CreateObjectSpace();
            PurchaseRequests ptrx = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(pos, ptrx, ViewEditMode.View);
            showMsg("Successful", "Submit Done.", InformationType.Success);
        }

        private void SubmitPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            PurchaseRequestDocTrail ds = ObjectSpace.CreateObject<PurchaseRequestDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseRequestDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseRequests trx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void DuplicatePR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                try
                {
                    PurchaseRequests pr = (PurchaseRequests)View.CurrentObject;
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseRequests newpr = os.CreateObject<PurchaseRequests>();

                    if (pr.Entity != null)
                    {
                        newpr.Entity = newpr.Session.GetObjectByKey<Entity>(pr.Entity.Oid);
                    }
                    if (pr.Department != null)
                    {
                        newpr.Department = newpr.Session.GetObjectByKey<vwDepartment>(pr.Department.DepartmentCode);
                    }
                    newpr.ExpectedDeliveryDate = pr.ExpectedDeliveryDate;
                    newpr.EventDate = pr.EventDate;
                    newpr.BanquetOrderNo = pr.BanquetOrderNo;
                    newpr.Remarks = pr.Remarks;

                    foreach (PurchaseRequestDetails dtl in pr.PurchaseRequestDetails)
                    {
                        PurchaseRequestDetails newprdetails = os.CreateObject<PurchaseRequestDetails>();

                        newprdetails.ItemCode = newprdetails.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode.ItemCode);
                        newprdetails.ItemDesc = dtl.ItemDesc;
                        newprdetails.UOM = dtl.UOM;
                        newprdetails.Quantity = dtl.Quantity;
                        newprdetails.UnitPrice = dtl.UnitPrice;
                        if (dtl.Tax != null)
                        {
                            newprdetails.Tax = newprdetails.Session.GetObjectByKey<vwTax>(dtl.Tax.BoCode);
                        }
                        newprdetails.TaxAmount = dtl.TaxAmount;
                        if (dtl.Entity != null)
                        {
                            newprdetails.Entity = newprdetails.Session.GetObjectByKey<Entity>(dtl.Entity.Oid);
                        }
                        newpr.PurchaseRequestDetails.Add(newprdetails);
                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newpr);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    showMsg("Success", "Duplicate Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    showMsg("Fail", "Duplicate Fail.", InformationType.Error);
                }
            }
            else if (e.SelectedObjects.Count > 1)
            {
                showMsg("Fail", "Duplicate Fail, Selected item more than 1.", InformationType.Error);
            }
            else
            {
                showMsg("Fail", "Duplicate Fail, No selected item.", InformationType.Error);
            }
        }
    }
}
