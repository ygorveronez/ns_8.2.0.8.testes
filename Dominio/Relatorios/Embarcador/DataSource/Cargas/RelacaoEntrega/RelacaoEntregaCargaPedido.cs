using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega
{
    public class RelacaoEntregaCargaPedido
    {
        public string NomeEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoCarga { get; set; }
        public string PlacaVeiculo { get; set; }
        public string NumeroFrotaVeiculo { get; set; }
        public double CapacidadeVeiculo { get; set; }
        public decimal QtdPeso { get; set; }
        public decimal ValorNotas { get; set; }
        public decimal ValorFreteSemICMS { get; set; }
        public double CNPJProprietario { get; set; }
        public string NomeProprietario { get; set; }
        public string ANTTEmpresa { get; set; }
        public string Rota { get; set; }
        private DateTime DataFinalizacaoEmissao { get; set; }
        public string DataFinalizacaoEmissaoFormatada
        {
            get { return DataFinalizacaoEmissao != DateTime.MinValue ? DataFinalizacaoEmissao.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); }
        }
        public string NomeRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string CEPRemetente { get; set; }
        public double CNPJRemetente { get; set; }
        public string IERemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string EstadoRemetente { get; set; }
        public string Motoristas { get; set; }
        public string CargasAgrupadas { get; set; }
        public string NumeroPedido { get; set; }


        //Junção com documentos
        public int CodigoCTe { get; set; }
        public string NumeroCTe { get; set; }
        public string NomeDestinatario { get; set; }
        public decimal QtdVolume { get; set; }

        public string NotasRevenda { get; set; }
        public string NotasNaoRevenda { get; set; }
        public string NotasNFEletronicos { get; set; }
        public int QtdNotasRevenda { get; set; }
        public int QtdNotasNaoRevenda { get; set; }
        public int QtdNotasNFEletronicos { get; set; }

        public string EnderecoDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string NumeroDestinatario { get; set; }
        public string ComplementoDestinatario { get; set; }
        public string CEPDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string EstadoDestinatario { get; set; }
        public string FoneDestinatario { get; set; }
        public string EmpresaFilial { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal ValorFrete { get; set; }
    }
}
