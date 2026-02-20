/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Ocorrencias/Ocorrencia/Ocorrencia.js" />
/// <reference path="Lotes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOcorrenciaLoteAvaria;
var _pesquisaOcorrenciaLoteAvaria;

var PesquisaOcorrenciaLoteAvaria = function () {
    this.LoteAvaria = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.LoteAvaria.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaLoteAvaria.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.AdicionarOcorrencia = PropertyEntity({ eventClick: adicionarNovaOcorrenciaClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarOcorrencia, visible: ko.observable(true) });
};

function loadOcorrenciaLoteAvaria() {
    _pesquisaOcorrenciaLoteAvaria = new PesquisaOcorrenciaLoteAvaria();
    KoBindings(_pesquisaOcorrenciaLoteAvaria, "knockoutOcorrencia", false, _pesquisaOcorrenciaLoteAvaria.Pesquisar.id);
    buscarOcorrenciasLoteAvaria();
    _pesquisaOcorrencia = new PesquisaOcorrencia();
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");

    $('#divModalOcorrencia').on('hidden.bs.modal', function () {
        _gridOcorrenciaLoteAvaria.CarregarGrid();
    });
}

function EtapaOcorrenciaLoteAvariaClick() {
    _pesquisaOcorrenciaLoteAvaria.LoteAvaria.val(_lote.Codigo.val());
    _pesquisaOcorrenciaLoteAvaria.LoteAvaria.codEntity(_lote.Codigo.val());

    _gridOcorrenciaLoteAvaria.CarregarGrid();
}

function buscarOcorrenciasLoteAvaria() {
    var editar = { descricao: Localization.Resources.Chamado.ChamadoOcorrencia.Detalhes, id: "clasEditar", evento: "onclick", metodo: editarOcorrenciaLoteAvaria, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    _gridOcorrenciaLoteAvaria = new GridView(_pesquisaOcorrenciaLoteAvaria.Pesquisar.idGrid, "Ocorrencia/Pesquisa", _pesquisaOcorrenciaLoteAvaria, menuOpcoes, null, null);
}

function adicionarNovaOcorrenciaClick() {
    limparCamposOcorrencia();
    executarReST("Lotes/ObterTipoOcorrencia", { MotivoAvaria: _lote.CodigoMotivoAvaria.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCamposOcorrencia();

                if (r.Data.Codigo > 0) {
                    _ocorrencia.TipoOcorrencia.codEntity(r.Data.Codigo);
                    _ocorrencia.TipoOcorrencia.val(r.Data.Descricao);
                    _ocorrencia.TipoOcorrencia.origemOcorrencia = r.Data.OrigemOcorrencia;

                    if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                        _ocorrencia.TipoOcorrencia.enable(false);
                    else
                        _ocorrencia.TipoOcorrencia.enable(true);
                }

                if (r.Data.ComponenteCodigo > 0) {
                    _ocorrencia.ComponenteFrete.codEntity(r.Data.ComponenteCodigo);
                    _ocorrencia.ComponenteFrete.val(r.Data.ComponenteDescricao);
                    if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                        _ocorrencia.ComponenteFrete.enable(false);
                    else
                        _ocorrencia.ComponenteFrete.enable(true);
                }

                _ocorrencia.LoteAvaria.val(_lote.NumeroLote.val());
                _ocorrencia.LoteAvaria.codEntity(_lote.Codigo.val());
                _ocorrencia.LoteAvaria.visible(false);
                _ocorrencia.LoteAvaria.enable(false);

                Global.abrirModal('divModalOcorrencia');
                visiblidadeValorOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function editarOcorrenciaLoteAvaria(data) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(data.Codigo);
    buscarOcorrenciaPorCodigo(function () {
        Global.abrirModal('divModalOcorrencia');
    });
}