
namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTESTACAO_ANEXOS", EntityName = "ContestacaoAnexo", Name = "Dominio.Entidades.Embarcador.Pallets.ContestacaoAnexo", NameType = typeof(ContestacaoAnexo))]

    public class ContestacaoAnexo : Anexo.Anexo<DevolucaoPallet> 
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override DevolucaoPallet EntidadeAnexo { get; set; }

        #endregion
    }
}

