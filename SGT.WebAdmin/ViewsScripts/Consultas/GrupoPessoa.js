/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Cliente.js" />
/// <reference path="../Enumeradores/EnumTipoGrupoPessoas.js" />

var BuscarGruposPessoas = function (knout, callbackRetorno, comFiltroPorCliente, basicGrid, knoutTipoGrupoPesssoa, isFiltrarPorConfiguracaoOperadorLogistica) {

    var idDiv = guid();
    var GridConsulta;
    var buscaCliente;

    comFiltroPorCliente = comFiltroPorCliente == null ? true : comFiltroPorCliente;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.GrupoPessoa.PesquisarGruposPessoas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.GrupoPessoa.GruposPessoas, type: types.local });

        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Cliente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: comFiltroPorCliente });
        this.RaizCNPJ = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.GrupoPessoa.RaizCNPJ.getFieldDescription() });

        this.TipoGrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.GrupoPessoa.TipoGrupoPessoa.getFieldDescription(), idBtnSearch: guid(), visible: false, val: ko.observable(0), def: ko.observable(0) });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        if (comFiltroPorCliente)
            buscaCliente = new BuscarClientes(knoutOpcoes.Cliente);
    });

    $("#" + knoutOpcoes.RaizCNPJ.id).mask("00.000.000/", { selectOnFocus: true, clearIfNotMatch: true });
    if (knoutTipoGrupoPesssoa != null) {
        knoutOpcoes.TipoGrupoPessoas.val(knoutTipoGrupoPesssoa);
        knoutOpcoes.TipoGrupoPessoas.codEntity(knoutTipoGrupoPesssoa);
    }

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoPessoas/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoPessoas/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.Destroy = function () {
        if (buscaCliente)
            buscaCliente.Destroy();

        divBusca.Destroy();
    };
}