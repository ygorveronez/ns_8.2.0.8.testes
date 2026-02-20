/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/PeriodoCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="Carregamento.js" />

// #region Objetos Globais do Arquivo

var _carregamentoFilial;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CarregamentoFilial = function () {
    this.UtilizarFilialPadrao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ListaDadosPorFilial = _carregamento.ListaDadosPorFilial;
}

var CarregamentoFilialDados = function () {
    var permitirInformarDataDescarregamentoPorFilial = isPermitirInformarDataDescarregamentoPorFilial();

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.FilialAtiva = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsultaEmpresa;
    this.ConsultaPeriodoCarregamento;

    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataCarregamento.getRequiredFieldDescription(), required: true, getType: (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? typesKnockout.dateTime : typesKnockout.date), enable: ko.observable(true), idBtnSearch: guid() });
    this.DataDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataDescarregamento.getRequiredFieldDescription(), required: permitirInformarDataDescarregamentoPorFilial, getType: (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? typesKnockout.dateTime : typesKnockout.date), enable: ko.observable(true), visible: ko.observable(permitirInformarDataDescarregamentoPorFilial) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: _carregamentoTransporte.Empresa.text, idBtnSearch: guid(), required: _carregamentoTransporte.Empresa.required, visible: _CONFIGURACAO_TMS.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento, enable: _carregamentoTransporte.Empresa.enable });
    this.EncaixarHorario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EscolherHorarioCarregamentoPorLista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, eventClick: alterarDataCarregamentoClick });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCarregamentoFilial() {
    _carregamentoFilial = new CarregamentoFilial();
    _carregamentoFilial.UtilizarFilialPadrao.val(isUtilizarDadosCarregamentoPorFilialPadrao());

    if (_carregamentoFilial.UtilizarFilialPadrao.val()) {
        var carregamentoDataFilialPadrao = new CarregamentoFilialDados();

        carregamentoDataFilialPadrao.FilialAtiva.val(true);

        _carregamentoFilial.ListaDadosPorFilial.push(carregamentoDataFilialPadrao);

        ConfigurarCamposKnockout(carregamentoDataFilialPadrao);
        criarConsultaPeriodoCarregamentoPorFilial(carregamentoDataFilialPadrao);
        criarConsultaEmpresaPorFilial(carregamentoDataFilialPadrao);
    }

    recarregarCarouselDataCarregamentoPorFilial();

    $('#carousel-montagem-carga-dados-por-filial').on('slid.bs.carousel', function () {
        var codigoFilial = $("#carousel-montagem-carga-dados-por-filial .carousel-inner .carousel-item.active").data('codigo-filial') || 0;
        var listaDadosPorFilial = _carregamentoFilial.ListaDadosPorFilial().slice();
        
        for (var i = 0; i < listaDadosPorFilial.length; i++)
            listaDadosPorFilial[i].FilialAtiva.val(listaDadosPorFilial[i].Filial.codEntity() == codigoFilial);

        buscarCapacidadeJanelaCarregamento();
    })
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function atualizarDadosCarregamentoPorFilial() {
    var dadosPorFiliaisAdicionados = _carregamentoFilial.ListaDadosPorFilial().slice();
    var pedidos = PEDIDOS_SELECIONADOS();

    if (_carregamentoFilial.UtilizarFilialPadrao.val()) {
        var dadosPorFilialPadrao = dadosPorFiliaisAdicionados[0];

        dadosPorFilialPadrao.Filial.codEntity((pedidos.length > 0) ? pedidos[0].CodigoFilial : 0);
        dadosPorFilialPadrao.Filial.val((pedidos.length > 0) ? pedidos[0].Filial : "");

        if (_carregamento.Carregamento.codEntity() == 0) {
            var datasPedidosSelecionados = _CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? pedidos.map(pedido => pedido.DataHoraCarregamentoPedido) : pedidos.map(pedido => pedido.DataCarregamentoPedido);
            var datasDistintas = datasPedidosSelecionados.filter((dataAtual, indiceDataAtual, datas) => datas.indexOf(dataAtual) === indiceDataAtual);

            if (datasDistintas.length == 1 && (!_CONFIGURACAO_TMS.MontagemCarga.DataAtualNovoCarregamento || dadosPorFilialPadrao.DataCarregamento.val() == ""))
                dadosPorFilialPadrao.DataCarregamento.val(datasDistintas[0]);
        }

        buscarCapacidadeJanelaCarregamento();
        return;
    }

    var filiaisDosPedidos = pedidos.map(function (pedido) { return { Codigo: pedido.CodigoFilial, Descricao: pedido.Filial }; }).filter((filialAtual, indexFilialAtual, filiais) => filiais.map(filial => filial.Codigo).indexOf(filialAtual.Codigo) === indexFilialAtual);
    var filiaisDosPedidosAdicionar = filiaisDosPedidos.filter((filial) => dadosPorFiliaisAdicionados.map(carregamentoDataFilial => carregamentoDataFilial.Filial.codEntity()).indexOf(filial.Codigo) == -1);
    var listaDadosPorFilial = dadosPorFiliaisAdicionados.filter((carregamentoDataFilial) => filiaisDosPedidos.map(filial => filial.Codigo).indexOf(carregamentoDataFilial.Filial.codEntity()) >= 0);

    for (var i = 0; i < filiaisDosPedidosAdicionar.length; i++) {
        var filialPedidoAdicionar = filiaisDosPedidosAdicionar[i];
        var carregamentoDataFilial = new CarregamentoFilialDados();

        carregamentoDataFilial.Filial.codEntity(filialPedidoAdicionar.Codigo);
        carregamentoDataFilial.Filial.val(filialPedidoAdicionar.Descricao);

        listaDadosPorFilial.push(carregamentoDataFilial);
    }

    limparListaDadosPorFilial();
    
    _carregamentoFilial.ListaDadosPorFilial(listaDadosPorFilial);

    for (var i = 0; i < listaDadosPorFilial.length; i++) {
        var dadosPorFilial = listaDadosPorFilial[i];

        if (_carregamento.Carregamento.codEntity() == 0) {
            var pedidosSelecionadosPorFilial = pedidos.filter((pedido) => pedido.CodigoFilial == dadosPorFilial.Filial.codEntity());
            var datasPedidosSelecionadosPorFilial = _CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? pedidosSelecionadosPorFilial.map(pedido => pedido.DataHoraCarregamentoPedido) : pedidosSelecionadosPorFilial.map(pedido => pedido.DataCarregamentoPedido);
            var datasDistintasPorFilial = datasPedidosSelecionadosPorFilial.filter((dataAtual, indiceDataAtual, datas) => datas.indexOf(dataAtual) === indiceDataAtual);

            if (datasDistintasPorFilial.length == 1 && (!_CONFIGURACAO_TMS.MontagemCarga.DataAtualNovoCarregamento || dadosPorFilial.DataCarregamento.val() == ""))
                dadosPorFilial.DataCarregamento.val(datasDistintasPorFilial[0]);
        }

        dadosPorFilial.FilialAtiva.val(i == 0);

        ConfigurarCamposKnockout(dadosPorFilial);
        criarConsultaPeriodoCarregamentoPorFilial(dadosPorFilial);
        criarConsultaEmpresaPorFilial(dadosPorFilial);
    }

    recarregarCarouselDataCarregamentoPorFilial();
    buscarCapacidadeJanelaCarregamento();
}

function definirDataCarregamentoPorFilial(dataCarregamento, encaixarHorario) {
    var dadosPorFilial = obterDadosPorFilialAtiva();

    if (!dadosPorFilial)
        return;

    dadosPorFilial.DataCarregamento.val(dataCarregamento);
    dadosPorFilial.EncaixarHorario.val(Boolean(encaixarHorario));

    buscarCapacidadeJanelaCarregamento();
}

function obterDadosCarregamentoPorFilial() {
    var dadosPorFilial = obterDadosPorFilialAtiva();

    if (!dadosPorFilial)
        return {
            CodigoFilial: 0,
            DescricaoFilial: "",
            DataCarregamento: ""
        };

    return {
        CodigoFilial: dadosPorFilial.Filial.codEntity(),
        DescricaoFilial: dadosPorFilial.Filial.val(),
        DataCarregamento: dadosPorFilial.DataCarregamento.val()
    };
}

function preencherDadosCarregamentoPorFilial(carregamento) {
    var dadosPorFiliaisAdicionados = _carregamentoFilial.ListaDadosPorFilial().slice();

    if (_carregamentoFilial.UtilizarFilialPadrao.val()) {
        var dadosPorFilialPadrao = dadosPorFiliaisAdicionados[0];

        dadosPorFilialPadrao.DataCarregamento.val(carregamento.DataCarregamento);
        dadosPorFilialPadrao.DataDescarregamento.val(carregamento.DataDescarregamento);
        dadosPorFilialPadrao.EncaixarHorario.val(carregamento.EncaixarHorario);
        dadosPorFilialPadrao.EscolherHorarioCarregamentoPorLista.val(carregamento.EscolherHorarioCarregamentoPorLista);

        buscarCapacidadeJanelaCarregamento();
        return;
    }

    _carregamentoFilial.ListaDadosPorFilial.removeAll();

    for (var i = 0; i < carregamento.DadosPorFiliais.length; i++) {
        var dadosPorFilialAdicionar = carregamento.DadosPorFiliais[i];
        var dadosPorFilial = new CarregamentoFilialDados();

        dadosPorFilial.DataCarregamento.val(dadosPorFilialAdicionar.DataCarregamento);
        dadosPorFilial.DataDescarregamento.val(dadosPorFilialAdicionar.DataDescarregamento);
        dadosPorFilial.Empresa.codEntity(dadosPorFilialAdicionar.Empresa.Codigo);
        dadosPorFilial.Empresa.val(dadosPorFilialAdicionar.Empresa.Descricao);
        dadosPorFilial.EncaixarHorario.val(dadosPorFilialAdicionar.EncaixarHorario);
        dadosPorFilial.EscolherHorarioCarregamentoPorLista.val(carregamento.EscolherHorarioCarregamentoPorLista);
        dadosPorFilial.Filial.codEntity(dadosPorFilialAdicionar.Filial.Codigo);
        dadosPorFilial.Filial.val(dadosPorFilialAdicionar.Filial.Descricao);
        dadosPorFilial.FilialAtiva.val(i == 0);

        _carregamentoFilial.ListaDadosPorFilial.push(dadosPorFilial);

        ConfigurarCamposKnockout(dadosPorFilial);
        criarConsultaPeriodoCarregamentoPorFilial(dadosPorFilial);
        criarConsultaEmpresaPorFilial(dadosPorFilial);
    }

    recarregarCarouselDataCarregamentoPorFilial();
    buscarCapacidadeJanelaCarregamento();
}

function preencherDadosCarregamentoPorFilialSalvar(carregamento) {
    var dadosPorFiliais = _carregamentoFilial.ListaDadosPorFilial().slice();

    if (_carregamentoFilial.UtilizarFilialPadrao.val()) {
        var dadosPorCarregamento = dadosPorFiliais[0];

        if (!validarCamposDadosCarregamentoPorFilial(dadosPorCarregamento, false))
            return false;

        carregamento["DadosPorFilialPadrao"] = JSON.stringify({
            DataCarregamento: dadosPorCarregamento.DataCarregamento.val(),
            DataDescarregamento: dadosPorCarregamento.DataDescarregamento.val(),
            EncaixarHorario: dadosPorCarregamento.EncaixarHorario.val()
        });

        return true;
    }

    var dadosPorFiliaisSalvar = [];

    for (var i = 0; i < dadosPorFiliais.length; i++) {
        var dadosPorFilial = dadosPorFiliais[i];
        var multiplasFiliaisInformadas = dadosPorFiliais.length > 1;

        if (!validarCamposDadosCarregamentoPorFilial(dadosPorFilial, multiplasFiliaisInformadas))
            return false;

        dadosPorFiliaisSalvar.push({
            DataCarregamento: dadosPorFilial.DataCarregamento.val(),
            DataDescarregamento: dadosPorFilial.DataDescarregamento.val(),
            Empresa: (multiplasFiliaisInformadas ? dadosPorFilial.Empresa.codEntity() : 0),
            EncaixarHorario: dadosPorFilial.EncaixarHorario.val(),
            Filial: dadosPorFilial.Filial.codEntity()
        });
    }

    carregamento["DadosPorFiliais"] = JSON.stringify(dadosPorFiliaisSalvar);

    return true;
}

function removerEmpresaDadosCarregamentoPorFilial() {
    var dadosPorFiliaisAdicionados = _carregamentoFilial.ListaDadosPorFilial().slice();

    for (var i = 0; i < dadosPorFiliaisAdicionados.length; i++)
        LimparCampoEntity(dadosPorFiliaisAdicionados[i].Empresa);
}

// #endregion Funções Públicas

// #region Funções Privadas

function criarConsultaEmpresaPorFilial(carregamentoDataFilial) {
    carregamentoDataFilial.ConsultaEmpresa = new BuscarTransportadores(carregamentoDataFilial.Empresa, function (registroSelecionado) {
        carregamentoDataFilial.Empresa.val(registroSelecionado.Descricao);
        carregamentoDataFilial.Empresa.entityDescription(registroSelecionado.Descricao);
        carregamentoDataFilial.Empresa.codEntity(registroSelecionado.Codigo);

        if (_carregamentoTransporte.Empresa.val() == "") {
            _carregamentoTransporte.Empresa.val(registroSelecionado.Descricao);
            _carregamentoTransporte.Empresa.entityDescription(registroSelecionado.Descricao);
            _carregamentoTransporte.Empresa.codEntity(registroSelecionado.Codigo);
            _carregamentoTransporte.RaizCNPJEmpresa.val(registroSelecionado.RaizCnpj);
        }
    }, null, null, null, null, null, null, _carregamentoTransporte.RaizCNPJEmpresa);
}

function criarConsultaPeriodoCarregamentoPorFilial(carregamentoDataFilial) {
    if (!_carregamento.InformarPeriodoCarregamento.val())
        return;

    var botoesDataCarregamento = document
        .getElementById(carregamentoDataFilial.DataCarregamento.id)
        .parentElement
        .getElementsByClassName("input-group-append")

    if (botoesDataCarregamento.length > 0)
        botoesDataCarregamento[0].classList.add("d-none");

    carregamentoDataFilial.ConsultaPeriodoCarregamento = new BuscarPeriodoCarregamento(
        carregamentoDataFilial.DataCarregamento,
        function (retorno) {
            if (retorno)
                carregamentoDataFilial.DataCarregamento.val(retorno.InicioCarregamento);
            else
                carregamentoDataFilial.DataCarregamento.val("");

            buscarCapacidadeJanelaCarregamento();
        },
        carregamentoDataFilial.DataCarregamento,
        undefined,
        carregamentoDataFilial.Filial,
        _carregamentoTransporte.TipoDeCarga,
        true
    );
}

function destruirConsultaEmpresaPorFilial(carregamentoDataFilial) {
    if (!carregamentoDataFilial.ConsultaEmpresa)
        return;

    carregamentoDataFilial.ConsultaEmpresa.Destroy();
}

function destruirConsultaPeriodoCarregamentoPorFilial(carregamentoDataFilial) {
    if (!carregamentoDataFilial.ConsultaPeriodoCarregamento)
        return;

    carregamentoDataFilial.ConsultaPeriodoCarregamento.Destroy();
}

function isPermitirInformarDataDescarregamentoPorFilial() {
    return _CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga && !_carregamento.InformarPeriodoCarregamento.val();
}

function isUtilizarDadosCarregamentoPorFilialPadrao() {
    return !_CONFIGURACAO_TMS.PermitirAlterarDataCarregamentoCargaNoPedido;
}

function limparListaDadosPorFilial() {
    var listaDadosPorFilial = _carregamentoFilial.ListaDadosPorFilial().slice();

    for (var i = 0; i < listaDadosPorFilial.length; i++) {
        var dadosPorFilial = listaDadosPorFilial[i];

        destruirConsultaPeriodoCarregamentoPorFilial(dadosPorFilial);
        destruirConsultaEmpresaPorFilial(dadosPorFilial);
    }

    _carregamentoFilial.ListaDadosPorFilial.removeAll();
}

function obterDadosPorFilialAtiva() {
    var dadosPorFiliaisAdicionados = _carregamentoFilial.ListaDadosPorFilial().slice();
    var dadosPorFiliais = dadosPorFiliaisAdicionados.filter((carregamentoFilialDados) => carregamentoFilialDados.FilialAtiva.val());

    if (dadosPorFiliais.length == 0)
        return undefined;

    return dadosPorFiliais[0];
}

function recarregarCarouselDataCarregamentoPorFilial() {
    var carouselCapacidadeCarregamento = document.querySelector('#carousel-montagem-carga-dados-por-filial');
    new bootstrap.Carousel(carouselCapacidadeCarregamento, { interval: false });
}

function validarCamposDadosCarregamentoPorFilial(dadosPorFilial, validarCamposPorFilial) {
    if (!ValidarCampoObrigatorioMap(dadosPorFilial.DataCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.CamposObrigatorios, Localization.Resources.Cargas.MontagemCarga.InformeDataDeCarregamento + (validarCamposPorFilial ? + Localization.Resources.Cargas.MontagemCarga.DaFilial + dadosPorFilial.Filial.val() : "") + "!");
        return false;
    }

    if (!ValidarCampoObrigatorioMap(dadosPorFilial.DataDescarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.CamposObrigatorios, Localization.Resources.Cargas.MontagemCarga.InformeDataDeDescarregamento + (validarCamposPorFilial ? + Localization.Resources.Cargas.MontagemCarga.DaFilial + dadosPorFilial.Filial.val() : "") + "!");
        return false;
    }

    if (validarCamposPorFilial) {
        if (!ValidarCampoObrigatorioEntity(dadosPorFilial.Empresa)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.CamposObrigatorios, Localization.Resources.Cargas.MontagemCarga.InformeTransportadorDaFilial.format(dadosPorFilial.Filial.val()));
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
