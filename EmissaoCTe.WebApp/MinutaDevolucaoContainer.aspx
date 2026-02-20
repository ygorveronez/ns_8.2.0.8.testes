<%@ Page Title="Minuta de Devolução de Container" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="MinutaDevolucaoContainer.aspx.cs" Inherits="EmissaoCTe.WebApp.MinutaDevolucaoContainer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
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
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/minutaDevolucaoContainer") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Minuta de Devolução de Container
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número para controle interno">Número</abbr>:
                </span>
                <input type="text" id="txtNumero" readonly class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Container*:
                </span>
                <input type="text" id="txtContainer" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Importador*:
                </span>
                <input type="text" id="txtImportador" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-3 col-md-2 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="1">Ativo</option>
                    <option value="0">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Terminal*:
                </span>
                <input type="text" id="txtTerminal" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTerminal" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Armador:
                </span>
                <input type="text" id="txtArmador" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo de Equipamento:
                </span>
                <input type="text" id="txtTipoEquipamento" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Quantidade:
                </span>
                <input type="text" id="txtQuantidade" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Peso/Tara:
                </span>
                <input type="text" id="txtPeso" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Navio:
                </span>
                <input type="text" id="txtNavio" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tração:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Reboque:
                </span>
                <input type="text" id="txtReboque" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarReboque" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CT-e:
                </span>
                <input type="text" id="txtCTe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Obs.:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3" maxlength="2000"></textarea>
            </div>
        </div>
    </div>
   
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnDownloadVia1" class="btn btn-primary" style="display: none;"><span class="glyphicon glyphicon-download"></span>&nbsp;1ª Via</button>
    <button type="button" id="btnDownloadVia2" class="btn btn-primary" style="display: none;"><span class="glyphicon glyphicon-download"></span>&nbsp;2ª Via</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
