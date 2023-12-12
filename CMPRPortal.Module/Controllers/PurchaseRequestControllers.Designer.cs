namespace CMPRPortal.Module.Controllers
{
    partial class PurchaseRequestControllers
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
            this.SubmitPR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CancelPR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.DuplicatePR = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SubmitPR
            // 
            this.SubmitPR.AcceptButtonCaption = null;
            this.SubmitPR.CancelButtonCaption = null;
            this.SubmitPR.Caption = "Submit";
            this.SubmitPR.Category = "ObjectsCreation";
            this.SubmitPR.ConfirmationMessage = null;
            this.SubmitPR.Id = "SubmitPR";
            this.SubmitPR.ToolTip = null;
            this.SubmitPR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.SubmitPR_CustomizePopupWindowParams);
            this.SubmitPR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.SubmitPR_Execute);
            // 
            // CancelPR
            // 
            this.CancelPR.AcceptButtonCaption = null;
            this.CancelPR.CancelButtonCaption = null;
            this.CancelPR.Caption = "Cancel";
            this.CancelPR.Category = "ObjectsCreation";
            this.CancelPR.ConfirmationMessage = null;
            this.CancelPR.Id = "CancelPR";
            this.CancelPR.ToolTip = null;
            this.CancelPR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CancelPR_CustomizePopupWindowParams);
            this.CancelPR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CancelPR_Execute);
            // 
            // DuplicatePR
            // 
            this.DuplicatePR.Caption = "Duplicate";
            this.DuplicatePR.Category = "ObjectsCreation";
            this.DuplicatePR.ConfirmationMessage = null;
            this.DuplicatePR.Id = "DuplicatePR";
            this.DuplicatePR.ToolTip = null;
            this.DuplicatePR.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicatePR_Execute);
            // 
            // PurchaseRequestControllers
            // 
            this.Actions.Add(this.SubmitPR);
            this.Actions.Add(this.CancelPR);
            this.Actions.Add(this.DuplicatePR);

        }

        #endregion
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction SubmitPR;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelPR;
        private DevExpress.ExpressApp.Actions.SimpleAction DuplicatePR;
    }
}
