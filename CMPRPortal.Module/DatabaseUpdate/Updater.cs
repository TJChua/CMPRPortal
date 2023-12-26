using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using CMPRPortal.Module.BusinessObjects;
using CMPRPortal.Module.BusinessObjects.PO;
using CMPRPortal.Module.BusinessObjects.PR;
using CMPRPortal.Module.BusinessObjects.Search_Screen;
using CMPRPortal.Module.BusinessObjects.Setup;
using CMPRPortal.Module.BusinessObjects.Maintenance;
using CMPRPortal.Module.BusinessObjects.View;

namespace CMPRPortal.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}
            SystemUsers sampleUser = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "User"));
            if(sampleUser == null) {
                sampleUser = ObjectSpace.CreateObject<SystemUsers>();
                sampleUser.UserName = "User";
                sampleUser.SetPassword("");
            }
            PermissionPolicyRole defaultRole = CreateDefaultRole();
            sampleUser.Roles.Add(defaultRole);

            SystemUsers userAdmin = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "Admin"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<SystemUsers>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
			// If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if(adminRole == null) {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;
			userAdmin.Roles.Add(adminRole);

            PermissionPolicyRole AppRole = CreateAppUserRole();
            PermissionPolicyRole EscalateRole = CreateEscalateRole();
            PermissionPolicyRole AccessRole = CreateAccessRole();

            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private PermissionPolicyRole CreateDefaultRole() {
            PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                defaultRole.Name = "Default";

				defaultRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
				defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            }
            return defaultRole;
        }

        private PermissionPolicyRole CreateAppUserRole()
        {
            PermissionPolicyRole ApprovalUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "ApprovalUserRole"));
            if (ApprovalUserRole == null)
            {
                ApprovalUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                ApprovalUserRole.Name = "ApprovalUserRole";
            }

            return ApprovalUserRole;
        }

        private PermissionPolicyRole CreateEscalateRole()
        {
            PermissionPolicyRole EscalateRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "EscalateRole"));
            if (EscalateRole == null)
            {
                EscalateRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                EscalateRole.Name = "EscalateRole";
            }

            return EscalateRole;
        }

        private PermissionPolicyRole CreateAccessRole()
        {
            PermissionPolicyRole AccessRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "AccessRole"));
            if (AccessRole == null)
            {
                AccessRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                AccessRole.Name = "AccessRole";

                AccessRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                AccessRole.AddMemberPermission<SystemUsers>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                AccessRole.AddMemberPermission<SystemUsers>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SystemUsers
                AccessRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //PO
                AccessRole.AddTypePermissionsRecursively<PurchaseOrders>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrders>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrders>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderAttachment>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDocTrail>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDocTrail>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseOrderDocTrail>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                //PR
                AccessRole.AddTypePermissionsRecursively<PurchaseRequests>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequests>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequests>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDetails>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStage>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStage>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStage>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStatus>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStatus>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAppStatus>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestAttachment>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDocTrail>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDocTrail>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PurchaseRequestDocTrail>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                //Search Screen
                AccessRole.AddTypePermissionsRecursively<PRInquiry>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PRInquiry>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PRInquiry>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PRInquiryDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PRInquiryDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<PRInquiryDetails>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                //Setup
                AccessRole.AddTypePermissionsRecursively<Approvals>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<Approvals>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<Approvals>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ApprovalUsers>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ApprovalUsers>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<ApprovalUsers>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<Entity>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<Entity>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<Entity>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                //View
                AccessRole.AddTypePermissionsRecursively<vwBusinessPartner>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<vwDepartment>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<vwItemMasters>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<vwPaymentTerm>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<vwTax>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<vwWarehouse>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //File data
                AccessRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<FileAttachmentBase>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<FileAttachmentBase>(SecurityOperations.Create, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<FileAttachmentBase>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                //Audit Trail
                AccessRole.AddTypePermissionsRecursively<AuditDataItemPersistent>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                AccessRole.AddTypePermissionsRecursively<AuditedObjectWeakReference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            }

            return AccessRole;
        }
    }
}
