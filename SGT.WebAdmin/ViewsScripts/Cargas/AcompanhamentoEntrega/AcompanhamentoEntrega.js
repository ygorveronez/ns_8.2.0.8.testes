/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

/// <reference path="../../Enumeradores/EnumStatusViagemControleEntrega.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaAcompanhamentoEntrega;
var _containerControleEntregaResumo;
var _containerControleEntrega;
var _itensPorPagina = 50;
var isMobile = false;
var _executarPesquisa = false;
var _etapaAtualFluxo = null;
var _controleEntregaVisaoPrevisao = false;
var _descricaoSituacaoResumo = "";
var _tamanhoArrayEntregas = false;
var urlAcompanhamentoEntrega = "cargas/acompanhamentoentrega";
var timeoutControleEntrega = 300000;

var PesquisaAcompanhamentoEntrega = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigocargaEmbarcador = PropertyEntity({ text: ko.observable("Número Carga:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: ko.observable("Numero Pedido:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veiculos:", idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "Placa", propCodigo: "Codigo" } });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusViagemControleEntrega = PropertyEntity({ text: "Viagem: ", val: ko.observable(EnumStatusViagemControleEntrega.Todas), options: EnumStatusViagemControleEntrega.obterOpcoesPesquisa(), def: EnumStatusViagemControleEntrega.Todas });
    this.DescSituacaoEntrega = PropertyEntity({ val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            $("#controle-entrega-conteudo-resumo").show();
            $("#controle-entrega-conteudo-container").show();
            _descricaoSituacaoResumo = "";
            obterAcompanhametoEntregas(1, false, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

function obterAcompanhamentoEntregasResumo(dataPesquisa) {
    executarReST("AcompanhamentoEntrega/ObterResumos", dataPesquisa, function (arg) {
        if (arg.Success) {

            if (arg.Data !== false) {
                _containerControleEntregaResumo = new ContainerControleEntregaResumo(arg.Data);
                KoBindings(_containerControleEntregaResumo, "knoutContainerAcompanhamentoEntregaResumo");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function obterAcompanhametoEntregas(page, eventoPorPaginacao, atualizacaoAutomatica) {
    if (_pesquisaAcompanhamentoEntrega.DescSituacaoEntrega.val() == _descricaoSituacaoResumo) {
        console.log("segunda vez");
        _pesquisaAcompanhamentoEntrega.DescSituacaoEntrega.val("");
        _descricaoSituacaoResumo = "";
    } else {
        _pesquisaAcompanhamentoEntrega.DescSituacaoEntrega.val(_descricaoSituacaoResumo);
    }

    if (atualizacaoAutomatica == undefined)
        atualizacaoAutomatica = false;

    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoEntrega);
    data.FiltroPesquisa = RetornarJsonFiltroPesquisa(_pesquisaAcompanhamentoEntrega);
    data.inicio = _itensPorPagina * (page - 1);
    data.limite = _itensPorPagina;
    _containerControleEntrega.Cargas.removeAll();

    executarReST("AcompanhamentoEntrega/ObterAcompanhamentoEntrega", data, function (arg) {
        if (arg.Success) {
            _KnoutsEntregas = new Array();
            if (arg.Data !== false) {

                $('[rel=popover-hover]').popover('destroy');
                _pesquisaAcompanhamentoEntrega.ExibirFiltros.visibleFade(false);

                $.each(arg.Data, function (i, carga) {
                    AdicionarCarga(carga);
                });

                obterAcompanhamentoEntregasResumo(data);

                if (!eventoPorPaginacao)
                    componentePaginacao(arg.QuantidadeRegistros);

                setTimeout(function () {
                    $("[rel=popover-hover]").popover({ trigger: "hover", container: "body", delay: { "show": 1000, "hide": 0 } });
                }, 1000);
            } else {
                if (!atualizacaoAutomatica)
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            if (!atualizacaoAutomatica)
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }

    }, function () { }, !atualizacaoAutomatica);
}


var ContainerControleEntrega = function () {
    this.Cargas = ko.observableArray([]);
};


var ContainerControleEntregaResumo = function (data) {
    this.ColetasEmTempo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ColetasAtraso1 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ColetasAtraso2 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ColetasAtraso3 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.EmtransitoOK = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.EmtransitoAtraso1 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.EmtransitoAtraso2 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.EmtransitoAtraso3 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DestinoEmTempo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DestinoAtraso1 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DestinoAtraso2 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DestinoAtraso3 = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.BuscarDadosEntregaPeloResumo = function (tipoResumo) {
        _descricaoSituacaoResumo = tipoResumo;
        obterAcompanhametoEntregas(1, false, false);
    }

    PreencherObjetoKnout(this, { Data: data });

};
function AdicionarCarga(carga) {
    var ko_containerCarga = new Carga(carga);
    _containerControleEntrega.Cargas.push(ko_containerCarga);
}

function componentePaginacao(totalRegistros) {
    if (totalRegistros > 0) {

        var resumo = 'Exibindo _START_ até _END_ de _TOTAL_ registros';
        resumo = resumo
            .replace("_START_", Globalize.format(1, "n0"))
            .replace("_END_", Globalize.format(totalRegistros, "n0"))
            .replace("_TOTAL_", Globalize.format(totalRegistros, "n0"));

        var $resumo = $('<ul style="float:left" class="dataTables_info">' + resumo + '</ul > ');
        var $ul = $('<ul style="float:right" class="pagination"></ul>');

        var paginas = Math.ceil(totalRegistros / _itensPorPagina);

        $("#paginacao-controle-entrega").addClass('dataTables_info');
        $("#paginacao-controle-entrega").empty().append($resumo, $ul);

        _executarPesquisa = false;

        $ul.twbsPagination({
            first: 'Primeiro',
            prev: 'Anterior',
            next: 'Próximo',
            last: 'Último',
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: null,
            onPageClick: function (event, page) {
                if (_executarPesquisa) {
                    obterAcompanhametoEntregas(page, true, false);
                }
                _executarPesquisa = true;
            }
        });

    } else {
        $("#paginacao-controle-entrega").html('<span>Nenhum Registro Encontrado</span>');
    }
}


// ********* EVENTOS ******** 

function loadAcompanhamentoEntrega() {
    isMobile = $(window).width() <= 980;

    buscarDetalhesOperador(function () {
        _containerControleEntrega = new ContainerControleEntrega();
        KoBindings(_containerControleEntrega, "knoutContainerAcompanhamentoEntrega");

        _pesquisaAcompanhamentoEntrega = new PesquisaAcompanhamentoEntrega();
        KoBindings(_pesquisaAcompanhamentoEntrega, "knoutPesquisaAcompanhamentoEntrega");

        loadFiltroPesquisa();

        new BuscarTiposOperacao(_pesquisaAcompanhamentoEntrega.TipoOperacao);
        new BuscarClientes(_pesquisaAcompanhamentoEntrega.Destinatario);
        new BuscarClientes(_pesquisaAcompanhamentoEntrega.Remetente);
        new BuscarFilial(_pesquisaAcompanhamentoEntrega.Filial);
        new BuscarVeiculos(_pesquisaAcompanhamentoEntrega.Veiculos);

        setTimeout(atualizacaoControleAcompanhamentoEntregaAutomatica, timeoutControleEntrega);
    });
}

function loadFiltroPesquisa() {
    var data = { TipoFiltro: 3 };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data))
            PreencherJsonFiltroPesquisa(_pesquisaAcompanhamentoEntrega, arg.Data.Dados);
    });
}

var Carga = function (data) {

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int });
    this.CargaCancelada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Backgroundcolor = PropertyEntity({});
    this.DataInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataPosicao = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Tooltip = PropertyEntity({ val: ko.observable("") });
    this.PermiteAdicionarColeta = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermiteAdicionarReentrega = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteDownloadBoletimViagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.InformacoesComplementares = PropertyEntity({ def: [], val: ko.observableArray([]) });
    this.Entregas = ko.observableArray();
    this.UtimaEntrega = PropertyEntity({ val: ko.observable(false) });
    this.EstadoCidadeUtimaEntrega = PropertyEntity({ val: ko.observable(false) });
    this.Monitoramento = PropertyEntity({ visible: ko.observable(true) });
    this.ProximoDestino = PropertyEntity({ val: ko.observable("") });
    this.InformacoesCarga = PropertyEntity({ visible: ko.observable(true) });
    this.LeftCaminhao = PropertyEntity({ val: ko.observable("2%") });

    PreencherObjetoKnout(this, { Data: data });
    _KnoutsEntregas.push(this);
    preencherEntregas(this, data);
}

function preencherEntregas(carga, dados) {
    var codigoCarga = dados.Carga;
    carga.ProximoDestino.val("");
    _tamanhoArrayEntregas = dados.Entregas.length;

    dados.Entregas.forEach(function (entrega, i) {
        objetoEntrega = MontaEntrega(carga, codigoCarga, entrega, i + 1)
        carga.Entregas.push(objetoEntrega);
    });
}


function MontaEntrega(carga, codigoCarga, entrega, index) {

    ObjetoEtapa = new Entrega(carga, codigoCarga, entrega, index)
    return ObjetoEtapa;
}


function atualizacaoControleAcompanhamentoEntregaAutomatica() {

    if ((document.URL) && (document.URL.toLowerCase().includes(urlAcompanhamentoEntrega))) {

        obterAcompanhametoEntregas(1, false, true).then(function () { setTimeout(atualizacaoControleAcompanhamentoEntregaAutomatica, timeoutControleEntrega) });
    }
}

var Entrega = function (carga, codigoCarga, data, index) {

    this.CodigoCarga = codigoCarga;
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Cliente = PropertyEntity({ visible: ko.observable(false) });
    this.CodigoCliente = PropertyEntity({ visible: ko.observable(false) });
    this.Coleta = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DataChegada = PropertyEntity({ visible: ko.observable(false) });
    this.DataEntrega = PropertyEntity({ visible: ko.observable(false) });
    this.DataPrevista = PropertyEntity({ visible: ko.observable(false) });
    this.DataRealizada = PropertyEntity({ visible: ko.observable(false) });
    this.DataReprogramada = PropertyEntity({ visible: ko.observable(false) });
    this.Descricao = PropertyEntity({ visible: ko.observable(false) });
    this.Tooltip = PropertyEntity({ val: ko.observable("") });
    this.Distancia = PropertyEntity({ val: ko.observable("") });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(true) });
    this.EstadoCidadeCliente = PropertyEntity({ val: ko.observable(false) });

    this.ColetaClass = PropertyEntity({ val: ko.observable(true) });
    this.EntregaClass = PropertyEntity({ val: ko.observable(true) });
    this.LinhaClass = PropertyEntity({ val: ko.observable(true) });

    this.ArrayEntrega = PropertyEntity({ val: ko.observable(false) });

    //calculo para posicao da linha e bolinhas (entregas)
    if (index > 2) {
        this.LeftLinha = PropertyEntity({ val: ko.observable((((10 * index) * 2) - 30) + "%") });
        this.LeftEntrega = PropertyEntity({ val: ko.observable((((10 * index) * 2) - 20) + "%") });
    } else {
        this.LeftLinha = PropertyEntity({ val: ko.observable(((10 * index) - 10) + "%") });
        this.LeftEntrega = PropertyEntity({ val: ko.observable((10 * index) + "%") });
    }

    PreencherObjetoKnout(this, { Data: data });
    if (data.Situacao === 0 && carga.ProximoDestino.val() === "") {
        if (index > 2) {
            carga.LeftCaminhao.val(((((10 * index) * 2) - 30) + "%"));
        } else {
            carga.LeftCaminhao.val((((10 * index) - 8) + "%"));
        }

        carga.ProximoDestino.val("Proximo Destino: " + this.CodigoCliente.val());
    }

    if (data.Situacao === 2 && carga.ProximoDestino.val() === "") {
        carga.LeftCaminhao.val((((10 * index) - 2) + "%"));
    }


    if (data.SituacaoEntrega === 0) {
        this.LinhaClass.val("linhaNaoEntregue");
        this.ColetaClass.val("btn btn-default");
        this.EntregaClass.val("btn btn-default rounded-circle");
    } else if (data.SituacaoEntrega === 1) {
        this.LinhaClass.val("linhaAtraso1");
        this.ColetaClass.val("btn btn-warning");
        this.EntregaClass.val("btn btn-warning rounded-circle");
    } else if (data.SituacaoEntrega === 2) {
        this.LinhaClass.val("linhaAtraso2");
        this.ColetaClass.val("btn btn-danger");
        this.EntregaClass.val("btn btn-danger rounded-circle");
    } else if (data.SituacaoEntrega === 3) {
        this.LinhaClass.val("linhaAtraso3");
        this.ColetaClass.val("btn btn-info");
        this.EntregaClass.val("btn btn-info rounded-circle");
    } else {
        this.LinhaClass.val("linhaEntregue");
        this.ColetaClass.val("btn btn-success");
        this.EntregaClass.val("btn btn-success rounded-circle");
    }

    if (_tamanhoArrayEntregas == 1) {
        //tem apenas um registro;
        if (data.Coleta == false) {
            //tem apenas uma e nao é coleta..
            carga.LeftCaminhao.val("-8%");
            this.ArrayEntrega.val(true);
        }
    }



    //data.SituacaoEntrega
    //naoEntregue = 0,
    //atraso1 = 1,
    //atraso2 = 2,
    //atraso3 = 3,
    //emtempo = 4


}