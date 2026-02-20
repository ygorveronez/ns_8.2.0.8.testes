/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="Constantes.js" />
/// <reference path="Etapa.js" />
/// <reference path="TipoIntegracao.js" />
/// <reference path="Atividades.js" />
/// <reference path="Funcionarios.js" />

var _pesquisaGrupoMotoristas;

var _grupoMotoristas;
var _grupoMotoristasFuncionario;
var _grupoMotoristasIntegracao;
var _grupoMotoristasIntegracaoArquivo;
var _crudGeral;

var _gridGrupoMotoristas;
var _gridTipoIntegracao;
var _gridFuncionario;


function LoadGrupoMotoristas() {

    _grupoMotoristas = new GrupoMotoristas();
    KoBindings(_grupoMotoristas, "knockoutDetalhes");

    HeaderAuditoria("GrupoMotoristas", _grupoMotoristas);

    _pesquisaGrupoMotoristas = new PesquisaGrupoMotoristas();
    KoBindings(_pesquisaGrupoMotoristas, "knockoutPesquisaGrupoMotoristas", false, _pesquisaGrupoMotoristas.Pesquisar.id);

    _crudGeral = new CRUDGeral();
    KoBindings(_crudGeral, "knockoutCRUDGrupoMotoristas");

    BuscarGrupoMotoristas();

    loadEtapaGrupoMotoristas();
    setarEtapaInicioGrupoMotoristas();
    loadGrupoMotoristasIntegracao();

    LoadFuncionarios();
    LoadGrupoMotoristasTipoIntegracao();
}

var CRUDGeral = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarGrupoMotoristasClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarGrupoMotoristasClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarEdicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirGrupoMotoristasClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var GrupoMotoristas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição", required: true, maxlength: 100, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", maxlength: 100, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6"), enable: false });
    this.Observacao = PropertyEntity({ text: "Observação", maxlength: 500 });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: EnumSituacaoGrupoMotoristas.obterOpcoes(), def: true, text: "Situação", visible: ko.observable(false) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação", issue: 557, visible: ko.observable(false) });

    this.TiposIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.Funcionarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
}

function ObterGrupoMotoristasAdicionarCompleto() {

    return {
        Descricao: _grupoMotoristas.Descricao.val(),
        CodigoIntegracao: _grupoMotoristas.CodigoIntegracao.val(),
        Observacao: _grupoMotoristas.Observacao.val(),
        Ativo: _grupoMotoristas.Ativo.val(),
        TiposIntegracao: JSON.stringify(_grupoMotoristas.TiposIntegracao.val()),
        Funcionarios: JSON.stringify(_grupoMotoristas.Funcionarios.val()),
    }
}

function ObterGrupoMotoristasAtualizarCompleto() {

    const ajuste = o => ({ ...o, CodigoRelacionamento: o.CodigoRelacionamento ?? 0 })

    return {
        Codigo: _grupoMotoristas.Codigo.val(),
        Descricao: _grupoMotoristas.Descricao.val(),
        CodigoIntegracao: _grupoMotoristas.CodigoIntegracao.val(),
        Observacao: _grupoMotoristas.Observacao.val(),
        Ativo: _grupoMotoristas.Ativo.val(),
        TiposIntegracao: JSON.stringify(_grupoMotoristas.TiposIntegracao.val()),
        Funcionarios: JSON.stringify(_grupoMotoristas.Funcionarios.val().map(ajuste)),
    }
}

function SelecionarGrupoMotoristas(grupoMotoristasGrid) {
    LimparGeral();

    RecuperarGrupoMotoristas(grupoMotoristasGrid.Codigo);
}


function CancelarEdicaoClick() {
    LimparGeral();
    LigarModoEdicao(false);
}


function PreencherGrupoMotoristas(data) {
    _grupoMotoristas.Codigo.val(data.Codigo);
    _grupoMotoristas.Descricao.val(data.Descricao);
    _grupoMotoristas.CodigoIntegracao.val(data.CodigoIntegracao);
    _grupoMotoristas.Observacao.val(data.Observacoes);
    _grupoMotoristas.Situacao.val(data.Situacao);
    _grupoMotoristas.Ativo.val(data.Ativo);
}