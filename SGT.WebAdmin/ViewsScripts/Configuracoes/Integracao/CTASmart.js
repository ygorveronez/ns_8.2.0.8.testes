var _configuracaoIntegracaoCTASmart, _gridCTASmart;

var ConfiguracaoIntegracaoCTASmart = function () {
    this.ConfiguracaoIntegracaoCTASmart = PropertyEntity({ type: types.listEntity, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PossuiIntegracaoCTASmart = PropertyEntity({ text: "Habilitar integração CTASmart?", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLCTASmart = PropertyEntity({ text: "*URL:", maxlength: 500, visible: ko.observable(true), required: ko.observable(false) });
    this.TokenCTASmart = PropertyEntity({ text: "*Token:", maxlength: 200, visible: ko.observable(true), required: ko.observable(false) });
    this.DataInicioCTASmart = PropertyEntity({ text: "Data de Início:", visible: ko.observable(true), required: ko.observable(false), getType: typesKnockout.date });
    this.CodigoEmpresaCTASmart = PropertyEntity({ text: "Código de Identificação da Empresa:", maxlength: 50, visible: ko.observable(true), required: ko.observable(false) });

    this.AdicionarDataCTASmart = PropertyEntity({ eventClick: adicionarCTASmartClick, text: "Adicionar", type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarDataCTASmart = PropertyEntity({ eventClick: atualizarCTASmartClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcluirDataCTASmart = PropertyEntity({ eventClick: excluirCTASmartClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarDataCTASmart = PropertyEntity({ eventClick: limparCTASmartClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
};

function LoadConfiguracaoIntegracaoCTASmart() {
    $("#knockoutConfiguracaoIntegracao").append($("#knockoutCTASmart"));

    _configuracaoIntegracaoCTASmart = new ConfiguracaoIntegracaoCTASmart();
    KoBindings(_configuracaoIntegracaoCTASmart, "knockoutCTASmart");

    _configuracaoIntegracao.PossuiIntegracaoCTASmart = _configuracaoIntegracaoCTASmart.PossuiIntegracaoCTASmart;


    ConfigurarGridConfiguracaoIntegracaoCTASmart();
}

function RecarregarGridCTASmart() {
    var data = new Array();
    $.each(_configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val(), function (i, configuracao) {
        data.push({
            Codigo: configuracao.Codigo,
            URLCTASmart: configuracao.URLCTASmart,
            TokenCTASmart: configuracao.TokenCTASmart,
            DataInicioCTASmart: configuracao.DataInicioCTASmart,
            CodigoEmpresaCTASmart: configuracao.CodigoEmpresaCTASmart
        });
    });

    _gridCTASmart.CarregarGrid(data);
}

function ConfigurarGridConfiguracaoIntegracaoCTASmart() {
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirCTASmartGridClick, visibilidade: true };
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarCTASmartGridClick, visibilidade: true };
    menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "URLCTASmart", title: "URL", width: "25%" },
        { data: "TokenCTASmart", title: "Token", width: "25%" },
        { data: "DataInicioCTASmart", title: "Data de Início", width: "25%" },
        { data: "CodigoEmpresaCTASmart", title: "Código da Empresa", width: "25%" },
    ];
    _gridCTASmart = new BasicDataTable(_configuracaoIntegracaoCTASmart.ConfiguracaoIntegracaoCTASmart.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridCTASmart();
}

function excluirCTASmart(codigo) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir?",
        function () {
            var lista = _configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val();
            for (var i = 0; i < lista.length; i++) {
                if (codigo == lista[i].Codigo) {
                    lista.splice(i, 1);
                    break;
                }
            }
            _configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val(lista);
            RecarregarGridCTASmart();
            limparCTASmart();    
        }
    );
}

function editarCTASmartGridClick(data) {
    _configuracaoIntegracaoCTASmart.Codigo.val(data.Codigo);
    _configuracaoIntegracaoCTASmart.URLCTASmart.val(data.URLCTASmart);
    _configuracaoIntegracaoCTASmart.TokenCTASmart.val(data.TokenCTASmart);
    _configuracaoIntegracaoCTASmart.DataInicioCTASmart.val(data.DataInicioCTASmart);
    _configuracaoIntegracaoCTASmart.CodigoEmpresaCTASmart.val(data.CodigoEmpresaCTASmart);
    ControlarBotoes(true);
}

function excluirCTASmartClick() {
    excluirCTASmart(_configuracaoIntegracaoCTASmart.Codigo.val());
}


function excluirCTASmartGridClick(data) {
    excluirCTASmart(data.Codigo);
}

function adicionarCTASmartClick() {
    if (isValidarCTASmart()) {
        var lista = _configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val();
        lista.push({
            Codigo: guid(),
            URLCTASmart: _configuracaoIntegracaoCTASmart.URLCTASmart.val(),
            TokenCTASmart: _configuracaoIntegracaoCTASmart.TokenCTASmart.val(),
            DataInicioCTASmart: _configuracaoIntegracaoCTASmart.DataInicioCTASmart.val(),
            CodigoEmpresaCTASmart: _configuracaoIntegracaoCTASmart.CodigoEmpresaCTASmart.val()

        });
        _configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val(lista);
        RecarregarGridCTASmart();
        limparCTASmart();
    }
}

function atualizarCTASmartClick() {    
    if (isValidarCTASmart()) {
        var lista = _configuracaoIntegracao.ListConfiguracoesIntegracaoCTASmart.val();
        const CTASmart = lista.find(el => el.Codigo == _configuracaoIntegracaoCTASmart.Codigo.val());     
        CTASmart.URLCTASmart = _configuracaoIntegracaoCTASmart.URLCTASmart.val();
        CTASmart.TokenCTASmart = _configuracaoIntegracaoCTASmart.TokenCTASmart.val();
        CTASmart.DataInicioCTASmart = _configuracaoIntegracaoCTASmart.DataInicioCTASmart.val();
        CTASmart.CodigoEmpresaCTASmart = _configuracaoIntegracaoCTASmart.CodigoEmpresaCTASmart.val();
        RecarregarGridCTASmart();
        limparCTASmart();
    }
}

function isValidarCTASmart() {
    _configuracaoIntegracaoCTASmart.URLCTASmart.required(true);
    _configuracaoIntegracaoCTASmart.TokenCTASmart.required(true);

    var validaURLCTASmart = ValidarCampoObrigatorioMap(_configuracaoIntegracaoCTASmart.URLCTASmart);
    var validaTokenCTASmart = ValidarCampoObrigatorioMap(_configuracaoIntegracaoCTASmart.TokenCTASmart);

    var isValided = validaURLCTASmart && validaTokenCTASmart;

    _configuracaoIntegracaoCTASmart.URLCTASmart.required(false);
    _configuracaoIntegracaoCTASmart.TokenCTASmart.required(false);

    if (!isValided) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    return true;
}

function limparCTASmartClick() {
    limparCTASmart();
}

function limparCTASmart() {
    LimparCampo(_configuracaoIntegracaoCTASmart.Codigo);
    LimparCampo(_configuracaoIntegracaoCTASmart.URLCTASmart);
    LimparCampo(_configuracaoIntegracaoCTASmart.TokenCTASmart);
    LimparCampo(_configuracaoIntegracaoCTASmart.DataInicioCTASmart);
    LimparCampo(_configuracaoIntegracaoCTASmart.CodigoEmpresaCTASmart);
    ControlarBotoes(false);
}

function ControlarBotoes(edit){
    _configuracaoIntegracaoCTASmart.AdicionarDataCTASmart.visible(!edit);
    _configuracaoIntegracaoCTASmart.AtualizarDataCTASmart.visible(edit);
    _configuracaoIntegracaoCTASmart.ExcluirDataCTASmart.visible(edit);
    _configuracaoIntegracaoCTASmart.CancelarDataCTASmart.visible(edit);
}