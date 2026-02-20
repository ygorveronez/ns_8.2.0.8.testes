namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_LAVACAO_ANEXO", EntityName = "VeiculoLavacaoAnexo", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo", NameType = typeof(VeiculoLavacaoAnexo))]
    public class VeiculoLavacaoAnexo : Anexo.Anexo<VeiculoLavacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VeiculoLavacao", Column = "VEL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override VeiculoLavacao EntidadeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAnexoLavacao", Column = "VLA_TIPO_ANEXO_LAVACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo TipoAnexoLavacao { get; set; }

        #endregion
    }
}