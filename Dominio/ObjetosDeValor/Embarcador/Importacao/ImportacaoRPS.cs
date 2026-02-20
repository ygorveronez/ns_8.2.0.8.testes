using System;

namespace Dominio.ObjetosDeValor.Embarcador.Importacao
{
    public class ImportacaoRPS
    {
        public string Chave { get; set; }
        public string CNPJRemetente { get; set; }
        public int Rota { get; set; }
        public int Loja { get; set; }
        public int NumeroNFSe { get; set; }
        public string SerieNFSe { get; set; }
        public int NumeroRPS { get; set; }
        public string SerieRPS { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CNPJEmitenteRPSNFSe { get; set; }
        public string CNPJTomadorNFSe { get; set; }
        public decimal ValorMercadoria { get; set; }
        public int Peso { get; set; }
        public string Veiculo { get; set; }
        public decimal ValorServico { get; set; }
        public int AliquotaISS { get; set; }
        public decimal BaseISS { get; set; }
        public decimal ValorISS { get; set; }
        public string NBS { get; set; }
        public string CodigoIndicadorOperacao { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }
    }
}
