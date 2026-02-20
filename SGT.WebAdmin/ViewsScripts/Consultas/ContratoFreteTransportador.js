var BuscarContratoFreteTransportador = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var descricaoTransportador = Localization.Resources.Consultas.ContratoFreteTransportador.Transportador;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        descricaoTransportador = Localization.Resources.Consultas.ContratoFreteTransportador.EmpresaFilial;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFreteTransportador.BuscarContratosFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFreteTransportador.ContratosFrete, type: types.local });

        this.NumeroContrato = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.ContratoFreteTransportador.Numero.getFieldDescription(), visible: true });
        this.Descricao = PropertyEntity({ col: 10, text: Localization.Resources.Consultas.ContratoFreteTransportador.Descricao.getFieldDescription(), visible: true });
        this.Transportador = PropertyEntity({ col: 12, type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Consultas.ContratoFreteTransportador.Transportador.getFieldDescription()), idBtnSearch: guid(), visible: true });

        this.Situacao = PropertyEntity({ col: 2, val: ko.observable(0), options: EnumSituacaoContratoFreteTransportador.ObterOpcoesPesquisa(), text: Localization.Resources.Consultas.ContratoFreteTransportador.Situacao.getFieldDescription(), issue: 0, visible: ko.observable(true) });
        this.Ativo = PropertyEntity({ col: 2, val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: ", visible: ko.observable(true) });
        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                e.BuscaAvancada.visibleFade(!e.BuscaAvancada.visibleFade());
                e.BuscaAvancada.icon(e.BuscaAvancada.visibleFade() ? "fal fa-minus" : "fal fa-plus");
            },
            buscaAvancada: true,
            type: types.event,
            text: Localization.Resources.Consultas.ContratoFreteTransportador.Adicionais,
            idFade: guid(),
            cssClass: "btn btn-default",
            icon: ko.observable("fal fa-plus"),
            visibleFade: ko.observable(false),
            visible: true
        });

        this.TipoContratoFrete = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ContratoFreteTransportador.TipoContrato, idBtnSearch: guid(), visible: this.BuscaAvancada.visibleFade });
        this.DataInicial = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFreteTransportador.DataInicial, col: 2, getType: typesKnockout.date, placeholder: Localization.Resources.Consultas.ContratoFreteTransportador.DataInicial, visible: this.BuscaAvancada.visibleFade });
        this.DataFinal = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFreteTransportador.DataFinal, col: 2, getType: typesKnockout.date, placeholder: Localization.Resources.Consultas.ContratoFreteTransportador.DataFinal, visible: this.BuscaAvancada.visibleFade });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.ContratoFreteTransportador.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        knoutOpcoes.Transportador.text(Localization.Resources.Consultas.ContratoFreteTransportador.EmpresaFilial);

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Transportador);
        new BuscarTipoContratoFrete(knoutOpcoes.TipoContratoFrete);

    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarContratoFreteFranquia = function (knout, _ko_ocorrencia, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Contratos de Frete", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Contratos de Frete", type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: "Descrição:" });
        this.PeriodoInicio = PropertyEntity({ col: 6, text: "Data inicial:", getType: typesKnockout.date, visible: true });
        this.PeriodoFim = PropertyEntity({ col: 6, text: "Data Final:", getType: typesKnockout.date, visible: true });
        this.Empresa = PropertyEntity({ col: 0, visible: false, type: types.entity, codEntity: ko.observable(0) });

        this.PeriodoInicio.dateRangeLimit = this.PeriodoFim;
        this.PeriodoFim.dateRangeInit = this.PeriodoInicio;

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var opcoesDinamicas = function () {
        knoutOpcoes.PeriodoInicio.val(_ko_ocorrencia.PeriodoInicio.val());
        knoutOpcoes.PeriodoFim.val(_ko_ocorrencia.PeriodoFim.val());
        knoutOpcoes.Empresa.val(_ko_ocorrencia.Empresa.val());
        knoutOpcoes.Empresa.codEntity(_ko_ocorrencia.Empresa.codEntity());
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, opcoesDinamicas);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/ObterContratoFranquia", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.ExecuteSearch = function () {
        opcoesDinamicas();

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
        });
    };
}

var BuscarContratoFreteTransportadorCliente = function (knout, callbackRetorno, basicGrid, knoutContratoFreteTransportador) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Clientes do Contrato Frete Transportador", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cliente do Contrato Frete Transportador", type: types.local });

        this.Nome = PropertyEntity({ col: 8, text: "Nome" });
        this.CpfCnpj = PropertyEntity({ col: 4, text: "CNPJ/CPF", getType: typesKnockout.cpfCnpj, maxlength: 14 });
        this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Contrato Frete Transportador", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutContratoFreteTransportador != null) {
        knoutOpcoes.ContratoFreteTransportador.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.ContratoFreteTransportador.codEntity(knoutContratoFreteTransportador.codEntity());
            knoutOpcoes.ContratoFreteTransportador.val(knoutContratoFreteTransportador.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/PesquisaContratoFreteTransportadorCliente", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/PesquisaContratoFreteTransportadorCliente", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarOutrosValoresRecursos = function (knout, callbackRetorno, basicGrid, knoutContratoFreteTransportador) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar de Valores Outros Recursos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Valores Outros Recursos", type: types.local });

        this.ValoresOutrosRecursosFechamento = PropertyEntity({ col: 12, text: "Tipo Mão de Obra" });
        this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Contrato Frete Transportador", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutContratoFreteTransportador != null) {
        knoutOpcoes.ContratoFreteTransportador.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.ContratoFreteTransportador.codEntity(knoutContratoFreteTransportador.codEntity());
            knoutOpcoes.ContratoFreteTransportador.val(knoutContratoFreteTransportador.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/PesquisaContratoFreteValoresOutrosRecursos", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteTransportador/PesquisaContratoFreteValoresOutrosRecursos", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};