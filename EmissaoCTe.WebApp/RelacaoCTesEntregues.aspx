<%@ Page Title="Relação de CT-es Entregues" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RelacaoCTesEntregues.aspx.cs" Inherits="EmissaoCTe.WebApp.RelacaoCTesEntregues" %>
<%@ Import Namespace="System.Web.Optimization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker") %>

        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/priceFormat",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/relacaoCTesEntregues") %>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relação de CT-es Entregues</h2>
    </div>
    <div id="messages-placeholder"></div>
    <button type="button" id="btnNovaRelacao" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Nova Relação de CT-es</button>
    <div class="row" style="margin-top: 15px;">
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Número Inicial:
                </span>
                <input type="text" id="txtFiltroNumeroInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Número Final:
                </span>
                <input type="text" id="txtFiltroNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Data Bip. Inicial:
                </span>
                <input type="text" id="txtFiltroDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Data Bip. Final:
                </span>
                <input type="text" id="txtFiltroDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtFiltroCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFiltroCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Controle:
                </span>
                <input type="text" id="txtFiltroControle" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selFiltroStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="1">Aberto</option>
                    <option value="2">Fechado</option>
                    <option value="9">Cancelado</option>
                </select>
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
            <div id="filtros" class="panel-collapse collapse">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon">Descr.:
                                </span>
                                <input type="text" id="txtFiltroDescricao" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. CT-e:
                                </span>
                                <input type="text" id="txtFiltroCTe" class="form-control maskedInput" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Controle:
                                </span>
                                <input type="text" id="txtFiltroNumeroControle" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <button type="button" id="btnConsultarGrid" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>

    <div style="margin-top: 10px">
        <button type="button" id="btnAtualizarGridCTes" class="hide btn btn-primary">Atualizar</button>

        <div id="tbl_relacoes_consulta" style="margin-top: 10px;">
        </div>
        <div id="tbl_relacoes_consulta_paginacao">
        </div>
        <div class="clearfix"></div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="divModalRelacaoCTes" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Relação de CT-es Entregues</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-validacao-relacao"></div>
                    <div role="tabpanel">
                        <ul class="nav nav-tabs" style="margin-bottom: 10px;">

                            <li class="active"><a href="#divDadosGerais" role="tab" data-toggle="tab">Dados</a></li>
                            <li><a href="#divCTes" role="tab" data-toggle="tab">CT-es</a></li>
                            <li><a href="#divColetas" role="tab" data-toggle="tab">Coletas</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="divDadosGerais">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Número:
                                            </span>
                                            <input type="text" id="txtNumero" class="form-control" disabled="disabled" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon"><abbr title="Número interno para controle manual">Controle</abbr>:
                                            </span>
                                            <input type="text" id="txtNumeroControle" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Data Bipagem*:
                                            </span>
                                            <input type="text" id="txtDataRelacao" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Data Entrega*:
                                            </span>
                                            <input type="text" id="txtDataEntrega" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Km Inicial:
                                            </span>
                                            <input type="text" id="txtKmInicial" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Km Final:
                                            </span>
                                            <input type="text" id="txtKmFinal" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Cliente*:
                                            </span>
                                            <input type="text" id="txtCliente" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Descrição:
                                            </span>
                                            <input type="text" id="txtDescricao" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Diária:
                                            </span>
                                            <select id="selTipoDiaria" class="form-control">
                                                <option value="0">Sem Diária</option>
                                                <option value="1">Diária Inteira</option>
                                                <option value="2">Meia Diária</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Diária:
                                            </span>
                                            <input type="text" id="txtDiaria" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Acrescimos:
                                            </span>
                                            <input type="text" id="txtValorAcrescimos" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Descontos:
                                            </span>
                                            <input type="text" id="txtValorDescontos" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-6" id="idValorTotal">
                                        <div class="input-group">
                                            <span class="input-group-addon">Valor Total:
                                            </span>
                                            <input type="text" id="txtValorTotal" class="form-control" />
                                        </div>
                                    </div>

                                    <div class="col-xs-12 col-sm-4 col-md-12">
                                        <div class="input-group">
                                            <span class="input-group-addon">Observação:
                                            </span>
                                            <textarea rows="3" id="txtObservacao" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="tab-pane" id="divCTes">
                                <div id="placeholder-validacao-cte"></div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12">
                                        <div class="input-group">
                                            <span class="input-group-addon">Chave:
                                            </span>
                                            <input type="text" id="txtChave" class="form-control" />
                                            <span class="input-group-btn">
                                                <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                    <table id="tblCTes" class="table table-bordered table-condensed table-hover">
                                        <thead>
                                            <tr>
                                                <th style="width: 7%;">Número</th>
                                                <th style="width: 25%;">Emitente</th>
                                                <th style="width: 13%;">Data Emissão</th>
                                                <th style="width: 15%;">Término Prestação</th>
                                                <th style="width: 10%;">Frete</th>
                                                <th style="width: 10%;">Peso</th>
                                                <th style="width: 10%;" class="text-center pode-modificar-grid">Opções</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            
                            <div class="tab-pane" id="divColetas">
                                <div id="placeholder-validacao-coletas"></div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Coleta:
                                            </span>
                                            <input type="text" id="txtColetaDescricao" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">Peso Total:
                                            </span>
                                            <input type="text" id="txtColetaPesoTotal" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-4">
                                        <button type="button" id="btnSalvarColeta" class="btn btn-primary">Adicionar</button>
                                    </div>
                                </div>
                                <div class="table-responsive" style="margin-top: 10px; height: 200px; overflow-y: scroll;">
                                    <table id="tblColetas" class="table table-bordered table-condensed table-hover">
                                        <thead>
                                            <tr>
                                                <th style="width: 30%;">Descrição</th>
                                                <th style="width: 20%;">Peso</th>
                                                <th style="width: 20%;">Valor por Evento</th>
                                                <th style="width: 20%;">Valor por Fração</th>
                                                <th style="width: 10%;" class="text-center pode-modificar-grid">Opções</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarRelacao" class="btn btn-primary"><span class="glyphicon glyphicon-floppy-disk"></span> &nbsp;Salvar Relação</button>
                    <button type="button" id="btnFinalizarRelacao" class="btn btn-primary" style="display: none"><span class="glyphicon glyphicon-ok"></span>&nbsp;Finalizar Relação</button>
                    <button type="button" id="btnCancelarRelacao" class="btn btn-default" data-dismiss="modal"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade" id="divCaptchaCTeSefaz" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Captcha consulta CT-e Sefaz</h4>
                </div>
                <div class="modal-body">
                    <div id="divCaptchaCTeSefazBody" class="row" style="padding: 15px;">
                        <div class="smart-form">
                            <section>
                                <div class="well">
                                    <div class="row">
                                        <div style="float: left; margin-top: 6px; margin-left: 100px;">
                                            <img src="" id="imgCaptcha" style="float: left; margin-top: 6px; border: 1px solid #CCC; width: 260px; height: 80px;" /><a href="javascript:;void(0)" style="float: left; margin-top: 25px; margin-left: 5px;"><i class="fa fa-refresh"></i></a>
                                        </div>
                                        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                            <div class="input-group" style="margin-top: 40px;">
                                                <span class="input-group-addon" style="margin-top: 6px">Captcha CT-e:
                                                </span>
                                                <input type="text" id="txtCaptchaCTeSefaz" class="form-control" autofocus />
                                                <span class="input-group-btn">
                                                    <button type="button" id="btnCaptchaCTeSefaz" class="btn btn-primary"><span class="glyphicon glyphicon-search"></span>&nbsp;Consultar</button>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <span class="input-group-btn">
                                            <button type="button" id="btnAtualizarCaptchaCTeSefaz" class="btn btn-link" style="margin-left: 135px"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Gerar novo Captcha</button>
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
</asp:Content>