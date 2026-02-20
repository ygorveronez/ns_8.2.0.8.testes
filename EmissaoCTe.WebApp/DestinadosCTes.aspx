<%@ Page Title="CT-es Destinados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="DestinadosCTes.aspx.cs" Inherits="EmissaoCTe.WebApp.DestinadosCTes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker","~/bundle/styles/plupload") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                                       "~/bundle/scripts/blockui",
                                                       "~/bundle/scripts/datepicker",
                                                       "~/bundle/scripts/fileDownload",
                                                       "~/bundle/scripts/datetimepicker",
                                                       "~/bundle/scripts/maskedinput",
                                                       "~/bundle/scripts/datatables",
                                                       "~/bundle/scripts/ajax",
                                                       "~/bundle/scripts/gridview",
                                                       "~/bundle/scripts/consulta",
                                                       "~/bundle/scripts/baseConsultas",
                                                       "~/bundle/scripts/mensagens",
                                                       "~/bundle/scripts/validaCampos",
                                                       "~/bundle/scripts/plupload",
                                                       "~/bundle/scripts/priceformat",
                                                       "~/bundle/scripts/destinadosCTes") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>CT-es Destinados</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnConsultarCTesDestinados" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Consultar CT-es Destinados</button>
    <div class="row" style="margin-top: 5px;">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Inicial:
                </span>
                <input type="text" id="txtFiltroDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Final:
                </span>
                <input type="text" id="txtFiltroDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Num. Inicial:
                </span>
                <input type="text" id="txtFiltroNumeroInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Num. Final:
                </span>
                <input type="text" id="txtFiltroNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CNPJ Emissor:
                </span>
                <input type="text" id="txtFiltroCNPJEmissor" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Emissor:
                </span>
                <input type="text" id="txtFiltroNomeEmissor" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CNPJ Remetente:
                </span>
                <input type="text" id="txtFiltroCNPJRemetente" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Remetente:
                </span>
                <input type="text" id="txtFiltroNomeRemetente" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CNPJ Tomador:
                </span>
                <input type="text" id="txtFiltroCNPJTomador" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Nome Tomador:
                </span>
                <input type="text" id="txtFiltroNomeTomador" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Filtrar pela Raiz do CNPJ:
                </span>
                <select id="selFiltroRaizCNPJ" class="form-control">
                    <option value="false" selected>Não</option>
                    <option value="true">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selFiltroStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="0" selected>Autorizados</option>
                    <option value="1">Cancelados</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridDocumentos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_documentos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_documentos">
    </div>
    <div class="clearfix"></div>
    
    <div class="modal fade" id="divEventoDesacordo" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Evento Desacordo Serviço</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-cadastroArquivo">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa*:
                                </span>
                                <input type="text" id="txtJustificativaEventoDesacordo" class="form-control" />
                            </div>
                        </div>       
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarEventoDesacordo" class="btn btn-primary">Salvar</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>