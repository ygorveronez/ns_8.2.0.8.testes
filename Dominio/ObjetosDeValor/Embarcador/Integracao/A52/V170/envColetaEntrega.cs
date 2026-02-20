using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envColetaEntrega
    {
        public string identificador { get; set; }

        public int? idOperacaoLogistico { get; set; }

        public int? idRota { get; set; }

        public envColetaEntregaOperacaoLogistico operacaoLogistico { get; set; }

        public int? tipo { get; set; }

        public int? tipoTransporte { get; set; }

        public string dataChegadaOrigem { get; set; }

        public string dataPrevisaoChegadaOrigem { get; set; }

        public string dataSaidaOrigem { get; set; }

        public string dataPrevisaoSaidaOrigem { get; set; }

        public string dataPrevisaoChegadaDestino { get; set; }

        public string dataChegadaDestino { get; set; }

        public string dataPrevisaoSaidaDestino { get; set; }

        public string dataSaidaDestino { get; set; }

        public int? idClienteOrigem { get; set; }

        public envColetaEntregaCliente clienteOrigem { get; set; }

        public int? idClienteOrigemEndereco { get; set; }

        public envColetaEntregaEnderecoCliente enderecoClienteOrigem { get; set; }

        public int? idClienteDestino { get; set; }

        public envColetaEntregaCliente clienteDestino { get; set; }

        public int? idClienteDestinoEndereco { get; set; }

        public envColetaEntregaEnderecoCliente enderecoClienteDestino { get; set; }

        public int? ordem { get; set; }

        public bool ativo { get; set; }

        public List<envColetaEntregaDocumentos> documentos { get; set; }

        public envColetaEntregaConjuntos conjuntos { get; set; }
    }

    public class envColetaEntregaOperacaoLogistico
    {
        public string descricao { get; set; }

        public string identificador { get; set; }

        public bool ativo { get; set; }

        public int? tempoDescarga { get; set; }

        public decimal? valorDiaria { get; set; }
    }

    public class envColetaEntregaCliente
    {
        public string nomeFantasia { get; set; }

        public string razaoSocial { get; set; }

        public string cnpj { get; set; }

        public string identificador { get; set; }

        public string sigla { get; set; }

        public bool ativo { get; set; }
    }

    public class envColetaEntregaEnderecoCliente
    {
        public string identificador { get; set; }

        public string sigla { get; set; }

        public int? tipo { get; set; }

        public string cep { get; set; }

        public string logradouro { get; set; }

        public string numero { get; set; }

        public string complemento { get; set; }

        public string bairro { get; set; }

        public int? idCidade { get; set; }

        public int? idPais { get; set; }

        public List<envColetaEntregaEnderecoClientePoligono> poligono { get; set; }

        public envColetaEntregaEnderecoClienteRaio raio { get; set; }
    }

    public class envColetaEntregaEnderecoClientePoligono
    {
        public decimal latitude { get; set; }

        public decimal longitude { get; set; }
    }

    public class envColetaEntregaEnderecoClienteRaio
    {
        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public int? raio { get; set; }
    }

    public class envColetaEntregaDocumentos
    {
        public string chaveCte { get; set; }

        public string chaveNf { get; set; }

        public string produto { get; set; }

        public decimal? peso { get; set; }

        public decimal? volumes { get; set; }

        public string identificador { get; set; }

        public int? numeroCte { get; set; }

        public string numeroNf { get; set; }

        public int? serieCte { get; set; }

        public int? serieNf { get; set; }

        public string dataEmissaoNf { get; set; }

        public decimal? valorCarga { get; set; }

        public decimal? valorICMS { get; set; }

        public decimal? freteLiquido { get; set; }

        public decimal? freteBruto { get; set; }

        public bool ativo { get; set; }
    }

    public class envColetaEntregaConjuntos
    {
        public int? idVeiculo { get; set; }

        public int? idCarreta1 { get; set; }

        public int? idCarreta2 { get; set; }

        public int? idMotorista { get; set; }
    }
}