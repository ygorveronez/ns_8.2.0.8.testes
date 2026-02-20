using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class ImportsysVP
    {
        public Dominio.ObjetosDeValor.EDI.ImportsysVP.ImportsysVP ConverterPagamentoParaImportsysVP(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            pagamentoEDI.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = pagamentoEDI.Pagamento;

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabils = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil>();
            documentoContabils.AddRange(repDocumentoContabil.BuscarCTesParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado));

            if (pagamentoEDI.Inicio > 0 || pagamentoEDI.Limite > 0)
                documentoContabils = documentoContabils.Skip(pagamentoEDI.Inicio).Take(pagamentoEDI.Limite).ToList();

            List<int> codigoCargas = (from o in documentoContabils where o.CodigoCarga.HasValue select o.CodigoCarga.Value).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaValePedagio> listaValePedagio = repCargaValePedagio.BuscarParaGeracaoEDIPorPagamento(codigoCargas);

            Dominio.ObjetosDeValor.EDI.ImportsysVP.ImportsysVP retorno = new Dominio.ObjetosDeValor.EDI.ImportsysVP.ImportsysVP();
            retorno.DataGeracao = DateTime.Now;
            retorno.ValePedagios = new List<Dominio.ObjetosDeValor.EDI.ImportsysVP.ValePedagio>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaValePedagio valePedagio in listaValePedagio)
            {
                Dominio.ObjetosDeValor.EDI.ImportsysVP.ValePedagio valePedagioEDI = new Dominio.ObjetosDeValor.EDI.ImportsysVP.ValePedagio();

                valePedagioEDI.ProcImportacao = repPedido.BuscarProcImportacao(valePedagio.CodigoCarga);
                valePedagioEDI.Numero = valePedagio.NumeroValePedagio;
                valePedagioEDI.Valor = valePedagio.ValorValePedagio;
                valePedagioEDI.DataEmissao = valePedagio.DataEmissao;
                valePedagioEDI.CnpjTransportador = valePedagio.CNPJEmpresa;
                valePedagioEDI.NumeroCarga = valePedagio.NumeroCarga;

                retorno.ValePedagios.Add(valePedagioEDI);
            }

            string extensao = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterExtencaoPadrao(pagamentoEDI.LayoutEDI);
            retorno.NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(pagamentoEDI, false, unidadeTrabalho);
            retorno.NomeArquivoSemExtencao = retorno.NomeArquivo.Replace(extensao, "");

            return retorno;
        }

    }
}
