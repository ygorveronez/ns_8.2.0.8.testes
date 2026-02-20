namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODAL_TRANSPORTE", EntityName = "ModalTransporte", Name = "Dominio.Entidades.ModalTransporte", NameType = typeof(ModalTransporte))]
    public class ModalTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MOA_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MOA_NUM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MOA_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }


        //todo: armazena em tempo de exucação qual o real modal do CT-e, como não existe outro modal apto a emissão além do Rodoviario, informa aqui para ignorar a importação de CT-es com outros modais
        //quando implementar outras modalidades de modais, remover essa opção.
        public virtual int modalProcCTe { get; set; }
    }
}
