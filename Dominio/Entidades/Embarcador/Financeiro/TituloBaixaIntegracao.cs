using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_INTEGRACAO", EntityName = "TituloBaixaIntegracao", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao", NameType = typeof(TituloBaixaIntegracao))]
    public class TituloBaixaIntegracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixa TituloBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoTituloBaixa", Column = "TBI_TIPO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoTituloBaixa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoTituloBaixa TipoIntegracaoTituloBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "TBI_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destinatarios", Column = "TBI_DESTINATARIOS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "TBI_ASSUNTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "TBI_MENSAGEM", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Mensagem { get; set; }

        public virtual string DescricaoTipoIntegracaoTituloBaixa
        {
            get
            {
                switch (this.TipoIntegracaoTituloBaixa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoTituloBaixa.Email:
                        return "E-mail";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return "Integração - " + this.DescricaoTipoIntegracaoTituloBaixa;
            }
        }

        public virtual bool Equals(TituloBaixaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
