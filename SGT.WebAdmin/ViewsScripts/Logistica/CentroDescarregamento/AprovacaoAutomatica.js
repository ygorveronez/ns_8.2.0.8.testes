var _aprovacaoAutomaticaCentroDescarregamento;
var _gridAprovacaoAutomaticaCentroDescarregamento;

//#region Mapeamento

var AprovacaoAutomaticaCentroDescarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), required: true });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), required: true });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    
    this.Grid = PropertyEntity({ type: types.local, val: ko.observable([]), idGrid: guid() });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarAprovacaoAutomaticaCentroDescarregamentoClick, text: Localization.Resources.Gerais.Geral.Adicionar });
}

function loadAprovacaoAutomaticaCentroDescarregamento() {
    _aprovacaoAutomaticaCentroDescarregamento = new AprovacaoAutomaticaCentroDescarregamento();
    KoBindings(_aprovacaoAutomaticaCentroDescarregamento, "knockoutAprovacaoAutomatica");
    
    loadGridAprovacaoAutomaticaCentroDescarregamento();
}

function loadGridAprovacaoAutomaticaCentroDescarregamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAprovacaoAutomaticaCentroDescarregamentoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataInicial", title: Localization.Resources.Gerais.Geral.Inicio, width: "50%" },
        { data: "DataFinal", title: Localization.Resources.Gerais.Geral.Final, width: "50%" }
    ];
    
    _gridAprovacaoAutomaticaCentroDescarregamento = new BasicDataTable(_aprovacaoAutomaticaCentroDescarregamento.Grid.idGrid, header, menuOpcoes);
    recarregarGridAprovacaoAutomaticaCentroDescarregamento();
}

//#endregion

//#region Métodos Click

function adicionarAprovacaoAutomaticaCentroDescarregamentoClick() {
    _aprovacaoAutomaticaCentroDescarregamento.Codigo.val(guid());

    _centroDescarregamento.HorariosAprovacaoAutomatica.list.push(SalvarListEntity(_aprovacaoAutomaticaCentroDescarregamento));

    recarregarGridAprovacaoAutomaticaCentroDescarregamento();
    
    LimparCampos(_aprovacaoAutomaticaCentroDescarregamento);
}

function removerAprovacaoAutomaticaCentroDescarregamentoClick(registroSelecionado) {
    var registros = _centroDescarregamento.HorariosAprovacaoAutomatica.list;
    
    registros = registros.filter((registro) => registro.Codigo.val != registroSelecionado.Codigo);
    
    _centroDescarregamento.HorariosAprovacaoAutomatica.list = registros;

    recarregarGridAprovacaoAutomaticaCentroDescarregamento();
}

//#endregion

//#region Métodos Privados

function recarregarGridAprovacaoAutomaticaCentroDescarregamento() {
    var data = new Array();

    $.each(_centroDescarregamento.HorariosAprovacaoAutomatica.list, function (i, horario) {
        var horarioGrid = new Object();

        horarioGrid.Codigo = horario.Codigo.val;

        horarioGrid.DataInicial = horario.DataInicial.val;
        horarioGrid.DataFinal = horario.DataFinal.val;

        data.push(horarioGrid);
    });
    
    _gridAprovacaoAutomaticaCentroDescarregamento.CarregarGrid(data);
}

//#endregion