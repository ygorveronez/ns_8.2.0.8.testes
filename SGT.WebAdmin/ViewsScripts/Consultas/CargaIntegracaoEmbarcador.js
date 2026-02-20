/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

var BuscarCargasIntegracaoEmbarcador = function (knout, callbackRetorno, multiplaEscolha, apenasConsulta) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Cargas Importadas do Embarcador", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas Importadas do Embarcador", type: types.local });

        this.NumeroCargaEmbarcador = PropertyEntity({ text: "Nº da Carga do Embarcador: ", col: 2 });
        this.NumeroCarga = PropertyEntity({ text: "Nº da Carga: ", col: 2 });

        this.DataInicialCarga = PropertyEntity({ text: "Data Inicial: ", col: 2, getType: typesKnockout.date, visible: true });
        this.DataFinalCarga = PropertyEntity({ text: "Data Final: ", col: 2, getType: typesKnockout.date, visible: true });

        this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
        this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;

        this.Situacao = PropertyEntity({ text: "Situação: ", col: 4, def: [], val: ko.observable([]), getType: typesKnockout.selectMultiple, options: EnumSituacaoCargaIntegracaoEmbarcador.ObterOpcoes(), visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
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
            }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default waves-effect waves-themed", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.NumeroCTe = PropertyEntity({ col: 2, text: "Nº do CT-e: ", getType: typesKnockout.int, visible: true });
        this.NumeroMDFe = PropertyEntity({ col: 2, text: "Nº do MDF-e: ", getType: typesKnockout.int, visible: true });
        this.SituacaoCarga = PropertyEntity({ text: "Situação da Carga: ", col: 4, def: [], val: ko.observable([]), getType: typesKnockout.selectMultiple, options: EnumSituacoesCarga.obterOpcoesTMS(), visible: true });
        this.SituacaoCancelamento = PropertyEntity({ text: "Situação do Cancelamento: ", col: 4, def: [], val: ko.observable([]), getType: typesKnockout.selectMultiple, options: EnumSituacaoCancelamentoCarga.ObterOpcoes(), visible: true });
        this.Empresa = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: true });
        this.Veiculo = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Motorista = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: true });
        this.TipoOperacao = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: true });
        
    };

    var opcaoAjusteManual = { descricao: "Ajuste manual", id: guid(), evento: "onclick", metodo: ajusteManualClick, tamanho: "10", icone: "" };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarVeiculos(knoutOpcoes.Veiculo, null, knoutOpcoes.Empresa);
        new BuscarMotoristas(knoutOpcoes.Motorista, null, knoutOpcoes.Empresa);
        new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        knoutOpcoes.NumeroCarga.val(knoutOpcoes.NumeroCarga.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    var configExportacao = {
        url: "CargaIntegracaoEmbarcador/ExportarPesquisa",
        btnText: "Exportar Excel"
    }

    var permiteAjusteManual = _CONFIGURACAO_TMS.InformarAjusteManualCargasImportadasEmbarcador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirAjusteManualImportadasEmbarcador, _PermissoesPersonalizadasCarga);

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoAjusteManual], tamanho: 5, };

    var opcoesApenasConsulta = permiteAjusteManual ? menuOpcoes : null;

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaIntegracaoEmbarcador/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid, null, null, configExportacao);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaIntegracaoEmbarcador/Pesquisa", knoutOpcoes, apenasConsulta ? opcoesApenasConsulta : divBusca.OpcaoPadrao(callback) , null, null, null, null, null, null, null, null, configExportacao);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroCargaEmbarcador.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });

    function ajusteManualClick (e) {
        executarReST("CargaIntegracaoEmbarcador/DefinirAcaoManual", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    GridConsulta.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}
