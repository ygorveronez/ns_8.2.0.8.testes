using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class PrevisaoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>
    {
        public PrevisaoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> BuscarPorExcecao(int codigoExcecao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.ExcecaoCapacidadeCarregamento.Codigo == codigoExcecao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> BuscarPorExcecao(int codigoExcecao, int codigoRota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.ExcecaoCapacidadeCarregamento.Codigo == codigoExcecao && obj.Rota.Codigo == codigoRota select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento BuscarPorExcecao(int codigoExcecao, int codigoRota, int modeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.ExcecaoCapacidadeCarregamento.Codigo == codigoExcecao && obj.Rota.Codigo == codigoRota && obj.ModelosVeiculos.Any(o => o.Codigo == modeloVeicular) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> BuscarPorCentroCarregamento(int codigoCentroCarregamento, int codigoRota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento && obj.Rota.Codigo == codigoRota && obj.Dia == diaSemana select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento BuscarPorCentroCarregamento(int codigoCentroCarregamento, int codigoRota, int modeloVeicular, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento && obj.Rota.Codigo == codigoRota && obj.Dia == diaSemana && obj.ModelosVeiculos.Any(o => o.Codigo == modeloVeicular) select obj;

            return result.FirstOrDefault();
        }

        public bool VerificarSePossuiRestricaoPorCentroCarregamentoNaRota(int codigoCentroCarregamento, int codigoRota, int modeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento && obj.Rota.Codigo == codigoRota && obj.ModelosVeiculos.Any(o => o.Codigo == modeloVeicular) select obj;

            return result.Any();
        }

        public bool VerificarSePossuiRestricaoPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> ConsultaPrevisoes(int centroCarregamento, int codigoExcecao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana, string descricaoRota, string descricaoPrevisao)
        {
            var consultaPrevisaoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            if (codigoExcecao > 0)
                consultaPrevisaoCarregamento = consultaPrevisaoCarregamento.Where(o => o.ExcecaoCapacidadeCarregamento.Codigo == codigoExcecao);
            else
            {
                if (centroCarregamento > 0)
                    consultaPrevisaoCarregamento = consultaPrevisaoCarregamento.Where(o => o.CentroCarregamento.Codigo == centroCarregamento);

                if (diaSemana != null)
                    consultaPrevisaoCarregamento = consultaPrevisaoCarregamento.Where(o => o.Dia == diaSemana.Value);
            }

            if (!string.IsNullOrWhiteSpace(descricaoRota))
                consultaPrevisaoCarregamento = consultaPrevisaoCarregamento.Where(o => o.Rota.Descricao.Contains(descricaoRota));

            if (!string.IsNullOrWhiteSpace(descricaoPrevisao))
                consultaPrevisaoCarregamento = consultaPrevisaoCarregamento.Where(o => o.Descricao.Contains(descricaoPrevisao));

            return consultaPrevisaoCarregamento.ToList();
        }
    }
}
