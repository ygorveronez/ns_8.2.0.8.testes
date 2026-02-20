namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_JUSTIFICATIVA_TEMPERATURA", EntityName = "JustificativaTemperatura", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura", NameType = typeof(JustificativaTemperatura))]
    public class JustificativaTemperatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CJT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CJT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CJT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
