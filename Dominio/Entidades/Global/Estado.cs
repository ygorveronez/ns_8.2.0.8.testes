using Dominio.Entidades.Embarcador.Localidades;
using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_UF", EntityName = "Estado", Name = "Dominio.Entidades.Estado", NameType = typeof(Estado))]
    public class Estado : EntidadeBase, IEquatable<Estado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Sigla", Type = "System.String", Length = 2, Column = "UF_SIGLA")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "assigned")]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "UF_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEstado", Column = "UF_CODIGO_ESTADO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "UF_DATAATU", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIBGE", Column = "UF_IBGE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIBGE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "UF_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Abreviacao", Column = "UF_ABREVIACAO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Abreviacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "UF_HABILITAR_CONTINGENCIA_EPEC_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarContingenciaEPECAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtivacaoContingenciaAutomatica", Column = "UF_DATA_ATIVACAO_CONTINGENCIA_AUTOMATICA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtivacaoContingenciaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        /// <summary>
        /// 1 - Normal
        /// 4 - EPEC pela SVC
        /// 5 - Contingência FSDA
        /// 7 - SVC-RS
        /// 8 - SVC-SP
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissao", Column = "UF_TIPO_EMISSAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoEmissao { get; set; }

        /// <summary>
        /// 1 - Normal
        /// 5 - Contingência MDFE
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoMDFe", Column = "UF_TIPO_EMISSAO_MDFE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoMDFe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoMDFe TipoEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Sefaz", Column = "UF_SEFAZ_CTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Sefaz SefazCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Sefaz", Column = "UF_SEFAZ_CTE_HOMOLOGACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Sefaz SefazCTeHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Sefaz", Column = "UF_SEFAZ_MDFE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Sefaz SefazMDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Sefaz", Column = "UF_SEFAZ_MDFE_HOMOLOGACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Sefaz SefazMDFeHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegiaoBrasil", Column = "RBR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegiaoBrasil RegiaoBrasil { get; set; }

        public virtual string Descricao
        {
            get { return this.Sigla; }
        }

        public virtual int Codigo
        {
            get { return this.CodigoIBGE; }
        }

        public virtual string DescricaoTipoEmissao
        {
            get
            {
                switch (TipoEmissao)
                {
                    case "1":
                        return "1 - Normal";
                    case "4":
                        return "4 - EPEC";
                    case "5":
                        return "5 - Contingência FSDA";
                    case "7":
                        return "7 - SVC-RS";
                    case "8":
                        return "8 - SVC-SP";
                    default:
                        return "Normal";
                }
            }
        }

        public virtual bool Equals(Estado other)
        {
            if (other.Sigla != this.Sigla)
                return false;
            else
                return true;
        }
    }
}
