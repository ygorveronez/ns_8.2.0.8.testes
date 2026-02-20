using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Terceiros
{
    public class TaxaTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro>
    {
        public TaxaTerceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (filtrosPesquisa.CnpjCpfTerceiro > 0)
                result = result.Where(o => o.Terceiro.CPF_CNPJ == filtrosPesquisa.CnpjCpfTerceiro);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoJustificativa > 0)
                result = result.Where(o => o.Justificativa.Codigo == filtrosPesquisa.CodigoJustificativa);

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
                var resultQueryCarga = from obj in queryCarga where obj.Codigo == filtrosPesquisa.CodigoCarga select obj;

                result = result.Where(o => (o.Veiculo == null || resultQueryCarga.Where(a => a.Veiculo.Codigo == o.Veiculo.Codigo || a.VeiculosVinculados.Where(v => v.Codigo == o.Veiculo.Codigo).Any()).Any())
                                && resultQueryCarga.Where(a => a.Terceiro.CPF_CNPJ == o.Terceiro.CPF_CNPJ).Any());

                result = result.Where(o => (o.VigenciaInicial == null && o.VigenciaFinal == null) ||
                                           (DateTime.Now.Date >= o.VigenciaInicial.Value && DateTime.Now.Date <= o.VigenciaFinal.Value) ||
                                           (DateTime.Now.Date >= o.VigenciaInicial.Value && o.VigenciaFinal == null) ||
                                           (DateTime.Now.Date <= o.VigenciaFinal.Value && o.VigenciaInicial == null));
            }

            return result;
        }

        #endregion
    }
}
