<%@ Page Title="Encerramento de MDF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EncerramentoMDFe.aspx.cs" Inherits="EmissaoCTe.WebApp.EncerramentoMDFe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <style type="text/css">
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
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            ObterSeries();
            ObterEstados();
            ObterDadosEmpresa();

            FormatarCampoDateTime("txtDataEncerramento");
            FormatarCampoDateTime("txtDataEventoEncerramento");

            FormatarCampoDate("txtDataEmissaoInicialMDFeFiltro");
            FormatarCampoDate("txtDataEmissaoFinalMDFeFiltro");

            $("#txtPlacaVeiculoFiltro").mask("*******");

            $("#txtNumeroInicialMDFeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
            $("#txtNumeroFinalMDFeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });

            $("#txtLacreEdiFiscal").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });

            $("#btnConsultarMDFe").click(function () {
                ConsultarMDFes();
            });

            $("#btnSalvarEncerramentoMDFe").click(function () {
                FinalizarEncerramentoMDFe();
            });

            $("#btnCancelarEncerramentoMDFe").click(function () {
                FecharTelaEncerramentoMDFe();
            });

            $("#btnGerarEDIFiscal").click(function () {
                BaixarEDIFiscal();
            });

            $("#btnCancelarEDIFiscal").click(function () {
                FecharTelaEDIFiscal();
            });
            
            // Coloca filtro de data antes de buscar os MDFes
            var today = new Date();
            var yesterday = new Date(today);
            var tomorrow = new Date(today);
            yesterday.setDate(today.getDate() - 1);
            tomorrow.setDate(today.getDate() + 1);

            $("#txtDataEmissaoInicialMDFeFiltro").val(Globalize.format(yesterday, "dd/MM/yyyy"));
            $("#txtDataEmissaoFinalMDFeFiltro").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

            ConsultarMDFes();
        });

        function ObterDadosEmpresa() {
            executarRest("/Empresa/ObterDetalhesDaEmpresaDoUsuario?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    $("body").data("empresa", r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ObterEstados() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    var selUFCargaFiltro = document.getElementById("selUFCargaFiltro");
                    var selUFDescargaFiltro = document.getElementById("selUFDescargaFiltro");

                    selUFCargaFiltro.options.length = 0;
                    selUFDescargaFiltro.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todos";
                    optnTodos.value = "";

                    selUFCargaFiltro.options.add(optnTodos);
                    selUFDescargaFiltro.options.add(optnTodos.cloneNode(true));

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Nome;
                        optn.value = r.Objeto[i].Sigla;

                        selUFCargaFiltro.options.add(optn.cloneNode(true));
                        selUFDescargaFiltro.options.add(optn.cloneNode(true));
                    }

                    $("#selUFCargaFiltro").val("");
                    $("#selUFDescargaFiltro").val("");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ObterSeries() {
            executarRest("/Usuario/ObterSeriesDoUsuario?callback=?", { Tipo: 1 }, function (r) {
                if (r.Sucesso) {

                    var selSerieMDFeFiltro = document.getElementById("selSerieMDFeFiltro");
                    selSerieMDFeFiltro.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todas";
                    optnTodos.value = "";

                    selSerieMDFeFiltro.options.add(optnTodos);

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Numero;
                        optn.value = r.Objeto[i].Codigo;

                        selSerieMDFeFiltro.add(optn.cloneNode(true));
                    }

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function VoltarAoTopoDaTela() {
            $("html, body").animate({ scrollTop: 0 }, "slow");
        }

        function ConsultarMDFes() {
            var dados = {
                DataEmissaoInicial: $("#txtDataEmissaoInicialMDFeFiltro").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinalMDFeFiltro").val(),
                NumeroInicial: $("#txtNumeroInicialMDFeFiltro").val(),
                NumeroFinal: $("#txtNumeroFinalMDFeFiltro").val(),
                Serie: $("#selSerieMDFeFiltro").val(),
                Status: $("#selStatusMDFeFiltro").val(),
                UFCarregamento: $("#selUFCargaFiltro").val(),
                UFDescarregamento: $("#selUFDescargaFiltro").val(),
                Placa: $("#txtPlacaVeiculoFiltro").val(),
                inicioRegistros: 0
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Encerrar", Evento: EncerrarMDFe });
            opcoes.push({ Descricao: "DAMDFE", Evento: BaixarDAMDFE });
            opcoes.push({ Descricao: "XML Autorização", Evento: BaixarXML });
            opcoes.push({ Descricao: "XML Cancelamento", Evento: BaixarXMLCancelamento });
            opcoes.push({ Descricao: "XML Encerramento", Evento: BaixarXMLEncerramento });
            opcoes.push({ Descricao: "EDI Fiscal", Evento: AbrirTelaEDIFiscal });

            CriarGridView("/ManifestoEletronicoDeDocumentosFiscais/Consultar?callback=?", dados, "tbl_mdfes_table", "tbl_mdfes", "tbl_paginacao_mdfes", opcoes, [0, 1], null);
        }

        function FinalizarEncerramentoMDFe() {
            if (ValidarCamposEncerramentoMDFe()) {
                executarRest("/ManifestoEletronicoDeDocumentosFiscais/Encerrar?callback=?", {
                    CodigoMDFe: $("body").data("mdfeEncerramento").Codigo,
                    CodigoMunicipio: $("#selMunicipioEncerramento").val(),
                    DataEncerramento: $("#txtDataEncerramento").val(),
                    DataEvento: $("#txtDataEventoEncerramento").val()
                }, function (r) {
                    if (r.Sucesso) {
                        jAlert("O MDF-e está em processo de encerramento.", "Atenção", function () {
                            FecharTelaEncerramentoMDFe();
                            ConsultarMDFes();
                        });
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgEncerramentoMDFe");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com um asterísco (*) são obrigatórios.", "Atenção!", "placeholder-msgEncerramentoMDFe");
            }
        }

        function FecharTelaEncerramentoMDFe() {
            LimparCamposEncerramentoMDFe();
            $("body").data("mdfeEncerramento", null);
            $("#divEncerramentoMDFe").modal('hide');
            VoltarAoTopoDaTela();
        }

        function EncerrarMDFe(mdfe) {
            if (mdfe.data.Status == 3 || mdfe.data.Status == 4) {
                $("body").data("mdfeEncerramento", mdfe.data);
                AbrirTelaEncerramentoMDFe();
                ObterInformacoesEncerramento();
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para encerrar o mesmo.", "Atenção!");
            }
        }

        function AbrirTelaEncerramentoMDFe() {
            LimparCamposEncerramentoMDFe();

            $("#divEncerramentoMDFe").modal({ keyboard: false, backdrop: 'static' });
        }

        function LimparCamposEncerramentoMDFe() {
            $("#txtDataEncerramento").val("");
            $("#txtDataEventoEncerramento").val("");
            $("#txtEstadoEncerramento").val("");
            $("#selMunicipioEncerramento").html("");
        }

        function ObterInformacoesEncerramento() {
            executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterDetalhesEncerramento?callback=?", { CodigoMDFe: $("body").data("mdfeEncerramento").Codigo }, function (r) {
                if (r.Sucesso) {
                    var data = new Date();
                    $("#txtDataEncerramento").val(Globalize.format(data, "dd/MM/yyyy HH:mm"));
                    $("#txtDataEventoEncerramento").val(Globalize.format(data, "dd/MM/yyyy HH:mm"));

                    $("#txtEstadoEncerramento").val(r.Objeto.DescricaoUF);

                    var selMunicipioEncerramento = document.getElementById("selMunicipioEncerramento");
                    selMunicipioEncerramento.options.length = 0;

                    for (var i = 0; i < r.Objeto.Municipios.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto.Municipios[i].Descricao;
                        optn.value = r.Objeto.Municipios[i].Codigo;

                        selMunicipioEncerramento.options.add(optn);
                    }

                    $("#selMunicipioEncerramento").val("");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgEncerramentoMDFe");
                }
            });
        }

        function ValidarCamposEncerramentoMDFe() {
            var data = $("#txtDataEncerramento").val();
            var hora = $("#txtHoraEncerramento").val();
            var municipio = $("#selMunicipioEncerramento").val();
            var valido = true;

            if (data != "") {
                CampoSemErro("#txtDataEncerramento");
            } else {
                CampoComErro("#txtDataEncerramento");
                valido = false;
            }

            if (hora != "") {
                CampoSemErro("#txtHoraEncerramento");
            } else {
                CampoComErro("#txtHoraEncerramento");
                valido = false;
            }

            if (municipio != null && municipio != "") {
                CampoSemErro("#selMunicipioEncerramento");
            } else {
                CampoComErro("#selMunicipioEncerramento");
                valido = false;
            }

            return valido;
        }

        function BaixarDAMDFE(mdfe) {
            if (mdfe.data.Status >= 3 && mdfe.data.Status < 9) {
                executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadDAMDFE", { CodigoMDFe: mdfe.data.Codigo, Contingencia: false });
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para baixar o DAMDFE.", "Atenção!");
            }
        }

        function BaixarXML(mdfe) {
            if (mdfe.data.Status >= 3 && mdfe.data.Status < 9) {
                executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLAutorizacao", { CodigoMDFe: mdfe.data.Codigo });
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para baixar o XML de autorização.", "Atenção!");
            }
        }

        function BaixarXMLCancelamento(mdfe) {
            if (mdfe.data.Status == 7) {
                executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLCancelamento", { CodigoMDFe: mdfe.data.Codigo });
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja cancelado para baixar o XML de cancelamento.", "Atenção!");
            }
        }

        function BaixarXMLEncerramento(mdfe) {
            if (mdfe.data.Status == 5) {
                executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLEncerramento", { CodigoMDFe: mdfe.data.Codigo });
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja encerrado para baixar o XML de encerramento.", "Atenção!");
            }
        }

        function AbrirTelaEDIFiscal(mdfe) {
            if (mdfe.data.Status == 3 || mdfe.data.Status == 4 || mdfe.data.Status == 5) {
                $("body").data("mdfeEDIFiscal", mdfe.data);
                $("#txtLacreEdiFiscal").val("");
                $("#divEDIFiscal").modal({ keyboard: false, backdrop: 'static' });
            } else {
                ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para gerar EDI Fiscal.", "Atenção!");
            }
        }

        function FecharTelaEDIFiscal() {
            $("body").data("mdfeEDIFiscal", null);
            $("#divEDIFiscal").modal('hide');
            VoltarAoTopoDaTela();
        }

        function BaixarEDIFiscal() {
            executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadEDIFiscal", { CodigoMDFe: $("body").data("mdfeEDIFiscal").Codigo, Lacre: $("#txtLacreEdiFiscal").val() });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Encerramento de MDF-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
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
                    </div>
                    <div class="row">
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
                                    <option value="9">Rejeição</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
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
                                    <abbr title="Placa do veículo principal">Veículo:</abbr>
                                </span>
                                <input type="text" id="txtPlacaVeiculoFiltro" class="form-control" />
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
    <div class="modal fade" id="divEDIFiscal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">EDI Fiscal</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEDIFiscal"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Número Lacre*:
                                </span>
                                <input type="text" id="txtLacreEdiFiscal" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnGerarEDIFiscal" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Gerar EDI Fiscal</button>
                    <button type="button" id="btnCancelarEDIFiscal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
