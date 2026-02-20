namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class GraficoDirecionamentoOperador
    {
        public int Codigo { get; set; }

        public int Quantidade { get; set; }

        public string Descricao { get; set; }

        public virtual string Nome { get; set; }

        public virtual string CNPJ { get; set; }

        public virtual string CNPJFormatado
        {
            get
            {
                return string.Format(@"{0:00\.000\.000\/0000\-00}", (!string.IsNullOrWhiteSpace(this.CNPJ) ? long.Parse(Utilidades.String.OnlyNumbers(this.CNPJ)) : 0));
            }
        }

        public int QuantidadeDirecionada { get; set; }

        public int QuantidadeRejeitada { get; set; }
    }
}
