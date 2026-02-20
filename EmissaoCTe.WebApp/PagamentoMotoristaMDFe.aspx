<%@ Page Title="Pagamentos de Motoristas por MDF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="PagamentoMotoristaMDFe.aspx.cs" Inherits="EmissaoCTe.WebApp.PagamentoMotoristaMDFe" %>

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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(function () {
            $("#txtDataPagamento").datepicker();
            $("#txtDataRecebimento").datepicker();
            $("#txtDataPagamento").mask("99/99/9999");
            $("#txtDataRecebimento").mask("99/99/9999");

            $("#txtValorFrete").priceFormat();
            $("#txtINSSSENAT").priceFormat();
            $("#txtSESTSENAT").priceFormat();
            $("#txtIR").priceFormat();
            $("#txtAdiantamento").priceFormat();
            $("#txtSalarioMotorista").priceFormat();
            $("#txtValorPedagio").priceFormat();

            $("#txtValorFrete").focusout(function () {
                AtualizarDeducaoDeValores();
                AtualizarSaldoPagar();
            });

            $("#txtINSSSENAT, #txtSESTSENAT, #txtIR, #txtAdiantamento, #txtValorPedagio, #txtSalarioMotorista").focusout(function () {
                AtualizarSaldoPagar();
            });

            $("#selDeduzir").change(function () {
                AtualizarDeducaoDeValores();
                AtualizarSaldoPagar();
            });

            $("#txtMDFe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("mdfe", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("motorista", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            LimparCampos();

            CarregarConsultaDeMDFes("btnBuscarMDFe", "btnBuscarMDFe", 3, RetornoConsultaMDFe, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
        });

        function RetornoConsultaMotorista(motorista) {
            $("body").data("motorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
            $("#txtSalarioMotorista").val(motorista.Salario);
        }

        function RetornoConsultaMDFe(mdfe) {
            $("#txtMDFe").val(mdfe.Numero + " - " + mdfe.Serie);
            $("body").data("mdfe", mdfe.Codigo);
        }

        function AtualizarDeducaoDeValores() {
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var valorTotalFreteAnterior = 0;
            var percentualBCINSS = 0.20;
            var percentualBCIR = 0;
            var valorTetoRetencaoINSS = 0;
            var percentualINSSAplicar = 0.11;
            var aliquotaSest = 0.015;
            var aliquotaSenat = 0.010;
            var valorTotalINSSAnterior = 0;
            var valorTotalIRAnterior = 0;
            var percentualIRAplicar = 0;
            var valorIRDeduzir = 0;
            var valorINSS = 0;
            var valorIR = 0;

            var dados = {
                Codigo: $("body").data("codigo"),
                CodigoMotorista: $("body").data("motorista"),
                ValorFrete: $("#txtValorFrete").val(),
                DataPagamento: $("#txtDataPagamento").val()
            };

            if ($("#selDeduzir").val() == "1") {
                executarRest("/PagamentoMotoristaMDFe/ObterImpostosDaEmpresa?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto.PercentualBCINSS > 0)
                            percentualBCINSS = r.Objeto.PercentualBCINSS / 100;
                        if (r.Objeto.PercentualBCIR > 0)
                            percentualBCIR = r.Objeto.PercentualBCIR / 100;
                        if (r.Objeto.ValorTetoRetencaoINSS > 0)
                            valorTetoRetencaoINSS = r.Objeto.ValorTetoRetencaoINSS;
                        if (r.Objeto.AliquotaSENAT > 0)
                            aliquotaSenat = r.Objeto.AliquotaSENAT / 100;
                        if (r.Objeto.AliquotaSEST > 0)
                            aliquotaSest = r.Objeto.AliquotaSEST / 100;
                        if (r.Objeto.ValorTotalINSS > 0)
                            valorTotalINSSAnterior = r.Objeto.ValorTotalINSS;
                        if (r.Objeto.PercentualINSSAplicar > 0)
                            percentualINSSAplicar = r.Objeto.PercentualINSSAplicar / 100;
                        if (r.Objeto.ValorTotalIR > 0)
                            valorTotalIRAnterior = r.Objeto.ValorTotalIR;
                        if (r.Objeto.PercentualIRAplicar > 0)
                            percentualIRAplicar = r.Objeto.PercentualIRAplicar / 100;
                        if (r.Objeto.ValorIRDeduzir > 0)
                            valorIRDeduzir = r.Objeto.ValorIRDeduzir;
                        if (r.Objeto.ValorTotalFrete > 0)
                            valorTotalFreteAnterior = r.Objeto.ValorTotalFrete;

                        //Calculos do Sest Senat
                        var valorSest = (valorFrete * percentualBCINSS) * aliquotaSest;
                        var valorSenat = (valorFrete * percentualBCINSS) * aliquotaSenat;
                        $("#txtSESTSENAT").val(Globalize.format(valorSest + valorSenat, "n2"));

                        //Calculos do INSS
                        valorINSS = (valorFrete * percentualBCINSS) * percentualINSSAplicar;
                        if (valorTetoRetencaoINSS > 0 && valorTotalINSSAnterior >= valorTetoRetencaoINSS) //Se o valor teto já foi atingido o valor do INSS é zerado
                            valorINSS = 0
                        else if (valorTetoRetencaoINSS > 0 && ((valorINSS + valorTotalINSSAnterior) > valorTetoRetencaoINSS)) //Se o acumulado de INSS é maior que o Teto o valor do INSS é somente o Teto                           
                            valorINSS = valorTetoRetencaoINSS - valorTotalINSSAnterior;
                        $("#txtINSSSENAT").val(Globalize.format(valorINSS, "n2"));

                        //Calculos do IR                       
                        var valorICMSTotal = valorTotalINSSAnterior + valorINSS;
                        valorIR = 0;
                        if (percentualIRAplicar > 0) {
                            var baseCalculoIR = (valorTotalFreteAnterior + valorFrete) * percentualBCIR;
                            baseCalculoIR = baseCalculoIR - valorICMSTotal;
                            valorIR = (baseCalculoIR * percentualIRAplicar) - valorIRDeduzir - valorTotalIRAnterior;
                        }
                        $("#txtIR").val(Globalize.format(valorIR, "n2"));

                        AtualizarSaldoPagar();
                    } else {

                        $("#txtINSSSENAT").val(Globalize.format(((valorFrete * 0.2) * 0.11), "n2"));
                        $("#txtSESTSENAT").val(Globalize.format(((valorFrete * 0.2) * 0.025), "n2"));
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                $("#txtINSSSENAT").val("0,00");
                $("#txtSESTSENAT").val("0,00");
                $("#txtIR").val("0,00");
            }
        }

        function AtualizarSaldoPagar() {
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var valorINSS = Globalize.parseFloat($("#txtINSSSENAT").val());
            var valorSEST = Globalize.parseFloat($("#txtSESTSENAT").val());
            var valorIR = Globalize.parseFloat($("#txtIR").val());
            var valorAdiantamento = Globalize.parseFloat($("#txtAdiantamento").val());
            var valorPedagio = Globalize.parseFloat($("#txtValorPedagio").val());
            var salarioMotorista = Globalize.parseFloat($("#txtSalarioMotorista").val());

            if ($("#selDeduzir").val() == "1") {
                $("#txtSaldoPagar").val(Globalize.format(valorFrete + valorPedagio + salarioMotorista - valorINSS - valorSEST - valorIR - valorAdiantamento, "n2"));
            } else {
                $("#txtSaldoPagar").val(Globalize.format(valorFrete + valorPedagio + salarioMotorista, "n2"));
            }
        }

        function LimparCampos() {
            $("#txtMDFe").val('');
            $("body").data("mdfe", null);

            $("body").data("codigo", null);

            $("#txtMotorista").val('');
            $("body").data("motorista", null);

            $("#txtDataPagamento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataRecebimento").val('');
            $("#txtValorFrete").val('0,00');
            $("#txtINSSSENAT").val('0,00');
            $("#txtSESTSENAT").val('0,00');
            $("#txtIR").val('0,00');
            $("#txtSaldoPagar").val("0,00");
            $("#txtAdiantamento").val("0,00");
            $("#txtValorPedagio").val("0,00");
            $("#txtSalarioMotorista").val("0,00");
            $("#selDeduzir").val($("#selDeduzir option:first").val());
            $("#selStatus").val($("#selStatus option:first").val()).change();
            $("#txtObservacao").val('');
        }

        function ValidarCampos() {
            var codigoMDFe = $("body").data("mdfe");
            var codigoMotorista = $("body").data("motorista");
            var valorFrete = Globalize.parseFloat($("#txtValorFrete").val());
            var dataPagamento = $("#txtDataPagamento").val();
            var dataRecebimento = $("#txtDataRecebimento").val();

            var valido = true;

            if (isNaN(codigoMDFe) || codigoMDFe <= 0) {
                CampoComErro("#txtMDFe");
                valido = false;
            } else {
                CampoSemErro("#txtMDFe");
            }

            if (isNaN(codigoMotorista) || codigoMotorista <= 0) {
                CampoComErro("#txtMotorista");
                valido = false;
            } else {
                CampoSemErro("#txtMotorista");
            }

            if (isNaN(valorFrete) || valorFrete <= 0) {
                CampoComErro("#txtValorFrete");
                valido = false;
            } else {
                CampoSemErro("#txtValorFrete");
            }

            if (dataPagamento == null || dataPagamento == "") {
                CampoComErro("#txtDataPagamento");
                valido = false;
            } else {
                CampoSemErro("#txtDataPagamento");
            }

            if (dataRecebimento == null || dataRecebimento == "") {
                CampoComErro("#txtDataRecebimento");
                valido = false;
            } else {
                CampoSemErro("#txtDataRecebimento");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("body").data("codigo"),
                    CodigoMDFe: $("body").data("mdfe"),
                    CodigoMotorista: $("body").data("motorista"),
                    ValorFrete: $("#txtValorFrete").val(),
                    INSSSENAT: $("#txtINSSSENAT").val(),
                    SESTSENAT: $("#txtSESTSENAT").val(),
                    IR: $("#txtIR").val(),
                    Adiantamento: $("#txtAdiantamento").val(),
                    SalarioMotorista: $("#txtSalarioMotorista").val(),
                    DataPagamento: $("#txtDataPagamento").val(),
                    DataRecebimento: $("#txtDataRecebimento").val(),
                    Deduzir: $("#selDeduzir").val(),
                    Status: $("#selStatus").val(),
                    Observacao: $("#txtObservacao").val(),
                    ValorPedagio: $("#txtValorPedagio").val()
                };

                executarRest("/PagamentoMotoristaMDFe/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                        LimparCampos();
                        CarregarPagamentos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }

        function Editar(pagamento) {
            executarRest("/PagamentoMotoristaMDFe/ObterDetalhes?callback=?", { Codigo: pagamento.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("body").data("codigo", r.Objeto.Codigo);
                    $("body").data("mdfe", r.Objeto.CodigoMDFe);
                    $("#txtMDFe").val(r.Objeto.DescricaoMDFe);
                    $("body").data("motorista", r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
                    $("#txtValorFrete").val(r.Objeto.ValorFrete);
                    $("#txtINSSSENAT").val(r.Objeto.ValorINSSSENAT);
                    $("#txtSESTSENAT").val(r.Objeto.ValorSESTSENAT);
                    $("#txtIR").val(r.Objeto.ValorImpostoRenda);
                    $("#txtDataPagamento").val(r.Objeto.DataPagamento);
                    $("#txtDataRecebimento").val(r.Objeto.DataRecebimento);
                    $("#selDeduzir").val(r.Objeto.Deduzir);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtAdiantamento").val(r.Objeto.ValorAdiantamento);
                    $("#txtSalarioMotorista").val(r.Objeto.SalarioMotorista);
                    $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
                    $("#selStatus").val(r.Objeto.Status).change();

                    AtualizarSaldoPagar();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeMDFes("btnBuscarMDFeFiltro", "btnBuscarMDFeFiltro", 3, RetornoConsultaMDFeFiltro, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotoristaFiltro", "btnBuscarMotoristaFiltro", RetornoConsultaMotoristaFiltro, true, false);

            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataInicial").datepicker();

            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker();

            $("#txtMDFeFiltro").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("mdfeFiltro", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtMotoristaFiltro").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("motoristaFiltro", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnConsultarPagamentos").click(function () {
                CarregarPagamentos();
            });

            CarregarPagamentos();
        });

        function RetornoConsultaMotoristaFiltro(motorista) {
            $("body").data("motoristaFiltro", motorista.Codigo);
            $("#txtMotoristaFiltro").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function RetornoConsultaMDFeFiltro(mdfe) {
            $("#txtMDFeFiltro").val(mdfe.Numero + " - " + mdfe.Serie);
            $("body").data("mdfeFiltro", mdfe.Codigo);
        }

        function CarregarPagamentos() {
            var dados = {
                inicioRegistros: 0,
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                CodigoMotorista: $("body").data("motoristaFiltro"),
                Status: $("#selStatusFiltro").val(),
                CodigoMDFe: $("body").data("mdfeFiltro")
            }

            var opcoes = new Array();
            opcoes.push({ Descricao: "Editar", Evento: Editar });
            opcoes.push({ Descricao: "Gerar Contrato", Evento: GerarContrato });

            CriarGridView("/PagamentoMotoristaMDFe/Consultar?callback=?", dados, "tbl_pagamentos_table", "tbl_pagamentos", "tbl_paginacao_pagamentos", opcoes, [0], null);
        }

        function GerarContrato(pagamento) {
            executarDownload("/PagamentoMotoristaMDFe/DownloadContrato", { Codigo: pagamento.data.Codigo });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Pagamentos de Motoristas por MDF-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="clearfix"></div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle btn-block" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Dt. Inicial:
                                </span>
                                <input type="text" id="txtDataInicial" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Dt. Final:
                                </span>
                                <input type="text" id="txtDataFinal" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">MDF-e:
                                </span>
                                <input type="text" id="txtMDFeFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMDFeFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Motorista:
                                </span>
                                <input type="text" id="txtMotoristaFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotoristaFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusFiltro" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="P">Pendente</option>
                                    <option value="A">Pago</option>
                                    <option value="C">Cancelado</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarPagamentos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar Pagamentos</button>
    <div id="tbl_pagamentos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_pagamentos">
    </div>
    <div class="clearfix"></div>
    <hr style="margin: 20px 0 10px 0;" />
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">MDF-e*:
                </span>
                <input type="text" id="txtMDFe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMDFe" class="btn btn-primary">Buscar</button>
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
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor*:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Pedágio:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Salário Motorista:
                </span>
                <input type="text" id="txtSalarioMotorista" class="form-control" />
            </div>
        </div>        
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Deduzir INSS/IR">Deduzir</abbr>*:
                </span>
                <select id="selDeduzir" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">INSS + SENAT:
                </span>
                <input type="text" id="txtINSSSENAT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">SEST SENAT:
                </span>
                <input type="text" id="txtSESTSENAT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Imposto Renda:
                </span>
                <input type="text" id="txtIR" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Adiantamento:
                </span>
                <input type="text" id="txtAdiantamento" class="form-control" />
            </div>
        </div>       
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Saldo a Pagar:
                </span>
                <input type="text" id="txtSaldoPagar" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Pgto.*:
                </span>
                <input type="text" id="txtDataPagamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Rcbto.*:
                </span>
                <input type="text" id="txtDataRecebimento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="P">Pendente</option>
                    <option value="A">Pago</option>
                    <option value="C">Cancelado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" rows="2" class="form-control" maxlength="300"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
