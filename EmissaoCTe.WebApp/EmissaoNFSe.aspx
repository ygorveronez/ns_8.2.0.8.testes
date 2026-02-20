<%@ Page Title="Emissão de NFS-e" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmissaoNFSe.aspx.cs" Inherits="EmissaoCTe.WebApp.EmissaoNFSe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                          "~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload") %>
    </asp:PlaceHolder>
    <style type="text/css">
        @media screen and (min-width: 1024px) {
            #divEmissaoNFSe .modal-dialog {
                right: auto;
                width: 1000px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoNFSe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media screen and (min-width: 1200px) {
            #divEmissaoNFSe .modal-dialog {
                right: auto;
                width: 1180px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoNFSe .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        #lblLogNFSe {
            margin-top: 20px;
            display: block;
        }
    </style>
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
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
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/emissaoNFSe") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddTomadorFiltro" value="" />
    </div>
    <div class="page-header">
        <h2>Emissão de NFS-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovaNFSe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Nova NFS-e</button>
    <span runat="server" id="spnImportarCSV">
        <button type="button" id="btnImportarCSV" class="btn btn-default"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;Importar CSV</button>
    </span>
    <span runat="server" id="spnImportarNFeSalvas">
        <button type="button" id="btnImportarNFeSalvas" class="btn btn-default"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;Importar NFe por Período</button>
    </span>
    <button type="button" id="btnImportarXMLNFSe" class="btn btn-default"><span class="glyphicon glyphicon-list"></span>&nbsp;Importar XML NFS-e Autorizada</button>
    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicialNFSeFiltro" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinalNFSeFiltro" class="form-control maskedInput" />
            </div>
        </div>
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Inicial:
                                </span>
                                <input type="text" id="txtNumeroInicialNFSeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. Final:
                                </span>
                                <input type="text" id="txtNumeroFinalNFSeFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. RPS:
                                </span>
                                <input type="text" id="txtNumeroRPSFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Número dos documentos anexados a Nota de Serviço">Núm. Doc.</abbr>:
                                </span>
                                <input type="text" id="txtNumeroDocumentoFiltro" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Série:
                                </span>
                                <select id="selSerieNFSeFiltro" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusNFSeFiltro" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="3">Autorizado</option>
                                    <option value="5">Cancelado</option>
                                    <option value="4">Em Cancelamento</option>
                                    <option value="0">Em Digitação</option>
                                    <option value="2">Enviado</option>
                                    <option value="1">Pendente</option>
                                    <option value="9">Rejeição</option>
                                    <option value="12">Inutilizada</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Tomador:
                                </span>
                                <input type="text" id="txtTomadorNFSeFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTomadorNFSeFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarNFSe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar NFS-e</button>
    <button type="button" id="btnEmitirTodasNFSes" class="btn btn-default" style="display: none"><span class="glyphicon glyphicon-paste"></span>&nbsp;Emitir todas NFS-es</button>
    <div id="tbl_nfses" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_nfses">
    </div>
    <div class="modal fade" id="divCancelamentoNFSe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cancelamento de NFS-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgCancelamentoNFSe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaCancelamentoNFSe" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCancelamentoNFSe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar a NFS-e</button>
                    <button type="button" id="btnCancelarCancelamentoNFSe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divAverbacaoNFSe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="tituloAverbacaoNFSe" class="modal-title"></h4>
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
                    <button type="button" id="btnReenviarAverbacao" class="btn btn-primary"><span class="glyphicon glyphicon-ok" visible="true"></span>&nbsp;Reenviar Averbação</button>
                    <button type="button" id="btnFecharAverbacoes" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão do NFS-e</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCancelamentoNFSePrefeitura" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Informar Cancelamento/Inutilização feito na Prefeitura</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgCancelamentoNFSePrefeitura"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Justificativa:
                                </span>
                                <input type="text" id="txtJustificativaCancelamentoNFSePrefeitura" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCancelamentoNFSePrefeitura" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar/Inutilizar a NFS-e</button>
                    <button type="button" id="btnCancelarCancelamentoNFSePrefeitura" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoNFSe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" aria-hidden="true" id="btnFecharEmissaoNFSe">&times;</button>
                    <button type="button" id="btnAbrirDicaEmissaoNFSe" class="close" aria-hidden="true" style="font-size: 15px; font-weight: bolder; margin-top: 1px; margin-right: 5px;">&#63;</button>
                    <h4 class="modal-title">Emissão de NFS-e</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEmissaoNFSe"></div>
                    <ul class="nav nav-tabs" id="tabsEmissaoNFSe">
                        <li class="active"><a href="#tabGeral" data-toggle="tab">Geral</a></li>
                        <li><a href="#tabTomador" tabindex="-1" data-toggle="tab">Tomador</a></li>
                        <li><a href="#tabDocumentos" tabindex="-1" data-toggle="tab">Documentos</a></li>
                        <li><a href="#tabIntermediario" tabindex="-1" data-toggle="tab">Intermediário</a></li>
                        <li><a href="#tabServicos" tabindex="-1" data-toggle="tab">Serviços</a></li>
                        <li><a href="#tabSubstituicao" tabindex="-1" data-toggle="tab">Substituição</a></li>
                        <li><a href="#tabValores" tabindex="-1" data-toggle="tab">Valores</a></li>
                    </ul>
                    <div class="tab-content" style="margin-top: 10px;">
                        <div class="tab-pane active" id="tabGeral">
                            <div class="row">
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número*:
                                        </span>
                                        <input type="text" id="txtNumero" value="Automático" class="form-control" disabled />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série:
                                        </span>
                                        <select id="selSerie" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Emissão*:
                                        </span>
                                        <input type="text" id="txtDataEmissao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Hora Emissão*:
                                        </span>
                                        <input type="text" id="txtHoraEmissao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">RPS:
                                        </span>
                                        <input type="text" id="txtNumeroRPS" value="Automático" class="form-control" disabled />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Natureza*:
                                        </span>
                                        <select id="selNatureza" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado Inc.*:
                                        </span>
                                        <select id="selEstadoPrestacaoServico" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Loc. Serviço*:
                                        </span>
                                        <select id="selLocalidadePrestacaoServico" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Outras Informações">Out. Info.</abbr>*:
                                        </span>
                                        <textarea id="txtOutrasInformacoes" class="form-control" rows="3"></textarea>
                                    </div>
                                </div>
                            </div>
                            <span id="lblLogNFSe" class="text-info"></span>
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
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCPFCNPJTomador">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ*:
                                        </span>
                                        <input type="text" id="txtCPFCNPJTomador" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldNumeroDocumentoTomador">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nº Documento*:
                                        </span>
                                        <input type="text" id="txtNumeroDocumentoExteriorTomador" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <input type="text" id="txtRGIETomador" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">IM:
                                        </span>
                                        <input type="text" id="txtIMTomador" maxlength="20" class="form-control" />
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
                                        <select id="selPaisTomador" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <select id="selEstadoTomador" class="form-control">
                                        </select>
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
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
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
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
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
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkSalvarEnderecoTomador" checked="checked" data-toggle="tooltip" data-container="#divEmissaoNFSe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para esta NFS-e." />
                                            Salvar Endereço no Tomador
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabDocumentos">
                            <div id="messages-placeholder-documento"></div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-8">
                                    <div class="input-group">
                                        <span class="input-group-addon">Chave:
                                        </span>
                                        <input type="text" id="txtDocumentoChave" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número:
                                        </span>
                                        <input type="text" id="txtDocumentoNumero" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série:
                                        </span>
                                        <input type="text" id="txtDocumentoSerie" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Data Emissão:
                                        </span>
                                        <input type="text" id="txtDocumentoDataEmissao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Valor:
                                        </span>
                                        <input type="text" id="txtDocumentoValor" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Peso (Kg): 
                                        </span>
                                        <input type="text" id="txtDocumentoPeso" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <button type="button" id="btnSalvarDocumento" class="btn btn-primary">Salvar</button>
                            <button type="button" id="btnCancelarDocumento" class="btn btn-default">Cancelar</button>
                            <button type="button" id="btnExcluirDocumento" class="btn btn-danger" style="display: none;">Excluir</button>

                            <div class="table-responsive" style="margin-top: 15px; max-height: 400px; overflow-y: scroll;">
                                <table id="tblDocumentos" class="table table-bordered table-condensed table-hover">
                                    <thead>
                                        <tr>
                                            <th style="width: 12%">Número</th>
                                            <th style="width: 30%">Chave</th>
                                            <th style="width: 12%">Série</th>
                                            <th style="width: 12%">Data da Emissão</th>
                                            <th style="width: 12%">Valor</th>
                                            <th style="width: 12%">Peso</th>
                                            <th style="width: 10%">Opções</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabIntermediario">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" id="chkIntermediarioExportacao" />
                                                Cliente Exportação
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCPFCNPJIntermediario">
                                    <div class="input-group">
                                        <span class="input-group-addon">CPF/CNPJ*:
                                        </span>
                                        <input type="text" id="txtCPFCNPJIntermediario" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarIntermediario" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldNumeroDocumentoIntermediario">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nº Documento*:
                                        </span>
                                        <input type="text" id="txtNumeroDocumentoExteriorIntermediario" class="form-control" maxlength="20" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">IE:
                                        </span>
                                        <input type="text" id="txtRGIEIntermediario" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">IM:
                                        </span>
                                        <input type="text" id="txtIMIntermediario" maxlength="20" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Razão Social*:
                                        </span>
                                        <input type="text" id="txtRazaoSocialIntermediario" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Nome Fantasia:
                                        </span>
                                        <input type="text" id="txtNomeFantasiaIntermediario" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 1*:
                                        </span>
                                        <input type="text" id="txtTelefone1Intermediario" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Fone 2:
                                        </span>
                                        <input type="text" id="txtTelefone2Intermediario" maxlength="15" class="form-control maskedInput phone" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Atividade*:
                                        </span>
                                        <input type="text" id="txtAtividadeIntermediario" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarAtividadeIntermediario" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Endereço*:
                                        </span>
                                        <input type="text" id="txtEnderecoIntermediario" maxlength="80" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número*:
                                        </span>
                                        <input type="text" id="txtNumeroIntermediario" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Bairro*:
                                        </span>
                                        <input type="text" id="txtBairroIntermediario" maxlength="40" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Complemento:
                                        </span>
                                        <input type="text" id="txtComplementoIntermediario" maxlength="60" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
                                    <div class="input-group">
                                        <span class="input-group-addon">CEP*:
                                        </span>
                                        <input type="text" id="txtCEPIntermediario" class="form-control maskedInput" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">País*:
                                        </span>
                                        <select id="selPaisIntermediario" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <select id="selEstadoIntermediario" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" id="divFieldCidadeIntermediario">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <select id="selCidadeIntermediario" class="form-control">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6" style="display: none;" id="divFieldCidadeExportacaoIntermediario">
                                    <div class="input-group">
                                        <span class="input-group-addon">Cidade*:
                                        </span>
                                        <input type="text" id="txtCidadeIntermediarioExportacao" class="form-control" maxlength="60" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Gerais">@ Gerais</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsIntermediario" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsIntermediario" />
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contato">@ Contato</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContatoIntermediario" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContatoIntermediario" />
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="E-mails Contador">@ Contador</abbr>:
                                        </span>
                                        <input type="text" id="txtEmailsContadorIntermediario" class="form-control" maxlength="1000" />
                                        <span class="input-group-addon">
                                            <input type="checkbox" id="chkStatusEmailsContadorIntermediario" />
                                            <abbr title="Enviar o XML Automaticamente">XML</abbr>
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkSalvarEnderecoIntermediario" checked="checked" data-toggle="tooltip" data-container="#divEmissaoNFSe .modal-body" title="Ao desmarcar esta opção o endereço será utilizado somente para esta NFS-e." />
                                            Salvar Endereço no Intermediario
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabServicos">
                            <div class="row">

                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">Serviço*:
                                        </span>
                                        <input type="text" id="txtServico" readonly="readonly" class="form-control" />
                                        <span class="input-group-btn">
                                            <button type="button" id="btnBuscarServico" class="btn btn-primary">Buscar</button>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado*:
                                        </span>
                                        <select id="selEstadoItem" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Localidade*:
                                        </span>
                                        <select id="selLocalidadeItem" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Estado Inc.*:
                                        </span>
                                        <select id="selEstadoIncidenciaItem" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">Localidade Inc.*:
                                        </span>
                                        <select id="selLocalidadeIncidenciaItem" class="form-control"></select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Serviço prestado no país?">Prest. no País*</abbr>:
                                        </span>
                                        <select id="selServicoPrestadoPais" class="form-control">
                                            <option value="true">Sim</option>
                                            <option value="false">Não</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon">País*:
                                        </span>
                                        <select id="selPaisItem" class="form-control"></select>
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Serviço*:
                                        </span>
                                        <input type="text" id="txtValorServicoItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Quantidade*:
                                        </span>
                                        <input type="text" id="txtQuantidadeItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Desc. Inc.*:
                                        </span>
                                        <input type="text" id="txtValorDescontoIncondicionadoItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Desc. Cond.*:
                                        </span>
                                        <input type="text" id="txtValorDescontoCondicionadoItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Deduções*:
                                        </span>
                                        <input type="text" id="txtValorDeducoesItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Total*:
                                        </span>
                                        <input type="text" id="txtValorTotalItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">B. C. ISS*:
                                        </span>
                                        <input type="text" id="txtBaseCalculoISSItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Aliquota ISS*:
                                        </span>
                                        <input type="text" id="txtAliquotaISSItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Valor ISS*:
                                        </span>
                                        <input type="text" id="txtValorISSItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">NBS:</span>
                                        <input type="text" id="txtNBSItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Indicador Op.:</span>
                                        <input type="text" id="txtCodigoIndicadorOperacaoItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">CST IBS/CBS:</span>
                                        <input type="text" id="txtCstibscbsItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Class. Trib.:</span>
                                        <input type="text" id="txtClassificacaoTributariaIBSCBSItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">BC IBS/CBS:</span>
                                        <input type="text" id="txtBaseCalculoIBSCBSItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Aliq. IBS Est.:</span>
                                        <input type="text" id="txtAliquotaIBSEstadualItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Red. IBS Est.:</span>
                                        <input type="text" id="txtPercentualReducaoIBSEstadualItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. IBS Est.:</span>
                                        <input type="text" id="txtValorIBSEstadualItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Aliq. IBS Mun.:</span>
                                        <input type="text" id="txtAliquotaIBSMunicipalItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Red. IBS Mun.:</span>
                                        <input type="text" id="txtPercentualReducaoIBSMunicipalItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. IBS Mun.:</span>
                                        <input type="text" id="txtValorIBSMunicipalItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Aliq. CBS:</span>
                                        <input type="text" id="txtAliquotaCBSItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Red. CBS:</span>
                                        <input type="text" id="txtPercentualReducaoCBSItem" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. CBS:</span>
                                        <input type="text" id="txtValorCBSItem" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Exigibilidade de ISS">Exig. ISS*</abbr>:
                                        </span>
                                        <select id="selExigibilidadeISSItem" class="form-control">
                                            <option value="1">Exigível</option>
                                            <option value="2">Não Incidência</option>
                                            <option value="3">Isenção</option>
                                            <option value="4">Exportação</option>
                                            <option value="5">Imunidade</option>
                                            <option value="6">Suspensa por Decisão Judicial</option>
                                            <option value="7">Suspensa por Processo Administrativo</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3 col-md-3 col-lg-3">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox" id="chkIncluirISSNoFrete" />
                                            Incluir ISS no Frete
                                        </label>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <abbr title="Discriminação">Disc.</abbr>*:
                                        </span>
                                        <textarea id="txtDiscriminacaoItem" class="form-control" rows="3"></textarea>
                                    </div>
                                </div>


                            </div>
                            <button type="button" id="btnSalvarItem" class="btn btn-primary">Salvar</button>
                            <button type="button" id="btnExcluirItem" class="btn btn-danger" style="display: none;">Excluir</button>
                            <button type="button" id="btnCancelarItem" class="btn btn-default">Cancelar</button>
                            <div class="table-responsive" style="margin-top: 10px;">
                                <table id="tblItens" class="table table-bordered table-condensed table-hover">
                                    <thead>
                                        <tr>
                                            <th style="width: 35%;">Serviço
                                            </th>
                                            <th style="width: 15%;">Quantidade
                                            </th>
                                            <th style="width: 15%;">Valor Total
                                            </th>
                                            <th style="width: 15%;">Valor ISS
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
                        <div class="tab-pane" id="tabSubstituicao">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Número:
                                        </span>
                                        <input type="text" id="txtNumeroSubstituicao" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Série:
                                        </span>
                                        <input type="text" id="txtSerieSubstituicao" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabValores">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Serviços:
                                        </span>
                                        <input type="text" id="txtValorServicos" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Deduções:
                                        </span>
                                        <input type="text" id="txtValorDeducoes" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. PIS:
                                        </span>
                                        <input type="text" id="txtValorPIS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. COFINS:
                                        </span>
                                        <input type="text" id="txtValorCOFINS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. INSS:
                                        </span>
                                        <input type="text" id="txtValorINSS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. IR:
                                        </span>
                                        <input type="text" id="txtValorIR" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. CSLL:
                                        </span>
                                        <input type="text" id="txtValorCSLL" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">ISS Ret.:
                                        </span>
                                        <select id="selISSRetido" class="form-control">
                                            <option value="false">Não</option>
                                            <option value="true">Sim</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. ISS Ret.:
                                        </span>
                                        <input type="text" id="txtValorISSRetido" class="form-control" />
                                    </div>
                                </div>
                                <div class="clearfix"></div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Out. Ret.:
                                        </span>
                                        <input type="text" id="txtValorOutrasRetencoes" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Desc. Inc.:
                                        </span>
                                        <input type="text" id="txtValorDescontoIncondicionado" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. Desc. Cond.:
                                        </span>
                                        <input type="text" id="txtValorDescontoCondicionado" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Aliq. ISS:
                                        </span>
                                        <input type="text" id="txtAliquotaISS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">B. C. ISS:
                                        </span>
                                        <input type="text" id="txtBaseCalculoISS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. ISS:
                                        </span>
                                        <input type="text" id="txtValorISS" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. IBS Est.:</span>
                                        <input type="text" id="txtValorIBSEstadual" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. IBS Mun.:</span>
                                        <input type="text" id="txtValorIBSMunicipal" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Vl. CBS:</span>
                                        <input type="text" id="txtValorCBS" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <span id="lblLogNFSe" class="text-info"></span>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirNFSe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir NFS-e</button>
                    <button type="button" id="btnSalvarNFSe" class="btn btn-primary"><span class="glyphicon glyphicon-floppy-disk"></span>&nbsp;Salvar NFS-e</button>
                    <button type="button" id="btnCancelarNFSe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divDicasNFSe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Dicas Para a Emissão</h4>
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
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Digitação</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divNotasImportadas" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão por NF-es Importadas</h4>
                </div>
                <div class="modal-body">
                    <ul class="nav nav-tabs">
                        <li><a href="#tabNotasImportadas" data-toggle="tab">Seleção NF-es</a></li>
                    </ul>
                    <div class="tab-content" style="margin-top: 10px;">
                        <div class="tab-pane active" id="tabNotasImportadas">
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
                                <div class="col-xs-12 col-sm-6 col-md-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">Sit. Emissão:
                                        </span>
                                        <select id="selDDNotasSemNFSe" class="form-control">
                                            <option value="true" selected="selected">Pendentes</option>
                                            <option value="false">Emitidos</option>
                                            <option value="">Todos</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon">CNPJ Emi.:
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
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <div class="input-group">
                                        <span class="input-group-addon">Chave:
                                        </span>
                                        <input type="text" id="txtDDChave" class="form-control maskedInput" autocomplete="off">
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4" style="float: right;">
                                    <div class="input-group">
                                        <span class="input-group-addon">Notas selecionadas:
                                        </span>
                                        <input type="text" id="txtDDNotasSelecionadas" class="form-control maskedInput" autocomplete="off" disabled>
                                    </div>
                                </div>
                            </div>

                            <div class="clearfix" style="margin-bottom: 5px">
                                <button type="button" id="btnConsultarNotasImportadas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Filtrar</button>
                                <button type="button" id="btnSelecionarTodosNotasImportadas" class="btn btn-default" style="float: right">Selecionar Todos</button>
                                <button type="button" id="btnLimparSelecaoNotasImportadas" class="btn btn-default" style="float: right">Limpar Seleção</button>
                            </div>

                            <div class="">
                                <div id="tbl_NotasImportadas" class="table-responsive">
                                </div>
                                <div id="tbl_paginacao_NotasImportadas">
                                </div>
                                <div class="clearfix"></div>
                            </div>

                            <div class="divNotasImportadasSelecionados">
                                <div class="tfs-tags">
                                    <div class="tags-items-container">
                                        <ul id="containerNotasImportadasSelecionados">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default" style="float: left;">&nbsp;Cancelar</button>
                    <button type="button" id="btnGerarNFSeNotasImportadas" class="btn btn-primary" style="float: left;">&nbsp;Gerar NFS-e</button>
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
                        <button type="button" id="btnFecharIntegracaoRetorno" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à emissão de NFS-e</button>
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
