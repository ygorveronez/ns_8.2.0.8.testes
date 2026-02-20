using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public class FiltroPesquisaTransportadorIntegracao
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoIntegracao? Situacao { get; set; }
        public int CodigoTransportador { get; set; }
        public bool? AtualizouCadastro { get; set; }
    }
}
