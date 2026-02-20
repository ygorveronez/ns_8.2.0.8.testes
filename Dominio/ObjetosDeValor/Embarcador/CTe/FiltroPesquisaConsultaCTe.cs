using System;
using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaConsultaCTe
    {
        public FiltroPesquisaConsultaCTe()
        {
            TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.Todos;
        }

        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int Serie { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CpfCnpjRemetente { get; set; }
        public string CpfCnpjDestinatario { get; set; }
        public string CpfCnpjTomador { get; set; }
        public string CpfCnpjTransportadorTerceiro { get; set; }
        public List<string> StatusCTe { get; set; }
        public string Chave { get; set; }
        public string DescricaoConsulta { get; set; }
        public List<int> CodigosFatura { get; set; }
        public int CodigoModeloDocumento { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public int CodigoTransportador { get; set; }
        public bool? VinculoCarga { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public int NumeroSerie { get; set; }
        public string NumeroControleCliente { get; set; }
        public string NumeroControle { get; set; }
        public List<TipoPropostaMultimodal> TipoProposta { get; set; }
        public List<Dominio.Enumeradores.TipoServico> TipoServico { get; set; }
        public bool? Documento { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoViagem { get; set; }
        public int CodigoViagemTransbordo { get; set; }
        public int CodigoPortoTransbordo { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public List<string> CpfCnpjTomadores { get; set; }
        public string Placa { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        public List<TipoServicoMultimodal> TipoServicoCarga { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa VeioPorImportacao { get; set; }
        public bool SomenteCTeSubstituido { get; set; }
        public Dominio.Enumeradores.TipoDocumento TipoDocumentoEmissao { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public List<int> CodigosCTe { get; set; }
        public int CodigoConciliacaoTransportador { get; set; }
        public TipoPagamento? TipoPagamento { get; set; }
    }
}
