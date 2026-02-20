using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class MotoristasPrestacaoServico
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string CNH { get; set; }
        public string RegistroCNH { get; set; }
        public string CategoriaCNH { get; set; }
        public DateTime EmissaoCNH { get; set; }
        public DateTime ValidadeCNH { get; set; }
        public DateTime PrimeiraCNH { get; set; }
        public string Telefone { get; set; }
        public int CodigoPedido { get; set; }
        public Boolean UtilizaInformacoesAdicionionais { get; set; }

        public string EmissaoCNHFormatada
        {
            get { return this.EmissaoCNH.ToString("d"); }
        }

        public string ValidadeCNHFormatada
        {
            get { return this.ValidadeCNH.ToString("d"); }
        }

        public string PrimeiraCNHFormatada
        {
            get { return this.PrimeiraCNH.ToString("d"); }
        }
    }
}
