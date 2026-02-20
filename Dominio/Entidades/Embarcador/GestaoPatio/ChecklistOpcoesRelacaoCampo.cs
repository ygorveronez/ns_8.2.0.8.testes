namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_OPCOES_RELACAO_CAMPO", EntityName = "ChecklistOpcoesRelacaoCampo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo", NameType = typeof(ChecklistOpcoesRelacaoCampo))]
    public class ChecklistOpcoesRelacaoCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_RELACAO_CAMPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CheckListOpcaoRelacaoCampo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CheckListOpcaoRelacaoCampo CheckListOpcaoRelacaoCampo { get; set; }
    }
}
