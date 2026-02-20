namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_PALLETS", EntityName = "PalletTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete", NameType = typeof(PalletTabelaFrete))]
    public class PalletTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFP_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicialPallet", Column = "TFP_NUMERO_INICIAL_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroInicialPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFinalPallet", Column = "TFP_NUMERO_FINAL_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFinalPallet { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete.PorFaixaPallets)
                {
                    if (NumeroInicialPallet.HasValue && NumeroFinalPallet.HasValue && NumeroInicialPallet.Value > 0 && NumeroFinalPallet.Value > 0)
                        return "De " + NumeroInicialPallet.Value.ToString() + " à " + NumeroFinalPallet.Value.ToString() + " pallets";
                    else if (!NumeroFinalPallet.HasValue || NumeroFinalPallet.Value <= 0)
                        return "À partir de " + NumeroInicialPallet.Value.ToString() + " pallets";
                    else
                        return "Até " + NumeroFinalPallet.Value.ToString() + " pallets";

                }
                else
                {
                    return "Valor fixo por pallet";
                }
            }
        }
    }
}
