using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioManifestosEmitidosAvon
    {
        public int CodigoManifesto { get; set; }

        public string NumeroManifesto { get; set; }

        public int NumeroFatura { get; set; }

        public string NumeroFrota { get; set; }

        public int QuantidadeCTes { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ValorAReceber { get; set; }

        public string Placa { get; set; }

        public string Proprietario { get; set; }

        public string Motorista { get; set; }

        public string CidadeDestino { get; set; }

        public string UFDestino { get; set; }

        public DateTime DataEmissao { get; set; }

        public int NumeroInicialCTe { get; set; }

        public int NumeroFinalCTe { get; set; }

        public decimal PesoCargaCTe { get; set; }

        public decimal ValorMercadoria { get; set; }
    }
}
