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
using CMPRPortal.Module.BusinessObjects.Setup;
using CMPRPortal.Module.BusinessObjects.View;
using CMPRPortal.Module.BusinessObjects.Maintenance;

namespace CMPRPortal.Module.BusinessObjects.PR
{
    [DefaultClassOptions]
    [XafDisplayName("Purchase Request")]
    [NavigationItem("Purchase Request")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_PendApp")]
    [Appearance("HideNew1", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_Approved")]
    [Appearance("HideNew2", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_PendApp")]
    [Appearance("HideNew3", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_Approved")]
    [Appearance("HideNew4", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_Escalate")]
    [Appearance("HideNew5", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_Escalate")]

    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitPR", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelPR", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    [Appearance("HideDup", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_PendApp")]
    [Appearance("HideDup1", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_Approved")]
    [Appearance("HideDup2", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_PendApp")]
    [Appearance("HideDup3", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_Approved")]
    [Appearance("HideDup4", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_ListView_Escalate")]
    [Appearance("HideDup5", AppearanceItemType.Action, "True", TargetItems = "DuplicatePR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "PurchaseRequests_DetailView_Escalate")]

    public class PurchaseRequests : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequests(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (user != null)
            {
                CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                UserID = user.UserName;
                UserName = user.StaffName;
                if (user.DefaultEntity != null)
                {
                    Entity = Session.FindObject<Entity>(new BinaryOperator("Oid", user.DefaultEntity.Oid));
                    DocType = Session.FindObject<DocTypes>(CriteriaOperator.Parse("BoCode = ? and Entity.Oid = ?  and IsActive = ?",
                    DocTypeList.PR, user.DefaultEntity.Oid, "True"));
                }
                if (user.DefaultDept != null)
                {
                    Department = Session.FindObject<vwDepartment>(new BinaryOperator("DepartmentCode", user.DefaultDept.DepartmentCode));
                }
            }
            CreateDate = DateTime.Now;
            DocDate = DateTime.Now;
            ExpectedDeliveryDate = DateTime.Now;
            EventDate = DateTime.Now;

            Status = DocStatus.Draft;
            AppStatus = ApprovalStatusType.Not_Applicable;
        }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateUser", Enabled = false)]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateDate", Enabled = false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateUser", Enabled = false)]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateDate", Enabled = false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private DocTypes _DocType;
        [XafDisplayName("Doc Type"), ToolTip("Enter Text")]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(304), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private string _DocNum;
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Document No")]
        [Appearance("DocNum", Enabled = false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private string _UserID;
        [XafDisplayName("User ID")]
        [Appearance("UserID", Enabled = false)]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string UserID
        {
            get { return _UserID; }
            set
            {
                SetPropertyValue("UserID", ref _UserID, value);
            }
        }

        private string _UserName;
        [XafDisplayName("User Name")]
        [Appearance("UserName", Enabled = false)]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string UserName
        {
            get { return _UserName; }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }

        private Entity _Entity;
        [DataSourceCriteria("IsActive = 'True'")]
        [XafDisplayName("Entity")]
        [Appearance("Entity", Enabled = false)]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public Entity Entity
        {
            get { return _Entity; }
            set { SetPropertyValue("Entity", ref _Entity, value); }
        }

        private vwDepartment _Department;
        [NoForeignKey]
        [XafDisplayName("Department")]
        [Appearance("Department", Enabled = false)]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwDepartment Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private DateTime _DocDate;
        [XafDisplayName("Document Date")]
        [Index(13), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
            }
        }

        private DateTime _ExpectedDeliveryDate;
        [XafDisplayName("Expected Delivery Date")]
        [Index(15), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime ExpectedDeliveryDate
        {
            get { return _ExpectedDeliveryDate; }
            set
            {
                SetPropertyValue("ExpectedDeliveryDate", ref _ExpectedDeliveryDate, value);
            }
        }

        private DateTime _EventDate;
        [XafDisplayName("Event Date")]
        [Index(18), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime EventDate
        {
            get { return _EventDate; }
            set
            {
                SetPropertyValue("EventDate", ref _EventDate, value);
            }
        }

        private DocStatus _Status;
        [XafDisplayName("Status")]
        [Appearance("Status", Enabled = false)]
        [Index(20), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DocStatus Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private ApprovalStatusType _AppStatus;
        [XafDisplayName("Approval Status")]
        [Appearance("AppStatus", Enabled = false)]
        [Index(23), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public ApprovalStatusType AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private string _BanquetOrderNo;
        [XafDisplayName("Banquet Order No")]
        [Index(25), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string BanquetOrderNo
        {
            get { return _BanquetOrderNo; }
            set
            {
                SetPropertyValue("BanquetOrderNo", ref _BanquetOrderNo, value);
            }
        }

        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Size(254)]
        [Index(28), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private string _NextApprover;
        [XafDisplayName("Next Approver")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(82), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("Next Approver", Enabled = false)]
        public string NextApprover
        {
            get { return _NextApprover; }
            set
            {
                SetPropertyValue("NextApprover", ref _NextApprover, value);
            }
        }

        private string _WhoApprove;
        [XafDisplayName("WhoApprove")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(83), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("WhoApprove", Enabled = false)]
        public string WhoApprove
        {
            get { return _WhoApprove; }
            set
            {
                SetPropertyValue("WhoApprove", ref _WhoApprove, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Association("PurchaseRequests-PurchaseRequestDetails")]
        [XafDisplayName("Items")]
        public XPCollection<PurchaseRequestDetails> PurchaseRequestDetails
        {
            get { return GetCollection<PurchaseRequestDetails>("PurchaseRequestDetails"); }
        }

        [Association("PurchaseRequests-PurchaseRequestDocTrail")]
        [XafDisplayName("Status History")]
        public XPCollection<PurchaseRequestDocTrail> PurchaseRequestDocTrail
        {
            get { return GetCollection<PurchaseRequestDocTrail>("PurchaseRequestDocTrail"); }
        }

        [Association("PurchaseRequests-PurchaseRequestAppStage")]
        [XafDisplayName("Approval Stage")]
        public XPCollection<PurchaseRequestAppStage> PurchaseRequestAppStage
        {
            get { return GetCollection<PurchaseRequestAppStage>("PurchaseRequestAppStage"); }
        }

        [Association("PurchaseRequests-PurchaseRequestAppStatus")]
        [XafDisplayName("Approval Status")]
        public XPCollection<PurchaseRequestAppStatus> PurchaseRequestAppStatus
        {
            get { return GetCollection<PurchaseRequestAppStatus>("PurchaseRequestAppStatus"); }
        }

        [Association("PurchaseRequests-PurchaseRequestAttachment")]
        [XafDisplayName("Attachment")]
        public XPCollection<PurchaseRequestAttachment> PurchaseRequestAttachment
        {
            get { return GetCollection<PurchaseRequestAttachment>("PurchaseRequestAttachment"); }
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

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                if (user != null)
                {
                    UpdateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                }
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {
                    PurchaseRequestDocTrail ds = new PurchaseRequestDocTrail(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocRemarks = "";
                    if (user != null)
                    {
                        ds.CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                        ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                    }
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateDate = DateTime.Now;
                    this.PurchaseRequestDocTrail.Add(ds);
                }
            }
        }
    }
}