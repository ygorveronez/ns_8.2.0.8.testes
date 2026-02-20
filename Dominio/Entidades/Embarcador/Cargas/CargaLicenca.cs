using Dominio.Entidades.Embarcador.Veiculos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LICENCA", EntityName = "CargaLicenca", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLicenca", NameType = typeof(CargaLicenca))]
    public class CargaLicenca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TCL_SITUACAO", TypeType = typeof(EnumSituacaoCargaLicenca), NotNull = true)]
        public virtual EnumSituacaoCargaLicenca Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "TCL_MENSAGEM", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidacao", Column = "TCL_DATA_VALIDACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataValidacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LicencaVeiculo", Column = "VLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LicencaVeiculo LicencaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }
    }
}
