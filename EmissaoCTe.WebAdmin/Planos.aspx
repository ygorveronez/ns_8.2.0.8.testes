<%@ Page Title="Planos de Emissão de CT-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Planos.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Planos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .fields .fields-title {
            border-bottom: #cdcdcd solid 1px;
            margin: 0 2px 5px 5px;
        }

            .fields .fields-title h3 {
                font-size: 1.5em;
                padding: 0 0 8px 8px;
                font-family: "Segoe UI", "Frutiger", "Tahoma", "Helvetica", "Helvetica Neue", "Arial", "sans-serif";
                color: #000;
                letter-spacing: -1px;
                font-weight: bold;
            }
    </style>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script id="ScriptPlanoDeEmissao" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDePlanosDeEmissao("default-search", "default-search", "", RetornoConsultaPlanosDeEmissao, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function RetornoConsultaPlanosDeEmissao(plano) {
            $("#hddCodigo").val(plano.Codigo);
            $("#txtDescricao").val(plano.Descricao);
            $("#txtDescricaoFaixas").val(plano.DescricaoFaixas);
            $("#selStatus").val(plano.Status);

            BuscarFaixasDeEmissao(plano.Codigo);
            BuscarValoresPorDocumento(plano.Codigo);
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(),
                    DescricaoFaixas: $("#txtDescricaoFaixas").val(),
                    Status: $("#selStatus").val(),
                    FaixasDeEmissao: $("#hddFaixasDeEmissao").val(),
                    ValoresPorDocumentos: $("#hddValoresPorDocumento").val()
                };

                executarRest("/PlanoEmissaoCTe/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#txtDescricao").val("");
            $("#txtDescricaoFaixas").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddFaixasDeEmissao").val("");
            $("#hddValoresPorDocumento").val("");

            LimparCamposFaixasDeEmissao();
            RenderizarFaixasDeEmissao();

            LimparCamposValoresPorDocumento();
            RenderizarValoresPorDocumento();
        }

        function ValidarDados() {
            var descricao = $("#txtDescricao").val();
            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            return valido;
        }
    </script>
    <script id="ScriptFaixasDeEmissao" type="text/javascript">
        $(document).ready(function () {
            $("#txtQuantidade").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtValor").priceFormat({ prefix: '' });

            $("#btnSalvarFaixaEmissao").click(function () {
                SalvarFaixaDeEmissao();
            });

            $("#btnExcluirFaixaEmissao").click(function () {
                ExcluirFaixaDeEmissao();
            });

            $("#btnCancelarFaixaEmissao").click(function () {
                LimparCamposFaixasDeEmissao();
            });
        });

        function SalvarFaixaDeEmissao() {
            if (ValidarDadosFaixaDeEmissao()) {

                var faixa = {
                    Codigo: Globalize.parseInt($("#hddCodigoFaixaEmissao").val()),
                    Quantidade: Globalize.parseInt($("#txtQuantidade").val()),
                    Valor: Globalize.parseFloat($("#txtValor").val()),
                    Excluir: false
                };

                var faixas = $("#hddFaixasDeEmissao").val() == "" ? new Array() : JSON.parse($("#hddFaixasDeEmissao").val());

                if (faixa.Codigo == 0)
                    faixa.Codigo = -(faixas.length + 1);

                for (var i = 0; i < faixas.length; i++) {
                    if (faixas[i].Codigo == faixa.Codigo) {
                        faixas.splice(i, 1);
                        break;
                    }
                }

                faixas.push(faixa);

                $("#hddFaixasDeEmissao").val(JSON.stringify(faixas));

                RenderizarFaixasDeEmissao();

                LimparCamposFaixasDeEmissao();

            }
        }

        function RenderizarFaixasDeEmissao() {
            var faixas = $("#hddFaixasDeEmissao").val() == "" ? new Array() : JSON.parse($("#hddFaixasDeEmissao").val());

            faixas = faixas.sort(function (a, b) { return a.Quantidade - b.Quantidade });

            $("#tblFaixasEmissao tbody").html("");

            for (var i = 0; i < faixas.length; i++)
                if (!faixas[i].Excluir)
                    $("#tblFaixasEmissao tbody").append("<tr><td>" + faixas[i].Quantidade + "</td><td>" + Globalize.format(faixas[i].Valor, "n2") + "</td><td><a href='javascript:void(0);' onclick='EditarFaixaDeEmissao(" + JSON.stringify(faixas[i]) + ")'>Editar</a></td></tr>");

            if ($("#tblFaixasEmissao tbody").html() == "")
                $("#tblFaixasEmissao tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
        }

        function BuscarFaixasDeEmissao(codigoPlano) {
            executarRest("/PlanoEmissaoCTe/ObterFaixasDoPlano?callback=?", { CodigoPlano: codigoPlano }, function (r) {
                if (r.Sucesso) {
                    $("#hddFaixasDeEmissao").val(JSON.stringify(r.Objeto));
                    RenderizarFaixasDeEmissao();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function EditarFaixaDeEmissao(faixa) {
            $("#hddCodigoFaixaEmissao").val(faixa.Codigo);
            $("#txtQuantidade").val(faixa.Quantidade);
            $("#txtValor").val(Globalize.format(faixa.Valor, "n2"));
            $("#btnExcluirFaixaEmissao").show();
        }

        function ExcluirFaixaDeEmissao() {
            var codigo = Globalize.parseInt($("#hddCodigoFaixaEmissao").val());
            var faixas = $("#hddFaixasDeEmissao").val() == "" ? new Array() : JSON.parse($("#hddFaixasDeEmissao").val());

            for (var i = 0; i < faixas.length; i++) {
                if (faixas[i].Codigo == codigo) {
                    if (codigo <= 0)
                        faixas.splice(i, 1);
                    else
                        faixas[i].Excluir = true;
                    break;
                }
            }

            $("#hddFaixasDeEmissao").val(JSON.stringify(faixas));

            RenderizarFaixasDeEmissao();
            LimparCamposFaixasDeEmissao();
        }

        function LimparCamposFaixasDeEmissao() {
            $("#hddCodigoFaixaEmissao").val('0');
            $("#txtQuantidade").val('0');
            $("#txtValor").val('0,00');
            $("#btnExcluirFaixaEmissao").hide();
        }

        function ValidarDadosFaixaDeEmissao() {
            var quantidade = Globalize.parseInt($("#txtQuantidade").val());

            var valor = Globalize.parseFloat($("#txtValor").val());
            var valido = true;

            if (!isNaN(quantidade) && quantidade > 0) {
                CampoSemErro("#txtQuantidade");
            } else {
                CampoComErro("#txtQuantidade");
                valido = false;
            }

            if (!isNaN(valor) && valor > 0) {
                CampoSemErro("#txtValor");
            } else {
                CampoComErro("#txtValor");
                valido = false;
            }

            return valido;
        }
    </script>
    <script id="ScriptValoresPorDocumento" type="text/javascript">
        $(document).ready(function () {
            $("#txtValorValoresPorDocumento").priceFormat({ prefix: '' });

            $("#btnSalvarValoresPorDocumento").click(function () {
                SalvarValoresPorDocumento();
            });

            $("#btnExcluirValoresPorDocumento").click(function () {
                ExcluirValoresPorDocumento();
            });

            $("#btnCancelarValoresPorDocumento").click(function () {
                LimparCamposValoresPorDocumento();
            });
        });

        function SalvarValoresPorDocumento() {
            if (ValidarDadosValoresPorDocumento()) {

                var valor = {
                    Codigo: Globalize.parseInt($("#hddCodigoValoresPorDocumento").val()),
                    Descricao: $("#txtDescricaoValoresPorDocumento").val(),
                    Series: $("#txtSeriesValoresPorDocumento").val(),
                    SerieDiferente: $("#chkSerieDiferenteValoresPorDocumento").prop('checked') ? "Sim" : "Não",
                    Valor: Globalize.parseFloat($("#txtValorValoresPorDocumento").val()),
                    Excluir: false
                };

                var valores = $("#hddValoresPorDocumento").val() == "" ? new Array() : JSON.parse($("#hddValoresPorDocumento").val());

                if (valor.Codigo == 0)
                    valor.Codigo = -(valores.length + 1);

                for (var i = 0; i < valores.length; i++) {
                    if (valores[i].Codigo == valor.Codigo) {
                        valores.splice(i, 1);
                        break;
                    }
                }

                valores.push(valor);

                $("#hddValoresPorDocumento").val(JSON.stringify(valores));

                RenderizarValoresPorDocumento();

                LimparCamposValoresPorDocumento();

            }
        }

        function RenderizarValoresPorDocumento() {
            var valores = $("#hddValoresPorDocumento").val() == "" ? new Array() : JSON.parse($("#hddValoresPorDocumento").val());

            valores = valores.sort(function (a, b) { return a.Quantidade - b.Quantidade });

            $("#tblValoresPorDocumento tbody").html("");

            for (var i = 0; i < valores.length; i++)
                if (!valores[i].Excluir)
                    $("#tblValoresPorDocumento tbody").append("<tr><td>" + valores[i].Descricao + "</td><td>" + valores[i].Series + "</td><td>" + valores[i].SerieDiferente + "</td><td>" + Globalize.format(valores[i].Valor, "n2") + "</td><td><a href='javascript:void(0);' onclick='EditarValoresPorDocumento(" + JSON.stringify(valores[i]) + ")'>Editar</a></td></tr>");

            if ($("#tblValoresPorDocumento tbody").html() == "")
                $("#tblValoresPorDocumento tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
        }

        function BuscarValoresPorDocumento(codigoPlano) {
            executarRest("/PlanoEmissaoCTe/ObterValoresPorDocumentos?callback=?", { CodigoPlano: codigoPlano }, function (r) {
                if (r.Sucesso) {
                    $("#hddValoresPorDocumento").val(JSON.stringify(r.Objeto));
                    RenderizarValoresPorDocumento();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function EditarValoresPorDocumento(valor) {
            $("#hddCodigoValoresPorDocumento").val(valor.Codigo);
            $("#txtDescricaoValoresPorDocumento").val(valor.Descricao);
            $("#txtSeriesValoresPorDocumento").val(valor.Series);
            $("#txtValorValoresPorDocumento").val(Globalize.format(valor.Valor, "n2"));
            $("#chkSerieDiferenteValoresPorDocumento").prop('checked', valor.SerieDiferente == "Sim");
            $("#btnExcluirValoresPorDocumento").show();
        }

        function ExcluirValoresPorDocumento() {
            var codigo = Globalize.parseInt($("#hddCodigoValoresPorDocumento").val());
            var valores = $("#hddValoresPorDocumento").val() == "" ? new Array() : JSON.parse($("#hddValoresPorDocumento").val());

            for (var i = 0; i < valores.length; i++) {
                if (valores[i].Codigo == codigo) {
                    if (codigo <= 0)
                        valores.splice(i, 1);
                    else
                        valores[i].Excluir = true;
                    break;
                }
            }

            $("#hddValoresPorDocumento").val(JSON.stringify(valores));

            RenderizarValoresPorDocumento();
            LimparCamposValoresPorDocumento();
        }

        function LimparCamposValoresPorDocumento() {
            $("#hddCodigoValoresPorDocumento").val('0');
            $("#txtDescricaoValoresPorDocumento").val('');
            $("#txtSeriesValoresPorDocumento").val('');
            $("#chkSerieDiferenteValoresPorDocumento").prop('checked', false);
            $("#txtValorValoresPorDocumento").val('0,00');
            $("#btnExcluirValoresPorDocumento").hide();
        }

        function ValidarDadosValoresPorDocumento() {
            var descricao = $("#txtDescricaoValoresPorDocumento").val();
            var series = Globalize.parseInt($("#txtSeriesValoresPorDocumento").val());

            var valor = Globalize.parseFloat($("#txtValorValoresPorDocumento").val());
            var valido = true;

            if (descricao == "") {
                CampoComErro("#txtDescricaoValoresPorDocumento");
                valido = false;
            } else {
                CampoSemErro("#txtDescricaoValoresPorDocumento");
            }

            if (series == "") {
                CampoComErro("#txtSeriesValoresPorDocumento");
                valido = false;
            } else {
                CampoSemErro("#txtSeriesValoresPorDocumento");
            }

            if (!isNaN(valor) && valor > 0) {
                CampoSemErro("#txtValorValoresPorDocumento");
            } else {
                CampoComErro("#txtValorValoresPorDocumento");
                valido = false;
            }

            return valido;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoFaixaEmissao" value="0" />
        <input type="hidden" id="hddCodigoValoresPorDocumento" value="0" />
        <input type="hidden" id="hddFaixasDeEmissao" value="" />
        <input type="hidden" id="hddValoresPorDocumento" value="" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Planos de Emissão de CT-e
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fieldcinco">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" maxlength="200" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Status*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selStatus" class="select">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div class="fields-title">
                            <h3>Valores por documentos emitidos
                            </h3>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricaoValoresPorDocumento" maxlength="100" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Séries*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtSeriesValoresPorDocumento" maxlength="100" />
                                </div>
                            </div>
                            <div class="field fieldum" style="margin: 22px 5px 0px;">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkSerieDiferenteValoresPorDocumento">
                                    <label for="chkSerieDiferenteValoresPorDocumento">
                                        Série diferente
                                    </label>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Valor*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtValorValoresPorDocumento" value="0,00" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fieldseis">
                                <div class="buttons">
                                    <input type="button" id="btnSalvarValoresPorDocumento" value="Salvar" />
                                    <input type="button" id="btnExcluirValoresPorDocumento" value="Excluir" style="display: none;" />
                                    <input type="button" id="btnCancelarValoresPorDocumento" value="Cancelar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px; width: 31.2%;">
                            <table id="tblValoresPorDocumento">
                                <thead>
                                    <tr>
                                        <th style="width: 20%;">Descricao</th>
                                        <th style="width: 20%;">Series</th>
                                        <th style="width: 10%;">Diferente</th>
                                        <th style="width: 30%;">Valor</th>
                                        <th style="width: 20%;">Opções</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="3">Nenhum registro encontrado.</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <div class="fields-title">
                            <h3>Valores por Faixas de Emissão
                            </h3>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Descrição faixas:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricaoFaixas" maxlength="100" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Quantidade*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtQuantidade" value="0" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Valor*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtValor" value="0,00" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fieldseis">
                                <div class="buttons">
                                    <input type="button" id="btnSalvarFaixaEmissao" value="Salvar" />
                                    <input type="button" id="btnExcluirFaixaEmissao" value="Excluir" style="display: none;" />
                                    <input type="button" id="btnCancelarFaixaEmissao" value="Cancelar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px; width: 31.2%;">
                            <table id="tblFaixasEmissao">
                                <thead>
                                    <tr>
                                        <th style="width: 40%;">Quantidade</th>
                                        <th style="width: 40%;">Valor</th>
                                        <th style="width: 20%;">Opções</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="3">Nenhum registro encontrado.</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
