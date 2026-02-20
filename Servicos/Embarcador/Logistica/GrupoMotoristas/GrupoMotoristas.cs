using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristas : Abstrato.ServicoAbstratoGrupoMotoristas
    {
        #region Construtores
        public GrupoMotoristas(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
            : base(unitOfWork, cancelationToken, Auditado) { }

        #endregion

        #region Métodos Públicos

        public async Task CriarGrupoMotoristas(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.CriacaoGrupoMotoristas criacaoGrupoMotoristas)
        {
            if (!await SalvarGrupoMotoristas(criacaoGrupoMotoristas))
                throw new Dominio.Excecoes.Embarcador.ServicoException("Entidade de GrupoMotoristas chegou nula em sua criação.");

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario> grupoMotoristasFuncionarios = await CriarMuitosGrupoMotoristasFuncionarios(criacaoGrupoMotoristas);
            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes = await CriarMuitosGrupoMotoristasTipoIntegracao(criacaoGrupoMotoristas);

            if (grupoMotoristasTipoIntegracoes.Count > 0)
                await CriarIntegracoes(grupoMotoristasTipoIntegracoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista.Criar);
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas> AtualizarGrupoMotoristas(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.AtualizacaoGrupoMotoristas atualizacaoGrupoMotoristas)
        {
            bool houveAlteracao = await AtualizarFuncionarios(atualizacaoGrupoMotoristas.ResultadoConsulta, atualizacaoGrupoMotoristas.Motoristas);

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes =
                await AtualizarTipoIntegracao(atualizacaoGrupoMotoristas.ResultadoConsulta, atualizacaoGrupoMotoristas.TiposIntegracao);

            if (grupoMotoristasTipoIntegracoes.Count > 0)
            {
                atualizacaoGrupoMotoristas.GrupoMotoristas.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas.AguardandoIntegracoes;
                await CriarIntegracoes(grupoMotoristasTipoIntegracoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista.Criar);
            }

            if (houveAlteracao && atualizacaoGrupoMotoristas.ResultadoConsulta.TiposIntegracao.Any(a => a == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyGrupoMotorista))
            {
                atualizacaoGrupoMotoristas.GrupoMotoristas.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas.AguardandoIntegracoes;

                List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristaIntegracao_Atualizacao = new List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao>();
                grupoMotoristaIntegracao_Atualizacao.Add(new Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao()
                {
                    GrupoMotoristas = atualizacaoGrupoMotoristas.GrupoMotoristas,
                    TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyGrupoMotorista
                });

                await CriarIntegracoes(grupoMotoristaIntegracao_Atualizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista.Atualizar);
            }


            await SalvarGrupoMotoristas(atualizacaoGrupoMotoristas);

            return new(
                atualizacaoGrupoMotoristas.GrupoMotoristas,
                atualizacaoGrupoMotoristas.Motoristas,
                atualizacaoGrupoMotoristas.TiposIntegracao);
        }


        public async Task VerificarIntegracoesGrupoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread identificadorControlePosicaoThread, CancellationToken cancellationToken)
        {
            _unitOfWork.FlushAndClear();

            try
            {
                Servicos.Log.TratarErro("Inicio Buscando Integrações Aguardando", "GrupoMotorista");

                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, identificadorControlePosicaoThread);

                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao(_unitOfWork, cancellationToken);
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy servIntegracaoTrizyOfertas = new Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy(_unitOfWork);

                List<int> listaIntegracoesAguardando = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioGrupoMotoristasIntegracao.BuscarIntegracoesAguardandoAsync(limiteRegistros).GetAwaiter().GetResult());

                Servicos.Log.TratarErro("Integrações Aguardando: " + listaIntegracoesAguardando.Count + "", "GrupoMotorista");

                List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> integracoes = repositorioGrupoMotoristasIntegracao.BuscarPorCodigos(listaIntegracoesAguardando, false);

                if (integracoes != null && integracoes.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao integracao in integracoes)
                    {
                        if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyGrupoMotorista)
                        {
                            switch (integracao.Tipo)
                            {
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista.Criar:
                                    await servIntegracaoTrizyOfertas.IntegrarCriacaoGrupoMotoristaAsync(integracao, cancellationToken);
                                    break;

                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista.Atualizar:
                                    await servIntegracaoTrizyOfertas.IntegrarAtualizacaoGrupoMotoristaAsync(integracao, cancellationToken);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            await servIntegracaoTrizyOfertas.IntegracaoNaoImplementadaAsync(integracao, cancellationToken);
                        }
                    }
                }

                Servicos.Log.TratarErro("Fim Integrações Aguardando", "GrupoMotorista");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }


        public async Task ExcluirGrupoMotoristas(Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas grupoMotoristasAExcluir, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new(_unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> integracoesExistentes = await repositorioGrupoMotoristasIntegracao.BuscarAsync(grupoMotoristasAExcluir.GrupoMotoristas.Codigo);

            if (integracoesExistentes.Count > 0)
                throw new ServicoException("Não é possível excluir esse Grupo Transportador, tente desativá-lo.");

            if (grupoMotoristasAExcluir.Funcionarios.Count > 0)
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario repositorioGrupoMotoristasFuncionario = new(_unitOfWork, cancellationToken);
                await repositorioGrupoMotoristasFuncionario.ExcluirFuncionariosGrupoMotoristasAsync(grupoMotoristasAExcluir.GrupoMotoristas.Codigo, cancellationToken);
            }

            if (grupoMotoristasAExcluir.TiposIntegracao.Count > 0)
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasTipoIntegracao repositorioGrupoMotoristasTipoIntegracao = new(_unitOfWork, cancellationToken);
                await repositorioGrupoMotoristasTipoIntegracao.ExcluirTipoIntegracaoGrupoMotoristasAsync(grupoMotoristasAExcluir.GrupoMotoristas.Codigo, cancellationToken);
            }

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new(_unitOfWork, cancellationToken);
            await repositorioGrupoMotoristas.DeletarAsync(grupoMotoristasAExcluir.GrupoMotoristas, _auditado);
        }

        #endregion

        #region Métodos Privados

        private async Task CriarIntegracoes(List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista tipo)
        {
            GrupoMotoristasIntegracao servicoIntegracao = new(_unitOfWork, _cancellationToken);

            await servicoIntegracao.CriarGrupoMotoristasIntegracoes(grupoMotoristasTipoIntegracoes, tipo);
        }

        private async Task<bool> SalvarGrupoMotoristas(Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.ISalvamentoGrupoMotoristas obj)
        {
            if (obj == null || obj.GrupoMotoristas == null)
                return false;

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new(_unitOfWork, _cancellationToken);

            if (obj.TiposIntegracao.Count < 1)
            {
                obj.GrupoMotoristas.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas.Finalizado;
            }

            if (obj.GrupoMotoristas.Codigo == 0)
            {
                await repositorioGrupoMotoristas.InserirAsync(obj.GrupoMotoristas, _auditado);
            }
            else
            {
                await repositorioGrupoMotoristas.AtualizarAsync(obj.GrupoMotoristas, _auditado);
            }

            return true;
        }

        private async Task<List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario>> CriarMuitosGrupoMotoristasFuncionarios(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.CriacaoGrupoMotoristas criacaoGrupoMotoristas)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario repositorioGrupoMotoristasFuncionario = new(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario> grupoMotoristasFuncionarios = new();

            for (int i = 0; i < criacaoGrupoMotoristas.Motoristas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario grupoMotoristasFuncionario = new()
                {
                    GrupoMotoristas = criacaoGrupoMotoristas.GrupoMotoristas,
                    Funcionario = new Dominio.Entidades.Usuario()
                    {
                        Codigo = criacaoGrupoMotoristas.Motoristas[i].Codigo
                    }
                };

                grupoMotoristasFuncionarios.Add(grupoMotoristasFuncionario);
            }

            await repositorioGrupoMotoristasFuncionario.InserirMuitosAsync(grupoMotoristasFuncionarios);

            return grupoMotoristasFuncionarios;
        }

        private async Task<List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao>> CriarMuitosGrupoMotoristasTipoIntegracao(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.CriacaoGrupoMotoristas criacaoGrupoMotoristas)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasTipoIntegracao repositorioGrupoMotoristasTipoIntegracao = new(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes = new();
            for (int i = 0; i < criacaoGrupoMotoristas.TiposIntegracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao grupoMotoristasTipoIntegracao = new()
                {
                    GrupoMotoristas = criacaoGrupoMotoristas.GrupoMotoristas,
                    TipoIntegracao = criacaoGrupoMotoristas.TiposIntegracao[i]
                };
                grupoMotoristasTipoIntegracoes.Add(grupoMotoristasTipoIntegracao);
            }
            await repositorioGrupoMotoristasTipoIntegracao.InserirMuitosAsync(grupoMotoristasTipoIntegracoes);

            return grupoMotoristasTipoIntegracoes;
        }

        private async Task<bool> AtualizarFuncionarios(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas resultadoConsulta,
            IEnumerable<Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IRelacionamentoGrupoMotoristas> novosFuncionarios)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario repositorio = new(_unitOfWork, _cancellationToken);

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao repositorioFuncionarioAlteracao = new(_unitOfWork, _cancellationToken);

            bool houveAlteracao = false;

            async Task inserir(Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IRelacionamentoGrupoMotoristas relacionamento)
            {
                Dominio.Entidades.Usuario funcionario = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                    new()
                    {
                        Funcionario = funcionario,
                        GrupoMotoristas = resultadoConsulta.GrupoMotoristas,
                    },
                    _auditado
                );
            }


            async Task inserirAlteracaoDeletar(Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IRelacionamentoGrupoMotoristas relacionamento)
            {
                try
                {
                    houveAlteracao = true;

                    Dominio.Entidades.Usuario funcionario = new() { Codigo = relacionamento.Codigo };

                    await repositorioFuncionarioAlteracao.InserirAsync(
                        new()
                        {
                            Funcionario = funcionario,
                            GrupoMotoristas = resultadoConsulta.GrupoMotoristas,
                            Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoMotoristaAtualizarAcao.Deletar
                        }, _auditado
                    );
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex);
                }

            }

            async Task inserirAlteracaoAdicionar(Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IRelacionamentoGrupoMotoristas relacionamento)
            {
                houveAlteracao = true;

                Dominio.Entidades.Usuario funcionario = new() { Codigo = relacionamento.Codigo };

                await repositorioFuncionarioAlteracao.InserirAsync(
                    new()
                    {
                        Funcionario = funcionario,
                        GrupoMotoristas = resultadoConsulta.GrupoMotoristas,
                        Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoMotoristaAtualizarAcao.Adicionar
                    }, _auditado
                );
            }

            resultadoConsulta.GrupoMotoristas.SetExternalChanges(await AtualizarRelacionamento(repositorio, resultadoConsulta.Funcionarios, novosFuncionarios, inserir, inserirAlteracaoDeletar, inserirAlteracaoAdicionar));

            return houveAlteracao;
        }

        private async Task<List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao>> AtualizarTipoIntegracao(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.RetornoGrupoMotoristas resultadoConsulta,
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> novosTiposIntegracao)
        {
            var atual = resultadoConsulta.TiposIntegracao;

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasTipoIntegracao repoGrupoMotoristasTipoIntegracao = new(_unitOfWork, _cancellationToken);

            var aInserir = novosTiposIntegracao.Except(atual).ToList();
            var aRemover = atual.Except(novosTiposIntegracao).ToList();

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> entidadesInseridas = new();
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new();

            for (int i = 0; i < aInserir.Count; i++)
            {
                Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao entidade = new()
                {
                    TipoIntegracao = aInserir[i],
                    GrupoMotoristas = resultadoConsulta.GrupoMotoristas
                };

                await repoGrupoMotoristasTipoIntegracao.InserirAsync(
                    entidade,
                    _auditado
                );

                entidadesInseridas.Add(entidade);
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Integração",
                    De = "",
                    Para = entidade.Descricao
                });
            }

            for (int i = 0; i < aRemover.Count; i++)
            {
                await repoGrupoMotoristasTipoIntegracao.DeletarPorTipo(aRemover[i], resultadoConsulta.GrupoMotoristas.Codigo);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Integração",
                    De = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(aRemover[i]),
                    Para = ""
                });
            }


            resultadoConsulta.GrupoMotoristas.SetExternalChanges(alteracoes);

            return entidadesInseridas;
        }

        #endregion
    }
}
