using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListOpcoes : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>
    {
        #region Construtores

        public CheckListOpcoes(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CheckListOpcoes(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> _Consultar(string descricao, CategoriaOpcaoCheckList? categoria, AplicacaoOpcaoCheckList? aplicacao, int codigoCheckListTipo, int codigoEmpresa, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (categoria.HasValue)
                result = result.Where(o => o.Categoria == categoria.Value);

            if (aplicacao.HasValue)
                result = result.Where(o => o.Aplicacao == aplicacao.Value);

            if (codigoCheckListTipo > 0)
                result = result.Where(o => o.CheckListTipo.Codigo == codigoCheckListTipo);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (codigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == codigoFilial);

            return result;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> Consultar(string descricao, CategoriaOpcaoCheckList? categoria, AplicacaoOpcaoCheckList? aplicacao, int codigoCheckListTipo, int codigoEmpresa, int codigoFilial, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, categoria, aplicacao, codigoCheckListTipo, codigoEmpresa, codigoFilial);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, CategoriaOpcaoCheckList? categoria, AplicacaoOpcaoCheckList? aplicacao, int codigoCheckListTipo, int codigoEmpresa, int codigoFilial)
        {
            var result = _Consultar(descricao, categoria, aplicacao, codigoCheckListTipo, codigoEmpresa, codigoFilial);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> BuscarPerguntasPorAplicacao(AplicacaoOpcaoCheckList aplicacao, int codigoFilial, EtapaCheckList etapaCheckList)
        {
            var consultaCheckListOpcoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>()
                .Where(o =>
                    (o.Aplicacao == aplicacao || o.Aplicacao == AplicacaoOpcaoCheckList.Sempre) &&
                    (o.Filial == null || o.Filial.Codigo == codigoFilial) &&
                    ((EtapaCheckList?)o.EtapaCheckList ?? EtapaCheckList.Checklist) == etapaCheckList
                );

            return consultaCheckListOpcoes
                .OrderBy(o => o.Ordem).ThenBy(O => O.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> BuscarPerguntasPorFilial(int codigoFilial)
        {
            var consultaCheckListOpcoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>()
                .Where(o => o.Filial == null || o.Filial.Codigo == codigoFilial);

            return consultaCheckListOpcoes
                .OrderBy(o => o.Ordem).ThenBy(O => O.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> BuscarPerguntasPorOrdem(int codigoCheckListTipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>();

            //var result = from obj in query orderby obj.Ordem ascending select obj;
            var result = from obj in query select obj;

            if (codigoCheckListTipo > 0)
                result = result.Where(o => o.CheckListTipo.Codigo == codigoCheckListTipo);

            result = result.OrderBy("Ordem ascending");

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> BuscarPerguntasPorCheckListTipo(int codigoCheckListTipo)
        {
            var consultaCheckListOpcoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>()
                .Where(o => o.CheckListTipo != null);

            if (codigoCheckListTipo > 0)
                consultaCheckListOpcoes = consultaCheckListOpcoes.Where(o => o.CheckListTipo.Codigo == codigoCheckListTipo);

            return consultaCheckListOpcoes
                .OrderBy(o => o.Ordem).ThenBy(O => O.Codigo)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>> BuscarPerguntasPorCheckListTipoAsync(int codigoCheckListTipo, CancellationToken cancellationToken)
        {
            var consultaCheckListOpcoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>()
                .Where(o => o.CheckListTipo != null);

            if (codigoCheckListTipo > 0)
                consultaCheckListOpcoes = consultaCheckListOpcoes.Where(o => o.CheckListTipo.Codigo == codigoCheckListTipo);

            return await consultaCheckListOpcoes
                .OrderBy(o => o.Ordem).ThenBy(O => O.Codigo)
                .ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo> BuscarRelacoesCampo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo>();

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo BuscarRelacaoCampoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo>()
                .Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta> BuscarRelacaoPergunta(int codigoTipoImpressaoChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta>();

            query = query.Where(o => o.TipoChecklistImpressao.Codigo == codigoTipoImpressaoChecklist);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta BuscarRelacaoPerguntaPorCodigo(int codigoRelacaoPergunta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta>();

            query = query.Where(o => o.Codigo == codigoRelacaoPergunta);

            return query.FirstOrDefault();
        }

        public bool ValidarRelacaoPergunta(int codigoFilial, int codigoRelacaoPergunta, int codigoPergunta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>();

            query = query.Where(o => o.Filial.Codigo == codigoFilial && o.RelacaoPergunta.Codigo == codigoRelacaoPergunta && o.Codigo != codigoPergunta);

            return query.Count() > 0;
        }

        public bool ExistePerguntaSolicitaModeloVeicularCarga()
        {
            var consultaCheckListOpcoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>()
                .Where(o =>
                    (o.Aplicacao == AplicacaoOpcaoCheckList.Carregamento || o.Aplicacao == AplicacaoOpcaoCheckList.Sempre) &&
                    o.ExibirSomenteParaFretesOndeRemetenteForTomador
                );

            return consultaCheckListOpcoes.Any();
        }

        #endregion
    }
}
