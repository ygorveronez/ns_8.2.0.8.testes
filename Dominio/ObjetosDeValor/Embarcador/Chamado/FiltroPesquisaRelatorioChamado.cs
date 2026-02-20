using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public sealed class FiltroPesquisaRelatorioChamado
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoResponsavel { get; set; }
        public SituacaoChamado Situacao { get; set; }
        public int CodigoFilial { get; set; }
        public List<int> CodigosMotivo { get; set; }
        public double CpfCnpjTomador { get; set; }
        public double CpfCnpjCliente { get; set; }
        public double CpfCnpjDestinatario { get; set; }
        public int CodigoGrupoPessoasCliente { get; set; }
        public int CodigoGrupoPessoasTomador { get; set; }
        public int CodigoGrupoPessoasDestinatario { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoRepresentante { get; set; }
        public int Nota { get; set; }
        public int NumeroCTe { get; set; }
        public string Placa { get; set; }
        public string Carga { get; set; }
        public bool GerouOcorrencia { get; set; }
        public DateTime DataCriacaoInicio { get; set; }
        public DateTime DataCriacaoFim { get; set; }
        public DateTime DataFinalizacaoInicio { get; set; }
        public DateTime DataFinalizacaoFim { get; set; }
        public int CodigoFilialVenda { get; set; }
        public DateTime DataInicialChegadaDiaria { get; set; }
        public DateTime DataFinalChegadaDiaria { get; set; }
        public DateTime DataInicialSaidaDiaria { get; set; }
        public DateTime DataFinalSaidaDiaria { get; set; }
        public List<double> CpfCnpjClienteResponsavel { get; set; }
        public List<int> CodigosGrupoPessoasResponsavel { get; set; }
        public List<int> Filiais { get; set; }
        public List<double> Recebedores { get; set; }
        public int CodigoVeiculo { get; set; }
        public bool SomenteAtendimentoEstornado { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public bool? PossuiAnexoNFSe { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }
    }
}
