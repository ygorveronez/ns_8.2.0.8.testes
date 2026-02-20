/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscal;
var _notaFiscal;

var ChamadoNotaFiscal = function () {
    this.GridNotaFiscal = PropertyEntity({ type: types.local });
    this.NFe = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais, idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(EnumTipoColetaEntregaDevolucao.Total), options: EnumTipoColetaEntregaDevolucao.obterOpcoes(), def: EnumTipoColetaEntregaDevolucao.Total, text: "Tipo de Devolução das Notas Selecionadas".getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), fadeVisible: ko.observable(false) });
    this.MotivoDaDevolucao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    this.TipoDevolucao.val.subscribe(function (novoValor) {
        if (_crudAbertura.Abrir.visible())
            _controleEntregaDevolucaoChamadoEtapa1.atualizarStatusDevolucaoTotalNotas(novoValor == EnumTipoColetaEntregaDevolucao.Total);

        _notaFiscal.MotivoDaDevolucao.visible(false);
        if (novoValor == EnumTipoColetaEntregaDevolucao.Total)
            _notaFiscal.MotivoDaDevolucao.visible(true);

        _notaFiscal.TipoDevolucao.fadeVisible(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && novoValor != EnumTipoColetaEntregaDevolucao.Total);

        if (typeof ControleCamposNFeDevolucaoAbertura === 'function')
            ControleCamposNFeDevolucaoAbertura(novoValor != EnumTipoColetaEntregaDevolucao.Total);
    });
}


//*******EVENTOS*******

function loadChamadoNotaFiscal() {
    _notaFiscal = new ChamadoNotaFiscal();
    KoBindings(_notaFiscal, "knockoutChamadoNotaFiscal");
    BuscarMotivosDevolucaoEntrega(_notaFiscal.MotivoDaDevolucao);
    loadGridChamadoNotaFiscal();
    if (typeof ControleCamposNFeDevolucaoAbertura === 'function')
        ControleCamposNFeDevolucaoAbertura(false);
}

function loadGridChamadoNotaFiscal() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirNotaFiscalClick(_notaFiscal.NFe, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "80%" },
    { data: "Emitente", title: Localization.Resources.Gerais.Geral.Emitente, width: "80%" },
    { data: "DataEmissao", title: Localization.Resources.Gerais.Geral.DataEmissao, width: "80%" },
    ];

    _gridNotaFiscal = new BasicDataTable(_notaFiscal.GridNotaFiscal.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridNotaFiscal.CarregarGrid([]);

    BuscarXMLNotaFiscal(_notaFiscal.NFe, mostrarGridNFeProdutos, _abertura.Carga, _gridNotaFiscal, null, null, _abertura.Cliente);

    if (_abertura.Carga.val() == "" || _abertura.Cliente.val() == "")
        _notaFiscal.NFe.enable(false);

}


function preencherXMLNotasFiscais(data) {
    _notaFiscal.TipoDevolucao.val(data.Abertura.TipoDevolucao);
    _notaFiscal.MotivoDaDevolucao.val(data.Abertura.MotivoDaDevolucao.Descricao);
    _notaFiscal.MotivoDaDevolucao.codEntity(data.Abertura.MotivoDaDevolucao.Codigo);
    PreencherObjetoKnout(_notaFiscal, { Data: data });
    recarregarGridNotaFiscal();

}

function recarregarGridNotaFiscal() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_abertura.NotasFiscais.val())) {

        $.each(_abertura.NotasFiscais.val(), function (i, notaFiscal) {
            var notaFiscalGrid = new Object();

            notaFiscalGrid.Codigo = notaFiscal.Codigo;
            notaFiscalGrid.Numero = notaFiscal.Numero;
            notaFiscalGrid.Emitente = notaFiscal.Emitente;
            notaFiscalGrid.DataEmissao = notaFiscal.DataEmissao;


            data.push(notaFiscalGrid);
        });

        _gridNotaFiscal.CarregarGrid(data);
    }
}


function excluirNotaFiscalClick(knoutNotaFiscal, data) {
    var notaFiscalGrid = knoutNotaFiscal.basicTable.BuscarRegistros();

    for (var i = 0; i < notaFiscalGrid.length; i++) {
        if (data.Codigo == notaFiscalGrid[i].Codigo) {
            notaFiscalGrid.splice(i, 1);
            break;
        }
    }

    knoutNotaFiscal.basicTable.CarregarGrid(notaFiscalGrid);
}

function limparCamposNotaFiscal() {
    LimparCampos(_notaFiscal);
    LimparCampos(_gridNotaFiscal);
    _gridNotaFiscal.CarregarGrid([]);
    $('#liTabNotaFiscal').hide();
}


function obterNotasFiscais() {
    let ChamadoNotasFiscais = new Array();

    if (!_notaFiscal.NFe.basicTable)
        return;
    $.each(_notaFiscal.NFe.basicTable.BuscarRegistros(), function (i, notaFiscal) {
        ChamadoNotasFiscais.push({ NFe: notaFiscal });
    });
    return JSON.stringify(ChamadoNotasFiscais);
}

function mostrarGridNFeProdutos(e) {
    _notaFiscal.NFe.basicTable = _gridNotaFiscal;

    var notasGrid = _gridNotaFiscal.BuscarRegistros();
    var notasAtualizado = notasGrid !== undefined && notasGrid != null && !notasGrid.some(nota => nota.codigo === e.codigo) ? notasGrid.concat(e) : e;

    _gridNotaFiscal.CarregarGrid(notasAtualizado);
    
    if (e.length > 0 && _motivoChamadoConfiguracao.MotivoDevolucao) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            executarReST("ControleEntregaDevolucao/ObterCargaEntrega", { Cliente: _abertura.Cliente.codEntity(), Carga: _abertura.Carga.codEntity() }, function (retorno) {
                if (retorno.Data) {
                    let listaNotasSelecionadas = _gridNotaFiscal.BuscarRegistros();
                    let controlarEdicao = _crudAbertura.Abrir.visible();
                    _abertura.CodigoCargaEntrega.val(retorno.Data.Codigo);
                    _controleEntregaDevolucaoChamadoEtapa1._inicializar();

                    _controleEntregaDevolucaoChamadoEtapa1._controleEntregaDevolucao.PermitirEdicao.val(controlarEdicao);
                    _controleEntregaDevolucaoChamadoEtapa1.preencher(retorno.Data.Codigo, controlarEdicao, _abertura.Codigo.val(), listaNotasSelecionadas, setarNotasComoDevolucaoTotal);

                    if (_motivoChamadoConfiguracao.MotivoDevolucao) {
                        _notaFiscal.TipoDevolucao.visible(true);
                        _notaFiscal.MotivoDaDevolucao.visible(true);
                    }

                    if (!_crudAbertura.Abrir.visible()) {
                        _notaFiscal.TipoDevolucao.enable(false);
                        _notaFiscal.MotivoDaDevolucao.enable(false);
                    }
                    else {
                        _notaFiscal.TipoDevolucao.enable(true);
                        _notaFiscal.MotivoDaDevolucao.enable(true);
                    }
                }
            });
        }
    }

}
function setarNotasComoDevolucaoTotal() {
    _controleEntregaDevolucaoChamadoEtapa1.atualizarStatusDevolucaoTotalNotas(_notaFiscal.TipoDevolucao.val() == EnumTipoColetaEntregaDevolucao.Total);
}
