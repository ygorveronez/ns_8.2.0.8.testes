/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaGuarita.js" />
/// <reference path="../../GestaoPatio/FluxoPatioPesagem/Pesagem.js" />
/// <reference path="../../GestaoPatio/FluxoPatioPesagem/PesagemFinal.js" />
/// <reference path="ChegadaVeiculo.js" />
/// <reference path="InicioViagemGuarita.js" />
/// <reference path="DetalhesCargaEmail.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridGuarita;
var _pesquisaGuarita;
var _gridCargaAnexoGuarita;
var _knoutCargaAnexoGuarita;
var _modalChegadaVeiculo;
var _modalCargaAnexo;
/*
 * Declaração das Classes
 */

var CargaAnexoGuarita = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
}

var PesquisaGuarita = function () {
    var dataDiaAnterior = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataDiaSeguinte = moment().add(1, 'days').format("DD/MM/YYYY");

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaGuarita.AguardandoLiberacao.toString()), options: EnumSituacaoCargaGuarita.obterOpcoesPesquisa(), def: EnumSituacaoCargaGuarita.AguardandoLiberacao.toString(), text: "Situação:" });

    this.DataInicialCarregamento = PropertyEntity({ text: "Data inicio Carregamento:", getType: typesKnockout.date, val: ko.observable(dataDiaAnterior), def: dataDiaAnterior });
    this.DataFinalCarregamento = PropertyEntity({ text: "Data limite Carregamento:", getType: typesKnockout.date, val: ko.observable(dataDiaSeguinte), def: dataDiaSeguinte });
    this.DataInicialChegada = PropertyEntity({ text: "Data inicio Chegada:", getType: typesKnockout.date, val: ko.observable()});
    this.DataFinalChegada = PropertyEntity({ text: "Data limite Chegada:", getType: typesKnockout.date, val: ko.observable() });

    this.Transportadores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Transportadores, idBtnSearch: guid() });
    this.Motoristas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Motoristas, idBtnSearch: guid() });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Veiculos, idBtnSearch: guid() });
    this.DataAgendada = PropertyEntity({ text: "Data Agendada:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, idBtnSearch: guid() });
    this.ListaCodigoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Número da Carga:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });

    this.DataInicialCarregamento.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinalCarregamento.dateRangeInit = this.DataInicialCarregamento;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGuarita.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "grid-guarita", visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    //Legendas
    var classe = "col-xs-12 col-sm-6 col-md-4 legenda-item";

    this.CarregamentoCancelado = PropertyEntity({ text: "Carregamento cancelado", cssClass: classe });
    this.AguardandoEntrada = PropertyEntity({ text: "Aguardando entrada", cssClass: classe });
    this.EntradaSaidaLiberada = PropertyEntity({ text: "Entrada/Saída liberada", cssClass: classe });
    this.AguardandoChegada = PropertyEntity({ text: "Aguardando chegada", cssClass: classe });
    this.CarregamentoAtrasado = PropertyEntity({ text: "Carregamento atrasado", cssClass: classe });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridGuarita() {
    var chegadaVeiculo = { descricao: "Veículo Chegou", id: guid(), evento: "onclick", metodo: chegadaDoVeiculoClick, tamanho: "10", icone: "", visibilidade: VisibilidadeChegadaVeiculo };
    var liberar = { descricao: "Liberar", id: guid(), evento: "onclick", metodo: liberarCargaClick, tamanho: "10", icone: "", visibilidade: VisibilidadeLiberacao };
    var saida = { descricao: "Saída do Veículo", id: guid(), evento: "onclick", metodo: saidaVeiculoClick, tamanho: "10", icone: "", visibilidade: VisibilidadeSaidaVeiculo };
    var downloadDetalhesCarga = { descricao: "Baixar Detalhes da Carga", id: guid(), evento: "onclick", metodo: downloadDetalhesCargaClick, tamanho: "10", icone: "", visibilidade: VisibilidadeCarga };
    var enviarDetalhesCargaEmail = { descricao: "Enviar Detalhes da Carga por E-mail", id: guid(), evento: "onclick", metodo: abrirTelaEnvioDetalhesCargaEmailClick, tamanho: "10", icone: "", visibilidade: VisibilidadeCarga };
    var observacaoFluxoPatio = { descricao: "Observação do Fluxo de Pátio", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { exibirObservacaoFluxoPatio(registroSelecionado.ObservacaoFluxoPatio); }, tamanho: "10", icone: "", visibilidade: VisibilidadeObservacaoFluxoPatio };
    var pesagemInicial = { descricao: "Pesagem Inicial", id: guid(), evento: "onclick", metodo: pesagemInicialClick, tamanho: "10", icone: "", visibilidade: VisibilidadePesagemInicial };
    var pesagemFinal = { descricao: "Pesagem Final", id: guid(), evento: "onclick", metodo: pesagemFinalClick, tamanho: "10", icone: "", visibilidade: VisibilidadePesagemFinal };
    var denegarChegada = { descricao: "Denegar Chegada", id: guid(), evento: "onclick", metodo: exibirModalDenegarChegada, tamanho: "10", icone: "", visibilidade: VisibilidadeDenegarChegada };
    var observacaoGuarita = { descricao: "Observação Guarita", id: guid(), evento: "onclick", metodo: exibirObservacaoGuaritaClick, tamanho: "10", icone: "", visibilidade: true };
    var anexos = { descricao: "Anexos", id: guid(), evento: "onclick", metodo: exibirAnexosGuaritaClick, tamanho: "10", icone: "", visibilidade: obterVisibilidadeAnexosGuarita };
    var alterarChegada = { descricao: "Alterar Chegada", id: guid(), evento: "onclick", metodo: alterarDataChegaVeiculo, tamanho: "10", icone: "", visibilidade: obterVisibilidadeAlterarCarga };
    var downloadOrdemColeta = { descricao: "Imprimir Ordem de Coleta", id: guid(), evento: "onclick", metodo: downloadOrdemColetaClick, tamanho: "10", icone: "", visibilidade: obterVisibilidadeImpressaoOrdemColeta };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [downloadDetalhesCarga, enviarDetalhesCargaEmail, liberar, observacaoFluxoPatio, saida, chegadaVeiculo, pesagemInicial, pesagemFinal, denegarChegada, observacaoGuarita, anexos, alterarChegada, downloadOrdemColeta] };

    _gridGuarita = new GridView(_pesquisaGuarita.Pesquisar.idGrid, "Guarita/Pesquisa", _pesquisaGuarita, menuOpcoes, { column: 1, dir: orderDir.desc }, 10);
    _gridGuarita.SetPermitirEdicaoColunas(true);
    _gridGuarita.SetSalvarPreferenciasGrid(true);
    _gridGuarita.CarregarGrid();
}

function loadGuarita() {
    _pesquisaGuarita = new PesquisaGuarita();
    KoBindings(_pesquisaGuarita, "knockoutPesquisaGuarita", _pesquisaGuarita.Pesquisar.id);

    _knoutCargaAnexoGuarita = new CargaAnexoGuarita();

    HeaderAuditoria("CargaJanelaCarregamentoGuarita", _pesquisaGuarita);
    new BuscarTransportadores(_pesquisaGuarita.Transportadores, null, null, true);
    new BuscarVeiculos(_pesquisaGuarita.Veiculos);
    new BuscarMotoristas(_pesquisaGuarita.Motoristas);
    new BuscarCargas(_pesquisaGuarita.ListaCodigoCarga);
    new BuscarTiposOperacao(_pesquisaGuarita.TipoOperacao);
    new BuscarTiposdeCarga(_pesquisaGuarita.TipoCarga);
    new BuscarFilial(_pesquisaGuarita.Filial);
    new BuscarClientes(_pesquisaGuarita.Destinatario);

    loadEnvioDetalhesCargaEmail();
    loadObservacaoFluxoPatio();
    LoadPesagemFluxoPatio();
    loadDadosCarga();
    loadDenegarChegada();
    loadGridGuarita();
    loadObservacaoGuarita();
    loadAnexoCargaGuarita();

    //_modalChegadaVeiculo = new bootstrap.Modal(document.getElementById("divModalChegadaVeiculo"), { backdrop: 'static', keyboard: true });
    _modalCargaAnexo = new bootstrap.Modal(document.getElementById("divModalCargaAnexo"), { backdrop: 'static', keyboard: true });
    loadAlterarChegada();
}

function loadAnexoCargaGuarita() {
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadCargaAnexoGuaritaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload] };

    _gridCargaAnexoGuarita = new GridView("tblAnexosCarga", "CargaAnexo/PesquisaAnexo", _knoutCargaAnexoGuarita, menuOpcoes);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadCargaAnexoGuaritaClick(anexo) {
    var dados = { Codigo: anexo.Codigo };

    executarDownload("CargaAnexo/DownloadAnexo", dados);
}

function chegadaDoVeiculoClick(cargaGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente informar a chegada do veículo para a carga " + cargaGrid.NumeroCarga + " ?", function () {
        executarReST("Guarita/InformarChegadaVeiculo", { Codigo: cargaGrid.Codigo, ChegadaInformadaNaGuarita: true }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Chegada informada com sucesso!");
                    _gridGuarita.CarregarGrid();
                   /* _modalChegadaVeiculo.hide();*/
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

function downloadDetalhesCargaClick(e) {
    executarDownload("Guarita/DownloadDetalhesCarga", { Codigo: e.Codigo });
}

function liberarCargaClick(cargaGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente liberar a carga " + cargaGrid.NumeroCarga + " para entrada no Centro?", function () {
        if (cargaGrid.PermiteInformarMotoristaEVeiculo) {
            var codigoCargaOuPreCarga = cargaGrid.Carga > 0 ? cargaGrid.Carga : cargaGrid.PreCarga;
            exibirModalMotoristaVeiculo(codigoCargaOuPreCarga, cargaGrid.Codigo);
        }
        else {
            liberarCarga(cargaGrid);
        }
    });
}

function liberarCarga(cargaGrid) {
    executarReST("Guarita/LiberarCarga", { Codigo: cargaGrid.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Carga liberada com sucesso!");
                _gridGuarita.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function saidaVeiculoClick(cargaGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente informar a saída do veículo do centro?", function () {
        executarReST("Guarita/SaidaVeiculo", { Codigo: cargaGrid.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Veículo liberado com sucesso!");
                    _gridGuarita.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function pesagemInicialClick(cargaGrid) {
    abrirPesagemFluxoPatioClick(cargaGrid.Codigo);
}

function pesagemFinalClick(cargaGrid) {
    abrirPesagemFinalFluxoPatioClick(cargaGrid.Codigo);
}

function exibirModalMotoristaVeiculo(codigo, codigoGuarita) {
    BuscarDadosDaCarga(codigo, codigoGuarita);
}

function exibirObservacaoGuaritaClick(cargaGrid) {
    var data = {
        CodigoCarga: cargaGrid.Carga,
        ObservacaoGuarita: cargaGrid.ObservacaoGuarita
    };

    exibirObservacaoGuarita(data);
}

function exibirAnexosGuaritaClick(cargaGrid) {
    _knoutCargaAnexoGuarita.Codigo.val(cargaGrid.Carga);
    _gridCargaAnexoGuarita.CarregarGrid();
    
    _modalCargaAnexo.show();
}

function downloadOrdemColetaClick(e) {
    executarDownload("Guarita/DownloadOrdemColetaGuarita", { Codigo: e.Codigo });
}

/*
 * Declaração das Funções Privadas
 */

function VisibilidadeSaidaVeiculo(data) {
    return (data.Situacao == EnumSituacaoCargaGuarita.Liberada);
}

function VisibilidadeChegadaVeiculo(data) {
    if (data.Situacao == EnumSituacaoCargaGuarita.AgChegadaVeiculo) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeLiberacao(data) {
    if (data.Situacao == EnumSituacaoCargaGuarita.AguardandoLiberacao) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDenegarChegada(data) {
    return ((data.Situacao == EnumSituacaoCargaGuarita.AguardandoLiberacao || data.Situacao == EnumSituacaoCargaGuarita.AgChegadaVeiculo) && !data.ChegadaDenegada && data.GuaritaEntradaPermiteDenegarChegada);
}

function obterVisibilidadeAnexosGuarita(data) {
    return data.PermiteVisualizarAnexos;
}

function VisibilidadeCarga(data) {
    if (!data.SomentePreCarga) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeObservacaoFluxoPatio(data) {
    return Boolean(data.ObservacaoFluxoPatio);
}

const obterVisibilidadeAlterarCarga = (data) => data.PermitirAlterarDataChegadaVeiculo;

function VisibilidadePesagemInicial(data) {
    if (data.PossuiPesagemInicial)
        return false;
    else
        return true;
}

function VisibilidadePesagemFinal(data) {
    if (data.PossuiPesagemFinal)
        return false;
    else
        return true;
}

const obterVisibilidadeImpressaoOrdemColeta = (data) => data.PermiteImprimirOrdemColetaNaGuarita;