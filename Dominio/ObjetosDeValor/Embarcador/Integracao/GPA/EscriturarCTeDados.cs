using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GPA
{
    public class EscriturarCTeDados
    {
        public string protocolo { get; set; }
        public string cenario { get; set; }
        public string empresa { get; set; }
        public string localNegocio { get; set; }
        public string centro { get; set; }
        public string idFiscal { get; set; }
        public string nfeNum { get; set; }
        public string serie { get; set; }
        public string material { get; set; }
        public string centroCusto { get; set; }
        public string doctoData { get; set; }
        public string doctoStatus { get; set; }
        public string doctoNro { get; set; }
        public string digitoChave { get; set; }
        public string protocoloData { get; set; }
        public string protocoloHora { get; set; }
        public string protocoloLog { get; set; }
        public string chaveDANFE { get; set; }
        public string codVerifNFSe { get; set; }
        public string CPFCNPJ { get; set; }
        public string CFOP { get; set; }
        public string CFOP_NF { get; set; }
        public int quantidade { get; set; }
        public string unidadeMedida { get; set; }
        public decimal precoLiquido { get; set; }
        public decimal valorLiquido { get; set; }
        public decimal precoComImpostos { get; set; }
        public decimal valorComImpostos { get; set; }
        public string numeroRPS { get; set; }
        public List<EscriturarCTeDadosTaxas> taxas { get; set; }          
    }
}
