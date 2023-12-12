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

namespace CMPRPortal.Module.BusinessObjects.Search_Screen
{
    [DefaultClassOptions]
    [DefaultProperty("ItemCode")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    [XafDisplayName("Purchase Request Inquiry Details")]
    public class PRInquiryDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PRInquiryDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _Entity;
        [XafDisplayName("Entity")]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string Entity
        {
            get { return _Entity; }
            set
            {
                SetPropertyValue("Entity", ref _Entity, value);
            }
        }

        private string _Department;
        [XafDisplayName("Department")]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private DateTime _ExpectedDeliveryDate;
        [XafDisplayName("Expected Delivery Date")]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime ExpectedDeliveryDate
        {
            get { return _ExpectedDeliveryDate; }
            set
            {
                SetPropertyValue("ExpectedDeliveryDate", ref _ExpectedDeliveryDate, value);
            }
        }

        private string _BanquetOrderNo;
        [XafDisplayName("Banquet Order No")]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string BanquetOrderNo
        {
            get { return _BanquetOrderNo; }
            set
            {
                SetPropertyValue("BanquetOrderNo", ref _BanquetOrderNo, value);
            }
        }

        private DateTime _EventDate;
        [XafDisplayName("Event Date")]
        [Index(13), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime EventDate
        {
            get { return _EventDate; }
            set
            {
                SetPropertyValue("EventDate", ref _EventDate, value);
            }
        }

        private string _ItemCode;
        [XafDisplayName("Item Code")]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ItemCode", Enabled = false)]
        public string ItemCode
        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
            }
        }

        private string _ItemDesc;
        [XafDisplayName("Item Desc.")]
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ItemDesc", Enabled = false)]
        public string ItemDesc

        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private string _UOM;
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("UOM")]
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
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        private decimal _UnitPrice;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Unit Price")]
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
            }
        }

        private int _BaseDoc;
        [XafDisplayName("BaseDoc")]
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int BaseDoc
        {
            get { return _BaseDoc; }
            set
            {
                SetPropertyValue("BaseDoc", ref _BaseDoc, value);
            }
        }

        private int _BaseID;
        [XafDisplayName("BaseID")]
        [Index(30), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int BaseID
        {
            get { return _BaseID; }
            set
            {
                SetPropertyValue("BaseID", ref _BaseID, value);
            }
        }

        private PRInquiry _PRInquiry;
        [Association("PRInquiry-PRInquiryDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PRInquiry", Enabled = false)]
        public PRInquiry PRInquiry
        {
            get { return _PRInquiry; }
            set { SetPropertyValue("PRInquiry", ref _PRInquiry, value); }
        }
    }
}