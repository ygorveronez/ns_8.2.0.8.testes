/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../Enumeradores/EnumSituacaoIntegracaoMercadoLivre.js" />

var _detalhesHandLingUnit;
var _gridDetalhesHandLingUnit;

function DetalhesHandLingUnit() {

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.string });
    this.GridDetalhesHandLingUnit = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.SituacaoIntegracaoMercadoLivre = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoMercadoLivre.Todas), options: EnumSituacaoIntegracaoMercadoLivre.obterOpcoesPesquisa(), text: "Situação Integração", def: EnumSituacaoIntegracaoMercadoLivre.Todas, enable: ko.observable(true) });
    this.ExibirApenasDocumentosComMensagemErro = PropertyEntity({ text: "Exibir Apenas Documentos com Mensagem de Erro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            let desconsiderar = { descricao: "Desconsiderar", id: guid(), evento: "onclick", metodo: DesconsiderarDocumentoMercadoLivreClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoIgnorar }

            let menuOpcoes = {
                tipo: TypeOptionMenu.list,
                descricao: Localization.Resources.Gerais.Geral.Opcoes,
                opcoes: [desconsiderar]
            };
            _gridDetalhesHandLingUnit = new GridView(_detalhesHandLingUnit.GridDetalhesHandLingUnit.idGrid, "CargaIntegracaoMercadoLivre/PesquisaDetalhesHandLingUnit", _detalhesHandLingUnit, menuOpcoes, null, 10, null, null, null, null, null, null, null);
            _gridDetalhesHandLingUnit.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

function DetalhesHandlingUnitMercadoLivreClick(obj) {
    CarregarGridDetalhesHandLingUnit(obj);
    Global.abrirModal("divModalDetalhesHandlingUnit");
}

function CarregarGridDetalhesHandLingUnit(obj) {
    _detalhesHandLingUnit = new DetalhesHandLingUnit();
    KoBindings(_detalhesHandLingUnit, "knockoutDetalhesHandlingUnit");

    _detalhesHandLingUnit.Codigo.val(obj.Codigo);
    _detalhesHandLingUnit.SituacaoIntegracao.val(obj.Situacao);

    let desconsiderar = { descricao: "Desconsiderar", id: guid(), evento: "onclick", metodo: DesconsiderarDocumentoMercadoLivreClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoIgnorar }

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        opcoes: [desconsiderar]
    };

    _gridDetalhesHandLingUnit = new GridView(_detalhesHandLingUnit.GridDetalhesHandLingUnit.idGrid, "CargaIntegracaoMercadoLivre/PesquisaDetalhesHandLingUnit", _detalhesHandLingUnit, menuOpcoes, null, 10, null, null, null, null, null, null, null);
    _gridDetalhesHandLingUnit.CarregarGrid();
}

function VisibilidadeOpcaoIgnorar(data) {
    if ((data.Situacao == "Pend. Download" || data.Situacao == "Pend. Processamento") && _detalhesHandLingUnit.SituacaoIntegracao.val() == "Falha") {
        return true;
    }
    else {
        return false;
    }
}

function DesconsiderarDocumentoMercadoLivreClick(obj) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteDesconsiderarODocumentoOMesmoNaoSeraImportado, function () {
        executarReST("CargaIntegracaoMercadoLivre/DesconsiderarDocumentoMercadoLivre", { Codigo: obj.Codigo, CodigoCargaIntegracaoMercadoLivre: obj.CodigoCargaIntegracaoMercadoLivre }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaEFacilitySeraReprocessadoEmInstantes);
                    _gridDetalhesHandLingUnit.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}