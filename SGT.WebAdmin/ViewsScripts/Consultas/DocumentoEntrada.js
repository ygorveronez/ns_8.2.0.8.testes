/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarDocumentoEntrada = function (knout, callbackRetorno, knoutAcertoViagem, knoutVeiculo, basicGrid, knoutDataEmissaoInicial, knoutDataEmissaoFinal, knoutDataEntradaInicial, knoutDataEntradaFinal, knoutSituacaoDocumentoSPEDFiscal) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null ? true : false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Notas Fiscais de Compras", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Notas Fiscais de Compras", type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 2, text: "Número Inicial: ", getType: typesKnockout.int });
        this.NumeroFinal = PropertyEntity({ col: 2, text: "Número Final: ", getType: typesKnockout.int });
        this.Serie = PropertyEntity({ col: 2, text: "Série: " });
        this.Chave = PropertyEntity({ col: 3, text: "Chave: " });
        this.ValorTotal = PropertyEntity({ col: 3, text: "Valor Total: ", getType: typesKnockout.decimal });

        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.Veiculo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Veiculo:", idBtnSearch: guid(), visible: false });
        this.DataEmissaoInicial = PropertyEntity({ col: 6, text: "Data Emissão Inicial: ", getType: typesKnockout.date, visible: false, val: ko.observable("") });
        this.DataEmissaoFinal = PropertyEntity({ col: 6, text: "Data Emissão Final: ", getType: typesKnockout.date, visible: false, val: ko.observable("") });
        this.DataEntradaInicial = PropertyEntity({ col: 6, text: "Data Entrada Inicial: ", getType: typesKnockout.date, visible: false, val: ko.observable("") });
        this.DataEntradaFinal = PropertyEntity({ col: 6, text: "Data Entrada Final: ", getType: typesKnockout.date, visible: false, val: ko.observable("") });
        this.SituacaoDocumentoSPEDFiscal = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Situacao SPED:", idBtnSearch: guid(), visible: false, val: ko.observable("") });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutAcertoViagem != null && knoutVeiculo != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        knoutOpcoes.Veiculo.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutAcertoViagem.val());
            knoutOpcoes.Veiculo.codEntity(knoutVeiculo.val());
            knoutOpcoes.Veiculo.val(knoutVeiculo.val());
        };
    } else if (knoutDataEmissaoInicial != null && knoutDataEmissaoFinal != null && knoutDataEntradaInicial != null && knoutDataEntradaFinal != null && knoutSituacaoDocumentoSPEDFiscal != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.SituacaoDocumentoSPEDFiscal.codEntity(knoutSituacaoDocumentoSPEDFiscal.val());
            knoutOpcoes.SituacaoDocumentoSPEDFiscal.val(knoutSituacaoDocumentoSPEDFiscal.val());

            knoutOpcoes.DataEmissaoInicial.val(knoutDataEmissaoInicial.val());
            knoutOpcoes.DataEmissaoFinal.val(knoutDataEmissaoFinal.val());
            knoutOpcoes.DataEntradaInicial.val(knoutDataEntradaInicial.val());
            knoutOpcoes.DataEntradaFinal.val(knoutDataEntradaFinal.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "DocumentoEntrada/PesquisaDocumentoReferencia", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "DocumentoEntrada/PesquisaDocumentoReferencia", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Chave.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
};