/// <reference path="Aprovacao.js" />
/// <reference path="DadosInfracao.js" />
/// <reference path="Etapas.js" />
/// <reference path="ProcessamentoInfracao.js" />
/// <reference path="Resumo.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Infracao.js" />
/// <reference path="../../Enumeradores/EnumAbaInfracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Enumeradores/EnumTipORecibo.js" />
/// <reference path="../../Configuracoes/Configuracao/Configuracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _CRUDInfracao;
var _gridInfracao;
var _infracao;
var _pesquisaInfracao;

/*
 * Declaração das Classes
 */

var PesquisaInfracao = function () {
    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade:", idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.NumeroAtuacao = PropertyEntity({ text: "Número da Autuação:", maxlength: 100 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoInfracao.Todas), options: EnumSituacaoInfracao.obterOpcoes(), def: EnumSituacaoInfracao.Todas, text: "Situação: " });
    this.TipoInfracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.OrgaoEmissor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Órgão Emissor:", idBtnSearch: guid() });
    this.TipoOcorrenciaInfracao = PropertyEntity({ val: ko.observable(""), options: EnumTipoOcorrenciaInfracao.ObterOpcoesPesquisa(), def: "", text: "Tipo de Ocorrência: " });
    this.TipoHistorico = PropertyEntity({ val: ko.observable(""), options: EnumTipoHistoricoInfracao.obterOpcoesPesquisa(), def: "", text: "Histórico: " });
    this.TipoInfracaoTransito = PropertyEntity({ text: "Tipo da Infração:", options: EnumTipoInfracaoTransito.obterOpcoesPesquisa(), val: ko.observable(EnumTipoInfracaoTransito.Todos), def: EnumTipoInfracaoTransito.Todos });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            $("#" + _pesquisaInfracao.Pesquisar.id).focus();
            setTimeout(
                function () {
                    _gridInfracao.CarregarGrid();
                }, 500);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Infracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoInfracao.Todas), def: EnumSituacaoInfracao.Todas, getType: typesKnockout.int });

    this.FaturadoTitulosEmpresa = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.OrigemIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var CRUDInfracao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Estornar = PropertyEntity({ eventClick: EstornarClick, type: types.event, text: "Estornar", visible: ko.observable(false) });
    this.ImprimirRecibo = PropertyEntity({ eventClick: imprimirReciboClick, type: types.event, text: "Recibo", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova", visible: ko.observable(true) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.BaixarArquivoIntegracao = PropertyEntity({ eventClick: baixarArquivoIntegracaoClick, type: types.event, text: "Baixar Arquivo Integração", visible: ko.observable(false) });
    this.AprovarIntegracao = PropertyEntity({ eventClick: aprovarIntegracaoClick, type: types.event, text: "Aprovar Integração", visible: ko.observable(false) });

    this.BuscarInfracoes = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridInfracao() {
    var editarRegistro = {
        descricao: "Editar",
        id: guid(),
        evento: "onclick",
        metodo: editarInfracaoClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editarRegistro]
    };

    var configuracaoExportacao = {
        url: "Infracao/ExportarPesquisa",
        titulo: "Infração"
    };

    _gridInfracao = new GridViewExportacao(_pesquisaInfracao.Pesquisar.idGrid, "Infracao/Pesquisa", _pesquisaInfracao, menuOpcoes, configuracaoExportacao, null, null, null, null, null);
    _gridInfracao.CarregarGrid();
}

function loadInfracao() {
    _infracao = new Infracao();
    HeaderAuditoria("Infracao", _infracao);

    _pesquisaInfracao = new PesquisaInfracao();
    KoBindings(_pesquisaInfracao, "knockoutPesquisaInfracao", false, _pesquisaInfracao.Pesquisar.id);

    _CRUDInfracao = new CRUDInfracao();
    KoBindings(_CRUDInfracao, "knockoutCRUDCadastroInfracao");

    new BuscarLocalidades(_pesquisaInfracao.Cidade);
    new BuscarTipoInfracao(_pesquisaInfracao.TipoInfracao);
    new BuscarClientes(_pesquisaInfracao.OrgaoEmissor);
    new BuscarFuncionario(_pesquisaInfracao.Funcionario);
    new BuscarMotoristas(_pesquisaInfracao.Motorista, null, null, null, null, EnumSituacaoColaborador.Todos);
    new BuscarVeiculos(_pesquisaInfracao.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    loadEtapaInfracao();
    loadResumoInfracao();
    loadDadosInfracao();
    loadProcessamentoInfracao();
    loadHistoricoInfracao();
    loadAprovacaoInfracao();
    loadComissaoMotorista();
    loadEmpresaInfracao();

    setarEtapaInicialInfracao();
    loadInfracaoDuplicar();
    controlarComponentesHabilitados();
    loadGridInfracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    switch (_abaAtiva) {
        case EnumAbaInfracao.Infracao:
            adicionarDadosInfracao();
            break;

        case EnumAbaInfracao.Processamento:
            adicionarProcessamentoInfracao();
            break;
    }
}

function buscarDadosOutraOcorrencia() {
    exibirConfirmacao("Confirmação", "Realmente deseja duplicar a infração?", function () {
        executarReST("Infracao/EstornarPorCodigo", {}, function (arg) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Estornado com sucesso");

                _gridInfracao.CarregarGrid();

                limparCamposInfracao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function EstornarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar a infração?", function () {
        executarReST("Infracao/EstornarPorCodigo", { Codigo: _infracao.Codigo.val() }, function (arg) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Estornado com sucesso");

                _gridInfracao.CarregarGrid();

                limparCamposInfracao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick() {
    if (_abaAtiva === EnumAbaInfracao.Processamento || _infracao.Situacao.val() === EnumSituacaoInfracao.AprovacaoRejeitada) {
        exibirConfirmacao("Confirmação", "Realmente deseja cancelar a infração?", function () {
            executarReST("Infracao/CancelarPorCodigo", { Codigo: _infracao.Codigo.val() }, function (arg) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");

                    _gridInfracao.CarregarGrid();

                    limparCamposInfracao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            }, null);
        });
    }
}

function baixarArquivoIntegracaoClick() {
    executarDownload("Infracao/DownloadArquivoIntegracao", { Codigo: _infracao.Codigo.val() });
}

function editarInfracaoClick(infracaoSelecionada) {
    _pesquisaInfracao.ExibirFiltros.visibleFade(false);

    editarInfracao(infracaoSelecionada);
}

function imprimirReciboClick() {
    if (_abaAtiva == EnumAbaInfracao.Aprovacao)
        imprimirRecibo();
}

function limparClick() {
    limparCamposInfracao();
}

function reprocessarRegrasClick() {
    executarReST("Infracao/ReprocessarRegras", { Codigo: _infracao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                buscarInfracaoPorCodigo(_infracao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a infração.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function aprovarIntegracaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja aprovar a integração?", function () {

        preecherDadosInfracao();
        executarReST("Infracao/AprovarIntegracao", RetornarObjetoPesquisa(_dadosInfracao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração atualizada e enviada para processamento.");

                    _gridInfracao.CarregarGrid();

                    limparCamposInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Falha", "Ocorreu uma falha ao enviar pra processamento.");
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

/*
 * Declaração das Funções
 */

function buscarInfracaoPorCodigo(codigo) {
    executarReST("Infracao/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            limparCamposInfracao();
            preencherInfracao(retorno.Data);
            setarEtapasInfracao();
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function editarInfracao(infracao) {
    buscarInfracaoPorCodigo(infracao.Codigo);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function controlarBotoesHabilitados() {
    var botaoAdicionarVisivel = false;
    var botaoAtualizarVisivel = false;
    var botaoCancelarVisivel = false;
    var botaoEstornarVisivel = false;
    var botaoImprimirReciboVisivel = false;
    var botaoLimparVisivel = true;
    var botaoReprocessarRegrasVisivel = false;
    var botaoAprovarIntegracaoVisivel = false;
    var textoBotaoAdicionar = "Adicionar";

    switch (_infracao.Situacao.val()) {
        case EnumSituacaoInfracao.AguardandoProcessamento:
            if (_abaAtiva === EnumAbaInfracao.Processamento) {
                botaoAdicionarVisivel = true;
                botaoCancelarVisivel = true;
                textoBotaoAdicionar = "Processar";
            } else
                botaoAtualizarVisivel = true;
            break;

        case EnumSituacaoInfracao.AprovacaoRejeitada:
            botaoCancelarVisivel = true;
            break;

        case EnumSituacaoInfracao.Todas:
            textoBotaoAdicionar = "Gerar";
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoInfracao.SemRegraAprovacao:
            if (_abaAtiva === EnumAbaInfracao.Aprovacao) {
                botaoReprocessarRegrasVisivel = true;
            }
            break;

        case EnumSituacaoInfracao.Finalizada:
            if (_abaAtiva === EnumAbaInfracao.Aprovacao) {
                botaoImprimirReciboVisivel = true;
            }
            if (_abaAtiva === EnumAbaInfracao.Empresa) {
                botaoEstornarVisivel = false;
            }
            else
                botaoEstornarVisivel = true;
            break;

        case EnumSituacaoInfracao.AguardandoConfirmacao:
            if (_abaAtiva === EnumAbaInfracao.Infracao) {
                botaoAprovarIntegracaoVisivel = true;
            }
            break;
    }

    _CRUDInfracao.Adicionar.text(textoBotaoAdicionar);
    _CRUDInfracao.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDInfracao.Estornar.visible(botaoEstornarVisivel);
    _CRUDInfracao.Cancelar.visible(botaoCancelarVisivel);
    _CRUDInfracao.ImprimirRecibo.visible(botaoImprimirReciboVisivel);
    _CRUDInfracao.Limpar.visible(botaoLimparVisivel);
    _CRUDInfracao.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
    _CRUDInfracao.Atualizar.visible(botaoAtualizarVisivel);
    _CRUDInfracao.BaixarArquivoIntegracao.visible(_infracao.OrigemIntegracao.val());
    _CRUDInfracao.AprovarIntegracao.visible(botaoAprovarIntegracaoVisivel);
}

function ControlarCamposHabilitados() {
    controlarCamposDadosInfracaoHabilitados();
    controlarCamposProcessamentoInfracao();
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function imprimirRecibo() {
    if (_CONFIGURACAO_TMS.TipoRecibo == EnumTipoRecibo.Padrao) {
        executarReST("Infracao/ImprimirRecibo", { Codigo: _infracao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Impressão do recibo solicitada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    } else
        executarDownload("Infracao/DownloadReciboCompleto", { Codigo: _infracao.Codigo.val() });
}

function limparCamposInfracao() {
    LimparCampos(_infracao);
    limparResumoInfracao();
    limparCamposDadosInfracao();
    limparCamposDadosSinistro();
    limparCamposProcessamentoInfracao();
    limparHistoricoInfracao();
    limparCamposAprovacao();
    limparCamposEmpresaInfracao();
    setarEtapaInicialInfracao();
    controlarComponentesHabilitados();
    limparAnexosInfracao();

    Global.ResetarAbas();
}

function preencherInfracao(dados) {
    _infracao.Codigo.val(dados.Codigo);
    _infracao.Situacao.val(dados.Situacao);
    _infracao.FaturadoTitulosEmpresa.val(dados.FaturadoTitulosEmpresa);
    _infracao.OrigemIntegracao.val(dados.OrigemIntegracao);

    preencherAnexosInfracao(dados.Anexos);
    preencherResumoInfracao(dados.Resumo);
    preencherDadosInfracao(dados.DadosInfracao);
    preencherDadosSinistro(dados.DadosSinistro);
    preencherProcessamentoInfracao(dados.ProcessamentoInfracao, dados.DadosComissaoMotorista);
    preencherHistoricoInfracao(dados.HistoricoInfracao);
    preencherAprovacao(dados.ResumoAprovacao, dados.Codigo, dados.Situacao);
    preencherEmpresaInfracao(dados.TitulosEmpresa);
}
