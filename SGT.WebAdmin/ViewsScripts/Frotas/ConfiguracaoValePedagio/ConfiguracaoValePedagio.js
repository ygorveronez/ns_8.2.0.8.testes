/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="IntegracaoSemParar.js" />
/// <reference path="IntegracaoTarget.js" />
/// <reference path="IntegracaoPagbem.js" />
/// <reference path="IntegracaoRepom.js" />
/// <reference path="IntegracaoDBTrans.js" />
/// <reference path="integracaoQualP.js" />
/// <reference path="IntegracaoPamcard.js" />
/// <reference path="IntegracaoEFrete.js" />
/// <reference path="IntegracaoExtratta.js" />
/// <reference path="IntegracaoDigitalCom.js" />
/// <reference path="IntegracaoAmbipar.js" />
/// <reference path="IntegracaoNDDCargo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaConfiguracaoValePedagio;
var _configuracaoValePedagio;
var _gridConfiguracaoValePedagio;
var _CRUDConfiguracaoValePedagio;
var _bindConfiguracoesValePedagio;

var PesquisaConfiguracaoValePedagio = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ text: "Grupo de Pessoas: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "Situação: " });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(EnumTipoIntegracao.NaoInformada), def: EnumTipoIntegracao.NaoInformada, options: ko.observableArray([]), text: "Tipo Integração:" });

    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridConfiguracaoValePedagio.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

var ConfiguracaoValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ text: "Grupo de Pessoas: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.ConsultarValorPedagioAntesAutorizarEmissao = PropertyEntity({ text: "Consultar o valor do pedágio antes de autorizar a emissão dos documentos", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.PermitirGerarValePedagioVeiculoProprio = PropertyEntity({ text: "Permitir gerar vale pedágio para veículos próprios", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.ConfiguracaoIntegracao = PropertyEntity({
        val: function () {
            var _koConfiguracaoIntegracao = GetConfiguracaoIntegracao();

            if (arguments.length > 0) {
                if (_koConfiguracaoIntegracao) {
                    PreencherObjetoKnout(_koConfiguracaoIntegracao, { Data: arguments[0] || {} });
                }
            } else {
                return JSON.stringify(RetornarObjetoPesquisa(_koConfiguracaoIntegracao));
            }
        }
    });

    this.TipoIntegracao = PropertyEntity({ val: ko.observable(EnumTipoIntegracao.NaoInformada), def: EnumTipoIntegracao.NaoInformada, options: ko.observableArray([]), text: "Tipo Integração:" });
    this.TipoIntegracao.val.subscribe(function () {
        changeTipoIntegracao();
    });

    this.GrupoPessoas.codEntity.subscribe(function (novoValor) {
        if (novoValor > 0)
            _configuracaoValePedagio.PermitirGerarValePedagioVeiculoProprio.visible(true);
        else {
            _configuracaoValePedagio.PermitirGerarValePedagioVeiculoProprio.visible(false);
            _configuracaoValePedagio.PermitirGerarValePedagioVeiculoProprio.val(false);
        }
    });
};

var CRUDConfiguracaoValePedagio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoValePedagio() {
    _configuracaoValePedagio = new ConfiguracaoValePedagio();
    KoBindings(_configuracaoValePedagio, "knockoutConfiguracaoValePedagio");

    _CRUDConfiguracaoValePedagio = new CRUDConfiguracaoValePedagio();
    KoBindings(_CRUDConfiguracaoValePedagio, "knoutCRUDConfiguracaoValePedagio");

    _pesquisaConfiguracaoValePedagio = new PesquisaConfiguracaoValePedagio();
    KoBindings(_pesquisaConfiguracaoValePedagio, "knockoutPesquisaConfiguracaoValePedagio", _pesquisaConfiguracaoValePedagio.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoValePedagio", _configuracaoValePedagio);

    new BuscarFilial(_configuracaoValePedagio.Filial);
    new BuscarTiposOperacao(_configuracaoValePedagio.TipoOperacao);
    new BuscarGruposPessoas(_configuracaoValePedagio.GrupoPessoas);

    new BuscarFilial(_pesquisaConfiguracaoValePedagio.Filial);
    new BuscarTiposOperacao(_pesquisaConfiguracaoValePedagio.TipoOperacao);
    new BuscarGruposPessoas(_pesquisaConfiguracaoValePedagio.GrupoPessoas);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _configuracaoValePedagio.Filial.visible(false);
        _pesquisaConfiguracaoValePedagio.Filial.visible(false);

        _configuracaoValePedagio.GrupoPessoas.visible(true);
        _pesquisaConfiguracaoValePedagio.GrupoPessoas.visible(true);
    }

    CarregarTiposIntegracaoValePedagio();
    BindConfiguracoesValePedagio();

    BuscarConfiguracaoValePedagio();

    loadConfiguracaoSemParar();
    loadConfiguracaoTarget();
    loadConfiguracaoRepom();
    loadConfiguracaoPagbem();
    loadConfiguracaoDBTrans();
    loadConfiguracaoPamcard();
    loadConfiguracaoQualP();
    loadConfiguracaoEFrete();
    loadConfiguracaoExtratta();
    loadConfiguracaoDigitalCom();
    loadConfiguracaoAmbipar();
    loadConfiguracaoNDDCargo();
}

function changeTipoIntegracao() {
    $(".configuracao-vale-pedagio").hide();
    _configuracaoValePedagio.ConsultarValorPedagioAntesAutorizarEmissao.visible(false);

    if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.SemParar)
        $("#liSemParar").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.Target)
    {
        $("#liTarget").show();
        _configuracaoValePedagio.ConsultarValorPedagioAntesAutorizarEmissao.visible(true);
    }       
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.DigitalCom)
        $("#liDigitalCom").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.Repom) {
        $("#liRepom").show();
        _configuracaoValePedagio.ConsultarValorPedagioAntesAutorizarEmissao.visible(true);
    }
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.PagBem)
        $("#liPagbem").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.DBTrans)
        $("#liDBTrans").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.Pamcard)
        $("#liPamcard").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.QualP)
        $("#liQualP").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.EFrete)
        $("#liEFrete").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.Extratta)
        $("#liExtratta").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.Ambipar)
        $("#liAmbipar").show();
    else if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.NDDCargo)
        $("#liNDDCargo").show();
}

function ValidarConfiguracaoValePedagio() {
    var valido = true;

    if (EnumTipoIntegracao.NaoDefinido != _configuracaoValePedagio.TipoIntegracao.val()) {
        var _koConfiguracaoIntegracao = GetConfiguracaoIntegracao();

        if (_koConfiguracaoIntegracao != null && !ValidarCamposObrigatorios(_koConfiguracaoIntegracao)) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A configurações da integração vale pedágio não estão válidas.");
            valido = false;
        }
    }

    if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.SemParar) {
        _integracaoSemParar.QuantidadeDiasConsultarExtrato.requiredClass("form-control");
        if (_integracaoSemParar.ConsultarExtrato.val() && _integracaoSemParar.QuantidadeDiasConsultarExtrato.val() > 30) {
            _integracaoSemParar.QuantidadeDiasConsultarExtrato.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A quantidade de dias para consultar os extratos não pode ser maior que 30 dias.");
            valido = false;
        }
        else if (_integracaoSemParar.ConsultarExtrato.val() && _integracaoSemParar.QuantidadeDiasConsultarExtrato.val() < 2) {
            _integracaoSemParar.QuantidadeDiasConsultarExtrato.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A quantidade de dias para consultar os extratos não pode ser menor que 2 dias.");
            valido = false;
        }
    }

    if (_configuracaoValePedagio.TipoIntegracao.val() == EnumTipoIntegracao.EFrete)
    {
        if (_integracaoEFrete.EnviarPontosPassagemRotaFrete.val() && _integracaoEFrete.EnviarPolilinhaRoteirizacaoCarga.val()) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A opção Enviar pontos de passagem da rota do frete não pode ser marcada ao mesmo tempo que a opção Enviar polilinha da roteirização da carga.");
            valido = false;
        } 
    }

    return valido;
}

function adicionarClick(e, sender) {
    if (!ValidarConfiguracaoValePedagio())
        return;

    Salvar(_configuracaoValePedagio, "ConfiguracaoValePedagio/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConfiguracaoValePedagio.CarregarGrid();
                LimparCamposConfiguracaoValePedagio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarConfiguracaoValePedagio())
        return;

    Salvar(_configuracaoValePedagio, "ConfiguracaoValePedagio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridConfiguracaoValePedagio.CarregarGrid();
                LimparCamposConfiguracaoValePedagio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta configuração?", function () {
        ExcluirPorCodigo(_configuracaoValePedagio, "ConfiguracaoValePedagio/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridConfiguracaoValePedagio.CarregarGrid();
                    LimparCamposConfiguracaoValePedagio();
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
    LimparCamposConfiguracaoValePedagio();
}

//*******MÉTODOS*******

function editarConfiguracaoValePedagio(valePedagio) {
    LimparCamposConfiguracaoValePedagio();
    _configuracaoValePedagio.Codigo.val(valePedagio.Codigo);
    BuscarPorCodigo(_configuracaoValePedagio, "ConfiguracaoValePedagio/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoValePedagio.ExibirFiltros.visibleFade(false);
        _CRUDConfiguracaoValePedagio.Atualizar.visible(true);
        _CRUDConfiguracaoValePedagio.Cancelar.visible(true);
        _CRUDConfiguracaoValePedagio.Excluir.visible(true);
        _CRUDConfiguracaoValePedagio.Adicionar.visible(false);

        changeTipoRotaSemParar();
    }, null);
}

function BuscarConfiguracaoValePedagio() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoValePedagio, tamanho: "20", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridConfiguracaoValePedagio = new GridView(_pesquisaConfiguracaoValePedagio.Pesquisar.idGrid, "ConfiguracaoValePedagio/Pesquisar", _pesquisaConfiguracaoValePedagio, menuOpcoes, null);
    _gridConfiguracaoValePedagio.CarregarGrid();
}

function LimparCamposConfiguracaoValePedagio() {
    _CRUDConfiguracaoValePedagio.Atualizar.visible(false);
    _CRUDConfiguracaoValePedagio.Cancelar.visible(false);
    _CRUDConfiguracaoValePedagio.Excluir.visible(false);
    _CRUDConfiguracaoValePedagio.Adicionar.visible(true);
    LimparCampos(_configuracaoValePedagio);

    // Limpar os campos de configrações
    _bindConfiguracoesValePedagio.forEach(function (integ) {
        var _integracao = integ.Callback();
        if (_integracao != null)
            LimparCampos(_integracao);
    });

    Global.ResetarAbas();
}

function CarregarTiposIntegracaoValePedagio() {
    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.NaoInformada,
            EnumTipoIntegracao.PagBem,
            EnumTipoIntegracao.SemParar,
            EnumTipoIntegracao.Target,
            EnumTipoIntegracao.DigitalCom,
            EnumTipoIntegracao.Repom,
            EnumTipoIntegracao.DBTrans,
            EnumTipoIntegracao.QualP,
            EnumTipoIntegracao.Pamcard,
            EnumTipoIntegracao.EFrete,
            EnumTipoIntegracao.Extratta,
            EnumTipoIntegracao.Ambipar,
            EnumTipoIntegracao.NDDCargo
        ])
    }, function (retorno) {
        if (retorno.Success) {
            var tiposIntegracaoValePedagio = retorno.Data.map(function (integracao) {
                return { value: integracao.Codigo, text: integracao.Descricao };
            })

            var opcaoNula = {
                text: 'Todos',
                value: null
            };

            _configuracaoValePedagio.TipoIntegracao.options(tiposIntegracaoValePedagio);
            _pesquisaConfiguracaoValePedagio.TipoIntegracao.options([opcaoNula].concat(tiposIntegracaoValePedagio));
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function BindConfiguracoesValePedagio() {
    _bindConfiguracoesValePedagio = [
        {
            Enum: EnumTipoIntegracao.SemParar,
            Callback: function () { return _integracaoSemParar; }
        },
        {
            Enum: EnumTipoIntegracao.Target,
            Callback: function () { return _integracaoTarget; }
        },
        {
            Enum: EnumTipoIntegracao.Repom,
            Callback: function () { return _integracaoRepom; }
        },
        {
            Enum: EnumTipoIntegracao.PagBem,
            Callback: function () { return _integracaoPagbem; }
        },
        {
            Enum: EnumTipoIntegracao.DBTrans,
            Callback: function () { return _integracaoDBTrans; }
        },
        {
            Enum: EnumTipoIntegracao.Pamcard,
            Callback: function () { return _integracaoPamcard; }
        },
        {
            Enum: EnumTipoIntegracao.QualP,
            Callback: function () { return _integracaoQualP; }
        },
        {
            Enum: EnumTipoIntegracao.EFrete,
            Callback: function () { return _integracaoEFrete; }
        },
        {
            Enum: EnumTipoIntegracao.Extratta,
            Callback: function () { return _integracaoExtratta; }
        },
        {
            Enum: EnumTipoIntegracao.DigitalCom,
            Callback: function () { return _integracaoDigitalCom; }
        },
        {
            Enum: EnumTipoIntegracao.Ambipar,
            Callback: function () { return _integracaoAmbipar; }
        },
        {
            Enum: EnumTipoIntegracao.NDDCargo,
            Callback: function () { return _integracaoNDDCargo; }
        }
    ];
}

function GetConfiguracaoIntegracao() {
    var enumerador = _configuracaoValePedagio.TipoIntegracao.val();
    var _ko = null;

    _bindConfiguracoesValePedagio.forEach(function (item) {
        if (item.Enum == enumerador)
            _ko = item.Callback();
    });

    return _ko;
}
