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

namespace CMPRPortal.Module.BusinessObjects.PO
{
    [DefaultClassOptions]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("PODetails")]

    public class PurchaseOrderDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrderDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;
            Quantity = 1;
        }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private vwItemMasters _ItemCode;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Item Code")]
        [DataSourceCriteria("frozenFor = 'N' and EntityCompany = '@this.Entity.CompanyName'")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ItemCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwItemMasters ItemCode
        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
                if (!IsLoading && value != null)
                {
                    ItemDesc = ItemCode.ItemName;
                    UOM = ItemCode.UOM;
                }
                else if (!IsLoading && value == null)
                {
                    ItemDesc = null;
                    UOM = null;
                }
            }
        }

        private string _ItemDesc;
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Item Description")]
        [Appearance("ItemDesc", Enabled = false)]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDesc
        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private string _UOM;
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Unit of Measure")]
        [Appearance("UOM", Enabled = false)]
        public string UOM
        {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
            }
        }

        private decimal _Quantity;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Quantity")]
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    LineTotal = (Quantity * UnitPrice) + TaxAmount - (Discount / 100 * Quantity * UnitPrice);
                    LineTotalWithoutDiscount = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private decimal _UnitPrice;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Unit Price")]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
                if (!IsLoading)
                {
                    if (Tax != null)
                    {
                        TaxAmount = (Tax.Rate / 100 * (Quantity * UnitPrice));
                    }
                    LineTotal = (Quantity * UnitPrice) + TaxAmount - (Discount/100 * Quantity * UnitPrice);
                    LineTotalWithoutDiscount = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private bool _FOC;
        [ImmediatePostData]
        [XafDisplayName("FOC")]
        [Index(13), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool FOC
        {
            get { return _FOC; }
            set
            {
                SetPropertyValue("FOC", ref _FOC, value);
                if (!IsLoading && value == true)
                {
                    UnitPrice = 0;
                    Discount = 0;
                }
            }
        }

        private decimal _Discount;
        [ImmediatePostData]
        [XafDisplayName("Discount")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(15), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal Discount
        {
            get { return _Discount; }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
                if (!IsLoading)
                {
                    LineTotal = (Quantity * UnitPrice) + TaxAmount - (Discount / 100 * Quantity * UnitPrice);
                    LineTotalWithoutDiscount = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private vwTax _Tax;
        [NoForeignKey]
        [ImmediatePostData]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Tax Code")]
        public vwTax Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
                if (!IsLoading)
                {
                    TaxAmount = ((Tax.Rate / 100) * (Quantity * UnitPrice));
                }
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Tax Amount")]
        [Appearance("TaxAmount", Enabled = false)]
        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
                if (!IsLoading)
                {
                    LineTotal = (Quantity * UnitPrice) + TaxAmount - (Discount / 100 * Quantity * UnitPrice);
                }
            }
        }

        private decimal _LineTotalWithoutDiscount;
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Line Total Without Discount")]
        [Appearance("LineTotalWithoutDiscount", Enabled = false)]
        [Index(25), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal LineTotalWithoutDiscount
        {
            get { return _LineTotalWithoutDiscount; }
            set
            {
                SetPropertyValue("LineTotalWithoutDiscount", ref _LineTotalWithoutDiscount, value);
            }
        }

        private decimal _LineTotal;
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Line Total")]
        [Appearance("LineTotal", Enabled = false)]
        [Index(28), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal LineTotal
        {
            get { return _LineTotal; }
            set
            {
                SetPropertyValue("LineTotal", ref _LineTotal, value);
            }
        }

        private DateTime _EventDate;
        [XafDisplayName("Event Date")]
        [Appearance("EventDate", Enabled = false)]
        [Index(30), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime EventDate
        {
            get { return _EventDate; }
            set
            {
                SetPropertyValue("EventDate", ref _EventDate, value);
            }
        }

        private string _BanquetOrderNo;
        [XafDisplayName("Banquet Order No")]
        [Appearance("BanquetOrderNo", Enabled = false)]
        [Index(33), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
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
        [Appearance("Remarks", Enabled = false)]
        [Index(35), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private Entity _Entity;
        [XafDisplayName("Entity")]
        [Index(50), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public Entity Entity
        {
            get { return _Entity; }
            set { SetPropertyValue("Entity", ref _Entity, value); }
        }

        private PurchaseOrders _PurchaseOrders;
        [Association("PurchaseOrders-PurchaseOrderDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PurchaseOrders", Enabled = false)]
        public PurchaseOrders PurchaseOrders
        {
            get { return _PurchaseOrders; }
            set { SetPropertyValue("PurchaseOrders", ref _PurchaseOrders, value); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {

                }
            }
        }
    }
}