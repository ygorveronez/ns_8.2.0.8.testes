<%@ Page Title="Configurações de Proposta" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ConfiguracoesProposta.aspx.cs" Inherits="EmissaoCTe.WebApp.ConfiguracoesProposta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <script defer="defer" type="text/javascript">
            CKEDITOR_BASEPATH = ObterPath() + '/Scripts/ckeditor/';
        </script>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/ckeditor",
                           "~/bundle/scripts/ckeditoradapters",
                           "~/bundle/scripts/configuracoesProposta") %>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server">
        <style type="text/css">
            .ck-container {
                border-right: 1px solid #ccc;
                border-top-right-radius: 4px;
                border-bottom-right-radius: 4px;
            }
        </style>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Configurações de Proposta</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-5">
            <div class="input-group">
                <span class="input-group-addon">Dias de Validade:
                </span>
                <input type="text" id="txtDiasValidade" class="form-control " />
            </div>
        </div>   
    </div>
    <br />
    <div class="panel-group" id="collapseTextos" role="tablist" aria-multiselectable="true">
        <div class="panel panel-default">
            <div class="panel-heading" role="tab">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoCustosAdicionais" aria-expanded="true" aria-controls="textoCustosAdicionais">Texto sobre Custos Adicionais</a>
                </h4>
            </div>
            <div id="textoCustosAdicionais" class="panel-collapse collapse in" role="tabpanel"">
                <div class="panel-body">
                    <div class="ck-container">
                        <textarea id="txtCustosAdicionais" class="form-control"></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading" role="tab">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoFormaCobranca" aria-expanded="true" aria-controls="textoFormaCobranca">Texto sobre Forma Cobrança</a>
                </h4>
            </div>
            <div id="textoFormaCobranca" class="panel-collapse collapse" role="tabpanel"">
                <div class="panel-body">
                    <div class="ck-container">
                        <textarea id="txtFormaCobranca" class="form-control"></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading" role="tab">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoCTRN" aria-expanded="true" aria-controls="textoCTRN">Texto sobre CTRN</a>
                </h4>
            </div>
            <div id="textoCTRN" class="panel-collapse collapse" role="tabpanel"">
                <div class="panel-body">
                    <div class="ck-container">
                        <textarea id="txtCTRN" class="form-control"></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
</asp:Content>
