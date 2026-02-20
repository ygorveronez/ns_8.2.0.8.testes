using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AREA_VEICULO_POSICAO", EntityName = "AreaVeiculoPosicao", Name = "Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao", NameType = typeof(AreaVeiculoPosicao))]
    public class AreaVeiculoPosicao : EntidadeBase, IEquatable<AreaVeiculoPosicao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AVP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QRCode", Column = "AVP_QR_CODE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string QRCode { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculo", Column = "ARV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AreaVeiculo AreaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desenho", Column = "ARV_DESENHO", Type = "StringClob", NotNull = false)]
        public virtual string Desenho { get; set; }


        public virtual string DescricaoAcao
        {
            get { return $"{AreaVeiculo.Descricao} - {Descricao}";  }
        }

        public virtual bool Equals(AreaVeiculoPosicao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
