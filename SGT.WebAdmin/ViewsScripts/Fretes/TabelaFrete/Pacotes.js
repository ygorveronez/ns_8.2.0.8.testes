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
/// <reference path="../../Enumeradores/EnumTipoPacote.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPacotes, _pacotes, _pacotesDadosGerais;

var Pacote = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(1), options: EnumTipoPacote.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: 1 });
    this.NumeroInicialPacote = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroInicial.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.NumeroFinalPacote = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroFinal.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ComAjudante = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ComAjudante, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPacoteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoPacote(novoValor);
    });
}

var PacoteDadosGerais = function () {
    this.PermiteValorAdicionalPacoteExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionalPorPacoteExcedente, issue: 714, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ComponenteFretePacotes = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete, visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFretePacotes = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

//*******EVENTOS*******

function loadPacotes() {
    _pacotes = new Pacote();
    KoBindings(_pacotes, "knockoutTabelaFretePacotes");

    _pacotesDadosGerais = new PacoteDadosGerais();
    KoBindings(_pacotesDadosGerais, "knockoutTabelaFretePacotesDadosGerais");

    _tabelaFrete.PermiteValorAdicionalPacoteExcedente = _pacotesDadosGerais.PermiteValorAdicionalPacoteExcedente;
    _tabelaFrete.UtilizarComponenteFretePacotes = _pacotesDadosGerais.UtilizarComponenteFretePacotes;
    _tabelaFrete.ComponenteFretePacotes = _pacotesDadosGerais.ComponenteFretePacotes;

    BuscarComponentesDeFrete(_pacotesDadosGerais.ComponenteFretePacotes);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFretePacotes, _tabelaFrete.UtilizarComponenteFretePacotes);

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirPacoteClick }] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "NumeroInicialPacote", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoPacote, width: "30%" },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "50%" },
        { data: "ComAjudanteDescri", title: Localization.Resources.Fretes.TabelaFrete.ComAjudante, width: "10%" }
    ];

    _gridPacotes = new BasicDataTable(_pacotes.Grid.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    recarregarGridPacote();
}

function criarNumeroEntregaGrid(numeroEntrega) {
    let numeroEntregaGrid = {
        Codigo: numeroEntrega.Codigo.val,
        Tipo: numeroEntrega.Tipo.val,
        NumeroInicialPacote: numeroEntrega.NumeroInicialPacote.val,
        DescricaoTipo: numeroEntrega.Tipo.val == EnumTipoPacote.PorFaixaPacote ?
            Localization.Resources.Fretes.TabelaFrete.PorFaixaPacote :
            Localization.Resources.Fretes.TabelaFrete.ValorFixoPacote,
        ComAjudanteDescri: "",
        ComAjudante: numeroEntrega.ComAjudante.val
    };

    return numeroEntregaGrid;
}

function determinarDescricao(numeroEntrega, numeroEntregaGrid) {
    if (numeroEntrega.Tipo.val == EnumTipoPacote.PorFaixaPacote) {
        let numeroInicial = Globalize.parseInt(numeroEntrega.NumeroInicialPacote.val.toString());
        let numeroFinal = Globalize.parseInt(numeroEntrega.NumeroFinalPacote.val.toString());

        if (numeroInicial > 0 && numeroFinal > 0)
            numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAXPacotes.format(numeroEntrega.NumeroInicialPacote.val, numeroEntrega.NumeroFinalPacote.val);
        else if (numeroFinal <= 0)
            numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeXPacotes.format(numeroEntrega.NumeroInicialPacote.val);
        else
            numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteXPacotes.format(numeroEntrega.NumeroFinalPacote.val);
    } else {
        numeroEntregaGrid.ComAjudanteDescri = "";
        numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ValorFixoPacote;
    }
}

function atualizarComAjudanteDescri(numeroEntrega, numeroEntregaGrid) {
    if (numeroEntrega.ComAjudante.val)
        numeroEntregaGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Sim;
    else
        numeroEntregaGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Nao;
}

function verificarValorAdicionalPacoteExcedente() {
    if (_tabelaFrete.Pacotes.list.some(function (item) { return item.Tipo.val == EnumTipoPacote.ValorFixoPorPacote || Globalize.parseInt(item.NumeroFinalPacote.val.toString()) == 0; })) {
        _pacotesDadosGerais.PermiteValorAdicionalPacoteExcedente.enable(false);
        _pacotesDadosGerais.PermiteValorAdicionalPacoteExcedente.val(false);
    } else {
        _pacotesDadosGerais.PermiteValorAdicionalPacoteExcedente.enable(true);
    }
}

function carregarGrid(data) {
    _gridPacotes.CarregarGrid(data);
}

function recarregarGridPacote() {
    let data = [];

    $.each(_tabelaFrete.Pacotes.list, function (i, numeroEntrega) {
        let numeroEntregaGrid = criarNumeroEntregaGrid(numeroEntrega);
        determinarDescricao(numeroEntrega, numeroEntregaGrid);
        atualizarComAjudanteDescri(numeroEntrega, numeroEntregaGrid);
        data.push(numeroEntregaGrid);
    });

    verificarValorAdicionalPacoteExcedente();
    carregarGrid(data);
}

function excluirPacoteClick(data) {
    for (let i = 0; i < _tabelaFrete.Pacotes.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Pacotes.list[i].Codigo.val) {
            _tabelaFrete.Pacotes.list.splice(i, 1);
            break;
        }
    }

    recarregarGridPacote();
}

function adicionarPacoteClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_pacotes);
    let podeAdicionar = false;

    if (valido) {
        if (_pacotes.Tipo.val() == EnumTipoPacote.PorFaixaPacote) {
            podeAdicionar = verificarSePodeAdicionarPorFaixaPacote();
        } else if (_pacotes.Tipo.val() == EnumTipoPacote.ValorFixoPorPacote) {
            podeAdicionar = verificarSePodeAdicionarValorFixoPorPacote();
        }

        if (podeAdicionar) {
            _pacotes.Codigo.val(guid());
            _tabelaFrete.Pacotes.list.push(SalvarListEntity(_pacotes));
            recarregarGridPacote();
            limparCamposPacotes();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function verificarSePodeAdicionarPorFaixaPacote() {
    let comAjudante = _pacotes.ComAjudante.val();
    let numeroInicial = Globalize.parseInt(_pacotes.NumeroInicialPacote.val().toString());
    let numeroFinal = Globalize.parseInt(_pacotes.NumeroFinalPacote.val().toString());

    if (numeroInicial <= 0 && numeroFinal <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeEntregaInvalido, Localization.Resources.Fretes.TabelaFrete.OsNumerosInicialEFinalDevemSerMaiorQueZero);
        return false;
    } else if (numeroFinal > 0 && numeroFinal < numeroInicial) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeEntregaInvalido, Localization.Resources.Fretes.TabelaFrete.NumeroInicialNaoPodeSerMaiorQueONumeroFinal);
        return false;
    }

    if (numeroFinal == 0)
        numeroFinal = 9999999999999999;

    for (let i = 0; i < _tabelaFrete.Pacotes.list.length; i++) {
        if (_tabelaFrete.Pacotes.list[i].Tipo.val == EnumTipoPacote.PorFaixaPacote) {
            if (!verificarConflitoFaixaPacote(comAjudante, numeroInicial, numeroFinal, i))
                return false;
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
            return false;
        }
    }

    return true;
}

function verificarConflitoFaixaPacote(comAjudante, numeroInicial, numeroFinal, index) {
    let comAjudanteCadastrado = _tabelaFrete.Pacotes.list[index].ComAjudante.val;
    let numeroInicialCadastrado = Globalize.parseInt(_tabelaFrete.Pacotes.list[index].NumeroInicialPacote.val.toString());
    let numeroFinalCadastrado = Globalize.parseInt(_tabelaFrete.Pacotes.list[index].NumeroFinalPacote.val.toString());

    if (numeroFinalCadastrado == 0)
        numeroFinalCadastrado = 9999999999999999;

    if ((numeroInicial >= numeroInicialCadastrado && numeroInicial <= numeroFinalCadastrado) ||
        (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
        (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
        (numeroFinalCadastrado >= numeroInicial && numeroFinalCadastrado <= numeroFinal)) {
        if (comAjudante == comAjudanteCadastrado) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeEntregaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDePacoteEntrouEmConflitoComOPacote.format(_tabelaFrete.Pacotes.list[index].NumeroInicialPacote.val, _tabelaFrete.Pacotes.list[index].NumeroFinalPacote.val))
            return false;
        }
    }

    return true;
}

function verificarSePodeAdicionarValorFixoPorPacote() {
    for (let i = 0; i < _tabelaFrete.Pacotes.list.length; i++) {
        if (_tabelaFrete.Pacotes.list[i].Tipo.val == EnumTipoPacote.ValorFixoPorPacote) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaDeValorFixoPorPacoteJaExiste);
            return false;
        } else if (_tabelaFrete.Pacotes.list[i].Tipo.val == EnumTipoPacote.PorFaixaPacote) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
            return false;
        }
    }

    return true;
}

function limparCamposPacotes() {
    LimparCampos(_pacotes);
}

function ChangeTipoPacote(novoValor) {
    if (novoValor == EnumTipoPacote.ValorFixoPorPacote) {
        _pacotes.NumeroInicialPacote.visible(false);
        _pacotes.NumeroFinalPacote.visible(false);
        _pacotes.ComAjudante.visible(false);
    } else if (novoValor == EnumTipoPacote.PorFaixaPacote) {
        _pacotes.NumeroInicialPacote.visible(true);
        _pacotes.NumeroFinalPacote.visible(true);
        _pacotes.ComAjudante.visible(true);
    }
}