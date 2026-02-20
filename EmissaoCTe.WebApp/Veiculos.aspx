<%@ Page Title="Cadastro de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Veiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.Veiculos" %>

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
                           "~/bundle/scripts/veiculos") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddVeiculosVinculados" value="" />
        <input type="hidden" id="hddCodigoTipoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoMarcaVeiculo" value="0" />
        <input type="hidden" id="hddCodigoModeloVeiculo" value="0" />
        <input type="hidden" id="hddCodigoProprietario" value="" />
        <input type="hidden" id="hddCodigoFornecedorValePedagio" value="" />
        <input type="hidden" id="hddCodigoResponsavelValePedagio" value="" />
        <input type="hidden" id="hddCodigoTecnologiaRastreador" value="" />
        <input type="hidden" id="hddCodigoTipoComunicacaoRastreador" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Veículos
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
                <span class="input-group-addon">Placa*:
                </span>
                <input type="text" id="txtPlaca" class="form-control maskedInput text-uppercase" maxlength="7" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">RENAVAM*:
                </span>
                <input type="text" id="txtRenavam" class="form-control" maxlength="11" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tara Kg*:
                </span>
                <input type="text" id="txtTaraKG" class="form-control maskedInput" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Capacidade em Quilogramas">Cap. Kg</abbr>:
                </span>
                <input type="text" id="txtCapacidadeKG" class="form-control maskedInput" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Capacidade em Metros Cúbicos">Cap. M&sup3;</abbr>:
                </span>
                <input type="text" id="txtCapacidadeM3" class="form-control maskedInput" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Km Atual:
                </span>
                <input type="text" id="txtKilometragemAtual" class="form-control maskedInput" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Propriedade*:
                </span>
                <select id="selTipoPropriedade" class="form-control">
                    <option value="P">Próprio</option>
                    <option value="T">Terceiros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Veíc.*:
                </span>
                <select id="selTipoVeiculo" class="form-control">
                    <option value="0">Tração</option>
                    <option value="1">Reboque</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Rodado*:
                </span>
                <select id="selTipoRodado" class="form-control">
                    <option value="00">Não Aplicado</option>
                    <option value="01">Truck </option>
                    <option value="02">Toco </option>
                    <option value="03">Cavalo </option>
                    <option value="04">Van </option>
                    <option value="05">Utilitário </option>
                    <option value="06">Outros </option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Carroc.*:
                </span>
                <select id="selCarroceria" class="form-control">
                    <option value="00">Não Aplicado</option>
                    <option value="01">Aberta </option>
                    <option value="02">Fechada/Baú </option>
                    <option value="03">Granel </option>
                    <option value="04">Porta Container</option>
                    <option value="05">Sider</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-7 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">UF*:
                </span>
                <select id="selUF" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="CPF do Motorista">CPF Mot.</abbr>:
                </span>
                <input type="text" id="txtCPFMotorista" class="form-control maskedInput" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Nome do Motorista">Nome Mot.</abbr>:
                </span>
                <input type="text" id="txtNomeMotorista" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
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
    <div class="panel-group" id="informacoesVeiculo">
        <div class="panel panel-default" id="divDadosProprietarioVeiculo" style="display: none;">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#informacoesVeiculo" href="#dadosProprietario">Informações do Proprietário
                    </a>
                </h4>
            </div>
            <div id="dadosProprietario" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Proprietário:
                                </span>
                                <input type="text" id="txtProprietarioVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarProprietarioVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Tipo de Proprietário">Tipo Prop.</abbr>:
                                </span>
                                <select id="selTipoProprietarioVeiculo" class="form-control">
                                    <option title="Transportador Autônomo de Cargas Agregado" value="0">TAC Agregado</option>
                                    <option title="Transportador Autônomo de Cargas Independente" value="1">TAC Independente</option>
                                    <option value="2">Outros</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">RNTRC:
                                </span>
                                <input type="text" id="txtRNTRCProprietarioVeiculo" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">TAF:
                                </span>
                                <input type="text" id="txtTAF" class="form-control maskedInput" maxlength="12" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Nº Reg. Estadual:
                                </span>
                                <input type="text" id="txtNroRegEstadual" class="form-control maskedInput" maxlength="25" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">CIOT:
                                </span>
                                <input type="text" id="txtCIOTProprietarioVeiculo" class="form-control maskedInput" maxlength="12" />
                            </div>
                        </div>
                    </div>
                    <span><b>Tags</b>:</span>
                    <button type="button" class="btn btn-default btn-xs" id="lnkPlacas">Placa</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkRenavans">RENAVAM</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkUFVeiculo">UF Veículo</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkMarcaVeiculo">Marca Veículo</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkNomeProprietario">Nome do Proprietário</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkCPFCNPJProprietario">CPF/CNPJ do Proprietário</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkRNTRCProprietario">RNTRC</button>
                    <button type="button" class="btn btn-default btn-xs" id="lnkPlacasVinculadas">Placas Vinculadas</button>
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Observação para a Emissão de CT-e">Obs. CT-e</abbr>:
                                </span>
                                <textarea type="text" id="txtObservacaoProprietario" class="form-control" rows="3"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#motoristasAdicionais" href="#dadosmotoristasAdicionais">Motoristas Adicionais
                    </a>
                </h4>
            </div>
            <div id="dadosmotoristasAdicionais" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="CPF do Motorista">CPF</abbr>:
                                </span>
                                <input type="text" id="txtCPFMotoristaAdicional" class="form-control maskedInput" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotoristaAdicional" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Nome do Motorista">Nome</abbr>:
                                </span>
                                <input type="text" id="txtNomeMotoristaAdicional" class="form-control" maxlength="80" />
                            </div>
                        </div>

                        <button type="button" id="btnSalvarMotoristaAdicional" class="btn btn-primary">Salvar</button>
                        <button type="button" id="btnExcluirMotoristaAdicional" class="btn btn-danger" style="display: none;">Excluir</button>
                        <button type="button" id="btnCancelarMotoristaAdicional" class="btn btn-default" style="display: none;">Cancelar</button>
                    </div>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblMotoristasAdicionais" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 20%;" colspan="1" rowspan="1">CPF</th>
                                    <th style="width: 50%;" colspan="1" rowspan="1">Nome</th>
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
            </div>
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#informacoesVeiculo" href="#dadosAdicionais">Informações Adicionais
                    </a>
                </h4>
            </div>
            <div id="dadosAdicionais" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Veíc.:
                                </span>
                                <input type="text" id="txtTipoVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTipoVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Marca Veíc.:
                                </span>
                                <input type="text" id="txtMarcaVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMarcaVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Modelo Veíc.:
                                </span>
                                <input type="text" id="txtModeloVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarModeloVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Mod. Carga:
                                </span>
                                <input type="text" id="txtModeloVeicularCarga" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarModeloVeicularCarga" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Chassi:
                                </span>
                                <input type="text" id="txtChassi" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Data Compra:
                                </span>
                                <input type="text" id="txtDataCompra" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Valor Aquisição:
                                </span>
                                <input type="text" id="txtValorAquisicao" class="form-control" value="0,00" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Cap. Tanque:
                                </span>
                                <input type="text" id="txtCapacidadeTanque" class="form-control" value="0" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Data Licença:
                                </span>
                                <input type="text" id="txtDataLicenca" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Ano Fabricação:
                                </span>
                                <input type="text" id="txtAnoFabricacao" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Ano Modelo:
                                </span>
                                <input type="text" id="txtAnoModelo" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Motor:
                                </span>
                                <input type="text" id="txtNumeroMotor" class="form-control" maxlength="60" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Média Padrão:
                                </span>
                                <input type="text" id="txtMediaPadrao" class="form-control" value="0,00" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data de Vencimento da Garantia Plena">Vcto. Gar. Pl.</abbr>:
                                </span>
                                <input type="text" id="txtDataVencimentoGarantiaPlena" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data de Vencimento da Garantia Escalonada">Vcto. Gar. Esc.</abbr>:
                                </span>
                                <input type="text" id="txtDataVencimentoGarantiaEscalonada" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Contrato:
                                </span>
                                <input type="text" id="txtContrato" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Nº Frota:
                                </span>
                                <input type="text" id="txtNumeroFrota" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Informação do campo para observação do contribuinte no CTe">XCampo</abbr>:
                                </span>
                                <input type="text" id="txtXCampo" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Informação do campo para observação do contribuinte no CTe">XTexto</abbr>:
                                </span>
                                <input type="text" id="txtXTexto" class="form-control" maxlength="160" />
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
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#informacoesVeiculo" href="#dadosPedagio">Informações Pedágio
                    </a>
                </h4>
            </div>
            <div id="dadosPedagio" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Fornecedor Vale Pedágio:
                                </span>
                                <input type="text" id="txtFornecedorValePedagio" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarFornecedorValePedagio" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Responsável Vale Pedágio:
                                </span>
                                <input type="text" id="txtResponsavelValePedagio" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarResponsavelValePedagio" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Número compra Vale Pedágio:
                                </span>
                                <input type="text" id="txtNumeroCompraValePedagio" class="form-control maskedInput" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Valor Vale Pedágio:
                                </span>
                                <input type="text" id="txtValorValePedagio" class="form-control" value="0,00" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#informacoesVeiculo" href="#dadosPedagioIntegracao">Informações Pedágio Integração
                    </a>
                </h4>
            </div>
            <div id="dadosPedagioIntegracao" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkPossuiTagValePedagio" />
                                        Veículo possui TAG Vale Pedágio
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Tag:
                                </span>
                                <select id="selTipoTag" class="form-control">
                                    <option value="2">Viafacil</option>
                                    <option value="5">Veloe</option>
                                    <option value="7">ConectCar</option>
                                    <option value="6">MoveMais</option>
                                    <option value="12">Taggy</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <!--
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicio Vigência Vale Pedágio:
                                </span>
                                <input type="text" id="txtDataInicioVigenciaTagValePedagio" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Data Fim Vigência Vale Pedágio:
                                </span>
                                <input type="text" id="txtDataFimVigenciaTagValePedagio" class="form-control" />
                            </div>
                        </div>
                    </div>
                    -->
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#informacoesVeiculo" href="#dadosRastreador">Rastreador</a>
                </h4>
            </div>
            <div id="dadosRastreador" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkPossuiRastreador" />
                                        Veículo possui rastreador?
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" id="infoRastreador" style="display: none">
                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tecnologia:</span>
                                <input type="text" id="txtTecnologiaRastreador" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTecnologiaRastreador" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Comunicação:</span>
                                <input type="text" id="txtTipoComunicacaoRastreador" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTipoComunicacaoRastreador" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Número do Equipamento:</span>
                                <input type="text" id="txtNumeroEquipamentoRastreador" class="form-control" maxlength="20" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <h3 style="margin-top: 15px; margin-bottom: 10px;">Veículos Vinculados
    </h3>
    <table id="tblVeiculosVinculados" class="table table-bordered table-condensed table-hover" style="max-width: 400px;">
        <thead>
            <tr>
                <th>Placa</th>
                <th>Renavam</th>
                <th>Tipo</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td colspan="3" class="text-center">Nenhum registro encontrado.</td>
            </tr>
        </tbody>
    </table>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
