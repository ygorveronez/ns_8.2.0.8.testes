using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class GuaritaCheckList
    {
        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public string Carga { get; set; }
        public int OrdemServico { get; set; }
        public string Motorista { get; set; }
        public string Operador { get; set; }
        public string TipoCheck { get; set; }
        public int KMAtual { get; set; }
        public DateTime DataAbertura { get; set; }
        public string Tipo { get; set; }
        public string Vistoria { get; set; }
        public string Croquis { get; set; }
        public string Observacao { get; set; }
        public string CPF { get; set; }
        public string RG{ get; set; }

    }
}
