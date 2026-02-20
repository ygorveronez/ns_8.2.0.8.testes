/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesFrete;
var _gridIntegracoesCarga;
var _cargaFreteIntegracao;
var _cargaIntegracao;
var _pesquisaCargaFreteIntegracoes;
var _pesquisaCargaIntegracoes;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;
var _codigoDadosEmisao;
var _codigoDadosEmisaoCarga;
var _liberacaoSemIntegracao;
var _htmlIntegracao;
var _htmlIntegracaoCarga;
/*
 * Declaração das Classes
 */
function PesquisaHistoricoIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaCargaFreteIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaIntegracaoOutros.Todos), options: EnumSituacaoCargaIntegracaoOutros.ObterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoCargaIntegracaoOutros.Todos });

    this.ReenviarTodos = PropertyEntity({
        eventClick: LiberarSemIntegracaoFrete, type: types.event, text: "Liberar Sem Integração", idGrid: guid(), visible: ko.observable(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete, _PermissoesPersonalizadasCarga)), visible: ko.observable(false)
    });
    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesCargaFrete, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};
function PesquisaCargaIntegracoes() {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), def: EnumSituacaoIntegracaoCarga.Todas, issue: 272 });

    this.ReenviarTodos = PropertyEntity({
        eventClick: LiberarSemIntegracaoFrete, type: types.event, text: "Liberar Sem Integração", idGrid: guid(), visible: ko.observable(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete, _PermissoesPersonalizadasCarga)), visible: ko.observable(false)
    });
    this.Pesquisar = PropertyEntity({ eventClick: () => _gridIntegracoesCarga.CarregarGrid(), type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};


function CargaFreteIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoCargaFrete, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

function CargaIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoCarga, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCargaFreteIntegracoes(etapaNota) {
    var linhasPorPaginas = 5;
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: reenviarIntegracoesCargaFreteClick, icone: "" };
    var downloadAquivos = { descricao: "Download", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoCargaFreteClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [reenviar, historico, downloadAquivos] }; //,opcaoProblemaIntegracao

    _gridIntegracoesFrete = new GridView(_pesquisaCargaFreteIntegracoes.Pesquisar.idGrid, "CargaFrete/PesquisaCargaFreteIntegracoes", _pesquisaCargaFreteIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesFrete.CarregarGrid();
    carregarTotaisIntegracaoCargaFrete(etapaNota);
}

function loadGridCargaIntegracoes() {
    var linhasPorPaginas = 5;
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: reenviarIntegracoesCargaFreteClick, icone: "" };
    var downloadAquivos = { descricao: "Download", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoCargaFreteClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [reenviar, historico, downloadAquivos] }; //,opcaoProblemaIntegracao

    _gridIntegracoesCarga = new GridView(_pesquisaCargaIntegracoes.Pesquisar.idGrid, "CargaIntegracaoCarga/Pesquisa", _pesquisaCargaIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesCarga.CarregarGrid();
    carregarTotaisIntegracaoCarga();
}


function loadDadosIntegracaoCargaFreteIntegracao(etapaNota) {
    CarregarHTMLIntegracao().then(function () {

        let id = "";
        let idIntegracaoCarga = "";

        if ((_cargaAtual.TipoOperacao.TipoConsolidacaoPrechekin || _cargaAtual.TipoOperacao.TipoMilkrun) && etapaNota) {
            id = "#tabIntegracaoFrete1_";
            idIntegracaoCarga = "#tabIntegracoesCargaNotas_";
            _codigoDadosEmisao = _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";
            document.getElementById("liTabIntegracaoFrete_" + _codigoDadosEmisao).style.display = "block";
            _htmlIntegracao = `<div class="card">
                <div class="card-header">
                Integrações
                </div>
                <div class="card-body">
                    ${_htmlIntegracao}
                  </div>
            </div>
            <div class="card my-3">
                <div class="card-header">
                Integrações
                </div>
                <div class="card-body">
                    <div id="tabIntegracoesCargaNotas" >
                    </div>
                  </div>
            </div>`
        }
        else {
            id = "#tabIntegracaoFrete_";
            _codigoDadosEmisao = _cargaAtual.DadosEmissaoFrete.id;
        }

        //_codigoDadosEmisao = _cargaAtual.ExigeNotaFiscalParaCalcularFrete.val() ? _cargaAtual.EtapaNotaFiscal.idGrid : _cargaAtual.DadosEmissaoFrete.id;//cargas do consolidado sao diferentes fluxos.

        $(id + _codigoDadosEmisao).removeClass("active");

        _htmlIntegracao = _htmlIntegracao.replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracao_" + _codigoDadosEmisao);
        _htmlIntegracao = _htmlIntegracao.replace("knockoutDadosIntegracao", "knockoutDadosIntegracao_" + _codigoDadosEmisao);

        $(id + _codigoDadosEmisao).html(_htmlIntegracao);

        LocalizeCurrentPage();

        let codigoCarga = _cargaAtual.Codigo.val();

        _cargaFreteIntegracao = new CargaFreteIntegracao();
        KoBindings(_cargaFreteIntegracao, "knockoutDadosIntegracao_" + _codigoDadosEmisao);

        _pesquisaCargaFreteIntegracoes = new PesquisaCargaFreteIntegracoes();
        KoBindings(_pesquisaCargaFreteIntegracoes, "knockoutPesquisaIntegracao_" + _codigoDadosEmisao, false, _pesquisaCargaFreteIntegracoes.Pesquisar.id);

        _pesquisaCargaFreteIntegracoes.Codigo.val(codigoCarga);

        if (idIntegracaoCarga != '' && !_cargaAtual.TipoOperacao.TipoMilkrun) {
            _htmlIntegracaoCarga = _htmlIntegracaoCarga.replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracaoCarga_" + _codigoDadosEmisao);
            _htmlIntegracaoCarga = _htmlIntegracaoCarga.replace("knockoutDadosIntegracao", "knockoutDadosIntegracaoCarga_" + _codigoDadosEmisao);
            $("#tabIntegracoesCargaNotas").html(_htmlIntegracaoCarga);

            _cargaIntegracao = new CargaIntegracao();
            KoBindings(_cargaIntegracao, "knockoutDadosIntegracaoCarga_" + _codigoDadosEmisao);

            _pesquisaCargaIntegracoes = new PesquisaCargaIntegracoes();
            KoBindings(_pesquisaCargaIntegracoes, "knockoutPesquisaIntegracaoCarga_" + _codigoDadosEmisao, false, _pesquisaCargaIntegracoes.Pesquisar.id);
            _pesquisaCargaIntegracoes.Carga.val(codigoCarga);
            loadGridCargaIntegracoes()
        }

        loadGridCargaFreteIntegracoes(etapaNota);
    });
}


function CarregarHTMLIntegracao() {
    var p = new promise.Promise();
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (data) {
        _htmlIntegracao = data;
        _htmlIntegracaoCarga = data;
        p.done();
    });

    return p;
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracaoCargaFrete(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCargaFrete");
}

function reenviarIntegracoesCargaFreteClick(e) {
    executarReST("CargaFrete/ReenviarIntegracaoCargaFrete", { Codigo: e.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesCargaFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoCargaFreteClick(integraco) {
    executarDownload("CargaFrete/DownloadArquivosHistoricoIntegracao", { Codigo: integraco.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarCargaFreteIntegracoes() {
    if (_pesquisaCargaFreteIntegracoes == null)
        return;

    _pesquisaCargaFreteIntegracoes.Codigo.val(_carga.Codigo.val());
    carregarIntegracoesCargaFrete();

}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracaoCargaFrete(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCargaFreteClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracaoCargaFreteIntegracao", "CargaFrete/ConsultarHistoricoIntegracaoCargaFrete", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoesCargaFrete() {
    _gridIntegracoesFrete.CarregarGrid();
    carregarTotaisIntegracaoCargaFrete();
}

function carregarTotaisIntegracaoCargaFrete(etapaNota) {
    executarReST("CargaFrete/ObterTotaisIntegracoesCargaFrete", { Codigo: _pesquisaCargaFreteIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _cargaFreteIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _cargaFreteIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _cargaFreteIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _cargaFreteIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _cargaFreteIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
            _pesquisaCargaFreteIntegracoes.ReenviarTodos.visible(retorno.Data.TotalProblemaIntegracao > 0);

            if ((_cargaAtual.TipoOperacao.TipoConsolidacaoPrechekin || _cargaAtual.TipoOperacao.TipoMilkrun) && retorno.Data.TotalGeral > 0 && etapaNota)
                $("#tabIntegracaoFrete1_" + _codigoDadosEmisao + "_li").show();
            else if (retorno.Data.TotalGeral > 0)
                $("#tabIntegracaoFrete_" + _codigoDadosEmisao + "_li").show();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}
function carregarTotaisIntegracaoCarga() {
    executarReST("CargaIntegracaoCarga/ObterTotais", { Carga: _pesquisaCargaIntegracoes.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            _cargaIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _cargaIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _cargaIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _cargaIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _cargaIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
            _pesquisaCargaIntegracoes.ReenviarTodos.visible(retorno.Data.TotalProblemaIntegracao > 0);


        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}


function LiberarSemIntegracaoFrete() {
    executarReST("CargaFrete/LiberarSemIntegracaoCargaFrete", { Codigo: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoesCargaFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function VerificarExisteDiv(id, html) {
    if (_cargaAtual.ExigeNotaFiscalParaCalcularFrete.val())
        $(`#liTabIntegracaoFrete_${_cargaAtual.EtapaNotaFiscal.idGrid}_knoutDocumentosParaEmissao`).hide();

    let idIntegracao = _cargaAtual.ExigeNotaFiscalParaCalcularFrete.val() ? `#tabIntegracoesConteudo_${id}` : `#tabIntegracoesConteudo1_${id}_knoutDocumentosParaEmissao`;
    html = html.replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracao_" + id);
    html = html.replace("knockoutDadosIntegracao", "knockoutDadosIntegracao_" + id);

    $(idIntegracao).html(html);
}