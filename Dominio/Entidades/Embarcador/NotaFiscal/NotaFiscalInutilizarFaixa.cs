using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_INUTILIZAR_FAIXA", EntityName = "NotaFiscalInutilizarFaixa", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa", NameType = typeof(NotaFiscalInutilizarFaixa))]
    public class NotaFiscalInutilizarFaixa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicial", Column = "NFF_NUMERO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFinal", Column = "NFF_NUMERO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NFF_SERIE", TypeType = typeof(int), NotNull = false)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "NFF_JUSTIFICATIVA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "NFF_MENSAGEM_RETORNO_SEFAZ", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "NFF_CODIGO_RETORNO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoRetorno { get; set; }

        public virtual bool Equals(NotaFiscalInutilizarFaixa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
