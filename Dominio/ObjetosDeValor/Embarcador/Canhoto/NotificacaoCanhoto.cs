using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class NotificacaoCanhoto
    {
        public int Transportador { get; set; }

        public string EmailTransportador { get; set; }

        public string NumeroCarga { get; set; }

        public DateTime? DataPrazo { get; set; }

        public TipoCanhoto TipoCanhoto { get; set; }

        public SituacaoDigitalizacaoCanhoto SituacaoDigitalizacao { get; set; }

        public string DescricaoTransportador { get; set; }

        public string TipoCarga { get; set; }

        public int Filial { get; set; }

        public string EmailFilial { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int Numero { get; set; }

        public string Estabelecimento { get; set; }

        public string Estabelecimento_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.Estabelecimento));
            }
        }

        public string Cliente { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string Serie { get; set; }

        public SituacaoCanhoto SituacaoCanhoto { get; set; }
        public string EmailEnvioCanhotoTransportador { get; set; }
        public string Destinatario { get; set; }
        public string CTe { get; set; }
        public string PlacaVeiculoRespEntrega { get; set; }
    }
}