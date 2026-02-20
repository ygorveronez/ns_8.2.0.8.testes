using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public sealed class FiltroPesquisaRelatorioNotas
    {
        public string EstadoEmitente { get; set; }
        public string NumeroModeloNF { get; set; }
        public int CodigoEmpresaFilial { get; set; }
        public int CodigoVeiculo { get; set; }
        public SituacaoDocumentoEntrada StatusNotaEntrada { get; set; }
        public StatusTitulo SituacaoFinanceiraNotaEntrada { get; set; }
        public int CodigoEmpresa { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int Serie { get; set; }
        public List<int> CodigosNaturezaOperacao { get; set; }
        public int CodigoModelo { get; set; }
        public double CnpjPessoa { get; set; }
        public string Chave { get; set; }
        public TipoEntradaSaida TipoMovimento { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public int CodigoSegmento { get; set; }
        public List<int> CodigosModeloDocumentoFiscal { get; set; }
        public DateTime DataEntradaInicial { get; set; }
        public DateTime DataEntradaFinal { get; set; }
        public int CodigoEquipamento { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
        public int OperadorLancamentoDocumento { get; set; }
        public int OperadorFinalizouDocumento { get; set; }
        public DateTime DataInicialFinalizacao { get; set; }
        public DateTime DataFinalFinalizacao { get; set; }
        public int DocFinalizadoAutomaticamente { get; set; }
        public int Categoria { get; set; }


    }
}
