<%@ Page Title="Fretes por Tipo de Veículo" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="FretesPorTipoDeVeiculo.aspx.cs" Inherits="EmissaoCTe.WebApp.FretesPorTipoDeVeiculo" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(function () {
            CarregarConsultadeClientes("btnBuscarClienteOrigem", "btnBuscarClienteOrigem", RetornoConsultaClienteOrigem, true, false);
            CarregarConsultadeClientes("btnBuscarClienteDestino", "btnBuscarClienteDestino", RetornoConsultaClienteDestino, true, false);
            CarregarConsultaDeTiposDeVeiculos("btnBuscarTipoVeiculo", "btnBuscarTipoVeiculo", "A", RetornoConsultaTipoDeVeiculo, true, false);
            CarregarConsultaDeFretesPorTipoDeVeiculo("default-search", "default-search", "", RetornoConsultaFrete, true, false);

            $("#txtClienteOrigem").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("clienteOrigem", null);
                        AlterarOrigemEDestino();
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtClienteDestino").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("clienteDestino", null);
                        AlterarOrigemEDestino();
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtValorFrete").priceFormat();
            $("#txtValorPedagio").priceFormat();
            $("#txtValorDescarga").priceFormat();
            $("#txtPercentualGris").priceFormat({ centsLimit: 4 });
            $("#txtPercentualAdValorem").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorAdValorem").priceFormat({ prefix: '', centsLimit: 2 });

            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtPercentualAdValorem, #txtValorAdValorem").on('keyup', AlternaCamposAdValorem);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();

            CarregarAliquotasICMS();
            $("#selAliquotaICMS").attr({ disabled: false });
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

        function RetornoConsultaClienteOrigem(cliente) {
            $("#txtClienteOrigem").val(cliente.CPFCNPJ + " - " + cliente.Nome);
            $("body").data("clienteOrigem", cliente.CPFCNPJ);
            AlterarOrigemEDestino();
        }

        function RetornoConsultaClienteDestino(cliente) {
            $("#txtClienteDestino").val(cliente.CPFCNPJ + " - " + cliente.Nome);
            $("body").data("clienteDestino", cliente.CPFCNPJ);
            AlterarOrigemEDestino();
        }

        function RetornoConsultaTipoDeVeiculo(tipo) {
            $("#txtTipoVeiculo").val(tipo.Descricao);
            $("body").data("tipoVeiculo", tipo.Codigo);
        }

        function LimparCampos() {
            $("body").data("clienteOrigem", null);
            $("#txtClienteOrigem").val('');

            $("body").data("clienteDestino", null);
            $("#txtClienteDestino").val('');

            $("body").data("tipoVeiculo", null);
            $("#txtTipoVeiculo").val('');

            $("body").data("codigo", null);

            $("#txtValorFrete").val("0,00");
            $("#txtValorPedagio").val("0,00");
            $("#txtValorDescarga").val("0,00");
            $("#txtPercentualGris").val("0,0000");

            $("#txtPercentualAdValorem").val("0,00");
            $("#txtValorAdValorem").val("0,00");
            $("#selAdicionarAdValoremBcICMS").val($("#selAdicionarAdValoremBcICMS option:first").val());

            $("#txtDataInicial").val("");
            $("#txtDataFinal").val("");
            $("#selStatus").val($("#selStatus option:first").val());

            $("#selUFOrigem").val($("#selUFOrigem option:first").val());
            $("#selUFDestino").val($("#selUFDestino option:first").val());
            $("#selLocalidadeOrigem").html("");
            $("#selLocalidadeDestino").html("");
            $("#selTipoPagamento").val($("#selTipoPagamento option:first").val());
            $("#selAliquotaICMS").val($("#selAliquotaICMS option:first").val());
            $("#chkTodasCidadesDoEstado").prop('checked', false);

            AlterarOrigemEDestino();
            AlternaCamposAdValorem();
        }

        function ValidarCampos() {
            var clienteOrigem = $("body").data("clienteOrigem");
            var clienteDestino = $("body").data("clienteDestino");
            var localidadeOrigem = $("#selLocalidadeOrigem").val();
            var localidadeDestino = $("#selLocalidadeDestino").val();
            var tipoVeiculo = $("body").data("tipoVeiculo");
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());

            var valido = true;

            if ((clienteOrigem != null && clienteOrigem != "") || (localidadeOrigem != null && localidadeOrigem != "")) {
                CampoSemErro("#txtClienteOrigem");
                CampoSemErro("#selLocalidadeOrigem");
            } else {
                CampoComErro("#txtClienteOrigem");
                CampoComErro("#selLocalidadeOrigem");
                valido = false;
            }

            //if ((clienteDestino != null && clienteDestino != "") || (localidadeDestino != null && localidadeDestino != "")) {
            //    CampoSemErro("#txtClienteDestino");
            //    CampoSemErro("#selLocalidadeDestino");
            //} else {
            //    CampoComErro("#txtClienteDestino");
            //    CampoComErro("#selLocalidadeDestino");
            //    valido = false;
            //}

            if (localidadeDestino != null && localidadeDestino != "") {
                CampoSemErro("#selLocalidadeDestino");
            } else if (clienteOrigem == null || clienteOrigem == "") {
                CampoComErro("#selLocalidadeDestino");
                valido = false;
            }

            if (tipoVeiculo != null && tipoVeiculo > 0) {
                CampoSemErro("#txtTipoVeiculo");
            } else {
                CampoComErro("#txtTipoVeiculo");
                valido = false;
            }

            if (!isNaN(valorFrete) != null && valorFrete > 0) {
                CampoSemErro("#txtValorFrete");
            } else {
                CampoComErro("#txtValorFrete");
                valido = false;
            }

            return valido;
        }

        function Salvar() {
            if ($("#chkTodasCidadesDoEstado").prop('checked')) {
                jConfirm("Valores da tabela vão ser replicado para todos municipios do estado destino selecionado, deseja continuar? ", "Atenção", function (ret) {
                    if (ret) {
                        if (ValidarCampos()) {
                            var dados = {
                                Codigo: $("body").data("codigo"),
                                CodigoClienteOrigem: $("body").data("clienteOrigem"),
                                CodigoClienteDestino: $("body").data("clienteDestino"),
                                CodigoTipoVeiculo: $("body").data("tipoVeiculo"),
                                ValorFrete: $("#txtValorFrete").val(),
                                ValorPedagio: $("#txtValorPedagio").val(),
                                ValorDescarga: $("#txtValorDescarga").val(),
                                PercentualGris: $("#txtPercentualGris").val(),
                                DataInicial: $("#txtDataInicial").val(),
                                DataFinal: $("#txtDataFinal").val(),
                                Status: $("#selStatus").val(),
                                CodigoLocalidadeOrigem: $("#selLocalidadeOrigem").val(),
                                CodigoLocalidadeDestino: $("#selLocalidadeDestino").val(),
                                TipoPagamento: $("#selTipoPagamento").val(),
                                AliquotaICMS: $("#selAliquotaICMS").val(),
                                IncluirICMS: $("#selIncluirICMS").val(),
                                AdicionarGrisBcICMS: $("#selAdicionarGrisBcICMS").val(),
                                AdicionarPedagioBcICMS: $("#selAdicionarPedagioBcICMS").val(),
                                AdicionarDescargaBcICMS: $("#selAdicionarDescargaBcICMS").val(),
                                PercentualAdValorem: $("#txtPercentualAdValorem").val(),
                                ValorAdValorem: $("#txtValorAdValorem").val(),
                                AdicionarAdValoremBcICMS: $("#selAdicionarAdValoremBcICMS").val(),
                                TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                            };

                            executarRest("/FretePorTipoDeVeiculo/Salvar?callback=?", dados, function (r) {
                                if (r.Sucesso) {
                                    ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                                    LimparCampos();
                                } else {
                                    ExibirMensagemErro(r.Erro, "Atenção");
                                }
                            });

                        } else {
                            ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
                        }

                    }
                });
            }
            else {
                if (ValidarCampos()) {
                    var dados = {
                        Codigo: $("body").data("codigo"),
                        CodigoClienteOrigem: $("body").data("clienteOrigem"),
                        CodigoClienteDestino: $("body").data("clienteDestino"),
                        CodigoTipoVeiculo: $("body").data("tipoVeiculo"),
                        ValorFrete: $("#txtValorFrete").val(),
                        ValorPedagio: $("#txtValorPedagio").val(),
                        ValorDescarga: $("#txtValorDescarga").val(),
                        PercentualGris: $("#txtPercentualGris").val(),
                        DataInicial: $("#txtDataInicial").val(),
                        DataFinal: $("#txtDataFinal").val(),
                        Status: $("#selStatus").val(),
                        CodigoLocalidadeOrigem: $("#selLocalidadeOrigem").val(),
                        CodigoLocalidadeDestino: $("#selLocalidadeDestino").val(),
                        TipoPagamento: $("#selTipoPagamento").val(),
                        AliquotaICMS: $("#selAliquotaICMS").val(),
                        IncluirICMS: $("#selIncluirICMS").val(),
                        AdicionarGrisBcICMS: $("#selAdicionarGrisBcICMS").val(),
                        AdicionarPedagioBcICMS: $("#selAdicionarPedagioBcICMS").val(),
                        AdicionarDescargaBcICMS: $("#selAdicionarDescargaBcICMS").val(),
                        PercentualAdValorem: $("#txtPercentualAdValorem").val(),
                        ValorAdValorem: $("#txtValorAdValorem").val(),
                        AdicionarAdValoremBcICMS: $("#selAdicionarAdValoremBcICMS").val(),
                        TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                    };

                    executarRest("/FretePorTipoDeVeiculo/Salvar?callback=?", dados, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                            LimparCampos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });

                } else {
                    ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
                }
            }
        }

        function RetornoConsultaFrete(frete) {
            executarRest("/FretePorTipoDeVeiculo/ObterDetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("body").data("clienteOrigem", r.Objeto.CPFCNPJClienteOrigem);
                    $("#txtClienteOrigem").val(r.Objeto.CPFCNPJClienteOrigem + " - " + r.Objeto.NomeClienteOrigem);

                    $("body").data("clienteDestino", r.Objeto.CPFCNPJClienteDestino);
                    $("#txtClienteDestino").val(r.Objeto.CPFCNPJClienteDestino + " - " + r.Objeto.NomeClienteDestino);

                    $("body").data("tipoVeiculo", r.Objeto.CodigoTipoVeiculo);
                    $("#txtTipoVeiculo").val(r.Objeto.DescricaoTipoVeiculo);

                    $("body").data("codigo", r.Objeto.Codigo);

                    $("#txtValorFrete").val(r.Objeto.ValorFrete);
                    $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
                    $("#txtValorDescarga").val(r.Objeto.ValorDescarga);
                    $("#txtPercentualGris").val(r.Objeto.PercentualGris);

                    $("#txtPercentualAdValorem").val(r.Objeto.PercentualAdValorem);
                    $("#txtValorAdValorem").val(r.Objeto.ValorAdValorem);
                    $("#selAdicionarAdValoremBcICMS").val(r.Objeto.AdicionarAdValoremBcICMS);

                    $("#selAdicionarGrisBcICMS").val(r.Objeto.AdicionarGrisBcICMS);
                    $("#selAdicionarPedagioBcICMS").val(r.Objeto.AdicionarPedagioBcICMS);
                    $("#selAdicionarDescargaBcICMS").val(r.Objeto.AdicionarDescargaBcICMS);

                    $("#selIncluirICMS").val(r.Objeto.IncluirICMS);

                    $("#txtDataInicial").val(r.Objeto.DataInicial);
                    $("#txtDataFinal").val(r.Objeto.DataFinal);
                    $("#selStatus").val(r.Objeto.Status);

                    $("#selUFOrigem").val(r.Objeto.UFOrigem);
                    BuscarLocalidades(r.Objeto.UFOrigem, "selLocalidadeOrigem", r.Objeto.CodigoLocalidadeOrigem);

                    $("#selUFDestino").val(r.Objeto.UFDestino);
                    BuscarLocalidades(r.Objeto.UFDestino, "selLocalidadeDestino", r.Objeto.CodigoLocalidadeDestino);

                    $("#selTipoPagamento").val(r.Objeto.TipoPagamento);
                    $("#selAliquotaICMS").val(r.Objeto.AliquotaICMS);

                    AlterarOrigemEDestino();
                    AlternaCamposAdValorem();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            BuscarUFs();

            $("#selUFOrigem").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidadeOrigem", null);
                AlterarOrigemEDestino();
            });

            $("#selUFDestino").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidadeDestino", null);
                AlterarOrigemEDestino();
            });
        });

        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function RenderizarLocalidades(localidades, idSelect, codigo) {
            var selLocalidades = document.getElementById(idSelect);
            selLocalidades.options.length = 0;
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                if (codigo != null) {
                    if (codigo == localidades[i].Codigo) {
                        optn.setAttribute("selected", "selected");
                    }
                }
                selLocalidades.options.add(optn);
            }
        }

        function BuscarUFs() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, "selUFOrigem");
                    RenderizarUFs(r.Objeto, "selUFDestino");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }

        function AlterarOrigemEDestino() {
            if ($("body").data("clienteOrigem") == null && $("#selUFOrigem").val() == "") { //&& $("body").data("clienteDestino") == null && $("#selUFDestino").val() == ""
                $("#divClientes").show();
                $("#divLocalidades").show();
            } else if (($("body").data("clienteOrigem") == null && $("#selUFOrigem").val() != "")) { // || ($("body").data("clienteDestino") == null && $("#selUFDestino").val() != "")
                $("#divClientes").hide();
                $("#divLocalidades").show();
                $("body").data("clienteOrigem", null);
                $("body").data("clienteDestino", null);
                $("#txtClienteOrigem").val("");
                $("#txtClienteDestino").val("");
            } else {
                $("#divClientes").show();
                $("#divLocalidades").hide();
                $("#selUFOrigem").val("");
                $("#selUFDestino").val("");
                $("#selLocalidadeOrigem").html("");
                $("#selLocalidadeDestino").html("");
            }

        }
        function AlternaCamposAdValorem() {
            var _disable = function ($el) {
                $el.val('0,00').prop('disabled', true);
            }

            var _enable = function ($el) {
                $el.prop('disabled', false);
            }

            if ($("#txtPercentualAdValorem").val() != "" && $("#txtPercentualAdValorem").val() != "0,00")
                _disable($("#txtValorAdValorem"));
            else if ($("#txtValorAdValorem").val() != "" && $("#txtValorAdValorem").val() != "0,00")
                _disable($("#txtPercentualAdValorem"));
            else
                _enable($("#txtValorAdValorem, #txtPercentualAdValorem"));
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Fretes por Tipo de Veículo
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div id="divClientes">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Origem*:
                    </span>
                    <input type="text" id="txtClienteOrigem" class="form-control" />
                    <span class="input-group-btn">
                        <button type="button" id="btnBuscarClienteOrigem" class="btn btn-primary">Buscar</button>
                    </span>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Destino*:
                    </span>
                    <input type="text" id="txtClienteDestino" class="form-control" />
                    <span class="input-group-btn">
                        <button type="button" id="btnBuscarClienteDestino" class="btn btn-primary">Buscar</button>
                    </span>
                </div>
            </div>
        </div>
        <div id="divLocalidades">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">UF Origem*:
                    </span>
                    <select id="selUFOrigem" class="form-control">
                        <option value="">Selecione</option>
                    </select>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Loc. Origem*:
                    </span>
                    <select id="selLocalidadeOrigem" class="form-control">
                        <option value="">Selecione</option>
                    </select>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">UF Destino*:
                    </span>
                    <select id="selUFDestino" class="form-control">
                        <option value="">Selecione</option>
                    </select>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Loc. Destino*:
                    </span>
                    <select id="selLocalidadeDestino" class="form-control">
                        <option value="">Selecione</option>
                    </select>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <div class="input-group">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" id="chkTodasCidadesDoEstado" />
                            Replicar tabela para todas cidades do estado destino selecionado
                        </label>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Veículo*:
                </span>
                <input type="text" id="txtTipoVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Pgto.*:
                </span>
                <select id="selTipoPagamento" class="form-control">
                    <option value="0">Todos</option>
                    <option value="1">Pago</option>
                    <option value="2">A Pagar</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Frete*:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Pedágio:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Add Pedágio BC ICMS:
                </span>
                <select id="selAdicionarPedagioBcICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Descarga:
                </span>
                <input type="text" id="txtValorDescarga" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Add Descarga BC ICMS:
                </span>
                <select id="selAdicionarDescargaBcICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">GRIS (%):
                </span>
                <input type="text" id="txtPercentualGris" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Add GRIS BC ICMS:
                </span>
                <select id="selAdicionarGrisBcICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual sobre o valor da mercadoria">% Ad Valorem</abbr>:
                </span>
                <input type="text" id="txtPercentualAdValorem" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual sobre o valor da mercadoria">Valor Ad Valorem</abbr>:
                </span>
                <input type="text" id="txtValorAdValorem" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Add Ad Valorem BC ICMS:
                </span>
                <select id="selAdicionarAdValoremBcICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Incluir ICMS*:
                </span>
                <select id="selIncluirICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group" style="visibility: hidden;">
                <span class="input-group-addon">% ICMS*:
                </span>
                <select id="selAliquotaICMS" class="form-control">
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
