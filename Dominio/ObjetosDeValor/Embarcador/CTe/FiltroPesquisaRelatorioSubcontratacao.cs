using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioSubcontratacao
    {
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialEmissaoCarga { get; set; }
        public DateTime DataFinalEmissaoCarga { get; set; }
        public DateTime DataInicialFinalizacaoEmissao { get; set; }
        public DateTime DataFinalFinalizacaoEmissao { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosCarga { get; set; }
        public List<int> CodigosOrigem { get; set; }
        public List<int> CodigosDestino { get; set; }
        public List<int> CodigosGrupoPessoas { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<double> CpfCnpjsRemetente { get; set; }
        public List<double> CpfCnpjsDestinatario { get; set; }
        public List<string> EstadosOrigem { get; set; }
        public List<string> EstadosDestino { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public List<int> TiposServicos { get; set; }
        public List<Dominio.Enumeradores.TipoCTE> TiposCTe { get; set; }
        public List<SituacaoCarga> SituacaoCarga { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }
        public List<SituacaoCTeSefaz> SituacaoSEFAZ { get; set; }
    }
}
