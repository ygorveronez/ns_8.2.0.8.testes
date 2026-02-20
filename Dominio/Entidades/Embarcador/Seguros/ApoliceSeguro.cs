using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APOLICE_SEGURO_GERAL", EntityName = "ApoliceSeguro", Name = "Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro", NameType = typeof(ApoliceSeguro))]
    public class ApoliceSeguro : EntidadeBase
    {
        public ApoliceSeguro()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "APS_NUMERO_APOLICE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "APS_NUMERO_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioVigencia", Column = "APS_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime InicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FimVigencia", Column = "APS_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime FimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAlerta", Column = "APS_DATA_ULTIMO_ALERTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "APS_RESPONSAVEL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APS_SEGURADORA_AVERBACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao SeguradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Seguradora", Column = "SEA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.Seguradora Seguradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimiteApolice", Column = "APS_VALOR_LIMITE_APOLICE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimiteApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixoAverbacao", Column = "APS_VALOR_FIXO_AVERBACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixoAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "APS_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoApolice", Column = "APS_DESCRICAO_APOLICE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string DescricaoApolice { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Descontos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_APOLICE_SEGURO_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "APS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ApoliceSeguroDesconto", Column = "APD_CODIGO")]
        public virtual ICollection<ApoliceSeguroDesconto> Descontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativa", Column = "APS_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroApolice;
            }
        }

        public virtual string DescricaoResponsavel
        {
            get
            {
                switch (Responsavel)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador:
                        return "Embarcador";
                    case ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Transportador:
                        return "Transportador";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoComSeguradora
        {
            get { return $"{NumeroApolice} - {Seguradora.Nome}"; }
        }

        public virtual string DescricaoSeguradoraAverbacao
        {
            get
            {
                switch (SeguradoraAverbacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM:
                        return "ATM";
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco:
                        return "Bradesco";
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ELT:
                        return "ELT";
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro:
                        return "Porto Seguro";
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Senig:
                        return "Senig";
                    case ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido:
                        return "Não Definido";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoAtiva
        {
            get { return ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisaHelper.ObterDescricao(this.Ativa); }
        }
    }
}
