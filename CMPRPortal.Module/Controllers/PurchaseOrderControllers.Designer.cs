namespace CMPRPortal.Module.Controllers
{
    partial class PurchaseOrderControllers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PRInquiry = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.SubmitPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CancelPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // PRInquiry
            // 
            this.PRInquiry.AcceptButtonCaption = null;
            this.PRInquiry.CancelButtonCaption = null;
            this.PRInquiry.Caption = "PR. Inquiry";
            this.PRInquiry.Category = "ObjectsCreation";
            this.PRInquiry.ConfirmationMessage = null;
            this.PRInquiry.Id = "PRInquiry";
            this.PRInquiry.ToolTip = null;
            this.PRInquiry.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.PRInquiry_CustomizePopupWindowParams);
            this.PRInquiry.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.PRInquiry_Execute);
            // 
            // SubmitPO
            // 
            this.SubmitPO.AcceptButtonCaption = null;
            this.SubmitPO.CancelButtonCaption = null;
            this.SubmitPO.Caption = "Submit";
            this.SubmitPO.Category = "ObjectsCreation";
            this.SubmitPO.ConfirmationMessage = null;
            this.SubmitPO.Id = "SubmitPO";
            this.SubmitPO.ToolTip = null;
            this.SubmitPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.SubmitPO_CustomizePopupWindowParams);
            this.SubmitPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.SubmitPO_Execute);
            // 
            // CancelPO
            // 
            this.CancelPO.AcceptButtonCaption = null;
            this.CancelPO.CancelButtonCaption = null;
            this.CancelPO.Caption = "Cancel";
            this.CancelPO.Category = "ObjectsCreation";
            this.CancelPO.ConfirmationMessage = null;
            this.CancelPO.Id = "CancelPO";
            this.CancelPO.ToolTip = null;
            this.CancelPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CancelPO_CustomizePopupWindowParams);
            this.CancelPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CancelPO_Execute);
            // 
            // PurchaseOrderControllers
            // 
            this.Actions.Add(this.PRInquiry);
            this.Actions.Add(this.SubmitPO);
            this.Actions.Add(this.CancelPO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PRInquiry;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction SubmitPO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelPO;
    }
}
