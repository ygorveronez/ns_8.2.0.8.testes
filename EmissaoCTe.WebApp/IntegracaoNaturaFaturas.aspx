<%@ Page Title="Natura - Faturas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoNaturaFaturas.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoNaturaFaturas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker") %>
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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script id="ScriptConsultaFaturas" type="text/javascript">
        $(document).ready(function () {
            LimparCamposConsultaFaturas();

            FormatarCampoDate("txtDataInicialConsulta");
            FormatarCampoDate("txtDataFinalConsulta");

            CarregarConsultadeClientes("btnBuscarSacado", "btnBuscarSacado", RetornoConsultaSacado, true, false);

            $("#txtNumeroFaturaConsulta").priceFormat({ centsLimit: 0, centsSeparator: '' });

            $("#btnAbrirTelaConsultaFaturas").click(function () {
                AbrirTelaConsultaFaturas();
            });

            $("#btnFecharConsultaFaturas").click(function () {
                FecharTelaConsultaFaturas();
            });

            $("#btnConsultarFaturas").click(function () {
                ConsultarFaturas();
            });

            $("#btnConsultarIntegracoesNatura").click(function () {
                AtualizarIntegracoesNatura();
            });

            $("#btnConsultarDocumentosVinculados").click(function () {
                AtualizarDocumentosVinculados();
            });


        });

        function RetornoConsultaSacado(cliente) {
            $("#txtCPFCNPJSacado").val(cliente.CPFCNPJ);
        }

        function LimparCamposConsultaFaturas() {
            $("#txtDataInicialConsulta").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataFinalConsulta").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtNumeroFaturaConsulta").val("");
        }

        function AbrirTelaConsultaFaturas() {
            LimparCamposConsultaFaturas();

            $("#divConsultaFaturas").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaConsultaFaturas() {
            LimparCamposConsultaFaturas();

            $("#divConsultaFaturas").modal('hide');
        }

        function ValidarDadosConsultaFaturas() {
            var valido = true;

            var dataInicial = $("#txtDataInicialConsulta").val();
            var dataFinal = $("#txtDataFinalConsulta").val();
            var numeroFatura = Globalize.parseInt($("#txtNumeroFaturaConsulta").val());

            if (isNaN(numeroFatura))
                numeroDT = 0;

            if (dataInicial != "" && dataFinal == "" || dataInicial == "" && dataFinal != "") {
                ExibirMensagemAlerta("É necessário selecionar a Data Inicial e a Data Final para consultar por período.", "Atenção!");
                valido = false;
            } else if ((dataInicial == "" && dataFinal == "") && numeroFatura <= 0) {
                ExibirMensagemAlerta("É necessário preencher um dos campos para realizar a consulta!", "Atenção!");
                valido = false;
            }

            return valido;
        }

        function ConsultarFaturas() {
            if (!ValidarDadosConsultaFaturas())
                return;

            var dados = {
                NumeroPreFatura: $("#txtNumeroFaturaConsulta").val(),
                DataInicial: $("#txtDataInicialConsulta").val(),
                DataFinal: $("#txtDataFinalConsulta").val(),
                AtualizarPreFatura: false
            };

            executarRest("/IntegracaoNaturaFatura/ConsultarFaturas?callback=?", dados, function (r) {

                if (r.Sucesso) {
                    FecharTelaConsultaFaturas();
                    ExibirMensagemSucesso("Faturas consultadas com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderConsultaFaturas");
                }

                AtualizarGridFaturas();
            });
        }
    </script>
    <script id="ScriptGridFaturas" type="text/javascript">
        $(document).ready(function () {
            $("#txtNumeroFaturaFiltro").priceFormat({ centsLimit: 0, centsSeparator: '' });
            $("#txtNumeroPreFaturaFiltro").priceFormat({ centsLimit: 0, centsSeparator: '' });

            FormatarCampoDate("txtDataInicialFiltro");
            FormatarCampoDate("txtDataFinalFiltro");

            $("#btnAtualizarGridFaturas").click(function () {
                AtualizarGridFaturas();
            });

            AtualizarGridFaturas();
        });

        function AtualizarGridFaturas() {
            var dados = {
                NumeroFatura: $("#txtNumeroFaturaFiltro").val(),
                NumeroPreFatura: $("#txtNumeroPreFaturaFiltro").val(),
                DataInicial: $("#txtDataInicialFiltro").val(),
                DataFinal: $("#txtDataFinalFiltro").val(),
                inicioRegistros: 0
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Atualizar Pré-Fatura", Evento: AtualizarPreFatura });
            opcoes.push({ Descricao: "Emitir Fatura", Evento: AbrirTelaEmissaoFatura });
            opcoes.push({ Descricao: "Quitar Fatura", Evento: QuitarFatura });
            opcoes.push({ Descricao: "Cancelar Fatura", Evento: CancelarFatura });
            opcoes.push({ Descricao: "Download Detalhes PDF", Evento: DownloadDetalhesFatura });
            opcoes.push({ Descricao: "Download Detalhes Excel", Evento: DownloadDetalhesFaturaEXCEL });
            opcoes.push({ Descricao: "Integrações Natura", Evento: AbrirTelaIntegracoesNatura });
            opcoes.push({ Descricao: "Documentos Vinculados", Evento: AbrirTelaDocumentosVinculados });

            CriarGridView("/IntegracaoNaturaFatura/Consultar?callback=?", dados, "tbl_faturas_table", "tbl_faturas", "tbl_paginacao_faturas", opcoes, [0,1], null);
        }

        function AtualizarPreFatura(preFatura) {
            if (preFatura.data.Status == "Pendente") {
                var dados = {
                    NumeroPreFatura: preFatura.data.NumeroPreFatura,
                    AtualizarPreFatura: true
                };

                executarRest("/IntegracaoNaturaFatura/ConsultarFaturas?callback=?", dados, function (r) {

                    if (r.Sucesso) {
                        FecharTelaConsultaFaturas();
                        ExibirMensagemSucesso("Fatura atualizada com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderConsultaFaturas");
                    }

                    AtualizarGridFaturas();
                });

            }
            else {
                jAlert("Somente é possível atualizar pré-fatura com status Pendente!", "Atenção!");
            }
        }

    </script>
    <script id="ScriptEmissaoFatura" type="text/javascript">
        $(document).ready(function () {

            FormatarCampoDate("txtDataEmissao");
            FormatarCampoDate("txtDataVencimento");

            $("#btnEmitirFatura").click(function () {
                EmitirFatura();
            });

            $("#btnCancelarEmissaoFatura").click(function () {
                FecharTelaEmissaoFatura();
            });

        });

        function LimparCamposEmissaoFatura() {
            $("body").data("codigoFatura", null);
            $("#txtDataEmissao").val("");
            $("#txtDataVencimento").val("");
            $("#txtCPFCNPJSacado").val("");
        }

        function AbrirTelaEmissaoFatura(fatura) {
            LimparCamposEmissaoFatura();

            $("body").data("codigoFatura", fatura.data.Codigo);
            $("#txtDataEmissao").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtCPFCNPJSacado").val(fatura.data.Sacado);

            $("#divEmissaoFatura").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaEmissaoFatura() {
            LimparCamposEmissaoFatura();

            $("#divEmissaoFatura").modal('hide');
        }

        function ValidarDadosEmissaoFatura() {
            var dataEmissao = $("#txtDataEmissao").val();
            var dataVencimento = $("#txtDataVencimento").val();
            var valido = true;

            if (dataEmissao == "") {
                valido = false;
                CampoComErro("#txtDataEmissao");
            } else {
                CampoSemErro("#txtDataEmissao");
            }

            if (dataVencimento == "") {
                valido = false;
                CampoComErro("#txtDataVencimento");
            } else {
                CampoSemErro("#txtDataVencimento");
            }

            return valido;
        }

        function EmitirFatura() {
            if (ValidarDadosEmissaoFatura()) {

                var dados = {
                    CodigoFatura: $("body").data("codigoFatura"),
                    DataEmissao: $("#txtDataEmissao").val(),
                    DataVencimento: $("#txtDataVencimento").val(),
                    Sacado: $("#txtCPFCNPJSacado").val()
                };

                executarRest("/IntegracaoNaturaFatura/EmitirFatura?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        FecharTelaEmissaoFatura();
                        AtualizarGridFaturas();
                        ExibirMensagemSucesso("Fatura emitida com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoFatura");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "messages-placeholderEmissaoFatura");
            }
        }
    </script>
    <script id="QuitarFaturas" type="text/javascript">
        function QuitarFatura(fatura) {
            jConfirm("Deseja realmente quitar a fatura <b>" + fatura.data.Numero + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoNaturaFatura/QuitarFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("Fatura quitada com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
    <script id="ScriptCancelarFatura">
        function CancelarFatura(fatura) {
            jConfirm("Deseja realmente cancelar a fatura <b>" + fatura.data.Numero + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoNaturaFatura/CancelarFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("Fatura cancelada com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
    <script id="DownloadDetalhesFatura" type="text/javascript">
        function DownloadDetalhesFatura(fatura) {
            executarDownload("/IntegracaoNaturaFatura/DownloadDetalhesFatura", { CodigoFatura: fatura.data.Codigo, Tipo: "PDF" });
        }
        function DownloadDetalhesFaturaEXCEL(fatura) {
            executarDownload("/IntegracaoNaturaFatura/DownloadDetalhesFatura", { CodigoFatura: fatura.data.Codigo, Tipo: "EXCEL" });
        }
    </script>
    <script id="IntegracoesNatura" type="text/javascript">

        function AbrirTelaIntegracoesNatura(fatura) {
            $("body").data("codigoFaturaIntegracaoNatura", fatura.data.Codigo);
            AtualizarIntegracoesNatura();
            $("#divIntegracoesNatura").modal({ keyboard: false, backdrop: 'static' });
        }

        function AtualizarIntegracoesNatura() {
            var dados = {
                CodigoFatura: $("body").data("codigoFaturaIntegracaoNatura")
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Baixar XML Envio", Evento: BaixarXMLEnvioNatura });
            opcoes.push({ Descricao: "Baixar XML Retorno", Evento: BaixarXMLRetornoNatura });

            CriarGridView("/IntegracaoNaturaFatura/ConsultarIntegracoesNatura?callback=?", dados, "tbl_paginacao_integracoes_natura_table", "tbl_integracoes_natura", "tbl_paginacao_integracoes_natura", opcoes, [0, 1], null);
        }

        function BaixarXMLEnvioNatura(naturaXML) {
            executarDownload("/IntegracaoNaturaFatura/DownloadXMLIntegracao", { Codigo: naturaXML.data.Codigo, Tipo: "E" });
        }

        function BaixarXMLRetornoNatura(naturaXML) {
            executarDownload("/IntegracaoNaturaFatura/DownloadXMLIntegracao", { Codigo: naturaXML.data.Codigo, Tipo: "R" });
        }
    </script>

    <script id="DocumentosVinculados" type="text/javascript">

        function AbrirTelaDocumentosVinculados(fatura) {
            $("body").data("codigoFaturaIntegracaoNatura", fatura.data.Codigo);
            AtualizarDocumentosVinculados();
            $("#divDocumentosVinculados").modal({ keyboard: false, backdrop: 'static' });
        }

        function AtualizarDocumentosVinculados() {
            var dados = {
                CodigoFatura: $("body").data("codigoFaturaIntegracaoNatura"),
                Tipo: $("#selTipoDocumento").val(),
                Numero: $("#txtNumeroDocumentoVinculado").val()
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Excluir", Evento: ExcluirDocumentoFatura });

            CriarGridView("/IntegracaoNaturaFatura/ConsultarDocumentosVinculados?callback=?", dados, "tbl_paginacao_documentos_vinculados_table", "tbl_documentos_vinculados", "tbl_paginacao_documentos_vinculados", opcoes, [0], null);
        }

        function ExcluirDocumentoFatura(fatura) {
            jConfirm("Deseja realmente remover o documento da Fatura? (Processo irreversível)", "Atenção!", function (resp) {
                if (resp) {
                    executarRest("/IntegracaoNaturaFatura/RemoverDocumentoFatura?callback=?", { Codigo: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Documento removido com sucesso.", "Sucesso!", "messages-placeholderCTesEmitidos");
                            AtualizarDocumentosVinculados();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderCTesEmitidos");
                        }
                    });
                }
            });
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Natura - Faturas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnAbrirTelaConsultaFaturas" class="btn btn-default"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar Fatura(s)</button>
    <div class="row" style="margin-top: 10px; margin-bottom: 5px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Fat.:
                </span>
                <input type="text" id="txtNumeroFaturaFiltro" class="form-control" maxlength="10" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Pré Fat.:
                </span>
                <input type="text" id="txtNumeroPreFaturaFiltro" class="form-control" maxlength="10" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicialFiltro" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinalFiltro" class="form-control" />
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridFaturas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Buscar / Atualizar Faturas</button>
    <div id="tbl_faturas" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_faturas">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divEmissaoFatura" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de Fatura</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderEmissaoFatura">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Emissão*:
                                </span>
                                <input type="text" id="txtDataEmissao" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Vcto.*:
                                </span>
                                <input type="text" id="txtDataVencimento" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Sacado:
                                </span>
                                <input type="text" id="txtCPFCNPJSacado" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarSacado" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirFatura" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir Fatura</button>
                    <button type="button" id="btnCancelarEmissaoFatura" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divConsultaFaturas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de Faturas</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderConsultaFaturas">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nº Pré Fat.:
                                </span>
                                <input type="text" id="txtNumeroFaturaConsulta" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicial:
                                </span>
                                <input type="text" id="txtDataInicialConsulta" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Final:
                                </span>
                                <input type="text" id="txtDataFinalConsulta" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnConsultarFaturas" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Consultar Faturas</button>
                    <button type="button" id="btnFecharConsultaFaturas" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divIntegracoesNatura" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Integrações Natura</h4>
                </div>
                <div class="modal-body">
                    <button type="button" id="btnConsultarIntegracoesNatura" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="messages-placeholderIntegracoesNatura">
                    </div>
                    <div id="tbl_integracoes_natura" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_integracoes_natura">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDocumentosVinculados" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Documentos vinculados na fatura</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderDocumentosVinculados"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Documento:
                                </span>
                                <select id="selTipoDocumento" class="form-control">
                                    <option value="0">Todos</option>
                                    <option value="1">CT-e</option>
                                    <option value="2">NFS-e</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Número.:
                                </span>
                                <input type="text" id="txtNumeroDocumentoVinculado" class="form-control" />
                            </div>
                        </div>
                        <button type="button" id="btnConsultarDocumentosVinculados" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Filtrar</button>
                    </div>
                    <div id="tbl_documentos_vinculados" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_documentos_vinculados">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
