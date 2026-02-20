<%@ Page Title="Cadastros de Ocorrências de CT-es em Lote" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OcorrenciasDeCTesLote.aspx.cs" Inherits="EmissaoCTe.WebApp.OcorrenciasDeCTesLote" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos") %>
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
    </asp:PlaceHolder>
    <script type="text/javascript">
        var CodigoTipoOcorrencia = 0;
        $(document).ready(function () {
            var today = new Date();
            var yesterday = new Date(today);
            var tomorrow = new Date(today);
            yesterday.setDate(today.getDate() - 1);
            tomorrow.setDate(today.getDate() + 1);

            FormatarCampoDateTime("txtDataDaOcorrencia");
            FormatarCampoDate("txtDataEmissaoInicial");
            FormatarCampoDate("txtDataEmissaoFinal");

            $("#txtDataEmissaoInicial").val(Globalize.format(today, "dd/MM/yyyy"));
            $("#txtDataEmissaoFinal").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

            $("#txtNumeroInicial").mask("9?999999999");
            $("#txtNumeroFinal").mask("9?999999999");

            CarregarConsultaDeTiposDeOcorrenciasDeCTes("btnBuscarOcorrencia", "btnBuscarOcorrencia", RetornoConsultaTipoOcorrenciaCTe, true, false);

            RemoveConsulta($("#txtOcorrencia"), function ($this) {
                $this.val("");
                CodigoTipoOcorrencia = 0;
            });

            $("#btnGerarOcorrencias").click(function () {
                GerarOcorrencias();
                AtualizarGridCTes();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnAtualizar").click(function () {
                AtualizarGridCTes();
            });

            $("#btnSelecionarTodos").click(function () {
                SelecionarTodos();
            });

            LimparCampos();            
        });

        function RetornoConsultaOcorrenciaCTe(ocorrencia) {
            executarRest("/OcorrenciaDeCTe/ObterDetalhes?callback=?", { Codigo: ocorrencia.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigoTipoOcorrencia").val(r.Objeto.CodigoTipoOcorrencia);
                    $("#txtOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);
                    $("#hddCodigoCTe").val(r.Objeto.CodigoCTe);
                    $("#txtCTe").val(r.Objeto.DescricaoCTe);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#btnSalvar").attr("disabled", true);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaTipoOcorrenciaCTe(tipoOcorrencia) {
            CodigoTipoOcorrencia = tipoOcorrencia.Codigo;
            $("#txtOcorrencia").val(tipoOcorrencia.Descricao);
        }
        function SetarDadosPadrao() {
            $("#txtDataDaOcorrencia").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
        }
        function LimparCampos() {
            CodigoTipoOcorrencia = 0;

            $("#txtDataDaOcorrencia").val("");
            $("#txtObservacao").val("");
            $("#txtOcorrencia").val("");

            $("#containerCTesSelecionados .tag-delete").click();

            SetarDadosPadrao();
        }
        function ValidarCampos() {
            var dataOcorrencia = $("#txtDataDaOcorrencia").val();
            var valido = true;
            if (dataOcorrencia != "") {
                CampoSemErro("#txtDataDaOcorrencia");
            } else {
                CampoComErro("#txtDataDaOcorrencia");
                valido = false;
            }
            if (CodigoTipoOcorrencia > 0) {
                CampoSemErro("#txtOcorrencia");
            } else {
                CampoComErro("#txtOcorrencia");
                valido = false;
            }
            return valido;
        }
        function GerarOcorrencias() {
            if (ValidarCampos()) {
                var ctesSelecionados = $("body").data("ctesSelecionados") == null ? new Array() : $("body").data("ctesSelecionados");

                if (ctesSelecionados.length == 0)
                    return jAlert("Nenhum CT-e foi selecionado", "Seleção CT-e");

                var dados = {
                    CodigoTipoOcorrencia: CodigoTipoOcorrencia,
                    CodigoCTes: JSON.stringify(ctesSelecionados),
                    DataDaOcorrencia: $("#txtDataDaOcorrencia").val(),
                    Observacao: $("#txtObservacao").val()
                };
                executarRest("/OcorrenciaDeCTe/GerarEmLote?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }

        function AtualizarGridCTes() {
            var dados = {
                inicioRegistros: 0,
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                TipoOcorrencia : $("#selSituacao").val()
            };

            CriarGridView("/ConhecimentoDeTransporteEletronico/Consultar?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: RetornoConsultaCTe }], [0, 1, 2, 13, 14], null, null, 20);
        }

        function SelecionarTodos() {
            var dados = {
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                TipoOcorrencia: $("#selSituacao").val()
            };

            executarRest("/OcorrenciaDeCTe/SelecionarTodos?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    for (var i = 0; i < r.Objeto.length; i++) {
                        var cte = r.Objeto[i];

                        // Reutiliza funcao de Selecionar
                        RetornoConsultaCTe({
                            target: null,
                            data: {
                                Codigo: cte.Codigo,
                                Numero: cte.Numero,
                                Valor: cte.ValorFrete
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
            descricao.innerHTML = "<b>" + cte.Numero + "</b> | " + cte.Valor;

            var opcaoExcluir = document.createElement("span");
            opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
            opcaoExcluir.innerHTML = "&nbsp;";
            opcaoExcluir.onclick = function () {
                RemoverCTe(cte);
            };

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
        <h2>Cadastro de Ocorrências de CT-es em Lote</h2>
    </div>

    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Ocorrência*:
                </span>
                <input type="text" id="txtDataDaOcorrencia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Ocorrência*:
                </span>
                <input type="text" id="txtOcorrencia" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarOcorrencia" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>

    <div class="divCTesSelecionados" style="margin-bottom: 5px;">
        <div class="tfs-tags">
            <div class="tags-items-container">
                <ul id="containerCTesSelecionados">
                </ul>
            </div>
        </div>
    </div>

    <button type="button" id="btnGerarOcorrencias" class="btn btn-primary">Gerar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>

    <div style="margin-top: 20px">
        <div class="row" style="margin-top: 15px;">
            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                <div class="input-group">
                    <span class="input-group-addon">Data Inicial:
                    </span>
                    <input type="text" id="txtDataEmissaoInicial" class="form-control maskedInput" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                <div class="input-group">
                    <span class="input-group-addon">Data Final:
                    </span>
                    <input type="text" id="txtDataEmissaoFinal" class="form-control maskedInput" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                <div class="input-group">
                    <span class="input-group-addon">Núm. Inicial:
                    </span>
                    <input type="text" id="txtNumeroInicial" class="form-control maskedInput" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                <div class="input-group">
                    <span class="input-group-addon">Núm. Final:
                    </span>
                    <input type="text" id="txtNumeroFinal" class="form-control maskedInput" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                <div class="input-group">
                    <span class="input-group-addon">Situação:
                    </span>
                    <select id="selSituacao" class="form-control">
                        <option value="P">Sem Ocorrência Final</option>
                        <option value="F">Com Ocorrência Final</option>
                        <option value="">Todas</option>
                    </select>
                </div>
            </div>
        </div>
        <button type="button" id="btnAtualizar" class="btn btn-primary">Atualizar</button>
        <button type="button" id="btnSelecionarTodos" class="btn btn-default">Selecionar Todos</button>

        <div id="tbl_ctes_consulta" style="margin-top: 10px;">
        </div>
        <div id="tbl_ctes_consulta_paginacao">
        </div>
        <div class="clearfix"></div>
    </div>
</asp:Content>
