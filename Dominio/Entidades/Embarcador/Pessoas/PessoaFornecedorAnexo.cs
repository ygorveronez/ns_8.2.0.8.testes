namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_FORNECEDOR_ANEXO", EntityName = "PessoaFornecedorAnexo", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo", NameType = typeof(PessoaFornecedorAnexo))]
    public class PessoaFornecedorAnexo : Anexo.Anexo<ModalidadeFornecedorPessoas>
    {
        #region Propriedades Sobrescritas
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadeFornecedorPessoas", Column = "MOF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ModalidadeFornecedorPessoas EntidadeAnexo { get; set; }
        #endregion
    }
}

