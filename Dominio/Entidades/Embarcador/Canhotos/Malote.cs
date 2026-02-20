using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MALOTE_CANHOTO", EntityName = "Malote", Name = "Dominio.Entidades.Embarcador.Canhotos.Malote", NameType = typeof(Malote))]
    public class Malote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_PROTOCOLO", TypeType = typeof(int), NotNull = true)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_DATA_MALOTE", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_NOME_OPERADOR", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeOperador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_QUANTIDADE_CANHOTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_MOTIVO_INCONSISTENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoInconsistencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Canhotos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MALOTE_CANHOTO_CANHOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MaloteCanhoto", Column = "MCC_CODIGO")]
        public virtual ICollection<MaloteCanhoto> Canhotos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return this.Situacao.Descricao();
            }
        }
    }
}
