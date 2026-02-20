/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportarOcorrencia.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _movitoRejeicaoOcorrencia;
var _pesquisaImportarOcorrencia;
var _gridImportarOcorrencia;

var ImportarOcorrencia = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades    
    this.NumeroCTe = PropertyEntity({ text: "Nº CT-e:", required: false, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(false) });
    this.SerieCTe = PropertyEntity({ text: "Nº CT-e:", required: false, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(false) });
    this.DataOcorrencia = PropertyEntity({ text: "Data da Ocorrência:", required: false, getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.ObservacaoImpressa = PropertyEntity({ text: "Observação Interna:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.Booking = PropertyEntity({ text: "Booking:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.OrdemServico = PropertyEntity({ text: "Nº O.S.:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.NumeroCarga = PropertyEntity({ text: "Nº Carga:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.CodigoTipoOcorrencia = PropertyEntity({ text: "Cod. Tipo. Ocorrencia:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.CodigoComponenteFrete = PropertyEntity({ text: "Cod. Comp. Frete:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.ValorOcorrencia = PropertyEntity({ text: "Valor Ocorrência", required: false, getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 20, enable: ko.observable(false) });
    this.AliquotaICMS = PropertyEntity({ text: "Alíquota ICMS", required: false, getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 20, enable: ko.observable(false) });    
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Ocorrência:", idBtnSearch: guid(), visible: true, enable: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: true, enable: ko.observable(false) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: true, enable: ko.observable(false) });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Componente:", idBtnSearch: guid(), visible: true, enable: ko.observable(false) });
    this.CargaOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: true, enable: ko.observable(false) });
    this.RetornoImportacao = PropertyEntity({ text: "Retorno Importação:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.NomeArquivo = PropertyEntity({ text: "Nome do Arquivo:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 5000, enable: ko.observable(false) });
    this.SituacaoImportarOcorrencia = PropertyEntity({ val: ko.observable(EnumSituacaoImportarOcorrencia.AgIntegracao), options: EnumSituacaoImportarOcorrencia.obterOpcoes(), def: EnumSituacaoImportarOcorrencia.AgIntegracao, text: "Situação: ", enable: ko.observable(false) });
    this.CST = PropertyEntity({ text: "CST:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 2, enable: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ImportarOcorrencia/Importar",
        UrlConfiguracao: "ImportarOcorrencia/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O037_ImportarOcorrencia,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridImportarOcorrencia.CarregarGrid();
        }
    });
}

var PesquisaImportarOcorrencia = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoImportarOcorrencia.Todas), options: EnumSituacaoImportarOcorrencia.obterOpcoesPesquisa(), def: EnumSituacaoImportarOcorrencia.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImportarOcorrencia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadImportarOcorrencia() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaImportarOcorrencia = new PesquisaImportarOcorrencia();
    KoBindings(_pesquisaImportarOcorrencia, "knockoutPesquisaImportarOcorrencia", false, _pesquisaImportarOcorrencia.Pesquisar.id);

    // Instancia Produto
    _importarOcorrencia = new ImportarOcorrencia();
    KoBindings(_importarOcorrencia, "knockoutImportarOcorrencia");

    new BuscarTipoOcorrencia(_importarOcorrencia.TipoOcorrencia);
    new BuscarCargas(_importarOcorrencia.Carga);
    new BuscarCTes(_importarOcorrencia.CTe);
    new BuscarComponentesDeFrete(_importarOcorrencia.ComponenteFrete);
    new BuscarOcorrencias(_importarOcorrencia.CargaOcorrencia);

    HeaderAuditoria("ImportarOcorrencia", _importarOcorrencia);

    // Inicia busca
    BuscarImportarOcorrencia();
}

function adicionarClick(e, sender) {
    Salvar(_importarOcorrencia, "ImportarOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridImportarOcorrencia.CarregarGrid();
                limparCamposImportarOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposImportarOcorrencia();
}

function editarImportarOcorrenciaClick(itemGrid) {
    // Limpa os campos
    limparCamposImportarOcorrencia();

    // Seta o codigo do Produto
    _importarOcorrencia.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_importarOcorrencia, "ImportarOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaImportarOcorrencia.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD                                
                _importarOcorrencia.Cancelar.visible(true);
                _importarOcorrencia.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarImportarOcorrencia() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarImportarOcorrenciaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "ImportarOcorrencia/ExportarPesquisa",
        titulo: "Motivos de Rejeição de Ocorrência"
    };

    // Inicia Grid de busca
    _gridImportarOcorrencia = new GridViewExportacao(_pesquisaImportarOcorrencia.Pesquisar.idGrid, "ImportarOcorrencia/Pesquisa", _pesquisaImportarOcorrencia, menuOpcoes, configExportacao);
    _gridImportarOcorrencia.CarregarGrid();
}

function limparCamposImportarOcorrencia() {
    _importarOcorrencia.Cancelar.visible(false);
    _importarOcorrencia.Adicionar.visible(false);
    LimparCampos(_importarOcorrencia);
}