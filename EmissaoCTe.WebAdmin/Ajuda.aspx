<%@ Page Title="Ajuda" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Ajuda.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Ajuda" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".portlet-header").hover(function () {
                $(this).addClass("ui-portlet-hover");
            },
            function () {
                $(this).removeClass("ui-portlet-hover");
            });
            $(".portlet-header .ui-icon").click(function () {
                $(this).toggleClass("ui-icon-circle-arrow-n");
                $(this).parents(".portlet:first").find(".portlet-content").toggle();
            });
        });
        $(function () {
            var allVideos = $("iframe[src^='http://www.youtube.com']"),
                fluidElement = $("#divAjudaCadastroEmpresas");

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
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Ajuda
                </h3>
            </div>
            <div class="content-box" style="min-height: 500px;">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="three-column">
                                <div class="three-col-mid">
                                    <div class="column col1">
                                        <div class="portlet ui-widget ui-widget-content ui-helper-clearfix ui-corner-all">
                                            <div class="portlet-header ui-widget-header ui-portlet-hover">
                                                Administração de Empresas Emissoras
                                                <span class="ui-icon ui-icon-circle-arrow-s"></span>
                                            </div>
                                            <div id="divAjudaCadastroEmpresas" class="portlet-content">
                                                <iframe width="640" height="390" src="http://www.youtube.com/embed/G9pDTYFBWKs" frameborder="0" allowfullscreen></iframe>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="column col2 ui-sortable">
                                        <div class="portlet ui-widget ui-widget-content ui-helper-clearfix ui-corner-all">
                                            <div class="portlet-header ui-widget-header ui-portlet-hover">
                                                Informações
                                                <span class="ui-icon ui-icon-circle-arrow-s"></span>
                                            </div>
                                            <div class="portlet-content">
                                                <p>
                                                    O Conhecimento de Transporte eletrônico (CT-e) foi instituído pelo <b><a href="http://www.fazenda.gov.br/confaz/confaz/ajustes/2007/AJ_009_07.htm">Ajuste SINIEF 09/2007</a></b>, de 25/10/2007.
                                                </p>
                                                <p>
                                                    Segundo o <b><a href="http://www.fazenda.gov.br/confaz/confaz/ajustes/2011/aj_018_11.htm">Ajuste SINIEF 18</a></b>, de 21 de Dezembro de 2011, para os contribuintes do modal rodoviário cadastrados com regime de apuração normal, a obrigatoriedade de emissão de CT-e tem inicio em 01/08/2013.
                                                </p>
                                                <p>
                                                    Ainda segundo o mesmo Ajuste SINIEF, para os contribuintes optantes pelo regime do Simples Nacional a obrigatoriedade de emissão de CT-e tem inicio em 01/12/2013.
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="column col3 ui-sortable">
                                        <div class="portlet ui-widget ui-widget-content ui-helper-clearfix ui-corner-all">
                                            <div class="portlet-header ui-widget-header ui-portlet-hover">
                                                Dados Adicionais
                                                <span class="ui-icon ui-icon-circle-arrow-s"></span>
                                            </div>
                                            <div class="portlet-content">
                                                <p>
                                                    Para visualizar informações à respeito do Projeto CT-e é possível acessar o 
                                                    <b><a href="http://www.cte.fazenda.gov.br/">site do SEFAZ</a></b>.
                                                </p>
                                                <p>
                                                    Para baixar o manual do CT-e da versão 1.0.4c <b><a href="http://www.cte.fazenda.gov.br/exibirArquivo.aspx?conteudo=jTQBSxPUInM=">clique aqui</a></b>.
                                                </p>
                                                <p>
                                                    Se você deseja consultar a disponibilidade dos serviços do SEFAZ <b><a href="http://www.cte.fazenda.gov.br/disponibilidade.aspx?versao=1.00&tipoConteudo=XbSeqxE8pl8=">clique aqui</a></b>.
                                                </p>
                                                <p>
                                                    Para visualizar e fazer o download das notas técnicas do Projeto CT-e <b><a href="http://www.cte.fazenda.gov.br/listaConteudo.aspx?tipoConteudo=Y0nErnoZpsg=">clique aqui</a></b>.
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
