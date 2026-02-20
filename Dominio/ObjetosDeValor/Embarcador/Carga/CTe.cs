using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CTe
    {
        public int NumeroProtocolo { get; set; }

        public string Chave { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string Modelo { get; set; }

        public string DataEmissao { get; set; }

        public Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public string CNPJCPFDestinatario { get; set; }

        public string CNPJCPFRemetente { get; set; }

        public string CNPJCPFExpedidor { get; set; }

        public string CNPJCPFRecebedor { get; set; }

        public string CNPJCPFTomador { get; set; }

        public decimal ValorPrestacaoServico { get; set; }

        public decimal ValorAReceber { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal AliquotaICMS { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal PercentualReducaoBaseCalculoICMS { get; set; }

        public decimal PercentualICMSIncluirNoFrete { get; set; }

        public decimal ValorPresumido { get; set; }

        public decimal ValorICMSDevido { get; set; }

        public bool IncluirICMSNoFrete { get; set; }

        public decimal Peso { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public string VersaoCTE { get; set; }

        public string CFOP { get; set; }

        public string CST { get; set; }

        public bool Lotacao { get; set; }

        public List<ComponenteFreteCTe> componentesFreteCTe { get; set; }

        public List<int> ProtocolosDePedidos { get; set; }

    }
}
