/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoSinistro.js" />
/// <reference path="../../Enumeradores/EnumEtapaSinistro.js" />
/// <reference path="SinistroEtapaAcompanhamento.js" />
/// <reference path="SinistroEtapaAcompanhamentoHistorico.js" />
/// <reference path="SinistroEtapaAcompanhamentoHistoricoAnexo.js" />
/// <reference path="SinistroEtapaDados.js" />
/// <reference path="SinistroEtapaDocumentacao.js" />
/// <reference path="SinistroEtapaDocumentacaoAnexo.js" />
/// <reference path="SinistroEtapaIndicacaoPagador.js" />
/// <reference path="SinistroEtapaIndicacaoPagadorAnexo.js" />
/// <reference path="SinistroEtapaManutencao.js" />
/// <reference path="SinistroEtapas.js" />

var _pesquisaSinistro;
var _gridSinistro;

var PesquisaSinistro = function () {
    this.DataSinistroInicial = PropertyEntity({ text: "Data Sinistro Inicial: ", getType: typesKnockout.date });
    this.DataSinistroFinal = PropertyEntity({ text: "Data Sinistro Final: ", getType: typesKnockout.date });
    this.DataSinistroInicial.dateRangeLimit = this.DataSinistroFinal;
    this.DataSinistroFinal.dateRangeInit = this.DataSinistroInicial;

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.NumeroBoletimOcorrencia = PropertyEntity({ text: "Número do B.O.:" });

    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoEtapaFluxoSinistro.Todos), options: EnumSituacaoEtapaFluxoSinistro.obterOpcoesPesquisa(), def: EnumSituacaoEtapaFluxoSinistro.Todos });
    this.Etapa = PropertyEntity({ text: "Etapa Atual:", val: ko.observable(EnumEtapaSinistro.Todos), options: EnumEtapaSinistro.obterOpcoesPesquisa(), def: EnumEtapaSinistro.Todos });

    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo do Sinistro:", idBtnSearch: guid() });
    this.VeiculoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.TipoSinistro = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Tipo Sinistro:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.GravidadeSinistro = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Gravidade Sinistro:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSinistro.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

function loadSinistro() {
    _pesquisaSinistro = new PesquisaSinistro();
    KoBindings(_pesquisaSinistro, "knockoutPesquisaFluxoSinistro", false, _pesquisaSinistro.Pesquisar.id);

    new BuscarLocalidades(_pesquisaSinistro.Cidade);
    new BuscarVeiculos(_pesquisaSinistro.Veiculo);
    new BuscarReboques(_pesquisaSinistro.VeiculoReboque);
    new BuscarMotoristas(_pesquisaSinistro.Motorista);
    new BuscarTipoSinistro(_pesquisaSinistro.TipoSinistro);
    new BuscarGravidadeSinistro(_pesquisaSinistro.GravidadeSinistro);


    loadGridSinistro();

    loadEtapasFluxoSinistro();

    loadEtapaDadosSinistro();
    loadEtapaDocumentacaoSinistro();
    loadEtapaManutencaoSinistro();
    loadEtapaIndicacaoPagadorSinistro();
    loadEtapaAcompanhamentoSinistro();

    ajustarEtapas(EnumEtapaSinistro.Dados, EnumSituacaoEtapaFluxoSinistro.Aberto);
}

function loadGridSinistro() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSinistroClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSinistro = new GridView(_pesquisaSinistro.Pesquisar.idGrid, "Sinistro/Pesquisa", _pesquisaSinistro, menuOpcoes, { column: 0, dir: orderDir.desc });
    _gridSinistro.CarregarGrid();
}

function recarregarGridSinistro() {
    _gridSinistro.CarregarGrid();
}

function limparFluxoSinistro(editando) {
    limparCamposSinistroEtapaDados();
    limparCamposSinistroEtapaDocumentacao();
    limparCamposSinistroEtapaManutencao();
    limparCamposSinistroEtapaIndicacaoPagador();
    limparCamposSinistroEtapaAcompanhamento();

    if (!editando)
        ajustarEtapas(EnumEtapaSinistro.Dados, EnumSituacaoEtapaFluxoSinistro.Aberto);

    Global.ResetarAbas();    
}

function editarSinistroClick(registroSelecionado) {
    limparFluxoSinistro(true);
    CarregarDadosSinistro(registroSelecionado.Codigo);
}

function CarregarDadosSinistro(codigo) {
    if (codigo > 0) {
        executarReST("Sinistro/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _pesquisaSinistro.ExibirFiltros.visibleFade(false);

                    var data = retorno.Data;

                    preencherEtapaDadosSinistro(data.DadosSinistro);
                    preencherEtapaDocumentacao(data.Documentacao);
                    preencherEtapaManutencao(data.Manutencao);
                    preencherEtapaIndicacaoPagador(data.IndicacaoPagador);
                    preencherEtapaAcompanhamento(data.Acompanhamento);
                    ajustarEtapas(data.Etapa, data.Situacao);                    
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}