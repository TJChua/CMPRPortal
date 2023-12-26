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
    [NavigationItem("Setup")]
    [DefaultProperty("BoFullName")]
    [XafDisplayName("Approval")]
    //[Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("hideSaveAndClose", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("hideSaveAndNew", AppearanceItemType = "Action", TargetItems = "SaveAndNew", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    public class Approvals : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Approvals(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;
            ApprovalCnt = 1;
            DocAmount = 0;
            AppType = ApprovalTypes.Document;
        }

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return BoCode + "-" + BoName; }
        }

        private DocTypes _DocType;
        [XafDisplayName("Document Type"), ToolTip("Select Document")]
        [RuleRequiredField(DefaultContexts.Save)]
        [DataSourceCriteria("Entity.CompanyName = '@this.Entity.CompanyName'")]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [Index(5)]
        public DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private ApprovalTypes _AppType;
        [ImmediatePostData]
        [XafDisplayName("Approval Type"), ToolTip("Select Type")]
        [Appearance("AppType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(8)]
        public ApprovalTypes AppType
        {
            get { return _AppType; }
            set
            {
                if (SetPropertyValue("AppType", ref _AppType, value))
                {
                    if (!IsLoading)
                    {
                        SetPropertyValue("DocAmount", ref _DocAmount, 0);
                    }
                }
            }
        }

        private int _ApprovalCnt;
        [XafDisplayName("Number of Approval"), ToolTip("Enter Number")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public int ApprovalCnt
        {
            get { return _ApprovalCnt; }
            set
            {
                SetPropertyValue("ApprovalCnt", ref _ApprovalCnt, value);
            }
        }

        private string _ApprovalLevel;
        [XafDisplayName("Approval Level"), ToolTip("Enter Number")]
        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue]
        [Index(13)]
        public string ApprovalLevel
        {
            get { return _ApprovalLevel; }
            set
            {
                SetPropertyValue("ApprovalLevel", ref _ApprovalLevel, value);
            }
        }

        private Entity _Entity;
        [ImmediatePostData]
        [DataSourceCriteria("IsActive = 'True'")]
        [XafDisplayName("Entity")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(15)]
        public Entity Entity
        {
            get { return _Entity; }
            set
            {
                SetPropertyValue("Entity", ref _Entity, value);
            }
        }

        private vwDepartment _Department;
        [NoForeignKey]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [XafDisplayName("Department")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(18)]
        public vwDepartment Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private decimal _DocAmount;
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Document Amount"), ToolTip("Enter Number")]
        [Index(50)]
        public decimal DocAmount
        {
            get { return _DocAmount; }
            set
            {
                SetPropertyValue("DocAmount", ref _DocAmount, value);
            }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        [Index(80)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Association("Approvals-ApprovalUsers")]
        [XafDisplayName("Approval Users")]
        public XPCollection<ApprovalUsers> ApprovalUsers
        {
            get { return GetCollection<ApprovalUsers>("ApprovalUsers"); }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        public XPCollection<AuditDataItemPersistent> AuditTrail
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
        }
    }
}