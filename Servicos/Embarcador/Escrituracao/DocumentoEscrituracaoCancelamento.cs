using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class DocumentoEscrituracaoCancelamento
    {
        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaCTe.CTe.TomadorPagador.Cliente;

            if ((cargaCTe.CTe.Empresa?.GerarLoteEscrituracaoCancelamento ?? false) || tomador.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento || (tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false))
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento escrituracaoGerada = repDocumentoEscrituracaoCancelamento.BuscarPorCTe(cargaCTe.CTe.Codigo);

                if (escrituracaoGerada == null)
                {
                    escrituracaoGerada = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento
                    {
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumentoCancelamento.AgEscrituracao,
                        Filial = cargaCTe.Carga?.Filial,
                        CTe = cargaCTe.CTe,
                        Carga = cargaCTe.Carga
                    };

                    repDocumentoEscrituracaoCancelamento.Inserir(escrituracaoGerada);
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.CargaTransbordo)
                return;

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (carga.CargaAgrupada)
            {
                cargas = carga.CargasAgrupamento.ToList();
                cargas.Add(carga); //isso é feito para pegar pedidos encaixados na carga já agrupada nesse caso a cargaorigem na t_carga_cte será a propria carga agrupada.
            }
            else
                cargas.Add(carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargas)
            {
                List<Dominio.Entidades.Cliente> tomadores = repCargaCTe.ObterTomadoresPorCarga(cargaOrigem.Codigo);

                if (tomadores.Count > 0 && (carga.Empresa?.GerarLoteEscrituracaoCancelamento ?? false) || (carga.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false) || tomadores.Any(obj => (obj.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento || (obj.GrupoPessoas != null && obj.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento)) && (!obj.DataViradaProvisao.HasValue || obj.DataViradaProvisao.Value < carga.DataCriacaoCarga)))
                {
                    List<int> codigosCTes = repCargaCTe.BuscarCodigosCTeTransportadorPorCarga(cargaOrigem.Codigo);
                    List<int> escrituracaoesGeradas = repDocumentoEscrituracaoCancelamento.BuscarCodigosCTesPorCarga(cargaOrigem.Codigo);

                    int countCTes = codigosCTes.Count();

                    for (int i = 0; i < countCTes; i++)
                    {
                        int codigoCTe = codigosCTes[i];

                        if (!escrituracaoesGeradas.Contains(codigoCTe))
                        {
                            Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documentoEscrituracaoCancelamento = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento
                            {
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumentoCancelamento.AgEscrituracao,
                                Filial = cargaOrigem.Filial,
                                CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigoCTe },
                                Carga = cargaOrigem
                            };

                            repDocumentoEscrituracaoCancelamento.Inserir(documentoEscrituracaoCancelamento);
                        }
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            if ((lancamentoNFSManual.CTe?.Empresa?.GerarLoteEscrituracaoCancelamento ?? false) || (lancamentoNFSManual.CTe?.CargaCTes?.Any(obj => obj.Carga.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false) ?? false) || (((lancamentoNFSManual.CTe?.TomadorPagador.Cliente.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false) || (lancamentoNFSManual.CTe?.TomadorPagador.Cliente.GrupoPessoas != null && lancamentoNFSManual.CTe.TomadorPagador.Cliente.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento)) && (!lancamentoNFSManual.CTe.TomadorPagador.Cliente.DataViradaProvisao.HasValue || lancamentoNFSManual.CTe.TomadorPagador.Cliente.DataViradaProvisao.Value < lancamentoNFSManual.DataCriacao)))
            {
                if (!lancamentoNFSManual.CTe.ModeloDocumentoFiscal.NaoGerarEscrituracao)
                {
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);

                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documentoEscrituracaoCancelamento = repDocumentoEscrituracaoCancelamento.BuscarPorCTe(lancamentoNFSManual.CTe.Codigo);

                    if (documentoEscrituracaoCancelamento == null)
                    {
                        documentoEscrituracaoCancelamento = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumentoCancelamento.AgEscrituracao,
                            Filial = lancamentoNFSManual.Filial,
                            CTe = lancamentoNFSManual.CTe,
                            LancamentoNFSManual = lancamentoNFSManual,
                            Carga = null
                        };

                        repDocumentoEscrituracaoCancelamento.Inserir(documentoEscrituracaoCancelamento);
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplentoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = repCargaCTeComplentoInfo.BuscarTomadoresPorOcorrencia(cargaOcorrencia.Codigo, false);

            if ((cargaOcorrencia?.Carga?.Empresa?.GerarLoteEscrituracaoCancelamento ?? false) || (cargaOcorrencia?.Carga?.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false) || (tomadores.Count > 0 && tomadores.Any(obj => (obj.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento || (obj.GrupoPessoas != null && obj.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento)) && (!obj.DataViradaProvisao.HasValue || obj.DataViradaProvisao.Value < cargaOcorrencia.DataOcorrencia))))
            {
                List<int> codigosCTes = repCargaCTeComplentoInfo.BuscarCodigosCTePorOcorrencia(cargaOcorrencia.Codigo, false);

                int countCTes = codigosCTes.Count();

                for (int i = 0; i < countCTes; i++)
                {
                    int codigoCTe = codigosCTes[i];

                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documentoEscrituracaoCancelamento = repDocumentoEscrituracaoCancelamento.BuscarPorCTe(codigoCTe);

                    if (documentoEscrituracaoCancelamento == null)
                    {
                        documentoEscrituracaoCancelamento = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumentoCancelamento.AgEscrituracao,
                            Filial = cargaOcorrencia.Carga?.Filial,
                            CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigoCTe },
                            CargaOcorrencia = cargaOcorrencia
                        };

                        repDocumentoEscrituracaoCancelamento.Inserir(documentoEscrituracaoCancelamento);
                    }
                }
            }
        }
    }
}
