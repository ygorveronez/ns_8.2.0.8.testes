<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="EmissaoCTe.WebApp.Logon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <title>Acesso ao Sistema</title>
    <link rel="shortcut icon" href="Images/favicon.ico" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        body
        {
            padding-top: 40px;
            padding-bottom: 40px;
            background-color: #eee;
        }

        .form-signin
        {
            max-width: 330px;
            padding: 15px;
            margin: 0 auto;
        }

            .form-signin .form-signin-heading,
            .form-signin .checkbox
            {
                margin-bottom: 10px;
                text-align: center;
            }

            .form-signin .checkbox
            {
                font-weight: normal;
            }

            .form-signin .form-control
            {
                position: relative;
                font-size: 16px;
                height: auto;
                padding: 10px;
                -webkit-box-sizing: border-box;
                -moz-box-sizing: border-box;
                box-sizing: border-box;
            }

                .form-signin .form-control:focus
                {
                    z-index: 2;
                }

            .form-signin input[type="text"]
            {
                margin-bottom: -1px;
                border-bottom-left-radius: 0;
                border-bottom-right-radius: 0;
            }

            .form-signin input[type="password"]
            {
                margin-bottom: 10px;
                border-top-left-radius: 0;
                border-top-right-radius: 0;
            }
    </style>
    <script src="Scripts/jquery-3.0.0.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.js" type="text/javascript"></script>
    <% #if !DEBUG %>
    <script type="text/javascript">
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
        ga('create', 'UA-43029832-1', 'cloudapp.net');
        ga('send', 'pageview');
    </script>
    <% #endif %>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server" class="form-signin">
            <img class="img-responsive img-rounded" src="Images/logocopyservices.png" />
            <h3 class="form-signin-heading">Acesso ao Sistema</h3>
            <div id="divMsgErroLogin" runat="server" class="alert alert-danger"
                style="display: none;">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                Usu&aacute;rio ou senha incorretos!
            </div>
            <div id="divMsgEnvioEmail" runat="server" class="alert alert-danger"
                style="display: none;">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                <strong>Tentativas de Acesso Excedidas</strong> Uma nova senha foi encaminhada para seu e-mail.<br />
                Favor consultar seu e&#45;mail ou entrar em contato com o suporte!
            </div>

            <asp:TextBox ID="txtUsuario" runat="server" MaxLength="100" CssClass="form-control" placeholder="Usuário" ClientIDMode="Static" autofocus></asp:TextBox>
            <asp:TextBox ID="txtSenha" runat="server" MaxLength="100" CssClass="form-control"
                TextMode="Password" ClientIDMode="Static" placeholder="Senha"></asp:TextBox>
            <asp:Button ID="btnLogar" runat="server" OnClick="btnLogar_Click" Text="Acessar"
                CssClass="btn btn-lg btn-primary btn-block" />
        </form>
    </div>
</body>
</html>
