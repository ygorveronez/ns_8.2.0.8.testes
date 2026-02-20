using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListCargaPergunta : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>
    {

        public CheckListCargaPergunta(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CheckListCargaPergunta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> BuscarPorCheckList(int checkList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            var result = from obj in query where obj.CheckListCarga.Codigo == checkList select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>> BuscarPorCheckListAsync(int checkList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            var result = from obj in query where obj.CheckListCarga.Codigo == checkList select obj;
            return result.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> BuscarPerguntasObrigatoriasPorCheckList(int codigoChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            query = query.Where(obj => obj.CheckListCarga.Codigo == codigoChecklist && obj.Obrigatorio == true);

            return query.ToList();
        }



        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> BuscarPerguntasCargaPorIdIntegracaoBalanca(int codigoIntegracaoBalanca)
        {
            string sql = $@" select CCP.CLP_CODIGO from T_PESAGEM_INTEGRACAO I 
                            INNER JOIN T_PESAGEM P ON P.PSG_CODIGO = I.PSG_CODIGO
                            INNER JOIN T_CARGA_JANELA_CARREGAMENTO_GUARITA CJ ON CJ.JCG_CODIGO = P.JCG_CODIGO
                            INNER JOIN T_CHECK_LIST_CARGA CC ON CC.FGP_CODIGO = CJ.FGP_CODIGO
                            INNER JOIN T_CHECK_LIST_CARGA_PERGUNTA CCP ON CCP. CLC_CODIGO = CC.CLC_CODIGO
                            WHERE INT_CODIGO = {codigoIntegracaoBalanca} ";

            var queryIDs = this.SessionNHiBernate.CreateSQLQuery(sql);
            queryIDs.AddScalar("CLP_CODIGO", NHibernateUtil.Int32);
            IList<int> codigosPerguntaCarga = queryIDs.List<int>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta>();
            query = query.Where(obj => codigosPerguntaCarga.Contains(obj.Codigo));
            return query.ToList();
        }
    }
}
