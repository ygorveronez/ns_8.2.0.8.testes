function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

//Consultas de Localidades
function CarregarConsultaDeLocalidades(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: guid() }, { DescricaoBusca: 'IBGE', Descricao: 'IBGE', Id: guid() }], "/Localidade/Consulta?callback=?", "Consulta de Cidades", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consuta de Atividades
function CarregarConsultaDeAtividades(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtNomeOuCodigoAtividade' + guid()) }], "/Atividade/Consulta?callback=?", "Consulta de Atividades", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [], fecharConsulta, usarKeyDown);
}

function CarregarConsultaIBSCBSCstClassTrib(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CST', Descricao: 'CST', Id: ('txtIBSCBS_CST' + guid()) }], "/OutrasAliquotas/BuscarCstClassificacao?callback=?", "Consulta de CST e Classificação Tributária", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [3], fecharConsulta, usarKeyDown);
}

//Consulta de Notas Fiscais Eletronicas
function CarregarConsultaDeNotasFiscaisEletronicas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NumeroNota', Descricao: 'Número', Id: ('txtNumeroXMLNotaFiscalEletronica' + guid()), Tipo: 'number' },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: ('txtDataEmissaoXMLNotaFiscalEletronica' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'CPF_CNPJ_Emitente', Descricao: 'CPF/CNPJ Emitente', Id: ('txtCPF_CNPJEmitenteXMLNotaFiscalEletronica' + guid()), Tipo: 'cpf_cnpj' }], "/XMLNotaFiscalEletronica/Consultar?callback=?", "Consulta de Notas Fiscais Eletrônicas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3, 4], fecharConsulta, usarKeyDown);
}

//Consulta de Observações
function CarregarConsultaDeObservacoesPorTipo(idTxtBuscar, idBtnBuscar, tipoObservacao, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoObservacao' + guid()) }], "/Observacao/ConsultarTodos?callback=?&Tipo=" + tipoObservacao, "Consulta de Observações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Observações
function CarregarConsultaDeObservacoes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoObservacao' + guid()) }], "/Observacao/Consultar?callback=?", "Consulta de Observações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 7, 8, 9, 11], fecharConsulta, usarKeyDown);
}

//Consulta de Naturezas das Operações
function CarregarConsultaDeNaturezasDasOperacoes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoNaturezaDaOperacao' + guid()) }], "/NaturezaDaOperacao/Consultar?callback=?", "Consulta de Naturezas das Operações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [], fecharConsulta, usarKeyDown);
}

//Consulta de CFOPs
function CarregarConsultaDeCFOPs(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    if ($.isFunction(tipo)) {
        usarKeyDown = fecharConsulta;
        fecharConsulta = callbackSelecionar;
        callbackSelecionar = tipo;
        tipo = "";
    }
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CFOP', Descricao: 'CFOP', Id: ('txtCodigoCFOP' + guid()) }, { DescricaoBusca: 'NaturezaDaOperacao', Descricao: 'Natureza da Operação', Id: ('txtNaturezaDaOperacao' + guid()) }], "/CFOP/Consultar?callback=?&Tipo=" + tipo, "Consulta de CFOPs", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Usuários
//function CarregarConsultaDeUsuarios(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
//    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtNomeUsuario' + guid()) }], "/Usuario/Consultar?callback=?", "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
//}

//Consulta de Motoristas
function CarregarConsultaDeMotoristas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    // FIX #5361
    // Por padrao, busca os motoristas ativos
    // A = ATIVOS (padrao)
    // I = INATIVOS
    // "" = TODOS
    if (typeof status == "function") {
        // Quando nao for informado o status
        usarKeyDown = fecharConsulta;
        fecharConsulta = callbackSelecionar;
        callbackSelecionar = status;
        status = "A";
    }
    // END FIX #5361
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtNomeUsuario' + guid()) }, { DescricaoBusca: 'CPF', Descricao: 'CPF', Id: ('txtCPFUsuario' + guid()) }], "/Motorista/Consultar?callback=?&Status=" + status, "Consulta de Motoristas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de Empresas
function CarregarConsultaDeEmpresasComConfiguracao(idTxtBuscar, idBtnBuscar, callbackSelecionar, callbackConfigurar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social/Nome Fantasia', Id: ('txtRazaoSocialNomeFantasia' + guid()) }], "/Empresa/Consultar?callback=?", "Consulta de Empresas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }, { Descricao: "Configurar", Evento: callbackConfigurar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Empresas
function CarregarConsultaDeEmpresas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social/Nome Fantasia', Id: ('txtRazaoSocialNomeFantasia' + guid()) }], "/Empresa/Consultar?callback=?", "Consulta de Empresas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Veículos
function CarregarConsultaDeVeiculos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown, tipoVeiculo) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Placa', Descricao: 'Placa', Id: ('txtPlacaVeiculo' + guid()) },
    { DescricaoBusca: 'Renavam', Descricao: 'Renavam', Id: ('txtRenavamVeiculo' + guid()) },
    { DescricaoBusca: 'Status', Descricao: 'Status', Id: ('txtStatusVeiculo' + guid()) }], "/Veiculo/Consultar?callback=?&TipoVeiculo=" + (tipoVeiculo != null ? tipoVeiculo : ""), "Consulta de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Clientes
function CarregarConsultadeClientes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown, tipo) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CPFCNPJ', Descricao: 'CPF / CNPJ', Id: ('txtCPFCNPJCliente' + guid()), Tipo: 'cpf_cnpj' },
    { DescricaoBusca: 'Nome', Descricao: 'Razão Social', Id: ('txtNomeRazaoCliente' + guid()) }], "/Cliente/Consultar?callback=?" + (tipo != null ? ("&Tipo=" + tipo) : ""), "Consulta de Clientes", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de CTes
function CarregarConsultaDeCTes(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown, dataEmissao) {
    if (dataEmissao == null)
        dataEmissao = Globalize.format(new Date(), "dd/MM/yyyy");
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: (guid()) },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: (guid()), Tipo: 'date', Default: dataEmissao },
    { DescricaoBusca: 'NumeroDocumento', Descricao: 'Núm. NF', Id: (guid()), Tipo: 'number' }], "/ConhecimentoDeTransporteEletronico/ConsultarPorTipo?callback=?&TipoCTe=" + tipo, "Consulta de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de CTes Sefaz
function CarregarConsultaDeCTesPortal(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown, dataEmissao) {
    if (dataEmissao == null)
        dataEmissao = Globalize.format(new Date(), "dd/MM/yyyy");
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: (guid()) },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: (guid()), Tipo: 'date', Default: dataEmissao },
    { DescricaoBusca: 'CNPJEmitente', Descricao: 'Emitente', Id: (guid()), Tipo: 'cpf_cnpj' }], "/ConhecimentoDeTransporteEletronico/ConsultarImportadosPortal?callback=?", "Consulta de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Cargas
function CarregarConsultaDeTiposDeCargas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeCarga/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Cargas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Despesas
function CarregarConsultaDeTiposDeDespesas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeDespesa/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Despesas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de Acertos de Viagem
function CarregarConsultaDeAcertosDeViagens(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()) },
        { DescricaoBusca: 'Veiculo', Descricao: 'Veículo', Id: ('txtVeiculo' + guid()) },
        { DescricaoBusca: 'Motorista', Descricao: 'Motorista', Id: ('txtMotorista' + guid()) }
    ], "/AcertoDeViagem/Consultar?callback=?&Status=" + status, "Consulta de Acertos de Viagens", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Abastecimentos
function CarregarConsultaDeAbastecimentos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Posto', Descricao: 'Posto', Id: ('txtPosto' + guid()) }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Veículo', Id: ('txtVeiculo' + guid()) }, { DescricaoBusca: 'Data', Descricao: 'Data', Id: ('txtData' + guid()), Tipo: 'date' }], "/Abastecimento/Consultar?callback=?", "Consulta de Abastecimentos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Veículos
function CarregarConsultaDeTiposDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de Modelos de Veículos
function CarregarConsultaDeModelosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/ModeloDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Modelos de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Ordem de Compra
function CarregarConsultaOrdemDeCompra(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'Data', Descricao: 'Data', Id: ('txtDescricao' + guid()), Tipo: 'date' }, { DescricaoBusca: 'Solicitante', Descricao: 'Solicitante', Id: ('txtSolicitante' + guid()) }], "/OrdemCompra/Consultar?callback=?", "Consulta de Ordem de Compra", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Ordem de Compra
function CarregarConsultaOrdemDeCompraMateriais(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'Data', Descricao: 'Data', Id: ('txtDescricao' + guid()), Tipo: 'date' }, { DescricaoBusca: 'Solicitante', Descricao: 'Solicitante', Id: ('txtSolicitante' + guid()) }], "/OrdemCompra/ConsultarPorMateriais?callback=?", "Consulta de Ordem de Compra", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Marcas de Veículos
function CarregarConsultaDeMarcasDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/MarcaDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Marcas de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Serviços de Veículos
function CarregarConsultaDeServicosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/ServicoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Serviços de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Históricos de Veículos
function CarregarConsultaDeHistoricosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Placa', Descricao: 'Placa do Veículo', Id: ('txtPlaca' + guid()) }, { DescricaoBusca: 'Servico', Descricao: 'Descrição do Serviço', Id: ('txtPlaca' + guid()) }], "/HistoricoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Históricos de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Eixos de Veículos
function CarregarConsultaDeEixosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/EixoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Eixos de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3], fecharConsulta, usarKeyDown);
}

//Consulta de Eixos de Veículos por Histórico
function CarregarConsultaDeEixosDeVeiculosPorHistorico(idTxtBuscar, idBtnBuscar, tipoHistorico, codigoVeiculo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/EixoDeVeiculo/ConsultarPorVeiculo?callback=?&CodigoVeiculo=" + codigoVeiculo + "&Tipo=" + tipoHistorico, "Consulta de Eixos de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3], fecharConsulta, usarKeyDown);
}

//Consulta de Status de Pneus
function CarregarConsultaDeStatusDePneus(idTxtBuscar, idBtnBuscar, status, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/StatusDePneu/Consultar?callback=?&Status=" + status + "&Tipo=" + tipo, "Consulta de Status de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de Dimensões de Pneus
function CarregarConsultaDeDimensoesDePneus(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/DimensaoDePneu/Consultar?callback=?&Status=" + status, "Consulta de Dimensões de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Marcas de Pneus
function CarregarConsultaDeMarcasDePneus(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/MarcaDePneu/Consultar?callback=?&Status=" + status, "Consulta de Marcas de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Modelos de Pneus
function CarregarConsultaDeModelosDePneus(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/ModeloDePneu/Consultar?callback=?&Status=" + status, "Consulta de Modelos de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Pneus
function CarregarConsultaDePneus(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Serie', Descricao: 'Série', Id: ('txtSerie' + guid()) }, { DescricaoBusca: 'MarcaPneu', Descricao: 'Marca', Id: ('txtMarca' + guid()) }, { DescricaoBusca: 'ModeloPneu', Descricao: 'Modelo', Id: ('txtModelo' + guid()) }, { DescricaoBusca: 'DimensaoPneu', Descricao: 'Dimensão', Id: ('txtDimensao' + guid()) }, { DescricaoBusca: 'StatusPneu', Descricao: 'Status', Id: ('txtStatus' + guid()) }], "/Pneu/Consultar?callback=?&Status=" + status, "Consulta de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Pneus por Tipo de Histórico
function CarregarConsultaDePneusPorTipoDeHistorico(idTxtBuscar, idBtnBuscar, tipoHistorico, codigoVeiculo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'MarcaPneu', Descricao: 'Marca', Id: ('txtMarca' + guid()) }, { DescricaoBusca: 'ModeloPneu', Descricao: 'Modelo', Id: ('txtModelo' + guid()) }, { DescricaoBusca: 'DimensaoPneu', Descricao: 'Dimensão', Id: ('txtDimensao' + guid()) }, { DescricaoBusca: 'StatusPneu', Descricao: 'Status', Id: ('txtStatus' + guid()) }], "/Pneu/ConsultarPorTipoDeHistorico?callback=?&Tipo=" + tipoHistorico + "&CodigoVeiculo=" + codigoVeiculo, "Consulta de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Históricos de Pneus
function CarregarConsultaDeHistoricosDePneus(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'SeriePneu', Descricao: 'Série do Pneu', Id: ('txtSerie' + guid()) }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Placa do Veículo', Id: ('txtPlacaVeiculo' + guid()) }], "/HistoricoDePneu/Consultar?callback=?", "Consulta de Históricos de Pneus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Apólices de Seguros
function CarregarConsultaDeApolicesDeSeguros(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CPFCNPJCliente', Descricao: 'Cliente', Id: ('txtCliente' + guid()), Tipo: 'cpf_cnpj' }, { DescricaoBusca: 'NomeSeguradora', Descricao: 'Seguradora', Id: ('txtSeguradora' + guid()) }, { DescricaoBusca: 'NumeroApolice', Descricao: 'Apólice', Id: ('txtApolice' + guid()) }, { DescricaoBusca: 'Ramo', Descricao: 'Ramo', Id: ('txtRamo' + guid()) }], "/ApoliceDeSeguro/Consultar?callback=?&Status=" + status, "Consulta de Apólices de Seguro", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Apólices de Seguros por Cliente
function CarregarConsultaDeApolicesDeSegurosPorCliente(idTxtBuscar, idBtnBuscar, cpfCnpjCliente, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeSeguradora', Descricao: 'Seguradora', Id: ('txtSeguradora' + guid()) }, { DescricaoBusca: 'NumeroApolice', Descricao: 'Apólice', Id: ('txtApolice' + guid()) }, { DescricaoBusca: 'Ramo', Descricao: 'Ramo', Id: ('txtRamo' + guid()) }], "/ApoliceDeSeguro/ConsultarPorClienteParaEmissao?callback=?&CPFCNPJCliente=" + cpfCnpjCliente, "Consulta de Apólices de Seguro", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Planos de Contas
function CarregarConsultaDePlanosDeContas(idTxtBuscar, idBtnBuscar, status, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'Conta', Descricao: 'Conta', Id: ('txtConta' + guid()) }], "/PlanoDeConta/Consultar?callback=?&Status=" + status + "&Tipo=" + tipo, "Consulta de Planos de Contas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3], fecharConsulta, usarKeyDown);
}

//Consulta de Movimentos do Financeiro
function CarregarConsultaDeMovimentosDoFinanceiro(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Documento', Descricao: 'Documento', Id: ('txtDocumento' + guid()) },
    { DescricaoBusca: 'ValorInicial', Descricao: 'Valor Inicial', Id: ('txtValorInicial' + guid()), Tipo: 'decimal' },
    { DescricaoBusca: 'ValorFinal', Descricao: 'Valor Final', Id: ('txtValorFinal' + guid()), Tipo: 'decimal' },
    { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: ('txtDataInicial' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: ('txtDataFinal' + guid()), Tipo: 'date' }], "/MovimentoDoFinanceiro/Consultar?callback=?", "Consulta de Movimentos do Financeiro", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Unidades de Medidas
function CarregarConsultaDeUnidadesDeMedidas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/UnidadeDeMedida/Consultar?callback=?", "Consulta de Unidades de Medidas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Fretes
function CarregarConsultaDeFretes(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeCliente', Descricao: 'Nome do Cliente', Id: ('txtNomeCliente' + guid()) }, { DescricaoBusca: 'CPFCNPJCliente', Descricao: 'CPF/CNPJ do Cliente', Id: ('txtCPFCNPJCliente' + guid()) }], "/Frete/Consultar?callback=?&Status=" + status, "Consulta de Fretes", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Séries
function CarregarConsultaDeSeries(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Serie', Descricao: 'Série', Id: ('txtSerie' + guid()) }], "/EmpresaSerie/Consultar?callback=?&Status=" + status, "Consulta de Séries", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Séries
function CarregarConsultaDeSeriesPorTipo(idTxtBuscar, idBtnBuscar, status, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Serie', Descricao: 'Série', Id: ('txtSerie' + guid()) }], "/EmpresaSerie/ConsultarPorTipo?callback=?&Status=" + status + "&Tipo=" + tipo, "Consulta de Séries", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Ocorrências
function CarregarConsultaDeTiposDeOcorrencias(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeOcorrencia/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Ocorrências", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

//Consulta de Funcionarios
function CarregarConsultaDeFuncionarios(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtDescricao' + guid()) }], "/Usuario/ConsultarTodos?callback=?&Status=" + status, "Consulta de Funcionários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Ocorrências de Funcionários
function CarregarConsultaDeOcorrenciasDeFuncionarios(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeFuncionario', Descricao: 'Funcionário', Id: ('txtFuncionario' + guid()) }, { DescricaoBusca: 'DescricaoTipoOcorrencia', Descricao: 'Tipo de Ocorrência', Id: ('txtTipoOcorrencia' + guid()) }], "/OcorrenciaDeFuncionario/Consultar?callback=?&Status=" + status, "Consulta de Ocorrências de Funcionários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Leituras de Tacógrafos
function CarregarConsultaDeLeiturasDeTacografos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeMotorista', Descricao: 'Motorista', Id: ('txtMotorista' + guid()) }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Veículo', Id: ('txtVeículo' + guid()) }], "/LeituraDeTacografo/Consultar?callback=?&Status=" + status, "Consulta de Leituras de Tacógrafos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Ocorrência de CT-e
function CarregarConsultaDeTiposDeOcorrenciasDeCTes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoTipoOcorrencia' + guid()) }], "/TipoDeOcorrenciaDeCTe/Consultar?callback=?", "Consulta de Tipos de Ocorrências de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaDeTiposDeOcorrenciasDeCTesCadastro(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoTipoOcorrencia' + guid()) }], "/TipoDeOcorrenciaDeCTe/Consultar?callback=?&Tipo=Cadastro", "Consulta de Tipos de Ocorrências de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Ocorrência de CT-e
function CarregarConsultaDeOcorrenciasDeCTes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DescricaoTipoOcorrencia', Descricao: 'Ocorrência', Id: ('txtDescricaoTipoOcorrencia' + guid()) },
    { DescricaoBusca: 'ObservacaoOcorrencia', Descricao: 'Observação', Id: ('txtObservacaoOcorrencia' + guid()) },
    { DescricaoBusca: 'NumeroNF', Descricao: 'Núm. NF', Id: (guid()) }], "/OcorrenciaDeCTe/Consultar?callback=?", "Consulta de Ocorrências de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Ocorrência de NFS-e
function CarregarConsultaDeOcorrenciasDeNFSes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DescricaoTipoOcorrencia', Descricao: 'Ocorrência', Id: ('txtDescricaoTipoOcorrencia' + guid()) },
    { DescricaoBusca: 'ObservacaoOcorrencia', Descricao: 'Observação', Id: ('txtObservacaoOcorrencia' + guid()) },
    { DescricaoBusca: 'NumeroNFSe', Descricao: 'Núm. NFSe', Id: (guid()) }], "/OcorrenciaDeNFSe/Consultar?callback=?", "Consulta de Ocorrências de NFS-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Ocorrência de NF-e
function CarregarConsultaDeOcorrenciasDeNFes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DescricaoTipoOcorrencia', Descricao: 'Ocorrência', Id: ('txtDescricaoTipoOcorrencia' + guid()) },
    { DescricaoBusca: 'ObservacaoOcorrencia', Descricao: 'Observação', Id: ('txtObservacaoOcorrencia' + guid()) },
    { DescricaoBusca: 'NumeroNFe', Descricao: 'Núm. NFSe', Id: (guid()) }], "/OcorrenciaDeNFe/Consultar?callback=?", "Consulta de Ocorrências de NF-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}



//Consulta de Fretes por Valor
function CarregarConsultaDeFretesPorValor(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeCliente', Descricao: 'Nome do Cliente', Id: ('txtNomeCliente' + guid()) }, { DescricaoBusca: 'CPFCNPJCliente', Descricao: 'CPF/CNPJ do Cliente', Id: ('txtCPFCNPJCliente' + guid()) }], "/FretePorValor/Consultar?callback=?&Status=" + status, "Consulta de Fretes por Valor", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Custos Fixos
function CarregarConsultaDeTiposDeCustosFixos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeCustoFixo/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Custos Fixos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Custos Fixos
//function CarregarConsultaDeCustosFixosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
//    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Veículo', Id: ('txtVeiculo' + guid()) }], "/CustoFixo/Consultar?callback=?&Status=" + status, "Consulta de Custos Fixos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
//}

//Consulta de Custos Fixos
function CarregarConsultaDeCustosFixosDeVeiculos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Veículo', Id: ('txtVeiculo' + guid()) }, { DescricaoBusca: 'NomeMotorista', Descricao: 'Motorista', Id: ('txtMotorista' + guid()) }], "/CustoFixo/Consultar?callback=?&Status=" + status, "Consulta de Custos Fixos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Aliquotas de ICMS
function CarregarConsultaDeAliquotasDeICMS(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Aliquota', Descricao: 'Alíquota', Id: ('txtAliquota' + guid()), Tipo: 'decimal' }], "/AliquotaDeICMS/Consultar?callback=?&Status=" + status, "Consulta de Alíquotas de ICMS", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Campos da Carta de Correção Eletrônica
function CarregarConsultaDeCamposDaCartaDeCorrecaoEletronica(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/CartaDeCorrecaoEletronica/ConsultarCampos?callback=?", "Consulta de Campos da Carta de Correção Eletrõnica", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Campos da Carta de Correção Eletrônica
function CarregarConsultaDePercursosEntreEstados(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'UFOrigem', Descricao: 'UF Origem', Id: ('txtUFOrigem' + guid()) },
    { DescricaoBusca: 'UFDestino', Descricao: 'UF Destino', Id: ('txtUFDestino' + guid()) }], "/PercursoEstado/Consultar?callback=?", "Consulta de Percursos entre Estados", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de NCM
function CarregarConsultaDeNCMs(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Código', Id: ('txtCodigo' + guid()) }, { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/NCM/Consultar?callback=?", "Consulta de NCM", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Produtos
function CarregarConsultaDeProdutos(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) },
    { DescricaoBusca: 'CodigoProduto', Descricao: 'Cód. Produto', Id: ('txtCoditoProduto' + guid()) }], "/Produto/Consultar?callback=?&Status=" + status, "Consulta de Produtos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Unidades de Medidas Gerais
function CarregarConsultaDeUnidadesDeMedidaGerais(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'Sigla', Descricao: 'Sigla', Id: ('txtSigla' + guid()) }], "/UnidadeMedidaGeral/Consultar?callback=?&Status=" + status, "Consulta de Unidades de Medida", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Documentos de Entrada
function CarregarConsultaDeDocumentosDeEntrada(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()), Tipo: 'number' },
    { DescricaoBusca: 'NomeFornecedor', Descricao: 'Fornecedor', Id: ('txtFornecedor' + guid()) },
    { DescricaoBusca: 'DataEntrada', Descricao: 'Data Entrada', Id: ('txtDataEntrada' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: ('txtDataEmissao' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'CFOP', Descricao: 'CFOP Itens', Id: ('txtCFOP' + guid()) },], "/DocumentoEntrada/Consultar?callback=?&Status=" + status, "Consulta de Documentos de Entrada", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Duplicatas
function CarregarConsultaDuplicatas(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()), Tipo: 'number' },
    { DescricaoBusca: 'Pessoa', Descricao: 'Pessoa', Id: ('txtPessoa' + guid()), Tipo: 'cpf_cnpj' },
    { DescricaoBusca: 'DataLancamento', Descricao: 'Data Lançamento', Id: ('txtDataLcto' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'Documento', Descricao: 'Documento', Id: ('txtDocumento' + guid()) },
    { DescricaoBusca: 'CTe', Descricao: 'CT-e', Id: ('CTe' + guid()) }], "/Duplicatas/Consultar?callback=?&Tipo=" + tipo, "Consulta de Duplicata", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de CTes Duplicatas
function CarregarConsultaDeCtesDuplicatas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown, hiddenURL) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: (guid()) },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: (guid()), Tipo: 'date' },
    { DescricaoBusca: 'NumeroDocumento', Descricao: 'Núm. NF', Id: (guid()), Tipo: 'number' }], "", "Consulta de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown, hiddenURL);
}

//Consulta de Minutas Devolucao Container
function CarregarConsultaDeMinutasDevolucaoContainer(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()), Tipo: 'number' },
        { DescricaoBusca: 'Container', Descricao: 'Container', Id: ('txtContainer' + guid()) },
        { DescricaoBusca: 'Importador', Descricao: 'Importador', Id: ('txtImportador' + guid()) },
        { DescricaoBusca: 'NomeTerminal', Descricao: 'Nome Terminal', Id: ('txtNomeTerminal' + guid()) },
        { DescricaoBusca: 'Armador', Descricao: 'Armador', Id: ('txtArmador' + guid()) },
        { DescricaoBusca: 'Navio', Descricao: 'Navio', Id: ('txtNavio' + guid()) },
        { DescricaoBusca: 'NomeMotorista', Descricao: 'Motorista', Id: ('txtNomeMotorista' + guid()) },
        { DescricaoBusca: 'PlacaTracao', Descricao: 'Tração', Id: ('txtPlacaTracao' + guid()) },
        { DescricaoBusca: 'PlacaReboque', Descricao: 'Reboque', Id: ('txtPlacaReboque' + guid()) },
        { DescricaoBusca: 'CTe', Descricao: 'CT-e', Id: ('CTe' + guid()) }
    ],  "/MinutaDevolucaoContainer/Consultar?callback=?", "Consulta de Minutas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}


//Consulta de Fretes por Tipo de Veículo
function CarregarConsultaDeFretesPorTipoDeVeiculo(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'TipoVeiculo', Descricao: 'Tipo Veículo', Id: guid() },
        { DescricaoBusca: 'ClienteOrigem', Descricao: 'Origem', Id: guid() },
        { DescricaoBusca: 'ClienteDestino', Descricao: 'Destino', Id: guid() },
        { DescricaoBusca: 'CNPJOrigem', Descricao: 'CNPJ Origem', Id: guid(), Tipo: 'cpf_cnpj' },
        { DescricaoBusca: 'CNPJDestino', Descricao: 'CNPJ Destino', Id: guid(), Tipo: 'cpf_cnpj' }
    ], "/FretePorTipoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Fretes por Tipo de Veículo", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}


//Consulta de Fretes por Tipo de Veículo
function CarregarConsultaDeFretesPorKMTipoDeVeiculo(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'TipoVeiculo', Descricao: 'Tipo Veículo', Id: guid() },
        { DescricaoBusca: 'KMInicial', Descricao: 'KM Inicial', Id: guid() },
        { DescricaoBusca: 'KMFinal', Descricao: 'KM Final', Id: guid() }
    ], "/FretePorKMTipoDeVeiculo/Consultar?callback=?&Status=" + status, "Consulta de Fretes por KM por Tipo de Veículo", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Manifestos Avon
function CarregarConsultaDeManifestosAvon(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: guid(), Tipo: 'date' }, { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: guid(), Tipo: 'date' }, { DescricaoBusca: 'NumeroManifesto', Descricao: 'Número', Id: guid(), Tipo: 'number' }], "/IntegracaoAvon/ConsultarSumarizado?callback=?&Status=" + status, "Consulta de Manifestos Avon", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Usuarios
function CarregarConsultaUsuario(idTxtBuscar, idBtnBuscar, status, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CPF', Descricao: 'CPF', Id: guid(), Tipo: 'cpf_cnpj' }, { DescricaoBusca: 'Nome', Descricao: 'Nome', Id: guid(), Tipo: 'text' }, { DescricaoBusca: 'Login', Descricao: 'Login', Id: guid(), Tipo: 'text' }], "/Empresa/BuscarUsuarios?callback=?&Status=" + status + "&Tipo=" + tipo, "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de DT Natura
function CarregarConsultaDeDtNatura(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: guid(), Tipo: 'date' }, { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: guid(), Tipo: 'date' }, { DescricaoBusca: 'NumeroDocumentoTransporte', Descricao: 'Número', Id: guid(), Tipo: 'number' }], "/IntegracaoNatura/Consultar?callback=?&Status=" + status, "Consulta de Manifestos Natura", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de CT-es Siga Fácil
function CarregarConsultaDeCTesSigaFacil(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NumeroInicial', Descricao: 'Nº Inicial', Id: guid(), Tipo: 'number' },
    { DescricaoBusca: 'NumeroFinal', Descricao: 'Nº Final', Id: guid(), Tipo: 'number' }], "/IntegracaoSigaFacil/ConsultarCTes?callback=?", "Consulta de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de MDF-e
function CarregarConsultaDeMDFes(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NumeroInicial', Descricao: 'Nº Inicial', Id: guid(), Tipo: 'number' }, { DescricaoBusca: 'NumeroFinal', Descricao: 'Nº Final', Id: guid(), Tipo: 'number' }], "/ManifestoEletronicoDeDocumentosFiscais/ConsultarSumarizado?callback=?&Status=" + status, "Consulta de MDF-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Servicos da NFS-e
function CarregarConsultaDeServicosNFSe(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: guid() }], "/ServicoNFSe/Consultar?callback=?&Status=" + status, "Consulta de Serviços", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Entregas
function CarregarConsultaDeEntregas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DataEntrega', Descricao: 'Data', Id: guid(), Tipo: 'date' }, { DescricaoBusca: 'PlacaVeiculo', Descricao: 'Veículo', Id: guid() }, { DescricaoBusca: 'NomeMotorista', Descricao: 'Motorista', Id: guid() }], "/Entrega/Consultar?callback=?", "Consulta de Entregas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Coletas
function CarregarConsultaDeTiposDeColetas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/TipoDeColeta/Consultar?callback=?&Status=" + status, "Consulta de Tipos de Coletas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Coletas
function CarregarConsultaDeColetas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: (guid()), Tipo: 'date' },
    { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: (guid()), Tipo: 'date' },
    { DescricaoBusca: 'NomeCliente', Descricao: 'Nome Remetente', Id: ('txtNomeCliente' + guid()) },
    { DescricaoBusca: 'CPFCNPJCliente', Descricao: 'CPF/CNPJ Remetente', Id: ('txtCPFCNPJCliente' + guid()) }], "/Coleta/Consultar?callback=?", "Consulta de Coletas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Bancos
function CarregarConsultaDeBancos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: (guid()), Tipo: 'number' }, { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: (guid()) }], "/Banco/Consultar?callback=?", "Consulta de Bancos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Naturezas da NFS-e
function CarregarConsultaDeNaturezasNFSe(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: guid() }], "/NaturezaNFSe/Consultar?callback=?&Status=" + status, "Consulta de Naturezas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Despesas Adicionais da Empresa
function CarregarConsultaDeMinutasAvon(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Número MinutaAvon', Descricao: 'Número Minuta Avon', Id: ('txtNumeroMinutaAvon' + guid()) },
    { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: ('txtDataInicial' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: ('txtDataFinal' + guid()), Tipo: 'date' }], "/IntegracaoAvon/Consultar?callback=?&Status=" + status, "Consulta de Minutas Avon", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Clientes Comissão
function CarregarConsultaClienteComissao(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeParceiro', Descricao: 'Nome Parceiro', Id: ('txtNomeParceiro' + guid()) },
    { DescricaoBusca: 'NomeCidade', Descricao: 'Nome Cidade', Id: ('txtNomeCidade' + guid()) }], "/ClienteComissao/Consultar?callback=?&Tipo=" + tipo, "Consulta Comissão Cliente", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Duplicatas
function CarregarConsultaFreteSubcontratado(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Parceiro', Descricao: 'CNPJ Parceiro', Id: ('txtParceiro' + guid()), Tipo: 'cpf_cnpj' },
    { DescricaoBusca: 'NomeParceiro', Descricao: 'Nome Parceiro', Id: ('txtNomeParceiro' + guid()) },
    { DescricaoBusca: 'DataEntrada', Descricao: 'Data Entrada', Id: ('txtDataEntrada' + guid()), Tipo: 'date' },
    { DescricaoBusca: 'CTe', Descricao: 'Número CTe', Id: ('txtCTe' + guid()), Tipo: 'number' },
    { DescricaoBusca: 'NFe', Descricao: 'Número NFe', Id: ('txtNFe' + guid()), Tipo: 'number' }], "/FreteSubcontratado/Consultar?callback=?&Status=" + status, "Consulta de Frete Subcontratado", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Operação
function CarregarConsultaTipoOperacao(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: (guid()) }], "/TipoDeOperacao/Consultar?callback=?", "Consulta de Tipos de Operações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Carga Embarcador
function CarregarConsultaTipoCargaEmbarcador(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: (guid()) }], "/TipoDeCarga/ConsultarTipoCargaEmbarcador?callback=?", "Consulta de Tipos de Carga", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tipos de Carga Embarcador
function CarregarConsultaModeloVeicularCarga(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: (guid()) }], "/ModeloVeicularCarga/Consultar?callback=?", "Consulta de Modelos Veiculares de Carga", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Usuários
function CarregarConsultaDeUsuarios(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtUSuario' + guid()) },
    { DescricaoBusca: 'Login', Descricao: 'Login', Id: ('txtLogin' + guid()) }], "/Usuario/Consultar?callback=?", "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Fretes
function CarregarConsultaDeFreteFracionadoUnidade(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeCliente', Descricao: 'Nome do Cliente', Id: ('txtNomeCliente' + guid()) },
    { DescricaoBusca: 'CPFCNPJCliente', Descricao: 'CPF/CNPJ do Cliente', Id: ('txtCPFCNPJCliente' + guid()) },
    { DescricaoBusca: 'NomeCidade', Descricao: 'Nome Cidade', Id: ('txtNomeCidade' + guid()) }], "/FreteFracionadoUnidade/Consultar?callback=?&Status=" + status, "Consulta de Fretes Fracionado Unidade", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Perfil Permissão
function CarregarConsultaDePerfil(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Descricao', Id: ('txtDescricao' + guid()) }], "/PerfilPermissao/Consultar?callback=?", "Consulta de Perfil", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Perfil Permissão
function CarregarConsultaDePerfilAtivo(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Descricao', Id: ('txtDescricao' + guid()) }], "/PerfilPermissao/ConsultarAtivos?callback=?", "Consulta de Perfil", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

// Consulta de Layout EDI
function CarregarConsultaDeLayoutEDI(idTxtBuscar, idBtnBuscar, tipoLayoutEDI, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/LayoutEDI/Consulta?callback=?&TipoLayout=" + tipoLayoutEDI, "Consulta de Layout EDI", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

// Consulta de Produto Fornecedor
function CarregarConsultaDeProdutoFornecedor(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()) },
    { DescricaoBusca: 'Produto', Descricao: 'Produto', Id: ('txtProduto' + guid()) },
    { DescricaoBusca: 'Fornecedor', Descricao: 'Fornecedor', Id: ('txtFornecedor' + guid()), Tipo: "cpf_cnpj" }], "/ProdutoFornecedor/Consultar?callback=?", "Consulta de Produto Fornecedor", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2], fecharConsulta, usarKeyDown);
}

// Consulta de Proposta
function CarregarConsultaProposta(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Cliente', Descricao: 'Cliente', Id: ('txtCliente' + guid()), Tipo: "cpf_cnpj" },
    { DescricaoBusca: 'Data', Descricao: 'Data', Id: ('txtDataLancamento' + guid()), Tipo: "date" },
    { DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtNome' + guid()) }], "/Proposta/Consultar?callback=?", "Consulta de Proposta", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

// Consulta de Impressoras
function CarregarConsultaDeImpressoras(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Impressora', Descricao: 'Impressora', Id: ('txtImpressora' + guid()) },
    { DescricaoBusca: 'Unidade', Descricao: 'Unidade', Id: ('txtUnidade' + guid()), Tipo: 'number' }], "/Impressoras/Consultar?callback=?&Status=" + status, "Consulta de Impressoras", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

// Consulta de NFSe
function CarregarConsultaDeNFSe(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'NumeroInicial', Descricao: 'Número Inicial', Id: (guid()), Tipo: 'number' },
        { DescricaoBusca: 'NumeroFinal', Descricao: 'Número Final', Id: (guid()), Tipo: 'number' }],

        "/NotaFiscalDeServicosEletronica/Consultar?callback=?&Status=" + status, "Consulta de NFS-e", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 4, 7, 8], fecharConsulta, usarKeyDown);
}

// Consulta de NFe
function CarregarConsultaDeNFe(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'NumeroNota', Descricao: 'Número', Id: (guid()), Tipo: 'number' }],

        "/XMLNotaFiscalEletronica/Pesquisar?callback=?&Status=" + status, "Consulta de NF-e", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3, 4], fecharConsulta, usarKeyDown);
}

// Consulta de Impressoras
function CarregarConsultaDeRegraICMS(idTxtBuscar, idBtnBuscar, callbackSelecionar, status, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'UFEmitete', Descricao: 'Emitete', Id: ('txtUFEmitete' + guid()) },
        { DescricaoBusca: 'UFOrigem', Descricao: 'Origem', Id: ('txtUFOrigem' + guid()) },
        { DescricaoBusca: 'UFDestino', Descricao: 'Destino', Id: ('txtUFDestino' + guid()) },
        { DescricaoBusca: 'UFTomador', Descricao: 'Tomador', Id: ('txtUFTomador' + guid()) },
    ], "/RegraICMS/Consultar?callback=?&Status=" + status, "Regras de ICMS", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaCidadesExportacao(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }],
        "/Localidade/Consulta?UF=EX&SomenteEmpresa=true&callback=?", "Consulta de Cidades Exportação", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaCadastroArquivo(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) },
        { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: ('txtDataInicial' + guid()), Tipo: 'date' },
        { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: ('txtDataFinal' + guid()), Tipo: 'date' }
    ],
        "/ArquivoTransportador/Consultar?callback=?", "Consulta de Arquivos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Clientes
function CarregarConsultaCalculoRelacaoCTesEntregues(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Cliente', Descricao: 'CPF / CNPJ', Id: ('txtCPFCNPJCliente' + guid()), Tipo: 'cpf_cnpj' }], "/CalculoRelacaoCTesEntregues/Consultar?callback=?", "Consulta de Configuração Valores Relação CT-es Entregues", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Fretes
function CarregarConsultaDeFreteFracionadoValor(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeCliente', Descricao: 'Nome do Cliente', Id: ('txtNomeCliente' + guid()) },
    { DescricaoBusca: 'CPFCNPJCliente', Descricao: 'CPF/CNPJ do Cliente', Id: ('txtCPFCNPJCliente' + guid()) },
    { DescricaoBusca: 'NomeCidade', Descricao: 'Nome Cidade', Id: ('txtNomeCidade' + guid()) }], "/FreteFracionadoValor/Consultar?callback=?&Status=" + status, "Consulta de Fretes Fracionado Valor", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de CTes para Referencia
function CarregarConsultaDeCTesParaReferencia(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown, dataEmissao) {
    if (dataEmissao == null)
        dataEmissao = Globalize.format(new Date(), "dd/MM/yyyy");
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: (guid()) },
    { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: (guid()), Tipo: 'date', Default: dataEmissao },
    { DescricaoBusca: 'NumeroDocumento', Descricao: 'Núm. NF', Id: (guid()), Tipo: 'number' }], "/CTesReferencia/ConsultarPorTipo?callback=?&TipoCTe=" + tipo, "Consulta de CT-es", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Tecnologia do rastreador
function CarregarConsultaDeTecnologiaRastreador(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(
        idTxtBuscar,
        idBtnBuscar,
        [
            { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: guid() }
        ],
        "/TecnologiaRastreador/Consultar?callback=?",
        "Consulta de Tecnologia do Rastreador",
        [
            { Descricao: "Selecionar", Evento: callbackSelecionar }
        ],
        [0],
        fecharConsulta,
        usarKeyDown
    );
}

//Consulta de Tipo de Comunicação do Rastreador
function CarregarConsultaDeTipoComunicacaoRastreador(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(
        idTxtBuscar,
        idBtnBuscar,
        [
            { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: guid() }
        ],
        "/TipoComunicacaoRastreador/Consultar?callback=?",
        "Consulta de Tipo de Comunicação do Rastreador",
        [
            { Descricao: "Selecionar", Evento: callbackSelecionar }
        ],
        [0],
        fecharConsulta,
        usarKeyDown
    );
}