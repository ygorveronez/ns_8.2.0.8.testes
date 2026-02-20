/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="SelecaoDocumentos.js" />
/// <reference path="AnexosNFSManual.js" />
/// <reference path="NFSManualSignalR.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNFS;
var _nfsManual;
var _CRUDNFSManual;
var _pesquisaNFS;
var _possuiResiduais;

var _pesquisaResiduais = [
    { text: "Todos", value: 0 },
    { text: "Sim", value: 1 },
    { text: "Não", value: 2 }
];

var NFSManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLancamentoNFSManual.Todas), def: EnumSituacaoLancamentoNFSManual.Todas, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoLancamentoNFSManual.todos), options: EnumSituacaoLancamentoNFSManual.obterOpcoes(), def: EnumSituacaoLancamentoNFSManual.todos, text: "Situação: " });
    this.ContemEDI = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var CRUDNFSManual = function () {
    this.Limpar = PropertyEntity({ eventClick: limparLancamentoClick, type: types.event, text: "Limpar (Gerar Nova NFS)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaNFS = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número NFS:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Número do Pedido no Cliente:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.string });
    this.NumeroDOC = PropertyEntity({ text: "Número do Documento:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLancamentoNFSManual.todos), options: EnumSituacaoLancamentoNFSManual.obterOpcoesPesquisa(), def: EnumSituacaoLancamentoNFSManual.todos, text: "Situação: " });
    this.Residuais = PropertyEntity({ val: ko.observable(0), options: _pesquisaResiduais, def: 0, text: "Residuais: ", visible: ko.observable(_possuiResiduais) });

    this.Ocorrencia = PropertyEntity({ text: "Ocorrência:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNFS.CarregarGrid();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ImportarEDI = PropertyEntity({ eventClick: ImportarEDIClick, type: types.event, text: "Importar EDI", visible: ko.observable(false) });
    this.GerarRPS = PropertyEntity({ eventClick: GerarMultiploRPSClick, type: types.event, text: "Gerar RPS", visible: ko.observable(false) });
};


//*******EVENTOS*******

function loadNFSManual() {
    _nfsManual = new NFSManual();
    HeaderAuditoria("LancamentoNFSManual", _nfsManual);

    _CRUDNFSManual = new CRUDNFSManual();
    KoBindings(_CRUDNFSManual, "knockoutCRUD");

    _pesquisaNFS = new PesquisaNFS();
    KoBindings(_pesquisaNFS, "knockoutPesquisaNFS", false, _pesquisaNFS.Pesquisar.id);

    executarReST("NFSManual/ContemEDI", {}, function (arg) {
        if (arg.Data !== null) {
            if (arg.Data.ContemEDI === true) {
                _pesquisaNFS.ImportarEDI.visible(true);
                _pesquisaNFS.GerarRPS.visible(true);
            } else {
                _pesquisaNFS.ImportarEDI.visible(false);
                _pesquisaNFS.GerarRPS.visible(false);
            }

            loadEtapasNFS();
            loadSelecaoDocumentos();
            loadDadosEmissao();
            loadAprovacao();
            //loadIntegracao();
            BuscarHTMLINtegracaoNFSManual();
            // Inicia as buscas
            new BuscarTransportadores(_pesquisaNFS.Empresa);
            new BuscarCargas(_pesquisaNFS.Carga, null, null, null, null, null, null, null, null, true);
            new BuscarClientes(_pesquisaNFS.Tomador);
            new BuscarLocalidades(_pesquisaNFS.LocalidadePrestacao);
            new BuscarFilial(_pesquisaNFS.Filial);
            new BuscarFilial(_pesquisaNFS.Filiais);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                _pesquisaNFS.Empresa.visible(false);
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaNFS.Empresa.text("Empresa/Filial: ");
                _pesquisaNFS.Filial.visible(false);
            }

            if (_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual) {
                _pesquisaNFS.Filiais.visible(true);
                _pesquisaNFS.Filial.visible(false);
            }
            else {
                _pesquisaNFS.Filiais.visible(false);
                _pesquisaNFS.Filial.visible(true);
            }

            BuscarNFS();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
    LoadSignalRNFSManual();
}

function GerarMultiploRPSClick(e, sender) {
    exibirConfirmacao("Gerar RPS", "Você tem certeza que deseja gerar o arquivo de RPS das notas contidas no filtro realizado?", function () {
        executarDownload("NFSManual/GerarMultiploRPS", RetornarObjetoPesquisa(_pesquisaNFS), null, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function limparLancamentoClick(e, sender) {
    LimparCamposLancamento();
    GridSelecaoDocumentos();
}

function ImportarEDIClick(e, sender) {
    _dadosEmissao.EDI.val("");
    $("#" + _dadosEmissao.EDI.id).trigger("click");
}

function EnviarEDI(e, sender) {
    if (_dadosEmissao.EDI.val() != "") {
        let file = document.getElementById(_dadosEmissao.EDI.id);

        let formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("NFSManual/ProcessarRetornoNotaServico", {}, formData, function (arg) {

            let fileControl = $("#" + _dadosEmissao.EDI.id);
            fileControl.replaceWith(fileControl = fileControl.clone(true));
            _dadosEmissao.EDI.val("");

            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo de retorno processado com sucesso.");
                    LimparCamposLancamento();
                    GridSelecaoDocumentos();
                    _gridNFS.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                _dadosEmissao.EDI.val("");
            }
        });
    }
}

//*******MÉTODOS*******
function BuscarNFS() {
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarNFS, tamanho: "15", icone: "" };
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridNFS = new GridView(_pesquisaNFS.Pesquisar.idGrid, "NFSManual/Pesquisa", _pesquisaNFS, menuOpcoes);
    _gridNFS.CarregarGrid();
}

function editarNFS(itemGrid) {
    // Limpa os campos
    LimparCamposLancamento();

    // Esconde filtros
    _pesquisaNFS.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarNFSPorCodigo(itemGrid.Codigo);
}

function BuscarNFSPorCodigo(codigo, cb, setarEtapa) {
    executarReST("NFSManual/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarNFSManual(arg.Data);

            EditarSelecaoDocumentos(arg.Data);

            EditarDadosNFS(arg.Data);

            ListarAprovacoes(arg.Data);

            if (setarEtapa !== false)
                SetarEtapasNFS();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarNFSManual(data) {
    _nfsManual.Codigo.val(data.Codigo);
    _nfsManual.Situacao.val(data.Situacao);
    _nfsManual.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _nfsManual.ContemEDI.val(data.ContemEDI);
    _CRUDNFSManual.Limpar.visible(true);

    // Insere na lista de anexos o código
    _listaAnexos.Codigo.val(data.Codigo);
    obterAnexosDoServer(data.Codigo);
}

function LimparCamposLancamento() {
    LimparCampos(_nfsManual);
    _CRUDNFSManual.Limpar.visible(false);

    SetarEtapaInicioLancamento();

    LimparCamposSelecaoDocumentos();
    LimparCamposDadosNFS();

    // Limpa da lista de anexos o codigo do item atual
    _listaAnexos.Codigo.val(null);
}

function obterAnexosDoServer(codigo) {
    executarReST("AnexoLancamentoNFSManual/ObterAnexo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaAnexos.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
function obterServicoAliquotaRetencao(valor) {
    
    let CodigoServico = 0;
    if (valor != undefined) {
        _dadosEmissao.ServicoNFSe.val(valor.Descricao);
        _dadosEmissao.ServicoNFSe.codEntity(valor.Codigo);
        CodigoServico = valor.Codigo;
    }
    executarReST("NFSManual/ObterAliquotaRetancaoISS", { Localidade: _dadosEmissao.LocalidadePrestacao.codEntity(), ServicoNFSe: CodigoServico, Empresa: _dadosEmissao.Transportador.codEntity() }, function (arg) {
        if (arg.Data != null) {
            if (arg.Data.CodigoServico > 0) {
                _dadosEmissao.PercentualRetencao.val(arg.Data.RetencaoISS);
                _dadosEmissao.AliquotaISS.val(arg.Data.AliquotaISS);
                _dadosEmissao.ServicoNFSe.val(arg.Data.DescricaoServico);
                _dadosEmissao.ServicoNFSe.codEntity(arg.Data.CodigoServico);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

