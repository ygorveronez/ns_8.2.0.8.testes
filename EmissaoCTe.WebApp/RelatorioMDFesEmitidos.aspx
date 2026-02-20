<%@ Page Title="Relatório de MDF-es Emitidos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioMDFesEmitidos.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioMDFesEmitidos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtDataEmissaoInicial, #txtDataEmissaoFinal").mask("99/99/9999");
            $("#txtDataEmissaoInicial, #txtDataEmissaoFinal").datepicker();
            $("#txtNumeroInicial, #txtNumeroFinal, #txtCPFMotorista").mask("9?9999999999999");

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            $("#btnAtualizarGrid").click(function () {
                AtualizarGridMDFes();
            });

            RemoveConsulta("#txtUsuario", function ($this) {
                $this.val("");
                for (var i in $this.data())
                    $this.data(i, "");
            });

            var date = new Date();
            $("#txtDataEmissaoInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 1), "dd/MM/yyyy"));
            $("#txtDataEmissaoFinal").val(Globalize.format(date, "dd/MM/yyyy"));

            $("#btnBaixarLote").click(function () {
                var dados = DadosGrid();
                executarDownload("/RelatorioMDFesEmitidos/DownloadLoteXML", dados);
            });

            $("#btnBaixarLoteDAMDFE").click(function () {
                var dados = DadosGrid();
                executarDownload("/RelatorioMDFesEmitidos/DownloadLoteDAMDFE", dados);
            });

            CarregarConsultaUsuario("btnBuscarUsuario", "btnBuscarUsuario", "A", "U", RetornoConsultaUsuario, true, false);
            AtualizarGridMDFes();
        });

        function AtualizarGridMDFes() {
            var dados = DadosGrid();

            var opcoes = [
                { Descricao: "DAMDFE", Evento: FactoryDownloaMDFe("DAMDFE") },
                { Descricao: "XML Autorizacao", Evento: FactoryDownloaMDFe("XMLAutorizacao") },
                { Descricao: "XML Cancelamento", Evento: FactoryDownloaMDFe("XMLCancelamento") },
                { Descricao: "XML Encerramento", Evento: FactoryDownloaMDFe("XMLEncerramento") }
            ];

            CriarGridView("/RelatorioMDFesEmitidos/Consultar?callback=?", dados, "tbl_mdfes_table", "tbl_mdfes", "tbl_paginacao_mdfes", opcoes, [0, 1]);
        }

        function RetornoConsultaUsuario(user) {
            $("#txtUsuario").val(user.Nome);
            $("#txtUsuario").data("nome", user.Nome);
            $("#txtUsuario").data("codigo", user.Codigo);
        }

        function FactoryDownloaMDFe(url) {
            return function (mdfe) {
                executarDownload("/ManifestoEletronicoDeDocumentosFiscais/Download" + url, { CodigoMDFe: mdfe.data.Codigo });
            }
        }
        function DownloadRelatorio() {
            var dados = DadosGrid()

            executarDownload("/RelatorioMDFesEmitidos/DownloadRelatorio", dados);
        }

        function DadosGrid() {
            return {
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataInicial: $("#txtDataEmissaoInicial").val(),
                DataFinal: $("#txtDataEmissaoFinal").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                UFCarregamento: $("#selUFCarregamento").val(),
                UFDescarregamento: $("#selUFDescarregamento").val(),
                Status: $("#ddlStatus").val(),
                TipoRelatorio: $("#ddlTipoRelatorio").val(),
                NomeMotorista: $("#txtNomeMotorista").val(),
                CPFMotorista: $("#txtCPFMotorista").val(),
                Placa: $("#txtVeiculo").val(),
                PlacaVeiculo: $("#txtVeiculo").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                NomeUsuario: $("#txtUsuario").data("nome"),
                NumeroCarga: $("#txtNumeroCarga").val(),
                NumeroUnidade: $("#txtNumeroUnidade").val(),            
                inicioRegistros: 0
            };
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de MDF-es Emitidos
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">UF Carga:
                </span>
                <asp:DropDownList ID="selUFCarregamento" runat="server" ClientIDMode="Static" CssClass="form-control">
                    <asp:ListItem Value="" Text="Todas"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">UF Descarga:
                </span>
                <asp:DropDownList ID="selUFDescarregamento" runat="server" ClientIDMode="Static" CssClass="form-control">
                    <asp:ListItem Value="" Text="Todas"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="ddlStatus" class="form-control">
                    <option value="-1">Todos</option>
                    <option value="0">Em Digitação</option>
                    <option value="1">Pendente</option>
                    <option value="2">Enviado</option>
                    <option value="3">Autorizados</option>
                    <option value="4">Em Encerramento</option>
                    <option value="5">Encerrados</option>
                    <option value="6">Em Cancelamento</option>
                    <option value="7">Cancelados</option>
                    <option value="9">Rejeitados</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="ddlTipoRelatorio" class="form-control">
                    <option value="RelatorioMDFesEmitidosSumarizado">Sumarizado - Veículos</option>
                    <option value="RelatorioMDFesEmitidosCompleto">Completo - Veículos</option>
                    <option value="RelatorioMDFesEmitidosSumarizadoMotorista">Sumarizado - Motorista</option>
                    <option value="RelatorioMDFesEmitidosCompletoMotorista">Completo - Motorista</option>
                    <option value="RelatorioMDFesEmitidosCompletoCTes">Completo - CTes</option>
                    <option value="RelatorioMDFesEmitidosAverbacaoSumarizado">Averbações</option>
                    <option value="RelatorioMDFesEmitidosSeguradora">Seguradora</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
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
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros Adicionais
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Motorista:
                                </span>
                                <input type="text" id="txtNomeMotorista" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">CPF Motorista:
                                </span>
                                <input type="text" id="txtCPFMotorista" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">Placa Veículo:
                                </span>
                                <input type="text" id="txtVeiculo" class="form-control" maxlength="7" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input type="text" id="txtUsuario" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarUsuario" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Apenas para MDF-es gerados via integração">Número da Carga</abbr>:
                                </span>
                                <input type="text" id="txtNumeroCarga" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Apenas para MDF-es gerados via integração">Número da Unidade</abbr>:
                                </span>
                                <input type="text" id="txtNumeroUnidade" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGrid" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary"><span class="glyphicon glyphicon-print"></span>&nbsp;Gerar Relatório</button>
    <button type="button" id="btnBaixarLote" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de XML</button>
    <button type="button" id="btnBaixarLoteDAMDFE" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de DAMDFE</button>
    <div id="tbl_mdfes" class="table-responsive" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_mdfes">
    </div>
</asp:Content>
