<%@ Page Title="Cadastro Fretes Subcontratados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="FreteSubcontratado.aspx.cs" Inherits="EmissaoCTe.WebApp.FreteSubcontratado" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker")%>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/freteSubcontratado",
                           "~/bundle/scripts/fileDownload")%>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro Fretes Subcontratados
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Parceiro*:
                </span>
                <input type="text" id="txtParceiro" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarParceiro" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Filial*:
                </span>
                <input type="text" id="txtFilial" class="form-control" value="" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">CTe*:
                </span>
                <input type="text" id="txtCTe" class="form-control" maxlength="9" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">NFe*:
                </span>
                <input type="text" id="txtNFe" class="form-control" maxlength="9" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrada*:
                </span>
                <input type="text" id="txtDataEntrada" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">Entrega</option>
                    <option value="1">Reentrega</option>
                    <option value="2">Coleta</option>
                    <option value="3">Devolução</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control" disabled="disabled">
                    <option value="0">Aberto</option>
                    <option value="1">Fechado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Remetente*:
                </span>
                <input type="text" id="txtRemetente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Destinatario*:
                </span>
                <input type="text" id="txtDestinatario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Cidade:
                </span>
                <input type="text" id="txtLocalidade" class="form-control" value="" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual comissão (%)">Perc. Com.(%)</abbr>:
                </span>
                <input type="text" id="txtPercentualComissao" class="form-control" maxlength="18" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor mínimo da comissão">Min. Com.</abbr>:
                </span>
                <input type="text" id="txtValorComissaoMinimo" class="form-control" maxlength="18" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Peso (kg) da mercadoria">Peso</abbr>*:
                </span>
                <input type="text" id="txtPeso" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Quantidade (unitária) da mercadoria">Qtd.</abbr>:
                </span>
                <input type="text" id="txtQuantidade" class="form-control" maxlength="18" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Vlr Frete*:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Vlr. ICMS:
                </span>
                <input type="text" id="txtValorICMS" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Tx. Adic.:
                </span>
                <input type="text" id="txtValorTaxaAdicional" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor do Frete Líquido (Valor Frete - Valor ICMS - Taxa Adicional)">Frete Liq.</abbr>:
                </span>
                <input type="text" id="txtValorFreteLiquido" class="form-control" maxlength="18" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Perc. Comissão sobre o Frete Liquido ou Valor Mínimo Comissão">Comissão</abbr>:
                </span>
                <input type="text" id="txtValorComissao" class="form-control" maxlength="18" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor TDA:
                </span>
                <input type="text" id="txtValorTDA" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor TDE:
                </span>
                <input type="text" id="txtValorTDE" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Carro Ded.:
                </span>
                <input type="text" id="txtValorCarroDedicado" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor Comissão + TDA + TDE + Tx. Adicional">Vlr. Total Comissão</abbr>:
                </span>
                <input type="text" id="txtValorTotalComissao" class="form-control" maxlength="18" disabled />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Receb. / Doc. :
                </span>
                <input type="text" id="txtRecebedorDocumento" class="form-control" value="" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrega:
                </span>
                <input type="text" id="txtDataEntrega" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
