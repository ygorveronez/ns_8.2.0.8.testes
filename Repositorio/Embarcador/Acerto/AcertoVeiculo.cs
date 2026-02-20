using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>
    {
        public AcertoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarVeiculosDoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigo);

            return query.Select(o => o.Veiculo.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo BuscarPorAcertoEVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo> BuscarPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosAcertoCarga(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto);

            return query.Select(o => o.Carga.Veiculo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosVinculadosAcertoCarga(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto);

            return query.Select(o => o.Carga.VeiculosVinculados).SelectMany(o => o).Distinct().ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosVinculadosAcertoCargaVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Carga.Veiculo.Codigo == codigoVeiculo);

            return query.Select(o => o.Carga.VeiculosVinculados).SelectMany(o => o).Distinct().ToList();
        }

        public bool VeiculoEmAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, Dominio.Entidades.Veiculo veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();

            query = query.Where(o => o.AcertoViagem.Codigo == acerto.Codigo && o.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado && o.Veiculo.Codigo == veiculo.Codigo);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo> BuscarPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculoPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj.Veiculo;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarVeiculoPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj.Veiculo;
            return result.Count();
        }


    }
}