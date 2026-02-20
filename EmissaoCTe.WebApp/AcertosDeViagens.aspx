<%@ Page Title="Cadastro de Acertos de Viagens" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="AcertosDeViagens.aspx.cs" Inherits="EmissaoCTe.WebApp.AcertosDeViagens" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload") %>

        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/acertosDeViagens") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoAcertoViagem" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddDescricaoVeiculo" value="" />
        <input type="hidden" id="hddCodigoMotorista" value="" />
        <input type="hidden" id="hddDescricaoMotorista" value="" />
        <input type="hidden" id="hddCodigoDestino" value="0" />
        <input type="hidden" id="hddDestinos" value="" />
        <input type="hidden" id="hddCodigoFornecedor" value="" />
        <input type="hidden" id="hddDescricaoFornecedor" value="" />
        <input type="hidden" id="hddCodigoTipoDespesa" value="0" />
        <input type="hidden" id="hddDescricaoTipoDespesa" value="" />
        <input type="hidden" id="hddCodigoDespesa" value="0" />
        <input type="hidden" id="hddDespesas" value="" />
        <input type="hidden" id="hddCodigoPosto" value="" />
        <input type="hidden" id="hddDescricaoPosto" value="" />
        <input type="hidden" id="hddCodigoAbastecimento" value="0" />
        <input type="hidden" id="hddAbastecimentos" value="" />
        <input type="hidden" id="hddKmVeiculo" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Acertos de Viagens
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row acerto-viagem-container">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número:
                </span>
                <input type="text" id="txtNumero" disabled="disabled" value="Automático" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt Lançam.*:
                </span>
                <input type="text" id="txtDataLancamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt Vcto.:
                </span>
                <input type="text" id="txtDataVcto" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12"></div>
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista*:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Adiantamento*:
                </span>
                <input type="text" id="txtAdiantamento" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Comissão*:
                </span>
                <input type="text" id="txtComissao" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Comissão Sobre*:
                </span>
                <select id="selTipoComissao" class="form-control">
                    <option value="0">Valor Líquido</option>
                    <option value="1">Valor Bruto</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total de Receitas*:
                </span>
                <input type="text" id="txtTotalReceita" class="form-control" value="0,00" disabled="disabled" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total de Despesas*:
                </span>
                <input type="text" id="txtTotalDespesa" class="form-control" value="0,00" disabled="disabled" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Situação*:
                </span>
                <select id="selSituacao" class="form-control">
                    <option value="A">Aberto</option>
                    <option value="P">Pendente Pagamento</option>
                    <option value="F">Fechado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
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
    <div class="row acerto-viagem-container">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <ul class="nav nav-tabs" id="tabDadosAcertoViagem" style="margin-bottom: 10px;">
        <li><a href="#divDestinos">Destinos</a></li>
        <li><a href="#divDespesas">Despesas</a></li>
        <li><a href="#divAbastecimentos">Abastecimentos</a></li>
        <li><a href="#divVales">Vales / Devoluções</a></li>
        <li><a href="#divAnexos">Anexos</a></li>
    </ul>
    <div class="tab-content">
        <div class="tab-pane fade" id="divDestinos">
            <div id="mensagensDestinos-placeholder">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo Carga*:
                        </span>
                        <input type="text" id="txtTipoDaCarga" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarTipoDaCarga" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">CT-e:
                        </span>
                        <input type="text" id="txtCTe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">Cliente:
                        </span>
                        <input type="text" id="txtCliente" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Peso:
                        </span>
                        <input type="text" id="txtPeso" class="form-control" value="0,0000" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Valor Unitário:
                        </span>
                        <input type="text" id="txtValorUnitario" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Valor do Frete:
                        </span>
                        <input type="text" id="txtValorFrete" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Outros Desc.:
                        </span>
                        <input type="text" id="txtOutrosDescontos" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">KM Inicial:
                        </span>
                        <input type="text" id="txtKMInicial" class="form-control" value="0" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">KM Final:
                        </span>
                        <input type="text" id="txtKMFinal" class="form-control" value="0" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Data Inicial:
                        </span>
                        <input type="text" id="txtDataInicial" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="input-group">
                        <span class="input-group-addon">Data Final:
                        </span>
                        <input type="text" id="txtDataFinal" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">UF Origem*:
                        </span>
                        <select id="selUFOrigem" class="form-control">
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Município Origem*:
                        </span>
                        <select id="selMunicipioOrigem" class="form-control">
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">UF Destino*:
                        </span>
                        <select id="selUFDestino" class="form-control">
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Município Destino*:
                        </span>
                        <select id="selMunicipioDestino" class="form-control">
                        </select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">Observação:
                        </span>
                        <textarea id="txtObservacaoDestino" class="form-control" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarDestino" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirDestino" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarDestino" class="btn btn-default" style="display: none;">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblDestinos" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 10%;" colspan="1" rowspan="1">CT-e</th>
                            <th style="width: 20%;" colspan="1" rowspan="1">Tipo da Carga</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data Inicial</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data Final</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Origem</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Destino</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Peso</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor Frete</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Opções</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="8">Nenhum registro encontrado.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane fade" id="divDespesas">
            <div id="mensagensDespesas-placeholder">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Fornecedor:
                        </span>
                        <input type="text" id="txtFornecedorDespesa" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarFornecedorDespesa" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Fornecedor Não Cadastrado">Forn. Não Cad.</abbr>:
                        </span>
                        <input type="text" id="txtDescricaoFornecedorNaoCadastrado" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Tipo de Despesa">Tipo Desp.*</abbr>:
                        </span>
                        <input type="text" id="txtTipoDespesa" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarTipoDespesa" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Descrição*:
                        </span>
                        <input type="text" id="txtDescricaoDespesa" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Quantidade:
                        </span>
                        <input type="text" id="txtQuantidadeDespesa" class="form-control" value="0,00" />
                    </div>
                </div>

                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Valor Unitário:
                        </span>
                        <input type="text" id="txtValorUnitarioDespesa" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Data:
                        </span>
                        <input type="text" id="txtDataDespesa" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" id="chkDespesaPaga" value="" />
                            Paga pelo Motorista
                        </label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">Observação:
                        </span>
                        <textarea id="txtObservacaoDespesa" class="form-control" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarDespesa" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirDespesa" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarDespesa" class="btn btn-default" style="display: none;">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblDespesas" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 20%;" colspan="1" rowspan="1">Fornecedor
                            </th>
                            <th style="width: 20%;" colspan="1" rowspan="1">Tipo de Despesa
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Quantidade
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Valor Un.
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Paga
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="7">Nenhum registro encontrado.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane fade" id="divAbastecimentos">
            <div id="mensagensAbastecimentos-placeholder">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Posto:
                        </span>
                        <input type="text" id="txtPostoAbastecimento" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarPostoAbastecimento" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Posto Não Cadastrado">Pos. Não Cad.</abbr>:
                        </span>
                        <input type="text" id="txtDescricaoPostoNaoCadastrado" class="form-control" maxlength="500" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Data:
                        </span>
                        <input type="text" id="txtDataAbastecimento" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">KM Inicial:
                        </span>
                        <input type="text" id="txtKMInicialAbastecimento" class="form-control" value="0" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">KM Final:
                        </span>
                        <input type="text" id="txtKMFinalAbastecimento" class="form-control" value="0" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Litros:
                        </span>
                        <input type="text" id="txtLitrosAbastecimento" class="form-control" value="0,0000" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Valor Unitário:
                        </span>
                        <input type="text" id="txtValorUnitarioAbastecimento" class="form-control" value="0,0000" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Valor Total:
                        </span>
                        <input type="text" id="txtValorTotalAbastecimento" class="form-control" value="0,00" disabled />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Média:
                        </span>
                        <input type="text" id="txtMediaAbastecimento" class="form-control" value="0,00" disabled="disabled" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" id="chkAbastecimentoPago" value="" />
                            Pago
                        </label>
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarAbastecimento" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirAbastecimento" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarAbastecimento" class="btn btn-default" style="display: none;">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblAbastecimentos" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 20%;" colspan="1" rowspan="1">Posto
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">KM Inicial
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">KM Final
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Litros
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor Un.
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Pago
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="8">Nenhum registro encontrado.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane fade" id="divVales">
            <div id="mensagensVales-placeholder">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                    <div class="input-group">
                        <span class="input-group-addon">Número:
                        </span>
                        <input type="text" id="txtNumeroVale" disabled="disabled" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-4 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Descrição*:
                        </span>
                        <input type="text" id="txtDescricaoVale" class="form-control" value="" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Data*:
                        </span>
                        <input type="text" id="txtDataVale" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo:
                        </span>
                        <select class="form-control" id="selTipoVale">
                            <option value="1">Vale</option>
                            <option value="2">Devolução</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Valor*:
                        </span>
                        <input type="text" id="txtValorVale" class="form-control" value="0,00" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">Observação:
                        </span>
                        <textarea id="txtObservacaoVale" class="form-control" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarVale" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirVale" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarVale" class="btn btn-default">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblVales" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 10%;" colspan="1" rowspan="1">Número</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data</th>
                            <th style="width: 45%;" colspan="1" rowspan="1">Descrição</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Tipo</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Opções</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="6">Nenhum registro encontrado.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane fade" id="divAnexos">
            <div id="mensagensAnexos-placeholder">
            </div>
            <button type="button" id="btnAnexar" class="btn btn-primary">
                <span class="glyphicon glyphicon-file"></span>&nbsp;Anexar
            </button>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblAnexos" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 85%;" colspan="1" rowspan="1">Arquivo</th>
                                    <th style="width: 15%;" colspan="1" rowspan="1">Opções</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
    <button type="button" id="btnVisualizar" style="display: none" class="btn btn-primary">Visualizar</button>
</asp:Content>
