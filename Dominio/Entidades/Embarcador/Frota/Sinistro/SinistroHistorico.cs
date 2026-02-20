using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_HISTORICO", EntityName = "SinistroHistorico", Name = "Dominio.Entidades.Embarcador.Frota.SinistroHistorico", NameType = typeof(SinistroHistorico))]
    public class SinistroHistorico : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SHC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SHC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SHC_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "SHC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoInfracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoInfracao Tipo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroDados", Column = "SDS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SinistroDados Sinistro { get; set; }

        public virtual string Descricao => $"Histórico {Codigo}";
    }
}
