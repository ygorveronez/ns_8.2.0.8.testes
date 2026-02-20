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
/// <reference path="Pessoa.js" />
/// <reference path="../../Consultas/TipoContainer.js" />

var _dadosArmador;
var _gridPessoaArmador;

//*******MAPEAMENTO KNOUCKOUT*******

var PessoaArmadorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoTipoContainer = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoContainer = PropertyEntity({ type: types.map, val: "" });
    this.DiasFreetime = PropertyEntity({ type: types.map, val: "" });
    this.VigenciaDescricao = PropertyEntity({ type: types.map, val: "" });
    this.DataVigenciaInicial = PropertyEntity({ getType: typesKnockout.date, val: "", def: "" });
    this.DataVigenciaFinal = PropertyEntity({ getType: typesKnockout.date, val: "", def: "" });

    this.ValorDiariaAposFreetime = PropertyEntity({ type: types.decimal, val: ko.observable(0), getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
};


var DadosArmador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: "", text: ko.observable(Localization.Resources.Pessoas.Pessoa.TipoContainer), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DiasFreetime = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiasDeFreetime.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 7 });
    this.ValorDiariaAposFreetime = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorDaDiariaAposFreetime.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.DataVigenciaInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataVigenciaInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataVigenciaFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataVigenciaFinal.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.VigenciaDescricao = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.SomenteVigentes = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SomenteVigentes, val: ko.observable(true), def: ko.observable(false), getType: typesKnockout.bool });

    this.DataVigenciaInicial.dateRangeLimit = this.DataVigenciaFinal;
    this.DataVigenciaFinal.dateRangeInit = this.DataVigenciaInicial;

    this.GridPessoaArmador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarPessoaArmador = PropertyEntity({ eventClick: adicionarPessoaArmadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarPessoaArmador = PropertyEntity({ eventClick: atualizarPessoaArmadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirPessoaArmador = PropertyEntity({ eventClick: excluirPessoaArmadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarPessoaArmador = PropertyEntity({ eventClick: LimparCamposPessoaArmador, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    this.SomenteVigentes.val.subscribe(function (valor) {
        recarregarGridPessoaArmador();
    })
}

//*******EVENTOS*******

function loadPessoaArmador() {

    _dadosArmador = new DadosArmador();
    KoBindings(_dadosArmador, "knockoutDadosArmador");

    new BuscarTiposContainer(_dadosArmador.TipoContainer);

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPessoaArmador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoContainer", visible: false },
        { data: "DataVigenciaInicial", visible: false },
        { data: "DataVigenciaFinal", visible: false },
        { data: "DescricaoTipoContainer", title: Localization.Resources.Pessoas.Pessoa.TipoContainer, width: "35%", className: "text-align-left" },
        { data: "DiasFreetime", title: Localization.Resources.Pessoas.Pessoa.DiasDeFreetime, width: "15%", className: "text-align-left" },
        { data: "VigenciaDescricao", title: Localization.Resources.Pessoas.Pessoa.Vigencia, width: "15%", className: "text-align-left" },
        { data: "ValorDiariaAposFreetime", title: Localization.Resources.Pessoas.Pessoa.ValorDaDiariaAposFreetime, width: "15%", className: "text-align-center" },
    ];

    _gridPessoaArmador = new BasicDataTable(_dadosArmador.GridPessoaArmador.idGrid, header, menuOpcoes);
    recarregarGridPessoaArmador();
}

function adicionarPessoaArmadorClick() {
    var tudoCerto = true;
    if (_dadosArmador.DiasFreetime.val() == "")
        tudoCerto = false;
    if (_dadosArmador.ValorDiariaAposFreetime.val() == "")
        tudoCerto = false;
    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var pessoaArmador = new PessoaArmadorMap();
            pessoaArmador.Codigo.val = guid();
            pessoaArmador.DescricaoTipoContainer.val = _dadosArmador.TipoContainer.val();
            pessoaArmador.CodigoTipoContainer.val = _dadosArmador.TipoContainer.codEntity();
            pessoaArmador.DiasFreetime.val = _dadosArmador.DiasFreetime.val();
            pessoaArmador.VigenciaDescricao.val = _dadosArmador.DataVigenciaInicial.val() != '' ? `${Localization.Resources.Gerais.Geral.De} ${_dadosArmador.DataVigenciaInicial.val()} `.concat(_dadosArmador.DataVigenciaFinal.val() != '' ? `${Localization.Resources.Gerais.Geral.Ate} ${_dadosArmador.DataVigenciaFinal.val()}` : '') : _dadosArmador.DataVigenciaFinal.val() != '' ? `${Localization.Resources.Gerais.Geral.Ate} ${_dadosArmador.DataVigenciaFinal.val()}` : '';
            pessoaArmador.DataVigenciaInicial.val = _dadosArmador.DataVigenciaInicial.val();
            pessoaArmador.DataVigenciaFinal.val = _dadosArmador.DataVigenciaFinal.val();
            pessoaArmador.ValorDiariaAposFreetime.val = _dadosArmador.ValorDiariaAposFreetime.val();
            pessoaArmador.VigenciaDescricao.val = pessoaArmador.VigenciaDescricao.val.trim();

            var existeVigenteIgual = validarVigenteIgual(pessoaArmador);

            if (existeVigenteIgual)
                exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.JaExisteUmaDataVigenteCadastrada);
            else {
                _pessoa.ListaDadosArmador.list.push(pessoaArmador);
                recarregarGridPessoaArmador();
            }
        }
        if (!existeVigenteIgual)
            LimparCamposPessoaArmador();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPessoaArmadorClick() {
    var tudoCerto = true;
    if (_dadosArmador.DiasFreetime.val() == "")
        tudoCerto = false;
    if (_dadosArmador.ValorDiariaAposFreetime.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_pessoa.ListaDadosArmador.list, function (i, dadosArmador) {
            if (dadosArmador.Codigo.val == _dadosArmador.Codigo.val()) {

                var existeVigenteIgual = validarVigenteIgual(dadosArmador);

                if (!existeVigenteIgual) {
                    dadosArmador.DiasFreetime.val = _dadosArmador.DiasFreetime.val();
                    dadosArmador.ValorDiariaAposFreetime.val = _dadosArmador.ValorDiariaAposFreetime.val();
                    dadosArmador.VigenciaDescricao.val = _dadosArmador.DataVigenciaInicial.val() != '' ? `${Localization.Resources.Gerais.Geral.De} ${_dadosArmador.DataVigenciaInicial.val()} `.concat(_dadosArmador.DataVigenciaFinal.val() != '' ? `${Localization.Resources.Gerais.Geral.Ate} ${_dadosArmador.DataVigenciaFinal.val()}` : '') : _dadosArmador.DataVigenciaFinal.val() != '' ? `${Localization.Resources.Gerais.Geral.Ate} ${_dadosArmador.DataVigenciaFinal.val()}` : '';
                    dadosArmador.DataVigenciaInicial.val = _dadosArmador.DataVigenciaInicial.val();
                    dadosArmador.DataVigenciaFinal.val = _dadosArmador.DataVigenciaFinal.val();
                    dadosArmador.DescricaoTipoContainer.val = _dadosArmador.TipoContainer.val();
                    dadosArmador.CodigoTipoContainer.val = _dadosArmador.TipoContainer.codEntity();
                }

                return false;
            }
        });
        if (!existeVigenteIgual) {
            LimparCamposPessoaArmador();
            recarregarGridPessoaArmador();
        }
        
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPessoaArmadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirTipoContainerValorDiaria, function () {
        var listaAtualizada = new Array();
        $.each(_pessoa.ListaDadosArmador.list, function (i, dadosArmador) {
            if (dadosArmador.Codigo.val != _dadosArmador.Codigo.val()) {
                listaAtualizada.push(dadosArmador);
            }
        });
        _pessoa.ListaDadosArmador.list = listaAtualizada;
        recarregarGridPessoaArmador();
        LimparCamposPessoaArmador();
    });
}

//*******MÉTODOS*******

function recarregarGridPessoaArmador() {
    var data = new Array();
    var dataAtual = new Date().getTime();

    $.each(_pessoa.ListaDadosArmador.list, function (i, dadosArmador) {
        var pessoaDadosArmador = new Object();

        pessoaDadosArmador.Codigo = dadosArmador.Codigo.val;
        pessoaDadosArmador.CodigoTipoContainer = dadosArmador.CodigoTipoContainer.val;
        pessoaDadosArmador.DescricaoTipoContainer = dadosArmador.DescricaoTipoContainer.val;
        pessoaDadosArmador.DiasFreetime = dadosArmador.DiasFreetime.val;
        pessoaDadosArmador.DataVigenciaInicial = dadosArmador.DataVigenciaInicial.val;
        pessoaDadosArmador.DataVigenciaFinal = dadosArmador.DataVigenciaFinal.val;
        pessoaDadosArmador.VigenciaDescricao = dadosArmador.VigenciaDescricao.val;
        pessoaDadosArmador.ValorDiariaAposFreetime = dadosArmador.ValorDiariaAposFreetime.val;

        data.push(pessoaDadosArmador);
    });

    // "Somente Vigentes"
    if (_dadosArmador.SomenteVigentes.val()) {
        data = data.filter(function (obj) {
            let arrInicial = obj.DataVigenciaInicial.split('/');
            let arrFinal = obj.DataVigenciaFinal.split('/');
            let dataInicial = obj.DataVigenciaInicial != '' ? new Date(parseInt(arrInicial[2]), parseInt(arrInicial[1]) - 1, parseInt(arrInicial[0])) : '';
            let dataFinal = obj.DataVigenciaFinal != '' ? new Date(parseInt(arrFinal[2]), parseInt(arrFinal[1]) - 1, parseInt(arrFinal[0])) : '';

            dataInicial = dataInicial != '' ? dataInicial.getTime() : ''
            dataFinal = dataFinal != '' ? dataFinal.getTime() : ''

            if (dataInicial == '' && dataFinal == '')
                return true;
            if (dataInicial != '' && dataFinal != '') {
                if (dataInicial <= dataAtual && dataAtual <= dataFinal)
                    return true;
                return false;
            }
            if (dataInicial != '' && dataInicial <= dataAtual)
                return true;
            if (dataFinal != '' && dataAtual <= dataFinal)
                return true;
            return false;
        }
        )
    }

    _gridPessoaArmador.CarregarGrid(data);
}

function editarPessoaArmador(data) {
    LimparCamposPessoaArmador();
    $.each(_pessoa.ListaDadosArmador.list, function (i, pessoaArmador) {
        if (pessoaArmador.Codigo.val == data.Codigo) {

            console.log(pessoaArmador.CodigoTipoContainer.val);

            _dadosArmador.Codigo.val(pessoaArmador.Codigo.val);
            _dadosArmador.ValorDiariaAposFreetime.val(pessoaArmador.ValorDiariaAposFreetime.val);
            _dadosArmador.DiasFreetime.val(pessoaArmador.DiasFreetime.val);
            _dadosArmador.TipoContainer.val(pessoaArmador.DescricaoTipoContainer.val);
            _dadosArmador.DataVigenciaInicial.val(pessoaArmador.DataVigenciaInicial.val);
            _dadosArmador.DataVigenciaFinal.val(pessoaArmador.DataVigenciaFinal.val);
            _dadosArmador.VigenciaDescricao.val(pessoaArmador.VigenciaDescricao.val);
            _dadosArmador.TipoContainer.codEntity(pessoaArmador.CodigoTipoContainer.val);

            return false;
        }
    });

    _dadosArmador.AdicionarPessoaArmador.visible(false);
    _dadosArmador.AtualizarPessoaArmador.visible(true);
    _dadosArmador.ExcluirPessoaArmador.visible(true);
    _dadosArmador.CancelarPessoaArmador.visible(true);
}

function LimparCamposPessoaArmador() {
    _dadosArmador.DiasFreetime.val("");
    _dadosArmador.ValorDiariaAposFreetime.val("");
    _dadosArmador.DataVigenciaInicial.val("");
    _dadosArmador.DataVigenciaFinal.val("");
    _dadosArmador.VigenciaDescricao.val("");

    LimparCampoEntity(_dadosArmador.TipoContainer);

    _dadosArmador.AdicionarPessoaArmador.visible(true);
    _dadosArmador.AtualizarPessoaArmador.visible(false);
    _dadosArmador.ExcluirPessoaArmador.visible(false);
    _dadosArmador.CancelarPessoaArmador.visible(false);
}

function validarVigenteIgual(dadosArmador) {
    for (let i = 0; i < _pessoa.ListaDadosArmador.list.length; i++) {
        let obj = _pessoa.ListaDadosArmador.list[i]
        let arrDataOBJInicial = obj.DataVigenciaInicial.val.split('/');
        let arrDataOBJFinal = obj.DataVigenciaFinal.val.split('/');
        let dataOBJInicial = obj.DataVigenciaInicial.val != '' ? new Date(parseInt(arrDataOBJInicial[2]), parseInt(arrDataOBJInicial[1]) - 1, parseInt(arrDataOBJInicial[0])) : '';
        let dataOBJFinal = obj.DataVigenciaFinal.val != '' ? new Date(parseInt(arrDataOBJFinal[2]), parseInt(arrDataOBJFinal[1]) - 1, parseInt(arrDataOBJFinal[0])) : '';

        let arrDataInicial = dadosArmador.DataVigenciaInicial.val.split('/');
        let arrDataFinal = dadosArmador.DataVigenciaFinal.val.split('/');
        let dataInicial = dadosArmador.DataVigenciaInicial.val != '' ? new Date(parseInt(arrDataInicial[2]), parseInt(arrDataInicial[1]) - 1, parseInt(arrDataInicial[0])) : '';
        let dataFinal = dadosArmador.DataVigenciaFinal.val != '' ? new Date(parseInt(arrDataFinal[2]), parseInt(arrDataFinal[1]) - 1, parseInt(arrDataFinal[0])) : '';

        if (dadosArmador.CodigoTipoContainer.val != obj.CodigoTipoContainer.val)
            continue;

        if ((dataOBJInicial == '' && dataOBJFinal == '') || (dataInicial == '' && dataFinal == ''))
            return true;
        if (dataOBJInicial != '' && dataOBJFinal != '') {
            if (dataInicial != '' && dataFinal != '') {
                if (dataOBJInicial <= dataInicial && dataFinal <= dataOBJFinal)
                    return true;
                continue;
            }
            if (dataInicial != '' && dataOBJInicial <= dataInicial && dataInicial <= dataOBJFinal)
                return true;
            if (dataFinal != '' && dataOBJInicial <= dataFinal && dataFinal <= dataOBJFinal)
                return true;
            continue;
        }
        if (dataInicial != '' && dataFinal != '') {
            if (dataOBJInicial != '' && dataOBJInicial <= dataInicial)
                return true;
            if (dataOBJFinal != '' && dataFinal <= dataOBJFinal)
                return true;
            continue;
        }
        if (dataOBJInicial != '') {
            if (dataFinal != '' && dataOBJInicial <= dataFinal)
                return true;
            if (dataInicial != '')
                return true;
            continue;
        }
        if (dataOBJFinal != '') {
            if (dataInicial != '' && dataInicial <= dataOBJFinal)
                return true;
            if (dataFinal != '')
                return true;
            continue;
        }
        continue;
    }
    return false;
}