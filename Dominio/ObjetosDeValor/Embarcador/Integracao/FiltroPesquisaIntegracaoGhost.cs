using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaIntegracaoGhost
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public List<SituacaoIntegracao> SituacaoIntegracao { get; set; }
        public List<TipoDestinoGhost> TipoDestino{ get; set; }
        public string Chave { get; set; }
    }
}
