/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoManobraTracao.js" />
/// <reference path="ManobraTracaoDetalhes.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _manobraTracao;
var _manobraTracaoCadastro;

/*
 * Declaração das Classes
 */

var ManobraTracao = function () {
    this.ListaManobraTracao = ko.observableArray();

    this.Adicionar = PropertyEntity({ eventClick: adicionarManobraTracaoModalClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var ManobraTracaoCadastro = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), required: false });
    this.Tracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tração:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarManobraTracaoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var ManobraTracaoDados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AcaoAtual = PropertyEntity({ text: "Ação Atual:" });
    this.ClasseCor = PropertyEntity({ val: ko.observable("well-white") });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação:" });
    this.Motorista = PropertyEntity({ text: "Motorista:" });
    this.Placa = PropertyEntity({});
    this.Situacao = PropertyEntity({});
    this.Tracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ text: "Transportador:" });
}

ManobraTracaoDados.prototype = {
    atualizarDados: function (dadosManobraTracao) {
        this._removerEventos();
        this.carregarDados(dadosManobraTracao);
    },
    carregarDados: function (dadosManobraTracao) {
        PreencherObjetoKnout(this, { Data: dadosManobraTracao });

        this._adicionarEventos();
    },
    _adicionarEventoClick: function () {
        var self = this;

        $("#manobra-tracao-" + self.Codigo.val()).bind("click", function () { self._exibirDetalhes(); });
    },
    _adicionarEventoDraggable: function () {
        var self = this;

        if (self._isSituacaoPermiteAdicionarEventoDraggable()) {
            $("#manobra-tracao-" + self.Codigo.val()).draggable({
                helper: function (event) {
                    return '<div style="width: ' + $(event.currentTarget).width() + 'px;" class="lista-manobra-tracao-item" id="' + event.currentTarget.id + '">' + event.currentTarget.innerHTML + '</div>';
                },
                revert: 'invalid',
                start: function () {
                    if (self._isSituacaoPermiteVincularManobra()) {
                        $("#grid-manobra tbody .manobra-tracao-droppable").droppable({
                            drop: function (event) { self._vincularManobra(event.target.id) },
                            hoverClass: "ui-state-active backgroundDropHover",
                        });
                    }

                    $("#remocao-item-manobra-tracao").show();
                },
                stop: function () {
                    try {
                        if (self._isSituacaoPermiteVincularManobra())
                            $("#grid-manobra tbody .manobra-tracao-droppable").droppable("destroy");
                    }
                    catch (e) { }

                    $("#remocao-item-manobra-tracao").hide();
                },
                zIndex: 11,
            });
        }
    },
    _adicionarEventos: function () {
        this._adicionarEventoClick();
        this._adicionarEventoDraggable();
    },
    _exibirDetalhes: function () {
        exibirManobraTracaoDetalhes({ Codigo: this.Codigo.val(), Tracao: this.Tracao.val() });
    },
    _isSituacaoPermiteAdicionarEventoDraggable: function () {
        return (this.Situacao.val() == EnumSituacaoManobraTracao.Ocioso) || (this.Situacao.val() == EnumSituacaoManobraTracao.SemMotorista);
    },
    _isSituacaoPermiteVincularManobra: function () {
        return (this.Situacao.val() == EnumSituacaoManobraTracao.Ocioso);
    },
    _removerEventoClick: function () {
        var self = this;

        $("#manobra-tracao-" + self.Codigo.val()).unbind("click");
    },
    _removerEventoDraggable: function () {
        try {
            if (this._isSituacaoPermiteAdicionarEventoDraggable())
                $("#manobra-tracao-" + this.Codigo.val()).draggable("destroy");
        }
        catch (e) { }
    },
    _removerEventos: function () {
        this._removerEventoClick();
        this._removerEventoDraggable();
    },
    _vincularManobra: function (codigoManobra) {
        var dados = {
            ManobraTracao: this.Codigo.val(),
            Manobra: codigoManobra
        }

        exibirConfirmacao("Confirmação", "Deseja realmente vincular a tração de manobra?", function () {
            executarReST("ManobraTracao/VincularManobra", dados, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Tração de manobra vinculada com sucesso");
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        });
    }
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDroppableManobraTracao() {
    $("#remocao-item-manobra-tracao").droppable({
        accept: '.lista-manobra-tracao-item',
        drop: function (event, ui) {
            removerManobraTracao($(ui.draggable[0]).data("codigo"));
        },
        hoverClass: "remocao-item-container-hover"
    });
}

function loadManobraTracao() {
    _manobraTracao = new ManobraTracao();
    KoBindings(_manobraTracao, "knockoutManobraTracao");

    _manobraTracaoCadastro = new ManobraTracaoCadastro();
    KoBindings(_manobraTracaoCadastro, "knockoutCadastroManobraTracao");

    new BuscarMotoristasMobile(_manobraTracaoCadastro.Motorista);
    new BuscarTracaoManobra(_manobraTracaoCadastro.Tracao);
    
    loadDroppableManobraTracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarManobraTracaoClick() {
    if (ValidarCamposObrigatorios(_manobraTracaoCadastro)) {
        var manobraTracaoCadastrar = {
            CentroCarregamento: _pesquisaManobraAuxiliar.CentroCarregamento.codEntity(),
            Motorista: _manobraTracaoCadastro.Motorista.codEntity(),
            Tracao: _manobraTracaoCadastro.Tracao.codEntity()
        };

        executarReST("ManobraTracao/Adicionar", manobraTracaoCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionada tração para manobra com sucesso");

                    fecharManobraTracaoModal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarManobraTracaoModalClick() {
    if (_pesquisaManobraAuxiliar.CentroCarregamento.codEntity() > 0) {
        Global.abrirModal('divModalCadastroManobraTracao');
        $("#divModalCadastroManobraTracao").one('hidden.bs.modal', function () {
            LimparCampos(_manobraTracaoCadastro);
        });
    }
}

/*
 * Declaração das Funções Públicas
 */

function adicionarOuAtualizarManobraTracaoDados(dadosManobraTracao) {
    if (!atualizarManobraTracaoDados(dadosManobraTracao))
        adicionarManobraTracaoDados(dadosManobraTracao);
}

function removerManobraTracaoDados(codigoManobraTracao) {
    for (var i = 0; i < _manobraTracao.ListaManobraTracao().length; i++) {
        var manobraTracao = _manobraTracao.ListaManobraTracao()[i];

        if (manobraTracao.Codigo.val() == codigoManobraTracao) {
            _manobraTracao.ListaManobraTracao.remove(manobraTracao);
            break;
        }
    }
}

function carregarListaManobraTracao() {
    _manobraTracao.ListaManobraTracao.removeAll();

    executarReST("ManobraTracao/Pesquisa", RetornarObjetoPesquisa(_pesquisaManobraAuxiliar), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                retorno.Data.forEach(function (manobraTracao) {
                    adicionarManobraTracaoDados(manobraTracao);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções
 */

function adicionarManobraTracaoDados(dadosManobraTracao) {
    var manobraTracao = new ManobraTracaoDados();

    _manobraTracao.ListaManobraTracao.push(manobraTracao);

    manobraTracao.carregarDados(dadosManobraTracao);
}

function atualizarManobraTracaoDados(dadosManobraTracao) {
    for (var i = 0; i < _manobraTracao.ListaManobraTracao().length; i++) {
        var manobraTracao = _manobraTracao.ListaManobraTracao()[i];

        if (manobraTracao.Codigo.val() == dadosManobraTracao.Codigo) {
            manobraTracao.atualizarDados(dadosManobraTracao);
            return true;
        }
    }

    return false;
}

function fecharManobraTracaoModal() {
    Global.fecharModal("divModalCadastroManobraTracao");
}

function removerManobraTracao(codigoManobraTracao) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a tração de manobra?", function () {
        executarReST("ManobraTracao/Remover", { Codigo: codigoManobraTracao }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Removida tração de manobra com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}
