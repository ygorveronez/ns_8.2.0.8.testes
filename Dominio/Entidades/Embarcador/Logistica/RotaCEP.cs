using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_CEP", EntityName = "RotaCEP", Name = "Dominio.Entidades.Embarcador.Logistica.RotaCEP", NameType = typeof(RotaCEP))]
    public class RotaCEP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ROC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPInicial", Column = "ROC_CEP_INICIAL", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPFinal", Column = "ROC_CEP_FINAL", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CEPFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Rota", Column = "ROT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Rota Rota { get; set; }

        public virtual string CEPInicialFormatado
        {
            get
            {
                return Convert.ToUInt64(this.CEPInicial).ToString(@"00\.000\-000");
            }
        }

        public virtual string CEPFinalFormatado
        {
            get
            {
                return Convert.ToUInt64(this.CEPFinal).ToString(@"00\.000\-000");
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
