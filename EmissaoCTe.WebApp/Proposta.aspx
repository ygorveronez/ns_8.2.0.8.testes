<%@ Page Title="Proposta" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Proposta.aspx.cs" Inherits="EmissaoCTe.WebApp.Proposta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <script defer="defer" type="text/javascript">
            CKEDITOR_BASEPATH = ObterPath() + '/Scripts/ckeditor/';
        </script>
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/ckeditor",
                           "~/bundle/scripts/ckeditoradapters",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/proposta") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Proposta de Frete</h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Data Lançamento:</span>
                <input type="text" id="txtDataLancamento" readonly class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-2">
            <div class="input-group">
                <span class="input-group-addon">Número:</span>
                <input type="text" id="txtNumero" disabled class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Data:</span>
                <input type="text" id="txtData" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:</span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Nome:</span>
                <input type="text" id="txtNome" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Telefone:</span>
                <input type="text" id="txtTelefone" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">E-mail:</span>
                <input type="text" id="txtEmail" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo de Processo:</span>
                <input type="text" id="txtTipoColeta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoColeta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo de Carga:</span>
                <input type="text" id="txtTipoCarga" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoCarga" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Modal:</span>
                <select id="selModalProposta" class="form-control">
                    <option value="1">Rodoviario</option>
                    <option value="2">Rodoaereo</option>
                    <option value="9">Outros</option>
                </select>
            </div>
        </div>

        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Veículo:</span>
                <select id="selTipoVeiculo" class="form-control">
                    <option value="00">Não Aplicado</option>
                    <option value="01">Truck</option>
                    <option value="02">Toco</option>
                    <option value="03">Cavalo</option>
                    <option value="04">Van</option>
                    <option value="05">Utilitário</option>
                    <option value="07">VUC</option>
                    <option value="08">3/4</option>
                    <option value="09">Carreta</option>
                    <option value="06">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Carroceria:</span>
                <select id="selTipoCarroceria" class="form-control">
                    <option value="00">Não Aplicado</option>
                    <option value="01">Aberta</option>
                    <option value="02">Fechada/Baú</option>
                    <option value="03">Granel</option>
                    <option value="04">Porta Container</option>
                    <option value="05">Sider</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Rastreador:</span>
                <select id="selRastreador" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Mercadoria:</span>
                <input type="text" id="txtValorMercadoria" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Cifrão:</span>
                <input type="text" id="txtUnidadeMonetaria" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Peso:</span>
                <input type="text" id="txtPeso" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Volumes:</span>
                <input type="text" id="txtVolumes" class="form-control" />
            </div>
        </div>
        <div class="col-xs-4 col-sm-4 col-md-2">
            <div class="input-group">
                <span class="input-group-addon">Dimensões:</span>
                <input type="text" id="txtDimensoesA" class="form-control" />
            </div>
        </div>
        <div class="col-xs-4 col-sm-4 col-md-1">
                <input type="text" id="txtDimensoesL" class="form-control" />
        </div>
        <div class="col-xs-4 col-sm-4 col-md-1">
                <input type="text" id="txtDimensoesP" class="form-control" />
        </div>
    </div>
    <div class="row">        
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Origem:</span>
                <input type="text" id="txtOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarOrigem" data-consulta="Origem" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Destino:</span>
                <input type="text" id="txtDestino" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestino" data-consulta="Destino" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente Origem:</span>
                <input type="text" id="txtClienteOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarClienteOrigem" data-consulta="ClienteOrigem" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente Destino:</span>
                <input type="text" id="txtClienteDestino" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarClienteDestino" data-consulta="ClienteDestino" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:</span>
                <textarea id="txtObservacoes" class="form-control" rows="4"></textarea>
            </div>
        </div>
    </div>
    <br />
    
    <div class="row">
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Descrição:</span>
                <input id="txtDescricaoItem" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Valor:</span>
                <input id="txtValorItem" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo:</span>
                <select id="selTipoItem" class="form-control">
                    <option value="0">Valor (R$)</option>
                    <option value="1">Percentual (%)</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvarItem" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelarItem" class="btn btn-default">Cancelar</button>
    <button type="button" id="btnExcluirItem" style="display: none" class="btn btn-danger">Excluir</button>
    <br /><br />
    <table id="tblItensProposta" class="table table-bordered table-condensed table-hover">
        <thead>
            <tr>
                <th width="50%">Descrição</th>
                <th width="30%">Valor</th>
                <th width="20%">Opção</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    <br />

    <div class="row">
        <div class="col-xs-12 col-sm-3">
            <div class="input-group">
                <span class="input-group-addon">Dias de Validade:
                </span>
                <input type="text" id="txtDiasValidade" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12">
            <div class="panel-group" id="collapseTextos" role="tablist" aria-multiselectable="true">
                <div class="panel panel-default">
                    <div class="panel-heading" role="tab">
                        <h4 class="panel-title">
                            <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoCustosAdicionais" aria-expanded="true" aria-controls="textoCustosAdicionais">Texto sobre Custos Adicionais</a>
                        </h4>
                    </div>
                    <div id="textoCustosAdicionais" class="panel-collapse collapse in" role="tabpanel"">
                        <div class="panel-body">
                            <div class="ck-container">
                                <textarea id="txtCustosAdicionais" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading" role="tab">
                        <h4 class="panel-title">
                            <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoFormaCobranca" aria-expanded="true" aria-controls="textoFormaCobranca">Texto sobre Forma Cobrança</a>
                        </h4>
                    </div>
                    <div id="textoFormaCobranca" class="panel-collapse collapse" role="tabpanel"">
                        <div class="panel-body">
                            <div class="ck-container">
                                <textarea id="txtFormaCobranca" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading" role="tab">
                        <h4 class="panel-title">
                            <a role="button" data-toggle="collapse" data-parent="#collapseTextos" href="#textoCTRN" aria-expanded="true" aria-controls="textoCTRN">Texto sobre CTNR</a>
                        </h4>
                    </div>
                    <div id="textoCTRN" class="panel-collapse collapse" role="tabpanel"">
                        <div class="panel-body">
                            <div class="ck-container">
                                <textarea id="txtCTRN" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <br />
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
    <button type="button" id="btnExcluir" style="display: none" class="btn btn-danger">Excluir</button>
    <button type="button" id="btnDownloadProposta" class="btn btn-primary" style="display: none;"><span class="glyphicon glyphicon-download"></span>&nbsp;Visualizar Proposta</button>
</asp:Content>
