using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class Contabilizacao
    {
        public static void GerarLoteContabilizacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (configuracao.AutomatizarGeracaoLotePagamento)
            {
                //todo: regra fixa para Big, se outros clientes solicitarem ou existir variação por algum tipo é necessário criar um cadastro para essas parametrizações.
                if (DateTime.Now.Hour > 0 || DateTime.Now.Minute >= 10)
                {
                    Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);
                    Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil repDocumentoExportacaoContabil = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil(unitOfWork);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                    DateTime dataFinal = DateTime.Now.Date.AddDays(-1);

                    Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao()
                    {
                        DataLimite = dataFinal
                    };

                    int totalDocumentos = repDocumentoExportacaoContabil.ContarConsultaNovo(filtrosPesquisa);

                    if (totalDocumentos <= 0)
                        return;

                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

                    IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> documentosExportacaoContabil = repDocumentoExportacaoContabil.ConsultarNovo(filtrosPesquisa, parametrosConsulta);

                    List<int> empresas = documentosExportacaoContabil.Select(o => o.CodigoEmpresa).Distinct().ToList();

                    foreach (int empresa in empresas)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> documentosEmpresa = documentosExportacaoContabil.Where(o => o.CodigoEmpresa == empresa).ToList();

                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = new Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao()
                        {
                            DataFinal = dataFinal,
                            Empresa = repEmpresa.BuscarPorCodigo(empresa),
                            DataGeracaoLote = DateTime.Now,
                            Numero = repLoteContabilizacao.BuscarUltimoNumero() + 1,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.AgIntegracao
                        };

                        repLoteContabilizacao.Inserir(loteContabilizacao);
                        repDocumentoExportacaoContabil.SetarLoteContabilizacao(loteContabilizacao.Codigo, documentosEmpresa.Select(o => o.Codigo).ToList());

                        unitOfWork.CommitChanges();

                        unitOfWork.FlushAndClear();
                    }
                }
            }
        }
    }
}
