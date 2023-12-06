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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using CMPRPortal.Module.BusinessObjects.View;
using CMPRPortal.Module.BusinessObjects.Maintenance;

namespace CMPRPortal.Module.BusinessObjects {

    [DefaultClassOptions]
    public class SystemUsers : PermissionPolicyUser
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SystemUsers(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _StaffEmail;
        [Size(150)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string StaffEmail
        {
            get { return _StaffEmail; }
            set { SetPropertyValue("StaffEmail", ref _StaffEmail, value); }
        }

        private string _StaffName;
        [RuleRequiredField(DefaultContexts.Save)]
        public string StaffName
        {
            get { return _StaffName; }
            set { SetPropertyValue("StaffName", ref _StaffName, value); }
        }

        private vwDepartment _DefaultDept;
        [NoForeignKey]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [XafDisplayName("Default Department")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwDepartment DefaultDept
        {
            get { return _DefaultDept; }
            set { SetPropertyValue("DefaultDept", ref _DefaultDept, value); }
        }

        private Entity _DefaultEntity;
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("IsActive = 'True'")]
        [XafDisplayName("Default Entity")]
        [RuleRequiredField(DefaultContexts.Save)]
        public Entity DefaultEntity
        {
            get { return _DefaultEntity; }
            set { SetPropertyValue("DefaultEntity", ref _DefaultEntity, value); }
        }
    }
}