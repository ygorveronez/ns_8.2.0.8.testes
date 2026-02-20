/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoOrdemServico;
var _tipoOrdemServico;
var _pesquisaTipoOrdemServico;
var _gridTipoOrdemServico;

/*
 * Declaração das Classes
 */

var CRUDTipoOrdemServico = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoOrdemServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 100 });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.ObrigarInformarLocalDeArmazenamentoOS = PropertyEntity({ text: "Obrigar informar local de armazenamento na abertura da OS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OSCorretiva = PropertyEntity({ text: "OS Corretiva?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LancarServicosOSManualmente = PropertyEntity({ text: "Lançar Serviços na OS manualmente", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false)  });
    this.ObrigarInformarCondicaoPagamento = PropertyEntity({ text: "Obrigar informar a condição de pagamento na OS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarMotivoLiberarVeiculoManutencao = PropertyEntity({ text: "Informar motivo para liberar veiculo da manutenção", getType: typesKnockout.bool, val: ko.observable(false) });

    this.OSCorretiva.val.subscribe((valor) => {
        _tipoOrdemServico.LancarServicosOSManualmente.visible(valor);
    });
};

var PesquisaTipoOrdemServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoOrdemServico, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoOrdemServico() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoOrdemServico/ExportarPesquisa", titulo: "Tipos de Ordem de Serviço" };

    _gridTipoOrdemServico = new GridViewExportacao(_pesquisaTipoOrdemServico.Pesquisar.idGrid, "TipoOrdemServico/Pesquisa", _pesquisaTipoOrdemServico, menuOpcoes, configuracoesExportacao);
    _gridTipoOrdemServico.CarregarGrid();
}

function LoadTipoOrdemServico() {
    _tipoOrdemServico = new TipoOrdemServico();
    KoBindings(_tipoOrdemServico, "knockoutTipoOrdemServico");

    HeaderAuditoria("OrdemServicoFrotaTipo", _tipoOrdemServico);

    _CRUDTipoOrdemServico = new CRUDTipoOrdemServico();
    KoBindings(_CRUDTipoOrdemServico, "knockoutCRUDTipoOrdemServico");

    _pesquisaTipoOrdemServico = new PesquisaTipoOrdemServico();
    KoBindings(_pesquisaTipoOrdemServico, "knockoutPesquisaTipoOrdemServico", false, _pesquisaTipoOrdemServico.Pesquisar.id);

    LoadGridTipoOrdemServico();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tipoOrdemServico, "TipoOrdemServico/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoOrdemServico();
                LimparCamposTipoOrdemServico();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoOrdemServico, "TipoOrdemServico/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTipoOrdemServico();
                LimparCamposTipoOrdemServico();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoOrdemServico();
}

function EditarClick(registroSelecionado) {
    LimparCamposTipoOrdemServico();

    _tipoOrdemServico.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoOrdemServico, "TipoOrdemServico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoOrdemServico.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoOrdemServico, "TipoOrdemServico/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridTipoOrdemServico();
                    LimparCamposTipoOrdemServico();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDTipoOrdemServico.Atualizar.visible(isEdicao);
    _CRUDTipoOrdemServico.Excluir.visible(isEdicao);
    _CRUDTipoOrdemServico.Cancelar.visible(isEdicao);
    _CRUDTipoOrdemServico.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoOrdemServico() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoOrdemServico);
}

function RecarregarGridTipoOrdemServico() {
    _gridTipoOrdemServico.CarregarGrid();
}