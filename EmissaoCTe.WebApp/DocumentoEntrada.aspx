<%@ Page Title="Cadastro de Documentos de Entrada" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="DocumentoEntrada.aspx.cs" Inherits="EmissaoCTe.WebApp.DocumentoEntrada" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload") %>
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
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/documentoEntrada") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="hddAbastecimentos" value="" />
    <div class="page-header">
        <h2>Cadastro de Documentos de Entrada
        </h2>
    </div>
    <div class="hidden" id="xmlUpload">
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <button type="button" id="btnImportarNotaFiscal" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-barcode"></span>&nbsp;Importar NF-e
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Lançamento*:
                </span>
                <input type="text" id="txtNumeroLancamento" class="form-control" value="Automático" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrada*:
                </span>
                <input type="text" id="txtDataEntrada" class="form-control" />
            </div>
        </div>
    </div>
    <hr style="margin: 5px 0 10px 0;" />
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Emissão*:
                </span>
                <input type="text" id="txtDataEmissao" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número*:
                </span>
                <input type="text" id="txtNumero" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Série:
                </span>
                <input type="text" id="txtSerie" class="form-control" maxlength="10" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Espécie*:
                </span>
                <select id="selEspecieDocumento" class="form-control"></select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Modelo*:
                </span>
                <select id="selModeloDocumento" class="form-control"></select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6 hidden nfe">
            <div class="input-group">
                <span class="input-group-addon">Chave:
                </span>
                <input type="text" id="txtChave" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Fornecedor*:
                </span>
                <input type="text" id="txtFornecedor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFornecedor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Produtos*:
                </span>
                <input type="text" id="txtValorProdutos" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Total*:
                </span>
                <input type="text" id="txtValorTotal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total Desc.*:
                </span>
                <input type="text" id="txtValorTotalDesconto" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor total de outras despesas">Total Out. D.</abbr>*:
                </span>
                <input type="text" id="txtValorTotalOutrasDespesas" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total Frete*:
                </span>
                <input type="text" id="txtValorTotalFrete" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">B. C. ICMS*:
                </span>
                <input type="text" id="txtBaseCalculoICMS" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total ICMS*:
                </span>
                <input type="text" id="txtValorTotalICMS" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">B. C. ICMS ST*:
                </span>
                <input type="text" id="txtBaseCalculoICMSST" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total ICMS ST*:
                </span>
                <input type="text" id="txtValorTotalICMSST" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total IPI*:
                </span>
                <input type="text" id="txtValorTotalIPI" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total PIS*:
                </span>
                <input type="text" id="txtValorTotalPIS" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Total COFINS*:
                </span>
                <input type="text" id="txtValorTotalCOFINS" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="0">Aberto</option>
                    <option value="1">Finalizado</option>
                    <option value="2">Cancelado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Pagamento*:
                </span>
                <select id="selIndicadorPagamento" class="form-control">
                    <option value="0">À vista</option>
                    <option value="1">A prazo</option>
                    <option value="9">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Conta*:
                </span>
                <input type="text" id="txtPlanoConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoConta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <ul class="nav nav-tabs">
        <li class="active"><a href="#itens" data-toggle="tab">Itens</a></li>
        <li><a href="#cobrancas" data-toggle="tab">Cobranças</a></li>
        <li><a href="#abastecimentos" data-toggle="tab">Abastecimentos</a></li>
    </ul>
    <div class="tab-content" style="margin-top: 10px;">
        <div class="tab-pane active" id="itens">
            <div id="placeholder-mensagem-itens">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-2">
                    <div class="input-group">
                        <span class="input-group-addon">Item:
                        </span>
                        <input type="text" id="txtItem" class="form-control" value="Automático" disabled />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-8 col-md-6 col-lg-7">
                    <div class="input-group">
                        <span class="input-group-addon">Produto*:
                        </span>
                        <input type="text" id="txtProduto" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarProduto" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Quant.*:
                        </span>
                        <input type="text" id="txtQuantidade" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Unidade de Medida">Un.</abbr>*:
                        </span>
                        <select id="selUnidadeMedida" class="form-control"></select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor Unitário">Val. Un.</abbr>*:
                        </span>
                        <input type="text" id="txtValorUnitario" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Desconto">Desc.</abbr>:
                        </span>
                        <input type="text" id="txtDesconto" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor Total">Total</abbr>*:
                        </span>
                        <input type="text" id="txtValorTotalItem" class="form-control" />
                    </div>
                </div>

                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">CFOP*:
                        </span>
                        <select id="selCFOP" class="form-control"></select>
                    </div>
                </div>
            </div>
            <hr style="margin: 5px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-9">
                    <div class="input-group">
                        <span class="input-group-addon">CST ICMS*:
                        </span>
                        <select id="selCSTICMS" class="form-control">
                            <option value="">Nenhum</option>
                            <option value="000">000 - Tributada integralmente</option>
                            <option value="010">010 - Tributada e com cobrança do ICMS por substituição tributária</option>
                            <option value="020">020 - Com redução de base de cálculo</option>
                            <option value="030">030 - Isenta ou não tributada e com cobrança do ICMS por substituição tributária</option>
                            <option value="040">040 - Isenta</option>
                            <option value="041">041 - Não tributada</option>
                            <option value="050">050 - Suspensão</option>
                            <option value="051">051 - Diferimento</option>
                            <option value="060">060 - ICMS cobrado anteriormente por substituição tributária</option>
                            <option value="070">070 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária</option>
                            <option value="090">090 - Outras</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Base de Cálculo do ICMS">B.C. ICMS</abbr>*:
                        </span>
                        <input type="text" id="txtBaseCalculoICMSItem" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Alíquota do ICMS">Al. ICMS</abbr>*:
                        </span>
                        <input type="text" id="txtAliquotaICMS" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor do ICMS">Vl. ICMS</abbr>*:
                        </span>
                        <input type="text" id="txtValorICMS" class="form-control" />
                    </div>
                </div>
            </div>
            <hr style="margin: 5px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-9">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="CST do PIS">CST PIS</abbr>*:
                        </span>
                        <select id="selCSTPIS" class="form-control">
                            <option value="">Nenhum</option>
                            <option value="01">01 - Operação Tributável com Alíquota Básica</option>
                            <option value="02">02 - Operação Tributável com Alíquota Diferenciada</option>
                            <option value="03">03 - Operação Tributável com Alíquota por Unidade de Medida de Produto</option>
                            <option value="04">04 - Operação Tributável Monofásica - Revenda a Alíquota Zero</option>
                            <option value="05">05 - Operação Tributável por Substituição Tributária</option>
                            <option value="06">06 - Operação Tributável a Alíquota Zero</option>
                            <option value="07">07 - Operação Isenta da Contribuição</option>
                            <option value="08">08 - Operação sem Incidência da Contribuição</option>
                            <option value="09">09 - Operação com Suspensão da Contribuição</option>
                            <option value="49">49 - Outras Operações de Saída</option>
                            <option value="50">50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno</option>
                            <option value="51">51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno</option>
                            <option value="52">52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação</option>
                            <option value="53">53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno</option>
                            <option value="54">54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação</option>
                            <option value="55">55 - Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação</option>
                            <option value="56">56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação</option>
                            <option value="60">60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno</option>
                            <option value="61">61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno</option>
                            <option value="62">62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação</option>
                            <option value="63">63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno</option>
                            <option value="64">64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação</option>
                            <option value="65">65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação</option>
                            <option value="66">66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação</option>
                            <option value="67">67 - Crédito Presumido - Outras Operações</option>
                            <option value="70">70 - Operação de Aquisição sem Direito a Crédito</option>
                            <option value="71">71 - Operação de Aquisição com Isenção</option>
                            <option value="72">72 - Operação de Aquisição com Suspensão</option>
                            <option value="73">73 - Operação de Aquisição a Alíquota Zero</option>
                            <option value="74">74 - Operação de Aquisição sem Incidência da Contribuição</option>
                            <option value="75">75 - Operação de Aquisição por Substituição Tributária</option>
                            <option value="98">98 - Outras Operações de Entrada</option>
                            <option value="99">99 - Outras Operações</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor do PIS">Vl. PIS</abbr>*:
                        </span>
                        <input type="text" id="txtValorPIS" class="form-control" />
                    </div>
                </div>
            </div>
            <hr style="margin: 5px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-9">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="CST da COFINS">CST COFINS</abbr>*:
                        </span>
                        <select id="selCSTCOFINS" class="form-control">
                            <option value="">Nenhum</option>
                            <option value="01">01 - Operação Tributável com Alíquota Básica</option>
                            <option value="02">02 - Operação Tributável com Alíquota Diferenciada</option>
                            <option value="03">03 - Operação Tributável com Alíquota por Unidade de Medida de Produto</option>
                            <option value="04">04 - Operação Tributável Monofásica - Revenda a Alíquota Zero</option>
                            <option value="05">05 - Operação Tributável por Substituição Tributária</option>
                            <option value="06">06 - Operação Tributável a Alíquota Zero</option>
                            <option value="07">07 - Operação Isenta da Contribuição</option>
                            <option value="08">08 - Operação sem Incidência da Contribuição</option>
                            <option value="09">09 - Operação com Suspensão da Contribuição</option>
                            <option value="49">49 - Outras Operações de Saída</option>
                            <option value="50">50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno</option>
                            <option value="51">51 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno</option>
                            <option value="52">52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação</option>
                            <option value="53">53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno</option>
                            <option value="54">54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação</option>
                            <option value="55">55 - Operação com Direito a Crédito - Vinculada a Receitas Não Tributadas no Mercado Interno e de Exportação</option>
                            <option value="56">56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação</option>
                            <option value="60">60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno</option>
                            <option value="61">61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno</option>
                            <option value="62">62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação</option>
                            <option value="63">63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno</option>
                            <option value="64">64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação</option>
                            <option value="65">65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação</option>
                            <option value="66">66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação</option>
                            <option value="67">67 - Crédito Presumido - Outras Operações</option>
                            <option value="70">70 - Operação de Aquisição sem Direito a Crédito</option>
                            <option value="71">71 - Operação de Aquisição com Isenção</option>
                            <option value="72">72 - Operação de Aquisição com Suspensão</option>
                            <option value="73">73 - Operação de Aquisição a Alíquota Zero</option>
                            <option value="74">74 - Operação de Aquisição sem Incidência da Contribuição</option>
                            <option value="75">75 - Operação de Aquisição por Substituição Tributária</option>
                            <option value="98">98 - Outras Operações de Entrada</option>
                            <option value="99">99 - Outras Operações</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor da COFINS">Vl. COFINS</abbr>*:
                        </span>
                        <input type="text" id="txtValorCOFINS" class="form-control" />
                    </div>
                </div>
            </div>
            <hr style="margin: 5px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-9">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="CST do IPI">CST IPI</abbr>*:
                        </span>
                        <select id="selCSTIPI" class="form-control">
                            <option value="">Nenhum</option>
                            <option value="00">00 - Entrada com Recuperação de Crédito</option>
                            <option value="01">01 - Entrada Tributável com Alíquota Zero</option>
                            <option value="02">02 - Entrada Isenta</option>
                            <option value="03">03 - Entrada Não-Tributada</option>
                            <option value="04">04 - Entrada Imune</option>
                            <option value="05">05 - Entrada com Suspensão</option>
                            <option value="49">49 - Outras Entradas</option>
                            <option value="50">50 - Saída Tributada</option>
                            <option value="51">51 - Saída Tributável com Alíquota Zero</option>
                            <option value="52">52 - Saida Isenta</option>
                            <option value="53">53 - Saída Não-Tributada</option>
                            <option value="54">54 - Saída Imune</option>
                            <option value="55">55 - Saída com Suspensão</option>
                            <option value="99">99 - Outras Saídas</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Base de Cálculo do IPI">B.C. IPI</abbr>*:
                        </span>
                        <input type="text" id="txtBaseCalculoIPIItem" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Alíquota do IPI">Al. IPI</abbr>*:
                        </span>
                        <input type="text" id="txtAliquotaIPI" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor do IPI">Vl. IPI</abbr>*:
                        </span>
                        <input type="text" id="txtValorIPI" class="form-control" />
                    </div>
                </div>
            </div>
            <hr style="margin: 5px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Base de Cálculo do ICMS de Substituição Tributária">B.C. ICMS ST</abbr>*:
                        </span>
                        <input type="text" id="txtBaseCalculoICMSSTItem" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor do ICMS de Substituição Tributária">Vl. ICMS ST</abbr>*:
                        </span>
                        <input type="text" id="txtValorICMSST" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor de Outras Despesas">Out. Desp.</abbr>*:
                        </span>
                        <input type="text" id="txtValorOutrasDespesas" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor de Frete">Vl. Frete</abbr>*:
                        </span>
                        <input type="text" id="txtValorFrete" class="form-control" />
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarItem" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirItem" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarItem" class="btn btn-default">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px; max-height: 500px; overflow-y: scroll;">
                <table id="tblItens" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 10%;">Item
                            </th>
                            <th style="width: 30%;">Produto
                            </th>
                            <th style="width: 10%;">Quantidade
                            </th>
                            <th style="width: 10%;">Vl. Unitário
                            </th>
                            <th style="width: 10%;">Vl. Total
                            </th>
                            <th style="width: 10%;">CFOP
                            </th>
                            <th style="width: 10%;">CST
                            </th>
                            <th style="width: 10%;">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="8">Nenhum registro encontrado!
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane" id="cobrancas">
            <div id="placeholder-mensagem-cobrancas">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Parcelas*:
                        </span>
                        <input type="text" id="txtQuantidadeParcelasDuplicata" class="form-control" maxlength="2" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Frequência de dias para vencimento">Qtd. Dias</abbr>*:
                        </span>
                        <input type="text" id="txtDiasDuplicata" class="form-control" maxlength="2" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Data do primeiro vencimento">Dt. Vcto.</abbr>*:
                        </span>
                        <input type="text" id="txtDataPrimeiroVencimentoDuplicata" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Arredondamento de valores">Arredondar</abbr>*:
                        </span>
                        <select id="selArredondamentoDuplicata" class="form-control">
                            <option value="0">Primeira</option>
                            <option value="1">Última</option>
                        </select>
                    </div>
                </div>
            </div>
            <button type="button" id="btnGerarDuplicatasAutomaticamente" class="btn btn-primary">Gerar Parcelas</button>
            <hr style="margin: 10px 0 10px 0;" />
            <div class="row">
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Número*:
                        </span>
                        <input type="text" id="txtNumeroDuplicata" class="form-control" maxlength="60" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Valor*:
                        </span>
                        <input type="text" id="txtValorDuplicata" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Data de Vencimento">Data Vcto.</abbr>*:
                        </span>
                        <input type="text" id="txtDataVencimento" class="form-control" />
                    </div>
                </div>
            </div>
            <button type="button" id="btnSalvarDuplicata" class="btn btn-primary">Salvar</button>
            <button type="button" id="btnExcluirDuplicata" class="btn btn-danger" style="display: none;">Excluir</button>
            <button type="button" id="btnCancelarDuplicata" class="btn btn-default">Cancelar</button>
            <div class="table-responsive" style="margin-top: 10px; max-height: 500px; overflow-y: scroll;">
                <table id="tblCobrancas" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 28%;">Número
                            </th>
                            <th style="width: 28%;">Valor
                            </th>
                            <th style="width: 28%;">Data Vencimento
                            </th>
                            <th style="width: 10%;">Opções
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="4">Nenhum registro encontrado!
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tab-pane fade" id="abastecimentos">
            <div id="mensagensAbastecimentos-placeholder">
            </div>
            <div class="row">
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
            <button type="button" id="btnCopiarDadosItens" class="btn btn-default" style="float: right">Copiar do primeiro item</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblAbastecimentos" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">KM Inicial</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">KM Final</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Litros</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor Un.</th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Pago</th>
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
    <hr style="margin: 5px 0 10px 0;" />
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar Documento</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar e Limpar Campos</button>
</asp:Content>
