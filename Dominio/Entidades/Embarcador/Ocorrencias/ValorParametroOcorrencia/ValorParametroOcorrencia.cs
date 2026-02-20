using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_OCORRENCIA", EntityName = "ValorParametroOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia", NameType = typeof(ValorParametroOcorrencia))]
    public class ValorParametroOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPO_TIPO_PESSOA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        /// <summary>
        /// Obsoleto, agora é uma lista
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPO_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime VigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPO_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime VigenciaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPO_OBSERVACAO", TypeType = typeof(string), NotNull = true, Length = 1000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroHoraExtraOcorrencia", Column = "VPH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia ValorParametroHoraExtraOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroEstadiaOcorrencia", Column = "VPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia ValorParametroEstadiaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroPernoiteOcorrencia", Column = "VPP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia ValorParametroPernoiteOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroEscoltaOcorrencia", Column = "VPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia ValorParametroEscoltaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroTipoOperacao", Column = "CTO_CODIGO")]
        public virtual IList<ValorParametroTipoOperacao> TiposOperacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                string descricao = this.VigenciaInicial.ToString("dd/MM/yyyy") + " - " + this.VigenciaFinal.ToString("dd/MM/yyyy");

                if (this.TipoPessoa == ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && this.GrupoPessoas != null)
                    return this.GrupoPessoas.Descricao + " - (" + descricao + ")";

                else if (this.TipoPessoa == ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && this.Pessoa != null)
                    return this.Pessoa.Descricao + " - (" + descricao + ")";

                else
                    return descricao;
            }
        }

        public virtual string DescricaoPessoaGrupoPessoa
        {
            get
            {
                if (this.TipoPessoa == ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && this.GrupoPessoas != null)
                    return this.GrupoPessoas.Descricao;

                else if (this.TipoPessoa == ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && this.Pessoa != null)
                    return this.Pessoa.Descricao;

                else
                    return "";
            }
        }
    }
}
