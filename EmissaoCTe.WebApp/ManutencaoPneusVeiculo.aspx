<%@ Page Title="Manutenção de Pneus do Veículo" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ManutencaoPneusVeiculo.aspx.cs" Inherits="EmissaoCTe.WebApp.ManutencaoPneusVeiculo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .pneu, .pneuEixo, .pneuManutencao {
            height: 100px;
            width: 101px;
            float: left;
            border: none;
            margin-right: 5px;
            text-align: center;
            cursor: move;
            background: transparent url('Images/pneu.png') no-repeat bottom;
            z-index: 999;
        }

            .pneu header, .pneuEixo header, .pneuManutencao header {
                padding: 5px;
                overflow: hidden;
                height: 35px;
                line-height: 1;
            }

        .droppableActive {
            border: 2px dashed #000 !important;
        }

        .droppableHover {
            background-color: rgba(209, 209, 209, 0.50);
        }
    </style>
    <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker") %>
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
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
                                               "~/bundle/scripts/jqueryUI",
                                               "~/bundle/scripts/twbsPagination",
                                               "~/bundle/scripts/manutencaoPneusVeiculo") %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Manutenção de Pneus do Veículo
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row" id="selecaoPneus">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="panel panel-default">
                <div class="panel-heading bold">
                    <span class="glyphicon glyphicon-ok"></span>&nbsp;Pneus em Estoque
                </div>
                <div class="panel-body">
                    <div id="divContainerEstoquePneu" class="row" style="min-height: 110px;">
                        Arraste um pneu aqui para adicionar ao estoque.
                    </div>
                    <ul id="paginationEstoquePneu" class="pagination-sm"></ul>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="panel panel-default">
                <div class="panel-heading bold">
                    <span class="glyphicon glyphicon-wrench"></span>&nbsp;Pneus em Manutenção
                </div>
                <div class="panel-body">
                    <div id="divContainerManutencaoPneu" class="row" style="min-height: 110px;">
                    </div>
                    <ul id="paginationManutencaoPneu" class="pagination-sm"></ul>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="panel panel-default">
                <div class="panel-heading bold">
                    <span class="glyphicon glyphicon-trash"></span>&nbsp;Descartar Pneu
                </div>
                <div id="divDescartePneu" class="panel-body">
                    Arraste um pneu aqui para descartá-lo.
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
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
    <div class="row" id="veiculo">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
            <div class="row" id="eixosEsquerda">
            </div>
        </div>
        <div class="col-md-2 col-lg-2 hidden-sm hidden-xs">
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-5">
            <div class="row" id="eixosDireita">
            </div>
        </div>
        <div class="col-12">
            <div class="row" id="estepes">
            </div>
        </div>
    </div>
    <div class="modal fade" id="divEntradaPneu" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" id="btnFecharEntradaPneu" class="close" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Movimentação de Pneu</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-entrada-pneu">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataEntrada" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Status*:
                                </span>
                                <input type="text" id="txtStatusEntradaPneu" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarStatusEntradaPneu" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Km:
                                </span>
                                <input type="text" id="txtKMEntrada" class="form-control" value="0" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Calibragem:
                                </span>
                                <input type="text" id="txtCalibragemEntradaPneu" class="form-control" value="0" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Obs:
                                </span>
                                <textarea id="txtObservacaoEntradaPneu" class="form-control" rows="4" maxlength="1000"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarEntradaPneu" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnCancelarEntradaPneu" class="btn btn-default">Cancelar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="divSaidaPneu" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" id="btnFecharSaidaPneu" class="close" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Movimentação de Pneu</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholder-saida-pneu">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataSaida" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Status*:
                                </span>
                                <input type="text" id="txtStatusSaidaPneu" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarStatusSaidaPneu" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Km:
                                </span>
                                <input type="text" id="txtKMSaida" class="form-control" value="0" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Calibragem:
                                </span>
                                <input type="text" id="txtCalibragemSaidaPneu" class="form-control" value="0" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Obs:
                                </span>
                                <textarea id="txtObservacaoSaidaPneu" class="form-control" rows="4" maxlength="1000"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvarSaidaPneu" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnCancelarSaidaPneu" class="btn btn-default">Cancelar</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
