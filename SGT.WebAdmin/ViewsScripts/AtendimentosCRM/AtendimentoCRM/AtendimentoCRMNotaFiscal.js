/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _gridNotaFiscal;
var _notaFiscal;

function AtendimentoCRMNotaFiscal () {
    this.GridNotaFiscal = PropertyEntity({ type: types.local });    
    this.NFe = PropertyEntity({ type: types.event, text: "Nota Fiscal", idBtnSearch: guid(), enable: ko.observable(true) });

    $(document).ready(function () {
        _novoAtendimento.NumeroDT.val.subscribe(function (newValue) {
            if (newValue !== undefined && newValue !== '') {
                $('#panelAtendimentoCRMNotaFiscal').show();
            } else {
                $('#panelAtendimentoCRMNotaFiscal').hide();
            }
        });
    });
}

function loadAtendimentoCRMNotaFiscal() {
    _notaFiscal = new AtendimentoCRMNotaFiscal();
    KoBindings(_notaFiscal, "knockoutAtendimentoCRMNotaFiscal");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirNotaFiscalClick(_notaFiscal.NFe, data)
            }
        }]
    };

    let header = [{ data: "Codigo", visible: false },
    { data: "Etapa", title: "Etapa", width: "10%" },
    { data: "Remessa", title: "Remessa", width: "10%" },
    { data: "NFE", title: "NFe", width: "10" },
    { data: "RazaoSocial", title: "Razão social", width: "10%" },
    { data: "Endereco", title: "Endereço", width: "10%" },
    { data: "Cidade", title: "Cidade", width: "10%" },
    { data: "Estado", title: "UF", width: "10" },
    ];

    _gridNotaFiscal = new BasicDataTable(_notaFiscal.GridNotaFiscal.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridNotaFiscal.CarregarGrid([]);

    BuscarAtendimentoCRMNotaFiscal(_notaFiscal.NFe, atendimentoCRMnotasCallback, _novoAtendimento.NumeroDT, _gridNotaFiscal);
    _notaFiscal.NFe.basicTable = _gridNotaFiscal;

    _gridNotaFiscal.CarregarGrid(new Array());
}

function excluirNotaFiscalClick(knoutNotaFiscal, data) {
    let notaFiscalGrid = knoutNotaFiscal.basicTable.BuscarRegistros();

    for (let i = 0; i < notaFiscalGrid.length; i++) {
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
    recarregarGridNotaFiscal();
}

function atendimentoCRMnotasCallback(notas) {
    let notasGrid = _gridNotaFiscal.BuscarRegistros();
    let notasAtualizado = notasGrid !== undefined && notasGrid != null ? notasGrid.concat(notas) : notas;
    _gridNotaFiscal.CarregarGrid(notasAtualizado);
}

function BuscarAtendimentoCRMNotaFiscal(knout, callbackRetorno, knoutCarga, basicGrid) {
    let idDiv = guid();
    let GridConsulta;    

    function OpcoesKnout () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.BuscarNotasFiscais, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.NotasFiscais, type: types.local });

        this.Numero = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Numero.getFieldDescription() });
        this.Serie = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Serie.getFieldDescription() });
        this.Emitente = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.NotaFiscalEletronica.Emitente.getFieldDescription(), codEntity: ko.observable(0) });
        this.DataEmissao = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao.getFieldDescription() });
        this.Chave = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.NotaFiscalEletronica.Chave.getFieldDescription() });
        this.Carga = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: false, text: Localization.Resources.Consultas.NotaFiscalEletronica.Carga.getFieldDescription(), idBtnSearch: guid() });
        this.CargaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    let knoutOpcoes = new OpcoesKnout();

    let funcaoParamentroDinamico = null;
    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParamentroDinamico = function () {
            if (knoutCarga != null) {
                knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
                knoutOpcoes.Carga.val(knoutCarga.val());
            }
        }
    }

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, true, function () {
            BuscarClientes(knoutOpcoes.Emitente);
        });

    let callback = function (e) {
            divBusca.DefCallback(e);
        }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    let objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AtendimentoCRM/PesquisaNotaFiscal", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {

            if (knout.val().length < 44)
                knoutOpcoes.Numero.val(knout.val());
            else
                knoutOpcoes.Chave.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}