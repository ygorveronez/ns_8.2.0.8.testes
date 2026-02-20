<%@ Page Title="Emissão de MDF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmissaoMDFe.aspx.cs" Inherits="EmissaoCTe.WebApp.EmissaoMDFe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <style type="text/css">
        body.cte-200 .hidecte-200,
        body.cte-400 .hidecte-400,
        body.mdfe-100 .hidemdfe-100,
        body.mdfe-300 .hidemdfe-300 {
            display: none !important;
            visibility: hidden !important;
            opacity: 0 !important;
        }

        @media screen and (min-width: 1024px) {
            #divEmissaoMDFe .modal-dialog {
                right: auto;
                width: 1000px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoMDFe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media screen and (min-width: 1200px) {
            #divEmissaoMDFe .modal-dialog {
                right: auto;
                width: 1180px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoMDFe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        /*TAGS*/
        div.tfs-tags {
            margin-top: 10px;
            margin-bottom: 6px;
            line-height: 25px;
            margin-left: 5px;
        }

        div.tags-label-container {
            float: left;
            padding-left: 4px;
            padding-right: 10px;
            font-size: 11px;
            color: #000;
        }

        div.tags-items-container {
            overflow: hidden;
        }

        .tags-items-container ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
            display: block;
            -webkit-margin-before: 1em;
            -webkit-margin-after: 1em;
            -webkit-margin-start: 0px;
            -webkit-margin-end: 0px;
            -webkit-padding-start: 0px;
        }

            .tags-items-container ul > li {
                display: inline-block;
                margin-right: 5px;
                padding: 0;
                text-align: -webkit-match-parent;
            }

        .tag-item-delete-experience {
            white-space: nowrap;
            overflow: hidden;
        }

        .tag-container-delete-experience {
            cursor: pointer;
        }

        .tag-container {
            outline: none;
            padding-top: 2px;
            padding-bottom: 2px;
            border: 1px solid #fff !important;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

        .tag-box, .tag-delete {
            cursor: default;
            margin: 0;
            padding-left: 6px;
            padding-top: 2px;
            padding-right: 6px;
            padding-bottom: 2px;
            font-size: 12px;
            color: #4f4f4f;
            background-color: #d7e6f3;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            font-family: Segoe UI,Tahoma,Arial,Verdana;
            font-size: 12px;
            border-radius: 2px 0 0 2px;
        }

        .tag-delete {
            padding-left: 9px;
            padding-right: 9px;
            background: url('images/icon-close-small.png') no-repeat 50% 50%;
            background-color: #d7e6f3;
            border-radius: 0 2px 2px 0;
        }

            .tag-delete:focus, .tag-delete:hover {
                cursor: pointer;
                color: #fff;
                background-color: #b4c8d7;
            }

        .tbl-municipios tbody tr .btns-municipios-descarregamento {
            vertical-align: middle;
            text-align: center;
            padding: 0;
        }

        .tbl-municipios tbody tr td button {
            padding: 2px 2px 5px 3px;
            margin-right: 2px;
            font-size: 15px;
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
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/emissaoMDFe") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Emissão de MDF-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovoMDFe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Novo MDF-e</button>

    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicialMDFeFiltro" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinalMDFeFiltro" class="form-control maskedInput" />
            </div>
        </div>
    </div>

    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicialMDFeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Final:
                                </span>
                                <input type="text" id="txtNumeroFinalMDFeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Série:
                                </span>
                                <select id="selSerieMDFeFiltro" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. CT-e:
                                </span>
                                <input type="text" id="txtNumeroCTe" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusMDFeFiltro" class="form-control">
                                    <option value="-1">Todos</option>
                                    <option value="3">Autorizado</option>
                                    <option value="7">Cancelado</option>
                                    <option value="6">Em Cancelamento</option>
                                    <option value="0">Em Digitação</option>
                                    <option value="4">Em Encerramento</option>
                                    <option value="5">Encerrado</option>
                                    <option value="2">Enviado</option>
                                    <option value="1">Pendente</option>
                                    <option value="11">Aguardando compra vale pedágio</option>
                                    <option value="9">Rejeição</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">UF Carga:
                                </span>
                                <select id="selUFCargaFiltro" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">UF Descarga:
                                </span>
                                <select id="selUFDescargaFiltro" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Placa do veículo tração">Veículo:</abbr>
                                </span>
                                <input type="text" id="txtPlacaVeiculoFiltro" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Placa do veículo reboque">Reboque:</abbr>
                                </span>
                                <input type="text" id="txtPlacaReboqueFiltro" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar MDF-e</button>
    <div id="tbl_mdfes" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_mdfes">
    </div>

    <div class="modal fade" id="divIntegracaoRetorno" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloIntegracaoRetorno" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderIntegracaoRetorno"></div>
                    <button type="button" id="btnConsultarIntegracaoRetorno" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_IntegracaoRetorno" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_IntegracaoRetorno">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharIntegracaoRetorno" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do CTe-e</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divIntegracaoRetornoLog" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloIntegracaoRetornoLog" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderIntegracaoRetornoLog"></div>
                    <div id="tbl_IntegracaoRetornoLog" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_IntegracaoRetornoLog">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharIntegracaoRetornoLog" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divRetornosSefaz" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloRetornosSefaz" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="tbl_retornossefaz" class="table-responsive">
                    </div>
                    <div id="tbl_paginacao_retornossefaz">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divAverbacaoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloAverbacaoMDFe" class="modal-title"></h4>
                </div>
                <div class="modal-body" style="min-height: 480px;">
                    <div id="placeholder-msgAverbacoes"></div>
                    <button type="button" id="btnConsultarAverbacoes" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_averbacao" class="table-responsive">
                    </div>
                    <div id="tbl_paginacao_averbacao">
                    </div>
                </div>
                <div class="clearfix">
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnReenviarAverbacao" class="btn btn-primary"><span class="glyphicon glyphicon-ok" visible="true"></span>&nbsp;Reenviar Averbação</button>
                    <button type="button" id="btnFecharAverbacoes" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do MDF-e</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCompraValePedagioMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloCompraValePedagioMDFe" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderCompraValePedagioMDFe"></div>
                    <button type="button" id="btnConsultarCompraValePedagio" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_compraValePedagio" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_compraValePedagio">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharCompraValePedagio" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do MDF-e</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCompraValePedagioMDFeLog" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloCompraValePedagioMDFeLog" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderCompraValePedagioMDFeLog"></div>
                    <div id="tbl_compraValePedagioLog" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_compraValePedagioLog">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharCompraValePedagioLog" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divSMViagemMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloSMViagemMDFe" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderSMViagemMDFe"></div>
                    <button type="button" id="btnConsultarSMViagemMDFe" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_SMViagemMDFe" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_SMViagemMDFe">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnEnviarSMViagemMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok" visible="true"></span>&nbsp;Enviar Integração</button>
                        <button type="button" id="btnFecharSMViagemMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do MDF-e</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divSMViagemMDFeLog" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloSMViagemMDFeLog" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderSMViagemMDFeLog"></div>
                    <div id="tbl_SMViagemMDFeLog" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_SMViagemMDFeLog">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharSMViagemMDFeLog" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>


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
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Encerramento*:
                                </span>
                                <input type="text" id="txtDataEncerramento" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Evento*:
                                </span>
                                <input type="text" id="txtDataEventoEncerramento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Estado*:
                                </span>
                                <input type="text" id="txtEstadoEncerramento" value="" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Município*:
                                </span>
                                <select id="selMunicipioEncerramento" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarEncerramentoMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Encerrar o MDF-e</button>
                    <button type="button" id="btnCancelarEncerramentoMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divInclusaoMotorista" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Inclusão Motorista</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgInclusaoMotorista"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Evento*:
                                </span>
                                <input type="text" id="txtDataEventoInclusao" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">CPF*:
                                </span>
                                <input type="text" id="txtCPFMotoristaInclusao" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotoristaInclusao" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Nome*:
                                </span>
                                <input type="text" id="txtNomeMotoristaInclusao" class="form-control" maxlength="60" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarInclusaoMotorista" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Incluir Motorista</button>
                    <button type="button" id="btnCancelarInclusaoMotorista" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCancelamentoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cancelamento de MDF-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgCancelamentoMDFe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataCancelamento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaCancelamentoMDFe" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row" id="divOpcoesCobrancaCancelamento" style="display: none">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Cobrar:</span>
                                <select id="selCobrarCancelamento" class="form-control">
                                    <option value="">Selecione</option>
                                    <option value="Sim">Sim</option>
                                    <option value="Nao">Não</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCancelamentoMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar o MDF-e</button>
                    <button type="button" id="btnCancelarCancelamentoMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divConsultaCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de CT-es</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgConsultaCTes"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicial:
                                </span>
                                <input type="text" id="txtDataEmissaoInicialCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Final:
                                </span>
                                <input type="text" id="txtDataEmissaoFinalCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Serviço:
                                </span>
                                <select id="selTipoServico" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="0">0 - Normal</option>
                                    <option value="1">1 - Subcontratação</option>
                                    <option value="2">2 - Redespacho</option>
                                    <option value="3">3 - Red. Intermediário</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicialCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Final:
                                </span>
                                <input type="text" id="txtNumeroFinalCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo:
                                </span>
                                <select id="selTipoCTe" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="0">0 - Normal</option>
                                    <option value="1">1 - Complemento</option>
                                    <option value="2">2 - Anulação</option>
                                    <option value="3">3 - Substituto</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Mot.:
                                </span>
                                <input type="text" id="txtNomeMotoristaCTeConsulta" class="form-control" maxlength="60" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF Mot.:
                                </span>
                                <input type="text" id="txtCPFMotoristaCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Placa:
                                </span>
                                <input type="text" id="txtPlacaVeiculoCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnBuscarCTesConsulta" class="btn btn-primary">Buscar</button>
                    <button type="button" id="btnSelecionarTodosOsCTes" class="btn btn-default pull-right disabled">Selecionar Todos</button>
                    <div id="tbl_ctes_consulta" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_ctes_consulta_paginacao">
                    </div>
                    <div class="clearfix"></div>
                    <div class="divCTesSelecionados">
                        <div class="tfs-tags">
                            <div class="tags-items-container">
                                <ul id="containerCTesSelecionados">
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnObterInformacoesCTesSelecionados" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Finalizar Seleção de CT-es</button>
                    <button type="button" id="btnCancelarConsultaAvancada" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDocumentosMunicipioDescarregamento" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" aria-hidden="true" id="btnFecharTelaDocumentosMunicipioDescarregamento">&times;</button>
                    <h4 class="modal-title">CT-es do Municipio de Descarregamento</h4>
                </div>
                <div class="modal-body" style="padding-top: 0;">
                    <h4 id="tituloMunicipioDescarregamento"></h4>
                    <div id="placeholder-msgDocumentosMunicipioDescarregamento"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">CT-e*:
                                </span>
                                <input type="text" id="txtCTeMunicipioDescarregamento" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarCTeMunicipioDescarregamento" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarDocumentoMunicipioDescarregamento" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirDocumentoMunicipioDescarregamento" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarDocumentoMunicipioDescarregamento" class="btn btn-default">Cancelar</button>
                    <div class="table-responsive" style="margin-top: 10px; height: 400px; overflow-y: scroll;">
                        <table id="tblDocumentosMunicipioDescarregamento" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 20%;">CT-e
                                    </th>
                                    <th style="width: 60%;">Valor Frete
                                    </th>
                                    <th style="width: 20%;">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="2">Nenhum registro encontrado!
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarAlteracoesDocumentosMunicipioDescarregamento" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Salvar Alterações</button>
                    <button type="button" id="btnCancelarAlteracoesDocumentosMunicipioDescarregamento" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Cancelar Alterações e Voltar à Emissão</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divProdutosPerigosos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Produtos Perigosos do CT-e <span id="spnCTeProdutosPerigosos"></span></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgProdutosPerigososCTe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-5 col-md-4">
                            <div class="input-group">
                                <span class="input-group-addon">Número ONU/UN*:
                                </span>
                                <input type="text" id="txtNumeroONU" class="form-control" maxlength="4" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Nome Apropriado Embarque Produto">Nome*</abbr>:
                                </span>
                                <input type="text" id="txtNomeApropriadoEmbarqueProduto" class="form-control" maxlength="150" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4">
                            <div class="input-group">
                                <span class="input-group-addon">Classe Risco*:
                                </span>
                                <input type="text" id="txtClasseRisco" class="form-control" maxlength="40" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4">
                            <div class="input-group">
                                <span class="input-group-addon">Grupo Embalagem:
                                </span>
                                <input type="text" id="txtGrupoEmbalagem" class="form-control" maxlength="6" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Quantidade Total por Produto">Quant. Prod.:</abbr>
                                </span>
                                <input type="text" id="txtQuantidadeTotalPorProduto" class="form-control" maxlength="20" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Quantidade e Tipo de Volumes">Quant. Vol.:</abbr>
                                </span>
                                <input type="text" id="txtQuantidadeETipoDeVolumes" class="form-control" maxlength="60" autocomplete="off">
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarProdutoPerigoso" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirProdutoPerigoso" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarProdutoPerigoso" class="btn btn-default">Cancelar</button>
                    <div class="table-responsive" style="margin-top: 10px; height: 400px; overflow-y: scroll;">
                        <table id="tblProdutosPerigosos" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 10%;">Núm. ONU</th>
                                    <th style="width: 15%;">Nome Apropriado</th>
                                    <th style="width: 13%;">Classe de Risco</th>
                                    <th style="width: 14%;">Grupo de Embalagem</th>
                                    <th style="width: 14%;">Quantidade Total</th>
                                    <th style="width: 14%;">Quantidade e Tipo</th>
                                    <th style="width: 10%;">Opções</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarAlteracoesProdutoPerigoso" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Salvar Alterações</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span>&nbsp;Cancelar e Voltar à Emissão</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divNFesMunicipioDescarregamento" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" aria-hidden="true" id="btnFecharTelaNFesMunicipioDescarregamento">&times;</button>
                    <h4 class="modal-title">NF-es do Municipio de Descarregamento</h4>
                </div>
                <div class="modal-body" style="padding-top: 0;">
                    <h4 id="tituloMunicipioDescarregamentoNFe"></h4>
                    <div id="placeholder-msgNFesMunicipioDescarregamento"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">NF-e*:
                                </span>
                                <input type="text" id="txtChaveNFeMunicipioDescarregamento" class="form-control" />
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Segundo Código De Barra">Seg Cód Barra</abbr>:
                                </span>
                                <input type="text" id="txtSegCodBarraNFeMunicipioDescarregamento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarNFeMunicipioDescarregamento" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirNFeMunicipioDescarregamento" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarNFeMunicipioDescarregamento" class="btn btn-default">Cancelar</button>
                    <button type="button" id="btnImportarDocumentoMunicipioDescarregamento" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>Importar NF-es</button>
                    <div class="table-responsive" style="margin-top: 10px; height: 400px; overflow-y: scroll;">
                        <table id="tblNFesMunicipioDescarregamento" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 90%;">Chave
                                    </th>
                                    <th style="width: 10%;">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="2">Nenhum registro encontrado!
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarAlteracoesNFesMunicipioDescarregamento" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Salvar Alterações</button>
                    <button type="button" id="btnCancelarAlteracoesNFesMunicipioDescarregamento" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Cancelar Alterações e Voltar à Emissão</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divChaveCTesMunicipioDescarregamento" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" aria-hidden="true" id="btnFecharTelaChaveCTesMunicipioDescarregamento">&times;</button>
                    <h4 class="modal-title">Chave CT-es do Municipio de Descarregamento</h4>
                </div>
                <div class="modal-body" style="padding-top: 0;">
                    <h4 id="tituloMunicipioDescarregamentoChaveCTe"></h4>
                    <div id="placeholder-msgChaveCTesMunicipioDescarregamento"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Chave CT-e*:
                                </span>
                                <input type="text" id="txtChaveCTeMunicipioDescarregamento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarChaveCTeMunicipioDescarregamento" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirChaveCTeMunicipioDescarregamento" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarChaveCTeMunicipioDescarregamento" class="btn btn-default">Cancelar</button>
                    <div class="table-responsive" style="margin-top: 10px; height: 400px; overflow-y: scroll;">
                        <table id="tblChaveCTesMunicipioDescarregamento" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 80%;">Chave
                                    </th>
                                    <th style="width: 20%;">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="2">Nenhum registro encontrado!
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarAlteracoesChaveCTesMunicipioDescarregamento" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Salvar Alterações</button>
                    <button type="button" id="btnCancelarAlteracoesChaveCTesMunicipioDescarregamento" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Cancelar Alterações e Voltar à Emissão</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de MDF-e<span id="spnLabelVersao"></span></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEmissaoMDFe"></div>
                    <ul class="nav nav-tabs" id="tabsEmissaoMDFe">
                        <li class="active"><a href="#tabDados" data-toggle="tab">Dados</a></li>
                        <li><a href="#tabRodoviario" tabindex="-1" data-toggle="tab">Rodoviário</a></li>
                        <li><a href="#tabTotais" tabindex="-1" data-toggle="tab">Totais</a></li>
                        <li><a href="#tabObservacoes" tabindex="-1" data-toggle="tab">Observações</a></li>
                        <li><a href="#tabCancelamento" tabindex="-1" data-toggle="tab">Cancelamento</a></li>
                    </ul>
                    <div class="tab-content" style="margin-top: 10px;">
                        <div class="tab-pane active" id="tabDados">
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número:
                                        </span>
                                        <input type="text" id="txtNumero" value="Automático" class="form-control" disabled />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série:
                                        </span>
                                        <select id="selSerie" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Emissão*:
                                        </span>
                                        <input type="text" id="txtDataEmissao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Hora Emissão*:
                                        </span>
                                        <input type="text" id="txtHoraEmissao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Modal*:
                                        </span>
                                        <select id="selModal" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tipo Emitente*:
                                        </span>
                                        <select id="selTipoEmitente" class="form-control">
                                            <option value="1">Prestador de Serviço de Transporte</option>
                                            <option value="2">Não Prestador de Serviço de Transporte</option>
                                            <option value="3">Prestador de Serviço de Transporte CTe Globalizado</option>
                                            <option value="9">Prestador de Serviço de Transporte (Apenas chave CT-e)</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">UF Carga*:
                                        </span>
                                        <select id="selUFCarregamento" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">UF Descarga*:
                                        </span>
                                        <select id="selUFDescarregamento" class="form-control">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <ul class="nav nav-tabs" id="tabsDados">
                                <li class="active"><a href="#tabCarregamentoEDescarregamento" data-toggle="tab">Carregamento e Descarregamento</a></li>
                                <li><a href="#tabPercurso" tabindex="-1" data-toggle="tab">Percurso</a></li>
                                <li><a href="#tabLacres" tabindex="-1" data-toggle="tab">Lacres</a></li>
                            </ul>
                            <div class="tab-content" style="margin-top: 10px;">
                                <div class="tab-pane active" id="tabCarregamentoEDescarregamento">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">Municípios de Carregamento</div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Mun.*:
                                                                </span>
                                                                <select id="selMunicipioCarregamento" class="form-control">
                                                                </select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <button type="button" id="btnSalvarMunicipioCarregamento" class="btn btn-primary">Salvar</button>
                                                    <button type="button" id="btnExcluirMunicipioCarregamento" class="btn btn-danger" style="display: none;">Excluir</button>
                                                    <button type="button" id="btnCancelarMunicipioCarregamento" class="btn btn-default">Cancelar</button>
                                                    <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                                        <table id="tblMunicipiosCarregamento" class="table table-bordered table-condensed table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 80%;">Município
                                                                    </th>
                                                                    <th style="width: 20%;">Opções
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td colspan="2">Nenhum registro encontrado!
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">Municípios de Descarregamento</div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Mun.*:
                                                                </span>
                                                                <select id="selMunicipioDescarregamento" class="form-control">
                                                                </select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <button type="button" id="btnSalvarMunicipioDescarregamento" class="btn btn-primary">Salvar</button>
                                                    <button type="button" id="btnExcluirMunicipioDescarregamento" class="btn btn-danger" style="display: none;">Excluir</button>
                                                    <button type="button" id="btnCancelarMunicipioDescarregamento" class="btn btn-default">Cancelar</button>
                                                    <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                                        <table id="tblMunicipiosDescarregamento" class="tbl-municipios table table-bordered table-condensed table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 80%;">Município
                                                                    </th>
                                                                    <th style="width: 20%;">Opções
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td colspan="2">Nenhum registro encontrado!
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabPercurso">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">Estado*:
                                                </span>
                                                <select id="selUFPercurso" class="form-control">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data prevista de início da viagem">Data</abbr>:
                                                </span>
                                                <input type="text" id="txtDataPercurso" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Hora prevista de início da viagem">Hora</abbr>:
                                                </span>
                                                <input type="text" id="txtHoraPercurso" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarPercurso" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirPercurso" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarPercurso" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblPercurso" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 65%;">Estado
                                                    </th>
                                                    <th style="width: 20%;">Data Prev.
                                                    </th>
                                                    <th style="width: 15%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabLacres">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">Número*:
                                                </span>
                                                <input type="text" id="txtNumeroLacre" maxlength="60" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarLacre" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirLacre" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarLacre" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblLacres" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 85%;">Número
                                                    </th>
                                                    <th style="width: 15%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="2">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabRodoviario">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">RNTRC*:
                                        </span>
                                        <input type="text" id="txtRNTRC" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 hidemdfe-300">
                                    <div class="input-group">
                                        <span class="input-group-addon">CIOT:
                                        </span>
                                        <input type="text" id="txtCIOT" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <ul class="nav nav-tabs" id="tabsRodoviario">
                                <li class="active"><a href="#tabVeiculo" data-toggle="tab">Veículo</a></li>
                                <li><a href="#tabReboques" tabindex="-1" data-toggle="tab">Reboques</a></li>
                                <li><a href="#tabMotoristas" tabindex="-1" data-toggle="tab">Motoristas</a></li>
                                <li class="hidemdfe-100"><a href="#tabContratantes" tabindex="-1" data-toggle="tab">Contratantes</a></li>
                                <li class="hidemdfe-100"><a href="#tabCIOT" tabindex="-1" data-toggle="tab">CIOT</a></li>
                                <li class="hidemdfe-100"><a href="#tabSeguros" tabindex="-1" data-toggle="tab">Seguros</a></li>
                                <li><a href="#tabValePedagio" tabindex="-1" data-toggle="tab">Vale Pedágio</a></li>
                                <li><a href="#tabLotacao" tabindex="-1" data-toggle="tab">Lotação</a></li>
                                <li class="hidemdfe-100"><a href="#tabInfPagamento" tabindex="-1" data-toggle="tab">Informações de Pagamento do Frete</a></li>
                            </ul>
                            <div class="tab-content" style="margin-top: 10px;">
                                <div class="tab-pane active" id="tabVeiculo">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Placa*:
                                                </span>
                                                <input type="text" id="txtPlacaVeiculo" class="form-control" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">RENAVAM:
                                                </span>
                                                <input type="text" id="txtRENAVAMVeiculo" class="form-control" maxlength="11" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Tara KG*:
                                                </span>
                                                <input type="text" id="txtTaraVeiculo" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Cap. KG:
                                                </span>
                                                <input type="text" id="txtCapacidadeKGVeiculo" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Cap. M3:
                                                </span>
                                                <input type="text" id="txtCapacidadeM3Veiculo" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Rodado*:
                                                </span>
                                                <select id="selRodadoVeiculo" class="form-control">
                                                    <option value="01">Truck</option>
                                                    <option value="02">Toco</option>
                                                    <option value="03">Cavalo Mecânico</option>
                                                    <option value="04">VAN</option>
                                                    <option value="05">Utilitário</option>
                                                    <option value="06">Outros</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Carroceria*:
                                                </span>
                                                <select id="selCarroceriaVeiculo" class="form-control">
                                                    <option value="00">Não aplicável</option>
                                                    <option value="01">Aberta</option>
                                                    <option value="02">Fechada/Baú</option>
                                                    <option value="03">Graneleira</option>
                                                    <option value="04">Porta Container</option>
                                                    <option value="05">Sider</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                                            <div class="input-group">
                                                <span class="input-group-addon">UF*:
                                                </span>
                                                <select id="selUFVeiculo" class="form-control">
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title">Proprietário (somente para veículo de terceiros)</h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF/CNPJ*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJProprietarioVeiculo" class="form-control" maxlength="14" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IE:
                                                        </span>
                                                        <input type="text" id="txtIEProprietarioVeiculo" class="form-control" maxlength="14" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">RNTRC*:
                                                        </span>
                                                        <input type="text" id="txtRNTRCVeiculo" class="form-control" value="0" maxlength="8" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome*:
                                                        </span>
                                                        <input type="text" id="txtNomeProprietarioVeiculo" class="form-control" maxlength="60" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:
                                                        </span>
                                                        <select id="selUFProprietarioVeiculo" class="form-control">
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo*:
                                                        </span>
                                                        <select id="selTipoProprietarioVeiculo" class="form-control">
                                                            <option value="0">TAC Agregado</option>
                                                            <option value="1">TAC Independente</option>
                                                            <option value="2">Outros</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabReboques">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Placa*:
                                                </span>
                                                <input type="text" id="txtPlacaReboque" class="form-control" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarReboque" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">RENAVAM:
                                                </span>
                                                <input type="text" id="txtRENAVAMReboque" class="form-control" maxlength="11" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Tara KG*:
                                                </span>
                                                <input type="text" id="txtTaraKGReboque" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Cap. KG:
                                                </span>
                                                <input type="text" id="txtCapacidadeKGReboque" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Cap. M3:
                                                </span>
                                                <input type="text" id="txtCapacidadeM3Reboque" class="form-control" value="0" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Carroceria*:
                                                </span>
                                                <select id="selCarroceriaReboque" class="form-control">
                                                    <option value="00">Não aplicável</option>
                                                    <option value="01">Aberta</option>
                                                    <option value="02">Fechada/Baú</option>
                                                    <option value="03">Graneleira</option>
                                                    <option value="04">Porta Container</option>
                                                    <option value="05">Sider</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                                            <div class="input-group">
                                                <span class="input-group-addon">UF*:
                                                </span>
                                                <select id="selUFReboque" class="form-control">
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title">Proprietário (somente para reboque de terceiros)</h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF/CNPJ*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJProprietarioReboque" class="form-control" maxlength="14" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IE:
                                                        </span>
                                                        <input type="text" id="txtIEProprietarioReboque" class="form-control" maxlength="14" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">RNTRC*:
                                                        </span>
                                                        <input type="text" id="txtRNTRCReboque" class="form-control" value="0" maxlength="8" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome*:
                                                        </span>
                                                        <input type="text" id="txtNomeProprietarioReboque" class="form-control" maxlength="60" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:
                                                        </span>
                                                        <select id="selUFProprietarioReboque" class="form-control">
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo*:
                                                        </span>
                                                        <select id="selTipoProprietarioReboque" class="form-control">
                                                            <option value="0">TAC Agregado</option>
                                                            <option value="1">TAC Independente</option>
                                                            <option value="2">Outros</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarReboque" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirReboque" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarReboque" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblReboques" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 15%;">Placa
                                                    </th>
                                                    <th style="width: 15%;">RENAVAM
                                                    </th>
                                                    <th style="width: 15%;">Tara
                                                    </th>
                                                    <th style="width: 12%;">Cap. KG
                                                    </th>
                                                    <th style="width: 12%;">Cap. M3
                                                    </th>
                                                    <th style="width: 15%;">RNTRC
                                                    </th>
                                                    <th style="width: 16%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="6">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabMotoristas">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">CPF*:
                                                </span>
                                                <input type="text" id="txtCPFMotorista" class="form-control" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Nome*:
                                                </span>
                                                <input type="text" id="txtNomeMotorista" class="form-control" maxlength="60" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarMotorista" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirMotorista" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarMotorista" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblMotoristas" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 20%;">CPF
                                                    </th>
                                                    <th style="width: 65%;">Nome
                                                    </th>
                                                    <th style="width: 15%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabContratantes">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">Cliente:
                                                </span>
                                                <input type="text" id="txtCNPJTomador" class="form-control" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="margin-top: 15px">
                                        <button type="button" id="btnSalvarTomador" class="btn btn-primary">Salvar</button>
                                        <button type="button" id="btnCancelarTomador" class="btn btn-default">Cancelar</button>
                                    </div>
                                    <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblTomadores" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th>Nome</th>
                                                    <th>CNPJ</th>
                                                    <th>Opções</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado!</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabCIOT">
                                    <div id="placeholder-validacao-ciot"></div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-5">
                                            <div class="input-group">
                                                <span class="input-group-addon">Número do CIOT:
                                                </span>
                                                <input type="text" id="txtNumeroCIOT" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-5">
                                            <div class="input-group">
                                                <span class="input-group-addon">CNPJ Responsável:
                                                </span>
                                                <input type="text" id="txtCNPJResponsavelCIOT" class="form-control" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarResponsavel" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="margin-top: 15px">
                                        <button type="button" id="btnSalvarCIOT" class="btn btn-primary">Salvar</button>
                                        <button type="button" id="btnCancelarCIOT" class="btn btn-default">Cancelar</button>
                                        <button type="button" id="btnExcluirCIOT" class="btn btn-danger" style="display: none">Excluir</button>
                                    </div>
                                    <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblCIOT" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 40%">CIOT</th>
                                                    <th style="width: 40%">Responsável</th>
                                                    <th style="width: 20%">Opções</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado!</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabSeguros">
                                    <div id="placeholder-validacao-seguro"></div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo do responsável do seguro">Tipo</abbr>*:
                                                </span>
                                                <select id="selResponsavelSeguro" class="form-control">
                                                    <option value="1">1 - Emitente</option>
                                                    <option value="2">2 - Contratante</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="O CNPJ da Seguradora é obrigatório quando o tipo do seguro é contratante">CNPJ Seguradora</abbr>*:</span>
                                                <input type="text" id="txtCNPJSeguradora" class="form-control maskedInput" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Nome da Seguradora">Seguradora</abbr>*:
                                                </span>
                                                <input type="text" id="txtNomeSeguradora" class="form-control" maxlength="30" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="CPF/CNPJ do Emitente do MDF-e (1) ou Responsável do contratante (2)">Responsável</abbr>*:</span>
                                                <input type="text" id="txtCNPJResponsavelSeguro" class="form-control" maxlength="18" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Apólice*:
                                                </span>
                                                <input type="text" id="txtNumeroApolice" class="form-control" maxlength="20" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Averbação:
                                                </span>
                                                <input type="text" id="txtNumeroAverbacao" class="form-control" maxlength="40" autocomplete="off">
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-2 col-lg-2">
                                            <button type="button" id="btnBuscarApoliceSeguro" class="btn btn-primary">Buscar Apólice</button>
                                        </div>
                                    </div>
                                    <div style="margin-top: 15px">
                                        <button type="button" id="btnSalvarInformacaoSeguro" class="btn btn-primary">Salvar</button>
                                        <button type="button" id="btnExcluirInformacaoSeguro" class="btn btn-danger" style="display: none;">Excluir</button>
                                        <button type="button" id="btnCancelarInformacaoSeguro" class="btn btn-default">Cancelar</button>
                                    </div>
                                    <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblInformacaoSeguro" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th>Tipo</th>
                                                    <th><span class="hidden-xs hidden-sm">CPF/CNPJ </span>Responsável</th>
                                                    <th>CNPJ Seguradora</th>
                                                    <th>Seguradora</th>
                                                    <th><span class="hidden-xs hidden-sm">Nº da </span>Apólice</th>
                                                    <th><span class="hidden-xs hidden-sm">Nº da </span>Averbação</th>
                                                    <th>Opções</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="6">Nenhum registro encontrado!</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabValePedagio">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="CNPJ da empresa fornecedora do Vale-Pedágio">CNPJ Forn.</abbr>*:
                                                </span>
                                                <input type="text" id="txtCNPJFornecedorValePedagio" class="form-control" maxlength="14" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarFornecedorValePedagio" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="CNPJ do responsável pelo pagamento do Vale-Pedágio">CNPJ Resp.</abbr>:
                                                </span>
                                                <input type="text" id="txtCNPJResponsavelValePedagio" class="form-control" maxlength="14" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarResponsavelValePedagio" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Número do comprovante de compra">Nº Comp.</abbr>*:
                                                </span>
                                                <input type="text" id="txtNumeroComprovanteValePedagio" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-2 col-lg-2 hidemdfe-300">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Código de agendamento no porto">Ag. Porto</abbr>:
                                                </span>
                                                <input type="text" id="txtCodigoAgendamentoPortoValePedagio" class="form-control" maxlength="16" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo de compra">Tipo</abbr>*:
                                                </span>
                                                <select id="selTipoCompraValePedagio" class="form-control">
                                                    <option value="2">Tag</option>
                                                    <option value="1">Cartão</option>
                                                    <option value="3">Cupom</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 hidemdfe-100">
                                            <div class="input-group">
                                                <span class="input-group-addon">Valor:
                                                </span>
                                                <input type="text" id="txtValorValePedagio" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Quantidade Eixps totais">Qtd. Eixos</abbr>*:
                                                </span>
                                                <input type="text" id="txtQuantidadeEixosValePedagio" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarValePedagio" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirValePedagio" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarValePedagio" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblValePedagio" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 28%;">CNPJ Fornecedor</th>
                                                    <th style="width: 28%;">CNPJ Responsável</th>
                                                    <th style="width: 28%;">Número Comprovante</th>
                                                    <th style="width: 28%;" class="hidemdfe-100">Valor</th>
                                                    <th style="width: 16%;">Opções</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="4">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="tab-pane" id="tabLotacao">
                                    <div id="placeholder-validacao-lotacao"></div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">CEP Carregamento
                                                </span>
                                                <input type="text" id="txtCEPCarregamento" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Latitude Carregamento
                                                </span>
                                                <input type="text" id="txtLatitudeCarregamento" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Longitude Carregamento
                                                </span>
                                                <input type="text" id="txtLongitudeCarregamento" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">CEP Descarregamento
                                                </span>
                                                <input type="text" id="txtCEPDescarregamento" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Latitude Descarregamento
                                                </span>
                                                <input type="text" id="txtLatitudeDescarregamento" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Longitude Descarregamento
                                                </span>
                                                <input type="text" id="txtLongitudeDescarregamento" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>




                                <div class="tab-pane" id="tabInfPagamento">
                                    <div id="placeholder-validacao-inf-pagamento"></div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Forma de Pagamento*:
                                                </span>
                                                <select id="selFormaPagamento" class="form-control" disabled>
                                                    <option value="0">À Vista</option>
                                                    <option value="1">À Prazo</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Valor do Adiantamento:
                                                </span>
                                                <input type="text" id="txtValorDoAdiantamento" value="0,00" class="form-control" autocomplete="off" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Inf. Bancária*:
                                                </span>
                                                <select id="selInformacoesBancarias" class="form-control">
                                                    <option value="1">Chave PIX</option>
                                                    <option value="2">Banco/Agência</option>
                                                    <option value="3">IPEF</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 row row-banco">
                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Nº Banco:</span>
                                                    <input type="text" id="txtNumeroBanco" class="form-control" autocomplete="off" />
                                                </div>
                                            </div>
                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Agência:</span>
                                                    <input type="text" id="txtAgencia" class="form-control" autocomplete="off" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 row-pix">
                                            <div class="input-group">
                                                <span class="input-group-addon">Chave Pix:</span>
                                                <input type="text" id="txtChavePix" class="form-control" autocomplete="off" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6 row-ipef">
                                            <div class="input-group">
                                                <span class="input-group-addon">Inst. de Pagamento Eletrônico de Frete:</span>
                                                <input type="text" id="txtIPEF" class="form-control" autocomplete="off" />
                                            </div>
                                         </div>
                                    </div>

                                    <ul class="nav nav-tabs" id="tabsInfPagamento">
                                        <li id="li-tab-componentes" class="active"><a href="#tabInfPagamentoComponente" data-toggle="tab">Componentes</a></li>
                                        <li id="li-tab-parcelas"><a href="#tabInfPagamentoParcela" data-toggle="tab">Parcelas</a></li>
                                    </ul>
                                    <div class="tab-content" style="margin-top: 10px;">
                                        <div class="tab-pane active" id="tabInfPagamentoComponente">
                                            <div id="placeholder-validacao-componentes"></div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo*:
                                                        </span>
                                                        <select id="txtTipoInfPgto" class="form-control">
                                                            <option value="" disabled selected>Selecione</option>
                                                            <option value="1">Vale Pedagio</option>
                                                            <option value="2">Impostos</option>
                                                            <option value="3">Despesas</option>
                                                            <option value="4">Frete</option>
                                                            <option value="5">Outros</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor Total*:
                                                        </span>
                                                        <input type="text" id="txtValorDoPagamento" value="0,00" class="form-control" autocomplete="off" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div style="margin-top: 15px">
                                                <button type="button" id="btnSalvarComponente" class="btn btn-primary">Salvar</button>
                                                <button type="button" id="btnCancelarComponente" class="btn btn-default">Cancelar</button>
                                                <button type="button" id="btnExcluirComponente" class="btn btn-danger" style="display: none">Excluir</button>
                                            </div>
                                            <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                                <table id="tblComponente" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 40%">Tipo</th>
                                                            <th style="width: 40%">Valor Total</th>
                                                            <th style="width: 20%">Opções</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="3">Nenhum registro encontrado!</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        
                                        <div class="tab-pane" id="tabInfPagamentoParcela">
                                            <div id="placeholder-validacao-inf-pagamento-parcelas"></div>
                                            <div class="row row-parcela">
                                                <div class="col-xs-12 col-sm-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Parcela:
                                                        </span>
                                                        <input type="text" id="txtNumeroDaParcela" class="form-control" autocomplete="off" disabled />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Data de Vencimento:
                                                        </span>
                                                        <input type="text" id="txtVencimentoDaParcela" class="form-control" autocomplete="off" disabled />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor da Parcela:
                                                        </span>
                                                        <input type="text" id="txtValorDaParcela" value="0,00" class="form-control" autocomplete="off" disabled />
                                                    </div>
                                                </div>
                                            </div>
                                            <div style="margin-top: 15px">
                                                <button type="button" id="btnSalvarParcela" class="btn btn-primary">Salvar</button>
                                                <button type="button" id="btnCancelarParcela" class="btn btn-default">Cancelar</button>
                                                <button type="button" id="btnExcluirParcela" class="btn btn-danger" style="display: none">Excluir</button>
                                                <button type="button" id="btnRecalcularParcela" class="btn btn-info" style="display: none">Recalcular</button>
                                            </div>
                                            <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                                <table id="tblParcela" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 30%">Nº Parcela</th>
                                                            <th style="width: 30%">Data Vencimento</th>
                                                            <th style="width: 30%">Valor Parcela</th>
                                                            <th style="width: 10%">Opções</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="3">Nenhum registro encontrado!</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>




                            </div>
                        </div>
                        <div class="tab-pane" id="tabTotais">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tipo Carga*:
                                        </span>
                                        <select id="selTipoCarga" class="form-control">
                                            <option value="5">Carga Geral</option>
                                            <option value="1">Granel Solido</option>
                                            <option value="2">Granel Liquido</option>
                                            <option value="3">Frigorificada</option>
                                            <option value="4">Conteinerizada</option>
                                            <option value="6">Neogranel</option>
                                            <option value="7">Perigosa Granel Solido</option>
                                            <option value="8">Perigosa Granel Liquido</option>
                                            <option value="9">Perigosa Frigorificada</option>
                                            <option value="10">Perigosa Conteinerizada</option>
                                            <option value="11">Perigosa Carga Geral</option>
                                            <option value="12">Granel Pressurizada</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Produto Pred.*:
                                        </span>
                                        <input type="text" id="txtProdutoPredominanteDescricao" class="form-control" maxlength="120" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEAN:
                                        </span>
                                        <input type="text" id="txtProdutoPredominanteCEAN" class="form-control" maxlength="14" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">NCM*:
                                        </span>
                                        <input type="text" id="txtProdutoPredominanteNCM" class="form-control" maxlength="8" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">U.M.*:
                                        </span>
                                        <select id="selUnidadeMedida" class="form-control">
                                            <option value="1">KG</option>
                                            <option value="2">TON</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Peso Bruto Total da Carga / Mercadoria Transportada">Peso Bruto</abbr>*:
                                        </span>
                                        <input type="text" id="txtPesoBruto" value="0,0000" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor Total da Mercadoria / Carga Transportada">Valor Carga</abbr>*:
                                        </span>
                                        <input type="text" id="txtValorTotal" value="0,00" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabObservacoes">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Informações adicionais de interesse do Fisco">Obs. Fisco</abbr>:
                                        </span>
                                        <textarea id="txtObservacaoFisco" class="form-control" rows="4" maxlength="2000"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Informações complementares de interesse do Contribuinte">Obs. Contr.</abbr>:
                                        </span>
                                        <textarea id="txtObservacaoContribuinte" class="form-control" rows="4" maxlength="5000"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabCancelamento">
                            <div class="row">
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data:
                                        </span>
                                        <input type="text" id="txtDataCancelamentoMDFee" class="form-control" disabled />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">Protocolo:
                                        </span>
                                        <input type="text" id="txtProtocoloCancelamentoMDFee" class="form-control" disabled />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">Justificativa:
                                        </span>
                                        <input type="text" id="txtJustificativaCancelamentoMDFee" class="form-control" disabled />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <span id="lblLogMDFe" class="text-info"></span>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir MDF-e</button>
                    <button type="button" id="btnSalvarMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-floppy-disk"></span>&nbsp;Salvar MDF-e</button>
                    <button type="button" id="btnCancelarMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divUploadArquivos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloUploadArquivos" class="modal-title"></h4>
                </div>
                <div class="modal-body" id="divUploadArquivosBody">
                    Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                </div>
            </div>
        </div>
    </div>
</asp:Content>
