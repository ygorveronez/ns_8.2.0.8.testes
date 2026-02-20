/// <reference path="AutorizacaoPagamentoEletronico.js" />
/// <reference path="Titulos.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _autorizacao;
var _gridRegras;
var _regra;

/*
 * Declaração das Classes
 */

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentoEletronicoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

var Regra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRegras() {
    var opcaoAprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarPagamentoEletronico
    };

    var opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: rejeitarPagamentoEletronico
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    var knoutPagamentoEletronico = {
        Codigo: _pagamentoEletronico.Codigo,
        Usuario: _pagamentoEletronico.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoPagamentoEletronico/RegrasAprovacao", knoutPagamentoEletronico, menuOpcoes);
}

function loadRegras() {
    _regra = new Regra();

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    loadGridRegras();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoPagamentoEletronico/AprovarMultiplasRegras", { Codigo: _pagamentoEletronico.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");

                        //setTimeout(function () {
                        var data = {
                            Codigo: _pagamentoEletronico.Codigo.val()
                        };
                        executarDownload("PagamentoDigital/DownloadArquivoRemessa", data);
                        //}, 5000);

                        //setTimeout(function () {
                        //    executarDownload("PagamentoDigital/DownloadRelatorioRemessa", data);
                        //}, 5000);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridPagamentosEletronico();
                    atualizarPagamentoEletronico()
                    atualizarGridRegras();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

function cancelarAutorizacaoClick() {
    limparRegras();
}

function rejeitarPagamentoEletronicoClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o Pagamento Eletrônico é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o Pagamento Eletrônico?", function () {
        var dados = {
            Codigo: _regra.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoPagamentoEletronico/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridPagamentosEletronico();
                    atualizarPagamentoEletronico()
                    atualizarGridRegras();
                    limparRegras();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        });
    });
}

/*
 * Declaração das Funções
 */

function AprovarPagamentoEletronico(registroSelecionado) {
    executarReST("AutorizacaoPagamentoEletronico/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                //setTimeout(function () {
                var data = {
                    Codigo: _pagamentoEletronico.Codigo.val()
                };
                executarDownload("PagamentoDigital/DownloadArquivoRemessa", data);
                //}, 3000);
                //setTimeout(function () {
                //    executarDownload("PagamentoDigital/DownloadRelatorioRemessa", data);
                //}, 5000);

                atualizarGridPagamentosEletronico();
                atualizarPagamentoEletronico()
                atualizarGridRegras();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function atualizarGridRegras() {
    _gridRegras.CarregarGrid(function (retorno) {
        var exibirBotaoAprovarTodas = false;

        retorno.data.forEach(function (autorizacao) {
            if (autorizacao.PodeAprovar)
                exibirBotaoAprovarTodas = true;
        });

        _autorizacao.AprovarTodas.visible(exibirBotaoAprovarTodas);
    });
}

function atualizarPagamentoEletronico() {
    BuscarPorCodigo(_pagamentoEletronico, "AutorizacaoPagamentoEletronico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao);
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function rejeitarPagamentoEletronico(registroSelecionado) {
    _regra.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function limparRegras() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}
