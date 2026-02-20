using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.MDFe
{
    public class MDFeImportado : ServicoBase
    {
        public MDFeImportado(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public static bool RemoverMDFeCargaPedido(out string erro, int codigoCargaPedido, int codigoMDFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = null)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            if (cargaPedidoDocumentoMDFe == null)
                cargaPedidoDocumentoMDFe = repCargaPedidoDocumentoMDFe.BuscarPorMDFeECargaPedido(codigoMDFe, codigoCargaPedido);

            if (cargaPedidoDocumentoMDFe != null)
            {
                if (cargaPedidoDocumentoMDFe.CargaPedido.Carga.ProcessandoDocumentosFiscais)
                {
                    erro = "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";
                    return false;
                }

                if (cargaPedidoDocumentoMDFe.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                {
                    erro = "Não é possivel remover o MDF-e na atual situação da Carga (" + cargaPedidoDocumentoMDFe.CargaPedido.Carga.DescricaoSituacaoCarga + ").";
                    return false;
                }

                if (cargaPedidoDocumentoMDFe.CargaPedido.CTeEmitidoNoEmbarcador)
                {
                    cargaPedidoDocumentoMDFe.MDFe.MDFeSemCarga = true;

                    repMDFe.Atualizar(cargaPedidoDocumentoMDFe.MDFe);
                }

                repCargaPedidoDocumentoMDFe.Deletar(cargaPedidoDocumentoMDFe);

                List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFe = repDocumentoMunicipioDescarregamentoMDFe.BuscarPorMDFe(codigoMDFe);

                for (var i = 0; i < documentosMDFe.Count; i++)
                {
                    Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMDFe = documentosMDFe[i];

                    if (documentoMDFe.CTe == null)
                        continue;

                    if (!Servicos.Embarcador.CTe.CTEsImportados.RemoverCTeCargaPedido(out erro, documentoMDFe.CTe.Codigo, codigoCargaPedido, unitOfWork, null))
                        return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        public static bool VincularMDFeACargaPedido(out string erro, int codigoCargaPedido, int codigoMDFe, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFe = repDocumentoMunicipioDescarregamentoMDFe.BuscarPorMDFe(codigoMDFe);

            if (mdfe == null)
            {
                erro = "MDF-e não encontrado.";
                return false;
            }

            if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
            {
                erro = "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";
                return false;
            }

            List<string> numeroCargas = repCargaPedidoDocumentoMDFe.BuscarNumeroCargaPorMDFe(mdfe.Codigo);

            if (numeroCargas.Count == 0)
                numeroCargas = repCargaMDFe.BuscarNumeroCargaAutorizadoPorMDFe(mdfe.Codigo);

            if (numeroCargas.Count > 0)
            {
                erro = $"O MDF-e informado já está vinculado à(s) carga(s) {string.Join(", ", numeroCargas)}.";
                return false;
            }

            bool permiteVincularCTeComplementar = Servicos.Embarcador.Carga.Carga.PermiteVincularCTeComplementoCarga(cargaPedido);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = documentosMDFe.Where(o => o.CTe != null).Select(o => o.CTe).ToList();
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosSemCTeVinculado = documentosMDFe.Where(o => o.CTe == null).ToList();

            if (documentosSemCTeVinculado.Count > 0)
                ctes.AddRange(repCTe.BuscarPorChave(documentosSemCTeVinculado.Where(o => !string.IsNullOrWhiteSpace(o.Chave)).Select(o => o.Chave).ToList()));

            for (int i = 0; i < documentosSemCTeVinculado.Count; i++)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoSemCTeVinculado = documentosSemCTeVinculado[i];

                documentoSemCTeVinculado.CTe = ctes.Find(o => o.Chave == documentoSemCTeVinculado.Chave);

                if (documentoSemCTeVinculado.CTe == null)
                {
                    erro = $"O CT-e ['{documentoSemCTeVinculado.Chave}'] não foi localizado. É necessário que este CT-e esteja na base para utilizar o MDF-e.";
                    return false;
                }

                repDocumentoMunicipioDescarregamentoMDFe.Atualizar(documentoSemCTeVinculado);
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteCancelado = ctes.Find(o => o.Status == "C");

            if (cteCancelado != null)
            {
                erro = $"O CT-e ['{cteCancelado.Chave}'] está cancelado, não sendo possível vincular à carga.";
                return false;
            }

            if (!permiteVincularCTeComplementar)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementar = ctes.Find(o => o.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento);

                if (cteComplementar != null)
                {
                    erro = $"O CT-e {cteComplementar.Numero}-{cteComplementar.Serie.Numero} é um complemento, não sendo possível vincular à carga.";
                    return false;
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTeExistente = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedidoDiff(ctes.Select(o => o.Codigo).ToList(), cargaPedido.Codigo);

            if (cargaPedidoDocumentoCTeExistente != null)
            {
                erro = $"O CT-e {cargaPedidoDocumentoCTeExistente.CTe.Numero}-{cargaPedidoDocumentoCTeExistente.CTe.Serie.Numero} já está vinculado à carga {cargaPedidoDocumentoCTeExistente.CargaPedido.Carga.CodigoCargaEmbarcador}.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExistente = repCargaCTe.BuscarAutorizadoPorCTe(ctes.Select(o => o.Codigo).ToList());

            if (cargaCTeExistente != null)
            {
                erro = $"O CT-e {cargaCTeExistente.CTe.Numero}-{cargaCTeExistente.CTe.Serie.Numero} já está vinculado à carga {cargaCTeExistente.Carga.CodigoCargaEmbarcador}.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTesExistentes = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(ctes.Select(o => o.Codigo).ToList(), cargaPedido.Codigo);

            for (int i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                if (cargaPedidoDocumentosCTesExistentes.Exists(o => o.CTe.Codigo == cte.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                {
                    CargaPedido = cargaPedido,
                    CTe = cte
                };

                repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga.", unitOfWork);

                cte.CTeSemCarga = false;

                repCTe.Atualizar(cte);

                if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);
            }

            if (!configuracaoTMS.UtilizaEmissaoMultimodal)
            {
                cargaPedido.Carga.CargaIntegradaEmbarcador = true;

                repCarga.Atualizar(cargaPedido.Carga);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe()
            {
                CargaPedido = cargaPedido,
                MDFe = mdfe
            };

            repCargaPedidoDocumentoMDFe.Inserir(cargaPedidoDocumentoMDFe, auditado);

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, $"Vinculou o MDF-e {mdfe.Descricao} à carga.", unitOfWork);

            mdfe.MDFeSemCarga = false;

            repMDFe.Atualizar(mdfe);

            erro = string.Empty;
            return true;
        }

        public static bool VerificarSeCargaPossuiAlgumMDFeCompativel(out string erro, int codigoCargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

            if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
            {
                erro = "Não é possível verificar os MDF-es compatíveis pois os documentos da carga estão sendo processados.";
                return true;
            }

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (!cargaPedido.Carga.ProcessandoDocumentosFiscais)
            {
                if ((tomador?.GrupoPessoas?.VincularMDFePeloNumeroPedido ?? false) && !string.IsNullOrWhiteSpace(cargaPedido.Pedido?.NumeroPedidoEmbarcador))
                    mdfes = repMDFe.BuscarPorNumeroPedido(cargaPedido.Pedido.NumeroPedidoEmbarcador);
                else if (cargaPedido.Carga.Veiculo != null)
                    mdfes = repMDFe.ConsultarMDFesSemCargaCompativeis(cargaPedido.Carga.Veiculo.Placa, cargaPedido.Origem.Codigo, 0);
            }

            foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                if (!Servicos.Embarcador.MDFe.MDFeImportado.VincularMDFeACargaPedido(out erro, cargaPedido.Codigo, mdfe.Codigo, unitOfWork, auditado, configuracaoTMS))
                    return false;

            erro = string.Empty;
            return true;
        }

        public async Task<bool> DestinarMDFeImportadoParaSeuDestinoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            string erro;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, _cancellationToken);
            Repositorio.VeiculoMDFe repositorioVeiculoMDFe = new Repositorio.VeiculoMDFe(_unitOfWork);
            Repositorio.MunicipioCarregamentoMDFe repositorioMunicipioCarregamentoMDFe = new Repositorio.MunicipioCarregamentoMDFe(_unitOfWork, _cancellationToken);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioDocumentoMunicipioDescarregamentoMDFe.BuscarPrimeiroCTePorMDFeAsync(mdfe.Codigo);

            if ((cte?.TomadorPagador?.GrupoPessoas?.VincularMDFePeloNumeroPedido ?? false) && !string.IsNullOrWhiteSpace(mdfe.NumeroPedido))
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repositorioCargaPedido.BuscarPorNumeroPedidoEmbarcadorAsync(mdfe.NumeroPedido, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, _cancellationToken);

                if (cargaPedidos.Count == 1)
                {
                    if (VincularMDFeACargaPedido(out erro, cargaPedidos[0].Codigo, mdfe.Codigo, _unitOfWork, auditado, configuracaoTMS))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = await repositorioVeiculoMDFe.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken);
                List<Dominio.Entidades.MunicipioCarregamentoMDFe> municipiosCarregamentoMDFe = repositorioMunicipioCarregamentoMDFe.BuscarPorMDFe(mdfe.Codigo);

                if (veiculoMDFe != null && municipiosCarregamentoMDFe.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorVeiculoOrigemSituacao(veiculoMDFe.Placa, municipiosCarregamentoMDFe.Select(o => o.Municipio.Codigo).ToArray(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                    if (cargaPedidos.Count == 1)
                    {
                        if (VincularMDFeACargaPedido(out erro, cargaPedidos[0].Codigo, mdfe.Codigo, _unitOfWork, auditado, configuracaoTMS))
                            return true;
                        else
                            return false;
                    }
                }
            }

            return false;
        }
    }
}
