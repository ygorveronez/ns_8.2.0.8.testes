using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosEmissaoSeguroController : BaseController
    {
        #region Construtores

        public DadosEmissaoSeguroController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> AutorizarSeguro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarSeguro) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                unitOfWork.Start();

                Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga repApoliceSeguraAutorizacaoCarga = new Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga> apolicesSeguroAutorizacaoCarga = repApoliceSeguraAutorizacaoCarga.BuscarPorCargaESituacao(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.AgLiberacao);
                foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga apoliceSeguraAutorizacaoCarga in apolicesSeguroAutorizacaoCarga)
                {
                    apoliceSeguraAutorizacaoCarga.SituacaoAutorizacaoApolice = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.Liberada;
                    apoliceSeguraAutorizacaoCarga.Usuario = this.Usuario;
                    apoliceSeguraAutorizacaoCarga.DataHora = DateTime.Now;

                    repApoliceSeguraAutorizacaoCarga.Atualizar(apoliceSeguraAutorizacaoCarga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguraAutorizacaoCarga, null, "Autorizou o Seguro da Carga", unitOfWork);
                }

                if (apolicesSeguroAutorizacaoCarga.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apolicesSeguroAutorizacaoCarga.FirstOrDefault().Carga, null, "Autorizou o Seguro", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações de seguro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarSeguro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarDadosSeguro) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Servicos.Embarcador.Seguro.Seguro serSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados do seguro na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.BloquearAjusteConfiguracoesFreteCarga ?? false)
                    return new JsonpResult(false, true, "A operação não permite que estas informações sejam alteradas manualmente.");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                dynamic configuracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoEmissaoCTe"));

                unitOfWork.Start();

                if (carga.CargaAgrupada)
                    carga.AgIntegracaoAgrupamentoCarga = true;

                serSeguro.ExtornarAutorizacoesApolices(carga, unitOfWork, "Os seguros da carga foram atualizados pelo operador " + this.Usuario.Nome + ". ");

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                {
                    // Limpa todos seguros
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> excluirSeguros = cargaPedido.ApoliceSeguroAverbacao.ToList();

                    for (int i = 0; i < excluirSeguros.Count; i++)
                        repApoliceSeguroAverbacao.Deletar(excluirSeguros[i], Auditado);

                    for (int i = 0; i < configuracao.ApolicesSeguro.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = repApoliceSeguro.BuscarPorCodigo((int)configuracao.ApolicesSeguro[i]);
                        Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceAverbacao = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                        {
                            ApoliceSeguro = apolice,
                            CargaPedido = cargaPedido,
                            SeguroFilialEmissora = carga.EmpresaFilialEmissora != null,
                            Desconto = null
                        };

                        repApoliceSeguroAverbacao.Inserir(apoliceAverbacao, Auditado);

                        if (!apolicesSeguro.Contains(apolice))
                            apolicesSeguro.Add(apolice);
                    }
                }

                if (configuracao.ApolicesSeguro.Count > 0)
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = true;
                    carga.ApoliceSeguroInformadaManualmente = true;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = ((ConfiguracaoEmbarcador.AverbarMDFe && !configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT) || (ConfiguracaoEmbarcador.AverbarMDFe && configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT && repositorioCargaCIOT.ExisteCIOTPorCarga(carga.Codigo)));
                }
                else
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = false;
                    carga.ApoliceSeguroInformadaManualmente = false;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = false;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                serSeguro.VerificarSeNecessariaAutorizacaoSeguro(carga, cargaPedidos, unitOfWork);

                decimal valorDescontoSeguro = carga.DescontoSeguro;
                decimal percentualDescontoSeguro = carga.PercentualDescontoSeguro;

                Servicos.Embarcador.Seguro.Seguro.InformarValorSeguroCarga(carga, apolicesSeguro, carga.ModeloVeicularCarga, unitOfWork);

                if (valorDescontoSeguro != carga.DescontoSeguro || percentualDescontoSeguro != carga.PercentualDescontoSeguro)
                {
                    carga.DataInicioCalculoFrete = DateTime.Now;
                    carga.CalculandoFrete = true;
                }

                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Atualizou o Seguro da Carga", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações de seguro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
