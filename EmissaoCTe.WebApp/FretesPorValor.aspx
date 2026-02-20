<%@ Page Title="Cadastro de Fretes por Valor" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="FretesPorValor.aspx.cs" Inherits="EmissaoCTe.WebApp.FretesPorValor" %>

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
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            BuscarUFs();

            $("#selUFDestino").change(function () {
                BuscarLocalidades($(this).val(), "selCidadeDestino", null);
            });

            CarregarConsultadeClientes("btnBuscarClienteOrigem", "btnBuscarClienteOrigem", RetornoConsultaClienteOrigem, true, false);

            $("#txtClienteOrigem").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoClienteOrigem").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtPercentualSobreNF").priceFormat({ prefix: '', limit: 5 });
            $("#txtValorMinimoFrete").priceFormat({ prefix: '' });
            $("#txtValorPedagio").priceFormat({ prefix: '' });
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            CarregarConsultaDeFretesPorValor("default-search", "default-search", "", RetornoConsultaFrete, true, false);
        });
        function RetornoConsultaFrete(frete) {
            executarRest("/FretePorValor/ObterDetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    frete = r.Objeto;
                    $("#hddCodigo").val(frete.Codigo);
                    $("#hddCodigoClienteOrigem").val(frete.CPFCNPJClienteOrigem);
                    $("#txtClienteOrigem").val(frete.NomeClienteOrigem);
                    BuscarLocalidades(frete.UFDestino, "selCidadeDestino", frete.CodigoLocalidadeDestino);
                    $("#selUFDestino").val(frete.UFDestino);
                    $("#txtValorMinimoFrete").val(frete.ValorMinimoFrete);
                    $("#txtPercentualSobreNF").val(frete.PercentualSobreNF);
                    $("#selincluirPedagioBC").val(frete.IncluirPedagioBC);
                    $("#txtDataInicial").val(frete.DataInicio);
                    $("#txtDataFinal").val(frete.DataFim);
                    $("#selStatus").val(frete.Status);
                    $("#selTipoFrete").val(frete.Tipo);
                    $("#selTipoPagamento").val(frete.TipoPagamento);
                    $("#txtValorPedagio").val(frete.ValorPedagio);
                    $("#selTipoRateio").val(frete.TipoRateio);
                    $("#selIncluiICMS").val(frete.IncluirICMS);
                    $("#txtCodigoIntegracao").val(frete.CodigoIntegracao);
                    $("#selTipoCliente").val(frete.TipoCliente);
                    //$("#chkTodasCidadesDoEstado").prop('checked', r.Objeto.TodasCidadesDoEstado);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    jAlert(r.Erro, "Atenção");
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
                    RenderizarUFs(r.Objeto, "selUFDestino");
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }
        function RetornoConsultaClienteOrigem(cliente) {
            $("#txtClienteOrigem").val(cliente.CPFCNPJ + " - " + cliente.Nome);
            $("#hddCodigoClienteOrigem").val(cliente.CPFCNPJ);
        }
        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#hddCodigoClienteOrigem").val("");
            $("#txtClienteOrigem").val("");
            $("#selUFDestino").val($("#selUFDestino option:first").val());
            $("#selCidadeDestino").html("");
            $("#txtValorMinimoFrete").val("0,00");
            $("#txtPercentualSobreNF").val("0,00");
            $("#txtDataInicial").val("");
            $("#txtDataFinal").val("");
            $("#selincluirPedagioBC").val($("#selincluirPedagioBC option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipoFrete").val($("#selTipoFrete option:first").val());
            $("#selTipoPagamento").val($("#selTipoPagamento option:first").val());
            $("#txtValorPedagio").val("0,00");
            $("#selTipoRateio").val($("#selTipoRateio option:first").val());
            $("#selIncluiICMS").val($("#selIncluiICMS option:first").val());
            $("#txtCodigoIntegracao").val("");
            $("#chkTodasCidadesDoEstado").prop('checked', false);
            $("#selTipoCliente").val($("#selTipoCliente option:first").val());
        }
        function ValidarCampos() {
            var cidadeDestino = 0;
            var clienteOrigem = 0;
            if ($("#selCidadeDestino").val() != null && $("#selCidadeDestino").val() != "")
                cidadeDestino = Globalize.parseInt($("#selCidadeDestino").val());
            if ($("#hddCodigoClienteOrigem").val() != null && $("#hddCodigoClienteOrigem").val() != "")
                clienteOrigem = Globalize.parseInt($("#hddCodigoClienteOrigem").val());

            var valido = true;

            if (clienteOrigem == 0 && cidadeDestino == 0) {
                ExibirMensagemAlerta("Necessáio informar um cliente origem ou uma cidade destino.", "Atenção!");
                valido = false;
            }

            if (cidadeDestino > 0) {
                CampoSemErro("#selCidadeDestino");
            } else {
                if (clienteOrigem == 0) {
                    CampoComErro("#selCidadeDestino");
                    valido = false;
                }
            }

            return valido;
        }
        function Salvar() {
            if ($("#chkTodasCidadesDoEstado").prop('checked')) {
                jConfirm("Valores da tabela vão ser replicado para todos municipios do estado selecionado, deseja continuar? ", "Atenção", function (ret) {
                    if (ret) {
                        if (ValidarCampos()) {
                            var dados = {
                                Codigo: $("#hddCodigo").val(),
                                CodigoClienteOrigem: $("#hddCodigoClienteOrigem").val(),
                                CodigoCidadeDestino: $("#selCidadeDestino").val(),
                                ValorMinimoFrete: $("#txtValorMinimoFrete").val(),
                                PercentualSobreNF: $("#txtPercentualSobreNF").val(),
                                Tipo: $("#selTipoFrete").val(),
                                IncluirPedagioBC: $("#selincluirPedagioBC").val(),
                                DataInicial: $("#txtDataInicial").val(),
                                DataFinal: $("#txtDataFinal").val(),
                                Status: $("#selStatus").val(),
                                TipoPagamento: $("#selTipoPagamento").val(),
                                ValorPedagio: $("#txtValorPedagio").val(),
                                TipoRateio: $("#selTipoRateio").val(),
                                IncluirICMS: $("#selIncluiICMS").val(),
                                CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                                TipoCliente: $("#selTipoCliente").val(),
                                TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                            };
                            executarRest("/FretePorValor/Salvar?callback=?", dados, function (r) {
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
                        Codigo: $("#hddCodigo").val(),
                        CodigoClienteOrigem: $("#hddCodigoClienteOrigem").val(),
                        CodigoCidadeDestino: $("#selCidadeDestino").val(),
                        ValorMinimoFrete: $("#txtValorMinimoFrete").val(),
                        PercentualSobreNF: $("#txtPercentualSobreNF").val(),
                        Tipo: $("#selTipoFrete").val(),
                        IncluirPedagioBC: $("#selincluirPedagioBC").val(),
                        DataInicial: $("#txtDataInicial").val(),
                        DataFinal: $("#txtDataFinal").val(),
                        Status: $("#selStatus").val(),
                        TipoPagamento: $("#selTipoPagamento").val(),
                        ValorPedagio: $("#txtValorPedagio").val(),
                        TipoRateio: $("#selTipoRateio").val(),
                        IncluirICMS: $("#selIncluiICMS").val(),
                        CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                        TipoCliente: $("#selTipoCliente").val(),
                        TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                    };
                    executarRest("/FretePorValor/Salvar?callback=?", dados, function (r) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoClienteOrigem" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Fretes por Valor
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtClienteOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarClienteOrigem" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Cliente:
                </span>
                <select id="selTipoCliente" class="form-control">
                    <option value="0">Remetente</option>
                    <option value="1">Expedidor</option>
                    <option value="2">Recebedor</option>
                    <option value="3">Destinatário</option>
                    <option value="4">Tomador</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">UF de Destino:
                </span>
                <select id="selUFDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Cidade Destino:
                </span>
                <select id="selCidadeDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkTodasCidadesDoEstado" />
                        Todas cidades do estado selecionado
                    </label>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor Mínimo do Frete">Val. Mín. Frete*</abbr>:
                </span>
                <input type="text" id="txtValorMinimoFrete" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual Cobrado Sobre a Nota Fiscal">% Sobre NF*</abbr>:
                </span>
                <input type="text" id="txtPercentualSobreNF" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor Mínimo do Frete">Val. Pedágio*</abbr>:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipoFrete" class="form-control">
                    <option value="0">Valor Mínimo + Percentual Sobre NF</option>
                    <option value="1">Valor Mínimo Garantido</option>
                    <option value="2">Somente Percentual Sobre NF</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir o valor do pedágio na BC do ICMS">Incluir pedágio na BC</abbr>:
                </span>
                <select id="selincluirPedagioBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo Rateio*:
                </span>
                <select id="selTipoRateio" class="form-control">
                    <option value="0">Nenhum</option>
                    <option value="1">Valor Nota Fiscal</option>
                    <option value="2">Peso</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Incluir ICMS Frete*:
                </span>
                <select id="selIncluiICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código para integração">Codigo Integração</abbr>:
                </span>
                <input type="text" id="txtCodigoIntegracao" class="form-control" value="" maxlength="100" />
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
