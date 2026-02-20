/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="OrdenacaoPedidos.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _gridReorderEntregas = null;
var _reordenarEntrega = null;
var _Pontos = null;
var _PontosOriginais = null;

var _tiposRota = [
    { text: "Mais Rápida", value: "fastest" },
    { text: "Menor distância", value: "shortest" }
];

/*
 * Declaração das Classes
 */
var ReordenarEntregas = function () {
    this.Map = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), idGrid2: guid(), visibleReorder: ko.observable(false) });
    this.SemPontos = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.Carga = PropertyEntity();
    //this.Origem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: ko.observable("Origem : "), def: "", enable: false });
    this.Coletas = PropertyEntity({ val: ko.observable(""), options: ko.observable(new Array()), def: "", text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Coleta.getFieldDescription()), required: false, visible: ko.observable(true) });
    this.PedidosDeColeta = PropertyEntity({});

    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.AteOrigem), enable: ko.observable(true), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: _CONFIGURACAO_TMS.TipoUltimoPontoRoteirizacao, text: "Ultimo Ponto", required: true });
    this.EntregasReordenadas = PropertyEntity({ val: ko.observable(new Array()), type: types.map, getType: typesKnockout.dynamic });

    //this.Roteirizar = PropertyEntity({ eventClick: RoteirizarCarregamentoClick, type: types.event, text: "Buscar rota", visible: ko.observable(false) });
    this.SalvarRota = PropertyEntity({ eventClick: salvarRotaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(false) });
    this.AtualizarRota = PropertyEntity({ eventClick: function () { }, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
}


/*
 * Declaração das Funções de Inicialização
 */
function loadReordenarEntrega() {
    _reordenarEntrega = new ReordenarEntregas();
    KoBindings(_reordenarEntrega, "knockouReordenarEntrega");
}


/*
 * Declaração das Funções Associadas a Eventos
 */
function reordenarEntregasClick() {
    LimparCampos(_reordenarEntrega);

    var carga = _etapaAtualFluxo.Carga.val();
    _reordenarEntrega.Carga.val(carga);

    exibirModalReordenarEntrega();
}


function exibirModalReordenarEntrega() {

    var data = { CargaControleEntrega: _etapaAtualFluxo.Carga.val() };
    executarReST("ControleEntregaEntrega/BuscarDadosRoteirizacaoReodenarEntregas", data, function (arg) {
        if (arg.Success) {
            carregarGridReorder();
            //criarMapaMontagemCarga();

            _reordenarEntrega.TipoUltimoPontoRoteirizacao.val(arg.Data.TipoUltimoPontoRoteirizacao);
            PreecherOrdemEntregas(arg.Data.rotasEntregas);

            Global.abrirModal("divModalReordenarEntrega");
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function salvarRotaClick() {
    SalvarRota();
}


//*******METODOS*******

function PreecherOrdemEntregas(retorno) {

    retorno = retorno.filter(function (el) {
        return el != null;
    });

    var registrosGridReorderCarregamento = [];
    var coletas = new Array();

    if (retorno.length > 0) {
        var coletas = new Array();

        if (retorno[0].coleta)
            coletas.push({ text: retorno[0].pessoa.RazaoSocial + " (" + retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF + ")", value: retorno[0].pessoa.Codigo });
        else
            registrosGridReorderCarregamento.push(obterRegistroGridReorder(retorno[0], 0));
    }

    for (var i = 1; i < retorno.length; i++) {
        var rotaPessoa = retorno[i];

        registrosGridReorderCarregamento.push(obterRegistroGridReorder(rotaPessoa, i));

        if (rotaPessoa.coleta)
            coletas.push({ text: rotaPessoa.pessoa.RazaoSocial + " (" + rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF + ")", value: rotaPessoa.pessoa.Codigo });
    }

    _reordenarEntrega.Coletas.options(coletas);
    _reordenarEntrega.Map.visible(true);
    _reordenarEntrega.SemPontos.visible(false);
    _reordenarEntrega.SalvarRota.visible(true);

    _gridReorderEntregas.CarregarGrid(registrosGridReorderCarregamento);
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

function carregarGridReorder() {
    var isExibirPaginacao = false;
    var mostrarInfo = false;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var quantidadePorPagina = 9999999;

    var header = [
        { data: "Ordem", title: "Ordem", width: "10%", className: "text-align-center", orderable: false },
        { data: "CodigoEntrega", title: "Entrega", width: "10%", className: "text-align-center", orderable: false },
        { data: "Cliente", title: "Cliente", width: "40%", className: "text-align-left", orderable: false },
        { data: "Cidade", title: "Cidade", width: "40%", className: "text-align-left", orderable: false }
    ];

    _gridReorderEntregas = new BasicDataTable(_reordenarEntrega.Map.idGrid, header, null, ordenacao, null, quantidadePorPagina, mostrarInfo, isExibirPaginacao, null, function (retornoOrdenacao) {
        var listaRegistros = _gridReorderEntregas.BuscarRegistros();
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

        _gridReorderEntregas.CarregarGrid(listaRegistrosReordenada);
        // renderizarCarregamentoGoogleMapsClick();
        // _centralizarMapa = true;
    });

    _gridReorderEntregas.CarregarGrid([]);
}


function obterRegistroGridReorder(rotaPessoa, i) {
    var corLinha = "";

    if (rotaPessoa.Finalizada || ((rotaPessoa.coordenadas.RestricoesEntregas) && (rotaPessoa.coordenadas.RestricoesEntregas.length > 0))) {

        if (rotaPessoa.Finalizada) {
            corLinha = "#f4f4f4";
        } else {
            corLinha = "#fcf8e3";
        }
    }

    return {
        Ordem: i,
        Cliente: (rotaPessoa.pessoa.CodigoIntegracao ? rotaPessoa.pessoa.CodigoIntegracao + " - " : "") + rotaPessoa.pessoa.RazaoSocial,
        Cidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF,
        CodigoEntrega: rotaPessoa.CodigoEntrega,
        DT_RowId: i,
        DT_RowColor: corLinha
    };
}


function SalvarRota() {

    var listaReorder = _gridReorderEntregas.BuscarRegistros();

    var entregasReordenadas = new Array();

    for (var i = 0; i < listaReorder.length; i++) {
        if (listaReorder[i] == null) continue;

        entregasReordenadas.push({ Cliente: listaReorder[i].Cliente, Ordem: listaReorder[i].Ordem, CodigoEntrega: listaReorder[i].CodigoEntrega });
    }

    _reordenarEntrega.EntregasReordenadas.val(JSON.stringify(entregasReordenadas));

    Salvar(_reordenarEntrega, "ControleEntregaEntrega/SalvarReordenacaoEntrega", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.OrdenacaoSalvaComSucesso);
                Global.fecharModal('divModalReordenarEntrega');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
