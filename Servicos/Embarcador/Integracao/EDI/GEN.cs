using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.EDI.GEN;
using Repositorio;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class GEN
    {
        internal Dominio.ObjetosDeValor.EDI.GEN.Cabecalho ConverterOcorrenciaParaGEN(List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementos, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, UnitOfWork unidadeDeTrabalho)
        {
             return new Cabecalho {
                Registros = (
                    from complemento in complementos
                    select new Dominio.ObjetosDeValor.EDI.GEN.GEN()
                    {
                        CodigoEvento = "GEN",
                        CodigoSubEvento = "001",
                        CodigoVersao = complemento.CTe?.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Credito ? "02" : "03",
                        IdSistema = "",
                        DataGeracao = DateTime.Now,
                        UnidadeNegocio = "",
                        UnidadeNegocioFiliado = fechamentoFrete.Contrato.Filiais?.FirstOrDefault()?.Filial.CNPJ ?? "",
                        LocalidadeAtividade = "",
                        LocalidadeAtividadeFiliado = "",
                        IdDepartamento = "",
                        DataTransicao = fechamentoFrete.DataFim,
                        SerieNotaFiscal = "",
                        NumeroNotaFiscal = complemento.CTe?.Numero.ToString(),
                        IdOrigem = "",
                        DataNotaFiscal = complemento.CTe?.DataEmissao ?? fechamentoFrete.DataFechamento, //ObterDataPorPeriodoAcordoContratoFrete(fechamentoFrete, complemento.CTe?.DataEmissao, unidadeDeTrabalho),
                        DataEntrega = null,
                        DataPagamento = null,
                        IdFornecedor = "",
                        CNPJFornecedor = fechamentoFrete.Contrato?.Transportador?.CNPJ_SemFormato,
                        IdTransicaoFiscal = complemento.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? "",
                        GrupoContabil = "",
                        ValorTotal = complemento.CTe?.ValorFrete ?? 0,
                        ValorISS = complemento.CTe?.ValorISS ?? 0,
                        ValorINSSRetido = complemento.CTe?.ValorISSRetido ?? 0,
                        ValorIRRF = complemento.CTe?.ValorIR ?? 0,
                        TratamentoEspecial = "",
                        ValorDescontoCondicional = 0,
                        DescricaoCabecalho = complemento.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? "",
                        CodigoMontato = complemento.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? "",
                        ALLOC_NMBR = complemento.CargaOcorrencia?.NumeroOcorrencia.ToString(),
                        Moeda = complemento.CTe?.ValorFrete ?? 0,
                        CFOP = "",
                        CFOPEntrada = "",
                        DescricaoLinha = $"{(string.IsNullOrWhiteSpace(complemento.CargaOcorrencia?.TipoOcorrencia?.Descricao) ? "" : $"{complemento.CargaOcorrencia?.TipoOcorrencia?.Descricao} ")}{complemento.CTe?.DataEmissao?.ToString("MM-dd-yyyy") ?? ""}",
                        Nome = "",
                        CodigoPostal = "",
                        Cidade = "",
                        Pais = "",
                        PaisBanco = "",
                        CodigoBanco = "",
                        NumeroConta = "",
                        DigitoVerificador = "",
                        NumeroAgencia = "",
                        IDFiscalCNPJ = "",
                        IDFiscalCPF = "",
                        Regiao = "",
                        ChaveIntercambio = "",
                    }
                ).ToList()
            };
        }

        private DateTime? ObterDataPorPeriodoAcordoContratoFrete(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, DateTime? dataReferencia, UnitOfWork unidadeDeTrabalho)
        {
            if ((fechamentoFrete == null) || !dataReferencia.HasValue)
                return null;

            Fechamento.FechamentoFrete servicoFechamentoFrete = new Fechamento.FechamentoFrete(unidadeDeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = servicoFechamentoFrete.ObterFechamentoFretePeriodo(fechamentoFrete.Contrato.PeriodoAcordo, dataReferencia.Value);

            return fechamentoFretePeriodo.DataFim;
        }
    }
}
