using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Transportadores
{
    public class MotoristaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>
    {
        #region Construtores

        public MotoristaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MotoristaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> Consultar(List<int> codigosMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaMotoristaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>()
                .Where(o => codigosMotorista.Contains(o.Motorista.Codigo));

            if (situacao.HasValue)
                consultaMotoristaIntegracao = consultaMotoristaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaMotoristaIntegracao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao BuscarPorCodigo(int codigo)
        {
            var motoristaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motoristaIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> Consultar(List<int> codigosMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigosMotorista, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(List<int> codigosMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigosMotorista, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarPendentesIntegracaoPorTipo(int numeroTentativas, double minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarPendentesIntegracaoPorTipos(int numeroTentativas, double minutosACadaTentativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && tiposIntegracao.Contains(obj.TipoIntegracao.Tipo) && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarIntegracaoAguardandoRetornoPorTiposEDataHora(int numeroTentativas, double minutosACadaTentativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno &&
                                obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa) &&
                                obj.NumeroTentativas < numeroTentativas
                            )
                            && tiposIntegracao.Contains(obj.TipoIntegracao.Tipo) && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarPorMotorista(int motora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query where obj.Motorista.Codigo == motora select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarPorMotoristas(List<int> motoristas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query where motoristas.Contains(obj.Motorista.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao BuscarPorMotoristaETipo(int motorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query where obj.Motorista.Codigo == motorista && obj.TipoIntegracao.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> BuscarPendentesIntegracaoPorDataHora(DateTime dataParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query
                         where
                              obj.DataIntegracao <= dataParametro
                              && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public int ContarPorMotoristaETipo(int motorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query where obj.Motorista.Codigo == motorista && obj.TipoIntegracao.Tipo == tipo && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado select obj;

            return result.Count();
        }

        public bool PossuiIntegracaoPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.Any();
        }

        public bool PossuiIntegracaoPorMotoristaETipo(int motorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

            var result = from obj in query where obj.Motorista.Codigo == motorista && obj.TipoIntegracao.Tipo == tipo select obj;

            return result.Any();
        }

        #endregion Métodos Públicos
    }
}
