<%@ Page Title="Cadastro de CFOP" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CFOP.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CFOP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtCFOP").mask("9999");
            BuscarNaturezasDasOperacoes("selNaturezaDaOperacao");
            CarregarConsultaDeCFOPs("default-search", "default-search", RetornoConsultaCFOP, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#selNaturezaDaOperacao").change(function () {
                BuscarCFOPsDaNaturezaDaOperacao();
            });
        });
        function BuscarNaturezasDasOperacoes(idSelect) {
            executarRest("/NaturezaDaOperacao/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarNaturezasDasOperacoes(r.Objeto, idSelect);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarNaturezasDasOperacoes(naturezas, idSelect) {
            var sel = document.getElementById(idSelect);
            sel.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            sel.options.add(optn);
            for (var i = 0; i < naturezas.length; i++) {
                var optn = document.createElement("option");
                optn.text = naturezas[i].Codigo + " - " + naturezas[i].Descricao;
                optn.value = naturezas[i].Codigo;
                sel.options.add(optn);
            }
        }
        function LimparCampos() {
            $("#selNaturezaDaOperacao").val($("#selNaturezaDaOperacao option:first").val());
            $("#txtCFOP").val('');
            $("#hddCodigo").val('0');
            $("#btnCancelar").hide();
            BuscarCFOPsDaNaturezaDaOperacao();
        }
        function ValidarCampos() {
            var naturezaDaOperacao = $("#selNaturezaDaOperacao").val();
            var cfop = $("#txtCFOP").val().trim();
            var valido = true;
            if (naturezaDaOperacao != "0") {
                CampoSemErro("#selNaturezaDaOperacao");
            } else {
                CampoComErro("#selNaturezaDaOperacao");
                valido = false;
            }
            if (cfop != "" && cfop.length == 4) {
                CampoSemErro("#txtCFOP");
            } else {
                CampoComErro("#txtCFOP");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    NaturezaDaOperacao: $("#selNaturezaDaOperacao").val(),
                    CFOP: $("#txtCFOP").val()
                };
                executarRest("/CFOP/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco(*) são obrigatórios!", "Atenção");
            }
        }
        function RetornoConsultaCFOP(cfop) {
            $("#hddCodigo").val(cfop.Codigo);
            $("#selNaturezaDaOperacao").val(cfop.CodigoNaturezaDaOperacao);
            $("#txtCFOP").val(cfop.CFOP);
            $("#btnCancelar").show();
        }
        function BuscarCFOPsDaNaturezaDaOperacao() {
            if ($("#selNaturezaDaOperacao").val() != "0") {
                executarRest("/CFOP/BuscarPorNaturezaDaOperacao?callback=?", { IdNaturezaOperacao: $("#selNaturezaDaOperacao").val() }, function (r) {
                    if (r.Sucesso) {
                        RenderizarCFOPsDaNaturezaDaOperacao(r.Objeto);
                        $("#divTableCFOPs").slideDown();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                $("#divTableCFOPs").slideUp();
            }
        }
        function RenderizarCFOPsDaNaturezaDaOperacao(cfops) {
            $("#tblCFOPs tbody").html("");
            for (var i = 0; i < cfops.length; i++) {
                $("#tblCFOPs tbody").append("<tr><td>" + cfops[i].CodigoCFOP + "</td></tr>");
            }
            if ($("#tblCFOPs tbody").html() == "") {
                $("#tblCFOPs tbody").html("<tr><td>Nenhum registro encontrado!</td></tr>");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>
                    Cadastro de CFOPs
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
                        <div class="fieldzao">
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Natureza da Operação*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selNaturezaDaOperacao" class="select">
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        CFOP*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCFOP" class="maskedInput" />
                                </div>
                            </div>
                        </div>
                        <div id="divTableCFOPs" class="table" style="width: 200px; margin-left: 5px; display: none;">
                            <table id="tblCFOPs">
                                <thead>
                                    <tr>
                                        <th>
                                            CFOP
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>
                                            Nenhum registro encontrado!
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
