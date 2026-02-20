<%@ Page Title="Configurações da Empresa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ConfiguracoesEmpresasEmissoras.aspx.cs" Inherits="EmissaoCTe.WebApp.ConfiguracoesEmpresasEmissoras" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <script defer="defer" type="text/javascript">
            CKEDITOR_BASEPATH = ObterPath() + '/Scripts/ckeditor/';
        </script>
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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/ckeditor",
                           "~/bundle/scripts/ckeditoradapters",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/configuracoesempresasemissoras") %>
        <asp:PlaceHolder runat="server">
            <%: Styles.Render("~/bundle/styles/plupload") %>
            <style type="text/css">
                .ck-container {
                    border-right: 1px solid #ccc;
                    border-top-right-radius: 4px;
                    border-bottom-right-radius: 4px;
                }

                #txtLogDicasCTe p {
                    margin-top: 0px;
                    margin-bottom: 0px;
                }

                .lista-arquivos {
                    border: 1px solid #ccc;
                    border-left: none;
                    border-top-right-radius: 4px;
                    border-bottom-right-radius: 4px;
                    padding: 20px;
                }

                    .lista-arquivos .arquivo {
                        border-top-right-radius: 4px;
                        border-bottom-right-radius: 4px;
                        border-top-left-radius: 4px;
                        border-bottom-left-radius: 4px;
                        background-color: #eee;
                        padding: 10px;
                        height: 160px;
                        margin-bottom: 20px;
                    }

                    .lista-arquivos .arquivo-inserir a:hover,
                    .lista-arquivos .arquivo-inserir a:active,
                    .lista-arquivos .arquivo-inserir a:focus {
                        text-decoration: none;
                    }

                    .lista-arquivos .arquivo .arquivo-icone {
                        text-align: center;
                        padding-bottom: 20px;
                    }

                        .lista-arquivos .arquivo .arquivo-icone div {
                            width: 100%;
                            height: 50px;
                            background-position: center;
                            background-size: contain;
                            background-repeat: no-repeat;
                        }

                    .lista-arquivos .arquivo .arquivo-nome {
                        text-align: center;
                        font-weight: bold;
                        line-height: 15px;
                        min-height: 45px;
                        word-break: break-all;
                    }

                        .lista-arquivos .arquivo .arquivo-nome a {
                            color: #000;
                        }

                            .lista-arquivos .arquivo .arquivo-nome a:hover,
                            .lista-arquivos .arquivo .arquivo-nome a:active,
                            .lista-arquivos .arquivo .arquivo-nome a:focus {
                                text-decoration: none;
                            }

                    .lista-arquivos .arquivo-inserir {
                        padding-top: 25px;
                    }

                        .lista-arquivos .arquivo-inserir .arquivo-nome {
                            word-break: normal;
                        }

                .log-dica {
                    border-top-right-radius: 4px;
                    border-bottom-right-radius: 4px;
                    border-top-left-radius: 4px;
                    border-bottom-left-radius: 4px;
                    border: 1px solid #ccc;
                    padding: 20px;
                    background-color: #eee;
                }
            </style>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoAtividade" value="0" />
        <input type="hidden" id="hddCodigoPlanoAbastecimento" value="0" />
        <input type="hidden" id="hddCodigoPlanoCTe" value="0" />
        <input type="hidden" id="hddCodigoEmpresa" value="0" />
    </div>
    <div class="page-header">
        <h2>Configurações da Empresa
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <ul class="nav nav-tabs" id="tabsConfiguracoes">
        <li class="active"><a href="#tabGeral" data-toggle="tab">CT-e / MDF-e</a></li>
        <li><a href="#tabNFSe" data-toggle="tab">NFS-e</a></li>
        <li><a href="#tabFinanceiro" data-toggle="tab">Financeiro</a></li>
        <li><a href="#tabAverbacao" data-toggle="tab">Averbação</a></li>
        <li><a href="#tabCIOT" data-toggle="tab">CIOT</a></li>
        <li><a href="#tabDicas" data-toggle="tab">Dicas</a></li>
        <li><a href="#tabSeries" data-toggle="tab">Series</a></li>
        <li><a href="#tabIntegracao" data-toggle="tab">Integração</a></li>
        <li><a href="#tabNatura" data-toggle="tab">Natura</a></li>
        <li><a href="#tabConfiguracaoXml" data-toggle="tab">Aut. XML</a></li>
        <li><a href="#tabAssEmail" data-toggle="tab">Ass. e-mail</a></li>
        <li><a href="#tabFTP" data-toggle="tab">FTP</a></li>
        <li><a href="#tabEstadosBloqueados" data-toggle="tab">Estados</a></li>
    </ul>
    <div class="tab-content" style="margin-top: 10px; margin-bottom: 10px;">
        <div class="tab-pane active" id="tabGeral">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Dias Para Entrega Padrão">Dias Entrega</abbr>:
                        </span>
                        <input type="text" id="txtDiasParaEntrega" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Limite de Dias Para Emissão de CT-e de Anulação">Dias Anulação</abbr>:
                        </span>
                        <input type="text" id="txtDiasParaEmissaoCTeAnulacao" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Limite de Dias Para Emissão de CT-e Complementar">Dias Compl.</abbr>:
                        </span>
                        <input type="text" id="txtDiasParaEmissaoCTeComplementar" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Limite de Dias Para Emissão de CT-e de Substituição">Dias Subs.</abbr>:
                        </span>
                        <input type="text" id="txtDiasParaEmissaoCTeSubstituicao" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Limite de Horas Cancelamento do CT-e">Horas Canc. CT-e</abbr>:
                        </span>
                        <input type="text" id="txtPrazoCancelamentoCTe" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Limite de Horas Cancelamento do MDF-e">Horas Canc. MDF-e</abbr>:
                        </span>
                        <input type="text" id="txtPrazoCancelamentoMDFe" class="form-control maskedInput" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Produto Predominante Padrão Para a Emissão de CT-e">Produto Pred.</abbr>:
                        </span>
                        <input type="text" id="txtProdutoPredominante" class="form-control" maxlength="60" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Outras Características Para a Emissão de CT-e">Outras Caract.</abbr>:
                        </span>
                        <input type="text" id="txtOutrasCaracteristicas" class="form-control" maxlength="30" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-4 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Descrição padrão para a unidade de medida KG nos CTes">Descrição KG CTe</abbr>:
                        </span>
                        <input type="text" id="txtDescricaoMedidaKgCTe" class="form-control" maxlength="30" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Apólice de Seguro Padrão para a Emissão de CT-e">Seguro</abbr>:
                        </span>
                        <input type="text" id="txtApoliceSeguro" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarApoliceSeguro" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Atividade Padrão Para Novos Clientes">Atividade</abbr>:
                        </span>
                        <input type="text" id="txtAtividadePadrao" class="form-control" onblur="ValidarAtividade();" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarAtividade" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Tipo de Impressão Padrão da DACTE na Emissão de CT-e">Tipo Imp.</abbr>:
                        </span>
                        <select id="selTipoImpressao" class="form-control">
                            <option value="1">Retrato</option>
                            <option value="2">Paisagem</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Modelo padrão para emissão de CT-e">Modelo CTe</abbr>:
                        </span>
                        <select id="selModeloPadrao" class="form-control">
                            <option value="57">57 - CT-e</option>
                            <option value="67">67 - CT-e OS</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Versão para emissão do CTe">Versão CT-e</abbr>:
                        </span>
                        <select id="selVersaoCTe" class="form-control">
                            <option value="">Padrão Ambiente</option>
                            <option value="2.00">2.00 (Prazo final 04/12/2017)</option>
                            <option value="3.00">3.00 (Prazo final 31/01/2024)</option>
                            <option value="4.00">4.00</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Valor limite de frete para emissão de CTe. Zero utiliza valor padrão do ambiente.">Valor Limite Frete</abbr>:
                        </span>
                        <input type="text" id="txtValorLimiteFrete" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Tipo de peso utilizado na importação de NFe">Peso NFe</abbr>:
                        </span>
                        <select id="selTipoPesoNFe" class="form-control">
                            <option value="0">Bruto</option>
                            <option value="1">Liquido</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Tipo de geração de CT-e utilizado na integração de CT-e">Integração</abbr>:
                        </span>
                        <select id="selTipoGeracaoCTeWS" class="form-control">
                            <option value="0">Gerar CT-e</option>
                            <option value="1">Gerar MDF-e</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Incluir ICMS no Total do Frete (apenas para integração)">Incluir ICMS</abbr>:
                        </span>
                        <select id="selIncluirICMS" class="form-control">
                            <option value="0">Não</option>
                            <option value="1">Sim</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Percentual de imposto para empresas do Simples Nacional">% Imp. S. N.</abbr>:
                        </span>
                        <input type="text" id="txtPercentualImpostoSimplesNacional" class="form-control" maxlength="5" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para CT-e Normal">Obs. CT-e</abbr>:
                        </span>
                        <textarea id="txtObservacaoPadraoNormal" class="form-control" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação para CT-e do Simples Nacional (Apenas integração)">Obs. Simples Nacional</abbr>:
                        </span>
                        <textarea id="txtObservacaoSimplesNacional" class="form-control" rows="1"></textarea>
                    </div>
                </div>
            </div>
            <b>Tags para observação avançada: </b>
            <button type="button" id="lnkCPFCNPJProprietarioVeiculo" class="btn btn-default btn-xs">CPF/CNPJ do Prop. do Veículo</button>
            <button type="button" id="lnkNomeProprietarioVeiculo" class="btn btn-default btn-xs">Nome do Prop. do Veículo</button>
            <button type="button" id="lnkRNTRCProprietario" class="btn btn-default btn-xs">RNTRC do Prop.</button>
            <button type="button" id="lnkPlaca" class="btn btn-default btn-xs">Placa Veículo</button>
            <button type="button" id="lnkRENAVAMVeiculo" class="btn btn-default btn-xs">Renavam Veic.</button>
            <button type="button" id="lnkUFVeiculo" class="btn btn-default btn-xs">UF Veic.</button>
            <button type="button" id="lnkMarcaVeiculo" class="btn btn-default btn-xs">Marca Veic.</button>
            <button type="button" id="lnkPlacasVinculadas" class="btn btn-default btn-xs">Placas Vinculadas</button>
            <button type="button" id="lnkPlacasRenavamVinculadas" class="btn btn-default btn-xs">Placas/Renavam Vinculadas</button>
            <button type="button" id="lnkNomeMotorista" class="btn btn-default btn-xs">Nome Motorista</button>
            <button type="button" id="lnkCPFMotorista" class="btn btn-default btn-xs">CPF Motorista</button>
            <button type="button" id="lnkQuantidadeNotas" class="btn btn-default btn-xs">Quantidades de Notas</button>
            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação avançada para todos veículos próprios">Obs. CT-e/MDF-e (Próp.)</abbr>:
                        </span>
                        <textarea id="txtObservacaoCTeAvancadaProprio" class="form-control taggedInput2" rows="3"></textarea>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação avançada para todos veículos terceiros">Obs. CT-e/MDF-e (Terc.)</abbr>:
                        </span>
                        <textarea id="txtObservacaoCTeAvancadaTerceiros" class="form-control taggedInput2" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <b>Tags para observação: </b>
            <button type="button" id="lnkNumeroCTe" class="btn btn-default btn-xs">Número do CT-e</button>
            <button type="button" id="lnkChaveCTe" class="btn btn-default btn-xs">Chave do CT-e</button>
            <button type="button" id="lnkDataEmissaoCTe" class="btn btn-default btn-xs">Data de Emissão do CT-e</button>
            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para CT-e Complementar">Obs. Comp.</abbr>:
                        </span>
                        <textarea id="txtObservacaoPadraoComplementar" class="form-control taggedInput" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para CT-e de Anulação">Obs. Anul.</abbr>:
                        </span>
                        <textarea id="txtObservacaoPadraoAnulacao" class="form-control taggedInput" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para CT-e de Substituição">Obs. Subs.</abbr>:
                        </span>
                        <textarea id="txtObservacaoPadraoSubstituicao" class="form-control taggedInput" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <b>Tags para nome PDF CTe: </b>
            <button type="button" id="lnkNomePDFCNPJTransportador" class="btn btn-default btn-xs">CNPJ Transportador</button>
            <button type="button" id="lnkNomePDFNomeTransportador" class="btn btn-default btn-xs">Nome Transportador</button>
            <button type="button" id="lnkNomePDFCTeNumeroCTe" class="btn btn-default btn-xs">Número do CT-e</button>
            <button type="button" id="lnkNomePDFCTeSerieCTe" class="btn btn-default btn-xs">Série do CT-e</button>
            <button type="button" id="lnkNomePDFChaveCTe" class="btn btn-default btn-xs">Chave do CT-e</button>
            <button type="button" id="lnkNomePDFPlaca" class="btn btn-default btn-xs">Placa Veículo</button>
            <button type="button" id="lnkNomePDFMotorista" class="btn btn-default btn-xs">Nome Motorista</button>
            <button type="button" id="lnkNomePDFClienteRemetente" class="btn btn-default btn-xs">Cliente Remetente</button>
            <button type="button" id="lnkNomePDFCidadeOrigem" class="btn btn-default btn-xs">Cidade Origem</button>
            <button type="button" id="lnkNomePDFUFOrigem" class="btn btn-default btn-xs">UF Origem</button>
            <button type="button" id="lnkNomePDFClienteDestinatario" class="btn btn-default btn-xs">Cliente Destinatário</button>
            <button type="button" id="lnkNomePDFCidadeDestino" class="btn btn-default btn-xs">Cidade Destino</button>
            <button type="button" id="lnkNomePDFUFDestino" class="btn btn-default btn-xs">UF Destino</button>
            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-6 col-sm-3 col-md-2 col-lg-2">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="">Tamanho</abbr>:
                        </span>
                        <input type="text" id="txtTamanhoTag" class="form-control maskedInput" value="5" />
                    </div>
                </div>
                <button type="button" id="lnkNomeTamanhoTag" class="btn btn-default btn-xs">Tamanho Tag</button>
            </div>
            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para CT-e Complementar">Nome PDF CTe</abbr>:
                        </span>
                        <textarea id="txtNomePDFCTe" class="form-control taggedInput3" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Disponível apenas quando não existe nenhum MDFe salvo">Numeração inicial MDFe</abbr>:
                        </span>
                        <input type="text" id="txtPrimeiroNumeroMDFe" class="form-control maskedInput" />
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-12 col-sm-6 col-md-6">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Texto que identifica na observação da NF-e a informação a ser copiada, exemplo PEDIDO:">Identificador Obs NFe.</abbr>:
                        </span>
                        <input type="text" id="txtTagParaImportarObservacaoNFe" class="form-control" maxlength="60" />
                    </div>
                </div>
                <div class="col-xs-6 col-sm-3 col-md-2 col-lg-2">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Tamanho do texto para copiar da observaçãoda NF-e">Tamanho</abbr>:
                        </span>
                        <input type="text" id="txtTamanhoTagObservacaoNFe" class="form-control maskedInput" value="0" />
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top: 10px;">
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkUtilizaNovaImportacaoEDI" />
                                Utiliza nova importação EDI
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoImportarNotaDuplicadaEDINovaImportacao" />
                                Não importar NFe duplicada (Nova importação EDI)
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkGerarNFSeImportacoes" />
                                Imp. XML/NOTFIS geram NFSe quando municipal
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkArmazenaNotasParaGerarPorPeriodo" />
                                Permite amazenar nota para gerar NFSe por período
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkEmitirSemValorDaCarga" />
                                Permite emitir sem valor da carga
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkIndicadorLotacao" />
                                Padrão Indicador de Lotação
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkUtilizaTabelaFrete" />
                                Utiliza Tabela de Frete
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkAtualizaVeiculoImpXMLCTe" />
                                Atualiza Veículo Imp. XML CT-e
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkPermiteVincularMesmaPlacaOutrosVeiculos" />
                                Permite vincular placa em varios veículos
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoCopiarValoresCTeAnterior" />
                                Não Copiar Valores do CT-e Anterior
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoCopiarImpostosCTeAnterior" />
                                Não Copiar Impostos do CT-e Anterior
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoCopiarSeguroCTeAnterior" />
                                Não Copiar Seguro do CT-e Anterior
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkCopiarObservacaoFiscoContribuinteCTeAnterior" />
                                Copiar Obs. Fisco/Contribuinte do CT-e Anterior
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkICMSIsento" />
                                ICMS Isento
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkBloquearEmissaoMDFeWS" />
                                Bloquear emissão MDF-e WebService
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkFormatarPlacaComHifenNaObservacao" />
                                Formatar placas com hífen nas observações
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkImportacaoNaoRateiaPedagio" />
                                Importação não rateia o valor do pedágio
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkGerarCTeIntegracaoDocumentosMunicipais" />
                                Gerar CTe na integração de documentos municipais
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoCalcularDIFALCTeOS" />
                                Não calcular DIFAL
                            </label>
                        </div>
                    </div>
                </div>

                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeVencimentoCertificado" />
                                Exibir aviso de vencimento do certificado na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomePendenciasEntrega" />
                                Exibir aviso de pendências de entrega na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeGraficosEmissoes" />
                                Exibir graficos de emissões na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeServicosVeiculos" />
                                Exibir Serviços dos Veículos na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeParcelaDuplicatas" />
                                Exibir Parcelas Duplicatas na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomePagamentosMotoristas" />
                                Exibir Pagamento de Motoristas na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeAcertoViagem" />
                                Exibir Acertos de Viagem na página inicial.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExibirHomeMDFesPendenteEncerramento" />
                                Exibir aviso de MDFes pendente de ecerramento.
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkBloquearEmissaoCTeComUFDestinosDiferentes" />
                                Bloquear emissão CTe com UF destinos diferentes
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkBloquearEmissaoCTeCargaMunicipal" />
                                Bloquear emissão CTe carga municipal
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkBloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca" />
                                Bloquear emissão MDFe com MDFe Autorizado com mesma Placa
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkAdicionarResponsavelSeguroObsContribuinte" />
                                Adicionar Responsável do Seguro na Observação do Contribuinte (ATM)
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkPermiteImportarXMLNFSe" />
                                Permite Importar XML NFS-e Autorizada
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoImportarValoresImportacaoCTe" />
                                Não importar valores na importação de CT-es
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkExigirObservacaoContribuinteValorContainer" />
                                Exigir observação contribuinte ValorContainer
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkUsarRegraICMSParaCteDeSubcontratacao" />
                                Usar regra de ICMS para CTe de Subcontratação
                            </label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNaoSmarCreditoICMSNoValorDaPrestacao" />
                                Não somar crédito ICMS no valor da prestação
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabNFSe">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Série*:
                        </span>
                        <input type="text" id="txtSerieNFSe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarSerieNFSe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Série RPS:
                        </span>
                        <input type="text" id="txtSerieRPSNFSe" class="form-control" maxlength="10" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">CPF Integração:
                        </span>
                        <input type="text" id="txtCPFNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Senha:
                        </span>
                        <input type="text" id="txtSenhaNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Frase:
                        </span>
                        <input type="text" id="txtFraseSecretaNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Serviço prestado no município do transportador">Serviço no município</abbr>:
                        </span>
                        <input type="text" id="txtServicoNFSe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarServicoNFSe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Serviço prestado fora do município do transportador">Serviço fora do município</abbr>:
                        </span>
                        <input type="text" id="txtServicoNFSeFora" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarServicoNFSeFora" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Natureza no município:
                        </span>
                        <input type="text" id="txtNaturezaNFSe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarNaturezaNFSe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Natureza fora do município:
                        </span>
                        <input type="text" id="txtNaturezaForaNFSe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarNaturezaForaNFSe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Site Prefeitura:
                        </span>
                        <input type="text" id="txtURLPrefeituraNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Login Site Prefeitura:
                        </span>
                        <input type="text" id="txtLoginSitePrefeituraNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Senha Site Prefeitura:
                        </span>
                        <input type="text" id="txtSenhaSitePrefeituraNFSe" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Utiliza integração com site http://app.enotasgw.com.br para intergar NFS-e">Utiliza eNotas para integrar NFSe</abbr>:
                        </span>
                        <select id="selUtilizaENotasNFSe" class="form-control" disabled="disabled">
                            <option value="0">Não</option>
                            <option value="1">Sim</option>
                        </select>
                    </div>
                </div>

                <div class="col-xs-6 col-sm-6 col-md-3">
                    <button type="button" id="btnImportarPreNFSItupega" class="btn btn-primary">Importar Pré-NFS de Itupeva</button>
                </div>

                <div class="col-xs-6 col-sm-6 col-md-3">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkEmiteNFSeForaEmbarcador">
                                Emite NFS-e Fora do Embarcador
                            </label>
                        </div>
                    </div>
                </div>

                <div class="col-xs-6 col-sm-6 col-md-2">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkNFSeNacional">
                                NFS-e Nacional
                            </label>
                        </div>
                    </div>
                </div>

                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 idENotas">
                    <div class="input-group">
                        <span class="input-group-addon">ID eNotas:
                        </span>
                        <input type="text" id="txtIDEnotas" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação Padrão para NFSe">Obs. Padrão</abbr>:
                        </span>
                        <textarea id="txtObservacaoPadraoNFSe" class="form-control" rows="3"></textarea>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Observação referente a integração NFS-e">Obs. Integração NFS-e</abbr>:
                        </span>
                        <textarea id="txtObservacaoIntegracaoNFSe" class="form-control" rows="3"></textarea>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Serviços por Município</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Localidade de prestação do serviço">Cidade</abbr>:
                                </span>
                                <input type="text" id="txtCidadeNFe" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarCidadeNFe" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Natureza de operação">Natureza</abbr>:
                                </span>
                                <input type="text" id="txtNaturezaPorCidade" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarNaturezaPorCidade" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Serviço">Serviço</abbr>:
                                </span>
                                <input type="text" id="txtServicoPorCidade" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarServicoPorCidade" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnAdicionarServicoPorCidade" class="btn btn-primary">Adicionar</button>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblServicoPorCidade" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 20%;" colspan="1" rowspan="1">Cidade
                                    </th>
                                    <th style="width: 20%;" colspan="1" rowspan="1">Natureza
                                    </th>
                                    <th style="width: 30%;" colspan="1" rowspan="1">Serviço
                                    </th>
                                    <th style="width: 10%;">Opções
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
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Códigos Serviços NFS-e</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Código Tributação</span>
                                <input type="text" id="txtCodigoTributacaoCodigosServicos" class="form-control" maxlength="100" />
                            </div>
                        </div>
                        <div class="col-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Código Tributação Prefeitura</span>
                                <input type="text" id="txtCodigoTributacaoPrefeituraCodigosServicos" class="form-control" maxlength="100" />
                            </div>
                        </div>
                        <div class="col-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Número Tributação Prefeitura</span>
                                <input type="text" id="txtNumeroTributacaoPrefeituraCodigosServicos" class="form-control" maxlength="50" />
                            </div>
                        </div>
                        <div class="col-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">CNAE</span>
                                <input type="text" id="txtCNAECodigosServicos" class="form-control" maxlength="50" />
                            </div>
                        </div>

                    </div>
                    <button type="button" id="btnAdicionarCodigoServico" class="btn btn-primary">Adicionar</button>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblCodigosServicos" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Código Tributação
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Código Tributação Prefeitura
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">Número Tributação Prefeitura
                                    </th>
                                    <th style="width: 22%;" colspan="1" rowspan="1">CNAE
                                    </th>
                                    <th style="width: 10%;">Opções
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
            </div>
        </div>
        <div class="tab-pane" id="tabFinanceiro">
            <div class="panel panel-default">
                <div class="panel-heading">Duplicatas</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-5">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Contábil Padrão Para a Emissão de CT-e">Conta Cont. CTe</abbr>:
                                </span>
                                <input type="text" id="txtContaCTe" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarContaCTe" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Gerar movimentos automaticamente na emissão do CT-e">Gerar Dup.</abbr>:
                                </span>
                                <select class="form-control" id="selGerarDuplicatasAutomaticamente">
                                    <option value="0">Não</option>
                                    <option value="1">Sim</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Dias para vencimento dos movimentos gerados automaticamente na emissão do CT-e">Dias p/ Vencto.</abbr>:
                                </span>
                                <input type="text" id="txtDiasParaVencimento" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Número de parcelas que serão geradas na emissão do CT-e">Núm. Parcelas</abbr>:
                                </span>
                                <input type="text" id="txtNumeroParcelasDuplicatas" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Dias para aviso dos vencimentos (Com zero não gera a notificação)">Dias Aviso Vencimentos</abbr>:
                                </span>
                                <input type="text" id="txtDiasParaAvisoVencimentos" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-6">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkCadastrarItemDocumentoEntrada" />
                                        Cadastrar Item Automático Documento de Entrada
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-6">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkPermiteSelecionarCTeOutroTomador" />
                                        Permite selecionar CTe com outro tomador
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-6">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkNaoPermitirDuplciataMesmoDocumento" />
                                        Não permitir salvar duplicata com mesmo número de documento
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">SPED e CTe-OS</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-4 col-md-2 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Perfil da Empresa (SPED Fiscal)">Perfil</abbr>:
                                </span>
                                <select id="selPerfilSPED" class="form-control">
                                    <option value="0">A</option>
                                    <option value="1">B</option>
                                    <option value="2">C</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Indicador da Incidência Tributária no Período (SPED Contribuições)">Inc. Trib.</abbr>:
                                </span>
                                <select id="selIncidenciaTributariaNoPeriodo" class="form-control">
                                    <option value="1">Escrituração de operações com incidência exclusivamente no regime não-cumulativo</option>
                                    <option value="2">Escrituração de operações com incidência exclusivamente no regime cumulativo</option>
                                    <option value="3">Escrituração de operações com incidência nos regimes não-cumulativo e cumulativo</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Critério de Escrituração e Apuração Adotado (SPED Contribuições)">Cri. Escr.</abbr>:
                                </span>
                                <select id="selCriterioEscrituracaoEApuracao" class="form-control">
                                    <option value="1">Regime de Caixa - Escrituração Consolidada</option>
                                    <option value="2">Regime de Competência - Escrituração Consolidada</option>
                                    <option value="9">Regime de Competência - Escrituração Detalhada</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="CST do PIS para CT-e (SPED Contribuições e CTe-OS)">CST PIS</abbr>:
                                </span>
                                <select id="selCSTPIS" class="form-control">
                                    <option value="">Nenhum</option>
                                    <option value="1">01 - Operação Tributável com Alíquota Básica</option>
                                    <option value="2">02 - Operação Tributável com Alíquota Diferenciada</option>
                                    <option value="6">06 - Operação Tributável a Alíquota Zero</option>
                                    <option value="7">07 - Operação Isenta da Contribuição</option>
                                    <option value="8">08 - Operação sem Incidência da Contribuição</option>
                                    <option value="9">09 - Operação com Suspensão da Contribuição</option>
                                    <option value="49">49 - Outras Operações de Saída</option>
                                    <option value="99">99 - Outras Operações</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Alíquota do PIS para CT-e (SPED Contribuições e CTe-OS)">Alíq. PIS</abbr>:
                                </span>
                                <select id="selAliquotaPIS" class="form-control">
                                    <option value="">Nenhuma</option>
                                    <option value="0,65">0,65%</option>
                                    <option value="1,65">1,65%</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="CST da COFINS para CT-e (SPED Contribuições e CTe-OS)">CST COFINS</abbr>:
                                </span>
                                <select id="selCSTCOFINS" class="form-control">
                                    <option value="">Nenhum</option>
                                    <option value="1">01 - Operação Tributável com Alíquota Básica</option>
                                    <option value="2">02 - Operação Tributável com Alíquota Diferenciada</option>
                                    <option value="6">06 - Operação Tributável a Alíquota Zero</option>
                                    <option value="7">07 - Operação Isenta da Contribuição</option>
                                    <option value="8">08 - Operação sem Incidência da Contribuição</option>
                                    <option value="9">09 - Operação com Suspensão da Contribuição</option>
                                    <option value="49">49 - Outras Operações de Saída</option>
                                    <option value="99">99 - Outras Operações</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Alíquota da COFINS para CT-e (SPED Contribuições e CTe-OS)">Alíq. COFINS</abbr>:
                                </span>
                                <select id="selAliquotaCOFINS" class="form-control">
                                    <option value="">Nenhuma</option>
                                    <option value="3,00">3,00%</option>
                                    <option value="7,60">7,60%</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-2 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Alíquota de IR para CT-e OS">Alíq. IR</abbr>:
                                </span>
                                <input type="text" id="txtAliquotaIR" class="form-control" maxlength="5" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-2 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Alíquota de CSLL para CT-e OS">Alíq. CSLL</abbr>:
                                </span>
                                <input type="text" id="txtAliquotaCSLL" class="form-control" maxlength="5" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-2 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Alíquota de INSS para CT-e OS">Alíq. INSS</abbr>:
                                </span>
                                <input type="text" id="txtAliquotaINSS" class="form-control" maxlength="5" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-2 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Percentual da BC de INSS para CT-e OS">% BC INSS</abbr>:
                                </span>
                                <input type="text" id="txtPercentualBaseINSS" class="form-control" maxlength="5" />
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkDescontarINSSValorReceber" />
                                        Descontar INSS do Valor a Receber do CTe-OS
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Contas Contábeis</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Contábil Padrão para Abastecimentos">Abastecimento</abbr>:
                                </span>
                                <input type="text" id="txtContaAbastecimento" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarContaAbastecimento" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Contábil Padrão para Pagamento de Motoristas">P. Motorista</abbr>:
                                </span>
                                <input type="text" id="txtContaPagamentoMotorista" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarContaPagamentoMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Acerto de Viagem</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkBloquearDuplicidadeCTeAcerto" />
                                        Bloquear lançamento de Acerto duplicado para CTe
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-8">
                            <div class="input-group">
                                <span class="input-group-addon">Conta Receitas:</span>
                                <input type="text" id="txtAcertoViagemContaReceitas" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaReceitas" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Movimento:</span>
                                <select id="selAcertoViagemMovimentoReceitas" class="form-control">
                                    <option value="0">Por Acerto</option>
                                    <option value="1">Detalhado</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-sm-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Quando não informada gera movimento pelas contas configuradas nos Tipos de Despesas">Conta Despesas:</abbr>:</span>
                                <input type="text" id="txtAcertoViagemContaDespesas" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaDespesas" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Movimento:</span>
                                <select id="selAcertoViagemMovimentoDespesas" class="form-control">
                                    <option value="0">Por Acerto</option>
                                    <option value="1">Detalhado</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-sm-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Despesas Abastecimentos">Conta Desp. Abastecimentos</abbr>:</span>
                                <input type="text" id="txtAcertoViagemContaDespesasAbastecimentos" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaDespesasAbastecimentos" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Movimento:</span>
                                <select id="selAcertoViagemMovimentoDespesasAbastecimentos" class="form-control">
                                    <option value="0">Por Acerto</option>
                                    <option value="1">Detalhado</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-sm-8">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Despesas Adiantamento Motorista">Conta Desp. Adiant. Motorista</abbr>:</span>
                                <input type="text" id="txtAcertoViagemContaDespesasAdiantamentosMotorista" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaDespesasAdiantamentosMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Movimento:</span>
                                <select id="selAcertoViagemMovimentoDespesasAdiantamentosMotorista" class="form-control">
                                    <option value="0">Por Acerto</option>
                                    <option value="1">Detalhado</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-sm-8 divAcertoViagemContaReceitasDevolucoesMotorista" style="display: none">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Receitas Devoluções Motorista">Conta Rec. Devoluções Motorista</abbr>:</span>
                                <input type="text" id="txtAcertoViagemContaReceitasDevolucoesMotorista" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaReceitasDevolucoesMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-4 divAcertoViagemContaReceitasDevolucoesMotorista" style="display: none">
                            <div class="input-group">
                                <span class="input-group-addon">Movimento:</span>
                                <select id="selAcertoViagemMovimentoReceitasDevolucoesMotorista" class="form-control">
                                    <option value="0">Por Acerto</option>
                                    <option value="1">Detalhado</option>
                                </select>
                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Conta Despesas Pagamentos Motorista">Conta Desp. Pagamentos Motorista</abbr>:</span>
                                <input type="text" id="txtAcertoViagemContaDespesasPagamentosMotorista" class="form-control" autocomplete="off">
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarAcertoViagemContaDespesasPagamentosMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabAverbacao">
            <div class="panel panel-default">
                <div class="panel-heading">Configuração Padrão</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Enviar averbações automaticamente de todos CT-e(s)">Averba automaticamente</abbr>:
                                </span>
                                <select id="selAverbaAutomaticoATM" class="form-control">
                                    <option value="0">Não</option>
                                    <option value="1">Sim</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Seguradora de averbação dos CT-e(s)">Integradora</abbr>:
                                </span>
                                <select id="selSeguradoraAverbacao" class="form-control">
                                    <option value="">Não definida</option>
                                    <option value="A">ATM</option>
                                    <option value="B">Quorum</option>
                                    <option value="P">Porto Seguro</option>
                                    <option value="E">ELT</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Tipos de CT-e que averbam automaticamente">Tipos de CT-e</abbr>:
                                </span>
                                <select id="selTipoCTeAverbacao" class="form-control">
                                    <option value="0">Apenas Normal</option>
                                    <option value="1">Apenas Subcontratação</option>
                                    <option value="2">Apenas Redespacho</option>
                                    <option value="3">Apenas Red. Intermediario</option>
                                    <option value="4">Apenas Serv. Vinc. Multimodal</option>
                                    <option value="6">Apenas Transp. de Pessoas</option>
                                    <option value="7">Apenas Transp. de Valores</option>
                                    <option value="8">Apenas Excesso de Bagagem</option>
                                    <option value="99">Todos</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkAguardarAverbacaoCTeParaEmitirMDFe" />
                                        Aguardar averbação CT-e para emitir MDF-e
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkAverbarMDFe" />
                                        Averbar MDFe
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkAverbarNFSe" />
                                        Averbar NFSe
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Código ATM:
                                </span>
                                <input type="text" id="txtCodigoATM" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input type="text" id="txtUsuarioATM" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Senha:
                                </span>
                                <input type="text" id="txtSenhaATM" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12">
                            <div class="input-group">
                                <span class="input-group-addon">Token:
                                </span>
                                <input type="text" id="txtTokenSeguroBradesco" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12">
                            <div class="input-group">
                                <span class="input-group-addon">WSDL Quorum:
                                </span>
                                <input type="text" id="txtWsdlQuorum" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" id="divAverbacoes">
                <div class="panel-heading">
                    <h4 class="panel-title">
                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#divAverbacoes" href="#listaDeAverbacoes">Configuração por Cliente</a>
                    </h4>
                </div>
                <div id="listaDeAverbacoes" class="panel-collapse collapse">
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-sm-12 col-lg-12">
                                <div class="row">
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Cliente:
                                            </span>
                                            <input type="text" id="txtCliente" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Tipo:
                                            </span>
                                            <select id="selTipoAverbacao" class="form-control">
                                                <option value="0">Remetente</option>
                                                <option value="1">Expedidor</option>
                                                <option value="2">Recebedor</option>
                                                <option value="3">Destinatário</option>
                                                <option value="4">Tomador</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                                        <div class="input-group">
                                            <div class="checkbox">
                                                <label>
                                                    <input type="checkbox" id="chkAverbacaoConfiguracaoClienteRaizCNPJ">
                                                    Raíz do CNPJ
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                                        <div class="input-group">
                                            <div class="checkbox">
                                                <label>
                                                    <input type="checkbox" id="chkAverbacaoConfiguracaoClienteNaoAverbar">
                                                    Não Averbar
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Seguradora de averbação dos CT-e(s)">Integradora</abbr>:
                                            </span>
                                            <select id="selSeguradoraAverbacaoCliente" class="form-control">
                                                <option value="">Não definida</option>
                                                <option value="A">ATM</option>
                                                <option value="B">Quorum</option>
                                                <option value="P">Porto Seguro</option>
                                                <option value="E">ELT</option>
                                            </select>
                                        </div>
                                    </div>
                                    <%--<div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Tipos de CT-e que averbam automaticamente">Tipo CT-e</abbr>:
                                            </span>
                                            <select id="selTipoCTeAverbacaoCliente" class="form-control">
                                                <option value="0">Apenas CT-e Normal</option>
                                                <option value="1">Apenas CT-e Subcontratação</option>
                                                <option value="9">Todos</option>
                                            </select>
                                        </div>
                                    </div>--%>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Código ATM:
                                            </span>
                                            <input type="text" id="txtCodigoATMCliente" class="form-control" maxlength="200" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Usuário:
                                            </span>
                                            <input type="text" id="txtUsuarioATMCliente" class="form-control" maxlength="200" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Senha:
                                            </span>
                                            <input type="text" id="txtSenhaATMCliente" class="form-control" maxlength="200" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12">
                                        <div class="input-group">
                                            <span class="input-group-addon">Token Bradesco:
                                            </span>
                                            <input type="text" id="txtTokenSeguroBradescoCliente" class="form-control" maxlength="200" />
                                        </div>
                                    </div>
                                </div>
                                <button type="button" id="btnSalvarAverbacao" style="width: 83px" class="btn btn-primary">Salvar</button>
                                <button type="button" id="btnCancelarAverbacao" class="btn btn-default">Cancelar</button>
                                <button type="button" id="btnExcluirAverbacao" class="btn btn-danger" style="display: none">Excluir</button>
                            </div>
                            <div class="col-sm-12 col-lg-12">
                                <div class="table-responsive" style="margin-top: 20px">
                                    <table id="tblAverbacoes" class="table table-bordered table-hover table-condensed">
                                        <thead>
                                            <tr>
                                                <th width="40%">Cliente</th>
                                                <th width="10%">Raiz CNPJ</th>
                                                <th width="20%">Tipo</th>
                                                <th width="10%">Seguradora</th>
                                                <th width="20%">Opções</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="3">Nenhum registro encontrado.</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <input type="hidden" id="hddAverbacao" />
                        <input type="hidden" id="hddInformacoesAverbacao" />
                        <input type="hidden" id="hddIdAverbacaoEmEdicao" value="0" />
                    </div>
                </div>
            </div>

            <div class="panel panel-default" id="divAverbacoesSerie">
                <div class="panel-heading">
                    <h4 class="panel-title">
                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#divAverbacoesSerie" href="#listaDeAverbacoesSerie">Configuração de Série para não Averbar</a>
                    </h4>
                </div>
                <div id="listaDeAverbacoesSerie" class="panel-collapse collapse">
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-sm-12 col-lg-12">
                                <div class="row">
                                    <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Série CTe:
                                            </span>
                                            <input type="text" id="txtSerieCTeAverbacao" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarSerieCTeAverbacao" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <button type="button" id="btnSalvarAverbacaoSerie" style="width: 83px" class="btn btn-primary">Salvar</button>
                                <button type="button" id="btnCancelarAverbacaoSerie" class="btn btn-default">Cancelar</button>
                                <button type="button" id="btnExcluirAverbacaoSerie" class="btn btn-danger" style="display: none">Excluir</button>
                            </div>
                            <div class="col-sm-12 col-lg-12">
                                <div class="table-responsive" style="margin-top: 20px">
                                    <table id="tblAverbacoesSerie" class="table table-bordered table-hover table-condensed">
                                        <thead>
                                            <tr>
                                                <th width="80%">Serie</th>
                                                <th width="20%">Opções</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="3">Nenhum registro encontrado.</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <input type="hidden" id="hddAverbacaoSerie" />
                        <input type="hidden" id="hddInformacoesAverbacaoSerie" />
                        <input type="hidden" id="hddIdAverbacaoSerieEmEdicao" value="0" />
                    </div>
                </div>
            </div>


            <div class="panel panel-default" id="divAverbacaoPadrao">
                <div class="panel-heading">
                    <h4 class="panel-title">
                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#divAverbacaoPadrao" href="#listaDeAverbacaoPadrao" aria-expanded="true">Averbação Padrão MDF-e</a>
                    </h4>
                </div>
                <div id="listaDeAverbacaoPadrao" class="panel-collapse collapse">
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-xs-12 col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">CNPJ Seguradora:
                                    </span>
                                    <input type="text" id="txtSeguradoraCNPJ" class="form-control" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Nome Seguradora:
                                    </span>
                                    <input type="text" id="txtSeguradoraNome" class="form-control" maxlength="30" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Responsável:
                                    </span>
                                    <select id="selSeguroResponsavel" class="form-control">
                                        <option value="">Não definido</option>
                                        <option value="0">Remetente</option>
                                        <option value="1">Expedidor</option>
                                        <option value="2">Recebedor</option>
                                        <option value="3">Destinatário</option>
                                        <option value="4">Emitente (Transportador)</option>
                                        <option value="5">Contratante (Tomador do Serviço)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Nº da Apólice:
                                    </span>
                                    <input type="text" id="txtSeguradoraNApolice" class="form-control" maxlength="20" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Nº da Averbação:
                                    </span>
                                    <input type="text" id="txtSeguradoraNAverbacao" class="form-control" maxlength="40" />
                                </div>
                            </div>
                            <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                                <div class="input-group">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkNaoUtilizarDadosSeguroEmpresaPai" />
                                            Não utiliza dados da empresa pai
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabCIOT">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo Empresa*:
                        </span>
                        <select id="selTipoEmpresa" class="form-control">
                            <option value="0" selected="selected">Transportador</option>
                            <option value="1">Embarcador</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">Integradora*:
                        </span>
                        <select id="selTipoIntegradoraCIOT" class="form-control">
                            <option value="">Nenhuma</option>
                            <!--<option value="1">Siga Fácil</option>-->
                            <option value="2">PamCard</option>
                            <option value="5">PamCard (Opção de abertura)</option>
                            <option value="3">e-Frete</option>
                            <option value="4">e-Frete (Opção de abertura)</option>
                            <option value="6">TruckPad</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoPgtoCIOT">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo Pgto.*:
                        </span>
                        <select id="selTipoPagamentoCIOT" class="form-control">
                            <option value="">Nenhum</option>
                            <option value="0">Sem Pgto (eFrete)</option>
                            <option value="1">Cartão</option>
                            <option value="2">Depósito Bancário</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoCNPJMatrizCIOT">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="CNPJ da empresa Matriz (Quando filial)">CNPJ Matriz</abbr>*:
                        </span>
                        <input type="text" id="txtCNPJMatrizCIOT" class="form-control" maxlength="20" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoSigaFacil">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Chave Criptográfica">Chave Crip.</abbr>*:
                        </span>
                        <input type="text" id="txtChaveCriptograficaSigaFacil" class="form-control" maxlength="50" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoSigaFacil">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Código Contratante">Cód. Contrat.</abbr>*:
                        </span>
                        <input type="text" id="txtCodigoContratanteSigaFacil" class="form-control" maxlength="3" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoEFrete">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Usuário e-Frete">Usuário</abbr>*:
                        </span>
                        <input type="text" id="txtUsuarioEFrete" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoEFrete">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Senha e-Frete">Senha</abbr>*:
                        </span>
                        <input type="text" id="txtSenhaEFrete" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoEFrete">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Código do Integrador e-Frete">Integrador</abbr>*:
                        </span>
                        <input type="text" id="txtCodigoIntegradorEFrete" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoEFrete">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Bloquear Não Equiparado e-Frete">Bloq. Não Equip.</abbr>*:
                        </span>
                        <select id="selBloquearNaoEquiparadoEFrete" class="form-control">
                            <option value="true">Sim</option>
                            <option value="false">Não</option>
                        </select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoEFrete">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Utilizar Emissão Gratuita e-Frete">Gratuito</abbr>*:
                        </span>
                        <select id="selEmissaoGratuitaEFrete" class="form-control">
                            <option value="true">Sim</option>
                            <option value="false">Não</option>
                        </select>
                    </div>
                </div>

                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoTruckpad">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Usuário Truckpad">URL</abbr>*:
                        </span>
                        <input type="text" id="txtTruckPadURL" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 campoTruckpad">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Senha Truckpad">Usuário</abbr>*:
                        </span>
                        <input type="text" id="txtTruckPadUser" class="form-control" maxlength="200" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoTruckpad">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Senha Truckpad">Senha</abbr>*:
                        </span>
                        <input type="text" id="txtTruckPadPassword" class="form-control" maxlength="200" />
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabDicas">
            <p class="text-right">
                <button type="button" id="btnVisualizarLogDicas" class="btn btn-secundary">Visualizar Log</button>
            </p>

            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Dicas para a Emissão do CT-e">Dicas CT-e</abbr>:
                </span>
                <div class="ck-container">
                    <textarea id="txtDicasEmissaoCTe" class="form-control"></textarea>
                </div>
            </div>
            <br />
            <div id="messages-placeholderArquivosDicas"></div>

            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Arquivos referente as dicas para a Emissão do CT-e">Arquivos Dicas</abbr>:
                </span>

                <div class="lista-arquivos">
                    <div class="row">
                        <div class="col-sm-4 col-md-3 col-lg-2">
                            <div class="arquivo arquivo-inserir">
                                <a href="#" id="btnAdicionarArquivosDicas" title="Adicionar arquivos">
                                    <div class="arquivo-icone">
                                        <div style="background-image: url('Images/inserir-arquivo-dicas.png')"></div>
                                    </div>
                                    <div class="arquivo-nome">Adicionar arquivos</div>
                                </a>
                            </div>
                        </div>

                        <div id="arqInseridos"></div>
                    </div>
                </div>

            </div>
        </div>
        <div class="tab-pane" id="tabSeries">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Série para conhecimentos de transporte realizados para dentro do estado.">Série D.</abbr>:
                        </span>
                        <input type="text" id="txtSerieIntraestadual" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarSerieIntraestadual" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Série para conhecimentos de transporte realizados para fora do estado.">Série F.</abbr>:
                        </span>
                        <input type="text" id="txtSerieInterestadual" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarSerieInterestadual" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Série para conhecimentos de transporte complementares emitidos via integração.">Série Complementar</abbr>:
                        </span>
                        <input type="text" id="txtSerieComplementar" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarSerieComplementar" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Série para manifestos eletrônicos de documentos fiscais.">Série MDF-e</abbr>:
                        </span>
                        <input type="text" id="txtSerieMDFe" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarSerieMDFe" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Série CT-e por Estado Origem</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">UF*:
                                </span>
                                <select id="selUF" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Série para conhecimentos de transporte iniciados no estado selecionado.">Série</abbr>:
                                </span>
                                <input type="text" id="txtSeriePorUF" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarSeriePorUF" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <button type="button" id="btnAdicionarUFSerie" class="btn btn-primary">Adicionar</button>
                    </div>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblSeriesUF" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 30%;" colspan="1" rowspan="1">UF
                                    </th>
                                    <th style="width: 40%;" colspan="1" rowspan="1">Série
                                    </th>
                                    <th style="width: 10%;">Opções
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
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Série CT-e por Cliente</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Cliente:
                                </span>
                                <input type="text" id="txtClienteSerie" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarClienteSerie" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo:
                                </span>
                                <select id="selTipoClienteSerie" class="form-control">
                                    <option value="0">Remetente</option>
                                    <option value="1">Expedidor</option>
                                    <option value="2">Recebedor</option>
                                    <option value="3">Destinatário</option>
                                    <option value="4">Tomador</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Série para CTes com o Remetente o Cliente selecionado.">Série</abbr>:
                                </span>
                                <input type="text" id="txtSeriePorCliente" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarSeriePorCliente" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkSerieClienteRaizCNPJ">
                                        Raíz do CNPJ
                                    </label>
                                </div>
                            </div>
                        </div>
                        <button type="button" id="btnAdicionarSerieCliente" class="btn btn-primary">Adicionar</button>
                    </div>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblSeriesCliente" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 30%;" colspan="1" rowspan="1">Cliente
                                    </th>
                                    <th style="width: 10%;" colspan="1" rowspan="1">Raiz CNPJ
                                    </th>
                                    <th style="width: 20%;" colspan="1" rowspan="1">Tipo Cliente
                                    </th>
                                    <th style="width: 10%;" colspan="1" rowspan="1">Série
                                    </th>
                                    <th style="width: 10%;">Opções
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
            </div>
        </div>
        <div class="tab-pane" id="tabIntegracao">
            <div class="panel panel-default">
                <div class="panel-heading">Integração MultiCTe</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Token Integração:
                                </span>
                                <input type="text" id="txtTokenIntegracaoCTe" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Token Integração Envio CT-e:
                                </span>
                                <input type="text" id="txtTokenIntegracaoEnvioCTe" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">WS Integração Envio CT-e:
                                </span>
                                <input type="text" id="txtWsIntegracaoEnvioCTe" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Integração SM</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Integradora*:
                                </span>
                                <select id="selTipoIntegradoraSM" class="form-control">
                                    <option value="">Nenhuma</option>
                                    <option value="1">Trafegus</option>
                                    <option value="2">Buonny</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoTrafegus">
                            <div class="input-group">
                                <span class="input-group-addon">URL Trafegus:
                                </span>
                                <input type="text" id="txtTrafegusURL" class="form-control" maxlength="500" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoTrafegus">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário Trafegus:
                                </span>
                                <input type="text" id="txtTrafegusUsuario" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoTrafegus">
                            <div class="input-group">
                                <span class="input-group-addon">Senha Trafegus:
                                </span>
                                <input type="text" id="txtTrafegusSenha" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoBuonny">
                            <div class="input-group">
                                <span class="input-group-addon">URL Buonny:
                                </span>
                                <input type="text" id="txtBuonnyURL" class="form-control" maxlength="500" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoBuonny">
                            <div class="input-group">
                                <span class="input-group-addon">Token Buonny:
                                </span>
                                <input type="text" id="txtBuonnyToken" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoBuonny">
                            <div class="input-group">
                                <span class="input-group-addon">CNPJ Gerenciadora:
                                </span>
                                <input type="text" id="txtBuonnyGerenciadora" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8 campoBuonny">
                            <div class="input-group">
                                <span class="input-group-addon">Codigo tipo produto:
                                </span>
                                <input type="text" id="txtBuonnyCodigoTipoProduto" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">Mercado Livre</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">URL Mercado Livre:
                                </span>
                                <input type="text" id="txtMercadoLivreURL" class="form-control" maxlength="500" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">ID Client:
                                </span>
                                <input type="text" id="txtMercadoLivreID" class="form-control" maxlength="200" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Secret key:
                                </span>
                                <input type="text" id="txtMercadoLivreSecretKey" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-heading">Integração MultiTMS</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">WS Integração CTe:
                                </span>
                                <input type="text" id="txtWsIntegracaoEnvioCTeEmbarcadorTMS" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">WS Integração NFSe:
                                </span>
                                <input type="text" id="txtWsIntegracaoEnvioNFSeEmbarcadorTMS" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">WS Integração MDFe:
                                </span>
                                <input type="text" id="txtWsIntegracaoEnvioMDFeEmbarcadorTMS" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Token integração:
                                </span>
                                <input type="text" id="txtTokenIntegracaoEmbarcadorTMS" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <div class="tab-pane" id="tabNatura">
            <div class="panel panel-default">
                <div class="panel-heading">Natura</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Matriz:
                                </span>
                                <input type="text" id="txtNaturaMatriz" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Filial:
                                </span>
                                <input type="text" id="txtNaturaFilial" class="form-control" maxlength="20" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input type="text" id="txtNaturaUsuario" class="form-control" maxlength="50" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Senha:
                                </span>
                                <input type="text" id="txtNaturaSenha" class="form-control" maxlength="50" autocomplete="off" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Layout EDI:
                                </span>
                                <input type="text" id="txtNaturaLayoutEDI" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarNaturaLayoutEDI" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">FTP Natura</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Host:
                                </span>
                                <input id="txtFTPNaturaHost" class="form-control" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Porta:
                                </span>
                                <input id="txtFTPNaturaPorta" class="form-control" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input id="txtFTPNaturaUsuario" class="form-control" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Senha:
                                </span>
                                <input id="txtFTPNaturaSenha" class="form-control" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Diretório:
                                </span>
                                <input id="txtFTPNaturaDiretorio" class="form-control" autocomplete="off">
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPNaturaPassivo">
                                        Passivo
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPNaturaSeguro">
                                        Seguro
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8">
                            <div class="input-group">
                                <span class="input-group-addon">Layout EDI OCOREN:
                                </span>
                                <input type="text" id="txtNaturaLayoutEDIOcoren" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarNaturaLayoutEDIOcoren" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabConfiguracaoXml">
            <div class="panel panel-default">
                <div class="panel-heading">Lista de CNPJ/CPF permitidos</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-sm-12 col-lg-12">
                            <div class="row">
                                <div class="col-xs-10 col-sm-10 col-md-9 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CNPJ/CPF:</span>
                                        <input type="text" id="txtCNPJCPF" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-2 col-sm-2 col-md-3 col-lg-3">
                                    <button type="button" id="btnAdicionarCNPJCPF" class="btn btn-primary">Salvar</button>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-12 col-lg-12">
                            <div class="table-responsive" style="margin-top: 20px">
                                <table id="tblDocumentosDownload" class="table table-bordered table-hover table-condensed">
                                    <thead>
                                        <tr>
                                            <th width="80%">Documento</th>
                                            <th width="20%">Opções</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td colspan="2">Nenhum registro encontrado.</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <input type="hidden" id="hddDocumentosXML" />
                    <input type="hidden" id="hddInformacoesDocumento" />
                    <input type="hidden" id="hddIdDocumentosXML" value="0" />
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabAssEmail">
            <div class="row">
                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
                    <div class="input-group">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkEmailSemTexto" />
                                Enviar e-mails sem texto
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Assinatura para e-mails enviados automaticamente">Ass. e-mail</abbr>:
                        </span>
                        <textarea id="txtAssinaturaEmail" class="form-control" rows="30"></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="tabFTP">
            <div class="panel panel-default">
                <div class="panel-heading">FTP</div>
                <div class="panel-body">
                    <div id="placeholder-validacao-ftp"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Cliente:
                                </span>
                                <input type="text" id="txtClienteFTP" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarClienteFTP" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo:
                                </span>
                                <select id="selTipoFTP" class="form-control">
                                    <option value="0">Importação NOTFIS</option>
                                    <option value="1">Importação XML NFe</option>
                                    <option value="3">Envio EDI OCOREN CTe</option>
                                    <option value="4">Envio EDI OCOREN NFSe</option>
                                    <option value="5">Envio XML CTe</option>
                                    <option value="6">Envio EDI OCOREN NFe</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4">
                            <div class="input-group">
                                <span class="input-group-addon">Layout EDI:
                                </span>
                                <input type="text" id="txtLayoutEDIFTP" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarLayoutEDIFTP" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Host:
                                </span>
                                <input id="txtFTPHost" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Porta:
                                </span>
                                <input id="txtFTPPorta" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Usuário:
                                </span>
                                <input id="txtFTPUsuario" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Senha:
                                </span>
                                <input id="txtFTPSenha" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Diretório:
                                </span>
                                <input id="txtFTPDiretorio" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Agrupamento:
                                </span>
                                <select id="selFTPRateio" class="form-control">
                                    <option value="0">Remetente</option>
                                    <option value="1">Destinatário</option>
                                    <option value="2">Remetente e Destinatário</option>
                                    <option value="3">CTe por NF-e</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Arquivo:
                                </span>
                                <select id="selFTPTipoArquivo" class="form-control">
                                    <option value="1">Obter pela extensão</option>
                                    <option value="2">Texto</option>
                                    <option value="3">XML</option>
                                    <option value="6">XLSX (Riachuelo)</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPPassivo" />
                                        Passivo
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPSSL" />
                                        SSL
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPSeguro" />
                                        Seguro
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPGerarNFSe" />
                                        Gerar NFSe
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPEmitirDocumento" />
                                        Emitir Documento
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-4 col-sm-3 col-md-2">
                            <div class="input-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkFTPUtilizarContratanteComoTomador" />
                                        Utilizar cliente como tomador e origem dos documentos
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="button-groups">
                        <button type="button" id="btnSalvarFTP" class="btn btn-primary">Salvar</button>
                        <button type="button" id="btnCancelarFTP" class="btn btn-default">Cancelar</button>
                        <button type="button" id="btnExcluirFTP" class="btn btn-danger" style="display: none">Excluir</button>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-lg-12">
                            <div class="table-responsive" style="margin-top: 20px">
                                <table id="tblConfiguracoesFTP" class="table table-bordered table-hover table-condensed">
                                    <thead>
                                        <tr>
                                            <th>Cliente</th>
                                            <th>Layout</th>
                                            <th width="10%">Tipo</th>
                                            <th width="15%">Host</th>
                                            <th width="10%">Opções</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane" id="tabEstadosBloqueados">
            <div class="panel panel-default">
                <div class="panel-heading">Estados bloqueados para emissão de CT-e (Início da Prestação)</div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">UF*:
                                </span>
                                <select id="selEstadoBloqueado" class="form-control">
                                </select>
                            </div>
                        </div>
                        <button type="button" id="btnAdicionarEstadoBloquado" class="btn btn-primary">Adicionar</button>
                    </div>
                    <div class="table-responsive" style="margin-top: 10px;">
                        <table id="tblEstadosBloqueados" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 70%;" colspan="1" rowspan="1">UF
                                    </th>
                                    <th style="width: 10%;">Opções
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
            </div>
        </div>
    </div>

    <div class="modal fade" id="divLogDicas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Log Dicas CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderLogDicas"></div>
                    <div id="tbl_log_dicas" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_log_dicas">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDicas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Dica CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderDicas"></div>
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="log-dica">
                                <div id="txtLogDicasCTe"></div>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
</asp:Content>
