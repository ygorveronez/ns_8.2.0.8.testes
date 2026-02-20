/// <autosync enabled="true" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/TipoLacre.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _cargaDadosEmissaoLacre;
var _gridCargaDadosEmissaoLacre;
var _pesquisaCargaDadosEmissaoLacre;

var PesquisaCargaDadosEmissaoLacre = function () {
    this.Carga = PropertyEntity({});
};

var CargaDadosEmissaoLacre = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ idGrid: guid(), codEntity: ko.observable(0) });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Numero.getRequiredFieldDescription(), maxlength: 60, required: true });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Cliente.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoLacre = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoLacre.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaDadosEmissaoLacreClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarLacre, visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadCargaDadosEmissaoLacre() {
    _cargaDadosEmissaoLacre = new CargaDadosEmissaoLacre();
    KoBindings(_cargaDadosEmissaoLacre, "tabLacres_" + _cargaAtual.DadosEmissaoFrete.id);

    $("#tabLacres_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    _pesquisaCargaDadosEmissaoLacre = new PesquisaCargaDadosEmissaoLacre();

    _cargaDadosEmissaoLacre.Carga.codEntity(_cargaAtual.Codigo.val());
    _cargaDadosEmissaoLacre.Carga.val(_cargaAtual.Codigo.val());
    _cargaDadosEmissaoLacre.Carga.def = _cargaAtual.Codigo.val();
    _cargaDadosEmissaoLacre.Pedido.enable(isHabilitarEdiçãoLacres());
    _cargaDadosEmissaoLacre.Adicionar.enable(_cargaDadosEmissaoLacre.Pedido.enable());

    _pesquisaCargaDadosEmissaoLacre.Carga.val(_cargaAtual.Codigo.val());

    new BuscarClientesCarga(_cargaDadosEmissaoLacre.Cliente, null, _cargaDadosEmissaoLacre.Carga, false);
    new BuscarTipoLacre(_cargaDadosEmissaoLacre.TipoLacre);

    LoadGridCargaDadosEmissaoLacres();

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#tabLacres_" + _cargaAtual.DadosEmissaoFrete.id + "_li").hide();

    if (_CONFIGURACAO_TMS.ExibirTipoLacre) {
        _cargaDadosEmissaoLacre.Cliente.visible(true);
        _cargaDadosEmissaoLacre.Cliente.required(true);
        _cargaDadosEmissaoLacre.TipoLacre.visible(true);
        _cargaDadosEmissaoLacre.TipoLacre.required(true);
    }
}

function LoadGridCargaDadosEmissaoLacres() {
    var menuOpcoes = null;
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirCargaDadosEmissaoLacre, tamanho: 7, icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("CargaLacre", "Codigo", _notaFiscal), tamanho: 7, icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    if (_cargaDadosEmissaoLacre.Pedido.enable())
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [excluir, auditar], tamanho: 15 };
    else if (PermiteAuditar())
        menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Auditar, opcoes: [auditar], tamanho: 15 };

    _gridCargaDadosEmissaoLacre = new GridView(_cargaDadosEmissaoLacre.Carga.idGrid, "DadosLacre/Pesquisa", _pesquisaCargaDadosEmissaoLacre, menuOpcoes, null);
    _gridCargaDadosEmissaoLacre.CarregarGrid();
}

function AdicionarCargaDadosEmissaoLacreClick(e, sender) {
    Salvar(e, "DadosLacre/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LacreAdicionadoComSucesso);
                _gridCargaDadosEmissaoLacre.CarregarGrid();

                var codigoCliente = _cargaDadosEmissaoLacre.Cliente.codEntity();
                var descricaoCliente = _cargaDadosEmissaoLacre.Cliente.val();
                LimparCamposCargaDadosEmissaoLacre();
                _cargaDadosEmissaoLacre.Cliente.codEntity(codigoCliente);
                _cargaDadosEmissaoLacre.Cliente.val(descricaoCliente);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function ExcluirCargaDadosEmissaoLacre(lacre) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirLacre.format(lacre.Numero), function () {

        _cargaDadosEmissaoLacre.Codigo.val(lacre.Codigo);

        ExcluirPorCodigo(_cargaDadosEmissaoLacre, "DadosLacre/Excluir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LacreExcluidoComSucesso);
                    _gridCargaDadosEmissaoLacre.CarregarGrid();
                    LimparCamposCargaDadosEmissaoLacre();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function isHabilitarEdiçãoLacres() {
    var habilitar = _cargaAtual.EtapaFreteEmbarcador.enable() || _CONFIGURACAO_TMS.PermitirAlterarLacres;

    if (!habilitar && _CONFIGURACAO_TMS.PossuiTipoOperacaoUtilizarPlanoViagem) {
        habilitar = _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.EmTransporte || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Encerrada;
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarLacres, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura)
        habilitar = false;

    return habilitar;
}

function LimparCamposCargaDadosEmissaoLacre() {
    LimparCampos(_cargaDadosEmissaoLacre);
}