namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_PACOTE", EntityName = "PacoteTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete", NameType = typeof(PacoteTabelaFrete))]
    public class PacoteTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFP_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicialPacote", Column = "TFP_NUMERO_INICIAL_PACOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroInicialPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFinalPacote", Column = "TFP_NUMERO_FINAL_PACOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFinalPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_COM_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComAjudante { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete.PorFaixaPacote)
                {
                    string comAjudante = "";
                    if (ComAjudante)
                        comAjudante = " (Com Ajudante)";

                    if (NumeroInicialPacote.HasValue && NumeroFinalPacote.HasValue && NumeroInicialPacote.Value > 0 && NumeroFinalPacote.Value > 0)
                        return "De " + NumeroInicialPacote.Value.ToString() + " à " + NumeroFinalPacote.Value.ToString() + " pacotes" + comAjudante;
                    else if (!NumeroFinalPacote.HasValue || NumeroFinalPacote.Value <= 0)
                        return "Acima de " + NumeroInicialPacote.Value.ToString() + " pacotes" + comAjudante;
                    else
                        return "Até " + NumeroFinalPacote.Value.ToString() + " pacotes" + comAjudante;
                }
                else
                {
                    return "Valor fixo por pacote";
                }
            }
        }
    }
}
