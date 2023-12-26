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
using CMPRPortal.Module.BusinessObjects.Maintenance;
using CMPRPortal.Module.BusinessObjects.View;

namespace CMPRPortal.Module.BusinessObjects.Setup
{
    [DefaultClassOptions]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("Approval Users")]

    public class ApprovalUsers : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ApprovalUsers(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Entity _Entity;
        [ImmediatePostData]
        [DataSourceCriteria("IsActive = 'True'")]
        [XafDisplayName("Entity")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public Entity Entity
        {
            get { return _Entity; }
            set
            {
                SetPropertyValue("Entity", ref _Entity, value);
            }
        }

        private vwDepartment _Department;
        [ImmediatePostData]
        [NoForeignKey]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [XafDisplayName("Department")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(3), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwDepartment Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private SystemUsers _User;
        [XafDisplayName("User")]
        //[DataSourceCriteria("IsActive = 'True' and DefaultEntity.CompanyName = '@this.Entity.CompanyName' and DefaultDept.DepartmentCode = '@this.Department.DepartmentCode'")]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        public SystemUsers User

        {
            get { return _User; }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private Approvals _Approvals;
        [Association("Approvals-ApprovalUsers")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("Approvals", Enabled = false)]
        public Approvals Approvals
        {
            get { return _Approvals; }
            set { SetPropertyValue("Approvals", ref _Approvals, value); }
        }
    }
}