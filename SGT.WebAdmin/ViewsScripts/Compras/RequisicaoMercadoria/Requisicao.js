/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/MotivoCompra.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="RequisicaoMercadoria.js" />
/// <reference path="Mercadorias.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _requisicao;
var _requisicaoObservacao;
var usuarioLogado = null;
var empresaLogado = null;

var Requisicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.map, enable: false });
    this.Colaborador = PropertyEntity({ text: "Colaborador: ", getType: typesKnockout.map, enable: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.FuncionarioRequisitado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Requisitado:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Observacao = PropertyEntity({ text: "Observação: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });

};

var RequisicaoObservacao = function () {
    this.Observacao = _requisicao.Observacao;
};

//*******EVENTOS*******

function LoadRequisicao() {
    _requisicao = new Requisicao();
    KoBindings(_requisicao, "knockoutRequisicao");

    _requisicaoObservacao = new RequisicaoObservacao();
    KoBindings(_requisicaoObservacao, "knockoutObservacaoRequisicao");

    _requisicaoMercadoria.Codigo.val.subscribe(function (val) {
        _requisicao.Codigo.val(val);
    });

    BuscarEmpresa(_requisicao.Filial);
    BuscarMotivoCompra(_requisicao.Motivo, retornoMotivo);
    BuscarFuncionario(_requisicao.FuncionarioRequisitado);
    BuscarVeiculos(_requisicao.Veiculo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _requisicao.Filial.enable(false);

    _requisicao.Filial.codEntity.subscribe(function (val) {
        // Só exibe a busca de produto quando filial for selecionada
        const visible = val > 0;
        _mercadorias.BuscarEstoqueMinimo.visible(visible);
        _mercadorias.Produto.enable(visible);
        _mercadorias.LocalArmazenamento.enable(visible);
    });

    _requisicao.Veiculo.required.subscribe((val) => {
        _requisicao.Veiculo.text(val ? "*Veículo:" : "Veículo:");
    });

    PreencheUsuarioLogado();
}


//*******METODOS*******
function retornoMotivo(data) {
    if (data != null) {
        _requisicao.Motivo.codEntity(data.Codigo);
        _requisicao.Motivo.val(data.Descricao);
        _requisicao.Veiculo.required(data.ExigeInformarVeiculoObrigatoriamente);
    }
}

function PreencheUsuarioLogado() {
    const _fillName = function () {
        _requisicao.Colaborador.val(usuarioLogado.Nome);
    }

    const _fillNameEmpresa = function () {
        _requisicao.Filial.val(empresaLogado.RazaoEmpresa);
        _requisicao.Filial.codEntity(empresaLogado.CodigoEmpresa);
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
            _requisicao.Filial.enable(false);
    }

    if (usuarioLogado != null) _fillName();
    if (empresaLogado != null) _fillNameEmpresa();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                usuarioLogado = {
                    Nome: arg.Data.Nome
                };
                _fillName();

                empresaLogado = {
                    RazaoEmpresa: arg.Data.RazaoEmpresa,
                    CodigoEmpresa: arg.Data.CodigoEmpresa,
                };
                _fillNameEmpresa();
            }
        }
    });
}

function ControleCamposRequisicao(status) {
    _requisicao.Filial.enable(status);
    _requisicao.Motivo.enable(status);
    _requisicao.Data.enable(status);
    _requisicao.Observacao.enable(status);
    _requisicao.FuncionarioRequisitado.enable(status);
    _requisicao.Veiculo.enable(status);

    if (_requisicaoMercadoria.Situacao.val() == EnumSituacaoRequisicaoMercadoria.AgAprovacao) {
        _mercadorias.Produto.visible(true);
        _mercadorias.BuscarEstoqueMinimo.visible(false);        
    } else {
        _mercadorias.Produto.visible(status);
    }

    _mercadorias.LocalArmazenamento.visible(status && _CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _requisicao.Filial.enable(false);
}