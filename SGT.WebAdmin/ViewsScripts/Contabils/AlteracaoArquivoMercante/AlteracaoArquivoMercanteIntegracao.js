var _gridAlteracaoArquivoMercanteIntegracao;
var _gridAlteracaoArquivoMercanteIntegracaoPortal;
var _alteracaoArquivoMercanteIntegracao;
var _alteracaoArquivoMercanteIntegracaoPortal;
var _pesquisaAlteracaoArquivoMercanteIntegracao;
var _pesquisaAlteracaoArquivoMercanteIntegracaoPortal;
var _problemaAlteracaoArquivoMercanteIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Historico = PropertyEntity({ idGrid: guid()});
};

function AlteracaoArquivoMercanteIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na integração:" });
    ;
    this.ObterTotais = PropertyEntity({ eventClick: () => carregarTotaisIntegracao(new EnumTipoIntegracaoHelper().Intercab), type: types.event, text: ko.observable("Obter totais"), idGrid: guid(), visible: ko.observable(true) });
};

function AlteracaoArquivoMercanteIntegracaoPortal() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: () => carregarTotaisIntegracao(new EnumTipoIntegracaoHelper().EMP), type: types.event, text: ko.observable("Obter totais"), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaAlteracaoArquivoMercanteIntegracao() {
    const tipoIntegracaoIntercab = new EnumTipoIntegracaoHelper().Intercab;
    
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(tipoIntegracaoIntercab), getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({ eventClick: () => carregarIntegracoes(tipoIntegracaoIntercab), type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: () => reenviarTodasIntegracoesClick(tipoIntegracaoIntercab), type: types.event, text: ko.observable("Reenviar todas"), visible: ko.observable(false) });
};

function PesquisaAlteracaoArquivoMercanteIntegracaoPortal() {
    const tipoIntegracaoEMP = new EnumTipoIntegracaoHelper().EMP;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(tipoIntegracaoEMP), getType: typesKnockout.int });
    
    this.Pesquisar = PropertyEntity({ eventClick: () => carregarIntegracoes(tipoIntegracaoEMP), type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: () => reenviarTodasIntegracoesClick(tipoIntegracaoEMP), type: types.event, text: ko.observable("Reenviar todas"), visible: ko.observable(false) });
};

function ProblemaAlteracaoArquivoMercanteIntegracao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable("Motivo"), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function ProblemaAlteracaoArquivoMercanteIntegracaoPortal() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable("Motivo"), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProblemaIntegracaoPortalClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function loadGridAlteracaoArquivoMercanteIntegracao(enumTipoIntegracao) {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarClick, icone: "" };

    var historico = { descricao: "Histórico de integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] }; // opcaoProblemaIntegracao

    if (enumTipoIntegracao == new EnumTipoIntegracaoHelper().Intercab) {//_gridAlteracaoArquivoMercanteIntegracao === undefined && _pesquisaAlteracaoArquivoMercanteIntegracao !== undefined
        _gridAlteracaoArquivoMercanteIntegracao = new GridView(_pesquisaAlteracaoArquivoMercanteIntegracao.Pesquisar.idGrid, "AlteracaoArquivoMercanteIntegracao/PesquisaIntegracao", _pesquisaAlteracaoArquivoMercanteIntegracao, menuOpcoes, null, linhasPorPaginas);
        _gridAlteracaoArquivoMercanteIntegracao.CarregarGrid();
    }

    if (enumTipoIntegracao == new EnumTipoIntegracaoHelper().EMP) {//_gridAlteracaoArquivoMercanteIntegracaoPortal === undefined && _pesquisaAlteracaoArquivoMercanteIntegracaoPortal !== undefined
        _gridAlteracaoArquivoMercanteIntegracaoPortal = new GridView(_pesquisaAlteracaoArquivoMercanteIntegracaoPortal.Pesquisar.idGrid, "AlteracaoArquivoMercanteIntegracao/PesquisaIntegracao", _pesquisaAlteracaoArquivoMercanteIntegracaoPortal, menuOpcoes, null, linhasPorPaginas);
        _gridAlteracaoArquivoMercanteIntegracaoPortal.CarregarGrid();
    }
}

function loadAlteracaoArquivoMercanteIntegracao() {

    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesIntercabConteudo").append(html);

        LocalizeCurrentPage();

        _alteracaoArquivoMercanteIntegracao = new AlteracaoArquivoMercanteIntegracao();
        KoBindings(_alteracaoArquivoMercanteIntegracao, "knockoutDadosIntegracao");

        _pesquisaAlteracaoArquivoMercanteIntegracao = new PesquisaAlteracaoArquivoMercanteIntegracao();
        KoBindings(_pesquisaAlteracaoArquivoMercanteIntegracao, "knockoutPesquisaIntegracao", false, _pesquisaAlteracaoArquivoMercanteIntegracao.Pesquisar.id);

        _problemaAlteracaoArquivoMercanteIntegracao = new ProblemaAlteracaoArquivoMercanteIntegracao();
        KoBindings(_problemaAlteracaoArquivoMercanteIntegracao, "knockoutMotivoProblemaIntegracao");

        loadGridAlteracaoArquivoMercanteIntegracao(new EnumTipoIntegracaoHelper().Intercab);
    });

    $.get("Content/Static/Integracao/IntegracaoPortal.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesPortalConteudo").append(html);

        LocalizeCurrentPage();

        _alteracaoArquivoMercanteIntegracaoPortal = new AlteracaoArquivoMercanteIntegracaoPortal();
        KoBindings(_alteracaoArquivoMercanteIntegracaoPortal, "knockoutDadosIntegracaoPortal");

        _pesquisaAlteracaoArquivoMercanteIntegracaoPortal = new PesquisaAlteracaoArquivoMercanteIntegracaoPortal();
        KoBindings(_pesquisaAlteracaoArquivoMercanteIntegracaoPortal, "knockoutPesquisaIntegracaoPortal", false, _pesquisaAlteracaoArquivoMercanteIntegracaoPortal.Pesquisar.id);

        loadGridAlteracaoArquivoMercanteIntegracao(new EnumTipoIntegracaoHelper().EMP);
    });

    carregarTotaisIntegracao(new EnumTipoIntegracaoHelper().Intercab);
    carregarTotaisIntegracao(new EnumTipoIntegracaoHelper().EMP);
}

/******* EVENTOS ******/

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function adicionarProblemaIntegracaoClick() {
    if (ValidarCamposObrigatorios(_problemaAlteracaoArquivoMercanteIntegracao)) {
        var dadosProblemaIntegracao = RetornarObjetoPesquisa(_problemaAlteracaoArquivoMercanteIntegracao);

        executarReST("AlteracaoArquivoMercanteIntegracao/ProblemaIntegracao", dadosProblemaIntegracao, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalProblemaIntegracao();
                    carregarIntegracoes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function exibirModalProblemaIntegracaoClick(registroSelecionado) {
    _problemaAlteracaoArquivoMercanteIntegracao.Codigo.val(registroSelecionado.Codigo);

    Global.abrirModal('divModalMotivoProblemaIntegracao');
    $("#divModalMotivoProblemaIntegracao").one('hidden.bs.modal', function () {
        LimparCampos(_problemaAlteracaoArquivoMercanteIntegracao);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("AlteracaoArquivoMercanteIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("AlteracaoArquivoMercanteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

/****** MÉTODOS ******/
function recarregarAlteracaoArquivoMercanteIntegracao() {
    if (_pesquisaAlteracaoArquivoMercanteIntegracao !== undefined)
        _pesquisaAlteracaoArquivoMercanteIntegracao.Codigo.val(_arquivoMercante.Codigo.val());

    if (_pesquisaAlteracaoArquivoMercanteIntegracaoPortal !== undefined)
        _pesquisaAlteracaoArquivoMercanteIntegracaoPortal.Codigo.val(_arquivoMercante.Codigo.val());

    controlarExibicaoAbaIntegracoes();
    carregarIntegracoes();
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    KoBindings("KnockoutHistoricoIntegracao")
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView(_pesquisaHistoricoIntegracao.Historico.idGrid, "AlteracaoArquivoMercanteIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes(enumTipoIntegracao) {
    const tipoIntercab = new EnumTipoIntegracaoHelper().Intercab;
    const tipoPortal = new EnumTipoIntegracaoHelper().EMP;

    if (enumTipoIntegracao == undefined || enumTipoIntegracao == tipoIntercab) {
        if (_gridAlteracaoArquivoMercanteIntegracao !== undefined)
            _gridAlteracaoArquivoMercanteIntegracao.CarregarGrid();

        carregarTotaisIntegracao(tipoIntercab);
    }
    if (enumTipoIntegracao == undefined || enumTipoIntegracao == tipoPortal) {
        if (_gridAlteracaoArquivoMercanteIntegracaoPortal !== undefined)
            _gridAlteracaoArquivoMercanteIntegracaoPortal.CarregarGrid();

        carregarTotaisIntegracao(tipoPortal);
    }
}

function carregarTotaisIntegracao(enumTipoIntegracao) {
    let pesquisa = enumTipoIntegracao == new EnumTipoIntegracaoHelper().Intercab ? _pesquisaAlteracaoArquivoMercanteIntegracao : _pesquisaAlteracaoArquivoMercanteIntegracaoPortal;
    let codigo = pesquisa !== undefined ? pesquisa.Codigo.val() : null;
    executarReST("AlteracaoArquivoMercanteIntegracao/ObterTotaisIntegracoes", { Codigo: codigo, TipoIntegracao: enumTipoIntegracao }, function (retorno) {
        if (retorno.Success && retorno.Data != null) {
            let alteracao = enumTipoIntegracao == new EnumTipoIntegracaoHelper().Intercab ? _alteracaoArquivoMercanteIntegracao : _alteracaoArquivoMercanteIntegracaoPortal;

            if (alteracao != null && alteracao.TotalGeral != null && alteracao.TotalGeral != undefined) {
                alteracao.TotalGeral.val(retorno.Data.TotalGeral);
                alteracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
                alteracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
                alteracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
                alteracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}


function controlarAbaIntegracao() {
    executarReST("Integracao/ObterDados", null, function (arg) {
        if (arg.Success) {
            let data = arg.Data;
            data.IntegracaoIntercab.AtivarIntegracaoMercante || data.IntegracaoEMP.AtivarIntegracaoCEMercanteEMP ? $("#liTabIntegracoes").show() : $("#liTabIntegracoes").hide();
            data.IntegracaoIntercab.AtivarIntegracaoMercante ? $("#liIntegraIntercab").show() : $("#liIntegraIntercab").hide();
            data.IntegracaoEMP.AtivarIntegracaoCEMercanteEMP ? $("#liIntegraPortal").show() : $("#liIntegraPortal").hide();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
        }
    });
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal('divModalMotivoProblemaIntegracao');
}

function limparAlteracaoArquivoMercanteIntegracao() {
    LimparCampos(_pesquisaAlteracaoArquivoMercanteIntegracao);
    LimparCampos(_pesquisaAlteracaoArquivoMercanteIntegracaoPortal);
    LimparCampos(_alteracaoArquivoMercanteIntegracao);
    LimparCampos(_alteracaoArquivoMercanteIntegracaoPortal);
    LimparCampos(_problemaAlteracaoArquivoMercanteIntegracao);
    recarregarAlteracaoArquivoMercanteIntegracao();
}
