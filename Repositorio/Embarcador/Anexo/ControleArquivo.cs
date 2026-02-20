using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Anexo
{
    public sealed class ControleArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Anexo.ControleArquivo>
    {
        #region Construtores

        public ControleArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Anexo.ControleArquivo> Consultar(Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo filtrosPesquisa)
        {
            var consultaControleArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Anexo.ControleArquivo>();

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
                consultaControleArquivo = consultaControleArquivo.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjPessoa);

            if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue)
                consultaControleArquivo = consultaControleArquivo.Where(o => o.DataVencimento.Value.Date >= filtrosPesquisa.DataVencimentoInicial.Date);

            if (filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                consultaControleArquivo = consultaControleArquivo.Where(o => o.DataVencimento.Value.Date <= filtrosPesquisa.DataVencimentoFinal.Date);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaControleArquivo = consultaControleArquivo.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaControleArquivo = consultaControleArquivo.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            return consultaControleArquivo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Anexo.ControleArquivo BuscarPorCodigo(int codigo)
        {
            var consultaControleArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Anexo.ControleArquivo>()
                .Where(o => o.Codigo == codigo);

            return consultaControleArquivo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Anexo.ControleArquivo> BuscarControlesPendentesBaixar()
        {
            var queryAnexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo>();
            queryAnexos = queryAnexos.Where(o => !o.RealizouDownload);

            return queryAnexos.Select(o => o.EntidadeAnexo).Where(p => p.DataVencimento != null).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Anexo.ControleArquivo> Consultar(Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaControleArquivo = Consultar(filtrosPesquisa);

            return ObterLista(consultaControleArquivo, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo filtrosPesquisa)
        {
            var consultaControleArquivo = Consultar(filtrosPesquisa);

            return consultaControleArquivo.Count();
        }

        #endregion
    }
}
