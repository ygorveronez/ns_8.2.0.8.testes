<%@ Page Title="Emissão de CIOT" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoSigaFacil.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoSigaFacil" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload")%>
    </asp:PlaceHolder>
    <style type="text/css">
        .integrador-abertura {
            display: none;
        }

        @media screen and (min-width: 1024px) {
            #divEmissaoCIOT .modal-dialog {
                right: auto;
                width: 1000px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoCIOT .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }

        @media screen and (min-width: 1200px) {
            #divEmissaoCIOT .modal-dialog {
                right: auto;
                width: 1180px;
                padding-top: 30px;
                padding-bottom: 30px;
            }

            #divEmissaoCIOT .modal-content {
                -webkit-box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
                box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5);
            }
        }
        #divMsgValorAdiantamento {
            padding: 5px 10px 5px 10px;
            margin-top: -7px;
            margin-bottom: 0;
            border-top-left-radius: 0px;
            border-top-right-radius: 0px;
            font-size: 12px;
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/integracaoSigaFacil") %>
    </asp:PlaceHolder>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Emissão de CIOT
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovoCIOT" style="display: none" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Novo CIOT</button>
    <button type="button" id="btnAbrirCIOT" style="display: none" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Abrir CIOT</button>
    <button type="button" id="btnImportarCTe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Importar XML CTe</button>
    <div class="row" style="margin-top: 5px;">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Placa:
                </span>
                <input type="text" id="txtFiltroPlaca" class="form-control text-uppercase maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CIOT:
                </span>
                <input type="text" id="txtFiltroCIOT" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selFiltroStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="1">Autorizado</option>
                    <option value="2">Encerrado</option>
                    <option value="3">Cancelado</option>
                    <option value="4">Aberto</option>
                    <option value="9">Rejeitado</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridCIOT" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar CIOT</button>
    <div id="tbl_ciot" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_ciot">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divIntegracoesCIOT" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Integrações do CIOT</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-integracaoCIOT"></div>
                    <div id="tbl_integracao" style="margin-top: 10px;"></div>
                    <div id="tbl_paginacao_integracao"></div>
                    <div class="clearfix"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divCancelamentoCIOT" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="tituloCancelamento">Cancelamento de CIOT</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-cancelamentoCIOT">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Motivo*:
                                </span>
                                <input type="text" id="txtMotivoCancelamentoCIOT" class="form-control" maxlength="200" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCancelamentoCIOT" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Cancelar CIOT</button>
                    <button type="button" id="btnCancelarCancelamentoCIOT" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEmissaoCIOT" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de CIOT</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-emissaoCIOT">
                    </div>
                    <div role="tabpanel">
                        <ul class="nav nav-tabs" style="margin-bottom: 10px;">

                            <li class="active"><a href="#divDadosGerais" role="tab" data-toggle="tab">Dados Gerais</a></li>
                            <li><a href="#divCidadesPedagio" role="tab" data-toggle="tab">Cidades Pedágio</a></li>
                            <li><a href="#divCTes" role="tab" data-toggle="tab">CT-es</a></li>
                            <li><a href="#divEncerramento" role="tab" data-toggle="tab">Encerramento</a></li>
                            <li><a href="#divJustificativa" role="tab" data-toggle="tab">Justificativa</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="divDadosGerais">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-2 col-md-2 col-lg-2">
                                        <div class="input-group">
                                            <span class="input-group-addon">Nº Viagem:
                                            </span>
                                            <input type="text" id="txtNumero" class="form-control" value="Automático" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Nº CIOT:
                                            </span>
                                            <input type="text" id="txtNumeroCIOT" class="form-control" value="Automático" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-2 col-md-2 col-lg-2">
                                        <div class="input-group">                            
                                            <span class="input-group-addon">
                                                <abbr title="Código de verificação do CIOT">Cod. Verif.:</abbr>*:
                                            </span>
                                            <input type="text" id="txtCodigoVerificadorCIOT" class="form-control" value="Automático" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Dt. Emissão*:
                                            </span>
                                            <input type="text" id="txtDataEmissao" class="form-control" value="Automático" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Data de Início da Viagem">Dt. Início</abbr>*:
                                            </span>
                                            <input type="text" id="txtDataInicioViagem" class="form-control maskedInput" value="" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Data de Término da Viagem">Dt. Fim</abbr>*:
                                            </span>
                                            <input type="text" id="txtDataFimViagem" class="form-control maskedInput" value="" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Transportador*:
                                            </span>
                                            <input type="text" id="txtTransportador" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarTransportador" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Motorista*:
                                            </span>
                                            <input type="text" id="txtMotorista" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
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
                                    <div class="integrador-padrao">
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
                                                <span class="input-group-addon">Loc. Origem*:
                                                </span>
                                                <select id="selLocalidadeOrigem" class="form-control">
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
                                                <span class="input-group-addon">Loc. Destino*:
                                                </span>
                                                <select id="selLocalidadeDestino" class="form-control">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo Favorecido Pagamento">Tipo Favorecido</abbr>*:
                                                </span>
                                                <select id="selTipoFavorecido" class="form-control">
                                                    <option value="3" selected="selected">Motorista</option>
                                                    <option value="2">SubContratante</option>
                                                    <option value="1">Contratado</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Categoria do Transportador na ANTT">Cat. Transp.</abbr>*:
                                                </span>
                                                <select id="selCategoriaTransportadorANTT" class="form-control">
                                                    <option value="0" selected="selected">Não TAC</option>
                                                    <option value="1">TAC</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Regra de local de pagamento do frete - Adiantamento">Adiantamento</abbr>*:
                                                </span>
                                                <select id="selRegraQuitacaoAdiantamento" class="form-control">
                                                    <option value="1" selected="selected">Troca em posto</option>
                                                    <option value="2">Troca em filial</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Regra de local de pagamento do frete - Quitação">Quitação</abbr>*:
                                                </span>
                                                <select id="selRegraQuitacaoQuitacao" class="form-control">
                                                    <option value="1" selected="selected">Troca em posto</option>
                                                    <option value="2">Troca em filial</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Tipo de viagem (medida da carga)">Tipo Viagem</abbr>*:
                                                </span>
                                                <select id="selTipoViagem" class="form-control">
                                                    <option value="0" selected="selected">Por Peso</option>
                                                    <option value="1">Por Unidade</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                            <div class="input-group">
                                                <span class="input-group-addon">
                                                    <abbr title="Documentos obrigatórios que serão solicitados no Estabelecimento Trocador de Frete (posto ou filial)">Doc. Obr.</abbr>*:
                                                </span>
                                                <select id="selDocumentosObrigatorios" class="form-control">
                                                    <option value="5" selected="selected">CT-e</option>
                                                    <option value="6">CT-e e Ticket Balança</option>
                                                    <option value="7">CT-e e Canhoto da NF-e</option>
                                                    <option value="8">CT-e, Ticket Balança e Canhoto da NF-e</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-12 col-lg-12">
                                        <div class="input-group">
                                            <span class="input-group-addon">Nat. Carga*:
                                            </span>
                                            <select id="selNaturezaCarga" class="form-control">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4 integrador-padrao">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Pedágio Ida e Volta">Pedágio Ida e Volta</abbr>:
                                            </span>
                                            <select id="selPedagioIdaVolta" class="form-control">
                                                <option value="0" selected="selected">Não</option>
                                                <option value="1">Sim</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4 pamcard-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Estimado Total da prestação">Vl Estimado</abbr>*:</span>
                                            <input type="text" id="txtValorEstimado" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4 pamcard-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Adiantamento na Abertura do CIOT">Vl Adiantamento</abbr>*:</span>
                                            <input type="text" id="txtValorAdiantamentoAbertura" class="form-control" />
                                        </div>
                                        <div class="alert alert-danger" id="divMsgValorAdiantamento">Após abertura, esse valor não poderá ser alterado ou cancelado.</div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="divCTes">
                                <div class="panel panel-default">
                                    <div class="panel-heading">CT-es do CIOT</div>
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="integrador-abertura">
                                                <div class="col-xs-12 col-sm-12">
                                                    <button type="button" id="btnBuscarMultiCTe" class="btn btn-primary">Vincular CT-es</button>
                                                </div>
                                            </div>
                                            <div class="integrador-padrao">
                                                <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">CT-e*:
                                                        </span>
                                                        <input type="text" id="txtCTe" class="form-control" />
                                                        <span class="input-group-btn">
                                                            <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                                                        </span>
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Quantidade:
                                                        </span>
                                                        <input type="text" id="txtQuantidade" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Espécie:
                                                        </span>
                                                        <input type="text" id="txtEspecie" class="form-control" maxlength="2" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Tipo Peso*:
                                                        </span>
                                                        <select id="selTipoPeso" class="form-control">
                                                            <option value="0">Peso Carregado</option>
                                                            <option value="1">Peso Lotação</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Peso Bruto*:
                                                        </span>
                                                        <input type="text" id="txtPesoBruto" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Peso Lotação*:
                                                        </span>
                                                        <input type="text" id="txtPesoLotacao" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Mercadoria por KG">Vl. Merc. KG</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorMercadoriaKG" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor Total da Mercadoria Carregada">Vl. Merc.</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorTotalMercadoria" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor utilizado para recálculo do frete na chegada, por quilo de mercadoria entregue.">Vl. Tar. Frete</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorTarifaFrete" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor do Frete Negociado na Saída (deve ser igual a 'Vl. Tar. Frete' x 'Peso Bruto')">Vl. Frete</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorFrete" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Define se na quitação há recalculo de frete pelo peso de chegada, se houver diferença.">Recalcular Frete</abbr>*:
                                                        </span>
                                                        <select id="selCobraDiferencaFrete" class="form-control">
                                                            <option value="1" selected="selected">Cobra Diferença</option>
                                                            <option value="2">Não Cobra Diferença</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Na troca do frete (quitação) será exigido no POS peso de chegada conforme ticket de balança.">Ex. Peso Cheg.</abbr>*:
                                                        </span>
                                                        <select id="selExigePesoChegada" class="form-control">
                                                            <option value="0" selected="selected">Não</option>
                                                            <option value="1">Sim</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Define como será efetuado o cálculo de quebra, se houver.">Tipo Quebra</abbr>*:
                                                        </span>
                                                        <select id="selTipoQuebra" class="form-control">
                                                            <option value="1">Integral (cobra valor correspondente a toda a mercadoria faltante)</option>
                                                            <option value="2">Parcial (cobra apenas o valor da mercadoria faltante que ultrapassar a tolerância)</option>
                                                            <option value="3" selected="selected">Sem Quebra (não cobra mercadoria faltante)</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Define como será a tolerância para diferença de peso na entrega (a menor ou a maior).">Tp. Tolerân.</abbr>*:
                                                        </span>
                                                        <select id="selTipoTolerancia" class="form-control">
                                                            <option value="1">Percentual (diferença de peso permitida é um percentual sobre o peso de saída)</option>
                                                            <option value="2">Peso (diferença de peso permitida é valor fixo em quilos)</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Tolerância para cálculo de Quebra (perda de quilos de mercadoria no trajeto).">Tolerância</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtTolerancia" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Tolerância para entrega acima do peso de saída.">Tolerância Sup.</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtToleranciaSuperior" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor do Adiantamento, se houver.">Vl. Adiantamento</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorAdiantamento" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor do Seguro, se houver.">Vl. Seguro</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorSeguro" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor da Tarifa por Emissão do Novo Cartão">Vl. Cartão</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorTarifaEmissaoCartao" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor Pedágio, se o contratante quiser descontar da viagem.">Vl. Pedágio</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorPedagio" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor Cartão Pedágio, se o contratante quiser descontar da viagem.">Vl. C. Ped.</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorCartaoPedagio" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Vl. IRRF*:
                                                        </span>
                                                        <input type="text" id="txtValorIRRF" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Vl. INSS*:
                                                        </span>
                                                        <input type="text" id="txtValorINSS" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Vl. SENAT*:
                                                        </span>
                                                        <input type="text" id="txtValorSENAT" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Vl. SEST*:
                                                        </span>
                                                        <input type="text" id="txtValorSEST" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Outros Descontos, se o contratante quiser descontar da viagem.">Vl. Out. Desc.</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorOutrosDescontos" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">
                                                            <abbr title="Valor Abastecimento">Vl. Abastecimento</abbr>*:
                                                        </span>
                                                        <input type="text" id="txtValorAbastecimento" class="form-control maskedInput" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12">
                                                    <button type="button" id="btnSalvarDocumento" class="btn btn-primary">Salvar</button>
                                                    <button type="button" id="btnExcluirDocumento" class="btn btn-danger" style="display: none;">Excluir</button>
                                                    <button type="button" id="btnCancelarDocumento" class="btn btn-default">Cancelar</button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                            <table id="tblDocumentos" class="table table-bordered table-condensed table-hover">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 20%;">CT-e</th>
                                                        <th style="width: 20%;">Quantidade</th>
                                                        <th style="width: 20%;">Peso Bruto</th>
                                                        <th style="width: 20%;">Valor Frete</th>
                                                        <th style="width: 20%;">Opções
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

                                <div style="margin-top: -20px" id="divDocumentosVinculados"></div>
                            </div>
                            <div class="tab-pane" id="divEncerramento">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor do Frete Negociado na Saída (deve ser igual a 'Vl. Tar. Frete' x 'Peso Bruto')">Vl. Frete</abbr>:</span>
                                            <input type="text" id="txtEncerramentoValorFrete" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor utilizado para recálculo do frete na chegada, por quilo de mercadoria entregue.">Vl. Tar. Frete</abbr>:</span>
                                            <input type="text" id="txtEncerramentoValorTarifa" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-4" style="margin-bottom: 5px;">
                                        <button type="button" id="btnEncerramentoCarregarPorCTe" class="btn btn-primary" style="margin-right: 5px;">Calcular valores por CT-es</button>
                                        <button type="button" id="btnEncerramentoCalcularImpostos" class="btn btn-primary">Calcular Impostos</button>
                                    </div>

                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Peso Total:
                                            </span>
                                            <input type="text" id="txtEncerramentoPesoTotal" class="form-control" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Total da Mercadoria Carregada">Vl. Merc.</abbr>:</span>
                                            <input type="text" id="txtEncerramentoValorMercadoria" class="form-control" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor da Mercadoria por KG">Vl. Merc. KG</abbr>:</span>
                                            <input type="text" id="txtEncerramentoValorMercadoriaPorKG" class="form-control" disabled="disabled" />
                                        </div>
                                    </div>

                                    <div class="col-sm-12"></div>

                                    <div class="col-xs-12 col-sm-8 col-md-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Define como será efetuado o cálculo de quebra, se houver.">Tipo Quebra</abbr>*:</span>
                                            <select id="selEncerramentoTipoQuebra" class="form-control">
                                                <option value="">Tipo Quebra</option>
                                                <option value="1">Integral (cobra valor correspondente a toda a mercadoria faltante)</option>
                                                <option value="2">Parcial (cobra apenas o valor da mercadoria faltante que ultrapassar a tolerância)</option>
                                                <option value="3" selected="selected">Sem Quebra (não cobra mercadoria faltante)</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Define como será a tolerância para diferença de peso na entrega (a menor ou a maior).">Tp. Tolerân.</abbr>*:</span>
                                            <select id="selEncerramentoTipoTolerancia" class="form-control">
                                                <option value="1" selected="selected">Percentual (diferença de peso permitida é um percentual sobre o peso de saída)</option>
                                                <option value="2">Peso (diferença de peso permitida é valor fixo em quilos)</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Percentual Tolerância">Percentual Tol.</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoPercentualTolerancia" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4 pamcard-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Abastecimento">Vl Abastec.</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorAbastecimento" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-8 col-md-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor do Adiantamento, se houver.">Vl. Adiantamento</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorAdiantamento" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4 efrete-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor do Seguro, se houver.">Vl. Seguro</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorSeguro" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-8 col-md-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Pedágio, se o contratante quiser descontar da viagem.">Vl. Pedágio</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorPedagio" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor IRRF*:</span>
                                            <input type="text" id="txtEncerramentoValorIRRF" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor INSS*:</span>
                                            <input type="text" id="txtEncerramentoValorINSS" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor SEST*:</span>
                                            <input type="text" id="txtEncerramentoValorSEST" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor SENAT*:</span>
                                            <input type="text" id="txtEncerramentoValorSENAT" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-4 efrete-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Total Operação (Valor Frete + Valor Pedágio + Valor Seguro + Valor INSS + Valor IRRF + Valor SENAT + Valor SEST)">Vl. Tot. Operação</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoTotalOperacao" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 efrete-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Total Quitação (Valor Total Operação - Valor Adiantamento)">Vl. Tot. Quitação</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoTotalQuitacao" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 pamcard-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Total Operação (Valor Frete + Valor INSS + Valor IRRF + Valor SENAT + Valor SEST + Valor Adiantamento)">Vl. Bruto</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorBruto" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4 pamcard-abertura">
                                        <div class="input-group">
                                            <span class="input-group-addon">
                                                <abbr title="Valor Líquido (Valor Frete - Valor Impostos - Valor Adiantamento)">Vl. Líquido</abbr>*:</span>
                                            <input type="text" id="txtEncerramentoValorLiquido" class="form-control always-disabled" disabled="disabled" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane" id="divCidadesPedagio">
                                <div class="panel panel-default">
                                    <div class="panel-heading">Cidades com pedágio</div>
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Cidade:
                                                    </span>
                                                    <input type="text" id="txtCidadePedagio" class="form-control" />
                                                    <span class="input-group-btn">
                                                        <button type="button" id="btnBuscarCidadePedagio" class="btn btn-primary">Buscar</button>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                        <button type="button" id="btnSalvarCidadesPedagio" class="btn btn-primary">Salvar</button>
                                        <button type="button" id="btnExcluirCidadesPedagio" class="btn btn-danger" style="display: none;">Excluir</button>
                                        <button type="button" id="btnCancelarCidadesPedagio" class="btn btn-default">Cancelar</button>
                                        <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                            <table id="tblCidadesPedagio" class="table table-bordered table-condensed table-hover">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 80%;">Nome</th>
                                                        <th style="width: 20%;">Opções
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
                            </div>
                            <div class="tab-pane" id="divJustificativa">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12">
                                        <div class="input-group">
                                            <span class="input-group-addon">Justificativa:
                                            </span>
                                            <input type="text" id="txtJustificativa" class="form-control" value="" disabled="disabled" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarCTes" class="btn btn-primary" style="display: none"><span class="glyphicon glyphicon-floppy-saved"></span>&nbsp;Salvar CT-es</button>
                    <button type="button" id="btnSalvarCIOT" class="btn btn-primary" style="display: none"><span class="glyphicon glyphicon-floppy-disk"></span>&nbsp;Salvar CIOT</button>
                    <button type="button" id="btnEmitirCIOT" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;<span class="descricao">Emitir CIOT</span></button>
                    <button type="button" id="btnCancelarCIOT" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="divConsultaCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de CT-es</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgConsultaCTes"></div>
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
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CIOT:
                                </span>
                                <input type="text" id="txtNumeroCIOTConsulta" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Inicial:
                                </span>
                                <input type="text" id="txtDataInicialCTeConsulta" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data Final:
                                </span>
                                <input type="text" id="txtDataFinalCTeConsulta" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="checkbox-group">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkSemCIOT">
                                        Sem CIOT
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusCTeConsulta" class="form-control">
                                    <option value="N">Não autorizados</option>
                                    <option value="A">Autorizados</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Placa:
                                </span>
                                <input type="text" id="txtPlacaConsulta" class="form-control maskedInput text-uppercase" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnBuscarCTesConsulta" class="btn btn-primary">Buscar</button>
                    <button type="button" id="btnSelecionarTodosOsCTes" class="btn btn-default pull-right">Selecionar Todos</button>
                    <div id="tbl_ctes_consulta" style="margin-top: 10px;">
                    </div>
                    <div id="tbl_ctes_consulta_paginacao">
                    </div>
                    <div class="clearfix"></div>
                    <div class="divCTesSelecionados">
                        <div class="tfs-tags">
                            <div class="tags-items-container">
                                <ul id="containerCTesSelecionados"></ul>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnObterInformacoesCTesSelecionados" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Finalizar Seleção de CT-es</button>
                    <button type="button" id="btnCancelarConsultaAvancada" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalUploadArquivos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Upload de Arquivos</h4>
                </div>
                <div class="modal-body">
                    <div id="divUploadArquivos">
                        Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
