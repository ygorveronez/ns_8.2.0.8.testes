/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Bonificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaComissaoMotorista, _tabelaComissaoMotorista, _pesquisaTabelaComissaoMotorista, _crudTabelaComissaoMotorista;

var _gridModelo, _gridSegmento, _gridTipoOperacao;

var PesquisaTabelaComissaoMotorista = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaComissaoMotorista.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TabelaComissaoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.PercentualComissaoPadrao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: "*% Comissão Padrão:", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataVigencia = PropertyEntity({ text: "*Vigência:", getType: typesKnockout.date, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: true });

    this.Segmentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridSegmento = PropertyEntity({ type: types.local });
    this.AdicionarSegmento = PropertyEntity({ type: types.event, text: "Adicionar Segmento", idBtnSearch: guid() });

    this.Modelos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridModelo = PropertyEntity({ type: types.local });
    this.AdicionarModelo = PropertyEntity({ type: types.event, text: "Adicionar Modelo", idBtnSearch: guid() });

    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridTipoOperacao = PropertyEntity({ type: types.local });
    this.AdicionarTipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid() });

    //Listas aba Bonificações
    this.AtivarBonificacaoMediaCombustivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Medias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AtivarBonificacaoRotaFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.RotasFretes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AtivarBonificacaoRepresentacaoCombustivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Representacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AtivarBonificacaoFaturamentoDia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.FaturamentoDia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDTabelaComissaoMotorista = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTabelaComissaoMotorista() {
    _pesquisaTabelaComissaoMotorista = new PesquisaTabelaComissaoMotorista();
    KoBindings(_pesquisaTabelaComissaoMotorista, "knockoutPesquisaTabelaComissaoMotorista", false, _pesquisaTabelaComissaoMotorista.Pesquisar.id);

    _tabelaComissaoMotorista = new TabelaComissaoMotorista();
    KoBindings(_tabelaComissaoMotorista, "knockoutCadastroTabelaComissaoMotorista");

    _crudTabelaComissaoMotorista = new CRUDTabelaComissaoMotorista();
    KoBindings(_crudTabelaComissaoMotorista, "knockoutCRUDTabelaComissaoMotorista");

    HeaderAuditoria("TabelaComissaoMotorista", _tabelaComissaoMotorista);

    loadGridSegmento();
    loadGridModelo();
    loadGridTipoOperacao();
    loadBonificacao();

    limparCamposTabelaComissaoMotorista();
    buscarTabelaComissaoMotorista();
}

function loadGridSegmento() {
    const menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirSegmentoClick(_tabelaComissaoMotorista.AdicionarSegmento, data);
            }
        }]
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" }
    ];

    _gridSegmento = new BasicDataTable(_tabelaComissaoMotorista.GridSegmento.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarSegmentoVeiculo(_tabelaComissaoMotorista.AdicionarSegmento, null, null, function (r) {
        if (r !== null) {
            const segmentos = _gridSegmento.BuscarRegistros();
            for (let i = 0; i < r.length; i++)
                segmentos.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });
            _gridSegmento.CarregarGrid(segmentos);
        }
    }, _gridSegmento);

    _tabelaComissaoMotorista.AdicionarSegmento.basicTable = _gridSegmento;

    RecarregarGridSegmento();
}

function ExcluirSegmentoClick(knoutSegmento, data) {
    const segmentoGrid = knoutSegmento.basicTable.BuscarRegistros();

    for (let i = 0; i < segmentoGrid.length; i++) {
        if (data.Codigo === segmentoGrid[i].Codigo) {
            segmentoGrid.splice(i, 1);
            break;
        }
    }

    knoutSegmento.basicTable.CarregarGrid(segmentoGrid);
}

function RecarregarGridSegmento() {

    const data = new Array();
    let segmentos = new Array();

    if (_tabelaComissaoMotorista.Segmentos.val() instanceof Array)
        segmentos = _tabelaComissaoMotorista.Segmentos.val();
    else if (!string.IsNullOrWhiteSpace(_tabelaComissaoMotorista.Segmentos.val()))
        segmentos = JSON.parse(_tabelaComissaoMotorista.Segmentos.val());
        
    for (let i = 0; i < segmentos.length; i++) {
        const segmento = segmentos[i];

        data.push({
            Codigo: segmento.Segmento.Codigo,
            Descricao: segmento.Segmento.Descricao,
        });
    };

    _gridSegmento.CarregarGrid(data);
}

function loadGridModelo() {

    const menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirModeloClick(_tabelaComissaoMotorista.AdicionarModelo, data);
            }
        }]
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" }
    ];

    _gridModelo = new BasicDataTable(_tabelaComissaoMotorista.GridModelo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarModelosVeiculo(_tabelaComissaoMotorista.AdicionarModelo, null, null, function (r) {
        if (r !== null) {
            const segmentos = _gridModelo.BuscarRegistros();
            for (let i = 0; i < r.length; i++)
                segmentos.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });
            _gridModelo.CarregarGrid(segmentos);
        }
    }, null, _gridModelo);

    _tabelaComissaoMotorista.AdicionarModelo.basicTable = _gridModelo;

    RecarregarGridModelo();
}

function ExcluirModeloClick(knoutModelo, data) {
    const segmentoGrid = knoutModelo.basicTable.BuscarRegistros();

    for (let i = 0; i < segmentoGrid.length; i++) {
        if (data.Codigo === segmentoGrid[i].Codigo) {
            segmentoGrid.splice(i, 1);
            break;
        }
    }

    knoutModelo.basicTable.CarregarGrid(segmentoGrid);
}

function RecarregarGridModelo() {
    const data = new Array();
    let modelos = new Array();

    if (_tabelaComissaoMotorista.Modelos.val() instanceof Array)
        modelos = _tabelaComissaoMotorista.Modelos.val();
    else if (!string.IsNullOrWhiteSpace(_tabelaComissaoMotorista.Modelos.val()))
        modelos = JSON.parse(_tabelaComissaoMotorista.Modelos.val());
        
    for (let i = 0; i < modelos.length; i++) {
        const modelo = modelos[i];

        data.push({
            Codigo: modelo.Modelo.Codigo,
            Descricao: modelo.Modelo.Descricao,
        });
    };

    _gridModelo.CarregarGrid(data);
}

function loadGridTipoOperacao() {
    const menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClick(_tabelaComissaoMotorista.AdicionarTipoOperacao, data);
            }
        }]
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_tabelaComissaoMotorista.GridTipoOperacao.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarTiposOperacao(_tabelaComissaoMotorista.AdicionarTipoOperacao, function (r) {
        if (r !== null) {
            const tiposOperacao = _gridTipoOperacao.BuscarRegistros();
            for (let i = 0; i < r.length; i++)
                tiposOperacao.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });
            _gridTipoOperacao.CarregarGrid(tiposOperacao);
        }
    }, null, null, _gridTipoOperacao);

    _tabelaComissaoMotorista.AdicionarTipoOperacao.basicTable = _gridTipoOperacao;

    RecarregarGridTipoOperacao();
}

function ExcluirTipoOperacaoClick(knoutTipoOperacao, data) {
    const tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (let i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo === tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

function RecarregarGridTipoOperacao() {
    const data = new Array();
    let tiposOperacao = new Array();

    if (_tabelaComissaoMotorista.TiposOperacao.val() instanceof Array)
        tiposOperacao = _tabelaComissaoMotorista.TiposOperacao.val();
    else if (!string.IsNullOrWhiteSpace(_tabelaComissaoMotorista.TiposOperacao.val()))
        tiposOperacao = JSON.parse(_tabelaComissaoMotorista.TiposOperacao.val());

    for (let i = 0; i < tiposOperacao.length; i++) {
        const tipoOperacao = tiposOperacao[i];

        data.push({
            Codigo: tipoOperacao.TipoOperacao.Codigo,
            Descricao: tipoOperacao.TipoOperacao.Descricao,
        });
    };

    _gridTipoOperacao.CarregarGrid(data);
}

function adicionarClick(e, sender) {
    preencherListasSelecaoTabela();

    Salvar(_tabelaComissaoMotorista, "TabelaComissaoMotorista/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaComissaoMotorista.CarregarGrid();
                limparCamposTabelaComissaoMotorista();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoTabela();

    Salvar(_tabelaComissaoMotorista, "TabelaComissaoMotorista/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaComissaoMotorista.CarregarGrid();
                limparCamposTabelaComissaoMotorista();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function cancelarClick(e) {
    limparCamposTabelaComissaoMotorista();
}

//*******MÉTODOS*******

function preencherListasSelecaoTabela() {
    const modelos = new Array();
    const segmentos = new Array();
    const tiposOperacao = new Array();

    const gridModelos = _tabelaComissaoMotorista.AdicionarModelo.basicTable.BuscarRegistros();
    const gridSegmentos = _tabelaComissaoMotorista.AdicionarSegmento.basicTable.BuscarRegistros();
    const gridTiposOperacao = _tabelaComissaoMotorista.AdicionarTipoOperacao.basicTable.BuscarRegistros();

    for (let i = 0; i < gridModelos.length; i++)
        modelos.push({ Modelo: gridModelos[i] });

    for (let i = 0; i < gridSegmentos.length; i++)
        segmentos.push({ Segmento: gridSegmentos[i] });

    for (let i = 0; i < gridTiposOperacao.length; i++)
        tiposOperacao.push({ TipoOperacao: gridTiposOperacao[i] });


    _tabelaComissaoMotorista.FaturamentoDia.val(JSON.stringify(_bonificacao.FaturamentoDia.list));
    _tabelaComissaoMotorista.Medias.val(JSON.stringify(_bonificacao.Medias.list));
    _tabelaComissaoMotorista.RotasFretes.val(JSON.stringify(_bonificacao.RotasFretes.list));
    _tabelaComissaoMotorista.Representacoes.val(JSON.stringify(_bonificacao.Representacaos.list));

    _tabelaComissaoMotorista.Modelos.val(JSON.stringify(modelos));
    _tabelaComissaoMotorista.Segmentos.val(JSON.stringify(segmentos));
    _tabelaComissaoMotorista.TiposOperacao.val(JSON.stringify(tiposOperacao));

    _tabelaComissaoMotorista.AtivarBonificacaoMediaCombustivel.val(_bonificacao.AtivarBonificacaoMediaCombustivel.val());

    _tabelaComissaoMotorista.AtivarBonificacaoRotaFrete.val(_bonificacao.AtivarBonificacaoRotaFrete.val());

    _tabelaComissaoMotorista.AtivarBonificacaoFaturamentoDia.val(_bonificacao.AtivarBonificacaoFaturamentoDia.val());

    _tabelaComissaoMotorista.AtivarBonificacaoRepresentacaoCombustivel.val(_bonificacao.AtivarBonificacaoRepresentacaoCombustivel.val());
}

function buscarTabelaComissaoMotorista() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaComissaoMotorista, tamanho: "15", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] }

    _gridTabelaComissaoMotorista = new GridView(_pesquisaTabelaComissaoMotorista.Pesquisar.idGrid, "TabelaComissaoMotorista/Pesquisa", _pesquisaTabelaComissaoMotorista, menuOpcoes, null);
    _gridTabelaComissaoMotorista.CarregarGrid();
}

function editarTabelaComissaoMotorista(tabelaComissaoMotoristaGrid) {
    limparCamposTabelaComissaoMotorista();
    _tabelaComissaoMotorista.Codigo.val(tabelaComissaoMotoristaGrid.Codigo);
    BuscarPorCodigo(_tabelaComissaoMotorista, "TabelaComissaoMotorista/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaComissaoMotorista.ExibirFiltros.visibleFade(false);
        _crudTabelaComissaoMotorista.Atualizar.visible(true);
        _crudTabelaComissaoMotorista.Cancelar.visible(true);
        _crudTabelaComissaoMotorista.Adicionar.visible(false);

        const dataBonificacao = { Data: arg.Data };
        PreencherObjetoKnout(_bonificacao, dataBonificacao);

        _bonificacao.Medias.list = new Array();
        _bonificacao.Medias.list = arg.Data.Medias;

        _bonificacao.RotasFretes.list = new Array();
        _bonificacao.RotasFretes.list = arg.Data.RotasFretes;

        _bonificacao.FaturamentoDia.list = new Array();
        _bonificacao.FaturamentoDia.list = arg.Data.FaturamentoDia;

        _bonificacao.Representacaos.list = new Array();
        _bonificacao.Representacaos.list = arg.Data.Representacaos;

        RecarregarGridMedia();
        RecarregarGridRotaFrete();
        RecarregarGridFaturamentoDia();
        RecarregarGridRepresentacao();
        RecarregarGridSegmento();
        RecarregarGridModelo();
        RecarregarGridTipoOperacao();
    }, null);
}

function limparCamposTabelaComissaoMotorista() {
    _crudTabelaComissaoMotorista.Atualizar.visible(false);
    _crudTabelaComissaoMotorista.Cancelar.visible(false);
    _crudTabelaComissaoMotorista.Adicionar.visible(true);
    LimparCampos(_tabelaComissaoMotorista);
    LimparCamposBonificacao();

    RecarregarGridMedia();
    RecarregarGridRotaFrete();
    RecarregarGridFaturamentoDia();
    RecarregarGridRepresentacao();
    RecarregarGridSegmento();
    RecarregarGridModelo();
    RecarregarGridTipoOperacao();

    Global.ResetarAbas();
}
