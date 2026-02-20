using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Embarcador.CTe
{
    public class Averbacao
    {
        public async Task IntegrarAverbacoesPendentesAutorizacaoAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.IntegrarAverbacoesPendentesAutorizacao);

            List<int> averbacoes = servicoOrquestradorFila.Ordenar((limiteRegistros) => repAverbacaoCTe.BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao, limiteRegistros));

            List<Task> tasks = new List<Task>();
            foreach (int codigo in averbacoes)
                tasks.Add(Task.Run(() => ExecutaIntegrarAverbacoesPendentesAutorizacaoAsync(codigo, unitOfWork)));

            await Task.WhenAll(tasks);
        }

        private async Task ExecutaIntegrarAverbacoesPendentesAutorizacaoAsync(int codigoAverbacao, Repositorio.UnitOfWork unitOfWorkBase)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(unitOfWorkBase.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            int tentativas = 0;

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.IntegrarAverbacoesPendentesAutorizacao);

            try
            {
                svcCTe.EmitirAverbacaoOracle(codigoAverbacao, ref tentativas, unitOfWork, unitOfWork.StringConexao);
                
                servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoAverbacao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                servicoOrquestradorFila.RegistroComFalha(codigoAverbacao, excecao.Message);
            }

            unitOfWork.FlushAndClear();
        }


        public static void IntegrarAverbacoesPendentesAutorizacaoImportadoEmbarcador(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarAverbacoesPendentesEmbarcador(Dominio.Enumeradores.StatusAverbacaoCTe.Pendente, 100);

            foreach (var averbacao in averbacoes)
            {
                if (averbacao != null && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao)
                {
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;
                    repAverbacaoCTe.Atualizar(averbacao);
                    unitOfWork.CommitChanges();

                }
                else if (averbacao != null && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento)
                {
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgCancelamento;
                    repAverbacaoCTe.Atualizar(averbacao);
                    unitOfWork.CommitChanges();
                }
            }

            unitOfWork.FlushAndClear();

        }

        public static void IntegrarAverbacoesPendentesAutorizacaoInverso(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> averbacoes = repAverbacaoCTe.BuscarAverbacoesInverso(Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao, 500);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            int countAverbacoes = averbacoes.Count;

            int tentativas = 0;
            for (var i = 0; i < countAverbacoes; i++)
            {
                Servicos.Log.TratarErro("Inicio " + averbacoes[i]);
                svcCTe.EmitirAverbacaoOracle(averbacoes[i], ref tentativas, unitOfWork, stringConexao);
                if (tentativas > 2)
                    break;

                unitOfWork.FlushAndClear();
                Servicos.Log.TratarErro("Fim " + averbacoes[i]);
            }

            if (tentativas >= 2)
            {
                for (var i = 0; i < countAverbacoes; i++)
                {
                    Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigo(averbacoes[i]);
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da averbadora não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    averbacao.tentativasIntegracao = 0;
                    averbacao.DataRetorno = DateTime.Now;
                    repAverbacaoCTe.Atualizar(averbacao);
                }
            }


        }

        public static void IntegrarAverbacoesPendentesCancelamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

            List<int> averbacoes = repAverbacaoCTe.BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoCTe.AgCancelamento, 100);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            int countAverbacoes = averbacoes.Count;
            int tentativas = 0;

            for (var i = 0; i < countAverbacoes; i++)
            {
                svcCTe.CancelarAverbacaoOracle(averbacoes[i], ref tentativas, unitOfWork, stringConexao);
                if (tentativas > 2)
                    break;
            }

            if (tentativas >= 2)
            {
                for (var i = 0; i < countAverbacoes; i++)
                {
                    Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigo(averbacoes[i]);
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da averbadora não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    averbacao.tentativasIntegracao = 0;
                    averbacao.DataRetorno = DateTime.Now;
                    repAverbacaoCTe.Atualizar(averbacao);
                }
            }

        }
    }
}
