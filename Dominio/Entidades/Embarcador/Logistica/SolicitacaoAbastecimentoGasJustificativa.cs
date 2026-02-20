namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_ABASTECIMENTO_GAS_JUSTIFICATIVA", EntityName = "SolicitacaoAbastecimentoGasJustificativa", Name = "Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGasJustificativa", NameType = typeof(SolicitacaoAbastecimentoGasJustificativa))]
    public class SolicitacaoAbastecimentoGasJustificativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AGJ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "AGJ_JUSTIFICATIVA", TypeType = typeof(string), NotNull = true, Length = 300)]
        public virtual string Justificativa { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return Justificativa;
            }
        }
    }
}
