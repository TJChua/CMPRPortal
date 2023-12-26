using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using CMPRPortal.Module.BusinessObjects.View;

namespace CMPRPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("Multiple Department")]

    public class SystemUsersDepartment : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SystemUsersDepartment(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private vwDepartment _Department;
        [NoForeignKey]
        [XafDisplayName("Department")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(0)]
        public vwDepartment Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("_Department", ref _Department, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private SystemUsers _SystemUsers;
        [Association("SystemUsers-SystemUsersDepartment")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SystemUsers", Enabled = false)]
        public SystemUsers SystemUsers
        {
            get { return _SystemUsers; }
            set { SetPropertyValue("SystemUsers", ref _SystemUsers, value); }
        }
    }
}