using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class SubareaCliente : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>
    {
        public SubareaCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> BuscarPorCliente(Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query.Where(obj => obj.Cliente == cliente);
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);
            return query.OrderBy(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> BuscarPorClientes(List<double> codigosClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query.Where(obj => codigosClientes.Contains(obj.Cliente.CPF_CNPJ));
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);
            query = query.OrderBy(obj => obj.Cliente.Nome).ThenBy(obj => obj.Descricao);

            return query.Fetch(obj => obj.Cliente).Fetch(obj => obj.TipoSubarea).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] BuscarAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query = query.Where(obj => obj.Ativo == true);
            return query.OrderBy(obj => obj.Codigo).ToArray();
        }

        public Dominio.Entidades.Embarcador.Logistica.SubareaCliente BuscarPorCodigoTag(string CodigoTag)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query = query.Where(obj => obj.Ativo && obj.CodigoTag == CodigoTag);
            return query.FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] BuscarAtivasObjetoDeValor()
        {
            string sql = @"
                select 
	                SAC_CODIGO,
	                SAC_AREA,
                    CLI_CGCCPF
                from 
	                T_SUBAREA_CLIENTE
                where
	                SAC_ATIVO = 1 and CLI_CGCCPF is not null 
                order by
	                CLI_CGCCPF";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            var subareasDynamic = query.List<dynamic>().ToArray();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente> subareas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente>();
            int total = subareasDynamic.Length;
            for (int i = 0; i < total; i++)
            {
                subareas.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente
                {
                    Codigo = subareasDynamic[i][0],
                    Area = subareasDynamic[i][1],
                    CodigoCliente = subareasDynamic[i][2]
                });
            }
            return subareas.ToArray();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> BuscarAtivasDosClientes(List<double> codigosClientes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query = query.Where(obj => obj.Ativo == true && codigosClientes.Contains(obj.Cliente.CPF_CNPJ));
            return query.OrderBy(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> BuscarAtivasComMovimentacaoDeFluxoDePatio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            query = query.Where(obj => obj.Ativo && (obj.TipoSubarea.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea ?? false));
            return query.OrderBy(obj => obj.Codigo).ToList();
        }

    }
}
