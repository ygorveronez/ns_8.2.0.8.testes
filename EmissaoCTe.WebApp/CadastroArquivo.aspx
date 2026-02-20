<%@ Page Title="Cadastro de Arquivo" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CadastroArquivo.aspx.cs" Inherits="EmissaoCTe.WebApp.CadastroArquivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker","~/bundle/styles/plupload") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                                       "~/bundle/scripts/blockui",
                                                       "~/bundle/scripts/datepicker",
                                                       "~/bundle/scripts/fileDownload",
                                                       "~/bundle/scripts/datetimepicker",
                                                       "~/bundle/scripts/maskedinput",
                                                       "~/bundle/scripts/datatables",
                                                       "~/bundle/scripts/ajax",
                                                       "~/bundle/scripts/gridview",
                                                       "~/bundle/scripts/consulta",
                                                       "~/bundle/scripts/baseConsultas",
                                                       "~/bundle/scripts/mensagens",
                                                       "~/bundle/scripts/validaCampos",
                                                       "~/bundle/scripts/plupload",
                                                       "~/bundle/scripts/priceformat",
                                                       "~/bundle/scripts/cadastroArquivo") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Arquivo</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovoArquivo" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Novo Arquivo</button>
    <div class="row" style="margin-top: 5px;">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Inicial:
                </span>
                <input type="text" id="txtFiltroDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Final:
                </span>
                <input type="text" id="txtFiltroDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Descrição:
                </span>
                <input type="text" id="txtFiltroDescricao" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selFiltroStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="true" selected>Ativo</option>
                    <option value="false">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridArquivos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_arquivos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_arquivos">
    </div>
    <div class="clearfix"></div>
    
    <div class="modal fade" id="divArquivo" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cadastro Arquivo</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-cadastroArquivo">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Descrição*:
                                </span>
                                <input type="text" id="txtDescricao" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtData" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatus" class="form-control">
                                    <option value="true">Ativo</option>
                                    <option value="false">Inativo</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Log:
                                </span>
                                <textarea id="txtLog" class="form-control" rows="3" disabled="disabled"></textarea>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <button type="button" id="btnAnexo" class="btn btn-primary">Anexo</button>
                            <button type="button" id="btnDownloadAnexo" class="btn btn-default">Download</button>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>