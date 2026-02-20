using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class ParametrosRetornarTabelasValidas
    {
        public int CodigoCanalEntrega { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoGrupoPessoaTomador { get; set; }

        public int CodigoModeloVeicularDaCarga { get; set; }

        public int CodigoModeloVeicularDoVeiculo { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOcorrencia { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoRotaFrete { get; set; }

        public int CodigoVeiculo { get; set; }

        public List<int> CodigosReboque { get; set; }

        public double CpfCnpjTerceiro { get; set; }

        public decimal Pallets { get; set; }

        public double CpfCnpjTomador { get; set; }
        public double CodigoFronteira { get; set; }

        public DateTime? DataCarregamento { get; set; }

        public DateTime? DataCriacaoCarga { get; set; }

        public DateTime? DataVigencia { get; set; }

        public LocalFreeTime? LocalFreeTime { get; set; }

        public bool PagamentoTerceiro { get; set; }

        public bool RetornarPrimeiraValida { get; set; }

        public Entidades.Embarcador.Cargas.Retornos.RetornoCarga RetornoCarga { get; set; }
        public bool CalcularVariacoes { get; set; }
        public int CodigoContratoFreteCliente { get; set; }
    }
}
