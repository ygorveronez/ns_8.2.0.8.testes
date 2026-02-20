/// <reference path="../../Enumeradores/EnumRegimeTributario.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../Consultas/TipoTerceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridImposto, _imposto, _pesquisaImposto, _crudImposto;

var PesquisaImposto = function () {
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImposto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Imposto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoTerceiro = PropertyEntity({ text: Localization.Resources.Terceiros.Imposto.TipoTerceiro.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoIntegracaoTributaria = PropertyEntity({ text: "Cód. Integração Tributária: ", required: false, maxlength: 400, visible: ko.observable(true) });
    this.UtilizarBaseCalculoAcumulada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: "Utilizar base de cálculo acumulada do mês", visible: ko.observable(true) });
    this.UtilizarCalculoIrSobreFaixaValorTotal = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: "Efetuar Cálculo IR sobre faixa do valor total", visible: ko.observable(true) });
    this.CalcularPorRaizCNPJ = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: "Calcular (INSS/IRRPF) pela raíz do CNPJ", visible: ko.observable(true) });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Todas), options: EnumTipoPessoa.obterOpcoes(), text: "Tipo Pessoa: ", required: false, def: EnumTipoPessoa.Todas, enable: ko.observable(true) });
    this.RegimeTributario = PropertyEntity({ val: ko.observable(EnumRegimeTributario.NaoSelecionado), options: EnumRegimeTributario.obterOpcoesPesquisa(), def: EnumRegimeTributario.NaoSelecionado, text: "Regime Tributário: ", required: false, enable: ko.observable(true) });

    this.ListaINSS = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaIRRF = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDImposto = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadImposto() {

    _imposto = new Imposto();
    KoBindings(_imposto, "knockoutImposto");

    _crudImposto = new CRUDImposto();
    KoBindings(_crudImposto, "knockoutCRUDImposto");

    _pesquisaImposto = new PesquisaImposto();
    KoBindings(_pesquisaImposto, "knockoutPesquisaImposto", false, _pesquisaImposto.Pesquisar.id);

    HeaderAuditoria("ImpostoContratoFrete", _imposto);

    new BuscarClientes(_imposto.Terceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarClientes(_pesquisaImposto.Terceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarTipoTerceiro(_imposto.TipoTerceiro);

    BuscarImpostos();

    LoadINSS();
    LoadIRRF();
    LoadSEST();
    LoadSENAT();
    LoadPIS();
    LoadCOFINS();
}

function AdicionarClick(e, sender) {
    Salvar(_imposto, "Imposto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridImposto.CarregarGrid();
                LimparCamposImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_imposto, "Imposto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridImposto.CarregarGrid();
                LimparCamposImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este registro?", function () {
        ExcluirPorCodigo(_imposto, "Imposto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridImposto.CarregarGrid();
                    LimparCamposImposto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposImposto();
}

//*******MÉTODOS*******

function BuscarImpostos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarImposto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridImposto = new GridView(_pesquisaImposto.Pesquisar.idGrid, "Imposto/Pesquisa", _pesquisaImposto, menuOpcoes, null);
    _gridImposto.CarregarGrid();
}

function EditarImposto(impostoGrid) {
    LimparCamposImposto();

    _imposto.Codigo.val(impostoGrid.Codigo);

    BuscarPorCodigo(_imposto, "Imposto/BuscarPorCodigo", function (arg) {
        _pesquisaImposto.ExibirFiltros.visibleFade(false);

        _crudImposto.Atualizar.visible(true);
        _crudImposto.Cancelar.visible(true);
        _crudImposto.Excluir.visible(true);
        _crudImposto.Adicionar.visible(false);

        _imposto.Terceiro.enable(false);
        _imposto.TipoTerceiro.enable(false);

        ResetarTabs();

        RecarregarGridINSS();
        RecarregarGridIRRF();
    }, null);
}

function LimparCamposImposto() {
    _crudImposto.Atualizar.visible(false);
    _crudImposto.Cancelar.visible(false);
    _crudImposto.Excluir.visible(false);
    _crudImposto.Adicionar.visible(true);

    _imposto.Terceiro.enable(true);
    _imposto.TipoTerceiro.enable(true);

    LimparCampos(_imposto);

    LimparCamposINSS();
    LimparCamposIRRF();
    
    ResetarTabs();

    RecarregarGridINSS();
    RecarregarGridIRRF();
}

function ResetarTabs() {
    $("#tabImposto a:first").tab("show");
}