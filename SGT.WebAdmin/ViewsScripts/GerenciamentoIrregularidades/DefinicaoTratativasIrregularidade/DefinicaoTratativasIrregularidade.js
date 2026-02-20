/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />
/// <reference path="../../Consultas/Irregularidade.js" />
/// <reference path="../../Consultas/GrupoTipoOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDDefinicaoTratativasIrregularidade;
var _pesquisaDefinicaoTratativasIrregularidade;
var _gridDefinicaoTratativasIrregularidade;
var _gridTratativasIrregularidade;
var _definicaoTratativasIrregularidade;

/*
 * Declaração das Classes
 */

var PesquisaDefinicaoTratativaIrregularidade = function () {

    this.PortfolioModuloControle = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portfólio Módulo Controle:", idBtnSearch: guid(), required: false });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid(), required: false });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusFemPesquisa, val: ko.observable(0), text: "Situação:" });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridDefinicaoTratativaIrregularidade, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var DefinicaoTratativaIrregularidade = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PortfolioModuloControle = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Portfólio Módulo Controle:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Irregularidade:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: true, options: _statusFem, val: ko.observable(true), text: "*Situação:", required: ko.observable(true) });

    this.TratativasIrregularidade = PropertyEntity({ type: types.map, required: true, getType: typesKnockout.dynamic, idGrid: guid(), visible: ko.observable(true) });

    this.AdicionarTratativa = PropertyEntity({ eventClick: PreencherNovaTratativaClick, type: types.event, text: "Adicionar Tratativa", visible: ko.observable(true), enable: ko.observable(true) });

    this.TratativasIrregularidade.val.subscribe(function () {
        RenderizarGridTratativasIrregularidade();
        Global.fecharModal('divAdicionarTratativa');
    });
}

var CRUDDefinicaoTratativaIrregularidade = function () {

    this.Salvar = PropertyEntity({ type: types.event, eventClick: SalvarClick, text: "Salvar", visible: ko.observable(false), enable: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridDefinicaoTratativasIrregularidade() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { EditarDefinicaoTratativaIrregularidadeClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridDefinicaoTratativasIrregularidade = new GridView(_pesquisaDefinicaoTratativasIrregularidade.Pesquisar.idGrid, "DefinicaoTratativasIrregularidade/Pesquisa", _pesquisaDefinicaoTratativasIrregularidade, menuOpcoes);
    _gridDefinicaoTratativasIrregularidade.CarregarGrid();
}

function LoadGridTratativasIrregularidade() {

    // Menu
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { EditarTratativaClick(item); }, tamanho: "5", icone: "" };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (item) { ExcluirTratativaClick(item); }, tamanho: "5", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar, opcaoExcluir] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Sequencia", title: "Sequência", width: "20%", className: "text-align-center" },
        { data: "Setor", title: "Setor", width: "25%", className: "text-align-center" },
        { data: "ProximaSequencia", title: "Próxima Sequência", width: "20%", className: "text-align-center" },
        { data: "GrupoOperacao", title: "Grupo de Operação", width: "25%", className: "text-align-center" },
    ];

    // Grid
    _gridTratativasIrregularidade = new BasicDataTable(_definicaoTratativasIrregularidade.TratativasIrregularidade.idGrid, header, menuOpcoes, null, null, 10);
    _definicaoTratativasIrregularidade.TratativasIrregularidade.basicTable = _gridTratativasIrregularidade;
    _gridTratativasIrregularidade.CarregarGrid([]);
}

function LoadDefinicaoTratativaIrregularidade() {

    _definicaoTratativasIrregularidade = new DefinicaoTratativaIrregularidade();
    KoBindings(_definicaoTratativasIrregularidade, "knockoutDefinicaoTratativaIrregularidade");

    _pesquisaDefinicaoTratativasIrregularidade = new PesquisaDefinicaoTratativaIrregularidade();
    KoBindings(_pesquisaDefinicaoTratativasIrregularidade, "knockoutPesquisaDefinicaoTratativaIrregularidade", false, _pesquisaDefinicaoTratativasIrregularidade.Pesquisar.id);

    _CRUDDefinicaoTratativasIrregularidade = new CRUDDefinicaoTratativaIrregularidade();
    KoBindings(_CRUDDefinicaoTratativasIrregularidade, "knockoutCRUDDefinicaoTratativaIrregularidade");

    HeaderAuditoria("DefinicaoTratativasIrregularidade", _definicaoTratativasIrregularidade);

    LoadGridDefinicaoTratativasIrregularidade();
    LoadGridTratativasIrregularidade();
    LoadTratativaIrregularidade();

    BuscarPortfolioModuloControle(_pesquisaDefinicaoTratativasIrregularidade.PortfolioModuloControle);
    BuscarIrregularidades(_pesquisaDefinicaoTratativasIrregularidade.Irregularidade);
    BuscarPortfolioModuloControle(_definicaoTratativasIrregularidade.PortfolioModuloControle);
    BuscarIrregularidades(_definicaoTratativasIrregularidade.Irregularidade,retornorPesquisaIrregularidade);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function retornorPesquisaIrregularidade(data) {
    _definicaoTratativasIrregularidade.Irregularidade.codEntity(data.Codigo)
    _definicaoTratativasIrregularidade.Irregularidade.val(data.Descricao)
    _definicaoTratativasIrregularidade.Irregularidade.Gatilho = data.Gatilho
}

function SalvarClick(e, sender) {

    if (!ValidarDefinicao())
        return;

    if (!ValidarCamposObrigatorios(_definicaoTratativasIrregularidade)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios")
        return;
    }

    executarReST("DefinicaoTratativasIrregularidade/Atualizar", PreencherObjetoParaBack(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");

                LimparTudo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AdicionarClick(e, sender) {

    if (!ValidarDefinicao())
        return;

    if (!ValidarCamposObrigatorios(_definicaoTratativasIrregularidade)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios")
        return;
    }

    executarReST("DefinicaoTratativasIrregularidade/Adicionar", PreencherObjetoParaBack(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");

                LimparTudo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function CancelarClick(e) {
    LimparTudo();
}

function EditarDefinicaoTratativaIrregularidadeClick(registro) {

    LimparTudo();

    _definicaoTratativasIrregularidade.Codigo.val(registro.Codigo);

    BuscarPorCodigo(_definicaoTratativasIrregularidade, "DefinicaoTratativasIrregularidade/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                var dados = retorno.Data;

                _gridTratativasIrregularidade.CarregarGrid(dados.TratativasIrregularidade);

                _pesquisaDefinicaoTratativasIrregularidade.ExibirFiltros.visibleFade(false);

                _CRUDDefinicaoTratativasIrregularidade.Adicionar.visible(false);
                _CRUDDefinicaoTratativasIrregularidade.Adicionar.enable(false);
                _CRUDDefinicaoTratativasIrregularidade.Cancelar.visible(true);
                _CRUDDefinicaoTratativasIrregularidade.Cancelar.enable(true);
                _CRUDDefinicaoTratativasIrregularidade.Salvar.visible(true);
                _CRUDDefinicaoTratativasIrregularidade.Salvar.enable(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function LimparTudo() {
    LimparCampos(_definicaoTratativasIrregularidade);
    LimparCamposTratativaIrregularidade();

    recarregarGridDefinicaoTratativaIrregularidade();
    recarregarGridTratativasIrregularidade();

    _CRUDDefinicaoTratativasIrregularidade.Salvar.visible(false);
    _CRUDDefinicaoTratativasIrregularidade.Salvar.enable(false);
    _CRUDDefinicaoTratativasIrregularidade.Adicionar.visible(true);
    _CRUDDefinicaoTratativasIrregularidade.Adicionar.enable(true);
}

function PreencherNovaTratativaClick(e, data) {
    if (_definicaoTratativasIrregularidade.Irregularidade.codEntity() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Irregularidade não informada", "Selecione uma Irregularidade antes de adicionar uma Tratativa");
        return;
    }

    LimparCamposTratativaIrregularidadeClick();
    GerarCheckboxsAcoes(_definicaoTratativasIrregularidade.Irregularidade.Gatilho);
    Global.ResetarMultiplasAbas();
    Global.abrirModal('divAdicionarTratativa');
}

/*
 * Declaração das Funções
 */

function recarregarGridDefinicaoTratativaIrregularidade() {
    _gridDefinicaoTratativasIrregularidade.CarregarGrid();
}

function recarregarGridTratativasIrregularidade() {
    _gridTratativasIrregularidade.CarregarGrid([]);
}

function RenderizarGridTratativasIrregularidade() {

    var itens = _gridTratativasIrregularidade.BuscarRegistros();

    _gridTratativasIrregularidade.CarregarGrid(itens);
}

function PreencherObjetoParaBack() {

    var tratativasFront = _definicaoTratativasIrregularidade.TratativasIrregularidade.val();
    var Tratativas = [];

    tratativasFront.forEach(tratativa => {

        if (isNaN(tratativa.Codigo))
            tratativa.Codigo = 0;

        var obj = {
            Codigo: tratativa.Codigo,
            Sequencia: parseInt(tratativa.Sequencia),
            CodigoSetor: parseInt(tratativa.CodigoSetor),
            ProximaSequencia: parseInt(tratativa.ProximaSequencia),
            CodigoGrupoOperacao: parseInt(tratativa.CodigoGrupoOperacao),
            InformarMotivo: tratativa.InformarMotivo,
            Motivos: tratativa.Motivos.map(motivo => motivo = parseInt(motivo.Codigo)),
            Acoes: tratativa.Acoes
        };

        Tratativas.push(obj);
    });

    var definicao = RetornarObjetoPesquisa(_definicaoTratativasIrregularidade);
    definicao["Tratativas"] = JSON.stringify(Tratativas)

    return definicao;
}

function ValidarDefinicao() {

    var tratativas = _definicaoTratativasIrregularidade.TratativasIrregularidade.val();
    if (tratativas.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Definição Inválida", "É necessário definir pelo menos uma Tratativa");
        return false
    }

    var proximaSequenciaInexistente = true;
    var tratativaRepetida = false;
    for (var i = 0; i < tratativas.length; i++) {
        proximaSequenciaInexistente = true;

        for (var j = i + 1; j < tratativas.length; j++) {

            if (parseInt(tratativas[i].Sequencia) == parseInt(tratativas[j].Sequencia) && parseInt(tratativas[i].CodigoGrupoOperacao) == parseInt(tratativas[j].CodigoGrupoOperacao)) {
                tratativaRepetida = true;
                break;
            }
        }

        if (tratativaRepetida) {
            exibirMensagem(tipoMensagem.atencao, "Definição Inválida", "Não é permitido adicionar tratativas com mesma Sequência e Grupo de Operação");
            return false;
        }

        if (parseInt(tratativas[i].ProximaSequencia) != 0)
            for (var j = 0; j < tratativas.length; j++) {
                if (i != j)
                    if (parseInt(tratativas[i].ProximaSequencia) == parseInt(tratativas[j].Sequencia)) {
                        proximaSequenciaInexistente = false;
                        break;
                    }
            }
        else
            proximaSequenciaInexistente = false;

        if (proximaSequenciaInexistente) {
            exibirMensagem(tipoMensagem.atencao, "Definição Inválida", "A Sequência: " + tratativas[i].Sequencia + " possui uma Próxima Sequência inexistente");
            return !proximaSequenciaInexistente;
        }
    }

    return true;
}
