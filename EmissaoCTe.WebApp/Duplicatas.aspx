<%@ Page Title="Cadastro de Duplicata" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Duplicatas.aspx.cs" Inherits="EmissaoCTe.WebApp.Duplicatas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload",
                          "~/bundle/styles/multiplaSelecao") %>
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/duplicatas",
                           "~/bundle/scripts/fileDownload")%>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoDocumentoEntrada" value="0" />
        <input type="hidden" id="hddCodigoCte" value="0" />
        <input type="hidden" id="hddCodigoParcela" value="0" />
        <input type="hidden" id="hddDescricaoCte" value="" />
        <input type="hidden" id="hddValorCte" value="" />
        <input type="hidden" id="hddTomadorCte" value="" />
        <input type="hidden" id="hddURLBuscaCTe" value="/Duplicatas/ConsultarCtesSemDuplicata?callback=?&Pessoa=0" />
    </div>

    <div class="page-header">
        <h2>Cadastro de Duplicata
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Número*:
                </span>
                <input type="text" id="txtNumero" class="form-control" value="Automatico" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Usuário:
                </span>
                <input type="text" id="txtFuncionario" class="form-control" value="Automatico" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">A Receber</option>
                    <option value="1">A Pagar</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Lcto.*:
                </span>
                <input type="text" id="txtDataLcto" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
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
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Documento*:
                </span>
                <input type="text" id="txtDocumento" class="form-control" value="" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Doc.*:
                </span>
                <input type="text" id="txtDataDocumento" class="form-control" value="" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Valor*:
                </span>
                <input type="text" id="txtValor" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Acresc.:
                </span>
                <input type="text" id="txtAcrescimo" class="form-control" maxlength="18" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Desc.:
                </span>
                <input type="text" id="txtDesconto" class="form-control" maxlength="18" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tomador*:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
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
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veic.1:
                </span>
                <input type="text" id="txtVeiculo1" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo1" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veic.2:
                </span>
                <input type="text" id="txtVeiculo2" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo2" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veic.3:
                </span>
                <input type="text" id="txtVeiculo3" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo3" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Plano de Conta*:
                </span>
                <input type="text" id="txtPlanoDeConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoDeConta" class="btn btn-primary">Buscar</button>
                </span>
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
    <ul class="nav nav-tabs" id="tabsDetalhes">
        <li class="active"><a href="#parcelas" data-toggle="tab">Parcelas</a></li>
        <li><a href="#ctes" data-toggle="tab">CTe/NFSe</a></li>
        <li><a href="#adicionais" data-toggle="tab">Adicionais</a></li>
    </ul>
    <div class="tab-content" style="margin-top: 10px;">
        <div class="tab-pane active" id="parcelas">
            <div id="placeholder-mensagem-parcelas">
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Parcelas*:
                        </span>
                        <input type="text" id="txtParcelas" class="form-control" maxlength="100" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">
                            <abbr title="Quando não informado serão geradas parcelas mensais com o dia do primeiro vencimento.">Intervalo Dias:</abbr>:
                        </span>
                        <input type="text" id="txtIntervaloDias" class="form-control" maxlength="100" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Data Vcto.*:
                        </span>
                        <input type="text" id="txtDataVcto" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Arredondar*:
                        </span>
                        <select id="selArredondar" class="form-control">
                            <option value="0">Primeira</option>
                            <option value="1">Última</option>
                        </select>
                    </div>
                </div>
            </div>
            <button type="button" id="btnGerarParcelas" class="btn btn-primary">Gerar Parcelas</button>

            <div id="divAlterarParcela" style="margin-top: 10px; display: none;">
                <!--style="display: none"-->
                <div class="row">
                    <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
                        <div class="input-group">
                            <span class="input-group-addon">Data Vcto.*:
                            </span>
                            <input type="text" id="txtDataVctoParcela" class="form-control" />
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                        <div class="input-group">
                            <span class="input-group-addon">Valor*:
                            </span>
                            <input type="text" id="txtValorParcela" class="form-control" maxlength="18" />
                        </div>
                    </div>
                    <button type="button" id="btnSalvarParcela" class="btn btn-primary">Salvar Parcela</button>
                </div>
            </div>

            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblParcelas" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 5%;" colspan="1" rowspan="1">Parcela
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data Vcto.
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Valor Pgto.
                            </th>
                            <th style="width: 10%;" colspan="1" rowspan="1">Data Pgto.
                            </th>
                            <th style="width: 35%;" colspan="1" rowspan="1">Obs. Baixa
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
        <div class="tab-pane" id="ctes">
            <div id="placeholder-mensagem-ctes">
            </div>
            <div class="row hide">
                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">CT-e:
                        </span>
                        <input type="text" id="txtCte" class="form-control" />
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarCte" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
            </div>
            <button type="button" id="btnSelecionarCTes" class="btn btn-default">Selecionar</button>
            <button type="button" id="btnImportarCTes" class="btn btn-default">Importar XML CTe</button>
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblCtes" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 15%;" colspan="1" rowspan="1">Número/Série
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Doc.
                            </th>
                            <th style="width: 40%;" colspan="1" rowspan="1">Tomador
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Valor a Receber
                            </th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Opções
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
        <div class="tab-pane" id="adicionais">
            <div class="row">
                <div class="col-xs-12 col-sm-12">
                    <div class="input-group">
                        <span class="input-group-addon">Dados Bancários:
                        </span>
                        <input type="text" id="txtDadosBancarios" class="form-control" />
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-10">
                    <div class="input-group">
                        <span class="input-group-addon">Embarcador:
                        </span>
                        <input type="text" id="txtEmbarcador" class="form-control" autocomplete="off">
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarEmbarcador" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">UF Origem:
                        </span>
                        <select id="selAdicionaisUFOrigem" class="form-control"></select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Cidade Origem:
                        </span>
                        <select id="selAdicionaisCidadeOrigem" class="form-control"></select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-addon">UF Destino:
                        </span>
                        <select id="selAdicionaisUFDestino" class="form-control"></select>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
                    <div class="input-group">
                        <span class="input-group-addon">Cidade Destino:
                        </span>
                        <select id="selAdicionaisCidadeDestino" class="form-control"></select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Peso:
                        </span>
                        <input type="text" id="txtAdicionaisPeso" class="form-control" maxlength="18" autocomplete="off">
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="input-group">
                        <span class="input-group-addon">Volumes:
                        </span>
                        <input type="text" id="txtAdicionaisVolumes" class="form-control" maxlength="18" autocomplete="off">
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon">Tipo Veículo:
                        </span>
                        <input type="text" id="txtTipoVeiculo" class="form-control" autocomplete="off">
                        <span class="input-group-btn">
                            <button type="button" id="btnBuscarTipoVeiculo" class="btn btn-primary">Buscar</button>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
    <button type="button" id="btnVisualizar" class="btn btn-primary">Visualizar Duplicata</button>
    <button type="button" id="btnEDICaterpillar" class="btn btn-primary">EDI Caterpillar</button>
    <div class="modal fade" id="divConsultaCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de CT-es e NFS-es</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgConsultaCTes"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Número Inicial:
                                </span>
                                <input type="text" id="txtCTeNumeroInicio" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Número Final:
                                </span>
                                <input type="text" id="txtCTeNumeroFim" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. NF:
                                </span>
                                <input type="text" id="txtCTeNumeroNF" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data Emissão Inicial">Data Emi.</abbr>
                                </span>
                                <input type="text" id="txtCTeDataEmissaoInicio" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Data Emissão Final">Data Emi.</abbr>
                                </span>
                                <input type="text" id="txtCTeDataEmissaoFim" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tomador:
                                </span>
                                <input type="text" id="txtCTeTomador" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Remetente:
                                </span>
                                <input type="text" id="txtCTeRemetente" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Destinatário:
                                </span>
                                <input type="text" id="txtCTeDestinatario" class="form-control maskedInput" />
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
                                <ul id="containerCTesSelecionados">
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnFinalizarSelecaoCTes" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Finalizar Seleção de CT-es</button>
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
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
