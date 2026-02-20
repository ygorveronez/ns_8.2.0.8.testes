using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.RegraAutorizacao
{
    public class AprovacaoAlcada<TAprovacao, TRegra, TOrigemAprovacao> : RepositorioBase<TAprovacao>
        where TAprovacao : Dominio.Entidades.Embarcador.RegraAutorizacao.AprovacaoAlcada<TOrigemAprovacao, TRegra>
        where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
        where TOrigemAprovacao : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Construtores

        public AprovacaoAlcada(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AprovacaoAlcada(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<TAprovacao> ConsultarAutorizacoes(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                 .Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            return aprovacoes;
        }

        private IQueryable<TAprovacao> ConsultarAutorizacoesDesbloqueadas(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao => (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) && !aprovacao.Bloqueada);

            return aprovacoes;
        }

        #endregion

        #region Métodos Públicos

        public bool ContemAlcadaPendente(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>().Where(o => o.OrigemAprovacao.Codigo == codigoOrigem && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return aprovacoes.Any();
        }

        public List<TAprovacao> BuscarDesbloqueada(int codigoOrigem, int codigoUsuario)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(o => !o.Bloqueada && o.OrigemAprovacao.Codigo == codigoOrigem);

            if (codigoUsuario > 0)
                aprovacoes = aprovacoes.Where(aprovacao => aprovacao.Usuario.Codigo == codigoUsuario);

            return aprovacoes.ToList();
        }

        public List<TAprovacao> BuscarTodos(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>().Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            return aprovacoes.ToList();
        }

        public List<TAprovacao> BuscarPendentes(int codigoOrigem, int codigoUsuario)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Usuario.Codigo == codigoUsuario) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente) &&
                    !aprovacao.Bloqueada
                )
                .ToList();

            return aprovacoes;
        }

        public List<TAprovacao> BuscarPendentesBloqueadas(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente) &&
                    aprovacao.Bloqueada
                )
                .ToList();

            return aprovacoes;
        }

        public TAprovacao BuscarPorCodigo(int codigo)
        {
            var aprovacaoAlcada = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao => aprovacao.Codigo == codigo)
                .FirstOrDefault();

            return aprovacaoAlcada;
        }
        public async Task<TAprovacao> BuscarPorCodigoAsync(int codigo)
        {
            var aprovacaoAlcada = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao => aprovacao.Codigo == codigo)
                .FirstOrDefaultAsync();

            return await aprovacaoAlcada;
        }

        public List<TAprovacao> BuscarPorCodigos(List<int> codigos)
        {
            var aprovacoesAlcada = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao => codigos.Contains(aprovacao.OrigemAprovacao.Codigo)).ToList();

            return aprovacoesAlcada;
        }

        public int BuscarNumeroAprovacoesNecessariasPorRegra(int codigoOrigem, int codigoRegra)
        {
            var aprovacoes = ConsultarAutorizacoesDesbloqueadas(codigoOrigem);

            int numeroAprovacoesNecessarias = aprovacoes
                .Where(aprovacao => aprovacao.RegraAutorizacao.Codigo == codigoRegra)
                .Select(aprovacao => aprovacao.NumeroAprovadores)
                .FirstOrDefault();

            return numeroAprovacoesNecessarias;
        }

        public List<TRegra> BuscarRegrasDesbloqueadas(int codigoOrigem)
        {
            var aprovacoes = ConsultarAutorizacoesDesbloqueadas(codigoOrigem);
            var regras = aprovacoes
                .Where(aprovacao => aprovacao.RegraAutorizacao != null)
                .Select(aprovacao => aprovacao.RegraAutorizacao)
                .Distinct()
                .ToList();

            return regras;
        }

        public List<TAprovacao> ConsultarAutorizacoes(int codigoOrigem, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var aprovacoes = ConsultarAutorizacoes(codigoOrigem);

            return ObterLista(aprovacoes, parametrosConsulta);
        }

        public int ContarAprovacoesNecessarias(int codigoOrigem)
        {
            var regrasAutorizacao = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(o => (o.OrigemAprovacao.Codigo == codigoOrigem) && (o.RegraAutorizacao != null))
                .Select(aprovacao => new { aprovacao.RegraAutorizacao, aprovacao.NumeroAprovadores })
                .Distinct()
                .ToList();

            int numeroAprovacoesNecessarias = regrasAutorizacao.Sum(o => o.NumeroAprovadores);

            return numeroAprovacoesNecessarias;
        }

        public async Task<int> ContarAprovacoesNecessariasAsync(int codigoOrigem)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<TAprovacao>();
            var regrasAutorizacao = await queryAutorizacao.Where(o => (o.OrigemAprovacao.Codigo == codigoOrigem) && (o.RegraAutorizacao != null))
                .Select(aprovacao => new { aprovacao.RegraAutorizacao, aprovacao.NumeroAprovadores })
                .Distinct()
                .ToListAsync(CancellationToken);

            var resultado = regrasAutorizacao.Sum(o => o.NumeroAprovadores);
            return resultado;
        }

        public int ContarAprovacoes(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                );

            return aprovacoes.Count();
        }

        public Task<int> ContarAprovacoesAsync(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                );

            return aprovacoes.CountAsync(CancellationToken);
        }

        public int ContarAprovacoes(int codigoOrigem, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    ((aprovacao.RegraAutorizacao == null) || (aprovacao.RegraAutorizacao.Codigo == codigoRegra)) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                );

            return aprovacoes.Count();
        }

        public int ContarAutorizacoes(int codigoOrigem)
        {
            var aprovacoes = ConsultarAutorizacoes(codigoOrigem);

            return aprovacoes.Count();
        }

        public int ContarReprovacoes(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.Count();
        }

        public Task<int> ContarReprovacoesAsync(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.CountAsync(CancellationToken);
        }
        public int ContarReprovacoes(int codigoOrigem, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<TAprovacao>()
                .Where(aprovacao =>
                    (aprovacao.OrigemAprovacao.Codigo == codigoOrigem) &&
                    ((aprovacao.RegraAutorizacao == null) || (aprovacao.RegraAutorizacao.Codigo == codigoRegra)) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.Count();
        }

        public void DeletarPorOrigemAprovacao(int codigo)
        {
            try
            {
                string nomeClasseAprovacao = typeof(TAprovacao).Name;

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete {nomeClasseAprovacao} aprovacao where aprovacao.OrigemAprovacao.Codigo = :codigo ") // SQL-INJECTION-SAFE
                        .SetInt32("codigo", codigo)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete {nomeClasseAprovacao} aprovacao where aprovacao.OrigemAprovacao.Codigo = :codigo ") // SQL-INJECTION-SAFE
                            .SetInt32("codigo", codigo)
                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion
    }
}
