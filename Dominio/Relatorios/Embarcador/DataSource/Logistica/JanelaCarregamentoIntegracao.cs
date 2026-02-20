using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class JanelaCarregamentoIntegracao
    {
        #region Propriedades

        public int Codigo { get; set; }
        private DateTime DataCriacao { get; set; }
        public string Carga { get; set; }
        public string Protocolo { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string Veiculos { get; set; }
        private bool ExecutouRotaTres { get; set; }

        #endregion

        #region Propriedades com regras

        public string ExecutouRotaTresFormatado
        {
            get { return ExecutouRotaTres ? "Sim" : "Não"; }
        }
        public string DataCriacaoFormatada
        {
            get
            {
                return DataCriacao != DateTime.MinValue ? DataCriacao.ToString("dd/MM/yyyy") : "";
            }
        }

        public string CPFMotoristaFormatado
        {
            get
            {
                return CPFMotorista.ObterCpfFormatado();
            }
        }

        #endregion
    }


}
