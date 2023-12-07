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

namespace CMPRPortal.Module.BusinessObjects.View
{
    [DefaultClassOptions]
    [NavigationItem("SAP")]
    [XafDisplayName("Supplier")]
    [DefaultProperty("BoFullName")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class vwBusinessPartner : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public vwBusinessPartner(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Key]
        [Browsable(true)]
        [XafDisplayName("Code")]
        [Appearance("BoCode", Enabled = false)]
        [Index(0)]
        public string BoCode
        {
            get; set;
        }

        [XafDisplayName("Name")]
        [Appearance("BoName", Enabled = false)]
        [Index(3)]
        public string BoName
        {
            get; set;
        }

        [XafDisplayName("Currency")]
        [Appearance("Currency", Enabled = false)]
        [Index(5)]
        public string Currency
        {
            get; set;
        }

        [XafDisplayName("Address")]
        [Appearance("Address", Enabled = false)]
        [Index(10)]
        public string Address
        {
            get; set;
        }

        [XafDisplayName("ValidFor")]
        [Appearance("ValidFor", Enabled = false)]
        [Index(11)]
        public string ValidFor
        {
            get; set;
        }

        [XafDisplayName("validFrom")]
        [Appearance("validFrom", Enabled = false)]
        [Index(13)]
        public DateTime validFrom
        {
            get; set;
        }

        [XafDisplayName("validTo")]
        [Appearance("validTo", Enabled = false)]
        [Index(15)]
        public DateTime validTo
        {
            get; set;
        }

        [XafDisplayName("PaymentTerm")]
        [NoForeignKey]
        [Appearance("PaymentTerm", Enabled = false)]
        [Index(18)]
        public vwPaymentTerm PaymentTerm
        {
            get; set;
        }

        [XafDisplayName("EntityCompany")]
        [Appearance("EntityCompany", Enabled = false)]
        [Index(20)]
        public string EntityCompany
        {
            get; set;
        }

        [Index(20), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        public string BoFullName
        {
            get { return BoCode + "-" + BoName; }
        }
    }
}