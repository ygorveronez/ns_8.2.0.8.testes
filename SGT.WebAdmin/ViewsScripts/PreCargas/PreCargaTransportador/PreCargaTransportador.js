/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPreCargaOfertaTransportador.js" />

// #region Objetos Globais do Arquivo

var _dadosPesquisaPreCargas;
var _pesquisaPreCargaTransportador;
var _preCargaDetalhes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaPreCargaTransportador = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid() });
    this.NumeroPreCarga = PropertyEntity({ text: "Número do Pré Planejamento:", def: "", val: ko.observable(""), maxlength: 50 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPreCargaOfertaTransportador.Todas), options: EnumSituacaoPreCargaOfertaTransportador.obterOpcoesPesquisa(), def: EnumSituacaoPreCargaOfertaTransportador.Todas, text: "Situação:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(false);
            recarregarPesquisaPreCargas();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: preCargasPesquisaOnScroll });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.Requisicao = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
    this.PreCargas = ko.observableArray();
}

var PreCargaDetalhes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, visible: false });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

    this.NumeroPreCarga = PropertyEntity({ text: "Pré Planejamento:" });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data:" });
    this.Filial = PropertyEntity({ text: "Filial:" });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo do Veículo:", type: types.entity, codEntity: ko.observable(0) });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:" });
    this.RegioesDestino = PropertyEntity({ text: "Regiões de Destino:" });
    this.CidadesDestino = PropertyEntity({ text: "Cidades de Destino:" });
    this.EstadosDestino = PropertyEntity({ text: "Estados de Destino:" });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "*Motorista:", label: "Motorista:", val: ko.observable(""), def: "", enable: ko.observable(false), required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo: "), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta): "), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false), required: false });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta 2):"), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false), required: false });

    this.ConfirmarCarga = PropertyEntity({ eventClick: confirmarPreCargaClick, type: types.event, text: "Aceitar sem Informar Dados", idGrid: guid(), visible: ko.observable(false) });
    this.SalvarDadosTransporte = PropertyEntity({ eventClick: salvarDadosTransportePreCargaClick, type: types.event, text: ko.observable("Confirmar"), idGrid: guid(), visible: ko.observable(false) });
    this.RejeitarCarga = PropertyEntity({ eventClick: rejeitarPreCargaClick, type: types.event, text: "Rejeitar", idGrid: guid(), visible: ko.observable(false) });
}

var PreCargaModel = function (dadosPreCarga) {
    var self = this;

    this.Codigo = PropertyEntity({ });
    this.Situacao = PropertyEntity({ cssClass: ko.observable(EnumSituacaoPreCargaOfertaTransportador.obterClasseCor(dadosPreCarga.Situacao)) });
    this.HorarioLimiteConfirmacao = PropertyEntity({ val: ko.observable(dadosPreCarga.HorarioLimiteConfirmacao), visible: ko.observable(false) });

    this.NumeroPreCarga = PropertyEntity({ text: "Pré Planejamento:" });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data:" });
    this.Filial = PropertyEntity({ text: "Filial:" });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo do Veículo:" });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:" });
    this.RegioesDestino = PropertyEntity({ text: "Regiões de Destino:" });
    this.CidadesDestino = PropertyEntity({ text: "Cidades de Destino:" });
    this.EstadosDestino = PropertyEntity({ text: "Estados de Destino:" });
    this.Motorista = PropertyEntity({ text: "Motorista:", visible: ko.observable(false) });
    this.Placas = PropertyEntity({ text: "Veículos:", visible: ko.observable(false) });

    this.ExibirDetalhes = PropertyEntity({ eventClick: exibirDetalhesPreCargaClick, type: types.event, val: ko.observable(false), def: false });
    this.TenhoInteresse = PropertyEntity({ eventClick: informarInteressePreCargaClick, type: types.event, text: "Tenho Interesse", icon: "fa fa-thumbs-up", visible: ko.observable(false) });
    this.InformarDadosTransporte = PropertyEntity({ eventClick: exibirDetalhesPreCargaClick, type: types.event, text: "Informar Dados de Transporte", icon: "fa fa-truck", visible: ko.observable(false) });

    if (self.HorarioLimiteConfirmacao.val()) {
        setTimeout(function () {
            $("#" + self.HorarioLimiteConfirmacao.id)
                .countdown(moment(self.HorarioLimiteConfirmacao.val(), "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                .on('update.countdown', function (event) {
                    if (event.elapsed)
                        $(this).text(" [esgotado]")
                    else {
                        if (event.offset.totalDays > 0)
                            $(this).text(" [" + event.strftime('%-Dd %H:%M:%S') + "]");
                        else
                            $(this).text(" [" + event.strftime('%H:%M:%S') + "]");
                    }
                });
        }, 300);
    }

    switch (dadosPreCarga.Situacao) {
        case EnumSituacaoPreCargaOfertaTransportador.AguardandoAceite:
        case EnumSituacaoPreCargaOfertaTransportador.AguardandoConfirmacao:
            this.InformarDadosTransporte.visible(true);
            break;

        case EnumSituacaoPreCargaOfertaTransportador.Confirmada:
            this.Motorista.visible(true);
            this.Placas.visible(true);
            this.ExibirDetalhes.val(true);
            break;

        case EnumSituacaoPreCargaOfertaTransportador.Disponivel:
            this.TenhoInteresse.visible(true);
            break;
    }

    PreencherObjetoKnout(self, { Data: dadosPreCarga });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPreCargaTransportador() {
    _pesquisaPreCargaTransportador = new PesquisaPreCargaTransportador();
    KoBindings(_pesquisaPreCargaTransportador, "knockoutPesquisaPreCargaTransportador", false);

    _preCargaDetalhes = new PreCargaDetalhes();
    KoBindings(_preCargaDetalhes, "knockoutPreCargaDetalhes");

    new BuscarModelosVeicularesCarga(_pesquisaPreCargaTransportador.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_pesquisaPreCargaTransportador.TipoCarga);
    new BuscarTiposOperacao(_pesquisaPreCargaTransportador.TipoOperacao);

    new BuscarMotoristas(_preCargaDetalhes.Motorista, null, _preCargaDetalhes.Transportador, null, null, null, null, null, true);
    new BuscarVeiculos(_preCargaDetalhes.Veiculo, retornoConsultaVeiculo, _preCargaDetalhes.Transportador, _preCargaDetalhes.ModeloVeicularCarga, null, true, null, _preCargaDetalhes.TipoCarga, true, null, null, "0");
    new BuscarVeiculos(_preCargaDetalhes.Reboque, retornoConsultaReboque, _preCargaDetalhes.Transportador, null, null, true, null, null, true, null, null, "1");
    new BuscarVeiculos(_preCargaDetalhes.SegundoReboque, retornoConsultaSegundoReboque, _preCargaDetalhes.Transportador, null, null, true, null, null, true, null, null, "1");

    //previne a página de dar scroll quando o usuário está usando o scroll de uma das divs
    $('.scrollable').bind('mousewheel DOMMouseScroll', function (e) {
        if ($(this)[0].scrollHeight !== $(this).outerHeight()) {
            var e0 = e.originalEvent;
            var delta = e0.wheelDelta || -e0.detail;

            this.scrollTop += (delta < 0 ? 1 : -1) * 30;

            e.preventDefault();
        }
    });

    recarregarPesquisaPreCargas();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a eventos

function confirmarPreCargaClick() {
    executarReST("PreCargaTransportador/ConfirmarPreCarga", { Codigo: _preCargaDetalhes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalPreCargaDetalhes');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Pré planejamento confirmado com sucesso!");
                atualizarPreCarga(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function exibirDetalhesPreCargaClick(registroSelecionado) {
    executarReST("PreCargaTransportador/ObterDetalhesPreCarga", { Codigo: registroSelecionado.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampos(_preCargaDetalhes);
                PreencherObjetoKnout(_preCargaDetalhes, retorno);

                var habilitarEdicaoDadosTransporte = retorno.Data.PermitirEdicaoDadosTransporte;

                _preCargaDetalhes.Motorista.enable(habilitarEdicaoDadosTransporte);
                _preCargaDetalhes.Veiculo.enable(habilitarEdicaoDadosTransporte);
                _preCargaDetalhes.Reboque.enable(habilitarEdicaoDadosTransporte);
                _preCargaDetalhes.SegundoReboque.enable(habilitarEdicaoDadosTransporte);

                _preCargaDetalhes.ConfirmarCarga.visible(habilitarEdicaoDadosTransporte && Boolean(registroSelecionado.HorarioLimiteConfirmacao.val()));
                _preCargaDetalhes.RejeitarCarga.visible(habilitarEdicaoDadosTransporte);
                _preCargaDetalhes.SalvarDadosTransporte.visible(habilitarEdicaoDadosTransporte);
                _preCargaDetalhes.SalvarDadosTransporte.text(_preCargaDetalhes.ConfirmarCarga.visible() ? "Confirmar com Dados" : "Confirmar");

                controlarExibicaoCamposVeiculo(habilitarEdicaoDadosTransporte);

                Global.abrirModal("divModalPreCargaDetalhes");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function informarInteressePreCargaClick(registroSelecionado) {
    executarReST("PreCargaTransportador/InformarInteressePreCarga", { Codigo: registroSelecionado.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalPreCargaDetalhes');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Interesse no pré planejamento marcado com sucesso!");
                atualizarPreCarga(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function preCargasPesquisaOnScroll(e, sender) {
    var elemento = sender.target;

    if ((_pesquisaPreCargaTransportador.Inicio.val() < _pesquisaPreCargaTransportador.Total.val()) && (elemento.scrollTop >= (elemento.scrollHeight - elemento.offsetHeight)))
        carregarPreCargas();
}

function rejeitarPreCargaClick() {
    executarReST("PreCargaTransportador/RejeitarPreCarga", { Codigo: _preCargaDetalhes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalPreCargaDetalhes');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Pré planejamento rejeitado com sucesso!");
                atualizarPreCarga(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function salvarDadosTransportePreCargaClick() {
    if (!ValidarCamposObrigatorios(_preCargaDetalhes)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios!", "Informe os campos obrigatórios!");
        return;
    }

    var dadosTransporteSalvar = {
        Codigo: _preCargaDetalhes.Codigo.val(),
        Veiculo: _preCargaDetalhes.Veiculo.codEntity(),
        Reboque: _preCargaDetalhes.Reboque.codEntity(),
        SegundoReboque: _preCargaDetalhes.SegundoReboque.codEntity(),
        Motorista: _preCargaDetalhes.Motorista.codEntity()
    };

    executarReST("PreCargaTransportador/SalvarDadosTransportePreCarga", dadosTransporteSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalPreCargaDetalhes');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados de transporte salvos com sucesso!");
                atualizarPreCarga(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

// 3endregion Funções Associadas a Eventos

// #region Funções Públicas

// #endregion Funções Públicas

function atualizarPreCarga(preCarga) {
    var preCargas = _pesquisaPreCargaTransportador.PreCargas();

    for (var i = 0; i < preCargas.length; i++) {
        if (preCargas[i].Codigo.val() == preCarga.Codigo) {
            _pesquisaPreCargaTransportador.PreCargas.replace(preCargas[i], new PreCargaModel(preCarga));
            break;
        }
    }
}

// #region Funções Privadas

function carregarPreCargas() {
    if (_pesquisaPreCargaTransportador.Requisicao.val())
        return;

    _pesquisaPreCargaTransportador.Requisicao.val(true);

    var quantidadePorVez = 12;

    _dadosPesquisaPreCargas.Inicio = _pesquisaPreCargaTransportador.Inicio.val();
    _dadosPesquisaPreCargas.Limite = quantidadePorVez;

    executarReST("PreCargaTransportador/ConsultarPreCargas", _dadosPesquisaPreCargas, function (retorno) {
        if (retorno.Success) {
            _pesquisaPreCargaTransportador.Total.val(retorno.Data.Total);
            _pesquisaPreCargaTransportador.Inicio.val(_pesquisaPreCargaTransportador.Inicio.val() + quantidadePorVez);

            $.each(retorno.Data.PreCargas, function (i, preCarga) {
                _pesquisaPreCargaTransportador.PreCargas.push(new PreCargaModel(preCarga));
            });
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        _pesquisaPreCargaTransportador.Requisicao.val(false);
    }, null, false);
}

function controlarExibicaoCamposVeiculo(habilitarEdicaoDadosTransporte) {
    var reboqueVisivel = (_preCargaDetalhes.NumeroReboques.val() >= 1);
    var segundoReboqueVisivel = (_preCargaDetalhes.NumeroReboques.val() > 1);

    if (!reboqueVisivel)
        LimparCampoEntity(_preCargaDetalhes.Reboque);

    if (!segundoReboqueVisivel)
        LimparCampoEntity(_preCargaDetalhes.SegundoReboque);

    _preCargaDetalhes.Veiculo.required = habilitarEdicaoDadosTransporte;
    _preCargaDetalhes.Veiculo.text((_preCargaDetalhes.Veiculo.required ? "*" : "") + (reboqueVisivel ? "Tração (Cavalo):" : "Veiculo:"));

    _preCargaDetalhes.Reboque.required = (reboqueVisivel && habilitarEdicaoDadosTransporte);
    _preCargaDetalhes.Reboque.text((_preCargaDetalhes.Reboque.required ? "*" : "") + (segundoReboqueVisivel ? "Veículo (Carreta 1):" : "Veículo (Carreta):"));
    _preCargaDetalhes.Reboque.visible(reboqueVisivel);

    _preCargaDetalhes.SegundoReboque.required = (segundoReboqueVisivel && habilitarEdicaoDadosTransporte);
    _preCargaDetalhes.SegundoReboque.text((_preCargaDetalhes.SegundoReboque.required ? "*" : "") + "Veículo (Carreta 2):");
    _preCargaDetalhes.SegundoReboque.visible(segundoReboqueVisivel);
}

function recarregarPesquisaPreCargas() {
    _pesquisaPreCargaTransportador.Total.val(0);
    _pesquisaPreCargaTransportador.Inicio.val(0);
    _pesquisaPreCargaTransportador.PreCargas.removeAll();

    _dadosPesquisaPreCargas = RetornarObjetoPesquisa(_pesquisaPreCargaTransportador);

    carregarPreCargas();
}

function retornoConsultaReboque(reboqueSelecionado) {
    if (_preCargaDetalhes.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_preCargaDetalhes.Reboque);
    }
    else {
        _preCargaDetalhes.Reboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaDetalhes.Reboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaDetalhes.Reboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaSegundoReboque(reboqueSelecionado) {
    if (_preCargaDetalhes.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_preCargaDetalhes.SegundoReboque);
    }
    else {
        _preCargaDetalhes.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaDetalhes.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaDetalhes.SegundoReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaVeiculo(veiculoSelecionado) {
    _preCargaDetalhes.Veiculo.codEntity(veiculoSelecionado.Codigo);
    _preCargaDetalhes.Veiculo.entityDescription(veiculoSelecionado.Placa);
    _preCargaDetalhes.Veiculo.val(veiculoSelecionado.Placa);

    Global.setarFocoProximoCampo(_preCargaDetalhes.Veiculo.id);
}

// #endregion Funções Privadas
