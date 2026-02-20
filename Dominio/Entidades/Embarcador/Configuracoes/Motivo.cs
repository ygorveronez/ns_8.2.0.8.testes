using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO", EntityName = "Motivo", Name = "Dominio.Entidades.Embarcador.Configuracoes.Motivo", NameType = typeof(Motivo))]
    public class Motivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotivo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotivo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                return Tipo.ObterDescricao();
            }

        }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo; }
        }
    }
}
