<%@ Page Title="Baixa de CTe a Receber" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="BaixaDeDuplicatasAReceber.aspx.cs" Inherits="EmissaoCTe.WebApp.BaixaDeDuplicatasAReceber" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        div.tfs-tags {
            margin-bottom: 6px;
            line-height: 25px;
            margin-left: 5px;
        }

        div.tags-label-container {
            float: left;
            padding-left: 4px;
            padding-right: 10px;
            font-size: 11px;
            color: #000;
        }

        div.tags-items-container {
            overflow: hidden;
        }

        .tags-items-container ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
            display: block;
            -webkit-margin-before: 1em;
            -webkit-margin-after: 1em;
            -webkit-margin-start: 0;
            -webkit-margin-end: 0px;
            -webkit-padding-start: 0;
        }

            .tags-items-container ul > li {
                display: inline-block;
                margin-right: 5px;
                padding: 0;
                text-align: -webkit-match-parent;
            }

        .tag-item-delete-experience {
            white-space: nowrap;
            overflow: hidden;
        }

        .tag-container-delete-experience {
            cursor: pointer;
        }

        .tag-container {
            outline: none;
            padding-top: 2px;
            padding-bottom: 2px;
            border: 1px solid #fff !important;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }


        .tag-box, .tag-delete {
            cursor: default;
            margin: 0;
            padding-left: 6px;
            padding-top: 2px;
            padding-right: 6px;
            padding-bottom: 2px;
            font-size: 12px;
            color: #4f4f4f;
            background-color: #d7e6f3;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            font-family: Segoe UI,Tahoma,Arial,Verdana;
            border-radius: 2px 0 0 2px;
        }

        .tag-delete {
            padding-left: 9px;
            padding-right: 9px;
            background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJAgMAAACd/+6DAAAACVBMVEUAAABlZWVlZWXtPovbAAAAAnRSTlMAnxYjQ+0AAAAlSURBVHheLcgxDQAACMTAsrwH3GDj/SGUELikS1mpQoboS773BjdcAscFjXmNAAAAAElFTkSuQmCC') /*images/icon-close-small.png*/ no-repeat 50% 50%;
            background-color: #d7e6f3;
            border-radius: 0 2px 2px 0;
        }

            .tag-delete:focus, .tag-delete:hover {
                cursor: pointer;
                color: #fff;
                background-color: #b4c8d7;
            }
    </style>
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
        $(document).ready(function () {
            CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);
            CarregarConsultaDeCTes("btnBuscarCTe", "btnBuscarCTe", "A", RetornoConsultaCTe, true, false);

            $("#txtCliente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoCliente").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtCTe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoCTe").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtDataInicial").datepicker({});
            $("#txtDataFinal").datepicker({});

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnAtualizarGridDuplicatas").click(function () {
                AtualizarGridDuplicatas();
            });

            $("#btnSelecionarTodasDuplicatas").click(function () {
                AdicionarTodasDuplicatas();
            });

            LimparCampos();
        });
        function RetornoConsultaCliente(cliente) {
            $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome);
            $("#hddCodigoCliente").val(cliente.CPFCNPJ);
        }
        function RetornoConsultaCTe(cte) {
            $("#txtCTe").val(cte.Numero + " - " + cte.Serie);
            $("#hddCodigoCTe").val(cte.Codigo);
        }
        function LimparCampos() {
            $("#hddCodigoCliente").val("");
            $("#txtCliente").val("");
            $("#txtDataInicial").val("");
            $("#txtDataFinal").val("");
            $("#txtObservacao").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddStatus").val($("#selStatus").val());
            $("#hddDuplicatas").val("");
            $("#containerDuplicatasSelecionadas").html("");
            $("#lblSemDuplicatas").show();
            AtualizarGridDuplicatas();
        }
        function AtualizarGridDuplicatas() {
            var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
            if ($("#hddStatus").val() != $("#selStatus").val() && duplicatas.length > 0) {
                jConfirm("Há duplicatas selecionadas, deseja realmente continuar? Ao clicar em 'Sim' as duplicatas selecionadas serão removidas.", "Atenção", function (r) {
                    if (r) {
                        $("#hddStatus").val($("#selStatus").val());
                        $("#hddDuplicatas").val("");
                        $("#containerDuplicatasSelecionadas").html("");
                        $("#lblSemDuplicatas").show();
                        CarregarInformacoesDuplicatas();
                    }
                });
            } else {
                CarregarInformacoesDuplicatas();
            }
        }
        function CarregarInformacoesDuplicatas() {
            var dados = ObterFiltros();

            var colunas = new Array();

            if ($("#selStatus").val() == "0")
                colunas[0] = { Descricao: "Selecionar", Evento: AdicionarDuplicata };
            else
                colunas[0] = { Descricao: "Reverter", Evento: ReverterDuplicata };

            CriarGridView("/CobrancaCTe/Consultar?callback=?", dados, "tbl_duplicatas_table", "tbl_duplicatas", "tbl_paginacao_duplicatas", colunas, [0], null);
        }
        function ObterFiltros() {
            var filtros = {
                inicioRegistros: 0,
                Status: $("#selStatus").val(),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                CodigoCTe: $("#hddCodigoCTe").val(),
                CpfCnpjCliente: $("#hddCodigoCliente").val()
            };

            return filtros;
        }
        function AdicionarDuplicata(duplicata) {
            var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());

            for (var i = 0; i < duplicatas.length; i++)
                if (duplicatas[i].Codigo == duplicata.data.Codigo)
                    return;

            duplicatas.push(duplicata.data);
            $("#hddDuplicatas").val(JSON.stringify(duplicatas));

            var tag = document.createElement("li");
            tag.className = "tag-item tag-item-delete-experience";
            tag.id = "duplicataSelecionada_" + duplicata.data.Codigo;

            var container = document.createElement("span");
            container.className = "tag-container tag-container-delete-experience";

            var descricao = document.createElement("span");
            descricao.className = "tag-box tag-box-delete-experience";
            descricao.innerHTML = "<b>" + duplicata.data.Numero + "</b> | <b>" + duplicata.data.Valor + "</b> | <b>" + duplicata.data.DataVencimento + "</b>: " + duplicata.data.Cliente

            var opcaoExcluir = document.createElement("span");
            opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
            opcaoExcluir.innerHTML = "&nbsp;";
            opcaoExcluir.onclick = function () { RemoverDuplicataSelecionada(duplicata.data.Codigo) };

            container.appendChild(descricao);
            container.appendChild(opcaoExcluir);

            tag.appendChild(container);

            document.getElementById("containerDuplicatasSelecionadas").appendChild(tag);

            $("#lblSemDuplicatas").hide();
        }
        function AdicionarTodasDuplicatas() {
            if ($("#selStatus").val() == "0") {
                executarRest("/CobrancaCTe/Consultar?callback=?", ObterFiltros(), function (r) {
                    if (r.Sucesso) {
                        for (var i = 0; i < r.Objeto.length; i++) {
                            AdicionarDuplicata({ data: r.Objeto[i] });
                        }
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Esta opção só pode ser utilizada para duplicatas pendentes.", "Atenção!");
            }
        }
        function ReverterDuplicata(duplicata) {
            jConfirm("Deseja realmente estornar a duplicata <b>" + duplicata.data.Numero + "</b>?", "Atenção", function (r) {
                if (r) {
                    executarRest("/CobrancaCTe/ReverterDuplicata?callback=?", { Codigo: duplicata.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                            AtualizarGridDuplicatas();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }
        function Salvar() {
            var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
            if (duplicatas.length > 0) {
                var duplicatasSelecionadas = new Array();
                for (var i = 0; i < duplicatas.length; i++)
                    duplicatasSelecionadas.push(duplicatas[i].Codigo);

                var dados = {
                    Duplicatas: JSON.stringify(duplicatasSelecionadas),
                    Observacao: $("#txtObservacao").val()
                };

                executarRest("/CobrancaCTe/BaixarDuplicatas?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Selecione ao menos uma duplicata para realizar a baixa.", "Atenção");
            }
        }
        function RemoverDuplicataSelecionada(codigo) {
            var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());

            for (var i = 0; i < duplicatas.length; i++) {
                if (duplicatas[i].Codigo == codigo) {
                    duplicatas.splice(i, 1);
                    $("#duplicataSelecionada_" + codigo).remove();
                    break;
                }
            }

            $("#hddDuplicatas").val(JSON.stringify(duplicatas));

            if (duplicatas.length <= 0)
                $("#lblSemDuplicatas").show();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoCliente" value="" />
        <input type="hidden" id="hddCodigoCTe" value="" />
        <input type="hidden" id="hddDuplicatas" value="" />
        <input type="hidden" id="hddStatus" value="0" />
    </div>
    <div class="page-header">
        <h2>Baixa de CTe a Receber
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Inicial">Vcto. Inicial</abbr>:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Final">Vcto. Final</abbr>:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Status da Duplicata">Status Dup.</abbr>:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="0">Pendente</option>
                    <option value="1">Paga</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">CT-e:
                </span>
                <input type="text" id="txtCTe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridDuplicatas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <button type="button" id="btnSelecionarTodasDuplicatas" class="btn btn-default pull-right">Selecionar Todas</button>
    <div id="tbl_duplicatas" class="table-responsive" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_duplicatas">
    </div>
    <div class="clearfix"></div>
    <h3 style="margin-bottom: 10px;">Duplicatas Selecionadas
    </h3>
    <div class="clearfix"></div>
    <span id="lblSemDuplicatas">Nenhuma duplicata selecionada.</span>
    <div class="tfs-tags">
        <div class="tags-items-container">
            <ul id="containerDuplicatasSelecionadas">
            </ul>
        </div>
    </div>
    <div class="clearfix"></div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs. Baixa:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Baixar Duplicatas</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
