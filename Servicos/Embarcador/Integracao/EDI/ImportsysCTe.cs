using System;
using System.Collections.Generic;
using System.Linq;
namespace Servicos.Embarcador.Integracao.EDI
{
    public class ImportsysCTe
    {
        public Dominio.ObjetosDeValor.EDI.ImportsysCTe.ImportsysCTe ConverterPagamentoParaImportsysCTe(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            pagamentoEDI.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = pagamentoEDI.Pagamento;

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabils = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil>();
            documentoContabils.AddRange(repDocumentoContabil.BuscarCTesParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado));

            if (pagamentoEDI.Inicio > 0 || pagamentoEDI.Limite > 0)
                documentoContabils = documentoContabils.Skip(pagamentoEDI.Inicio).Take(pagamentoEDI.Limite).ToList();

            Dominio.ObjetosDeValor.EDI.ImportsysCTe.ImportsysCTe retorno = new Dominio.ObjetosDeValor.EDI.ImportsysCTe.ImportsysCTe();
            retorno.DataGeracao = DateTime.Now;
            retorno.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.ImportsysCTe.Conhecimento>();

            // Agrupando e Filtrando para pegar apenas um por carga
            var distincDocumentosContabeis = documentoContabils
                .GroupBy(p => p.NumeroCarga)
                .Select(g => g.First())
                .ToList();

            foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documento in distincDocumentosContabeis)
            {
                Dominio.ObjetosDeValor.EDI.ImportsysCTe.Conhecimento conhecimento = new Dominio.ObjetosDeValor.EDI.ImportsysCTe.Conhecimento();

                if (documento.CodigoCarga.HasValue)
                {
                    conhecimento.ProcImportacao = repPedido.BuscarProcImportacao(documento.CodigoCarga.Value);
                    conhecimento.TipoOperacao = documento.TipoOperacao;
                    conhecimento.TipoOcorrencia = documento.TipoOcorrencia ?? "";
                    conhecimento.NumeroDocumento = documento.Numero;
                    conhecimento.Serie = documento.Serie;
                    conhecimento.DataEmissao = documento.DataEmissao;
                    conhecimento.ValorAReceber = documento.ValorAReceber;
                    conhecimento.CnpjTransportador = documento.CNPJEmpresa;
                    conhecimento.NumeroCarga = documento.NumeroCarga;

                    retorno.Conhecimentos.Add(conhecimento);
                }
            }

            string extensao = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterExtencaoPadrao(pagamentoEDI.LayoutEDI);
            retorno.NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(pagamentoEDI, false, unidadeTrabalho);
            retorno.NomeArquivoSemExtencao = retorno.NomeArquivo.Replace(extensao, "");

            return retorno;
        }

    }
}
