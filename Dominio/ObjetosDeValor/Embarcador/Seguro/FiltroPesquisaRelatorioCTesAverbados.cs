using System;
using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Seguro
{
    public sealed class FiltroPesquisaRelatorioCTesAverbados
    {
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int CodigoSeguradora { get; set; }
        public StatusAverbacaoCTe? Status { get; set; }
        public SituacaoAverbacaoFechamento? SituacaoFechamento { get; set; }
        public int CodigoModeloDocumentoFiscal { get; set; }
        public double CodigoClienteProvedorOS { get; set; }
        public DateTime DataServicoInicial { get; set; }
        public DateTime DataServicoFinal { get; set; }
        public string TipoPropriedadeVeiculo { get; set; }
        public List<int> GrupoTomador { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
    }
}
