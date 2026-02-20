<%@ Page Title="Geração de Arquivos de Integração - DOCCOB" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="DOCCOB.aspx.cs" Inherits="EmissaoCTe.WebApp.DOCCOB" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <style type="text/css">
        div.tfs-tags {
            margin-top: 10px;
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
            -webkit-margin-start: 0px;
            -webkit-margin-end: 0px;
            -webkit-padding-start: 0px;
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
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#btnGerarDOCCOB").click(function () {
                GerarDOCCOB();
            });

            $("#btnSelecionarTodos").click(function () {
                SelecionarTodos();
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data('cpfCnpj', null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtDestinatario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data('cpfCnpj', null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            CarregarLayoutsEDI();

            SetarDadosPadrao();

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);
            CarregarConsultaDuplicatas("btnBuscarDuplicata", "btnBuscarDuplicata", "AReceber", RetornoConsultaDuplicatas, true, false);
        });
        function GerarDOCCOB() {
            //executarDownload("/DOCCOB/Gerar", {
            //    DataInicial: $("#txtDataInicial").val(),
            //    DataFinal: $("#txtDataFinal").val(),
            //    Versao: $("#selVersao").val(),
            //    CPFCNPJRemetente: $("#txtRemetente").data('cpfCnpj'),
            //    CTes: $("body").data("ctesSelecionados") != null ? JSON.stringify($("body").data("ctesSelecionados")) : "",
            //    CodigoDuplicata: $("#txtDuplicata").data('codigoDuplicata')
            //});
            executarRest("/DOCCOB/PreparaGerarao?callback=?", { CTes: $("body").data("ctesSelecionados") != null ? JSON.stringify($("body").data("ctesSelecionados")) : "" }, function (r) {
                if (r.Sucesso) {
                    executarDownload("/DOCCOB/Gerar", {
                        DataInicial: $("#txtDataInicial").val(),
                        DataFinal: $("#txtDataFinal").val(),
                        Versao: $("#selVersao").val(),
                        CPFCNPJRemetente: $("#txtRemetente").data('cpfCnpj'),
                        CPFCNPJDestinatario: $("#txtDestinatario").data('cpfCnpj'),
                        CodigoDuplicata: $("#txtDuplicata").data('codigoDuplicata'),
                        Sessao: r.Objeto,
                    });
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
        }
        function RetornoConsultaRemetente(remetente) {
            $("#txtRemetente").data('cpfCnpj', remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.CPFCNPJ + " - " + remetente.Nome);
        }
        function RetornoConsultaDestinatario(destinatario) {
            $("#txtDestinatario").data('cpfCnpj', destinatario.CPFCNPJ);
            $("#txtDestinatario").val(destinatario.CPFCNPJ + " - " + destinatario.Nome);
        }
        function RetornoConsultaDuplicatas(duplicata) {
            $("#txtDuplicata").data('codigoDuplicata', duplicata.Codigo);
            $("#txtDuplicata").val(duplicata.Numero + " - " + duplicata.Pessoa);
        }

        function CarregarLayoutsEDI() {
            executarRest("/LayoutEDI/BuscarTodosPorTipo?callback=?", { TipoLayout: 1 }, function (r) {
                if (r.Sucesso) {

                    var selVersao = document.getElementById("selVersao");

                    for (var i = 0; i < r.Objeto.length; i++) {

                        var option = document.createElement("option");

                        option.text = r.Objeto[i].Descricao;
                        option.value = r.Objeto[i].Codigo;

                        selVersao.add(option);
                    }

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
    <script id="ScriptConsultaCTe" type="text/javascript">
        $(document).ready(function () {
            AtualizarGridCTes();

            $("#btnAtualizarGridCTes").click(function () {
                AtualizarGridCTes();
            });
        });

        function AtualizarGridCTes() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                CPFCNPJRemetente: $("#txtRemetente").data("cpfCnpj"),
                CPFCNPJDestinatario: $("#txtDestinatario").data("cpfCnpj"),
                CodigoDuplicata: $("#txtDuplicata").data('codigoDuplicata'),
                inicioRegistros: 0
            };

            CriarGridView("/DOCCOB/Consultar?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: RetornoConsultaCTe }], [0], null);
        }

        function SelecionarTodos() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                CPFCNPJRemetente: $("#txtRemetente").data("cpfCnpj"),
                CPFCNPJDestinatario: $("#txtDestinatario").data("cpfCnpj"),
                CodigoDuplicata: $("#txtDuplicata").data('codigoDuplicata')
            };

            executarRest("/DOCCOB/SelecionarTodos?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    for (var i = 0; i < r.Objeto.length; i++) {
                        var cte = r.Objeto[i];

                        // Reutiliza funcao de Selecionar
                        RetornoConsultaCTe({
                            target: null,
                            data: {
                                Codigo: cte.Codigo,
                                Numero: cte.Numero,
                                ValorFrete: cte.ValorFrete
                            }
                        });
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaCTe(cte) {
            var ctesSelecionados = $("body").data("ctesSelecionados") == null ? new Array() : $("body").data("ctesSelecionados");

            for (var i = 0; i < ctesSelecionados.length; i++) {
                if (ctesSelecionados[i] == cte.data.Codigo)
                    return;
            }

            ctesSelecionados.push(cte.data.Codigo);

            $("body").data("ctesSelecionados", ctesSelecionados);

            AdicionarCTe(cte.data);
        }

        function AdicionarCTe(cte) {
            var tag = document.createElement("li");
            tag.className = "tag-item tag-item-delete-experience";
            tag.id = "cteSelecionado_" + cte.Codigo;

            var container = document.createElement("span");
            container.className = "tag-container tag-container-delete-experience";

            var descricao = document.createElement("span");
            descricao.className = "tag-box tag-box-delete-experience";
            descricao.innerHTML = "<b>" + cte.Numero + "</b> | " + cte.ValorFrete;

            var opcaoExcluir = document.createElement("span");
            opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
            opcaoExcluir.innerHTML = "&nbsp;";
            opcaoExcluir.onclick = function () { RemoverCTe(cte) };

            container.appendChild(descricao);
            container.appendChild(opcaoExcluir);

            tag.appendChild(container);

            document.getElementById("containerCTesSelecionados").appendChild(tag);
        }

        function RemoverCTe(cte) {
            var ctesSelecionados = $("body").data("ctesSelecionados") == null ? new Array() : $("body").data("ctesSelecionados");

            for (var i = 0; i < ctesSelecionados.length; i++) {
                if (ctesSelecionados[i] == cte.Codigo)
                    ctesSelecionados.splice(i, 1);
            }

            $("body").data("ctesSelecionados", ctesSelecionados);

            $("#cteSelecionado_" + cte.Codigo).remove();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Geração de Arquivos de Integração - DOCCOB
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Versão*:
                </span>
                <select id="selVersao" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Remetente:
                </span>
                <input type="text" id="txtRemetente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Destinatario:
                </span>
                <input type="text" id="txtDestinatario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Duplicata:
                </span>
                <input type="text" id="txtDuplicata" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDuplicata" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridCTes" class="btn btn-primary">Atualizar</button>
    <button type="button" id="btnGerarDOCCOB" class="btn btn-primary">Gerar DOCCOB</button>
    <button type="button" id="btnSelecionarTodos" class="btn btn-default pull-right">Selecionar Todos</button>
    <div id="tbl_ctes_consulta" style="margin-top: 10px;">
    </div>
    <div id="tbl_ctes_consulta_paginacao">
    </div>
    <div class="clearfix"></div>
    <div class="divCTesSelecionados">
        <div class="tfs-tags">
            <div class="tags-items-container">
                <ul id="containerCTesSelecionados">
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
