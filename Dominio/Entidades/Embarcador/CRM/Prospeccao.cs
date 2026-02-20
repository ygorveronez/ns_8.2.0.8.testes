using System;

namespace Dominio.Entidades.Embarcador.CRM
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROSPECCAO", EntityName = "Prospeccao", Name = "Dominio.Entidades.Embarcador.CRM.Prospeccao", NameType = typeof(Prospeccao))]
    public class Prospeccao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoProspect", Column = "PP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.ProdutoProspect ProdutoProspect { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteProspect", Column = "CPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.ClienteProspect Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrigemContatoClienteProspect", Column = "OCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect OrigemContatoClienteProspect { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_CNPJ", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_TIPO_CONTATO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento TipoContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_CONTATO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_EMAIL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_TELEFONE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Telefone { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATE_SATISFACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao Satisfacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_FATURADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Faturado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }


        public virtual string DescricaoFaturado
        {
            get
            {
                if (this.Faturado)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public virtual string DescricaoSatisfacao
        {
            get
            {
                switch (Satisfacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.Otimo:
                        return "Ótimo";
                    case ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.Bom:
                        return "Bom";
                    case ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.Ruim:
                        return "Ruim";
                    case ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.NaoAvaliado:
                        return "Não Avaliado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.NaoVendido:
                        return "Não Vendido";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Vendido:
                        return "Vendido";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Pendente:
                        return "Pendente";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoContato
        {
            get
            {
                switch (TipoContato)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Celular:
                        return "Celular";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.ChatWeb:
                        return "Chat Web";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Email:
                        return "Email";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Outros:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Skype:
                        return "Skype";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Telefone:
                        return "Telefone";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Nome + (this.Cidade != null ? " (" + this.Cidade.DescricaoCidadeEstado + ")" : string.Empty);
            }
        }

        public virtual string CNPJ_Formatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CNPJ) ? String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ)) : string.Empty;
            }
        }

        public virtual string CNPJ_SemFormato
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CNPJ) ? String.Format(@"{0:00000000000000}", long.Parse(this.CNPJ)) : string.Empty;
            }
        }
    }
}
