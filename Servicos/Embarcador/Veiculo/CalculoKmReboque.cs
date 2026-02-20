using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Veiculo
{
    public class CalculoKmReboque
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CalculoKmReboque(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void CalcularKmReboque(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Veiculo reboque, TipoMovimentoKmReboque tipo, int KmInformada, bool edicao, Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque historicoVinculoKmReboque, out string msgErro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {            
            msgErro = "";
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (veiculo == null || reboque == null || ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
            {
                msgErro = "Calculo Km Reboque não atende aos requisitos";
                return;
            }
                

            if (KmInformada <= 0)
            {
                msgErro = "Km Informada Inválida.";
                return;
            }
                
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

            Dominio.Entidades.Abastecimento abastecimento = null;
            Dominio.Entidades.Abastecimento abastecimentoPosterior = null;

            if (edicao)
            {
                if (tipo == TipoMovimentoKmReboque.Vinculo)
                    abastecimentoPosterior = repAbastecimento.BuscarAbastecimentoPosterior(veiculo.Codigo, historicoVinculoKmReboque.DataCriacao.Value);
                else
                    abastecimento = repAbastecimento.BuscarUltimoAbastecimentoSemTipo(veiculo.Codigo, historicoVinculoKmReboque.DataCriacao.Value);
            }
            else
                abastecimento = repAbastecimento.BuscarUltimoAbastecimentoSemTipo(veiculo.Codigo, DateTime.Now);

            decimal qtdKMRodado = 0;

            if (tipo == TipoMovimentoKmReboque.Vinculo)
            {
                if (edicao)
                {
                    if (historicoVinculoKmReboque == null)
                    {
                        msgErro = "Calculo Km Reboque não atende aos requisitos";
                        return;
                    }
                        

                    if (abastecimento != null && KmInformada < abastecimento.Kilometragem)
                    {
                        msgErro = "Km Informada é maior que a KM no abastecimento anterior!";
                        return;
                    }
                        
                    if (abastecimentoPosterior != null && KmInformada > abastecimentoPosterior.Kilometragem)
                    {
                        msgErro = "Km Informada é maior que a KM no abastecimento posterior!";
                        return;
                    }
                        
                    if (abastecimentoPosterior == null && repHistoricoVinculoKmReboque.BuscarDesvinculoDepoisVinculo(veiculo.Codigo, reboque.Codigo, historicoVinculoKmReboque.DataCriacao.Value))
                    {
                        qtdKMRodado = (decimal)historicoVinculoKmReboque.KMAtual - (decimal)KmInformada;

                        if (reboque != null)
                            AtualizarReboque(reboque, qtdKMRodado, unitOfWork, auditado, "via Engate/Vínculo -edição");
                    }
                }
            }
            else if (tipo == TipoMovimentoKmReboque.Desvinculo)
            {
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque vinculo = repHistoricoVinculoKmReboque.BuscarUltimoKmVinculo(veiculo.Codigo, reboque.Codigo, TipoMovimentoKmReboque.Vinculo);

                if (edicao)
                {
                    qtdKMRodado = (decimal)KmInformada - (decimal)historicoVinculoKmReboque.KMAtual;

                    if (reboque != null)
                        AtualizarReboque(reboque, qtdKMRodado, unitOfWork, auditado, "via Desengate/Desvínculo -edição");
                }
                else
                {
                    if (vinculo != null)
                    {
                        if (KmInformada < vinculo.KMAtual)
                        {
                            msgErro = "Km Informada é menor que a KM do vínculo!";
                            return;
                        }
                            
                        if (KmInformada < veiculo.KilometragemAtual)
                        {
                            msgErro = "Km Informada é menor que o Km Atual do Veículo!";
                            return;
                        }
                        
                        if (veiculo != null && veiculo.KilometragemAtual < KmInformada)
                        {
                            if (abastecimento != null && abastecimento.Data > vinculo.DataCriacao)
                                qtdKMRodado = (decimal)KmInformada - (decimal)veiculo.KilometragemAtual;
                            else
                                qtdKMRodado = (decimal)KmInformada - (decimal)vinculo.KMAtual;
                        }

                        if (reboque != null && qtdKMRodado > 0)
                            AtualizarReboque(reboque, qtdKMRodado, unitOfWork, auditado, "via Desengate / Desvínculo");
                    }
                }
            }
        }

        private static void AtualizarReboque(Dominio.Entidades.Veiculo reboque, decimal kmRodado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string observacao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

            if (reboque.Pneus != null && reboque.Pneus.Count > 0)
            {
                foreach (var eixo in reboque.Pneus)
                {
                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                    if (pneu != null)
                    {
                        pneu.KmAnteriorRodado = 0;
                        pneu.KmAtualRodado += (int)kmRodado;

                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;

                        repPneu.Atualizar(pneu);
                    }
                }
            }

            reboque.KilometragemAnterior = reboque.KilometragemAtual;            
            reboque.KilometragemAtual = reboque.KilometragemAtual + (int)kmRodado;

            repVeiculo.Atualizar(reboque, auditado, null, string.Concat("Atualizada a Quilometragem Atual do Reboque ",observacao));
        }
        #endregion
    }
}
