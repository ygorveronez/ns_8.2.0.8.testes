using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class INTDNE
    {

        private Dominio.ObjetosDeValor.EDI.INTDNE.Parceiro ObterParceiroINTDNE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Servicos.WebService.Pessoas.Pessoa srvPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa tomador = srvPessoa.ConverterObjetoParticipamenteCTe(cte.Tomador);
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = srvPessoa.ConverterObjetoParticipamenteCTe(cte.Destinatario);

            // Preenche a nota fiscal com valores do CTE

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
            transportador.CNPJ = cte.Empresa.CNPJ_SemFormato;

            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal()
            {
                Numero = cte.Numero,
                Serie = cte.Serie.Numero.ToString(),
                DataEmissao = cte.DataEmissao.Value.ToString("ddMMyyyy"),
                NaturezaOP = cte.CFOP != null ? cte.CFOP.CodigoCFOP.ToString() : "",
                VolumesTotal = 0,
                Chave = cte.Chave,
                PesoBruto = cte.XMLNotaFiscais.Sum(obj => obj.Peso),
                PesoLiquido = cte.XMLNotaFiscais.Sum(obj => obj.PesoLiquido),
                ValorTotalProdutos = cte.ValorPrestacaoServico,
                Destinatario = destinatario,
                Emitente = tomador,
                Valor = cte.ValorPrestacaoServico,
                Transportador = transportador
            };

            notaFiscal.Destinatario.Endereco.CEP = Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.Endereco.CEP);

            if (notaFiscal.Destinatario.Endereco.CEP.Length > 8)
                notaFiscal.Destinatario.Endereco.CEP = notaFiscal.Destinatario.Endereco.CEP.Substring(0, 7);

            Dominio.ObjetosDeValor.EDI.INTDNE.Parceiro parceiro = new Dominio.ObjetosDeValor.EDI.INTDNE.Parceiro();
            parceiro.Pessoa = destinatario;
            parceiro.InscricaoMunicipal = destinatario.IM;
            parceiro.InscricaoEstadual = destinatario.RGIE;

            parceiro.Pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers(parceiro.Pessoa.Endereco.CEP);
            if (parceiro.Pessoa.Endereco.CEP.Length > 8)
                parceiro.Pessoa.Endereco.CEP = parceiro.Pessoa.Endereco.CEP.Substring(0, 7);

            Dominio.ObjetosDeValor.EDI.INTDNE.Notas cteIntDNE = new Dominio.ObjetosDeValor.EDI.INTDNE.Notas();
            cteIntDNE.Pessoa = tomador;
            cteIntDNE.DataEmbarque = notaFiscal.DataEmissao;
            cteIntDNE.ZonaTransporte = "";
            cteIntDNE.DocumentoNegFrete = "";
            cteIntDNE.Equipamento = "";
            cteIntDNE.Embalagem = "";
            cteIntDNE.Territorio = "";
            cteIntDNE.Vendedor = "";
            cteIntDNE.Separador = "";
            cteIntDNE.Lote = "";
            cteIntDNE.Romaneio = "";
            cteIntDNE.DocumentoVinculado = "";
            cteIntDNE.DocumentoVinculadoSerie = "";
            cteIntDNE.ItemSubstituicaoTributaria = "";
            cteIntDNE.TipoFinalidadeOperacao = "";
            cteIntDNE.DataPrevisaoColeta = "";
            cteIntDNE.DataPrevisaoEntrega = "";
            cteIntDNE.StatusDNE = 0;
            cteIntDNE.ExcluirRegistroEmbarcador = "";
            cteIntDNE.ExcluirItemDNE = "";
            cteIntDNE.ExcluirReferencia = "";
            cteIntDNE.NotaFiscal = notaFiscal;
            cteIntDNE.Itens = new List<Dominio.ObjetosDeValor.EDI.INTDNE.Itens>();
            Dominio.ObjetosDeValor.EDI.INTDNE.Itens item = new Dominio.ObjetosDeValor.EDI.INTDNE.Itens();

            item.Pessoa = tomador;
            item.NotaFiscal = notaFiscal;

            item.ContaContabil = "";
            item.CentroCusto = "";
            item.DescricaoUnidadeItem = "";
            item.ValorFretePagoCliente = "";
            item.CreditoICMS = "";
            item.CreditoImposto1 = "";
            item.CreditoImposto2 = "";
            item.CreditoImposto3 = "";
            item.QuantidadeEmbaladaTransportador = "";
            cteIntDNE.Itens.Add(item);
            parceiro.Notas = new List<Dominio.ObjetosDeValor.EDI.INTDNE.Notas>();
            parceiro.Notas.Add(cteIntDNE);
            return parceiro;
        }

        public Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE ConverterCargaParaINTDNE(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            // Repositórios e Serviços
            Servicos.WebService.Pessoas.Pessoa srvPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            // Objeto do EDI principal
            Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE intdne = new Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE();

            // Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = lancamentoNFSManual.Pedidos.FirstOrDefault();

            intdne.Ambiente = ""; // Não achei
            intdne.Remetente = lancamentoNFSManual.Tomador.Nome;
            intdne.Destinatario = lancamentoNFSManual.Tomador.Nome;
            intdne.Parceiros = new List<Dominio.ObjetosDeValor.EDI.INTDNE.Parceiro>();
            intdne.Parceiros.Add(ObterParceiroINTDNE(lancamentoNFSManual.CTe, unidadeTrabalho));
            return intdne;
        }

        public Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE ConverterCargaParaINTDNE(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            // Repositórios e Serviços
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Servicos.WebService.Pessoas.Pessoa srvPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            // Objeto do EDI principal
            Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE intdne = new Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

            intdne.Ambiente = ""; // Não achei
            intdne.Remetente = cargaPedido.Pedido.Remetente.Nome;
            intdne.Destinatario = cargaPedido.Pedido.Destinatario.Nome;
            intdne.Parceiros = new List<Dominio.ObjetosDeValor.EDI.INTDNE.Parceiro>();

            //List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
            List<int> empresas = new List<int>();
            if (carga.Empresa != null)
                empresas.Add(carga.Empresa.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaCTesSemSubcontratacaoFilialEmissora(carga.Codigo, empresas);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                intdne.Parceiros.Add(ObterParceiroINTDNE(cargaCTe.CTe, unidadeTrabalho));
            }

            return intdne;
        }

    }
}
