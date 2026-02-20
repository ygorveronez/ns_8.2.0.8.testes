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
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/CargaEntrega.js" />
/// <reference path="../../Consultas/JustificativaCancelamentoCarga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="./ImagemNotaFiscal.js" />
/// <reference path="./Geolocalizacao.js" />
/// <reference path="./IntegracaoComprovanteEntrega.js" />

var _wizardLoteComprovanteEntrega;
var _dadosComprovanteEtapa;
var _gridCargaEntregaAdicionada;
var _CRUDLoteComprovanteEntrega;
var _pesquisaLoteComprovanteEntrega;
var _gridLoteComprovanteEntrega;

// region: ENTIDADES 

var WizardLoteComprovanteEntrega = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados do Comprovante", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Inserir lista de paradas da carga que o lote terá"),
        tooltipTitle: ko.observable("Dados do Comprovante")
    });

    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Disponibilização das informações dos documentos via integração entre o sistema e o SEFAZ"),
        tooltipTitle: ko.observable("Integração de Documentos")
    });
}

var DadosComprovanteEtapa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(), getType: typesKnockout.int, text: "Lote", enable: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Carga:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Carga.val.subscribe(limparListaCargaEntrega)
    this.ListaCargaEntrega = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.AdicionarCargaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar", idBtnSearch: guid(), enable: ko.observable(true) });
}

var CRUDLoteComprovanteEntrega = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarLoteClick, type: types.event, text: ko.observable("Atualizar"), visible: ko.observable(false), enable: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: ko.observable("Limpar"), visible: ko.observable(true), enable: ko.observable(true) });
};

var PesquisaLoteComprovanteEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", text: "Número:" });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteComprovanteEntrega.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

// region: LOADING

async function loadLoteComprovanteEntrega(isModal = false) {
    await carregarHtmlWizardLoteComprovante();
    await carregarHtmlModaisLoteComprovante();

    _wizardLoteComprovanteEntrega = new WizardLoteComprovanteEntrega();
    KoBindings(_wizardLoteComprovanteEntrega, "knockoutEtapaLoteComprovanteEntrega");

    _pesquisaLoteComprovanteEntrega = new PesquisaLoteComprovanteEntrega();
    _CRUDLoteComprovanteEntrega = new CRUDLoteComprovanteEntrega();

    // CRUD
    KoBindings(_CRUDLoteComprovanteEntrega, "knockoutCRUDLoteComprovanteEntrega");
    buscarLotesComprovanteEntrega();

    if (!isModal) {
         // Pesquisa
        KoBindings(_pesquisaLoteComprovanteEntrega, "knockoutPesquisaLoteComprovanteEntrega", false, _pesquisaLoteComprovanteEntrega.Pesquisar.id);
        new BuscarCargas(_pesquisaLoteComprovanteEntrega.Carga);

        
    }

    // Primeira etapa do Wizard
    loadDadosComprovanteEtapa();

    $("[rel=popover-hover]").popover({ trigger: "hover" });

    // load modais (em outros arquivos)
    loadModalImagemCanhoto();
    loadModalDadosRecebedor();
    LoadIntegracaoComprovanteEntrega(_dadosComprovanteEtapa, "knockoutIntegracaoEtapa");
}

function loadDadosComprovanteEtapa() {
    _dadosComprovanteEtapa = new DadosComprovanteEtapa();
    KoBindings(_dadosComprovanteEtapa, "knockoutDadosComprovanteEtapa");

    new BuscarCargas(_dadosComprovanteEtapa.Carga, onChooseCarga, null, [EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.EmTransporte]);
    new BuscarCargaEntrega(_dadosComprovanteEtapa.AdicionarCargaEntrega, onChooseCargaEntrega, _dadosComprovanteEtapa.Carga);

    var ordenacao = { column: 0, dir: orderDir.asc };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = [
        { descricao: "Geo Localização", id: guid(), evento: "onclick", metodo: onClickAlterarGeolocalizacao, tamanho: "7", icone: "" },
        { descricao: "Dados Recebedor", id: guid(), evento: "onclick", metodo: onClickAlterarDadosRecebedor, tamanho: "7", icone: "" },
        { descricao: "Imagens dos canhotos", id: guid(), evento: "onclick", metodo: onClickAlterarImagensCanhotos, tamanho: "7", icone: "" },
        { descricao: "Remover", id: guid(), evento: "onclick", metodo: onClickRemoverParada, tamanho: "7", icone: "", enable: false }
    ];

    var linhasPorPaginas = 5;

    var header = [
        { data: "Codigo", title: "Código" },
        { data: "Destinatario", title: "Destinatário" },
        { data: "Imagem", title: "Imagem" },
        { data: "DadosRecebedor", title: "Dados Recebedor" },
    ];

    $("#" + _wizardLoteComprovanteEntrega.Etapa1.idTab + " .step").attr("class", "step yellow");
    $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab).removeAttr("data-bs-toggle");

    _gridCargaEntregaAdicionada = new BasicDataTable(_dadosComprovanteEtapa.ListaCargaEntrega.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCargaEntregaAdicionada.CarregarGrid([]);
}


// region: ACTIONS DA GRID DE PESQUISA  

function buscarLotesComprovanteEntrega() {
    var visualizar = { descricao: "Visualizar", id: guid(), evento: "onclick", metodo: function (pedidoGrid) { VisualizarClick(pedidoGrid, false); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [visualizar], tamanho: 10 };

    _gridLoteComprovanteEntrega = new GridView(_pesquisaLoteComprovanteEntrega.Pesquisar.idGrid, "LoteComprovanteEntrega/Pesquisa", _pesquisaLoteComprovanteEntrega, menuOpcoes, null, null, null);
    _gridLoteComprovanteEntrega.CarregarGrid();
}

// region: ACTIONS DO CRUD

function AdicionarClick(e, sender) {
    if (isListaCargaEntregaCompletaParaAdicionar()) {
        executarReST("LoteComprovanteEntrega/Adicionar", obterLoteComprovanteEntregaSalvar(), function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote criado com sucesso");
                    enviarImagens(r.Data.Codigo);
                    _dadosComprovanteEtapa.Carga.val(r.Data.Carga);
                    _dadosComprovanteEtapa.Codigo.val(r.Data.Codigo);
                    _gridLoteComprovanteEntrega.CarregarGrid();
                    etapa2Liberada(true);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha", "Todas os canhotos devem ter Imagem e Dados do Recebedor");
    }

}

function AtualizarLoteClick(e, sender) {
    if (isListaCargaEntregaCompletaParaAdicionar()) {
        executarReST("LoteComprovanteEntrega/Atualizar", obterLoteComprovanteEntregaSalvar(), function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote criado com sucesso");
                    enviarImagens(r.Data.Codigo);
                    _dadosComprovanteEtapa.Carga.val(r.Data.Carga);
                    _dadosComprovanteEtapa.Codigo.val(r.Data.Codigo);
                    _gridLoteComprovanteEntrega.CarregarGrid();
                    etapa2Liberada(true);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha", "Todas os canhotos devem ter Imagem e Dados do Recebedor");
    }

}

function VisualizarClick(registroSelecionado) {
    limparCamposLoteComprovanteEntrega();

    _dadosComprovanteEtapa.Codigo.val(registroSelecionado.Codigo);
    $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab + " .step").attr("class", "step yellow");

    executarReST("LoteComprovanteEntrega/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_dadosComprovanteEtapa, { Data: retorno.Data });
                _dadosComprovanteEtapa.Carga.val(retorno.Data.Carga);
                _dadosComprovanteEtapa.Carga.enable(false);
                _dadosComprovanteEtapa.AdicionarCargaEntrega.enable(false);
                _dadosComprovanteEtapa.ListaCargaEntrega.val(retorno.Data.ListaCargaEntrega);
                recarregarGridCargaEntrega();

                etapa2Liberada(false);

                _CRUDLoteComprovanteEntrega.Adicionar.visible(false);
                _CRUDLoteComprovanteEntrega.Adicionar.enable(false);
                _CRUDLoteComprovanteEntrega.Atualizar.visible(true);
                _CRUDLoteComprovanteEntrega.Atualizar.enable(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function etapa2Liberada(click) {
    $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab).attr("data-bs-toggle", "tab");
    _wizardLoteComprovanteEntrega.Etapa2.eventClick = function () { EfetuarPesquisaIntegracaoLote(_dadosComprovanteEtapa.Codigo.val()) };
    $("#" + _wizardLoteComprovanteEntrega.Etapa1.idTab + " .step").attr("class", "step green");

    if (click) {
        $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab).click();
    } else {
        $("#" + _wizardLoteComprovanteEntrega.Etapa1.idTab).click();
    }
}

function podeEditarLote() {
    // TODO: Verificar aqui se está editando ou não. Se sim, ver se o status é editável
    return true;

    let codigoLote = _dadosComprovanteEtapa.Codigo.val();
    return codigoLote;
}

function LimparClick() {
    exibirConfirmacao("Limpar", "Tem certeza que deseja limpar todos os dados do cadastro?", function () {
        limparListaCargaEntrega();
        _dadosComprovanteEtapa.Codigo.val(null);
        LimparCampos(_dadosComprovanteEtapa);
        _dadosComprovanteEtapa.Carga.enable(true);
        _dadosComprovanteEtapa.AdicionarCargaEntrega.enable(true);

        _CRUDLoteComprovanteEntrega.Adicionar.visible(true);
        _CRUDLoteComprovanteEntrega.Adicionar.enable(true);
        _CRUDLoteComprovanteEntrega.Atualizar.visible(false);
        _CRUDLoteComprovanteEntrega.Atualizar.enable(false);
    })
}

function adicionarCargaEntrega(cargaEntrega) {
    _dadosComprovanteEtapa.ListaCargaEntrega.val().push(cargaEntrega);
    recarregarGridCargaEntrega();
}

function onChooseCarga(carga) {
    _dadosComprovanteEtapa.Carga.codEntity(carga.Codigo);
    _dadosComprovanteEtapa.Carga.val(carga.CodigoCargaEmbarcador);
}

function onChooseCargaEntrega(cargaEntrega) {

    let canhotoExistente = _dadosComprovanteEtapa.ListaCargaEntrega.val().find(n => n.Codigo == cargaEntrega.Codigo);

    if (canhotoExistente) {
        exibirMensagem(tipoMensagem.falha, "Erro", "Essa parada já foi adicionada nesse lote");
        return;
    }

    executarReST("LoteComprovanteEntrega/ObterDetalhesCargaEntrega", { Codigo: cargaEntrega.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                adicionarCargaEntrega(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function onClickAlterarGeolocalizacao(cargaEntrega) {
    let index = _gridCargaEntregaAdicionada.BuscarRegistros().indexOf(cargaEntrega);
    let cargaEntregaDaEntidade = _dadosComprovanteEtapa.ListaCargaEntrega.val()[index];
    exibirModalGeolocalizacaoCargaEntrega(cargaEntregaDaEntidade);
}

function onClickAlterarDadosRecebedor(cargaEntrega) {
    let index = _gridCargaEntregaAdicionada.BuscarRegistros().indexOf(cargaEntrega);
    let cargaEntregaDaEntidade = _dadosComprovanteEtapa.ListaCargaEntrega.val()[index];
    exibirModalDadosRecebedor(cargaEntregaDaEntidade);
}

function onClickAlterarImagensCanhotos(cargaEntrega) {
    let index = _gridCargaEntregaAdicionada.BuscarRegistros().indexOf(cargaEntrega);
    let cargaEntregaDaEntidade = _dadosComprovanteEtapa.ListaCargaEntrega.val()[index];
    exibirModalListaCanhotos(cargaEntregaDaEntidade);
}

function onClickRemoverParada(cargaEntrega) {
    if (!podeEditarLote()) {
        exibirMensagem(tipoMensagem.atencao, "Inválido", "Não é possível remover uma parada de um lote já cadastrado");
        return;
    }

    exibirConfirmacao("Remover parada", "Tem certeza que deseja remover essa parada?", async function () {
        let index = _gridCargaEntregaAdicionada.BuscarRegistros().indexOf(cargaEntrega);
        _dadosComprovanteEtapa.ListaCargaEntrega.val().splice(index, 1);
        recarregarGridCargaEntrega();
    })

}

function recarregarGridCargaEntrega() {
    var listaCargaEntrega = obterListaCargaEntrega();

    const dadosGrid = listaCargaEntrega.map(cargaEntrega => {
        return {
            Codigo: cargaEntrega.Codigo,
            CodigoCanhoto: cargaEntrega.CodigoCanhoto,
            Numero: cargaEntrega.Numero,
            Serie: cargaEntrega.Serie,
            Origem: cargaEntrega.Origem,
            Destinatario: cargaEntrega.Destinatario,
            DataEmissao: cargaEntrega.DataEmissao,
            Imagem: cargaEntrega.Canhotos.find(c => !c.Imagem) ? "Não" : "Sim",
            DadosRecebedor: isDadosRecebedorPreenchido(cargaEntrega.DadosRecebedor) ? "Sim" : "Não",
        };
    });

    _gridCargaEntregaAdicionada.CarregarGrid(dadosGrid);
}

function obterListaCargaEntrega() {
    return _dadosComprovanteEtapa.ListaCargaEntrega.val().slice();
}

function limparListaCargaEntrega() {
    _dadosComprovanteEtapa.ListaCargaEntrega.val([]);
    recarregarGridCargaEntrega();
}

function limparCamposLoteComprovanteEntrega() {
    LimparCampos(_dadosComprovanteEtapa);
}

/*
 * Retorna o objeto que vai ser enviado para o backend
 */
function obterLoteComprovanteEntregaSalvar() {

    return {
        Codigo: _dadosComprovanteEtapa.Codigo.val(),
        ListaCargaEntrega: JSON.stringify(_dadosComprovanteEtapa.ListaCargaEntrega.val()),
        Carga: _dadosComprovanteEtapa.Carga.codEntity(),
    }
}

function enviarImagens(codigoLoteComprovanteEntrega) {
    // Para cada parada adicionada e para canhoto nela, enviar sua imagem para o server
    for (let cargaEntrega of _dadosComprovanteEtapa.ListaCargaEntrega.val()) {
        for (let canhoto of cargaEntrega.Canhotos) {
            if (!canhoto.ImagemFile) {
                break;
            }
            var formData = new FormData();
            formData.append("upload", canhoto.ImagemFile);

            var data = {
                Codigo: canhoto.Codigo,
                DataEnvioCanhoto: Global.DataHoraAtual()
            }
            enviarArquivo("Canhoto/EnviarImagemCanhoto?callback=?", data, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

                }
                else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 20000);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        });
        }
    }
}

function isListaCargaEntregaCompletaParaAdicionar() {
    for (let cargaEntrega of _dadosComprovanteEtapa.ListaCargaEntrega.val()) {
        let temDadosRecebedor = isDadosRecebedorPreenchido(cargaEntrega.DadosRecebedor);
        let temTodasImagesNosCanhotos = !cargaEntrega.Canhotos.find(c => !c.Imagem);
        if (!temDadosRecebedor || !temTodasImagesNosCanhotos) {
            return false;
        }
    }

    return true;
}

function isDadosRecebedorPreenchido(dadosRecebedor) {
    return dadosRecebedor.Nome && dadosRecebedor.Nome != ""
        && dadosRecebedor.CPF && dadosRecebedor.CPF != ""
        && dadosRecebedor.DataEntrega && dadosRecebedor.DataEntrega != "";
}

function carregarHtmlWizardLoteComprovante() {
    return new Promise((resolve) => {
        $.get('Content/Static/Carga/LoteComprovanteEntrega/Wizard.html', function (htmlWizard) {
            $('#wizardLoteComprovanteEntrega').html(htmlWizard);
            resolve();
        });
    });
}

function carregarHtmlModaisLoteComprovante() {
    return new Promise((resolve) => {
        $.get('Content/Static/Carga/LoteComprovanteEntrega/Modal.html', function (htmlModais) {
            $('#divModaisLoteComprovanteEntrega').html(htmlModais);
            resolve();
        });
    });
}