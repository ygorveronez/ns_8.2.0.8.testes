/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="../../../js/Global/Mapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _roteirizadorCarregamento = null;
var _gridReorderCarregamento = null;
var _Pontos = null;
var _PontosOriginais = null;
var gridSemPontos = null;
var map = null;
var dir = null;
var _gridNarrativa = null;
var _mapaRenderizado = false;
var _pessoasReordenadas = null;
var _directions = [];
var _mapa = null;
var _centralizarMapa = false;
var _responseOSM = false;
var coletas = new Array();

var _tiposRota = [
    { text: "Mais Rápida", value: "fastest" },
    { text: "Menor distância", value: "shortest" }
];

var RoteirizadorCarregamento = function () {
    this.Map = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), idGrid2: guid(), visibleReorder: ko.observable(false), visibleReorderClick: visibleReorderClick });
    this.SemPontos = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.Narativa = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.Carregamento = PropertyEntity();
    //this.Origem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: ko.observable("Origem : "), def: "", enable: false });
    this.Coletas = PropertyEntity({ val: ko.observable(""), options: ko.observable(new Array()), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription()), required: false, visible: ko.observable(true), eventChange: changeColeta });

    this.Distancia = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.MontagemCargaMapa.Distancia.getFieldDescription(), def: "", enable: false });
    this.Tempo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.MontagemCargaMapa.TempoDeViagem.getFieldDescription(), def: "", enable: false });
    this.TipoRota = PropertyEntity({ val: ko.observable("shortest"), options: _tiposRota, def: "shortest", text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDaRota.getFieldDescription(), issue: 1291, required: true });

    this.PolilinhaRota = PropertyEntity({});
    this.TempoDeViagemEmMinutos = PropertyEntity({});
    this.PontosDaRota = PropertyEntity({});
    this.PedidosDeColeta = PropertyEntity({});

    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.AteOrigem), enable: ko.observable(true), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: _CONFIGURACAO_TMS.TipoUltimoPontoRoteirizacao, text: Localization.Resources.Cargas.MontagemCargaMapa.UltimoPonto.getFieldDescription(), issue: 1292, required: true });
    this.Pessoas = PropertyEntity({ val: ko.observable(new Array()), type: types.map, getType: typesKnockout.dynamic });

    this.Roteirizado = PropertyEntity({ visible: ko.observable(false) });
    this.ModoEdicao = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.Blocos = PropertyEntity({ idGrid: guid() });

    //this.Reordenar = PropertyEntity({ eventClick: reordenarPosicoesEntregasCarregamento, type: types.event, text: "Reordenas", visible: ko.observable(true) });
    this.Roteirizar = PropertyEntity({ eventClick: RoteirizarCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.BuscarRota, visible: ko.observable(true) });
    this.SalvarRota = PropertyEntity({ eventClick: salvarRotaCarregamentoCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.SalvarRotaDoCarregamento, visible: ko.observable(false) });
    this.AtualizarRota = PropertyEntity({ eventClick: function () { }, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadRoteirizadorCarregamento() {
    $.get("Content/Static/Carga/MapaRoteirizacao.html?dyn=" + guid(), function (data) {
        _roteirizadorCarregamento = new RoteirizadorCarregamento();
        $("#fdsRoteirizacao").html(data.replace(/#Codigo/g, _roteirizadorCarregamento.Map.id));
        KoBindings(_roteirizadorCarregamento, "fdsRoteirizacao");
        $("#Titulo_" + _roteirizadorCarregamento.Map.id).hide();
        $("#chat-container").hide();

        //max-heigth
        var heigth = $(window).height();
        $("#RotaEntregas_" + _roteirizadorCarregamento.Map.id).css('max-height', (heigth - 280) + 'px');
        $("#Narativa_" + _roteirizadorCarregamento.Map.id).css('max-height', (heigth - 280) + 'px');

        loadGoogleMapa();
        CarregarGridSemPontos();
    });

}

function loadConfiguracaoColeta() {
    _roteirizadorCarregamento.Coletas.text(Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription());
    _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.enable(true);
    _mapa.opcoes.moverPrimeiroPonto = false;
    _mapa.opcoes.moverUltimoPonto = true;
    _mapa.opcoes.plotarPontoPassagem = true;
    if (_roteirizadorCarregamento.PedidosDeColeta.val()) {
        _mapa.opcoes.moverPrimeiroPonto = true;
        _mapa.opcoes.moverUltimoPonto = false;
        _roteirizadorCarregamento.Coletas.text(Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription());
        _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val(EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante);
        _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.enable(false);
    }
}

function visibleReorderClick(e) {
    e.Map.visibleReorder(!e.Map.visibleReorder());
}

function salvarRotaCarregamentoCarregamentoClick() {
    salvarRotaCarregamento();
}

function renderizarCarregamentoGoogleMapsClick() {
    var reprocessar = ValidarSeMudouOrdemManualmente();
    //renderizarGoogleMapsCarregamento(reprocessar);
    if (reprocessar)
        gerarRoteirizacaoSemOrdem();
    else if (_centralizarMapa) {
        _centralizarMapa = false;

        setTimeout(function () {
            _mapa.centralizarBounds();
        }, 500);
    }
}

//*******METODOS*******

function definirPrimeiraEntregaClick(item) {
    var indiceClicado = obterIndicePontoCNPJ(item.Codigo);
    if (indiceClicado >= 0) {
        if (_Pontos[indiceClicado].TipoPonto == 2) { // Entrega
            var registrosGridReorderCarregamento = [];
            if (_gridReorderCarregamento) {
                var registros = _gridReorderCarregamento.BuscarRegistros();
                for (var i = 0; i < registros.length; i++) {
                    var indice = obterIndicePontoCNPJ(registros[i].Codigo);
                    if (indice >= 0) {
                        _Pontos[indice].coordenadas.PrimeiraEntrega = (_Pontos[indice].pessoa.CPFCNPJ == item.Codigo && !_Pontos[indice].coordenadas.PrimeiraEntrega);
                        var rotaPessoa = _Pontos[indice];
                        registrosGridReorderCarregamento.push(obterRegistroGridReorderCarregamento(rotaPessoa, i + 1));
                    }
                }
            }
            _gridReorderCarregamento.CarregarGrid(registrosGridReorderCarregamento);
        } else {
            exibirAlerta('Atenção', 'Somente pontos do tipo "Entrega" podem ser definidos como 1º Entrega.');
        }
    }
}

function carregarGridReorderCarregamento() {
    var isExibirPaginacao = false;
    var mostrarInfo = false;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var quantidadePorPagina = 9999999;

    var opcaoSetarPrimeiraEntrega = { descricao: "Definir 1º Entrega", id: guid(), metodo: definirPrimeiraEntregaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoSetarPrimeiraEntrega] };

    var header = [
        { data: "Ordem", title: Localization.Resources.Cargas.MontagemCargaMapa.Ordem, width: "5%", className: "text-align-center", orderable: false },
        { data: "Cliente", title: Localization.Resources.Cargas.MontagemCargaMapa.Cliente, width: "25%", className: "text-align-left", orderable: false },
        { data: "Cidade", title: Localization.Resources.Cargas.MontagemCargaMapa.Cidade, width: "25%", className: "text-align-left", orderable: false },
        { data: "DataAgendamento", title: Localization.Resources.Cargas.MontagemCargaMapa.DataAgendamento, width: "15%", className: "text-align-center", orderable: false },
        { data: "PrimeiraEntrega", title: "1º Entrega", width: "5%", className: "text-align-center", orderable: false },
        { data: "Codigo", visible: false }
    ];

    _gridReorderCarregamento = new BasicDataTable(_roteirizadorCarregamento.Map.idGrid, header, menuOpcoes, ordenacao, null, quantidadePorPagina, mostrarInfo, isExibirPaginacao, null, function (retornoOrdenacao) {

        var listaRegistros = _gridReorderCarregamento.BuscarRegistros();
        var listaRegistrosReordenada = [];
        var descontoPosicao = (listaRegistros[0].Ordem == 0) ? 1 : 0;

        for (var i = 0; i < retornoOrdenacao.listaRegistrosReordenada.length; i++) {
            var registroReordenado = retornoOrdenacao.listaRegistrosReordenada[i];

            for (var j = 0; j < listaRegistros.length; j++) {
                var registro = listaRegistros[j];

                if (registro.DT_RowId == registroReordenado.idLinha) {
                    registro.Ordem = registroReordenado.posicao - descontoPosicao;
                    listaRegistrosReordenada.push(registro);
                    break;
                }
            }
        }

        _gridReorderCarregamento.CarregarGrid(listaRegistrosReordenada);
        renderizarCarregamentoGoogleMapsClick();
        _centralizarMapa = true;
    });

    _gridReorderCarregamento.CarregarGrid([]);
}

function CarregarGridSemPontos() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "CPFCNPJ", title: Localization.Resources.Cargas.MontagemCargaMapa.CNPJCPF, width: "25%", className: "text-align-left", orderable: false },
        { data: "RazaoSocial", title: Localization.Resources.Cargas.MontagemCargaMapa.Pessoa, width: "50%", className: "text-align-left", orderable: true },
        { data: "Localidade", title: Localization.Resources.Cargas.MontagemCargaMapa.Localidade, width: "25%", className: "text-align-left", orderable: true }
    ];
    gridSemPontos = new BasicDataTable(_roteirizadorCarregamento.SemPontos.idGrid, header, null);
}

function carregarRoteiroCarregamento(codigoCarregamento, modal_roteirizar, callback) {

    //if (PEDIDOS_SELECIONADOS().length == 0)
    //    return exibirMensagem(tipoMensagem.aviso, "Roteirização", "Não há nenhum pedido selecionado para poder roteirizar o carregamento.");

    limparOrdemCarregamento();

    var data = { Carregamento: codigoCarregamento };// _carregamento.Carregamento.codEntity() };
    executarReST("MontagemCargaRoteirizacao/BuscarDadosRoteirizacao", data, function (arg) {
        if (arg.Success) {

            var retorno = arg.Data;

            _mapaRenderizado = false;

            LimparModalRoteirizacao();

            $("#Open_" + _roteirizadorCarregamento.Map.id + "_li").unbind();
            $("#Narativa_" + _roteirizadorCarregamento.Map.id + "_li").unbind();
            $("#Open_" + _roteirizadorCarregamento.Map.id + "_li").on("click", renderizarCarregamentoGoogleMapsClick);
            $("#Narativa_" + _roteirizadorCarregamento.Map.id + "_li").on("click", renderizarNarrativaCarregamento);

            carregarGridReorderCarregamento();

            _roteirizadorCarregamento.Carregamento.val(codigoCarregamento);//_carregamento.Carregamento.codEntity());

            criarMapaMontagemCarga();

            _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val(arg.Data.TipoUltimoPontoRoteirizacao);

            if (arg.Data.roteirizado) {
                _roteirizadorCarregamento.TipoRota.val(arg.Data.TipoRota);
                PreecherOrdemCarregamento(arg.Data.rotasInformacaoPessoa);
                _roteirizadorCarregamento.Distancia.val(Globalize.format(arg.Data.DistanciaKM, "n2") + " km");
                _roteirizadorCarregamento.Tempo.val(formatHora(arg.Data.TempoDeViagemEmMinutos));
                _roteirizadorCarregamento.PolilinhaRota.val(arg.Data.PolilinhaRota);
                _roteirizadorCarregamento.TempoDeViagemEmMinutos.val(arg.Data.TempoDeViagemEmMinutos);
                _roteirizadorCarregamento.PontosDaRota.val(arg.Data.PontosDaRota);
            }
            else {
                var pessoasNaoEncontradas = new Array();
                for (var i = 0; i < retorno.rotasInformacaoPessoa.length; i++) {
                    var rotaPessoa = retorno.rotasInformacaoPessoa[i];
                    if (rotaPessoa.coordenadas.tipoLocalizacao == EnumTipoLocalizacao.naoEncontrado) {
                        pessoasNaoEncontradas.push({ Codigo: rotaPessoa.pessoa.Codigo, CPFCNPJ: rotaPessoa.pessoa.CPFCNPJ, RazaoSocial: rotaPessoa.pessoa.RazaoSocial, Localidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF });
                    }
                }
                if (pessoasNaoEncontradas.length > 0) {
                    exibirGridPontosNaoEncontradosCarregamento(pessoasNaoEncontradas);
                    _roteirizadorCarregamento.SalvarRota.visible(false);
                    _roteirizadorCarregamento.Roteirizado.visible(false);
                }
            }

            var pontosUnicos = {};
            for (var i = 0; i < retorno.rotasInformacaoPessoa.length; i++) {
                var rotaPessoa = retorno.rotasInformacaoPessoa[i];
                //pontosUnicos[i] = rotaPessoa;
                //if ((rotaPessoa.pessoa.CPFCNPJ + "_" + rotaPessoa.coordenadas.latitude) in pontosUnicos) continue;
                //pontosUnicos[rotaPessoa.pessoa.CPFCNPJ + "_" + rotaPessoa.coordenadas.latitude] = rotaPessoa;
                var chave = rotaPessoa.pessoa.CPFCNPJ + "_" + rotaPessoa.coordenadas.CodigoOutroEndereco;
                if (chave in pontosUnicos) continue;
                pontosUnicos[chave] = rotaPessoa;
            }

            _roteirizadorCarregamento.PedidosDeColeta.val(arg.Data.PedidosDeColeta)
            _Pontos = [];
            for (var i in pontosUnicos) _Pontos.push(pontosUnicos[i]);

            if (modal_roteirizar) {
                Global.abrirModal("divModalRoteirizacao");
                $("#fdsRoteirizacao .nav li a").first().click();
                loadConfiguracaoColeta();
            }
            else {
                callback(true);
            }

        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function formatHora(minutos) {
    var hours = Math.floor(minutos / 60);
    var minutes = Math.floor(minutos % 60); //Math.floor((minutos - (hours * 60)) / 60);
    var seconds = 0;//minutos - (hours * 60) - (minutes * 60);
    var time = "";

    if (hours != 0) {
        time = hours + ":";
    }
    if (minutes != 0 || time !== "") {
        minutes = (minutes < 10 && time !== "") ? "0" + minutes : String(minutes);
        time += minutes + ":";
    }
    if (time === "") {
        time = seconds + "s";
    }
    else {
        time += (seconds < 10) ? "0" + seconds : String(seconds);
    }
    return time;
}

function LimparModalRoteirizacao() {
    LimparCampos(_roteirizadorCarregamento)
}

function salvarRotaCarregamento(callback) {
    var pessoasOrdenadas = new Array();
    for (var i = 0; i < _Pontos.length; i++) {
        if (_Pontos[i] == null) continue;
        pessoasOrdenadas.push({ CPFCNPJ: _Pontos[i].pessoa.CPFCNPJ, Coleta: _Pontos[i].coleta, CodigoOutroEndereco: _Pontos[i].CodigoOutroEndereco });
    }
    _roteirizadorCarregamento.Pessoas.val(JSON.stringify(pessoasOrdenadas));
    if (_roteirizadorCarregamento.TipoRota.val() == "") {
        _roteirizadorCarregamento.TipoRota.val('shortest');
    }
    Salvar(_roteirizadorCarregamento, "MontagemCargaRoteirizacao/SalvarRotaCarga", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.RoteirizacaoSalvaComSucesso);

                if (callback instanceof Function) {
                    callback();
                    return;
                } else {
                    Global.fecharModal('divModalRoteirizacao');
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function renderizarNarrativaCarregamento() {
    var reprocessar = ValidarSeMudouOrdemManualmente();
    renderizarGoogleMapsCarregamento(reprocessar);
}

function InicializarPontoOriginal() {
    _PontosOriginais = new Array();
    for (var i = 0; i < _Pontos.length; i++)
        _PontosOriginais.push(_Pontos[i]);
}

function reordenarPosicoesEntregasCarregamento() {
    var alterouOrdemEntregas = false;

    if (_PontosOriginais === null) {
        var alterouOrdemEntregas = true;
        if ((_roteirizadorCarregamento.PontosDaRota.val() !== null) && (_roteirizadorCarregamento.PontosDaRota.val() !== "")) {
            _mapa.roteirizarComPontosDaRota(_roteirizadorCarregamento.PontosDaRota.val(), function () { });
            alterouOrdemEntregas = false;
        }

        InicializarPontoOriginal();
    }
    var listaReorder = _gridReorderCarregamento.BuscarRegistros();
    var pessoasReordenadas = new Array();

    if (!_roteirizadorCarregamento.PedidosDeColeta.val())
        pessoasReordenadas.push(_PontosOriginais[0])


    $.each(listaReorder, function (i, ordem) {
        pessoasReordenadas.push(_PontosOriginais[ordem.DT_RowId]);
    });

    for (var i = 0; i < pessoasReordenadas.length; i++) {
        if (_Pontos[i] !== pessoasReordenadas[i]) {
            alterouOrdemEntregas = true;
            break;
        }
    }

    if (_roteirizadorCarregamento.PedidosDeColeta.val())
        pessoasReordenadas.push(_PontosOriginais[_PontosOriginais.length - 1])

    _Pontos = pessoasReordenadas;

    return alterouOrdemEntregas;
}

function PreecherOrdemCarregamento(retorno) {
    if (_roteirizadorCarregamento.PedidosDeColeta.val())
        PreecherOrdemColetaCarregamento(retorno)
    else
        PreecherOrdemEntregaCarregamento(retorno)
}

function PreecherOrdemEntregaCarregamento(retorno) {

    retorno = retorno.filter(function (el) {
        return el != null;
    });

    var registrosGridReorderCarregamento = [];
    var distancia = 0;

    if (retorno.length > 0) {
        coletas = new Array();
        coletas.push({ text: retorno[0].pessoa.RazaoSocial + " (" + retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF + ")", value: retorno[0].pessoa.Codigo });
        distancia = retorno[0].distancia;
    }

    for (var i = 1; i < retorno.length; i++) {
        var rotaPessoa = retorno[i];

        registrosGridReorderCarregamento.push(obterRegistroGridReorderCarregamento(rotaPessoa, i));

        distancia += rotaPessoa.distancia;

        if (rotaPessoa.coleta && !coletas.some(function (coleta) { return coleta.value == rotaPessoa.pessoa.Codigo; }))
            coletas.push({ text: rotaPessoa.pessoa.RazaoSocial + " (" + rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF + ")", value: rotaPessoa.pessoa.Codigo });
    }

    _roteirizadorCarregamento.Coletas.options(coletas);
    _roteirizadorCarregamento.Map.visible(true);
    _roteirizadorCarregamento.SemPontos.visible(false);
    _roteirizadorCarregamento.Distancia.val(Globalize.format(distancia, "n2") + " km");
    _roteirizadorCarregamento.SalvarRota.visible(true);
    _roteirizadorCarregamento.Roteirizado.visible(true);

    _gridReorderCarregamento.CarregarGrid(registrosGridReorderCarregamento);
}

function PreecherOrdemColetaCarregamento(retorno) {

    var id = retorno.length - 1;
    var distancia = 0;
    var registrosGridReorderCarregamento = [];

    if (id >= 0) {
        coletas = new Array();
        //_roteirizadorCarregamento.Origem.val(retorno[id].pessoa.RazaoSocial + " (" + retorno[id].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[id].pessoa.Endereco.Cidade.SiglaUF + ")");
        coletas.push({ text: retorno[id].pessoa.RazaoSocial + " (" + retorno[id].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[id].pessoa.Endereco.Cidade.SiglaUF + ")", value: retorno[id].pessoa.Codigo });
        distancia = retorno[id].distancia;
    }

    for (var i = 0; i < retorno.length - 1; i++) {
        var rotaPessoa = retorno[i];

        registrosGridReorderCarregamento.push(obterRegistroGridReorderCarregamento(rotaPessoa, i));

        distancia += rotaPessoa.distancia;
    }

    _roteirizadorCarregamento.Coletas.options(coletas);
    _roteirizadorCarregamento.Map.visible(true);
    _roteirizadorCarregamento.SemPontos.visible(false);
    _roteirizadorCarregamento.Distancia.val(Globalize.format(distancia, "n2") + " km");
    _roteirizadorCarregamento.SalvarRota.visible(true);
    _roteirizadorCarregamento.Roteirizado.visible(true);

    _gridReorderCarregamento.CarregarGrid(registrosGridReorderCarregamento);
}

function obterRegistroGridReorderCarregamento(rotaPessoa, i) {
    var corLinha = "";
    var restricoes = "";

    if ((rotaPessoa.coordenadas.RestricoesEntregas) && (rotaPessoa.coordenadas.RestricoesEntregas.length > 0)) {
        var listaRestricoes = rotaPessoa.coordenadas.RestricoesEntregas.map(function (r) { return r.Descricao });

        //corLinha = rotaPessoa.coordenadas.RestricoesEntregas[0].CorVisualizacao;
        corLinha = "#fcf8e3";
        restricoes = listaRestricoes.join("<br/>");
    }

    return {
        Codigo: rotaPessoa.pessoa.CPFCNPJ,// ? rotaPessoa.pessoa.Codigo,
        Ordem: i,
        Cliente: (rotaPessoa.pessoa.CodigoIntegracao ? rotaPessoa.pessoa.CodigoIntegracao + " - " : "") + rotaPessoa.pessoa.RazaoSocial,
        Cidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF,
        DataAgendamento: rotaPessoa.DataAgendamento,
        PrimeiraEntrega: (rotaPessoa.coordenadas.PrimeiraEntrega ? 'SIM' : (rotaPessoa.coordenadas.possuiPrimeiraEntrega ? 'SIM' : '')),
        DT_RowId: i,
        DT_RowColor: corLinha,
        DT_PopoverContent: restricoes,
        DT_PopoverTitle: Localization.Resources.Cargas.MontagemCargaMapa.RestricaoDeEntrega.getFieldDescription()
    };
}

function limparOrdemCarregamento() {
    _Pontos = new Array();
    _PontosOriginais = null;

    if (gridSemPontos)
        gridSemPontos.CarregarGrid([]);

    if (_gridReorderCarregamento != null)
        _gridReorderCarregamento.CarregarGrid([]);

    _roteirizadorCarregamento.Map.visible(false);
    _roteirizadorCarregamento.SalvarRota.visible(false);
    _roteirizadorCarregamento.SemPontos.visible(false);
    _roteirizadorCarregamento.Roteirizado.visible(false);

    LimparCampos(_roteirizadorCarregamento)
}

function obterIndicePontoCNPJ(CNPJ) {
    
    for (var i = 0; i < _Pontos.length; i++) {
        if (_Pontos[i]) {
            if (_Pontos[i].pessoa.Codigo === CNPJ) {
                return i;
            }
        }
    }
    
    return - 1
}

function MontagemCargaAlterouOrdemEntrega(resposta, pontoOrigem, pontoDestino) {
    var idOrigem = obterIndicePontoCNPJ(pontoOrigem.descricao)
    var idDestino = obterIndicePontoCNPJ(pontoDestino.descricao)

    if ((idOrigem >= 0) && (idDestino >= 0)) {

        var pontoorigem = _Pontos[idOrigem];
        var pontodestino = _Pontos[idDestino];

        _Pontos[idDestino] = pontoorigem;
        _Pontos[idOrigem] = pontodestino;

        PreecherOrdemCarregamento(_Pontos);
        InicializarPontoOriginal();

        setInfoRespostaRoteirizacao(resposta);
    }
}

function MontagemCargaAlterouRota(resposta) {
    setInfoRespostaRoteirizacao(resposta);
}

function criarMapaMontagemCarga() {
    if (_mapa === null) {
        _mapa = new Mapa(_roteirizadorCarregamento.Map.id, false);
        _mapa.setarCallbackAlteracaoEntrega(MontagemCargaAlterouOrdemEntrega);
        _mapa.setarCallbackAlteracaoRota(MontagemCargaAlterouRota);
    }
}

function obterPontoRota(ponto, sequencia) {

    codigo = ponto.pessoa.Codigo;
    var descricao = '';

    if (codigo != null) {
        codigo = codigo.replace(/\./g, '').replace('/', '').replace('-', '');
        descricao = codigo;
    } else {
        codigo = ponto.pessoa.Codigo;
        descricao = ponto.pessoa.NomeFantasia;
    }
    var tipoPonto = EnumTipoPontoPassagem.Entrega;

    if (ponto.coleta)
        tipoPonto = EnumTipoPontoPassagem.Coleta;
    else
        tipoPonto = ponto.TipoPonto;

    var pontoRota = new PontosRota(descricao, ponto.coordenadas.latitude, ponto.coordenadas.longitude, false, (tipoPonto == EnumTipoPontoPassagem.Passagem), 0, 0, codigo, sequencia, tipoPonto, ponto.pessoa.RazaoSocial + ' - ' + ponto.pessoa.CPFCNPJ);
    pontoRota.primeiraEntrega = ponto.coordenadas.PrimeiraEntrega;
    pontoRota.codigoOutroEndereco = ponto.coordenadas.CodigoOutroEndereco;

    return pontoRota;
}

function obterListaPontos(ignorarPrimeiraEntrega) {
    var listapontos = [];
    id = -1;
    for (var i = 0; i < _Pontos.length; i++) {

        id++;
        if (_Pontos[id] == null) continue;

        if ((ignorarPrimeiraEntrega) && (_Pontos[id].coordenadas.PrimeiraEntrega || _Pontos[id].coordenadas.possuiPrimeiraEntrega)) {
            continue;
        }

        var ponto = obterPontoRota(_Pontos[id], id);
        if (ponto) {
            listapontos.push(ponto);
        }
    }
    return listapontos;
}

function OrdenarPontosGrid(resposta) {
    // Se for responsa Google RoteriizarSemOrdem..
    var pontosrota = JSON.parse(resposta.pontosroteirizacao);

    var novaLista = new Array();
    var ultimo = pontosrota.length;
    //Comentado pois o roteirizarSemOrdem está retornando já sem o ponto de retorno.
    //if (_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante)
    //    ultimo = pontosrota.length - 1;

    for (var i = 0; i < ultimo; i++) {
        //if (pontosrota[i].tipoponto != EnumTipoPontoPassagem.Passagem)
        if (pontosrota[i].sequencia >= 0) {
            novaLista.push(_Pontos[pontosrota[i].sequencia]);
        } else {
            // Pontos de apoio.. 
            var ponto = {
                distancia: pontosrota[i].distancia,
                tempo: pontosrota[i].tempo,
                pessoa: {
                    Codigo: pontosrota[i].codigo,
                    NomeFantasia: pontosrota[i].informacao,
                    CPFCNPJ: null, //pontosrota[i].codigo,
                    RazaoSocial: pontosrota[i].descricao,
                    Endereco: {
                        Cidade: {
                            Descricao: Localization.Resources.Cargas.MontagemCargaMapa.PontoDeApoio,
                            SiglaUF: ''
                        }
                    }
                },
                coordenadas: {
                    RestricoesEntregas: [],
                    latitude: pontosrota[i].lat,
                    longitude: pontosrota[i].lng,
                    possuiPrimeiraEntrega: false,
                    tipoLocalizacao: 1,
                    PrimeiraEntrega: pontosrota[i].PrimeiraEntrega,
                    CodigoOutroEndereco: pontosrota[i].CodigoOutroEndereco
                },
                coleta: false,
                TipoPonto: EnumTipoPontoPassagem.Passagem
            };
            novaLista.push(ponto);
        }
    }

    _Pontos = novaLista;
}

function ObterPontosDeApoioCarregamento() {

    var pontos = _roteirizadorCarregamento.PontosDaRota.val();
    var novaLista = new Array();

    if (pontos != "" && pontos != undefined) {
        var pontosrota = JSON.parse(pontos);
        for (var i = 0; i < pontosrota.length; i++) {
            if (pontosrota[i].tipoponto == EnumTipoPontoPassagem.Passagem)
                novaLista.push(pontosrota[i]);
        }
    }

    return novaLista;
}

function setInfoRespostaRoteirizacao(resposta) {
    var distanciaKM = resposta.distancia / 1000;
    _roteirizadorCarregamento.PolilinhaRota.val(resposta.polilinha);
    _roteirizadorCarregamento.Distancia.val(Globalize.format(distanciaKM, "n2") + " km");
    _roteirizadorCarregamento.TempoDeViagemEmMinutos.val(Math.trunc(resposta.tempo / 60));
    _roteirizadorCarregamento.PontosDaRota.val(resposta.pontosroteirizacao);
}

//function drawMarkerPontosDeApoioRoteirizacao() {
//    var pontosDeApoio = ObterPontosDeApoioCarregamento();
//    _mapa.adicionarMarcador(pontosDeApoio, false, false, true);
//}

function gerarRoteirizacaoSemOrdem() {
    iniciarControleManualRequisicao();
    this._mapa.roteirizarSemOrdem(obterListaPontos(), _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val(), function (resposta) {
        if (resposta.status == "OK") {
            setInfoRespostaRoteirizacao(resposta);
        }
        finalizarControleManualRequisicao();
    });
}

function obterIdPrimeiraEntrega() {
    for (var i = 0; i < _Pontos.length; i++) {
        if (_Pontos[i] != null) {
            if (_Pontos[i].coordenadas.PrimeiraEntrega || _Pontos[i].coordenadas.possuiPrimeiraEntrega) {
                return i;
            }
        }
    }
    return -1;
}

function obterIdCodigo(codigo, callback) {
    for (var i = 0; i < _Pontos.length; i++) {
        if (_Pontos[i] != null) {
            if (_Pontos[i].pessoa.Codigo == codigo) {
                callback(i);
            }
        }
    }
    callback(-1);
}

function converterCoordenada(coordenada) {

    if (typeof (coordenada) == "string")
        return parseFloat(coordenada.replace(",", "."))

    return coordenada;

}

function ValidarCoordenadaFronteira() {

    if (_carregamentoTransporte.Fronteira.codEntity() == 0)
        return true;

    if (_carregamentoTransporte.Fronteira.Latitude == 0 || _carregamentoTransporte.Fronteira.Latitude == "" ||
        _carregamentoTransporte.Fronteira.Longitude == 0 || _carregamentoTransporte.Fronteira.Longitude == "") {

        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCargaMapa.Roteirizacao, Localization.Resources.Cargas.MontagemCargaMapa.FronteiraSemLatitudeBarraLongitude);

        return false;

    }

    return true;
}

function AdicionarFronteira(pontos) {

    if (_carregamentoTransporte.Fronteira.codEntity() == 0)
        return pontos;

    var listaPontos = [];


    var pontoFornteira = new PontosRota(_carregamentoTransporte.Fronteira.codEntity(),
        converterCoordenada(_carregamentoTransporte.Fronteira.Latitude),
        converterCoordenada(_carregamentoTransporte.Fronteira.Longitude),
        false, true, 0, 0, codigo, 1, EnumTipoPontoPassagem.Passagem,
        _carregamentoTransporte.Fronteira.val());

    var adicionouFronteria = false;

    for (i = 0; i < pontos.length; i++) {

        if ((pontos[i].tipoponto == EnumTipoPontoPassagem.Entrega) && (!adicionouFronteria)) {
            listaPontos.push(pontoFornteira);
            adicionouFronteria = true;
        }
        listaPontos.push(pontos[i]);
    }

    return listaPontos;

}

function gerarRoteirizacaoGoogleMapsOSM(modal_roteirizar, callback) {
    if (!ValidarCoordenadaFronteira())
        return;

    iniciarControleManualRequisicao();
    _PontosOriginais = null;
    this._mapa.limparMapa();

    idPrimeiraEntrega = obterIdPrimeiraEntrega();

    var possuiPrimeriaEntrega = ((idPrimeiraEntrega > 0) && (_Pontos.length > 2));

    if (possuiPrimeriaEntrega) {
        var primeiraEntrega = obterPontoRota(_Pontos[idPrimeiraEntrega], idPrimeiraEntrega);
        //primeiraEntrega.sequencia = idPrimeiraEntrega; 
        var origem = obterPontoRota(_Pontos[0], 0);
    }

    var listaPontos = obterListaPontos(possuiPrimeriaEntrega);

    //se possui primeira entrega troca origem com a primeira entrega
    if (possuiPrimeriaEntrega)
        listaPontos.unshift(primeiraEntrega); //         listaPontos[0] = primeiraEntrega;

    var manterPontoDestino = _roteirizadorCarregamento.PedidosDeColeta.val();

    var tipo = _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val(); //EnumTipoUltimoPontoRoteirizacao.AteOrigem;
    //if (modal_roteirizar)
    //    tipo = _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val();

    this._mapa.ordenarRotaOpenStreetMap(listaPontos, tipo, function (respostaOrdenada, status, respostaOSM) {

        if (status === "OK") {

            //Apos ordenação coloca a origem novamente para primeira entrega
            if (possuiPrimeriaEntrega) {
                origem.lat = parseFloat(origem.lat);
                origem.lng = parseFloat(origem.lng);
                respostaOrdenada.unshift(origem);
            }

            respostaOrdenada = AdicionarFronteira(respostaOrdenada);
            var retorno = Array.from(respostaOrdenada);

            var sucesso = false;
            if (respostaOSM != null) {

                if (respostaOSM.Status == "OK") {

                    sucesso = true;

                    var roteirizado = {
                        distancia: respostaOSM.Distancia * 1000,
                        tempo: respostaOSM.TempoMinutos * 60,
                        polilinha: respostaOSM.Polilinha,
                        status: google.maps.DirectionsStatus.OK,
                        pontos: [],
                        pontosroteirizacao: ""
                    }

                    var pontosroteirizacao = [];
                    for (var i = 0; i < respostaOrdenada.length; i++) {
                        pontosroteirizacao.push({
                            codigo: respostaOrdenada[i].codigo,
                            codigo_cliente: respostaOrdenada[i].codigoCliente,
                            utilizaLocalidade: respostaOrdenada[i].utilizaLocalidade,
                            usarOutroEndereco: respostaOrdenada[i].usarOutroEndereco,
                            descricao: respostaOrdenada[i].descricao,
                            distancia: respostaOrdenada[i].distancia,
                            informacao: respostaOrdenada[i].informacao,
                            lat: respostaOrdenada[i].lat,
                            lng: respostaOrdenada[i].lng,
                            pedagio: respostaOrdenada[i].pedagio,
                            sequencia: respostaOrdenada[i].sequencia,
                            tempo: respostaOrdenada[i].tempo,
                            tipoponto: respostaOrdenada[i].tipoponto,
                            primeiraEntrega: respostaOrdenada[i].PrimeiraEntrega,
                            codigoOutroEndereco: respostaOrdenada[i].codigoOutroEndereco
                        });
                    }

                    _responseOSM = true;
                    roteirizado.pontosroteirizacao = JSON.stringify(pontosroteirizacao);
                    OrdenarPontosGrid(roteirizado);
                    PreecherOrdemCarregamento(_Pontos);
                    setInfoRespostaRoteirizacao(roteirizado);
                    _responseOSM = false;
                }
            }

            if (!sucesso) {
                this._mapa.roteirizarSemOrdem(respostaOrdenada, tipo, function (resposta) {
                    if (resposta.status == "OK") {
                        //Aki, precisamos atualizar a _Pontos pois 
                        OrdenarPontosGrid(resposta);
                        PreecherOrdemCarregamento(_Pontos);
                        setInfoRespostaRoteirizacao(resposta);
                    }

                    if (resposta.status != "OK")
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCargaMapa.Roteirizacao, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelEncontrarUmarotaVerifiqueLatitudeLongitudeDaOrigemDestinatarios);
                });
            }

            if (modal_roteirizar) {
                finalizarControleManualRequisicao();
            } else {
                if (callback != undefined) {
                    callback(retorno);
                }
            }
        }
        else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, status);
            finalizarControleManualRequisicao();
        }
    }, manterPontoDestino, _roteirizadorCarregamento.Carregamento.val());
}

function changeColeta(e) {

    if (_Pontos.length > 0) {
        for (var i = 0; i < _Pontos.length; i++) {
            if (_Pontos[i] != null) {
                if (_Pontos[i].pessoa.Codigo == _roteirizadorCarregamento.Coletas.val()) {
                    var pontoSelecionado = _Pontos[i];
                    _Pontos.splice(i, 1);
                    _Pontos = [pontoSelecionado].concat(_Pontos);
                    break;
                }
            }
        }
        if (!_responseOSM)
            gerarRoteirizacaoGoogleMapsOSM(true);
    }
}

function RoteirizarCarregamentoClick(e) {
    gerarRoteirizacaoGoogleMapsOSM(true);
}

function exibirGridPontosNaoEncontradosCarregamento(pessoasNaoEncontradas) {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.PontosNaoEncontrados, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelRoteirizarCargaPoisAlgunsPontosNaoForamEncontradosVerifiqueListaAjusteManualmente);

    gridSemPontos.CarregarGrid(pessoasNaoEncontradas);

    _roteirizadorCarregamento.Map.visible(false);
    _roteirizadorCarregamento.SemPontos.visible(true);
}

function preencherRetornoRoteirizacao(responseArray, indexUltimoPonto, otimizar) {
    if (otimizar && _roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.Retornando) {
        renderizarGoogleMapsCarregamento(true);
    }
    else {
        var registrosGridReorderCarregamento = [];
        var pessoas = new Array();
        var total = 0;
        var distancia = 0;

        for (var j = 0; j < responseArray.length; j++) {
            pessoas.push(_Pontos[total]);

            if (total == 0 && otimizar) {
                for (var p = 1; p < _Pontos.length; p++) {
                    if (_Pontos[p].coordenadas.possuiPrimeiraEntrega) {
                        pessoas.push(_Pontos[p]);
                        total++;
                        if (indexUltimoPonto > 0)
                            indexUltimoPonto++;
                    }
                }
            }

            retorno = responseArray[j];

            for (var i = 0; i < retorno.routes[0].waypoint_order.length; i++) {
                var index = (retorno.routes[0].waypoint_order[i] + 1) + total;
                if (index >= indexUltimoPonto && indexUltimoPonto > 0)
                    index++;
                pessoas.push(_Pontos[index]);
            }

            for (var i = 0; i < retorno.routes[0].legs.length; i++) {
                distancia += retorno.routes[0].legs[i].distance.value;
            }

            total += retorno.routes[0].waypoint_order.length + 1;
        }

        if (indexUltimoPonto > 0)
            pessoas.push(_Pontos[indexUltimoPonto]);

        //_roteirizadorCarregamento.Origem.val(pessoas[0].pessoa.RazaoSocial + " (" + pessoas[0].pessoa.Endereco.Cidade.Descricao + ' - ' + pessoas[0].pessoa.Endereco.Cidade.SiglaUF + ")")
        for (var i = 1; i < pessoas.length; i++) {
            var rotaPessoa = pessoas[i];

            if ((_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem && (_roteirizadorCarregamento.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.Retornando || otimizar)) || i != total)
                registrosGridReorderCarregamento.push(obterRegistroGridReorderCarregamento(rotaPessoa, i));
        }

        _Pontos = pessoas;

        _roteirizadorCarregamento.Map.visible(true);
        _roteirizadorCarregamento.SemPontos.visible(false);
        _roteirizadorCarregamento.Distancia.val(Globalize.format(distancia / 1000, "n2") + " km");
        _roteirizadorCarregamento.SalvarRota.visible(true);
        _roteirizadorCarregamento.Roteirizado.visible(true);

        _gridReorderCarregamento.CarregarGrid(registrosGridReorderCarregamento);
    }
}
