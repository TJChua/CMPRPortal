namespace CMPRPortal.Module.Controllers
{
    partial class SearchControllers
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
            this.SearchPR = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CreatePO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.AddToPO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SearchPR
            // 
            this.SearchPR.Caption = "Search";
            this.SearchPR.Category = "ListView";
            this.SearchPR.ConfirmationMessage = null;
            this.SearchPR.Id = "SearchPR";
            this.SearchPR.ToolTip = null;
            this.SearchPR.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SearchPR_Execute);
            // 
            // CreatePO
            // 
            this.CreatePO.Caption = "Create PO";
            this.CreatePO.ConfirmationMessage = null;
            this.CreatePO.Id = "CreatePO";
            this.CreatePO.ToolTip = null;
            this.CreatePO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CreatePO_Execute);
            // 
            // AddToPO
            // 
            this.AddToPO.Caption = "Add To PO";
            this.AddToPO.ConfirmationMessage = null;
            this.AddToPO.Id = "AddToPO";
            this.AddToPO.ToolTip = null;
            this.AddToPO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.AddToPO_Execute);
            // 
            // SearchControllers
            // 
            this.Actions.Add(this.SearchPR);
            this.Actions.Add(this.CreatePO);
            this.Actions.Add(this.AddToPO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SearchPR;
        private DevExpress.ExpressApp.Actions.SimpleAction CreatePO;
        private DevExpress.ExpressApp.Actions.SimpleAction AddToPO;
    }
}
