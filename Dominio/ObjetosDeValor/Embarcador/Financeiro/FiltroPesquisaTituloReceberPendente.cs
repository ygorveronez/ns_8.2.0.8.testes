using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaTituloReceberPendente
    {
        public DateTime DataProgramacaoPagamentoInicial { get; set; }
        public DateTime DataProgramacaoPagamentoFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int NumeroTitulo { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao? SomenteTitulosDeNegociacao { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroOcorrencia { get; set; }
        public int CodigoFatura { get; set; }
        public List<int> CodigoConhecimento { get; set; }
        public int CodigoCarga { get; set; }
        public double CNPJPessoa { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoBaixa { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public bool SelecionarTodos { get; set; }
        public List<int> CodigosTitulos { get; set; }
        public int NumeroDocumentoOriginario { get; set; }
        public string NumeroOcorrenciaCliente { get; set; }
        public decimal Valor { get; set; }
    }
}
