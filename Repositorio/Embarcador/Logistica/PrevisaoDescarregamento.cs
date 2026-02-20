using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class PrevisaoDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>
    {
        public PrevisaoDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> BuscarPorExcecao(int codigoExcecao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecao select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento BuscarPorExcecao(int codigoExcecao, int codigoRota, int modeloVeicular, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecao && obj.Rota.Codigo == codigoRota && obj.Dia == diaSemana && obj.ModelosVeiculos.Any(o => o.Codigo == modeloVeicular) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento, int codigoRota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.CentroDescarregamento.Codigo == codigoCentroDescarregamento && obj.Rota.Codigo == codigoRota && obj.Dia == diaSemana select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento BuscarPorCentroDescarregamento(int codigoCentroDescarregamento, int codigoRota, int modeloVeicular, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.CentroDescarregamento.Codigo == codigoCentroDescarregamento && obj.Rota.Codigo == codigoRota && obj.Dia == diaSemana && obj.ModelosVeiculos.Any(o => o.Codigo == modeloVeicular) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> ConsultaPrevisoes(int centroDescarregamento, int rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana, string descricaoRota, string descricaoPrevisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query select obj;

            if (centroDescarregamento > 0)
                result = result.Where(o => o.CentroDescarregamento.Codigo == centroDescarregamento);

            if (rota > 0)
                result = result.Where(o => o.Rota.Codigo == rota);

            if (diaSemana != null)
                result = result.Where(o => o.Dia == diaSemana.Value);

            if (!string.IsNullOrWhiteSpace(descricaoRota))
                result = result.Where(o => o.Rota.Descricao.Contains(descricaoRota));

            if (!string.IsNullOrWhiteSpace(descricaoPrevisao))
                result = result.Where(o => o.Descricao.Contains(descricaoPrevisao));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> BuscarPorDiaMes(int codigoCentro, int dia, int mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            var result = from obj in query where obj.CentroDescarregamento.Codigo == codigoCentro && obj.DiaDoMes == dia && obj.Mes == mes select obj;

            return result.ToList();
        }
    }
}
