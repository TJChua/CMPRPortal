using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects;
using CMPRPortal.Module.BusinessObjects.PO;
using CMPRPortal.Module.BusinessObjects.PR;
using CMPRPortal.Module.Controllers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace CMPRPortal.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class WebModificationControllers : WebModificationsController
    {
        GeneralControllers genCon;

        public WebModificationControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            Frame.GetController<ModificationsController>().SaveAndNewAction.Active.SetItemValue("Enabled", false);
            Frame.GetController<ModificationsController>().SaveAndCloseAction.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        protected override void Save(SimpleActionExecuteEventArgs args)
        {
            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                PurchaseRequests CurrObject = (PurchaseRequests)args.CurrentObject;

                base.Save(args);
                if (CurrObject.DocNum == null)
                {
                    CurrObject.DocNum = genCon.GenerateDocNum(DocTypeList.PR, ObjectSpace, CurrObject.Entity);
                }

                base.Save(args);
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrders))
            {
                PurchaseOrders CurrObject = (PurchaseOrders)args.CurrentObject;

                base.Save(args);
                if (CurrObject.DocNum == null)
                {
                    CurrObject.DocNum = genCon.GenerateDocNum(DocTypeList.PO, ObjectSpace, CurrObject.Entity);

                    foreach (PurchaseOrderDetails dtl in CurrObject.PurchaseOrderDetails)
                    {
                        if (dtl.BaseDoc != 0)
                        {
                            genCon.UpdPR(dtl.BaseDoc, dtl.BaseID, "Create", ObjectSpace, dtl.Quantity);
                        }
                    }
                }

                base.Save(args);
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else
            {
                base.Save(args);
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
        }
    }
}
