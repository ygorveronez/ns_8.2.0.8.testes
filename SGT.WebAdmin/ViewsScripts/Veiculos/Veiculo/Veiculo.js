/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/ModeloCarroceria.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/TipoPlotagem.js" />
/// <reference path="../../Consultas/TipoComunicacaoRastreador.js"/>
/// <reference path="../../Consultas/TecnologiaRastreador.js"/>
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../Consultas/CorVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../Enumeradores/EnumTipoFrota.js" />
/// <reference path="../../Enumeradores/EnumPaises.js" />
/// <reference path="../../Enumeradores/EnumTipoCombustivel.js" />
/// <reference path="../../Enumeradores/EnumTipoRodadoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoSistemaElevacao.js" />
/// <reference path="../../Enumeradores/EnumTipoCarreta.js" />
/// <reference path="../../Enumeradores/EnumTipoMaterial.js" />
/// <reference path="../../Enumeradores/EnumTipoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumFormaPagamentoCIOT.js" />
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js"/>
/// <reference path="../../Enumeradores/EnumTipoPropriedadeVeiculo.js"/>
/// <reference path="../../Enumeradores/EnumPosicaoReboque.js"/>
/// <reference path="../../Enumeradores/EnumModoCompraValePedagioTarget.js"/>
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="Integracoes.js" />
/// <reference path="VeiculosVinculados.js" />
/// <reference path="../VeiculoLicenca/VeiculoLicenca.js" />
/// <reference path="Anexo.js" />
/// <reference path="Currais.js" />
/// <reference path="Transportador.js" />
/// <reference path="Equipamento.js" />
/// <reference path="Motorista.js" />
/// <reference path="LiberacoesGR.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracoesDisponiveis = [];
var _gridVeiculo;
var _veiculo;
var _pesquisaVeiculo;
var _informarGrupoServico;
var _configuracaoPadrao;
var _configuracaoEmissaoCTeOpcoesTipoIntegracaoVeiculo;
var _PermissoesPersonalizadas;
var _existeEmOutroVeiculo;

var _estados = [
    { text: "Acre", value: "AC" },
    { text: "Alagoas", value: "AL" },
    { text: "Amapá", value: "AP" },
    { text: "Amazonas", value: "AM" },
    { text: "Bahia", value: "BA" },
    { text: "Ceará", value: "CE" },
    { text: "Distrito Federal", value: "DF" },
    { text: "Espírito Santo", value: "ES" },
    { text: "Goiás", value: "GO" },
    { text: "Maranhão", value: "MA" },
    { text: "Mato Grosso", value: "MT" },
    { text: "Mato Grosso do Sul", value: "MS" },
    { text: "Minas Gerais", value: "MG" },
    { text: "Pará", value: "PA" },
    { text: "Paraíba", value: "PB" },
    { text: "Paraná", value: "PR" },
    { text: "Pernambuco", value: "PE" },
    { text: "Piauí", value: "PI" },
    { text: "Rio de Janeiro", value: "RJ" },
    { text: "Rio Grande do Norte", value: "RN" },
    { text: "Rio Grande do Sul", value: "RS" },
    { text: "Rondônia", value: "RO" },
    { text: "Roraima", value: "RR" },
    { text: "Santa Catarina", value: "SC" },
    { text: "São Paulo", value: "SP" },
    { text: "Sergipe", value: "SE" },
    { text: "Tocantins", value: "TO" }
];

var _tipoCarrocerie;

var _quantidadeCurrais = [
    { text: "0", value: 0 },
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 }
];

var PesquisaVeiculo = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Placa.getFieldDescription(), val: ko.observable(""), maxlength: 7 });
    this.Renavam = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Renavam.getFieldDescription(), maxlength: 11 });
    this.Chassi = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroChassi.getFieldDescription(), maxlength: 20 });
    this.FormularioCadastro = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.ModeloVeicularCarga.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-6"), visible: ko.observable(true) });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(EnumTipoVeiculo.Todos), options: EnumTipoVeiculo.obterOpcoesPesquisa(), def: EnumTipoVeiculo.Todos, text: Localization.Resources.Veiculos.Veiculo.TipoVeiculo.getFieldDescription() });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Modelo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Marca.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-4") });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroFrota = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroFrota.getFieldDescription(), maxlength: 30 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeVeiculo.Todos), def: EnumTipoPropriedadeVeiculo.Todos, options: EnumTipoPropriedadeVeiculo.obterOpcoesPesquisa(), text: Localization.Resources.Veiculos.Veiculo.TipoPropriedade.getFieldDescription() });
    this.SomenteDisponveis = PropertyEntity({ val: ko.observable(false), def: false });
    this.SomenteEmpresasAtivas = PropertyEntity({ val: ko.observable(false), def: false });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Reboque.getFieldDescription(), idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Segmento.getFieldDescription(), idBtnSearch: guid(), issue: 0 });
    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.PropriedadeTerceiro.getFieldDescription(), idBtnSearch: guid() });
    // Vide filtro no controller
    this.ForcarFiltroModelo = PropertyEntity({ val: ko.observable(true), def: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculo.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BaixarTodosQRCode = PropertyEntity({ eventClick: baixarTodosQrCodeClick, type: types.event, text: Localization.Resources.Veiculos.Veiculo.BaixarTodosQRCode });
};

var Veiculo = function () {
    let self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Veiculos.Veiculo.Transportador.getFieldDescription() : Localization.Resources.Veiculos.Veiculo.PropriedadeTerceiro.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable("A"), getType: typesKnockout.bool, def: "A" });
    this.SituacaoCadastro = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });

    this.AlterarPlaca = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, enable: ko.observable(false), visible: ko.observable(true), required: false });
    this.Placa = PropertyEntity({ getType: typesKnockout.placa, text: Localization.Resources.Veiculos.Veiculo.Placa.getRequiredFieldDescription(), enable: ko.observable(true), required: true });
    this.Tara = PropertyEntity({ getType: typesKnockout.int, maxlength: 6, text: Localization.Resources.Veiculos.Veiculo.Tara.getRequiredFieldDescription(), issue: 804, required: true, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.AlterarPlaca.val.subscribe(function (valor) {
        _veiculo.Placa.enable(valor);
    });

    this.ValidarCamposReferenteCIOT = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarCamposReferenteCIOT.val.subscribe(function (val) {
        ValidarCamposReferenteCIOTChange(self);
    });

    this.CapacidadeM3 = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Veiculos.Veiculo.MetrosCubicosAbr.getFieldDescription(), issue: 805, required: false, maxlength: 3, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.CapacidadeQuilo = PropertyEntity({ getType: typesKnockout.int, maxlength: 6, text: Localization.Resources.Veiculos.Veiculo.CapacidadeQuilos.getRequiredFieldDescription(), issue: 806, required: true, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.CapacidadeTanque = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Veiculos.Veiculo.CapacidadeTanque.getFieldDescription(), required: false, maxlength: 6, enable: ko.observable(true), visible: ko.observable(true) });
    this.CapacidadeMaximaTanque = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 11, text: Localization.Resources.Veiculos.Veiculo.CapacidadeTanqueMax.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true) });
    this.CapacidadeTanqueArla = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Veiculos.Veiculo.CapacidadeTanqueArla.getFieldDescription(), required: false, maxlength: 6, enable: ko.observable(true), visible: ko.observable(true) });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeVeiculo.Proprio), def: EnumTipoPropriedadeVeiculo.Proprio, idFade: guid(), visible: ko.observable(true), visibleFade: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS), eventChange: mudouPropriedadeOnChange, enable: ko.observable(true), options: EnumTipoPropriedadeVeiculo.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoPropriedade.getRequiredFieldDescription(), required: true, issue: 151, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.TipoFrota = PropertyEntity({ val: ko.observable(EnumTipoFrota.NaoDefinido), options: EnumTipoFrota.obterOpcoes(), def: EnumTipoFrota.NaoDefinido, text: Localization.Resources.Veiculos.Veiculo.TipoFrota.getRequiredFieldDescription(), enable: ko.observable(true) });
    this.TipoRodado = PropertyEntity({ val: ko.observable(EnumTipoRodadoVeiculo.NaoAplicado), options: EnumTipoRodadoVeiculo.obterOpcoes(), def: EnumTipoRodadoVeiculo.NaoAplicado, text: Localization.Resources.Veiculos.Veiculo.TipoRodado.getRequiredFieldDescription(), required: true, issue: 153, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(EnumTipoVeiculo.Tracao), eventChange: verificarTipoVeiculoChange, options: EnumTipoVeiculo.obterOpcoes(), def: EnumTipoVeiculo.Tracao, text: Localization.Resources.Veiculos.Veiculo.TipoVeiculo.getRequiredFieldDescription(), required: true, issue: 152, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.TipoCarroceria = PropertyEntity({ val: ko.observable("00"), options: _tipoCarrocerie, def: "00", text: Localization.Resources.Veiculos.Veiculo.TipoCarroceria.getRequiredFieldDescription(), required: true, issue: 154, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.Veiculo.ModeloVeicularCarga.getRequiredFieldDescription()), issue: 44, required: ko.observable(true), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-7 col-lg-7") }); //issue: 44,

    this.ModeloVeicularCarga.codEntity.subscribe(function (novoValor) {
        if (novoValor <= 0)
            _veiculo.PossuiLocalizador.visible(false);
    });

    this.Ativo = PropertyEntity({ val: ko.observable([true]), visible: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2"), enable: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.MarcaVeiculo.getRequiredFieldDescription()), required: true, idBtnSearch: guid(), issue: 155, enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.ModeloVeiculo.getRequiredFieldDescription()), required: true, idBtnSearch: guid(), issue: 156, enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloCarroceria = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.ModeloCarroceria.getFieldDescription()), issue: 658, required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.GrupoPessoas.getFieldDescription()), required: false, idBtnSearch: guid(), issue: 58, enable: ko.observable(true), visible: ko.observable(true) });
    this.Estado = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? "AN" : "SC"), options: _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? [{ text: "AN", value: "AN" }] : _estados, def: _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? "AN" : "SC", text: "*UF: ", required: _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? false : true, enable: ko.observable(true), visible: _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? false : true });
    this.LocalidadeEmplacamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Veiculos.Veiculo.LocalidadeEmplacamento.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior ? false : true });
    this.DataVigencia = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataInicioProprietario.getFieldDescription()), getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(false), required: ko.observable(false) });

    this.LocalidadeEmplacamento.codEntity.subscribe(function (novoValor) {
        if (novoValor <= 0)
            _veiculo.Estado.enable(true);
        else
            _veiculo.Estado.enable(false);
    });

    this.PossuiTagValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.PossuiTagValePedagio, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.PossuiTagValePedagio.val.subscribe(function (novoValor) {
        if (novoValor && _integracoesDisponiveis.some(function (o) { return o === EnumTipoIntegracao.Target || o === EnumTipoIntegracao.EFrete || o === EnumTipoIntegracao.Ambipar || o === EnumTipoIntegracao.NDDCargo || o === EnumTipoIntegracao.Pamcard }))
            $("#liTarget").removeClass("d-none");
        else if (novoValor && _integracoesDisponiveis.some(function (o) { return o === EnumTipoIntegracao.Pamcard })) {
            $("#liTarget").removeClass("d-none");
        }
        else
            $("#liTarget").addClass("d-none");
    });

    this.NaoComprarValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NaoCompraValePedagio, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.NaoComprarValePedagioRetorno = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NaoCompraValePedagioRetorno, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.VeiculoCooperado = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.VeiculoCooperado, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.EmpresaVeiculoCooperado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Veiculos.Veiculo.Empresa.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.VeiculoCooperado.val.subscribe(function (val) {
        _veiculo.EmpresaVeiculoCooperado.visible(val);
        _veiculo.EmpresaVeiculoCooperado.required(val);
    });

    this.Renavam = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.Renavam.getRequiredFieldDescription()), issue: 803, maxlength: 11, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.ObservacaoCTe.getFieldDescription(), maxlength: 5000, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.NaoValidarIntegracaoParaFilaCarregamento = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NaoValidarIntegracoesFilaCarregamento, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.CPFMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.CPFMotorista.getFieldDescription()), issue: 3, enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false), idBtnSearch: guid(), getType: typesKnockout.cpf, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.CPFMotorista.val.subscribe(function (val) {
        CodigoMotoristaVeiculoModificado(val, self);
    });
    this.NomeMotorista = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NomeMotorista.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.CPF = PropertyEntity({ visible: false });
    this.CodigoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.ViradaHodometro = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.OcorreuViradaHodometro, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.KilometragemVirada = PropertyEntity({ getType: typesKnockout.int, maxlength: 11, text: Localization.Resources.Veiculos.Veiculo.KmVirada.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.ViradaHodometro.val.subscribe(function (novoValor) {
        _veiculo.KilometragemVirada.required(novoValor);
        _veiculo.KilometragemVirada.visible(novoValor);
        if (!novoValor)
            _veiculo.KilometragemVirada.val("");
    });

    this.NaoComprarValePedagio.val.subscribe(function (novoValor) {
        _veiculo.NaoComprarValePedagioRetorno.visible(!novoValor);
        _veiculo.TipoIntegracaoValePedagio.visible(!novoValor);
    });

    this.DataValidadeAdicionalCarroceria = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataDeValidadeDoAdicionalDeCarroceria.getFieldDescription()), getType: typesKnockout.date, val: ko.observable(""), def: "", text: ko.observable(""), visible: ko.observable(false), required: ko.observable(false) });

    this.ModeloCarroceria.codEntity.subscribe(function (novoValor) {
        if (novoValor > 0) {
            _veiculo.DataValidadeAdicionalCarroceria.visible(true);
        } else {
            _veiculo.DataValidadeAdicionalCarroceria.visible(false);
        }
    });

    this.TipoIntegracaoValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.OperadoraValePedagio.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: _configuracaoEmissaoCTeOpcoesTipoIntegracaoVeiculo, def: [], visible: ko.observable(true) });
    this.TagSemParar = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.TagSemParar.getFieldDescription()), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoIntegracaoValePedagio.val.subscribe(function (novoValor) {
        _veiculo.ModoCompraValePedagioTarget.options(EnumModoCompraValePedagioTarget.ObterOpcoesTag(novoValor));
    });

    this.CIOTEmitidoContratanteDiferenteEmbarcador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.CIOTEmitidoContratanteDiferenteEmbarcador, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicialCIOTTemporario = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataInicialCIOTTemporario.getFieldDescription()), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataFinalCIOTTemporario = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataFinalCIOTTemporario.getFieldDescription()), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.FilialCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.FilialCarregamento.getRequiredFieldDescription(), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true) });

    this.CIOTEmitidoContratanteDiferenteEmbarcador.val.subscribe(function (novoValor) {
        if (novoValor) {
            _veiculo.DataInicialCIOTTemporario.text(Localization.Resources.Veiculos.Veiculo.DataInicialCIOTTemporario.getRequiredFieldDescription());
            _veiculo.DataFinalCIOTTemporario.text(Localization.Resources.Veiculos.Veiculo.DataFinalCIOTTemporario.getRequiredFieldDescription());
        }
        else {
            _veiculo.DataInicialCIOTTemporario.text(Localization.Resources.Veiculos.Veiculo.DataInicialCIOTTemporario.getFieldDescription());
            _veiculo.DataFinalCIOTTemporario.text(Localization.Resources.Veiculos.Veiculo.DataFinalCIOTTemporario.getFieldDescription());

        }
        _veiculo.DataInicialCIOTTemporario.required(novoValor);
        _veiculo.DataFinalCIOTTemporario.required(novoValor);
    });

    //Prop Paletizado para Geração de frota automatizada Marfrig:
    this.PaletizadoGeracaoFrota = PropertyEntity({ text: "Veículo Paletizado (Geração de Frota)", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });

    ///Vale Pedagio
    this.ModoCompraValePedagioTarget = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.ModoCompraValePedagio.getFieldDescription(), options: ko.observable(EnumModoCompraValePedagioTarget.ObterOpcoesTag()), val: ko.observable(null), def: null, visible: ko.observable(true) });
    this.NumeroCartaoValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroCartaoValePedagio.getFieldDescription(), maxlength: 50, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroCartaoAbastecimento = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroCartaoAbastecimento.getFieldDescription(), maxlength: 50, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    //Grupo Dados Terceiros
    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Veiculos.Veiculo.Proprietario.getRequiredFieldDescription(), issue: 56, required: false, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });
    this.Locador = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Veiculos.Veiculo.Locador.getFieldDescription(), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.TipoProprietario = PropertyEntity({ val: ko.observable(), options: ko.observable(EnumTipoProprietarioVeiculo.obterOpcoes()), text: Localization.Resources.Veiculos.Veiculo.TipoTerceiro.getRequiredFieldDescription(), required: ko.observable(true), enable: ko.observable(this.Proprietario.val()), visible: ko.observable(false) });

    this.RNTRC = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.Rntrc.getFieldDescription()), issue: 660, maxlength: 8, required: false, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CIOT = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Ciot.getFieldDescription(), maxlength: 12, required: false, cssClass: ko.observable("col col-3"), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorValePedagio = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Veiculos.Veiculo.ValorValePedagio.getFieldDescription(), required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorFrete = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Frete: ", required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros) });
    this.ValorAdiantamento = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Adiantamento: ", required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros) });
    this.FormaPagamento = PropertyEntity({ text: "Forma de Pagamento", options: EnumFormaPagamentoCIOT.obterOpcoes(), val: ko.observable(EnumFormaPagamentoCIOT.NaoSelecionado), def: EnumFormaPagamentoCIOT.NaoSelecionado, visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: "Data Vencimento: ", required: ko.observable(false), visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros), enable: ko.observable(true) });

    this.TipoPagamento = PropertyEntity({ text: "Tipo", options: EnumTipoPagamentoMDFe.ObterOpcoes(), val: ko.observable(EnumTipoPagamentoMDFe.NaoSelecionado), def: EnumTipoPagamentoMDFe.NaoSelecionado, visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros) });
    this.CNPJInstituicaoPagamento = PropertyEntity({ text: ko.observable("CNPJ Instituição de Pagamento: "), maxlength: 20, required: ko.observable(false), getType: typesKnockout.cnpj, visible: ko.observable(false) });
    this.ContaCIOT = PropertyEntity({ text: ko.observable("Banco: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });
    this.AgenciaCIOT = PropertyEntity({ text: ko.observable("Agencia: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });
    this.TipoChavePIX = PropertyEntity({ text: "Tipo Chave PIX :", options: EnumTipoChavePix.obterOpcoes(), val: ko.observable(EnumTipoChavePix.Nenhum), def: EnumTipoChavePix.Nenhum, visible: ko.observable(this.Tipo.val() == EnumTipoPropriedadeVeiculo.Terceiros) });
    this.ChavePIXCPFCNPJ = PropertyEntity({ text: ko.observable("Chave PIX (CPF/CNPJ): "), maxlength: 18, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.cpfCnpj });
    this.ChavePIXEmail = PropertyEntity({ text: ko.observable("Chave PIX (E-mail): "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.email });
    this.ChavePIXCelular = PropertyEntity({ text: ko.observable("Chave PIX (Celular): "), maxlength: 15, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.phone });
    this.ChavePIXAleatoria = PropertyEntity({ text: ko.observable("Chave PIX (Aleatória): "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.text });

    this.TipoChavePIX.val.subscribe(function (tipoPix) {
        ObterCampoPixVeiculo(self, self.TipoPagamento.val(), tipoPix, null);
    });

    this.TipoPagamento.val.subscribe(function (tipoPagamento) {
        const tipoBanco = tipoPagamento === EnumTipoPagamentoMDFe.Banco
        const tipoIpef = tipoPagamento === EnumTipoPagamentoMDFe.Ipef

        self.ContaCIOT.visible(tipoBanco);
        self.ContaCIOT.required(tipoBanco);
        self.AgenciaCIOT.visible(tipoBanco);
        self.AgenciaCIOT.required(tipoBanco);

        self.CNPJInstituicaoPagamento.visible(tipoIpef);
        self.CNPJInstituicaoPagamento.required(tipoIpef);

        ObterCampoPixVeiculo(self, tipoPagamento, self.TipoChavePIX.val(), null);
    });

    this.Tipo.val.subscribe(function (novoValor) {
        const mostrarDadosBancarios = novoValor === EnumTipoPropriedadeVeiculo.Terceiros

        self.ValorFrete.visible(mostrarDadosBancarios);
        self.ValorAdiantamento.visible(mostrarDadosBancarios);
        self.FormaPagamento.visible(mostrarDadosBancarios);
        self.DataVencimento.visible(mostrarDadosBancarios);
        self.TipoPagamento.visible(mostrarDadosBancarios);
        self.TipoChavePIX.visible(mostrarDadosBancarios);

        if (mostrarDadosBancarios)
            ObterCampoPixVeiculo(self, self.TipoPagamento.val(), self.TipoChavePIX.val(), null);
        else {
            self.ChavePIXCPFCNPJ.visible(mostrarDadosBancarios);
            self.ChavePIXEmail.visible(mostrarDadosBancarios);
            self.ChavePIXCelular.visible(mostrarDadosBancarios);
            self.ChavePIXAleatoria.visible(mostrarDadosBancarios);
        }
    });

    this.NumeroCompraValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroCompraValePedagio.getFieldDescription(), maxlength: 12, required: false, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(false) });
    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.FornecedorValePedagio.getFieldDescription()), required: false, idBtnSearch: guid(), issue: 58, visible: ko.observable(false), enable: ko.observable(true) });
    this.ResponsavelValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.ResponsavelValePedagio.getFieldDescription()), required: false, idBtnSearch: guid(), issue: 58, visible: ko.observable(false), enable: ko.observable(true) });
    this.EmpresaFilial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Veiculos.Veiculo.Empresa.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.TagPlaca = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#PlacaVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.Placa, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagRanavam = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#RENAVAMVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.Renavam, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNomeProprietario = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#NomeProprietarioVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.NomeProprietario, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCPFCNPJProprietario = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#CPFCNPJProprietarioVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.CPFCNPJProprietario, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagRNTRC = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#RNTRCProprietario#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.Rntrc, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagUFVeiculo = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#UFVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.UF, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagMarcaVeiculo = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#MarcaVeiculo#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.Marca, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagPlacasVinculadas = PropertyEntity({ eventClick: function (e) { InserirTag(_veiculo.ObservacaoCTe.id, "#PlacasVinculadas#"); }, type: types.event, text: Localization.Resources.Veiculos.Veiculo.PlacasVinculadas, enable: ko.observable(true), visible: ko.observable(true) });

    //Aba Outros Dados
    this.Chassi = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.NumeroChassi.getFieldDescription()), maxlength: 20, required: false, enable: ko.observable(true) });
    this.DataAquisicao = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.DataAquisicao.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataValidadeGerenciadoraRisco = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.DataValidadeGR.getFieldDescription(), required: false, visible: ko.observable(true), enable: false });
    this.DataUltimoChecklist = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.DataUltimoChecklist.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataValidadeANTT = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataValidadeANTT.getFieldDescription()), required: ko.observable(false), enable: ko.observable(true) });
    this.DataValidadeLiberacaoSeguradora = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.DataValidadeSeguroTerceiro.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.ValorAquisicao = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Veiculos.Veiculo.ValorAquisicao.getFieldDescription(), required: false, maxlength: 12, visible: ko.observable(true), enable: ko.observable(true) });
    this.AnoFabricacao = PropertyEntity({ getType: typesKnockout.year, maxlength: 5, text: ko.observable(Localization.Resources.Veiculos.Veiculo.AnoFabricacao.getFieldDescription()), required: ko.observable(false), enable: ko.observable(true) });
    this.AnoModelo = PropertyEntity({ getType: typesKnockout.year, maxlength: 5, text: ko.observable(Localization.Resources.Veiculos.Veiculo.AnoModelo.getFieldDescription()), required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroMotor = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroMotor.getFieldDescription(), maxlength: 50, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.GarantiaEscalonada = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.VencimentoGarantiaEscalonada.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.GarantiaPlena = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Veiculos.Veiculo.VencimentoGarantiaPlena.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroContrato.getFieldDescription(), maxlength: 50, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.SegmentoVeiculo.getFieldDescription()), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroFrota = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroFrota.getFieldDescription(), required: false, maxlength: 30, visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), required: false, maxlength: 100, visible: ko.observable(true), enable: ko.observable(true) });

    this.PossuiControleDisponibilidade = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.ControlarDisponibilidade, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.PossuiTravaQuintaDeRoda = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.PossuiTravaQuintaDeRoda, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.PossuiTelemetria = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.PossuiTelemetria, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.PossuiImobilizador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.PossuiImobilizador, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.AtivarConsultarAbastecimentoAngelLira = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.RealizarConsultaAbastecimentoVeiculoAngelLira.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 300, required: false, enable: ko.observable(true) });
    this.KilometragemAtual = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Veiculos.Veiculo.KMAtual.getFieldDescription(), required: false, configInt: { precision: 0, allowZero: true }, visible: ko.observable(true), enable: ko.observable(true) });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.FuncionarioResponsavel.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoPlotagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.TipoPlotagem.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.FormaDeducaoValePedagio = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.FormaDeducaoValePedagio.getFieldDescription(), options: EnumFormaDeducaoValePedagio.obterOpcoes(), val: ko.observable(EnumFormaDeducaoValePedagio.NaoAplicado), def: EnumFormaDeducaoValePedagio.NaoAplicado, visible: ko.observable(true) });
    this.LocalAtualFisicoDoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.Veiculo.LocalOndeVeiculoEstaMomento.getFieldDescription()), required: false, idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.PadraoEmissao = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.PadraoEmissao.getFieldDescription(), visible: ko.observable(true) });
    this.FatorEmissao = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.FatorEmissao.getFieldDescription(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.CentroResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Cor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.Cor.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.ValorContainerAverbacao = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Veiculos.Veiculo.ValorContainerAverbacao.getFieldDescription(), required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true) });
    this.PosicaoReboque = PropertyEntity({ val: ko.observable(PosicaoReboque.NaoInformado), options: PosicaoReboque.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.PosicaoReboque.getFieldDescription(), def: PosicaoReboque.NaoInformado, enable: ko.observable(true), visible: ko.observable(false) });
    this.BloquearVeiculo = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Veiculos.Veiculo.BloquearVeiculo, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ObrigatorioInformarDataValidadeAdicionalCarroceria = PropertyEntity({ getType: typesKnockout.bool });
    this.VeiculoAlugado = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.VeiculoAlugado, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.FormaPagamento.val.subscribe(function (novoValor) {
        if (novoValor === EnumFormaPagamentoCIOT.AVista) {
            _veiculo.DataVencimento.visible(false);
            _veiculo.ValorAdiantamento.val("");
            _veiculo.ValorAdiantamento.visible(false);
            _veiculo.DataVencimento.required(false);
            _veiculo.DataVencimento.val("");
        } else if (novoValor === EnumFormaPagamentoCIOT.APrazo) {
            _veiculo.DataVencimento.visible(true);
            _veiculo.DataVencimento.required(true);
            _veiculo.ValorAdiantamento.visible(true);
        } else {
            _veiculo.DataVencimento.visible(false);
            _veiculo.DataVencimento.required(false);
            _veiculo.ValorAdiantamento.visible(false);
        }
    });

    this.VeiculoAlugado.val.subscribe(function (novoValor) {
        if (novoValor) {
            _veiculo.Locador.visible(true);
            _veiculo.Locador.required(true);
        }
        else {
            _veiculo.Locador.visible(false);
            _veiculo.Locador.required(false);
        }
    });

    this.BloquearVeiculo.val.subscribe(function (valor) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo, _PermissoesPersonalizadas))
            return;

        _veiculo.MotivoBloqueio.visible(valor);
    });

    this.MotivoBloqueio = PropertyEntity({ maxlength: 150, text: Localization.Resources.Veiculos.Veiculo.MotivoBloqueio.getFieldDescription(), visible: ko.observable(false) });

    //Grupo Bovinos
    this.TipoCombustivel = PropertyEntity({ val: ko.observable(EnumTipoCombustivel.Outros), options: EnumTipoCombustivel.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoCombustivel.getFieldDescription(), def: EnumTipoCombustivel.Outros, enable: ko.observable(true) });
    this.TipoCarreta = PropertyEntity({ val: ko.observable(EnumTipoCarreta.Outros), options: EnumTipoCarreta.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoCarreta.getFieldDescription(), def: EnumTipoCarreta.Outros, enable: ko.observable(true) });
    this.TipoMaterialGaiola = PropertyEntity({ val: ko.observable(EnumTipoMaterial.Outros), options: EnumTipoMaterial.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoMaterialGaiola.getFieldDescription(), def: EnumTipoMaterial.Outros, enable: ko.observable(true) });
    this.TipoMaterialPiso = PropertyEntity({ val: ko.observable(EnumTipoMaterial.Outros), options: EnumTipoMaterial.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoMaterialPiso.getFieldDescription(), def: EnumTipoMaterial.Outros, enable: ko.observable(true) });
    this.TipoSistemaElevacao = PropertyEntity({ val: ko.observable(EnumTipoSistemaElevacao.Outros), options: EnumTipoSistemaElevacao.obterOpcoes(), text: Localization.Resources.Veiculos.Veiculo.TipoSistemaElevacao.getFieldDescription(), def: EnumTipoSistemaElevacao.Outros, enable: ko.observable(true) });
    this.QuantidadeCurrais = PropertyEntity({ val: ko.observable(0), options: _quantidadeCurrais, text: Localization.Resources.Veiculos.Veiculo.QuantidadeCurrais.getFieldDescription(), def: 0, enable: ko.observable(true) });
    this.ListaCurrais = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.QuantidadeCurrais.val.subscribe(function (novoValor) {
        ajustarAbasCurrais(novoValor);
    });

    //Aba Transportadoras
    this.Transportadoras = PropertyEntity({ type: types.listEntity, list: new Array(), text: Localization.Resources.Veiculos.Veiculo.Transportador.getRequiredFieldDescription(), issue: 69, codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarTransportadora = PropertyEntity({ eventClick: adicionarTransportadorClick, type: types.event, text: Localization.Resources.Veiculos.Veiculo.AdicionarTransportador, visible: ko.observable(true) });

    //Aba Equipamentos
    this.Equipamentos = PropertyEntity({ type: types.listEntity, list: new Array(), text: Localization.Resources.Veiculos.Veiculo.Equipamento.getRequiredFieldDescription(), codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarEquipamento = PropertyEntity({ eventClick: adicionarEquipamentoClick, type: types.event, text: Localization.Resources.Veiculos.Veiculo.AdicionarEquipamento, visible: ko.observable(true) });

    //Aba Motoristas Secundários
    this.Motoristas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.CPFMotoristaSecundario = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Veiculos.Veiculo.CPFMotorista.getRequiredFieldDescription(), issue: 3, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid(), getType: typesKnockout.cpf });
    this.NomeMotoristaSecundario = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NomeMotorista.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.CPFMotoristaSecundario.val.subscribe(function (val) { CodigoMotoristaSecundarioVeiculoModificado(val, self); });
    this.CPFSecundario = PropertyEntity({ visible: false });
    this.CodigoMotoristaSecundario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.AdicionarMotorista = PropertyEntity({ eventClick: adicionarMotoristaClick, type: types.event, text: Localization.Resources.Veiculos.Veiculo.AdicionarMotorista, visible: ko.observable(true) });

    //Aba Veículos Vinculados
    this.ValidarPlaca = PropertyEntity({ eventClick: validarPlacaClick, idBtnSearch: guid(), type: types.event, text: Localization.Resources.Veiculos.Veiculo.Validar, visible: ko.observable(true) });
    this.VeiculosVinculados = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });

    //Aba Licenças
    this.Licencas = PropertyEntity({ type: types.local, val: ko.observableArray(new Array()), def: new Array(), idGrid: guid() });
    this.AdicionarLicencaVeiculo = PropertyEntity({ eventClick: adicionarVeiculoLicenca, type: types.event, text: Localization.Resources.Veiculos.Veiculo.AdicionarLicenca, visible: ko.observable(true), enable: ko.observable(true) });

    //Aba Rastreador
    this.PossuiRastreador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.VeiculoPossuiRastreador, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.PossuiLocalizador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.VeiculoPossuiLocalizador, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoIntegrarOpentech = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NaoGerarIntegracaoOpentech, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.TipoComunicacaoRastreador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.TipoComunicacaoRastreador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TecnologiaRastreador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.TecnologiaRastreador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroEquipamentoRastreador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.NumeroEquipamentoRastreador.getRequiredFieldDescription(), maxlength: 150, enable: ko.observable(true), required: ko.observable(false) });

    this.Terminal = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Terminal.getFieldDescription(), maxlength: 150, enable: ko.observable(false), required: false, visible: ko.observable(true) });
    this.DataUltimaPosicao = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.DataUltimaPosicao.getFieldDescription(), maxlength: 150, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.Rastreador = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.Rastreador.getFieldDescription(), getType: typesKnockout.bool, required: false, enable: ko.observable(false), visible: ko.observable(true) });

    this.PossuiRastreador.val.subscribe(function (novoValor) {
        self.TipoComunicacaoRastreador.required = novoValor;
        self.TecnologiaRastreador.required = novoValor;
        self.NumeroEquipamentoRastreador.required = novoValor;

        if (novoValor == false) {
            self.NumeroEquipamentoRastreador.val("");
            LimparCampoEntity(self.TecnologiaRastreador);
            LimparCampoEntity(self.TipoComunicacaoRastreador);
        }
    });

    this.Empresa.val.subscribe(function (novoValor) {
        if (novoValor == "" && _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
            _veiculo.Renavam.required = true;
            _veiculo.Renavam.enable(true);
            _veiculo.Renavam.text(Localization.Resources.Veiculos.Veiculo.Renavam.getRequiredFieldDescription());
        }
    });

    //Aba Rotas Frete
    this.GridRotasFretes = PropertyEntity({ type: types.local, id: guid() });
    this.RotaFrete = PropertyEntity({ type: types.event, text: Localization.Resources.Veiculos.Veiculo.AdicionarRotaDeFrete, idBtnSearch: guid() });
    this.RotasFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Aba Liberacões GR
    this.CodigoLiberacaoGR = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LicencaLiberacaoGR = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Veiculos.Veiculo.Licenca), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DescricaoLiberacaoGR = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription()), maxlength: 200, required: false });
    this.NumeroLiberacaoGR = PropertyEntity({ text: ko.observable(Localization.Resources.Veiculos.Veiculo.Numero.getRequiredFieldDescription()), maxlength: 20, required: false });
    this.DataEmissaoLiberacaoGR = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataEmissao.getRequiredFieldDescription()), required: false });
    this.DataVencimentoLiberacaoGR = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Veiculos.Veiculo.DataVencimento.getRequiredFieldDescription()), required: false });

    this.GridVeiculoLiberacoesGR = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.AdicionarLiberacaoGR = PropertyEntity({ eventClick: adicionarVeiculoLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarLiberacaoGR = PropertyEntity({ eventClick: atualizarVeiculoLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirLiberacaoGR = PropertyEntity({ eventClick: excluirVeiculoLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarLiberacaoGR = PropertyEntity({ eventClick: limparCamposVeiculoLiberacoesGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.BaixarQrCode = PropertyEntity({ eventClick: baixarQrCodeClick, type: types.event, text: Localization.Resources.Veiculos.Veiculo.BaixarQRCode, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "Veiculo/Importar",
        UrlConfiguracao: "Veiculo/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.I002_Veiculos,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        },
        CallbackImportacao: function () {
            _gridVeiculo.CarregarGrid();
        }
    });
};

var InformarGrupoServico = function () {
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.Veiculo.GrupoServico, idBtnSearch: guid(), required: ko.observable(true) });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            informarGrupoServicoClick(e);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function ObterCampoPixVeiculo(objeto, tipoPagamento, tipoChavePix, valor) {
    objeto.TipoChavePIX.visible(false);
    objeto.ChavePIXCPFCNPJ.visible(false);
    objeto.ChavePIXCPFCNPJ.required(false);
    objeto.ChavePIXEmail.visible(false);
    objeto.ChavePIXEmail.required(false);
    objeto.ChavePIXCelular.visible(false);
    objeto.ChavePIXCelular.required(false);
    objeto.ChavePIXAleatoria.visible(false);
    objeto.ChavePIXAleatoria.required(false);

    if (tipoPagamento === EnumTipoPagamentoMDFe.PIX) {
        objeto.TipoChavePIX.visible(true);
        objeto.TipoChavePIX.val(tipoChavePix);

        switch (tipoChavePix) {
            case EnumTipoChavePix.CPFCNPJ:
                if (valor) objeto.ChavePIXCPFCNPJ.val(valor);
                objeto.ChavePIXCPFCNPJ.visible(true);
                objeto.ChavePIXCPFCNPJ.required(true);
                break;
            case EnumTipoChavePix.Email:
                if (valor) objeto.ChavePIXEmail.val(valor);
                objeto.ChavePIXEmail.visible(true);
                objeto.ChavePIXEmail.required(true);
                break;
            case EnumTipoChavePix.Celular:
                if (valor) objeto.ChavePIXCelular.val(valor);
                objeto.ChavePIXCelular.visible(true);
                objeto.ChavePIXCelular.required(true);
                break;
            case EnumTipoChavePix.Aleatoria:
                if (valor) objeto.ChavePIXAleatoria.val(valor);
                objeto.ChavePIXAleatoria.visible(true);
                objeto.ChavePIXAleatoria.required(true);
                break;
        }
    }
}

function loadVeiculo() {

    ObterTiposIntegracaoVeiculo().then(function () {
        _tipoCarrocerie = [
            { text: Localization.Resources.Veiculos.Veiculo.NaoAplicado, value: "00" },
            { text: Localization.Resources.Veiculos.Veiculo.Aberta, value: "01" },
            { text: Localization.Resources.Veiculos.Veiculo.FechadaBau, value: "02" },
            { text: Localization.Resources.Veiculos.Veiculo.Granel, value: "03" },
            { text: Localization.Resources.Veiculos.Veiculo.PortaContainer, value: "04" },
            { text: Localization.Resources.Veiculos.Veiculo.Utilitario, value: "05" },
            { text: Localization.Resources.Veiculos.Veiculo.Sider, value: "06" }
        ];

        _veiculo = new Veiculo();

        KoBindings(_veiculo, "knockoutCadastroVeiculo");

        _pesquisaVeiculo = new PesquisaVeiculo();
        KoBindings(_pesquisaVeiculo, "knockoutPesquisaVeiculo", false, _pesquisaVeiculo.Pesquisar.id);
        carregarFiltrosPesquisaInicial();

        HeaderAuditoria("Veiculo", _veiculo);

        _informarGrupoServico = new InformarGrupoServico();
        KoBindings(_informarGrupoServico, "divModalInformarGrupoServico");

        ObterConfiguracaoPadrao().then(function () {
            if (_configuracaoPadrao.ObrigatorioInformarAnoFabricacao) {
                _veiculo.AnoFabricacao.text(Localization.Resources.Veiculos.Veiculo.AnoFabricacao.getRequiredFieldDescription());
                _veiculo.AnoFabricacao.required(true);

                _veiculo.AnoModelo.text(Localization.Resources.Veiculos.Veiculo.AnoModelo.getRequiredFieldDescription());
                _veiculo.AnoModelo.required(true);
            } else {
                _veiculo.AnoFabricacao.text(Localization.Resources.Veiculos.Veiculo.AnoFabricacao.getFieldDescription());
                _veiculo.AnoFabricacao.required(false);

                _veiculo.AnoModelo.text(Localization.Resources.Veiculos.Veiculo.AnoModelo.getFieldDescription());
                _veiculo.AnoModelo.required(false);
            }
        });

        loadVeiculoIntegracoes();
        loadAprovacaoCadastroVeiculo();
        loadCadastroVeiculoAutorizacao();
        LoadRotasFrete();

        ObterIntegracoesHabilitadas();

        if (_CONFIGURACAO_TMS.ExibirInformacoesBovinos)
            $("#liExibirInformacoesBovinos").show();

        if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
            _veiculo.Renavam.required = true;
            _veiculo.Renavam.visible(true);
            _veiculo.Renavam.text(Localization.Resources.Veiculos.Veiculo.Renavam.getRequiredFieldDescription());
            _veiculo.FatorEmissao.visible(false);
            _veiculo.PadraoEmissao.visible(false);
        }
        else if (_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) {
            _veiculo.Renavam.required = false;
            _veiculo.Renavam.visible(false);
            _veiculo.Renavam.text(Localization.Resources.Veiculos.Veiculo.Renavam.getFieldDescription());
            _veiculo.FatorEmissao.visible(true);
            _veiculo.PadraoEmissao.visible(true);
        }
        new BuscarTransportadores(_pesquisaVeiculo.Empresa, null, null, null, null, null, null, null, null, true);

        $("#" + _veiculo.RNTRC.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

        $("#" + _veiculo.Renavam.id).mask("00000000000", { selectOnFocus: true, clearIfNotMatch: true });

        $("#" + _veiculo.Renavam.id).on("blur", function () {
            var val = $(this).val();
            if ((val.length != 11 || !/^\d+$/.test(val)) && _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Veiculos.Veiculo.RenavamInvalido, Localization.Resources.Veiculos.Veiculo.RenavamDevePossuirOnzeDigitos);
            }
        });

        $("#" + _veiculo.ChavePIXCelular.id).mask("(00) 00000-0000", { selectOnFocus: true, clearIfNotMatch: true });

        $("#" + _veiculo.ChavePIXCPFCNPJ.id).mask('000.000.000-000', {
            selectOnFocus: true,
            translation: {
                '0': { pattern: /[0-9]/ }
            },
            onKeyPress: function (val, e, field, options) {
                var cleanVal = val.replace(/\D/g, '');
                var masks = ['000.000.000-000', '00.000.000/0000-00'];
                var mask = (cleanVal.length > 11) ? masks[1] : masks[0];

                field.mask(mask, {
                    selectOnFocus: true,
                    translation: {
                        '0': { pattern: /[0-9]/ }
                    }
                });
            },
            onComplete: function (val) {
                var cleanVal = val.replace(/\D/g, '');
                if (cleanVal.length !== 11 && cleanVal.length !== 14) {
                    console.log('CPF/CNPJ incompleto');
                }
            }
        });

        $("#" + _veiculo.ChavePIXEmail.id).on('input blur', function () {
            var email = this.value;
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (email && !emailRegex.test(email)) {
                $(this).addClass('is-invalid');
            } else {
                $(this).removeClass('is-invalid');
            }

            _veiculo.ChavePIXEmail.val(email);
        });

        BuscarGrupoServico(_informarGrupoServico.GrupoServico);
        BuscarModelosVeiculo(_pesquisaVeiculo.ModeloVeiculo);
        BuscarMarcasVeiculo(_pesquisaVeiculo.MarcaVeiculo);
        BuscarModelosVeicularesCarga(_pesquisaVeiculo.ModeloVeicularCarga);
        BuscarMotoristas(_pesquisaVeiculo.Motorista);
        BuscarReboques(_pesquisaVeiculo.Reboque);
        BuscarSegmentoVeiculo(_pesquisaVeiculo.Segmento);
        BuscarClientes(_pesquisaVeiculo.Proprietario);
        BuscarModelosCarroceria(_veiculo.ModeloCarroceria, RetornoBuscaCarroceria);
        BuscarTransportadores(_veiculo.Empresa, RetornoEmpresa);
        BuscarModelosVeicularesCarga(_veiculo.ModeloVeicularCarga, retornoModeloVeicularCargaSelecionado, null, null, null, _veiculo.TipoVeiculo);
        _veiculo.ModeloVeicularCarga.codEntity.subscribe(ModeloVeicularCargaAlterado);
        BuscarClientes(_veiculo.Proprietario, retornoProprietario);
        BuscarClientes(_veiculo.Locador);
        BuscarModelosVeiculo(_veiculo.ModeloVeiculo, null, null, retornoModeloSelecionado, _veiculo.MarcaVeiculo);
        BuscarMarcasVeiculo(_veiculo.MarcaVeiculo, null, null, retornoMarcaSelecionada, _veiculo.ModeloVeiculo, _veiculo.TipoVeiculo);
        BuscarGruposPessoas(_veiculo.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Empresa);
        BuscarClientes(_veiculo.FornecedorValePedagio);
        BuscarClientes(_veiculo.LocalAtualFisicoDoVeiculo);
        BuscarClientes(_veiculo.ResponsavelValePedagio);
        BuscarSegmentoVeiculo(_veiculo.SegmentoVeiculo);
        BuscarTipoComunicacaoRastreador(_veiculo.TipoComunicacaoRastreador);
        BuscarTecnologiaRastreador(_veiculo.TecnologiaRastreador);
        BuscarFuncionario(_veiculo.FuncionarioResponsavel);
        BuscarTipoPlotagem(_veiculo.TipoPlotagem);
        BuscarCentroResultado(_veiculo.CentroResultado);
        BuscarRotasFrete(_veiculo.RotaFrete, null, _gridRotasFrete);
        BuscarCorVeiculo(_veiculo.Cor);
        BuscarFilial(_veiculo.FilialCarregamento);
        BuscarLocalidades(_veiculo.LocalidadeEmplacamento, null, null, retornoConsultaLocalidade);
        BuscarTransportadores(_veiculo.EmpresaFilial);
        BuscarTransportadores(_veiculo.EmpresaVeiculoCooperado);

        buscarVeiculos();

        loadVeiculoVinculado();
        loadTransportadorVeiculo();
        loadEquipamento();
        loadMotorista();
        loadVeiculoLicenca(_veiculo);
        loadAnexos();
        loadVeiculoLiberacaoGR();

        ConfigurarCamposPorTipoServico();

        ConfigurarIntegracoesDisponiveis();

        ValidarCamposReferenteCIOTChange(_veiculo);

        _veiculo.PossuiTagValePedagio.val(_CONFIGURACAO_TMS.PadraoTagValePedagioVeiculos);

        ValidarCampoTagValePedagioSemParar();

        buscarSeExisteRegraAlcadaCadastroVeiculo();

        if (_notificacaoGlobal.CodigoObjeto.val() != null && _notificacaoGlobal.CodigoObjeto.val() > 0) {
            var gridVeiGlobal = { Codigo: _notificacaoGlobal.CodigoObjeto.val() };
            editarVeiculo(gridVeiGlobal);
        }

    })
}

function ValidarCamposReferenteCIOTChange(knoutVeiculo) {

    let validarCamposReferenteCIOT = knoutVeiculo.ValidarCamposReferenteCIOT.val();

    if (validarCamposReferenteCIOT) {
        knoutVeiculo.Chassi.text("*" + _veiculo.Chassi.text());
        knoutVeiculo.Chassi.required = true;
        $("#liTabOutrosDadosVeiculo").show();
    } else {
        knoutVeiculo.Chassi.text(knoutVeiculo.Chassi.text().replace("*", ""));
        knoutVeiculo.Chassi.required = false;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        if (validarCamposReferenteCIOT) {
            knoutVeiculo.CPFMotorista.text("*" + knoutVeiculo.CPFMotorista.text());
            knoutVeiculo.CPFMotorista.required = true;
        } else {
            knoutVeiculo.CPFMotorista.text(knoutVeiculo.CPFMotorista.text().replace("*", ""));
            knoutVeiculo.CPFMotorista.required = false;
        }
    }
}

function ValidarCampoTagValePedagioSemParar() {

    executarReST("Veiculo/ObterConfiguracaoVeiculoSemParar", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.ConsultarVeiculoPossuiCadastroSemParar) {
                _veiculo.PossuiTagValePedagio.enable(false);
            }
        }
    });
}

function RetornoEmpresa(data) {
    _veiculo.Empresa.val(data.Descricao);
    _veiculo.Empresa.codEntity(data.Codigo);
    _veiculo.ValidarCamposReferenteCIOT.val(data.HabilitarCIOT);

    bloquearCampoRenavamPorTipoEmpresa(data.Tipo);
}

function SetarOpcoesTipoProprietario(cpfCnpj) {
    switch (cpfCnpj.length) {
        case 18: _veiculo.TipoProprietario.options(EnumTipoProprietarioVeiculo.obterOpcoesPessoaJuridica()); break;
        case 14: _veiculo.TipoProprietario.options(EnumTipoProprietarioVeiculo.obterOpcoesPessoaFisica()); break;
        default: _veiculo.TipoProprietario.options(EnumTipoProprietarioVeiculo.obterOpcoes()); break;
    }
}

function retornoProprietario(row) {
    _veiculo.Proprietario.codEntity(row.Codigo);
    _veiculo.Proprietario.val(row.Descricao);
    _veiculo.DataVigencia.val("");
    var data = { Codigo: row.Codigo };
    executarReST("Pessoa/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                SetarOpcoesTipoProprietario(arg.Data.CNPJ_CPF)

                if (arg.Data.TransportadorTerceiro != null) {
                    _veiculo.RNTRC.val(arg.Data.TransportadorTerceiro.RNTRC);
                    _veiculo.TipoProprietario.val(arg.Data.TransportadorTerceiro.TipoTransportadorTerceiro);
                }
            } else {
                SetarOpcoesTipoProprietario(arg.Data.CNPJ_CPF)
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
//function retornoLocador(row) {
//    _veiculo.Locador.codEntity(row.Codigo);
//    _veiculo.Locador.val(row.Descricao);
//    var data = { Codigo: row.Codigo };
//    executarReST("Pessoa/BuscarPorCodigo", data, function (arg) {
//        if (arg.Success) {
//            if (arg.Data !== false) {
//                if (arg.Data.TransportadorTerceiro != null) {
//                    _veiculo.RNTRC.val(arg.Data.TransportadorTerceiro.RNTRC);
//                    _veiculo.TipoProprietario.val(arg.Data.TransportadorTerceiro.TipoTransportadorTerceiro);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
//        }
//    });
//}

function retornoConsultaLocalidade(registroSelecionado) {
    _veiculo.LocalidadeEmplacamento.codEntity(registroSelecionado.Codigo);
    _veiculo.LocalidadeEmplacamento.val(registroSelecionado.Descricao);
    _veiculo.Estado.val(registroSelecionado.Estado);
}

function ConfigurarCamposPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        ConfigurarCamposNFe();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        ConfigurarCamposTMS();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        ConfigurarCamposEmbarcador();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        ConfigurarCamposMultiCTe();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros) {
        ConfigurarCamposTerceiros();
    }

    if (_CONFIGURACAO_TMS.ObrigatorioSegmentoVeiculo) {
        _veiculo.SegmentoVeiculo.required(true);
        _veiculo.SegmentoVeiculo.text(Localization.Resources.Veiculos.Veiculo.SegmentoVeiculo.getRequiredFieldDescription());
    }
}

function ConfigurarCamposTerceiros() {
    _pesquisaVeiculo.Empresa.visible(false);
    _pesquisaVeiculo.MarcaVeiculo.visible(false);
    _pesquisaVeiculo.ModeloVeiculo.visible(false);

    _veiculo.Empresa.visible(false);
    _veiculo.Empresa.required = false;
    _veiculo.MarcaVeiculo.visible(false);
    _veiculo.MarcaVeiculo.required = false;
    _veiculo.ModeloVeiculo.visible(false);
    _veiculo.ModeloVeiculo.required = false;
    _veiculo.GrupoPessoa.visible(false);
    _veiculo.GrupoPessoa.required = false;
    _veiculo.ModeloVeicularCarga.visible(true);
    _veiculo.ModeloVeicularCarga.required(false);
    _veiculo.ModeloCarroceria.visible(false);
    _veiculo.ModeloCarroceria.required = false;
    _veiculo.ModeloVeicularCarga.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.Ativo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.TipoVeiculo.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-3");
    _veiculo.CPFMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.NomeMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.TipoCarroceria.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
    _veiculo.Tipo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.TipoRodado.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
    _veiculo.CIOT.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
    _veiculo.RNTRC.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");

    _veiculo.Tipo.val("T");
    _veiculo.Tipo.visible(false);
    mudouPropriedadeOnChange();

    _veiculo.Proprietario.visible(false);
    _veiculo.Proprietario.required = false;

    _veiculo.TagPlaca.visible(true);
    _veiculo.TagRanavam.visible(true);
    _veiculo.TagNomeProprietario.visible(false);
    _veiculo.TagCPFCNPJProprietario.visible(false);
    _veiculo.TagRNTRC.visible(false);
    _veiculo.TagUFVeiculo.visible(true);
    _veiculo.TagMarcaVeiculo.visible(true);
    _veiculo.TagPlacasVinculadas.visible(true);

    $("#liTabOutrosDadosVeiculo").hide();
    $("#liTabVeiculoEquipamento").hide();
    $("#liTabVeiculoTransportador").hide();
    $(".removeMultiCTe").remove();
}

function ConfigurarCamposMultiCTe() {
    _pesquisaVeiculo.Empresa.visible(false);
    _pesquisaVeiculo.MarcaVeiculo.visible(false);
    _pesquisaVeiculo.ModeloVeiculo.visible(false);

    _veiculo.ModeloVeicularCarga.cssClass("col col-xs-12 col-sm-12 col-md-8 col-lg-6");
    _veiculo.Ativo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.Empresa.visible(false);
    _veiculo.Empresa.required = false;
    _veiculo.MarcaVeiculo.visible(false);
    _veiculo.MarcaVeiculo.required = false;
    _veiculo.ModeloVeiculo.visible(false);
    _veiculo.ModeloVeiculo.required = false;
    _veiculo.ModeloCarroceria.visible(false);
    _veiculo.ModeloCarroceria.required = false;
    _veiculo.GrupoPessoa.visible(false);
    _veiculo.GrupoPessoa.required = false;
    _veiculo.DataValidadeLiberacaoSeguradora.visible(true);
    _veiculo.TipoVeiculo.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-3");
    _veiculo.CPFMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.NomeMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.TipoCarroceria.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
    _veiculo.Tipo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.TipoRodado.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");

    if (_veiculo.ValidarCamposReferenteCIOT)
        $("#liTabOutrosDadosVeiculo").show();
    else
        $("#liTabOutrosDadosVeiculo").hide();

    $("#liTabVeiculoEquipamento").hide();
    $("#liTabVeiculoTransportador").hide();

    $(".removeMultiCTe").remove();

    if (_CONFIGURACAO_TMS.BloquearAlteracaoVeiculoPortalTransportador) {
        $("#liTabOutrosDadosVeiculo").hide();
        $("#liTabVeiculoVeiculado").hide();
        $("#liTabVeiculoLicenca").hide();
        $("#liTabVeiculoRastreador").hide();
        $("#liTabAnexos").hide();
        $("#liTabIntegracoes").hide();
        _veiculo.Importar.visible(false);
        _veiculo.Adicionar.visible(false);
        _veiculo.Excluir.visible(false);

        SetarEnableCamposKnockout(_veiculo, false);

        _veiculo.Tipo.enable(true);
        _veiculo.Proprietario.enable(true);
        _veiculo.TipoProprietario.enable(true);
        _veiculo.RNTRC.enable(true);
        _veiculo.ValorValePedagio.enable(true);
        _veiculo.NumeroCompraValePedagio.enable(true);
        _veiculo.FornecedorValePedagio.enable(true);
        _veiculo.ResponsavelValePedagio.enable(true);
    }

    if (_CONFIGURACAO_TMS.NaoPermitirQueTransportadorInativeVeiculo) {
        _veiculo.Ativo.visible(false);
        _veiculo.Ativo.enable(false);
        _veiculo.ModeloVeicularCarga.cssClass("col col-xs-12 col-sm-12 col-md-9 col-lg-3");
    }
};

function ConfigurarCamposEmbarcador() {
    _pesquisaVeiculo.Empresa.visible(true);
    _pesquisaVeiculo.MarcaVeiculo.visible(false);

    _veiculo.Empresa.visible(true);
    _veiculo.Empresa.required = true;
    _veiculo.Empresa.text(Localization.Resources.Veiculos.Veiculo.Transportador.getRequiredFieldDescription());
    _veiculo.MarcaVeiculo.required = false;
    _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.MarcaVeiculo.getFieldDescription());
    _veiculo.ModeloVeiculo.required = false;
    _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.ModeloVeiculo.getFieldDescription());
    _veiculo.LocalAtualFisicoDoVeiculo.visible(true);
    _veiculo.KilometragemAtual.visible(true);
    _veiculo.DataAquisicao.visible(false);
    _veiculo.ValorAquisicao.visible(false);
    _veiculo.NumeroMotor.visible(false);
    _veiculo.GarantiaEscalonada.visible(false);
    _veiculo.GarantiaPlena.visible(false);
    _veiculo.NumeroContrato.visible(false);
    _veiculo.SegmentoVeiculo.visible(false);
    _veiculo.DataValidadeLiberacaoSeguradora.visible(true);
    _veiculo.NaoValidarIntegracaoParaFilaCarregamento.visible(_CONFIGURACAO_TMS.UtilizarFilaCarregamento);
    _veiculo.CapacidadeTanque.visible(false);
    _veiculo.CapacidadeMaximaTanque.visible(false);
    _veiculo.DataValidadeLiberacaoSeguradora.enable(true);

    var permiteBloquearVeiculo = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo, _PermissoesPersonalizadas);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagio, _PermissoesPersonalizadas))
        _veiculo.NaoComprarValePedagio.enable(true);
    else
        _veiculo.NaoComprarValePedagio.enable(false);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagioRetorno, _PermissoesPersonalizadas))
        _veiculo.NaoComprarValePedagioRetorno.enable(true);
    else
        _veiculo.NaoComprarValePedagioRetorno.enable(false);

    _veiculo.BloquearVeiculo.visible(permiteBloquearVeiculo);

    $("#liTabOutrosDadosVeiculo").show();
    $("#liTabVeiculoEquipamento").show();
    $("#liTabVeiculoTransportador").hide();
    $("#liRotasFrete").removeClass("d-none");
    $(".removeEmbarcador").remove();

    if (!(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermitirAcessoRastreador, _PermissoesPersonalizadas))) {
        $("#liTabVeiculoRastreador").hide();
    } else {
        $("#liTabVeiculoRastreador").show();
    }

    if (_CONFIGURACAO_TMS.ObrigarANTTVeiculoValidarSalvarDadosTransporte) {
        _veiculo.DataValidadeANTT.required(true);
        _veiculo.DataValidadeANTT.text(Localization.Resources.Veiculos.Veiculo.DataValidadeANTT.getRequiredFieldDescription());
    }
}

function ConfigurarCamposNFe() {
    _pesquisaVeiculo.Empresa.visible(false);
    _pesquisaVeiculo.MarcaVeiculo.visible(false);
    _pesquisaVeiculo.ModeloVeiculo.visible(false);
    _pesquisaVeiculo.ModeloVeicularCarga.visible(false);

    _veiculo.Ativo.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _veiculo.Empresa.visible(false);
    _veiculo.Empresa.required = false;
    _veiculo.MarcaVeiculo.visible(true);
    _veiculo.MarcaVeiculo.required = false;
    _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.MarcaVeiculo.getFieldDescription());
    _veiculo.ModeloVeiculo.visible(true);
    _veiculo.ModeloVeiculo.required = false;
    _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.ModeloVeiculo.getFieldDescription());
    _veiculo.GrupoPessoa.visible(false);
    _veiculo.GrupoPessoa.required = false;
    _veiculo.ModeloVeicularCarga.visible(true);
    _veiculo.ModeloVeicularCarga.required(false);
    _veiculo.ModeloVeicularCarga.text(Localization.Resources.Veiculos.Veiculo.ModeloVeicularCarga.getFieldDescription());
    _veiculo.ModeloCarroceria.visible(false);
    _veiculo.ModeloCarroceria.required = false;
    _veiculo.Importar.visible(false);
    _veiculo.TipoVeiculo.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-3");
    _veiculo.CPFMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.NomeMotorista.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
    _veiculo.TipoCarroceria.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
    _veiculo.Tipo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.TipoRodado.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
    _veiculo.CIOT.visible(false);
    _veiculo.PossuiTagValePedagio.visible(false);
    _veiculo.NaoComprarValePedagio.visible(false);
    _veiculo.TipoIntegracaoValePedagio.visible(false);
    _veiculo.ObservacaoCTe.visible(false);

    $("#liTabOutrosDadosVeiculo").show();
    $("#liTabVeiculoEquipamento").hide();
    $("#liTabVeiculoTransportador").hide();
    $("#liTabVeiculoRastreador").hide();

    $(".removeNFe").remove();
}

function ConfigurarCamposTMS() {
    _pesquisaVeiculo.Empresa.visible(false);
    _pesquisaVeiculo.MarcaVeiculo.visible(true);
    _pesquisaVeiculo.ModeloVeiculo.visible(true);

    _veiculo.Empresa.visible(false);
    _veiculo.Empresa.required = false;
    _veiculo.MarcaVeiculo.visible(true);
    _veiculo.MarcaVeiculo.required = true;
    _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.MarcaVeiculo.getRequiredFieldDescription());
    _veiculo.ModeloVeiculo.visible(true);
    _veiculo.ModeloVeiculo.required = true;
    _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.ModeloVeiculo.getRequiredFieldDescription());
    _veiculo.CentroResultado.visible(true);
    _veiculo.TipoPlotagem.visible(true);
    _veiculo.PossuiControleDisponibilidade.visible(true);
    _veiculo.VeiculoAlugado.visible(true);
    _veiculo.NaoValidarIntegracaoParaFilaCarregamento.visible(_CONFIGURACAO_TMS.UtilizarFilaCarregamento);

    $("#liTabOutrosDadosVeiculo").show();
    $("#liTabVeiculoEquipamento").show();
    $("#liTabVeiculoTransportador").show();

    $(".removeTMS").remove();
}

function loadTransportadorVeiculo() {
    new BuscarTransportadores(_veiculo.Transportadoras, retornoTransportador);
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirTransportadorClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJ", title: Localization.Resources.Veiculos.Veiculo.CNPJ, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Veiculos.Veiculo.RazaoSocial, width: "70%", className: "text-align-left" }
    ];
    _gridTransportadoras = new BasicDataTable(_veiculo.Transportadoras.idGrid, header, menuOpcoes);
    recarregarGridTransportadoras();
}

function ModeloVeicularCargaAlterado(cod) {
    if (cod > 0)
        return;

    _veiculo.ModeloVeicularCarga.val("");
    _veiculo.ModeloVeicularCarga.entityDescription("");
    _veiculo.CapacidadeQuilo.enable(true);
}

function retornoModeloVeicularCargaSelecionado(data) {
    _veiculo.ModeloVeicularCarga.codEntity(data.Codigo);
    _veiculo.ModeloVeicularCarga.val(data.Descricao);
    _veiculo.ModeloVeicularCarga.entityDescription(data.Descricao);
    _veiculo.PossuiLocalizador.visible(data.ModeloVeicularAceitaLocalizador);

    _veiculo.CapacidadeQuilo.enable(true);
    if (data.UnidadeCapacidade == EnumUnidadeCapacidade.Peso) {
        let isAcessoTransportador = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
        let capacidadeVeiculo = Globalize.parseInt(_veiculo.CapacidadeQuilo.val()) || 0;
        let capacidadeModeloVeicular = Globalize.parseInt(data.CapacidadePesoTransporte) || 0;

        if ((capacidadeVeiculo == 0 && !isAcessoTransportador) || isAcessoTransportador)
            _veiculo.CapacidadeQuilo.val(Globalize.format(capacidadeModeloVeicular, "n0"));

    }
}

function retornoMarcaSelecionada(data) {
    if (data != null && data.Codigo > 0) {
        _veiculo.MarcaVeiculo.codEntity(data.Codigo);
        _veiculo.MarcaVeiculo.val(data.Descricao);
    }
    if (data != null && data.DescricaoModelo != "" && data.CodigoModelo != 0) {
        _veiculo.ModeloVeiculo.codEntity(data.CodigoModelo);
        _veiculo.ModeloVeiculo.val(data.DescricaoModelo);
    }
}

function retornoModeloSelecionado(data) {
    if (data != null && data.NumeroEixo > 0) {
        _veiculo.MarcaVeiculo.codEntity(data.NumeroEixo);
        _veiculo.MarcaVeiculo.val(data.DescricaoMarca);
    }
    if (data != null && data.Codigo > 0) {
        _veiculo.ModeloVeiculo.codEntity(data.Codigo);
        _veiculo.ModeloVeiculo.val(data.Descricao);
    }
}

function retornoTransportador(data) {
    _transportador = new TransportadorMap();
    _transportador.Codigo.val = data.Codigo;
    _transportador.CNPJ.val = data.CNPJ;
    _transportador.Nome.val = data.Descricao;

    _veiculo.Transportadoras.codEntity(data.Codigo);
    _veiculo.Transportadoras.val(data.Descricao);
}

function adicionarTransportadorClick() {
    _veiculo.Transportadoras.requiredClass("form-control");
    let tudoCerto = ValidarCampoObrigatorioEntity(_veiculo.Transportadoras);
    tudoCerto = _veiculo.Transportadoras.codEntity() > 0;
    if (tudoCerto) {
        var existe = false;
        $.each(_veiculo.Transportadoras.list, function (i, Transportador) {
            if (Transportador.Codigo.val == _veiculo.Transportadoras.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _veiculo.Transportadoras.list.push(_transportador);
            recarregarGridTransportadoras();
            $("#" + _veiculo.Transportadoras.id).focus();
            _veiculo.Transportadoras.requiredClass("form-control ");
        } else {
            exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.TransportadorJaInformado, Localization.Resources.Veiculos.Veiculo.TransportadorJaInformadoParaEsteVeiculo.format(_veiculo.Transportadoras.val()));
        }
        LimparCampoEntity(_veiculo.Transportadoras);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Veiculos.Veiculo.InformeOsCamposObrigatorios);
        _veiculo.Transportadoras.requiredClass("form-control is-invalid");
    }
}

function excluirTransportadorClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirTransportador.format(data.Nome), function () {
        let listaAtualizada = new Array();
        $.each(_veiculo.Transportadoras.list, function (i, Transportador) {
            if (Transportador.Codigo.val != data.Codigo) {
                listaAtualizada.push(Transportador);
            }
        });
        _veiculo.Transportadoras.list = listaAtualizada;
        recarregarGridTransportadoras();
    });
}

function loadEquipamento() {
    new BuscarEquipamentos(_veiculo.Equipamentos, retornoEquipamento);
    let editar = { descricao: Localization.Resources.Veiculos.Veiculo.Remover, id: guid(), evento: "onclick", metodo: excluirEquipamentoClick, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "20%", className: "text-align-left" }
    ];
    _gridEquipamentos = new BasicDataTable(_veiculo.Equipamentos.idGrid, header, menuOpcoes);
    recarregarGridEquipamentos();
}

function retornoEquipamento(data) {
    _equipamento = new EquipamentoMap();
    _equipamento.Codigo.val = data.Codigo;
    _equipamento.Descricao.val = data.Descricao;
    _equipamento.Numero.val = data.Numero;

    _veiculo.Equipamentos.codEntity(data.Codigo);
    _veiculo.Equipamentos.val(data.Descricao);
}

function adicionarEquipamentoClick() {
    _veiculo.Equipamentos.requiredClass("form-control");
    let tudoCerto = ValidarCampoObrigatorioEntity(_veiculo.Equipamentos);
    tudoCerto = _veiculo.Equipamentos.codEntity() > 0;
    if (tudoCerto) {
        var existe = false;
        $.each(_veiculo.Equipamentos.list, function (i, Equipamento) {
            if (Equipamento.Codigo.val == _veiculo.Equipamentos.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {

            var data = { Codigo: _veiculo.Equipamentos.codEntity() };

            executarReST("Veiculo/ValidarEquipamentoPorCodigo", data, function (arg) {

                if (arg.Success && arg.Data != null) {
                    exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformado, arg.Data.Mensagem);
                } else {
                    _veiculo.Equipamentos.list.push(_equipamento);
                    recarregarGridEquipamentos();
                    $("#" + _veiculo.Equipamentos.id).focus();
                    _veiculo.Equipamentos.requiredClass("form-control");
                }
            });

        } else {
            exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformado, Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformadoParaEsteVeiculo.format(_veiculo.Equipamentos.val()));
        }



        LimparCampoEntity(_veiculo.Equipamentos);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Veiculos.Veiculo.InformeOsCamposObrigatorios);
        _veiculo.Equipamentos.requiredClass("form-control is-invalid");
    }
}

function excluirEquipamentoClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirEquipamento.format(data.Descricao), function () {
        let listaAtualizada = new Array();
        $.each(_veiculo.Equipamentos.list, function (i, Equipamento) {
            if (Equipamento.Codigo.val != data.Codigo) {
                listaAtualizada.push(Equipamento);
            }
        });
        _veiculo.Equipamentos.list = listaAtualizada;
        recarregarGridEquipamentos();
    });
}

var _integracaoSemParar = false;
function ObterIntegracoesHabilitadas() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.Target; }))
                    _integracaoSemParar = true;
                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.OpenTech; })) {
                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermitirAlterarDadosIntegracaoGR, _PermissoesPersonalizadas)))
                        _veiculo.NaoIntegrarOpentech.visible(true);
                }

                if (r.Data.TiposExistentes.some(function (o) {
                    return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.Repom || o == EnumTipoIntegracao.PagBem || o == EnumTipoIntegracao.DBTrans ||
                        o == EnumTipoIntegracao.Pamcard || o == EnumTipoIntegracao.QualP || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.Extratta || o == EnumTipoIntegracao.DigitalCom ||
                        o == EnumTipoIntegracao.RepomFrete || o == EnumTipoIntegracao.Ambipar || o == EnumTipoIntegracao.NDDCargo;
                }))
                    _veiculo.NaoComprarValePedagio.visible(true);

                if (_CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.SemParar || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Target || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Repom
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.PagBem || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.DBTrans || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Pamcard
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.QualP || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.EFrete || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Extratta
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.DigitalCom || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.RepomFrete || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Ambipar
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.NDDCargo) {
                    _veiculo.TipoIntegracaoValePedagio.val([_CONFIGURACAO_TMS.TipoIntegracaoValePedagio]);
                    $("#" + _veiculo.TipoIntegracaoValePedagio.id).trigger("change");
                }
            }
        }
    });
}

function adicionarClick(e, sender) {
    const valorCIOT = _veiculo.CIOT.val();

    if (valorCIOT && valorCIOT.trim() && valorCIOT.length !== 12) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "O Campo CIOT deve conter 12 digitos");
        return _veiculo.CIOT.requiredClass("form-control  is-invalid");
    }

    if (!validarRegrasVeiculo())
        return;

    if (_CONFIGURACAO_TMS.GerarOSAutomaticamenteCadastroVeiculoEquipamento) {

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.DesejaGerarUmaOrdemDeServicoParaNovoVeiculo, function () {
            Global.abrirModal('divModalInformarGrupoServico');
        }, function () {
            adicionarVeiculo();
        });
    }
    else {
        adicionarVeiculo();
    }

}

function adicionarVeiculo() {

    let mensagem = _integracaoSemParar
        ? Localization.Resources.Veiculos.Veiculo.TemCertezaQueTodosOsDadosDeCadastroEstaoCorretosInclusiveTag
        : Localization.Resources.Veiculos.Veiculo.TemCertezaQueTodosOsDadosDeCadastroEstaoCorretos;

    let veiculoEnviar = obterVeiculoSalvar();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, mensagem, function () {

        executarReST("Veiculo/Adicionar", veiculoEnviar, function (arg) {

            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    _gridVeiculo.CarregarGrid();
                    limparCamposVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function atualizarClick(e, sender) {

    if (!validarRegrasVeiculo())
        return;

    let mensagem = _integracaoSemParar
        ? Localization.Resources.Veiculos.Veiculo.TemCertezaQueTodosOsDadosDeCadastroEstaoCorretosInclusiveTag
        : Localization.Resources.Veiculos.Veiculo.TemCertezaQueTodosOsDadosDeCadastroEstaoCorretos;

    let veiculoEnviar = obterVeiculoSalvar();
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, mensagem, function () {
        executarReST("Veiculo/Atualizar", veiculoEnviar, function (arg) {

            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    _gridVeiculo.CarregarGrid();
                    limparCamposVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function baixarQrCodeClick(e) {
    executarDownload("Veiculo/BaixarQrCode", { Codigo: _veiculo.Codigo.val() });
}

function baixarTodosQrCodeClick() {
    executarDownloadArquivo("Veiculo/BaixarTodosQrCode", RetornarObjetoPesquisa(_pesquisaVeiculo));
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirOVeiculo.format(_veiculo.Placa.val()), function () {
        ExcluirPorCodigo(_veiculo, "Veiculo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridVeiculo.CarregarGrid();
                    limparCamposVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposVeiculo();
}

function verificarTipoVeiculoChange() {
    verificarTipoVeiculo(false);
}

function verificarTipoVeiculo(editando) {
    if (_veiculo.TipoVeiculo.val() == EnumTipoVeiculo.Reboque) {
        resetarTabs();
        $("#liTabVeiculoVeiculado").hide();
        _veiculo.PosicaoReboque.visible(true);

        if (!editando)
            LimparCampo(_veiculo.ModeloVeicularCarga);
    } else {
        $("#liTabVeiculoVeiculado").show();
        _veiculo.PosicaoReboque.visible(false);
        LimparCampo(_veiculo.PosicaoReboque);

        if (!editando)
            LimparCampo(_veiculo.ModeloVeicularCarga);
    }
}

function mudouPropriedadeOnChange(e) {
    if (_veiculo.Tipo.val() == "T") {
        _veiculo.TipoProprietario.required = true;
        _veiculo.RNTRC.required = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS;
        _veiculo.RNTRC.text(Localization.Resources.Veiculos.Veiculo.Rntrc.getRequiredFieldDescription());
        _veiculo.Proprietario.required = true;
        _veiculo.Tipo.visibleFade(true);
        _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.Marca.getFieldDescription());
        _veiculo.MarcaVeiculo.required = false;
        _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.Modelo.getFieldDescription());
        _veiculo.ModeloVeiculo.required = false;

        _veiculo.TagPlaca.visible(true);
        _veiculo.TagRanavam.visible(true);
        _veiculo.TagNomeProprietario.visible(true);
        _veiculo.TagCPFCNPJProprietario.visible(true);
        _veiculo.TagRNTRC.visible(true);
        _veiculo.TagUFVeiculo.visible(true);
        _veiculo.TagMarcaVeiculo.visible(true);
        _veiculo.TagPlacasVinculadas.visible(true);

        _veiculo.Proprietario.visible(true);
        _veiculo.TipoProprietario.visible(true);
        _veiculo.ValorValePedagio.visible(true);
        _veiculo.NumeroCompraValePedagio.visible(true);
        _veiculo.FornecedorValePedagio.visible(true);
        _veiculo.ResponsavelValePedagio.visible(true);
        _veiculo.DataVigencia.visible(true);

        _veiculo.EmpresaFilial.visible(false);

    } else {
        _veiculo.TipoProprietario.required = false;
        _veiculo.RNTRC.required = false;
        _veiculo.RNTRC.text(Localization.Resources.Veiculos.Veiculo.Rntrc.getFieldDescription());
        _veiculo.Proprietario.required = false;
        _veiculo.Tipo.visibleFade(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS);
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.Marca.getRequiredFieldDescription());
            _veiculo.MarcaVeiculo.required = true;
            _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.Modelo.getRequiredFieldDescription());
            _veiculo.ModeloVeiculo.required = true;

            _veiculo.Proprietario.visible(false);
            _veiculo.TipoProprietario.visible(false);
            _veiculo.ValorValePedagio.visible(false);
            _veiculo.NumeroCompraValePedagio.visible(false);
            _veiculo.FornecedorValePedagio.visible(false);
            _veiculo.ResponsavelValePedagio.visible(false);
        }
        _veiculo.TagPlaca.visible(true);
        _veiculo.TagRanavam.visible(true);
        _veiculo.TagNomeProprietario.visible(false);
        _veiculo.TagCPFCNPJProprietario.visible(false);
        _veiculo.TagRNTRC.visible(false);
        _veiculo.TagUFVeiculo.visible(true);
        _veiculo.TagMarcaVeiculo.visible(true);
        _veiculo.TagPlacasVinculadas.visible(true);
        _veiculo.DataVigencia.visible(false);

        _veiculo.EmpresaFilial.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS);

    }
}

//*******MÉTODOS*******

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function validarRegrasVeiculo() {

    if (_veiculo.TipoVeiculo.val() == "0" && _veiculo.TipoRodado.val() == "00") {
        resetarTabs();
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarTipoRodado);
        return false;
    }

    if ((_veiculo.CPFMotorista.val() != "") && (_veiculo.NomeMotorista.val() == "")) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Veiculos.Veiculo.NomeDoMotoristaObrigatorioQuandoCPFDoMotoristaForInformado);
        _veiculo.NomeMotorista.requiredClass("form-control is-invalid");
        return false;
    }
    _veiculo.NomeMotorista.requiredClass("form-control");

    if ((_veiculo.NomeMotorista.val() != "") && (_veiculo.CPF.val() == "")) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Veiculos.Veiculo.CPFDoMotoristaObrigadoQuandoNomeDoMotoristaInformado);
        _veiculo.CPFMotorista.requiredClass("form-control is-invalid");
        return false;
    }
    _veiculo.CPFMotorista.requiredClass("form-control");

    if (_veiculo.TipoVeiculo.val() == "0" && _veiculo.TipoRodado.val() == "00") {
        resetarTabs();
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarTipoRodado);
        return false;
    }

    if (_CONFIGURACAO_TMS.ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo && _veiculo.TipoRodado.val() === EnumTipoRodadoVeiculo.Cavalo && _veiculo.VeiculosVinculados.list.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarVeiculoVinculadoQuandoTipoRodadoForCavalo);
        return false;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _veiculo.RNTRC.val().length < 8 && _veiculo.Tipo == EnumTipoPropriedadeVeiculo.Terceiros) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarRNTRCCorretamente)
        _veiculo.RNTRC.requiredClass("form-control is-invalid");
        return false;
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        let permiteBloquearVeiculo = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo, _PermissoesPersonalizadas);
        if (permiteBloquearVeiculo && _veiculo.BloquearVeiculo.val() && string.IsNullOrWhiteSpace(_veiculo.MotivoBloqueio.val())) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarMotivoBloqueio)
            _veiculo.MotivoBloqueio.requiredClass("form-control is-invalid");
            return false;
        }
    }

    if (_CONFIGURACAO_TMS.ObrigatorioCadastrarRastreadorNosVeiculos && _veiculo.PossuiRastreador.val() == true) {
        if (_veiculo.NumeroEquipamentoRastreador.val() == "" || _veiculo.NumeroEquipamentoRastreador.val() == null) {
            _veiculo.NumeroEquipamentoRastreador.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarNumeroEquipamentoRastreador);
            return false;
        }
    }

    if (_veiculo.AnoModelo.val() == "" && _veiculo.AnoModelo.required() === true) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarAnoModelo);
        return false;
    }

    if (_veiculo.AnoFabricacao.val() == "" && _veiculo.AnoFabricacao.required() === true) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Veiculos.Veiculo.NecessarioInformarAnoFabricacao);
        return false;
    }

    if (!ValidarCamposObrigatorios(_veiculo)) {
        exibirCamposObrigatorio();
        return false;
    }

    return true;
}

function informarGrupoServicoClick(e) {
    let valido = ValidarCamposObrigatorios(e);
    if (!valido) {
        exibirCamposObrigatorio();
        return;
    }

    _veiculo.GrupoServico.codEntity(e.GrupoServico.codEntity());
    _veiculo.GrupoServico.val(e.GrupoServico.val());
    Global.fecharModal('divModalInformarGrupoServico');

    adicionarVeiculo();
}

function buscarVeiculos() {
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarVeiculo, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridVeiculo = new GridView(_pesquisaVeiculo.Pesquisar.idGrid, "Veiculo/Pesquisa", _pesquisaVeiculo, menuOpcoes, null);
    _gridVeiculo.CarregarGrid();
}

function editarVeiculo(veiculoGrid) {
    limparCamposVeiculo();
    _veiculo.Codigo.val(veiculoGrid.Codigo);
    BuscarPorCodigo(_veiculo, "Veiculo/BuscarPorCodigo", function (arg) {
        _pesquisaVeiculo.ExibirFiltros.visibleFade(false);
        _veiculo.Placa.enable(false);
        _veiculo.Atualizar.visible(true);
        _veiculo.Cancelar.visible(true);
        _veiculo.Excluir.visible(true);
        _veiculo.Adicionar.visible(false);

        ObterCampoPixVeiculo(_veiculo, arg.Data.TipoPagamento, arg.Data.TipoChavePIX, arg.Data.ChavePIXCIOT);

        if (_CONFIGURACAO_TMS.NaoPermitirDesabilitarCompraValePedagioVeiculo && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
            _veiculo.NaoComprarValePedagio.enable(_veiculo.NaoComprarValePedagio.val());
            _veiculo.NaoComprarValePedagioRetorno.enable(_veiculo.NaoComprarValePedagioRetorno.val() || _veiculo.NaoComprarValePedagio.val());
        }

        if (!_CONFIGURACAO_TMS.PermitirTransportadorAlterarModeloVeicular && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
            if (_veiculo.ModeloVeicularCarga.codEntity() > 0) {
                _veiculo.ModeloVeicularCarga.enable(false);
            } else {
                _veiculo.ModeloVeicularCarga.enable(true);
            }
        }

        controlarObrigatoriedadeDataValidadeAdicionalCarroceria(arg.Data.ObrigatorioInformarDataValidadeAdicionalCarroceria);

        _veiculo.PossuiLocalizador.visible(arg.Data.PermiteAdicionarLocalizador);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteAlterarPlaca, _PermissoesPersonalizadas) && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
            _veiculo.AlterarPlaca.enable(true);
        else
            _veiculo.AlterarPlaca.enable(false);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagio, _PermissoesPersonalizadas))
            _veiculo.NaoComprarValePedagio.enable(true);
        else
            _veiculo.NaoComprarValePedagio.enable(false);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagioRetorno, _PermissoesPersonalizadas))
            _veiculo.NaoComprarValePedagioRetorno.enable(true);
        else
            _veiculo.NaoComprarValePedagioRetorno.enable(false);

        _veiculo.AlterarPlaca.val(false);
        _veiculo.Licencas.val(arg.Data.Licencas);
        _veiculo.PossuiTagValePedagio.visible(arg.Data.CompraValePedagio);
        _veiculo.ModoCompraValePedagioTarget.val(arg.Data.ModoCompraValePedagioTarget);

        mudouPropriedadeOnChange();
        recarregarGridVeiculosViculados();
        recarregarGridTransportadoras();
        recarregarGridEquipamentos();
        recarregarGridMotoristas();
        recarregarGridLicencaCadastroVeiculo();
        recarregarVeiculoIntegracoes();

        verificarTipoVeiculo(true);
        EditarListarAnexos(arg);
        preencherCurrais(arg.Data.Currais);
        preencherDadosAprovacao(arg.Data);
        preencherDadosRastreamento(arg.Data.DadosRastreamento);
        RecarregarGridRotasFrete();
        carregarGridVeiculoLiberacoesGR(arg.Data.LiberacoesGR);

        resetarTabs();

        ConfigurarCamposPorTipoServico();

        if (arg.Data.Empresa != null)
            bloquearCampoRenavamPorTipoEmpresa(arg.Data.Empresa.Tipo);

        _notificacaoGlobal.CodigoObjeto.val(0);

    }, null);
}

function limparCamposVeiculo() {
    _veiculo.AlterarPlaca.enable(false);
    _veiculo.AlterarPlaca.val(true);

    _veiculo.Atualizar.visible(false);
    _veiculo.Cancelar.visible(false);
    _veiculo.Excluir.visible(false);
    _veiculo.Adicionar.visible(true);
    _veiculo.TipoProprietario.required = false;
    _veiculo.RNTRC.required = false;
    _veiculo.Proprietario.required = false;
    _veiculo.Tipo.visibleFade(false);
    _veiculo.ModeloVeicularCarga.enable(true);
    _veiculo.Placa.enable(true);
    _veiculo.Licencas.val(_veiculo.Licencas.def);
    _veiculo.PossuiTagValePedagio.visible(false);
    _veiculo.NaoComprarValePedagio.enable(true);
    _veiculo.NaoComprarValePedagioRetorno.enable(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _veiculo.MarcaVeiculo.text(Localization.Resources.Veiculos.Veiculo.Marca.getRequiredFieldDescription());
        _veiculo.MarcaVeiculo.required = true;
        _veiculo.ModeloVeiculo.text(Localization.Resources.Veiculos.Veiculo.Modelo.getRequiredFieldDescription());
        _veiculo.ModeloVeiculo.required = true;
    }

    if (IsAprovacaoCadastroAtiva()) {
        DesbloquearCamposCadastroVeiculo();
        $("#" + _etapaCadastroVeiculo.Etapa1.idTab).click();
    }

    LimparCampos(_veiculo);
    LimparCampos(_informarGrupoServico);
    LimparCamposVeiculosVinculados();
    recarregarGridVeiculosViculados();
    recarregarGridTransportadoras();
    recarregarGridEquipamentos();
    recarregarGridMotoristas();
    recarregarGridLicencaCadastroVeiculo();
    recarregarVeiculoIntegracoes();
    limparAnexosTela();
    LimparCamposRotasFrete();
    setarEtapasCadastroVeiculo();
    limparCamposVeiculoLiberacoesGR();
    recarregarGridVeiculoLiberacoesGR();

    $("#liTabAnexos").hide();
    $("#liTabVeiculoVeiculado").show();

    resetarTabs();
    ConfigurarCamposPorTipoServico();

    _veiculo.PossuiTagValePedagio.val(_CONFIGURACAO_TMS.PadraoTagValePedagioVeiculos);
}

function preencherDadosRastreamento(data) {
    if (data == null)
        return;

    _veiculo.Terminal.val(data.Terminal);
    _veiculo.DataUltimaPosicao.val(data.DataUltimaPosicao);
    _veiculo.Rastreador.val(data.Rastreador);
}

function resetarTabs() {
    $("#liTabVeiculoVeiculado").removeAttr("class");
    $("#liTabVeiculo").attr("class", "active");
    $("#tabVeiculo").attr("class", "tab-pane fade active in padding-10 no-padding-bottom");
    $("#tabVinculadosVinculados").attr("class", "tab-pane fade");
    $("#liTabVeiculoVeiculado").show();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function carregarFiltrosPesquisaInicial() {
    const placa = sessionStorage.getItem('placaVeiculo');
    if (placa) {
        _pesquisaVeiculo.Placa.val(placa || '');
        _pesquisaVeiculo.ExibirFiltros.visibleFade(true);
        sessionStorage.removeItem('placaVeiculo');
    }
}

function ConfigurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {

                _integracoesDisponiveis = r.Data.TiposExistentes;

                if (_veiculo.PossuiTagValePedagio.val() && r.Data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.Target || o === EnumTipoIntegracao.EFrete || o === EnumTipoIntegracao.Ambipar || o === EnumTipoIntegracao.Repom; }))
                    $("#liTarget").removeClass("d-none");

                if (r.Data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.AngelLira; }))
                    _veiculo.AtivarConsultarAbastecimentoAngelLira.visible(true);
            }
        }
    });
}

function obterVeiculoSalvar() {

    preencherListasSelecao();

    let veiculoEnviar = RetornarObjetoPesquisa(_veiculo);

    let chavePIX = "";
    if (_veiculo.TipoPagamento.val() === EnumTipoPagamentoMDFe.PIX) {
        switch (_veiculo.TipoChavePIX.val()) {
            case EnumTipoChavePix.CPFCNPJ:
                chavePIX = _veiculo.ChavePIXCPFCNPJ.val();
                break;
            case EnumTipoChavePix.Email:
                chavePIX = _veiculo.ChavePIXEmail.val();
                break;
            case EnumTipoChavePix.Celular:
                chavePIX = _veiculo.ChavePIXCelular.val();
                break;
            case EnumTipoChavePix.Aleatoria:
                chavePIX = _veiculo.ChavePIXAleatoria.val();
                break;
        }
    }

    veiculoEnviar["ChavePIXCIOT"] = chavePIX;
    veiculoEnviar["ListaCurrais"] = obterListaCurrais();
    veiculoEnviar["Licencas"] = obterListaLicencasSalvar();
    veiculoEnviar["LiberacoesGR"] = obterListaLiberacoesGR();

    return veiculoEnviar;
}

function ObterConfiguracaoPadrao() {
    let p = new promise.Promise()

    executarReST("Veiculo/ObterConfiguracaoPadrao", {}, function (r) {
        if (r.Success && r.Data) {
            _configuracaoPadrao = r.Data;
            p.done();
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });

    return p;
}

function bloquearCampoRenavamPorTipoEmpresa(tipoEmpresa) {
    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
        if (tipoEmpresa == "E") {
            _veiculo.Renavam.required = false;
            _veiculo.Renavam.enable(false);
            _veiculo.Renavam.text(Localization.Resources.Veiculos.Veiculo.Renavam.getFieldDescription());
        } else {
            _veiculo.Renavam.required = true;
            _veiculo.Renavam.enable(true);
            _veiculo.Renavam.text(Localization.Resources.Veiculos.Veiculo.Renavam.getRequiredFieldDescription());
        }
    }
}

function preencherListasSelecao() {
    _veiculo.RotasFrete.val(JSON.stringify(_gridRotasFrete.BuscarRegistros()));
}

function RetornoBuscaCarroceria(carroceria) {
    _veiculo.ModeloCarroceria.codEntity(carroceria.Codigo);
    _veiculo.ModeloCarroceria.val(carroceria.Descricao);
    controlarObrigatoriedadeDataValidadeAdicionalCarroceria(carroceria.ObrigatorioInformarDataValidadeAdicionalCarroceria);
}

function controlarObrigatoriedadeDataValidadeAdicionalCarroceria(obrigatorioInformarData) {
    if (obrigatorioInformarData) {
        _veiculo.DataValidadeAdicionalCarroceria.required(true);
        _veiculo.DataValidadeAdicionalCarroceria.text(Localization.Resources.Veiculos.Veiculo.DataDeValidadeDoAdicionalDeCarroceria.getRequiredFieldDescription());
    } else {
        _veiculo.DataValidadeAdicionalCarroceria.required(false);
        _veiculo.DataValidadeAdicionalCarroceria.text(Localization.Resources.Veiculos.Veiculo.DataDeValidadeDoAdicionalDeCarroceria.getFieldDescription());
    }
}

function ObterTiposIntegracaoVeiculo() {
    let p = new promise.Promise();

    executarReST("Veiculo/BuscarValePedagioAtivo", {}, function (r) {
        if (r.Success) {
            _configuracaoEmissaoCTeOpcoesTipoIntegracaoVeiculo = new Array();
            _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada = new Array();

            for (var i = 0; i < r.Data.length; i++) {
                _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

                _configuracaoEmissaoCTeOpcoesTipoIntegracaoVeiculo.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function buscarSeExisteRegraAlcadaCadastroVeiculo() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe && _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoVeiculo && _CONFIGURACAO_TMS.ExisteRegraAlcadaComFilial) {
        _veiculo.FilialCarregamento.visible(true);
        _veiculo.FilialCarregamento.required(true);
    } else {
        _veiculo.FilialCarregamento.visible(false);
        _veiculo.FilialCarregamento.required(false);
    }
}
