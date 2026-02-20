using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA", EntityName = "Rota", Name = "Dominio.Entidades.Embarcador.Logistica.Rota", NameType = typeof(Rota))]
    public class Rota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ROT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaKM", Column = "ROT_DISTANCIA_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int DistanciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaKMAPI", Column = "ROT_DISTANCIA_KM_API", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaKMAPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPedagio", Column = "ROT_POSSUI_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedagios", Column = "ROT_NUMERO_PEDADIOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroPedagios { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoViagemEmMinutos", Column = "ROT_TEMPO_VIAGEM_EM_MINUTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoViagemEmMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRotaSemParar", Column = "ROT_DESCRICAO_ROTA_SEM_PARAR", TypeType = typeof(string), Length = 64, NotNull = false)]
        public virtual string DescricaoRotaSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigosPracaSemParar", Column = "ROT_CODIGOS_PRACA_SEM_PARAR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigosPracaSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradaSemParar", Column = "ROT_INTEGRADA_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradaSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ROT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CEPs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_CEP")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaCEP", Column = "ROC_CODIGO")]
        public virtual ICollection<RotaCEP> CEPs { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Origem?.Descricao ?? string.Empty) + " - " + this.Destino?.Descricao ?? string.Empty;
            }
        }
    }
}
