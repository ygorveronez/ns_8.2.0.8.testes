<%@ Page Title="Cadastro de Entregas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Entregas.aspx.cs" Inherits="EmissaoCTe.WebApp.Entregas" %>

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
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker") %>
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
            CarregarConsultaDeEntregas("default-search", "default-search", RetornoConsultaEntrega, true, false);

            $("#txtDataEntrega").datepicker();
            $("#txtDataEntrega").mask("99/99/9999");

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoVeiculo", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoMotorista", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaEntrega(entrega) {
            LimparCampos();

            executarRest("/Entrega/ObterDetalhes?callback=?", { CodigoEntrega: entrega.Codigo }, function (r) {
                if (r.Sucesso) {

                    $("body").data("codigo", r.Objeto.Codigo);

                    $("body").data("codigoVeiculo", r.Objeto.CodigoVeiculo);
                    $("#txtVeiculo").val(r.Objeto.PlacaVeiculo);

                    $("body").data("codigoMotorista", r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.NomeMotorista);

                    $("#txtNumero").val(r.Objeto.Numero);
                    $("#txtDataEntrega").val(r.Objeto.Data);
                    $("#txtObservacao").val(r.Objeto.Observacao);

                    for (var i = 0; i < r.Objeto.CTes.length; i++)
                        AdicionarCTe({ data: r.Objeto.CTes[i] });

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("codigoVeiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("codigoMotorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function LimparCampos() {
            $("body").data("codigo", null);

            $("body").data("codigoVeiculo", null);
            $("#txtVeiculo").val("");

            $("body").data("codigoMotorista", null);
            $("#txtMotorista").val("");

            $("#txtNumero").val("Automático");
            $("#txtDataEntrega").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtObservacao").val("");

            $("body").data("ctesSelecionados", null);
            $("#containerCTesSelecionados").html("");
            $("#lblSemCTes").show();
        }

        function ValidarCampos() {
            var codigoMotorista = $("body").data("codigoMotorista");
            var codigoVeiculo = $("body").data("codigoVeiculo");
            var dataEntrega = $("#txtDataEntrega").val();
            var valido = true;

            if (codigoMotorista <= 0) {
                CampoComErro("#txtMotorista");
                valido = false;
            } else {
                CampoSemErro("#txtMotorista");
            }

            if (codigoVeiculo <= 0) {
                CampoComErro("#txtVeiculo");
                valido = false;
            } else {
                CampoSemErro("#txtVeiculo");
            }

            if (dataEntrega == "") {
                CampoComErro("#txtDataEntrega");
                valido = false;
            } else {
                CampoSemErro("#txtDataEntrega");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("body").data("codigo"),
                    CTes: JSON.stringify(ObterCodigosCTes()),
                    CodigoMotorista: $("body").data("codigoMotorista"),
                    CodigoVeiculo: $("body").data("codigoVeiculo"),
                    DataEntrega: $("#txtDataEntrega").val(),
                    Observacao: $("#txtObservacao").val()
                };

                executarRest("/Entrega/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {

                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");

                        LimparCampos();

                    } else {

                        ExibirMensagemErro(r.Erro, "Atenção!");

                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function ObterCodigosCTes() {
            var listaCTes = new Array();
            var ctes = $("body").data("ctesSelecionados") == null ? new Array() : $("body").data("ctesSelecionados");

            for (var i = 0; i < ctes.length; i++)
                listaCTes.push(ctes[i].Codigo);

            return listaCTes;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);
            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
            CarregarConsultaDeLocalidades("btnBuscarDestino", "btnBuscarDestino", RetornoConsultaDestino, true, false);

            $("#txtDataEmissaoInicial").datepicker();
            $("#txtDataEmissaoInicial").mask("99/99/9999");

            $("#txtDataEmissaoFinal").datepicker();
            $("#txtDataEmissaoFinal").mask("99/99/9999");

            $("#txtNumeroInicial").mask("9?9999999999999");
            $("#txtNumeroFinal").mask("9?9999999999999");
            $("#txtNumeroDocumento").mask("9?9999999999999");

            $("#txtDestino").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoDestino", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoRemetente", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtDestinatario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoDestinatario", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnAtualizarGridCTes").click(function () {
                AtualizarGridCTes();
            });

            $("#btnSelecionarTodosCTes").click(function () {
                AdicionarTodosCTes();
            });

            AtualizarGridCTes();
        });

        function RetornoConsultaRemetente(remetente) {
            $("body").data("codigoRemetente", remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.CPFCNPJ + " - " + remetente.Nome);
        }

        function RetornoConsultaDestinatario(destinatario) {
            $("body").data("codigoDestinatario", destinatario.CPFCNPJ);
            $("#txtDestinatario").val(destinatario.CPFCNPJ + " - " + destinatario.Nome);
        }

        function RetornoConsultaDestino(destino) {
            $("body").data("codigoDestino", destino.Codigo);
            $("#txtDestino").val(destino.Descricao + " - " + destino.UF);
        }

        function AtualizarGridCTes() {
            var colunas = new Array();
            colunas[0] = { Descricao: "Selecionar", Evento: AdicionarCTe };

            CriarGridView("/Entrega/ConsultarCTes?callback=?", ObterFiltros(), "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", colunas, [0], null);
        }

        function ObterFiltros() {
            var dados = {
                CPFCNPJRemetente: $("body").data("codigoRemetente"),
                CPFCNPJDestinatario: $("body").data("codigoDestinatario"),
                CodigoDestino: $("body").data("codigoDestino"),
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                NumeroDocumento: $("#txtNumeroDocumento").val(),
                inicioRegistros: 0
            };

            return dados;
        }

        function AdicionarCTe(cte) {
            var ctes = $("body").data("ctesSelecionados") == null ? new Array() : $("body").data("ctesSelecionados");

            for (var i = 0; i < ctes.length; i++)
                if (ctes[i].Codigo == cte.data.Codigo)
                    return;

            ctes.push(cte.data);
            $("body").data("ctesSelecionados", ctes);

            var tag = document.createElement("li");
            tag.className = "tag-item tag-item-delete-experience";
            tag.id = "cteSelecionado_" + cte.data.Codigo;

            var container = document.createElement("span");
            container.className = "tag-container tag-container-delete-experience";

            var descricao = document.createElement("span");
            descricao.className = "tag-box tag-box-delete-experience";
            descricao.innerHTML = "<b>" + cte.data.Numero + "</b> | " + cte.data.Destinatario;

            var opcaoExcluir = document.createElement("span");
            opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
            opcaoExcluir.innerHTML = "&nbsp;";
            opcaoExcluir.onclick = function () { RemoverCTeSelecionado(cte.data.Codigo) };

            container.appendChild(descricao);
            container.appendChild(opcaoExcluir);

            tag.appendChild(container);

            document.getElementById("containerCTesSelecionados").appendChild(tag);

            $("#lblSemCTes").hide();
        }

        function RemoverCTeSelecionado(codigo) {
            var ctes = $("body").data("ctesSelecionados") == "" ? new Array() : $("body").data("ctesSelecionados");

            for (var i = 0; i < ctes.length; i++) {
                if (ctes[i].Codigo == codigo) {
                    ctes.splice(i, 1);
                    $("#cteSelecionado_" + codigo).remove();
                    break;
                }
            }

            $("body").data("ctesSelecionados", ctes);

            if (ctes.length <= 0)
                $("#lblSemCTes").show();
        }

        function AdicionarTodosCTes() {
            executarRest("/Entrega/BuscarTodosCTes?callback=?", ObterFiltros(), function (r) {
                if (r.Sucesso) {
                    for (var i = 0; i < r.Objeto.length; i++) {
                        AdicionarCTe({ data: r.Objeto[i] });
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Entregas
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle btn-block" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Seleção de CT-es
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data de Emissão Inicial">Dt. Inicial</abbr>:
                                </span>
                                <input type="text" id="txtDataEmissaoInicial" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data de Emissão Final">Dt. Final</abbr>:
                                </span>
                                <input type="text" id="txtDataEmissaoFinal" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Nº. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicial" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Nº. Final:
                                </span>
                                <input type="text" id="txtNumeroFinal" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Remetente:
                                </span>
                                <input type="text" id="txtRemetente" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Destinatário:
                                </span>
                                <input type="text" id="txtDestinatario" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Destino:
                                </span>
                                <input type="text" id="txtDestino" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDestino" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data de Emissão Inicial">Núm. NF</abbr>:
                                </span>
                                <input type="text" id="txtNumeroDocumento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnAtualizarGridCTes" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <button type="button" id="btnSelecionarTodosCTes" class="btn btn-default pull-right">Selecionar Todos</button>
                    <div id="tbl_ctes" class="table-responsive" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_ctes">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="clearfix"></div>
    <h3 style="margin-bottom: 10px;">CT-es Selecionados
    </h3>
    <div class="clearfix"></div>
    <span id="lblSemCTes">Nenhum CT-e selecionado.</span>
    <div class="tfs-tags">
        <div class="tags-items-container">
            <ul id="containerCTesSelecionados">
            </ul>
        </div>
    </div>
    <div class="clearfix"></div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Entrega:
                </span>
                <input type="text" id="txtNumero" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data da Entrega">Dt. Entrega</abbr>*:
                </span>
                <input type="text" id="txtDataEntrega" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista*:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
