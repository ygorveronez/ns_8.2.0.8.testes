using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class ModeloCarroceria : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>
    {
        public ModeloCarroceria(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ModeloCarroceria(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>()
                .Where(obj => obj.Codigo == codigo).FirstOrDefaultAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            List<int> codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>> BuscarPorCodigosIntegracaoAsync(List<string> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            var codigosUnicos = codigos.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>()
                    .Where(obj => bloco.Contains(obj.CodigoIntegracao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> Consultar(string descricao, SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo == true);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Ativo == false);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo == true);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Ativo == false);

            return query.Count();
        }
    }
}
