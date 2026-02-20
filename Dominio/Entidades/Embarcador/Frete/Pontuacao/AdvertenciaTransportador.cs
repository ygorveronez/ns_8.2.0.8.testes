using System;

namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ADVERTENCIA_TRANSPORTADOR", EntityName = "AdvertenciaTransportador", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador", NameType = typeof(AdvertenciaTransportador))]
    public class AdvertenciaTransportador : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ATR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ATR_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "ATR_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAdvertenciaTransportador", Column = "MAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAdvertenciaTransportador Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public override int PontuacaoConvertida
        {
            get { return -Pontuacao; }
        }

        public override string Descricao
        {
            get { return $"Advertência para o transportador. Motivo: {Motivo.Descricao.Substring(0, Math.Min(100, Motivo.Descricao.Length))}";  }
        }

        public virtual bool Equals(AdvertenciaTransportador other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual string CnpjTransportador
        {
            get
            {
                return this.Transportador.CNPJ_Formatado;
            }
        }
    }
}
