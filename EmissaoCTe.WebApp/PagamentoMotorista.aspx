<%@ Page Title="Pagamentos de Motoristas por CT-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="PagamentoMotorista.aspx.cs" Inherits="EmissaoCTe.WebApp.PagamentoMotorista" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
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
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/pagamentoMotorista",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoCte" value="0" />
        <input type="hidden" id="hddDescricaoCte" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Pagamentos de Motoristas por CT-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="clearfix"></div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle btn-block" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Dt. Inicial:
                                </span>
                                <input type="text" id="txtDataInicial" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Dt. Final:
                                </span>
                                <input type="text" id="txtDataFinal" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Número Inicial:
                                </span>
                                <input type="text" id="txtoNumeroInicial" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Número Final:
                                </span>
                                <input type="text" id="txtoNumeroFinal" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">CT-e:
                                </span>
                                <input type="text" id="txtCTeFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarCTeFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Motorista:
                                </span>
                                <input type="text" id="txtMotoristaFiltro" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotoristaFiltro" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
                            <div class="input-group">
                                <span class="input-group-addon">Status:
                                </span>
                                <select id="selStatusFiltro" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="P">Pendente</option>
                                    <option value="A">Pago</option>
                                    <option value="C">Cancelado</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnConsultarPagamentos" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar Pagamentos</button>
    <div id="tbl_pagamentos" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_pagamentos">
    </div>
    <div class="clearfix"></div>
    <hr style="margin: 20px 0 10px 0;" />
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número:
                </span>
                <input type="text" id="txtNumero" readonly class="form-control" />
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
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Veiculo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor*:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Pedágio:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Salário Motorista:
                </span>
                <input type="text" id="txtSalarioMotorista" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Deduzir INSS/IR">Deduzir</abbr>*:
                </span>
                <select id="selDeduzir" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">INSS + SENAT:
                </span>
                <input type="text" id="txtINSSSENAT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">SEST SENAT:
                </span>
                <input type="text" id="txtSESTSENAT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Imposto Renda:
                </span>
                <input type="text" id="txtIR" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Adiantamento:
                </span>
                <input type="text" id="txtAdiantamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Outros:
                </span>
                <input type="text" id="txtValorOutros" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Saldo a Pagar:
                </span>
                <input type="text" id="txtSaldoPagar" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Pgto.*:
                </span>
                <input type="text" id="txtDataPagamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Rcbto.*:
                </span>
                <input type="text" id="txtDataRecebimento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="P">Pendente</option>
                    <option value="A">Pago</option>
                    <option value="C">Cancelado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" rows="2" class="form-control" maxlength="300"></textarea>
            </div>
        </div>
    </div>
    <div class="row hide">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">CT-e*:
                </span>
                <input type="text" id="txtCTe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnSelecionarCTes" class="btn btn-default">Selecionar Documentos</button>
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
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>

    <div class="modal fade" id="divConsultaCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Consulta de Documentos</h4>
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
                    <button type="button" id="btnFinalizarSelecaoCTes" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Finalizar Seleção de Documentos</button>
                    <button type="button" data-dismiss="modal" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
