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


//*******MAPEAMENTO KNOUCKOUT*******

var _condicaoPagamento;
var _pesquisaCondicaoPagamento;
var _gridCondicaoPagamento;

var CondicaoPagamento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ text: "Código:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.QuantidadeParcelas = PropertyEntity({ text: "Qnt. Parcela", val: ko.observable(1), options: _parcelas, def: 1, text: "*Qtd. Parcelas: ", required: false });
    this.IntervaloDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", required: false, maxlength: 100, getType: typesKnockout.string });
    this.DiasParaPrimeiroVencimento = PropertyEntity({ text: "Dias para primeiro vencimento: ", required: false, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AtualizarDACTE = PropertyEntity({ eventClick: AtualizarDACTEClick, type: types.event, text: "Atualizar DACTE", visible: ko.observable(false) });    
    this.AtualizarDataVencimento = PropertyEntity({ eventClick: AtualizarDataVencimentoClick, type: types.event, text: "Atualizar Vcto.", visible: ko.observable(false) });
    this.EnviarWhatsApp = PropertyEntity({ eventClick: EnviarWhatsAppClick, type: types.event, text: "Enviar Whats App", visible: ko.observable(false) });
}

var PesquisaCondicaoPagamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCondicaoPagamento.CarregarGrid();
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
}

var _parcelas = [
    { value: 1, text: "1X" },
    { value: 2, text: "2X" },
    { value: 3, text: "3X" },
    { value: 4, text: "4X" },
    { value: 5, text: "5X" },
    { value: 6, text: "6X" },
    { value: 7, text: "7X" },
    { value: 8, text: "8X" },
    { value: 9, text: "9X" },
    { value: 10, text: "10X" },
    { value: 11, text: "11X" },
    { value: 12, text: "12X" },
    { value: 13, text: "13X" },
    { value: 14, text: "14X" },
    { value: 15, text: "15X" },
    { value: 16, text: "16X" },
    { value: 17, text: "17X" },
    { value: 18, text: "18X" },
    { value: 19, text: "19X" },
    { value: 20, text: "20X" },
    { value: 21, text: "21X" },
    { value: 22, text: "22X" },
    { value: 23, text: "23X" },
    { value: 24, text: "24X" },
    { value: 25, text: "25X" },
    { value: 26, text: "26X" },
    { value: 27, text: "27X" },
    { value: 28, text: "28X" },
    { value: 29, text: "29X" },
    { value: 30, text: "30X" },
    { value: 31, text: "31X" },
    { value: 32, text: "32X" },
    { value: 33, text: "33X" },
    { value: 34, text: "34X" },
    { value: 35, text: "35X" },
    { value: 36, text: "36X" },
    { value: 37, text: "37X" },
    { value: 38, text: "38X" },
    { value: 39, text: "39X" },
    { value: 40, text: "40X" },
    { value: 41, text: "41X" },
    { value: 42, text: "42X" },
    { value: 43, text: "43X" },
    { value: 44, text: "44X" },
    { value: 45, text: "45X" },
    { value: 46, text: "46X" },
    { value: 47, text: "47X" },
    { value: 48, text: "48X" },
    { value: 49, text: "49X" },
    { value: 50, text: "50X" }
];

//*******EVENTOS*******
function loadCondicaoPagamento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCondicaoPagamento = new PesquisaCondicaoPagamento();
    KoBindings(_pesquisaCondicaoPagamento, "knockoutPesquisaCondicaoPagamento", false, _pesquisaCondicaoPagamento.Pesquisar.id);

    // Instancia objeto principal
    _condicaoPagamento = new CondicaoPagamento();
    KoBindings(_condicaoPagamento, "knockoutCondicaoPagamento");

    HeaderAuditoria("CondicaoPagamento", _condicaoPagamento);

    // Inicia busca
    buscarCondicaoPagamento();
}

function AtualizarDataVencimentoClick(e, sender) {
    Salvar(_condicaoPagamento, "CondicaoPagamento/ReprocessarDataPreviaVencimento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processamento concluido com sucesso.");
                _gridCondicaoPagamento.CarregarGrid();
                limparCamposCondicaoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function EnviarWhatsAppClick(e, sender) {
    Salvar(_condicaoPagamento, "CondicaoPagamento/EnviarWhatsApp", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Envio realizado com sucesso.");
                //_gridCondicaoPagamento.CarregarGrid();
                //limparCamposCondicaoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarDACTEClick(e, sender) {
    Salvar(_condicaoPagamento, "CondicaoPagamento/ReprocessarDACTEsCTesImportados", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processamento concluido com sucesso.");
                _gridCondicaoPagamento.CarregarGrid();
                limparCamposCondicaoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function adicionarClick(e, sender) {
    Salvar(_condicaoPagamento, "CondicaoPagamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCondicaoPagamento.CarregarGrid();
                limparCamposCondicaoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    //executarDownload("CondicaoPagamento/Atualizar", null, null, function (arg) {
    //    var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
    //    if (retorno.Success) {
    //        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
    //    } else
    //        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    //});
    Salvar(_condicaoPagamento, "CondicaoPagamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCondicaoPagamento.CarregarGrid();
                limparCamposCondicaoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_condicaoPagamento, "CondicaoPagamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCondicaoPagamento.CarregarGrid();
                    limparCamposCondicaoPagamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCondicaoPagamento();
}

function editarCondicaoPagamentoClick(itemGrid) {
    // Limpa os campos
    limparCamposCondicaoPagamento();

    // Seta o codigo do objeto
    _condicaoPagamento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_condicaoPagamento, "CondicaoPagamento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCondicaoPagamento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _condicaoPagamento.Atualizar.visible(true);
                _condicaoPagamento.Excluir.visible(true);
                _condicaoPagamento.Cancelar.visible(true);
                _condicaoPagamento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCondicaoPagamento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCondicaoPagamentoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCondicaoPagamento = new GridView(_pesquisaCondicaoPagamento.Pesquisar.idGrid, "CondicaoPagamento/Pesquisa", _pesquisaCondicaoPagamento, menuOpcoes, null);
    _gridCondicaoPagamento.CarregarGrid();
}

function limparCamposCondicaoPagamento() {
    _condicaoPagamento.Atualizar.visible(false);
    _condicaoPagamento.Cancelar.visible(false);
    _condicaoPagamento.Excluir.visible(false);
    _condicaoPagamento.Adicionar.visible(true);
    LimparCampos(_condicaoPagamento);
}