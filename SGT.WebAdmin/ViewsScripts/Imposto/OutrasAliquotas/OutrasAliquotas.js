/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../www/js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _outrasAliquotas;
var _pesquisaOutrasAliquotas;
var _gridOutrasAliquotas;
var _gridImpostosVinculados;
var _vincularImposto;
var _modalListarImpostos;

var OutrasAliquotas = function () {
    this.CST = PropertyEntity({ text: "*CST", type: types.string, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable(""), maxlength: 3, required: true });
    this.CodigoClassificacaoTributaria = PropertyEntity({ text: "*Código de Classificação Tributária", type: types.string, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable(""), maxlength: 6, required: true });
    this.CodigoIndicadorOperacao = PropertyEntity({ text: "Código Indicador de Operação (NFS-e)", type: types.string, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable(""), maxlength: 6, required: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação", issue: 121, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.AtivoInativo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: true, text: ko.observable("Inativo"), val: ko.observable("") });
    this.CodigoOutrasAliquotas = PropertyEntity({ type: types.map, val: ko.observable(0), getType: typesKnockout.int });
    this.ZerarBase = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: ko.observable("Zerar a base"), val: ko.observable(""), visible: ko.observable(true) });
    this.Exportacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: ko.observable("Exportação"), val: ko.observable(""), visible: ko.observable(true) });
    this.CalcularImpostoDocumento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: true, text: ko.observable("Somar imposto no documento"), val: ko.observable("") });

    this.Confirmar = PropertyEntity({ eventClick: SalvarOutrasAliquotas, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: EditarOutrasAliquotas, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(false) });
    this.Excluir_Aliquota = PropertyEntity({ eventClick: ExcluirOutrasAliquotas, type: types.event, text: "Excluir", idGrid: guid(), visible: ko.observable(false) });

    this.AtivoInativo.val.subscribe(function (value) {
        _outrasAliquotas.AtivoInativo.text(value ? "Ativar Configurações" : "Inativo");
    });

    this.CalcularImpostoDocumento.val.subscribe(function (value) {
        _outrasAliquotas.CalcularImpostoDocumento.text("Somar impostos no documento");
    });

    this.TituloModal = PropertyEntity({ text: ko.observable("Adicionar Configurações") });
    this.ExibirEditar = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
}

var VincularImposto = function () {
    let dataDoDia = Global.DataAtual();
    let dataAmanha = Global.Data(EnumTipoOperacaoDate.Add, 1, EnumTipoOperacaoObjetoDate.Days);

    this.TipoImposto = PropertyEntity({ text: "Tipo de Imposto", val: ko.observable(EnumTipoImposto.CBS), def: EnumTipoImposto.CBS, getType: typesKnockout.select, options: EnumTipoImposto.obterOpcoes(), enable: ko.observable(true)  });
    this.ExibirCBS = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExibirIBS = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.Aliquota = PropertyEntity({ text: "*Alíquota", type: types.decimal, def: 0.00, val: ko.observable(0), getType: typesKnockout.decimal, visible: ko.observable(false), required: true });
    this.Reducao = PropertyEntity({ text: "% Redução", type: types.decimal, val: ko.observable(0), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.DataVigenciaFinal = PropertyEntity({ text: "*Data Vigência Final: ", getType: typesKnockout.date, val: ko.observable(dataAmanha), def: dataAmanha, required: true });
    this.DataVigenciaInicio = PropertyEntity({ text: "*Data Vigência Inicial: ", getType: typesKnockout.date, val: ko.observable(dataDoDia), def: dataDoDia, required: true });
    this.InclusaoDocumento = PropertyEntity({ text: "Inclusão no Documento?", val: ko.observable(EnumSimNao.Nao), def: EnumSimNao.Nao, getType: typesKnockout.select, options: EnumSimNao.obterOpcoes(), cssClass: ko.observable("") });
    this.AliquotaUF = PropertyEntity({ text: "*Alíquota UF", type: types.decimal, val: ko.observable(0), getType: typesKnockout.decimal, visible: ko.observable(false), required: true });
    this.AliquotaMunicipio = PropertyEntity({ text: "Alíquota Município", type: types.decimal, val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.UF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*UF", issue: 121, idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Municipio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Município", issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ReducaoUF = PropertyEntity({ text: "% Redução UF", type: types.decimal, val: ko.observable(0), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.ReducaoMunicipio = PropertyEntity({ text: "% Redução Município", type: types.decimal, val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.CodigoOutrasAliquotasImposto = PropertyEntity({ type: types.map, val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoOutrasAliquotas = PropertyEntity({ type: types.map, val: ko.observable(0), getType: typesKnockout.int });

    this.DataVigenciaInicio.dateRangeLimit = this.DataVigenciaFinal;
    this.DataVigenciaFinal.dateRangeInit = this.DataVigenciaInicio;

    this.TipoImposto.val.subscribe(function (value) {
        ValidarVisualizacaoImposto(value);
    });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            SalvarOutrasAliquotasImposto();
        }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Excluir = PropertyEntity({
        eventClick: function (e) {
            ExcluirOutrasAliquotasImposto();
        }, type: types.event, text: "Excluir", idGrid: guid(), visible: ko.observable(false)
    });

    this.ExibirCBS.visible(true);
    this.TituloModal = PropertyEntity({ text: ko.observable("Vincular Imposto") });
}

//*******EVENTOS*******

var PesquisaOutrasAliquotas = function () {
    this.AdicionarConfiguracoes = PropertyEntity({
        eventClick: function (e) {
            LimparCampos(_outrasAliquotas);

            _outrasAliquotas.Confirmar.visible(true);
            _outrasAliquotas.Atualizar.visible(false);
            _outrasAliquotas.Excluir_Aliquota.visible(false);
            _outrasAliquotas.TituloModal.text("Adicionar Configurações");

            Global.abrirModal("divModalAdicionarConfiguracoes");
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var ModalListarImpostos = function () {
    this.TipoImposto = PropertyEntity({ text: "Tipo de Imposto", val: ko.observable(EnumTipoImposto.CBS), def: EnumTipoImposto.CBS, getType: typesKnockout.select, options: EnumTipoImposto.obterOpcoes(), cssClass: ko.observable("") });
    this.CodigoOutrasAliquotas = PropertyEntity({ type: types.map, val: ko.observable(0), getType: typesKnockout.int });

    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarOutrasAliquotasImposto };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    _gridImpostosVinculados = new GridView("grid-impostos-vinculados", "OutrasAliquotas/PesquisaAliquotaImposto", this, menuOpcoes);

    this.TipoImposto.val.subscribe(function (value) {
        _gridImpostosVinculados.CarregarGrid();
    });

    _gridImpostosVinculados.CarregarGrid();
}

function LimparDadosVincularImposto() {
    LimparCampos(_vincularImposto);
}

function ValidarVisualizacaoImposto(value) {
    if (value == EnumTipoImposto.CBS) {
        _vincularImposto.ExibirCBS.visible(true);
        _vincularImposto.ExibirIBS.visible(false);
    }
    else if (value == EnumTipoImposto.IBS) {
        _vincularImposto.ExibirCBS.visible(false);
        _vincularImposto.ExibirIBS.visible(true);
    }
}

function ValidarOutrasAliquotas() {
    const valorCST = _outrasAliquotas.CST.val();
    if (!/^\d{3}$/.test(valorCST)) {
        exibirMensagem(tipoMensagem.atencao, "CST inválido", "O campo CST deve conter exatamente 3 dígitos numéricos.");
        return false;
    }

    const valorCodigoClassificacaoTributaria = _outrasAliquotas.CodigoClassificacaoTributaria.val();
    if (!/^\d{6}$/.test(valorCodigoClassificacaoTributaria)) {
    exibirMensagem(tipoMensagem.atencao, "Código Classificação Tributária inválido", "O campo Código Classificação Tributária deve conter exatamente 6 dígitos numéricos.");
        return false
    }

    return true;
}

function SalvarOutrasAliquotas() {
    var valido = ValidarCamposObrigatorios(_outrasAliquotas);

    if (!valido)  return;

    if (!ValidarOutrasAliquotas()) return;

    executarReST("OutrasAliquotas/Adicionar",
    RetornarObjetoPesquisa(_outrasAliquotas), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_gridOutrasAliquotas) {
                    Global.fecharModal('divModalAdicionarConfiguracoes');

                    _gridOutrasAliquotas.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarOutrasAliquotas() {
    var valido = ValidarCamposObrigatorios(_outrasAliquotas);

    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");

    if (!ValidarOutrasAliquotas()) return;

    executarReST("OutrasAliquotas/Atualizar",
    RetornarObjetoPesquisa(_outrasAliquotas), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_gridOutrasAliquotas) {
                    Global.fecharModal('divModalAdicionarConfiguracoes');

                    _gridOutrasAliquotas.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function ExcluirOutrasAliquotas() {
    exibirConfirmacao("Excluir Alíquota", "Deseja realmente excluir esta alíquota?", function () {
        executarReST("OutrasAliquotas/ExcluirOutrasAliquotas",
            { CodigoOutrasAliquotas: _outrasAliquotas.CodigoOutrasAliquotas.val() },
            function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        if (_gridOutrasAliquotas) {
                            Global.fecharModal('divModalAdicionarConfiguracoes');

                            _gridOutrasAliquotas.CarregarGrid();
                        }
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, null);
    });
}

function SalvarOutrasAliquotasImposto() {
    executarReST("OutrasAliquotas/SalvarOutrasAliquotasImposto",
        RetornarObjetoPesquisa(_vincularImposto), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_gridOutrasAliquotas) {
                    Global.fecharModal('divModalVincularImposto');
                    LimparDadosVincularImposto();

                    _gridImpostosVinculados.CarregarGrid();
                    _vincularImposto.TituloModal.text("Vincular Imposto");
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function buscarOutrasAliquotas() {
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Vincular Imposto", id: guid(), metodo: VincularImpostoClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Impostos Vinculados", id: guid(), metodo: ImpostosVinculadosClick, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Editar", id: guid(), metodo: EditarClick, tamanho: "20", icone: "" });

    _gridOutrasAliquotas = new GridView("grid-outras-aliquotas", "OutrasAliquotas/Pesquisa", null, menuOpcoes);
    _gridOutrasAliquotas.CarregarGrid();
}

function EditarClick(item) {
    LimparCampos(_outrasAliquotas);

    _outrasAliquotas.CST.val(item.CST);
    _outrasAliquotas.CodigoClassificacaoTributaria.val(item.CodigoClassificacaoTributaria);
    _outrasAliquotas.CodigoIndicadorOperacao.val(item.CodigoIndicadorOperacao);
    _outrasAliquotas.TipoOperacao.val(item.TipoOperacao);
    _outrasAliquotas.AtivoInativo.val(item.StatusAtividade);
    _outrasAliquotas.ZerarBase.val(item.ZerarBase);
    _outrasAliquotas.Exportacao.val(item.Exportacao);
    _outrasAliquotas.CalcularImpostoDocumento.val(item.CalcularImpostoDocumento);
    _outrasAliquotas.CodigoOutrasAliquotas.val(item.Codigo);
    _outrasAliquotas.ExibirEditar.visible(true);
    _outrasAliquotas.TipoOperacao.codEntity(item.CodigoTipoOperacao);
    _outrasAliquotas.Atualizar.visible(true);
    _outrasAliquotas.Excluir_Aliquota.visible(true);
    _outrasAliquotas.Confirmar.visible(false);

    _outrasAliquotas.TituloModal.text("Editar Configurações");

    Global.abrirModal("divModalAdicionarConfiguracoes");
}

function VincularImpostoClick(item) {
    LimparDadosVincularImposto();

    _vincularImposto.Excluir.visible(false);
    _vincularImposto.TipoImposto.enable(true);
    _vincularImposto.CodigoOutrasAliquotas.val(item.Codigo);

    _vincularImposto.TituloModal.text("Vincular Imposto");

    Global.abrirModal("divModalVincularImposto");
}

function ImpostosVinculadosClick(item) {
    _modalListarImpostos.CodigoOutrasAliquotas.val(parseInt(item.Codigo));
    _gridImpostosVinculados.CarregarGrid();
    Global.abrirModal('divModalListarImpostosVinculados');
}

function loadOutrasAliquotas() {
    _pesquisaOutrasAliquotas = new PesquisaOutrasAliquotas();
    KoBindings(_pesquisaOutrasAliquotas, "knockoutPesquisaOutrasAliquotas");

    _outrasAliquotas = new OutrasAliquotas();
    KoBindings(_outrasAliquotas, "knockoutOutrasAliquotas");

    _vincularImposto = new VincularImposto();
    KoBindings(_vincularImposto, "knockoutVincularImposto");

    _modalListarImpostos = new ModalListarImpostos();
    KoBindings(_modalListarImpostos, "knockoutListarImpostosVinculados");

    BuscarTiposOperacao(_outrasAliquotas.TipoOperacao);
    BuscarEstados(_vincularImposto.UF);    
    BuscarLocalidades(_vincularImposto.Municipio, null, null, null, null, null, _vincularImposto.UF);

    buscarOutrasAliquotas();

    $("#" + _outrasAliquotas.CST.id).mask("000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _outrasAliquotas.CST.id).on("blur", function () {
        var val = $(this).val();
        if (val.length != 3 || !/^\d+$/.test(val)) {
            exibirMensagem(tipoMensagem.atencao, "O campo CST deve conter exatamente 3 dígitos numéricos.");
        }
    });

    $("#" + _outrasAliquotas.CodigoClassificacaoTributaria.id).mask("000000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _outrasAliquotas.CodigoClassificacaoTributaria.id).on("blur", function () {
        var val = $(this).val();
        if (val.length != 6 || !/^\d+$/.test(val)) {
            exibirMensagem(tipoMensagem.atencao, "O campo Código de Classificação Tributária deve conter exatamente 6 dígitos numéricos.");
        }
    });
}
function editarOutrasAliquotasImposto(codigoOutrasAliquotasImposto) {
    LimparDadosVincularImposto();

    var data = { CodigoOutrasAliquotasImposto: codigoOutrasAliquotasImposto.Codigo};

    executarReST("OutrasAliquotas/BuscarOutrasAliquotasImpostoPorCodigo", data, 
        function (arg) {
            if (arg.Success && arg.Data) {
                PreencherObjetoKnout(_vincularImposto, arg);
                _vincularImposto.TipoImposto.enable(false);

                _vincularImposto.TituloModal.text("Editar Imposto");

                _vincularImposto.Excluir.visible(true);
                
                Global.abrirModal("divModalVincularImposto");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível carregar os dados do imposto.");
            }
        }, 
        null
    );
}

function ExcluirOutrasAliquotasImposto() {
    exibirConfirmacao("Excluir Imposto", "Deseja realmente excluir este imposto?", function () {
        executarReST("OutrasAliquotas/ExcluirOutrasAliquotasImposto",
            { CodigoOutrasAliquotasImposto: _vincularImposto.CodigoOutrasAliquotasImposto.val() },
            function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        if (_gridOutrasAliquotas) {
                            Global.fecharModal('divModalVincularImposto');
                            LimparDadosVincularImposto();

                            _gridImpostosVinculados.CarregarGrid();
                            _vincularImposto.TituloModal.text("Vincular Imposto");
                        }
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, null);
    });
}