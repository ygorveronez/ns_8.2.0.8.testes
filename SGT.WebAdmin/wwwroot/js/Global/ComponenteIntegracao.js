const ComponenteIntegracao = function (idDiv, idContainerIntegracao = null) {

    this.idContainer = idContainerIntegracao ? idContainerIntegracao : "#integracao";
    this.idGrid = guid();

    this.setSituacoes = (situacoes) => {
        this.situacoes = situacoes;
    }

    this.setOnPesquisar = (onPesquisar) => {
        this.onPesquisar = onPesquisar;
    }

    this.setOnReenviarTodos = (onReenviarTodos) => {
        this.onReenviarTodos = onReenviarTodos;
    }

    this.setOnObterTotais = (onObterTotais) => {
        this.onObterTotais = onObterTotais;
    }

    this.configurarGrid = (url, dados, menuOpcoes) => {
        this.url = url;
        this.menuOpcoes = menuOpcoes;
        this.dados = dados;
    }

    this.CarregarGrid = () => {
        this.gridView.CarregarGrid();
    }

    this.render = () => {
        this.gridView = new GridView(this.idGrid, this.url, this.dados, this.menuOpcoes);

        $(idDiv).html(getHtmlLayoutIntegracao(this.idContainer, this.idGrid));
        const integracaoGenerica = new IntegracaoGenerica(this.situacoes, this.onPesquisar, this.onReenviarTodos, this.onObterTotais, this.gridView, this.dados);
        KoBindings(integracaoGenerica, this.idContainer);

        this.gridView.CarregarGrid();
        this.onObterTotais(integracaoGenerica);
    }
}

var IntegracaoGenerica = function (situacoes, onPesquisar, onReenviarTodos, onObterTotais, gridView, dados) {
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: situacoes, text: "Situação:", def: "" });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            onPesquisar(gridView, dados, this.Situacao.val());
        }, type: types.event, text: "Pesquisar", visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            onReenviarTodos();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            onObterTotais(this);
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

function getHtmlLayoutIntegracao(idContainer, idGrid) {
    return `
    <div class="tab-pane" id="${idContainer}">
        <div class="row">
            <div class="col-12 col-md-6 col-lg-4" data-bind="visible: Situacao.visible">
                <div class="form-group">
                    <label class="form-label" data-bind="text: Situacao.text, attr: { for: Situacao.id }"></label>
                    <select class="form-control" data-bind="class: Situacao.requiredClass, options: Situacao.options, optionsText: 'text', optionsValue: 'value', value: Situacao.val, attr: { id : Situacao.id}"></select>
                </div>
            </div>
            <div class="divBotoesIntegracaoCTe col-12 col-md-6 col-lg-8">
                <button data-bind="click: Pesquisar.eventClick, attr: { id: Pesquisar.id}" class="btn btn-primary waves-effect waves-themed float-start" type="button">
                    <i class="fa fa-search"></i>&nbsp;<span data-bind="text: Pesquisar.text"></span>
                </button>
                <button data-bind="click: ReenviarTodos.eventClick, attr: { id: ReenviarTodos.id}, visible: ReenviarTodos.visible" class="btn btn-default waves-effect waves-themed float-end" style="padding: 6px 12px !important;" type="button">
                    <i class="fa fa-mail-forward"></i>&nbsp;<span data-bind="text: ReenviarTodos.text"></span>
                </button>
            </div>
        </div>
        <table width="100%" class="table table-bordered table-hover" data-bind="attr: { id: '${idGrid}'}" cellspacing="0"></table>
        <div class="card mt-5">
            <div class="card-header d-flex align-items-center justify-content-between">
                <h3 class="card-title">Legenda</h3>
                <button class="btn btn-primary waves-effect waves-themed" type="button" data-bind="click: ObterTotais.eventClick, attr: { id: ObterTotais.id}" id="aea754d17ce1f2aea0c836de49f3bfa73">
                    <i class="fal fa-redo"></i> Atualizar
                </button>
            </div>
            <div class="card-body">
                <div class="legends">
                    <div class="badge legend green">
                        <b data-bind="text: TotalIntegrado.text">{Localize::Cargas.Carga.IntegradoComSucesso}</b>
                        <span data-bind="text: TotalIntegrado.val()"></span>
                    </div>
                    <div class="badge legend red">
                        <b data-bind="text: TotalProblemaIntegracao.text">{Localize::Cargas.Carga.ProblemasNaIntegracao}</b>
                        <span data-bind="text: TotalProblemaIntegracao.val()"></span>
                    </div>
                    <div class="badge legend blue">
                        <b data-bind="text: TotalAguardandoIntegracao.text">{Localize::Cargas.Carga.AguardandoIntegracao}</b>
                        <span data-bind="text: TotalAguardandoIntegracao.val()"></span>
                    </div>
                    <div class="badge legend gray">
                        <b data-bind="text: TotalGeral.text">Total Geral:</b>
                        <span data-bind="text: TotalGeral.val()"></span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;
}
