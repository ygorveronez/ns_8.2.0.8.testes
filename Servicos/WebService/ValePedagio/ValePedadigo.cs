
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Chamado;

namespace Servicos.WebService.ValePedagio
{
    public class ValePedadigo : ServicoWebServiceBase
    {
        #region Variáveis Privadas

        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion Variáveis Privadas

        #region Construtores

        public ValePedadigo(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion Construtores

        #region Métodos Publicos

        public Retorno<bool> IntegraValePedagio(Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio, Dominio.Entidades.WebService.Integradora integradora)
        {
            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaPedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Cliente repClient = new Repositorio.Cliente(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga exiteCarga = repositorioCarga.BuscarPorProtocolo(valePedagio?.ProtocoloCarga ?? 0);

                if (exiteCarga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possivel encontrar uma carga como o protocolo {valePedagio.ProtocoloCarga}");

                if (exiteCarga?.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && exiteCarga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.Agrupadora)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Stage stage = repositorioAgrupamento.BuscarStagePorCargaMaeENumeroStage(valePedagio.ProtocoloCarga, valePedagio.NumeroStage);

                    if ((stage?.StageAgrupamento?.CargaGerada != null))
                        exiteCarga = stage.StageAgrupamento.CargaGerada;

                    if (exiteCarga == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possivel encontrar uma carga filha para a carga mão com o protocolo {valePedagio.ProtocoloCarga}");
                }

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;
                Dominio.Entidades.Cliente fornecedor = null;

                bool statusCanceladoOuComprado = valePedagio.Status == SituacaoValePedagio.Comprada || valePedagio.Status == SituacaoValePedagio.Cancelada;

                if (!string.IsNullOrEmpty(valePedagio.NumeroRecibo) && statusCanceladoOuComprado && !string.IsNullOrEmpty(valePedagio.CNPJFornecedor))
                {
                    fornecedor = repClient.BuscarPorCPFCNPJ(valePedagio.CNPJFornecedor.ToDouble());

                    if (fornecedor == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Fornecedor não cadastrado no sistema");

                    if (fornecedor.TipoIntegradoraValePedagio == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Fornecedor do vale pedágio não esta configurado na Multisoftware");

                    tipoIntegracao = fornecedor.TipoIntegradoraValePedagio;
                }

                bool statusRecusadoOuSemCusto = valePedagio.Status == SituacaoValePedagio.Recusada || valePedagio.Status == SituacaoValePedagio.RotaSemCusto;

                if (tipoIntegracao == null && string.IsNullOrEmpty(valePedagio.NumeroRecibo) && statusRecusadoOuSemCusto && string.IsNullOrEmpty(valePedagio.CNPJFornecedor))
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.NaoPossuiIntegracao);

                if (tipoIntegracao == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Operadora de vale pedágio não encontrada.");

                _unitOfWork.Start();

                if (valePedagio.NaoPossuiValePedagio)
                {
                    exiteCarga.ProblemaIntegracaoValePedagio = false;
                    exiteCarga.PossuiPendencia = false;
                    exiteCarga.MotivoPendencia = "";
                    repositorioCarga.Atualizar(exiteCarga);
                    return Retorno<bool>.CriarRetornoSucesso(true, "Integração feita com sucesso");
                }

                if (valePedagio.VpCancelado)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagios = repCargaIntegracaoValePedagio.BuscarPorProtocoloCarga(valePedagio.ProtocoloCarga);

                    foreach (var atualValePedagio in listaValePedagios)
                    {
                        if (!(atualValePedagio.NumeroValePedagio == valePedagio.NumeroValePedagio && valePedagio.ValorValePedagio == atualValePedagio.ValorValePedagio))
                            continue;

                        atualValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                        repCargaIntegracaoValePedagio.Atualizar(atualValePedagio);
                    }
                    return Retorno<bool>.CriarRetornoSucesso(true, "Integração feita com sucesso");
                }

                if (exiteCarga.ProblemaIntegracaoValePedagio)
                {
                    exiteCarga.PossuiPendencia = false;
                    exiteCarga.ProblemaIntegracaoValePedagio = false;
                    exiteCarga.MotivoPendencia = "";
                    repositorioCarga.Atualizar(exiteCarga);
                }

                if (exiteCarga.AutorizouTodosCTes)
                    return Retorno<bool>.CriarRetornoExcecao("Na situação atual da carga não permite que seja integrado um novo vale pedagio");

                bool naoIncluirMDFe = false;

                if (fornecedor == null)
                {
                    fornecedor = exiteCarga.Pedidos.Where(p => p.Recebedor != null).Select(p => p.Pedido.Recebedor).FirstOrDefault();
                    naoIncluirMDFe = true;
                }

                if (fornecedor == null)
                    fornecedor = exiteCarga.Pedidos.Where(p => p.Pedido.Remetente != null).Select(p => p.Pedido.Remetente).FirstOrDefault();

                if (valePedagio.Status == SituacaoValePedagio.Reembolso)
                {

                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(valePedagio.CNPJPagador);
                    if (usuario == null)
                        throw new ServicoException($"Usuário não encontrado com o CPF: {valePedagio.CNPJPagador}");

                    ObjetoChamado valueObjChamado = MontarObjetoDeValorChamado(valePedagio, exiteCarga, usuario);

                    Servicos.Embarcador.Chamado.Chamado.AbrirChamado(valueObjChamado, usuario, _tipoServicoMultisoftware, _auditado, _unitOfWork);

                    naoIncluirMDFe = true;
                }

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio novaIntegracaoValepedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio()
                {
                    Carga = exiteCarga,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    SituacaoIntegracao = SituacaoIntegracao.Integrado,
                    NumeroValePedagio = valePedagio.NumeroValePedagio,
                    Observacao1 = valePedagio.Observacao,
                    SituacaoValePedagio = valePedagio.Status,
                    ProblemaIntegracao = "",
                    TipoIntegracao = tipoIntegracao,
                    ValorValePedagio = valePedagio.ValorValePedagio,
                    TipoPercursoVP = valePedagio.TipoPercursoVP
                };
                repCargaIntegracaoValePedagio.Inserir(novaIntegracaoValepedagio);

                Dominio.Entidades.Embarcador.Cargas.CargaValePedagio novoValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio()
                {
                    Carga = exiteCarga,
                    Valor = valePedagio.ValorValePedagio,
                    NumeroComprovante = valePedagio?.NumeroRecibo ?? string.Empty,
                    Responsavel = repClient.BuscarPorCPFCNPJ(valePedagio.CNPJPagador.ToDouble()),
                    NaoIncluirMDFe = naoIncluirMDFe,
                    Fornecedor = fornecedor,
                    TipoCompra = valePedagio.TipoCompra,
                    CargaIntegracaoValePedagio = novaIntegracaoValepedagio
                };

                repositorioCargaPedagio.Inserir(novoValePedagio);

                if (!string.IsNullOrEmpty(valePedagio.PDF))
                {
                    string nomeArquivo = Guid.NewGuid().ToString().Replace("-", "") + ".pdf";

                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(_unitOfWork);
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo repositorioAnexo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo(_unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo cargaValePedagioAnexo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo()
                    {
                        EntidadeAnexo = novaIntegracaoValepedagio,
                        NomeArquivo = nomeArquivo,
                        GuidArquivo = nomeArquivo,
                        Descricao = ""
                    };

                    repositorioAnexo.Inserir(cargaValePedagioAnexo);

                    string caminho = servicoValePedagio.ObterCaminhoPorTipoIntegracaoValePedagio(tipoIntegracao.Tipo.ObterDescricao().Replace(" ", ""));
                    byte[] imageBytes = Convert.FromBase64String(valePedagio.PDF);
                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(CaminhoArquivoPorTipoIntegracao(tipoIntegracao.Tipo, _unitOfWork) + nomeArquivo, imageBytes);
                }

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Integração feita com sucesso");
            }
            catch (ServicoException ex)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao Integrar Vale Pedágio");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion Métodos Publicos

        #region Métodos Privados

        private ObjetoChamado MontarObjetoDeValorChamado(Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario)
        {
            if (carga.TipoOperacao == null)
                throw new ServicoException($"Tipo de Operação inexistente para a Carga: {carga.CodigoCargaEmbarcador}");

            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(_unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupoMotivoChamado = repGrupoMotivoChamado.BuscarPorTipoOperacaoERecebeOcorrenciaERP(carga.TipoOperacao.Codigo);
            if (grupoMotivoChamado == null)
                throw new ServicoException($"Não existe um Grupo de Motivo de Atendimento cadastrado com o mesmo Tipo de Operação da Carga: {carga.TipoOperacao.Descricao}, e que esteja Configurado para receber Ocorrência do ERP");

            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPrimeiroPorGrupoMotivo(grupoMotivoChamado.Codigo);
            if (motivoChamado == null)
                throw new ServicoException($"Não existe nenhum Motivo de Atendimento cadastrado para o Grupo de Motivo de Atendimento: {grupoMotivoChamado.Descricao}");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPrimeiroPorNumeroStage(valePedagio.NumeroStage);
            if (pedido == null)
                throw new ServicoException($"Não existe nenhum Pedido cadastrado para o Numero de Stage: {valePedagio.NumeroStage}");

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPrimeiraPorPedidoECarga(pedido.Codigo, carga.Codigo);
            if (cargaEntrega == null)
                throw new ServicoException($"Não existe nenhuma Entrega/Coleta cadastrada para o Pedido: {pedido.CodigoCargaEmbarcador} da Carga: {carga.CodigoCargaEmbarcador}");

            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado valueObjChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
            {
                NumeroEmbarcador = valePedagio.NumeroRecibo,
                Carga = carga,
                CargaEntrega = cargaEntrega,
                GrupoMotivoChamado = grupoMotivoChamado,
                MotivoChamado = motivoChamado,
                Cliente = pedido?.Destinatario,
                Empresa = usuario?.Empresa,
            };

            return valueObjChamado;
        }

        private string CaminhoArquivoPorTipoIntegracao(TipoIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            string diretorioArquivos = string.Empty;

            diretorioArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivos, integracao.ObterDescricao().Replace(" ", ""));

            return caminhoArquivo;
        }

        #endregion Métodos Privados
    }
}
