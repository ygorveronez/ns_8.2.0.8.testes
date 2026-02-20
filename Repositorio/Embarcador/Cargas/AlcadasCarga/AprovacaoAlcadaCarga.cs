using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.AlcadasCarga
{
    public sealed class AprovacaoAlcadaCarga : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
        #region Construtores

        public AprovacaoAlcadaCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AprovacaoAlcadaCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var consultaAlcadaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>()
                .Where(o => !o.Bloqueada);

            consultaCarga = consultaCarga.Where(obj => obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCarga = consultaCarga.Where(o => o.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Filial.Codigo));
            else
                consultaCarga = consultaCarga.Where(o => o.SituacaoAlteracaoFreteCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.NaoInformada);

            if (filtrosPesquisa.CodigosMotivoSolicitacaoFrete?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosMotivoSolicitacaoFrete.Contains(o.MotivoSolicitacaoFrete.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.TipoOperacao.Codigo));

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoAlteracaoFrete.HasValue)
                consultaCarga = consultaCarga.Where(o => o.SituacaoAlteracaoFreteCarga == filtrosPesquisa.SituacaoAlteracaoFrete.Value);

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                consultaCarga = consultaCarga.Where(o => o.PortoOrigem.Codigo == filtrosPesquisa.CodigoPortoOrigem);

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                consultaCarga = consultaCarga.Where(o => o.PortoDestino.Codigo == filtrosPesquisa.CodigoPortoDestino);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
            {
                consultaCarga = consultaCarga.Where(o =>
                    o.Pedidos.Any(p =>
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && p.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && p.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && p.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && p.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador)
                    )
                );
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.TipoDeCarga.Codigo));

            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                consultaCarga = consultaCarga.Where(o => o.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaCarga = consultaAlcadaCarga.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoAlteracaoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.AguardandoAprovacao)
                consultaAlcadaCarga = consultaAlcadaCarga.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaCarga.Where(o => consultaAlcadaCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> ConsultaAutorizacoes(int codigoOrigem, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>()
                 .Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }

            return consulta
                .Fetch(obj => obj.RegraAutorizacao)
                .Fetch(obj => obj.Usuario)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>> ConsultaAutorizacoesAsync(int codigoOrigem, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>()
                 .Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }

            return await consulta
                .Fetch(obj => obj.RegraAutorizacao)
                .Fetch(obj => obj.Usuario)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            consultaCarga = consultaCarga
                .Fetch(o => o.Filial)
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.TipoDeCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.PortoDestino)
                .Fetch(o => o.PortoOrigem)
                .Fetch(o => o.DadosSumarizados);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> ConsultaPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>();

            consulta = consulta.Where(o => o.OrigemAprovacao.Codigo == codigoCarga);

            if (situacao != null)
                consulta = consulta.Where(o => o.Situacao == situacao);

            return consulta
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> ConsultaPorCargas(List<int> codigosCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>();

            consulta = consulta.Where(o => codigosCarga.Contains(o.OrigemAprovacao.Codigo));

            if (situacao != null)
                consulta = consulta.Where(o => o.Situacao == situacao);

            return consulta
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga BuscarPorGuid(string guid)
        {
            var consultaAprovacaoAlcadaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>()
                .Where(o => o.GuidCarga == guid);

            return consultaAprovacaoAlcadaCarga.FirstOrDefault();
        }

        #endregion
    }
}
