<%@ Page Title="Natura - Documentos de Transporte" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoNatura.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoNatura" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
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
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script id="ScriptConsultaDocumentosTransporte" type="text/javascript">
        $(document).ready(function () {
            LimparCamposConsultaDocumentosTransporte();

            FormatarCampoDate("txtDataInicialConsulta");
            FormatarCampoDate("txtDataFinalConsulta");

            $("#txtNumeroDocumentoTransporteConsulta").priceFormat({ centsLimit: 0, centsSeparator: '' });

            $("#btnAbrirTelaConsultaDocumentosTransporte").click(function () {
                AbrirTelaConsultaDocumentosTransporte();
            });

            $("#btnFecharConsultaDocumentosTransporte").click(function () {
                FecharTelaConsultaDocumentosTransporte();
            });

            $("#btnConsultarDocumentosTransporte").click(function () {
                ConsultarDocumentosTransporte();
            });
        });

        function LimparCamposConsultaDocumentosTransporte() {
            $("#txtDataInicialConsulta").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataFinalConsulta").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtNumeroDocumentoTransporteConsulta").val("");
        }

        function AbrirTelaConsultaDocumentosTransporte() {
            LimparCamposConsultaDocumentosTransporte();

            $("#divConsultaDocumentosTransporte").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaConsultaDocumentosTransporte() {
            LimparCamposConsultaDocumentosTransporte();

            $("#divConsultaDocumentosTransporte").modal('hide');
        }

        function ValidarDadosConsultaDocumentosTransporte() {
            var valido = true;

            var dataInicial = $("#txtDataInicialConsulta").val();
            var dataFinal = $("#txtDataFinalConsulta").val();
            var numeroDT = Globalize.parseInt($("#txtNumeroDocumentoTransporteConsulta").val());

            if (isNaN(numeroDT))
                numeroDT = 0;

            if (dataInicial != "" && dataFinal == "" || dataInicial == "" && dataFinal != "") {
                ExibirMensagemAlerta("É necessário selecionar a Data Inicial e a Data Final para consultar por período.", "Atenção!");
                valido = false;
            } else if ((dataInicial == "" && dataFinal == "") && numeroDT <= 0) {
                ExibirMensagemAlerta("É necessário preencher um dos campos para realizar a consulta!", "Atenção!");
                valido = false;
            }

            return valido;
        }

        function ConsultarDocumentosTransporte() {
            if (!ValidarDadosConsultaDocumentosTransporte())
                return;

            var dados = {
                NumeroDT: $("#txtNumeroDocumentoTransporteConsulta").val(),
                DataInicial: $("#txtDataInicialConsulta").val(),
                DataFinal: $("#txtDataFinalConsulta").val()
            };

            executarRest("/IntegracaoNatura/ConsultarDocumentosTransporte?callback=?", dados, function (r) {

                if (r.Sucesso) {
                    FecharTelaConsultaDocumentosTransporte();
                    ExibirMensagemSucesso("Documentos de transporte consultados com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderConsultaDocumentosTransporte");
                }

                AtualizarGridDocumentosTransporte();
            });
        }
    </script>
    <script id="ScriptGridDocumentosTransporte" type="text/javascript">
        $(document).ready(function () {
            $("#txtNumeroDocumentoTransporteFiltro").priceFormat({ centsLimit: 0, centsSeparator: '' });

            FormatarCampoDate("txtDataInicialFiltro");
            FormatarCampoDate("txtDataFinalFiltro");

            var today = new Date();
            var yesterday = new Date(today);
            var tomorrow = new Date(today);
            yesterday.setDate(today.getDate() - 5);
            tomorrow.setDate(today.getDate() + 1);

            $("#txtDataInicialFiltro").val(Globalize.format(yesterday, "dd/MM/yyyy"));
            $("#txtDataFinalFiltro").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

            $("#btnAtualizarGridDocumentosTransporte").click(function () {
                AtualizarGridDocumentosTransporte();
            });
            AtualizarGridDocumentosTransporte();
        });

        function AtualizarGridDocumentosTransporte() {
            var dados = {
                NumeroDocumentoTransporte: $("#txtNumeroDocumentoTransporteFiltro").val(),
                DataInicial: $("#txtDataInicialFiltro").val(),
                DataFinal: $("#txtDataFinalFiltro").val(),
                NumeroNFe: $("#txtNumeroNFe").val(),
                StatusDT: $("#selStatusFiltro").val(),
                inicioRegistros: 0
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Gerar CT-es", Evento: AbrirTelaEmissaoCTes });
            opcoes.push({ Descricao: "Gerar NFS-es", Evento: AbrirTelaEmissaoNFSes });
            opcoes.push({ Descricao: "CT-es Gerados", Evento: AbrirTelaCTesEmitidos });
            opcoes.push({ Descricao: "NFS-es Geradas", Evento: AbrirTelaNFSesEmitidas });
            opcoes.push({ Descricao: "Enviar Retorno", Evento: EnviarRetornoDocumentoTransporte });
            opcoes.push({ Descricao: "Enviar Ocorrências", Evento: EnviarOcorrenciasDocumentoTransporte });
            opcoes.push({ Descricao: "Enviar Retorno CT-e Complementar", Evento: EnviarRetornoDocumentoTransporteComplementar });
            opcoes.push({ Descricao: "Atualizar DT", Evento: AtualizarDT });
            opcoes.push({ Descricao: "Cancelar DT", Evento: CancelarDT });
            opcoes.push({ Descricao: "Download XML Consulta DT", Evento: DownloadXMLConsultaDT });
            opcoes.push({ Descricao: "Download XML DT", Evento: DownloadXMLDT });
            opcoes.push({ Descricao: "Integrações Retorno DT", Evento: AbrirTelaIntegracoesRetorno });

            CriarGridView("/IntegracaoNatura/Consultar?callback=?", dados, "tbl_documentos_transporte_table", "tbl_documentos_transporte", "tbl_paginacao_documentos_transporte", opcoes, [0], null);
        }

    </script>
    <script id="ScriptEmissaoCTes" type="text/javascript">
        $(document).ready(function () {
            $("#txtValorFrete").priceFormat();
            $("#txtValorComplemento").priceFormat();

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
            CarregarConsultaTipoOperacao("btnBuscarTipoOperacao", "btnBuscarTipoOperacao", RetornoConsultaTipoOperacao, true, false);
            CarregarConsultaTipoCargaEmbarcador("btnBuscarTipoCarga", "btnBuscarTipoCarga", RetornoConsultaTipoCarga, true, false);
            CarregarConsultaModeloVeicularCarga("btnBuscarModeloVeicularCarga", "btnBuscarModeloVeicularCarga", RetornoConsultaModeloVeicularCarga, true, false);

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoVeiculo", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoMotorista", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtTipoCarga").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoTipoCarga", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtTipoOperacao").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoTipoOperacao", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtModeloVeicularCarga").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoModeloVeicularCarga", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnEmitirCTes").click(function () {
                EmitirCTes();
            });

            $("#btnCancelarEmissaoCTes").click(function () {
                FecharTelaEmissaoCTes();
            });


            $("#btnConsultarCTe").click(function () {
                AtualizarCTesEmitidos();
            });

            $("#btnEmitirCTeComplementar").click(function () {
                emitirCTeComplmentar();
            });

            $("#btnCalcularFrete").click(function () {
                CalcularFrete();
            });
        });

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("codigoVeiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);

            executarRest("/IntegracaoNatura/ObterDadosVeiculo?callback=?", { CodigoVeiculo: veiculo.Codigo }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.CodigoMotorista > 0) {
                        $("body").data("codigoMotorista", r.Objeto.CodigoMotorista);
                        $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                    } else {
                        $("body").data("codigoMotorista", null);
                        $("#txtMotorista").val('');
                    }

                    if (r.Objeto.CodigoModeloVeicularCarga > 0) {
                        $("body").data("codigoModeloVeicularCarga", r.Objeto.CodigoModeloVeicularCarga);
                        $("#txtModeloVeicularCarga").val(r.Objeto.DescricaoModeloVeicularCarga);
                    } else {
                        $("body").data("codigoModeloVeicularCarga", null);
                        $("#txtModeloVeicularCarga").val("");
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoCTes");
                }
            });
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("codigoMotorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function RetornoConsultaTipoOperacao(tipoOperacao) {
            $("body").data("codigoTipoOperacao", tipoOperacao.Codigo);
            $("#txtTipoOperacao").val(tipoOperacao.Descricao);
        }

        function RetornoConsultaTipoCarga(tipoCarga) {
            $("body").data("codigoTipoCarga", tipoCarga.Codigo);
            $("#txtTipoCarga").val(tipoCarga.Descricao);
        }

        function RetornoConsultaModeloVeicularCarga(modelo) {
            $("body").data("codigoModeloVeicularCarga", modelo.Codigo);
            $("#txtModeloVeicularCarga").val(modelo.Descricao);
        }

        function LimparCamposEmissaoCTes() {
            $("body").data("codigoVeiculo", null);
            $("#txtVeiculo").val("");
            $("body").data("codigoMotorista", null);
            $("#txtMotorista").val("");
            $("body").data("codigoTipoOperacao", null);
            $("#txtTipoOperacao").val("");
            $("body").data("codigoTipoCarga", null);
            $("#txtTipoCarga").val("");
            $("body").data("codigoModeloVeicularCarga", null);
            $("#txtModeloVeicularCarga").val("");
            $("#txtOrigem").val("");
            $("#txtDestino").val("");
            $("#txtQtdNotasCTe").val("");
            $("#txtValorFrete").val("");
            $("#txtValorFreteCalculado").val("");
            $("#txtObservacao").val("");
            $("body").data("freteFoiCalculado", false);
        }

        function AbrirTelaEmissaoCTes(documentoTransporte) {
            var enumTipoDocumento = {
                WebService: 0,
                FTP: 1
            }

            if (documentoTransporte.data.Status == "Cancelado")
                jAlert("Não é possível gerar CT-e(s) com DT Cancelada.", "Atenção!");
            else {
                LimparCamposEmissaoCTes();

                $("body").data("documentoTransporteEmissaoCTe", documentoTransporte.data);

                executarRest("/IntegracaoNatura/ObterDetalhesEmissaoCTe?callback=?", { CodigoDT: documentoTransporte.data.Codigo }, function (r) {
                    if (r.Sucesso) {
                        // Alterna campos de acordo com o tipo da importacao
                        if (r.Objeto.Tipo == enumTipoDocumento.FTP) {
                            $("#colModeloVeicularCarga").hide();
                            $("#colTipoOperacao").hide();
                            $("#colTipoCarga").hide();
                            $("#colValorFreteCalculado").hide();
                            $("#txtValorFrete").prop('disabled', false);
                        } else if (r.Objeto.Tipo == enumTipoDocumento.WebService) {
                            $("#colModeloVeicularCarga").show();
                            $("#colTipoOperacao").show();
                            $("#colTipoCarga").show();
                            $("#colValorFreteCalculado").show();
                            $("#txtValorFrete").prop('disabled', true);
                        }

                        $("body").data("codigoMotorista", r.Objeto.CodigoMotorista);
                        $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);

                        $("body").data("codigoModeloVeicularCarga", r.Objeto.CodigoModeloVeicularCarga);
                        $("#txtModeloVeicularCarga").val(r.Objeto.DescricaoModeloVeicularCarga);

                        $("body").data("codigoVeiculo", r.Objeto.CodigoVeiculo);
                        $("#txtVeiculo").val(r.Objeto.PlacaVeiculo);

                        $("body").data("codigoTipoOperacao", r.Objeto.CodigoTipoOperacao);
                        $("#txtTipoOperacao").val(r.Objeto.DescricaoTipoOperacao);

                        $("body").data("codigoTipoCarga", r.Objeto.CodigoTipoCarga);
                        $("#txtTipoCarga").val(r.Objeto.DescricaoTipoCarga);

                        $("#txtOrigem").val(r.Objeto.Origem);
                        $("#txtDestino").val(r.Objeto.Destino);
                        $("#txtQtdNotasCTe").val(r.Objeto.QuantidadeNotas);

                        $("#txtValorFrete").val(r.Objeto.ValorFrete);
                        $("#txtValorFreteCalculado").val(r.Objeto.ValorFreteCalculado);

                        if (r.Objeto.QuantidadeNotasNFSe != null && r.Objeto.QuantidadeNotasNFSe > 0) {
                            $("#txtObservacao").val("EXISTEM " + r.Objeto.QuantidadeNotasNFSe + " ENTREGA(S) MUNICIPAIS PARA GERAR NFS-E.");
                            $("#colObsCTe").show();
                        }
                        else
                            $("#colObsCTe").hide();

                        if (Globalize.parseFloat(r.Objeto.ValorFreteCalculado) > 0)
                            $("body").data("freteFoiCalculado", true);

                        $("#divEmissaoCTes").modal({ keyboard: false, backdrop: 'static' });
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        var _CodigoDocumentoTransporte = 0;
        function AbrirTelaCTesEmitidos(documentoTransporte) {
            _CodigoDocumentoTransporte = documentoTransporte.data.Codigo;
            AtualizarCTesEmitidos();
            $("#divCTesEmitidos").modal({ keyboard: false, backdrop: 'static' });
        }

        function AtualizarCTesEmitidos() {
            var dados = {
                NumeroDocumentoTransporte: _CodigoDocumentoTransporte
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Baixar DACTE", Evento: DownloadDacte });
            opcoes.push({ Descricao: "Baixar XML", Evento: DownloadXML });
            opcoes.push({ Descricao: "Gerar CT-e Complementar", Evento: abrirModalCTEComplementar });
            opcoes.push({ Descricao: "Remover CT-e", Evento: RemoverCTeDoDT });

            CriarGridView("/IntegracaoNatura/ConsultarCTesEmitidos?callback=?", dados, "tbl_paginacao_cte_documentos_transporte_table", "tbl_cte_documentos_transporte", "tbl_paginacao_cte_documentos_transporte", opcoes, [0, 1, 2, 3], null);
        }

        function DownloadDacte(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadDacte", { CodigoCTe: cte.data.Codigo });
        }

        function DownloadXML(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXML", { CodigoCTe: cte.data.Codigo });
        }

        function RemoverCTeDoDT(data) {
            jConfirm("Deseja realmente remover o CT-e " + data.data.Numero + " do documento de transporte?", "Atenção!", function (resp) {
                if (resp) {
                    executarRest("/IntegracaoNatura/RemoverCTeDocumentoTransporte?callback=?", { CodigoCTe: data.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("CT-e removido com sucesso.", "Sucesso!", "messages-placeholderCTesEmitidos");
                            AtualizarCTesEmitidos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderCTesEmitidos");
                        }
                    });
                }
            });
        }

        var _CTE = 0;
        function abrirModalCTEComplementar(cte) {
            _CTE = cte.data.Codigo;
            $("#divCTesComplementares").modal({ keyboard: false, backdrop: 'static' });
        }

        function emitirCTeComplmentar() {
            if (ValidarDadosEmissaoCTeComplementar()) {

                var dados = {
                    CodigoCte: _CTE,
                    NumeroDocumentoTransporte: _CodigoDocumentoTransporte,
                    ValorComplemento: $("#txtValorComplemento").val(),
                    IncluirICMSFrete: $("#ckbIncluirICMSFrete")[0].checked,
                    Observacao: $("#txtObservacaoCTeComplementar").val()
                };

                executarRest("/IntegracaoNatura/EmitirCTeComplentar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        FecharTelaEmissaoCTeComplentar();
                        AtualizarCTesEmitidos();
                        ExibirMensagemSucesso("CT-es complementar emitido com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoCTes");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "messages-placeholderEmissaoCTes");
            }
        }

        function LimparCamposEmissaoCTeComplementar() {
            $("#txtValorComplemento").val("");
            $("#txtObservacaoCTeComplementar").val("");
            $("#ckbIncluirICMSFrete").attr("checked", true);
        }


        function FecharTelaEmissaoCTes() {
            LimparCamposEmissaoCTes();

            $("#divEmissaoCTes").modal('hide');
        }

        function FecharTelaEmissaoCTeComplentar() {
            LimparCamposEmissaoCTeComplementar();

            $("#divCTesComplementares").modal('hide');
        }

        function ValidarDadosEmissaoCTeComplementar() {
            var valorComplementoFrete = Globalize.parseFloat($("#txtValorComplemento").val());
            var valido = true;

            if (isNaN(valorComplementoFrete) || valorComplementoFrete <= 0) {
                valido = false;
                CampoComErro("#txtValorComplemento");
            } else {
                CampoSemErro("#txtValorComplemento");
            }

            return valido;
        }

        function ValidarDadosEmissaoCTes() {
            var codigoVeiculo = $("body").data("codigoVeiculo");
            var codigoMotorista = $("body").data("codigoMotorista");
            var codigoTipoOperacao = $("body").data("codigoTipoOperacao");
            var codigoTipoCarga = $("body").data("codigoTipoCarga");
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var valido = true;

            if (codigoVeiculo == null || codigoVeiculo <= 0) {
                valido = false;
                CampoComErro("#txtVeiculo");
            } else {
                CampoSemErro("#txtVeiculo");
            }

            if (codigoMotorista == null || codigoMotorista <= 0) {
                valido = false;
                CampoComErro("#txtMotorista");
            } else {
                CampoSemErro("#txtMotorista");
            }

            //if (codigoTipoOperacao == null || codigoTipoOperacao <= 0) {
            //    valido = false;
            //    CampoComErro("#txtTipoOperacao");
            //} else {
            //    CampoSemErro("#txtTipoOperacao");
            //}

            //if (codigoTipoCarga == null || codigoTipoCarga <= 0) {
            //    valido = false;
            //    CampoComErro("#txtTipoCarga");
            //} else {
            //    CampoSemErro("#txtTipoCarga");
            //}

            if (isNaN(valorFrete) || valorFrete <= 0) {
                valido = false;
                CampoComErro("#txtValorFrete");
            } else {
                CampoSemErro("#txtValorFrete");
            }

            return valido;
        }

        function EmitirCTes() {
            //if ($("body").data("freteFoiCalculado") !== true) {
            //    ExibirMensagemAlerta("Calcule o frete antes de realizar a emissão do(s) CT-e(s)!", "Atenção!", "messages-placeholderEmissaoCTes");
            //    return;
            //}

            if (ValidarDadosEmissaoCTes()) {

                var dados = {
                    CodigoDocumentoTransporte: $("body").data("documentoTransporteEmissaoCTe").Codigo,
                    CodigoMotorista: $("body").data("codigoMotorista"),
                    CodigoVeiculo: $("body").data("codigoVeiculo"),
                    CodigoTipoOperacao: $("body").data("codigoTipoOperacao"),
                    CodigoTipoCarga: $("body").data("codigoTipoCarga"),
                    ValorFrete: $("#txtValorFrete").val(),
                    ValorFreteCalculado: $("#txtValorFreteCalculado").val(),
                    Observacao: $("#txtObservacaoCTe").val()
                };

                executarRest("/IntegracaoNatura/EmitirCTes?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        FecharTelaEmissaoCTes();
                        AtualizarGridDocumentosTransporte();
                        ExibirMensagemSucesso("CT-es emitidos com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoCTes");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "messages-placeholderEmissaoCTes");
            }
        }

        function CalcularFrete() {
            var dados = {
                CodigoDocumentoTransporte: $("body").data("documentoTransporteEmissaoCTe").Codigo,
                CodigoModeloVeicularCarga: $("body").data("codigoModeloVeicularCarga"),
                CodigoVeiculo: $("body").data("codigoVeiculo"),
                CodigoTipoOperacao: $("body").data("codigoTipoOperacao"),
                CodigoTipoCarga: $("body").data("codigoTipoCarga")
            };

            executarRest("/IntegracaoNatura/CalcularFrete?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    $("#txtValorFreteCalculado").val(r.Objeto.ValorFrete);
                } else {
                    ExibirMensagemErro(r.Erro.replace(/\n/g, "<br />"), "Atenção!<br/>", "messages-placeholderEmissaoCTes");
                }
                $("body").data("freteFoiCalculado", true);
            });
        }
    </script>
    <script id="ScriptEnvioRetornoDocumentoTransporte" type="text/javascript">
        function EnviarRetornoDocumentoTransporte(documentoTransporte) {
            jConfirm("Deseja realmente enviar o retorno do documento de transporte <b>" + documentoTransporte.data.NumeroDT + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoNatura/EnviarRetornoDocumentoTransporte?callback=?", { CodigoDocumentoTransporte: documentoTransporte.data.Codigo, tipoRetorno: "Normal" }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridDocumentosTransporte();
                            ExibirMensagemSucesso("Retorno enviado com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function EnviarRetornoDocumentoTransporteComplementar(documentoTransporte) {
            jConfirm("Deseja realmente enviar o retorno do CT-e complementar da DT <b>" + documentoTransporte.data.NumeroDT + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoNatura/EnviarRetornoDocumentoTransporte?callback=?", { CodigoDocumentoTransporte: documentoTransporte.data.Codigo, tipoRetorno: "Complementar" }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridDocumentosTransporte();
                            ExibirMensagemSucesso("CT-e complementar enviado com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function AtualizarDT(documentoTransporte) {
            if (documentoTransporte.data.Status == "Em Digitação" || documentoTransporte.data.Status.indexOf("Erro") > -1)
                jConfirm("Deseja realmente atualizar os dados da DT <b>" + documentoTransporte.data.NumeroDT + "</b>?", "Atenção!", function (retorno) {
                    if (retorno) {
                        var dados = {
                            NumeroDT: documentoTransporte.data.NumeroDT,
                            DataInicial: documentoTransporte.data.DataEmissao.substring(0, 10),
                            DataFinal: documentoTransporte.data.DataEmissao.substring(0, 10),
                            AtualizaDT: true
                        };

                        executarRest("/IntegracaoNatura/ConsultarDocumentosTransporte?callback=?", dados, function (r) {

                            if (r.Sucesso) {
                                jAlert("Documentos de transporte consultados com sucesso!", "Sucesso!");
                            } else {
                                jAlert(r.Erro, "Atenção!");
                            }

                            AtualizarGridDocumentosTransporte();
                        });
                    }
                });
            else {
                jAlert("Somente é possível atualizar DT com status Em Digitação!", "Atenção!");
            }
        }

        function CancelarDT(documentoTransporte) {
            if (documentoTransporte.data.Status == "Cancelado")
                jAlert("DT já está Cancelada.", "Atenção!");
            else {
                jConfirm("Deseja realmente cancelar a DT <b>" + documentoTransporte.data.NumeroDT + "</b>?", "Atenção!", function (retorno) {
                    if (retorno) {
                        var dados = {
                            CodigoDocumentoTransporte: documentoTransporte.data.Codigo
                        };

                        executarRest("/IntegracaoNatura/CancelarDocumentosTransporte?callback=?", dados, function (r) {
                            if (r.Sucesso) {
                                jAlert("Documentos de transporte cancelado com sucesso!", "Sucesso!");
                            } else {
                                jAlert(r.Erro, "Atenção!");
                            }

                            AtualizarGridDocumentosTransporte();
                        });
                    }
                });
            }
        }

        function DownloadXMLDT(documentoTransporte) {
            executarDownload("/IntegracaoNatura/DownloadXMLDT", { CodigoDT: documentoTransporte.data.Codigo });
        }

        function DownloadXMLConsultaDT(documentoTransporte) {
            executarDownload("/IntegracaoNatura/DownloadXMLConsultaDT", { CodigoDT: documentoTransporte.data.Codigo });
        }

    </script>
    <script id="ScriptEnvioOcorrenciasDocumentoTransporte">
        function EnviarOcorrenciasDocumentoTransporte(documentoTransporte) {
            jConfirm("Deseja realmente enviar as ocorrências do documento de transporte <b>" + documentoTransporte.data.NumeroDT + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoNatura/EnviarOcorrenciasDocumentoTransporte?callback=?", { CodigoDocumentoTransporte: documentoTransporte.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridDocumentosTransporte();
                            ExibirMensagemSucesso("Ocorrências enviadas com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
    <script type="text/javascript" id="ScriptEmissaoNFSes">
        $(document).ready(function () {
            $("#txtValorFreteNFSe").priceFormat();

            $("#btnEmitirNFSes").click(function () {
                EmitirNFSes();
            });

            $("#btnCancelarEmissaoNFSes").click(function () {
                FecharTelaEmissaoNFSes();
            });

            $("#btnConsultarNFSe").click(function () {
                AtualizarNFSesEmitidas();
            });
        });

        function LimparCamposEmissaoNFSes() {
            $("#txtValorFreteNFSe").val("");
            $("#txtQtdNotasNFSe").val("");
            $("#txtOrigemNFSe").val("");
            $("#txtDestinoNFSe").val("");
        }

        function AbrirTelaEmissaoNFSes(documentoTransporte) {
            var enumTipoDocumento = {
                WebService: 0,
                FTP: 1
            }

            if (documentoTransporte.data.Status == "Cancelado")
                jAlert("Não é possível gerar NFS-e(s) com DT Cancelada.", "Atenção!");
            else {
                LimparCamposEmissaoNFSes();

                $("body").data("documentoTransporteEmissaoNFSe", documentoTransporte.data);

                executarRest("/IntegracaoNatura/ObterDetalhesEmissaoNFSe?callback=?", { CodigoDT: documentoTransporte.data.Codigo }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto.Tipo == enumTipoDocumento.FTP) {
                            $("#txtQtdNotasNFSe").val(r.Objeto.QuantidadeNotas);
                            $("#txtValorFreteNFSe").val(r.Objeto.ValorFrete);
                            $("#txtOrigemNFSe").val(r.Objeto.Origem);
                            $("#txtDestinoNFSe").val(r.Objeto.Destino);
                            $("#txtOrigemNFSe").show;
                            $("#txtDestinoNFSe").show;

                            if (r.Objeto.ValorFrete == "0,00")
                                $("#txtValorFreteNFSe").prop('disabled', false);
                            else
                                $("#txtValorFreteNFSe").prop('disabled', true);

                            $("#colObsNFSe").show();
                            if (r.Objeto.QuantidadeNotasNFSe != null && r.Objeto.QuantidadeNotasNFSe > 0)
                                $("#txtObservacaoNFSe").val("EXISTEM " + r.Objeto.QuantidadeNotasNFSe + " ENTREGA(S) MUNICIPAIS PARA GERAR NFS-E.");
                            else
                                $("#txtObservacaoNFSe").val("NÃO EXISTEM ENTREGAS MUNICIPAIS PARA GERAR NFS-E.");

                        } else if (r.Objeto.Tipo == enumTipoDocumento.WebService) {
                            $("#txtQtdNotasNFSe").val(r.Objeto.QuantidadeNotas);
                            $("#txtValorFreteNFSe").val(documentoTransporte.data.ValorFrete.replace(" (Valor Natura)", ""));
                            $("#txtOrigemNFSe").hide;
                            $("#txtDestinoNFSe").hide;
                            $("#colObsNFSe").hide();
                        }

                        $("#divEmissaoNFSes").modal({ keyboard: false, backdrop: 'static' });
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function FecharTelaEmissaoNFSes() {
            LimparCamposEmissaoNFSes();

            $("#divEmissaoNFSes").modal('hide');
        }

        function ValidarDadosEmissaoNFSes() {
            var valorFrete = Globalize.parseFloat($("#txtValorFreteNFSe").val());
            var valido = true;

            if (isNaN(valorFrete) || valorFrete <= 0) {
                valido = false;
                CampoComErro("#txtValorFreteNFSe");
            } else {
                CampoSemErro("#txtValorFreteNFSe");
            }

            return valido;
        }

        function EmitirNFSes() {
            if (ValidarDadosEmissaoNFSes()) {

                var dados = {
                    CodigoDocumentoTransporte: $("body").data("documentoTransporteEmissaoNFSe").Codigo,
                    ValorFrete: $("#txtValorFreteNFSe").val()
                };

                executarRest("/IntegracaoNatura/EmitirNFSes?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        FecharTelaEmissaoNFSes();
                        AtualizarGridDocumentosTransporte();
                        ExibirMensagemSucesso("As NFS-es foram geradas com sucesso e estão em digitação!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoNFSes");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "messages-placeholderEmissaoNFSes");
            }
        }

        function AbrirTelaNFSesEmitidas(documentoTransporte) {
            $("body").data("codigoDocumentoTransporteNFSe", documentoTransporte.data.Codigo);
            AtualizarNFSesEmitidas();
            $("#divNFSesEmitidas").modal({ keyboard: false, backdrop: 'static' });
        }

        function AtualizarNFSesEmitidas() {
            var dados = {
                NumeroDocumentoTransporte: $("body").data("codigoDocumentoTransporteNFSe")
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Baixar DANFSE", Evento: BaixarDANFSE });
            opcoes.push({ Descricao: "Baixar XML", Evento: BaixarXMLAutorizacao });
            opcoes.push({ Descricao: "Remover NFS-e", Evento: RemoverNFSeDoDT });

            CriarGridView("/IntegracaoNatura/ConsultarNFSesEmitidas?callback=?", dados, "tbl_paginacao_nfse_documentos_transporte_table", "tbl_nfse_documentos_transporte", "tbl_paginacao_nfse_documentos_transporte", opcoes, [0, 1], null);
        }

        function BaixarDANFSE(nfse) {
            executarDownload("/NotaFiscalDeServicosEletronica/DownloadDANFSE", { CodigoNFSe: nfse.data.Codigo });
        }

        function BaixarXMLAutorizacao(nfse) {
            executarDownload("/NotaFiscalDeServicosEletronica/DownloadXMLAutorizacao", { CodigoNFSe: nfse.data.Codigo });
        }

        function RemoverNFSeDoDT(data) {
            jConfirm("Deseja realmente remover o NFSe-e " + data.data.Numero + " do documento de transporte?", "Atenção!", function (resp) {
                if (resp) {
                    executarRest("/IntegracaoNatura/RemoverNFSeDocumentoTransporte?callback=?", { CodigoNFSe: data.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("NFS-e removida com sucesso.", "Sucesso!", "messages-placeholderNFSesEmitidas");
                            AtualizarNFSesEmitidas();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderNFSesEmitidas");
                        }
                    });
                }
            });
        }

        function AbrirTelaIntegracoesRetorno(documentoTransporte) {
            $("body").data("codigoDocumentoTransporteIntegracaoRetorno", documentoTransporte.data.Codigo);
            AtualizarIntegracoesRetorno();
            $("#divIntegracoesRetorno").modal({ keyboard: false, backdrop: 'static' });
        }

        function AtualizarIntegracoesRetorno() {
            var dados = {
                NumeroDocumentoTransporte: $("body").data("codigoDocumentoTransporteIntegracaoRetorno")
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Baixar XML", Evento: BaixarXMLEnvioRetornoDT });

            CriarGridView("/IntegracaoNatura/ConsultarIntegracoesDT?callback=?", dados, "tbl_paginacao_integracoes_retorno_documentos_transporte_table", "tbl_integracoes_retorno_documentos_transporte", "tbl_paginacao_integracoes_retorno_documentos_transporte", opcoes, [0, 1], null);
        }

        function BaixarXMLEnvioRetornoDT(naturaXML) {
            executarDownload("/IntegracaoNatura/DownloadXMLNatura", { Codigo: naturaXML.data.Codigo });
        }


    </script>
    <script type="text/javascript" id="ScriptImportacaoNOTFIS">
        var path = "";
        var countArquivosNOTFIS = 0;

        $(document).ready(function () {
            if (document.location.pathname.split("/").length > 1) {
                var paths = document.location.pathname.split("/");
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") {
                        path += "/" + paths[i];
                    }
                }
            }

            $("#btnImportarNOTFIS").click(function () {
                AbrirDivUploadArquivosNOTFIS();
            });
        });

        function AbrirDivUploadArquivosNOTFIS() {
            //InicializarPlUploadNOTFIS();
            //$('#divUploadArquivoNOTFIS').modal("show");
            AbrirUploadPadrao({
                title: "Importação de NOTFIS",
                url: "/IntegracaoNatura/ImportarNOTFIS?callback=?",
                filter: [
                    { title: 'Arquivos TXT', extensions: 'txt' }
                ],
                multiple: true,
                max_file_size: '2000kb',
                onFinish: function (arquivos, erros) {
                    if (erros.length > 0) {
                        var uuErros = [];
                        for (var i in erros)
                            if ($.inArray(erros[i], uuErros) < 0)
                                uuErros.push(erros[i]);
                        ExibirMensagemErro(uuErros.join("<br>"), "Erro no envio de NOTFIS:<br>");
                    }
                }
            });
        }

        function FecharDivUploadArquivosNOTFIS() {
            $('#divUploadArquivoNOTFIS').modal("hide");
        }

        function InicializarPlUploadNOTFIS() {
            countArquivosNOTFIS = 0;
            $("#divUploadArquivoNOTFISBody").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/IntegracaoNatura/ImportarNOTFIS?callback=?',
                max_file_size: '2000kb',
                unique_names: true,
                //filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    FilesAdded: function (up, files) {
                        countArquivosNOTFIS += files.length;
                        if (countArquivosNOTFIS > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo por vez. Remova os demais!', 'Atenção');
                        }
                    },
                    FilesRemoved: function (up, files) {
                        countArquivosNOTFIS -= files.length;
                        if (countArquivosNOTFIS <= 1) {
                            $(".plupload_start").css("display", "");
                        }
                    },
                    FileUploaded: function (up, file, info) {
                        var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                        if (retorno.Sucesso) {
                            jAlert("O NOTFIS foi importado com sucesso!<br/><br/>DT disponível para geração.", "Sucesso", function () {
                                AtualizarGridDocumentosTransporte();
                                FecharDivUploadArquivosNOTFIS();
                            });
                        } else {
                            file.status = plupload.FAILED;
                            jAlert(retorno.Erro, "Falha no Envio", function () {
                                FecharDivUploadArquivosNOTFIS();
                            });
                            up.trigger('UploadProgress', file);
                        }
                    }
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Natura - Documentos de Transporte
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnAbrirTelaConsultaDocumentosTransporte" class="btn btn-default"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar Documento(s) de Transporte</button>
    <button type="button" id="btnImportarNOTFIS" class="btn btn-default"><span class="glyphicon glyphicon-globe"></span>&nbsp;Importar NOTFIS Natura</button>
    <div class="row" style="margin-top: 10px; margin-bottom: 5px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº DT:
                </span>
                <input type="text" id="txtNumeroDocumentoTransporteFiltro" class="form-control" maxlength="12" />
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº NF-e:
                </span>
                <input type="text" id="txtNumeroNFe" class="form-control" maxlength="12" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatusFiltro" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Em Digitação</option>
                    <option value="1">Em Emissão</option>
                    <option value="2">Emitido</option>
                    <option value="3">Retornado</option>
                    <option value="4">Finalizado</option>
                    <option value="5">Cancelado</option>
                    <option value="9">Erro Consulta</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridDocumentosTransporte" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Buscar / Atualizar DTs</button>
    <div id="tbl_documentos_transporte" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_documentos_transporte">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divCTesEmitidos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">CT-e(s) emitidos para a DT</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderCTesEmitidos"></div>
                    <button type="button" id="btnConsultarCTe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar CT-e</button>
                    <div id="tbl_cte_documentos_transporte" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_cte_documentos_transporte">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divIntegracoesRetorno" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Integrações Retorno Documento de Transporte</h4>
                </div>
                <div class="modal-body">
                    <button type="button" id="btnConsultarIntegracoesRetorno" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="messages-placeholderIntegracoesRetornos">
                    </div>
                    <div id="tbl_integracoes_retorno_documentos_transporte" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_integracoes_retorno_documentos_transporte">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divNFSesEmitidas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">NFS-e(s) emitidas para a DT</h4>
                </div>
                <div class="modal-body">
                    <button type="button" id="btnConsultarNFSe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar NFS-e</button>
                    <div id="messages-placeholderNFSesEmitidas">
                    </div>
                    <div id="tbl_nfse_documentos_transporte" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_nfse_documentos_transporte">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCTesComplementares" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emitir CT-e Complementar</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderEmissaoCTeComplementar">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Valor CT-e Complementar*:
                                </span>
                                <input type="text" id="txtValorComplemento" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" checked="checked" id="ckbIncluirICMSFrete" />
                                        Incluir ICMS no Frete?
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Observação no CT-e:
                                </span>
                                <textarea style="width: 100%" rows="3" id="txtObservacaoCTeComplementar" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirCTeComplementar" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CT-e Complementar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de CT-es do Documento de Transporte</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderEmissaoCTes">
                    </div>
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Origem:
                                </span>
                                <input type="text" id="txtOrigem" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Qtd. Notas DT:
                                </span>
                                <input type="text" id="txtQtdNotasCTe" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Destino:
                                </span>
                                <input type="text" id="txtDestino" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Veículo*:
                                </span>
                                <input type="text" id="txtVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="colModeloVeicularCarga">
                            <div class="input-group">
                                <span class="input-group-addon">Mod. Carga*:
                                </span>
                                <input type="text" id="txtModeloVeicularCarga" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarModeloVeicularCarga" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Motorista*:
                                </span>
                                <input type="text" id="txtMotorista" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="colTipoOperacao">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Operação*:
                                </span>
                                <input type="text" id="txtTipoOperacao" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTipoOperacao" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="colTipoCarga">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Carga*:
                                </span>
                                <input type="text" id="txtTipoCarga" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTipoCarga" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Vl. Frete Total DT*:
                                </span>
                                <input type="text" id="txtValorFrete" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="colValorFreteCalculado">
                            <div class="input-group">
                                <span class="input-group-addon">Vl. Frete Calculado*:
                                </span>
                                <input type="text" id="txtValorFreteCalculado" class="form-control" disabled />
                                <span class="input-group-btn">
                                    <button type="button" id="btnCalcularFrete" class="btn btn-primary">Calcular</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Observação CTe">Observação CT-e</abbr>:
                                </span>
                                <textarea id="txtObservacaoCTe" class="form-control" rows="2"> </textarea>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-12" id="colObsCTe">
                            <div class="input-group">
                                <span class="input-group-addon">Atenção:
                                </span>
                                <input type="text" id="txtObservacao" class="form-control" disabled />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirCTes" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CT-es</button>
                    <button type="button" id="btnCancelarEmissaoCTes" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoNFSes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de NFS-es do Documento de Transporte</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderEmissaoNFSes">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Origem:
                                </span>
                                <input type="text" id="txtOrigemNFSe" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Destino:
                                </span>
                                <input type="text" id="txtDestinoNFSe" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Qtd. Notas DT:
                                </span>
                                <input type="text" id="txtQtdNotasNFSe" class="form-control" disabled />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Vl. Frete Total DT*:
                                </span>
                                <input type="text" id="txtValorFreteNFSe" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-12"id="colObsNFSe">
                            <div class="input-group">
                                <span class="input-group-addon">Observação:
                                </span>
                                <textarea style="width: 100%" rows="1" id="txtObservacaoNFSe" class="form-control" disabled></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirNFSes" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir NFS-es</button>
                    <button type="button" id="btnCancelarEmissaoNFSes" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divConsultaDocumentosTransporte" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de Documentos de Transporte</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderConsultaDocumentosTransporte">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nº DT:
                                </span>
                                <input type="text" id="txtNumeroDocumentoTransporteConsulta" class="form-control" />
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
                    <button type="button" id="btnConsultarDocumentosTransporte" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Consultar DTs</button>
                    <button type="button" id="btnFecharConsultaDocumentosTransporte" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divUploadArquivoNOTFIS" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Upload de NOTFIS</h4>
                </div>
                <div class="modal-body">
                    <div id="divUploadArquivoNOTFISBody">
                        Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
