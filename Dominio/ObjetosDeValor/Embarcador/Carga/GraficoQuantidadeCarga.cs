using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class GraficoQuantidadeCarga
    {
        public int Codigo { get; set; }

        public virtual int Quantidade { get; set; }

        public virtual string Descricao { get; set; }

        public virtual string Nome { get; set; }

        public virtual double CPFCNPJ { get; set; }

        public virtual string TipoPessoa { get; set; }

        public virtual string NumeroCarga { get; set; }

        public virtual decimal ValorFrete { get; set; }

        public virtual DateTime DataCarregamentoProgramada { get; set; }

        public virtual DateTime InicioCarregamento { get; set; }

        public virtual int DiasAtraso
        {
            get
            {
                int dias = (InicioCarregamento - DataCarregamentoProgramada).Days;

                if (dias < 0)
                    return 0;

                return dias;
            }
        }

        public virtual string CPFCNPJFormatado
        {
            get
            {
                return this.TipoPessoa == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJ) : String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJ);
            }
        }

        public virtual TipoQuantidadeCarga Tipo { get; set; }

        public virtual int Ordem
        {
            get { return Tipo.ObterOrdem(); }
        }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }

        public virtual string Cor
        {
            get { return Tipo.ObterCor(); }
        }
    }
}
