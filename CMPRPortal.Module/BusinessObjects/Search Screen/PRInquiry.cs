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

namespace CMPRPortal.Module.BusinessObjects.Search_Screen
{
    [DefaultClassOptions]
    [XafDisplayName("Purchase Request Inquiry")]
    [DefaultProperty("Cart")]

    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("SaveAndCloseDocRecord", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "Cancel", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class PRInquiry : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PRInquiry(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _DocNum;
        [XafDisplayName("Doc Num")]
        [Appearance("DocNum", Enabled = false)]
        [Index(3), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private Entity _Entity;
        [ImmediatePostData]
        [DataSourceCriteria("IsActive = 'True'")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [XafDisplayName("Entity")]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
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
        [XafDisplayName("Department")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("EntityCompany = '@this.Entity.CompanyName'")]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public vwDepartment Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private DateTime _ExpectedDeliveryDate;
        [XafDisplayName("Expected Delivery Date")]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime ExpectedDeliveryDate
        {
            get { return _ExpectedDeliveryDate; }
            set
            {
                SetPropertyValue("ExpectedDeliveryDate", ref _ExpectedDeliveryDate, value);
            }
        }

        [Association("PRInquiry-PRInquiryDetails")]
        [XafDisplayName("Purchase Requests")]
        public XPCollection<PRInquiryDetails> PRInquiryDetails
        {
            get { return GetCollection<PRInquiryDetails>("PRInquiryDetails"); }
        }
    }
}