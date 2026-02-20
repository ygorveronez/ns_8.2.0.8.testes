//*******MAPEAMENTO KNOUCKOUT*******

var _gridFinalidadeProdutoOrdemServico;
var _finalidadeProdutoOrdemServico;
var _pesquisaFinalidadeProdutoOrdemServico;

var PesquisaFinalidadeProdutoOrdemServico = function () {
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFinalidadeProdutoOrdemServico.CarregarGrid();
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

var FinalidadeProdutoOrdemServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 150, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 100 });
    this.TipoMovimentoUso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento de Uso:", idBtnSearch: guid(), required: true });
    this.TipoMovimentoReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento de Reversão:", idBtnSearch: guid(), required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 400 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadFinalidadeProdutoOrdemServico() {

    _finalidadeProdutoOrdemServico = new FinalidadeProdutoOrdemServico();
    KoBindings(_finalidadeProdutoOrdemServico, "knockoutCadastroFinalidadeProdutoOrdemServico");

    _pesquisaFinalidadeProdutoOrdemServico = new PesquisaFinalidadeProdutoOrdemServico();
    KoBindings(_pesquisaFinalidadeProdutoOrdemServico, "knockoutPesquisaFinalidadeProdutoOrdemServico", _pesquisaFinalidadeProdutoOrdemServico.Pesquisar.id);

    new BuscarTipoMovimento(_finalidadeProdutoOrdemServico.TipoMovimentoUso);
    new BuscarTipoMovimento(_finalidadeProdutoOrdemServico.TipoMovimentoReversao);

    BuscarFinalidadesProdutoOrdemServico();
}

function AdicionarClick(e, sender) {
    Salvar(e, "FinalidadeProdutoOrdemServico/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridFinalidadeProdutoOrdemServico.CarregarGrid();
                LimparCamposFinalidadeProdutoOrdemServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(e, "FinalidadeProdutoOrdemServico/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridFinalidadeProdutoOrdemServico.CarregarGrid();
                LimparCamposFinalidadeProdutoOrdemServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir esta finalidade de produto?", function () {
        ExcluirPorCodigo(_finalidadeProdutoOrdemServico, "FinalidadeProdutoOrdemServico/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridFinalidadeProdutoOrdemServico.CarregarGrid();
                    LimparCamposFinalidadeProdutoOrdemServico();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposFinalidadeProdutoOrdemServico();
}

//*******MÉTODOS*******

function EditarFinalidadeProdutoOrdemServico(finalidadeProdutoGrid) {
    LimparCamposFinalidadeProdutoOrdemServico();
    _finalidadeProdutoOrdemServico.Codigo.val(finalidadeProdutoGrid.Codigo);
    BuscarPorCodigo(_finalidadeProdutoOrdemServico, "FinalidadeProdutoOrdemServico/BuscarPorCodigo", function (arg) {
        _pesquisaFinalidadeProdutoOrdemServico.ExibirFiltros.visibleFade(false);
        _finalidadeProdutoOrdemServico.Atualizar.visible(true);
        _finalidadeProdutoOrdemServico.Cancelar.visible(true);
        _finalidadeProdutoOrdemServico.Excluir.visible(true);
        _finalidadeProdutoOrdemServico.Adicionar.visible(false);
    }, null);
}

function BuscarFinalidadesProdutoOrdemServico() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarFinalidadeProdutoOrdemServico, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFinalidadeProdutoOrdemServico = new GridView(_pesquisaFinalidadeProdutoOrdemServico.Pesquisar.idGrid, "FinalidadeProdutoOrdemServico/Pesquisa", _pesquisaFinalidadeProdutoOrdemServico, menuOpcoes, null);
    _gridFinalidadeProdutoOrdemServico.CarregarGrid();
}

function LimparCamposFinalidadeProdutoOrdemServico() {
    _finalidadeProdutoOrdemServico.Atualizar.visible(false);
    _finalidadeProdutoOrdemServico.Cancelar.visible(false);
    _finalidadeProdutoOrdemServico.Excluir.visible(false);
    _finalidadeProdutoOrdemServico.Adicionar.visible(true);
    LimparCampos(_finalidadeProdutoOrdemServico);
}

