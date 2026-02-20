using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume
{
    public class RelacaoSeparacaoVolume
    {
        public string Empresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string NumeroCarga { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Motoristas { get; set; }
        public int Sequencia { get; set; }
        public string NumeroNota { get; set; }
        public string Destinatario { get; set; }
        public int Volumes { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroRemessa { get; set; }
        public string Remetente { get; set; }
        public string Rota { get; set; }
        public decimal ValorMercadorias { get; set; }
        public DateTime DataCarga { get; set; }
        public int CodigoCTe { get; set; }
    }
}
