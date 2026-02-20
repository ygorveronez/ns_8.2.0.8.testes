/// <reference path="../../Enumeradores/EnumTipoArredondamentoTabelaFrete.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHora, _hora, _horaDadosGerais, _permiteCalcularTodasFaixas;

var Hora = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora), options: EnumTipoCobrancaHoraTabelaFrete.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora });
    this.MinutoInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicial.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.MinutoFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinal.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarHoraClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoHora(novoValor);
    });
};

var HoraDadosGerais = function () {
    this.PermiteValorAdicionalHoraExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirvalorAdicionalPorHoraExcedente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.MultiplicarValorFaixaHoraPelaHoraCorrida = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarOValorDaFaixaDeHoraPelaHoraCorrida, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoArredondamentoHoras = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoArredondamento.getFieldDescription(), val: ko.observable(EnumTipoArredondamentoTabelaFrete.NaoArredondar), options: EnumTipoArredondamentoTabelaFrete.obterOpcoes(), def: EnumTipoArredondamentoTabelaFrete.NaoArredondar });
    this.CalcularComTodasFaixasHora = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CalcularComTodasFaixas, val: ko.observable(true), def: false, getType: typesKnockout.bool, visible: _permiteCalcularTodasFaixas });

    this.ComponenteFreteHora = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteFrete.getFieldDescription(), visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteHora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

//*******EVENTOS*******

function LoadHora() {

    _hora = new Hora();
    KoBindings(_hora, "knockoutHora");

    _permiteCalcularTodasFaixas = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? true : _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? true : false;

    _horaDadosGerais = new HoraDadosGerais();
    KoBindings(_horaDadosGerais, "knockoutHoraDadosGerais");

    _tabelaFrete.ComponenteFreteHora = _horaDadosGerais.ComponenteFreteHora;
    _tabelaFrete.UtilizarComponenteFreteHora = _horaDadosGerais.UtilizarComponenteFreteHora;
    _tabelaFrete.PermiteValorAdicionalHoraExcedente = _horaDadosGerais.PermiteValorAdicionalHoraExcedente;
    _tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida = _horaDadosGerais.MultiplicarValorFaixaHoraPelaHoraCorrida;
    _tabelaFrete.TipoArredondamentoHoras = _horaDadosGerais.TipoArredondamentoHoras;
    _tabelaFrete.CalcularComTodasFaixasHora = _horaDadosGerais.CalcularComTodasFaixasHora;

    new BuscarComponentesDeFrete(_horaDadosGerais.ComponenteFreteHora);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteHora, _tabelaFrete.UtilizarComponenteFreteHora);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirHoraClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca, width: "30%" },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "50%" }
    ];

    _gridHora = new BasicDataTable(_hora.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridHora();
}

function RecarregarGridHora() {

    var data = new Array();

    $.each(_tabelaFrete.Horas.list, function (i, hora) {
        var horaGrid = new Object();

        horaGrid.Codigo = hora.Codigo.val;
        horaGrid.DescricaoTipo = hora.Tipo.val == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDeHoras : Localization.Resources.Fretes.TabelaFrete.ValorFixoPorHora;

        if (hora.Tipo.val == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora) {
            var numeroInicial = Globalize.parseInt(hora.MinutoInicial.val.toString());
            var numeroFinal = Globalize.parseInt(hora.MinutoFinal.val.toString());

            if (numeroInicial > 0 && numeroFinal > 0)
                horaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAXHoras.format(hora.MinutoInicial.val, hora.MinutoFinal.val);
            else if (numeroFinal <= 0)
                horaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeXHoras.format(hora.MinutoInicial.val);
            else
                horaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteXHoras.format(hora.MinutoFinal.val);

        } else {
            horaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ValorFixoPorHora;
        }

        data.push(horaGrid);
    });

    if (_tabelaFrete.Horas.list.some(function (item) { return item.Tipo.val == EnumTipoCobrancaHoraTabelaFrete.ValorFixoPorHora || Globalize.parseInt(item.MinutoFinal.val.toString()) == 0; })) {
        _horaDadosGerais.PermiteValorAdicionalHoraExcedente.enable(false);
        _horaDadosGerais.PermiteValorAdicionalHoraExcedente.val(false);
    } else {
        _horaDadosGerais.PermiteValorAdicionalHoraExcedente.enable(true);
    }

    _gridHora.CarregarGrid(data);
}


function excluirHoraClick(data) {
    for (var i = 0; i < _tabelaFrete.Horas.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Horas.list[i].Codigo.val) {
            _tabelaFrete.Horas.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridHora();
}

function AdicionarHoraClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_hora);

    if (valido) {

        if (_hora.Tipo.val() == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora) {
            var calcularComTodasFaixasHora = _horaDadosGerais.CalcularComTodasFaixasHora.val();
            var numeroInicial = Globalize.parseInt(_hora.MinutoInicial.val().toString());
            var numeroFinal = Globalize.parseInt(_hora.MinutoFinal.val().toString());

            if (numeroInicial <= 0 && numeroFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeHorasInvalido, Localization.Resources.Fretes.TabelaFrete.OsNumerosInicialEFinalDevemSerMaiorQueZero);
                return;
            } else if (numeroFinal > 0 && numeroFinal < numeroInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeHorasInvalido, Localization.Resources.Fretes.TabelaFrete.NumeroInicialNaoPodeSerMaiorQueONumeroFinal);
                return;
            }

            if (numeroFinal == 0)
                numeroFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.Horas.list.length; i++) {
                if (_tabelaFrete.Horas.list[i].Tipo.val == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora) {
                    var numeroInicialCadastrado = Globalize.parseInt(_tabelaFrete.Horas.list[i].MinutoInicial.val.toString());
                    var numeroFinalCadastrado = Globalize.parseInt(_tabelaFrete.Horas.list[i].MinutoFinal.val.toString());

                    if (numeroFinalCadastrado == 0)
                        numeroFinalCadastrado = 9999999999999999;

                    /**
                     * Perdoe-me essa gambiarra. Mas estava com tempo curto pra finalizar a tarefa
                     * Quando o cálculo percorre todas faixas, é preciso permitir a inserção de datas
                     * sobrepostas. Ex: 0 - 30; 30 - 45; 45 - 50;
                     * Sem isso, só é possível inserir 0 - 30; 31 - 45; 46 - 50;
                     * Note que isso gera um gap entre as faixas, gerando valores menores nos cálculos.
                     */
                    var faixaInvalida = false;
                    if (calcularComTodasFaixasHora) {
                        faixaInvalida = (numeroInicial >= numeroInicialCadastrado && numeroInicial < numeroFinalCadastrado) ||
                            (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
                            (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
                            (numeroFinalCadastrado > numeroInicial && numeroFinalCadastrado <= numeroFinal);
                    } else {
                        faixaInvalida = (numeroInicial >= numeroInicialCadastrado && numeroInicial <= numeroFinalCadastrado) ||
                            (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
                            (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
                            (numeroFinalCadastrado >= numeroInicial && numeroFinalCadastrado <= numeroFinal);
                    }

                    if (faixaInvalida) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.ONumeroDeCobrancaEntrouEmConflitoComONumeroDeHoras.format(_tabelaFrete.Horas.list[i].MinutoInicial.val, _tabelaFrete.Horas.list[i].MinutoFinal.val))
                        return;
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }

        } else if (_hora.Tipo.val() == EnumTipoCobrancaHoraTabelaFrete.ValorFixoPorHora) {
            for (var i = 0; i < _tabelaFrete.Horas.list.length; i++) {
                if (_tabelaFrete.Horas.list[i].Tipo.val == EnumTipoCobrancaHoraTabelaFrete.ValorFixoPorHora) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaDeValorFixoPorHoraJaExiste);
                    return;
                } else if (_tabelaFrete.Horas.list[i].Tipo.val == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        }

        _hora.Codigo.val(guid());

        _tabelaFrete.Horas.list.push(SalvarListEntity(_hora));

        RecarregarGridHora();

        LimparCamposHora();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function LimparCamposHora() {
    LimparCampos(_hora);
}

function ChangeTipoHora(novoValor) {
    if (novoValor == EnumTipoCobrancaHoraTabelaFrete.ValorFixoPorHora) {
        _hora.MinutoInicial.visible(false);
        _hora.MinutoFinal.visible(false);
    } else if (novoValor == EnumTipoCobrancaHoraTabelaFrete.PorFaixaHora) {
        _hora.MinutoInicial.visible(true);
        _hora.MinutoFinal.visible(true);
    }
}