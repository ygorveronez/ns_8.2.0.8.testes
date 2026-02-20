using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_CHAMADO_OCORRENCIA", EntityName = "LoteChamadoOcorrencia", Name = "Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia", NameType = typeof(LoteChamadoOcorrencia))]
    public class LoteChamadoOcorrencia : EntidadeBase, IEntidade
    {
        public LoteChamadoOcorrencia() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "LCO_NUMERO_LOTE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "LCO_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "LCO_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteChamadoOcorrencia), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteChamadoOcorrencia Situacao { get; set; }

        public virtual string Descricao
        {
            get { return NumeroLote.ToString(); }
        }
    }
}
