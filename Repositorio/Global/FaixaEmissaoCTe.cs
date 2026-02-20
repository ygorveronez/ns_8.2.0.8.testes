using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FaixaEmissaoCTe : RepositorioBase<Dominio.Entidades.FaixaEmissaoCTe>, Dominio.Interfaces.Repositorios.FaixaEmissaoCTe
    {
        public FaixaEmissaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FaixaEmissaoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaixaEmissaoCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.FaixaEmissaoCTe> BuscarPorPlano(int codigoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaixaEmissaoCTe>();
            var result = from obj in query where obj.Plano.Codigo == codigoPlano select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.FaixaEmissaoCTe> BuscarPorPlanoOrdenado(int codigoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaixaEmissaoCTe>();
            var result = from obj in query
                         where obj.Plano.Codigo == codigoPlano
                         orderby obj.Quantidade descending
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.FaixaEmissaoCTe BuscarPorPlanoEQuantidade(int codigoPlano, int quantidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaixaEmissaoCTe>();
            var result = from obj in query where obj.Plano.Codigo == codigoPlano && obj.Quantidade >= quantidade select obj;
            return result.OrderBy(o => o.Quantidade).FirstOrDefault();  
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioMensalidade> RelatorioDeMensalidades()
        {
            throw new NotImplementedException("Método não implementado.");
        }
    }
}
