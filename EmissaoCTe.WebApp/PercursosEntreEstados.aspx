<%@ Page Title="Percursos entre Estados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="PercursosEntreEstados.aspx.cs" Inherits="EmissaoCTe.WebApp.PercursosEntreEstados" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            HeaderAuditoria("PercursoEstado");

            $("#txtOrdem").mask("9?9");

            ObterEstados();

            CarregarConsultaDePercursosEntreEstados("default-search", "default-search", EditarPercurso, true, false);

            $("#btnSalvar").click(function () {
                SalvarPercurso();
            });

            $("#btnCancelar").click(function () {
                LimparCamposPercurso();
            });

        });

        function ObterEstados() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    var selUFOrigem = document.getElementById("selUFOrigem");
                    var selUFDestino = document.getElementById("selUFDestino");
                    var selUFPassagem = document.getElementById("selUFPassagem");

                    selUFOrigem.options.length = 0;
                    selUFDestino.options.length = 0;
                    selUFPassagem.options.length = 0;

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Nome;
                        optn.value = r.Objeto[i].Sigla;

                        selUFOrigem.options.add(optn);
                        selUFDestino.options.add(optn.cloneNode(true));
                        selUFPassagem.options.add(optn.cloneNode(true));
                    }

                    $("#selUFOrigem").val("");
                    $("#selUFDestino").val("");
                    $("#selUFPassagem").val("");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ValidarCamposPercurso() {

            var ufOrigem = $("#selUFOrigem").val();
            var ufDestino = $("#selUFDestino").val();

            var valido = true;

            if (ufOrigem != null && ufOrigem != "") {
                CampoSemErro("#selUFOrigem");
            } else {
                CampoComErro("#selUFOrigem");
                valido = false;
            }

            if (ufDestino != null && ufDestino != "") {
                CampoSemErro("#selUFDestino");
            } else {
                CampoComErro("#selUFDestino");
                valido = false;
            }

            return valido;
        }

        function SalvarPercurso() {
            if (ValidarCamposPercurso()) {
                var percurso = {
                    Codigo: $("body").data("percurso") != null ? $("body").data("percurso").Codigo : 0,
                    UFOrigem: $("#selUFOrigem").val(),
                    UFDestino: $("#selUFDestino").val(),
                    Passagens: JSON.stringify($("body").data("percurso") != null ? $("body").data("percurso").Passagens : new Array()),
                    Excluir: false
                };

                executarRest("/PercursoEstado/Salvar?callback=?", percurso, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCamposPercurso();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");
            }
        }

        function EditarPercurso(percurso) {
            executarRest("/PercursoEstado/ObterDetalhes?callback=?", { Codigo: percurso.Codigo }, function (r) {
                if (r.Sucesso) {
                    HeaderAuditoriaCodigo(percurso.Codigo);

                    $("body").data("percurso", r.Objeto);
                    $("#selUFOrigem").val(r.Objeto.UFOrigem);
                    $("#selUFDestino").val(r.Objeto.UFDestino);
                    RenderizarPassagens();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function LimparCamposPercurso() {
            HeaderAuditoriaCodigo(0);

            $("body").data("percurso", null);
            $("#selUFOrigem").val("");
            $("#selUFDestino").val("");
            LimparCamposPassagem();
            RenderizarPassagens();
        }
    </script>
    <script id="ScriptPassagens" type="text/javascript">
        $(document).ready(function () {

            $("#btnSalvarPassagem").click(function () {
                SalvarPassagem();
            });

            $("#btnExcluirPassagem").click(function () {
                ExcluirPassagem();
            });

            $("#btnCancelarPassagem").click(function () {
                LimparCamposPassagem();
            });

        });
        function RenderizarPassagens() {
            var passagens = $("body").data("percurso") == null ? new Array() : $("body").data("percurso").Passagens;

            $("#tblPassagens tbody").html("");

            for (var i = 0; i < passagens.length; i++) {
                if (!passagens[i].Excluir)
                    $("#tblPassagens tbody").append("<tr><td>" + passagens[i].DescricaoUFPassagem + "</td><td>" + passagens[i].Ordem + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarPassagem(" + JSON.stringify(passagens[i]) + ")'>Editar</button></td></tr>");
            }

            if ($("#tblPassagens tbody").html() == "")
                $("#tblPassagens tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
        }

        function ValidarCamposPassagem() {
            var passagem = $("#selUFPassagem").val();
            var ordem = Globalize.parseInt($("#txtOrdem").val());
            var valido = true;

            if (passagem != null && passagem != "") {
                CampoSemErro("#selUFPassagem");
            } else {
                CampoComErro("#selUFPassagem");
                valido = false;
            }

            if (!isNaN(ordem) || ordem <= 0) {
                CampoSemErro("#txtOrdem");
            } else {
                CampoComErro("#txtOrdem");
                valido = false;
            }

            return valido;
        }

        function SalvarPassagem() {
            if (ValidarCamposPassagem()) {
                var passagem = {
                    Codigo: $("body").data("passagem") != null ? $("body").data("passagem").Codigo : 0,
                    UFPassagem: $("#selUFPassagem").val(),
                    DescricaoUFPassagem: $("#selUFPassagem option:selected").text(),
                    Ordem: Globalize.parseInt($("#txtOrdem").val()),
                    Excluir: false
                };

                var passagens = $("body").data("percurso") == null ? new Array() : $("body").data("percurso").Passagens;

                for (var i = 0; i < passagens.length; i++) {
                    //if (passagens[i].UFPassagem == passagem.UFPassagem && passagens[i].Codigo != passagem.Codigo && passagens[i].Excluir == false) {
                    //    ExibirMensagemAlerta("Esta passagem já foi utilizada.", "Atenção!");
                    //    return;
                    //} else 
                    if (passagens[i].Ordem == passagem.Ordem && passagens[i].Codigo != passagem.Codigo && passagens[i].Excluir == false) {
                        ExibirMensagemAlerta("Esta ordem já foi utilizada.", "Atenção!");
                        return;
                    }
                }

                passagens.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

                if (passagem.Codigo == 0)
                    passagem.Codigo = (passagens.length > 0 ? (passagens[0].Codigo > 0 ? -1 : (passagens[0].Codigo - 1)) : -1);

                for (var i = 0; i < passagens.length; i++) {
                    if (passagens[i].Codigo == passagem.Codigo) {
                        passagens.splice(i, 1);
                        break;
                    }
                }

                passagens.push(passagem);

                passagens.sort(function (a, b) { return a.Ordem < b.Ordem ? -1 : 1; });

                percurso = $("body").data("percurso") != null ? $("body").data("percurso") : {};

                percurso.Passagens = passagens;

                $("body").data("percurso", percurso);

                RenderizarPassagens();
                LimparCamposPassagem();
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");
            }
        }

        function EditarPassagem(passagem) {
            $("body").data("passagem", passagem);
            $("#selUFPassagem").val(passagem.UFPassagem);
            $("#txtOrdem").val(passagem.Ordem);
            $("#btnExcluirPassagem").show();
        }

        function ExcluirPassagem() {
            var passagem = $("body").data("passagem");

            var percurso = $("body").data("percurso") == null ? new Array() : $("body").data("percurso");

            for (var i = 0; i < percurso.Passagens.length; i++) {
                if (percurso.Passagens[i].Codigo == passagem.Codigo) {
                    if (passagem.Codigo <= 0)
                        percurso.Passagens.splice(i, 1);
                    else
                        percurso.Passagens[i].Excluir = true;
                    break;
                }
            }

            $("body").data("percurso", percurso);

            RenderizarPassagens();
            LimparCamposPassagem();
        }

        function LimparCamposPassagem() {
            $("body").data("passagem", null);
            $("#selUFPassagem").val("");
            $("#txtOrdem").val("");
            $("#btnExcluirPassagem").hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Percursos entre Estados
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Origem*:
                </span>
                <select id="selUFOrigem" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Destino*:
                </span>
                <select id="selUFDestino" class="form-control">
                </select>
            </div>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-heading">Passagens</div>
        <div class="panel-body">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                    <div class="input-group">
                        <span class="input-group-addon">Passagem*:
                        </span>
                        <select id="selUFPassagem" class="form-control">
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Ordem*:
                        </span>
                        <input type="text" id="txtOrdem" class="form-control" />
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarPassagem" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirPassagem" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarPassagem" class="btn btn-default">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblPassagens" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 70%;">Estado de Passagem
                            </th>
                            <th style="width: 15%;">Ordem
                            </th>
                            <th style="width: 15%;">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="3">Nenhum registro encontrado!
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
