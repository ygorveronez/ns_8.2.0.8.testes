var ConfigImport = {
    Veiculos: [
        { nome: "UF", id: "UF", width: 105, obrigatorio: false, validacao: "size:2" },
        { nome: "KM Atual", id: "KMAtual", width: 125, obrigatorio: false, validacao: "numbers" },
        { nome: "Placa", id: "Placa", width: 110, obrigatorio: true, validacao: "required" },
        { nome: "Chassi", id: "Chassi", width: 150, obrigatorio: false, validacao: "" },
        { nome: "Ano", id: "Ano", width: 110, obrigatorio: false, validacao: "numbers" },
        { nome: "Ano Modelo", id: "AnoModelo", width: 170, obrigatorio: false, validacao: "numbers" },
        { nome: "Renavam", id: "Renavam", width: 150, obrigatorio: true, validacao: "required|numbers|max_length:11" },
        { nome: "Tara", id: "Tara", width: 130, obrigatorio: false, validacao: "numbers" },
        { nome: "Tipo Dono", id: "Tipo", width: 130, obrigatorio: false, validacao: "tipo_dono" },
        { nome: "Tipo Veiculo", id: "TipoVeiculo", width: 180, obrigatorio: true, validacao: "tipo_veiculo" },
        { nome: "Tipo Rodado", id: "TipoRodado", width: 190, obrigatorio: false, validacao: "tipo_rodado" },
        { nome: "Tipo Carroceria", id: "TipoCarroceria", width: 200, obrigatorio: false, validacao: "tipo_carroceria" },
        { nome: "Cap. KG", id: "CapKG", width: 150, obrigatorio: false, validacao: "numbers" },
        { nome: "Cap. M3", id: "CapM3", width: 150, obrigatorio: false, validacao: "numbers" },
        { nome: "Observação", id: "Observacao", width: 200, obrigatorio: false, validacao: "" },
        { nome: "CPF Motorista", id: "CPFMotorista", width: 190, obrigatorio: false, validacao: "numbers|cpfcnpj" },
        { nome: "Nome Motorista", id: "NomeMotorista", width: 200, obrigatorio: false, validacao: "" },
        { nome: "CNPJ/CPF Proprietário", id: "CNPJCPFProprietario", width: 225, obrigatorio: false, validacao: "required_if:Tipo,T|cpfcnpj" },
        { nome: "RNTRC Proprietáro", id: "RNTRCProprietaro", width: 235, obrigatorio: false, validacao: "required_if:Tipo,T|numbers" },
        { nome: "Tipo Proprietário", id: "TipoProprietario", width: 185, obrigatorio: false, validacao: "tipo_proprietario" },
    ],
    Clientes: [
    ],
    Motoristas: [
        { nome: "Nome", id: "Nome", width: 200, obrigatorio: true, validacao: "required" },
        { nome: "CPF", id: "CPF", width: 190, obrigatorio: true, validacao: "required|numbers|cpfcnpj" },
        { nome: "IBGE", id: "IBGE", width: 190, obrigatorio: false, validacao: "numbers" },
        { nome: "CNH", id: "CNH", width: 190, obrigatorio: false, validacao: "numbers" },
    ]
};

window.importacao = ConfigImport;