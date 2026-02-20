<%@ Page Title="Encerramento de MDF-e Manual" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EncerramentoManualMDFe.aspx.cs" Inherits="EmissaoCTe.WebApp.EmissaoMDFe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <style type="text/css">
        @media screen and (min-width: 1024px) {
            #divEncerramentoMDFe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media screen and (min-width: 1200px) {
            #divEncerramentoMDFe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }
    </style>
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
                           "~/bundle/scripts/encerramentoMDFe") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Encerramento de MDF-e Manual</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovoEncerramentoMDFe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Novo Encerramento de MDF-e</button>
    
    <div class="row" style="margin-top: 15px;"> 
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Chave MDFe:
                </span>
                <input type="text" id="txtChaveMDFe" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Protocolo MDFe:
                </span>
                <input type="text" id="txtProtocolo" class="form-control maskedInput" />
            </div>
        </div>
    </div>

    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros Adicionais
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicial:
                                </span>
                                <input type="text" id="txtDataEncerramentoInicial" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Data Final:
                                </span>
                                <input type="text" id="txtDataEncerramentoFinal" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarEncerramentos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar Encerramentos</button>
    <div id="tbl_encerramentos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_encerramentos">
    </div>
    <div class="clearfix" style="height: 400px;"></div>

    <div class="modal fade" id="divEncerramentoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Encerramento de MDF-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEncerramentoMDFe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Chave do MDF-e*:
                                </span>
                                <input type="text" id="txtEncerramentoChave" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Protocolo*:
                                </span>
                                <input type="text" id="txtEncerramentoProtocolo" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon"><abbr title="Data do Encerramento">Data do Enc.</abbr>*:
                                </span>
                                <input type="text" id="txtEncerramentoData" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon"><abbr title="Hora do Encerramento">Hora do Enc.</abbr>*:
                                </span>
                                <input type="text" id="txtEncerramentoHora" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Estado*:
                                </span>
                                <select id="selEncerramentoEstado" class="form-control"></select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Município*:
                                </span>
                                <select id="selEncerramentoMunicipio" class="form-control"></select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarEncerramentoMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Encerrar o MDF-e</button>
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
