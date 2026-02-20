<%@ Page Title="Controle Certificados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ControleCertificados.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ControleCertificados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #mdlTitulo {
            margin-left: 5px;
            margin-bottom: 10px;
            font-weight: bold;
        }
        #divDetalhadoDetalhes {
            max-height: 320px;
            overflow-y: auto;
        }
    </style>

    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.filedownload.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(function () {
            $("#txtDiasVencimento").mask("9?9999", { placeholder: "" });
            $("#txtDiasVencimento").val("45");
            $("#txtDataInicio, #txtDataFim").mask("99/99/9999").datepicker({ changeMonth: true, changeYear: true });
            AtualizarGrid();

            $("#btnConsultar").click(function () {
                AtualizarGrid();
            });
            $("#btnExportar").click(function () {
                ExportarGrid();
            });
        });

        function DadosGrid() {
            return {
                inicioRegistros: 0,
                Status: $("#selFiltroStatus").val(),
                Cidade: $("#txtCidade").val(),
                StatusVenda: $("#selFiltroStatusVenda").val(),
                Satisfacao: $("#selFiltroSatisfacao").val(),
                DiasVencimento: $("#txtDiasVencimento").val(),
                CNPJ: $("#txtCNPJ").val(),
                Nome: $("#txtNome").val(),
                Ambiente: $("#txtAmbiente").val(),
                DataInicio: $("#txtDataInicio").val(),
                DataFim: $("#txtDataFim").val()
            };
        }

        function ExportarGrid() {
            var dados = DadosGrid();

            executarDownload("/VencimentoCertificado/ExportarConsulta?callback=?", dados);
        }

        function AtualizarGrid() {
            var dados = DadosGrid();
            var opcoes = [
                { Descricao: "Lançar Histórico", Evento: LancarHistorico },
                { Descricao: "Informar Atualização", Evento: InformarAtualizacao },
                { Descricao: "Inativar", Evento: Inativar },
                { Descricao: "Visualizar Históricos", Evento: AbrirModalDetalhes },
                { Descricao: "Acessar Cadastro", Evento: AcessarCadastro },
            ];
            var colunasOcultar = [0];

            CriarGridView("/VencimentoCertificado/Consulta?callback=?", dados, "tbl_certificados_table", "tbl_certificados", "tbl_paginacao_certificados", opcoes, colunasOcultar, null, true);
        }

        function LancarHistorico(item) {
            var exibirOpcoes = true;
            AbrirModalHistorico(item, "Lançamento de Histórico", "LancamentoHistorico", exibirOpcoes);
        }
        function InformarAtualizacao(item) {
            var exibirOpcoes = false;
            AbrirModalHistorico(item, "Informar Atualização", "AtualizarHistorico", exibirOpcoes);
        }
        function Inativar(item) {
            var exibirOpcoes = false;
            AbrirModalHistorico(item, "Inativar", "InativarCertificado", exibirOpcoes);
        }
        function AcessarCadastro(item) {
            var $a = $("<a>", { target: "_blank" });
            $a.attr("href", "EmpresasEmissoras.aspx?CNPJ=" + item.data.Cnpj);

            // Força o clique acessando o elemento direto
            $a[0].click();
        }
    </script>
    <script type="text/javascript" id="Historico">
        $(function () {
            executarRest("/Usuario/ObterUsuarioLogado?callback=?", {}, function (r) {
                $("#txtUsuario").val(r.Objeto.Nome);
            });

            $("#btnSalvar").click(SalvarHistorico);
        });

        function LimparCamposHistorico() {
            $("#divHistorico").data('Historico', {});
            $("#txtDataHora").val(DataHoraAtual());
            $("#txtDetalhe").val("");
            $("#selStatusVenda").val($("#selStatusVenda option:first").val());
            $("#selSatisfacao").val("4");            
        }

        function SalvarHistorico() {
            var dadosHistorico = $("#divHistorico").data('Historico');
            
            var dados = {
                CNPJ: dadosHistorico.Cnpj,
                Vencimento: dadosHistorico.Vencimento,
                Detalhes: $("#txtDetalhe").val(),
                StatusVenda: $("#selStatusVenda").val(),
                Satisfacao: $("#selSatisfacao").val(),
            };

            executarRest("/VencimentoCertificado/" + dadosHistorico.Metodo + "?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                    $.fancybox.close();
                    LimparCamposHistorico();
                    AtualizarGrid();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção", "divMensagemErroHistorico");
                }
            });
        }

        function DataHoraAtual() {
            var currentdate = new Date();
            var hora = currentdate.getHours();
            var dia = currentdate.getDate();
            var mes = currentdate.getMonth() + 1;
            var datetime = (dia < 10 ? '0' + dia : dia) + "/"
                            + (mes < 10 ? '0' + mes : mes) + "/"
                            + currentdate.getFullYear() + " "
                            + (hora < 10 ? '0' + hora : hora) + ":"
                            + currentdate.getMinutes();
            return datetime;
        }

        function AbrirModalHistorico(item, titulo, metodo, exibirOpcoes) {
            LimparCamposHistorico();

            var dados = item.data;
            
            dados.Metodo = metodo;
            $("#divHistorico").data('Historico', dados);
            $("#mdlTitulo").html(titulo);

            if (dados.Satisfacao != null)
                $("#selSatisfacao").val(dados.Satisfacao);

            if (exibirOpcoes)
                $("#divStatusVenda").show();
            else
                $("#divStatusVenda").hide();

            $.fancybox({
                href: '#divHistorico',
                width: 800,
                height: 400,
                fitToView: false,
                autoSize: false,
                closeClick: false,
                closeBtn: true,
                openEffect: 'none',
                closeEffect: 'none',
                centerOnScroll: true,
                type: 'inline',
                padding: 7,
                scrolling: 'no',
                helpers: { overlay: { css: { cursor: 'auto' }, closeClick: false } },
            });
        }
    </script>
    <script type="text/javascript" id="Detalhes">
        $(function () {
        });

        function AtualizarGridDetalhes() {
            var detalhes = $("#divDetalhes").data("Detalhes");
            var dados = {
                inicioRegistros: 0,
                Vencimento: detalhes.Vencimento,
                CNPJ: detalhes.Cnpj
            };

            var opcoes = [
                { Descricao: "Abrir", Evento: DetalharInformacao }
            ];
            var colunasOcultar = [0, 1];

            CriarGridView("/VencimentoCertificado/ConsultaDetalhes?callback=?", dados, "tbl_detalhes_table", "tbl_detalhes", "tbl_paginacao_detalhes", opcoes, colunasOcultar, null, false);
        }

        function DetalharInformacao(info) {
            var data = info.data;

            $("#txtDetalhadoDataLancamento").val(data.DataLancamento);
            $("#txtDetalhadoTipo").val(data.Tipo);
            $("#txtDetalhadoSatisfacao").val(data.Satisfacao);
            $("#txtDetalhadoUsuario").val(data.Usuario);
            $("#divDetalhadoDetalhes").html(data.Detalhes.replace(/(?:\r\n|\r|\n)/g, '<br />'));

            // Calcula tamanho da div de detalhes
            var max_height = 700;
            var table_height = $("#tbl_detalhes").outerHeight();
            var div_height = max_height - table_height;
            $("#divDetalhadoDetalhes").css('max-height', div_height + 'px');

            $("#divDetalhado").show();
        }

        function AbrirModalDetalhes(item) {
            $("#divDetalhado").hide();
            $("#divDetalhes").data("Detalhes", item.data);

            AtualizarGridDetalhes();

            $.fancybox({
                href: '#divDetalhes',
                width: 800,
                height: 900,
                fitToView: false,
                autoSize: false,
                closeClick: false,
                closeBtn: true,
                openEffect: 'none',
                closeEffect: 'none',
                centerOnScroll: true,
                type: 'inline',
                padding: 7,
                scrolling: 'no',
                helpers: { overlay: { css: { cursor: 'auto' }, closeClick: false } },
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Controle Certificados</h3>
            </div>
            <div class="content-box gridview-container">
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

                        <div class="">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Nome:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNome" />
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        CNPJ:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCNPJ" />
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Ambiente:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtAmbiente" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Data Início:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataInicio" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Data Fim:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataFim" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Dias Vencimento:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDiasVencimento" />
                                </div>
                            </div>
                        </div>
                        <div class="">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Cidade:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCidade" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieltres">
                                <div class="label">
                                    <label>
                                        Status:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selFiltroStatus" class="select">
                                        <option value="P">Vencidos / Não Vencidos</option>
                                        <option value="V">Vencidos</option>                                    
                                        <option value="N">Não Vencidos</option>           
                                        <option value="A">Atualizados</option>
                                        <option value="C">Atualização confirmada</option>
                                        <option value="I">Inativos</option>
                                        <option value="">Todos</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieltres">
                                <div class="label">
                                    <label>
                                        Status Venda:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selFiltroStatusVenda" class="select">
                                        <option value="">Todos</option>
                                        <option value="!3">Todos com contato</option>
                                        <option value="1">Aguardando Dados para Adesão</option>
                                        <option value="2">Certificado Vendido</option>
                                        <option value="3">Sem Contato</option>
                                        <option value="4">Retornar</option>
                                        <option value="5">Providenciando</option>
                                        <option value="6">Bloqueado</option>
                                        <option value="8">Atualizado</option>
                                        <option value="9">Inativado</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieltres">
                                <div class="label">
                                    <label>
                                        Satisfação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selFiltroSatisfacao" class="select">
                                        <option value="">Todos</option>
                                        <option value="4">Não Avaliado</option>
                                        <option value="1">Ruim</option>
                                        <option value="2">Bom</option>
                                        <option value="3">Ótimo</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="fields">
                        <div class="buttons" style="margin-left: 5px; margin-top: 0px;">
                            <input type="button" id="btnConsultar" value="Consultar" />

                            <input type="button" id="btnExportar" value="Exportar" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_certificados">
                            </div>
                            <div id="tbl_paginacao_certificados" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div style="display: none;">
        <div id="divDetalhes" style="height: 300px;">
            <div class="content-box gridview-container">
                <div class="form">
                    <h3>Detalhes Certificado</h3>
                    <div class="fields">
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_detalhes">
                            </div>
                            <div id="tbl_paginacao_detalhes" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="content-box" id="divDetalhado">
                <div class="form">
                    <h3>Detalhado</h3>
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Usuário:</label>
                                </div>
                                <div class="input">
                                    <input type="text" readonly="readonly" id="txtDetalhadoUsuario" />
                                </div>
                            </div>
                            <div class="field fieldum"> 
                                <div class="input"></div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Data:</label>
                                </div>
                                <div class="input">
                                    <input type="text" readonly="readonly" id="txtDetalhadoDataLancamento" />
                                </div>
                            </div>
                            <div class="field fieldum"> 
                                <div class="input"></div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Tipo:</label>
                                </div>
                                <div class="input">
                                    <input type="text" readonly="readonly" id="txtDetalhadoTipo" />
                                </div>
                            </div>
                            <div class="field fieldum"> 
                                <div class="input"></div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Satisfação:</label>
                                </div>
                                <div class="input">
                                    <input type="text" readonly="readonly" id="txtDetalhadoSatisfacao" />
                                </div>
                            </div>
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>Detalhes:</label>
                                </div>
                                <div class="input">
                                    <div id="divDetalhadoDetalhes"  style="width: 99%"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <div id="divHistorico" style="height: 300px;">
            <div class="content-box">
                <div class="form">
                    <h3 id="mdlTitulo">Lançamento de Histórico</h3>
                    <div class="fields">
                        <div class="response-msg error ui-corner-all" id="divMensagemErroHistorico" style="display: none;"><span></span><label class="mensagem"></label></div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlertaHistorico" style="display: none;"><span></span><label class="mensagem"></label></div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucessoHistorico" style="display: none;"><span></span><label class="mensagem"></label></div>

                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        <b>Data Hora:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" disabled="disabled" id="txtDataHora" />
                                </div>
                            </div>
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        <b>Usuário:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" disabled="disabled" id="txtUsuario" />
                                </div>
                            </div>
                            <div id="divStatusVenda">
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            <b>Status da Venda:</b>
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selStatusVenda" class="select">
                                            <option value="1">Aguardando Dados para Adesão</option>
                                            <option value="2">Certificado Vendido</option>
                                            <option value="3">Sem Contato</option>
                                            <option value="4">Retornar</option>
                                            <option value="5">Providenciando</option>
                                            <option value="6">Bloqueado</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            <b>Satisfação:</b>
                                        </label>
                                    </div>
                                    <div class="input">
                                    <select id="selSatisfacao">
                                        <option value="3">Ótimo</option>
                                        <option value="2">Bom</option>
                                        <option value="1">Ruim</option>
                                        <option value="4" selected="selected">Não Avaliado</option>
                                    </select>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 10px;">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Detalhes:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtDetalhe" rows="11" cols="20" style="width: 99.5%;"></textarea>
                                </div>
                            </div>
                        </div>

                        <div class="fieldzao">
                            <div class="buttons">
                                <input type="button" id="btnSalvar" value="Salvar" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
