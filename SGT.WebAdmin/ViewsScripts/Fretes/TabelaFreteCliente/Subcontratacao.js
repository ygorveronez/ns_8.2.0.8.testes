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
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="TabelaFreteCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSubcontratacao, _subcontratacao, _gridSubcontratacaoValorAdicional, _valorAdicionalSubcontratacao, _valorAdicionalSubcontratacaoGeral, _crudSubcontratacao, _subcontratacaoGeral;

var Subcontratacao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.TransportadorTerceiro.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 56 });
    this.PercentualDesconto = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.PorcentagemDesconto.getFieldDescription(), issue: 702, val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, maxlength: 5, required: false, configDecimal: { precision: 2, allowZero: true } });
    this.ValorFixoSubContratacaoParcial = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.ValorFixoParaSubcontratacaoParcial.getFieldDescription(), issue: 730, val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, maxlength: 8, required: false, configDecimal: { precision: 2, allowZero: true } });

    this.Valores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var SubcontratacaoGeral = function () {
    this.PercentualCobrancaPadrao = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.PorcentagemCobrancaPadrao.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: false, configDecimal: { precision: 6, allowZero: false } });
    this.PercentualCobrancaVeiculoFrota = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.PercentualCobrancaVeiculoFrota.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: false, configDecimal: { precision: 6, allowZero: false } });
    this.Valores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var CRUDSubcontratacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarSubcontratacaoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarSubcontratacaoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirSubcontratacaoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposSubcontratacao, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Cancelar, visible: ko.observable(false) });
}

var ValorAdicionalSubcontratacao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Valor = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: true, visible: ko.observable(true) });

    this.AdicionarSub = PropertyEntity({ eventClick: adicionarSubcontratacaoValorAdicionalClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, visible: ko.observable(true) });
    this.AtualizarSub = PropertyEntity({ eventClick: atualizarSubcontratacaoValorAdicionalClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Atualizar, visible: ko.observable(false) });
    this.ExcluirSub = PropertyEntity({ eventClick: excluirSubcontratacaoValorAdicionalClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, visible: ko.observable(false) });
    this.CancelarSub = PropertyEntity({ eventClick: limparCamposSubcontratacaoValorAdicional, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Cancelar, visible: ko.observable(false) });
}

var ValorAdicionalSubcontratacaoGeral = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Valor = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFreteCliente.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: true, visible: ko.observable(true) });

    this.AdicionarGeral = PropertyEntity({ eventClick: adicionarSubcontratacaoValorAdicionalGeralClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, visible: ko.observable(true) });
    this.AtualizarGeral = PropertyEntity({ eventClick: atualizarSubcontratacaoValorAdicionalGeralClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Atualizar, visible: ko.observable(false) });
    this.ExcluirGeral = PropertyEntity({ eventClick: excluirSubcontratacaoValorAdicionalGeralClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, visible: ko.observable(false) });
    this.CancelarGeral = PropertyEntity({ eventClick: limparCamposSubcontratacaoValorAdicionalGeral, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Cancelar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadSubcontratacao() {

    _subcontratacaoGeral = new SubcontratacaoGeral();
    KoBindings(_subcontratacaoGeral, "knockoutSubcontratacaoGeral");

    _subcontratacao = new Subcontratacao();
    KoBindings(_subcontratacao, "knockoutCadastroSubcontratacao");

    _crudSubcontratacao = new CRUDSubcontratacao();
    KoBindings(_crudSubcontratacao, "knockoutCRUDSubcontratacao");

    _valorAdicionalSubcontratacao = new ValorAdicionalSubcontratacao();
    KoBindings(_valorAdicionalSubcontratacao, "knockoutValorAdicionalSubcontratacao");

    _valorAdicionalSubcontratacaoGeral = new ValorAdicionalSubcontratacaoGeral();
    KoBindings(_valorAdicionalSubcontratacaoGeral, "knockoutValorAdicionalSubcontratacaoGeral");

    new BuscarClientes(_subcontratacao.Pessoa, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarJustificativas(_valorAdicionalSubcontratacao.Justificativa, null, null, EnumTipoFinalidadeJustificativa.ContratoFrete);
    new BuscarJustificativas(_valorAdicionalSubcontratacaoGeral.Justificativa, null, null, EnumTipoFinalidadeJustificativa.ContratoFrete);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFreteCliente.Editar, id: guid(), metodo: editarSubcontratacaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Transportador, width: "45%" },
        { data: "PercentualDesconto", title: Localization.Resources.Fretes.TabelaFreteCliente.PorcentagemDesconto, width: "20%" },
        { data: "ValorFixoSubContratacaoParcial", title: Localization.Resources.Fretes.TabelaFreteCliente.ValorFixoSubcontratacaoParcial, width: "20%" }
    ];

    _gridSubcontratacao = new BasicDataTable(_subcontratacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    var menuOpcoesValorAdicional = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFreteCliente.Editar, id: guid(), metodo: editarSubcontratacaoValorAdicionalClick }] };
   
    var headerValorAdicional = [
        { data: "Codigo", visible: false },
        { data: "Justificativa", title: Localization.Resources.Fretes.TabelaFreteCliente.Justificativa, width: "60%" },
        { data: "Valor", title: Localization.Resources.Fretes.TabelaFreteCliente.Valor, width: "30%" }
    ];

    _gridSubcontratacaoValorAdicional = new BasicDataTable(_valorAdicionalSubcontratacao.Grid.id, headerValorAdicional, menuOpcoesValorAdicional, { column: 1, dir: orderDir.asc });

    var menuOpcoesValorAdicionalGeral = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFreteCliente.Editar, id: guid(), metodo: editarSubcontratacaoValorAdicionalGeralClick }] };

    var headerValorAdicionalGeral = [
        { data: "Codigo", visible: false },
        { data: "Justificativa", title: Localization.Resources.Fretes.TabelaFreteCliente.Justificativa, width: "60%" },
        { data: "Valor", title: Localization.Resources.Fretes.TabelaFreteCliente.Valor, width: "30%" }
    ];
    _gridSubcontratacaoValorAdicionalGeral = new BasicDataTable(_valorAdicionalSubcontratacaoGeral.Grid.id, headerValorAdicionalGeral, menuOpcoesValorAdicionalGeral, { column: 1, dir: orderDir.asc });
    
    recarregarGridSubcontratacaoValorAdicional();
    recarregarGridSubcontratacaoValorAdicionalGeral();
    
}

function recarregarGridSubcontratacao() {

    var data = new Array();
    var dataVal = new Array();
    
    $.each(_tabelaFreteCliente.Subcontratacoes.list, function (i, subcontratacao) {
        var subcontratacaoGrid = new Object();
        
        subcontratacaoGrid.Codigo = subcontratacao.Codigo.val;
        subcontratacaoGrid.Descricao = subcontratacao.Pessoa.val;
        subcontratacaoGrid.PercentualDesconto = subcontratacao.PercentualDesconto.val;
        subcontratacaoGrid.ValorFixoSubContratacaoParcial = subcontratacao.ValorFixoSubContratacaoParcial.val;
        _subcontratacao.Pessoa.val(subcontratacao.Pessoa.val);
        _subcontratacao.Pessoa.codEntity(subcontratacao.Pessoa.codEntity);
        _subcontratacao.Valores.list = subcontratacao.Valores.list;

        data.push(subcontratacaoGrid);
    });

    $.each(_tabelaFreteCliente.SubcontratacoesGeral.list, function (i, subcontratacao) {
        _subcontratacaoGeral.Valores.list = subcontratacao.Valores.list;
    });

    _subcontratacaoGeral.PercentualCobrancaPadrao.val(_tabelaFreteCliente.PercentualCobrancaPadrao.val());
    _subcontratacaoGeral.PercentualCobrancaVeiculoFrota.val(_tabelaFreteCliente.PercentualCobrancaVeiculoFrota.val());

   
    _gridSubcontratacao.CarregarGrid(data);
}

function recarregarGridSubcontratacaoValorAdicional() {
    var data = new Array();

    $.each(_subcontratacao.Valores.list, function (i, valor) {
        var valorGrid = new Object();

        valorGrid.Codigo = valor.Codigo.val;
        valorGrid.Justificativa = valor.Justificativa.val;
        valorGrid.Valor = valor.Valor.val;

        data.push(valorGrid);
    });

    recarregarGridSubcontratacaoValorAdicionalGeral();

    _gridSubcontratacaoValorAdicional.CarregarGrid(data);
}

function recarregarGridSubcontratacaoValorAdicionalGeral() {
    var data = new Array();
    
    $.each(_subcontratacaoGeral.Valores.list, function (i, valor) {
        var valorGrid = new Object();

        valorGrid.Codigo = valor.Codigo.val;
        valorGrid.Justificativa = valor.Justificativa.val;
        valorGrid.Valor = valor.Valor.val;

        data.push(valorGrid);
    });

    _gridSubcontratacaoValorAdicionalGeral.CarregarGrid(data);
}

function editarSubcontratacaoClick(data) {

    for (var i = 0; i < _tabelaFreteCliente.Subcontratacoes.list.length; i++) {
        if (data.Codigo == _tabelaFreteCliente.Subcontratacoes.list[i].Codigo.val) {
            var sub = _tabelaFreteCliente.Subcontratacoes.list[i];

            _subcontratacao.Codigo.val(sub.Codigo.val);
            _subcontratacao.Pessoa.val(sub.Pessoa.val);
            _subcontratacao.Pessoa.codEntity(sub.Pessoa.codEntity);
            _subcontratacao.PercentualDesconto.val(sub.PercentualDesconto.val);
            _subcontratacao.ValorFixoSubContratacaoParcial.val(sub.ValorFixoSubContratacaoParcial.val);
            _subcontratacao.Valores.list = sub.Valores.list;

            _crudSubcontratacao.Atualizar.visible(true);
            _crudSubcontratacao.Excluir.visible(true);
            _crudSubcontratacao.Cancelar.visible(true);
            _crudSubcontratacao.Adicionar.visible(false);

            recarregarGridSubcontratacaoValorAdicional();

            break;
        }
    }
    
    for (var i = 0; i < _tabelaFreteCliente.SubcontratacoesGeral.list.length; i++) {
        if (data.Codigo == _tabelaFreteCliente.SubcontratacoesGeral.list[i].Codigo.val) {
            var sub = _tabelaFreteCliente.SubcontratacoesGeral.list[i];

            _subcontratacaoGeral.Valores.list = sub.Valores.list;

            recarregarGridSubcontratacaoValorAdicionalGeral();

            break;
        }
    }
}

function editarSubcontratacaoValorAdicionalClick(data) {

    for (var i = 0; i < _subcontratacao.Valores.list.length; i++) {
        if (data.Codigo == _subcontratacao.Valores.list[i].Codigo.val) {
            var valor = _subcontratacao.Valores.list[i];

            _valorAdicionalSubcontratacao.Codigo.val(valor.Codigo.val);
            _valorAdicionalSubcontratacao.Justificativa.val(valor.Justificativa.val);
            _valorAdicionalSubcontratacao.Justificativa.codEntity(valor.Justificativa.codEntity);
            _valorAdicionalSubcontratacao.Valor.val(valor.Valor.val);
            
            _valorAdicionalSubcontratacao.AtualizarSub.visible(true);
            _valorAdicionalSubcontratacao.ExcluirSub.visible(true);
            _valorAdicionalSubcontratacao.CancelarSub.visible(true);
            _valorAdicionalSubcontratacao.AdicionarSub.visible(false);

            break;
        }
    }
}

function editarSubcontratacaoValorAdicionalGeralClick(data) {

    for (var i = 0; i < _subcontratacaoGeral.Valores.list.length; i++) {
        if (data.Codigo == _subcontratacaoGeral.Valores.list[i].Codigo.val) {
            var valor = _subcontratacaoGeral.Valores.list[i];

            _valorAdicionalSubcontratacaoGeral.Codigo.val(valor.Codigo.val);
            _valorAdicionalSubcontratacaoGeral.Justificativa.val(valor.Justificativa.val);
            _valorAdicionalSubcontratacaoGeral.Justificativa.codEntity(valor.Justificativa.codEntity);
            _valorAdicionalSubcontratacaoGeral.Valor.val(valor.Valor.val);

            _valorAdicionalSubcontratacaoGeral.AtualizarGeral.visible(true);
            _valorAdicionalSubcontratacaoGeral.ExcluirGeral.visible(true);
            _valorAdicionalSubcontratacaoGeral.CancelarGeral.visible(true);
            _valorAdicionalSubcontratacaoGeral.AdicionarGeral.visible(false);

            break;
        }
    }
}

function excluirSubcontratacaoClick() {
    for (var i = 0; i < _tabelaFreteCliente.Subcontratacoes.list.length; i++) {
        if (_subcontratacao.Codigo.val() == _tabelaFreteCliente.Subcontratacoes.list[i].Codigo.val) {
            _tabelaFreteCliente.Subcontratacoes.list.splice(i, 1);
            break;
        }
    }

    limparCamposSubcontratacao();
    recarregarGridSubcontratacao();
}

function excluirSubcontratacaoValorAdicionalClick() {
    for (var i = 0; i < _subcontratacao.Valores.list.length; i++) {
        if (_valorAdicionalSubcontratacao.Codigo.val() == _subcontratacao.Valores.list[i].Codigo.val) {
            _subcontratacao.Valores.list.splice(i, 1);
            break;
        }
    }

    limparCamposSubcontratacaoValorAdicional();
    recarregarGridSubcontratacaoValorAdicional();
}

function excluirSubcontratacaoValorAdicionalGeralClick() {
    for (var i = 0; i < _subcontratacaoGeral.Valores.list.length; i++) {
        if (_valorAdicionalSubcontratacaoGeral.Codigo.val() == _subcontratacaoGeral.Valores.list[i].Codigo.val) {
            _subcontratacaoGeral.Valores.list.splice(i, 1);
            break;
        }
    }

    limparCamposSubcontratacaoValorAdicionalGeral();
    recarregarGridSubcontratacaoValorAdicionalGeral();
}

function adicionarSubcontratacaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_subcontratacao);

    if (valido) {

        for (var i = 0; i < _tabelaFreteCliente.Subcontratacoes.list.length; i++) {
            if (_tabelaFreteCliente.Subcontratacoes.list[i].Pessoa.codEntity == _subcontratacao.Pessoa.codEntity()) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.TransportadorJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.OTransportadorJaEstaCadastrado.format(_subcontratacao.Pessoa.val()));
                return;
            }
        }

        _tabelaFreteCliente.Subcontratacoes.list.push(SalvarListEntity(_subcontratacao));

        recarregarGridSubcontratacao();

        $("#" + _subcontratacao.Pessoa.id).focus();

        limparCamposSubcontratacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCamposObrigatorios);
    }
}

function adicionarSubcontratacaoValorAdicionalClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_valorAdicionalSubcontratacao);
    if (valido) {
        
        for (var i = 0; i < _subcontratacao.Valores.list.length; i++) {
            if (_subcontratacao.Valores.list[i].Justificativa.codEntity == _valorAdicionalSubcontratacao.Justificativa.codEntity()) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.JustificativaJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.AJustificativaJaEstaCadastrada.format(_valorAdicionalSubcontratacao.Justificativa.val()));
                return;
            }
        }

        _valorAdicionalSubcontratacao.Codigo.val(guid());

        _subcontratacao.Valores.list.push(SalvarListEntity(_valorAdicionalSubcontratacao));

        _tabelaFreteCliente.Subcontratacoes.list.push(SalvarListEntity(_subcontratacao));
        recarregarGridSubcontratacaoValorAdicional();

        limparCamposSubcontratacaoValorAdicional();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCampsObrigatorios);
    }
}

function adicionarSubcontratacaoValorAdicionalGeralClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_valorAdicionalSubcontratacaoGeral);
    
    if (valido) {
        for (var i = 0; i < _subcontratacaoGeral.Valores.list.length; i++) {
            if (_subcontratacaoGeral.Valores.list[i].Justificativa.codEntity == _valorAdicionalSubcontratacaoGeral.Justificativa.codEntity()) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.JustificativaJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.AJustificativaJaEstaCadastrada.format(_valorAdicionalSubcontratacaoGeral.Justificativa.val()));
                return;
            }
        }

        _valorAdicionalSubcontratacaoGeral.Codigo.val(guid());

        _subcontratacaoGeral.Valores.list.push(SalvarListEntity(_valorAdicionalSubcontratacaoGeral));

        _tabelaFreteCliente.SubcontratacoesGeral.list.push(SalvarListEntity(_subcontratacaoGeral));
        recarregarGridSubcontratacaoValorAdicionalGeral();

        limparCamposSubcontratacaoValorAdicionalGeral();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCampsObrigatorios);
    }
}

function atualizarSubcontratacaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_subcontratacao);

    if (valido) {

        for (var i = 0; i < _tabelaFreteCliente.Subcontratacoes.list.length; i++) {
            if (_tabelaFreteCliente.Subcontratacoes.list[i].Pessoa.codEntity == _subcontratacao.Pessoa.codEntity().val() && _subcontratacao.Codigo.val() != _tabelaFreteCliente.Subcontratacoes.list[i].Codigo.val) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.TransportadorJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.OTransportadorJaEstaCadastrado.format(_subcontratacao.Pessoa.val()));
                return;
            }
        }

        for (var i = 0; i < _tabelaFreteCliente.Subcontratacoes.list.length; i++) {
            if (_subcontratacao.Codigo.val() == _tabelaFreteCliente.Subcontratacoes.list[i].Codigo.val) {
                _tabelaFreteCliente.Subcontratacoes.list[i] = SalvarListEntity(_subcontratacao);
                break;
            }
        }

        recarregarGridSubcontratacao();

        $("#" + _subcontratacao.Pessoa.id).focus();

        limparCamposSubcontratacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCampsObrigatorios);
    }
}

function atualizarSubcontratacaoValorAdicionalClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_valorAdicionalSubcontratacao);

    if (valido) {

        for (var i = 0; i < _subcontratacao.Valores.list.length; i++) {
            if (_subcontratacao.Valores.list[i].Justificativa.codEntity == _valorAdicionalSubcontratacao.Justificativa.codEntity() && _valorAdicionalSubcontratacao.Codigo.val() != _subcontratacao.Valores.list[i].Codigo.val) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.JustificativaJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.AJustificativaJaEstaCadastrada.format(_valorAdicionalSubcontratacao.Justificativa.val()));
                return;
            }
        }

        for (var i = 0; i < _subcontratacao.Valores.list.length; i++) {
            if (_valorAdicionalSubcontratacao.Codigo.val() == _subcontratacao.Valores.list[i].Codigo.val) {
                _subcontratacao.Valores.list[i] = SalvarListEntity(_valorAdicionalSubcontratacao);
                break;
            }
        }

        recarregarGridSubcontratacaoValorAdicional();

        limparCamposSubcontratacaoValorAdicional();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCampsObrigatorios);
    }
}

function atualizarSubcontratacaoValorAdicionalGeralClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_valorAdicionalSubcontratacaoGeral);

    if (valido) {

        for (var i = 0; i < _subcontratacaoGeral.Valores.list.length; i++) {
            if (_subcontratacaoGeral.Valores.list[i].Justificativa.codEntity == _valorAdicionalSubcontratacaoGeral.Justificativa.codEntity() && _valorAdicionalSubcontratacaoGeral.Codigo.val() != _subcontratacaoGeral.Valores.list[i].Codigo.val) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.JustificativaJaExistente, Localization.Resources.Fretes.TabelaFreteCliente.AJustificativaJaEstaCadastrada.format(_valorAdicionalSubcontratacaoGeral.Justificativa.val()));
                return;
            }
        }

        for (var i = 0; i < _subcontratacaoGeral.Valores.list.length; i++) {
            if (_valorAdicionalSubcontratacaoGeral.Codigo.val() == _subcontratacaoGeral.Valores.list[i].Codigo.val) {
                _subcontratacaoGeral.Valores.list[i] = SalvarListEntity(_valorAdicionalSubcontratacaoGeral);
                 
                break;
            }
        }
        
        recarregarGridSubcontratacaoValorAdicionalGeral();

        limparCamposSubcontratacaoValorAdicionalGeral();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFreteCliente.CamposObrigatorios, Localization.Resources.Fretes.TabelaFreteCliente.InformeOsCampsObrigatorios);
    }
}

function limparCamposSubcontratacao() {
    LimparCampos(_subcontratacao);
    _crudSubcontratacao.Excluir.visible(false);
    _crudSubcontratacao.Atualizar.visible(false);
    _crudSubcontratacao.Cancelar.visible(false);
    _crudSubcontratacao.Adicionar.visible(true);

    limparCamposSubcontratacaoValorAdicional();
    recarregarGridSubcontratacaoValorAdicional();

    limparCamposSubcontratacaoValorAdicionalGeral();
    recarregarGridSubcontratacaoValorAdicionalGeral();
}

function limparCamposSubcontratacaoValorAdicional() {
    LimparCampos(_valorAdicionalSubcontratacao);
    _valorAdicionalSubcontratacao.ExcluirSub.visible(false);
    _valorAdicionalSubcontratacao.AtualizarSub.visible(false);
    _valorAdicionalSubcontratacao.CancelarSub.visible(false);
    _valorAdicionalSubcontratacao.AdicionarSub.visible(true);
}

function limparCamposSubcontratacaoValorAdicionalGeral() {
    LimparCampos(_valorAdicionalSubcontratacaoGeral);
    _valorAdicionalSubcontratacaoGeral.ExcluirGeral.visible(false);
    _valorAdicionalSubcontratacaoGeral.AtualizarGeral.visible(false);
    _valorAdicionalSubcontratacaoGeral.CancelarGeral.visible(false);
    _valorAdicionalSubcontratacaoGeral.AdicionarGeral.visible(true);
}