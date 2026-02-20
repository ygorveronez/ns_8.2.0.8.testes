using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas
    {
        public List<int> CodigosGruposPessoas { get; set; }
        public List<int> CodigosModeloDocumentoFiscal { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialAutorizacao { get; set; }
        public DateTime DataFinalAutorizacao { get; set; }
        public string PropriedadeVeiculo { get; set; }
        public bool? SomenteCTesDeMinutas { get; set; }
        public bool? DocumentoFaturavel { get; set; }
        public bool? VinculoCarga { get; set; }
        public TipoAmbiente? TipoAmbiente { get; set; }
        public TipoPropostaMultimodal? TipoProposta { get; set; }
    }
}
