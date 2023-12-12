using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects.PO;
using CMPRPortal.Module.BusinessObjects.PR;
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

namespace CMPRPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ListViewControllers : ViewController<ListView>
    {
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        ListViewController listViewController;
        public ListViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests) || View.ObjectTypeInfo.Type == typeof(PurchaseOrders))
            {
                listViewController = Frame.GetController<ListViewController>();
                if (listViewController != null)
                {
                    listViewController.EditAction.Active["123"] = false;
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests) || View.ObjectTypeInfo.Type == typeof(PurchaseOrders))
            {
                if (listViewController != null)
                {
                    listViewController.EditAction.Active.RemoveItem("123");
                    listViewController = null;
                }
            }

            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
