using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO_HISTORICO", EntityName = "InfracaoHistorico", Name = "Dominio.Entidades.Embarcador.Frota.InfracaoHistorico", NameType = typeof(InfracaoHistorico))]
    public class InfracaoHistorico : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "IFH_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFH_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoInfracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoInfracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Infracao Infracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INFRACAO_HISTORICO_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IFH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InfracaoHistoricoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<InfracaoHistoricoAnexo> Anexos { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual InfracaoHistoricoAnexo Anexo
        {
            get { return Anexos.FirstOrDefault(); }
        }
    }
}
