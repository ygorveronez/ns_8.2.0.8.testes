using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Terceiros
{
    public class RomaneioEntrega
    {
        public string NomeEmpresa { get; set; }

        public string CidadeEmpresa { get; set; }
        public string UFEmpresa { get; set; }
        public string NomeMotorista { get; set; }
        public string CPFMotorista { get; set; }
        public string RGMotorista { get; set; }
        public string PlacaCarreta { get; set; }
        public string Frota { get; set; }
        public string NomeProprietario { get; set; }
        public string TipoPessoaProprietario { get; set; }
        public double CPFCNPJProprietario { get; set; }
        public string CPFCNPJProprietarioFormatado
        {
            get
            {
                if (TipoPessoaProprietario == "J")
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJProprietario);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJProprietario);
            }
        }
        public string CidadeProprietario { get; set; }
        public string UFProprietario { get; set; }
        public string EnderecoProprietario { get; set; }
        public string NumeroProprietario { get; set; }
        public string CEPProprietario { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Observacao { get; set; }
        public DateTime DataSaida { get; set; }
        public int Numero { get; set; }
        public string NumeroCarga { get; set; }

    }
}
