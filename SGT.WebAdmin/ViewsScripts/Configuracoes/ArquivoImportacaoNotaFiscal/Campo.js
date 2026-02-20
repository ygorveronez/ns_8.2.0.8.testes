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
/// <reference path="../../../js/plugin/moment/moment.min.js" />
/// <reference path="../../Enumeradores/EnumTipoCampo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCampo;
var _campo;

var _tipoPropriedade = [
    { text: "Alfanumérico", value: EnumTipoCampo.Alfanumerico },
    { text: "Inteiro", value: EnumTipoCampo.Inteiro },
    { text: "Decimal", value: EnumTipoCampo.Decimal },
    { text: "Data", value: EnumTipoCampo.Data },
    { text: "Hora", value: EnumTipoCampo.Hora },
    { text: "Data e Hora", value: EnumTipoCampo.DataHora }
];

var _propriedade = [
    { value: 'Protocolo', text: 'Protocolo' },
    { value: 'Chave', text: 'Chave' },
    { value: 'Rota', text: 'Rota' },
    { value: 'Numero', text: 'Número' },
    { value: 'Serie', text: 'Série' },
    { value: 'Modelo', text: 'Modelo' },
    { value: 'Valor', text: 'Valor' },
    { value: 'BaseCalculoICMS', text: 'Base de Cálculo do ICMS' },
    { value: 'ValorICMS', text: 'Valor do ICMS' },
    { value: 'BaseCalculoST', text: 'Base de Cálculo do ICMS ST' },
    { value: 'ValorST', text: 'Valor do ICMS ST' },
    { value: 'ValorImpostoImportacao', text: 'Valor do Imposto de Importação' },
    { value: 'ValorPIS', text: 'Valor do PIS' },
    { value: 'ValorCOFINS', text: 'Valor da COFINS' },
    { value: 'ValorIPI', text: 'Valor do IPI' },
    { value: 'ValorOutros', text: 'Valor de Outros Impostos' },
    { value: 'ValorFrete', text: 'Valor do Frete' },
    { value: 'ValorTotalProdutos', text: 'Valor dos Produtos' },
    { value: 'ValorSeguro', text: 'Valor do Seguro' },
    { value: 'ValorDesconto', text: 'Valor de Desconto' },
    { value: 'PesoBruto', text: 'Peso Bruto' },
    { value: 'PesoLiquido', text: 'Peso Líquido' },
    { value: 'VolumesTotal', text: 'Total de Volumes' },
    { value: 'DataEmissao', text: 'Data de Emissão' },
    { value: 'NaturezaOP', text: 'Natureza da Operação' },
    { value: 'InformacoesComplementares', text: 'Informações Complementares' },
    { value: 'Emitente.CPFCNPJ', text: 'Emitente - CPF/CNPJ' },
    { value: 'Emitente.CodigoAtividade', text: 'Emitente - Código de Atividade' },
    { value: 'Emitente.RGIE', text: 'Emitente - RG/IE' },
    { value: 'Emitente.IM', text: 'Emitente - Inscrição Municipal' },
    { value: 'Emitente.RazaoSocial', text: 'Emitente - Razão Social' },
    { value: 'Emitente.NomeFantasia', text: 'Emitente - Nome Fantasia' },
    { value: 'Emitente.Email', text: 'Emitente - e-mail' },
    { value: 'Emitente.EmailContador', text: 'Emitente - e-mail do Contador' },
    { value: 'Emitente.EmailContato', text: 'Emitente - e-mail do Contato' },
    { value: 'Emitente.Endereco.Logradouro', text: 'Emitente - Endereço - Logradouro' },
    { value: 'Emitente.Endereco.Numero', text: 'Emitente - Endereço - Número' },
    { value: 'Emitente.Endereco.Complemento', text: 'Emitente - Endereço - Complemento' },
    { value: 'Emitente.Endereco.CEP', text: 'Emitente - Endereço - CEP' },
    { value: 'Emitente.Endereco.Bairro', text: 'Emitente - Endereço - Bairro' },
    { value: 'Emitente.Endereco.Telefone', text: 'Emitente - Endereço - Telefone' },
    { value: 'Emitente.Endereco.Cidade.Descricao', text: 'Emitente - Endereço - Nome da Cidade' },
    { value: 'Emitente.Endereco.Cidade.SiglaUF', text: 'Emitente - Endereço - Sigla da UF da Cidade' },
    { value: 'Emitente.Endereco.Cidade.IBGE', text: 'Emitente - Endereço - IBGE da Cidade' },
    { value: 'Destinatario.CPFCNPJ', text: 'Destinatário - CPF/CNPJ' },
    { value: 'Destinatario.CodigoAtividade', text: 'Destinatário - Código de Atividade' },
    { value: 'Destinatario.RGIE', text: 'Destinatário - RG/IE' },
    { value: 'Destinatario.IM', text: 'Destinatário - Inscrição Municipal' },
    { value: 'Destinatario.RazaoSocial', text: 'Destinatário - Razão Social' },
    { value: 'Destinatario.NomeFantasia', text: 'Destinatário - Nome Fantasia' },
    { value: 'Destinatario.Email', text: 'Destinatário - e-mail' },
    { value: 'Destinatario.EmailContador', text: 'Destinatário - e-mail do Contador' },
    { value: 'Destinatario.EmailContato', text: 'Destinatário - e-mail do Contato' },
    { value: 'Destinatario.Endereco.Logradouro', text: 'Destinatário - Endereço - Logradouro' },
    { value: 'Destinatario.Endereco.Numero', text: 'Destinatário - Endereço - Número' },
    { value: 'Destinatario.Endereco.Complemento', text: 'Destinatário - Endereço - Complemento' },
    { value: 'Destinatario.Endereco.CEP', text: 'Destinatário - Endereço - CEP' },
    { value: 'Destinatario.Endereco.Bairro', text: 'Destinatário - Endereço - Bairro' },
    { value: 'Destinatario.Endereco.Telefone', text: 'Destinatário - Endereço - Telefone' },
    { value: 'Destinatario.Endereco.Cidade.Descricao', text: 'Destinatário - Endereço - Nome da Cidade' },
    { value: 'Destinatario.Endereco.Cidade.SiglaUF', text: 'Destinatário - Endereço - Sigla da UF da Cidade' },
    { value: 'Destinatario.Endereco.Cidade.IBGE', text: 'Destinatário - Endereço - IBGE da Cidade' },
    { value: 'Transportador.CPFCNPJ', text: 'Transportador - CPF/CNPJ' },
    { value: 'Transportador.CodigoAtividade', text: 'Transportador - Código de Atividade' },
    { value: 'Transportador.RGIE', text: 'Transportador - RG/IE' },
    { value: 'Transportador.IM', text: 'Transportador - Inscrição Municipal' },
    { value: 'Transportador.RazaoSocial', text: 'Transportador - Razão Social' },
    { value: 'Transportador.NomeFantasia', text: 'Transportador - Nome Fantasia' },
    { value: 'Transportador.Email', text: 'Transportador - e-mail' },
    { value: 'Transportador.EmailContador', text: 'Transportador - e-mail do Contador' },
    { value: 'Transportador.EmailContato', text: 'Transportador - e-mail do Contato' },
    { value: 'Transportador.Endereco.Logradouro', text: 'Transportador - Endereço - Logradouro' },
    { value: 'Transportador.Endereco.Numero', text: 'Transportador - Endereço - Número' },
    { value: 'Transportador.Endereco.Complemento', text: 'Transportador - Endereço - Complemento' },
    { value: 'Transportador.Endereco.CEP', text: 'Transportador - Endereço - CEP' },
    { value: 'Transportador.Endereco.Bairro', text: 'Transportador - Endereço - Bairro' },
    { value: 'Transportador.Endereco.Telefone', text: 'Transportador - Endereço - Telefone' },
    { value: 'Transportador.Endereco.Cidade.Descricao', text: 'Transportador - Endereço - Nome da Cidade' },
    { value: 'Transportador.Endereco.Cidade.SiglaUF', text: 'Transportador - Endereço - Sigla da UF da Cidade' },
    { value: 'Transportador.Endereco.Cidade.IBGE', text: 'Transportador - Endereço - IBGE da Cidade' },
    { value: 'Veiculo.Placa', text: 'Veículo - Placa' },
    { value: 'Veiculo.Renavam', text: 'Veículo - RENAVAM' },
    { value: 'Veiculo.UF', text: 'Veículo - Sigla da UF' },
    { value: 'Veiculo.RNTC', text: 'Veículo - RNTRC' }
];

var Campo = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Propriedade = PropertyEntity({ val: ko.observable("Protocolo"), options: _propriedade, def: "Protocolo", text: "*Propriedade: " });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable(EnumTipoCampo.Alfanumerico.toString()), options: _tipoPropriedade, def: EnumTipoCampo.Alfanumerico.toString(), text: "*Tipo da Propriedade: " });
    this.Posicao = PropertyEntity({ text: "*Posição: ", required: true, getType: typesKnockout.int, maxlength: 3 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCampoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadCampo() {

    _campo = new Campo();
    KoBindings(_campo, "knockoutCampo");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirCampoClick }] };

    var header = [{ data: "Codigo", visible: false },
                  { data: "Posicao", title: "Posição", width: "10%" },
                  { data: "TipoPropriedade", title: "Tipo da Propriedade", width: "20%" },
                  { data: "Propriedade", title: "Propriedade", width: "60%" }];

    _gridCampo = new BasicDataTable(_campo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    recarregarGridCampo();
}

function recarregarGridCampo() {

    var data = new Array();

    $.each(_arquivoImportacaoNotaFiscal.ListaColunas.list, function (i, campo) {
        var campoGrid = new Object();

        campoGrid.Codigo = campo.Codigo.val;
        campoGrid.Posicao = campo.Posicao.val;
        campoGrid.TipoPropriedade = ObterDescricaoTipoPropriedade(campo.TipoPropriedade.val);
        campoGrid.Propriedade = ObterDescricaoPropriedade(campo.Propriedade.val);

        data.push(campoGrid);
    });

    _gridCampo.CarregarGrid(data);
}

function ObterDescricaoPropriedade(prop) {
    for (var i = 0; i < _propriedade.length; i++) 
        if (_propriedade[i].value == prop)
            return _propriedade[i].text;
}

function ObterDescricaoTipoPropriedade(prop) {
    for (var i = 0; i < _tipoPropriedade.length; i++)
        if (_tipoPropriedade[i].value == prop)
            return _tipoPropriedade[i].text;
}

function excluirCampoClick(data) {
    for (var i = 0; i < _arquivoImportacaoNotaFiscal.ListaColunas.list.length; i++) {
        if (data.Codigo == _arquivoImportacaoNotaFiscal.ListaColunas.list[i].Codigo.val) {
            _arquivoImportacaoNotaFiscal.ListaColunas.list.splice(i, 1);
            break;
        }
    }

    recarregarGridCampo();
}

function adicionarCampoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_campo);

    if (valido) {
        //for (var i = 0; i < _arquivoImportacaoNotaFiscal.ListaColunas.list.length; i++) {
        //    if (_arquivoImportacaoNotaFiscal.ListaColunas.list[i].Posicao.val == _campo.Posicao.val()) {
        //        exibirMensagem(tipoMensagem.aviso, "Posição já existente", "A posição informada já existe.");
        //        return;
        //    }

        //    if (_arquivoImportacaoNotaFiscal.ListaColunas.list[i].Propriedade.val == _campo.Propriedade.val()) {
        //        exibirMensagem(tipoMensagem.aviso, "Propriedade já existente", "A propriedade informada já existe.");
        //        return;
        //    }
        //}

        _arquivoImportacaoNotaFiscal.ListaColunas.list.push(SalvarListEntity(_campo));

        recarregarGridCampo();

        limparCamposCampo();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function limparCamposCampo() {
    LimparCampos(_campo);
}