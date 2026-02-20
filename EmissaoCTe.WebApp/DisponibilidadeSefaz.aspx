<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisponibilidadeSefaz.aspx.cs" Inherits="EmissaoCTe.WebApp.DisponibilidadeSefaz" ViewStateMode="Disabled" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        .reportContainer {
            margin-top: 3px;
            height: 100%;
            border: 2px solid #dedede;
            width: 600px;
        }
    </style>
    <link rel="shortcut icon" href="Images/favicon.ico" />
    <script src="Scripts/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockui.js"></script>
    <script src="Scripts/jquery.maskedinput.js"></script>
    <script src="Scripts/CTE.Mensagens.js"></script>
    <script src="Scripts/CTe.Ajax.js"></script>
    <script src="Scripts/validaCampos.js"></script>
    <script src="Scripts/powerbi.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            Carregar();
        });

        function Carregar() {
            executarRest("/BI/Report?callback=?", { ID: "1" }, function (r) {
                if (r.Sucesso) {

                    var models = window['powerbi-client'].models;

                    var config = {
                        type: 'report',
                        tokenType: models.TokenType.Embed,
                        accessToken: r.Objeto.accessToken,
                        embedUrl: r.Objeto.embedUrl,
                        id: r.Objeto.embedReportId,
                        permissions: models.Permissions.All,
                        settings: {
                            filterPaneEnabled: true,
                            navContentPaneEnabled: true,
                        }
                    };

                    var reportContainer = document.getElementById('reportContainer');

                    var report = powerbi.embed(reportContainer, config);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</head>
<body>
    <div class="page-header">
        <h2>Disponibilidade Sefaz CTe
        </h2>
    </div>

    <div id="reportContainer" style="width: 90%; height: 90%;"></div>
</body>
</html>
