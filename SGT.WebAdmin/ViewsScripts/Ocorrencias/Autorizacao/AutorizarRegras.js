/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/MotivoRejeicaoOcorrencia.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrenciaAutorizacao.js" />
/// <reference path="Autorizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegras;
var _regraOcorrencia;
var _detalhesRegraOcorrencia;
var _rowGridOcorrenciaAtual;
var _codigoOcorrenciaAtual;

var RegraOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.InformarMotivoNaAprovacao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.OcorrenciaProvisionada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var DetalhesRegraOcorrencia = function () {
    this.MotivoRejeicao = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoRejeicao.getFieldDescription()) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Observacao.getFieldDescription() });
};

//*******EVENTOS*******

function loadRegras() {
    _regraOcorrencia = new RegraOcorrencia();

    _detalhesRegraOcorrencia = new DetalhesRegraOcorrencia();
    KoBindings(_detalhesRegraOcorrencia, "knockoutDetalhesRegraOcorrencia");

    new BuscarMotivoRejeicaoOcorrencia(_autorizacao.Justificativa, null, EnumAprovacaoRejeicao.Rejeicao);
    new BuscarMotivoRejeicaoOcorrencia(_autorizacao.JustificativaAprovacao, null, EnumAprovacaoRejeicao.Aprovacao);
    new BuscarCentroResultado(_autorizacao.CentroResultado, null, null, null, null, null, null, null, true);

    var aprovar = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Aprovar,
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarOcorrencia
    };
    var rejeitar = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Rejeitar,
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarOcorrencia
    };

    var detalhesAprovacao = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Detalhes,
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.Situacao == EnumSituacaoOcorrenciaAutorizacao.Rejeitada || dataRow.Situacao == EnumSituacaoOcorrenciaAutorizacao.Aprovada && dataRow.InformarMotivoNaAprovacao || dataRow.Situacao == EnumSituacaoOcorrenciaAutorizacao.Aprovada && dataRow.Observacao!= null;
        },
        metodo: DetalhesRegra
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Opcoes,
        tamanho: 7,
        opcoes: [aprovar, rejeitar, detalhesAprovacao]
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoOcorrencia/RegrasAprovacao", _ocorrencia, menuOpcoes);
}

function aprovarMultiplasRegrasClick() {
    var dados = {
        Codigo: _ocorrencia.Codigo.val(),
        Tomador: _autorizacao.Tomador.val(),
        Pagamento: _autorizacao.Pagamento.val(),
        ClienteTomador: _autorizacao.ClienteTomador.codEntity(),
        PercentualJurosParcela: _autorizacao.PercentualJurosParcela.val(),
        QuantidadeParcelas: _autorizacao.QuantidadeParcelas.val(),
        PeriodoPagamento: _autorizacao.PeriodoPagamento.val()
    };

    executarReST("AutorizacaoOcorrencia/AprovarMultiplasRegras", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;

                if (data.RegrasModificadas > 1)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadasOcorrenciasForamAprovadas.format(data.RegrasModificadas));
                else if (data.RegrasModificadas == 1)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadaOcorrenciaFoiaPROVADA.format(data.RegrasModificadas));
                else if (data.RegrasExigemMotivo == 0)
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NenhumaAlcadaPendenteParaSeuUsuario);

                if (data.RegrasExigemMotivo > 0)
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.HaAlcadasOcorrenciasQueExigemJustificativaParaAprovacao.format(data.RegrasExigemMotivo));

                atualizarOcorrencia();
                buscarOcorrencias();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })
}

function cancelarRejeicaoClick() {
    limparRejeicao();
}

function rejeitarOcorrenciaClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CaracteresMinimos, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParaRejeitarOcorrenciaENecessarioInformarAoMinimoVinteCarcteres);

    if (_autorizacao.Justificativa.codEntity() == 0)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Justificativa, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CampoJustificativaEObrigatorio);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RealmenteDesejaRejeitarOcorrencia, function () {
        var dados = {
            Codigo: _regraOcorrencia.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
            Justificativa: _autorizacao.Justificativa.codEntity()
        };

        executarReST("AutorizacaoOcorrencia/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.EnviadoComSucesso);
                    atualizarOcorrencia();
                    AtualizarGridOcorrencias();
                    limparRejeicao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function aprovarOcorrenciaClick() {
    if (_regraOcorrencia.InformarMotivoNaAprovacao.val()) {
        if (_autorizacao.Motivo.val().length < 20)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CaracteresMinimos, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParaAprovarOcorrenciaNecessarioInformarNoMinimoVinteCaracteres);

        if (_autorizacao.JustificativaAprovacao.codEntity() == 0)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Justificativa, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CampoJustificativaEObrigatorio);

        if (_CONFIGURACAO_TMS.PermiteInformarCentroResultadoAprovacaoOcorrencia && _regraOcorrencia.OcorrenciaProvisionada.val() && _autorizacao.CentroResultado.codEntity() == 0)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CentroResultado, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CampoCentroResultadoEObrigatorioParaOcorrenciaProvisionada);
    }

    var dados = {
        Codigo: _regraOcorrencia.Codigo.val(),
        Tomador: _autorizacao.Tomador.val(),
        Pagamento: _autorizacao.Pagamento.val(),
        ClienteTomador: _autorizacao.ClienteTomador.codEntity(),
        PercentualJurosParcela: _autorizacao.PercentualJurosParcela.val(),
        QuantidadeParcelas: _autorizacao.QuantidadeParcelas.val(),
        Motivo: _autorizacao.Motivo.val(),
        JustificativaAprovacao: _autorizacao.JustificativaAprovacao.codEntity(),
        CentroResultado: _autorizacao.CentroResultado.codEntity(),
        PeriodoPagamento: _autorizacao.PeriodoPagamento.val()
    };

    executarReST("AutorizacaoOcorrencia/Aprovar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.EnviadoComSucesso);
                atualizarOcorrencia();
                AtualizarGridOcorrencias();
                limparRejeicao();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function AtualizarGridRegras() {
    _gridRegras.CarregarGrid(function (arg) {
        var exibeBtn = false;
        arg.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibeBtn = true;
        });

        _autorizacao.AprovarTodas.visible(exibeBtn);
        _autorizacao.PercentualJurosParcela.enable(exibeBtn);
        _autorizacao.QuantidadeParcelas.enable(exibeBtn);
    });
}

function RejeitarOcorrencia(dataRow) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaReprovar, function () {
        limparRejeicao();

        _regraOcorrencia.Codigo.val(dataRow.Codigo);
        _autorizacao.Tomador.visible(false);
        _autorizacao.Motivo.visible(true);
        _autorizacao.Justificativa.visible(true);
        _autorizacao.Rejeitar.visible(true);
    });
}

function DetalhesRegra(dataRow) {
    _detalhesRegraOcorrencia.MotivoRejeicao.val(dataRow.MotivoRejeicao);
    _detalhesRegraOcorrencia.Observacao.val(dataRow.Observacao);

    if (dataRow.Situacao == EnumSituacaoOcorrenciaAutorizacao.Aprovada)
        _detalhesRegraOcorrencia.MotivoRejeicao.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoAprovacao.getFieldDescription());
    else
        _detalhesRegraOcorrencia.MotivoRejeicao.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoRejeicao.getFieldDescription());

    Global.abrirModal('divModalDetalhesRegraOcorrencia');
}

function AtualizarGridOcorrencias() {
    executarReST("AutorizacaoOcorrencia/BuscarOcorrenciaPorCodigo", { Codigo: _codigoOcorrenciaAtual }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _gridOcorrencia.AtualizarDataRow(_rowGridOcorrenciaAtual, arg.Data)

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AprovarOcorrencia(dataRow) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaAprovar, function () {
        limparRejeicao();

        _regraOcorrencia.Codigo.val(dataRow.Codigo);
        _regraOcorrencia.InformarMotivoNaAprovacao.val(dataRow.InformarMotivoNaAprovacao);
        _regraOcorrencia.OcorrenciaProvisionada.val(dataRow.OcorrenciaProvisionada);

        if (dataRow.InformarMotivoNaAprovacao) {
            _autorizacao.Motivo.visible(true);
            _autorizacao.JustificativaAprovacao.visible(true);
            _autorizacao.Aprovar.visible(true);

            if (_CONFIGURACAO_TMS.PermiteInformarCentroResultadoAprovacaoOcorrencia && _regraOcorrencia.OcorrenciaProvisionada.val())
                _autorizacao.CentroResultado.visible(true);
        } else {
            _autorizacao.Motivo.visible(true);
            _autorizacao.JustificativaAprovacao.visible(false);
            _autorizacao.Aprovar.visible(true);
        }
           
    });
}

function limparRegras() {
    ocultarComponentesRejeicao();
    LimparCampos(_autorizacao);
}

function limparRejeicao() {
    ocultarComponentesRejeicao();
    LimparCampo(_autorizacao.Tomador);
    LimparCampo(_autorizacao.Motivo);
    LimparCampo(_autorizacao.Justificativa);
    LimparCampo(_autorizacao.JustificativaAprovacao);
    LimparCampo(_autorizacao.CentroResultado);
}

function ocultarComponentesRejeicao() {
    _autorizacao.Tomador.visible(true);
    _autorizacao.Motivo.visible(false);
    _autorizacao.Justificativa.visible(false);
    _autorizacao.JustificativaAprovacao.visible(false);
    _autorizacao.CentroResultado.visible(false);
    _autorizacao.Rejeitar.visible(false);
    _autorizacao.Aprovar.visible(false);
}
