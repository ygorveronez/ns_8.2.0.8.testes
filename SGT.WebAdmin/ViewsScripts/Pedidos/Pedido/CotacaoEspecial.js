/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacao.js" />

var _pesquisaCotacao;
var _simulacaoCotacao;
var _gridCotacao = null
var codigoCotacaoSelecionado = null;

function PesquisaCotacao() {
    this.StatusCotacaoEspecial = PropertyEntity({ val: ko.observable(EnumStatusCotacao.Todos), options: EnumStatusCotacao.obterOpcoesPesquisa(), def: EnumStatusCotacao.Todos });
    this.DataCotacaoInicial = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date });
    this.DataCotacaoFinal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date });
    this.DataCotacaoInicial.dateRangeLimit = this.DataCotacaoInicial;
    this.DataCotacaoFinal.dateRangeInit = this.DataCotacaoFinal;
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), maxlength: 50 });
    this.NumeroCotacao = PropertyEntity({ val: ko.observable("") });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumModalCotacaoEspecial.Todos), options: EnumModalCotacaoEspecial.obterOpcoesPesquisa(), def: EnumModalCotacaoEspecial.Todos });
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });


    this.StatusCotacaoEspecial.val.subscribe(buscarCotacoes);
    this.DataCotacaoInicial.val.subscribe(buscarCotacoes);
    this.DataCotacaoFinal.val.subscribe(buscarCotacoes);
    this.NumeroPedido.val.subscribe(buscarCotacoes);
    this.NumeroCotacao.val.subscribe(buscarCotacoes);
    this.TipoModal.val.subscribe(buscarCotacoes);
}

function SimularCotacaoEspecial() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, visible: ko.observable(true), codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), required: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, visible: ko.observable(true), codEntity: ko.observable(0), text: "*Tipo Operação:", idBtnSearch: guid(), required: true });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, visible: ko.observable(true), codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), required: true });
    this.ValorSimulado = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true), enable: ko.observable(false) });
}

function loadCotacaoEspecial() {
    _pesquisaCotacao = new PesquisaCotacao();
    KoBindings(_pesquisaCotacao, "knoutPesquisaCotacao");
    _simulacaoCotacao = new SimularCotacaoEspecial();
    KoBindings(_simulacaoCotacao, "knoutSimulacaoCotacao");

    buscarCotacoes();
}

function buscarCotacoes() {
    let Aprovar = { descricao: Localization.Resources.Gerais.Geral.Aprovar, id: guid(), tamanho: 9, metodo: aprovarCotacaoClick, visibilidade: VisibilidadeFornecedor };
    let Rejeitar = { descricao: Localization.Resources.Gerais.Geral.Rejeitar, id: guid(), tamanho: 9, metodo: rejeitarCotacaoClick, visibilidade: VisibilidadeFornecedor };
    let SimularCotacao = {
        descricao: Localization.Resources.Gerais.Geral.SimularCotacao,
        id: guid(),
        tamanho: 9,
        metodo: function (e) {
            simularCotacaoClick(e.Codigo);
        },
        visibilidade: VisibilidadeEmbarcador
    };

    let menuOpcoesEmb = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [SimularCotacao] };
    let menuOpcoesCli = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [Aprovar, Rejeitar] };
    const opcoesMenu = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? menuOpcoesEmb : menuOpcoesCli;

    _gridCotacao = new GridView(_pesquisaCotacao.Grid.id, "CotacaoEspecial/Pesquisa", _pesquisaCotacao, opcoesMenu, null, null, null, null, null, null, 25);
    _gridCotacao.SetHabilitarLinhaClicavel(true);
    _gridCotacao.CarregarGrid();
}

function simularCotacaoClick(registroSelecionado) {
    LimparCampos(_simulacaoCotacao);

    new BuscarTransportadores(_simulacaoCotacao.Transportador);
    new BuscarTiposOperacao(_simulacaoCotacao.TipoOperacao);
    new BuscarModelosVeicularesCarga(_simulacaoCotacao.ModeloVeicular);
    _simulacaoCotacao.Codigo.val(registroSelecionado);

    Global.abrirModal('#knoutSimulacaoCotacao');

    $('#btnCalcularFrete').off('click').on('click', function () {
        _simulacaoCotacao.ValorSimulado.enable(false);

        Salvar(_simulacaoCotacao, "CotacaoEspecial/SimularFreteCotacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _simulacaoCotacao.ValorSimulado.val(arg.Data.ValorSimulado);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Simulação de frete realizada com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function confirmarCotacaoClick() {
    const valorSimulado = _simulacaoCotacao.ValorSimulado.val();

    if (!valorSimulado) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um valor antes de confirmar.");
        return;
    }

    Salvar(_simulacaoCotacao, "CotacaoEspecial/ConfirmarCotacao", function (response) {
        if (response.Success) {
            if (response.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cotação confirmada com sucesso.");
                _gridCotacao.CarregarGrid();
                Global.fecharModal('#knoutSimulacaoCotacao');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, response.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Msg);
        }
    });
}

function aprovarCotacaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente aprovar essa cotação?", function () {
        var dados = {
            CodigoCotacao: e.Codigo
        };
        executarReST("CotacaoEspecial/AprovarCotacao", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    _gridCotacao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function rejeitarCotacaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente rejeitar essa cotação?", function () {
        var dados = {
            CodigoCotacao: e.Codigo
        };
        executarReST("CotacaoEspecial/RejeitarCotacao", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    _gridCotacao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function informarManualmenteClick() {
    _simulacaoCotacao.ValorSimulado.enable(true);
    _simulacaoCotacao.ValorSimulado.val('');
}

function VisibilidadeFornecedor(row) {
    return row.Status == EnumStatusCotacao.AguardandoAprovacao;
}

function VisibilidadeEmbarcador(row) {
    return row.Status == EnumStatusCotacao.AguardandoAnalise;
}