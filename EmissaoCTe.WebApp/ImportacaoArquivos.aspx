<%@ Page Title="Importação de Arquivos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="false" CodeBehind="ImportacaoArquivos.aspx.cs" Inherits="EmissaoCTe.WebApp.ImportacaoArquivos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/importacao.css" rel="stylesheet">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/importacaoArquivos")%>

        <script>
            $(document).ready(function () {
                $("#form1").on('submit', function (e) {
                    if (e && e.preventDefault) e.preventDefault();
                });
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Importação de Arquivos</h2>
    </div>
    <div class="importador">
		<div class="header-container">				
            <a href="#" id="btnIniciaProcesso" class="btn-main btn btn-success">
                <span class="glyphicon glyphicon-upload btn-icon"></span>
                <span class="btn-title">Iniciar</span>
            </a>

            <a href="#" id="btnConverter" class="btn-main btn btn-info hide">
                <span class="glyphicon glyphicon-random btn-icon"></span>
                <span class="btn-title">Importar</span>
            </a>
		</div>

        <div id="messages-placeholder"></div>

		<div class="table-container">
			<table class="table table-hover main-table empty">
                <thead>
                    <tr>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td></td></tr><tr><td></td></tr><tr><td></td></tr><tr><td></td></tr><tr><td></td></tr>
                    <tr><td class="table-placeholder">Tabela de Importação</td></tr>
                    <tr><td></td></tr><tr><td></td></tr><tr><td></td></tr><tr><td></td></tr><tr><td></td></tr>
                </tbody>
			</table>
		</div>

		<div class="legendas">
            <h4><strong>Legenda (Veículos):</strong></h4>
			<div><strong>Tipo:</strong> P = Próprio / T = Terceiro</div>
            <div><strong>Tipo Veículo:</strong> 0 = Tração / 1 = Reboque</div>
            <div><strong>Tipo Rodado:</strong> 00 = Não Aplicado / 01 = Truck / 02 = Toco / 03 = Cavalo / 04 = Van / 05 = Utilitários / 06 = Outros</div>
            <div><strong>Tipo Carroceria:</strong> 00 = Não Aplicado / 01 = Aberta / 02 = Fechada Baú / 03 = Granel / 04 = Porta Container / 05 = Sider</div>
            <div><strong>Tipo Proprietário:</strong> 0 = TAC Agregado / 1 = TAC Independente / 2 = Outros</div>
		</div>

		<div class="modal fade modal-importacao modal-result" tabindex="-1" role="dialog">
			<div class="modal-dialog modal-lg" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h4 class="modal-title">Resultado Importação</h4>
					</div>
					<div class="modal-body">
                        <div id="carregandoImportacao">
                            <div class="text-center">Aguarde...</div>
                        </div>

                        <div id="mensagemRetorno"></div>
                        <div id="errosRetorno"></div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-primary" data-dismiss="modal" disabled="disabled">Fechar</button>
					</div>
				</div><!-- /.modal-content -->
			</div><!-- /.modal-dialog -->
		</div><!-- /.modal -->

		<div class="modal fade modal-importacao modal-tipo-importacao" tabindex="-1" role="dialog">
			<div class="modal-dialog modal-lg" role="document">
				<div class="modal-content">
					<div class="modal-close">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
					</div>

					<div class="modal-header">
						<h3 class="modal-title">Escolha o tipo de importação</h3>
					</div>
					<div class="modal-body">
                        <ul class="boxes tipos-importacao">
                            <li>
                                <a href="#" class="box tipo-importacao" data-tipo="Veiculos" data-titulo="Veículos">
                                    <div class="box-icon">
                                        <span class="glyphicon glyphicon-road"></span>
                                    </div>
                                    <div class="box-title">Veículos</div>
                                 </a>
                            </li>
                            
                            <li>
                                <a href="#" class="box tipo-importacao" data-tipo="Motoristas" data-titulo="Motoristas">
                                    <div class="box-icon">
                                        <span class="glyphicon glyphicon-user"></span>
                                    </div>
                                    <div class="box-title">Motoristas</div>
                                 </a>
                            </li>
                        </ul>

                        <a href="#" class="modal-arrows arrow-right hide"><span class="glyphicon glyphicon-menu-right"></span></a>
					</div>
				</div><!-- /.modal-content -->
			</div><!-- /.modal-dialog -->
		</div><!-- /.modal -->

		<div class="modal fade modal-importacao modal-upload-arquivo" tabindex="-1" role="dialog">
			<div class="modal-dialog modal-lg" role="document">
				<div class="modal-content">
					<div class="modal-close">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
					</div>

					<div class="modal-header">
						<h3 class="modal-title">Selecione o arquivo (<span class="tipo-importar"></span>)</h3>
					</div>
					<div class="modal-body">

                        <div class="text-center file-upload">
                            <a href="#" class="btn-main btn btn-primary file-button" id="file-button">
                                <span class="glyphicon glyphicon-file btn-icon"></span>
                                <span class="btn-title">Arquivo</span>
                            </a>
                            
                            <a href="#" class="btn-main btn btn-success processar-arquivo hide">
                                <span class="glyphicon glyphicon-ok btn-icon"></span>
                                <span class="btn-title">Processar!</span>
                            </a>

                            <div><span class="file-name"></span><span class="file-percent"></span></div>
                        </div>

                        <div id="messages-placeholder-upload" style="margin-top: 20px;"></div>

                        <a href="#" class="modal-arrows arrow-left"><span class="glyphicon glyphicon-menu-left"></span></a>
					</div>
				</div><!-- /.modal-content -->
			</div><!-- /.modal-dialog -->
		</div><!-- /.modal -->
    </div>
</asp:Content>
