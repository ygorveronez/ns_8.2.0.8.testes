<%@ Page Title="Ajuda" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Ajuda.aspx.cs" Inherits="EmissaoCTe.WebApp.Ajuda" %>

<%@ Import Namespace="System.Web.Optimization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/apolicesdeseguros",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <style>
        .video-single .video-title {
            font-weight: bold;
            margin-bottom: 10px;
            display: block;
        }
        .video-single {
            margin-bottom: 30px;
        }
        #divAjudaVideos {
            margin-bottom: 0px;
        }
        .arquivo-container {
            padding: 20px;
        }
        .arquivo-container .arquivo {
            border-top-right-radius: 4px;
            border-bottom-right-radius: 4px;
            border-top-left-radius: 4px;
            border-bottom-left-radius: 4px;
            background-color: #eee;
            padding: 10px;
            height: 160px;
            margin-bottom: 20px;
        }

        .arquivo-container .arquivo .arquivo-icone {
            text-align: center;
            padding-bottom: 20px;
        }

        .arquivo-container .arquivo .arquivo-icone div {
            width: 100%;
            height: 50px;
            background-position: center;
            background-size: contain;
            background-repeat: no-repeat;
        }

        .arquivo-container .arquivo .arquivo-nome {
            text-align: center;
            font-weight: bold;
            line-height: 15px;
            min-height: 45px;
            word-break: break-all;
        }

        .arquivo-container .arquivo .arquivo-nome a {
            color: #000;
        }

        .arquivo-container .arquivo .arquivo-nome a:hover,
        .arquivo-container .arquivo .arquivo-nome a:active,
        .arquivo-container .arquivo .arquivo-nome a:focus {
            text-decoration: none;
        }
    </style>
    <script type="text/javascript">
        var EnumTipoAjuda = {
            Arquivo: 1,
            Video: 2
        };
        function stop(e) {
            if (e && e.preventDefault) e.preventDefault();
        }
        $(document).ready(function () {
            var allVideos = $("iframe[src^='http://www.youtube.com']"),
                fluidElement = $("#divAjudaEmissaoCTe");

            allVideos.each(function () {
                $(this)
                    .data('aspectRatio', this.height / this.width)
                    .removeAttr('height')
                    .removeAttr('width');
            });

            $(window).resize(function () {
                var newWidth = fluidElement.width();
                allVideos.each(function () {
                    var element = $(this);
                    element.width(newWidth)
                           .height(newWidth * element.data('aspectRatio'));
                });
            }).resize();

            BuscaAjudas();
        });

        function DownloadArquivo(codigo) {
            var dados = {
                Codigo: codigo,
                Download: "MultiCTe"
            };

            executarDownload("/Ajuda/DownloadArquivo?callback=?", dados);
        }

        function BuscaAjudas() {
            executarRest("/Ajuda/ObterAjudasPorEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    var arquivos = FiltrarAjudas(r.Objeto, EnumTipoAjuda.Arquivo);
                    var videos = FiltrarAjudas(r.Objeto, EnumTipoAjuda.Video);

                    RenderizarVideos(videos);
                    RenderizarArquivos(arquivos);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function RenderizarVideos(ajudas) {
            var $ajudas_html = [];

            ajudas.forEach(function (ajuda) {
                var template = '<div class="panel panel-default">' +
                                    '<div class="panel-heading" role="tab" id="heading' + ajuda.Codigo + '">' +
                                        '<h4 class="panel-title">' +
                                            '<a role="button" data-toggle="collapse" data-parent="#divAjudaVideos" href="#collapse' + ajuda.Codigo + '">' + ajuda.Descricao + '</a>' +
                                        '</h4>' +
                                    '</div>' +
                                    '<div id="collapse' + ajuda.Codigo + '" class="panel-collapse collapse" role="tabpanel" aria-labelledby="heading' + ajuda.Codigo + '">' +
                                        '<div class="panel-body"><iframe width="560" height="315" src="https://www.youtube.com/embed/' + ajuda.LinkVideo + '" frameborder="0" allowfullscreen></iframe></div>' +
                                    '</div>' +
                                '</div>';
                var $html = $(template);
                $ajudas_html.push($html);
            });

            $("#divAjudaVideos").append($ajudas_html);
        }


        function RenderizarArquivos(ajudas) {
            var $ajudas_html = [];

            ajudas.forEach(function (ajuda) {
                var template =  '<div class="col-sm-4 col-md-3 col-lg-2">' +
                                    '<div class="arquivo">' +
                                        '<div class="arquivo-icone">' +
                                            '<div style="background-image: url(\'Images/arquivo-dicas.png\')"></div>' +
                                        '</div>' +
                                        '<div class="arquivo-nome"><a href="#" title="' + ajuda.Descricao + '">' + ajuda.Descricao + '</a></div>' +
                                    '</div>' +
                                '</div>' ;
                var $html = $(template);
                $html.on('click', 'a', function (e) {
                    stop(e);
                    DownloadArquivo(ajuda.Codigo);
                });
                $ajudas_html.push($html);
            });

            $("#divAjudaArquivos").append($ajudas_html);
        }

        function FiltrarAjudas(ajudas, tipo) {
            return ajudas.filter(function (ajuda) {
                return ajuda.Tipo == tipo;
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Ajuda</h2>
    </div>
    <div class="panel-group" id="accordion">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#videos">Videos</a>
                </h4>
            </div>
            <div id="videos" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="panel-group" id="divAjudaVideos" role="tablist" aria-multiselectable="true"></div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#arquivos">Manuais</a>
                </h4>
            </div>
            <div id="arquivos" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="arquivo-container">
                        <div class="row" id="divAjudaArquivos">
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#informacoes">Informações
                    </a>
                </h4>
            </div>
            <div id="informacoes" class="panel-collapse collapse">
                <div class="panel-body">
                    <p>
                        O Conhecimento de Transporte eletrônico (CT-e) foi instituído pelo <b><a href="http://www.fazenda.gov.br/confaz/confaz/ajustes/2007/AJ_009_07.htm">Ajuste SINIEF 09/2007</a></b>, de 25/10/2007.
                    </p>
                    <p>
                        Segundo o <b><a href="http://www.fazenda.gov.br/confaz/confaz/ajustes/2011/aj_018_11.htm">Ajuste SINIEF 18</a></b>, de 21 de Dezembro de 2011, para os contribuintes do modal rodoviário cadastrados com regime de apuração normal, a obrigatoriedade de emissão de CT-e tem inicio em 01/08/2013.
                    </p>
                    <p>
                        Ainda segundo o mesmo Ajuste SINIEF, para os contribuintes optantes pelo regime do Simples Nacional a obrigatoriedade de emissão de CT-e tem inicio em 01/12/2013.
                    </p>
                    <p>
                        Para visualizar informações à respeito do Projeto CT-e é possível acessar o <b><a href="http://www.cte.fazenda.gov.br/">site do SEFAZ</a></b>.
                    </p>
                    <p>
                        Para visualizar e fazer o download dos manuais do Projeto CT-e <b><a href="http://www.cte.fazenda.gov.br/portal/listaConteudo.aspx?tipoConteudo=YIi+H8VETH0=">clique aqui</a></b>.
                    </p>
                    <p>
                        Se você deseja consultar a disponibilidade dos serviços do SEFAZ <b><a href="http://www.cte.fazenda.gov.br/disponibilidade.aspx?versao=1.00&tipoConteudo=XbSeqxE8pl8=">clique aqui</a></b>.
                    </p>
                    <p>
                        Para visualizar e fazer o download das notas técnicas do Projeto CT-e <b><a href="http://www.cte.fazenda.gov.br/portal/listaConteudo.aspx?tipoConteudo=Y0nErnoZpsg=">clique aqui</a></b>.
                    </p>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#dadosAdicionais">Problemas na Visualização ou Utilização do Sistema
                    </a>
                </h4>
            </div>
            <div id="dadosAdicionais" class="panel-collapse collapse">
                <div class="panel-body">
                    <p>
                        O MultiCTe utiliza as mais novas tecnologias para desenvolvimento de sistemas Web. Devido a isto, navegadores antigos não suportam estas novas tecnologias.
                        Se você está tendo problemas com a visualização de páginas (a página fica desconfigurada) ou a utilização do sistema fica lenta / pouco fluída, recomendamos que você atualize seu navegador para uma versão mais recente.
                        Seguem abaixo algumas opções de navegadores que recomendamos:
                    </p>
                    <ul>
                        <li><b><a href="http://www.google.com/chrome/" target="_blank">Google Chrome</a></b></li>
                        <li><b><a href="http://windows.microsoft.com/pt-br/internet-explorer/download-ie" target="_blank">Internet Explorer 10</a></b></li>
                        <li><b><a href="http://www.mozilla.org/pt-BR/firefox/new/" target="_blank">Mozilla Firefox</a></b></li>
                        <li><b><a href="http://www.opera.com/pt-br/computer/windows" target="_blank">Opera</a></b></li>
                        <li><b><a href="http://support.apple.com/kb/DL1531?viewlocale=pt_BR" target="_blank">Safari</a></b></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
