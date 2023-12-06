;#DropDB CMPRPortalEasyTest

#Application CMPRPortalWin
#Application CMPRPortalWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef CMPRPortalWin

*Action About...
 
*Action OK

#EndIf

#IfDef CMPRPortalWeb
*Action Navigation(My Details)
#EndIf
