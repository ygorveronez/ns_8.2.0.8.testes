<%@ Page Title="Importar CT-e(s)" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ImportarCTes.aspx.cs" Inherits="EmissaoCTe.WebApp.ImportarCTes" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/importacaoctes") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Importação de CT-e(s)</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnImportarCTe" class="btn btn-default"><span class="glyphicon glyphicon-road"></span>&nbsp;Importar CT-e</button>
    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtInicialCTeFiltro" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtFinalCTeFiltro" class="form-control maskedInput" />
            </div>
        </div>
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle btn-block" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros Adicionais
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicialCTeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Final:
                                </span>
                                <input type="text" id="txtNumeroFinalCTeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Série:
                                </span>
                                <asp:DropDownList ID="ddlSerieFiltro" runat="server" CssClass="form-control" ClientIDMode="Static">
                                    <asp:ListItem Text="Todas" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Finalidade:
                                </span>
                                <select id="selFinalidadeCTeFiltro" class="form-control">
                                    <option value="">Todas</option>
                                    <option value="0">Normal</option>
                                    <option value="1">Complemento</option>
                                    <option value="2">Anulação</option>
                                    <option value="3">Substituto</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusCTeFiltro" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="A">Autorizado</option>
                                    <option value="C">Cancelado</option>
                                    <option value="D">Denegado</option>
                                    <option value="S">Em Digitação</option>
                                    <option value="E">Enviado</option>
                                    <option value="I">Inutilizado</option>
                                    <option value="P">Pendente</option>
                                    <option value="R">Rejeição</option>
                                    <option value="K">Em Cancelamento</option>
                                    <option value="L">Em Inutilização</option>
                                    <option value="Y">Aguardando Consulta Manual</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Ocorrência:
                                </span>
                                <select id="selTipoOcorrencia" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="P">Pendente</option>
                                    <option value="F">Final</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Rem.:
                                </span>
                                <input type="text" id="txtCPFCNPJRemetenteFiltro" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Rem.:
                                </span>
                                <input type="text" id="txtRemetenteCTeFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRemetenteCTeFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Dest.:
                                </span>
                                <input type="text" id="txtCPFCNPJDestinatarioFiltro" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Dest.:
                                </span>
                                <input type="text" id="txtDestinatarioCTeFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDestinatarioCTeFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 col-sm-2 col-md-2 col-lg-1">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <asp:CheckBox ID="chkContem" runat="server" ClientIDMode="Static" />
                                        Contém
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-4 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. NF:
                                </span>
                                <input type="text" id="txtNumeroNF" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Motorista:
                                </span>
                                <input type="text" id="txtMotoristaCTeFiltro" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Placa Veíc.:
                                </span>
                                <input type="text" id="txtPlacaCTeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarCTe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar CT-e</button>
    <div id="tbl_ctes" style="margin-top: 10px;"></div>
    <div id="tbl_paginacao_ctes"></div>
    <div class="clearfix"></div>
    <div style="height: 470px;"></div>
</asp:Content>
