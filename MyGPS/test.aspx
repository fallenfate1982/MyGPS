<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="MyGPS.test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="js/endless/endless_wizard.js"></script>
        <link rel="stylesheet" href="css/bootstrap-timepicker.css" />
    
     <script src="js/bootstrap-timepicker.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="input-append bootstrap-timepicker">
            <input id="timepicker1" type="text" class="input-small">
            <span class="add-on"><i class="fa fa-clock-o fa-lg"></i></span>
        </div>
 
        <script type="text/javascript">
            $('#timepicker1').timepicker();
        </script>
    <div class="panel panel-default">
					<form class="form-horizontal no-margin form-border" id="formWizard1" novalidate>
						<div class="panel-heading">
							Form Wizard
						</div>
						<div class="panel-tab">
							<ul class="wizard-steps wizard-demo" id="wizardDemo1"> 
								<li class="active">
									<a href="#wizardContent1" data-toggle="tab">Step 1</a>
								</li> 
								<li>
									<a href="#wizardContent2" data-toggle="tab">Step 2</a>
								</li> 
								<li>
									<a href="#wizardContent3" data-toggle="tab">Step 3</a>
								</li>
							</ul>
						</div>
							
						<div class="panel-body">
							<div class="tab-content">
								<div class="tab-pane fade in active" id="wizardContent1">
									<div class="form-group">
										<label class="control-label col-lg-2">Username</label>
										<div class="col-lg-6">
											<input type="text" placeholder="Normal text input" class="form-control input-sm" data-required="true">
										</div><!-- /.col -->
									</div><!-- /form-group -->
									<div class="form-group">
										<label class="control-label col-lg-2">Password</label>
										<div class="col-lg-6">
											  <input type="password" placeholder="Password" class="form-control input-sm" data-required="true">
										</div><!-- /.col -->
									</div><!-- /form-group -->
									<div class="form-group">
										<label class="control-label col-lg-2">Email</label>
										<div class="col-lg-6">
											<input type="text" class="form-control input-sm" placeholder="test@example.com" data-required="true" data-type="email">
										</div><!-- /.col -->
									</div><!-- /form-group -->		
								</div>
								<div class="tab-pane fade" id="wizardContent2">
									<div class="form-group">
										<label class="control-label col-lg-2">Phone</label>
										<div class="col-lg-6">
											<input type="text" class="form-control input-sm" placeholder="(XXX) XXXX XXX" data-required="true" data-type="phone">
										</div><!-- /.col -->
									</div><!-- /form-group -->		
									<div class="form-group">
										<label class="control-label col-lg-2">Website</label>
										<div class="col-lg-6">
											<input type="text" class="form-control input-sm" placeholder="Website url" data-required="true" data-type="urlstrict">
										</div><!-- /.col -->
									</div><!-- /form-group -->
								</div>
								<div class="tab-pane fade padding-md" id="wizardContent3">
									<h4>Finish!</h4>
								</div>
							</div>
						</div>
						<div class="panel-footer clearfix">
							<div class="pull-left">
								<button class="btn btn-success btn-sm disabled" id="prevStep1" disabled>Previous</button>
								<button type="submit" class="btn btn-sm btn-success" id="nextStep1">Next</button>
							</div>

							<div class="pull-right" style="width:30%">
								<div class="progress progress-striped active m-top-sm m-bottom-none">
									<div class="progress-bar progress-bar-success" id="wizardProgress" style="width:33%;">
									</div>
								</div>
							</div>
						</div>
					</form>
				</div><!-- /panel -->


    </asp:Content>

