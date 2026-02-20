using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.ISS
{
    public class FiltroPesquisaAliquotaISS
    {
        public string Descricao { get; set; }

        public SituacaoAtivoPesquisa Ativo { get; set; }

        public int CodigoLocalidade { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public SituacaoVigencia SituacaoVigencia { get; set; }
    }
}