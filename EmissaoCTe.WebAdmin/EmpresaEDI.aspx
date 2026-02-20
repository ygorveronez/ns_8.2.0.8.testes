<%@ Page Title="Configuração de EDI por Empresa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmpresaEDI.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EmpresaEDI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
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
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            ObterLayouts();

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);

            $("#btnAdicionarLayout").click(function () {
                AdicionarLayoutEmpresa();
            });

        });

        function RetornoConsultaEmpresa(empresa) {

            $("body").data("codigoEmpresa", empresa.Codigo);

            $("#txtEmpresa").val(empresa.CNPJ + " - " + empresa.NomeFantasia);

            ObterLayoutsDaEmpresa(empresa.Codigo);

        }

        function ObterLayoutsDaEmpresa(codigoEmpresa) {
            executarRest("/EmpresaLayoutEDI/ObterLayoutsPorEmpresa?callback=?", { CodigoEmpresa: codigoEmpresa }, function (r) {
                if (r.Sucesso) {
                    $("body").data("layouts", r.Objeto);
                    RenderizarLayoutsEmpresa();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function ObterLayouts() {
            executarRest("/LayoutEDI/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    var selLayout = document.getElementById("selLayout");

                    selLayout.options.length = 0;

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Descricao + " (" + r.Objeto[i].DescricaoTipo + ")";
                        optn.value = r.Objeto[i].Codigo;

                        selLayout.options.add(optn);
                    }

                    $("#selLayout").val("");
                }
            });
        }

        function RenderizarLayoutsEmpresa() {
            var layouts = $("body").data("layouts") == null ? new Array() : $("body").data("layouts");

            $("#tblLayouts tbody").html("");

            for (var i = 0; i < layouts.length; i++)
                $("#tblLayouts tbody").append("<tr><td>" + layouts[i].Descricao + "</td><td><a href='javascript:void(0);' onclick='ExcluirLayoutEmpresa(" + JSON.stringify(layouts[i]) + ")'>Excluir</a></td></tr>");


            if ($("#tblLayouts tbody").html() == "")
                $("#tblLayouts tbody").html("<tr><td colspan='2'>Nenhum registro encontrado.</td></tr>");
        }

        function AdicionarLayoutEmpresa() {
            if (ValidarLayoutEmpresa()) {

                executarRest("/EmpresaLayoutEDI/AdicionarLayoutEmpresa?callback=?", { CodigoEmpresa: $("body").data("codigoEmpresa"), CodigoLayout: $("#selLayout").val() }, function (r) {
                    if (r.Sucesso) {
                        ObterLayoutsDaEmpresa($("body").data("codigoEmpresa"));
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");
            }
        }

        function ValidarLayoutEmpresa() {
            var valido = true;
            var layout = $("#selLayout").val();

            if (layout == null || layout == "") {
                CampoComErro("#selLayout");
                valido = false;
            } else {
                CampoSemErro("#selLayout");
            }

            return valido;
        }

        function ExcluirLayoutEmpresa(layout) {
            jConfirm("Deseja realmente remover o layout <b>'" + layout.Descricao + "'</b> desta empresa?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/EmpresaLayoutEDI/DeletarLayoutEmpresa?callback=?", { CodigoEmpresa: $("body").data("codigoEmpresa"), CodigoLayout: layout.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ObterLayoutsDaEmpresa($("body").data("codigoEmpresa"));
                            ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
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
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Configuração de EDI por Empresa
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
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
                                        Empresa:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresa" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Layout:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selLayout" class="select">
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 78px;">
                                <div class="buttons">
                                    <input type="button" id="btnAdicionarLayout" value="Adicionar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="width: 600px; margin-left: 5px;">
                            <table id="tblLayouts">
                                <thead>
                                    <tr>
                                        <th style="width: 80%;">Descrição
                                        </th>
                                        <th>Opções
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2">Nenhum registro encontrado!
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
