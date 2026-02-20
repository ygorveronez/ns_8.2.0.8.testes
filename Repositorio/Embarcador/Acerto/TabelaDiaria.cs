using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaDiaria : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>
    {
        public TabelaDiaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaDiaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarTabelaDiaria(int codigoSegmento, List<int> codigosModeloVeicular, int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true && codigosModeloVeicular.Contains(obj.ModeloVeicularCarga.Codigo) select obj;
            if (codigoCentroResultado > 0)
                result = result.Where(o => o.CentroResultado.Codigo == codigoCentroResultado);
            if (codigoSegmento > 0)
                result = result.Where(o => o.SegmentoVeiculo.Codigo == codigoCentroResultado);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarPorModeloVeicularFilial(List<int> codigosModeloVeicular, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true && codigosModeloVeicular.Contains(obj.ModeloVeicularCarga.Codigo) select obj;
            if (codigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == codigoFilial);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarTabelaDiaria(int codigoSegmento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true select obj;
            if (codigoSegmento > 0)
                result = result.Where(o => o.SegmentoVeiculo.Codigo == codigoSegmento);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarTabelaDiariaPorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true && obj.CentroResultado.Codigo == codigoCentroResultado select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarTabelaDiaria(List<int> codigosModeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true && codigosModeloVeicular.Contains(obj.ModeloVeicularCarga.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> BuscarTabelaDiaria()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();
            var result = from obj in query where obj.Ativo == true select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

        public bool BuscarExistenciaPorFilialModeloVeicular(int codigoTabela, int codigoFilial, int codigoModeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>();

            query = query.Where(o => o.Filial.Codigo == codigoFilial && o.ModeloVeicularCarga.Codigo == codigoModeloVeicular && o.Codigo != codigoTabela);

            return query.Count() > 0;
        }
    }
}
