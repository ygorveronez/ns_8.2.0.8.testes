using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Localidades
{
    public class Regiao : RepositorioBase<Dominio.Entidades.Embarcador.Localidades.Regiao>
    {
        public Regiao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Regiao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Localidades.Regiao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Localidades.Regiao>> BuscarPorCodigosAsync(List<int> codigos, List<Dominio.Entidades.Embarcador.Localidades.Regiao> lstRegiao = null, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            var codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> resultado = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            if (lstRegiao != null)
            {
                return lstRegiao
                    .Where(r => codigosUnicos.Contains(r.Codigo))
                    .ToList();
            }

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>()
                    .Where(r => bloco.Contains(r.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Localidades.Regiao BuscarPorCodigoIntegracaoDiferente(string codigoIntegacao, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegacao && obj.Codigo != codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Localidades.Regiao BuscarPorCodigoIntegracao(string codigoIntegacao, List<Dominio.Entidades.Embarcador.Localidades.Regiao> lstRegiao = null)
        {
            if (lstRegiao != null)
                return lstRegiao.Where(obj => obj.CodigoIntegracao == codigoIntegacao).FirstOrDefault();
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>().Where(obj => obj.CodigoIntegracao == codigoIntegacao).FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Localidades.Regiao>> BuscarPorCodigosIntegracaoAsync(List<string> codigosIntegracao, List<Dominio.Entidades.Embarcador.Localidades.Regiao> lstRegiao = null, int pageSize = 2000)
        {
            if (codigosIntegracao == null || codigosIntegracao.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            var codigos = codigosIntegracao.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> resultado = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            if (lstRegiao != null)
            {
                return lstRegiao
                    .Where(r => codigos.Contains(r.CodigoIntegracao))
                    .ToList();
            }

            for (int i = 0; i < codigos.Count; i += pageSize)
            {
                var bloco = codigos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>()
                    .Where(r => bloco.Contains(r.CodigoIntegracao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Localidades.Regiao BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Localidades.Regiao>> BuscarPorDescricoesAsync(List<string> descricoes, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            var listaDescricoes = descricoes.Select(d => d.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> resultado = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            for (int i = 0; i < listaDescricoes.Count; i += pageSize)
            {
                var bloco = listaDescricoes.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>()
                    .Where(r => bloco.Contains(r.Descricao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> Consultar(string descricao, string codigoIntegracao, Dominio.Entidades.Localidade localidadePolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.Entidades.Localidade localidadeRegiao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao codigoTipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga codigoTipoCarga, Dominio.Entidades.Embarcador.Filiais.Filial codigoFilial, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (localidadePolo != null)
                result = result.Where(obj => obj.LocalidadePolo.Codigo == localidadePolo.Codigo);

            if (localidadeRegiao != null)
                result = result.Where(obj => obj.Localidades.Contains(localidadeRegiao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo == true);
                else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => obj.Ativo == false);
            }

            var queryRegiaoPrazoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

            var resultRegiaoPrazoEntrega = from obj in queryRegiaoPrazoEntrega select obj;

            if (codigoTipoCarga != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.TipoDeCarga.Codigo == codigoTipoCarga.Codigo));

            if (codigoTipoOperacao != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.TipoOperacao.Codigo == codigoTipoOperacao.Codigo));

            if (codigoFilial != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.Filial.Codigo == codigoFilial.Codigo));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao, Dominio.Entidades.Localidade localidadePolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.Entidades.Localidade localidadeRegiao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao codigoTipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga codigoTipoCarga, Dominio.Entidades.Embarcador.Filiais.Filial codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (localidadePolo != null)
                result = result.Where(obj => obj.LocalidadePolo.Codigo == localidadePolo.Codigo);

            if (localidadeRegiao != null)
                result = result.Where(obj => obj.Localidades.Contains(localidadeRegiao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo == true);
                else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => obj.Ativo == false);
            }

            var queryRegiaoPrazoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

            var resultRegiaoPrazoEntrega = from obj in queryRegiaoPrazoEntrega select obj;

            if (codigoTipoCarga != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.TipoDeCarga.Codigo == codigoTipoCarga.Codigo));

            if (codigoTipoOperacao != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.TipoOperacao.Codigo == codigoTipoOperacao.Codigo));

            if (codigoFilial != null)
                result = result.Where(obj => resultRegiaoPrazoEntrega.Any(p => p.Regiao.Codigo == obj.Codigo && p.Filial.Codigo == codigoFilial.Codigo));

            return result.Count();
        }

        #endregion
    }
}
