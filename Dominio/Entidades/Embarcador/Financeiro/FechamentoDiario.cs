using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_DIARIO", EntityName = "FechamentoDiario", Name = "Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario", NameType = typeof(FechamentoDiario))]
    public class FechamentoDiario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Data do fechamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFechamento { get; set; }

        /// <summary>
        /// Data em que foi gerado o fechamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDE_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearApenasDocumentoEntrada", Column = "FDI_BLOQUEAR_APENAS_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearApenasDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get { return this.DataFechamento.ToString("dd/MM/yyyy"); }
        }
    }
}
