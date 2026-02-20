using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class LeilaoParticipante : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>
    {
        public LeilaoParticipante(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante BuscarPorEmpresa(int codigoLeilao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var resut = from obj in query where obj.Leilao.Codigo == codigoLeilao && obj.Empresa.Codigo == codigoEmpresa select obj;
            return resut.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> ConsultarParticipantes(int leilao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query where obj.Leilao.Codigo == leilao select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaParticipantes(int leilao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query where obj.Leilao.Codigo == leilao select obj;
            return result.Count();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> ConsultarLances(int leilao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query where obj.Leilao.Codigo == leilao && obj.ValorLance > 0 select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarLances(int leilao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query where obj.Leilao.Codigo == leilao && obj.ValorLance > 0 select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> ConsultarLeiloesParaParticipante(int empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Empresa.Codigo == empresa &&
                obj.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.iniciado &&
                (obj.Leilao.DataParaEncerramentoLeilao >= DateTime.Now || obj.Leilao.DataParaEncerramentoLeilao == null)
                );
            return result.OrderBy(obj => obj.Leilao.DataInicioLeilao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarLeiloesParaParticipante(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Empresa.Codigo == empresa &&
                obj.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.iniciado &&
                 (obj.Leilao.DataParaEncerramentoLeilao >= DateTime.Now || obj.Leilao.DataParaEncerramentoLeilao == null)
                );

            return result.Count();
        }

    }
}
