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

namespace CMPRPortal.Module.BusinessObjects
{
    public enum DocTypeList
    {
        [XafDisplayName("Purchase Request")] PR = 0,
        [XafDisplayName("Purchase Order")] PO = 1
    }

    public enum DocStatus
    {
        [XafDisplayName("Draft")] Draft = 0,
        [XafDisplayName("Submitted")] Submitted = 1,
        [XafDisplayName("Cancelled")] Cancelled = 2,
        [XafDisplayName("Closed")] Closed = 3,
        [XafDisplayName("Pending Post")] PendPost = 4,
        [XafDisplayName("Posted")] Post = 5,
        [XafDisplayName("Rejected")] Rejected = 6
    }

    public enum ApprovalStatusType
    {
        Not_Applicable = 0,
        Approved = 1,
        Required_Approval = 2,
        Rejected = 3
    }

    public enum ApprovalActions
    {
        [XafDisplayName("Please Select Action...")] NA = 0,
        [XafDisplayName("Yes")] Yes = 1,
        [XafDisplayName("No")] No = 2
    }

    public enum ApprovalTypes
    {
        Document = 0
    }
}