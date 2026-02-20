namespace Dominio.ObjetosDeValor.Embarcador.Relatorios
{
    public class ConfiguracaoRelatorioColuna
    {
        public virtual int Posicao { get; set; }

        public virtual string Titulo { get; set; }

        public virtual string Propriedade { get; set; }

        public virtual decimal Tamanho { get; set; }

        public virtual bool Visivel { get; set; }

        public virtual bool PermiteAgrupamento { get; set; }

        public virtual bool UtilizarFormatoTexto { get; set; }

        public virtual int PrecisaoDecimal { get; set; }

        public virtual string DataTypeExportacao { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento Alinhamento { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao TipoSumarizacao { get; set; }

        public virtual int CodigoDinamico { get; set; }
    }
}
