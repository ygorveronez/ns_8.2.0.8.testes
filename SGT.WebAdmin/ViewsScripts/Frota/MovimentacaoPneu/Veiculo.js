/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoContainerPneu.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />


/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosVeiculo;
var _pesquisaVeiculo;
var _veiculo;
var _PermissoesPersonalizadas;

/*
 * Declaração das Classes
 */

var DadosVeiculo = function () {
    this.Hodometro = PropertyEntity({ text: "*Hodômetro (Km):", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, enable: ko.observable(false) });
    this.PlacaVeiculo = PropertyEntity({ visible: ko.observable(true), text: "Placa:", val: ko.observable(""), enable: false });
    this.FrotaVeiculo = PropertyEntity({ visible: ko.observable(true), text: "Frota:", val: ko.observable(""), enable: false });

    this.LiberarVeiculoManutencao = PropertyEntity({ eventClick: LiberarVeiculoManutencaoClick, type: types.event, text: "Liberar veículo da manutenção?", visible: ko.observable(false) });

    this.ExibirDados = PropertyEntity({
        eventClick: function (e) {
            e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade());
        }, type: types.event, text: "", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
}

var EixoPneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PosicaoLadoDireito = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.PosicaoPneu = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int });

    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.KmAtualRodado = PropertyEntity({});
    this.Marca = PropertyEntity({});
    this.NumeroFogo = PropertyEntity({});
    this.Sulco = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false } });
    this.Vida = PropertyEntity({});
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataMovimentacao = PropertyEntity({});
}

var Eixo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeImagem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "" });

    this.EixosPneu = ko.observableArray();
}

var EstepePneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({});

    this.CodigoPneu = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.KmAtualRodado = PropertyEntity({});
    this.Marca = PropertyEntity({});
    this.NumeroFogo = PropertyEntity({});
    this.ValorProdutos = PropertyEntity({});
    this.ValorMaoObra = PropertyEntity({});
    this.Sulco = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false } });
    this.Vida = PropertyEntity({});
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataMovimentacao = PropertyEntity({});
}

var PesquisaVeiculo = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: false });
    this.NumeroFogo = PropertyEntity({ text: "Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaVeiculo))
                buscarVeiculo();
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar Aferições",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "MovimentacaoPneu/Importar",
        UrlConfiguracao: "MovimentacaoPneu/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O065_ImportacaoAfericoes,
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var Veiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Eixos = ko.observableArray();
    this.Estepes = ko.observableArray();

    this.ExibirDados = PropertyEntity({
        eventClick: function (e) {
            e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade());
        }, type: types.event, text: "", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadVeiculo() {
    _pesquisaVeiculo = new PesquisaVeiculo();
    KoBindings(_pesquisaVeiculo, "knockoutPesquisaVeiculo", false, _pesquisaVeiculo.Pesquisar.id);

    _dadosVeiculo = new DadosVeiculo();
    
    KoBindings(_dadosVeiculo, "knockoutDadosVeiculo");

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirInformarHodometro, _PermissoesPersonalizadas)) {
        _dadosVeiculo.Hodometro.enable(true);
    }



    _veiculo = new Veiculo();
    KoBindings(_veiculo, "knockoutVeiculo");

    new BuscarVeiculos(_pesquisaVeiculo.Veiculo);
}

/*
 * Declaração das Funções Públicas
 */

function obterPneuEstepe(codigoEstepe) {
    for (var i = 0; i < _veiculo.Estepes().length; i++) {
        var estepe = _veiculo.Estepes()[i];

        if (estepe.Codigo.val() == codigoEstepe) {
            return estepe;
        }
    }

    return undefined;
}

function obterPneuVeiculo(codigoEixo, codigoEixoPneu) {
    for (var i = 0; i < _veiculo.Eixos().length; i++) {
        var eixo = _veiculo.Eixos()[i];

        if (eixo.Codigo.val() == codigoEixo) {
            for (var j = 0; j < eixo.EixosPneu().length; j++) {
                var eixoPneu = eixo.EixosPneu()[j];

                if (eixoPneu.Codigo.val() == codigoEixoPneu) {
                    return eixoPneu;
                }
            }
        }
    }

    return undefined;
}

/*
 * Declaração das Funções Privadas
 */

function buscarVeiculo() {
    limparVeiculo();

    executarReST("MovimentacaoPneu/BuscarVeiculoPorCodigo", { Codigo: _pesquisaVeiculo.Veiculo.codEntity(), NumeroFogo: _pesquisaVeiculo.NumeroFogo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaVeiculo.ExibirFiltros.visibleFade(false);

                preencherVeiculo(retorno.Data);

                $(".dados-veiculo-container").show();

                _dadosVeiculo.ExibirDados.visibleFade(true);
                _veiculo.ExibirDados.visibleFade(true);
            }
            else if (_pesquisaVeiculo.NumeroFogo.val()) {
                _pesquisaEstoque.NumeroFogo.val(_pesquisaVeiculo.NumeroFogo.val());
                buscarEstoque();

                _pesquisaReforma.NumeroFogo.val(_pesquisaVeiculo.NumeroFogo.val());
                buscarReforma();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function LiberarVeiculoManutencaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja liberar o veículo do status de Em Manutenção?", function () {
        executarReST("MovimentacaoPneu/LiberarVeiculoManutencao", { Codigo: _veiculo.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _dadosVeiculo.LiberarVeiculoManutencao.visible(false);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Veículo liberado.");
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function limparVeiculo() {
    $(".dados-veiculo-container").hide();

    _veiculo.Codigo.val(0);
    _veiculo.Eixos.removeAll();
    _veiculo.Estepes.removeAll();

    LimparCampos(_dadosVeiculo);
}

function preencherDadosVeiculo(dadosVeiculo) {
    PreencherObjetoKnout(_dadosVeiculo, { Data: dadosVeiculo });

    if (dadosVeiculo.SituacaoVeiculo == EnumSituacaoVeiculo.EmManutencao)
        _dadosVeiculo.LiberarVeiculoManutencao.visible(true);
    else
        _dadosVeiculo.LiberarVeiculoManutencao.visible(false);
}

function preencherVeiculo(veiculo) {
    _veiculo.Codigo.val(veiculo.Codigo);

    preencherDadosVeiculo(veiculo.DadosVeiculo);
    preencherVeiculoEixos(veiculo.Eixos);
    preencherVeiculoEstepes(veiculo.Estepes);
}

function preencherVeiculoEixos(eixos) {
    var listaIdDraggable = new Array();
    var listaIdDroppable = new Array();
    var i = 0;

    for (i = 0; i < eixos.length; i++) {
        var eixoVeiculo = eixos[i];
        var eixo = new Eixo();

        PreencherObjetoKnout(eixo, { Data: eixoVeiculo });

        for (var j = 0; j < eixoVeiculo.Pneus.length; j++) {
            var eixoPneuVeiculo = eixoVeiculo.Pneus[j];
            var eixoPneu = new EixoPneu();

            PreencherObjetoKnout(eixoPneu, { Data: eixoPneuVeiculo });

            if (eixoPneu.CodigoPneu.val() > 0)
                listaIdDraggable.push(eixoPneu.Codigo.val());
            else
                listaIdDroppable.push(eixoPneu.Codigo.val());

            eixo.EixosPneu.push(eixoPneu);
        }

        _veiculo.Eixos.push(eixo);
    }

    for (i = 0; i < listaIdDraggable.length; i++)
        adicionarEventosDraggableEClickPneu("eixo-pneu-" + listaIdDraggable[i], true);

    for (i = 0; i < listaIdDroppable.length; i++)
        adicionarEventoDroppablePneu("eixo-pneu-" + listaIdDroppable[i]);
}

function preencherVeiculoEstepes(estepes) {
    for (var i = 0; i < estepes.length; i++) {
        var estepe = estepes[i];
        var estepePneu = new EstepePneu();

        PreencherObjetoKnout(estepePneu, { Data: estepe });

        _veiculo.Estepes.push(estepePneu);

        if (estepePneu.CodigoPneu.val() > 0)
            adicionarEventosDraggableEClickPneu("estepe-" + estepePneu.Codigo.val());
        else
            adicionarEventoDroppablePneu("estepe-" + estepePneu.Codigo.val());
    }
}
