<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCobrancaCTe.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioCobrancaCTe" %>

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
    <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                                               "~/bundle/scripts/maskedinput",
                                               "~/bundle/scripts/datatables",
                                               "~/bundle/scripts/ajax",
                                               "~/bundle/scripts/gridview",
                                               "~/bundle/scripts/consulta",
                                               "~/bundle/scripts/baseConsultas",
                                               "~/bundle/scripts/mensagens",
                                               "~/bundle/scripts/validaCampos",
                                               "~/bundle/scripts/datetimepicker",
                                               "~/bundle/scripts/fileDownload") %>

    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtNumeroInicial").mask("9?999999999999999");
            $("#txtNumeroFinal").mask("9?999999999999999");

            FormatarCampoDate("txtDataInicial");
            FormatarCampoDate("txtDataFinal");

            $("#txtTomador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val('');
                        $("body").data("tomador", null);
                    }

                    e.preventDefault();
                }
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            $("#btnSelecionarTodos").click(function () {
                SelecionarTodos();
            });
            
            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
        });

        function SelecionarTodos() {
            var dados = DadosFiltro();

            executarRest("/RelatorioCobrancaCTe/SelecionarTodos?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto != null) {
                        for (var i in r.Objeto.CTes)
                            AdicionarCTe(r.Objeto.CTes[i]);
                    }
                } else {
                    ExibirMensagemErro("Ocorreu um erro ao selecionar todos CTes", "Erro ao Selecionar Todos");
                }
            });
        }

        function DownloadRelatorio() {

            if ($("body").data("ctesSelecionados") == null || $("body").data("ctesSelecionados").length <= 0) {
                ExibirMensagemAlerta("Selecione um ou mais CT-es para realizar a geração do relatório de cobrança!", "Atenção!");
                return;
            }

            if ($("body").data("tomadorCobranca") == null) {
                ExibirMensagemAlerta("Selecione o tomador para realizar a geração do relatório de cobrança!", "Atenção!");
                return;
            }
            
            //executarDownload("/RelatorioCobrancaCTe/DownloadRelatorio", dados);

            executarRest("/RelatorioCobrancaCTe/PreparaGerarao?callback=?", { CTes: JSON.stringify(ObterCTesSelecionados()) }, function (r) {
                if (r.Sucesso) {
                    executarDownload("/RelatorioCobrancaCTe/DownloadRelatorio", {
                        Numero: $("#txtNumeroCobranca").val(),
                        DataEmissao: $("#txtDataEmissao").val(),
                        DataVencimento: $("#txtDataVencimento").val(),
                        Tomador: $("body").data("tomadorCobranca"),
                        Observacao: $("#txtObservacao").val(),
                        TipoArquivo: $("#selTipoArquivo").val(),
                        Sessao: r.Objeto,
                    });
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaTomador(tomador) {
            $("body").data("tomador", tomador.CPFCNPJ);
            $("#txtTomador").val(tomador.CPFCNPJ + " - " + tomador.Nome);
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            FormatarCampoDate("txtDataEmissao");
            FormatarCampoDate("txtDataVencimento");

            var dataInicial = new Date();
            var dataFinal = new Date();
            dataInicial.setDate(dataFinal.getDate() - 1);

            $("#txtDataInicial").val(Globalize.format(dataInicial, "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(dataFinal, "dd/MM/yyyy"));

            CarregarConsultadeClientes("btnBuscarTomadorCobranca", "btnBuscarTomadorCobranca", RetornoConsultaTomadorCobranca, true, false);

            $("#txtTomadorCobranca").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val('');
                        $("body").data("tomadorCobranca", null);
                    }

                    e.preventDefault();
                }
            });

            $("#btnAtualizarGridCTes").click(function () {
                AtualizarGridCTes();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
            AtualizarGridCTes();
        });

        function RetornoConsultaTomadorCobranca(tomador) {
            $("body").data("tomadorCobranca", tomador.CPFCNPJ);
            $("#txtTomadorCobranca").val(tomador.CPFCNPJ + " - " + tomador.Nome);
        }

        function AtualizarGridCTes() {
            var dados = DadosFiltro();

            var opcoes = new Array();
            opcoes.push({ Descricao: "Selecionar", Evento: AdicionarCTe });

            CriarGridView("/RelatorioCobrancaCTe/ConsultarCTes?callback=?", dados, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", opcoes, [0], null, null);
        }

        function ObterCTesSelecionados() {
            var ctes = $("body").data("ctesSelecionados") || new Array();
            var codigos = new Array();

            for (var i = 0; i < ctes.length; i++)
                codigos.push(ctes[i].Codigo);

            return codigos;
        }

        function AdicionarCTe(cte) {
            var ctes = $("body").data("ctesSelecionados") || new Array();
            if (typeof cte.data != "undefined")
                cte = cte.data;

            for (var i = 0; i < ctes.length; i++)
                if (ctes[i].Codigo == cte.Codigo)
                    return;

            ctes.push(cte);
            $("body").data("ctesSelecionados", ctes);

            var tag = document.createElement("li");
            tag.className = "tag-item tag-item-delete-experience";
            tag.id = "cteSelecionado_" + cte.Codigo;

            var container = document.createElement("span");
            container.className = "tag-container tag-container-delete-experience";

            var descricao = document.createElement("span");
            descricao.className = "tag-box tag-box-delete-experience";
            descricao.innerHTML = "<b>" + cte.Numero + "-" + cte.Serie + "</b> | " + cte.Valor;

            var opcaoExcluir = document.createElement("span");
            opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
            opcaoExcluir.innerHTML = "&nbsp;";
            opcaoExcluir.onclick = function () { RemoverCTeSelecionado(cte.Codigo) };

            container.appendChild(descricao);
            container.appendChild(opcaoExcluir);

            tag.appendChild(container);

            document.getElementById("containerCTesSelecionados").appendChild(tag);

            $("#lblSemCTes").hide();
        }

        function RemoverCTeSelecionado(codigo) {
            var ctes = $("body").data("ctesSelecionados");

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

        function LimparCampos() {
            $("#txtNumeroCobranca").val("");
            $("#txtObservacao").val("");
            $("#txtDataEmissao").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataVencimento").val("");
            $("#txtTomadorCobranca").val("");
            $("body").data("tomadorCobranca", null);
            $("#selTipoArquivo").val($("#selTipoArquivo option:first").val());

            $("body").data("ctesSelecionados", null);
            $("#containerCTesSelecionados").html("");
            $("#lblSemCTes").show();
        }
        function DadosFiltro() {
            return {
                InicioRegistros: 0,
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Tomador: $("body").data("tomador"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val()
            };
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Cobrança de CT-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tomador:
                </span>
                <input type="text" id="txtTomador" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridCTes" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <button type="button" id="btnSelecionarTodos" class="btn btn-default pull-right">Selecionar Todos</button>
    <div id="tbl_ctes" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_ctes">
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número:
                </span>
                <input type="text" id="txtNumeroCobranca" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Emissão:
                </span>
                <input type="text" id="txtDataEmissao" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Vencto.:
                </span>
                <input type="text" id="txtDataVencimento" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-9">
            <div class="input-group">
                <span class="input-group-addon">Tomador:
                </span>
                <input type="text" id="txtTomadorCobranca" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTomadorCobranca" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs.:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Relatório</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar / Limpar Campos</button>
</asp:Content>
