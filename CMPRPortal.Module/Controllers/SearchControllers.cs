using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;

namespace CMPRPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SearchControllers : ViewController
    {
        GeneralControllers genCon;
        public SearchControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.SearchPR.Active.SetItemValue("Enabled", false);
            this.CreatePO.Active.SetItemValue("Enabled", false);
            this.AddToPO.Active.SetItemValue("Enabled", false);

            if (typeof(PRInquiryDetails).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PRInquiryDetails))
                {
                    if (View.Id == "PRInquiry_PRInquiryDetails_ListView")
                    {
                        this.CreatePO.Active.SetItemValue("Enabled", true);
                    }
                    else
                    {
                        this.CreatePO.Active.SetItemValue("Enabled", false);
                    }

                    if (View.Id == "PRInquiry_PRInquiryDetails_ListView_PO")
                    {
                        this.AddToPO.Active.SetItemValue("Enabled", true);
                    }
                    else
                    {
                        this.AddToPO.Active.SetItemValue("Enabled", false);
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "PRInquiry_DetailView")
            {
                this.SearchPR.Active.SetItemValue("Enabled", true);
            }
            else if (View.Id == "PRInquiry_DetailView_PO")
            {
                this.SearchPR.Active.SetItemValue("Enabled", true);
            }
            else
            {
                this.SearchPR.Active.SetItemValue("Enabled", false);
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

        private void SearchPR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PRInquiry selectedObject = (PRInquiry)e.CurrentObject;

            if (selectedObject.Entity != null && selectedObject.Department != null)
            {
                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetPR", new OperandValue(selectedObject.Entity.Oid),
                    new OperandValue(selectedObject.Department.DepartmentCode),
                    new OperandValue(selectedObject.ExpectedDeliveryDate.Date), new OperandValue(selectedObject.Oid));

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                View.Refresh();
            }
            else
            {
                showMsg("Fail", "Entity/Department is empty.", InformationType.Error);
            }
        }

        private void CreatePO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            int oid = 0;
            int cnt = 0;

            IObjectSpace cos = Application.CreateObjectSpace();
            PurchaseOrders newPO = cos.CreateObject<PurchaseOrders>();
            IObjectSpace os = Application.CreateObjectSpace();

            foreach (PRInquiryDetails dtl in e.SelectedObjects)
            {
                cnt++;
                oid = dtl.PRInquiry.Oid;

                PurchaseOrderDetails newPOdetail = cos.CreateObject<PurchaseOrderDetails>();
                newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                newPOdetail.ItemDesc = dtl.ItemDesc;
                newPOdetail.UOM = dtl.UOM;
                newPOdetail.Quantity = dtl.Quantity;
                newPOdetail.UnitPrice = dtl.UnitPrice;
                newPOdetail.BaseDoc = dtl.BaseDoc;
                newPOdetail.BaseID = dtl.BaseID;
                newPO.PurchaseOrderDetails.Add(newPOdetail);
            }

            if (cnt == 0)
            {
                showMsg("Fail", "No PR selected.", InformationType.Error);
                return;
            }

            string deleterecord = "DELETE FROM PRInquiryDetails WHERE PRInquiry = " + oid;
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand(deleterecord, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            conn.Close();

            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(cos, newPO);
            dv.ViewEditMode = ViewEditMode.Edit;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        }

        private void AddToPO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            int oid = 0;
            int cnt = 0;
            int insertcnt = 0;

            IObjectSpace cos = Application.CreateObjectSpace();
            PurchaseOrders newPO = cos.CreateObject<PurchaseOrders>();
            IObjectSpace os = Application.CreateObjectSpace();

            foreach (PRInquiryDetails dtl in e.SelectedObjects)
            {
                cnt++;
                if (dtl.PRInquiry.DocNum != null)
                {
                    PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", dtl.PRInquiry.DocNum));

                    if (trx != null)
                    {
                        oid = dtl.PRInquiry.Oid;

                        PurchaseOrderDetails newPOdetail = os.CreateObject<PurchaseOrderDetails>();
                        newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                        newPOdetail.ItemDesc = dtl.ItemDesc;
                        newPOdetail.UOM = dtl.UOM;
                        newPOdetail.Quantity = dtl.Quantity;
                        newPOdetail.UnitPrice = dtl.UnitPrice;
                        newPOdetail.BaseDoc = dtl.BaseDoc;
                        newPOdetail.BaseID = dtl.BaseID;
                        trx.PurchaseOrderDetails.Add(newPOdetail);

                        insertcnt++;
                    }
                }
            }

            if (cnt == 0)
            {
                showMsg("Fail", "No PR selected.", InformationType.Error);
                return;
            }

            if (insertcnt > 0)
            {
                os.CommitChanges();

                foreach (PRInquiryDetails dtl in e.SelectedObjects)
                {
                    if (dtl.BaseDoc != 0)
                    {
                        genCon.UpdPR(dtl.BaseDoc, dtl.BaseID, "Create", ObjectSpace, dtl.Quantity);
                    }
                }

                showMsg("Success", "Added to Purchase Order.", InformationType.Success);
            }

            string deleterecord = "DELETE FROM PRInquiryDetails WHERE PRInquiry = " + oid;
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand(deleterecord, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            conn.Close();

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }
    }
}
