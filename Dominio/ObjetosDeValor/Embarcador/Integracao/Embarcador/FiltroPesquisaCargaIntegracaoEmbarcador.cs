using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador
{
    public class FiltroPesquisaCargaIntegracaoEmbarcador
    {
        public List<int> CodigoEmpresa { get; set; }
        public List<int> CodigoVeiculo { get; set; }
        public List<int> CodigoMotorista { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador> Situacao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> SituacaoCarga { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga> SituacaoCancelamento { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public DateTime? DataInicialCarga { get; set; }
        public DateTime? DataFinalCarga { get; set; }
        public int? NumeroCTe { get; set; }
        public int? NumeroMDFe { get; set; }
        public List<int> CodigoTipoOperacao { get; set; }
    }
}
