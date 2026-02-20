/// <reference path="CamposLayoutEDI.js" />
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
/// <reference path="../../Enumeradores/EnumTipoLayoutEDI.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLayoutEDI;
var _layoutEDI;
var _dadosCampos;
var _CRUDLayoutEDI;
var _pesquisaLayoutEDI;
var _encodingsDisponiveis;

var _statusLayoutEDI = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaLayoutEDI = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoLayoutEDI.Todos), options: EnumTipoLayoutEDI.obterOpcoesPesquisa(), def: EnumTipoLayoutEDI.Todos, text: "*Tipo Layout EDI:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLayoutEDI.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var LayoutEDI = function () {
    this.PermitirEdicao = PropertyEntity({ val: ko.observable(true), def: 0, getType: typesKnockout.bool });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoLayoutEDI.CONEMB), visible: ko.observable(true), options: EnumTipoLayoutEDI.obterOpcoes(), def: EnumTipoLayoutEDI.CONEMB, eventChange: TipoChange, text: "*Tipo Layout EDI: ", issue: 271 });
    this.QuantidadeNotasSequencia = PropertyEntity({ text: "Qtd. Notas Sequencial: ", visible: ko.observable(true), getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Separador = PropertyEntity({ text: "Caracter Separador: ", visible: ko.observable(true) });
    this.SeparadorDecimal = PropertyEntity({ text: "Caracter Separador de Decimal: ", visible: ko.observable(true) });
    this.SeparadorInicialFinal = PropertyEntity({ getType: typesKnockout.bool, text: "Utilizar separador de inicio e fim", val: ko.observable(false), visible: ko.observable(true), def: false });
    this.Campos = PropertyEntity({ getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), visible: ko.observable(true) });
    this.Nomenclatura = PropertyEntity({ text: "Nomenclatura do arquivo: ", required: false, visible: ko.observable(true) });
    this.ExtensaoArquivo = PropertyEntity({ text: "Extensão do arquivo: ", required: false, maxlength: 100, visible: ko.observable(true) });
    this.ValidarRota = PropertyEntity({ getType: typesKnockout.bool, text: "Validar Rota", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ValidarNumeroReferenciaEDI = PropertyEntity({ getType: typesKnockout.bool, text: "Validar Número Referência EDI", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GerarEDIEmOcorrencias = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar esse EDI nas ocorrências", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.IncluirCNPJEmitenteArquivoEDI = PropertyEntity({ getType: typesKnockout.bool, text: "Incluir CNPJ do Emitente no Arquivo EDI", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ConsiderarDadosExpedidorECTe = PropertyEntity({ getType: typesKnockout.bool, text: "Considerar dados do expedidor e do CTe", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RemoverDiacriticos = PropertyEntity({ getType: typesKnockout.bool, text: "Remover Diacríticos", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.BuscarNotaSemChaveDosDocumentosDestinados = PropertyEntity({ getType: typesKnockout.bool, text: "Buscar notas fiscais sem chave dos documentos destinados", val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusLayoutEDI, def: "A", text: "Situação:", visible: ko.observable(true) });
    this.AgruparNotasFiscaisDosCTesParaSubcontratacao = PropertyEntity({ getType: typesKnockout.bool, text: "Agrupar notas fiscais dos CT-es para subcontratação", val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.Encoding = PropertyEntity({ val: ko.observable(""), options: _encodingsDisponiveis, def: "", text: "Encoding: ", visible: ko.observable(true) });
    this.EmailLeitura = PropertyEntity({ text: "Email Leitura: ", maxlength: 500, getType: typesKnockout.email});

    // INDICEEDI
    this.CamposPorIndices = PropertyEntity({ visible: ko.observable(true), getType: typesKnockout.bool, text: "Campos Por Índices", val: ko.observable(false), def: false });
    this.CamposPorIndices.val.subscribe(function (valor) {
        AlternaModoPorInidice(valor);
    });

    this.TagTransportadoraCNPJ = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#CNPJTransportadora#"); }, type: types.event, text: "CNPJ Transportador", visible: ko.observable(true) });
    this.TagAno = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Ano#"); }, type: types.event, text: "Ano", visible: ko.observable(true) });
    this.TagAnoAbreviado = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#AnoAbreviado#"); }, type: types.event, text: "Ano Abreviado", visible: ko.observable(true) });
    this.TagMes = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Mes#"); }, type: types.event, text: "Mes", visible: ko.observable(true) });
    this.TagDia = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Dia#"); }, type: types.event, text: "Dia", visible: ko.observable(true) });
    this.TagHora = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Hora#"); }, type: types.event, text: "Hora", visible: ko.observable(true) });
    this.TagMin = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Minutos#"); }, type: types.event, text: "Min", visible: ko.observable(true) });
    this.TagSeg = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Segundos#"); }, type: types.event, text: "Seg", visible: ko.observable(true) });
    this.TagNumero = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#Numero#"); }, type: types.event, text: "Número", visible: ko.observable(true) });
    this.TagNumeroCarregamento = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#NumeroCarregamento#"); }, type: types.event, text: "Número Carregamento", visible: ko.observable(true) });
    this.TagCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#CNPJCliente#"); }, type: types.event, text: "CNPJ Cliente", visible: ko.observable(true) });
    this.TagFinalCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#FinalCNPJCliente#"); }, type: types.event, text: "Final do CNPJ Cliente", visible: ko.observable(true) });
    this.TagIdRegistro = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#IdRegistro#"); }, type: types.event, text: "ID Registro (único)", visible: ko.observable(true) });
    this.TagCodigoEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#CodigoEmpresa#"); }, type: types.event, text: "Código da Empresa", visible: ko.observable(true) });
    this.TagCodigoEstabelecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#CodigoEstabelecimento#"); }, type: types.event, text: "Código do Estabelecimento", visible: ko.observable(true) });
    this.TagNumeroFatura = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#NumeroFatura#"); }, type: types.event, text: "Número da Fatura", visible: ko.observable(true) });
    this.TagProtocoloCarga = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#ProtocoloCarga#"); }, type: types.event, text: "Protocolo da Carga", visible: ko.observable(true) });
    this.TagTipoOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(_layoutEDI.Nomenclatura.id, "#TipoOcorrencia#"); }, type: types.event, text: "Tipo ocorrencia", visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (valor) {
        _layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados.visible((valor === EnumTipoLayoutEDI.NOTFIS || valor === EnumTipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO));
        _layoutEDI.AgruparNotasFiscaisDosCTesParaSubcontratacao.visible(valor === EnumTipoLayoutEDI.NOTFIS);
    });
};

var DadosCampos = function () {
    this.DadosCampos = PropertyEntity({ text: "*Dados dos Campos: " });
    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
    this.Exportar = PropertyEntity({ eventClick: exportarClick, type: types.event, text: "Exportar", visible: ko.observable(true) });
};

var CrudLayoutEDI = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******



function TipoChange() {
    if (_layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB_CANCELAMENTO || _layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB || _layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB_CT_EXP || _layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB_CT_IMP || _layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB_MB || _layoutEDI.Tipo.val() == EnumTipoLayoutEDI.CONEMB_NF)
        _layoutEDI.GerarEDIEmOcorrencias.visible(true);
    else
        _layoutEDI.GerarEDIEmOcorrencias.visible(false);
}

function loadLayoutEDI() {

    ObterEncodingsDisponiveis().then(function () {
        _layoutEDI = new LayoutEDI();
        KoBindings(_layoutEDI, "knockoutCadastroLayoutEDI");

        _CRUDLayoutEDI = new CrudLayoutEDI();
        KoBindings(_CRUDLayoutEDI, "knockoutCRUDLayoutEDI");

        _pesquisaLayoutEDI = new PesquisaLayoutEDI();
        KoBindings(_pesquisaLayoutEDI, "knockoutPesquisaLayoutEDI", false, _pesquisaLayoutEDI.Pesquisar.id);

        _dadosCampos = new DadosCampos();
        KoBindings(_dadosCampos, "knockoutDadosCamposEDI");

        HeaderAuditoria("LayoutEDI", _layoutEDI);

        buscarLayoutEDIs();
        loadTipoCamposLayoutEDI();
    });

}

function adicionarClick(e, sender) {
    reordenarPosicoesCampos();
    $.each(_layoutEDI.Campos.val(), function (i, campo) {
        if (campo.Codigo.length > 10) {
            _layoutEDI.Campos.val()[i].Codigo = 0;
        }
    });
    _layoutEDI.Campos.val(JSON.stringify(ObterCamposOrdenados()));
    Salvar(_layoutEDI, "LayoutEDI/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridLayoutEDI.CarregarGrid();
                limparCamposLayoutEDI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    reordenarPosicoesCampos();
    $.each(_layoutEDI.Campos.val(), function (i, campo) {
        if (campo.Codigo.length > 10) {
            _layoutEDI.Campos.val()[i].Codigo = 0;
        }
    });
    _layoutEDI.Campos.val(JSON.stringify(ObterCamposOrdenados()));
    Salvar(_layoutEDI, "LayoutEDI/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridLayoutEDI.CarregarGrid();
                limparCamposLayoutEDI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a layout EDI " + _layoutEDI.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_layoutEDI, "LayoutEDI/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridLayoutEDI.CarregarGrid();
                    limparCamposLayoutEDI();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposLayoutEDI();
}

function importarClick() {
    exibirConfirmacao("Importar Dados", "Tem certeza que deseja importar os dados do EDI?", function () {
        // Pega os dados
        try {
            var json = JSON.parse(_dadosCampos.DadosCampos.val());

            if (!$.isArray(json)) throw ("");
        } catch (e) {
            exibirMensagem(tipoMensagem.falha, "Falha Importação", "O conteúdo não é válido");
            return false;
        }

        // Valida cada entrada
        for (var i = 0; i < json.length; i++) {
            if (typeof json[i].Descricao == "undefined" || json[i].Descricao == "") {
                console.log("Descricao invalida ou nao existe. Index: " + i);
                exibirMensagem(tipoMensagem.atencao, "Falha Importação", "Algumas informações estão incompletas");
                return;
            }

            if (typeof json[i].Tipo == "undefined" || isNaN((json[i].Tipo))) {
                console.log("Tipo invalido ou nao existe. Index: " + i);
                exibirMensagem(tipoMensagem.atencao, "Falha Importação", "Algumas informações estão incompletas");
                return;
            }

            if (typeof json[i].QuantidadeCaracteres == "undefined" || isNaN((json[i].QuantidadeCaracteres))) {
                console.log("QuantidadeCaracteres invalido ou nao existe. Index: " + i);
                exibirMensagem(tipoMensagem.atencao, "Falha Importação", "Algumas informações estão incompletas");
                return;
            }

            if (typeof json[i].QuantidadeDecimais == "undefined" || isNaN((json[i].QuantidadeDecimais))) {
                console.log("QuantidadeDecimais invalido ou nao existe. Index: " + i);
                exibirMensagem(tipoMensagem.atencao, "Falha Importação", "Algumas informações estão incompletas");
                return;
            }

            if (typeof json[i].QuantidadeInteiros == "undefined" || isNaN((json[i].QuantidadeInteiros))) {
                console.log("QuantidadeInteiros invalido ou nao existe. Index: " + i);
                exibirMensagem(tipoMensagem.atencao, "Falha Importação", "Algumas informações estão incompletas");
                return;
            }

            json[i].Codigo = ((i + 1) * -1);
        }

        // Ordena
        json.sort(function (a, b) { return a.Ordem - b.Ordem });

        // Adiciona aos campos
        _layoutEDI.Campos.val(json);
        recarregarGridReorder();
        _dadosCampos.DadosCampos.val("");
    });
}

function exportarClick() {
    var campos = ObterCamposOrdenados();

    for (var i = 0; i < campos.length; i++)
        campos[i].Codigo = 0

    _dadosCampos.DadosCampos.val(JSON.stringify(campos, null, "\t"));
    document.getElementById(_dadosCampos.DadosCampos.id).select();
}

//*******MÉTODOS*******

function buscarLayoutEDIs() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarLayoutEDI, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLayoutEDI = new GridView(_pesquisaLayoutEDI.Pesquisar.idGrid, "LayoutEDI/Pesquisar", _pesquisaLayoutEDI, menuOpcoes, null);
    _gridLayoutEDI.CarregarGrid();
}

function editarLayoutEDI(layoutEDIGrid) {
    limparCamposLayoutEDI();
    _layoutEDI.Codigo.val(layoutEDIGrid.Codigo);
    BuscarPorCodigo(_layoutEDI, "LayoutEDI/BuscarPorCodigo", function (arg) {
        _pesquisaLayoutEDI.ExibirFiltros.visibleFade(false);
        _CRUDLayoutEDI.Atualizar.visible(true);
        _CRUDLayoutEDI.Cancelar.visible(true);
        _CRUDLayoutEDI.Excluir.visible(true);
        _CRUDLayoutEDI.Adicionar.visible(false);
        TipoChange();
        recarregarGridReorder();
        CamposEditaveis();
    }, null);
}

function limparCamposLayoutEDI() {
    _CRUDLayoutEDI.Atualizar.visible(false);
    _CRUDLayoutEDI.Cancelar.visible(false);
    _CRUDLayoutEDI.Excluir.visible(false);
    _CRUDLayoutEDI.Adicionar.visible(true);
    _layoutEDI.GerarEDIEmOcorrencias.visible(true);
    _layoutEDI.Campos.val(new Array());
    recarregarGridReorder();
    LimparCampos(_layoutEDI);
    _dadosCampos.DadosCampos.val("");
    $("#liTabTipoCarga a").click();
}

function ObterEncodingsDisponiveis() {
    var p = new promise.Promise();

    executarReST("Encoding/ObterTodos", {}, function (r) {
        if (r.Success && r.Data) {
            _encodingsDisponiveis = [{ value: "", text: "Padrão" }].concat(r.Data);

            p.done();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });

    return p;
}

function CamposEditaveis() {

    let permitirEdicao = _layoutEDI.PermitirEdicao.val()


    _layoutEDI.Descricao.visible(permitirEdicao);
    _layoutEDI.Tipo.visible(permitirEdicao);
    _layoutEDI.QuantidadeNotasSequencia.visible(permitirEdicao);
    _layoutEDI.Separador.visible(permitirEdicao);
    _layoutEDI.SeparadorDecimal.visible(permitirEdicao);
    _layoutEDI.SeparadorInicialFinal.visible(permitirEdicao);
    _layoutEDI.Campos.visible(permitirEdicao);
    _layoutEDI.Nomenclatura.visible(permitirEdicao);
    _layoutEDI.ExtensaoArquivo.visible(permitirEdicao);
    _layoutEDI.ValidarRota.visible(permitirEdicao);
    _layoutEDI.ValidarNumeroReferenciaEDI.visible(permitirEdicao);
    _layoutEDI.GerarEDIEmOcorrencias.visible(permitirEdicao);
    _layoutEDI.RemoverDiacriticos.visible(permitirEdicao);
    _layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados.visible(permitirEdicao);
    _layoutEDI.Status.visible(permitirEdicao);
    _layoutEDI.AgruparNotasFiscaisDosCTesParaSubcontratacao.visible(permitirEdicao);
    _layoutEDI.Encoding.visible(permitirEdicao);
    _layoutEDI.TagTransportadoraCNPJ.visible(permitirEdicao);
    _layoutEDI.TagAno.visible(permitirEdicao);
    _layoutEDI.TagAnoAbreviado.visible(permitirEdicao);
    _layoutEDI.TagMes.visible(permitirEdicao);
    _layoutEDI.TagDia.visible(permitirEdicao);
    _layoutEDI.TagHora.visible(permitirEdicao);
    _layoutEDI.TagMin.visible(permitirEdicao);
    _layoutEDI.TagSeg.visible(permitirEdicao);
    _layoutEDI.TagNumero.visible(permitirEdicao);
    _layoutEDI.TagNumeroCarregamento.visible(permitirEdicao);
    _layoutEDI.TagCNPJCliente.visible(permitirEdicao);
    _layoutEDI.TagFinalCNPJCliente.visible(permitirEdicao);
    _layoutEDI.TagIdRegistro.visible(permitirEdicao);
    _layoutEDI.TagCodigoEmpresa.visible(permitirEdicao);
    _layoutEDI.TagCodigoEstabelecimento.visible(permitirEdicao);
    _layoutEDI.TagNumeroFatura.visible(permitirEdicao);
    _layoutEDI.TagProtocoloCarga.visible(permitirEdicao);
    _layoutEDI.TagTipoOcorrencia.visible(permitirEdicao);
    _layoutEDI.CamposPorIndices.visible(permitirEdicao);

    if (permitirEdicao) {
        $("#liDadosCamposEDI").show();
        $("#liModeloVeicular").show();
    } else {
        $("#liDadosCamposEDI").hide();
        $("#liModeloVeicular").hide();
        $("#EtiquetaTags").hide();
    }
}
  