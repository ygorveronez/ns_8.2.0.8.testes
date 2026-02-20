using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public static class DocumentoEscrituracao
    {
        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaCTe.CTe.TomadorPagador.Cliente;

            if ((cargaCTe.CTe.Empresa?.GerarLoteEscrituracao ?? false) || tomador.DisponibilizarDocumentosParaLoteEscrituracao || (tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false))
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao escrituracaoesGerada = repDocumentoEscrituracao.BuscarPorCte(cargaCTe.CTe.Codigo);

                if (escrituracaoesGerada == null)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao
                    {
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao,
                        Filial = cargaCTe.Carga?.Filial,
                        TipoOperacao = cargaCTe.Carga?.TipoOperacao,
                        CTe = cargaCTe.CTe,
                        Carga = cargaCTe.Carga,
                        DataCriacao = DateTime.Now,
                    };

                    repDocumentoEscrituracao.Inserir(cTeEscrituracao);
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

            Dominio.Entidades.Cliente tomador = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.TomadorPagador.Cliente;

            if ((controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Empresa?.GerarLoteEscrituracao ?? false) || (tomador.DisponibilizarDocumentosParaLoteEscrituracao || (tomador.GrupoPessoas != null && tomador.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)))
            {
                if (controleGeracaoCTeAnulacao.CargaCTeAnulacao != null)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao escrituracaoesGerada = repDocumentoEscrituracao.BuscarPorCte(controleGeracaoCTeAnulacao.CargaCTeAnulacao.CTe.Codigo);

                    if (escrituracaoesGerada == null)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao,
                            Filial = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga?.Filial,
                            TipoOperacao = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga?.TipoOperacao,
                            CTe = controleGeracaoCTeAnulacao.CargaCTeAnulacao.CTe,
                            Carga = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga,
                            DataCriacao = DateTime.Now,
                        };

                        repDocumentoEscrituracao.Inserir(cTeEscrituracao);
                    }
                }

                if (controleGeracaoCTeAnulacao.CargaCTeSubstituicao != null)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao escrituracaoesGerada = repDocumentoEscrituracao.BuscarPorCte(controleGeracaoCTeAnulacao.CargaCTeSubstituicao.CTe.Codigo);

                    if (escrituracaoesGerada == null)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao,
                            Filial = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga?.Filial,
                            TipoOperacao = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga?.TipoOperacao,
                            CTe = controleGeracaoCTeAnulacao.CargaCTeSubstituicao.CTe,
                            Carga = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga,
                            DataCriacao = DateTime.Now,
                        };

                        repDocumentoEscrituracao.Inserir(cTeEscrituracao);
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplemento in cargaCTesComplementoInfo)
            {
                Dominio.Entidades.Cliente tomador = cargaCTeComplemento.CTe.TomadorPagador.Cliente;
                if ((fechamentoFrete.Contrato.Transportador?.GerarLoteEscrituracao ?? false) || (tomador.DisponibilizarDocumentosParaLoteEscrituracao || (tomador.GrupoPessoas != null && tomador.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao escrituracaoesGerada = repCTeEscrituracao.BuscarPorCte(cargaCTeComplemento.CTe.Codigo);

                    if (escrituracaoesGerada == null)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao();
                        cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao;
                        cTeEscrituracao.Filial = cargaCTeComplemento.CargaCTeComplementado?.Carga?.Filial;
                        cTeEscrituracao.TipoOperacao = cargaCTeComplemento.CargaCTeComplementado?.Carga?.TipoOperacao;
                        cTeEscrituracao.CTe = cargaCTeComplemento.CTe;
                        cTeEscrituracao.FechamentoFrete = fechamentoFrete;
                        cTeEscrituracao.DataCriacao = DateTime.Now;
                        repCTeEscrituracao.Inserir(cTeEscrituracao);
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.CargaTransbordo)
                return;

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (carga.CargaAgrupada)
            {
                cargas = repCarga.BuscarCargasOriginais(carga.Codigo);
                cargas.Add(carga); //isso é feito para pegar pedidos encaixados na carga já agrupada nesse caso a cargaorigem na t_carga_cte será a propria carga agrupada.
            }
            else
                cargas.Add(carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargas)
            {
                List<Dominio.Entidades.Cliente> tomadores = repCargaCTe.ObterTomadoresPorCarga(cargaOrigem.Codigo);

                if (tomadores.Count > 0 && (carga.Empresa?.GerarLoteEscrituracao ?? false) || (carga.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao ?? false) || tomadores.Any(obj => obj != null && (obj.DisponibilizarDocumentosParaLoteEscrituracao || (obj.GrupoPessoas != null && obj.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)) && (!obj.DataViradaProvisao.HasValue || obj.DataViradaProvisao.Value < cargaOrigem.DataCriacaoCarga)))
                {
                    List<int> codigosCtes = repCargaCTe.BuscarCodigosCTeTransportadorPorCarga(cargaOrigem.Codigo);
                    List<int> escrituracaoesGeradas = repCTeEscrituracao.BuscarCodigosCTesPorCarga(cargaOrigem.Codigo);

                    for (int i = 0; i < codigosCtes.Count(); i++)
                    {
                        if (!escrituracaoesGeradas.Contains(codigosCtes[i]))
                        {
                            Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao
                            {
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao,
                                Filial = cargaOrigem.Filial,
                                TipoOperacao = cargaOrigem.TipoOperacao,
                                CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigosCtes[i] },
                                Carga = cargaOrigem,
                                DataCriacao = DateTime.Now,
                            };

                            repCTeEscrituracao.Inserir(cTeEscrituracao);
                        }
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((lancamentoNFSManual.CTe.Empresa?.GerarLoteEscrituracao ?? false) ||
                (lancamentoNFSManual.CTe.CargaCTes != null && lancamentoNFSManual.CTe.CargaCTes.Any(obj => obj.Carga?.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao ?? false)) ||
                ((lancamentoNFSManual.CTe.TomadorPagador.Cliente.DisponibilizarDocumentosParaLoteEscrituracao || (lancamentoNFSManual.CTe.TomadorPagador.Cliente.GrupoPessoas != null && lancamentoNFSManual.CTe.TomadorPagador.Cliente.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)) &&
                 (!lancamentoNFSManual.CTe.TomadorPagador.Cliente.DataViradaProvisao.HasValue || lancamentoNFSManual.CTe.TomadorPagador.Cliente.DataViradaProvisao.Value < lancamentoNFSManual.DataCriacao)))
            {
                if (!lancamentoNFSManual.CTe.ModeloDocumentoFiscal.NaoGerarEscrituracao)
                {
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = repCTeEscrituracao.BuscarPorCte(lancamentoNFSManual.CTe.Codigo);
                    if (cTeEscrituracao == null)
                    {
                        cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao();
                        cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao;
                        cTeEscrituracao.Filial = lancamentoNFSManual.Filial;
                        cTeEscrituracao.TipoOperacao = lancamentoNFSManual.TipoOperacao;
                        cTeEscrituracao.CTe = lancamentoNFSManual.CTe;
                        cTeEscrituracao.LancamentoNFSManual = lancamentoNFSManual;
                        cTeEscrituracao.Carga = null;
                        cTeEscrituracao.DataCriacao = DateTime.Now;
                        repCTeEscrituracao.Inserir(cTeEscrituracao);
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaEscrituracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplentoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = repCargaCTeComplentoInfo.BuscarTomadoresPorOcorrencia(cargaOcorrencia.Codigo, false);

            if ((cargaOcorrencia?.Carga?.Empresa?.GerarLoteEscrituracao ?? false) || (cargaOcorrencia?.Carga?.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao ?? false) || (tomadores.Count > 0 && tomadores.Any(obj => (obj.DisponibilizarDocumentosParaLoteEscrituracao || (obj.GrupoPessoas != null && obj.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)) && (!obj.DataViradaProvisao.HasValue || obj.DataViradaProvisao.Value < cargaOcorrencia.DataOcorrencia))))
            {
                List<int> codigosCtes = repCargaCTeComplentoInfo.BuscarCodigosCTePorOcorrencia(cargaOcorrencia.Codigo, false);
                for (int i = 0; i < codigosCtes?.Count; i++)
                {
                    Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = repCTeEscrituracao.BuscarPorCte(codigosCtes[i]);
                    if (cTeEscrituracao == null)
                    {
                        cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao();
                        cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao;
                        cTeEscrituracao.Filial = cargaOcorrencia.Carga?.Filial;
                        cTeEscrituracao.TipoOperacao = cargaOcorrencia.Carga?.TipoOperacao;
                        cTeEscrituracao.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigosCtes[i] };
                        cTeEscrituracao.CargaOcorrencia = cargaOcorrencia;
                        cTeEscrituracao.DataCriacao = DateTime.Now;

                        repCTeEscrituracao.Inserir(cTeEscrituracao);
                    }
                }
            }
        }
    }
}
