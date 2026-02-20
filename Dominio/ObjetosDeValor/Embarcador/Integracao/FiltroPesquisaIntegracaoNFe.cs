using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaIntegracaoNFe
    {
        public string NumeroCarga { get; set; }
        public string Chave { get; set; }
        public string NumeroPedido { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public List<SituacaoIntegracao> SituacaoIntegracao { get; set; }
        public List<SituacaoProcessamentoRegistro> SituacaoProcessamentoRegistro{ get; set; }
    }
}
