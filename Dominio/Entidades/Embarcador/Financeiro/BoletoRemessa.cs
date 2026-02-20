using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_REMESSA", EntityName = "BoletoRemessa", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa", NameType = typeof(BoletoRemessa))]
    public class BoletoRemessa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencial", Column = "BRE_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "BRE_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "BRE_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DownloadRealizado", Column = "BRE_DOWNLOAD_REALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DownloadRealizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemessaDeCancelamento", Column = "BRE_REMESSA_DE_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemessaDeCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoConfiguracao BoletoConfiguracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroSequencial.ToString();
            }
        }

        public virtual bool Equals(BoletoRemessa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
