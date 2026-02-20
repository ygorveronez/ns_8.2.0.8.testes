<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Logon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>Acesso ao Sistema</title>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <link href="Styles/ui/ui.base.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.login.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/theme/ui.min.css" rel="stylesheet" type="text/css" />
    <!--[if IE 7]>
	    <link href="Styles/ie7.min.css" rel="stylesheet" media="all" />
	<![endif]-->
    <!--[if IE 6]>
	    <link href="Styles/ie6.min.css" rel="stylesheet" media="all" />
	    <script defer="defer" src="Scripts/pngfix.min.js" type="text/javascript"></script>
	    <script defer="defer">
	      /* Fix IE6 Transparent PNG */
	      DD_belatedPNG.fix('.logo, .other ul#dashboard-buttons li a');
	    </script>
	<![endif]-->
    <script defer="defer" src="Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.core.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/superfish.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/custom.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="page_wrapper">
            <div id="page-header">
                <div id="page-header-wrapper">
                    <div id="top">
                        <%--<a href="#" class="logo" title="Emiss�o de CTe">Emiss�o de CTe</a>--%>
                    </div>
                </div>
            </div>
            <div id="sub-nav">
                <div class="page-title">
                    <h1>Acesso ao Sistema Administrativo</h1>
                    <span>Utilize os campos abaixo para acessar o sistema.</span>
                </div>
            </div>
            <div class="clear">
            </div>
            <div id="page-layout">
                <div id="page-content">
                    <div id="page-content-wrapper">
                        <div id="tabs" class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                            <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all" style="width: 530px">
                                <li class="ui-state-default ui-corner-top ui-tabs-selected ui-state-active"><a style="cursor: default;">Login </a></li>
                            </ul>
                            <div class="ui-tabs-panel ui-widget-content ui-corner-bottom">
                                <div id="divMsgErroLogin" runat="server" class="response-msg error ui-corner-all" style="display: none;">
                                    <span>Falha ao Acessar</span> Usu&aacute;rio ou senha incorretos!
                                </div>
                                <div id="divMsgEnvioEmail" runat="server" class="response-msg error ui-corner-all" style="display: none;">
                                    <span>Tentativas de Acesso Excedidas</span> Uma nova senha foi encaminhada para seu e-mail.<br />
                                    Favor consultar seu e&#45;mail ou entrar em contato com o suporte!
                                </div>
                                <div id="divMsgErroSSO" runat="server" class="response-msg error ui-corner-all" style="display: none;">
                                    <span>Erro SSO</span> <asp:Label ID="lblErroSSO" runat="server"></asp:Label>
                                </div>
                                <ul>
                                    <li>
                                        <label class="desc">
                                            Usu&aacute;rio:
                                        </label>
                                        <div>
                                            <asp:TextBox ID="txtUsuario" runat="server" MaxLength="100" CssClass="field text full"></asp:TextBox>
                                        </div>
                                    </li>
                                    <li>
                                        <label class="desc">
                                            Senha:
                                        </label>
                                        <div>
                                            <asp:TextBox ID="txtSenha" runat="server" MaxLength="100" CssClass="field text full" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </li>
                                    <li class="buttons">
                                        <div>
                                            <asp:Button ID="btnLogar" runat="server" OnClick="btnLogar_Click" Text="Acessar" CssClass="ui-state-default ui-corner-all float-right ui-button" />
                                        </div>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
