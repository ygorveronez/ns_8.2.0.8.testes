using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoRoteirizacaoPontosPassagem : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem>
    {
        public CarregamentoRoteirizacaoPontosPassagem(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> BuscarPorCarregamentoRoteirizacao(int carregamentoRoteririzacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem>();
            var result = from obj in query where obj.CarregamentoRoteirizacao.Codigo == carregamentoRoteririzacao select obj;
            return result.Fetch(obj => obj.Cliente)
                         .OrderBy(obj => obj.Ordem).ToList();
        }


        public void DeletarPorCarregamentoRoteirizado(int carregamentoRoteririzacao)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CarregamentoRoteirizacaoPontosPassagem obj WHERE obj.CarregamentoRoteirizacao.Codigo = :codigoCarregamentoRoteririzacao ")
                              .SetInt32("codigoCarregamentoRoteririzacao", carregamentoRoteririzacao)
                              .ExecuteUpdate();

        }

        public void InserirSQL(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> itens)
        {
            if (itens != null && itens.Count > 0)
            {
                int take = 150;
                int start = 0;

                while (start < itens.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> itensTmp = itens.Skip(start).Take(take).ToList();

                    string parameros = "( :CPP_TIPO_PONTOS_PASSAGEM_[X], :CPP_LATITUDE_[X], :CPP_LONGITUDE_[X], :CPP_TEMPO_[X], :CPP_DISTANCIA_[X], :CPP_DISTANCIA_DIRETA_[X], :CPP_ORDEM_[X], :CRT_CODIGO_[X], :CLI_CGCCPF_[X], :PRP_CODIGO_[X], :LOC_CODIGO_[X], :COE_CODIGO_[X] )";
                    string sql = @"
INSERT INTO T_CARREGAMENTO_ROTEIRIZACAO_PONTOS_PASSAGEM ( CPP_TIPO_PONTOS_PASSAGEM, CPP_LATITUDE, CPP_LONGITUDE, CPP_TEMPO, CPP_DISTANCIA, CPP_DISTANCIA_DIRETA, CPP_ORDEM, CRT_CODIGO, CLI_CGCCPF, PRP_CODIGO, LOC_CODIGO, COE_CODIGO) VALUES  " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < itensTmp.Count; i++)
                        sql += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sql);

                    for (int i = 0; i < itensTmp.Count; i++)
                    {
                        double? cliente = itensTmp[i].Cliente?.Codigo;
                        int? pontoApoio = itensTmp[i].PontoDeApoio?.Codigo;
                        int? praca = itensTmp[i].PracaPedagio?.Codigo;
                        int? endereco = itensTmp[i].ClienteOutroEndereco?.Codigo;

                        query.SetParameter("CPP_TIPO_PONTOS_PASSAGEM_" + i.ToString(), itensTmp[i].TipoPontoPassagem);
                        query.SetParameter("CPP_LATITUDE_" + i.ToString(), itensTmp[i].Latitude);
                        query.SetParameter("CPP_LONGITUDE_" + i.ToString(), itensTmp[i].Longitude);
                        query.SetParameter("CPP_TEMPO_" + i.ToString(), itensTmp[i].Tempo);

                        query.SetParameter("CPP_DISTANCIA_" + i.ToString(), itensTmp[i].Distancia);
                        query.SetParameter("CPP_DISTANCIA_DIRETA_" + i.ToString(), itensTmp[i].DistanciaDireta);
                        query.SetParameter("CPP_ORDEM_" + i.ToString(), itensTmp[i].Ordem);
                        query.SetParameter("CRT_CODIGO_" + i.ToString(), itensTmp[i].CarregamentoRoteirizacao.Codigo);

                        query.SetParameter("CLI_CGCCPF_" + i.ToString(), cliente, NHibernateUtil.Double);
                        query.SetParameter("PRP_CODIGO_" + i.ToString(), praca, NHibernateUtil.Int32);
                        query.SetParameter("LOC_CODIGO_" + i.ToString(), pontoApoio, NHibernateUtil.Int32);
                        query.SetParameter("COE_CODIGO_" + i.ToString(), endereco, NHibernateUtil.Int32);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }
        }
    }
}
