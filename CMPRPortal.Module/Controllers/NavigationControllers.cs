using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects;
using CMPRPortal.Module.BusinessObjects.Maintenance;
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
    public partial class NavigationControllers : WindowController
    {
        private ShowNavigationItemController showNavigationItemController;
        public NavigationControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            // Perform various tasks depending on the target View.
            showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            showNavigationItemController.CustomShowNavigationItem += showNavigationItemController_CustomShowNavigationItem;
        }
        //protected override void OnViewControlsCreated()
        //{
        //    base.OnViewControlsCreated();
        //    // Access and customize the target View control.
        //}
        protected override void OnDeactivated()
        {
            showNavigationItemController.CustomShowNavigationItem -= showNavigationItemController_CustomShowNavigationItem;
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "PRInquiry_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                PRInquiry newprinquiry = objectSpace.CreateObject<PRInquiry>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "PRInquiry_DetailView", true, newprinquiry);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                if (user.DefaultEntity != null)
                {
                    ((PRInquiry)detailView.CurrentObject).Entity = ((PRInquiry)detailView.CurrentObject).Session.GetObjectByKey<Entity>
                        (user.DefaultEntity.Oid);
                }
                if (user.DefaultDept != null)
                {
                    ((PRInquiry)detailView.CurrentObject).Department = ((PRInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwDepartment>
                        (user.DefaultDept.DepartmentCode);
                }

                ((PRInquiry)detailView.CurrentObject).ExpectedDeliveryDate = DateTime.Today;

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }
        }
    }
}
