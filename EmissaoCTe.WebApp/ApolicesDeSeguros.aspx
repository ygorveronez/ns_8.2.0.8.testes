<%@ Page Title="Cadastro de Apólices de Seguros" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ApolicesDeSeguros.aspx.cs" Inherits="EmissaoCTe.WebApp.ApolicesDeSeguros" %>

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
                           "~/bundle/scripts/apolicesdeseguros") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCliente" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Apólices de Seguros
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkCNPJRaiz" />
                        Todos CNPJs do cliente selecionado
                    </label>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CNPJ Seg.*:
                </span>
                <input type="text" id="txtCNPJSeguradora" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Nome Seguradora*:
                </span>
                <input type="text" id="txtNomeSeguradora" class="form-control" maxlength="30" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Ramo:
                </span>
                <input type="text" id="txtRamo" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Núm. Apólice*:
                </span>
                <input type="text" id="txtNumeroApolice" class="form-control" maxlength="20" />
            </div>
        </div>

        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Início da Vigência">Dt. Ini. Vig.*:</abbr>
                </span>
                <input type="text" id="txtDataInicioVigencia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Término da Vigência:">Dt. Térm. Vig.*:</abbr>
                </span>
                <input type="text" id="txtDataFimVigencia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="CNPJ Responsável para observação de contribuinte">CNPJ Resp. p/ obs.*:</abbr>
                </span>
                <input type="text" id="txtCNPJResposavelNaObservacaoContribuinte" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Responsável:
                </span>
                <select id="selResponsavel" class="form-control">
                    <option value="-1">Não Informado</option>
                    <option value="0">0 - Remetente</option>
                    <option value="1">1 - Expedidor</option>
                    <option value="2">2 - Recebedor</option>
                    <option value="3">3 - Destinatario</option>
                    <option value="4">4 - Emitente</option>
                    <option value="5">5 - Tomador do Serviço</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
