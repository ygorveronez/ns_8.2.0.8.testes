using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public sealed class FiltroPesquisaMotivoRejeicao
    {
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public TipoMotivo TipoMotivo { get; set; }
        public string Descricao { get; set; }
    }
}
