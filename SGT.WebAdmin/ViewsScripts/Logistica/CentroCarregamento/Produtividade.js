/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CentroCarregamento.js" />

var _produtividadeCarregamento, _gridProdutividadeCarregamento, _produtividade;

var ProdutividadeCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Transportador, idBtnSearch: guid() });
    this.Picking = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Picking.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true });
    this.Separacao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Separacao.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 5, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true });
    this.Carregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Carregamento.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 5, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true });
    this.HorasTrabalho = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.HorasTrabalho.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 5, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProdutividadeCarregamento, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarProdutividade, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarProdutividadeCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirProdutividadeCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposProdutividadeCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

var Produtividade = function () {
    this.HorasTrabalho = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.HorasDeTrabalho.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" } });
};

function loadProdutividade() {
    _produtividadeCarregamento = new ProdutividadeCarregamento();
    KoBindings(_produtividadeCarregamento, "knockoutProdutividadeCarregamento");

    _produtividade = new Produtividade();
    KoBindings(_produtividade, "knockoutProdutividade");

    _centroCarregamento.HorasTrabalho = _produtividade.HorasTrabalho;

    new BuscarTiposOperacao(_produtividadeCarregamento.TipoOperacao);
    new BuscarGruposPessoas(_produtividadeCarregamento.GrupoPessoas);
    new BuscarEmpresa(_produtividadeCarregamento.Transportador);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarProdutividadeCarregamento }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "GrupoPessoas", title: Localization.Resources.Logistica.CentroCarregamento.GrupoDePessoas, width: "25%" },
        { data: "TipoOperacao", title: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao, width: "25%" },
        { data: "Transportador", title: Localization.Resources.Logistica.CentroCarregamento.Transportador, width: "25%" },
        { data: "Picking", title: Localization.Resources.Logistica.CentroCarregamento.Picking, width: "8%" },
        { data: "Separacao", title: Localization.Resources.Logistica.CentroCarregamento.Separacao, width: "8%" },
        { data: "Carregamento", title: Localization.Resources.Logistica.CentroCarregamento.Carregamento, width: "8%" }
    ];

    _gridProdutividadeCarregamento = new BasicDataTable(_produtividadeCarregamento.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc });

    recarregarGridProdutividadeCarregamento();
}

function recarregarGridProdutividadeCarregamento() {
    var data = new Array();

    $.each(_centroCarregamento.ProdutividadeCarregamentos.list, function (i, produtividadeCarregamento) {
        var gridProdutividadeCarregamento = new Object();

        gridProdutividadeCarregamento.Codigo = produtividadeCarregamento.Codigo.val;
        gridProdutividadeCarregamento.GrupoPessoas = produtividadeCarregamento.GrupoPessoas.val;
        gridProdutividadeCarregamento.TipoOperacao = produtividadeCarregamento.TipoOperacao.val;
        gridProdutividadeCarregamento.Transportador = produtividadeCarregamento.Transportador.val;
        gridProdutividadeCarregamento.Picking = produtividadeCarregamento.Picking.val;
        gridProdutividadeCarregamento.Separacao = produtividadeCarregamento.Separacao.val;
        gridProdutividadeCarregamento.Carregamento = produtividadeCarregamento.Carregamento.val;
        gridProdutividadeCarregamento.HorasTrabalho = produtividadeCarregamento.HorasTrabalho.val;

        data.push(gridProdutividadeCarregamento);
    });

    _gridProdutividadeCarregamento.CarregarGrid(data);
}

function adicionarProdutividadeCarregamento(e, sender) {
    if (!ValidarCamposObrigatorios(_produtividadeCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var existeTipoOperacaoPorGrupoPessoas = false;

    $.each(_centroCarregamento.ProdutividadeCarregamentos.list, function (i, frequenciasCarregamento) {
        if (frequenciasCarregamento.TipoOperacao.codEntity == _produtividadeCarregamento.TipoOperacao.val() && frequenciasCarregamento.GrupoPessoas.codEntity == _produtividadeCarregamento.GrupoPessoas.val()) {
            existeTipoOperacaoPorGrupoPessoas = true;
            return;
        }
    });

    if (existeTipoOperacaoPorGrupoPessoas) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.EsteTipoDeOperacaoJaFoiInseridoParaEsteCliente.format(_produtividadeCarregamento.TipoOperacao.val(), _produtividadeCarregamento.GrupoPessoas.val()));
        return;
    }

    _produtividadeCarregamento.Codigo.val(guid());
    _centroCarregamento.ProdutividadeCarregamentos.list.push(SalvarListEntity(_produtividadeCarregamento));

    limparCamposProdutividadeCarregamento();
}

function editarProdutividadeCarregamento(data) {
    for (var i = 0; i < _centroCarregamento.ProdutividadeCarregamentos.list.length; i++) {
        if (data.Codigo == _centroCarregamento.ProdutividadeCarregamentos.list[i].Codigo.val) {

            var produtividadeCarregamento = _centroCarregamento.ProdutividadeCarregamentos.list[i];

            _produtividadeCarregamento.Codigo.val(produtividadeCarregamento.Codigo.val);
            _produtividadeCarregamento.GrupoPessoas.val(produtividadeCarregamento.GrupoPessoas.val);
            _produtividadeCarregamento.GrupoPessoas.codEntity(produtividadeCarregamento.GrupoPessoas.codEntity);
            _produtividadeCarregamento.TipoOperacao.val(produtividadeCarregamento.TipoOperacao.val);
            _produtividadeCarregamento.TipoOperacao.codEntity(produtividadeCarregamento.TipoOperacao.codEntity);
            _produtividadeCarregamento.Transportador.val(produtividadeCarregamento.Transportador.val);
            _produtividadeCarregamento.Transportador.codEntity(produtividadeCarregamento.Transportador.codEntity);
            _produtividadeCarregamento.Picking.val(produtividadeCarregamento.Picking.val);
            _produtividadeCarregamento.Separacao.val(produtividadeCarregamento.Separacao.val);
            _produtividadeCarregamento.Carregamento.val(produtividadeCarregamento.Carregamento.val);
            _produtividadeCarregamento.HorasTrabalho.val(produtividadeCarregamento.HorasTrabalho.val);

            _produtividadeCarregamento.Adicionar.visible(false);
            _produtividadeCarregamento.Atualizar.visible(true);
            _produtividadeCarregamento.Excluir.visible(true);
            _produtividadeCarregamento.Cancelar.visible(true);

            break;
        }
    }
}

function atualizarProdutividadeCarregamento() {
    if (!ValidarCamposObrigatorios(_produtividadeCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var existeTipoOperacaoPorGrupoPessoas = false;

    $.each(_centroCarregamento.ProdutividadeCarregamentos.list, function (i, frequenciasCarregamento) {
        if (frequenciasCarregamento.TipoOperacao.codEntity == _produtividadeCarregamento.TipoOperacao.val() && frequenciasCarregamento.GrupoPessoas.codEntity == _produtividadeCarregamento.GrupoPessoas.val() && _produtividadeCarregamento.Codigo.val() != frequenciasCarregamento.Codigo.val) {
            existeTipoOperacaoPorGrupoPessoas = true;
            return;
        }
    });

    if (existeTipoOperacaoPorGrupoPessoas) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.EsteTipoDeOperacaoJaFoiInseridoParaEsteCliente.format(_produtividadeCarregamento.TipoOperacao.val(), _produtividadeCarregamento.GrupoPessoas.val()));
        return;
    }

    for (var i = 0; i < _centroCarregamento.ProdutividadeCarregamentos.list.length; i++) {
        if (_produtividadeCarregamento.Codigo.val() == _centroCarregamento.ProdutividadeCarregamentos.list[i].Codigo.val) {
            _centroCarregamento.ProdutividadeCarregamentos.list[i] = SalvarListEntity(_produtividadeCarregamento);
            break;
        }
    }

    limparCamposProdutividadeCarregamento();
}

function excluirProdutividadeCarregamento(data) {
    for (var i = 0; i < _centroCarregamento.ProdutividadeCarregamentos.list.length; i++) {
        if (_produtividadeCarregamento.Codigo.val() == _centroCarregamento.ProdutividadeCarregamentos.list[i].Codigo.val) {
            _centroCarregamento.ProdutividadeCarregamentos.list.splice(i, 1);
            break;
        }
    }

    limparCamposProdutividadeCarregamento();
}

function limparCamposProdutividadeCarregamento() {
    LimparCampos(_produtividadeCarregamento);
    _produtividadeCarregamento.Adicionar.visible(true);
    _produtividadeCarregamento.Atualizar.visible(false);
    _produtividadeCarregamento.Excluir.visible(false);
    _produtividadeCarregamento.Cancelar.visible(false);

    recarregarGridProdutividadeCarregamento();
}