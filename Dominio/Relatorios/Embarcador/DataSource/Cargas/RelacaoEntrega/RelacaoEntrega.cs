using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega
{
    public class RelacaoEntrega
    {
        public string NomeEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoCarga { get; set; }
        public string PlacaVeiculo { get; set; }
        public string NumeroFrotaVeiculo { get; set; }
        public double CapacidadeVeiculo { get; set; }
        public int ContemReboque { get; set; }
        public int ContemMotoristas { get; set; }
        public int QtdCTe { get; set; }
        public int QtdNotas { get; set; }
        public decimal QtdVolumes { get; set; }
        public decimal QtdPeso { get; set; }
        public decimal ValorNotas { get; set; }
        public decimal ValorFreteSemICMS { get; set; }
        public double CNPJProprietario { get; set; }
        public string NomeProprietario { get; set; }
        public string ANTTEmpresa { get; set; }
        public byte[] CodigoBarrasViagem { get; set; }
        public string CIOT { get; set; }
        public string Rota { get; set; }
        public DateTime DataFinalizacaoEmissao { get; set; }
        public string NomeRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string CEPRemetente { get; set; }
        public double CNPJRemetente { get; set; }
        public string IERemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string EstadoRemetente { get; set; }
    }
}
