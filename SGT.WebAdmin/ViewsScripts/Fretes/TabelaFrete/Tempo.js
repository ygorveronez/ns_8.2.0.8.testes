
//*******MAPEAMENTO KNOUCKOUT*******

var _gridTempo, _tempo, _tempoDadosGerais;

var Tempo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicial.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(true), required: true });
    this.HoraFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinal.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(true), required: true });

    this.PeriodoInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PeriodoInicialQ, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.HoraInicialCobrancaMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicialCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.HoraFinalCobrancaMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinalCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTempoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
}

var TempoDadosGerais = function () {
    this.MultiplicarValorTempoPorHora = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarValorPorHora, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.PossuiHorasMinimasCobranca = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PossuiHorasMinimasParaCobranca, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.HorasMinimasCobranca = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HorasMinimasParaCobranca.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(true) });

    this.UtilizarArredondamentoHoras = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarArredondamentoDeMinutosQ, issue: 2021, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarMinutosInformadosComoCorteParaHora, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MinutosArredondamentoHoras = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MinutosUtilizadosParaArredondamento.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(true) });

    this.ComponenteFreteTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete.getFieldDescription(), visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteTempo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.PossuiHorasMinimasCobranca.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tempo.HoraInicialCobrancaMinima.visible(true);
            _tempo.HoraFinalCobrancaMinima.visible(true);
        } else {
            _tempo.HoraInicialCobrancaMinima.visible(false);
            _tempo.HoraFinalCobrancaMinima.visible(false);
        }
    });
}

//*******EVENTOS*******

function LoadTempo() {

    _tempo = new Tempo();
    KoBindings(_tempo, "knockoutTempo");

    _tempoDadosGerais = new TempoDadosGerais();
    KoBindings(_tempoDadosGerais, "knockoutTempoDadosGerais");
    
    _tabelaFrete.MultiplicarValorTempoPorHora = _tempoDadosGerais.MultiplicarValorTempoPorHora;
    _tabelaFrete.PossuiHorasMinimasCobranca = _tempoDadosGerais.PossuiHorasMinimasCobranca;
    _tabelaFrete.HorasMinimasCobranca = _tempoDadosGerais.HorasMinimasCobranca;
    _tabelaFrete.ComponenteFreteTempo = _tempoDadosGerais.ComponenteFreteTempo;
    _tabelaFrete.UtilizarComponenteFreteTempo = _tempoDadosGerais.UtilizarComponenteFreteTempo;
    _tabelaFrete.UtilizarArredondamentoHoras = _tempoDadosGerais.UtilizarArredondamentoHoras;
    _tabelaFrete.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata = _tempoDadosGerais.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata;
    _tabelaFrete.MinutosArredondamentoHoras = _tempoDadosGerais.MinutosArredondamentoHoras;

    new BuscarComponentesDeFrete(_tempoDadosGerais.ComponenteFreteTempo);

    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteTempo, _tabelaFrete.UtilizarComponenteFreteTempo);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirTempoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Periodo", title: Localization.Resources.Fretes.TabelaFrete.Periodo, width: "40%" },
        { data: "PeriodoCobrancaMinima", title: Localization.Resources.Fretes.TabelaFrete.PeriodoCobrancaMinima, width: "40%" }
    ];

    _gridTempo = new BasicDataTable(_tempo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridTempo();
}

function RecarregarGridTempo() {

    var data = new Array();

    $.each(_tabelaFrete.Tempos.list, function (i, tempo) {
        var tempoGrid = new Object();

        tempoGrid.Codigo = tempo.Codigo.val;
        tempoGrid.Periodo = Localization.Resources.Fretes.TabelaFrete.PeriodoDasAsFormatado.format(tempo.HoraInicial.val, tempo.HoraFinal.val);

        if (_tempoDadosGerais.PossuiHorasMinimasCobranca.val() === true && tempo.HoraInicialCobrancaMinima.val != "" && tempo.HoraFinalCobrancaMinima.val != "")
            tempoGrid.PeriodoCobrancaMinima = Localization.Resources.Fretes.TabelaFrete.PeriodoDasAsFormatado.format(tempo.HoraInicialCobrancaMinima.val, tempo.HoraFinalCobrancaMinima.val);
        else
            tempoGrid.PeriodoCobrancaMinima = "";

        data.push(tempoGrid);
    });

    _gridTempo.CarregarGrid(data);
}


function ExcluirTempoClick(data) {
    for (var i = 0; i < _tabelaFrete.Tempos.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Tempos.list[i].Codigo.val) {
            _tabelaFrete.Tempos.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridTempo();
}

function AdicionarTempoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_tempo);

    if (valido) {
                
        var horaInicio = moment(_tempo.HoraInicial.val(), "HH:mm");
        var horaTermino = moment(_tempo.HoraFinal.val(), "HH:mm");

        for (var i = 0; i < _tabelaFrete.Tempos.list.length; i++) {
            var horaInicioGrid = moment(_tabelaFrete.Tempos.list[i].HoraInicial.val, "HH:mm");
            var horaTerminoGrid = moment(_tabelaFrete.Tempos.list[i].HoraFinal.val, "HH:mm");

            if (horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaInicio.diff(horaInicioGrid) == 0 ||
                horaInicio.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaInicioGrid) == 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.PeriodoJaExiste, PeriodoEntrouEmConflitoComUmPeriodoDasAs.format(_tabelaFrete.Tempos.list[i].HoraInicial.val, _tabelaFrete.Tempos.list[i].HoraFinal.val));
                return;
            }
        }

        _tempo.Codigo.val(guid());

        _tabelaFrete.Tempos.list.push(SalvarListEntity(_tempo));

        RecarregarGridTempo();

        LimparCamposTempo();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function LimparCamposTempo() {
    LimparCampos(_tempo);
}