/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDocumento.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="./DetalhesDocumento.js" />
/// <reference path="GestaoDocumento.js" />

// #region Objetos Globais do Arquivo

var _detalheGestaoDocumentoAprovacao;
//var _detalheCTeAprovacao;
var _detalhePreCTeAprovacao;
var _ctesAprovacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalheGestaoDocumentoAprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.SituacaoGestaoDocumento = PropertyEntity({ val: ko.observable(EnumSituacaoGestaoDocumento.Inconsistente), options: EnumSituacaoGestaoDocumento.obterOpcoes(), def: EnumSituacaoGestaoDocumento.Inconsistente });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalheGestaoDocumentoAprovacao() {
    _detalheGestaoDocumentoAprovacao = new DetalheGestaoDocumentoAprovacao();

    //_detalheCTeAprovacao = new DetalheDocumento();
    //KoBindings(_detalheCTeAprovacao, "knoutDetalheCTeAprovacao");

    _detalhePreCTeAprovacao = new DetalheDocumento();
    KoBindings(_detalhePreCTeAprovacao, "knoutDetalhePreCTeAprovacao");

    loadRegras();
    loadDelegar();

    _ctesAprovacao = new DetalhesDocumentosCTes();
    KoBindings(_ctesAprovacao, "knockoutDetalhesCTes");
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function atualizarGestaoDocumentoAprovacao() {
    executarReST("GestaoDocumento/BuscarPorCodigo", { Codigo: _detalheGestaoDocumentoAprovacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _detalheGestaoDocumentoAprovacao.SituacaoGestaoDocumento.val(retorno.Data.Detalhes.SituacaoGestaoDocumento);

            controlarComponentesDetalheGestaoDocumentoAprovacao();
            controlarExibicaoAbaDelegar(retorno.Data.Detalhes.SituacaoGestaoDocumento === EnumSituacaoGestaoDocumento.AguardandoAprovacao);
            atualizarGridRegras();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirDetalheGestaoDocumentoAprovacao(registroSelecionado) {
    //LimparCampos(_detalheCTeAprovacao);
    LimparCampos(_detalhePreCTeAprovacao);
    LimparCamposCTesAprovacao();

    executarReST("GestaoDocumento/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _detalheGestaoDocumentoAprovacao.Codigo.val(registroSelecionado.Codigo);
                _detalheGestaoDocumentoAprovacao.Usuario.val(_pesquisaGestaoDocumento.Usuario.codEntity());
                _detalheGestaoDocumentoAprovacao.SituacaoGestaoDocumento.val(retorno.Data.Detalhes.SituacaoGestaoDocumento);

                //PreencherObjetoKnout(_detalheCTeAprovacao, { Data: retorno.Data.CTe });
                //_detalheCTeAprovacao.ComponentesFrete.val(retorno.Data.ComponentesFrete);
                //_detalhePreCTeAprovacao.ComponentesFrete.val(retorno.Data.ComponentesFrete);
                PreencherDetalhesCTes(retorno.Data, _ctesAprovacao);

                controlarComponentesDetalheGestaoDocumentoAprovacao();
                controlarExibicaoAbaDelegar(retorno.Data.Detalhes.SituacaoGestaoDocumento === EnumSituacaoGestaoDocumento.AguardandoAprovacao);
                atualizarGridRegras();

                Global.abrirModal('divModalDetalheGestaoDocumentoAprovacao');

                $("#divModalDetalheGestaoDocumentoAprovacao").one('shown.bs.modal', function () {
                    if (_ctesAprovacao.CTes.val().length > 0) {
                        $(`a[href="#${_ctesAprovacao.CTes.val()[0].IdTab.val()}"]`).tab("show");
                    }
                });

                $("#divModalDetalheGestaoDocumentoAprovacao").one('hidden.bs.modal', function () {
                    limparCamposGestaoDocumentoAprovacao();
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function limparCamposGestaoDocumentoAprovacao() {
    $("#tab-detalhe-gestao-documento-aprovacao a:first").tab("show");

    limparRegras();
}

// #endregion Funções Privadas
