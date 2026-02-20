using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Natura
{
    public class EDINatura
    {
        public Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis ConverterDTEmNotFis(Dominio.Entidades.DocumentoTransporteNatura DT, Repositorio.UnitOfWork unitOfWork)
        {
            if (DT.NotasFiscais.Count > 0)
            {
                Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
                Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = new Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis();
                notfis.Emitente = DT.NotasFiscais.FirstOrDefault().Emitente.Nome;
                notfis.Destinatario = DT.Empresa.RazaoSocial;
                notfis.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento();
                notfis.CabecalhoDocumento.Embarcadores = new List<Dominio.ObjetosDeValor.EDI.Notfis.Embarcador>();
                notfis.CabecalhoDocumento.Totais = new Dominio.ObjetosDeValor.EDI.Notfis.Totais();

                List<Dominio.Entidades.Cliente> embarcadores = (from obj in DT.NotasFiscais select obj.Emitente).Distinct().ToList();
                foreach (Dominio.Entidades.Cliente cliEmbarcador in embarcadores)
                {
                    Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = new Dominio.ObjetosDeValor.EDI.Notfis.Embarcador();
                    embarcador.Pessoa = serPessoa.ConverterObjetoPessoa(cliEmbarcador);
                    embarcador.DataEmbarqueMercadoria = DT.DataEmissao;
                    embarcador.Destinatarios = new List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario>();

                    List<Dominio.Entidades.Cliente> destinatarios = (from obj in DT.NotasFiscais where obj.Emitente.CPF_CNPJ == cliEmbarcador.CPF_CNPJ select obj.Destinatario).Distinct().ToList();
                    foreach (Dominio.Entidades.Cliente cliDestinatario in destinatarios)
                    {
                        Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario = new Dominio.ObjetosDeValor.EDI.Notfis.Destinatario();
                        destinatario.Pessoa = serPessoa.ConverterObjetoPessoa(cliDestinatario);
                        destinatario.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal>();
                        List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscaisNatura = (from obj in DT.NotasFiscais where obj.Emitente.CPF_CNPJ == cliEmbarcador.CPF_CNPJ && obj.Destinatario.CPF_CNPJ == cliDestinatario.CPF_CNPJ select obj).ToList();
                        foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura in notasFiscaisNatura)
                        {
                            Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaNatura = new Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal();
                            notaNatura.NFe = ConverterNFNaturaEmNota(nfNatura, unitOfWork);
                            if(notaNatura.NFe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                                notaNatura.CondicaoPagamento = "F";
                            else
                                notaNatura.CondicaoPagamento = "C";

                            notaNatura.NumeroRomaneio = DT.NumeroDT.ToString();
                            notaNatura.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                            Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                            produto.QuantidadeComercial = nfNatura.Quantidade;
                            produto.Descricao = nfNatura.DocumentoTransporte.Empresa.Configuracao.ProdutoPredominante;
                            notaNatura.NFe.NaturezaOP = nfNatura.DocumentoTransporte.Empresa.Configuracao.ProdutoPredominante;
                            produto.ValorTotal = nfNatura.Valor;
                            notaNatura.Produtos.Add(produto);
                            destinatario.NotasFiscais.Add(notaNatura);
                            notfis.CabecalhoDocumento.Totais.PesoTotal += nfNatura.Peso;
                            notfis.CabecalhoDocumento.Totais.QuantidadeTotal += nfNatura.Quantidade;
                            notfis.CabecalhoDocumento.Totais.ValorTotal += nfNatura.Valor;
                            notfis.CabecalhoDocumento.Totais.ValorTotalFrete += nfNatura.ValorFrete;
                        }

                        embarcador.Destinatarios.Add(destinatario);
                    }

                    notfis.CabecalhoDocumento.Embarcadores.Add(embarcador);
                }


            return notfis;
            }
            else
            {
                return null;
            }

        }

        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ConverterNFNaturaEmNota(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
            notaFiscal.Chave = nfNatura.Chave;
            notaFiscal.DataEmissao = nfNatura.DataEmissao.HasValue ? nfNatura.DataEmissao.Value.ToString("ddMMyyyy") : nfNatura.DocumentoTransporte.DataEmissao.ToString("ddMMyyyy");
            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(nfNatura.Destinatario);
            notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(nfNatura.Emitente);
            notaFiscal.InformacoesComplementares = "";
            notaFiscal.ModalidadeFrete = nfNatura.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago : nfNatura.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            notaFiscal.Modelo = "55";
            notaFiscal.NaturezaOP = "";
            notaFiscal.Numero = nfNatura.Numero;
            notaFiscal.PesoBruto = nfNatura.Peso;
            notaFiscal.PesoLiquido = nfNatura.Peso;
            notaFiscal.Protocolo = nfNatura.Codigo;
            notaFiscal.Rota = "";
            notaFiscal.Serie = nfNatura.Serie.ToString();
            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            notaFiscal.Valor = nfNatura.Valor;
            notaFiscal.ValorFrete = nfNatura.ValorFrete;
            notaFiscal.ValorTotalProdutos = nfNatura.Valor;
            notaFiscal.VolumesTotal = nfNatura.Quantidade;
            return notaFiscal;
        }
    }
}
