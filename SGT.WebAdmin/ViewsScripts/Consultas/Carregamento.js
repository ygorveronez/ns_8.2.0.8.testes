/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Filial.js" />


var BuscarCarregamento = function (knout, callbackRetorno, situacao, knoutEmpresa, tipoCarregamento, knoutSessaoRoteirizador) {
    var visibleSituacao = false;
    var situacoes = [ EnumSituacaoCarregamento.EmMontagem ];

    if (!situacao)
        visibleSituacao = true;
    else if (situacao instanceof Array)
        situacoes = situacao;
    else
        situacoes = [situacao];

    if (tipoCarregamento == null)
        tipoCarregamento = EnumTipoMontagemCarga.Todos;

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;
    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.BuscarCarregamento, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.Carregamentos, type: types.local });
        this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.NumeroDoCarregamento.getFieldDescription(), col: visibleSituacao ? 3 : 4 });
        this.DataInicio = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.DataInicial.getFieldDescription(), col: visibleSituacao ? 3 : 4, getType: typesKnockout.date, visible: true, val: ko.observable(dataAtual) });
        this.DataFim = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.DataFinal.getFieldDescription(), col: visibleSituacao ? 3 : 4, getType: typesKnockout.date, visible: true });
        this.SituacaoCarregamento = PropertyEntity({ val: ko.observable(situacoes), def: situacoes, getType: typesKnockout.selectMultiple, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), options: ko.observableArray(EnumSituacaoCarregamento.obterOpcoes()), selectMultipleTitle: Localization.Resources.Gerais.Geral.Todas, col: 3, visible: visibleSituacao });
        this.TipoMontagemCarga = PropertyEntity({ val: ko.observable(tipoCarregamento), options: EnumTipoMontagemCarga.obterOpcoesPesquisa(), text: Localization.Resources.Consultas.Carregamento.TipoDaMontagem.getFieldDescription(), def: tipoCarregamento, col: 3, visible: false });
        
        this.DataInicio.dateRangeLimit = this.DataFim;
        this.DataFim.dateRangeInit = this.DataInicio;

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade() == true) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.NumeroDaCarga.getFieldDescription(), col: 3 });
        this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.NumeroDoPedido.getFieldDescription(), col: 3 });
        this.Filial = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carregamento.Filial.getFieldDescription(), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? true : false });
        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? 12 : 6, text: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Consultas.Carregamento.Transportador.getFieldDescription(): Localization.Resources.Consultas.Carregamento.EmpresaFilial.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Origem = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carregamento.Origem.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destino = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carregamento.Destino.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Remetente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carregamento.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carregamento.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.SessaoRoteirizador = PropertyEntity({ text: Localization.Resources.Consultas.Carregamento.SessaoDoRoteirizador.getFieldDescription(), visible: false });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        if (knoutEmpresa) {
            knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
            knoutOpcoes.Empresa.val(knoutEmpresa.val());
        }

        if (knoutSessaoRoteirizador)
            knoutOpcoes.SessaoRoteirizador.val(knoutSessaoRoteirizador.val());
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarLocalidades(knoutOpcoes.Origem, null);
        new BuscarLocalidades(knoutOpcoes.Destino, null);
        new BuscarClientes(knoutOpcoes.Remetente, null);
        new BuscarClientes(knoutOpcoes.Destinatario, null);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MontagemCarga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroCarregamento.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}
