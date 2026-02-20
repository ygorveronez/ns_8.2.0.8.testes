using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoRoteirizacaoClientesRota : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>
    {
        public CarregamentoRoteirizacaoClientesRota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoRoteirizacaoClientesRota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> BuscarPorCarregamentoRoteirizacao(int carregamentoRoteirizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var resut = from obj in query where obj.CarregamentoRoteirizacao.Codigo == carregamentoRoteirizacao select obj;
            return resut.Fetch(obj => obj.Cliente)
                        .OrderBy(obj => obj.Ordem).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> BuscarPorCarregamentoRoteirizacoes(List<int> carregamentosRoteirizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var resut = from obj in query where carregamentosRoteirizacao.Contains(obj.CarregamentoRoteirizacao.Codigo) select obj;
            return resut
                .Fetch(obj => obj.Cliente)
                .OrderBy(obj => obj.Ordem).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>> BuscarPorCarregamentoRoteirizacoesAsync(List<int> carregamentosRoteirizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var resut = from obj in query where carregamentosRoteirizacao.Contains(obj.CarregamentoRoteirizacao.Codigo) select obj;
            return resut
                .Fetch(obj => obj.Cliente)
                .OrderBy(obj => obj.Ordem).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var queryCarregamentoRoteirizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
            query = query.Where(r => queryCarregamentoRoteirizacao.Any(c => c.Codigo == r.CarregamentoRoteirizacao.Codigo && carregamentos.Contains(c.Carregamento.Codigo)));
            var resut = from obj in query select obj;
            return resut
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.CarregamentoRoteirizacao)
                .OrderBy(obj => obj.Ordem).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota BuscarPrimeiraColeta(int carregamentoRoteirizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
            var resut = from obj in query where obj.CarregamentoRoteirizacao.Codigo == carregamentoRoteirizacao && obj.Coleta select obj;
            return resut
                .Fetch(obj => obj.Cliente)
                .OrderBy(obj => obj.Ordem).FirstOrDefault();
        }

        public void InserirSQL(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota item)
        {
            string sql = @"
INSERT INTO T_CARREGAMENTO_ROTEIRIZACAO_CLIENTES_ROTA ( CTC_ORDEM, CRT_CODIGO, CLI_CGCCPF, CTC_COLETA ) VALUES ( :CTC_ORDEM, :CRT_CODIGO, :CLI_CGCCPF, :CTC_COLETA ) ";

            object cliente = DBNull.Value;
            if (item.Cliente != null)
                cliente = item.Cliente.Codigo;

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("CTC_ORDEM", item.Ordem);
            query.SetParameter("CRT_CODIGO", item.CarregamentoRoteirizacao.Codigo);
            query.SetParameter("CLI_CGCCPF", cliente);
            query.SetParameter("CTC_COLETA", item.Coleta);
            query.ExecuteUpdate();
        }

        public void InserirSQL(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> itens)
        {
            string parameros = "( :CTC_ORDEM_[X], :CRT_CODIGO_[X], :CLI_CGCCPF_[X], :CTC_COLETA_[X], :COE_CODIGO_[X] )";
            string sql = @"
INSERT INTO T_CARREGAMENTO_ROTEIRIZACAO_CLIENTES_ROTA ( CTC_ORDEM, CRT_CODIGO, CLI_CGCCPF, CTC_COLETA, COE_CODIGO ) VALUES  " + parameros.Replace("[X]", "0");

            for (int i = 1; i < itens.Count; i++)
                sql += ", " + parameros.Replace("[X]", i.ToString());

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            for (int i = 0; i < itens.Count; i++)
            {
                int? outroEndereco = itens[i].OutroEndereco?.Codigo;

                query.SetParameter("CTC_ORDEM_" + i.ToString(), itens[i].Ordem);
                query.SetParameter("CRT_CODIGO_" + i.ToString(), itens[i].CarregamentoRoteirizacao.Codigo);
                query.SetParameter("CLI_CGCCPF_" + i.ToString(), itens[i].Cliente?.Codigo, NHibernate.NHibernateUtil.Double);
                query.SetParameter("CTC_COLETA_" + i.ToString(), itens[i].Coleta);
                query.SetParameter("COE_CODIGO_" + i.ToString(), outroEndereco, NHibernate.NHibernateUtil.Int32);
            }

            query.ExecuteUpdate();
        }

        public void DeletarPorCarregamentoRoteirizado(int carregamentoRoteririzacao)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CarregamentoRoteirizacaoClientesRota obj WHERE obj.CarregamentoRoteirizacao.Codigo = :codigoCarregamentoRoteririzacao ")
                              .SetInt32("codigoCarregamentoRoteririzacao", carregamentoRoteririzacao)
                              .ExecuteUpdate();

        }
    }
}
