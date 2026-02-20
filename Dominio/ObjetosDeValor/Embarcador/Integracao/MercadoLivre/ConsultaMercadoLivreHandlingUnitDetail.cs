using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre
{
    public class ConsultaMercadoLivreHandlingUnitDetail
    {
        public int Codigo { get; set; }

        public int CodigoCargaIntegracaoMercadoLivre { get; set; }

        public string Situacao { get; set; }

        public string TipoDoDocumento { get; set; }

        public string ChaveDeAcesso { get; set; }

        public string Mensagem { get; set; }

        public decimal ValorDeMercadoria { get; set; }
    }
}
