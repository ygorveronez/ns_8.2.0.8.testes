using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebServiceCarrefour
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CTe" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CTe.svc or CTe.svc.cs at the Solution Explorer and start debugging.
    public class CTe : WebServiceBase, ICTe
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos protocolo, TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (limite > 50)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

                if (protocolo == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

                if (protocolo.protocoloIntegracaoCarga <= 0 && protocolo.protocoloIntegracaoPedido <= 0)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("Por favor, informe os códigos de integração.");

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;

                if (protocolo.Remetente != null)
                {
                    remetente = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Remetente.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                    if (remetente == null)
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos($"Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}.");
                }

                if (protocolo.Destinatario != null)
                {
                    destinatario = repositorioCliente.BuscarPorCPFCNPJ(protocolo.Destinatario.CPFCNPJ.ObterSomenteNumeros().ToDouble());

                    if (destinatario == null)
                        return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ {protocolo.Remetente.CPFCNPJ}.");
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedidoAutorizados(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (listaCargaPedido == null || listaCargaPedido.Count ==0)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("Os protocolos informados não existem na base Multisoftware.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido.FirstOrDefault();

                if (cargaPedido == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("Os protocolos informados não existem na base Multisoftware.");

                if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos("Os documentos do protocolo informado ainda estão em sendo emitidos.");

                if (remetente != null)
                {
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumento = repositorioCargaOcorrenciaDocumento.BuscarOcorrenciasPendentesCargaRemetenteDestinatario(protocolo.protocoloIntegracaoCarga, remetente.CPF_CNPJ, 0);

                    if (cargaOcorrenciaDocumento.Count > 0)
                    {
                        if (cargaOcorrenciaDocumento.FirstOrDefault().CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Rejeitada)
                            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos($"A Ocorrência foi rejeitada para o CT-e do remetente {remetente.Nome} na Carga.", codigoMensagem: 302);

                        else if (cargaOcorrenciaDocumento.Any(obj => obj.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Finalizada))
                            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoDadosInvalidos($"Existem ocorrências não aprovadas para o CT-e do remetente {remetente.Nome} na Carga.", codigoMensagem: 301);
                    }
                }

                Servicos.WebServiceCarrefour.CTe.CTe servicoCte = new Servicos.WebServiceCarrefour.CTe.CTe(unitOfWork);
                int totalRegistros = servicoCte.ContarCTes(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0);
                List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> listaCte = (totalRegistros > 0) ? servicoCte.BuscarCTes(cargaPedido, tipoDocumentoRetorno, remetente?.CPF_CNPJ ?? 0d, destinatario?.CPF_CNPJ ?? 0d, inicio, limite) : new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>();
                Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>()
                {
                    Itens = listaCte,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou CT-es", unitOfWork);
                new Servicos.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork).Atualizar(listaCte, Auditado.Integradora);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception excecao)
            {
                if (protocolo != null)
                    Servicos.Log.TratarErro($"Carga {protocolo.protocoloIntegracaoCarga} Pedido {protocolo.protocoloIntegracaoPedido}");

                Servicos.Log.TratarErro(excecao);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os CTes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCTe;
        }

        #endregion
    }
}
