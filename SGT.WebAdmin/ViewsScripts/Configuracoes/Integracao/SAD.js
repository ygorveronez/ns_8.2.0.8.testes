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
/// <reference path="../../Consultas/CentrosDescarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoIntegracaoSAD, _configuracaoIntegracaoSAD;

var ConfiguracaoIntegracaoSAD = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PossuiIntegracaoSAD = PropertyEntity({ text: "Habilitar integração?", getType: typesKnockout.bool, val: _configuracaoIntegracao.PossuiIntegracaoSAD.val, def: false });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Token = PropertyEntity({ text: "*Token:", maxlength: 8000, visible: ko.observable(true), required: true });
    this.URLIntegracaoSADBuscarSenha = PropertyEntity({ text: "*URL Integração SAD Buscar Senha:", maxlength: 300, visible: ko.observable(true), required: true });
    this.URLIntegracaoSADFinalizarAgenda = PropertyEntity({ text: "*URL Integração SAD Finalizar Agenda:", maxlength: 300, visible: ko.observable(true), required: true });
    this.URLIntegracaoSADCancelarAgenda = PropertyEntity({ text: "*URL Integração SAD Cancelar Agenda:", maxlength: 300, visible: ko.observable(true), required: true });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro Descarregamento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    
    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoIntegracaoSADClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadConfiguracaoIntegracaoSAD() {
    $("#knockoutConfiguracaoIntegracao").append($("#knockoutConfiguracaoIntegracaoSAD"));

    _configuracaoIntegracaoSAD = new ConfiguracaoIntegracaoSAD();
    KoBindings(_configuracaoIntegracaoSAD, "knockoutConfiguracaoIntegracaoSAD");

    new BuscarCentrosDescarregamento(_configuracaoIntegracaoSAD.CentroDescarregamento);

    ConfigurarGridConfiguracaoIntegracaoSAD();
}

function ConfigurarGridConfiguracaoIntegracaoSAD() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirConfiguracaoIntegracaoSADClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CentroDescarregamento", title: "Centro Descarregamento", width: "20%" },
        { data: "Token", title: "Token", width: "20%" },
        { data: "URLIntegracaoSADBuscarSenha", title: "URL Buscar Senha", width: "20%" },
        { data: "URLIntegracaoSADFinalizarAgenda", title: "Url Finalizar Agenda", width: "20%" },
        { data: "URLIntegracaoSADCancelarAgenda", title: "Url Cancelar Agenda", width: "20%" },
    ];

    _gridConfiguracaoIntegracaoSAD = new BasicDataTable(_configuracaoIntegracaoSAD.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoIntegracaoSAD();
}

function RecarregarGridConfiguracaoIntegracaoSAD() {

    var data = new Array();

    $.each(_configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val(), function (i, configuracao) {
        var configGrid = new Object();

        configGrid.Codigo = configuracao.Codigo;
        configGrid.Token = configuracao.Token;
        configGrid.URLIntegracaoSADBuscarSenha = configuracao.URLIntegracaoSADBuscarSenha;
        configGrid.URLIntegracaoSADFinalizarAgenda = configuracao.URLIntegracaoSADFinalizarAgenda;
        configGrid.URLIntegracaoSADCancelarAgenda = configuracao.URLIntegracaoSADCancelarAgenda;
        configGrid.CentroDescarregamento = configuracao.CentroDescarregamento.Descricao;

        data.push(configGrid);
    });

    _gridConfiguracaoIntegracaoSAD.CarregarGrid(data);
}


function ExcluirConfiguracaoIntegracaoSADClick(data) {
    var lista = _configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val();

    for (var i = 0; i < lista.length; i++) {
        if (data.Codigo == lista[i].Codigo) {
            lista.splice(i, 1);
            break;
        }
    }

    _configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val(lista);

    RecarregarGridConfiguracaoIntegracaoSAD();
}

function AdicionarConfiguracaoIntegracaoSADClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_configuracaoIntegracaoSAD);

    if (valido) {

        var lista = _configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val();

        if (lista.some((o) => { return o.CentroDescarregamento.Codigo == _configuracaoIntegracaoSAD.CentroDescarregamento.codEntity(); })) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Já existe uma configuração para este centro de descarregamento!");
            return;
        }

        lista.push({
            Codigo: guid(),
            CentroDescarregamento: {
                Codigo: _configuracaoIntegracaoSAD.CentroDescarregamento.codEntity(),
                Descricao: _configuracaoIntegracaoSAD.CentroDescarregamento.val()
            },
            Token: _configuracaoIntegracaoSAD.Token.val(),
            URLIntegracaoSADBuscarSenha: _configuracaoIntegracaoSAD.URLIntegracaoSADBuscarSenha.val(),
            URLIntegracaoSADFinalizarAgenda: _configuracaoIntegracaoSAD.URLIntegracaoSADFinalizarAgenda.val(),
            URLIntegracaoSADCancelarAgenda: _configuracaoIntegracaoSAD.URLIntegracaoSADCancelarAgenda.val()
        });
        
        _configuracaoIntegracao.ListaConfiguracoesIntegracaoSAD.val(lista);

        RecarregarGridConfiguracaoIntegracaoSAD();

        LimparCamposConfiguracaoIntegracaoSAD();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposConfiguracaoIntegracaoSAD() {
    LimparCampo(_configuracaoIntegracaoSAD.CentroDescarregamento);
    LimparCampo(_configuracaoIntegracaoSAD.Token);
    LimparCampo(_configuracaoIntegracaoSAD.URLIntegracaoSADBuscarSenha);
    LimparCampo(_configuracaoIntegracaoSAD.URLIntegracaoSADFinalizarAgenda);
    LimparCampo(_configuracaoIntegracaoSAD.URLIntegracaoSADCancelarAgenda);
    LimparCampo(_configuracaoIntegracaoSAD.Codigo);
}