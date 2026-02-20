using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class Devolucao
    {
        public int Codigo { get; set; }
        public string Cliente { get; set; }
        public string ClienteCpfCnpj { get; set; }
        public string ClienteCodigoIntegracao { get; set; }
        public string Filial { get; set; }
        public string FilialCnpj { get; set; }
        public string FilialCodigoIntegracao { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int NumeroPallets { get; set; }
        public int NumeroPalletsEntregues { get; set; }
        public DateTime DataDevolucao { get; set; }
        public string DescricaoDataDevolucao
        {
            get
            {
                if (DataDevolucao != DateTime.MinValue)
                    return DataDevolucao.ToString("dd/MM/yyyy");

                return string.Empty;
            }
        }
        public DateTime DataCarregamentoCarga { get; set; }
        public DateTime DataEmissaoNotaFiscal { get; set; }
        public string DescricaoDataTransporte
        {
            get
            {
                if (DataCarregamentoCarga != DateTime.MinValue)
                    return DataCarregamentoCarga.ToString("dd/MM/yyyy");
                else if (DataEmissaoNotaFiscal != DateTime.MinValue)
                    return DataEmissaoNotaFiscal.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public string Motorista { get; set; }
        public string Veiculo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet Situacao { get; set; }
        public string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega:
                        return "Ag. Entrega";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue:
                        return "Entregue";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado:
                        return "Cancelado";
                    default:
                        return string.Empty;
                }
            }
        }

        public int SituacaoPallet1 { get; set; }
        public int SituacaoPallet2 { get; set; }
        public int SituacaoPallet3 { get; set; }
        public int SituacaoPallet4 { get; set; }
        public int SituacaoPallet5 { get; set; }
        public int SituacaoPallet6 { get; set; }
        public int SituacaoPallet7 { get; set; }
        public int SituacaoPallet8 { get; set; }
        public int SituacaoPallet9 { get; set; }
        public int SituacaoPallet10 { get; set; }
        public decimal ValorTotalCobrado { get; set; }
        public string Destino { get; set; }

        public string DescricaoValorTotalCobrado
        {
            get
            {
                return ValorTotalCobrado.ToString("n2");
            }
        }


    }
}
