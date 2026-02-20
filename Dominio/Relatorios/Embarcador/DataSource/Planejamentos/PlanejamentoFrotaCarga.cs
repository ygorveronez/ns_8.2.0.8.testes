using Dominio.ObjetosDeValor.Embarcador.Frota;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.Planejamentos
{
    public class PlanejamentoFrotaCarga
    {
        public PlanejamentoFrotaCarga()
        {
            LogoGrupoPessoas = string.Empty;
        }

        public int Codigo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string ModeloVeicular { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public string LogoGrupoPessoas { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Peso { get; set; }

        private DateTime DataCarregamento { get; set; }
        private DateTime DataInicioCarregamento { get; set; }
        private DateTime DataFimCarregamento { get; set; }

        public string DadosPlanejamentoCarga { set { if (value != null) PlanejamentoCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlanejamentoCarga>>(value); } }
        public List<PlanejamentoCarga> PlanejamentoCarga { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota? SituacaoComprometimentoCarga
        {
            get
            {
                if (PlanejamentoCarga != null && PlanejamentoCarga.Where(x => x.SituacaoComprometimentoFrota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota.VeiculoAlteradoDeTrechosAnteriores).Any())
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota.VeiculoAlteradoDeTrechosAnteriores;
                else
                    return null;
            }
        }

        public string OrigemEDestino
        {
            get { return Origem + " para " + Destino; }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento.ToDateTimeString(); }
        }

        public string DataInicioCarregamentoFormatada
        {
            get { return DataInicioCarregamento != DateTime.MinValue ? DataInicioCarregamento.ToDateTimeString() : DataCarregamento.ToDateTimeString(); }
        }

        public string DataFimCarregamentoFormatada
        {
            get { return DataFimCarregamento != DateTime.MinValue ? DataFimCarregamento.ToDateTimeString() : ""; }
        }

    }
}
