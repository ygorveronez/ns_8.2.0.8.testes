/// <reference path="../../Enumeradores/EnumNivelInfracaoTransito.js" />
/// <reference path="../../Enumeradores/EnumTipoInfracaoTransito.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoInfracao;
var _tipoInfracao;
var _pesquisaTipoInfracao;
var _gridTipoInfracao;

/*
 * Declaração das Classes
 */

var CRUDTipoInfracao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCTB = PropertyEntity({ text: "Código C.T.B.:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 100 });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.Nivel = PropertyEntity({ text: "*Nivel: ", val: ko.observable(EnumNivelInfracaoTransito.Leve), options: EnumNivelInfracaoTransito.obterOpcoes(), def: true, required: true });
    this.Pontos = PropertyEntity({ text: "*Pontos:", getType: typesKnockout.int, val: ko.observable("0"), def: "0", configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 2, required: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoInfracaoTransito.Multa), options: EnumTipoInfracaoTransito.obterOpcoes(), def: true, required: true });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: true });
    this.Valor = PropertyEntity({ text: ko.observable("*Valor:"), getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true }, maxlength: 11, required: ko.observable(true) });

    this.GerarMovimentoFichaMotorista = PropertyEntity({ text: "Gerar movimento financeiro para a ficha do motorista?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoGerarTituloFinanceiro = PropertyEntity({ text: "Não gerar títulos financeiros ao finalizar a Infração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirReplicarInformacao = PropertyEntity({ text: "Permite replicar informações de outra Infração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AdicionarRenavamVeiculoObservacao = PropertyEntity({ text: "Adicionar Renavam do Veículo na observação da Infração?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoObrigarInformarCidade = PropertyEntity({ text: "Não obrigar informar local", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoObrigarInformarLocal = PropertyEntity({ text: "Não obrigar informar cidade", getType: typesKnockout.bool, val: ko.observable(false), def: false });


    this.TipoMovimentoFichaMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo Movimento p/ ficha do motorista:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoMovimentoTituloEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo Movimento p/ título gerado para a empresa:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.LancarDescontoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Lançar desconto para o motorista ao próximo acerto? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.DescontoComissaoMotorista = PropertyEntity({ text: "*Valor Desconto:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false), visible: ko.observable(true) });
    this.JustificativaDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.LancarDescontoMotorista.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoInfracao.DescontoComissaoMotorista.required(true);
            _tipoInfracao.JustificativaDesconto.required(true);
        } else {
            _tipoInfracao.DescontoComissaoMotorista.required(false);
            _tipoInfracao.JustificativaDesconto.required(false);
        }
    });

    this.ReduzirPercentualComissaoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: ko.observable("Reduzir o percentual de comissão ao próximo acerto? "), idFade: guid(), visibleFade: ko.observable(false) });
    this.PercentualReducaoComissaoMotorista = PropertyEntity({ text: "*- % Comissão:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false), visible: ko.observable(true) });
    this.ReduzirPercentualComissaoMotorista.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoInfracao.PercentualReducaoComissaoMotorista.required(true);
        } else {
            _tipoInfracao.PercentualReducaoComissaoMotorista.required(false);
        }
    });

    this.Tipo.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoInfracaoTransito.Multa) {
            _tipoInfracao.Valor.required(true);
            _tipoInfracao.Valor.text("*Valor");
        } else {
            _tipoInfracao.Valor.required(false);
            _tipoInfracao.Valor.text("Valor");
        }
    });
};

var PesquisaTipoInfracao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoCTB = PropertyEntity({ text: "Código C.T.B.:", maxlength: 100 });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoInfracao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoInfracao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoInfracao/ExportarPesquisa", titulo: "Tipo de Infração" };

    _gridTipoInfracao = new GridViewExportacao(_pesquisaTipoInfracao.Pesquisar.idGrid, "TipoInfracao/Pesquisa", _pesquisaTipoInfracao, menuOpcoes, configuracoesExportacao);
    _gridTipoInfracao.CarregarGrid();
}

function loadTipoInfracao() {
    _tipoInfracao = new TipoInfracao();
    KoBindings(_tipoInfracao, "knockoutTipoInfracao");

    HeaderAuditoria("TipoInfracao", _tipoInfracao);

    _CRUDTipoInfracao = new CRUDTipoInfracao();
    KoBindings(_CRUDTipoInfracao, "knockoutCRUDTipoInfracao");

    _pesquisaTipoInfracao = new PesquisaTipoInfracao();
    KoBindings(_pesquisaTipoInfracao, "knockoutPesquisaTipoInfracao", false, _pesquisaTipoInfracao.Pesquisar.id);

    new BuscarJustificativas(_tipoInfracao.JustificativaDesconto, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);
    new BuscarTipoMovimento(_tipoInfracao.TipoMovimentoFichaMotorista);
    new BuscarTipoMovimento(_tipoInfracao.TipoMovimentoTituloEmpresa);

    if (_CONFIGURACAO_TMS.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem) {
        _tipoInfracao.DescontoComissaoMotorista.visible(false);
        _tipoInfracao.PercentualReducaoComissaoMotorista.visible(true);
        _tipoInfracao.ReduzirPercentualComissaoMotorista.text("Reduzir o percentual de comissão ao motorista associado a ocorrência?");
    }

    loadGridTipoInfracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    if (_tipoInfracao.Valor.val() == '0,00' && _tipoInfracao.Tipo.val() == EnumTipoInfracaoTransito.Multa) {
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }

    Salvar(_tipoInfracao, "TipoInfracao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTipoInfracao();
                limparCamposTipoInfracao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoInfracao, "TipoInfracao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTipoInfracao();
                limparCamposTipoInfracao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposTipoInfracao();
}

function editarClick(registroSelecionado) {
    limparCamposTipoInfracao();

    _tipoInfracao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoInfracao, "TipoInfracao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoInfracao.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoInfracao, "TipoInfracao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTipoInfracao();
                    limparCamposTipoInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDTipoInfracao.Atualizar.visible(isEdicao);
    _CRUDTipoInfracao.Excluir.visible(isEdicao);
    _CRUDTipoInfracao.Cancelar.visible(isEdicao);
    _CRUDTipoInfracao.Adicionar.visible(!isEdicao);
}

function limparCamposTipoInfracao() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoInfracao);
}

function recarregarGridTipoInfracao() {
    _gridTipoInfracao.CarregarGrid();
}