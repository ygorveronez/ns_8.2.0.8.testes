//*******MAPEAMENTO KNOUCKOUT*******

var _gridAjudante, _ajudante, _ajudanteDadosGerais;

var Ajudante = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes), options: EnumTipoCobrancaAjudanteTabelaFrete.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeOcorrencia.getRequiredFieldDescription(), def: EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes });
    this.NumeroInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroInicial.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.NumeroFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroFinal.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAjudanteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoAjudante(novoValor);
    });
}

var AjudanteDadosGerais = function () {
    this.PermiteValorAdicionalAjudanteExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionarAjudanteExcedente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.ComponenteFreteAjudante = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteFrete.getFieldDescription(), visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteAjudante = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

//*******EVENTOS*******

function LoadAjudante() {

    _ajudante = new Ajudante();
    KoBindings(_ajudante, "knockoutAjudante");

    _ajudanteDadosGerais = new AjudanteDadosGerais();
    KoBindings(_ajudanteDadosGerais, "knockoutAjudanteDadosGerais");

    _tabelaFrete.ComponenteFreteAjudante = _ajudanteDadosGerais.ComponenteFreteAjudante;
    _tabelaFrete.UtilizarComponenteFreteAjudante = _ajudanteDadosGerais.UtilizarComponenteFreteAjudante;
    _tabelaFrete.PermiteValorAdicionalAjudanteExcedente = _ajudanteDadosGerais.PermiteValorAdicionalAjudanteExcedente;

    new BuscarComponentesDeFrete(_ajudanteDadosGerais.ComponenteFreteAjudante);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteAjudante, _tabelaFrete.UtilizarComponenteFreteAjudante);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirAjudanteClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca, width: "30%" },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "50%" }
    ];

    _gridAjudante = new BasicDataTable(_ajudante.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridAjudante();
}

function RecarregarGridAjudante() {

    var data = new Array();

    $.each(_tabelaFrete.Ajudantes.list, function (i, ajudante) {
        var ajudanteGrid = new Object();

        ajudanteGrid.Codigo = ajudante.Codigo.val;
        ajudanteGrid.DescricaoTipo = ajudante.Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDeAjudantes : Localization.Resources.Fretes.TabelaFrete.ValorFixoPorAjudante

        if (ajudante.Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes) {
            var numeroInicial = Globalize.parseInt(ajudante.NumeroInicial.val.toString());
            var numeroFinal = Globalize.parseInt(ajudante.NumeroFinal.val.toString());

            if (numeroInicial > 0 && numeroFinal > 0)
                ajudanteGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AjudanteDescricaoDeAte.format(ajudante.NumeroInicial.val, ajudante.NumeroFinal.val)
            else if (numeroFinal <= 0)
                ajudanteGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AjudanteDescricaoAPartirDe.format(ajudante.NumeroInicial.val);
            else
                ajudanteGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AjudanteDescricaoAte.format(ajudante.NumeroFinal.val);

        } else {
            ajudanteGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ValorFixoPorAjudante;
        }

        data.push(ajudanteGrid);
    });

    if (_tabelaFrete.Ajudantes.list.some(function (item) { return item.Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.ValorFixoPorAjudante || Globalize.parseInt(item.NumeroFinal.val.toString()) == 0; })) {
        _ajudanteDadosGerais.PermiteValorAdicionalAjudanteExcedente.enable(false);
        _ajudanteDadosGerais.PermiteValorAdicionalAjudanteExcedente.val(false);
    } else {
        _ajudanteDadosGerais.PermiteValorAdicionalAjudanteExcedente.enable(true);
    }

    _gridAjudante.CarregarGrid(data);
}


function excluirAjudanteClick(data) {
    for (var i = 0; i < _tabelaFrete.Ajudantes.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Ajudantes.list[i].Codigo.val) {
            _tabelaFrete.Ajudantes.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridAjudante();
}

function AdicionarAjudanteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ajudante);

    if (valido) {

        if (_ajudante.Tipo.val() == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes) {

            var numeroInicial = Globalize.parseInt(_ajudante.NumeroInicial.val().toString());
            var numeroFinal = Globalize.parseInt(_ajudante.NumeroFinal.val().toString());

            if (numeroInicial <= 0 && numeroFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroAjudantesInvalido, Localization.Resources.Fretes.TabelaFrete.OsNumerosInicialEFinalDevemSerMaiorQueZero);
                return;
            } else if (numeroFinal > 0 && numeroFinal < numeroInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroAjudantesInvalido, Localization.Resources.Fretes.TabelaFrete.NumeroInicialNaoPodeSerMaiorQueONumeroFinal);
                return;
            }

            if (numeroFinal == 0)
                numeroFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.Ajudantes.list.length; i++) {
                if (_tabelaFrete.Ajudantes.list[i].Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes) {
                    var numeroInicialCadastrado = Globalize.parseInt(_tabelaFrete.Ajudantes.list[i].NumeroInicial.val.toString());
                    var numeroFinalCadastrado = Globalize.parseInt(_tabelaFrete.Ajudantes.list[i].NumeroFinal.val.toString());

                    if (numeroFinalCadastrado == 0)
                        numeroFinalCadastrado = 9999999999999999;

                    if ((numeroInicial >= numeroInicialCadastrado && numeroInicial <= numeroFinalCadastrado) ||
                        (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
                        (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
                        (numeroFinalCadastrado >= numeroInicial && numeroFinalCadastrado <= numeroFinal)) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeEntregaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaEntrouEmConflitoComONumeroDeAjudantes.format(_tabelaFrete.Ajudantes.list[i].NumeroInicial.val, _tabelaFrete.Ajudantes.list[i].NumeroFinal.val))
                        return;
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        } else if (_ajudante.Tipo.val() == EnumTipoCobrancaAjudanteTabelaFrete.ValorFixoPorAjudante) {
            for (var i = 0; i < _tabelaFrete.Ajudantes.list.length; i++) {
                if (_tabelaFrete.Ajudantes.list[i].Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.ValorFixoPorAjudante) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaDeValorFixoPorAjudanteJaExiste);
                    return;
                } else if (_tabelaFrete.Ajudantes.list[i].Tipo.val == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        }

        _ajudante.Codigo.val(guid());

        _tabelaFrete.Ajudantes.list.push(SalvarListEntity(_ajudante));

        RecarregarGridAjudante();

        LimparCamposAjudante();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function LimparCamposAjudante() {
    LimparCampos(_ajudante);
}

function ChangeTipoAjudante(novoValor) {
    if (novoValor == EnumTipoCobrancaAjudanteTabelaFrete.ValorFixoPorAjudante) {
        _ajudante.NumeroInicial.visible(false);
        _ajudante.NumeroFinal.visible(false);
    } else if (novoValor == EnumTipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes) {
        _ajudante.NumeroInicial.visible(true);
        _ajudante.NumeroFinal.visible(true);
    }
}