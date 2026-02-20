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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="ResumoOcorrencia.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Enumeradores/EnumTipoEnvioXMLOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _CodigoCargaCTeClicado;
var _knoutArquivo;
var _TipoEnvioXMLOcorrencia;

//*******EVENTOS*******

var Arquivo = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false, visible: ko.observable(false) });
}


function loadCTeImportacao() {
    _knoutArquivo = new Arquivo();
    KoBindings(_knoutArquivo, "knoutEnviarArquivo");
    $("#" + _knoutArquivo.Arquivo.id).on("change", enviarXMLCTeClick);

}


function detalhesCTeVinculadoClick(e, sender) {
    var codigo = 0;
    $.each(_CTesImportadosParaComplemento, function (i, obj) {
        if (obj.CodigoCargaCTeParaComplementar == e.Codigo) {
            codigo = obj.CodigoCTeComplemetarImportado;
            return false;
        }
    });
    if (codigo > 0)
        exibirDetalhesCTe(codigo);
}

function vincularCTeClick(e) {
    _CodigoCargaCTeClicado = e.Codigo;
    $("#" + _ocorrencia.CTeSemCarga.idBtnSearch).trigger("click");
}

function importarXMLCTeComplementoClick(e) {
    _CodigoCargaCTeClicado = e.Codigo;
    _TipoEnvioXMLOcorrencia = EnumTipoEnvioXMLOcorrencia.cteComplementar;
    $("#" + _knoutArquivo.Arquivo.id).val("");
    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function enviarXMLCTeClick() {
    var file = document.getElementById(_knoutArquivo.Arquivo.id);
    if (file.files.length > 0) {
        exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaEnviarArquivo.format(file.files[0].name), function () {
            var formData = new FormData();
            formData.append("upload", file.files[0]);
            if (_TipoEnvioXMLOcorrencia == EnumTipoEnvioXMLOcorrencia.cteComplementar)
                enviarComplementar(formData);
            else
                enviarXMLCancelamento(formData)
        });
    }
}

function enviarComplementar(formData) {
    enviarArquivo("CTe/EnviarXMLCTe?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                removerCTeGrid(_CodigoCargaCTeClicado);
                var cte = arg.Data;
                var data = {
                    CodigoCargaCTeParaComplementar: _CodigoCargaCTeClicado,
                    CodigoCTeComplemetarImportado: cte.Codigo,
                    NumeroCTeComplemetarImportado: cte.Numero,
                    ValorCTeComplemetarImportado: cte.ValorFrete.ValorTotalAReceber
                }
                var valorOcorrencia = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
                _ocorrencia.ValorOcorrencia.val(Globalize.format(data.ValorCTeComplemetarImportado + valorOcorrencia, "n2"));
                _CTesImportadosParaComplemento.push(data);
                recarregarGridImportarCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirCTeVinculadoClick(e) {
    removerCTeGrid(e.Codigo);
    recarregarGridImportarCTe();
}

//*******MÉTODOS*******

function recarregarGridImportarCTe() {
    _gridCTe.AtualizarRegistrosSelecionados(BuscarSelecionados());
    _gridCTe.DrawTable();
}

function transformarGridImportarCTe() {
    if (_gridCTe != null) {
        _ocorrencia.SelecionarTodos.visible(false);
        _ocorrencia.SelecionarTodos.val(false);
        _ocorrencia.CTesParaComplemento.text(Localization.Resources.Ocorrencias.Ocorrencia.VincularCTesComplementaresImportadosOcorrencia);
        _gridCTe.SetarRegistrosSomenteLeitura(true);
        recarregarGridImportarCTe();
    }
}

function transformarGridEmissaoCTe() {
    if (_gridCTe == null)
        return;

    _ocorrencia.SelecionarTodos.visible(true);
    var apenasUmRegistroParaComplementar = _gridCTe.NumeroRegistros() == 1;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros && apenasUmRegistroParaComplementar)) {
        _ocorrencia.SelecionarTodos.val(!_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val());
    } else {
        _ocorrencia.SelecionarTodos.val(false);
    }
    
    _gridCTe.SetarRegistrosSomenteLeitura(false);
    _ocorrencia.CTesParaComplemento.text(Localization.Resources.Ocorrencias.Ocorrencia.SelecionarCTesParaComplemento);
    _gridCTe.DrawTable();
}

function removerCTeGrid(codigoCargaCte) {
    var novaLista = new Array();
    $.each(_CTesImportadosParaComplemento, function (i, obj) {
        if (obj.CodigoCargaCTeParaComplementar == codigoCargaCte) {
            var valorOcorrencia = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
            _ocorrencia.ValorOcorrencia.val(Globalize.format(valorOcorrencia - obj.ValorCTeComplemetarImportado, "n2"));
        } else {
            novaLista.push(obj);
        }
    });
    _CTesImportadosParaComplemento = novaLista;
}

function retornoBuscarCTeSemCarga(cte) {
    removerCTeGrid(_CodigoCargaCTeClicado);
    var valido = true;
    $.each(_CTesImportadosParaComplemento, function (i, obj) {
        if (obj.CodigoCTeComplemetarImportado == cte.Codigo) {
            valido = false;
            return false;
        }
    });

    if (valido) {
        var data = {
            CodigoCargaCTeParaComplementar: _CodigoCargaCTeClicado,
            CodigoCTeComplemetarImportado: cte.Codigo,
            NumeroCTeComplemetarImportado: cte.Numero,
            ValorCTeComplemetarImportado: Globalize.parseFloat(cte.ValorFrete)
        }
        var valorOcorrencia = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
        _ocorrencia.ValorOcorrencia.val(Globalize.format(data.ValorCTeComplemetarImportado + valorOcorrencia, "n2"));

        _CTesImportadosParaComplemento.push(data);
        recarregarGridImportarCTe();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.CTeJaFoiAdicionadoOcorrencia);
    }

}

function visibilidadeOpcaoImportacao(e) {
    if (_ocorrencia.CTeEmitidoNoEmbarcador.val() && _ocorrencia.ComponenteFrete.codEntity() > 0 && (_ocorrencia.CobrarOutroDocumento.val() === false))
        return true;
    else
        return false;
}

function visibilidadeCTeVinculado(e) {
    var visible = false;
    if (_ocorrencia.ComponenteFrete.codEntity() > 0) {
        $.each(_CTesImportadosParaComplemento, function (i, obj) {
            if (obj.CodigoCargaCTeParaComplementar == e.Codigo) {
                visible = true;
                return;
            }
        });
    }
    return visible;
}

