<%@ Page Title="Emissão de CT-e para Minutas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoAvon.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoAvon" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            LimparCampos();

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
            CarregarConsultaDeFretesPorTipoDeVeiculo("btnBuscarTabelaFrete", "btnBuscarTabelaFrete", "A", RetornoConsultaFrete, true, false);

            $("#txtNumeroManifestoFiltro").priceFormat({ centsLimit: 0, centsSeparator: '' });
            $("#txtNumeroManifesto").priceFormat({ centsLimit: 0, centsSeparator: '' });
            $("#txtValorFrete").priceFormat();

            $("#txtDataInicialFiltro").mask("99/99/9999");
            $("#txtDataFinalFiltro").mask("99/99/9999");

            $("#txtDataInicialFiltro").datepicker();
            $("#txtDataFinalFiltro").datepicker();

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoMotorista", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#chkOpcaoAvancada").click(function () {
                ExibirOcultarAvancadas();
            })

            $("#btnEmitirCTes").click(function () {
                EmitirCTes();
            });

            $("#btnCriarMinutaManual").click(function () {
                CriarMinutaManual();
            });


            $("#btnCancelar").click(function () {
                LimparCampos();
                FecharTelaEmissaoManifesto();
            });

            $("#btnNovoManifesto").click(function () {
                AbrirTelaEmissaoManifesto("MinutaViaWS");
            });

            $("#btnInfomarManifestoManual").click(function () {
                AbrirTelaEmissaoManifesto("MinutaManual");
            });

            $("#btnAtualizarGridManifestos").click(function () {
                AtualizarGridManifestos();
            });

            $("#selAliquotaICMS").change(function () {
                AtualizarValorAReceber();
            });

            AtualizarGridManifestos();
            CarregarAliquotasICMS();
        });

        function CarregarAliquotasICMS() {
            executarRest("/AliquotaDeICMS/ObterAliquotasDaEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    for (var i = 0; i < r.Objeto.length; i++)
                        $("#selAliquotaICMS").append('<option value="' + r.Objeto[i].Aliquota + '">' + Globalize.format(r.Objeto[i].Aliquota, "n2") + "%</option>");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function ExibirOcultarAvancadas() {
            if ($("#chkOpcaoAvancada")[0].checked) {
                $("#divOpcoesAvancadas").show();
            } else {
                $("#divOpcoesAvancadas").hide();
            }
        }

        function AtualizarGridManifestos() {
            var dados = {
                NumeroManifesto: $("#txtNumeroManifestoFiltro").val(),
                DataInicial: $("#txtDataInicialFiltro").val(),
                DataFinal: $("#txtDataFinalFiltro").val(),
                Integradora: $("#selIntegradoraFiltro").val(),
                NumeroCTe: $("#txtNumeroCTe").val(),
                inicioRegistros: 0
            };

            var opcoes = new Array();
            //opcoes.push({ Descricao: "Enviar Retorno", Evento: EnviarRetorno });
            opcoes.push({ Descricao: "Verificar Retornos", Evento: AbrirTelaDocumentosRetornados });
            opcoes.push({ Descricao: "Gerar MDF-e", Evento: AbrirTelaEmissaoMDFe });
            //opcoes.push({ Descricao: "Consultar Status SEFAZ", Evento: ConsultarStatusSEFAZ });

            CriarGridView("/IntegracaoAvon/Consultar?callback=?", dados, "tbl_manifestos_table", "tbl_manifestos", "tbl_paginacao_manifestos", opcoes, [0, 1], null);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("codigoVeiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);

            executarRest("/Veiculo/ObterMotoristaDoVeiculo?callback=?", { CodigoVeiculo: veiculo.Codigo }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.CodigoMotorista > 0) {
                        $("body").data("codigoMotorista", r.Objeto.CodigoMotorista);
                        $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                    } else {
                        $("body").data("codigoMotorista", null);
                        $("#txtMotorista").val('');
                    }
                }
            });
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("codigoMotorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function RetornoConsultaFrete(frete) {
            executarRest("/FretePorTipoDeVeiculo/ObterDetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("body").data("codigoTabelaFrete", r.Objeto.Codigo);

                    $("#txtTabelaFrete").val(r.Objeto.UFOrigem + " / " + r.Objeto.DescricaoLocalidadeOrigem + " -> " + r.Objeto.UFDestino + " / " + r.Objeto.DescricaoLocalidadeDestino);

                    $("#txtValorFrete").val(r.Objeto.ValorFrete);
                    $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
                    $("#selAliquotaICMS").val(r.Objeto.AliquotaICMS).change();

                    AtualizarValorAReceber();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function ValidarDados() {
            var numeroManifesto = Globalize.parseInt($("#txtNumeroManifesto").val());
            var codigoVeiculo = $("body").data("codigoVeiculo");
            var codigoMotorista = $("body").data("codigoMotorista");
            var codigoTabelaFrete = $("body").data("codigoTabelaFrete");
            var valorAReceber = Globalize.parseFloat($("#txtValorTotalReceber").val());
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var aliquotaICMS = $("#selAliquotaICMS").val();
            var valido = true;

            if (!isNaN(numeroManifesto) && numeroManifesto > 0) {
                CampoSemErro("#txtNumeroManifesto");
            } else {
                CampoComErro("#txtNumeroManifesto");
                valido = false;
            }

            if (!isNaN(valorAReceber) && valorAReceber > 0) {
                CampoSemErro("#txtValorTotalReceber");
            } else {
                CampoComErro("#txtValorTotalReceber");
                valido = false;
            }

            if (!isNaN(valorFrete) && valorFrete > 0) {
                CampoSemErro("#txtValorFrete");
            } else {
                CampoComErro("#txtValorFrete");
                valido = false;
            }

            if (codigoVeiculo != null && codigoVeiculo > 0) {
                CampoSemErro("#txtVeiculo");
            } else {
                CampoComErro("#txtVeiculo");
                valido = false;
            }

            if (codigoMotorista != null && codigoMotorista > 0) {
                CampoSemErro("#txtMotorista");
            } else {
                CampoComErro("#txtMotorista");
                valido = false;
            }

            if (codigoTabelaFrete != null && codigoTabelaFrete > 0) {
                CampoSemErro("#txtTabelaFrete");
            } else {
                CampoComErro("#txtTabelaFrete");
                valido = false;
            }

            if (aliquotaICMS == null) {
                valido = false;
                CampoComErro("#selAliquotaICMS");
            } else {
                CampoSemErro("#selAliquotaICMS");
            }

            return valido;
        }

        function ValidarMinutaManual() {
            var valido = true;

            var numeroManifesto = Globalize.parseInt($("#txtNumeroManifesto").val());
            var codigoVeiculo = $("body").data("codigoVeiculo");
            var codigoMotorista = $("body").data("codigoMotorista");

            if (!isNaN(numeroManifesto) && numeroManifesto > 0) {
                CampoSemErro("#txtNumeroManifesto");
            } else {
                CampoComErro("#txtNumeroManifesto");
                valido = false;
            }

            if (codigoVeiculo != null && codigoVeiculo > 0) {
                CampoSemErro("#txtVeiculo");
            } else {
                CampoComErro("#txtVeiculo");
                valido = false;
            }

            if (codigoMotorista != null && codigoMotorista > 0) {
                CampoSemErro("#txtMotorista");
            } else {
                CampoComErro("#txtMotorista");
                valido = false;
            }

            return valido;
        }

        function CriarMinutaManual() {
            jConfirm("Deseja realmente criar uma minuta manual com o número " + $("#txtNumeroManifesto").val() + ", lembrando que essa minuta não será enviada via integração para a Avon?", "Atenção!", function (confirm) {
                if (confirm) {
                    if (ValidarMinutaManual()) {
                        executarRest("/IntegracaoAvon/CriarMinutalManual?callback=?", {
                            NumeroManifesto: $("#txtNumeroManifesto").val(),
                            CodigoVeiculo: $("body").data("codigoVeiculo"),
                            CodigoMotorista: $("body").data("codigoMotorista")
                        }, function (r) {
                            if (r.Sucesso) {
                                ExibirMensagemSucesso("Manifesto criado com Sucesso!", "Sucesso!");
                                LimparCampos();
                                FecharTelaEmissaoManifesto();
                                AtualizarGridManifestos();
                            } else {
                                ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderManifesto");
                            }
                        });
                    } else {
                        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholderManifesto");
                    }
                }
            });

        }

        function EmitirCTes() {
            if (ValidarDados()) {
                executarRest("/IntegracaoAvon/EmitirCTes?callback=?", {
                    NumeroManifesto: $("#txtNumeroManifesto").val(),
                    CodigoVeiculo: $("body").data("codigoVeiculo"),
                    CodigoMotorista: $("body").data("codigoMotorista"),
                    CodigoTabelaFrete: $("body").data("codigoTabelaFrete"),
                    ValorPedagio: $("#txtValorPedagio").val(),
                    ValorFrete: $("#txtValorFrete").val(),
                    AliquotaICMS: $("#selAliquotaICMS").val(),
                    ValorAReceber: $("#txtValorTotalReceber").val(),
                    TipoIntegradora: $("#selTipoIntegradora").val(),
                    GroupId: $("#selStatusConsultaMinutaAvon").val(),
                }, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("CT-es Emitidos com Sucesso!", "Sucesso!");
                        LimparCampos();
                        FecharTelaEmissaoManifesto();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderManifesto");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholderManifesto");
            }
        }

        function LimparCampos() {
            $("body").data("codigoVeiculo", null);
            $("body").data("codigoMotorista", null);
            $("body").data("codigoTabelaFrete", null);

            $("#txtVeiculo").val("");
            $("#txtMotorista").val("");
            $("#txtTabelaFrete").val("");
            $("#txtNumeroManifesto").val("");
            $("#txtValorFrete").val("0,00");
            $("#txtValorTotalReceber").val("0,00");
            $("#selAliquotaICMS").val($("#selAliquotaICMS option:first").val());
            $("#txtValorPedagio").val("0,00");
        }

        function AbrirTelaEmissaoManifesto(tipoMinuta) {
            LimparCampos();
            if (tipoMinuta == "MinutaViaWS") {
                $("#selStatusConsultaMinutaAvon").val("OpenedManifestSearchIdPrimary");
                $("#btnCriarMinutaManual").hide();
                $("#btnEmitirCTes").show();
                $("#divConteudoViaIntegracao").show();
            } else {
                $("#btnCriarMinutaManual").show();
                $("#btnEmitirCTes").hide();
                $("#divConteudoViaIntegracao").hide();
            }
            $("#divEmissaoManifesto").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaEmissaoManifesto() {
            $("#divEmissaoManifesto").modal('hide');
        }

        function AtualizarValorAReceber() {
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var valorPedagio = Globalize.parseFloat($("#txtValorPedagio").val());
            var aliquotaICMS = Globalize.parseFloat($("#selAliquotaICMS").val());

            var valorAReceber = ((valorFrete + valorPedagio) / (1 - (aliquotaICMS / 100)));

            $("#txtValorTotalReceber").val(Globalize.format(valorAReceber, "n2"));
        }
    </script>
    <script id="ScriptRetornos" type="text/javascript">
        $(document).ready(function () {
            $("#btnFecharEnvioRetornos").click(function () {
                FecharTelaEnvioRetornos();
            });
            $("#btnConsultarDocumentosRetorno").click(function () {
                AtualizarEnvioRetornos();
            });

            $("#btnRetornosComFalha").click(function () {
                SolicitarReenvioRetornosComFalha();
            });

            $("#btnReenviarDocumentos").click(function () {
                SolicitarReenvioTodosDocumentos();
            });
        });

        var documentos, countDocumentosProcessados;

        function EnviarRetorno(manifesto) {
            jConfirm("Deseja realmente enviar os retornos da minuta nº " + manifesto.data.Numero + "?", "Atenção!", function (confirm) {
                if (confirm) {
                    documentos = new Array();
                    countDocumentosProcessados = 0;

                    iniciarRequisicao();
                    executarRest("/IntegracaoAvon/ObterDocumentosDoManifestoParaEnvio?callback=?", { CodigoManifesto: manifesto.data.Codigo }, function (r) {
                        finalizarRequisicao();
                        if (r.Sucesso) {
                            documentos = r.Objeto;

                            documentos = documentos.sort();

                            RenderizarDocumentos(documentos);

                            $("#btnFecharEnvioRetornos").prop("disabled", true);

                            AbrirTelaEnvioRetornos();

                            EnviarRetornos(documentos);

                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    }, false);
                }
            });
        }

        function EnviarRetornos(documentos) {
            for (var i = 0; i < documentos.length; i += 50) {

                var documentosSelecionados = new Array();

                for (var x = i; x < (i + 50) ; x++) {
                    if (documentos[x] != null)
                        documentosSelecionados.push(documentos[x].Codigo);
                    else
                        break;
                }

                executarRest("/IntegracaoAvon/EnviarRetornoDocumento?callback=?", { CodigosDocumentos: JSON.stringify(documentosSelecionados) }, function (r) {

                    countDocumentosProcessados += r.Objeto.Documentos != null ? r.Objeto.Documentos.length : 0;

                    if (r.Sucesso) {
                        for (var x = 0; x < r.Objeto.Documentos.length; x++) {
                            $("#trRetorno_" + r.Objeto.Documentos[x]).find('td').eq(3).html(r.Objeto.Mensagem);
                            $("#trRetorno_" + r.Objeto.Documentos[x]).find('td').eq(2).html("Sucesso");
                            $("#trRetorno_" + r.Objeto.Documentos[x]).addClass("success");
                        }
                    } else if (r.Objeto.Documentos != null) {
                        for (var x = 0; x < r.Objeto.Documentos.length; x++) {
                            $("#trRetorno_" + r.Objeto.Documentos[x]).find('td').eq(3).html(r.Objeto.Mensagem);
                            $("#trRetorno_" + r.Objeto.Documentos[x]).find('td').eq(2).html("Falha");
                            $("#trRetorno_" + r.Objeto.Documentos[x]).addClass("danger");
                        }
                    } else {
                        jAlert(r.Erro, "Atenção!");
                    }

                    if (countDocumentosProcessados == documentos.length) {
                        $("#btnFecharEnvioRetornos").prop("disabled", false);
                        jAlert("Os retornos foram enviados. Verifique se ocorreu alguma falha nos envios.", "Atençao!");
                    }

                }, false);
            }
        }

        function SolicitarReenvioRetornosComFalha() {
            iniciarRequisicao();
            executarRest("/IntegracaoAvon/ReenviarDocumentosComFalhaNaIntegracao?callback=?", { CodigoManifesto: _CodigoMinutaAvon }, function (r) {
                finalizarRequisicao();
                if (r.Sucesso) {
                    $("#divRetornoDocumentos").modal('hide');
                    ExibirMensagemSucesso("Solicitação de Re-envio feita com sucesso!", "Sucesso!");
                    AtualizarGridManifestos();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            }, false);
        }

        function SolicitarReenvioTodosDocumentos() {
            jConfirm("Deseja realmente Re-enviar todos CTes da minuta?", "Atenção!", function (confirm) {
                iniciarRequisicao();
                executarRest("/IntegracaoAvon/ReenviarTodosDocumentos?callback=?", { CodigoManifesto: _CodigoMinutaAvon }, function (r) {
                    finalizarRequisicao();
                    if (r.Sucesso) {
                        $("#divRetornoDocumentos").modal('hide');
                        ExibirMensagemSucesso("Solicitação de Re-envio feita com sucesso!", "Sucesso!");
                        AtualizarGridManifestos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                }, false);
            });
        }

        function RenderizarDocumentos(documentos) {
            $("#tblDocumentosParaRetorno tbody").html("");

            for (var i = 0; i < documentos.length; i++)
                $("#tblDocumentosParaRetorno tbody").append("<tr id='trRetorno_" + documentos[i].Codigo + "'><td>" + documentos[i].NumeroNFe + " - " + documentos[i].SerieNFe + "</td><td>" + documentos[i].NumeroCTe + " - " + documentos[i].SerieCTe + "</td><td>Pendente</td><td>Aguardando envio da solicitação...</td></tr>");

            if ($("#tblDocumentosParaRetorno tbody").html() == "")
                $("#tblDocumentosParaRetorno tbody").html("<tr><td colspan='4'>Nenhum registro encontrado!</td></tr>");
        }

        function AbrirTelaEnvioRetornos() {
            $("#divEnvioRetorno").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaEnvioRetornos() {
            $("#divEnvioRetorno").modal('hide');
        }

        var _CodigoMinutaAvon = 0;
        function AbrirTelaDocumentosRetornados(minuta) {
            _CodigoMinutaAvon = minuta.data.Codigo;
            if (minuta.data.CodStatus == 3) {
                $("#btnRetornosComFalha").show();
            } else {
                $("#btnRetornosComFalha").hide();
            }
            $("#selSituacaoConsulta").val(99);
            AtualizarEnvioRetornos();
            $("#divRetornoDocumentos").modal({ keyboard: false, backdrop: 'static' });
        }


        function AtualizarEnvioRetornos() {
            var dados = {
                MinutaAvon: _CodigoMinutaAvon,
                StatusDocumentoManifestoAvon: $("#selSituacaoConsulta").val()
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Baixar DACTE", Evento: DownloadDacte });
            opcoes.push({ Descricao: "Baixar XML", Evento: DownloadXML });
            opcoes.push({ Descricao: "Remover CT-e", Evento: RemoverCTeMinuta });
            //opcoes.push({ Descricao: "Gerar CT-e Complementar", Evento: abrirModalCTEComplementar });

            CriarGridView("/IntegracaoAvon/ConsultarDocumentosRetornoAvon?callback=?", dados, "tbl_documentos_retorno_table", "tbl_documentos_retorno", "tbl_paginacao_documentos_retorno", opcoes, [0, 1, 2, 3], null);
        }

        function DownloadDacte(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadDacte", { CodigoCTe: cte.data.CodigoCTe });
        }

        function DownloadXML(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXML", { CodigoCTe: cte.data.CodigoCTe });
        }

        function RemoverCTeMinuta(data) {
            jConfirm("Deseja realmente remover o CT-e " + data.data.Numero + " da minuta?", "Atenção!", function (resp) {
                if (resp) {
                    executarRest("/IntegracaoAvon/RemoverCTeMinuta?callback=?", { CodigoCTe: data.data.CodigoCTe }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("CT-e removido com sucesso.", "Sucesso!", "messages-placeholderDocumentosRetorno");
                            AtualizarEnvioRetornos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderDocumentosRetorno");
                        }
                    });
                }
            });
        }

    </script>
    <script id="ScriptMDFe" type="text/javascript">
        $(document).ready(function () {
            $("#btnEmitirMDFe").click(function () {
                GerarMDFe();
            });

            $("#btnCancelarMDFe").click(function () {
                FecharTelaEmissaoMDFe();
            });
        });

        function GerarMDFe() {
            var manifesto = $("body").data("manifestoEmissaoMDFe");

            executarRest("/IntegracaoAvon/GerarMDFeDoManifesto?callback=?", { CodigoManifesto: manifesto.Codigo, NumeroLacre: $("#txtNumeroLacreMDFe").val() }, function (r) {
                if (r.Sucesso) {
                    FecharTelaEmissaoMDFe();
                    ExibirMensagemSucesso("MDF-e emitido com sucesso.", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderEmissaoMDFe");
                }
            });
        }

        function AbrirTelaEmissaoMDFe(manifesto) {
            LimparCamposEmissaoMDFe();

            $("body").data("manifestoEmissaoMDFe", manifesto.data);

            $("#divEmissaoMDFe").modal({ keyboard: false });
        }

        function FecharTelaEmissaoMDFe() {
            $("#divEmissaoMDFe").modal("hide");
        }

        function LimparCamposEmissaoMDFe() {
            $("body").data("manifestoEmissaoMDFe", null);
            $("#txtNumeroLacreMDFe").val("");
        }
    </script>
    <script id="ScriptConsultaStatusSEFAZ" type="text/javascript">
        function ConsultarStatusSEFAZ(manifesto) {
            jConfirm("Deseja realmente consultar o status do SEFAZ dos CT-es da minuta nº " + manifesto.data.Numero + "?", "Atenção!", function (confirm) {
                if (confirm) {
                    executarRest("/IntegracaoAvon/ConsultarStatusSEFAZCTes?callback=?", { CodigoManifesto: manifesto.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Consulta iniciada sucesso.", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Emissão de CT-e para Minutas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovoManifesto" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Emitir CT-es de Minuta</button>
    <button type="button" id="btnInfomarManifestoManual" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Criar Manifesto Manual</button>
    <div class="row" style="margin-top: 10px; margin-bottom: 5px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Minuta:
                </span>
                <input type="text" id="txtNumeroManifestoFiltro" class="form-control" maxlength="10" />
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
                <span class="input-group-addon">Tipo Minuta:
                </span>
                <select id="selIntegradoraFiltro" class="form-control">
                    <option value="">Todas</option>
                    <option value="1">Avon</option>
                    <option value="3">Manual</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº CT-e:
                </span>
                <input type="text" id="txtNumeroCTe" class="form-control" maxlength="10" />
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridManifestos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Buscar / Atualizar Minutas</button>
    <div id="tbl_manifestos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_manifestos">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divEnvioRetorno" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Envio de Retornos da Minuta</h4>
                </div>
                <div class="modal-body" style="max-height: 750px; overflow-y: scroll;">
                    <div id="messages-placeholderEnvioRetorno">
                    </div>
                    <table id="tblDocumentosParaRetorno" class="table table-responsive table-bordered table-condensed">
                        <thead>
                            <tr>
                                <th>NF-e
                                </th>
                                <th>CT-e
                                </th>
                                <th>Status
                                </th>
                                <th>Retorno
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnFecharEnvioRetornos" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoManifesto" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de CT-es da Minuta</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderManifesto">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Nº Minuta*:
                                </span>
                                <input type="text" id="txtNumeroManifesto" class="form-control" maxlength="20" />
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
                        <div id="divConteudoViaIntegracao">
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tabela*:
                                    </span>
                                    <input type="text" id="txtTabelaFrete" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarTabelaFrete" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Vl. Frete:
                                    </span>
                                    <input type="text" id="txtValorFrete" class="form-control" disabled="disabled" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Vl. Pedágio:
                                    </span>
                                    <input type="text" id="txtValorPedagio" class="form-control" disabled="disabled" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Alíq. ICMS:
                                    </span>
                                    <select id="selAliquotaICMS" class="form-control">
                                    </select>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Vl. Receber:
                                    </span>
                                    <input type="text" id="txtValorTotalReceber" class="form-control" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none">
                                <div class="input-group">
                                    <span class="input-group-addon">Integradora*:
                                    </span>
                                    <select id="selTipoIntegradora" class="form-control">
                                        <option value="1">Avon</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                <div class="input-group">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkOpcaoAvancada" />
                                            Exibir opção avançada de Busca.
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divOpcoesAvancadas" style="display: none">
                                <div class="input-group">
                                    <span class="input-group-addon">Manifest Search Avon:
                                    </span>
                                    <select id="selStatusConsultaMinutaAvon" class="form-control">
                                        <option value="OpenedManifestSearchIdPrimary">Opened</option>
                                        <option value="ClosedManifestSearchIdPrimary">Closed</option>
                                        <option value="ManifestSearchDocument">Manifest</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnCriarMinutaManual" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Criar Minuta Manual</button>
                    <button type="button" id="btnEmitirCTes" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CT-es</button>
                    <button type="button" id="btnCancelar" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divRetornoDocumentos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Retorno de Documentos para Avon</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderDocumentosRetorno">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Situação Documento*:
                                </span>
                                <select id="selSituacaoConsulta" class="form-control">
                                    <option value="99">Todas</option>
                                    <option value="0">CT-e(s) em Emissão</option>
                                    <option value="1">Aguardando Envio Avon</option>
                                    <option value="2">Retornadas Avon com sucesso</option>
                                    <option value="3">Falha ao enviar Retorno Avon</option>
                                </select>
                            </div>
                        </div>
                        <button type="button" style="margin-left: 10px;" id="btnConsultarDocumentosRetorno" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar Retornos</button>
                    </div>

                    <button type="button" id="btnRetornosComFalha" style="display: none;" class="btn btn-primary">&nbsp;Re-enviar Retornos com Falha</button>
                    <button type="button" id="btnReenviarDocumentos" class="btn btn-primary">&nbsp;Re-enviar Todos</button>

                    <div id="tbl_documentos_retorno" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_documentos_retorno">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divEmissaoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de MDF-e</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderEmissaoMDFe">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Lacre*:
                                </span>
                                <input type="text" id="txtNumeroLacreMDFe" class="form-control" maxlength="60" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir MDF-e</button>
                    <button type="button" id="btnCancelarMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
