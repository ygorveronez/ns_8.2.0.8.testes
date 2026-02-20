using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEILAO_TIPO_OPERACAO_CONFIGURACAO", EntityName = "LeilaoTipoOperacaoConfiguracao", Name = "Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao", NameType = typeof(LeilaoTipoOperacaoConfiguracao))]
    public class LeilaoTipoOperacaoConfiguracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoEmissao", Column = "LTO_TIPO_OPERACAO_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao TipoOperacaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteLeilao", Column = "LTO_PERMITE_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteTempoLeilaoEmHoras", Column = "LTO_PREVISAO_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteTempoLeilaoEmHoras { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissaoDescricao.RetornarDescricao(this.TipoOperacaoEmissao);
            }
        }

        public virtual bool Equals(LeilaoTipoOperacaoConfiguracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
