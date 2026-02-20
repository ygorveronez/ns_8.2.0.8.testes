<%@ Page Title="Emissão de CT-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmissaoCTe.aspx.cs" Inherits="EmissaoCTe.WebApp.EmissaoCTe" Culture="pt-BR" UICulture="pt-BR" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <style type="text/css">
        body.cte-200 .hidecte-200,
        body.cte-400 .hidecte-400,
        body.mdfe-100 .hidemdfe-100,
        body.mdfe-300 .hidemdfe-300 {
            display: none !important;
            visibility: hidden !important;
            opacity: 0 !important;
        }

        #divDica p {
            margin-top: 0px;
            margin-bottom: 0px;
        }

        @media screen and (min-width: 1024px) {
            #divEmissaoCTe .modal-dialog {
                right: auto;
                width: 1000px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoCTe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media screen and (min-width: 1200px) {
            #divEmissaoCTe .modal-dialog {
                right: auto;
                width: 1180px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoCTe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media (min-width: 992px) {
            #divNFesDestinadas .modal-dialog {
                width: 1000px;
            }
        }

        /*TAGS*/
        div.tfs-tags {
            margin-top: 10px;
            margin-bottom: 6px;
            line-height: 25px;
            margin-left: 5px;
        }

        div.tags-label-container {
            float: left;
            padding-left: 4px;
            padding-right: 10px;
            font-size: 11px;
            color: #000;
        }

        div.tags-items-container {
            overflow: hidden;
        }

        .tags-items-container ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
            display: block;
            -webkit-margin-before: 1em;
            -webkit-margin-after: 1em;
            -webkit-margin-start: 0px;
            -webkit-margin-end: 0px;
            -webkit-padding-start: 0px;
        }

            .tags-items-container ul > li {
                display: inline-block;
                margin-right: 5px;
                padding: 0;
                text-align: -webkit-match-parent;
            }

        .tag-item-delete-experience {
            white-space: nowrap;
            overflow: hidden;
        }

        .tag-container-delete-experience {
            cursor: pointer;
        }

        .tag-container {
            outline: none;
            padding-top: 2px;
            padding-bottom: 2px;
            border: 1px solid #fff !important;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

        .tag-box, .tag-delete {
            cursor: default;
            margin: 0;
            padding-left: 6px;
            padding-top: 2px;
            padding-right: 6px;
            padding-bottom: 2px;
            font-size: 12px;
            color: #4f4f4f;
            background-color: #d7e6f3;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            font-family: Segoe UI,Tahoma,Arial,Verdana;
            font-size: 12px;
            border-radius: 2px 0 0 2px;
        }

        .tag-delete {
            padding-left: 9px;
            padding-right: 9px;
            background: url('images/icon-close-small.png') no-repeat 50% 50%;
            background-color: #d7e6f3;
            border-radius: 0 2px 2px 0;
        }

            .tag-delete:focus, .tag-delete:hover {
                cursor: pointer;
                color: #fff;
                background-color: #b4c8d7;
            }
    </style>
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
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/emissaoCTe",
                           "~/bundle/scripts/cookie",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddTipoEmissao" value="" />
        <input type="hidden" id="hddValorFreteContratadoOriginal" value="0" />
        <input type="hidden" id="hddDadosEmpresa" value="" />
        <input type="hidden" id="hddDadosRemetente" value="" />
        <input type="hidden" id="hddConfiguracoesEmpresa" value="" />
        <input type="hidden" id="hddRemetenteFiltro" value="" />
        <input type="hidden" id="hddDestinatarioFiltro" value="" />
        <input type="hidden" id="hddCodigoCTE" value="0" />
        <input type="hidden" id="hddChaveCTEOriginal" value="" />
        <input type="hidden" id="hddTomador" value="" />
        <input type="hidden" id="hddAtividadeTomador" value="0" />
        <input type="hidden" id="hddRemetente" value="" />
        <input type="hidden" id="hddAtividadeRemetente" value="0" />
        <input type="hidden" id="hddExpedidor" value="" />
        <input type="hidden" id="hddAtividadeExpedidor" value="0" />
        <input type="hidden" id="hddRecebedor" value="" />
        <input type="hidden" id="hddAtividadeRecebedor" value="0" />
        <input type="hidden" id="hddDestinatario" value="" />
        <input type="hidden" id="hddAtividadeDestinatario" value="0" />
        <input type="hidden" id="hddNotasFiscaisEletronicasRemetente" value="" />
        <input type="hidden" id="hddNotaFiscalEletronicaRemetenteEmEdicao" value="" />
        <input type="hidden" id="hddIdComponenteDaPrestacaoEmEdicao" value="0" />
        <input type="hidden" id="hddComponentesDaPrestacao" value="" />
        <input type="hidden" id="hddIdInformacaoSeguroEmEdicao" value="0" />
        <input type="hidden" id="hddInformacoesSeguro" value="" />
        <input type="hidden" id="hddIdInformacaoQuantidadeCargaEmEdicao" value="0" />
        <input type="hidden" id="hddInformacoesQuantidadeCarga" value="" />
        <input type="hidden" id="hddVeiculos" value="" />
        <input type="hidden" id="hddOutrosDocumentosRemetente" value="" />
        <input type="hidden" id="hddOutroDocumentoRemetenteEmEdicao" />
        <input type="hidden" id="hddMotoristas" value="" />
        <input type="hidden" id="hddIdMotoristaEmEdicao" value="0" />
        <input type="hidden" id="hddObservacoesContribuinte" value="" />
        <input type="hidden" id="hddIdObservacaoContribuinteEmEdicao" value="0" />
        <input type="hidden" id="hddObservacoesFisco" value="" />
        <input type="hidden" id="hddIdObservacaoFiscoEmEdicao" value="0" />
        <input type="hidden" id="hddNotaFiscalRemetenteEmEdicao" value="0" />
        <input type="hidden" id="hddNotasFiscaisRemetente" value="" />
        <input type="hidden" id="hddDocsTranspAntPapel" value="" />
        <input type="hidden" id="hddDocTranspAntPapelEmEdicao" value="0" />
        <input type="hidden" id="hddEmissorDocTranspAntPapel" value="" />
        <input type="hidden" id="hddDocTranspAntEletronicoEmEdicao" value="0" />
        <input type="hidden" id="hddDocsTranspAntEletronico" value="" />
        <input type="hidden" id="hddEmissorDocTranspAntEletronico" value="" />
        <input type="hidden" id="hddProdutoPerigosoEmEdicao" value="0" />
        <input type="hidden" id="hddProdutosPerigosos" value="" />
        <input type="hidden" id="hddDuplicataEmEdicao" value="0" />
        <input type="hidden" id="hddDuplicatas" value="" />
        <input type="hidden" id="hddLocalEntregaDiferenteDestinatario" value="" />
        <input type="hidden" id="hddAtividadeLocalEntregaDiferenteDestinatario" value="0" />
        <input type="hidden" id="hddConsultaNFeSefazViewState" value="" />
        <input type="hidden" id="hddConsultaNFeSefazEventValidation" value="" />
        <input type="hidden" id="hddConsultaNFeSefazToken" value="" />
        <input type="hidden" id="hddConsultaNFeSefazSessionId" value="" />
        <input type="hidden" id="hddCodigoTabelaFreteImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoMotoristaImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoVeiculoImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoReboqueImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoSeguroImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoExpedidorImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoRecebedorImportacaoXML" value="" />
        <input type="hidden" id="hddCodigoTabelaFreteImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoMotoristaImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoReboqueImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoVeiculoImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoSeguroImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoExpedidorImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoRecebedorImportacaoSefaz" value="" />
        <input type="hidden" id="hddCodigoTabelaFreteImportacaoEDI" value="" />
        <input type="hidden" id="hddVeiculoTracaoNotfis" value="" />
        <input type="hidden" id="hddVeiculoReboqueNotfis" value="" />
        <input type="hidden" id="hddMotoristaNotfis" value="" />
        <input type="hidden" id="hddDescontarINSSValorReceber" value="" />
        <input type="hidden" id="hddCodigoCTEReferenciado" value="0" />

        <input type="hidden" id="hddCodigoTomadorMercadoLivre" value="" />
        <input type="hidden" id="hddCodigoTomador2MercadoLivre" value="" />
        <input type="hidden" id="hddCodigoExpedidorMercadoLivre" value="" />
        <input type="hidden" id="hddCodigoRecebedorMercadoLivre" value="" />
        <input type="hidden" id="hddCodigoMotoristaMercadoLivre" value="" />
        <input type="hidden" id="hddCodigoReboqueMercadoLivre" value="" />
        <input type="hidden" id="hddCodigoVeiculoMercadoLivre" value="" />
    </div>
    <div class="page-header">
        <h2>Emissão de CT-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnImportarXMLNFe" class="btn btn-default"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;Importar XML NF-e</button>
    <button type="button" id="btnImportarNFeSefaz" class="btn btn-default"><span class="glyphicon glyphicon-barcode"></span>&nbsp;Importar NF-e Sefaz </button>
    <button type="button" id="btnImportarXMLCTe" class="btn btn-default"><span class="glyphicon glyphicon-road"></span>&nbsp;Importar CT-e</button>
    <button type="button" id="btnImportarNOTFIS" class="btn btn-default"><span class="glyphicon glyphicon-globe"></span>&nbsp;Importar NOTFIS</button>
    <span runat="server" id="spnNFesDestinadas">
        <button type="button" id="btnNFesDestinadas" class="btn btn-default"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;NF-es Destinadas</button>
    </span>
    <span runat="server" id="spnMercadoLivre">
        <button type="button" id="btnConsultarMercadoLivre" class="btn btn-default"><span class="glyphicon glyphicon-barcode"></span>&nbsp;Consultar Mercado Livre</button>
    </span>
    <button type="button" id="btnNovoCTe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Novo CT-e</button>

    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicialCTeFiltro" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinalCTeFiltro" class="form-control maskedInput" />
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
                                    <option value="Y">Aguardando Finalizar Carga</option>
                                    <option value="F">FSDA</option>
                                    <option value="V">Vedado Cancelamento</option>
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
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Averbação:
                                </span>
                                <select id="selAverbacaoCTe" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="1">Averbados</option>
                                    <option value="2">Não Averbados</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarCTe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar CT-e</button>
    <button type="button" id="btnEmitirTodosCTes" class="btn btn-default" style="display: none"><span class="glyphicon glyphicon-paste"></span>&nbsp;Emitir todos CT-es</button>
    <button type="button" id="btnReenviarAverbacaoCTes" class="btn btn-default" style="display: none"><span class="glyphicon glyphicon-paste"></span>&nbsp;Reenviar Averbação CT-es</button>
    <div id="tbl_ctes" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_ctes">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divRetornosSefaz" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloRetornosSefaz" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="tbl_retornossefaz" class="table-responsive">
                    </div>
                    <div id="tbl_paginacao_retornossefaz">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divNFesDestinadas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão por NF-es Destinadas</h4>
                </div>
                <div class="modal-body">
                    <ul class="nav nav-tabs" id="tabsDocumentosDestinados">
                        <li><a href="#tabDocumentosDestinados" data-toggle="tab">Seleção NF-es</a></li>
                        <li><a href="#tabEmissaoDestinados" tabindex="-1" data-toggle="tab">Emissão CT-e</a></li>
                    </ul>
                    <div class="tab-content" style="margin-top: 10px;">
                        <div class="tab-pane active" id="tabDocumentosDestinados">
                            <div class="clearfix">
                                <button type="button" id="btnConsultarNFesDestinadas" class="btn btn-default" style="margin-bottom: 5px; float: right">&nbsp;Consultar NF-es Destinadas para Transporte no SEFAZ</button>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Inicial:
                                        </span>
                                        <input type="text" id="txtDDDataInicial" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Final:
                                        </span>
                                        <input type="text" id="txtDDDataFinal" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Placa:
                                        </span>
                                        <input type="text" id="txtDDPlaca" class="form-control maskedInput text-uppercase" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ Emi.:
                                        </span>
                                        <input type="text" id="txtDDCPFCNPJEmiente" class="form-control" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-7 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Emi.:
                                        </span>
                                        <input type="text" id="txtDDEmiente" class="form-control" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número Inicial:
                                        </span>
                                        <input type="text" id="txtDDNumero" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número Final:
                                        </span>
                                        <input type="text" id="txtDDNumeroFinal" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série:
                                        </span>
                                        <input type="text" id="txtDDSerie" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Sigla estado destinatário:
                                        </span>
                                        <input type="text" id="txtUFDestinatario" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Chave:
                                        </span>
                                        <input type="text" id="txtDDChave" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cancelado:
                                        </span>
                                        <select id="selDDCancelado" class="form-control">
                                            <option value="">Todos</option>
                                            <option value="false" selected="selected">Não</option>
                                            <option value="true">Sim</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Sit. Emissão:
                                        </span>
                                        <select id="selDDNotasSemCTe" class="form-control">
                                            <option value="" selected="selected">Todos</option>
                                            <option value="true">Pendentes</option>
                                            <option value="false">Emitidos</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3" style="float: right;">
                                    <div class="input-group">
                                        <span class="input-group-addon">Notas selecionadas:
                                        </span>
                                        <input type="text" id="txtDDNotasSelecionadas" class="form-control maskedInput" autocomplete="off" disabled>
                                    </div>
                                </div>
                            </div>

                            <div class="clearfix" style="margin-bottom: 5px">
                                <button type="button" id="btnConsultarDocumentosDestinados" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Filtrar</button>
                                <button type="button" id="btnSelecionarTodosDocumentosDestinados" class="btn btn-default" style="float: right">Selecionar Todos</button>
                                <button type="button" id="btnLimparSelecaoDocumentosDestinados" class="btn btn-default" style="float: right">Limpar Seleção</button>
                            </div>

                            <div class="">
                                <div id="tbl_documentosdestinados" class="table-responsive">
                                </div>
                                <div id="tbl_paginacao_documentosdestinados">
                                </div>
                                <div class="clearfix"></div>
                            </div>

                            <div class="divDocumentosDestinadosSelecionados">
                                <div class="tfs-tags">
                                    <div class="tags-items-container">
                                        <ul id="containerDocumentosDestinadosSelecionados">
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="clearfix">
                                <button type="button" id="btnTelaDocumentosDestinadosProximo" class="btn btn-primary pull-right">Próximo&nbsp;<span class="glyphicon glyphicon-chevron-right"></span></button>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabEmissaoDestinados">
                            <div id="divAgrupamentoDocumentosDestinados" class="input-group">
                                <span class="input-group-addon">Gerar CTe(s) agrupados*:
                                </span>
                                <select id="selAgrupamentoDocumentosDestinados" class="form-control">
                                    <option value="">todas NF-e(s) em um CTe</option>
                                    <option value="0">por Remetente e Destinatário</option>
                                    <option value="1">por Remetente</option>
                                    <option value="2">por Destinatário</option>
                                    <option value="4">por UF Destino</option>
                                    <option value="3">um CTe por NF-e</option>
                                </select>
                            </div>
                            <div class="row" id="divOpcoesAvancadasDocumentosDestinados" style="display: none">
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tipo Rateio:</span>
                                        <select id="selTipoRateioDocumentosDestinados" class="form-control">
                                            <option value="2">Por Peso (KG)</option>
                                            <option value="4">Por Volume (UN)</option>
                                            <option value="1">Por Valor Mercadoria</option>
                                            <option value="3">Por NFe</option>
                                            <option value="5">Por CTe</option>
                                            <option value="6">Por Peso (Agrupado por Destinatário)</option>
                                            <option value="7">Por Vlr. Mercadoria (Agrup. por Destinatário)</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor Total do Frete">Vl Frete</abbr>:
                                        </span>
                                        <input type="text" id="txtValorFreteDocumentosDestinados" value="0,00" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-sm-6" id="idPedagioDocumentosDestinados">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor Pedágio">Vl Pedágio</abbr>:
                                        </span>
                                        <input type="text" id="txtValorPedagioDocumentosDestinados" value="0,00" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-sm-6" id="idAdcEntregaDocumentosDestinados">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor Seguro">Valor Seguro</abbr>:
                                        </span>
                                        <input type="text" id="txtValorAdcEntregaDocumentosDestinados" value="0,00" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tração:
                                        </span>
                                        <input type="text" id="txtVeiculoDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarVeiculoDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>

                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Reboque:
                                        </span>
                                        <input type="text" id="txtReboqueDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarReboqueDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>

                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Motorista:
                                        </span>
                                        <input type="text" id="txtMotoristaDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarMotoristaDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label style="margin-left: 15px;">
                                                <input type="checkbox" id="chkOpcaoDigitacaoDocumentosDestinados" />
                                                Deixar CT-es em digitação.
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tomador:</span>
                                        <select id="selTipoTomadorDocumentosDestinados" class="form-control">
                                            <option value="-1">Selecione...</option>
                                            <option value="">Conf. NFe</option>
                                            <option value="0">Remetente</option>
                                            <option value="1">Destinatário</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Seguro:
                                        </span>
                                        <input type="text" id="txtSeguroDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarSeguroDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Expedidor:
                                        </span>
                                        <input type="text" id="txtExpedidorDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarExpedidorDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Recebedor:
                                        </span>
                                        <input type="text" id="txtRecebedorDocumentosDestinados" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarRecebedorDocumentosDestinados" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Observação para CT-e">Obs. CT-e</abbr>:
                                        </span>
                                        <input type="text" id="txtObservacaoDocumentosDestinados" value="" class="form-control" />
                                    </div>
                                </div>
                            </div>


                            <div class="clearfix">
                                <button type="button" id="btnTelaDocumentosDestinadosAnterior" class="btn btn-primary pull-left"><span class="glyphicon glyphicon-chevron-left"></span>&nbsp;Anterior</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default" style="float: left;">&nbsp;Cancelar</button>
                    <button type="button" id="btnGerarCTeDocumentosDestinados" class="btn btn-primary" style="float: left;">&nbsp;Gerar CT-e(s)</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divOcorrenciasCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloOcorrenciaCTe" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="tbl_ocorrencias" class="table-responsive">
                    </div>
                    <div id="tbl_paginacao_ocorrencias">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCancelamentoCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cancelamento de CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgCancelamentoCTe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataCancelamentoCTe" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaCancelamentoCTe" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row" id="divOpcoesCobrancaCancelamento" style="display: none">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Cobrar:</span>
                                <select id="selCobrarCancelamento" class="form-control">
                                    <option value="">Selecione</option>
                                    <option value="Sim">Sim</option>
                                    <option value="Nao">Não</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnCancelamentoCTe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar o CT-e</button>
                    <button type="button" id="btnCancelarCancelamentoCTe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divInutilizacaoCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Inutilização de CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgInutilizacaoCTe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaInutilizacaoCTe" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnInutilizarCTe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Inutlizar o CT-e</button>
                    <button type="button" id="btnCancelarInutilizacaoCTe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEnvioEmailCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Envio de DACTE/XML do CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEnvioEmail"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">E-mail(s):
                                </span>
                                <input type="text" id="txtEmailsEnvioXMLDacteCTe" class="form-control" maxlength="1000" />
                            </div>
                        </div>
                    </div>
                    <span class="help-block">Se houver mais de um separe-os por ponto-e-vírgula, ex: abc@abc.com.br; def@def.com.br.
                    </span>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEnviarEmailXMLDacteCTe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Enviar E-mail(s)</button>
                    <button type="button" id="btnCancelarEnvioEmailXMLDacteCTe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divUploadArquivos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloUploadArquivos" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div id="divImportacaoCTe" class="input-group">
                            <span class="input-group-addon">Agrupamento:
                            </span>
                            <select id="selAgrupamentoCTe" class="form-control">
                                <option value="0">Um CTe para cada XML</option>
                                <option value="1">Um CTe para todos XMLs</option>
                            </select>
                        </div>
                        <div id="divAgrupamentoXML" class="input-group">
                            <span class="input-group-addon">Gerar CTe(s) agrupados*:
                            </span>
                            <select id="selAgrupamentoXML" class="form-control">
                                <option value="">todos XML(s) em um CTe</option>
                                <option value="0">por Remetente e Destinatário</option>
                                <option value="1">por Remetente</option>
                                <option value="2">por Destinatário</option>
                                <option value="4">por UF Destino</option>
                                <option value="3">um CTe por XML</option>
                            </select>
                        </div>
                        <div class="row" id="divOpcoesAvancadasImportacaoXML" style="display: none">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Tabela Frete Valor">Tb Frete Vl</abbr>:
                                    </span>
                                    <input type="text" id="txtTabelaImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarTabelaImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tipo Rateio:</span>
                                    <select id="selTipoRateioImportacaoXML" class="form-control">
                                        <option value="2">Por Peso (KG)</option>
                                        <option value="4">Por Volume (UN)</option>
                                        <option value="1">Por Valor Mercadoria</option>
                                        <option value="3">Por NFe</option>
                                        <option value="5">Por CTe</option>
                                        <option value="6">Por Peso (Agrupado por Destinatário)</option>
                                        <option value="7">Por Vlr. Mercadoria (Agrup. por Destinatário)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor do Frete será rateado entre os documentos">Valor Frete</abbr>:
                                    </span>
                                    <input type="text" id="txtValorFreteImportacaoXML" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6" id="idPedagioImportacaoXML">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor será rateado entre documentos no componente PEDAGIO">Valor Pedágio</abbr>:
                                    </span>
                                    <input type="text" id="txtValorPedagioImportacaoXML" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6" id="idAdcEntregaImportacaoXML">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor será rateado entre documentos no componente SEGURO">Valor Seguro</abbr>:
                                    </span>
                                    <input type="text" id="txtValorAdcEntregaImportacaoXML" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tração:
                                    </span>
                                    <input type="text" id="txtVeiculoImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarVeiculoImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Reboque:
                                    </span>
                                    <input type="text" id="txtReboqueImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarReboqueImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Motorista:
                                    </span>
                                    <input type="text" id="txtMotoristaImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarMotoristaImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <div class="checkbox">
                                        <label style="margin-left: 15px;">
                                            <input type="checkbox" id="chkOpcaoXMLNFesDigitacao" />
                                            Deixar CT-es em digitação.
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tomador:</span>
                                    <select id="selTipoTomadorImportacaoXML" class="form-control">
                                        <option value="-1">Selecione...</option>
                                        <option value="">Conf. NFe</option>
                                        <option value="0">Remetente</option>
                                        <option value="1">Destinatário</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Seguro:
                                    </span>
                                    <input type="text" id="txtSeguroImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarSeguroImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Expedidor:
                                    </span>
                                    <input type="text" id="txtExpedidorImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarExpedidorImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Recebedor:
                                    </span>
                                    <input type="text" id="txtRecebedorImportacaoXML" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarRecebedorImportacaoXML" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Observação para CT-e">Obs. CT-e</abbr>:
                                    </span>
                                    <input type="text" id="txtObservacaoImportacaoXML" value="" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divUploadArquivosBody" class="row" style="padding: 15px;">
                        Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divConsultaNFeSefaz" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloConsultaNFeSefaz" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div class="">
                        <button type="button" id="btnImportarChavesNFes" class="btn btn-primary" style="margin-bottom: 5px">&nbsp;Importar Chaves NF-es</button>
                        <button type="button" id="btnImportarHTMLPortalNFe" class="btn btn-primary" style="margin-bottom: 5px">&nbsp;Importar HTML</button>
                        <div id="divAgrupamentoNFeSefaz" class="input-group">
                            <span class="input-group-addon">Gerar CTe(s) agrupados*:
                            </span>
                            <select id="selAgrupamentoNFeSefaz" class="form-control">
                                <option value="">todas NF-e(s) em um CTe</option>
                                <option value="0">por Remetente e Destinatário</option>
                                <option value="1">por Remetente</option>
                                <option value="2">por Destinatário</option>
                                <option value="4">por UF Destino</option>
                                <option value="3">um CTe por NF-e</option>
                            </select>
                        </div>
                        <div class="input-group">
                            <span class="input-group-addon">Chave NF-e:
                            </span>
                            <input type="text" id="txtChaveConsultaNFeSefaz" class="form-control" />
                            <span class="input-group-btn">
                                <button type="button" id="btnConsultaNFeSefaz" class="btn btn-primary"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar</button>
                            </span>
                        </div>
                        <div class="row" id="divOpcoesAvancadasImportacaoSefaz" style="display: none">
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">Tabela Frete Valor:
                                    </span>
                                    <input type="text" id="txtTabelaImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarTabelaImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tipo Rateio:</span>
                                    <select id="selTipoRateioImportacaoSefaz" class="form-control">
                                        <option value="2">Por Peso (KG)</option>
                                        <option value="4">Por Volume (UN)</option>
                                        <option value="1">Por Valor Mercadoria</option>
                                        <option value="3">Por NFe</option>
                                        <option value="5">Por CTe</option>
                                        <option value="6">Por Peso (Agrupado por Destinatário)</option>
                                        <option value="7">Por Vlr. Mercadoria (Agrup. por Destinatário)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Total do Frete">Vl Frete</abbr>:
                                    </span>
                                    <input type="text" id="txtValorFreteImportacaoSefaz" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6" id="idPedagioImportacaoSefaz">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Pedágio">Vl Pedágio</abbr>:
                                    </span>
                                    <input type="text" id="txtValorPedagioImportacaoSefaz" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6" id="idAdcEntregaImportacaoSefaz">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor será rateado entre documentos no componente SEGURO">Valor Seguro</abbr>:
                                    </span>
                                    <input type="text" id="txtValorAdcEntregaImportacaoSefaz" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">Motorista:
                                    </span>
                                    <input type="text" id="txtMotoristaImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarMotoristaImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tração:
                                    </span>
                                    <input type="text" id="txtVeiculoImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarVeiculoImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Reboque:
                                    </span>
                                    <input type="text" id="txtReboqueImportacaoSefaz" class="form-control" autocomplete="off">
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarReboqueImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tomador:</span>
                                    <select id="selTipoTomadorImportacaoSefaz" class="form-control">
                                        <option value="-1">Selecione...</option>
                                        <option value="">Conf. NFe</option>
                                        <option value="0">Remetente</option>
                                        <option value="1">Destinatário</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Seguro:
                                    </span>
                                    <input type="text" id="txtSeguroImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarSeguroImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Expedidor:
                                    </span>
                                    <input type="text" id="txtExpedidorImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarExpedidorImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Recebedor:
                                    </span>
                                    <input type="text" id="txtRecebedorImportacaoSefaz" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarRecebedorImportacaoSefaz" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Observação para CT-e">Obs. CT-e</abbr>:
                                    </span>
                                    <input type="text" id="txtObservacaoImportacaoSefaz" value="" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divConsultaNFeSefazBody" class="row" style="padding: 15px; overflow: scroll; max-height: 400px;">
                        <div class="table-responsive" style="margin-top: 10px;">
                            <table id="tblNFesSefaz" class="table table-bordered table-condensed table-hover">
                                <thead>
                                    <tr>
                                        <th style="width: 15%;" colspan="1" rowspan="1">Número NFe
                                        </th>
                                        <th style="width: 40%;" colspan="1" rowspan="1">Chave NFe
                                        </th>
                                        <th style="width: 20%;" colspan="1" rowspan="1">Destino
                                        </th>
                                        <th style="width: 15%;" colspan="1" rowspan="1">Opções
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="7">Nenhuma NF-e importada.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnGerarCTeNFeSefaz" class="btn btn-primary" style="float: left;">&nbsp;Gerar CT-e(s)</button>
                    <button type="button" id="btnFecharConsultaNFeSefaz" class="btn btn-default" style="float: left;">&nbsp;Cancelar / Limpar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divConsultaMercadoLivre" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloConsultaMercadoLivre" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="input-group">
                            <span class="input-group-addon">Código de Barras:
                            </span>
                            <input type="text" id="txtCodigoBarrasConsultaMercadoLivre" class="form-control" />
                            <span class="input-group-btn">
                                <button type="button" id="btnAdicionarCodigoBarrasMercadoLivre" class="btn btn-primary"><span class="glyphicon glyphicon-paste"></span>&nbsp;Adicionar</button>
                            </span>
                        </div>
                    </div>
                    <div id="divConsultaMercadoLivreBody" class="row" style="padding: 15px; overflow: scroll; max-height: 400px;">
                        <div class="table-responsive" style="margin-top: 10px;">
                            <table id="tblMercadoLivre" class="table table-bordered table-condensed table-hover">
                                <thead>
                                    <tr>
                                        <th style="width: 75%;" colspan="1" rowspan="1">Código Barras
                                        </th>
                                        <th style="width: 15%;" colspan="1" rowspan="1">Opções
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="7">Nenhuma NF-e importada.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="row">
                            <div class="col-sm-3">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Total do Frete">Vl Frete</abbr>:
                                    </span>
                                    <input type="text" id="txtValorFreteMercadoLivre" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-3" id="idPedagioMercadoLivre">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Pedágio">Vl Pedágio</abbr>:
                                    </span>
                                    <input type="text" id="txtValorPedagioMercadoLivre" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-3" id="idOutrosMercadoLivre">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Pedágio">Vl Outros</abbr>:
                                    </span>
                                    <input type="text" id="txtValorOutrosMercadoLivre" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-3" id="idPercentualGrisMercadoLivre">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Valor Pedágio">% GRIS</abbr>:
                                    </span>
                                    <input type="text" id="txtPercentualGrisMercadoLivre" value="0,00" class="form-control" />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tomador:
                                    </span>
                                    <input type="text" id="txtTomadorMercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarTomadorMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tomador2:
                                    </span>
                                    <input type="text" id="txtTomador2MercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarTomador2MercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Expedidor:
                                    </span>
                                    <input type="text" id="txtExpedidorMercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarExpedidorMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Recebedor:
                                    </span>
                                    <input type="text" id="txtRecebedorMercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarRecebedorMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Motorista:
                                    </span>
                                    <input type="text" id="txtMotoristaMercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarMotoristaMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Tração:
                                    </span>
                                    <input type="text" id="txtVeiculoMercadoLivre" class="form-control" />
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarVeiculoMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Reboque:
                                    </span>
                                    <input type="text" id="txtReboqueMercadoLivre" class="form-control" autocomplete="off">
                                    <span class="input-group-btn">
                                        <button type="button" id="btnBuscarReboqueMercadoLivre" class="btn btn-primary">Buscar</button>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <abbr title="Observação para CT-e">Obs. CT-e</abbr>:
                                    </span>
                                    <input type="text" id="txtObservacaoMercadoLivre" value="" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnGerarCTeMercadoLivre" class="btn btn-primary" style="float: left;">&nbsp;Gerar CT-e</button>
                    <button type="button" id="btnFecharConsultaMercadoLivre" class="btn btn-default" style="float: left;">&nbsp;Cancelar / Limpar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divCaptchaNFeSefaz" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Captcha consulta NF-e Sefaz</h4>
                </div>
                <div class="modal-body">
                    <div id="divCaptchaNFeSefazBody" class="row" style="padding: 15px;">
                        <div class="smart-form">
                            <section>
                                <div class="well">
                                    <div class="row">
                                        <div style="float: left; margin-top: 6px; margin-left: 100px;">
                                            <img src="#" id="imgCaptcha" style="float: left; margin-top: 6px; border: 1px solid #CCC; width: 260px; height: 80px;" /><a href="javascript:;void(0)" style="float: left; margin-top: 25px; margin-left: 5px;"><i class="fa fa-refresh"></i></a>
                                        </div>
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group" style="margin-top: 40px;">
                                                <span class="input-group-addon" style="margin-top: 6px">Captcha NF-e:
                                                </span>
                                                <input type="text" id="txtCaptchaNFeSefaz" class="form-control" autofocus />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnCaptchaNFeSefaz" class="btn btn-primary"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnAtualizarCaptchaNFeSefaz" class="btn btn-link" style="margin-left: 135px"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Gerar novo Captcha</button>
                                        </span>
                                    </div>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divImportarChavesNFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Importação NF-e arquivo CSV</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <button type="button" id="btnImportarCSV" class="btn btn-primary" style="float: left; margin-left: 15px;">&nbsp;Arquivo CSV</button>
                    </div>
                    <div id="divChavesNFeImportadasBody" class="row" style="padding: 15px; max-height: 400px; overflow-y: scroll; margin-top: 15px; margin-bottom: 10px;">
                        <div class="table-responsive" style="margin-top: -10px;">
                            <table id="tblChavesNFeImportadas" class="table table-bordered table-condensed table-hover" style="">
                                <thead>
                                    <tr>
                                        <th style="width: 80%;" colspan="1" rowspan="1">Chave NFe
                                        </th>
                                        <th style="width: 15%;" colspan="1" rowspan="1">Opções
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="7">Nenhuma Chave importada.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divUploadArquivoNOTFIS" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Envio de Arquivo EDI - NOTFIS</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
                            <div id="divLayoutNOTFIS" class="input-group">
                                <span class="input-group-addon">Layout*:
                                </span>
                                <select id="selLayoutNOTFIS" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
                            <div id="divAgrupamentoNotis" class="input-group">
                                <span class="input-group-addon">Gerar CTe(s) agrupados:
                                </span>
                                <select id="selAgrupamentoNOTFIS" class="form-control">
                                    <option value="0">por Remetente e Destinatário</option>
                                    <option value="1">por Remetente</option>
                                    <option value="2">por Destinatário</option>
                                    <option value="3">um CTe por NF-e</option>
                                    <option value="4">por PréCTe</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
                            <div id="divTabelaFreteNOTFIS" class="input-group">
                                <span class="input-group-addon">Tabela Frete Valor:
                                </span>
                                <input type="text" id="txtTabelaImportacaoEDI" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTabelaImportacaoEDI" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default">
                        <div id="divAvancadasNOTFIS" class="panel-heading">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#opcoesAvancadasNotfis" href="#opcoesAvancadasNotfis">Opções Avançadas
                                </a>
                            </h4>
                        </div>
                        <div id="opcoesAvancadasNotfis" class="panel-collapse collapse">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Frete:
                                            </span>
                                            <input type="text" id="txtValorFreteNotfis" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Pedagio:
                                            </span>
                                            <input type="text" id="txtValorPedagioNotfis" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">% Gris:
                                            </span>
                                            <input type="text" id="txtPercentualGrisNotfis" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">% Ad. Valorem:
                                            </span>
                                            <input type="text" id="txtPercentualAdvalorem" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Tração:
                                            </span>
                                            <input type="text" id="txtVeiculoTracaoNotfis" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarVeiculoTracaoNotfis" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Reboque:
                                            </span>
                                            <input type="text" id="txtVeiculoReboqueNotfis" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarVeiculoReboqueNotfis" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Motorista:
                                            </span>
                                            <input type="text" id="txtMotoristaNotfis" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarMotoristaNotfis" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label style="margin-left: 15px;">
                                                <input type="checkbox" id="chkOpcaoNotfisDigitacao" />
                                                Deixar CT-es em digitação.
                                            </label>
                                        </div>
                                    </div>
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label style="margin-left: 15px;">
                                                <input type="checkbox" id="chkOpcaoNotfisIncluirICMS" />
                                                Incluir ICMS no Frete.
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divUploadArquivoNOTFISBody" class="row" style="padding: 15px;">
                        Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                    </div>

                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divGerarMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Gerar MDF-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgConsultaCTes"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicial:
                                </span>
                                <input type="text" id="txtDataEmissaoInicialCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Final:
                                </span>
                                <input type="text" id="txtDataEmissaoFinalCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicialCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Final:
                                </span>
                                <input type="text" id="txtNumeroFinalCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Mot.:
                                </span>
                                <input type="text" id="txtNomeMotoristaCTeConsulta" class="form-control" maxlength="60" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF Mot.:
                                </span>
                                <input type="text" id="txtCPFMotoristaCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Placa:
                                </span>
                                <input type="text" id="txtPlacaVeiculoCTeConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnBuscarCTesConsulta" class="btn btn-primary">Buscar</button>
                    <button type="button" id="btnSelecionarTodosOsCTes" class="btn btn-default pull-right disabled">Selecionar Todos</button>
                    <div id="tbl_ctes_consulta" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_ctes_consulta_paginacao">
                    </div>
                    <div class="clearfix"></div>
                    <div class="divCTesSelecionados">
                        <div class="tfs-tags">
                            <div class="tags-items-container">
                                <ul id="containerCTesSelecionados">
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnGerarMDFeCTesGerados" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Gerar MDF-e</button>
                    <button type="button" id="btnCancelarGerarMDFe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Cancelar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divConsultarMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">MDF-es emitidos para este CT-e</h4>
                </div>
                <div class="modal-body">
                    <button type="button" id="btnConsultarMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar MDF-e</button>
                    <button type="button" id="btnGerarNovoMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Gerar novo MDF-e</button>
                    <div id="messages-placeholderMDFesEmitidos">
                    </div>
                    <div id="tbl_mdfe_cte" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_mdfe_cte">
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <button type="button" id="btnAbrirDicaEmissaoCTe" class="close" aria-hidden="true" style="font-size: 15px; font-weight: bolder; margin-top: 1px; margin-right: 5px;">&#63;</button>
                    <button type="button" id="btnAbrirObservacaoDigitacaoCTe" class="close" data-toggle="modal" data-target="#divObservacaoDigitacaoCTe" aria-hidden="true" style="font-size: 15px; font-weight: bolder; margin-top: 1px; margin-right: 5px;">&#33;</button>
                    <h4 class="modal-title">Emissão de CT-e</h4>
                </div>
                <div class="modal-body">
                    <ul class="nav nav-tabs" id="tabsEmissaoCTe">
                        <li class="active"><a href="#tabDados" data-toggle="tab">Dados</a></li>
                        <li><a href="#tabRemetente" tabindex="-1" data-toggle="tab">Remetente</a></li>
                        <li><a href="#tabDestinatario" tabindex="-1" data-toggle="tab">Destinatário</a></li>
                        <li class="dropdown">
                            <a href="#" data-toggle="dropdown" class="dropdown-toggle">Outros<b class="caret"></b></a>
                            <ul class="dropdown-menu" role="menu">
                                <li><a href="#tabEmitente" tabindex="-1" data-toggle="tab">Emitente</a></li>
                                <li><a href="#tabTomador" tabindex="-1" data-toggle="tab">Tomador</a></li>
                                <li><a href="#tabExpedidor" tabindex="-1" data-toggle="tab">Expedidor</a></li>
                                <li><a href="#tabRecebedor" tabindex="-1" data-toggle="tab">Recebedor</a></li>
                            </ul>
                        </li>
                        <li><a href="#tabInformacaoCarga" data-toggle="tab">Info. da Carga</a></li>
                        <li><a href="#tabServicosEImpostos" data-toggle="tab">Serv. e Impostos</a></li>
                        <li><a href="#tabCTeOutros" data-toggle="tab">CT-e Outros</a></li>
                        <li><a href="#tabsObservacoes" data-toggle="tab">Observações</a></li>
                        <li><a href="#tabsCancelamento" data-toggle="tab">Canc./Inut.</a></li>
                        <li><a href="#tabsResumo" data-toggle="tab">Resumo</a></li>
                    </ul>
                    <div class="tab-content" style="margin-top: 10px;">
                        <div class="tab-pane active" id="tabDados">
                            <div class="row">
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">Núm.:
                                        </span>
                                        <input type="text" id="txtNumero" class="form-control" value="Automático" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série*:
                                        </span>
                                        <asp:DropDownList ID="ddlSerie" runat="server" CssClass="form-control" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">Modelo*:
                                        </span>
                                        <select id="ddlModelo" class="form-control">
                                            <option value="57">57</option>
                                            <option value="67">67</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Forma de Pagamento">Pagam.</abbr>*:
                                        </span>
                                        <select id="selPago_APagar" class="form-control">
                                            <option value="0">Pago</option>
                                            <option value="1">A Pagar</option>
                                            <option value="2">Outros</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Serviço*:
                                        </span>
                                        <select id="selTipoServico" class="form-control">
                                            <option value="0">0 - Normal</option>
                                            <option value="1">1 - Subcontratação</option>
                                            <option value="2">2 - Redespacho</option>
                                            <option value="3">3 - Red. Intermediário</option>
                                            <option value="4">4 - Serv. Vinculado a Multimodal</option>
                                            <option value="6">6 - Transporte de Pessoas</option>
                                            <option value="7">7 - Transporte de Valores</option>
                                            <option value="8">8 - Excesso de Bagagem</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tipo*:
                                        </span>
                                        <select id="selTipoCTE" class="form-control">
                                            <option value="0">0 - Normal</option>
                                            <option value="1">1 - Complemento</option>
                                            <option value="2">2 - Anulação</option>
                                            <option value="3">3 - Substituto</option>
                                            <option value="4">4 - Simplificado</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Impressão*:
                                        </span>
                                        <select id="selFormaImpressao" class="form-control">
                                            <option value="1">1 - Retrato</option>
                                            <option value="2">2 - Paisagem</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Modal*:
                                        </span>
                                        <asp:DropDownList ID="ddlModalTransporte" runat="server" CssClass="form-control" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tomador*:
                                        </span>
                                        <select id="selTomadorServico" class="form-control">
                                            <option value="-1">Não Informado</option>
                                            <option value="0">0 - Remetente</option>
                                            <option value="1">1 - Expedidor</option>
                                            <option value="2">2 - Recebedor</option>
                                            <option value="3">3 - Destinatário</option>
                                            <option value="4">4 - Outros</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Natureza da Operação">Natureza*</abbr>:
                                        </span>
                                        <asp:DropDownList ID="ddlNaturezaOperacao" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div data-cte200="col-xs-12 col-sm-4 col-md-4 col-lg-2" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CFOP*:
                                        </span>
                                        <select id="selCFOP" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div data-cte200="hide" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Indicador do papel do tomador na prestação do serviço.">Ind. Toma</abbr>:</span>
                                        <select id="selIndIEToma" class="form-control">
                                            <option value="1" selected>Contribuinte ICMS</option>
                                            <option value="2">Isento de Inscrição</option>
                                            <option value="9">Não Contribuinte</option>
                                        </select>
                                    </div>
                                </div>
                                <div data-cte200="hide" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Indicador Globalizado.">Global.</abbr>:</span>
                                        <select id="selGlobalizado" class="form-control">
                                            <option value="1">Sim</option>
                                            <option value="0" selected>Não</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Emissão*:
                                        </span>
                                        <input type="text" id="txtDataEmissao" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div data-cte200="col-xs-12 col-sm-4 col-md-4 col-lg-2" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Hora*:
                                        </span>
                                        <input type="text" id="txtHoraEmissao" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-7 col-md-7 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Chave CT-e*:
                                        </span>
                                        <input type="text" id="txtChaveCTe" class="form-control" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Número do Protocolo de Autorização">Prot. Auto.</abbr>:
                                        </span>
                                        <input type="text" id="txtNumeroProtocoloAutorizacao" class="form-control" disabled="disabled" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox" style="margin-bottom: 5px; margin-top: 0;">
                                        <label>
                                            <input type="checkbox" id="chkRecebedorRetiraDestino" />
                                            Recebedor Retira no Aeroporto, Filial, Porto ou Estação de Destino.
                                        </label>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Detalhes da Retirada do Recebedor">Detalhes</abbr>:
                                        </span>
                                        <textarea id="txtDetalhesRetiradaRecebedor" class="form-control" rows="2"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">Local de Emissão do CT-e</div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlUFLocalEmissaoCTe" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Mun.*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlMunicipioLocalEmissaoCTe" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">Local de Início da Prestação</div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlUFInicioPrestacao" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Mun.*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlMunicipioInicioPrestacao" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">Local de Término da Prestação</div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlUFTerminoPrestacao" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Mun.*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlMunicipioTerminoPrestacao" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <span id="lblLogCTe" class="text-info"></span>
                        </div>
                        <div class="tab-pane" id="tabEmitente">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CNPJ:
                                        </span>
                                        <asp:TextBox ID="txtCNPJEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <asp:TextBox ID="txtIEEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">RNTRC:
                                        </span>
                                        <asp:TextBox ID="txtRNTRCEmpresa" runat="server" CssClass="form-control" MaxLength="8" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Razão Social:
                                        </span>
                                        <asp:TextBox ID="txtRazaoSocialEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Fantasia:
                                        </span>
                                        <asp:TextBox ID="txtNomeFantasiaEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Telefone:
                                        </span>
                                        <asp:TextBox ID="txtTelefoneEmitente" runat="server" CssClass="form-control maskedInput phone" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Endereço:
                                        </span>
                                        <asp:TextBox ID="txtEnderecoEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número:
                                        </span>
                                        <asp:TextBox ID="txtNumeroEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Bairro:
                                        </span>
                                        <asp:TextBox ID="txtBairroEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Complemento:
                                        </span>
                                        <asp:TextBox ID="txtComplementoEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEP:
                                        </span>
                                        <asp:TextBox ID="txtCEPEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado:
                                        </span>
                                        <asp:TextBox ID="txtUFEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade:
                                        </span>
                                        <asp:TextBox ID="txtCidadeEmitente" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                        </span>
                                        <asp:TextBox ID="txtEmailsEmitente" runat="server" CssClass="form-control" MaxLength="1000" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <asp:CheckBox ID="chkEmailsStatusEmitente" runat="server" Text="Enviar XML" ClientIDMode="Static" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Administrativos">@ Admin.</abbr>:
                                        </span>
                                        <asp:TextBox ID="txtEmailsAdministrativosEmitente" runat="server" CssClass="form-control" MaxLength="1000" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <asp:CheckBox ID="chkEmailsAdministrativosStatusEmitente" runat="server" Text="Enviar XML" ClientIDMode="Static" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                        </span>
                                        <asp:TextBox ID="txtEmailsContadorEmitente" runat="server" CssClass="form-control" MaxLength="1000" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <asp:CheckBox ID="chkEmailsContadorStatusEmitente" runat="server" Text="Enviar XML" ClientIDMode="Static" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabTomador">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" id="chkTomadorExportacao" />
                                                Cliente Exportação
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ*:
                                        </span>
                                        <input type="text" id="txtCPFCNPJTomador" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-2 col-sm-2 col-md-2 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnConsultarTomadorReceita" class="btn btn-primary">Consultar CNPJ Receita</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <input type="text" id="txtRGIETomador" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Razão Social*:
                                        </span>
                                        <input type="text" id="txtRazaoSocialTomador" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Fantasia:
                                        </span>
                                        <input type="text" id="txtNomeFantasiaTomador" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 1*:
                                        </span>
                                        <input type="text" id="txtTelefone1Tomador" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 2:
                                        </span>
                                        <input type="text" id="txtTelefone2Tomador" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Atividade*:
                                        </span>
                                        <input type="text" id="txtAtividadeTomador" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarAtividadeTomador" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Endereço*:
                                        </span>
                                        <input type="text" id="txtEnderecoTomador" maxlength="80" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número*:
                                        </span>
                                        <input type="text" id="txtNumeroTomador" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Bairro*:
                                        </span>
                                        <input type="text" id="txtBairroTomador" maxlength="40" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Complemento:
                                        </span>
                                        <input type="text" id="txtComplementoTomador" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEP*:
                                        </span>
                                        <input type="text" id="txtCEPTomador" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">País*:
                                        </span>
                                        <asp:DropDownList ID="ddlPaisTomador" runat="server" CssClass="form-control" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <asp:DropDownList ID="ddlEstadoTomador" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeTomador">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <select id="selCidadeTomador" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoTomador">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <input type="text" id="txtCidadeTomadorExportacao" class="form-control" maxlength="60" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsTomador" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsTomador" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContatoTomador" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContatoTomador" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContadorTomador" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContadorTomador" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsTransportadorTomador" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsTransportadorTomador" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkSalvarEnderecoTomador" checked="checked" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para este CT-e." />
                                            Salvar Endereço no Tomador
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabRemetente">
                            <div class="row">
                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tipo de Documento:
                                        </span>
                                        <select id="selTipoDocumentoRemetente" class="form-control">
                                            <option value="1">NF-es (Notas Fiscais Eletrônicas)</option>
                                            <option value="2">Notas Fiscais</option>
                                            <option value="3">Outros Documentos</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <ul class="nav nav-tabs" id="tabsRemetente">
                                        <li class="active"><a href="#tabDadosRemetente" data-toggle="tab">Remetente</a></li>
                                        <li><a href="#tabNFeRemetente" data-toggle="tab">NF-es</a></li>
                                        <li><a href="#tabNotasFiscaisRemetente" data-toggle="tab">Notas Fiscais</a></li>
                                        <li><a href="#tabOutrosRemetente" data-toggle="tab">Outros Documentos</a></li>
                                    </ul>
                                    <div class="tab-content" style="margin-top: 10px;">
                                        <div class="tab-pane active" id="tabDadosRemetente">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <div class="checkbox">
                                                            <label>
                                                                <input type="checkbox" id="chkRemetenteExportacao" />
                                                                Cliente Exportação
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF/CNPJ*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJRemetente" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-2 col-sm-2 col-md-2 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnConsultarRemetenteReceita" class="btn btn-primary">Consultar CNPJ Receita</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IE:
                                                        </span>
                                                        <input type="text" id="txtRGIERemetente" maxlength="20" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Razão Social*:
                                                        </span>
                                                        <input type="text" id="txtRazaoSocialRemetente" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome Fantasia:
                                                        </span>
                                                        <input type="text" id="txtNomeFantasiaRemetente" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Fone 1*:
                                                        </span>
                                                        <input type="text" id="txtTelefone1Remetente" maxlength="15" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Fone 2:
                                                        </span>
                                                        <input type="text" id="txtTelefone2Remetente" maxlength="15" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Atividade*:
                                                        </span>
                                                        <input type="text" id="txtAtividadeRemetente" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarAtividadeRemetente" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Endereço*:
                                                        </span>
                                                        <input type="text" id="txtEnderecoRemetente" maxlength="80" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:
                                                        </span>
                                                        <input type="text" id="txtNumeroRemetente" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Bairro*:
                                                        </span>
                                                        <input type="text" id="txtBairroRemetente" maxlength="40" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Complemento:
                                                        </span>
                                                        <input type="text" id="txtComplementoRemetente" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CEP*:
                                                        </span>
                                                        <input type="text" id="txtCEPRemetente" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">País*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlPaisRemetente" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Estado*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlEstadoRemetente" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeRemetente">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Cidade*:
                                                        </span>
                                                        <select id="selCidadeRemetente" class="form-control">
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoRemetente">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Cidade*:
                                                        </span>
                                                        <input type="text" id="txtCidadeRemetenteExportacao" class="form-control" maxlength="60" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsRemetente" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsRemetente" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsContatoRemetente" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsContatoRemetente" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsContadorRemetente" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsContadorRemetente" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsTransportadorRemetente" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsTransportadorRemetente" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="checkbox">
                                                        <label>
                                                            <input type="checkbox" id="chkSalvarEnderecoRemetente" checked="checked" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para este CT-e." />
                                                            Salvar Endereço no Remetente
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabNFeRemetente">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-9 col-md-9 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Chave*:
                                                        </span>
                                                        <input type="text" id="txtChaveNFeRemetente" class="form-control maskedInput" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarNFeRemetente" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-3 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Emissão:
                                                        </span>
                                                        <input type="text" id="txtDataEmissaoNFeRemetente" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor:
                                                        </span>
                                                        <input type="text" id="txtValorTotalNFeRemetente" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Peso (Kg):
                                                        </span>
                                                        <input type="text" id="txtPesoNFeRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarNFeRemetente" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirNFeRemetente" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarNFeRemetente" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                                <table id="tblNFesRemetente" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 12%;">Número</th>
                                                            <th style="width: 38%;">Chave NFe</th>
                                                            <th style="width: 7%;">Rem.</th>
                                                            <th style="width: 7%;">Des.</th>
                                                            <th style="width: 14%;">Emissão</th>
                                                            <th style="width: 14%;">Valor Total</th>
                                                            <th style="width: 7%;">Peso</th>
                                                            <th style="width: 10%;">Opções</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="5">Nenhum registro encontrado!
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabNotasFiscaisRemetente">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Modelo:
                                                        </span>
                                                        <asp:DropDownList ID="ddlModeloNotaFiscaiRemetente" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Série*:
                                                        </span>
                                                        <input type="text" id="txtSerieNotaFiscalRemetente" class="form-control" maxlength="3" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:
                                                        </span>
                                                        <input type="text" id="txtNumeroNotaFiscalRemetente" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Data Emissão*:
                                                        </span>
                                                        <input type="text" id="txtDataEmissaoNotaFiscalRemetente" class="form-control maskedInput" maxlength="8" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CFOP:
                                                        </span>
                                                        <input type="text" id="txtCFOPNotaFiscalRemetente" class="form-control maskedInput" maxlength="4" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">B.C. ICMS:
                                                        </span>
                                                        <input type="text" id="txtBaseCalculoICMSNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor ICMS:
                                                        </span>
                                                        <input type="text" id="txtValorICMSNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">B.C. ICMS ST:
                                                        </span>
                                                        <input type="text" id="txtBaseCalculoICMSSTNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor ICMS ST:
                                                        </span>
                                                        <input type="text" id="txtValorICMSSTNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Peso (Kg):
                                                        </span>
                                                        <input type="text" id="txtPesoTotalNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">PIN:
                                                        </span>
                                                        <input type="text" id="txtPINNotaFiscalRemetente" class="form-control" maxlength="9" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor dos Produtos da Nota Fiscal">Valor Prod.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorProdutosNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor Total da Nota Fiscal">Valor NF*</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorNotaNotaFiscalRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarNotaFiscalRemetente" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirNotaFiscalRemetente" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarNotaFiscalRemetente" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px; max-height: 300px; overflow-y: scroll;">
                                                <table id="tblNotasFiscaisRemetente" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 15%;">Número
                                                            </th>
                                                            <th style="width: 10%;">Série
                                                            </th>
                                                            <th style="width: 15%;">Data Emissão
                                                            </th>
                                                            <th style="width: 15%;">Valor ICMS
                                                            </th>
                                                            <th style="width: 17%;">Valor ICMS ST
                                                            </th>
                                                            <th style="width: 18%;">Valor Nota
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="7">Nenhum registro encontrado!
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabOutrosRemetente">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo Doc.*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlTipoDocumentoOutrosRemetente" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-8 col-md-8 col-lg-8">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Descrição*:
                                                        </span>
                                                        <input type="text" id="txtDescricaoOutrosRemetente" class="form-control text-uppercase" maxlength="100" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:
                                                        </span>
                                                        <input type="text" id="txtNumeroOutrosRemetente" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Data Emissão*:
                                                        </span>
                                                        <input type="text" id="txtDataEmissaoOutrosRemetente" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor*:
                                                        </span>
                                                        <input type="text" id="txtValorDocumentoOutrosRemetente" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarOutrosRemetente" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirOutrosRemetente" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarOutrosRemetente" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                                <table id="tblOutrosRemetente" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 20%;">Tipo Documento
                                                            </th>
                                                            <th style="width: 25%;">Descrição
                                                            </th>
                                                            <th style="width: 15%;">Número
                                                            </th>
                                                            <th style="width: 15%;">Data Emissão
                                                            </th>
                                                            <th style="width: 15%;">Valor Documento
                                                            </th>
                                                            <th style="width: 10px;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="6">Nenhum registro encontrado!
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabExpedidor">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" id="chkExpedidorExportacao" />
                                                Cliente Exportação
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ*:
                                        </span>
                                        <input type="text" id="txtCPFCNPJExpedidor" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarExpedidor" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-2 col-sm-2 col-md-2 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnConsultarExpedidorReceita" class="btn btn-primary">Consultar CNPJ Receita</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <input type="text" id="txtRGIEExpedidor" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Razão Social*:
                                        </span>
                                        <input type="text" id="txtRazaoSocialExpedidor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Fantasia:
                                        </span>
                                        <input type="text" id="txtNomeFantasiaExpedidor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 1*:
                                        </span>
                                        <input type="text" id="txtTelefone1Expedidor" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 2:
                                        </span>
                                        <input type="text" id="txtTelefone2Expedidor" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Atividade*:
                                        </span>
                                        <input type="text" id="txtAtividadeExpedidor" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarAtividadeExpedidor" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Endereço*:
                                        </span>
                                        <input type="text" id="txtEnderecoExpedidor" maxlength="80" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número*:
                                        </span>
                                        <input type="text" id="txtNumeroExpedidor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Bairro*:
                                        </span>
                                        <input type="text" id="txtBairroExpedidor" maxlength="40" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Complemento:
                                        </span>
                                        <input type="text" id="txtComplementoExpedidor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEP*:
                                        </span>
                                        <input type="text" id="txtCEPExpedidor" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">País*:
                                        </span>
                                        <asp:DropDownList ID="ddlPaisExpedidor" runat="server" CssClass="form-control" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <asp:DropDownList ID="ddlEstadoExpedidor" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeExpedidor">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <select id="selCidadeExpedidor" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoExpedidor">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <input type="text" id="txtCidadeExpedidorExportacao" class="form-control" maxlength="60" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsExpedidor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsExpedidor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContatoExpedidor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContatoExpedidor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContadorExpedidor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContadorExpedidor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsTransportadorExpedidor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsTransportadorExpedidor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkSalvarEnderecoExpedidor" checked="checked" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para este CT-e." />
                                            Salvar Endereço no Expedidor
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabRecebedor">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" id="chkRecebedorExportacao" />
                                                Cliente Exportação
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ*:
                                        </span>
                                        <input type="text" id="txtCPFCNPJRecebedor" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarRecebedor" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-2 col-sm-2 col-md-2 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnConsultarRecebedorReceita" class="btn btn-primary">Consultar CNPJ Receita</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <input type="text" id="txtRGIERecebedor" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Razão Social*:
                                        </span>
                                        <input type="text" id="txtRazaoSocialRecebedor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Fantasia:
                                        </span>
                                        <input type="text" id="txtNomeFantasiaRecebedor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 1*:
                                        </span>
                                        <input type="text" id="txtTelefone1Recebedor" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 2:
                                        </span>
                                        <input type="text" id="txtTelefone2Recebedor" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Atividade*:
                                        </span>
                                        <input type="text" id="txtAtividadeRecebedor" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarAtividadeRecebedor" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Endereço*:
                                        </span>
                                        <input type="text" id="txtEnderecoRecebedor" maxlength="80" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número*:
                                        </span>
                                        <input type="text" id="txtNumeroRecebedor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Bairro*:
                                        </span>
                                        <input type="text" id="txtBairroRecebedor" maxlength="40" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Complemento:
                                        </span>
                                        <input type="text" id="txtComplementoRecebedor" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEP*:
                                        </span>
                                        <input type="text" id="txtCEPRecebedor" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">País*:
                                        </span>
                                        <asp:DropDownList ID="ddlPaisRecebedor" runat="server" CssClass="form-control" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <asp:DropDownList ID="ddlEstadoRecebedor" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeRecebedor">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <select id="selCidadeRecebedor" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoRecebedor">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <input type="text" id="txtCidadeRecebedorExportacao" class="form-control" maxlength="60" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsRecebedor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsRecebedor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContatoRecebedor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContatoRecebedor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContadorRecebedor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContadorRecebedor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsTransportadorRecebedor" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsTransportadorRecebedor" />
                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkSalvarEnderecoRecebedor" checked="checked" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para este CT-e." />
                                            Salvar Endereço no Recebedor
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabDestinatario">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <ul class="nav nav-tabs" id="tabsDestinatario">
                                        <li class="active"><a href="#tabGeralDestinatario" data-toggle="tab">Destinatário</a></li>
                                        <li><a href="#tabLocalEntregaDiferenteDestinatario" data-toggle="tab">Local de Entrega Diferente</a></li>
                                    </ul>
                                    <div class="tab-content" style="margin-top: 10px;">
                                        <div id="tabGeralDestinatario" class="tab-pane active">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <div class="checkbox">
                                                            <label>
                                                                <input type="checkbox" id="chkDestinatarioExportacao" />
                                                                Cliente Exportação
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF/CNPJ*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJDestinatario" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnConsultarDestinatarioReceita" class="btn btn-primary">Consultar CNPJ Receita</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IE:
                                                        </span>
                                                        <input type="text" id="txtRGIEDestinatario" maxlength="20" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Suframa:
                                                        </span>
                                                        <input type="text" id="txtSuframaDestinatario" maxlength="9" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Razão Social*:
                                                        </span>
                                                        <input type="text" id="txtRazaoSocialDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome Fantasia:
                                                        </span>
                                                        <input type="text" id="txtNomeFantasiaDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Fone 1*:
                                                        </span>
                                                        <input type="text" id="txtTelefone1Destinatario" maxlength="15" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Fone 2:
                                                        </span>
                                                        <input type="text" id="txtTelefone2Destinatario" maxlength="15" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Atividade*:
                                                        </span>
                                                        <input type="text" id="txtAtividadeDestinatario" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarAtividadeDestinatario" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Endereço*:
                                                        </span>
                                                        <input type="text" id="txtEnderecoDestinatario" maxlength="80" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:
                                                        </span>
                                                        <input type="text" id="txtNumeroDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Bairro*:
                                                        </span>
                                                        <input type="text" id="txtBairroDestinatario" maxlength="40" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Complemento:
                                                        </span>
                                                        <input type="text" id="txtComplementoDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CEP*:
                                                        </span>
                                                        <input type="text" id="txtCEPDestinatario" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">País*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlPaisDestinatario" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Estado*:
                                                        </span>
                                                        <asp:DropDownList ID="ddlEstadoDestinatario" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeDestinatario">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Cidade*:
                                                        </span>
                                                        <select id="selCidadeDestinatario" class="form-control">
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoDestinatario">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Cidade*:
                                                        </span>
                                                        <input type="text" id="txtCidadeDestinatarioExportacao" class="form-control" maxlength="60" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsDestinatario" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsDestinatario" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsContatoDestinatario" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsContatoDestinatario" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsContadorDestinatario" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsContadorDestinatario" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="E-mails Específico por Transportador">@ Por Transportador</abbr>:
                                                        </span>
                                                        <input type="text" id="txtEmailsTransportadorDestinatario" class="form-control" maxlength="1000" />
                                                        <span class="input-group-addon">
                                                            <input type="checkbox" id="chkStatusEmailsTransportadorDestinatario" />
                                                            <abbr title="Enviar o XML e a DACTE Automaticamente">XML</abbr>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="checkbox">
                                                        <label>
                                                            <input type="checkbox" id="chkSalvarEnderecoDestinatario" checked="checked" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para este CT-e." />
                                                            Salvar Endereço no Destinatário
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="tabLocalEntregaDiferenteDestinatario" class="tab-pane">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF/CNPJ*:</span>
                                                        <input type="text" id="txtCPFCNPJ_LocalEntregaDiferenteDestinatario" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarLocalEntregaDiferenteDestinatario" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IE:</span>
                                                        <input type="text" id="txtRGIE_LocalEntregaDiferenteDestinatario" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Razão Social*:</span>
                                                        <input type="text" id="txtRazaoSocial_LocalEntregaDiferenteDestinatario" maxlength="80" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome Fantasia:</span>
                                                        <input type="text" id="txtNomeFantasia_LocalEntregaDiferenteDestinatario" maxlength="80" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Telefone 1*:</span>
                                                        <input type="text" id="txtTelefone1_LocalEntregaDiferenteDestinatario" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Telefone 2:</span>
                                                        <input type="text" id="txtTelefone2_LocalEntregaDiferenteDestinatario" class="form-control maskedInput phone" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Atividade*:</span>
                                                        <input type="text" id="txtAtividade_LocalEntregaDiferenteDestinatario" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarAtividadeLocalEntregaDiferenteDestinatario" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Endereço*:</span>
                                                        <input type="text" id="txtLogradouro_LocalEntregaDiferenteDestinatario" maxlength="80" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:</span>
                                                        <input type="text" id="txtNumero_LocalEntregaDiferenteDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Bairro*:</span>
                                                        <input type="text" id="txtBairro_LocalEntregaDiferenteDestinatario" maxlength="60" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Complemento:</span>
                                                        <input type="text" id="txtComplemento_LocalEntregaDiferenteDestinatario" maxlength="40" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CEP*:</span>
                                                        <input type="text" id="txtCEP_LocalEntregaDiferenteDestinatario" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">UF*:</span>
                                                        <asp:DropDownList ID="ddlUFLocalEntregaDiferenteDestinatario" CssClass="form-control" ClientIDMode="Static" runat="server">
                                                            <asp:ListItem Text="Selecione" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Município*:</span>
                                                        <select id="selLocalidade_LocalEntregaDiferenteDestinatario" class="form-control">
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabServicosEImpostos">
                            <div id="divInformacaoServicosEImpostos" class="row hidden" style="padding: 0 0 10px 15px;">
                                <button type="button" id="btnRecalcularFrete" class="btn btn-xs btn-default">Recalcular Frete</button>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor do Frete Contratado">Frete*:</abbr>
                                        </span>
                                        <input type="text" id="txtValorFreteContratado" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor Total da Prestação de Serviço">Prestação*:</abbr>
                                        </span>
                                        <input type="text" id="txtValorTotalPrestacaoServico" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor a Receber">A Receber*:</abbr>
                                        </span>
                                        <input type="text" id="txtValorAReceber" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkIncluirICMSNoFrete" />
                                            Incluir ICMS no Frete
                                        </label>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divPercentualICMSRecolhido" style="display: none;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Percentual de ICMS Recolhido">Percentual:</abbr>
                                        </span>
                                        <input type="text" id="txtPercentualICMSRecolhido" value="100,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Componentes dos Valores da Prestação de Serviços
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Descrição do Componente da Prestação">Descrição*:</abbr>
                                                </span>
                                                <input type="text" id="txtDescricaoComponentePrestacaoServico" class="form-control text-uppercase" maxlength="15" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Valor do Componente da Prestação">Valor*:</abbr>
                                                </span>
                                                <input type="text" id="txtValorComponentePrestacaoServico" class="form-control maskedInput" value="0,00" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="checkbox" style="margin-top: -4px; margin-bottom: 0;">
                                                <label>
                                                    <input type="checkbox" id="chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS" checked="checked" />
                                                    Incluir Valor Na B.C. ICMS
                                                </label>
                                            </div>
                                            <div class="checkbox" style="margin-top: 0; margin-bottom: 0;">
                                                <label>
                                                    <input type="checkbox" id="chkIncluirValorComponentePrestacaoNoTotalAReceber" checked="checked" />
                                                    Incluir Valor no Total a Receber
                                                </label>
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>
                                    </div>
                                    <button type="button" id="btnSalvarComponentePrestacaoServico" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirComponentePrestacaoServico" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarComponentePrestacaoServico" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 115px; overflow-y: scroll;">
                                        <table id="tblComponentesPrestacaoServico" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 30%;">Descrição
                                                    </th>
                                                    <th style="width: 20%;">Valor
                                                    </th>
                                                    <th style="width: 20%;">Inclui B.C. ICMS
                                                    </th>
                                                    <th style="width: 20%;">Inclui Total Receber
                                                    </th>
                                                    <th style="width: 10%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="5">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Informações Relativas aos Impostos
                                </div>
                                <div class="panel-body">
                                    <ul class="nav nav-tabs" id="tabsImpostosCTe">
                                        <li class="active"><a href="#tabICMS" data-toggle="tab">ICMS</a></li>
                                        <li><a href="#tabPIS" data-toggle="tab">PIS</a></li>
                                        <li><a href="#tabCOFINS" data-toggle="tab">COFINS</a></li>
                                        <li><a href="#tabIR" data-toggle="tab">IR</a></li>
                                        <li><a href="#tabINSS" data-toggle="tab">INSS</a></li>
                                        <li><a href="#tabCSLL" data-toggle="tab">CSLL</a></li>
                                        <li><a href="#tabInfAdFisco" data-toggle="tab">Info. Adicional</a></li>
                                        <li><a href="#tabCBSIBS" data-toggle="tab">IBS e CBS</a></li>
                                    </ul>
                                    <div class="tab-content" style="margin-top: 10px;">
                                        <div class="tab-pane active" id="tabICMS">
                                            <div class="row">
                                                <div id="divBuscarImpostos" style="padding: 0 0 10px 15px;">
                                                    <button type="button" id="btnRecalcularImpostos" class="btn btn-xs btn-default">Atualizar Tributações</button>
                                                </div>
                                                <div class="col-xs-12 col-sm-10 col-md-10 col-lg-10">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">ICMS*:
                                                        </span>
                                                        <select id="selICMS" class="form-control">
                                                            <option value="0">Selecione</option>
                                                            <option value="1">00 - ICMS Normal</option>
                                                            <option value="2">20 - ICMS com Redução de Base de Cálculo</option>
                                                            <option value="3">40 - ICMS Isenção</option>
                                                            <option value="4">41 - ICMS Não Tributado</option>
                                                            <option value="5">51 - ICMS Diferido</option>
                                                            <option value="6">60 - ICMS Pagto atr. ao tomador ou 3º previsto para ST</option>
                                                            <option value="9">90 - ICMS Outras Situações</option>
                                                            <option value="10">90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente</option>
                                                            <option value="11">Simples Nacional</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" id="divDadosICMS" style="display: none;">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divReducaoBaseCalculoICMS">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Percentual de Redução da Base de Cálculo do ICMS">% Red. B.C.</abbr>
                                                        </span>
                                                        <input type="text" id="txtReducaoBaseCalculoICMS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorBaseCalculoICMS">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo do ICMS">Valor B.C.:</abbr>
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoICMS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divAliquotaICMS">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Alíquota:
                                                        </span>
                                                        <select id="selAliquotaICMS" class="form-control">
                                                            <option value="0">Selecione</option>
                                                            <option value="7">7,00%</option>
                                                            <option value="12">12,00%</option>
                                                            <option value="17">17,00%</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorICMS">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor:
                                                        </span>
                                                        <input type="text" id="txtValorICMS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorCreditoICMS">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor do Crédito Outorgado/Presumido">Crédito:</abbr>
                                                        </span>
                                                        <input type="text" id="txtValorCreditoICMS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divExibirICMSNaDACTE">
                                                    <div class="checkbox">
                                                        <label>
                                                            <input type="checkbox" id="chkExibirICMSNaDACTE" checked="checked" />
                                                            Exibir na DACTE
                                                        </label>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorICMSDesoneracao">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor do ICMS Desoneração">Valor Desoneração:</abbr>
                                                        </span>
                                                        <input type="text" id="txtValorICMSDesoneracao" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divCodigoBeneficio">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Código do Benefício">Cód. Benefício*:</abbr>
                                                        </span>
                                                        <input type="text" id="txtCodigoBeneficio" class="form-control text-uppercase" maxlength="15" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabPIS">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-10 col-md-10 col-lg-9">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CST*:
                                                        </span>
                                                        <select id="selPIS" class="form-control">
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
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Valor B.C.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoPIS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Alíquota:
                                                        </span>
                                                        <select id="selAliquotaPIS" class="form-control">
                                                            <option value="0,00">0,00%</option>
                                                            <option value="0,65">0,65%</option>
                                                            <option value="1,65">1,65%</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor:
                                                        </span>
                                                        <input type="text" id="txtValorPIS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabCOFINS">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-10 col-md-10 col-lg-9">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CST*:
                                                        </span>
                                                        <select id="selCOFINS" class="form-control">
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
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Valor B.C.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoCOFINS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Alíquota:
                                                        </span>
                                                        <select id="selAliquotaCOFINS" class="form-control">
                                                            <option value="0,00">0,00%</option>
                                                            <option value="3,00">3,00%</option>
                                                            <option value="7,60">7,60%</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor:
                                                        </span>
                                                        <input type="text" id="txtValorCOFINS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabIR">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Valor B.C.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoIR" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Alíquota</abbr>:
                                                        </span>
                                                        <input type="text" id="txtAliquotaIR" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor IR:
                                                        </span>
                                                        <input type="text" id="txtValorIR" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabINSS">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Valor B.C.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoINSS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Alíquota</abbr>:
                                                        </span>
                                                        <input type="text" id="txtAliquotaINSS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor INSS:
                                                        </span>
                                                        <input type="text" id="txtValorINSS" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabCSLL">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Valor B.C.</abbr>:
                                                        </span>
                                                        <input type="text" id="txtValorBaseCalculoCSLL" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Base de Cálculo">Alíquota</abbr>:
                                                        </span>
                                                        <input type="text" id="txtAliquotaCSLL" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor CSLL:
                                                        </span>
                                                        <input type="text" id="txtValorCSLL" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tabInfAdFisco">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Informações adicionais de interesse do Fisco">Inf. Ad. Fisco:</abbr>
                                                        </span>
                                                        <textarea id="txtInformacaoAdicionalFisco" class="form-control" rows="4" maxlength="2000"></textarea>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="tab-pane" id="tabCBSIBS">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CST*:
                                                        </span>
                                                        <input type="text" id="txtIBSCBS_CST" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarIBSCBS_CST_Class" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Classificação Tributária:</span>
                                                        <input type="text" id="txtIBSCBS_Class" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4" id="divBaseCalculoIbsCbs">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Base de Cálculo:</span>
                                                        <input type="text" id="txtIBSCBSBaseCalculo" value="0,00" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="row-ibscbs" class="row" style="margin-top: 10px;">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="panel panel-default">
                                                        <div class="panel-heading">IBS Estadual</div>
                                                        <div class="panel-body">
                                                            <div class="row">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota (%):</span>
                                                                        <input type="text" id="txtIBSEstadualAliquota" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Redução da Alíquota (%):</span>
                                                                        <input type="text" id="txtIBSEstadualReducao" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota Efetiva (%):</span>
                                                                        <input type="text" id="txtIBSEstadualEfetiva" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Valor:</span>
                                                                        <input type="text" id="txtIBSEstadualValor" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="panel panel-default">
                                                        <div class="panel-heading">IBS Municipal</div>
                                                        <div class="panel-body">
                                                            <div class="row">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota (%):</span>
                                                                        <input type="text" id="txtIBSMunAliquota" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Redução da Alíquota (%):</span>
                                                                        <input type="text" id="txtIBSMunReducao" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota Efetiva (%):</span>
                                                                        <input type="text" id="txtIBSMunEfetiva" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Valor:</span>
                                                                        <input type="text" id="txtIBSMunValor" value="0,00" class="form-control maskedInput" disabled />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="panel panel-default">
                                                        <div class="panel-heading">CBS</div>
                                                        <div class="panel-body">
                                                            <div class="row">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota (%):</span>
                                                                        <input type="text" id="txtCBSAliquota" value="0,00" class="form-control maskedInput" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Redução da Alíquota (%):</span>
                                                                        <input type="text" id="txtCBSReducao" value="0,00" class="form-control maskedInput" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Alíquota Efetiva (%):</span>
                                                                        <input type="text" id="txtCBSEfetiva" value="0,00" class="form-control maskedInput" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="margin-top: 5px;">
                                                                <div class="col-xs-12 col-sm-12">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon">Valor:</span>
                                                                        <input type="text" id="txtCBSValor" value="0,00" class="form-control maskedInput" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabInformacaoCarga">
                            <ul class="nav nav-tabs" id="tabsInformacoesCarga">
                                <li class="active"><a href="#tabInformacoes" tabindex="-1" data-toggle="tab">Informações</a></li>
                                <li><a href="#tabDocumentosAnteriores" tabindex="-1" data-toggle="tab">Doc. Anteriores</a></li>
                                <li><a href="#tabRodoviario" tabindex="-1" data-toggle="tab">Rodoviário</a></li>
                                <li><a href="#tabSeguro" tabindex="-1" data-toggle="tab">Seguro</a></li>
                                <li><a href="#tabProdutosPerigosos" tabindex="-1" data-toggle="tab">Prod. Perigosos</a></li>
                                <li><a href="#tabDadosCobranca" tabindex="-1" data-toggle="tab">Cobranças</a></li>
                                <li><a href="#tabPercurso" tabindex="-1" data-toggle="tab">Percursos</a></li>
                            </ul>
                            <div class="tab-content" style="margin-top: 10px;">
                                <div class="tab-pane active" id="tabInformacoes">
                                    <div class="row">
                                        <div data-cte200="col-xs-12 col-sm-4 col-md-4 col-lg-3" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Valor Total da Carga">Valor Carga*</abbr>:
                                                </span>
                                                <input type="text" id="txtValorTotalCarga" value="0,00" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div data-cte200="hide" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Valor da Carga para efeito de averbação">Vlr. Averbação</abbr>:
                                                </span>
                                                <input type="text" id="txtValorCargaAverbacao" value="0,00" class="form-control" maxlength="30" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Container:
                                                </span>
                                                <input type="text" id="txtConteiner" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data Prevista de Entrega do Conteiner">Entr. Container</abbr>:
                                                </span>
                                                <input type="text" id="txtDataPrevistaEntregaConteiner" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div data-cte200="col-xs-12 col-sm-4 col-md-4 col-lg-3" data-cte400="col-xs-12 col-sm-4 col-md-4 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Número do Lacre do Container">Núm. Lacre</abbr>:
                                                </span>
                                                <input type="text" id="txtNumeroLacre" class="form-control maskedInput" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Produto Predominante">Produto Pred.*</abbr>:
                                                </span>
                                                <input type="text" id="txtProdutoPredominante" class="form-control text-uppercase" maxlength="60" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Outras Características da Carga">Outras Carac.</abbr>:
                                                </span>
                                                <input type="text" id="txtOutrasCaracteristicasCarga" class="form-control text-uppercase" maxlength="30" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Característica Adicional do Transporte">Carac. Tra.</abbr>:
                                                </span>
                                                <input type="text" id="txtCaracteristicaAdicionalTransporte" class="form-control text-uppercase" maxlength="15" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Característica Adicional do Serviço">Carac. Ser.</abbr>:
                                                </span>
                                                <input type="text" id="txtCaracteristicaAdicionalServico" class="form-control text-uppercase" maxlength="30" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Indicador Negociável CTe MultiModal">Ind. Negociável:</abbr>
                                                </span>
                                                <select id="selIndNegociavel" class="form-control">
                                                    <option value="0">Não Negociável</option>
                                                    <option value="1">Negociável</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo de Fretamento para CTe OS">Tp. Fretamento:</abbr>
                                                </span>
                                                <select id="selTipoFretamento" class="form-control">
                                                    <option value="1">Eventual</option>
                                                    <option value="2">Contínuo</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data/hora da viagem para CTe OS do tipo Eventual">Dt. Viagem:</abbr>
                                                </span>
                                                <input type="text" id="txtDataHoraViagem" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Informações de Quantidade da Carga
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Unidade de Medida">UM*:</abbr>
                                                        </span>
                                                        <select id="selUnidadeMedida" class="form-control">
                                                            <option value="-1">Selecione</option>
                                                            <option value="0">M3</option>
                                                            <option value="1">KG</option>
                                                            <option value="2">TON</option>
                                                            <option value="3">UN</option>
                                                            <option value="4">LT</option>
                                                            <option value="5">MMBTU</option>
                                                            <option value="99">M3_ST</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo Medida*:
                                                        </span>
                                                        <input type="text" id="txtTipoUnidadeMedida" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Quantidade*:
                                                        </span>
                                                        <input type="text" id="txtQuantidade" value="0,0000" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarInformacaoQuantidadeCarga" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirInformacaoQuantidadeCarga" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarInformacaoQuantidadeCarga" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblInformacaoQuantidadeCarga" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 35%;">Unidade de Medida
                                                            </th>
                                                            <th style="width: 35%;">Tipo da Medida
                                                            </th>
                                                            <th style="width: 20%;">Quantidade
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="4">Nenhum registro encontrado.
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabDocumentosAnteriores">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Documentos de Transporte Anterior em Papel
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CNPJ Emi.*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJEmissorDocTranspAntPapel" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome Emi.*:
                                                        </span>
                                                        <input type="text" id="txtEmissorDocTranspAntPapel" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarEmissorDocTranspAntPapel" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo Doc.:
                                                        </span>
                                                        <select id="selTipoDocTranspAntPapel" class="form-control">
                                                            <!--<option value="00">CTRC</option>-->
                                                            <!--<option value="01">CTAC</option>-->
                                                            <!--<option value="02">ACT</option>-->
                                                            <!--<option value="03">NF Modelo 7</option>-->
                                                            <!--<option value="04">NF Modelo 27</option>-->
                                                            <!--<option value="05">Conhecimento Aéreo Nacional</option>-->
                                                            <!--<option value="06">CTMC</option>-->
                                                            <option value="07">ATRE</option>
                                                            <option value="08">DTA (Despacho de Trânsito Aduaneiro)</option>
                                                            <option value="09">Conhecimento Aéreo Internacional</option>
                                                            <option value="10">Conhecimento - Carta de Porte Internacional</option>
                                                            <option value="11" selected>Conhecimento Avulso</option>
                                                            <option value="12">TIF (Transporte Internacional Ferroviário)</option>
                                                            <option value="13">BIL (Bil of Lading)</option>
                                                            <!--<option value="99">Outros</option>-->
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Número*:
                                                        </span>
                                                        <input type="text" id="txtNumeroDocTranspAntPapel" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Série*:
                                                        </span>
                                                        <input type="text" id="txtSerieDocTranspAntPapel" class="form-control" maxlength="3" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Emissão*:
                                                        </span>
                                                        <input type="text" id="txtDataEmissaoDocTranspAntPapel" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarDocTranspAntPapel" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirDocTranspAntPapel" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarDocTranspAntPapel" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblDocsTranspAntPapel" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 20%;">Emissor
                                                            </th>
                                                            <th style="width: 30%;">Tipo do Documento
                                                            </th>
                                                            <th style="width: 15%;">Número
                                                            </th>
                                                            <th style="width: 10%;">Série
                                                            </th>
                                                            <th style="width: 15%;">Data de Emissão
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
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
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Documentos de Transporte Anterior Eletrônicos
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CNPJ Emi.*:
                                                        </span>
                                                        <input type="text" id="txtCPFCNPJEmissorDocTranspAntEletronico" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome Emi.*:
                                                        </span>
                                                        <input type="text" id="txtEmissorDocTranspAntEletronico" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarEmissorDocTranspAntEletronico" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Chave*:
                                                        </span>
                                                        <input type="text" id="txtChaveDocTranspAntEletronico" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarDocTranspAntEletronico" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirDocTranspAntEletronico" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarDocTranspAntEletronico" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblDocsTranspAntEletronico" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 20%;">Emissor
                                                            </th>
                                                            <th style="width: 70%;">Chave
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="3">Nenhum registro encontrado.
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabRodoviario">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">RNTRC*:
                                                </span>
                                                <asp:TextBox ID="txtRNTRC" runat="server" CssClass="form-control" MaxLength="8" ClientIDMode="Static"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data Prevista para Entrega da Carga">Prev. Ent.</abbr>:
                                                </span>
                                                <input type="text" id="txtDataPrevistaEntregaCargaRecebedor" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Série CTRB:
                                                </span>
                                                <input type="text" id="txtSerieCTRB" class="form-control" maxlength="3" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Núm. CTRB:
                                                </span>
                                                <input type="text" id="txtNumeroCTRB" class="form-control" maxlength="3" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">CIOT:
                                                </span>
                                                <input type="text" id="txtCIOT" class="form-control" maxlength="12" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <div class="checkbox">
                                                    <label>
                                                        <input type="checkbox" id="chkIndicadorLotacao" />
                                                        Indicador de Lotação
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Veículos
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Placa*:
                                                        </span>
                                                        <input type="text" id="txtPlacaVeiculo" class="form-control maskedInput text-uppercase" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <button type="button" id="btnAdicionarVeiculo" class="btn btn-primary">Salvar</button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblVeiculos" class="table table-bordered table-hover table-condensed">
                                                    <thead>
                                                        <tr>
                                                            <th>Placa</th>
                                                            <th>UF</th>
                                                            <th>Renavam</th>
                                                            <th>Tipo</th>
                                                            <th>Rodado</th>
                                                            <th>Carroc.</th>
                                                            <th>Tipo</th>
                                                            <th>Cap.(kg)</th>
                                                            <th>Cap.(m³)</th>
                                                            <th>Opções</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="10">Nenhum registro encontrado.</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Motoristas
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Nome*:
                                                        </span>
                                                        <input type="text" id="txtNomeMotorista" class="form-control text-uppercase" maxlength="200" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CPF*:
                                                        </span>
                                                        <input type="text" id="txtCPFMotorista" class="form-control maskedInput" maxlength="20" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                                    <button type="button" id="btnSalvarMotorista" class="btn btn-primary">Salvar</button>
                                                    <button type="button" id="btnExcluirMotorista" class="btn btn-danger" style="display: none;">Excluir</button>
                                                    <button type="button" id="btnCancelarMotorista" class="btn btn-default">Cancelar</button>
                                                </div>
                                            </div>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblMotoristas" class="table table-bordered table-condensed table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 60%;">Nome
                                                            </th>
                                                            <th style="width: 30%;">CPF
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="3">Nenhum registro encontrado.
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabSeguro">
                                    <div id="divInformacaoServicosSeguro" class="row hidden" style="padding: 0 0 10px 15px;">
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Responsável*:
                                                </span>
                                                <select id="selResponsavelSeguro" class="form-control">
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
                                                <span class="input-group-addon">Seguradora*:
                                                </span>
                                                <input type="text" id="txtNomeSeguradora" class="form-control" maxlength="30" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4 hidemdfe-100">
                                            <div class="input-group">
                                                <span class="input-group-addon">CNPJ Seguradora*:
                                                </span>
                                                <input type="text" id="txtCNPJSeguradora" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Apólice*:
                                                </span>
                                                <input type="text" id="txtNumeroApolice" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">Averbação*:
                                                </span>
                                                <input type="text" id="txtNumeroAverberacao" class="form-control" maxlength="40" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Valor da Mercadoria para Efeito de Averbação">Valor*</abbr>:
                                                </span>
                                                <input type="text" id="txtValorMercadoriaParaEfeitoDeAverbacao" value="0,00" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-2 col-md-2 col-lg-2">
                                            <button type="button" id="btnBuscarApoliceSeguro" class="btn btn-primary">Buscar Apólice</button>
                                        </div>
                                    </div>
                                    <div style="margin-top: 15px">
                                        <button type="button" id="btnSalvarInformacaoSeguro" class="btn btn-primary">Salvar</button>
                                        <button type="button" id="btnExcluirInformacaoSeguro" class="btn btn-danger" style="display: none;">Excluir</button>
                                        <button type="button" id="btnCancelarInformacaoSeguro" class="btn btn-default">Cancelar</button>
                                    </div>
                                    <div class="table-responsive" style="margin-top: 10px;">
                                        <table id="tblInformacaoSeguro" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th>Responsável</th>
                                                    <th>Seguradora</th>
                                                    <th class="hidemdfe-100">CNPJ</th>
                                                    <th>Nº da Apólice</th>
                                                    <th>Nº da Averbação</th>
                                                    <th>Valor da Mercadoria</th>
                                                    <th>Opções</th>
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
                                <div class="tab-pane" id="tabProdutosPerigosos">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Número ONU/UN*:
                                                </span>
                                                <input type="text" id="txtNumeroONU" class="form-control" maxlength="4" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-9">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Nome Apropriado Embarque Produto">Nome*</abbr>:
                                                </span>
                                                <input type="text" id="txtNomeApropriadoEmbarqueProduto" class="form-control text-uppercase" maxlength="150" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-5">
                                            <div class="input-group">
                                                <span class="input-group-addon">Classe Risco*:
                                                </span>
                                                <input type="text" id="txtClasseRisco" class="form-control" maxlength="40" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Grupo Embalagem:
                                                </span>
                                                <input type="text" id="txtGrupoEmbalagem" class="form-control" maxlength="6" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Quantidade Total por Produto">Quant. Prod.:</abbr>
                                                </span>
                                                <input type="text" id="txtQuantidadeTotalPorProduto" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Ponto de Fulgor">Fulgor</abbr>:
                                                </span>
                                                <input type="text" id="txtPontoDeFulgor" class="form-control" maxlength="6" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-9">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Quantidade e Tipo de Volumes">Quant. Vol.:</abbr>
                                                </span>
                                                <input type="text" id="txtQuantidadeETipoDeVolumes" class="form-control" maxlength="60" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarProdutoPerigoso" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirProdutoPerigoso" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarProdutoPerigoso" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px;">
                                        <table id="tblProdutosPerigosos" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 10%;">Núm. ONU
                                                    </th>
                                                    <th style="width: 15%;">Nome Apropriado
                                                    </th>
                                                    <th style="width: 13%;">Classe de Risco
                                                    </th>
                                                    <th style="width: 14%;">Grupo de Embalagem
                                                    </th>
                                                    <th style="width: 14%;">Quantidade Total
                                                    </th>
                                                    <th style="width: 14%;">Quantidade e Tipo
                                                    </th>
                                                    <th style="width: 10%;">Ponto de Fulgor
                                                    </th>
                                                    <th style="width: 10%;">Opções
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
                                <div class="tab-pane" id="tabDadosCobranca">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Quantidade de Duplicatas">Qtde. Dup.</abbr>:
                                                </span>
                                                <input type="text" id="txtQuantidadeDeDuplicatas" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Frequência dos Vencimentos">Freq. Venc.</abbr>:
                                                </span>
                                                <input type="text" id="txtFrequenciaVencimentos" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data do Primeiro Vencimento">1º Venc.</abbr>:
                                                </span>
                                                <input type="text" id="txtDataPrimeiroVencimento" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Arredondar na:
                                                </span>
                                                <select id="selArredondamentoDuplicatas" class="form-control">
                                                    <option value="0">Primeira Duplicata</option>
                                                    <option value="1">Última Duplicata</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-2 col-md-2 col-lg-2">
                                            <button type="button" id="btnGerarDuplicatas" class="btn btn-primary">Gerar Duplicatas</button>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Duplicatas
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Núm.:
                                                        </span>
                                                        <input type="text" id="txtNumeroDuplicata" class="form-control" disabled="disabled" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Parcela:
                                                        </span>
                                                        <input type="text" id="txtParcelaDuplicata" class="form-control" disabled="disabled" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Vencto.:
                                                        </span>
                                                        <input type="text" id="txtDataVencimentoDuplicata" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Valor:
                                                        </span>
                                                        <input type="text" id="txtValorDuplicata" class="form-control maskedInput" value="0,00" />
                                                    </div>
                                                </div>
                                            </div>
                                            <button type="button" id="btnSalvarDuplicata" class="btn btn-primary">Salvar</button>
                                            <button type="button" id="btnExcluirDuplicata" class="btn btn-danger" style="display: none;">Excluir</button>
                                            <button type="button" id="btnCancelarDuplicata" class="btn btn-default">Cancelar</button>
                                            <div class="table-responsive" style="margin-top: 10px;">
                                                <table id="tblDuplicatasCobranca" class="table table-bordered table-condensed table-hover table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 15%;">Número
                                                            </th>
                                                            <th style="width: 15%;">Parcela
                                                            </th>
                                                            <th style="width: 30%;">Data de Vencimento
                                                            </th>
                                                            <th style="width: 30%;">Valor
                                                            </th>
                                                            <th style="width: 10%;">Opções
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="5">Nenhum registro encontrado.
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div class="tab-pane" id="tabPercurso">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">Estado*:
                                                </span>
                                                <select id="selUFPercurso" class="form-control">
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarPercurso" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirPercurso" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarPercurso" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                                        <table id="tblPercurso" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 85%;">Estado
                                                    </th>
                                                    <th style="width: 15%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabCTeOutros">
                            <ul class="nav nav-tabs" id="tabsCTeOutros">
                                <li><a href="#tabCTeComlementar" tabindex="-1" data-toggle="tab">CT-e Complementar/Multimodal</a></li>
                                <li><a href="#tabCTeAnulacao" tabindex="-1" data-toggle="tab">CT-e Anulação</a></li>
                                <li><a href="#tabCTeSubstituicao" tabindex="-1" data-toggle="tab">CT-e Substituição</a></li>
                            </ul>
                            <div class="tab-content" style="margin-top: 10px;">
                                <div class="tab-pane" id="tabCTeComlementar">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave do CT-e a ser Complementado">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveCTeComplementado" class="form-control" disabled="disabled" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabCTeAnulacao">
                                    <!--<span class="text-info">CT-e de Anulação de valores é possível apenas quando o Tomador do CT-e anulado não for contribuinte do ICMS e possuir uma declaração de anulação.
                                    </span>-->
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave do CT-e a ser Anulado">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveCTeAnulado" class="form-control" disabled="disabled" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Data do Evento de Desacordo ou da Declaração de Anulação.">Data Evt./Dec.*:</abbr>
                                                </span>
                                                <input type="text" id="txtDataEmissaoDeclaracaoCTeAnulado" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="tabCTeSubstituicao">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave do CT-e a ser Substituído">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveCTeSubstituido" class="form-control" disabled="disabled" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tomador é Contribuinte do ICMS">Contrib.*:</abbr>
                                                </span>
                                                <select id="selTomadorContribuinteICMS" class="form-control">
                                                    <option value="0">Não</option>
                                                    <option value="1">Sim</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Subs. Tomador:
                                                </span>
                                                <select id="selSubTomador" class="form-control">
                                                    <option value="0" selected>Não</option>
                                                    <option value="1">Sim</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="divTomadorNaoContribuinte">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave de Acesso do CT-e de Anulação">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveAcessoCTeAnulacao" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="divTomadorContribuinte" style="display: none;">
                                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo de Documento">Tipo Doc.*:</abbr>
                                                </span>
                                                <select id="selTipoDocumentoTomadorContribuinte" class="form-control">
                                                    <option value="0">Selecione</option>
                                                    <option value="1">Chave de Acesso do CT-e Emitido pelo Tomador</option>
                                                    <option value="2">Chave de Acesso da NF-e Emitida pelo Tomador</option>
                                                    <option value="3">Informações da NF ou CT Emitido pelo Tomador</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="divChaveAcessoCTeTomador" style="display: none;">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave de Acesso do CT-e Emitido pelo Tomador">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveAcessoCTeEmitidoTomador" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="divChaveAcessoNFeTomador" style="display: none;">
                                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Chave de Acesso da NF-e Emitida pelo Tomador">Chave*:</abbr>
                                                </span>
                                                <input type="text" id="txtChaveAcessoNFeEmitidaTomador" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="divInformacaoNFouCTEmitidoTomador" style="display: none;">
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">CNPJ*:
                                                </span>
                                                <input type="text" id="txtCNPJNFouCTEmitidoTomador" class="form-control maskedInput" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarEmitenteDocumentoSubstituicao" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group">
                                                <span class="input-group-addon">Modelo*:
                                                </span>
                                                <asp:DropDownList ID="ddlModeloNFouCTEmitidoTomador" CssClass="form-control" runat="server" ClientIDMode="Static">
                                                    <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-xs-3 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Série*:
                                                </span>
                                                <input type="text" id="txtSerieNFouCTEmitidoTomador" class="form-control" maxlength="3" />
                                            </div>
                                        </div>
                                        <div class="col-xs-3 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Sub-série:
                                                </span>
                                                <input type="text" id="txtSubserieNFouCTEmitidoTomador" class="form-control" maxlength="3" />
                                            </div>
                                        </div>
                                        <div class="col-xs-3 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Número*:
                                                </span>
                                                <input type="text" id="txtNumeroNFouCTEmitidoTomador" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                        <div class="col-xs-3 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Valor*:
                                                </span>
                                                <input type="text" id="txtValorNFouCTEmitidoTomador" class="form-control maskedInput" value="0,00" />
                                            </div>
                                        </div>
                                        <div class="col-xs-3 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">Data*:
                                                </span>
                                                <input type="text" id="txtDataEmissaoNFouCTEmitidoTomador" class="form-control maskedInput" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabsObservacoes">
                            <div class="row">
                                <div class="col-xs-12 col-sm-9 col-md-9 col-lg-10">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Observações Gerais">Obs</abbr>:
                                        </span>
                                        <textarea id="txtObservacaoGeral" class="form-control" rows="3"></textarea>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2" id="divBotoesObservacaoGeralCTe">
                                    <button type="button" id="btnBuscarObservacaoGeral" class="btn btn-primary" style="margin-bottom: 10px;">Buscar</button>
                                    <button type="button" id="btnCopiarObservacaoGeral" class="btn btn-default" style="margin-bottom: 10px;" data-toggle="tooltip" data-container="#divBotoesObservacaoGeralCTe" title="Utilize esta opção para copiar a observação geral para a informação adicional do fisco, nos serviços e impostos.">Copiar</button>
                                </div>
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Observações do Contribuinte
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Identificador">Ident.*</abbr>:
                                                </span>
                                                <input type="text" id="txtIdentificadorObservacaoContribuinte" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-9 col-md-9 col-lg-9">
                                            <div class="input-group">
                                                <span class="input-group-addon">Descrição*:
                                                </span>
                                                <input type="text" id="txtDescricaoObservacaoContribuinte" class="form-control" maxlength="160" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarObservacaoContribuinte" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarObservacaoContribuinte" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirObservacaoContribuinte" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarObservacaoContribuinte" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px;">
                                        <table id="tblObservacoesContribuinte" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 15%;">Identificador
                                                    </th>
                                                    <th style="width: 75%;">Descrição
                                                    </th>
                                                    <th style="width: 10%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado.
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Observações ao Fisco
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Identificador">Ident.*</abbr>:
                                                </span>
                                                <input type="text" id="txtIdentificadorObservacaoFisco" class="form-control" maxlength="20" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-9 col-md-9 col-lg-9">
                                            <div class="input-group">
                                                <span class="input-group-addon">Descrição*:
                                                </span>
                                                <input type="text" id="txtDescricaoObservacaoFisco" class="form-control" maxlength="160" />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnBuscarObservacaoFisco" class="btn btn-primary">Buscar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btnSalvarObservacaoFisco" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirObservacaoFisco" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarObservacaoFisco" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px;">
                                        <table id="tblObservacoesFisco" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 15%;">Identificador
                                                    </th>
                                                    <th style="width: 75%;">Descrição
                                                    </th>
                                                    <th style="width: 10%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="3">Nenhum registro encontrado.
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <span id="lblIdCTe" class="text-info" style="color: black; font-size: 10px; margin-left: 95%;"></span>
                        </div>
                        <div class="tab-pane" id="tabsCancelamento">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Número do Protocolo de Cancelamento/Inutilização">Protocolo</abbr>:
                                        </span>
                                        <input type="text" id="txtNumeroProtocoloCancelamento" class="form-control" disabled="disabled" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">Justificativa:
                                        </span>
                                        <input type="text" id="txtJustificativaCancelamento" class="form-control" disabled="disabled" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabsResumo">
                            <div id="divInformacaoServicosEImpostosResumo" class="row hidden" style="padding: 0 0 10px 15px;">
                                <button type="button" id="btnRecalcularFreteResumo" class="btn btn-xs btn-default">Recalcular Frete</button>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CFOP*:
                                        </span>
                                        <select id="selCFOPResumo" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Tomador*:
                                        </span>
                                        <select id="selTomadorServicoResumo" class="form-control">
                                            <option value="-1">Não Informado</option>
                                            <option value="0">0 - Remetente</option>
                                            <option value="1">1 - Expedidor</option>
                                            <option value="2">2 - Recebedor</option>
                                            <option value="3">3 - Destinatário</option>
                                            <option value="4">4 - Outros</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor do Frete">Vl. Frete*:</abbr>
                                        </span>
                                        <input type="text" id="txtValorFreteContratadoResumo" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor a Receber">A Receber*:</abbr>
                                        </span>
                                        <input type="text" id="txtValorAReceberResumo" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-2 col-md-2 col-lg-1" style="margin-top: -12px; margin-left: -10px;">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkIncluirICMSNoFreteResumo" />
                                            Incluir ICMS
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">ICMS*:
                                        </span>
                                        <select id="selICMSResumo" class="form-control">
                                            <option value="0">Selecione</option>
                                            <option value="1">00 - ICMS Normal</option>
                                            <option value="2">20 - ICMS com Redução de Base de Cálculo</option>
                                            <option value="3">40 - ICMS Isenção</option>
                                            <option value="4">41 - ICMS Não Tributado</option>
                                            <option value="5">51 - ICMS Diferido</option>
                                            <option value="6">60 - ICMS Pagto atr. ao tomador ou 3º previsto para ST</option>
                                            <option value="9">90 - ICMS Outras Situações</option>
                                            <option value="10">90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente</option>
                                            <option value="11">Simples Nacional</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorBaseCalculoICMSResumo">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Valor da Base de Cálculo do ICMS">Valor B.C.:</abbr>
                                        </span>
                                        <input type="text" id="txtValorBaseCalculoICMSResumo" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divAliquotaICMSResumo">
                                    <div class="input-group">
                                        <span class="input-group-addon">Alíquota:
                                        </span>
                                        <select id="selAliquotaICMSResumo" class="form-control">
                                            <option value="0">Selecione</option>
                                            <option value="7">7,00%</option>
                                            <option value="12">12,00%</option>
                                            <option value="17">17,00%</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3" id="divValorICMSResumo">
                                    <div class="input-group">
                                        <span class="input-group-addon">Valor:
                                        </span>
                                        <input type="text" id="txtValorICMSResumo" value="0,00" class="form-control maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Componentes dos Valores da Prestação de Serviços
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Descrição do Componente da Prestação">Descrição*:</abbr>
                                                </span>
                                                <input type="text" id="txtDescricaoComponentePrestacaoServicoResumo" class="form-control text-uppercase" maxlength="15" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Valor do Componente da Prestação">Valor*:</abbr>
                                                </span>
                                                <input type="text" id="txtValorComponentePrestacaoServicoResumo" class="form-control maskedInput" value="0,00" />
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                            <div class="checkbox" style="margin-top: -4px; margin-bottom: 0;">
                                                <label>
                                                    <input type="checkbox" id="chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMSResumo" checked="checked" />
                                                    Incluir Valor Na B.C. ICMS
                                                </label>
                                            </div>
                                            <div class="checkbox" style="margin-top: 0; margin-bottom: 0;">
                                                <label>
                                                    <input type="checkbox" id="chkIncluirValorComponentePrestacaoNoTotalAReceberResumo" checked="checked" />
                                                    Incluir Valor no Total a Receber
                                                </label>
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>
                                    </div>
                                    <button type="button" id="btnSalvarComponentePrestacaoServicoResumo" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirComponentePrestacaoServicoResumo" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarComponentePrestacaoServicoResumo" class="btn btn-default">Cancelar</button>
                                    <div class="table-responsive" style="margin-top: 10px; max-height: 115px; overflow-y: scroll;">
                                        <table id="tblComponentesPrestacaoServicoResumo" class="table table-bordered table-condensed table-hover">
                                            <thead>
                                                <tr>
                                                    <th style="width: 30%;">Descrição
                                                    </th>
                                                    <th style="width: 20%;">Valor
                                                    </th>
                                                    <th style="width: 20%;">Inclui B.C. ICMS
                                                    </th>
                                                    <th style="width: 20%;">Inclui Total Receber
                                                    </th>
                                                    <th style="width: 10%;">Opções
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="5">Nenhum registro encontrado!
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">Placa*:
                                        </span>
                                        <input type="text" id="txtPlacaVeiculoResumo" class="form-control maskedInput text-uppercase" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarVeiculoResumo" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <button type="button" id="btnAdicionarVeiculoResumo" class="btn btn-primary">Salvar</button>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="table-responsive" style="margin-left: 15px; margin-right: 15px;">
                                    <table id="tblVeiculosResumo" class="table table-bordered table-hover table-condensed">
                                        <thead>
                                            <tr>
                                                <th>Placa</th>
                                                <th>UF</th>
                                                <th>Renavam</th>
                                                <th>Tipo</th>
                                                <th>Rodado</th>
                                                <th>Carroc.</th>
                                                <th>Tipo</th>
                                                <th>Cap.(kg)</th>
                                                <th>Cap.(m³)</th>
                                                <th>Opções</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="10">Nenhum registro encontrado.</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">Motorista*:
                                        </span>
                                        <input type="text" id="txtNomeMotoristaResumo" class="form-control text-uppercase" maxlength="200" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF*:
                                        </span>
                                        <input type="text" id="txtCPFMotoristaResumo" class="form-control maskedInput" maxlength="20" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarMotoristaResumo" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-5 col-md-5 col-lg-5">
                                    <button type="button" id="btnSalvarMotoristaResumo" class="btn btn-primary">Salvar</button>
                                    <button type="button" id="btnExcluirMotoristaResumo" class="btn btn-danger" style="display: none;">Excluir</button>
                                    <button type="button" id="btnCancelarMotoristaResumo" class="btn btn-default">Cancelar</button>
                                </div>
                            </div>
                            <div class="row">
                                <div class="table-responsive" style="margin-left: 15px; margin-right: 15px;">
                                    <table id="tblMotoristasResumo" class="table table-bordered table-condensed table-hover">
                                        <thead>
                                            <tr>
                                                <th style="width: 60%;">Nome
                                                </th>
                                                <th style="width: 30%;">CPF
                                                </th>
                                                <th style="width: 10%;">Opções
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="3">Nenhum registro encontrado.
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-10 col-sm-10 col-md-10 col-lg-10">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Observações Gerais">Observações</abbr>:
                                        </span>
                                        <textarea id="txtObservacaoGeralResumo" class="form-control" rows="2"></textarea>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-2" id="divBotoesObservacaoGeralCTeResumo">
                                    <button type="button" id="btnLiberarEdicaoCTe" class="btn btn-default pull-right" style="margin-top: 10px;">Liberar edição</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row" style="margin-bottom: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <button type="button" id="btnTelaEmissaoAnterior" class="btn btn-primary pull-left"><span class="glyphicon glyphicon-chevron-left"></span>&nbsp;Anterior</button>
                            <button type="button" id="btnTelaEmissaoProximo" class="btn btn-primary pull-right">Próximo&nbsp;<span class="glyphicon glyphicon-chevron-right"></span></button>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <button type="button" id="btnConsultarColeta" class="btn btn-default pull-left"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar Coletas</button>
                            <button type="button" id="btnEmitirCTe" class="btn btn-primary" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-footer"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CT-e</button>
                            <button type="button" id="btnSalvarCTe" class="btn btn-primary" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-footer"><span class="glyphicon glyphicon-floppy-disk"></span>&nbsp;Salvar CT-e</button>
                            <button type="button" id="btnCancelarCTe" class="btn btn-default" data-toggle="tooltip" data-container="#divEmissaoCTe .modal-footer"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divObservacaoDigitacaoCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Observações para a Digitação do CT-e</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Observação:
                                </span>
                                <textarea id="txtObservacaoDigitacaoCTe" class="form-control" rows="4" maxlength="1000"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Digitação do CT-e</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDicasCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Dicas Para a Emissão do CT-e</h4>
                </div>
                <div class="modal-body">
                    <div id="divDica" style="width: 830px; overflow: auto; margin-right: 5px; margin-bottom: 5px; margin-left: 5px; max-height: 500px;"></div>
                    <div id="divArquivosDicas" style="display: none; margin-top: 15px;">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th style="width: 85%">Nome do arquivo</th>
                                    <th style="width: 15%">Opção</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnAbrirArquivosDica" style="display: none" class="btn btn-primary pull-left"><span id="spnIconeAbrirArquivosDicas" class="glyphicon glyphicon-folder-open"></span>&nbsp; <span id="spnAbrirArquivosDicas">Abrir arquivos</span></button>
                    <button type="button" id="btnAbrirDica" class="btn btn-primary"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Abrir em uma nova janela</button>
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Digitação do CT-e</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divContingenciaCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de CT-e em Contingência</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <div class="input-group-addon">
                                    Forma de Emissão*:
                                </div>
                                <select id="selFormaEmissaoCTe" class="form-control">
                                    <option value="7">Autorização pela SVC-RS</option>
                                    <option value="8">Autorização pela SVC-SP</option>
                                    <option value="5">Contingência FSDA</option>
                                    <option value="4">Contingência EPEC</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirCTeContingencia" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CT-e</button>
                    <button type="button" id="btnCancelarCTeContingencia" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDadosSimilaresCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Dados de um CT-e Semelhante Encontrados</h4>
                </div>
                <div class="modal-body">
                    <div id="infoCTeSemelhante" class="row" style="padding-left: 10px;">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" id="chkImportarObservacaoGeral" checked="checked" />
                                    Importar Também a Observação Geral
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnImportarInfosSemelhantesDoCTe" class="btn btn-primary">Sim</button>
                    <button type="button" id="btnNaoImportarInfosSemelhantesDoCTe" class="btn btn-default">Não</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divColetaCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Coletas</h4>
                </div>
                <div class="modal-body">
                    <div class="well well-sm">
                        <p>
                            <strong>A coleta selecionada será finalizada ao salvar/emitir o CT-e.</strong>
                        </p>
                        <p>
                            <strong>Origem: </strong>
                            <span id="spnOrigemColeta"></span>
                        </p>
                        <p>
                            <strong>Destino: </strong>
                            <span id="spnDestinoColeta"></span>
                        </p>
                        <p id="divInfoVeiculosColeta">
                            <strong>Veículos:</strong>
                            <span id="spnVeiculosColeta"></span>
                        </p>
                    </div>
                    <div class="table-responsive">
                        <table id="tbl_coletas" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 15%;">Nº
                                    </th>
                                    <th style="width: 22%;">Origem
                                    </th>
                                    <th style="width: 22%;">Destino
                                    </th>
                                    <th style="width: 15%;">Vl. Notas
                                    </th>
                                    <th style="width: 15%;">Peso (kg)
                                    </th>
                                    <th>Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnFecharColeta" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Digitação do CT-e</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divAverbacaoCTe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloAverbacaoCTe" class="modal-title"></h4>
                </div>
                <div class="modal-body" style="min-height: 480px;">
                    <div id="placeholder-msgAverbacoes"></div>
                    <button type="button" id="btnConsultarAverbacoes" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_averbacao" class="table-responsive">
                    </div>
                    <div id="tbl_paginacao_averbacao">
                    </div>
                </div>
                <div class="clearfix">
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnReenviarAverbacao" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Reenviar Averbação</button>
                    <button type="button" id="btnFecharAverbacoes" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Digitação do CT-e</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divSelecionarPlacaVinculada" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">A placa <span id="spanPlacaFilho"></span>possui mais de para acoplamento, selecione a correta:</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderManifesto">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Placa:
                                </span>
                                <select id="selSelecionarPlacaVinculada" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnConfirmarPlaca" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Confirmar Placa</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divCaptchaReceita" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta CNPJ Receita</h4>
                </div>
                <div class="modal-body">
                    <div id="divCaptchaReceitaBody" class="row" style="padding: 15px;">
                        <div class="smart-form">
                            <section>
                                <div class="well">
                                    <div class="row">
                                        <div style="float: left; margin-top: 6px; margin-left: 100px;">
                                            <img src="" id="imgCaptchaReceita" style="float: left; margin-top: 6px; border: 1px solid #CCC; width: 260px; height: 80px;" /><a href="javascript:;void(0)" style="float: left; margin-top: 25px; margin-left: 5px;"><i class="fa fa-refresh"></i></a>
                                        </div>
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group" style="margin-top: 40px;">
                                                <span class="input-group-addon" style="margin-top: 6px">Captcha:
                                                </span>
                                                <input type="text" id="txtCaptchaReceita" class="form-control" autofocus />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnCaptchaReceita" class="btn btn-primary"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnAtualizarCaptchaReceita" class="btn btn-link" style="margin-left: 135px"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Gerar novo Captcha</button>
                                        </span>
                                    </div>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divCancelamentoMDFe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cancelamento de MDF-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgCancelamentoMDFe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataCancelamento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaCancelamentoMDFe" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row" id="divOpcoesCobrancaCancelamentoMDFe" style="display: none">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Cobrar:</span>
                                <select id="selCobrarCancelamentoMDFe" class="form-control">
                                    <option value="">Selecione</option>
                                    <option value="Sim">Sim</option>
                                    <option value="Nao">Não</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCancelamentoMDFe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar o MDF-e</button>
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divIntegracaoRetorno" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloIntegracaoRetorno" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderIntegracaoRetorno"></div>
                    <button type="button" id="btnConsultarIntegracaoRetorno" class="btn btn-default" style="margin-bottom: 10px;"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
                    <div id="tbl_IntegracaoRetorno" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_IntegracaoRetorno">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharIntegracaoRetorno" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do CT-e</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divIntegracaoRetornoLog" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloIntegracaoRetornoLog" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-placeholderIntegracaoRetornoLog"></div>
                    <div id="tbl_IntegracaoRetornoLog" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_paginacao_IntegracaoRetornoLog">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnFecharIntegracaoRetornoLog" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
