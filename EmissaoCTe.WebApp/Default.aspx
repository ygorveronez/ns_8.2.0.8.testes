<%@ Page Title="Home" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="EmissaoCTe.WebApp.Default" MasterPageFile="Site.Master" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <ampl-survey identifier="67f585df20864bf868b6a570"></ampl-survey>
    <script defer src="https://cdn.amplifique.me/amplifiqueme-inapp-survey.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/ajax") %>
    </asp:PlaceHolder>
    <script id="ScriptDataExpiracaoCertificado" type="text/javascript">
        $(document).ready(function () {
            BuscarDataVencimentoCertificado();
        });
        function BuscarDataVencimentoCertificado() {
            executarRest("/Empresa/ObterDetalhesCertificado?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.DataVencimento != null && r.Objeto.DataVencimento.trim() != "")
                        $("#informacoesGerais").append("Data de expiração do certificado digital: <b>" + r.Objeto.DataVencimento + "</b>.<br/>");
                }
            }, false);
        }
    </script>
    <script id="ScriptCTesPendentesEntrega" type="text/javascript">
        $(document).ready(function () {
            ObterTotalDeCTesPendentesDeEntrega();
        });
        function ObterTotalDeCTesPendentesDeEntrega() {
            executarRest("/ConhecimentoDeTransporteEletronico/ObterQuantidadeDeCTesPendentesDeEntrega?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.Total > 0)
                        $("#informacoesGerais").append("Há <b>" + r.Objeto.Total + "</b> CT-es não entregues esse mês.<br/>");
                }
            }, false);
        }
    </script>
    <script id="ScriptMDFEsPendenteEncerramento" type="text/javascript">
        $(document).ready(function () {
            ObterTotalDeMDFesPendenteEncerramento();
        });
        function ObterTotalDeMDFesPendenteEncerramento() {
            executarRest("/ManifestoEletronicoDeDocumentosFiscais/ConsultarPendentesDeEncerramento?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.length > 0) {
                        var ulPendentes = $("#ulMDFesPendenteEncerramento");
                        var urlPagina = "";
                        for (var i = 0; i < r.Objeto.length; i++) {
                            urlPagina = "EmissaoMDFe.aspx?x=" + r.Objeto[i].CodigoCriptografado;
                            var html = "<a href='" + urlPagina + "' class='list-group-item'><h5 class='list-group-item-heading'>MDFe "+ r.Objeto[i].Numero + " | Série: "+r.Objeto[i].Serie + " | Chave " +r.Objeto[i].Chave + "<br> Dt. Autorização: " + r.Objeto[i].DataAutorizacao + " | Veículo: " + r.Objeto[i].Veiculo +"<br /></h5><p class='list-group-item-text'>";
                            html += "</p></a>";
                            ulPendentes.append(html);
                        }
                        $("#divMDFesPendenteEncerramento").show();
                    }
                }
            }, false);
        }
    </script>
    <script id="ScriptGraficos" type="text/javascript">
        google.load("visualization", "1", { packages: ["corechart"] });
        google.setOnLoadCallback(RenderizarGraficos);
        function RenderizarGraficos() {
            RenderizarGraficoDeValoresDeCTe();
            RenderizarGraficoDeCTesEmitidos();
        }
        function RenderizarGraficoDeValoresDeCTe() {
            executarRest("/ConhecimentoDeTransporteEletronico/ObterValoresDoMes?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.MaiorValor > 0 || r.Objeto.ValorMedio > 0 || r.Objeto.MenorValor > 0) {
                        var dataTable = new google.visualization.DataTable();
                        dataTable.addColumn('string', 'Variação');
                        dataTable.addColumn('number', 'Valores');
                        dataTable.addColumn({ type: 'string', role: 'tooltip' });
                        dataTable.addRows([
                          ['Maior Valor', r.Objeto.MaiorValor, Globalize.format(r.Objeto.MaiorValor, "n2")],
                          ['Valor Médio', r.Objeto.ValorMedio, Globalize.format(r.Objeto.ValorMedio, "n2")],
                          ['Menor Valor', r.Objeto.MenorValor, Globalize.format(r.Objeto.MenorValor, "n2")]
                        ]);
                        var options = {
                            title: 'Valores de CT-es Emitidos no Mês Atual',
                            legend: {
                                position: 'none'
                            },
                            chartArea: { left: 40, top: 20, width: "80%", height: "78%" }
                        };
                        new google.visualization.ColumnChart(document.getElementById('divGraficoValoresCTe')).draw(dataTable, options);
                        $("#divGraficoValoresCTe").show();
                        $("#divGraficos").show();
                    }
                }
            }, false);
        }
        function RenderizarGraficoDeCTesEmitidos() {
            executarRest("/ConhecimentoDeTransporteEletronico/ObterQuantidadesDoMes?callback=?", {}, function (r) {

                if (r.Sucesso && r.Objeto != null) {

                    if (r.Objeto.length > 0) {

                        var dataTable = new google.visualization.DataTable();
                        var slices = new Array();

                        dataTable.addColumn('string', 'Variação');
                        dataTable.addColumn('number', 'Valores');

                        for (var i = 0; i < r.Objeto.length; i++) {
                            dataTable.addRow([r.Objeto[i].DescricaoStatus, r.Objeto[i].Quantidade]);
                            slices.push({ color: r.Objeto[i].Cor });
                        }

                        var options = {
                            title: 'Quantidades de CT-es no Mês Atual',
                            legend: { alignment: 'center' },
                            chartArea: { left: 0, top: 20, width: "80%", height: "78%" },
                            slices: slices,
                            pieSliceTextStyle: { color: 'black' }
                        };

                        new google.visualization.PieChart(document.getElementById('divGraficoQuantidadesCTe')).draw(dataTable, options);

                        $("#divGraficoQuantidadesCTe").show();
                        $("#divGraficos").show();

                    }
                }
            }, false);
        }
    </script>
    <script id="ScriptServicosDeVeiculos" type="text/javascript">
        $(document).ready(function () {
            ObterServicosDeVeiculosPendentes();
        });
        function ObterServicosDeVeiculosPendentes() {
            executarRest("/ServicoDeVeiculo/ObterServicosPendentes?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.length > 0) {
                        var ulServicos = $("#ulServicosPendentes");
                        for (var i = 0; i < r.Objeto.length; i++) {
                            var html = "<a href='javascript:void(0);' class='list-group-item'><h5 class='list-group-item-heading'>" + r.Objeto[i].Servicos[0].Placa + "</h5><p class='list-group-item-text'>";
                            for (var j = 0; j < r.Objeto[i].Servicos.length; j++) {
                                var dataServico = Globalize.parseDate(r.Objeto[i].Servicos[j].DataVcto);
                                html += Globalize.format(dataServico, "dd/MM/yyyy") + " - " + r.Objeto[i].Servicos[j].DescricaoServico + ".<br />";
                            }
                            html += "</p></a>";
                            ulServicos.append(html);
                        }
                        $("#divServicosPendentes").show();
                    }
                }
            }, false);
        }
    </script>

    <script id="ScriptParcelasPendentes" type="text/javascript">
        $(document).ready(function () {
            ObterParcelasPendentes();
        });
        function ObterParcelasPendentes() {
            executarRest("/Duplicatas/ObterParcelasPendentes?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.length > 0) {
                        var ulParcelas = $("#ulParcelasPendentes");
                        var urlPagina = "";
                        for (var i = 0; i < r.Objeto.length; i++) {
                            urlPagina = "BaixaDeParcelasDuplicatas.aspx?x=" + r.Objeto[i].CodigoCriptografado;
                            var html = "<a href='" + urlPagina + "' class='list-group-item'><h5 class='list-group-item-heading'>Duplicata "+ r.Objeto[i].Tipo+": " + r.Objeto[i].NumeroDuplicata + " | " + r.Objeto[i].PessoaDuplicata + "<br> Vencimento: " + r.Objeto[i].VencimentoParcela + " | Valor: R$" + r.Objeto[i].ValorParcela + "<br /></h5><p class='list-group-item-text'>";
                            html += "</p></a>";
                            ulParcelas.append(html);
                        }
                        $("#divParcelasPendentes").show();
                    }
                }
            }, false);
        }
    </script>

    <script id="ScriptPgtoMotoristasPendentes" type="text/javascript">
        $(document).ready(function () {
            ObterPgtoMotoristasPendentes();
        });
        function ObterPgtoMotoristasPendentes() {
            executarRest("/PagamentoMotorista/ObterPagamentosPendentes?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.length > 0) {
                        var ulParcelas = $("#ulPgtoMotoristasPendentes");
                        var urlPagina = "";
                        for (var i = 0; i < r.Objeto.length; i++) {
                            urlPagina = "PagamentoMotorista.aspx?x=" + r.Objeto[i].CodigoCriptografado;
                            var html = "<a href='" + urlPagina + "' class='list-group-item'><h5 class='list-group-item-heading'>Número: " + r.Objeto[i].Numero + " |  Motorista: " + r.Objeto[i].Motorista + "<br> Vencimento: " + r.Objeto[i].DataPagamento + " | Valor: R$" + r.Objeto[i].ValorFrete + "<br /></h5><p class='list-group-item-text'>";
                            html += "</p></a>";
                            ulParcelas.append(html);
                        }
                        $("#divPgtoMotoristasPendentes").show();
                    }
                }
            }, false);
        }
    </script>

    <script id="ScriptAcertosPendentes" type="text/javascript">
        $(document).ready(function () {
            ObterAcertosPendentes();
        });
        function ObterAcertosPendentes() {
            executarRest("/AcertoDeViagem/ObterAcertosPendentes?callback=?", {}, function (r) {
                if (r.Sucesso && r.Objeto != null) {
                    if (r.Objeto.length > 0) {
                        var ulParcelas = $("#ulAcertosPendentes");
                        var urlPagina = "";
                        for (var i = 0; i < r.Objeto.length; i++) {
                            urlPagina = "AcertosDeViagens.aspx?x=" + r.Objeto[i].CodigoCriptografado;
                            var html = "<a href='" + urlPagina + "' class='list-group-item'><h5 class='list-group-item-heading'>Número: " + r.Objeto[i].Numero + " |  Motorista: " + r.Objeto[i].Motorista + "<br> Vencimento: " + r.Objeto[i].DataPagamento + " | Valor: R$" + r.Objeto[i].Valor + "<br /></h5><p class='list-group-item-text'>";
                            html += "</p></a>";
                            ulParcelas.append(html);
                        }
                        $("#divAcertosPendentes").show();
                    }
                }
            }, false);
        }
    </script>
    
    <script id="ScriptMensagensDeAviso">
        $(document).ready(function () {
            ObterMensagensDeAviso();
        });
        function ObterMensagensDeAviso() {
            executarRest("/MensagemDeAviso/ObterMensagensParaExibicao?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    for (var i = 0; i < r.Objeto.length; i++) {
                        var div = document.createElement("div");
                        div.className = "alert alert-warning";
                        div.innerHTML = "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button><strong>" + r.Objeto[i].Titulo + "</strong><br/><br/>" + r.Objeto[i].Descricao.replace(/\n/g, "<br/>");

                        $("#messages-placeholder").append(div);
                    }
                }
            });
        }
    </script>
    <script id="ScriptLinkTermosDeUso">
        $(document).ready(function () {
            ExibirLinkTermosDeUso();
        });
        function ExibirLinkTermosDeUso() {
            executarRest("/EmpresaContrato/ObterContratoEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.Contrato != undefined && r.Objeto.Contrato != "")
                        $("#btnTermosDeUso").show();
                }
            });
        }
    </script>

    <script id="ScriptPesquisaNPS" type="text/javascript">
        $(document).ready(function () {
            ExibirPesquisaNPS();
        });
        function ExibirPesquisaNPS() {
            executarRest("/Usuario/ObterConfiguracaoParaPesquisaNPS?callback=?", {}, function (r) {
                const sistemaDescricao = 'MultiCT-e';

                if (r.Sucesso && r.Objeto != null) {
                    window.Amplifiqueme = {};
                    window.Amplifiqueme.onLoad = () => {
                        try {
                            window.Amplifiqueme.identify(
                                {
                                    name: r.Objeto.usuarioNome,
                                    email: r.Objeto.usuarioEmail,
                                    created_at: r.Objeto.empresaDataCadastro,
                                    company: sistemaDescricao,
                                    custom_fields: {
                                        "empresa_codigo": r.Objeto.empresaCodigo,
                                        "cpfcnpj": r.Objeto.empresaCNPJ,
                                        "solucao": sistemaDescricao,
                                        "produto": sistemaDescricao
                                    }
                                }, true);
                        } catch (error) {
                            console.error("Erro ao chamar Amplifiqueme.identify:", error);
                        }
                    };
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Home
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <p id="informacoesGerais">
    </p>

    <p id="termosDeUso">
        <button type="button" onclick="location.href='TermosDeUso.aspx?x=true';" class="btn btn-primary" id="btnTermosDeUso" style="display: none; overflow: hidden;">Termos de Uso</button>
    </p>

    <div class="panel panel-default" id="divGraficos" style="display: none; overflow: hidden;">
        <div class="panel-heading">
            <b>Gráficos de Utilização</b>
        </div>
        <div class="panel-body">
            <div id="divGraficoQuantidadesCTe" class="pull-left" style="width: 360px; height: 240px; display: none; overflow: hidden;">
            </div>
            <div id="divGraficoValoresCTe" class="pull-left" style="width: 360px; height: 240px; display: none; overflow: hidden;">
            </div>
        </div>
    </div>
    <div class="panel panel-default" id="divServicosPendentes" style="display: none;">
        <div class="panel-heading">
            <b>Serviços de Veículos Pendentes</b>
        </div>
        <div id="ulServicosPendentes" class="list-group">
        </div>
    </div>
    <div class="panel panel-default" id="divParcelasPendentes" style="display: none;">
        <div class="panel-heading">
            <b>Parcelas Duplicatas Pendentes</b>
        </div>
        <div id="ulParcelasPendentes" class="list-group">
        </div>
    </div>
    <div class="panel panel-default" id="divMDFesPendenteEncerramento" style="display: none;">
        <div class="panel-heading">
            <b>MDFe's pendentes de Encerramento com mais de um dia de emissão</b>
        </div>
        <div id="ulMDFesPendenteEncerramento" class="list-group">
        </div>
    </div>
    <div class="panel panel-default" id="divPgtoMotoristasPendentes" style="display: none;">
        <div class="panel-heading">
            <b>Pagamento Motoristas Pendentes</b>
        </div>
        <div id="ulPgtoMotoristasPendentes" class="list-group">
        </div>
    </div>
    <div class="panel panel-default" id="divAcertosPendentes" style="display: none;">
        <div class="panel-heading">
            <b>Acertos de Viagens Pendentes</b>
        </div>
        <div id="ulAcertosPendentes" class="list-group">
        </div>
    </div>
    
</asp:Content>
