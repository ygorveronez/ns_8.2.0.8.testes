using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESTINO_PRIORITARIO_CALCULO_FRETE", EntityName = "DestinoPrioritarioCalculoFrete", Name = "Dominio.Entidades.Embarcador.Ocorrencias.DestinoPrioritarioCalculoFrete", NameType = typeof(DestinoPrioritarioCalculoFrete))]
    public class DestinoPrioritarioCalculoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TabelasFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DESTINO_PRIORITARIO_CALCULO_FRETE_TABELA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFrete", Column = "TBF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Frete.TabelaFrete> TabelasFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Localidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DESTINO_PRIORITARIO_CALCULO_FRETE_LOCALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DestinoPrioritarioCalculoFreteLocalidade", Column = "DPL_CODIGO")]
        public virtual IList<DestinoPrioritarioCalculoFreteLocalidade> Localidades { get; set; }

        public virtual List<DestinoPrioritarioCalculoFreteLocalidade> Prioridades()
        {
            return (from o in Localidades orderby o.Ordem ascending select o).ToList();
        }

        public virtual List<DestinoPrioritarioCalculoFreteLocalidade> PrioridadesAtivas()
        {
            return (from o in Localidades where o.Ativo == true orderby o.Ordem ascending select o).ToList();
        }


        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }

}
