﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRPortal.Module.BusinessObjects.Maintenance;
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

namespace CMPRPortal.Module.BusinessObjects.Setup
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ApprovalDetailView : ObjectViewController<ListView, ApprovalUsers>
    {
        public ApprovalDetailView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                controller.ObjectCreated += Controller_ObjectCreated;
            }
        }

        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            if (e.CreatedObject is ApprovalUsers && View.IsRoot == false)
            {
                ApprovalUsers currentObject = (ApprovalUsers)e.CreatedObject;

                ListView lv = ((ListView)View);
                if (lv.CollectionSource is PropertyCollectionSource)
                {
                    PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                    if (collectionSource.MasterObject != null)
                    {

                        if (collectionSource.MasterObjectType == typeof(Approvals))
                        {
                            Approvals masterobject = (Approvals)collectionSource.MasterObject;

                            if (masterobject.Entity != null)
                            {
                                currentObject.Entity = currentObject.Session.GetObjectByKey<Entity>(masterobject.Entity.Oid);
                            }
                            if (masterobject.Department != null)
                            {
                                currentObject.Department = currentObject.Session.GetObjectByKey<vwDepartment>(masterobject.Department.DepartmentCode);
                            }
                        }
                    }
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
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute -= NewObjectAction_Execute;
                controller.ObjectCreated -= Controller_ObjectCreated;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
