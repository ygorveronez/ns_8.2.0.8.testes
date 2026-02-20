using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_DOCUMENTO_DESTINADO_EMPRESA", EntityName = "ConfiguracaoDocumentoDestinadoEmpresa", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa", NameType = typeof(ConfiguracaoDocumentoDestinadoEmpresa))]
    public class ConfiguracaoDocumentoDestinadoEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoNSU", Column = "CDD_ULTIMO_NSU", TypeType = typeof(long), NotNull = true)]
        public virtual long UltimoNSU { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_DATA_ULTIMA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimaConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_EM_CONSULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloDocumento", Column = "CDD_MODELO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado ModeloDocumento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Documento Destinado da Empresa";
            }
        }
    }
}
