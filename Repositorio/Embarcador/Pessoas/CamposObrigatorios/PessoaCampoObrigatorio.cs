using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas.CamposObrigatorios
{
    public class PessoaCampoObrigatorio : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio>
    {
        public PessoaCampoObrigatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> Consultar(Dominio.Enumeradores.OpcaoSimNaoPesquisa cliente, Dominio.Enumeradores.OpcaoSimNaoPesquisa fornecedor, Dominio.Enumeradores.OpcaoSimNaoPesquisa terceiro, bool? ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> query = ObterQueryConsulta(cliente, fornecedor, terceiro, ativo, propOrdenar, dirOrdenar, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(Dominio.Enumeradores.OpcaoSimNaoPesquisa cliente, Dominio.Enumeradores.OpcaoSimNaoPesquisa fornecedor, Dominio.Enumeradores.OpcaoSimNaoPesquisa terceiro, bool? ativo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> query = ObterQueryConsulta(cliente, fornecedor, terceiro, ativo);

            return query.Count();
        }

        public bool ExistePorTipoPessoa(int codigo, bool cliente, bool fornecedor, bool terceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio>();

            query = query.Where(o => o.Codigo != codigo && o.Ativo);
            query = query.Where(o => o.Cliente == cliente && o.Fornecedor == fornecedor && o.Terceiro == terceiro);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio BuscarParaPessoa(bool cliente, bool fornecedor, bool terceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio>();

            query = query.Where(o => o.Ativo && o.Cliente == cliente && o.Fornecedor == fornecedor && o.Terceiro == terceiro);

            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> ObterQueryConsulta(Dominio.Enumeradores.OpcaoSimNaoPesquisa cliente, Dominio.Enumeradores.OpcaoSimNaoPesquisa fornecedor, Dominio.Enumeradores.OpcaoSimNaoPesquisa terceiro, bool? ativo, string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio>();

            if (cliente == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query = query.Where(o => o.Cliente);
            else if (cliente == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query = query.Where(o => !o.Cliente);

            if (fornecedor == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query = query.Where(o => o.Fornecedor);
            else if (fornecedor == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query = query.Where(o => !o.Fornecedor);

            if (terceiro == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query = query.Where(o => o.Terceiro);
            else if (terceiro == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query = query.Where(o => !o.Terceiro);

            if (ativo.HasValue)
                query = query.Where(o => o.Ativo == ativo.Value);

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
