using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class AprovacaoAlcadaNaoConformidade : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade,//AprovacaoAutorizacao Tojken
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade, //RegraAutorizacao token
        Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade //Solicitacion TOken
    >
    {
        #region Construtores

        public AprovacaoAlcadaNaoConformidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao filtrosPesquisa)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal);

            if (filtrosPesquisa.Situacao.HasValue)
            {
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

                if (filtrosPesquisa.Situacao.Value == SituacaoNaoConformidade.SemRegraAprovacao)
                    return consultaNaoConformidade;
            }

            if (filtrosPesquisa.DataInicialEmissaoNotaFiscal.HasValue)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.XMLNotaFiscal.DataEmissao.Date >= filtrosPesquisa.DataInicialEmissaoNotaFiscal.Value.Date);

            if (filtrosPesquisa.DataFinalEmissaoNotaFiscal.HasValue)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.XMLNotaFiscal.DataEmissao.Date <= filtrosPesquisa.DataFinalEmissaoNotaFiscal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.DataInicialGeracaoNC.HasValue)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicialGeracaoNC.Value.Date);

            if (filtrosPesquisa.DataFinalGeracaoNC.HasValue)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataFinalGeracaoNC.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarga))
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.CargaPedido.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador);

            if (!string.IsNullOrEmpty(filtrosPesquisa.ItemNC))
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.ItemNaoConformidade.Descricao == filtrosPesquisa.ItemNC);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroOrdem))
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.CargaPedido.Pedido.NumeroOrdem == filtrosPesquisa.NumeroOrdem);
            
            if (!string.IsNullOrEmpty(filtrosPesquisa.Filial))
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.CargaPedido.Carga.Filial.Descricao == filtrosPesquisa.Filial);

            if (filtrosPesquisa.NumeroNotas.Count > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(obj => filtrosPesquisa.NumeroNotas.Contains(obj.XMLNotaFiscal.Codigo));

            if (filtrosPesquisa.Fornecedor > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.XMLNotaFiscal.Emitente.CPF_CNPJ == filtrosPesquisa.Fornecedor);

            if (filtrosPesquisa.Transportador > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.CargaPedido.Carga.Empresa.Codigo == filtrosPesquisa.Transportador);

            if (filtrosPesquisa.Destino > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(o => o.XMLNotaFiscal.Destinatario.CPF_CNPJ == filtrosPesquisa.Destino);

            if (filtrosPesquisa.Motorista.Count > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(obj => filtrosPesquisa.Motorista.Contains(obj.CargaPedido.Carga.CodigoPrimeiroMotorista));

            var consultaAprovacaoNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAprovacaoNaoConformidade = consultaAprovacaoNaoConformidade.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == SituacaoNaoConformidade.AguardandoTratativa)
                consultaAprovacaoNaoConformidade = consultaAprovacaoNaoConformidade.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.Situacao.HasValue)
                return consultaNaoConformidade.Where(o => consultaAprovacaoNaoConformidade.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaNaoConformidade.Where(o =>
                o.Situacao == SituacaoNaoConformidade.SemRegraAprovacao ||
                consultaAprovacaoNaoConformidade.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<int> BuscarCodigosSemRegraAprovacaoPorCodigos(List<int> codigos)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(o => codigos.Contains(o.Codigo) && o.Situacao == SituacaoNaoConformidade.SemRegraAprovacao);

            return consultaNaoConformidade.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNaoConformidade = Consultar(filtrosPesquisa);

            consultaNaoConformidade = consultaNaoConformidade
                .Fetch(o => o.XMLNotaFiscal)
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido);

            return ObterLista(consultaNaoConformidade, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao filtrosPesquisa)
        {
            var consultaNaoConformidade = Consultar(filtrosPesquisa);

            return consultaNaoConformidade.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade> BuscarPorNaoConformidades(List<int> codigosNaoConformidade)
        {
            var consultaAprovacaoNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade>()
                .Where(s => codigosNaoConformidade.Contains(s.OrigemAprovacao.Codigo));

            return consultaAprovacaoNaoConformidade.ToList();
        }
        #endregion Métodos Públicos
    }
}
