<%@ Page Title="Cadastro de Impostos para Contratos de Frete" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ImpostosParaContratoDeFrete.aspx.cs" Inherits="EmissaoCTe.WebApp.ImpostosParaContratoDeFrete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                               "~/bundle/scripts/blockui",
                                               "~/bundle/scripts/maskedinput",
                                               "~/bundle/scripts/datatables",
                                               "~/bundle/scripts/ajax",
                                               "~/bundle/scripts/gridview",
                                               "~/bundle/scripts/consulta",
                                               "~/bundle/scripts/baseConsultas",
                                               "~/bundle/scripts/mensagens",
                                               "~/bundle/scripts/validaCampos",
                                               "~/bundle/scripts/priceformat") %>
    <script id="scriptGeral">
        $(document).ready(function () {
            $("#txtAliquotaSEST").priceFormat();
            $("#txtAliquotaSENAT").priceFormat();
            $("#txtAliquotaINCRA").priceFormat();
            $("#txtAliquotaSalarioEducacao").priceFormat();            
            $("#txtValorPorDependenteDescontoIRRF").priceFormat();                        

            $("#btnSalvar").click(function () {
                Salvar();
            });

            ObterImpostosEmpresa();
        });

        function Salvar() {
            var dados = {
                AliquotaSEST: $("#txtAliquotaSEST").val(),
                AliquotaSENAT: $("#txtAliquotaSENAT").val(),
                AliquotaINCRA: $("#txtAliquotaINCRA").val(),
                AliquotaSalarioEducacao: $("#txtAliquotaSalarioEducacao").val(),
                BaseCalculoIR: $("#txtBaseCalculoIR").val(),
                BaseCalculoINSS: $("#txtBaseCalculoINSS").val(),
                RetencaoINSS: $("#txtValorTetoRetencaoINSS").val(),
                ValorPorDependenteDescontoIRRF: $("#txtValorPorDependenteDescontoIRRF").val(),
                IR: JSON.stringify($("body").data("impostosIR")),
                INSS: JSON.stringify($("body").data("impostosINSS"))
            };

            executarRest("/ImpostoContratoFrete/Salvar?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                    ObterImpostosEmpresa();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function ObterImpostosEmpresa() {
            executarRest("/ImpostoContratoFrete/ObterImpostosDaEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null && typeof r.Objeto == "object") {
                        $("#txtAliquotaSEST").val(Globalize.format(r.Objeto.AliquotaSEST, "n2"));
                        $("#txtAliquotaSENAT").val(Globalize.format(r.Objeto.AliquotaSENAT, "n2"));
                        $("#txtAliquotaINCRA").val(Globalize.format(r.Objeto.AliquotaINCRA, "n2"));
                        $("#txtAliquotaSalarioEducacao").val(Globalize.format(r.Objeto.AliquotaSalarioEducacao, "n2"));
                        $("#txtValorPorDependenteDescontoIRRF").val(Globalize.format(r.Objeto.ValorPorDependenteDescontoIRRF, "n2"));                        
                        $("#txtBaseCalculoIR").val(Globalize.format(r.Objeto.PercentualBCIR, "n2"));
                        $("#txtBaseCalculoINSS").val(Globalize.format(r.Objeto.PercentualBCINSS, "n2"));
                        $("#txtValorTetoRetencaoINSS").val(Globalize.format(r.Objeto.ValorTetoRetencaoINSS, "n2"));

                        $("body").data("impostosIR", r.Objeto.IR);
                        RenderizarIR();

                        $("body").data("impostosINSS", r.Objeto.INSS);
                        RenderizarINSS();
                    } else {
                        $("#txtAliquotaSEST").val('0,00');
                        $("#txtAliquotaSENAT").val('0,00');
                        $("#txtAliquotaINCRA").val('0,00');
                        $("#txtAliquotaSalarioEducacao").val('0,00');
                        $("#txtValorPorDependenteDescontoIRRF").val('0,00');                        
                        $("#txtBaseCalculoIR").val('0,00');
                        $("#txtBaseCalculoINSS").val('0,00');
                        $("#txtValorTetoRetencaoINSS").val('0,00');

                        LimparCamposINSS();
                        LimparCamposIR();
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
    <script id="scriptIR" type="text/javascript">
        $(document).ready(function () {
            $("#txtBaseCalculoIR").priceFormat();
            $("#txtValorInicialIR").priceFormat();
            $("#txtValorFinalIR").priceFormat();
            $("#txtPercentualAplicarIR").priceFormat();
            $("#txtValorDeduzirIR").priceFormat();

            LimparCamposIR();

            $("#btnSalvarIR").click(function () {
                SalvarIR();
            });

            $("#btnExcluirIR").click(function () {
                ExcluirIR();
            });

            $("#btnCancelarIR").click(function () {
                LimparCamposIR();
            });
        });

        function LimparCamposIR() {
            $("#txtValorInicialIR").val('0,00');
            $("#txtValorFinalIR").val('0,00');
            $("#txtPercentualAplicarIR").val('0,00');
            $("#txtValorDeduzirIR").val('0,00');

            $("#btnExcluirIR").hide();
        }

        function SalvarIR() {
            var impostoIR = {
                Codigo: $("body").data("impostoIR") != null ? $("body").data("impostoIR").Codigo : 0,
                ValorInicial: Globalize.parseFloat($("#txtValorInicialIR").val()),
                ValorFinal: Globalize.parseFloat($("#txtValorFinalIR").val()),
                PercentualAplicar: Globalize.parseFloat($("#txtPercentualAplicarIR").val()),
                ValorDeduzir: Globalize.parseFloat($("#txtValorDeduzirIR").val()),
                Excluir: false
            };

            var impostosIR = $("body").data("impostosIR") == null ? new Array() : $("body").data("impostosIR");

            for (var i = 0; i < impostosIR.length; i++) {
                if ((impostoIR.ValorInicial >= impostosIR[i].ValorInicial && impostoIR.ValorInicial <= impostosIR[i].ValorFinal) && !impostosIR[i].Excluir && impostosIR[i].Codigo != impostoIR.Codigo) {
                    ExibirMensagemAlerta("O valor incial está em uma faixa que já está sendo utilizada.", "Atenção!");
                    return;
                } else if ((impostoIR.ValorFinal >= impostosIR[i].ValorInicial && impostoIR.ValorFinal <= impostosIR[i].ValorFinal) && !impostosIR[i].Excluir && impostosIR[i].Codigo != impostoIR.Codigo) {
                    ExibirMensagemAlerta("O valor final está em uma faixa que já está sendo utilizada.", "Atenção!");
                    return;
                }
            }

            impostosIR.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

            if (impostoIR.Codigo == 0)
                impostoIR.Codigo = (impostosIR.length > 0 ? (impostosIR[0].Codigo > 0 ? -1 : (impostosIR[0].Codigo - 1)) : -1);

            for (var i = 0; i < impostosIR.length; i++) {
                if (impostosIR[i].Codigo == impostoIR.Codigo) {
                    impostosIR.splice(i, 1);
                    break;
                }
            }

            impostosIR.push(impostoIR);

            impostosIR.sort(function (a, b) { return a.ValorInicial < b.ValorInicial ? -1 : 1; });

            $("body").data("impostosIR", impostosIR);

            RenderizarIR();
            LimparCamposIR();
        }

        function EditarIR(impostoIR) {
            $("body").data("impostoIR", impostoIR);
            $("#txtValorInicialIR").val(Globalize.format(impostoIR.ValorInicial, "n2"));
            $("#txtValorFinalIR").val(Globalize.format(impostoIR.ValorFinal, "n2"));
            $("#txtPercentualAplicarIR").val(Globalize.format(impostoIR.PercentualAplicar, "n2"));
            $("#txtValorDeduzirIR").val(Globalize.format(impostoIR.ValorDeduzir, "n2"));

            $("#btnExcluirIR").show();
        }

        function ExcluirIR() {
            var impostoIR = $("body").data("impostoIR");

            var impostosIR = $("body").data("impostosIR") == null ? new Array() : $("body").data("impostosIR");

            for (var i = 0; i < impostosIR.length; i++) {
                if (impostosIR[i].Codigo == impostoIR.Codigo) {
                    if (impostoIR.Codigo <= 0)
                        impostosIR.splice(i, 1);
                    else
                        impostosIR[i].Excluir = true;

                    break;
                }
            }

            $("body").data("impostosIR", impostosIR);

            RenderizarIR();
            LimparCamposIR();
        }

        function RenderizarIR() {
            var impostosIR = $("body").data("impostosIR") == null ? new Array() : $("body").data("impostosIR");

            $("#tblIR tbody").html("");

            for (var i = 0; i < impostosIR.length; i++) {
                if (!impostosIR[i].Excluir)
                    $("#tblIR tbody").append("<tr><td>" + Globalize.format(impostosIR[i].ValorInicial, "n2") + "</td><td>" + Globalize.format(impostosIR[i].ValorFinal, "n2") + "</td><td>" + Globalize.format(impostosIR[i].PercentualAplicar, "n2") + "</td><td>" + Globalize.format(impostosIR[i].ValorDeduzir, "n2") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarIR(" + JSON.stringify(impostosIR[i]) + ")'>Editar</button></td></tr>");
            }

            if ($("#tblIR tbody").html() == "")
                $("#tblIR tbody").html("<tr><td colspan='5'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
    <script id="scriptINSS" type="text/javascript">
        $(document).ready(function () {
            $("#txtBaseCalculoINSS").priceFormat();
            $("#txtValorTetoRetencaoINSS").priceFormat();
            $("#txtValorInicialINSS").priceFormat();
            $("#txtValorFinalINSS").priceFormat();
            $("#txtPercentualAplicarINSS").priceFormat();
            $("#txtPercentualAplicarINSSContratante").priceFormat();

            LimparCamposINSS();

            $("#btnSalvarINSS").click(function () {
                SalvarINSS();
            });

            $("#btnExcluirINSS").click(function () {
                ExcluirINSS();
            });

            $("#btnCancelarINSS").click(function () {
                LimparCamposINSS();
            });
        });

        function LimparCamposINSS() {
            $("#txtValorInicialINSS").val('0,00');
            $("#txtValorFinalINSS").val('0,00');
            $("#txtPercentualAplicarINSS").val('0,00');
            $("#txtPercentualAplicarINSSContratante").val('0,00');

            $("#btnExcluirINSS").hide();
        }

        function SalvarINSS() {
            var impostoINSS = {
                Codigo: $("body").data("impostoINSS") != null ? $("body").data("impostoINSS").Codigo : 0,
                ValorInicial: Globalize.parseFloat($("#txtValorInicialINSS").val()),
                ValorFinal: Globalize.parseFloat($("#txtValorFinalINSS").val()),
                PercentualAplicar: Globalize.parseFloat($("#txtPercentualAplicarINSS").val()),
                PercentualAplicarContratante: Globalize.parseFloat($("#txtPercentualAplicarINSSContratante").val()),
                Excluir: false
            };

            var impostosINSS = $("body").data("impostosINSS") == null ? new Array() : $("body").data("impostosINSS");

            for (var i = 0; i < impostosINSS.length; i++) {
                if ((impostoINSS.ValorInicial >= impostosINSS[i].ValorInicial && impostoINSS.ValorInicial <= impostosINSS[i].ValorFinal) && !impostosINSS[i].Excluir && impostosINSS[i].Codigo != impostoINSS.Codigo) {
                    ExibirMensagemAlerta("O valor incial está em uma faixa que já está sendo utilizada.", "Atenção!");
                    return;
                } else if ((impostoINSS.ValorFinal >= impostosINSS[i].ValorInicial && impostoINSS.ValorFinal <= impostosINSS[i].ValorFinal) && !impostosINSS[i].Excluir && impostosINSS[i].Codigo != impostoINSS.Codigo) {
                    ExibirMensagemAlerta("O valor final está em uma faixa que já está sendo utilizada.", "Atenção!");
                    return;
                }
            }

            impostosINSS.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

            if (impostoINSS.Codigo == 0)
                impostoINSS.Codigo = (impostosINSS.length > 0 ? (impostosINSS[0].Codigo > 0 ? -1 : (impostosINSS[0].Codigo - 1)) : -1);

            for (var i = 0; i < impostosINSS.length; i++) {
                if (impostosINSS[i].Codigo == impostoINSS.Codigo) {
                    impostosINSS.splice(i, 1);
                    break;
                }
            }

            impostosINSS.push(impostoINSS);

            impostosINSS.sort(function (a, b) { return a.ValorInicial < b.ValorInicial ? -1 : 1; });

            $("body").data("impostosINSS", impostosINSS);

            RenderizarINSS();
            LimparCamposINSS();
        }

        function EditarINSS(impostoINSS) {
            $("body").data("impostoINSS", impostoINSS);
            $("#txtValorInicialINSS").val(Globalize.format(impostoINSS.ValorInicial, "n2"));
            $("#txtValorFinalINSS").val(Globalize.format(impostoINSS.ValorFinal, "n2"));
            $("#txtPercentualAplicarINSS").val(Globalize.format(impostoINSS.PercentualAplicar, "n2"));
            $("#txtPercentualAplicarINSSContratante").val(Globalize.format(impostoINSS.PercentualAplicarContratante, "n2"));

            $("#btnExcluirINSS").show();
        }

        function ExcluirINSS() {
            var impostoINSS = $("body").data("impostoINSS");

            var impostosINSS = $("body").data("impostosINSS") == null ? new Array() : $("body").data("impostosINSS");

            for (var i = 0; i < impostosINSS.length; i++) {
                if (impostosINSS[i].Codigo == impostoINSS.Codigo) {
                    if (impostoINSS.Codigo <= 0)
                        impostosINSS.splice(i, 1);
                    else
                        impostosINSS[i].Excluir = true;

                    break;
                }
            }

            $("body").data("impostosINSS", impostosINSS);

            RenderizarINSS();
            LimparCamposINSS();
        }

        function RenderizarINSS() {
            var impostosINSS = $("body").data("impostosINSS") == null ? new Array() : $("body").data("impostosINSS");

            $("#tblINSS tbody").html("");

            for (var i = 0; i < impostosINSS.length; i++) {
                if (!impostosINSS[i].Excluir)
                    $("#tblINSS tbody").append("<tr><td>" + Globalize.format(impostosINSS[i].ValorInicial, "n2") + "</td><td>" + Globalize.format(impostosINSS[i].ValorFinal, "n2") + "</td><td>" + Globalize.format(impostosINSS[i].PercentualAplicar, "n2") + "</td><td>" + Globalize.format(impostosINSS[i].PercentualAplicarContratante, "n2")  + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarINSS(" + JSON.stringify(impostosINSS[i]) + ")'>Editar</button></td></tr>");
            }

            if ($("#tblINSS tbody").html() == "")
                $("#tblINSS tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Impostos para Contratos de Frete
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="panel-group" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">Imposto de Renda (IR)
                </h4>
            </div>
            <div class="panel-collapse in">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Base Cálculo:
                                </span>
                                <input type="text" id="txtBaseCalculoIR" class="form-control" maxlength="15" />
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">De:
                                </span>
                                <input type="text" id="txtValorInicialIR" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Até:
                                </span>
                                <input type="text" id="txtValorFinalIR" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Aplicar:
                                </span>
                                <input type="text" id="txtPercentualAplicarIR" class="form-control" maxlength="6" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Deduzir:
                                </span>
                                <input type="text" id="txtValorDeduzirIR" class="form-control" maxlength="15" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarIR" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirIR" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarIR" class="btn btn-default">Cancelar</button>
                    <div class="table-responsive" style="margin-top: 5px;">
                        <table id="tblIR" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 22%;" colspan="1" rowspan="1">De
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Até
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Aplicar
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Deduzir
                                    </th>
                                    <th style="width: 12%;" colspan="1" rowspan="1">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="5">Nenhum registro encontrado.
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">INSS
                </h4>
            </div>
            <div class="panel-collapse in">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Base Cálculo:
                                </span>
                                <input type="text" id="txtBaseCalculoINSS" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Teto Retenção:
                                </span>
                                <input type="text" id="txtValorTetoRetencaoINSS" class="form-control" maxlength="15" />
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">De:
                                </span>
                                <input type="text" id="txtValorInicialINSS" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Até:
                                </span>
                                <input type="text" id="txtValorFinalINSS" class="form-control" maxlength="15" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Aplicar Contratado:
                                </span>
                                <input type="text" id="txtPercentualAplicarINSS" class="form-control" maxlength="6" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Aplicar Contratante:
                                </span>
                                <input type="text" id="txtPercentualAplicarINSSContratante" class="form-control" maxlength="6" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnSalvarINSS" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirINSS" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarINSS" class="btn btn-default">Cancelar</button>
                    <div class="table-responsive" style="margin-top: 5px;">
                        <table id="tblINSS" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 15%;" colspan="1" rowspan="1">De
                                    </th>
                                    <th style="width: 30%;" colspan="1" rowspan="1">Até
                                    </th>
                                    <th style="width: 15%;" colspan="1" rowspan="1">Aplicar Contratado
                                    </th>
                                    <th style="width: 15%;" colspan="1" rowspan="1">Aplicar Contratante
                                    </th>
                                    <th style="width: 10%;" colspan="1" rowspan="1">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="4">Nenhum registro encontrado.
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">SEST
                </h4>
            </div>
            <div class="panel-collapse in">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Alíquota:
                                </span>
                                <input type="text" id="txtAliquotaSEST" class="form-control" maxlength="6" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">SENAT
                </h4>
            </div>
            <div class="panel-collapse in">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Alíquota:
                                </span>
                                <input type="text" id="txtAliquotaSENAT" class="form-control" maxlength="6" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">Outros
                </h4>
            </div>
            <div class="panel-collapse in">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Alíquota INCRA:
                                </span>
                                <input type="text" id="txtAliquotaINCRA" class="form-control" maxlength="6" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Alíquota Salário Educação:
                                </span>
                                <input type="text" id="txtAliquotaSalarioEducacao" class="form-control" maxlength="6" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Valor por dependente:
                                </span>
                                <input type="text" id="txtValorPorDependenteDescontoIRRF" class="form-control" maxlength="6" />
                            </div>
                        </div>                        
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
</asp:Content>
