using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate;
using NHibernate.Linq;
using Repositorio.Embarcador.PreCTes.Consulta;
using Repositorio.Embarcador.Produtos.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PreConhecimentoDeTransporteEletronico : RepositorioBase<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>
    {
        #region Construtores

        public PreConhecimentoDeTransporteEletronico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.PreConhecimentoDeTransporteEletronico BuscarPorCodigo(int codigoPreCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoPreCTe select obj;

            return result.Fetch(obj => obj.Empresa)
                         .ThenFetch(obj => obj.Configuracao)
                         .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarSemControleDocumentoComDataCorte(DateTime dataCorte, int limit)
        {
            var queryCD = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(x => !queryCD.Any(p => p.CargaCTe.PreCTe.Codigo == x.PreCTe.Codigo));

            return query.Take(limit).ToList();
        }

        public void LiberarPagamentosPorPreCTes(List<int> preCtes, DateTime dataLiberacacao)
        {
            string hql = "update PreConhecimentoDeTransporteEletronico preCTe set  preCTe.PagamentoLiberado= :PagamentoLiberado, preCTe.DataLiberacaoPagamento= :DataLiberacaoPagamento where preCTe.Codigo in (:preCtes) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("PagamentoLiberado", true);
            query.SetDateTime("DataLiberacaoPagamento", dataLiberacacao);

            query.SetParameterList("preCtes", preCtes);
            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPreCtesParaPreviaCusto(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(pc => pc.Carga.Codigo == codigoCarga && pc.PreCTe != null);
            return query.ToList();
        }

        public List<Dominio.Entidades.PreConhecimentoDeTransporteEletronico> BuscarPreCTes(List<int> codigosPreCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>()
                .Where(o => codigosPreCTe.Contains(o.Codigo));
            
            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe> BuscarPreNFSeAguardandoEmissao(DateTime dataInicioEmissao, DateTime dataFimEmissao, string numeroCarga, List<int> codigosIbgePermitidos)
        {
            var consultaPreNFSe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(cargaCTe =>
                    cargaCTe.CTe == null &&
                    cargaCTe.PreCTe != null &&
                    cargaCTe.PreCTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe &&
                    cargaCTe.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    cargaCTe.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                consultaPreNFSe = consultaPreNFSe.Where(cargaCTe => cargaCTe.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (dataInicioEmissao != DateTime.MinValue)
                consultaPreNFSe = consultaPreNFSe.Where(cargaCTe => cargaCTe.PreCTe.DataEmissao >= dataInicioEmissao);

            if (dataFimEmissao != DateTime.MinValue)
                consultaPreNFSe = consultaPreNFSe.Where(cargaCTe => cargaCTe.PreCTe.DataEmissao <= dataFimEmissao);

            if (codigosIbgePermitidos?.Count > 0)
                consultaPreNFSe = consultaPreNFSe.Where(cargaCTe => codigosIbgePermitidos.Contains(cargaCTe.PreCTe.LocalidadeEmissao.CodigoIBGE));

            List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe> listaPreNFSe = consultaPreNFSe
                .OrderBy(cargaCTe => cargaCTe.PreCTe.DataEmissao)
                .WithOptions(opcoes => { opcoes.SetTimeout(300); })
                .Select(cargaCTe =>
                    new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe()
                    {
                        CodigoIdentificacao = cargaCTe.Codigo.ToString(),
                        NumeroCarga = cargaCTe.Carga.CodigoCargaEmbarcador,
                        AliquotaIss = cargaCTe.PreCTe.AliquotaISS,
                        BaseCalculoIss = cargaCTe.PreCTe.BaseCalculoISS,
                        DataEmissao = cargaCTe.PreCTe.DataEmissao,
                        PercentualRetencaoIss = cargaCTe.PreCTe.PercentualISSRetido,
                        ValorIss = cargaCTe.PreCTe.ValorISS,
                        ValorIssRetido = cargaCTe.PreCTe.ValorISSRetido,
                        ValorPrestacaoServico = cargaCTe.PreCTe.ValorPrestacaoServico,
                        ValorReceber = cargaCTe.PreCTe.ValorAReceber,
                        TipoTomador = cargaCTe.PreCTe.TipoTomador,
                        Transportador = (cargaCTe.PreCTe.Empresa == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Transportador()
                        {
                            Cnpj = cargaCTe.PreCTe.Empresa.CNPJ,
                            RazaoSocial = cargaCTe.PreCTe.Empresa.RazaoSocial,
                            inscricaoEstadual = cargaCTe.PreCTe.Empresa.InscricaoEstadual,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.Empresa.Endereco,
                                Numero = cargaCTe.PreCTe.Empresa.Numero,
                                Complemento = cargaCTe.PreCTe.Empresa.Complemento,
                                Bairro = cargaCTe.PreCTe.Empresa.Bairro,
                                Cep = cargaCTe.PreCTe.Empresa.CEP,
                                Localidade = (cargaCTe.PreCTe.Empresa.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.Empresa.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.Empresa.Localidade.Estado.Sigla
                                }
                            }
                        },
                        Remetente = (cargaCTe.PreCTe.Remetente == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Participante()
                        {
                            CpfCnpj = cargaCTe.PreCTe.Remetente.CPF_CNPJ,
                            Nome = cargaCTe.PreCTe.Remetente.Nome,
                            inscricaoEstadual = cargaCTe.PreCTe.Remetente.IE_RG,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.Remetente.Endereco,
                                Numero = cargaCTe.PreCTe.Remetente.Numero,
                                Complemento = cargaCTe.PreCTe.Remetente.Complemento,
                                Bairro = cargaCTe.PreCTe.Remetente.Bairro,
                                Cep = cargaCTe.PreCTe.Remetente.CEP,
                                Localidade = (cargaCTe.PreCTe.Remetente.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.Remetente.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.Remetente.Localidade.Estado.Sigla
                                }
                            }
                        },
                        Destinatario = (cargaCTe.PreCTe.Destinatario == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Participante()
                        {
                            CpfCnpj = cargaCTe.PreCTe.Destinatario.CPF_CNPJ,
                            Nome = cargaCTe.PreCTe.Destinatario.Nome,
                            inscricaoEstadual = cargaCTe.PreCTe.Destinatario.IE_RG,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.Destinatario.Endereco,
                                Numero = cargaCTe.PreCTe.Destinatario.Numero,
                                Complemento = cargaCTe.PreCTe.Destinatario.Complemento,
                                Bairro = cargaCTe.PreCTe.Destinatario.Bairro,
                                Cep = cargaCTe.PreCTe.Destinatario.CEP,
                                Localidade = (cargaCTe.PreCTe.Destinatario.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.Destinatario.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.Destinatario.Localidade.Estado.Sigla
                                }
                            }
                        },
                        Expedidor = (cargaCTe.PreCTe.Expedidor == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Participante()
                        {
                            CpfCnpj = cargaCTe.PreCTe.Expedidor.CPF_CNPJ,
                            Nome = cargaCTe.PreCTe.Expedidor.Nome,
                            inscricaoEstadual = cargaCTe.PreCTe.Expedidor.IE_RG,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.Expedidor.Endereco,
                                Numero = cargaCTe.PreCTe.Expedidor.Numero,
                                Complemento = cargaCTe.PreCTe.Expedidor.Complemento,
                                Bairro = cargaCTe.PreCTe.Expedidor.Bairro,
                                Cep = cargaCTe.PreCTe.Expedidor.CEP,
                                Localidade = (cargaCTe.PreCTe.Expedidor.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.Expedidor.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.Expedidor.Localidade.Estado.Sigla
                                }
                            }
                        },
                        Recebedor = (cargaCTe.PreCTe.Recebedor == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Participante()
                        {
                            CpfCnpj = cargaCTe.PreCTe.Recebedor.CPF_CNPJ,
                            Nome = cargaCTe.PreCTe.Recebedor.Nome,
                            inscricaoEstadual = cargaCTe.PreCTe.Recebedor.IE_RG,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.Recebedor.Endereco,
                                Numero = cargaCTe.PreCTe.Recebedor.Numero,
                                Complemento = cargaCTe.PreCTe.Recebedor.Complemento,
                                Bairro = cargaCTe.PreCTe.Recebedor.Bairro,
                                Cep = cargaCTe.PreCTe.Recebedor.CEP,
                                Localidade = (cargaCTe.PreCTe.Recebedor.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.Recebedor.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.Recebedor.Localidade.Estado.Sigla
                                }
                            }
                        },
                        Outros = (cargaCTe.PreCTe.OutrosTomador == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Participante()
                        {
                            CpfCnpj = cargaCTe.PreCTe.OutrosTomador.CPF_CNPJ,
                            Nome = cargaCTe.PreCTe.OutrosTomador.Nome,
                            inscricaoEstadual = cargaCTe.PreCTe.OutrosTomador.IE_RG,
                            Endereco = new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Endereco()
                            {
                                Logradouro = cargaCTe.PreCTe.OutrosTomador.Endereco,
                                Numero = cargaCTe.PreCTe.OutrosTomador.Numero,
                                Complemento = cargaCTe.PreCTe.OutrosTomador.Complemento,
                                Bairro = cargaCTe.PreCTe.OutrosTomador.Bairro,
                                Cep = cargaCTe.PreCTe.OutrosTomador.CEP,
                                Localidade = (cargaCTe.PreCTe.OutrosTomador.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                                {
                                    Descricao = cargaCTe.PreCTe.OutrosTomador.Localidade.Descricao,
                                    SiglaEstado = cargaCTe.PreCTe.OutrosTomador.Localidade.Estado.Sigla
                                }
                            }
                        },
                        LocalidadeEmissao = (cargaCTe.PreCTe.LocalidadeEmissao == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                        {
                            Descricao = cargaCTe.PreCTe.LocalidadeEmissao.Descricao,
                            SiglaEstado = cargaCTe.PreCTe.LocalidadeEmissao.Estado.Sigla
                        },
                        LocalidadePrestacao = (cargaCTe.PreCTe.LocalidadeInicioPrestacao == null) ? null : new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.Localidade()
                        {
                            Descricao = cargaCTe.PreCTe.LocalidadeInicioPrestacao.Descricao,
                            SiglaEstado = cargaCTe.PreCTe.LocalidadeInicioPrestacao.Estado.Sigla
                        }
                    }
                )
                .ToList();

            if (listaPreNFSe.Count > 0)
            {
                var consultaNotasFiscais = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                    .Where(nota =>
                        nota.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true &&
                        consultaPreNFSe.Any(cargaCTe => cargaCTe.Codigo == nota.CargaCTe.Codigo)
                    );

                List<(int CodigoCargaCTe, string NumeroStage, Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NotaFiscal NotaFiscal)> listaNotasFiscais = consultaNotasFiscais
                    .WithOptions(opcoes => { opcoes.SetTimeout(300); })
                    .Select(nota => ValueTuple.Create(
                        nota.CargaCTe.Codigo,
                        nota.PedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto == null ? "" : nota.PedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto.NumeroStage,
                        new Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NotaFiscal()
                        {
                            Chave = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                            DataEmissao = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao,
                            Peso = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                            PesoLiquido = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido,
                            Volumes = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes,
                            Valor = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor
                        }
                    ))
                    .ToList();

                foreach (Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe preNFSe in listaPreNFSe)
                {
                    List<(int CodigoCargaCTe, string NumeroStage, Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NotaFiscal NotaFiscal)> notasFiscaisPorPreNFSe = listaNotasFiscais
                        .Where(nota => nota.CodigoCargaCTe == preNFSe.CodigoIdentificacao.ToInt())
                        .ToList();

                    preNFSe.NumeroEtapa = notasFiscaisPorPreNFSe.Select(nota => nota.NumeroStage).FirstOrDefault();
                    preNFSe.NotasFiscais = notasFiscaisPorPreNFSe.Select(nota => nota.NotaFiscal).ToList();
                }
            }

            return listaPreNFSe;
        }

        public List<Dominio.Entidades.PreConhecimentoDeTransporteEletronico> BuscarPorCargaSemComplementares(int codigoCarga)
        {
            var queryCCT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCCT = queryCCT.Where(x => x.Carga.Codigo == codigoCarga && x.CargaCTeComplementoInfo == null);

            return queryCCT.Select(x => x.PreCTe).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe> ConsultarRelatorioPreCTe(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaPreCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe>();
        }

        public int ContarConsultaRelatorioPreCTe(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaPreCTe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion Métodos Públicos
    }
}
