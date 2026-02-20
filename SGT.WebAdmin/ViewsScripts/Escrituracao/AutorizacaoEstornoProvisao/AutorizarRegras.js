
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
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarEstornoProvisaoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
    this.Reprocessar = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: reprocessarEstornoProvisaoClick, text: "Reprocessar" });
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
        metodo: AprovarEstornoProvisao
    };

    var opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: rejeitarEstornoProvisao
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    var knoutProvisao = {
        Codigo: _estornoProvisao.Codigo,
        Usuario: _estornoProvisao.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoEstornoProvisao/RegrasAprovacaoController", knoutProvisao, menuOpcoes);
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
    var valorExcedido = parseFloat(_estornoProvisao.ValorProvisao.val()) > 135250;
    console.log(valorExcedido);
    exibirConfirmacao("Confirmação", valorExcedido ? "Conforme controle GFCF C4.3D (GR IR Clearing), para valores acima de EUR 25000.00 (R$ 135.250,00), é obrigatório anexar a aprovação por e-mail do controller Brasil, contendo a referência e o valor de cada documento de forma detalhada." : "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoEstornoProvisao/AprovarMultiplasRegrasController", { Codigo: _estornoProvisao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridEstornoProvisao();
                    atualizarEstornoProvisao()
                    atualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    }, null, valorExcedido ? "Sim já anexei e estou ciente" : null, valorExcedido ? "Não, ainda não anexei" : null);

}

function cancelarAutorizacaoClick() {
    limparRegras();
}

function rejeitarEstornoProvisaoClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o estorno da provisão é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o estorno da provisão?", function () {
        var dados = {
            Codigo: _regra.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoEstornoProvisao/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridEstornoProvisao();
                    atualizarEstornoProvisao()
                    atualizarGridRegras();
                    limparRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function reprocessarEstornoProvisaoClick() {
    executarReST("AutorizacaoEstornoProvisao/Reprocessar", { Codigo: _estornoProvisao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegraReprocessada) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O estorno da provisão foi reprocessado com sucesso.");

                    atualizarGridEstornoProvisao();
                    atualizarEstornoProvisao()
                    atualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada.");
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function AprovarEstornoProvisao(registroSelecionado) {
    var valorExcedido = parseFloat(_estornoProvisao.ValorProvisao.val()) > 135250;
    exibirConfirmacao("Confirmação", valorExcedido ? "Conforme controle GFCF C4.3D (GR IR Clearing), para valores acima de EUR 25000.00 (R$ 135.250,00), é obrigatório anexar a aprovação por e-mail do controller Brasil, contendo a referência e o valor de cada documento de forma detalhada." : "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoEstornoProvisao/AprovarRegras", registroSelecionado, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                    atualizarGridEstornoProvisao();
                    atualizarEstornoProvisao()
                    atualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }, null, valorExcedido ? "Sim já anexei e estou ciente" : null, valorExcedido ? "Não, ainda não anexei" : null);

}

function atualizarGridRegras() {
    _gridRegras.CarregarGrid(function (retorno) {
        var exibirBotaoAprovarTodas = false;
        var exibirBotaoReprocessar = true;

        retorno.data.forEach(function (autorizacao) {
            exibirBotaoReprocessar = false;

            if (autorizacao.PodeAprovar)
                exibirBotaoAprovarTodas = true;
        });

        _autorizacao.AprovarTodas.visible(exibirBotaoAprovarTodas);
        _autorizacao.Reprocessar.visible(exibirBotaoReprocessar);
    });
}

function atualizarEstornoProvisao() {
    BuscarPorCodigo(_estornoProvisao, "AutorizacaoEstornoProvisao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarExibicaoAbaDelegar(retorno.Data.SituacaoEstornoProvisaoSolicitacao === EnumSituacaoAprovacao.AguardandoAprovacao);
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function rejeitarEstornoProvisao(registroSelecionado) {
    _regra.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function limparRegras() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}
