/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoNumeroPalletsTabelaFrete.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoIntegracaoAvon, _configuracaoIntegracaoAvon;

var ConfiguracaoIntegracaoAvon = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnterpriseID = PropertyEntity({ text: "*Enterprise ID:", maxlength: 14, visible: ko.observable(true), required: true });
    this.TokenHomologacao = PropertyEntity({ text: "*Token Homologação:", maxlength: 50, visible: ko.observable(true), required: true });
    this.TokenProducao = PropertyEntity({ text: "*Token Produção:", maxlength: 50, visible: ko.observable(true), required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoIntegracaoAvonClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadConfiguracaoIntegracaoAvon() {
    $("#knockoutConfiguracaoIntegracao").append($("#knockoutConfiguracaoIntegracaoAvon"));
    
    _configuracaoIntegracaoAvon = new ConfiguracaoIntegracaoAvon();
    KoBindings(_configuracaoIntegracaoAvon, "knockoutConfiguracaoIntegracaoAvon");

    new BuscarTransportadores(_configuracaoIntegracaoAvon.Empresa);

    ConfigurarGridConfiguracaoIntegracaoAvon();
}

function ConfigurarGridConfiguracaoIntegracaoAvon() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirConfiguracaoIntegracaoAvonClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Empresa", title: "Empresa/Filial", width: "30%" },
        { data: "EnterpriseID", title: "Enterprise ID", width: "20%" },
        { data: "TokenHomologacao", title: "Token Homologação", width: "20%" },
        { data: "TokenProducao", title: "Token Produção", width: "20%" }
    ];

    _gridConfiguracaoIntegracaoAvon = new BasicDataTable(_configuracaoIntegracaoAvon.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoIntegracaoAvon();
}

function RecarregarGridConfiguracaoIntegracaoAvon() {

    var data = new Array();

    $.each(_configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val(), function (i, configuracao) {
        var configGrid = new Object();

        configGrid.Codigo = configuracao.Codigo;
        configGrid.TokenHomologacao = configuracao.TokenHomologacao;
        configGrid.TokenProducao = configuracao.TokenProducao;
        configGrid.EnterpriseID = configuracao.EnterpriseID;
        configGrid.Empresa = configuracao.Empresa.Descricao;

        data.push(configGrid);
    });

    _gridConfiguracaoIntegracaoAvon.CarregarGrid(data);
}


function ExcluirConfiguracaoIntegracaoAvonClick(data) {
    var lista = _configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val();

    for (var i = 0; i < lista.length; i++) {
        if (data.Codigo == lista[i].Codigo) {
            lista.splice(i, 1);
            break;
        }
    }

    _configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val(lista);

    RecarregarGridConfiguracaoIntegracaoAvon();
}

function AdicionarConfiguracaoIntegracaoAvonClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_configuracaoIntegracaoAvon);

    if (valido) {

        var lista = _configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val();

        if (lista.some((o) => { return o.Empresa.Codigo == _configuracaoIntegracaoAvon.Empresa.codEntity(); })) {
            exibirMensagem(tipoMensagem.atencao, "Empresa já configurada", "Já existe uma configuração para esta empresa!");
            return;
        }

        lista.push({
            Codigo: guid(),
            Empresa: {
                Codigo: _configuracaoIntegracaoAvon.Empresa.codEntity(),
                Descricao: _configuracaoIntegracaoAvon.Empresa.val()
            },
            EnterpriseID: _configuracaoIntegracaoAvon.EnterpriseID.val(),
            TokenHomologacao: _configuracaoIntegracaoAvon.TokenHomologacao.val(),
            TokenProducao: _configuracaoIntegracaoAvon.TokenProducao.val(),
        });

        _configuracaoIntegracao.ListaConfiguracoesIntegracaoAvon.val(lista);

        RecarregarGridConfiguracaoIntegracaoAvon();

        LimparCamposConfiguracaoIntegracaoAvon();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposConfiguracaoIntegracaoAvon() {
    LimparCampos(_configuracaoIntegracaoAvon);
}