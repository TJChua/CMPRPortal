using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
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
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
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
            this.ApprovalPR.Active.SetItemValue("Enabled", false);
            this.EntityFilter.Active.SetItemValue("Enabled", false);
            this.DepartmentFilter.Active.SetItemValue("Enabled", false);
            this.EscalateUser.Active.SetItemValue("Enabled", false);
            this.EscalatePR.Active.SetItemValue("Enabled", false);

            if (typeof(PurchaseRequests).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
                {
                    if (View.Id == "PurchaseRequests_ListView_Escalate")
                    {
                        SystemUsers curruser = (SystemUsers)SecuritySystem.CurrentUser;
                        PermissionPolicyRole EscalateRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('EscalateRole')"));

                        if (EscalateRole != null)
                        {
                            EntityFilter.Items.Clear();

                            EntityFilter.Items.Add(new ChoiceActionItem(curruser.DefaultEntity.EntityName, curruser.DefaultEntity.EntityName));
                            foreach (Entity entity in View.ObjectSpace.CreateCollection(typeof(Entity), CriteriaOperator.Parse("IsActive = ?", "True")))
                            {
                                if (entity.EntityName != curruser.DefaultEntity.EntityName)
                                {
                                    EntityFilter.Items.Add(new ChoiceActionItem(entity.EntityName, entity.EntityName));
                                }
                            }

                            EntityFilter.SelectedIndex = 0;
                            DepartmentFilter.Items.Clear();

                            DepartmentFilter.Items.Add(new ChoiceActionItem(curruser.DefaultDept.DepartmentCode, curruser.DefaultDept.DepartmentCode));
                            foreach (vwDepartment department in View.ObjectSpace.CreateCollection(typeof(vwDepartment), null))
                            {
                                if (curruser.DefaultDept.DepartmentCode != department.DepartmentCode)
                                {
                                    Entity currentity = ObjectSpace.FindObject<Entity>(CriteriaOperator.Parse("EntityName = ?", EntityFilter.SelectedItem.Id));

                                    if (currentity != null)
                                    {
                                        if (department.EntityCompany == currentity.CompanyName)
                                        {
                                            DepartmentFilter.Items.Add(new ChoiceActionItem(department.DepartmentCode, department.DepartmentCode));
                                        }
                                    }
                                }
                            }

                            DepartmentFilter.SelectedIndex = 0;

                            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                            //    "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                            //   EntityFilter.SelectedItem.Id, DepartmentFilter.SelectedItem.Id, 2);
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[AppStatus] = ?", 2);

                            this.EntityFilter.Active.SetItemValue("Enabled", true);
                            EntityFilter.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                            EntityFilter.CustomizeControl += action_CustomizeControl;
                            this.DepartmentFilter.Active.SetItemValue("Enabled", true);
                            DepartmentFilter.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                            DepartmentFilter.CustomizeControl += action_CustomizeControl;

                            EscalateUser.Items.Clear();

                            foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                            {
                                bool entity = false;
                                bool department = false;
                                foreach (SystemUsersEntity dtl in user.SystemUsersEntity)
                                {
                                    if (EntityFilter.SelectedItem.Id == dtl.Entity.EntityName)
                                    {
                                        entity = true;
                                        break;
                                    }
                                }
                                foreach (SystemUsersDepartment dtl2 in user.SystemUsersDepartment)
                                {
                                    if (DepartmentFilter.SelectedItem.Id == dtl2.Department.DepartmentCode)
                                    {
                                        department = true;
                                        break;
                                    }
                                }

                                if (entity == true && department == true)
                                {
                                    EscalateUser.Items.Add(new ChoiceActionItem(user.StaffName, user.StaffName));
                                }
                            }

                            this.EscalateUser.Active.SetItemValue("Enabled", true);
                            //EscalateUser.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                            EscalateUser.CustomizeControl += action_CustomizeControl;
                            this.EscalatePR.Active.SetItemValue("Enabled", true);
                        }
                    }

                    if (View.Id == "PurchaseRequests_ListView_PendApp")
                    {
                        SystemUsers curruser = (SystemUsers)SecuritySystem.CurrentUser;
                        PermissionPolicyRole EscalateRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('EscalateRole')"));

                        if (EscalateRole != null)
                        {
                            EntityFilter.Items.Clear();

                            EntityFilter.Items.Add(new ChoiceActionItem(curruser.DefaultEntity.EntityName, curruser.DefaultEntity.EntityName));
                            foreach (Entity entity in View.ObjectSpace.CreateCollection(typeof(Entity), CriteriaOperator.Parse("IsActive = ?", "True")))
                            {
                                if (entity.EntityName != curruser.DefaultEntity.EntityName)
                                {
                                    EntityFilter.Items.Add(new ChoiceActionItem(entity.EntityName, entity.EntityName));
                                }
                            }

                            EntityFilter.SelectedIndex = 0;
                            DepartmentFilter.Items.Clear();

                            DepartmentFilter.Items.Add(new ChoiceActionItem(curruser.DefaultDept.DepartmentCode, curruser.DefaultDept.DepartmentCode));
                            foreach (vwDepartment department in View.ObjectSpace.CreateCollection(typeof(vwDepartment), null))
                            {
                                if (curruser.DefaultDept.DepartmentCode != department.DepartmentCode)
                                {
                                    Entity currentity = ObjectSpace.FindObject<Entity>(CriteriaOperator.Parse("EntityName = ?", EntityFilter.SelectedItem.Id));

                                    if (currentity != null)
                                    {
                                        if (department.EntityCompany == currentity.CompanyName)
                                        {
                                            DepartmentFilter.Items.Add(new ChoiceActionItem(department.DepartmentCode, department.DepartmentCode));
                                        }
                                    }
                                }
                            }

                            DepartmentFilter.SelectedIndex = 0;

                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                                "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                               EntityFilter.SelectedItem.Id, DepartmentFilter.SelectedItem.Id, 2);

                            this.EntityFilter.Active.SetItemValue("Enabled", true);
                            EntityFilter.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                            EntityFilter.CustomizeControl += action_CustomizeControl;
                            this.DepartmentFilter.Active.SetItemValue("Enabled", true);
                            DepartmentFilter.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                            DepartmentFilter.CustomizeControl += action_CustomizeControl;
                        }
                    }
                }
            }
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
            else if (View.Id == "PurchaseRequests_ListView_PendApp")
            {
                this.ApprovalPR.Active.SetItemValue("Enabled", true);
            }
            else if (View.Id == "PurchaseRequests_DetailView_PendApp")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.ApprovalPR.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.ApprovalPR.Active.SetItemValue("Enabled", false);
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

        void action_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            SingleChoiceActionAsModeMenuActionItem actionItem = e.Control as SingleChoiceActionAsModeMenuActionItem;
            if (actionItem != null && actionItem.Action.PaintStyle == DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                control.Label.Text = actionItem.Action.Caption;
                control.Label.Style["padding-right"] = "5px";
                control.ComboBox.Width = Unit.Pixel(100);
            }
            if (actionItem != null && actionItem.Action.PaintStyle != DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                control.ComboBox.Width = Unit.Pixel(100);
            }
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

        private void ApprovalPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count > 1)
            {
                int totaldoc = 0;

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                foreach (PurchaseRequests dtl in e.SelectedObjects)
                {
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseRequests pr = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                    if (pr.NextApprover != null)
                    {
                        pr.WhoApprove = pr.WhoApprove + user.StaffName + ", ";
                        ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

                        if (appstatus == ApprovalStatusType.Not_Applicable)
                            appstatus = ApprovalStatusType.Required_Approval;


                        if (p.IsErr) return;
                        if (appstatus == ApprovalStatusType.Required_Approval && p.AppStatus == ApprovalActions.NA)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Approved && p.AppStatus == ApprovalActions.Yes)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Rejected && p.AppStatus == ApprovalActions.No)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        if (p.AppStatus == ApprovalActions.NA)
                        {
                            appstatus = ApprovalStatusType.Required_Approval;
                        }
                        if (p.AppStatus == ApprovalActions.Yes)
                        {
                            appstatus = ApprovalStatusType.Approved;
                        }
                        if (p.AppStatus == ApprovalActions.No)
                        {
                            appstatus = ApprovalStatusType.Rejected;
                        }

                        PurchaseRequestAppStatus ds = pos.CreateObject<PurchaseRequestAppStatus>();
                        ds.PurchaseRequests = pos.GetObjectByKey<PurchaseRequests>(pr.Oid);
                        ds.AppStatus = appstatus;
                        if (appstatus == ApprovalStatusType.Rejected)
                        {
                            pr.Status = DocStatus.Rejected;
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Reject User: " + user.StaffName + ")" +
                                System.Environment.NewLine + "(Reason: Approval Rejected)";
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("1842A3D5-897E-4213-9A2E-8A8065AD6B6B"));
                        }
                        else
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Approved User: " + user.StaffName + ")";
                        }
                        pr.PurchaseRequestAppStatus.Add(ds);

                        pos.CommitChanges();
                        pos.Refresh();

                        totaldoc++;

                        #region approval

                        List<string> ToEmails = new List<string>();
                        string emailbody = "";
                        string emailsubject = "";
                        string emailaddress = "";
                        Guid emailuser;
                        DateTime emailtime = DateTime.Now;

                        string getapproval = "EXEC sp_Approval '" + user.UserName + "', '" + pr.Oid + "', 'PurchaseRequests', " + ((int)appstatus);
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(getapproval, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                                      reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                                      System.Environment.NewLine + System.Environment.NewLine +
                                      "Regards" + System.Environment.NewLine +
                                      "Central purchasing system";

                            if (appstatus == ApprovalStatusType.Approved)
                                emailsubject = "Purchase Request Approval";
                            else if (appstatus == ApprovalStatusType.Rejected)
                                emailsubject = "Purchase Request Approval Rejected";

                            emailaddress = reader.GetString(1);
                            emailuser = reader.GetGuid(0);

                            ToEmails.Add(emailaddress);
                        }
                        conn.Close();

                        if (ToEmails.Count > 0)
                        {
                            if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                            {
                            }
                        }
                        #endregion
                    }

                    //ObjectSpace.CommitChanges(); //This line persists created object(s).
                    //ObjectSpace.Refresh();

                    showMsg("Info", "Total Document : " + totaldoc + " Approval Done.", InformationType.Info);

                    ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
                }
            }
            else if (e.SelectedObjects.Count == 1)
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                foreach (PurchaseRequests dtl in e.SelectedObjects)
                {
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseRequests pr = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                    if (pr.NextApprover != null)
                    {
                        pr.WhoApprove = pr.WhoApprove + user.StaffName + ", ";
                        ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

                        if (appstatus == ApprovalStatusType.Not_Applicable)
                            appstatus = ApprovalStatusType.Required_Approval;


                        if (p.IsErr) return;
                        if (appstatus == ApprovalStatusType.Required_Approval && p.AppStatus == ApprovalActions.NA)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Approved && p.AppStatus == ApprovalActions.Yes)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Rejected && p.AppStatus == ApprovalActions.No)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        if (p.AppStatus == ApprovalActions.NA)
                        {
                            appstatus = ApprovalStatusType.Required_Approval;
                        }
                        if (p.AppStatus == ApprovalActions.Yes)
                        {
                            appstatus = ApprovalStatusType.Approved;
                        }
                        if (p.AppStatus == ApprovalActions.No)
                        {
                            appstatus = ApprovalStatusType.Rejected;
                        }

                        PurchaseRequestAppStatus ds = pos.CreateObject<PurchaseRequestAppStatus>();
                        ds.PurchaseRequests = pos.GetObjectByKey<PurchaseRequests>(pr.Oid);
                        ds.AppStatus = appstatus;
                        if (appstatus == ApprovalStatusType.Rejected)
                        {
                            pr.Status = DocStatus.Rejected;
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Reject User: " + user.StaffName + ")" +
                                System.Environment.NewLine + "(Reason: Approval Rejected)";
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("1842A3D5-897E-4213-9A2E-8A8065AD6B6B"));
                        }
                        else
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Approved User: " + user.StaffName + ")";
                        }
                        pr.PurchaseRequestAppStatus.Add(ds);

                        pos.CommitChanges();
                        pos.Refresh();

                        #region approval

                        List<string> ToEmails = new List<string>();
                        string emailbody = "";
                        string emailsubject = "";
                        string emailaddress = "";
                        Guid emailuser;
                        DateTime emailtime = DateTime.Now;

                        string getapproval = "EXEC sp_Approval '" + user.UserName + "', '" + pr.Oid + "', 'PurchaseRequests', " + ((int)appstatus);
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(getapproval, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                                      reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                                      System.Environment.NewLine + System.Environment.NewLine +
                                      "Regards" + System.Environment.NewLine +
                                      "Central purchasing system";

                            if (appstatus == ApprovalStatusType.Approved)
                                emailsubject = "Purchase Request Approval";
                            else if (appstatus == ApprovalStatusType.Rejected)
                                emailsubject = "Purchase Request Approval Rejected";

                            emailaddress = reader.GetString(1);
                            emailuser = reader.GetGuid(0);

                            ToEmails.Add(emailaddress);
                        }
                        conn.Close();

                        if (ToEmails.Count > 0)
                        {
                            if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                            {
                            }
                        }
                        #endregion

                        IObjectSpace tos = Application.CreateObjectSpace();
                        PurchaseRequests trx = tos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", pr.Oid));
                        openNewView(tos, trx, ViewEditMode.View);
                        showMsg("Successful", "Approve Done.", InformationType.Success);
                    }
                }
            }
            else
            {
                showMsg("Fail", "No PR selected.", InformationType.Error);
            }
        }

        private void ApprovalPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            PurchaseRequests selectedObject = (PurchaseRequests)View.CurrentObject;

            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            switch (appstatus)
            {
                case ApprovalStatusType.Required_Approval:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    break;
                case ApprovalStatusType.Approved:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    break;
                case ApprovalStatusType.Rejected:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    break;
            }
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press Choose From Approval Status 'Approve or Reject' and Press OK to CONFIRM the action and SAVE else press Cancel.";

            e.View = dv;
        }

        private void EntityFilter_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (View.Id == "PurchaseRequests_ListView_Escalate")
            {
                //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[AppStatus] = ?", 2);
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                    "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                    EntityFilter.SelectedItem.Id, e.SelectedChoiceActionItem.Id, 2);

                PermissionPolicyRole EscalateRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('EscalateRole')"));

                if (EscalateRole != null)
                {
                    EscalateUser.Items.Clear();

                    foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                    {
                        bool entity = false;
                        bool department = false;
                        foreach (SystemUsersEntity dtl in user.SystemUsersEntity)
                        {
                            if (e.SelectedChoiceActionItem.Id == dtl.Entity.EntityName)
                            {
                                entity = true;
                                break;
                            }
                        }
                        foreach (SystemUsersDepartment dtl2 in user.SystemUsersDepartment)
                        {
                            if (DepartmentFilter.SelectedItem.Id == dtl2.Department.DepartmentCode)
                            {
                                department = true;
                                break;
                            }
                        }

                        if (entity == true && department == true)
                        {
                            EscalateUser.Items.Add(new ChoiceActionItem(user.StaffName, user.StaffName));
                        }
                    }
                }
            }
            else
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                    "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                    e.SelectedChoiceActionItem.Id, DepartmentFilter.SelectedItem.Id, 2);
            }
        }

        private void DepartmentFilter_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (View.Id == "PurchaseRequests_ListView_Escalate")
            {
                //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[AppStatus] = ?", 2);
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                    "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                    EntityFilter.SelectedItem.Id, e.SelectedChoiceActionItem.Id, 2);

                PermissionPolicyRole EscalateRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('EscalateRole')"));

                if (EscalateRole != null)
                {
                    EscalateUser.Items.Clear();

                    foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                    {
                        bool entity = false;
                        bool department = false;
                        foreach (SystemUsersEntity dtl in user.SystemUsersEntity)
                        {
                            if (EntityFilter.SelectedItem.Id == dtl.Entity.EntityName)
                            {
                                entity = true;
                                break;
                            }
                        }
                        foreach (SystemUsersDepartment dtl2 in user.SystemUsersDepartment)
                        {
                            if (e.SelectedChoiceActionItem.Id == dtl2.Department.DepartmentCode)
                            {
                                department = true;
                                break;
                            }
                        }

                        if (entity == true && department == true)
                        {
                            EscalateUser.Items.Add(new ChoiceActionItem(user.StaffName, user.StaffName));
                        }
                    }
                }
            }
            else
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Entity.EntityName] = ? " +
                    "and [Department.DepartmentCode] = ? and [AppStatus] = ?",
                    EntityFilter.SelectedItem.Id, e.SelectedChoiceActionItem.Id, 2);
            }
        }

        private void EscalateUser_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {

        }

        private void EscalatePR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                if (EscalateUser.SelectedItem != null)
                {
                    foreach (PurchaseRequests dtl in e.SelectedObjects)
                    {
                        IObjectSpace pos = Application.CreateObjectSpace();
                        PurchaseRequests CurrObject = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                        if (EscalateUser.SelectedItem.Id != null && CurrObject.AppStatus == ApprovalStatusType.Required_Approval)
                        {
                            PurchaseRequestDocTrail ds = pos.CreateObject<PurchaseRequestDocTrail>();
                            ds.DocStatus = CurrObject.Status;
                            ds.DocRemarks = "(System Escalated Info " + "- " + user.StaffName + " )";
                            CurrObject.PurchaseRequestDocTrail.Add(ds);

                            SystemUsers escalate = pos.FindObject<SystemUsers>(CriteriaOperator.Parse("StaffName = ?", EscalateUser.SelectedItem.Id));
                            CurrObject.NextApprover = escalate.StaffName + ", ";

                            pos.CommitChanges();
                            pos.Refresh();

                            genCon.showMsg("Successful", "Escalate Done.", InformationType.Success);

                            IObjectSpace eos = Application.CreateObjectSpace();
                            PurchaseRequests po = eos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));
                        }
                    }
                }
                else
                {
                    genCon.showMsg("Fail", "No user selected.", InformationType.Error);
                }

                ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }
    }
}
