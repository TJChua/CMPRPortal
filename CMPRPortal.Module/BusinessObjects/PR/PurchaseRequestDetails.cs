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

namespace CMPRPortal.Module.BusinessObjects.PR
{
    [DefaultClassOptions]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("PRDetails")]

    public class PurchaseRequestDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDetails(Session session)
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

            //Tax = Session.FindObject<vwTax>(new BinaryOperator("BoCode", "X1"));
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
                    OpenQty = Quantity;
                    Total = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private decimal _OpenQty;
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [Appearance("OpenQty", Enabled = false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("OpenQty")]
        public decimal OpenQty
        {
            get { return _OpenQty; }
            set
            {
                SetPropertyValue("OpenQty", ref _OpenQty, value);
            }
        }

        private decimal _UnitPrice;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Unit Price")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
                    Total = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private vwTax _Tax;
        [NoForeignKey]
        [ImmediatePostData]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(15), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("Tax Code")]
        public vwTax Tax
        {
            get { return _Tax;}
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
                if (!IsLoading)
                {
                    TaxAmount = ((Tax.Rate/100) * (Quantity * UnitPrice));
                }
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [Index(18), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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
                    Total = (Quantity * UnitPrice) + TaxAmount;
                }
            }
        }

        private decimal _Total;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Total")]
        [Appearance("Total", Enabled = false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Total
        {
            get { return _Total; }
            set
            {
                SetPropertyValue("Total", ref _Total, value);
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

        private PurchaseRequests _PurchaseRequests;
        [Association("PurchaseRequests-PurchaseRequestDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PurchaseRequests", Enabled = false)]
        public PurchaseRequests PurchaseRequests
        {
            get { return _PurchaseRequests; }
            set { SetPropertyValue("PurchaseRequests", ref _PurchaseRequests, value); }
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