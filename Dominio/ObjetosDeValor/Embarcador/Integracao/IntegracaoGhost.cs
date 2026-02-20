using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class IntegracaoGhost
    {
        public int Codigo { get; set; }
        public string Chave { get; set; }
        public DateTime DataIntegracao { get; set; }
        public SituacaoIntegracao SituacaoIntegracao { get; set; }
        public TipoDestinoGhost TipoDestino { get; set; }
        public string Retorno { get; set; }
    }
}