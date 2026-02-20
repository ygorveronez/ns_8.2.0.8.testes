using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class ControleVisita
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime PrevisaoSaida { get; set; }
        public DateTime Saida { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public DateTime Nascimento { get; set; }
        public string Identidade { get; set; }
        public string OrgaoEmissor { get; set; }
        public string Estado { get; set; }
        public string Empresa { get; set; }
        public string Setor { get; set; }
        public string Autorizador { get; set; }
        public string PlacaVeiculo { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Observacao { get; set; }

        public string DescricaoNascimento
        {
            get
            {
                if (Nascimento != DateTime.MinValue)
                    return Nascimento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoEntrada
        {
            get
            {
                if (Entrada != DateTime.MinValue)
                    return Entrada.ToString("dd/MM/yyyy HH:MM");
                else
                    return string.Empty;
            }
        }

        public string DescricaoPrevisaoSaida
        {
            get
            {
                if (PrevisaoSaida != DateTime.MinValue)
                    return PrevisaoSaida.ToString("dd/MM/yyyy HH:MM");
                else
                    return string.Empty;
            }
        }

        public string DescricaoSaida
        {
            get
            {
                if (Saida != DateTime.MinValue)
                    return Saida.ToString("dd/MM/yyyy HH:MM");
                else
                    return string.Empty;
            }
        }
    }
}
