using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Feriado : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Feriado>
    {
        #region Construtores

        public Feriado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Feriado> Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado? tipoFeriado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string estado, int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Feriado>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao.Contains(codigoIntegracao));

            if (tipoFeriado.HasValue)
                query = query.Where(o => o.Tipo == tipoFeriado);

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else
                    query = query.Where(o => !o.Ativo);
            }

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(o => o.Estado.Sigla == estado);

            if (codigoLocalidade > 0)
                query = query.Where(o => o.Localidade.Codigo == codigoLocalidade);

            return query;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Configuracoes.Feriado> BuscarAtivos(DateTime dataInicial, DateTime dataFinal, int codigoLocalidade, string siglaEstado)
        {
            int diferencaAno = dataFinal.Year - dataInicial.Year;

            var consultaFeriado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Feriado>()
                .Where(o =>
                    o.Ativo &&
                    (o.Localidade == null || o.Localidade.Codigo == codigoLocalidade) &&
                    (o.Estado == null || o.Estado.Sigla == siglaEstado) &&
                    (
                        (
                            (o.Ano != null) &&
                            (o.Ano > dataInicial.Year || (o.Ano == dataInicial.Year && (o.Mes > dataInicial.Month || (o.Mes == dataInicial.Month && o.Dia >= dataInicial.Day)))) &&
                            (o.Ano < dataFinal.Year || (o.Ano == dataFinal.Year && (o.Mes < dataFinal.Month || (o.Mes == dataFinal.Month && o.Dia <= dataFinal.Day))))
                        ) ||
                        (
                            (o.Ano == null) &&
                            (
                                (diferencaAno > 1) ||
                                (diferencaAno == 1 && ((o.Mes > dataInicial.Month || (o.Mes == dataInicial.Month && o.Dia >= dataInicial.Day)) || (o.Mes < dataFinal.Month || (o.Mes == dataFinal.Month && o.Dia <= dataFinal.Day)))) ||
                                (diferencaAno == 0 && ((o.Mes > dataInicial.Month || (o.Mes == dataInicial.Month && o.Dia >= dataInicial.Day)) && (o.Mes < dataFinal.Month || (o.Mes == dataFinal.Month && o.Dia <= dataFinal.Day))))
                            )
                        )
                    )
                );

            return consultaFeriado.ToList();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.Feriado BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var consultaFeriado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Feriado>()
                .Where(o => o.CodigoIntegracao.Contains(codigoIntegracao));

            return consultaFeriado.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.Feriado> Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado? tipoFeriado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string estado, int codigoLocalidade, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Feriado> query = Consultar(codigoIntegracao, descricao, tipoFeriado, ativo, estado, codigoLocalidade);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado? tipoFeriado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string estado, int codigoLocalidade)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Feriado> query = Consultar(codigoIntegracao, descricao, tipoFeriado, ativo, estado, codigoLocalidade);

            return query.Count();
        }

        public bool VerificarSeExisteFeriado(DateTime data, int codigoLocalidade, string siglaEstado)
        {
            var consultaFeriado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Feriado>()
                .Where(o =>
                    o.Ativo &&
                    (o.Localidade == null || o.Localidade.Codigo == codigoLocalidade) &&
                    (o.Estado == null || o.Estado.Sigla == siglaEstado) &&
                    (o.Dia == data.Day) &&
                    (o.Mes == data.Month) &&
                    (o.Ano == null || o.Ano == data.Year)
                );

            return consultaFeriado.Any();
        }

        #endregion Métodos Públicos
    }
}
