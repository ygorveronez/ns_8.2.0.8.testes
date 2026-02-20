using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>
    {
        #region Construtores

        public VeiculoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public VeiculoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> Consultar(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaVeiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>()
                .Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (situacao.HasValue)
                consultaVeiculoIntegracao = consultaVeiculoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaVeiculoIntegracao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPorVeiculo(int veiculo)
        {
            var veiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>()
                .Where(o => o.Veiculo.Codigo == veiculo)
                .ToList();

            return veiculoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPorVeiculos(List<int> veiculos)
        {
            var veiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>()
                .Where(o => veiculos.Contains(o.Veiculo.Codigo))
                .ToList();

            return veiculoIntegracao;
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao BuscarPorVeiculoETipo(int veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var veiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>()
                .Where(o => o.Veiculo.Codigo == veiculo && o.TipoIntegracao.Tipo == tipo)
                .FirstOrDefault();

            return veiculoIntegracao;
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao BuscarPorVeiculoETipoIntegracao(int veiculo, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            query = query.Where(o => o.Veiculo.Codigo == veiculo && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao BuscarPorCodigo(int codigo)
        {
            var veiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return veiculoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> Consultar(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoVeiculo, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoVeiculo, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPendentesIntegracaoPorTipo(int numeroTentativas, double minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                        (
                                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                            && obj.NumeroTentativas < numeroTentativas
                                            && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                        )) && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo);

            return query.Fetch(o => o.GrupoPessoas).Fetch(o => o.Veiculo).OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPendentesIntegracaoPorTipos(int numeroTentativas, double minutosACadaTentativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                        (
                                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                            && obj.NumeroTentativas < numeroTentativas
                                            && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                        )) && tipos.Contains(obj.TipoIntegracao.Tipo) && obj.TipoIntegracao.Ativo);

            return query.Fetch(o => o.GrupoPessoas).Fetch(o => o.Veiculo).OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarIntegracaoAguardandoRetornoPorTiposEDataHora(int numeroTentativas, double minutosACadaTentativa, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

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

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPendentesIntegracaoPorDataHora(DateTime dataParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            var result = from obj in query
                         where
                            obj.DataIntegracao <= dataParametro
                            && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> BuscarPendentesIntegracaoTrocaMotorista(int numeroTentativas, double minutosACadaTentativa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < numeroTentativas &&
                                        obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))) &&
                                        obj.TipoIntegracao.Ativo &&
                                        obj.TipoIntegracao.IntegrarVeiculoTrocaMotorista);

            return query.Fetch(o => o.GrupoPessoas).Fetch(o => o.Veiculo).OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public bool PossuiIntegracaoPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.Any();
        }

        public bool PossuiIntegracaoVeiculoeTipo(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            var result = query.Where(o => o.Veiculo.Codigo == codigoVeiculo && o.TipoIntegracao.Tipo == tipo);

            return result.Any();
        }

        public bool PossuiIntegracaoVeiculoeTipos(int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();

            var result = query.Where(o => o.Veiculo.Codigo == codigoVeiculo && tipos.Contains(o.TipoIntegracao.Tipo));

            return result.Any();
        }

        #endregion Métodos Públicos
    }
}
