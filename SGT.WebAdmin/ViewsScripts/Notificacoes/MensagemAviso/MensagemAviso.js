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
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAnexo;
var _gridMensagemAviso;
var _mensagemAviso;
var _pesquisaMensagemAviso;
var _opcoesTipoServicoMultisoftware = [];

var PesquisaMensagemAviso = function () {
    this.Titulo = PropertyEntity({ text: "Título:" });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "*Situação: " });
   

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMensagemAviso.CarregarGrid();
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

var MensagemAviso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: "*Título:", required: true, maxlength: 200 });
    this.Mensagem = PropertyEntity({ text: "*Mensagem:", required: true, maxlength: 10000 });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", required: true, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", required: true, getType: typesKnockout.date });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoServicoMultisoftware = PropertyEntity({ val: ko.observable(''), options: ko.observable(_opcoesTipoServicoMultisoftware), def: '', text: "*Exibir em: ", visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 1000, val: ko.observable('') });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DescricaoArquivo = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Arquivo.val.subscribe(function (novoValor) { _mensagemAviso.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);
    this.AdicionarArquivo = PropertyEntity({ eventClick: adicionarAnexoAvisoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadMensagemAviso() {
    _pesquisaMensagemAviso = new PesquisaMensagemAviso();
    KoBindings(_pesquisaMensagemAviso, "knockoutPesquisaMensagemAviso");

    _mensagemAviso = new MensagemAviso();
    KoBindings(_mensagemAviso, "knockoutCadastroMensagemAviso");

    CarregarOpcoesTipoServicoMultisoftware();
    BuscarMensagemAviso();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liAvisoAnexos").hide();
    }

    loadTabelaAnexo();
}

function loadTabelaAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: function () { return _mensagemAviso.Codigo.val(); } };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_mensagemAviso.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function adicionarAnexoAvisoClick() {
    var arquivo = document.getElementById(_mensagemAviso.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _mensagemAviso.DescricaoArquivo.val(),
        NomeArquivo: _mensagemAviso.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var codigo = _mensagemAviso.Codigo.val();
    if (codigo > 0) {
        enviarAnexos(codigo, [anexo]);
    } else {
        adicionarAnexosTemporarios(anexo);
    }

    _mensagemAviso.DescricaoArquivo.val(_mensagemAviso.DescricaoArquivo.def);
    _mensagemAviso.NomeArquivo.val(_mensagemAviso.NomeArquivo.def);

    arquivo.value = null;
}

function AdicionarClick(e, sender) {
    Salvar(_mensagemAviso, "MensagemAviso/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                EnviarAnexosAoAdicionarMensagem(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_mensagemAviso, "MensagemAviso/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMensagemAviso.CarregarGrid();
                LimparCamposMensagemAviso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirClick(e, sender) {
    var codigo = _mensagemAviso.Codigo.val();

    exibirConfirmacao("Confirmação", "Deseja realmente excluir esta mensagem de aviso?", function () {
        executarReST("MensagemAvisoAnexo/ExcluirTodosAnexos", { Codigo: codigo }, function () {
            ExcluirPorCodigo(_mensagemAviso, "MensagemAviso/ExcluirPorCodigo", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                        _gridMensagemAviso.CarregarGrid();
                        LimparCamposMensagemAviso();

                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }

            }, null);
        });
    });
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("MensagemAvisoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    var codigo = parseInt(registroSelecionado.Codigo) || 0;

    if (codigo == 0) {
        removerAnexoDaGrid(registroSelecionado.Codigo);
        return;
    }

    executarReST("MensagemAvisoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoDaGrid(registroSelecionado.Codigo);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function CancelarClick(e) {
    LimparCamposMensagemAviso();
}

//*******MÉTODOS*******


function EnviarAnexosAoAdicionarMensagem(codigo) {
    let mensagemSucesso = function () {
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
        _gridMensagemAviso.CarregarGrid();
        LimparCamposMensagemAviso();
    }

    let formData = obterFormDataAnexo(obterAnexos());

    if (formData) {
        enviarArquivo("MensagemAvisoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    mensagemSucesso();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Mensagem adicionada mas ocorreu uma falha no envio de anexo.", retorno.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else {
        mensagemSucesso();
    }
}

function CarregarOpcoesTipoServicoMultisoftware() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _opcoesTipoServicoMultisoftware = [
            { text: "MultiEmbarcador", value: EnumTipoServicoMultisoftware.MultiEmbarcador },
            { text: "Portal do Transportador", value: EnumTipoServicoMultisoftware.MultiCTe },
            { text: "Portal do Fornecedor", value: EnumTipoServicoMultisoftware.Fornecedor }
        ];
        _mensagemAviso.TipoServicoMultisoftware.options(_opcoesTipoServicoMultisoftware);
        _mensagemAviso.TipoServicoMultisoftware.def = EnumTipoServicoMultisoftware.MultiEmbarcador;
        _mensagemAviso.TipoServicoMultisoftware.val(EnumTipoServicoMultisoftware.MultiEmbarcador);
        _mensagemAviso.TipoServicoMultisoftware.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _opcoesTipoServicoMultisoftware = [
            { text: "MultiTMS", value: EnumTipoServicoMultisoftware.MultiTMS }
        ];
        _mensagemAviso.TipoServicoMultisoftware.options(_opcoesTipoServicoMultisoftware);
        _mensagemAviso.TipoServicoMultisoftware.def = EnumTipoServicoMultisoftware.MultiTMS;
        _mensagemAviso.TipoServicoMultisoftware.val(EnumTipoServicoMultisoftware.MultiTMS);
    }
}

function EditarModeloCarroceria(marcaEquipamentoGrid) {
    LimparCamposMensagemAviso();
    _mensagemAviso.Codigo.val(marcaEquipamentoGrid.Codigo);
    BuscarPorCodigo(_mensagemAviso, "MensagemAviso/BuscarPorCodigo", function (arg) {
        _pesquisaMensagemAviso.ExibirFiltros.visibleFade(false);
        _mensagemAviso.Atualizar.visible(true);
        _mensagemAviso.Cancelar.visible(true);
        _mensagemAviso.Excluir.visible(true);
        _mensagemAviso.Adicionar.visible(false);
        _mensagemAviso.Anexos.val(arg.Data.Anexos);
    }, null);
}


function BuscarMensagemAviso() {
    var editar = { descricao: "Editar", id: (guid()), evento: "onclick", metodo: EditarModeloCarroceria, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMensagemAviso = new GridView(_pesquisaMensagemAviso.Pesquisar.idGrid, "MensagemAviso/Pesquisa", _pesquisaMensagemAviso, menuOpcoes, null);
    _gridMensagemAviso.CarregarGrid();
}


function LimparCamposMensagemAviso() {
    _mensagemAviso.Atualizar.visible(false);
    _mensagemAviso.Cancelar.visible(false);
    _mensagemAviso.Excluir.visible(false);
    _mensagemAviso.Adicionar.visible(true);
    LimparCampos(_mensagemAviso);
    _mensagemAviso.Anexos.val([]);

    Global.ResetarAbas();
}

function obterFormDataAnexo(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function adicionarAnexosTemporarios(anexo) {
    var anexosGrid = obterAnexos();
    anexosGrid.push(anexo);
    _mensagemAviso.Anexos.val(anexosGrid);
}

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("MensagemAvisoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _mensagemAviso.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterAnexos() {
    return _mensagemAviso.Anexos.val().slice();
}

function recarregarGridAnexo() {
    var anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function removerAnexoDaGrid(codigo) {
    var anexos = obterAnexos();

    for (var i = 0; i < anexos.length; i++) {
        if (anexos[i].Codigo == codigo) {
            anexos.splice(i, 1);
            break;
        }
    }

    _mensagemAviso.Anexos.val(anexos);
}