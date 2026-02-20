var _gridRodizio;
var _pesquisaRodizio;
var _editarRodizio;
var _codigoSendoEditado = 0;

var filtroDiaSemana = {
    Todos: -1,
    Domingo: 1,
    Segunda: 2,
    Terca: 3,
    Quarta: 4,
    Quinta: 5,
    Sexta: 6,
    Sabado: 7,
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: filtroDiaSemana.Todos },
            { text: "Segunda", value: filtroDiaSemana.Segunda },
            { text: "Terça", value: filtroDiaSemana.Terca },
            { text: "Quarta", value: filtroDiaSemana.Quarta },
            { text: "Quinta", value: filtroDiaSemana.Quinta },
            { text: "Sexta", value: filtroDiaSemana.Sexta },
            { text: "Sábado", value: filtroDiaSemana.Sabado },
            { text: "Domingo", value: filtroDiaSemana.Domingo }
        ];
    },
    obterOpcoesCadastro: function () {
        return [
            { text: "Segunda", value: filtroDiaSemana.Segunda },
            { text: "Terça", value: filtroDiaSemana.Terca },
            { text: "Quarta", value: filtroDiaSemana.Quarta },
            { text: "Quinta", value: filtroDiaSemana.Quinta },
            { text: "Sexta", value: filtroDiaSemana.Sexta },
            { text: "Sábado", value: filtroDiaSemana.Sabado },
            { text: "Domingo", value: filtroDiaSemana.Domingo }
        ];
    }
};

var PesquisaRodizio = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.FinalDaPlaca = PropertyEntity({ type: types.map, text: "Final da Placa:" });
    this.DiaDaSemana = PropertyEntity({ type: types.map, val: ko.observable(filtroDiaSemana.Todos), options: filtroDiaSemana.obterOpcoesPesquisa(), def: filtroDiaSemana.Todos, text: "Dia da Semana:" });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) { _gridRodizio.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
}

var EditarRodizio = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.DiaDaSemana = PropertyEntity({ type: types.map, val: ko.observable(filtroDiaSemana.Segunda), options: filtroDiaSemana.obterOpcoesCadastro(), def: filtroDiaSemana.Segunda, text: "Dia da Semana:" });
    this.FinaisPlacas = PropertyEntity({ type: types.map, text: 'Finais de Placas:', val: ko.observable(''), maxlength: 100, required: false, visible: ko.observable(true) });
}

var CRUDRodizio = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarRodizio, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposRodizio, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirRodizio, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

function LoadRodizioPlacas() {
    _pesquisaRodizio = new PesquisaRodizio();
    _editarRodizio = new EditarRodizio();
    _CRUDRodizio = new CRUDRodizio();

    KoBindings(_pesquisaRodizio, "knockoutPesquisaRodizio");
    KoBindings(_editarRodizio, "knockoutEditarRodizio");
    KoBindings(_CRUDRodizio, "knockoutCRUDEditarRodizio");

    new BuscarFilial(_pesquisaRodizio.Filial);
    new BuscarFilial(_editarRodizio.Filial);

    BuscarRodizio();
    _pesquisaRodizio.ExibirFiltros.visibleFade(true);
    $('#divEditarRodizio').slideDown();
}

function BuscarRodizio() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarRodizioClick, tamanho: "20", icone: "" }
        ]
    };
    _gridRodizio = new GridView(_pesquisaRodizio.Pesquisar.idGrid, "RodizioPlacas/Pesquisa", _pesquisaRodizio, menuOpcoes);
    _gridRodizio.CarregarGrid();
}

function EditarRodizioClick(row) {
    _codigoSendoEditado = row.Codigo;
    _editarRodizio.Filial.codEntity(row.CodigoFilial);
    _editarRodizio.Filial.val(row.Filial);
    _editarRodizio.DiaDaSemana.val(row.EnumDia)
    _editarRodizio.FinaisPlacas.val(row.Finais)
    _CRUDRodizio.Excluir.visible(true);
    $('#divEditarRodizio').slideDown();
}

function LimparCamposRodizio() {
    LimparCampos(_editarRodizio);
    _CRUDRodizio.Excluir.visible(false);
    _codigoSendoEditado = 0;
}

function SalvarRodizio() {
    var dados = {
        Codigo: _codigoSendoEditado,
        Filial: _editarRodizio.Filial.codEntity(),
        FinalDaPlaca: _editarRodizio.FinaisPlacas.val(),
        DiaDaSemana: _editarRodizio.DiaDaSemana.val()
    };

    executarReST('RodizioPlacas/Salvar', dados, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro salvo com sucesso.");
            }
            LimparCamposRodizio();
            _gridRodizio.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
    });
}

function ExcluirRodizio() {
    executarReST('RodizioPlacas/Excluir', { Codigo: _codigoSendoEditado }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso.");
            }
            LimparCamposRodizio();
            _gridRodizio.CarregarGrid();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
    });
}