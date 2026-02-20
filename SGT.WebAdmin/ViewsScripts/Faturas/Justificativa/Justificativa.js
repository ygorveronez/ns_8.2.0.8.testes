/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridJustificativa;
var _justificativa;
var _pesquisaJustificativa;

var _opcoesTipoJustificativa = [
    { text: "Desconto", value: 1 },
    { text: "Acréscimo", value: 2 }
];

var _opcoesPesquisaTipoJustificativa = [
    { text: "Todas", value: "" },
    { text: "Desconto", value: 1 },
    { text: "Acréscimo", value: 2 }
];

var _opcoesFinalidadeJustificativa = [
    { text: "Todas", value: EnumTipoFinalidadeJustificativa.Todas },
    { text: "Fatura", value: EnumTipoFinalidadeJustificativa.Fatura },
    { text: "Títulos a Pagar", value: EnumTipoFinalidadeJustificativa.TitulosPagar },
    { text: "Títulos a Receber", value: EnumTipoFinalidadeJustificativa.TitulosReceber },
    { text: "Contrato de Frete", value: EnumTipoFinalidadeJustificativa.ContratoFrete },
    { text: "Acerto de Viagem - Justificativas do Motorista", value: EnumTipoFinalidadeJustificativa.AcertoViagemMotorista },
    { text: "Acerto de Viagem - Justificativas do Embarcador", value: EnumTipoFinalidadeJustificativa.AcertoViagemEmbarcador },
    { text: "Acerto de Viagem - Outras Despesas", value: EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas },
    { text: "Pendência Motorista", value: EnumTipoFinalidadeJustificativa.PendenciaMotorista }
];

var _opcoesPesquisaFinalidadeJustificativa = [
    { text: "Todas as opções", value: "" },
    { text: "Todas", value: EnumTipoFinalidadeJustificativa.Todas },
    { text: "Fatura", value: EnumTipoFinalidadeJustificativa.Fatura },
    { text: "Títulos a Pagar", value: EnumTipoFinalidadeJustificativa.TitulosPagar },
    { text: "Títulos a Receber", value: EnumTipoFinalidadeJustificativa.TitulosReceber },
    { text: "Contrato de Frete", value: EnumTipoFinalidadeJustificativa.ContratoFrete },
    { text: "Acerto de Viagem - Justificativas do Motorista", value: EnumTipoFinalidadeJustificativa.AcertoViagemMotorista },
    { text: "Acerto de Viagem - Justificativas do Embarcador", value: EnumTipoFinalidadeJustificativa.AcertoViagemEmbarcador },
    { text: "Acerto de Viagem - Outras Despesas", value: EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas },
    { text: "Pendência Motorista", value: EnumTipoFinalidadeJustificativa.PendenciaMotorista }
];

var PesquisaJustificativa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.FinalidadeJustificativa = PropertyEntity({ val: ko.observable(""), options: _opcoesPesquisaFinalidadeJustificativa, def: "", text: "Finalidade da Justificativa: " });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(""), options: _opcoesPesquisaTipoJustificativa, def: "", text: "Tipo da Justificativa: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativa.CarregarGrid();
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

var Justificativa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, visible: ko.observable(true) });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(1), options: _opcoesTipoJustificativa, text: "*Tipo da Justificativa:", def: 1, visible: ko.observable(true) });
    this.FinalidadeJustificativa = PropertyEntity({ val: ko.observable(EnumTipoFinalidadeJustificativa.Todas), options: _opcoesFinalidadeJustificativa, def: EnumTipoFinalidadeJustificativa.Todas, text: "*Finalidade da Justificativa:", issue: 454, visible: ko.observable(true) });
    this.AplicacaoValorContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoValorJustificativaContratoFrete.NoAdiantamento), options: EnumTipoAplicacaoValorJustificativaContratoFrete.obterOpcoes(), def: EnumTipoAplicacaoValorJustificativaContratoFrete.NoAdiantamento, text: "*Aplicação do Valor: ", visible: ko.observable(false), issue: 455 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", visible: ko.observable(true) });
    this.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto = PropertyEntity({ text: "Usar data de autorização para movimento de Acréscimo/Desconto", getType: typesKnockout.bool, visible: ko.observable(false) });

    this.GerarMovimentoAutomatico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro automatizado para essa justificativa: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoJustificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Justificativa:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoUsoJustificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Uso da Justificativa:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Avaria:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", visible: ko.observable(true), maxlength: 10 });

    this.CodigoIntegracaoRepom = PropertyEntity({ text: "Código de Integração: ", visible: ko.observable(true), maxlength: 5 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.FinalidadeJustificativa.val.subscribe(function (novoValor) {
        FinalidadeJustificativaChange(novoValor);
    });

    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoChange(novoValor);
    });
}

//*******EVENTOS*******

function loadJustificativa() {

    _justificativa = new Justificativa();
    KoBindings(_justificativa, "knockoutCadastroJustificativa");

    _pesquisaJustificativa = new PesquisaJustificativa();
    KoBindings(_pesquisaJustificativa, "knockoutPesquisaJustificativa", false, _pesquisaJustificativa.Pesquisar.id);

    HeaderAuditoria("Justificativa", _justificativa);

    new BuscarTipoMovimento(_justificativa.TipoMovimentoUsoJustificativa, null, null, null, null, EnumFinalidadeTipoMovimento.Justificativa);
    new BuscarTipoMovimento(_justificativa.TipoMovimentoReversaoUsoJustificativa, null, null, null, null, EnumFinalidadeTipoMovimento.Justificativa);
    new BuscarMotivoAvaria(_justificativa.MotivoAvaria);

    buscarJustificativas();
    ConfigurarIntegracoesDisponiveis();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
        $("#liTabMovimentoFinanceiro").removeClass("d-none");
}

function FinalidadeJustificativaChange(novoValor) {
    if (novoValor == EnumTipoFinalidadeJustificativa.ContratoFrete) {
        _justificativa.AplicacaoValorContratoFrete.visible(true);
        _justificativa.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto.visible(true);
    } else {
        _justificativa.AplicacaoValorContratoFrete.visible(false);
        _justificativa.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto.visible(false);
    }
}

function GerarMovimentoAutomaticoChange(novoValor) {
    if (novoValor) {
        _justificativa.GerarMovimentoAutomatico.visibleFade(true);
        _justificativa.TipoMovimentoUsoJustificativa.required(true);
        _justificativa.TipoMovimentoReversaoUsoJustificativa.required(true);
    } else {
        _justificativa.GerarMovimentoAutomatico.visibleFade(false);
        _justificativa.TipoMovimentoUsoJustificativa.required(false);
        _justificativa.TipoMovimentoReversaoUsoJustificativa.required(false);
    }
}

function adicionarClick(e, sender) {
    Salvar(e, "Justificativa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridJustificativa.CarregarGrid();
                limparCamposJustificativa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Justificativa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridJustificativa.CarregarGrid();
                limparCamposJustificativa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a justificativa " + _justificativa.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_justificativa, "Justificativa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridJustificativa.CarregarGrid();
                limparCamposJustificativa();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposJustificativa();
}

//*******MÉTODOS*******


function buscarJustificativas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarJustificativa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridJustificativa = new GridView(_pesquisaJustificativa.Pesquisar.idGrid, "Justificativa/Pesquisa", _pesquisaJustificativa, menuOpcoes, null);
    _gridJustificativa.CarregarGrid();
}

function editarJustificativa(justificativaGrid) {
    limparCamposJustificativa();
    _justificativa.Codigo.val(justificativaGrid.Codigo);
    BuscarPorCodigo(_justificativa, "Justificativa/BuscarPorCodigo", function (arg) {
        _pesquisaJustificativa.ExibirFiltros.visibleFade(false);
        _justificativa.Atualizar.visible(true);
        _justificativa.Cancelar.visible(true);
        _justificativa.Excluir.visible(true);
        _justificativa.Adicionar.visible(false);
    }, null);
}

function limparCamposJustificativa() {
    _justificativa.Atualizar.visible(false);
    _justificativa.Cancelar.visible(false);
    _justificativa.Excluir.visible(false);
    _justificativa.Adicionar.visible(true);
    LimparCampos(_justificativa);
    $(".nav-tabs a:first").tab("show");
}

function ConfigurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.OperadorasCIOTExistentes != null && r.Data.OperadorasCIOTExistentes.length > 0) {
                if (r.Data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Repom || o == EnumOperadoraCIOT.RepomFrete; })) {
                    $("#liRepom").removeClass("d-none");
                }
            }
        }
    });
}