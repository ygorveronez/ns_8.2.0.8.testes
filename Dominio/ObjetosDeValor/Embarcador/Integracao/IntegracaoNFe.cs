using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class IntegracaoNFe
    {
        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedido { get; set; }
        public DateTime DataIntegracao { get; set; }
        public SituacaoIntegracao SituacaoIntegracao { get; set; }
        public SituacaoProcessamentoRegistro SituacaoProcessamentoRegistro { get; set; }
        public string Retorno { get; set; }
    }
}