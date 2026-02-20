<%@ Page Title="Relatório de CT-es Emitidos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCTesEmitidos.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioCTesEmitidos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        var path = "";
        if (document.location.pathname.split("/").length > 1) {
            var paths = document.location.pathname.split("/");
            for (var i = 0; (paths.length - 1) > i; i++) {
                if (paths[i] != "") {
                    path += "/" + paths[i];
                }
            }
        }
        $(document).ready(function () {
            $("#txtDataEmissaoInicial").mask("99/99/9999");
            $("#txtDataEmissaoFinal").mask("99/99/9999");
            $("#txtDataEmissaoInicial").datepicker();
            $("#txtDataEmissaoFinal").datepicker();

            $("#txtDataAutorizacaoInicial").mask("99/99/9999");
            $("#txtDataAutorizacaoFinal").mask("99/99/9999");
            $("#txtDataAutorizacaoInicial").datepicker();
            $("#txtDataAutorizacaoFinal").datepicker();

            $("#txtNumeroInicial").mask("9?9999999999999");
            $("#txtNumeroFinal").mask("9?9999999999999");
            $("#txtCPFMotorista").mask("9?9999999999");
            $("#txtNumeroNotaFiscal").mask("9?9999999999999");

            ObterEstados();

            $("#selUfInicio").change(function () {
                ObterMunicipios($(this).val(), "selLocalidadeInicio");
            });

            $("#selUfFim").change(function () {
                ObterMunicipios($(this).val(), "selLocalidadeFim");
            });

            $("#btnGerarRelatorioJS").click(function () {
                var dados = {
                    Remetente: $("#hddRemetente").val(),
                    Expedidor: $("#hddExpedidor").val(),
                    Recebedor: $("#hddRecebedor").val(),
                    Destinatario: $("#hddDestinatario").val(),
                    Tomador: $("#hddTomador").val(),
                    DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                    DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                    DataAutorizacaoInicial: $("#txtDataAutorizacaoInicial").val(),
                    DataAutorizacaoFinal: $("#txtDataAutorizacaoFinal").val(),
                    Status: $("#ddlStatus").val(),
                    Finalidade: $("#ddlFinalidade").val(),
                    Serie: $("#ddlSerie").val(),
                    NumeroInicial: $("#txtNumeroInicial").val(),
                    NumeroFinal: $("#txtNumeroFinal").val(),
                    NomeMotorista: $("#txtNomeMotorista").val(),
                    CPFMotorista: $("#txtCPFMotorista").val(),
                    Veiculo: $("#txtVeiculo").val(),
                    TipoOcorrencia: $("#ddlTipoOcorrencia").val(),
                    NumeroNotaFiscal: $("#txtNumeroNotaFiscal").val(),
                    SomenteTracao: $("#chkExibirSomenteTracao")[0].checked,
                    Relatorio: $("#ddlTipoRelatorio").val(),
                    TipoArquivo: $("#selTipoArquivo").val(),
                    StatusPagamento: $("#selStatusPagamento").val(),
                    ExibirNotasFiscais: $("#chkExibirNotasFiscais")[0].checked,
                    CodigoLocalidadeInicio: $("#selLocalidadeInicio").val(),
                    CodigoLocalidadeFim: $("#selLocalidadeFim").val(),
                    UfInicio: $("#selUfInicio").val(),
                    UfFim: $("#selUfFim").val(),
                    Duplicata: $("#txtDuplicata").val(),
                    Importacao: $("#chkImportacao")[0].checked,
                    Exportacao: $("#chkExportacao")[0].checked,
                    RaizCNPJRemetente: $("#chkRaizCNPJRemetente")[0].checked,
                    RaizCNPJExpedidor: $("#chkRaizCNPJExpedidor")[0].checked,
                    RaizCNPJRecebedor: $("#chkRaizCNPJRecebedor")[0].checked,
                    RaizCNPJDestinatario: $("#chkRaizCNPJDestinatario")[0].checked,
                    RaizCNPJTomador: $("#chkRaizCNPJTomador")[0].checked,
                    ICMSCTe: $("#selICMSCTe").val(),
                    CSTCTe: $("#selCSTCTe").val(),
                    Servico: $("#ddlTipoServico").val(),
                    NomeUsuario: $("#txtUsuario").data("nome"),
                    CodigoUsuario: $("#txtUsuario").data("codigo"),
                    Observacao: $("#txtObservacao").val(),
                    NumeroCarga: $("#txtNumeroCarga").val(),
                    NumeroUnidade: $("#txtNumeroUnidade").val(),
                    AverbacaoCTe: $("#selAverbacaoCTe").val(),
                    RemoverCliente: $("#chkRemoverCliente")[0].checked
                };

                executarDownload("/RelatorioCTesEmitidos/DownloadRelatorio", dados);
            });

            $("#btnAtualizarGrid").click(function () {
                AtualizarGridCTes();
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddRemetente").val("");
                        $("#txtCPFCNPJRemetente").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJRemetente").focusout(function () {
                BuscarRemetenteFiltro();
            });

            $("#txtExpedidor").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddExpedidor").val("");
                        $("#txtCPFCNPJExpedidor").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJExpedidor").focusout(function () {
                BuscarExpedidorFiltro();
            });

            $("#txtRecebedor").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddRecebedor").val("");
                        $("#txtCPFCNPJRecebedor").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJRecebedor").focusout(function () {
                BuscarRecebedorFiltro();
            });

            $("#txtDestinatario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddDestinatario").val("");
                        $("#txtCPFCNPJDestinatario").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJDestinatario").focusout(function () {
                BuscarDestinatarioFiltro();
            });

            $("#txtTomador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddTomador").val("");
                        $("#txtCPFCNPJTomador").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJTomador").focusout(function () {
                BuscarTomadorFiltro();
            });

            $("#txtTomador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddTomador").val("");
                        $("#txtCPFCNPJTomador").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtUsuario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        for (var i in $(this).data()) $(this).data(i, "");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnBaixarLote").click(function () {
                var dados = {
                    Remetente: $("#hddRemetente").val(),
                    Expedidor: $("#hddExpedidor").val(),
                    Recebedor: $("#hddRecebedor").val(),
                    Destinatario: $("#hddDestinatario").val(),
                    Tomador: $("#hddTomador").val(),
                    DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                    DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                    DataAutorizacaoInicial: $("#txtDataAutorizacaoInicial").val(),
                    DataAutorizacaoFinal: $("#txtDataAutorizacaoFinal").val(),
                    Status: $("#ddlStatus").val(),
                    Finalidade: $("#ddlFinalidade").val(),
                    Serie: $("#ddlSerie").val(),
                    NumeroInicial: $("#txtNumeroInicial").val(),
                    NumeroFinal: $("#txtNumeroFinal").val(),
                    NomeMotorista: $("#txtNomeMotorista").val(),
                    CPFMotorista: $("#txtCPFMotorista").val(),
                    Veiculo: $("#txtVeiculo").val(),
                    TipoOcorrencia: $("#ddlTipoOcorrencia").val(),
                    NumeroNotaFiscal: $("#txtNumeroNotaFiscal").val(),
                    StatusPagamento: $("#selStatusPagamento").val(),
                    CodigoLocalidadeInicio: $("#selLocalidadeInicio").val(),
                    CodigoLocalidadeFim: $("#selLocalidadeFim").val(),
                    UfInicio: $("#selUfInicio").val(),
                    UfFim: $("#selUfFim").val(),
                    Duplicata: $("#txtDuplicata").val(),
                    Importacao: $("#chkImportacao")[0].checked,
                    Exportacao: $("#chkExportacao")[0].checked,
                    RaizCNPJRemetente: $("#chkRaizCNPJRemetente")[0].checked,
                    RaizCNPJExpedidor: $("#chkRaizCNPJExpedidor")[0].checked,
                    RaizCNPJRecebedor: $("#chkRaizCNPJRecebedor")[0].checked,
                    RaizCNPJDestinatario: $("#chkRaizCNPJDestinatario")[0].checked,
                    RaizCNPJTomador: $("#chkRaizCNPJTomador")[0].checked,
                    ICMSCTe: $("#selICMSCTe").val(),
                    CSTCTe: $("#selCSTCTe").val(),
                    Servico: $("#ddlTipoServico").val(),
                    NomeUsuario: $("#txtUsuario").data("nome"),
                    CodigoUsuario: $("#txtUsuario").data("codigo"),
                    Observacao: $("#txtObservacao").val(),
                    RemoverCliente: $("#chkRemoverCliente")[0].checked,
                    NumeroCarga: $("#txtNumeroCarga").val(),
                    NumeroUnidade: $("#txtNumeroUnidade").val(),
                    AverbacaoCTe: $("#selAverbacaoCTe").val(),
                    inicioRegistros: 0
                };

                executarDownload("/RelatorioCTesEmitidos/DownloadLoteXML", dados);
            });

            $("#btnBaixarLoteDACTE").click(function () {
                var dados = {
                    Remetente: $("#hddRemetente").val(),
                    Expedidor: $("#hddExpedidor").val(),
                    Recebedor: $("#hddRecebedor").val(),
                    Destinatario: $("#hddDestinatario").val(),
                    Tomador: $("#hddTomador").val(),
                    DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                    DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                    DataAutorizacaoInicial: $("#txtDataAutorizacaoInicial").val(),
                    DataAutorizacaoFinal: $("#txtDataAutorizacaoFinal").val(),
                    Status: $("#ddlStatus").val(),
                    Finalidade: $("#ddlFinalidade").val(),
                    Serie: $("#ddlSerie").val(),
                    NumeroInicial: $("#txtNumeroInicial").val(),
                    NumeroFinal: $("#txtNumeroFinal").val(),
                    NomeMotorista: $("#txtNomeMotorista").val(),
                    CPFMotorista: $("#txtCPFMotorista").val(),
                    Veiculo: $("#txtVeiculo").val(),
                    TipoOcorrencia: $("#ddlTipoOcorrencia").val(),
                    NumeroNotaFiscal: $("#txtNumeroNotaFiscal").val(),
                    StatusPagamento: $("#selStatusPagamento").val(),
                    CodigoLocalidadeInicio: $("#selLocalidadeInicio").val(),
                    CodigoLocalidadeFim: $("#selLocalidadeFim").val(),
                    UfInicio: $("#selUfInicio").val(),
                    UfFim: $("#selUfFim").val(),
                    Duplicata: $("#txtDuplicata").val(),
                    Importacao: $("#chkImportacao")[0].checked,
                    Exportacao: $("#chkExportacao")[0].checked,
                    RaizCNPJRemetente: $("#chkRaizCNPJRemetente")[0].checked,
                    RaizCNPJExpedidor: $("#chkRaizCNPJExpedidor")[0].checked,
                    RaizCNPJRecebedor: $("#chkRaizCNPJRecebedor")[0].checked,
                    RaizCNPJDestinatario: $("#chkRaizCNPJDestinatario")[0].checked,
                    RaizCNPJTomador: $("#chkRaizCNPJTomador")[0].checked,
                    ICMSCTe: $("#selICMSCTe").val(),
                    CSTCTe: $("#selCSTCTe").val(),
                    Servico: $("#ddlTipoServico").val(),
                    NomeUsuario: $("#txtUsuario").data("nome"),
                    CodigoUsuario: $("#txtUsuario").data("codigo"),
                    Observacao: $("#txtObservacao").val(),
                    RemoverCliente: $("#chkRemoverCliente")[0].checked,
                    NumeroCarga: $("#txtNumeroCarga").val(),
                    NumeroUnidade: $("#txtNumeroUnidade").val(),
                    AverbacaoCTe: $("#selAverbacaoCTe").val(),
                    inicioRegistros: 0
                };

                executarDownload("/RelatorioCTesEmitidos/DownloadLoteDACTE", dados);
            });

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
            CarregarConsultadeClientes("btnBuscarExpedidor", "btnBuscarExpedidor", RetornoConsultaExpedidor, true, false);
            CarregarConsultadeClientes("btnBuscarRecebedor", "btnBuscarRecebedor", RetornoConsultaRecebedor, true, false);
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);
            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDuplicatas("btnBuscarDuplicatas", "btnBuscarDuplicatas", "", RetornoConsultaDuplicata, true, false);
            CarregarConsultaUsuario("btnBuscarUsuario", "btnBuscarUsuario", "A", "U", RetornoConsultaUsuario, true, false);

            var date = new Date();
            $("#txtDataEmissaoInicial").val(Globalize.format(date, "dd/MM/yyyy"));
            $("#txtDataEmissaoFinal").val(Globalize.format(date, "dd/MM/yyyy"));

            AtualizarGridCTes();
        });

        function ObterMunicipios(estado, select, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: estado }, function (r) {
                if (r.Sucesso) {
                    var selMunicipio = document.getElementById(select);

                    selMunicipio.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todos";
                    optnTodos.value = "";

                    selMunicipio.options.add(optnTodos);

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Descricao;
                        optn.value = r.Objeto[i].Codigo;

                        if (codigo != null && codigo == r.Objeto[i].Codigo)
                            optn.selected = "selected";

                        selMunicipio.options.add(optn);
                    }
                } else {
                    ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
                }
            });
        }

        function ObterEstados() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    var selUfInicio = document.getElementById("selUfInicio");
                    var selUfFim = document.getElementById("selUfFim");

                    selUfInicio.options.length = 0;
                    selUfFim.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todos";
                    optnTodos.value = "";

                    selUfInicio.options.add(optnTodos);
                    selUfFim.options.add(optnTodos.cloneNode(true));

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Nome;
                        optn.value = r.Objeto[i].Sigla;

                        selUfInicio.options.add(optn);
                        selUfFim.options.add(optn.cloneNode(true));
                    }

                    $("#selUfInicio").val("");
                    $("#selUfFim").val("");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function BuscarRemetenteFiltro() {
            if ($("#hddRemetente").val() != $("#txtCPFCNPJRemetente").val()) {
                var cpfCnpj = $("#txtCPFCNPJRemetente").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("#hddRemetente").val(r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJRemetente").val(r.Objeto.CPF_CNPJ);
                                    $("#txtRemetente").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposRemetenteFiltro();
                                    jAlert("Remetente não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposRemetenteFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposRemetenteFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposRemetenteFiltro();
                }
            }
        }
        function LimparCamposRemetenteFiltro() {
            $("#hddRemetente").val("");
            $("#txtCPFCNPJRemetente").val("");
            $("#txtRemetente").val("");
        }
        function RetornoConsultaRemetente(remetente) {
            $("#hddRemetente").val(remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.Nome);
            $("#txtCPFCNPJRemetente").val(remetente.CPFCNPJ);
        }

        function BuscarExpedidorFiltro() {
            if ($("#txtCPFCNPJExpedidor").val() != $("#hddExpedidor").val()) {
                var cpfCnpj = $("#txtCPFCNPJExpedidor").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("#hddExpedidor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJExpedidor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtExpedidor").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposExpedidorFiltro();
                                    jAlert("Expedidor não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposExpedidorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposExpedidorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposExpedidorFiltro();
                }
            }
        }
        function LimparCamposExpedidorFiltro() {
            $("#hddExpedidor").val("");
            $("#txtCPFCNPJExpedidor").val("");
            $("#txtExpedidor").val("");
        }
        function RetornoConsultaExpedidor(expedidor) {
            $("#hddExpedidor").val(expedidor.CPFCNPJ);
            $("#txtExpedidor").val(expedidor.Nome);
            $("#txtCPFCNPJExpedidor").val(expedidor.CPFCNPJ);
        }

        function BuscarRecebedorFiltro() {
            if ($("#txtCPFCNPJRecebedor").val() != $("#hddRecebedor").val()) {
                var cpfCnpj = $("#txtCPFCNPJRecebedor").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("#hddRecebedor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJRecebedor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtRecebedor").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposRecebedorFiltro();
                                    jAlert("Recebedor não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposRecebedorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposRecebedorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposRecebedorFiltro();
                }
            }
        }
        function LimparCamposRecebedorFiltro() {
            $("#hddRecebedor").val("");
            $("#txtCPFCNPJRecebedor").val("");
            $("#txtRecebedor").val("");
        }
        function RetornoConsultaRecebedor(recebedor) {
            $("#hddRecebedor").val(recebedor.CPFCNPJ);
            $("#txtRecebedor").val(recebedor.Nome);
            $("#txtCPFCNPJRecebedor").val(recebedor.CPFCNPJ);
        }

        function BuscarDestinatarioFiltro() {
            if ($("#txtCPFCNPJDestinatario").val() != $("#hddDestinatario").val()) {
                var cpfCnpj = $("#txtCPFCNPJDestinatario").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("#hddDestinatario").val(r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJDestinatario").val(r.Objeto.CPF_CNPJ);
                                    $("#txtDestinatario").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposDestinatarioFiltro();
                                    jAlert("Destinatário não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposDestinatarioFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposDestinatarioFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposDestinatarioFiltro();
                }
            }
        }
        function LimparCamposDestinatarioFiltro() {
            $("#hddDestinatario").val("");
            $("#txtCPFCNPJDestinatario").val("");
            $("#txtDestinatario").val("");
        }
        function RetornoConsultaDestinatario(destinatario) {
            $("#hddDestinatario").val(destinatario.CPFCNPJ);
            $("#txtDestinatario").val(destinatario.Nome);
            $("#txtCPFCNPJDestinatario").val(destinatario.CPFCNPJ);
        }

        function BuscarTomadorFiltro() {
            if ($("#txtCPFCNPJTomador").val() != $("#hddTomador").val()) {
                var cpfCnpj = $("#txtCPFCNPJTomador").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("#hddTomador").val(r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJTomador").val(r.Objeto.CPF_CNPJ);
                                    $("#txtTomador").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposTomadorFiltro();
                                    jAlert("Destinatário não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposTomadorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposTomadorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido!", "Atenção!");
                    }
                } else {
                    LimparCamposTomadorFiltro();
                }
            }
        }
        function LimparCamposTomadorFiltro() {
            $("#hddTomador").val("");
            $("#txtCPFCNPJTomador").val("");
            $("#txtTomador").val("");
        }
        function RetornoConsultaTomador(tomador) {
            $("#hddTomador").val(tomador.CPFCNPJ);
            $("#txtTomador").val(tomador.Nome);
            $("#txtCPFCNPJTomador").val(tomador.CPFCNPJ);
        }
        function RetornoConsultaDuplicata(duplicata) {
            $("#txtDuplicata").val(duplicata.Numero);
        }
        function RetornoConsultaUsuario(user) {
            $("#txtUsuario").val(user.Nome);
            $("#txtUsuario").data("nome", user.Nome);
            $("#txtUsuario").data("codigo", user.Codigo);
        }
        function RetornoConsultaVeiculos(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function AtualizarGridCTes() {
            var dados = {
                Remetente: $("#hddRemetente").val(),
                Expedidor: $("#hddExpedidor").val(),
                Recebedor: $("#hddRecebedor").val(),
                Destinatario: $("#hddDestinatario").val(),
                Tomador: $("#hddTomador").val(),
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                DataAutorizacaoInicial: $("#txtDataAutorizacaoInicial").val(),
                DataAutorizacaoFinal: $("#txtDataAutorizacaoFinal").val(),
                Status: $("#ddlStatus").val(),
                Finalidade: $("#ddlFinalidade").val(),
                Serie: $("#ddlSerie").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                NomeMotorista: $("#txtNomeMotorista").val(),
                CPFMotorista: $("#txtCPFMotorista").val(),
                Veiculo: $("#txtVeiculo").val(),
                TipoOcorrencia: $("#ddlTipoOcorrencia").val(),
                NumeroNotaFiscal: $("#txtNumeroNotaFiscal").val(),
                StatusPagamento: $("#selStatusPagamento").val(),
                CodigoLocalidadeInicio: $("#selLocalidadeInicio").val(),
                CodigoLocalidadeFim: $("#selLocalidadeFim").val(),
                UfInicio: $("#selUfInicio").val(),
                UfFim: $("#selUfFim").val(),
                Duplicata: $("#txtDuplicata").val(),
                Importacao: $("#chkImportacao")[0].checked,
                Exportacao: $("#chkExportacao")[0].checked,
                RaizCNPJRemetente: $("#chkRaizCNPJRemetente")[0].checked,
                RaizCNPJExpedidor: $("#chkRaizCNPJExpedidor")[0].checked,
                RaizCNPJRecebedor: $("#chkRaizCNPJRecebedor")[0].checked,
                RaizCNPJDestinatario: $("#chkRaizCNPJDestinatario")[0].checked,
                RaizCNPJTomador: $("#chkRaizCNPJTomador")[0].checked,
                ICMSCTe: $("#selICMSCTe").val(),
                CSTCTe: $("#selCSTCTe").val(),
                Servico: $("#ddlTipoServico").val(),
                NomeUsuario: $("#txtUsuario").data("nome"),
                CodigoUsuario: $("#txtUsuario").data("codigo"),
                Observacao: $("#txtObservacao").val(),
                RemoverCliente: $("#chkRemoverCliente")[0].checked,
                NumeroCarga: $("#txtNumeroCarga").val(),
                NumeroUnidade: $("#txtNumeroUnidade").val(),
                AverbacaoCTe: $("#selAverbacaoCTe").val(),
                inicioRegistros: 0,
                fimRegistros: 20
            };

            var opcoes = [
                { Descricao: "Baixar DACTE", Evento: DownloadDacte },
                { Descricao: "Baixar XML", Evento: DownloadXML },
                { Descricao: "Baixar XML Cancelamento", Evento: DownloadXMLCancelamento },
                { Descricao: "Enviar por e-mail", Evento: AbrirTelaEnvioEmail },
                { Descricao: "Carta de Correção", Evento: AbrirTelaCartaCorrecao },
                { Descricao: "Retornos Sefaz", Evento: RetornosSefaz },
            ];
            executarRest("/RelatorioCTesEmitidos/Consultar?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    CriarGridView("/RelatorioCTesEmitidos/Consultar?callback=?", dados, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", opcoes, [0], r, null, 20);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
    <script defer="defer" type="text/javascript" id="ScriptDownloadDocumentos">
        function DownloadDacte(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadDacte", { CodigoCTe: cte.data.Codigo });
        }
        function DownloadXML(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXML", { CodigoCTe: cte.data.Codigo });
        }
        function DownloadXMLCancelamento(cte) {
            executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXMLCancelamento", { CodigoCTe: cte.data.Codigo });
        }
    </script>
    <script defer="defer" id="ScriptEnvioEmailCTe" type="text/javascript">
        $(document).ready(function () {
            $("#btnEnviarEmailXMLDacteCTe").click(function () {
                EnviarEmailXMLDacteCTe();
            });
            $("#btnCancelarEnvioEmailXMLDacteCTe").click(function () {
                FecharTelaEnvioEmail();
            });
        });
        function EnviarEmailXMLDacteCTe() {
            if (ValidarEnvioEmailCTe()) {
                executarRest("/ConhecimentoDeTransporteEletronico/EnviarPorEmail?callback=?", { CodigoCTe: $("#hddCodigoCTE").val(), Emails: $("#txtEmailsEnvioXMLDacteCTe").val() }, function (r) {
                    if (r.Sucesso) {
                        jAlert("E-mail enviado com sucesso.", "Sucesso!");
                        FecharTelaEnvioEmail();
                    } else {
                        jAlert(r.Erro, "Atenção!");
                    }
                });
            }
        }
        function ValidarEnvioEmailCTe() {
            var emails = $("#txtEmailsEnvioXMLDacteCTe").val().split(';');
            var valido = true;
            for (var i = 0; i < emails.length; i++) {
                if (!ValidarEmail(emails[i].trim())) {
                    valido = false;
                    break;
                }
            }
            if (!valido) {
                jAlert("E-mail inválido. Confira o(s) e-mail(s) digitado(s) e tente novamente.", "Atenção!");
                CampoComErro("#txtEmailsEnvioXMLDacteCTe");
            } else {
                CampoSemErro("#txtEmailsEnvioXMLDacteCTe");
            }
            return valido;
        }
        function AbrirTelaEnvioEmail(cte) {
            $("#hddCodigoCTE").val(cte.data.Codigo);
            $("#divEnvioEmailCTe").modal("show");
        }
        function FecharTelaEnvioEmail() {
            $("#divEnvioEmailCTe").modal("hide");
            $("#txtEmailsEnvioXMLDacteCTe").val('');
            CampoSemErro("#txtEmailsEnvioXMLDacteCTe");
        }

    </script>
    <script defer="defer" id="ScriptCartaCorrecao" type="text/javascript">
        $(document).ready(function () {
            $("#divCartaCorrecao").on("hidden.bs.modal", function () {
                $("#hddCodigoCTE").val("");
                $("#btnNovaCartaCorrecao").attr("href", "#");
                $("#spnCodigoCTe").text("");
                $("#hddCodigoCTE").val(0);

                $("body").data("CTeCodigoCriptografado", "");
            });
            $("#divCartaCorrecao").on("show.bs.modal", function () {
                AtualizarGridCCes();
            });
        });

        function AtualizarGridCCes() {
            var opcoes = [
                { Descricao: "Baixar XML", Evento: DownloadXMLCCe },
                { Descricao: "Baixar PDF", Evento: GerarRelartorio },
            ];
            CriarGridView("/CartaDeCorrecaoEletronica/Consultar?callback=?", { CodigoCTe: $("#hddCodigoCTE").val() }, "tbl_cces_table", "tbl_cces", "tbl_paginacao_cces", opcoes, [0, 1], null);
        }

        function AbrirTelaCartaCorrecao(cte) {
            executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("body").data("CTeCodigoCriptografado", r.Objeto.CodigoCriptografado);
                    $("#hddCodigoCTE").val(cte.data.Codigo);
                    $("#spnCodigoCTe").text(cte.data.Numero);

                    $("#btnNovaCartaCorrecao").attr("href", "CartaDeCorrecaoEletronica.aspx?voltar=RelatorioCTesEmitidos&x=" + $("body").data("CTeCodigoCriptografado"));

                    $("#divCartaCorrecao").modal("show");
                } else {
                    ExibirMensagemAlerta("Erro ao buscar informações.", "Atenção!");
                }
            });
        }

        function RetornosSefaz(cte) {
            var opcoes = new Array();
            opcoes.push({ Descricao: "Visualizar", Evento: VisualizarMensagemRetornoSefaz });

            CriarGridView("/ConhecimentoDeTransporteEletronico/ConsultarRetornosSefaz?callback=?", { CodigoCTe: cte.data.Codigo }, "tbl_retornossefaz_table", "tbl_retornossefaz", "tbl_paginacao_retornossefaz", opcoes, [0]);
            $("#tituloRetornosSefaz").text("Retornos Sefaz do CT-e " + cte.data.Numero + "-" + cte.data.Serie);
            $('#divRetornosSefaz').modal("show");
        }

        function VisualizarMensagemRetornoSefaz(retornoSefaz) {
            jAlert(retornoSefaz.data.RetornoSefaz, "Mensagem retorno Sefaz");
        }

        function FecharTelaCartaCorrecao() {
            $("#divCartaCorrecao").modal("hide");
        }

        function ErroModalCCe(msg) {
            ExibirMensagemErro(msg, "Atenção!", "placeholder-msgConsultaCCe");
        }

        function DownloadXMLCCe(cce) {
            cce = cce.data;
            if (cce.Status == 3) {
                executarDownload("/CartaDeCorrecaoEletronica/DownloadXML", { CodigoCCe: cce.Codigo, CodigoCTe: $("#hddCodigoCTE").val() }, function () { }, function (ret) {
                    retorno = JSON.parse(msg.replace("(", "").replace(");", ""));
                    ErroModalCCe(retorno.Erro);
                });
            } else {
                ErroModalCCe("O status da Carta de Correção não permite que seja feito o download do XML.");
            }
        }

        function GerarRelartorio(cce) {
            cce = cce.data;
            if (cce.Status == 3) {
                executarDownload("/CartaDeCorrecaoEletronica/DownloadPDF", { CodigoCCe: cce.Codigo }, function () { }, function (ret) {
                    retorno = JSON.parse(ret.replace("(", "").replace(");", ""));
                    ErroModalCCe(retorno.Erro);
                });
            } else {
                ErroModalCCe("O status da Carta de Correção não permite que seja feito o download do PDF.");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:HiddenField ID="hddRemetente" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddExpedidor" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddRecebedor" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddDestinatario" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddTomador" Value="" runat="server" ClientIDMode="Static" />
        <input type="hidden" id="hddCodigoCTE" value="0" />
    </div>
    <div class="page-header">
        <h2>Relatório de CT-es Emitidos
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Aut. Inicial:
                </span>
                <input type="text" id="txtDataAutorizacaoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Aut. Final:
                </span>
                <input type="text" id="txtDataAutorizacaoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Finalidade:
                </span>
                <select id="ddlFinalidade" class="form-control">
                    <option value="">Todas</option>
                    <option value="0">Normal</option>
                    <option value="1">Complemento</option>
                    <option value="2">Anulação</option>
                    <option value="3">Substituto</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Serviço:
                </span>
                <select id="ddlTipoServico" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">0 - Normal</option>
                    <option value="1">1 - Subcontratação</option>
                    <option value="2">2 - Redespacho</option>
                    <option value="3">3 - Red. Intermediário</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="ddlStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="A">Autorizados</option>
                    <option value="C">Cancelados</option>
                    <option value="I">Inutilizados</option>
                    <option value="R">Rejeitados</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="ddlTipoRelatorio" class="form-control">
                    <option value="RelatorioCTesEmitidos">Sumarizado</option>
                    <option value="RelatorioCTesEmitidosValoresFiscais">Sumarizado - Valores Fiscais</option>
                    <option value="RelatorioCTesEmitidosPorDocumento">Sumarizado - Documentos</option>
                    <option value="RelatorioCTesEmitidosPorDocumentoPaisagem">Sumarizado - Documentos Paisagem</option>
                    <option value="RelatorioCTesEmitidosPorDocumentoPaisagem2">Sumarizado - Documentos Paisagem (Mod. 2)</option>
                    <option value="RelatorioCTesEmitidosPaisagem">Sumarizado - Paisagem</option>
                    <option value="RelatorioCTesEmitidosObsFiscoContribuinte">Sumarizado - Obs. Fisco/Contribuinte</option>
                    <option value="RelatorioCTesEmitidosComponentes">Sumarizado - Componentes</option>
                    <option value="RelatorioCTesEmitidosComponentes2">Sumarizado - Componentes (Mod. 2)</option>
                    <option value="RelatorioCTesEmitidosComponentes3">Sumarizado - Componentes (Mod. 3)</option>
                    <option value="RelatorioCTesEmitidosComponentes4">Sumarizado - Componentes (Mod. 4)</option>
                    <option value="RelatorioCTesEmitidosPorDocumentoUsuario">Sumarizado - Usuário</option>
                    <option value="RelatorioCTesEmitidosProdutoPredominante">Sumarizado - Prod. Predominante</option>
                    <option value="RelatorioCTesEmitidosSuamrizadoPorVeiculo">Sumarizado - Veículo</option>
                    <option value="RelatorioCTesEmitidosSubcontratados">Sumarizado - Subcontratação</option>
                    <option value="RelatorioCTesEmitidosDuplicata">Sumarizado - Duplicata</option>
                    <option value="RelatorioCTesEmitidosImpostos">Sumarizado - Impostos</option>                    
                    <option value="RelatorioCTesEmitidosUnidades">Sumarizado - Unidades Medidas</option>
                    <option value="RelatorioCTesEmitidosCompleto">Completo</option>
                    <option value="RelatorioCTesEmitidosCompletoComChave">Completo - Com Chave do CT-e</option>
                    <option value="RelatorioCTesEmitidosCompletoComChaveENotas">Completo - Com Chave do CT-e e Notas</option>
                    <option value="RelatorioCTesEmitidosCompletoComMDFe">Completo - Com MDFe</option>
                    <option value="RelatorioCTesEmitidosCompletoComExpedidor">Completo - Com Expedidor</option>
                    <option value="RelatorioCTesEmitidosImpostosCompleto">Completo - Impostos</option>
                    <option value="RelatorioCTesEmitidosPorVeiculo">Agrupado por Veículo (tração)</option>
                    <option value="RelatorioCTesEmitidosPorDestinatário">Agrupado por Destinatário</option>
                    <option value="RelatorioCTesEmitidosPorTomador">Agrupado por Tomador</option>
                    <option value="RelatorioCTesEmitidosPorOrigem">Agrupado por Origem</option>
                    <option value="RelatorioCTesEmitidosPorTomadorTotais">Totais Agrupado por Tomador</option>                    
                    <option value="RelatorioCTesEmitidosFatura">Layout Fatura</option>                     
                    <!--<option value="RelatorioContratoYamaha">Contrato Yamaha</option>-->
                </select>
            </div>
        </div>
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px;">
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
                                <span class="input-group-addon">Série:
                                </span>
                                <asp:DropDownList ID="ddlSerie" runat="server" ClientIDMode="Static" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Todas"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Ocorrência:
                                </span>
                                <select id="ddlTipoOcorrencia" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="P">Pendente</option>
                                    <option value="F">Final</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status Pgto.:
                                </span>
                                <select id="selStatusPagamento" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="0">Pendente</option>
                                    <option value="1">Pago</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Valor ICMS:
                                </span>
                                <select id="selICMSCTe" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="0">Sem valor ICMS</option>
                                    <option value="1">Com valor ICMS</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Situação Tributária:
                                </span>
                                <select id="selCSTCTe" class="form-control">
                                    <option value="0">Todos</option>
                                    <option value="00">00 - ICMS Normal</option>
                                    <option value="20">20 - ICMS com Redução de Base de Cálculo</option>
                                    <option value="40">40 - ICMS Isenção</option>
                                    <option value="41">41 - ICMS Não Tributado</option>
                                    <option value="51">51 - ICMS Diferido</option>
                                    <option value="60">60 - ICMS Pagto atr. ao tomador ou 3º previsto para ST</option>
                                    <option value="90">90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente</option>
                                    <option value="91">90 - ICMS Outras Situações</option>
                                    <option value="">Simples Nacional</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Averbação:
                                </span>
                                <select id="selAverbacaoCTe" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="1">Averbados</option>
                                    <option value="2">Não Averbados</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Estado Origem:
                                </span>
                                <select id="selUfInicio" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Localidade de início da prestação">Cidade Origem</abbr>:
                                </span>
                                <select id="selLocalidadeInicio" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Estado Destino:
                                </span>
                                <select id="selUfFim" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Localidade de término da prestação">Cidade Destino</abbr>:
                                </span>
                                <select id="selLocalidadeFim" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-1 col-sm-1 col-md-1 col-lg-1">
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkImportacao" />Remetente Exterior
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkExportacao" />Destinatário Exterior
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRemoverCliente" />Não exibir CTes do(s) cliente(s) selecionados
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Rem.:
                                </span>
                                <input type="text" id="txtCPFCNPJRemetente" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Rem.:
                                </span>
                                <input type="text" id="txtRemetente" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRaizCNPJRemetente" />Filtrar Rem. pela Raiz do CNPJ
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Exp.:
                                </span>
                                <input type="text" id="txtCPFCNPJExpedidor" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Exp.:
                                </span>
                                <input type="text" id="txtExpedidor" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarExpedidor" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRaizCNPJExpedidor" />Filtrar Exp. pela Raiz do CNPJ
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Receb.:
                                </span>
                                <input type="text" id="txtCPFCNPJRecebedor" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Receb.:
                                </span>
                                <input type="text" id="txtRecebedor" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRecebedor" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRaizCNPJRecebedor" />Filtrar Rec. pela Raiz do CNPJ
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Dest.:
                                </span>
                                <input type="text" id="txtCPFCNPJDestinatario" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Dest.:
                                </span>
                                <input type="text" id="txtDestinatario" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRaizCNPJDestinatario" />Filtrar Dest. pela Raiz do CNPJ
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Toma.:
                                </span>
                                <input type="text" id="txtCPFCNPJTomador" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Toma.:
                                </span>
                                <input type="text" id="txtTomador" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRaizCNPJTomador" />Filtrar Toma. pela Raiz do CNPJ
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF Motorista:
                                </span>
                                <input type="text" id="txtCPFMotorista" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Motorista:
                                </span>
                                <input type="text" id="txtNomeMotorista" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Placa Veículo:
                                </span>
                                <input type="text" id="txtVeiculo" class="form-control" maxlength="7" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkExibirSomenteTracao" />Exibir Somente Trações
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. NF:
                                </span>
                                <input type="text" id="txtNumeroNotaFiscal" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkExibirNotasFiscais" />Exibir Notas Fiscais
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Duplicata:
                                </span>
                                <input type="text" id="txtDuplicata" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDuplicatas" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input type="text" id="txtUsuario" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarUsuario" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Observação:
                                </span>
                                <input type="text" id="txtObservacao" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Apenas para CT-es gerados via integração">Número da Carga</abbr>:
                                </span>
                                <input type="text" id="txtNumeroCarga" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Apenas para CT-es gerados via integração">Número da Unidade</abbr>:
                                </span>
                                <input type="text" id="txtNumeroUnidade" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGrid" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary"><span class="glyphicon glyphicon-print"></span>&nbsp;Gerar Relatório</button>
    <button type="button" id="btnBaixarLote" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de XML</button>
    <button type="button" id="btnBaixarLoteDACTE" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de DACTE</button>
    <div id="tbl_ctes" style="margin-top: 10px;"></div>
    <div id="tbl_paginacao_ctes"></div>

    <div class="modal fade" id="divCartaCorrecao" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Carta de Correção CT-e <span id="spnCodigoCTe"></span></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgConsultaCCe"></div>

                    <a href="#" class="btn btn-primary" id="btnNovaCartaCorrecao"><span class="glyphicon glyphicon-new-window"></span>Nova Carta de Correção</a>

                    <div id="tbl_cces" style="margin-top: 10px;"></div>
                    <div id="tbl_paginacao_cces"></div>
                    <div class="clearfix"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divEnvioEmailCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Envio de DACTE/XML do CT-e</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">E-mails:
                                </span>
                                <input type="text" id="txtEmailsEnvioXMLDacteCTe" maxlength="1000" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <span class="help-block">Se houver mais de um separe-os por ponto-e-vírgula, ex: abc@abc.com.br; def@def.com.br.
                    </span>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEnviarEmailXMLDacteCTe" class="btn btn-primary">Enviar E-mail(s)</button>
                    <button type="button" id="btnCancelarEnvioEmailXMLDacteCTe" class="btn btn-default">Voltar à Tela Principal</button>
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
</asp:Content>
