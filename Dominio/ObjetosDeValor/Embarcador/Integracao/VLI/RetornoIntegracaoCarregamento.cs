using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public class RetornoIntegracaoCarregamento
    {
        public DataCarregamento Data { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Nfe
    {
        public string NumeroNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string ChaveNfe { get; set; }
        public string PesoDeclarado { get; set; }
        public string PesoRateado { get; set; }
        public string ValorNotaFiscal { get; set; }
        public string PesoBalanca { get; set; }
    }

    public class DataCarregamento
    {
        public List<ListaCarregamentoFerroviario> ListaCarregamentoFerroviario { get; set; }
    }

    public class ListaCarregamentoFerroviario
    {
        public List<Nfe> ListaNFe { get; set; }
        public string Login { get; set; }
        public string CodigoVagao { get; set; }
        public string SerieVagao { get; set; }
        public string FerroviaProprietaria { get; set; }
        public string PesoLiquido { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTara { get; set; }
        public string ChaveCte { get; set; }
        public string SerieCte { get; set; }
        public string SerieDespacho { get; set; }
        public string NumeroCte { get; set; }
        public string NumeroDespacho { get; set; }
        public string RazaoSocialRemetente { get; set; }
        public string RazaoSocialDestinatario { get; set; }
        public string CodigoTerminalTransbordo { get; set; }
        public string DescricaoTerminalTransbordo { get; set; }
        public string CnpjTerminalTransbordo { get; set; }
        public DateTime? DataCarregamento { get; set; }

        public string CNPJ { get; set; }
        public string CodigoDestino { get; set; }
        public string CodigoTerminalDestino { get; set; }
        public string NomeFantasiaTerminalDestino { get; set; }
        public string CnpjTerminalDestino { get; set; }
        public string Fluxo { get; set; }

        public string CodigoEmpresaRemetente { get; set; }
        public string CodigoEmpresaDestinataria { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }

        public string DescricaoDetalhadaProduto { get; set; }
        public string CodigoMercadoria { get; set; }
        public string DescricaoMercadoria { get; set; }
        public string DescricaoDetalhadaMercadoria { get; set; }
    }
}
