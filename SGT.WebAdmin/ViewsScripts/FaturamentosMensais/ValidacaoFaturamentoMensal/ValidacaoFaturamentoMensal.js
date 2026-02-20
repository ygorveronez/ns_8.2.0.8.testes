/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/GrupoFaturamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridValidacaoFaturamentoMensal;
var _validacaoFaturamentoMensal;

var EmpresaMap = function () {
    this.CodigoEmpresa = PropertyEntity({ val: 0, def: 0 });
    this.CNPJCliente = PropertyEntity({ val: 0, def: 0 });
    this.CodigoFaturamentoMensalCliente = PropertyEntity({ val: 0, def: 0 });
    this.Empresa = PropertyEntity({ val: "", def: "" });
    this.DiaFaturamento = PropertyEntity({ val: 0, def: 0 });
    this.ProximoVencimento = PropertyEntity({ val: "", def: "" });
    this.UltimoVencimento = PropertyEntity({ val: "", def: "" });
    this.ValorFaturamento = PropertyEntity({ val: "", def: "" });
    this.CadastroFaturamento = PropertyEntity({ val: "", def: "" });
    this.PlanoMensal = PropertyEntity({ val: 0, def: 0 });
    this.QtdDocumento = PropertyEntity({ val: 0, def: 0 });
    this.QtdNFe = PropertyEntity({ val: 0, def: 0 });;
    this.QtdNFSe = PropertyEntity({ val: 0, def: 0 });
    this.QtdBoleto = PropertyEntity({ val: 0, def: 0 });
    this.QtdTitulo = PropertyEntity({ val: 0, def: 0 });
}

var PesquisaValidacaoFaturamentoMensal = function () {

    this.GrupoFaturamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Faturamento: ", idBtnSearch: guid(), visible: true, required: true });

    this.SalvarClientes2 = PropertyEntity({ eventClick: SalvarClienteClick, type: types.event, text: "Salvar Clientes", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });
    this.BuscarClientes = PropertyEntity({ eventClick: BuscarEmpresasClick, type: types.event, text: "Buscar Empresas", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar / Limpar", icon: ko.observable("fal fa-recycle"), idGrid: guid(), visible: ko.observable(true) });
    this.SalvarClientes = PropertyEntity({ eventClick: SalvarClienteClick, type: types.event, text: "Salvar Clientes", icon: ko.observable("fal fa-save"), idGrid: guid(), visible: ko.observable(true) });
    this.GerarRelatorio = PropertyEntity({ eventClick: GerarRelatorioClick, type: types.event, text: "Gerar Relatório", icon: ko.observable("fal fa-list"), idGrid: guid(), visible: ko.observable(true) });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ListaEmpresas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    //this.Empresas = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.Empresas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
}

//*******EVENTOS*******


function loadValidacaoFaturamentoMensal() {
    _validacaoFaturamentoMensal = new PesquisaValidacaoFaturamentoMensal();
    KoBindings(_validacaoFaturamentoMensal, "knockoutPesquisaValidacaoFaturamentoMensal", false);

    new BuscarGrupoFaturamento(_validacaoFaturamentoMensal.GrupoFaturamento);
}


function buscarEmpresas() {
    _gridValidacaoFaturamentoMensal = new GridView(_validacaoFaturamentoMensal.Empresas.idGrid, "ValidacaoFaturamentoMensal/PesquisarEmpresaFaturamento", _validacaoFaturamentoMensal);
    _gridValidacaoFaturamentoMensal.CarregarGrid();
}

//*******MÉTODOS*******

function BuscarEmpresasClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        buscarEmpresas();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione os campos obrigatórios antes de buscar as empresas.");
    }
}

function CancelarClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar / limpar todos os dados?", function () {
        LimparCamposGeracaoMovimento();
    });
}

function SalvarClienteClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja salvar as empresas pendente de faturamento?", function () {
        CarregarListaEmpresa();
        var data = { ListaEmpresas: _validacaoFaturamentoMensal.ListaEmpresas.val(), CodigoGrupoFaturamento: _validacaoFaturamentoMensal.GrupoFaturamento.codEntity() };
        executarReST("ValidacaoFaturamentoMensal/SalvarEmpresas", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Empresas cadastradas com sucesso.");
                buscarEmpresas();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    });
}

function GerarRelatorioClick(e) {
    var data = { CodigoGrupoFaturamento: _validacaoFaturamentoMensal.GrupoFaturamento.codEntity() };
    executarReST("ValidacaoFaturamentoMensal/GerarRelatorioEmpresas", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Por favor aguarde a geração do relatório.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function CarregarListaEmpresa() {
    _validacaoFaturamentoMensal.ListaEmpresas.val("");

    //var listaEmpresas = _gridValidacaoFaturamentoMensal.GridViewTable().rows().data();
    //_validacaoFaturamentoMensal.ListaEmpresas.val("");

    //if (listaEmpresas.length > 0) {
    //    var dataGrid = new Array();
    //    $.each(listaEmpresas, function (i, emp) {
    //        var map = new EmpresaMap();

    //        map.CodigoEmpresa.val = emp.CodigoEmpresa;
    //        map.CNPJCliente.val = emp.CNPJCliente;
    //        map.CodigoFaturamentoMensalCliente.val = emp.CodigoFaturamentoMensalCliente;
    //        map.Empresa.val = emp.Empresa;
    //        map.DiaFaturamento.val = emp.DiaFaturamento;
    //        map.ProximoVencimento.val = emp.ProximoVencimento;
    //        map.UltimoVencimento.val = emp.UltimoVencimento;
    //        map.ValorFaturamento.val = emp.ValorFaturamento;
    //        map.CadastroFaturamento.val = emp.CadastroFaturamento;
    //        map.PlanoMensal.val = emp.PlanoMensal;
    //        map.QtdDocumento.val = emp.QtdDocumento;
    //        map.QtdNFe.val = emp.QtdNFe;
    //        map.QtdNFSe.val = emp.QtdNFSe;
    //        map.QtdBoleto.val = emp.QtdBoleto;
    //        map.QtdTitulo.val = emp.QtdTitulo;

    //        dataGrid.push(map);
    //    });
    //    _validacaoFaturamentoMensal.ListaEmpresas.val(JSON.stringify(dataGrid));
    //}
}

function LimparCamposGeracaoMovimento() {
    LimparCampos(_validacaoFaturamentoMensal);
    _validacaoFaturamentoMensal.ListaEmpresas.val("");

    _gridValidacaoFaturamentoMensal = null;
    buscarEmpresas();
}
