<%@ Page Title="Integrações LS Translog" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="LSTranslogIntegracoes.aspx.cs" Inherits="EmissaoCTe.WebApp.LSTranslogIntegracoes" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/cookie",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/LSTranslogIntegracoes") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Integrações LS Translog</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Num. Doc. Inicial:
                </span>
                <input type="text" id="txtFiltroNumeroInical" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Num. Doc. Final:
                </span>
                <input type="text" id="txtFiltroNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtFiltroDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtFiltroDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo Doc.:
                </span>
                <select id="selFiltroTipoDocumento" class="form-control">
                    <option value="">Todos</option>
                    <option value="1">CTe</option>
                    <option value="2">NFSe</option>
                    <option value="3">NFe</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Status Envio:
                </span>
                <select id="selFiltroStatusEnvio" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Pendente</option>
                    <option value="1">Sucesso</option>
                    <option value="9">Erro</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Status Consulta:
                </span>
                <select id="selFiltroStatusConsulta" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Pendente</option>
                    <option value="1">Sucesso</option>
                    <option value="9">Erro</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Num. Nota:
                </span>
                <input type="text" id="txtFiltroNumeroNota" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Identificador (OS):
                </span>
                <input type="text" id="txtFiltroIdentificador" class="form-control maskedInput" />
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarIntegracoes" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_integracoes" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_integracoes">
    </div>
    

    <div class="modal fade" id="divModalLogs" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Log Integrações</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgModalLogs"></div>

                    <div id="tbl_logs"></div>
                    <div id="tbl_paginacao_logs"></div>
                    <div class="clearfix"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Fechar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divModalMensagem" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Mensagem Log</h4>
                </div>
                <div class="modal-body">
                    <div class="mensagem"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Fechar</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
