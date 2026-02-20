/// <reference path="Dados.js" />
/// <reference path="DadosTipoOperacao.js" />
/// <reference path="Escolta.js" />
/// <reference path="Estadia.js" />
/// <reference path="HoraExtra.js" />
/// <reference path="Pernoite.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridValorParametroOcorrencia;
var _pesquisaValorParametroOcorrencia;
var _CRUDValorParametroOcorrencia;

var CRUDValorParametroOcorrencia = function () {
    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarValorParametroOcorrencia, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarValorParametroOcorrencia, visible: ko.observable(false) });
    //this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDEstadia, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposValorParametroOcorrencia, visible: ko.observable(true) });
};

var PesquisaValorParametroOcorrencia = function () {
    this.GrupoPessoa = PropertyEntity({ text: "Grupo Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ text: "Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridValorParametroOcorrencia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******
function loadValorParametroOcorrencia() {
    _pesquisaValorParametroOcorrencia = new PesquisaValorParametroOcorrencia();
    KoBindings(_pesquisaValorParametroOcorrencia, "knockoutPesquisaValorParametroOcorrencia", false, _pesquisaValorParametroOcorrencia.Pesquisar.id);

    _CRUDValorParametroOcorrencia = new CRUDValorParametroOcorrencia();
    KoBindings(_CRUDValorParametroOcorrencia, "knockoutCRUDValorParametroOcorrencia");

    LoadDados();
    LoadHoraExtra();
    LoadEstadia();
    LoadPernoite();
    LoadEscolta();
    loadDadosTipoOperacao();
    
    new BuscarClientes(_pesquisaValorParametroOcorrencia.Pessoa);
    new BuscarGruposPessoas(_pesquisaValorParametroOcorrencia.GrupoPessoa);
    new BuscarTiposOperacao(_pesquisaValorParametroOcorrencia.TipoOperacao);

    HeaderAuditoria("ValorParametroOcorrencia", _dados);
    BuscarValorParametroOcorrencia();
    LimparCamposValorParametroOcorrencia();
}


//*******METODOS*******
function BuscarValorParametroOcorrencia() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar",
                id: guid(),
                evento: "onclick",
                metodo: EditarValorParametroOcorrencia,
                tamanho: "15",
                icone: ""
            }
        ]
    };

    _gridValorParametroOcorrencia = new GridView(_pesquisaValorParametroOcorrencia.Pesquisar.idGrid, "ValorParametroOcorrencia/Pesquisa", _pesquisaValorParametroOcorrencia, menuOpcoes, null);
    _gridValorParametroOcorrencia.CarregarGrid();
}

function EditarValorParametroOcorrencia(data) {
    BuscarValorParametroOcorrenciaPorCodigo(data.Codigo);
}

function BuscarValorParametroOcorrenciaPorCodigo(codigo) {
    LimparCamposValorParametroOcorrencia();
    _dados.Codigo.val(codigo);
    BuscarPorCodigo(_dados, "ValorParametroOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaValorParametroOcorrencia.ExibirFiltros.visibleFade(false);

                RenderizaGridHoraExtra(_horaExtraVeiculo);
                RenderizaGridHoraExtra(_horaExtraAjudante);

                RenderizaGridEstadia(_estadiaAjudante);
                RenderizaGridEstadia(_estadiaVeiculo);

                RenderizaGridPernoite(_pernoiteValor);

                _CRUDValorParametroOcorrencia.Adicionar.visible(false);
                _CRUDValorParametroOcorrencia.Atualizar.visible(true);

                RecarregarGridDadosTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AdicionarValorParametroOcorrencia(e, sender) {
    preencherListasSelecaoDadosTipoOperacao();

    Salvar(_dados, "ValorParametroOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridValorParametroOcorrencia.CarregarGrid();
                LimparCamposValorParametroOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarValorParametroOcorrencia(e, sender) {
    preencherListasSelecaoDadosTipoOperacao();

    Salvar(_dados, "ValorParametroOcorrencia/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridValorParametroOcorrencia.CarregarGrid();
                LimparCamposValorParametroOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function LimparCamposValorParametroOcorrencia() {
    LimparCamposDados();
    LimparCamposHoraExtra();
    LimparCamposEstadia();
    LimparCamposPernoite();
    LimparCamposEscolta();

    $("#tabDados").click();

    _CRUDValorParametroOcorrencia.Adicionar.visible(true);
    _CRUDValorParametroOcorrencia.Atualizar.visible(false);
}