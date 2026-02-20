/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumQuantidadePneuEixo.js" />
/// <reference path="../../Enumeradores/EnumTipoEixo.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoMultisoftware.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroEixo;
var _CRUDCadastroEixo;
var _eixo;
var _isControlarEdicaoEixo = false;
var _numeroEixosAnterior = 0;

/*
 * Declaração das Classes
 */

var CadastroEixo = function (numero, codigoModeloVeicularCarga) {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoModeloVeicularCarga = PropertyEntity({ val: ko.observable(codigoModeloVeicularCarga || 0), def: 0, getType: typesKnockout.int });
    this.NomeImagem = PropertyEntity({ val: ko.observable("img/Eixos/EixoSimplesSemPneu.png"), def: "img/Eixos/EixoSimplesSemPneu.png", cssClass: ko.observable("eixo-container") });
    this.Numero = PropertyEntity({ text: "Número do Eixo:", val: ko.observable(numero || ""), enable: false });
    this.PrevisaoPneuNovoKm = PropertyEntity({ text: "Previsao de Pneu Novo (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.PrevisaoReformaKm = PropertyEntity({ text: "Previsao de Reforma (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.PrevisaoRodizioKm = PropertyEntity({ text: "Previsao de Rodizio (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.QuantidadePneu = PropertyEntity({ text: "*Quantidade de Pneus: ", val: ko.observable(EnumQuantidadePneuEixo.Simples), options: EnumQuantidadePneuEixo.obterOpcoes(), def: EnumQuantidadePneuEixo.Simples });
    this.Tipo = PropertyEntity({ text: "*Tipo do Eixo: ", val: ko.observable(EnumTipoEixo.Direcional), options: EnumTipoEixo.obterOpcoes(), def: EnumTipoEixo.Direcional });
    this.ToleranciaPneuNovoKm = PropertyEntity({ text: "Tolerância de Pneu Novo (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.ToleranciaReformaKm = PropertyEntity({ text: "Tolerância de Reforma (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.ToleranciaRodizioKm = PropertyEntity({ text: "Tolerância de Rodizio (Km):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });

    this.PrevisaoPneuNovoDia = PropertyEntity({ text: "Previsao de Pneu Novo (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.PrevisaoReformaDia = PropertyEntity({ text: "Previsao de Reforma (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.PrevisaoRodizioDia = PropertyEntity({ text: "Previsao de Rodizio (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.ToleranciaPneuNovoDia = PropertyEntity({ text: "Tolerância de Pneu Novo (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.ToleranciaReformaDia = PropertyEntity({ text: "Tolerância de Reforma (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });
    this.ToleranciaRodizioDia = PropertyEntity({ text: "Tolerância de Rodizio (Dia):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, val: ko.observable(0), def: 0 });

    this.NomeImagem.val.subscribe(function (nomeImagem) {
        if (nomeImagem.indexOf("Menor") > -1)
            self.NomeImagem.cssClass("eixo-container eixo-container-menor");
        else
            self.NomeImagem.cssClass("eixo-container");
    });
}

var CRUDCadastroEixo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarEixoClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
    this.Replicar = PropertyEntity({ eventClick: replicarEixoClick, type: types.event, text: "Replicar", idGrid: guid(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarEixoClick, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEixoClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirEixoClick, type: types.event, text: "Excluir", idGrid: guid(), visible: ko.observable(true) });
}

var Eixo = function () {
    this.CustoPneuKM = PropertyEntity({ text: "Custo do Pneu por KM:", getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: false, thousands: "" }, val: ko.observable(0), def: 0, maxlength: 12 });
    this.CalibragemAposKm = PropertyEntity({ text: "Calibragem Após (KM):", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, val: ko.observable(0), def: 0, maxlength: 7 });
    this.GerarAlertaManutencao = PropertyEntity({ text: "Gerar Alerta de Manutenção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuantidadeEstepes = PropertyEntity({ text: "Quantidade Estepes:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, val: ko.observable(0), def: 0, maxlength: 7 });

    this.QuantidadeEstepes.val.subscribe(controlarQuantidadeEstepes);

    this.Eixos = ko.observableArray();
    this.Estepes = ko.observableArray();
}

var Estepe = function (numero) {
    this.Numero = PropertyEntity({ val: ko.observable(numero || "") });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEixo() {
    _eixo = new Eixo();
    KoBindings(_eixo, "knockoutEixo");

    _cadastroEixo = new CadastroEixo();
    KoBindings(_cadastroEixo, "knockoutCadastroEixo");

    _CRUDCadastroEixo = new CRUDCadastroEixo();
    KoBindings(_CRUDCadastroEixo, "knockoutCRUDCadastroEixo");

    _modeloVeicularCarga.CalibragemAposKm = _eixo.CalibragemAposKm;
    _modeloVeicularCarga.GerarAlertaManutencao = _eixo.GerarAlertaManutencao;
    _modeloVeicularCarga.QuantidadeEstepes = _eixo.QuantidadeEstepes;
    _modeloVeicularCarga.CustoPneuKM = _eixo.CustoPneuKM;

    _modeloVeicularCarga.NumeroEixos.val.subscribe(controlarEdicaoEixo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarEixoClick() {
    Salvar(_cadastroEixo, "ModeloVeicularCarga/AdicionarEixo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _cadastroEixo.Codigo.val(retorno.Data.Codigo);
                _cadastroEixo.NomeImagem.val(retorno.Data.NomeImagem);

                PreencherObjetoKnout(_eixo.Eixos()[_cadastroEixo.Numero.val() - 1], { Data: RetornarObjetoPesquisa(_cadastroEixo) });
                fecharModalCadastroEixo();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function replicarEixoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja replicar esta configuração aos demais eixos NÃO configurados?", function () {
        Salvar(_cadastroEixo, "ModeloVeicularCarga/ReplicarEixo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalCadastroEixo();
                    limparEixo();
                    executarReST("ModeloVeicularCarga/BuscarPorCodigo", { Codigo: _modeloVeicularCarga.Codigo.val() }, function (retorno) {
                        if (retorno.Success) {
                            PreencherObjetoKnout(_modeloVeicularCarga, retorno);

                            _pesquisaModeloVeicularCarga.ExibirFiltros.visibleFade(false);
                            _CRUDModeloVeicularCarga.Atualizar.visible(true);
                            _CRUDModeloVeicularCarga.Cancelar.visible(true);
                            _CRUDModeloVeicularCarga.Excluir.visible(true);
                            _CRUDModeloVeicularCarga.Adicionar.visible(false);

                            preencherEixo(retorno.Data.Eixo);
                            verificarPossuiPalete();
                            verificarPossuiCubagem();
                            $("#myTab a:eq(2)").tab("show");
                        }
                        else
                            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                    });
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function atualizarEixoClick() {
    Salvar(_cadastroEixo, "ModeloVeicularCarga/AtualizarEixo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _cadastroEixo.NomeImagem.val(retorno.Data.NomeImagem);

                PreencherObjetoKnout(_eixo.Eixos()[_cadastroEixo.Numero.val() - 1], { Data: RetornarObjetoPesquisa(_cadastroEixo) });
                fecharModalCadastroEixo();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarEixoClick() {
    fecharModalCadastroEixo();
}

function excluirEixoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o eixo " + _cadastroEixo.Numero.val() + "?", function () {
        executarReST("ModeloVeicularCarga/ExcluirEixoPorCodigo", { Codigo: _cadastroEixo.Codigo.val(), NumeroEixosAnterior: _numeroEixosAnterior }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    var cadastroEixo = new CadastroEixo(_cadastroEixo.Numero.val(), _modeloVeicularCarga.Codigo.val());

                    PreencherObjetoKnout(_eixo.Eixos()[_cadastroEixo.Numero.val() - 1], { Data: RetornarObjetoPesquisa(cadastroEixo) });

                    fecharModalCadastroEixo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function exibirModalCadastroEixoClick() {
    var numero = parseInt($(this).data("numero"));

    if (!isNaN(numero)) {
        var cadastroEixo = _eixo.Eixos()[numero - 1];

        if (cadastroEixo) {
            PreencherObjetoKnout(_cadastroEixo, { Data: RetornarObjetoPesquisa(cadastroEixo) });
            controlarBotoesHabilitados();
            exibirModalCadastroEixo();
        }
    }
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _cadastroEixo.Codigo.val() > 0;

    _CRUDCadastroEixo.Atualizar.visible(isEdicao);
    _CRUDCadastroEixo.Replicar.visible(isEdicao);
    _CRUDCadastroEixo.Excluir.visible(isEdicao);
    _CRUDCadastroEixo.Cancelar.visible(isEdicao);
    _CRUDCadastroEixo.Adicionar.visible(!isEdicao);
}

function controlarEdicaoEixo(novaQuantidade) {
    if (_isControlarEdicaoEixo && novaQuantidade) {
        if (novaQuantidade !== _eixo.Eixos().length) {
            var totalEixos = _eixo.Eixos().length;

            if (novaQuantidade > totalEixos) {
                desabilitarExibirModalCadastroEixoClick();

                for (var i = (totalEixos + 1); i <= novaQuantidade; i++) {
                    var eixo = new CadastroEixo(i, _modeloVeicularCarga.Codigo.val());

                    _eixo.Eixos.push(eixo);
                }

                habilitarExibirModalCadastroEixoClick();
            }
            else if (isExisteEixosCadastrados(novaQuantidade)) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Existem eixos cadastrados. Remova os eixos até o número desejado");

                _modeloVeicularCarga.NumeroEixos.val(totalEixos);
            }
            else
                _eixo.Eixos.splice(novaQuantidade);
        }
    }
}

function controlarQuantidadeEstepes(novaQuantidade) {
    _eixo.Estepes.removeAll();

    for (var i = 1; i <= novaQuantidade; i++) {
        var estepe = new Estepe(i);

        _eixo.Estepes.push(estepe);
    }
}

function desabilitarExibirModalCadastroEixoClick() {
    $(".eixo-conteudo-imagem").off("click", exibirModalCadastroEixoClick);
}

function exibirModalCadastroEixo() {
    Global.abrirModal('divModalCadastroEixo');
    $("#divModalCadastroEixo").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroEixo);
    });
}

function fecharModalCadastroEixo() {
    Global.fecharModal('divModalCadastroEixo');
}

function habilitarExibirModalCadastroEixoClick() {
    $(".eixo-conteudo-imagem").on("click", exibirModalCadastroEixoClick);
}

function isExisteEixosCadastrados(novaQuantidade) {
    var totalEixos = _eixo.Eixos().length;

    for (var i = novaQuantidade; i < totalEixos; i++) {
        if (_eixo.Eixos()[i].Codigo.val() > 0)
            return true;
    }

    return false;
}

function isNumeroEixosAlterado() {
    return _numeroEixosAnterior !== _modeloVeicularCarga.NumeroEixos.val();
}

function limparEixo() {
    _isControlarEdicaoEixo = false;
    _numeroEixosAnterior = 0;

    LimparCampos(_eixo);

    _eixo.Eixos.removeAll();
    _eixo.Estepes.removeAll();

    controlarBotoesHabilitados();

    desabilitarExibirModalCadastroEixoClick();
    $("#liEixos").hide();
}

function preencherEixo(dadosEixo) {
    _eixo.CalibragemAposKm.val(dadosEixo.CalibragemAposKm);
    _eixo.GerarAlertaManutencao.val(dadosEixo.GerarAlertaManutencao);
    _eixo.QuantidadeEstepes.val(dadosEixo.QuantidadeEstepes);

    for (var i = 0; i < dadosEixo.Eixos.length; i++) {
        var eixo = new CadastroEixo();

        PreencherObjetoKnout(eixo, { Data: dadosEixo.Eixos[i] });

        _eixo.Eixos.push(eixo);
    }

    habilitarExibirModalCadastroEixoClick();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.ExibirAbaDeEixosNoModeloVeicular)
        $("#liEixos").show();

    _isControlarEdicaoEixo = true;
    _numeroEixosAnterior = _modeloVeicularCarga.NumeroEixos.val();
}