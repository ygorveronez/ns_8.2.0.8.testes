using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class OCOREN
    {
        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotasFiscais, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, string codigoIntegracaoTipoOcorrencia, DateTime dataOcorrencia, DateTime? dataEvento, string observacao, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = null, Dominio.Entidades.LayoutEDI layoutEDI = null, Dominio.Entidades.Empresa empresa = null, Dominio.Entidades.Cliente remetente = null)
        {
            if ((ctes == null || ctes.Count <= 0) && (xMLNotasFiscais == null || xMLNotasFiscais.Count == 0))
                return null;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeTrabalho);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeTrabalho);
            List<int> codigosEmpresas = new List<int>();

            if (empresa == null)
                empresa = ctes?.FirstOrDefault()?.Empresa;
            else
                codigosEmpresas.Add(empresa.Codigo);

            if (remetente == null)
                remetente = ctes?.FirstOrDefault()?.Remetente?.Cliente;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente?.CPF_CNPJ_SemFormato ?? "";
            ocoren.Destinatario = remetente?.Nome ?? "";
            ocoren.CPFCNPJRemetente = empresa?.CNPJ ?? "";
            ocoren.CodigoRemetente = empresa?.CodigoIntegracao ?? "";
            ocoren.Remetente = empresa?.RazaoSocial ?? "";
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";
            ocoren.Intercambio50 = "OCO50" + DateTime.Now.ToString("ddMM") + "999";
            ocoren.TipoCarga = ocorrencia?.Carga?.TipoDeCarga?.Descricao ?? string.Empty;

            if (ctes != null && ctes.Count > 0)
                codigosEmpresas = (from obj in ctes select obj.Empresa.Codigo).Distinct().ToList();

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            foreach (int codigoEmpresa in codigosEmpresas)
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmpresa = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                Dominio.Entidades.Empresa empresaOcorre = empresa;
                if (ctes != null && ctes.Count > 0)
                {
                    ctesEmpresa = (from obj in ctes where obj.Empresa.Codigo == codigoEmpresa select obj).ToList();
                    empresaOcorre = ctesEmpresa.First().Empresa;
                }

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;
                if (ctes != null && ctes.Count > 0)
                {
                    int codigoPrimeiroCTe = ctes.FirstOrDefault()?.Codigo ?? 0;
                    carregamento = repositorioCarregamento.BuscarPorCTe(codigoPrimeiroCTe);
                }

                Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
                cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";
                cabecalhoDocumento.IdentificacaoDocumento50 = "OCORR50" + DateTime.Now.ToString("ddMM") + "999";

                cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
                cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(empresaOcorre);

                cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

                if (xMLNotasFiscais == null || xMLNotasFiscais.Count == 0)
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        if ((cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) &&
                            (layoutEDI == null || !layoutEDI.UtilizarInformacoesCTeOriginal))
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlnotasfiscais = null;
                            if (ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe)
                            {
                                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCTeEOcorrencia(cte.Codigo, ocorrencia.Codigo);
                                xmlnotasfiscais = cargaOcorrenciaDocumento.XMLNotaFiscais?.ToList();
                            }

                            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cte.Documentos)
                            {
                                if (xmlnotasfiscais != null && xmlnotasfiscais.Where(o => o.Chave == documentoCTe.ChaveNFE).FirstOrDefault() == null)
                                    continue;

                                totalOcorrencias++;
                                Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();

                                if (!string.IsNullOrWhiteSpace(documentoCTe.CNPJRemetente))
                                    notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoCTe.CNPJRemetente;
                                else if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE) && documentoCTe.ChaveNFE.Length >= 44)
                                    notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoCTe.ChaveNFE.Substring(6, 14);

                                notaFiscalOcorrencia.SerieNotaFiscal = documentoCTe.SerieOuSerieDaChave;

                                int numeroNotaFiscal;
                                int.TryParse(documentoCTe.Numero, out numeroNotaFiscal);

                                notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;

                                if (!string.IsNullOrEmpty(codigoIntegracaoTipoOcorrencia))
                                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = codigoIntegracaoTipoOcorrencia;
                                else
                                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = tipoOcorrencia.CodigoProceda;

                                notaFiscalOcorrencia.DataEvento = dataEvento.HasValue ? dataEvento.Value : dataOcorrencia;
                                notaFiscalOcorrencia.DataOcorrencia = dataOcorrencia;
                                notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = tipoOcorrencia.CodigoObservacao;
                                notaFiscalOcorrencia.TextoExplicativo = observacao;
                                notaFiscalOcorrencia.NumeroRomaneio = documentoCTe.NumeroRomaneio;
                                notaFiscalOcorrencia.NumeroPedido = documentoCTe.NumeroPedido;
                                notaFiscalOcorrencia.ChaveNotaFiscal = documentoCTe.ChaveNFE;
                                notaFiscalOcorrencia.ProtocoloNotaFiscal = documentoCTe.ProtocoloNFe;
                                notaFiscalOcorrencia.CNPJCarga = cte.Empresa?.CNPJ_SemFormato;
                                notaFiscalOcorrencia.NumeroCarregamento = carregamento?.AutoSequenciaNumero ?? 0;

                                notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                                notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = cte.Tomador.CPF_CNPJ_SemFormato;
                                notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                                notaFiscalOcorrencia.CTeOcorrencia.Serie = cte.Serie.Numero.ToString();
                                notaFiscalOcorrencia.CTeOcorrencia.Numero = cte.Numero.ToString();

                                cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                            }
                        }
                        else //quando é outros documentos, gera a ocorrência com o documento original
                        {
                            if (ocorrencia != null && ocorrencia.Carga != null)
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in ocorrencia.Carga.Pedidos)
                                {
                                    bool inseriu = false;

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeSubcontratacao in cargaPedido.PedidoCTesParaSubContratacao)
                                    {
                                        List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnteriores = repDocumentoAnterior.BuscarPorCTe(cte.Codigo);

                                        string chaveCTeAnterior = documentosAnteriores.Select(o => o.Chave).FirstOrDefault();

                                        if (pedidoCTeSubcontratacao.CTeTerceiro.ChaveAcesso == chaveCTeAnterior)
                                        {
                                            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe nfe in pedidoCTeSubcontratacao.CTeTerceiro.CTesTerceiroNFes)
                                            {
                                                totalOcorrencias++;

                                                Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                                                notaFiscalOcorrencia.CNPJEmissorNotaFiscal = pedidoCTeSubcontratacao.CTeTerceiro.Remetente.CPF_CNPJ;

                                                int numeroNotaFiscal, serie;
                                                int.TryParse(nfe.Chave.Substring(25, 9), out numeroNotaFiscal);
                                                int.TryParse(nfe.Chave.Substring(22, 3), out serie);

                                                notaFiscalOcorrencia.SerieNotaFiscal = serie.ToString();
                                                notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;

                                                if (!string.IsNullOrEmpty(codigoIntegracaoTipoOcorrencia))
                                                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = codigoIntegracaoTipoOcorrencia;
                                                else
                                                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = tipoOcorrencia.CodigoProceda;

                                                notaFiscalOcorrencia.DataEvento = dataEvento.HasValue ? dataEvento.Value : dataOcorrencia;
                                                notaFiscalOcorrencia.DataOcorrencia = dataOcorrencia;
                                                notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = tipoOcorrencia.CodigoObservacao;
                                                notaFiscalOcorrencia.TextoExplicativo = observacao;
                                                notaFiscalOcorrencia.NumeroRomaneio = !string.IsNullOrWhiteSpace(nfe.NumeroRomaneio) ? nfe.NumeroRomaneio : pedidoCTeSubcontratacao.CTeTerceiro.NumeroRomaneio;
                                                notaFiscalOcorrencia.NumeroPedido = !string.IsNullOrWhiteSpace(nfe.NumeroPedido) ? nfe.NumeroPedido : pedidoCTeSubcontratacao.CTeTerceiro.NumeroPedido;
                                                notaFiscalOcorrencia.CNPJCarga = empresa?.CNPJ_SemFormato ?? "";
                                                notaFiscalOcorrencia.NumeroCarregamento = 0;
                                                notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                                                notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = pedidoCTeSubcontratacao.CTeTerceiro.Remetente.CPF_CNPJ;
                                                notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = pedidoCTeSubcontratacao.CTeTerceiro.Emitente.Nome;
                                                notaFiscalOcorrencia.CTeOcorrencia.Serie = pedidoCTeSubcontratacao.CTeTerceiro.Serie;
                                                notaFiscalOcorrencia.CTeOcorrencia.Numero = pedidoCTeSubcontratacao.CTeTerceiro.Numero.ToString();

                                                ocoren.Destinatario = pedidoCTeSubcontratacao.CTeTerceiro.Emitente.Nome;
                                                //ocoren.Remetente = pedidoCTeSubcontratacao.CTeTerceiro.Emitente.Nome;

                                                cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(empresa); //svcPessoa.ConverterObjetoParticipamenteCTe(pedidoCTeSubcontratacao.CTeTerceiro.Emitente);
                                                cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                                            }

                                            inseriu = true;
                                            break;
                                        }
                                    }

                                    if (inseriu)
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal in xMLNotasFiscais)
                    {
                        totalOcorrencias++;

                        Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                        notaFiscalOcorrencia.CNPJEmissorNotaFiscal = xMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato;
                        notaFiscalOcorrencia.SerieNotaFiscal = xMLNotaFiscal.Serie.ToString();

                        int numeroNotaFiscal = xMLNotaFiscal.Numero;

                        notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;

                        if (!string.IsNullOrEmpty(codigoIntegracaoTipoOcorrencia))
                            notaFiscalOcorrencia.CodigoOcorrenciaEntrega = codigoIntegracaoTipoOcorrencia;
                        else
                            notaFiscalOcorrencia.CodigoOcorrenciaEntrega = tipoOcorrencia.CodigoProceda;

                        notaFiscalOcorrencia.DataEvento = dataEvento.HasValue ? dataEvento.Value : dataOcorrencia;
                        notaFiscalOcorrencia.DataOcorrencia = dataOcorrencia;
                        notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = tipoOcorrencia.CodigoObservacao;
                        notaFiscalOcorrencia.TextoExplicativo = observacao;
                        notaFiscalOcorrencia.NumeroRomaneio = xMLNotaFiscal.NumeroPedido.ToString();
                        notaFiscalOcorrencia.NumeroPedido = xMLNotaFiscal.NumeroPedido.ToString();
                        notaFiscalOcorrencia.ChaveNotaFiscal = xMLNotaFiscal.Chave;
                        notaFiscalOcorrencia.ProtocoloNotaFiscal = xMLNotaFiscal.Protocolo;
                        notaFiscalOcorrencia.CNPJCarga = empresa?.CNPJ_SemFormato ?? "";
                        notaFiscalOcorrencia.NumeroCarregamento = 0;

                        notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                        notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = xMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato;
                        notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                        notaFiscalOcorrencia.CTeOcorrencia.Serie = xMLNotaFiscal.Serie.ToString();
                        notaFiscalOcorrencia.CTeOcorrencia.Numero = xMLNotaFiscal.Numero.ToString();

                        cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                    }
                }

                ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);
            }

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork)
        {
            if (ctes == null || ctes.Count <= 0)
                return null;

            Dominio.Entidades.Empresa empresa = ctes.FirstOrDefault().Empresa;
            Dominio.Entidades.ParticipanteCTe remetente = ctes.FirstOrDefault().Remetente;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente.CPF_CNPJ_SemFormato;
            ocoren.Destinatario = remetente.Nome;
            ocoren.CPFCNPJRemetente = empresa.CNPJ;
            ocoren.Remetente = empresa.RazaoSocial;
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";
            ocoren.Intercambio50 = "OCO50" + DateTime.Now.ToString("ddMM") + "999";

            List<int> codigosEmpresas = (from obj in ctes select obj.Empresa.Codigo).Distinct().ToList();

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            foreach (int codigoEmpresa in codigosEmpresas)
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmpresa = (from obj in ctes where obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;
                if (ctes != null && ctes.Count > 0)
                {
                    int codigoPrimeiroCTe = ctes.FirstOrDefault()?.Codigo ?? 0;
                    carregamento = repositorioCarregamento.BuscarPorCTe(codigoPrimeiroCTe);
                }

                Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
                cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";
                cabecalhoDocumento.IdentificacaoDocumento50 = "OCORR50" + DateTime.Now.ToString("ddMM") + "999";

                cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
                cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(ctesEmpresa.First().Empresa);

                cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    Dominio.Entidades.OcorrenciaDeCTe ocorrencia = cte.Ocorrencias.OrderByDescending(o => o.DataDaOcorrencia).FirstOrDefault();

                    if (ocorrencia == null)
                        continue;

                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = ocorrencia?.Ocorrencia;
                    DateTime dataOcorrencia = ocorrencia?.DataDaOcorrencia ?? DateTime.Now;
                    string observacao = ocorrencia?.Observacao;

                    foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cte.Documentos)
                    {
                        totalOcorrencias++;

                        Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();

                        if (!string.IsNullOrWhiteSpace(documentoCTe.CNPJRemetente))
                            notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoCTe.CNPJRemetente;
                        else if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE) && documentoCTe.ChaveNFE.Length >= 44)
                            notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoCTe.ChaveNFE.Substring(6, 14);

                        notaFiscalOcorrencia.SerieNotaFiscal = documentoCTe.SerieOuSerieDaChave;

                        int numeroNotaFiscal;
                        int.TryParse(documentoCTe.Numero, out numeroNotaFiscal);

                        notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;
                        notaFiscalOcorrencia.CodigoOcorrenciaEntrega = tipoOcorrencia?.CodigoProceda;
                        notaFiscalOcorrencia.DataEvento = dataOcorrencia;
                        notaFiscalOcorrencia.DataOcorrencia = dataOcorrencia;
                        notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = tipoOcorrencia?.CodigoObservacao;
                        notaFiscalOcorrencia.TextoExplicativo = observacao;
                        notaFiscalOcorrencia.NumeroRomaneio = documentoCTe.NumeroRomaneio;
                        notaFiscalOcorrencia.NumeroPedido = documentoCTe.NumeroPedido;
                        notaFiscalOcorrencia.ChaveNotaFiscal = documentoCTe.ChaveNFE;
                        notaFiscalOcorrencia.ProtocoloNotaFiscal = documentoCTe.ProtocoloNFe;

                        notaFiscalOcorrencia.CNPJCarga = cte.Empresa?.CNPJ_SemFormato;
                        notaFiscalOcorrencia.NumeroCarregamento = carregamento?.AutoSequenciaNumero ?? 0;

                        notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                        notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = cte.Tomador.CPF_CNPJ_SemFormato;
                        notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                        notaFiscalOcorrencia.CTeOcorrencia.Serie = cte.Serie.Numero.ToString();
                        notaFiscalOcorrencia.CTeOcorrencia.Numero = cte.Numero.ToString();

                        cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                    }
                }

                ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);
            }

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.NFSe> nfse, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (nfse == null || nfse.Count <= 0)
                return null;

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = nfse.FirstOrDefault().Empresa;
            Dominio.Entidades.ParticipanteNFSe remetente = nfse.FirstOrDefault().Tomador;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente.CPF_CNPJ;
            ocoren.Destinatario = remetente.Nome;
            ocoren.CPFCNPJRemetente = empresa.CNPJ;
            ocoren.Remetente = empresa.RazaoSocial;
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";

            List<int> codigosEmpresas = (from obj in nfse select obj.Empresa.Codigo).Distinct().ToList();

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            foreach (int codigoEmpresa in codigosEmpresas)
            {
                List<Dominio.Entidades.NFSe> nfseEmpresa = (from obj in nfse where obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

                Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
                cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";

                cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
                cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(nfseEmpresa.First().Empresa);
                cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

                foreach (Dominio.Entidades.NFSe nfs in nfse)
                {
                    Dominio.Entidades.OcorrenciaDeNFSe ocorrencia = repOcorrenciaDeNFSe.BuscarUltimaOcorrenciaPorNFSe(nfs.Codigo);

                    foreach (Dominio.Entidades.DocumentosNFSe documentoNFSe in nfs.Documentos)
                    {
                        totalOcorrencias++;

                        Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                        notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoNFSe.NFSe.Tomador.CPF_CNPJ;
                        notaFiscalOcorrencia.SerieNotaFiscal = documentoNFSe.Serie;

                        int numeroNotaFiscal;
                        int.TryParse(documentoNFSe.Numero, out numeroNotaFiscal);

                        notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;
                        notaFiscalOcorrencia.CodigoOcorrenciaEntrega = ocorrencia.Ocorrencia?.CodigoProceda ?? string.Empty;
                        notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = ocorrencia.Ocorrencia?.CodigoObservacao ?? string.Empty;
                        notaFiscalOcorrencia.DataEvento = ocorrencia.DataDaOcorrencia;
                        notaFiscalOcorrencia.DataOcorrencia = ocorrencia.DataDaOcorrencia;
                        notaFiscalOcorrencia.TextoExplicativo = ocorrencia.Observacao;
                        notaFiscalOcorrencia.CNPJCarga = nfs.Empresa?.CNPJ_SemFormato;
                        notaFiscalOcorrencia.NumeroCarregamento = 0;

                        notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                        notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = nfs.Tomador.CPF_CNPJ;
                        notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                        notaFiscalOcorrencia.CTeOcorrencia.Serie = nfs.Serie.Numero.ToString();
                        notaFiscalOcorrencia.CTeOcorrencia.Numero = nfs.Numero.ToString();

                        cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                    }
                }

                ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);
            }

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.XMLNotaFiscalEletronica> nfe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (nfe == null || nfe.Count <= 0)
                return null;

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.OcorrenciaDeNFe repOcorrenciaDeNFe = new Repositorio.OcorrenciaDeNFe(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = nfe.FirstOrDefault().Empresa;
            Dominio.Entidades.Cliente remetente = nfe.FirstOrDefault().Emitente;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente.CPF_CNPJ_SemFormato;
            ocoren.Destinatario = remetente.Nome;
            ocoren.CPFCNPJRemetente = empresa.CNPJ;
            ocoren.Remetente = empresa.RazaoSocial;
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";

            List<int> codigosEmpresas = (from obj in nfe select obj.Empresa.Codigo).Distinct().ToList();

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            foreach (int codigoEmpresa in codigosEmpresas)
            {
                List<Dominio.Entidades.XMLNotaFiscalEletronica> nfseEmpresa = (from obj in nfe where obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

                Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
                cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";

                cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
                cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(nfseEmpresa.First().Empresa);
                cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

                foreach (Dominio.Entidades.XMLNotaFiscalEletronica nota in nfe)
                {
                    Dominio.Entidades.OcorrenciaDeNFe ocorrencia = repOcorrenciaDeNFe.BuscarUltimaOcorrenciaPorNFe(nota.Codigo);
                    totalOcorrencias++;

                    Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                    notaFiscalOcorrencia.CNPJEmissorNotaFiscal = nota.Emitente.CPF_CNPJ_SemFormato;

                    string serie = nota.Chave?.Substring(22, 3) ?? "0";
                    serie = serie.TrimStart('0');
                    notaFiscalOcorrencia.SerieNotaFiscal = serie;

                    int numeroNotaFiscal;
                    int.TryParse(nota.Numero, out numeroNotaFiscal);

                    notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;
                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = ocorrencia.Ocorrencia?.CodigoProceda ?? string.Empty;
                    notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = ocorrencia.Ocorrencia?.CodigoObservacao ?? string.Empty;
                    notaFiscalOcorrencia.DataEvento = ocorrencia.DataDaOcorrencia;
                    notaFiscalOcorrencia.DataOcorrencia = ocorrencia.DataDaOcorrencia;
                    notaFiscalOcorrencia.TextoExplicativo = ocorrencia.Observacao;
                    notaFiscalOcorrencia.CNPJCarga = empresa?.CNPJ_SemFormato;
                    notaFiscalOcorrencia.NumeroCarregamento = 0;

                    notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                    notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = nota.Emitente.CPF_CNPJ_SemFormato;
                    notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                    notaFiscalOcorrencia.CTeOcorrencia.Serie = serie;
                    notaFiscalOcorrencia.CTeOcorrencia.Numero = nota.Numero.ToString();

                    cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                }

                ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);
            }

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.OcorrenciaDeNFSe> listaOcorrenciasNFSe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (listaOcorrenciasNFSe == null || listaOcorrenciasNFSe.Count <= 0)
                return null;

            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeTrabalho);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = listaOcorrenciasNFSe.FirstOrDefault().NFSe.Empresa;
            Dominio.Entidades.ParticipanteNFSe remetente = listaOcorrenciasNFSe.FirstOrDefault().NFSe.Tomador;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente.CPF_CNPJ;
            ocoren.Destinatario = remetente.Nome;
            ocoren.CPFCNPJRemetente = empresa.CNPJ;
            ocoren.Remetente = empresa.RazaoSocial;
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
            cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";

            cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
            cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(empresa);
            cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

            foreach (Dominio.Entidades.OcorrenciaDeNFSe ocorrencia in listaOcorrenciasNFSe)
            {
                List<Dominio.Entidades.DocumentosNFSe> listaDocumentosNFSe = repDocumentosNFSe.BuscarPorNFSe(ocorrencia.NFSe.Codigo);

                foreach (Dominio.Entidades.DocumentosNFSe documentoNFSe in listaDocumentosNFSe)
                {
                    totalOcorrencias++;

                    Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                    notaFiscalOcorrencia.CNPJEmissorNotaFiscal = documentoNFSe.NFSe.Tomador.CPF_CNPJ;
                    notaFiscalOcorrencia.SerieNotaFiscal = documentoNFSe.Serie;

                    int numeroNotaFiscal;
                    int.TryParse(documentoNFSe.Numero, out numeroNotaFiscal);

                    notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;
                    notaFiscalOcorrencia.CodigoOcorrenciaEntrega = ocorrencia.Ocorrencia?.CodigoProceda ?? string.Empty;
                    notaFiscalOcorrencia.DataEvento = ocorrencia.DataDaOcorrencia;
                    notaFiscalOcorrencia.DataOcorrencia = ocorrencia.DataDaOcorrencia;
                    notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = ocorrencia.Ocorrencia?.CodigoObservacao ?? string.Empty;
                    notaFiscalOcorrencia.TextoExplicativo = ocorrencia.Observacao;
                    notaFiscalOcorrencia.CNPJCarga = documentoNFSe.NFSe?.Empresa?.CNPJ_SemFormato;
                    notaFiscalOcorrencia.NumeroCarregamento = 0;

                    notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                    notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = remetente.CPF_CNPJ;
                    notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                    notaFiscalOcorrencia.CTeOcorrencia.Serie = documentoNFSe.NFSe.Serie.Numero.ToString();
                    notaFiscalOcorrencia.CTeOcorrencia.Numero = documentoNFSe.NFSe.Numero.ToString();

                    cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
                }
            }

            ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ConverterParaOCOREN(List<Dominio.Entidades.OcorrenciaDeNFe> listaOcorrenciasNFe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (listaOcorrenciasNFe == null || listaOcorrenciasNFe.Count <= 0)
                return null;

            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeTrabalho);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = listaOcorrenciasNFe.FirstOrDefault().NFe.Empresa;
            Dominio.Entidades.Cliente remetente = listaOcorrenciasNFe.FirstOrDefault().NFe.Emitente;

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();

            ocoren.Data = DateTime.Now;
            ocoren.CPFCNPJDestinatario = remetente.CPF_CNPJ_SemFormato;
            ocoren.Destinatario = remetente.Nome;
            ocoren.CPFCNPJRemetente = empresa.CNPJ;
            ocoren.Remetente = empresa.RazaoSocial;
            ocoren.Intercambio = "OCO" + DateTime.Now.ToString("ddMMHHmm") + "1";

            ocoren.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento>();

            int totalOcorrencias = 0;

            Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.OCOREN.CabecalhoDocumento();
            cabecalhoDocumento.IdentificacaoDocumento = "OCORR" + DateTime.Now.ToString("ddMMHHmm") + "1";

            cabecalhoDocumento.Transportador = new Dominio.ObjetosDeValor.EDI.OCOREN.Transportador();
            cabecalhoDocumento.Transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(empresa);
            cabecalhoDocumento.Transportador.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia>();

            foreach (Dominio.Entidades.OcorrenciaDeNFe ocorrencia in listaOcorrenciasNFe)
            {
                totalOcorrencias++;

                Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia notaFiscalOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.NotaFiscalOcorrencia();
                notaFiscalOcorrencia.CNPJEmissorNotaFiscal = ocorrencia.NFe.Emitente.CPF_CNPJ_SemFormato;

                string serie = ocorrencia.NFe.Chave?.Substring(22, 3) ?? "0";
                serie = serie.TrimStart('0');
                notaFiscalOcorrencia.SerieNotaFiscal = serie;

                int numeroNotaFiscal;
                int.TryParse(ocorrencia.NFe.Numero, out numeroNotaFiscal);

                notaFiscalOcorrencia.NumeroNotaFiscal = numeroNotaFiscal;
                notaFiscalOcorrencia.CodigoOcorrenciaEntrega = ocorrencia.Ocorrencia?.CodigoProceda ?? string.Empty;
                notaFiscalOcorrencia.DataEvento = ocorrencia.DataDaOcorrencia;
                notaFiscalOcorrencia.DataOcorrencia = ocorrencia.DataDaOcorrencia;
                notaFiscalOcorrencia.CodigoObservacaoOcorrenciaEntrada = ocorrencia.Ocorrencia?.CodigoObservacao ?? string.Empty;
                notaFiscalOcorrencia.TextoExplicativo = ocorrencia.Observacao;
                notaFiscalOcorrencia.NumeroPedido = ocorrencia.NFe.Pedido;
                notaFiscalOcorrencia.CNPJCarga = empresa?.CNPJ_SemFormato;
                notaFiscalOcorrencia.NumeroCarregamento = 0;

                notaFiscalOcorrencia.CTeOcorrencia = new Dominio.ObjetosDeValor.EDI.OCOREN.CTeOcorrencia();
                notaFiscalOcorrencia.CTeOcorrencia.CNPJEmpresaContratante = ocorrencia.NFe.Emitente.CPF_CNPJ_SemFormato;
                notaFiscalOcorrencia.CTeOcorrencia.FilialEmissora = empresa.RazaoSocial;
                notaFiscalOcorrencia.CTeOcorrencia.Serie = serie;
                notaFiscalOcorrencia.CTeOcorrencia.Numero = ocorrencia.NFe.Numero.ToString();

                cabecalhoDocumento.Transportador.NotasFiscais.Add(notaFiscalOcorrencia);
            }

            ocoren.CabecalhosDocumento.Add(cabecalhoDocumento);

            ocoren.Total = new Dominio.ObjetosDeValor.EDI.OCOREN.Total()
            {
                NumeroRegistroOcorrencia = totalOcorrencias
            };

            return ocoren;
        }

        public MemoryStream GerarArquivoOcorenOTIF(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, DateTime dataOcorrencia, DateTime dataEvento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (ctes == null || ctes.Count <= 0)
                return null;

            MemoryStream memoStream = new MemoryStream();
            StringBuilder RegistroEDI = new StringBuilder();

            string linha1, linha2;

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cte.Documentos)
                {
                    string cnpjEmissor = string.Empty;
                    if (!string.IsNullOrWhiteSpace(documentoCTe.CNPJRemetente))
                        cnpjEmissor = documentoCTe.CNPJRemetente;
                    else if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE) && documentoCTe.ChaveNFE.Length >= 44)
                        cnpjEmissor = documentoCTe.ChaveNFE.Substring(6, 14);

                    linha1 = "1"; //1 – Reporte da chegada do caminhão no cliente
                    linha1 = string.Concat(linha1, cnpjEmissor.PadLeft(14, '0')); //CNPJ do Emissor da Nota Fiscal
                    linha1 = string.Concat(linha1, documentoCTe.Numero.PadLeft(7, '0')); //NR Nota Fiscal
                    linha1 = string.Concat(linha1, dataOcorrencia.ToString("ddMMyyyy")); //Data do Evento DDMMYYYY 
                    linha1 = string.Concat(linha1, dataOcorrencia.ToString("HHmm")); //Hora do Evento
                    linha1 = string.Concat(linha1, tipoOcorrencia?.CodigoProceda?.Left(2)); //Id Motivo Atraso
                    linha1 = string.Concat(linha1, ""); //Ds Motivo Atraso
                    RegistroEDI.AppendLine(linha1);


                    linha2 = "2"; //2 – Reporte da descarga (entrega) da mercadoria no cliente
                    linha2 = string.Concat(linha2, cnpjEmissor.PadLeft(14, '0')); //CNPJ do Emissor da Nota Fiscal
                    linha2 = string.Concat(linha2, documentoCTe.Numero.PadLeft(7, '0')); //NR Nota Fiscal
                    linha2 = string.Concat(linha2, dataEvento.ToString("ddMMyyyy")); //Data do Evento DDMMYYYY 
                    linha2 = string.Concat(linha2, dataEvento.ToString("HHmm")); //Hora do Evento
                    linha2 = string.Concat(linha2, tipoOcorrencia?.CodigoProceda?.Left(2)); //Id Motivo Atraso
                    linha2 = string.Concat(linha2, ""); //Ds Motivo Atraso
                    RegistroEDI.AppendLine(linha2);
                }
            }

            string arquivo = RegistroEDI.ToString();
            memoStream.Write(System.Text.Encoding.UTF8.GetBytes(arquivo), 0, arquivo.Length);
            memoStream.Position = 0;

            return memoStream;
        }

    }
}
