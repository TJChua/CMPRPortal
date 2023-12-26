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
using CMPRPortal.Module.BusinessObjects.Maintenance;
using CMPRPortal.Module.BusinessObjects.Setup;

namespace CMPRPortal.Module.BusinessObjects.PO
{
    [DefaultClassOptions]
    [XafDisplayName("Purchase Order")]
    [NavigationItem("Purchase Order")]
    [Appearance("HideNew", AppearanceItemType = "Action", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitPO", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelPO", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    [Appearance("HidePRInquiry", AppearanceItemType.Action, "True", TargetItems = "PRInquiry", Criteria = "Supplier = null or DocNum = null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class PurchaseOrders : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrders(Session session)
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
                        DocTypeList.PO, user.DefaultEntity.Oid, "True"));
                }
                if (user.DefaultDept != null)
                {
                    Department = Session.FindObject<vwDepartment>(new BinaryOperator("DepartmentCode", user.DefaultDept.DepartmentCode));
                }
            }
            CreateDate = DateTime.Now;
            DocDate = DateTime.Now;
            ExpectedDeliveryDate = DateTime.Now;

            Status = DocStatus.Draft;
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

        private vwBusinessPartner _Supplier;
        [XafDisplayName("Supplier")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [RuleRequiredField(DefaultContexts.Save)]
        [DataSourceCriteria("ValidFor = 'Y' and EntityCompany = '@this.Entity.CompanyName'")]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwBusinessPartner Supplier
        {
            get { return _Supplier; }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
                if (!IsLoading && value != null)
                {
                    SupplierName = Supplier.BoName;
                    SupplierAddress = Supplier.Address;
                    if (Supplier.PaymentTerm != null)
                    {
                        PaymentTerm = Session.FindObject<vwPaymentTerm>(CriteriaOperator.Parse("GroupNum = ? and EntityCompany = ?", 
                            Supplier.PaymentTerm.GroupNum, Entity.CompanyName));
                    }
                    Currency = Supplier.Currency;
                }
                else if (!IsLoading && value == null)
                {
                    SupplierName = null;
                    SupplierAddress = null;
                    PaymentTerm = null;
                    Currency = null;
                }
            }
        }

        private string _SupplierName;
        [XafDisplayName("Supplier Name")]
        [Appearance("SupplierName", Enabled = false)]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string SupplierName
        {
            get { return _SupplierName; }
            set
            {
                SetPropertyValue("SupplierName", ref _SupplierName, value);
            }
        }

        private string _SupplierAddress;
        [XafDisplayName("Supplier Address")]
        [Appearance("SupplierAddress", Enabled = false)]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string SupplierAddress
        {
            get { return _SupplierAddress; }
            set
            {
                SetPropertyValue("SupplierAddress", ref _SupplierAddress, value);
            }
        }

        private string _Currency;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Currency")]
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Currency", Enabled = false)]
        public string Currency
        {
            get { return _Currency; }
            set
            {
                SetPropertyValue("Currency", ref _Currency, value);
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

        private vwPaymentTerm _PaymentTerm;
        [NoForeignKey]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [XafDisplayName("Payment Term")]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [Appearance("PaymentTerm", Enabled = false)]
        [Index(13), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwPaymentTerm PaymentTerm
        {
            get { return _PaymentTerm; }
            set
            {
                SetPropertyValue("PaymentTerm", ref _PaymentTerm, value);
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

        private decimal _SubTotal;
        [XafDisplayName("SubTotal")]
        [Appearance("SubTotal", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal Total
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (PurchaseOrderDetails != null)
                        rtn += PurchaseOrderDetails.Sum(p => p.LineTotalWithoutDiscount);

                    return rtn;
                }
                else
                {
                    return _SubTotal;
                }
            }
            set
            {
                SetPropertyValue("SubTotal", ref _SubTotal, value);
            }
        }

        private decimal _Discount;
        [XafDisplayName("Discount")]
        [Appearance("Discount", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal Discount
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal sub = 0;
                    if (PurchaseOrderDetails != null)
                        sub += PurchaseOrderDetails.Sum(p => p.LineTotalWithoutDiscount);

                    decimal grand = 0;
                    if (PurchaseOrderDetails != null)
                        grand += PurchaseOrderDetails.Sum(p => p.LineTotal);

                    if (sub == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ((sub - grand) / sub) * 100;
                    }
                }
                else
                {
                    return _Discount;
                }
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }

        private decimal _TaxAmount;
        [XafDisplayName("Tax Amount")]
        [Appearance("TaxAmount", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal TaxAmount
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (PurchaseOrderDetails != null)
                        rtn += PurchaseOrderDetails.Sum(p => p.TaxAmount);

                    return rtn;
                }
                else
                {
                    return _TaxAmount;
                }
            }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
            }
        }

        private decimal _GrandTotal;
        [XafDisplayName("Grand Total")]
        [Appearance("GrandTotal", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal GrandTotal
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (PurchaseOrderDetails != null)
                        rtn += PurchaseOrderDetails.Sum(p => p.LineTotal);

                    return rtn;
                }
                else
                {
                    return _GrandTotal;
                }
            }
            set
            {
                SetPropertyValue("GrandTotal", ref _GrandTotal, value);
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

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Association("PurchaseOrders-PurchaseOrderDetails")]
        [XafDisplayName("Items")]
        public XPCollection<PurchaseOrderDetails> PurchaseOrderDetails
        {
            get { return GetCollection<PurchaseOrderDetails>("PurchaseOrderDetails"); }
        }

        [Association("PurchaseOrders-PurchaseOrderDocTrail")]
        [XafDisplayName("Status History")]
        public XPCollection<PurchaseOrderDocTrail> PurchaseOrderDocTrail
        {
            get { return GetCollection<PurchaseOrderDocTrail>("PurchaseOrderDocTrail"); }
        }

        [Association("PurchaseOrders-PurchaseOrderAttachment")]
        [XafDisplayName("Attachment")]
        public XPCollection<PurchaseOrderAttachment> PurchaseOrderAttachment
        {
            get { return GetCollection<PurchaseOrderAttachment>("PurchaseOrderAttachment"); }
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
                    PurchaseOrderDocTrail ds = new PurchaseOrderDocTrail(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocRemarks = "";
                    if (user != null)
                    {
                        ds.CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                        ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                    }
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateDate = DateTime.Now;
                    this.PurchaseOrderDocTrail.Add(ds);
                }
            }
        }
    }
}