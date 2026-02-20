using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class ComissaoFuncionario : RepositorioBase<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>
    {
        public ComissaoFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissaoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();
            var result = from obj in query where obj.SituacaoComissaoFuncionario == situacaoComissaoFuncionario select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> BuscarPorSituacaoEUsuario(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissaoFuncionario, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();
            var result = from obj in query where obj.SituacaoComissaoFuncionario == situacaoComissaoFuncionario && obj.UsuarioGerouComissao.Codigo == usuario select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionario VerificarComissaoPeriodoJaExiste(DateTime dataInicio, DateTime dataFim, int motorista, int cargo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();
            var result = from obj in query
                         where obj.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada &&
                         ((obj.DataInicio >= dataInicio && obj.DataInicio <= dataFim)
                         || (obj.DataFim >= dataInicio && obj.DataFim <= dataFim)
                         || (dataInicio >= obj.DataInicio && dataInicio <= obj.DataFim)
                         || (dataFim >= obj.DataInicio && dataFim <= obj.DataFim)
                         || obj.DataInicio == dataInicio
                         || obj.DataFim == dataInicio
                         || obj.DataInicio == dataFim
                         || obj.DataFim == dataFim)
                         select obj;

            if (motorista > 0 && cargo > 0)
                result = result.Where(obj => obj.Motorista.Codigo == motorista && obj.CargoMotorista.Codigo == cargo);
            else if (motorista > 0 && cargo == 0)
                result = result.Where(obj => obj.Motorista.Codigo == motorista && obj.CargoMotorista == null);
            else if (motorista == 0 && cargo > 0)
                result = result.Where(obj => obj.CargoMotorista.Codigo == motorista && obj.Motorista == null);
            else
                result = result.Where(obj => obj.CargoMotorista == null && obj.Motorista == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissaoFuncionario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();

            var result = from obj in query select obj;

            if (situacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.todos)
                result = result.Where(obj => obj.SituacaoComissaoFuncionario == situacaoComissaoFuncionario);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicio >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataFim <= dataFim);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissaoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>();

            var result = from obj in query select obj;

            if (situacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.todos)
                result = result.Where(obj => obj.SituacaoComissaoFuncionario == situacaoComissaoFuncionario);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicio >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataFim <= dataFim);

            return result.Count();
        }

    }
}
