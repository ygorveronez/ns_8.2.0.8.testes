var EnumOrigemAuditadoHelper = function () {
    this.Todas = "";
    this.Sistema = 0;
    this.SistemaImportacao = 1;
    this.WebServiceCargas = 2;
    this.WebServiceCTe = 3;
    this.WebServiceEmpresa = 4;
    this.WebServiceFilial = 5;
    this.WebServiceImpressao = 6;
    this.WebServiceIntegracaoCTe = 7;
    this.WebServiceIntegracaoMDFe = 8;
    this.WebServiceIntegracaoNFSe = 9;
    this.WebServiceJanelaCarregamento = 10;
    this.WebServiceMDFe = 11;
    this.WebServiceNFe = 12;
    this.WebServiceNFS = 13;
    this.WebServiceOcorrencias = 14;
    this.WebServicePallet = 15;
    this.WebServicePessoas = 16;
    this.WebServiceRota = 17;
    this.WebServicePedidos = 18;
    this.WebServiceModeloVeicular = 19;
    this.WebServiceRastreadora = 20;
    this.LeituraEmail = 21;
    this.WebServiceCanhotos = 22;
    this.WebServiceMonitoriamento = 23;
    this.WebServiceFaturamento = 24;
    this.WebServiceControleEntrega = 25;
    this.WebServiceUsuarios = 26;
    this.WebServiceOrdemEmbarque = 27;
    this.GerenciadorApp = 28;
    this.WebServiceMobile = 29;
    this.WebServiceValePedagio = 30;
    this.WebServiceFretes = 31;
    this.WebServiceFinanceiro = 32;
    this.WebServiceOrdemServico = 33;
    this.LeituraDeRIC_WebServiceMobile = 34;
    this.LeituraDeRIC_Sistema = 35;
    this.WebServiceFrota = 36;
    this.WebSContratoFreteTerceiro = 3;
}

EnumOrigemAuditadoHelper.prototype = {
    obterDescricao: function (origem) {
        switch (origem) {
            case this.Sistema: return "Sistema";
            case this.SistemaImportacao: return "Importação";
            case this.WebServiceCargas: return "WS Cargas";
            case this.WebServiceCTe: return "WS CT-e";
            case this.WebServiceEmpresa: return "WS Empresa";
            case this.WebServiceFilial: return "WS Filial";
            case this.WebServiceImpressao: return "WS Impressão";
            case this.WebServiceIntegracaoCTe: return "WS Integração CT-e";
            case this.WebServiceIntegracaoMDFe: return "WS Integração MDF-e";
            case this.WebServiceIntegracaoNFSe: return "WS Integração NFS-e";
            case this.WebServiceJanelaCarregamento: return "WS Janela de Carregamento";
            case this.WebServiceMDFe: return "WS MDF-e";
            case this.WebServiceNFe: return "WS NF-e";
            case this.WebServiceNFS: return "WS NFS";
            case this.WebServiceOcorrencias: return "WS Ocorrências";
            case this.WebServicePallet: return "WS Pallet";
            case this.WebServicePessoas: return "WS Pessoas";
            default: return "";
        }
    },
    obterNome: function (origem) {
        switch (origem) {
            case this.Sistema: return "Sistema";
            case this.SistemaImportacao: return "SistemaImportacao";
            case this.WebServiceCargas: return "WebServiceCargas";
            case this.WebServiceCTe: return "WebServiceCTe";
            case this.WebServiceEmpresa: return "WebServiceEmpresa";
            case this.WebServiceFilial: return "WebServiceFilial";
            case this.WebServiceImpressao: return "WebServiceImpressao";
            case this.WebServiceIntegracaoCTe: return "WebServiceIntegracaoCTe";
            case this.WebServiceIntegracaoMDFe: return "WebServiceIntegracaoMDFe";
            case this.WebServiceIntegracaoNFSe: return "WebServiceIntegracaoNFSe";
            case this.WebServiceJanelaCarregamento: return "WebServiceJanelaCarregamento";
            case this.WebServiceMDFe: return "WebServiceMDFe";
            case this.WebServiceNFe: return "WebServiceNFe";
            case this.WebServiceNFS: return "WebServiceNFS";
            case this.WebServiceOcorrencias: return "WebServiceOcorrencias";
            case this.WebServicePallet: return "WebServicePallet";
            case this.WebServicePessoas: return "WebServicePessoas";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "WS Cargas", value: this.WebServiceCargas },
            { text: "WS CT-e", value: this.WebServiceCTe },
            { text: "WS Empresa", value: this.WebServiceEmpresa },
            { text: "WS Filial", value: this.WebServiceFilial },
            { text: "WS Impressão", value: this.WebServiceImpressao },
            { text: "WS Integração CT-e", value: this.WebServiceIntegracaoCTe },
            { text: "WS Integração MDF-e", value: this.WebServiceIntegracaoMDFe },
            { text: "WS Integração NFS-e", value: this.WebServiceIntegracaoNFSe },
            { text: "WS Janela de Carregamento", value: this.WebServiceJanelaCarregamento },
            { text: "WS MDF-e", value: this.WebServiceMDFe },
            { text: "WS NF-e", value: this.WebServiceNFe },
            { text: "WS NFS", value: this.WebServiceNFS },
            { text: "WS Ocorrências", value: this.WebServiceOcorrencias },
            { text: "WS Pallet", value: this.WebServicePallet },
            { text: "WS Pessoas", value: this.WebServicePessoas },
            { text: "Pedidos", value: this.WebServicePedidos },
            { text: "Rastreadora", value: this.WebServiceRastreadora },
            { text: "Canhotos", value: this.WebServiceCanhotos },
            { text: "Faturamento", value: this.WebServiceFaturamento },
            { text: "Controle de Entrega", value: this.WebServiceControleEntrega },
            { text: "Usuarios", value: this.WebServiceUsuarios },
            { text: "Ordem de Embarque", value: this.WebServiceOrdemEmbarque },
            { text: "Financeiro", value: this.WebServiceMobile },
            { text: "Vale Pedágio", value: this.WebServiceValePedagio },
            { text: "Fretes", value: this.WebServiceFretes },
            { text: "Financeiro", value: this.WebServiceFinanceiro },
            { text: "Frota", value: this.WebServiceFrota },
            { text: "Contrato Frete Terceiro", value: this.WebSContratoFreteTerceiro },
        ];
    }
}

var EnumOrigemAuditado = Object.freeze(new EnumOrigemAuditadoHelper());