using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class LoteChamadoOcorrencia
    {

        public int Codigo { get; set; }

        public int NumeroLote { get; set; }

        public string Transportadores { get; set; }

        public string SituacaoDescricao { get { return Situacao >= 0 ? Situacao.ToString().ToEnum<SituacaoLoteChamadoOcorrencia>().ObterDescricao() : string.Empty; } }

        public DateTime DataCriacao { get; set; }

        private int Situacao { get; set; }

    }
}