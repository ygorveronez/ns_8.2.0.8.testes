using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoImportacao : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao>
    {
        public GestaoDevolucaoImportacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao BuscarPorNotaFiscalDevolucao(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao>();
            query = query.Where(o => o.ChaveNFD == chave || o.NotaFiscalDevolucao.Chave == chave);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao> consultaGestaoDevolucao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao>();

            consultaGestaoDevolucao = consultaGestaoDevolucao.Where(gestaoDevolucaoImportacao => gestaoDevolucaoImportacao.NotaFiscalDevolucao.TipoNotaFiscalIntegrada != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet);

            if (!string.IsNullOrEmpty(filtrosPesquisa.Carga))
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.Carga.CodigoCargaEmbarcador == filtrosPesquisa.Carga);

            if (filtrosPesquisa.NFOrigem != 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotaFiscalOrigem.Numero == filtrosPesquisa.NFOrigem);

            if (filtrosPesquisa.NFDevolucao != 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotaFiscalDevolucao.Numero == filtrosPesquisa.NFDevolucao);

            if (filtrosPesquisa.Transportadores.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.Transportadores.Contains(devolucao.Carga.Empresa.Codigo));

            if (filtrosPesquisa.Filiais.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.Filiais.Contains(devolucao.Carga.Filial.Codigo));

            if (filtrosPesquisa.OrigemRecebimento != null)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.OrigemRecebimento == filtrosPesquisa.OrigemRecebimento);

            if (filtrosPesquisa.DevolucaoGerada != null)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.EmDevolucao == filtrosPesquisa.DevolucaoGerada);

            if (filtrosPesquisa.DataEmissaoNFInicial != DateTime.MinValue)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotaFiscalDevolucao.DataEmissao >= filtrosPesquisa.DataEmissaoNFInicial);

            if (filtrosPesquisa.DataEmissaoNFFinal != DateTime.MinValue)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotaFiscalDevolucao.DataEmissao <= filtrosPesquisa.DataEmissaoNFFinal);

            if (filtrosPesquisa.Cliente > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotaFiscalOrigem.Destinatario.CPF_CNPJ == filtrosPesquisa.Cliente);

            return consultaGestaoDevolucao;
        }
    }
}
