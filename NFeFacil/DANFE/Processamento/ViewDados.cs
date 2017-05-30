﻿using BibliotecaCentral;
using BibliotecaCentral.ModeloXML;
using BibliotecaCentral.ModeloXML.PartesProcesso;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesTotal;
using NFeFacil.DANFE.Pacotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NFeFacil.DANFE.Processamento
{
    public sealed class ViewDados
    {
        private ConexãoDados conec;
        public int TotalPaginas { get; private set; }
        private DadosPrimeiraPagina dadosPrimeiraPagina;
        private List<DadosOutrasPaginas> dadosOutrasPaginas = new List<DadosOutrasPaginas>();

        public ViewDados(ref WebView view, Processo processo)
        {
            conec = new ConexãoDados(ref view);
            AdicionarFuncoesPaginacao(Converter(processo));
        }

        void AdicionarFuncoesPaginacao(Geral dadosDanfe)
        {
            if (dadosDanfe._DadosProdutos.Length == 0)
                throw new ArgumentNullException(nameof(dadosDanfe._DadosProdutos));
            
            var nProdutosPagina1 = 18 - (dadosDanfe._Duplicatas == null ? 0 : dadosDanfe._Duplicatas.Length);
            var nProdutosPaginaRestante = 25;

            if (dadosDanfe._DadosProdutos.Length <= nProdutosPagina1)
            {
                TotalPaginas = 1;
            }
            else if (dadosDanfe._DadosProdutos.Length <= nProdutosPagina1 + nProdutosPaginaRestante)
            {
                TotalPaginas = 2;
            }
            else
            {
                double quantRestante = dadosDanfe._DadosProdutos.Length - (nProdutosPagina1 + nProdutosPaginaRestante);
                var nExtraQuebrado = quantRestante / nProdutosPaginaRestante;
                var resto = quantRestante % nProdutosPaginaRestante;
                TotalPaginas = (int)nExtraQuebrado + 2;
                if (resto > 0) TotalPaginas++;
            }

            var nItensPorPagina = new List<int>();
            if (TotalPaginas == 1)
            {
                nItensPorPagina.Add(nProdutosPagina1);
            }
            else if (TotalPaginas == 2)
            {
                nItensPorPagina.Add(nProdutosPagina1);
                nItensPorPagina.Add(nProdutosPaginaRestante);
            }
            else
            {
                nItensPorPagina.Add(nProdutosPagina1);
                for (int i = 1; i < TotalPaginas; i++)
                {
                    nItensPorPagina.Add(nProdutosPaginaRestante);
                }
            }

            var paginas = new Dictionary<int, List<DadosProduto>>();
            for (int i = 0; i < TotalPaginas; i++) paginas.Add(i, new List<DadosProduto>());

            var pagina = 0;
            foreach (var Produto in dadosDanfe._DadosProdutos)
            {
                if (paginas[pagina].Count == nItensPorPagina[pagina]) pagina++;
                paginas[pagina].Add(Produto);
            }

            for (int i = 0; i < paginas.Keys.Count; i++)
            {
                dadosDanfe._DadosNFe.DefinirPagina(TotalPaginas, i + 1);

                if (i == 0)
                {
                    dadosPrimeiraPagina = new DadosPrimeiraPagina
                    {
                        cabec = dadosDanfe._DadosCabecalho,
                        nfe = dadosDanfe._DadosNFe,
                        cliente = dadosDanfe._DadosCliente,
                        motorista = dadosDanfe._DadosMotorista,
                        imposto = dadosDanfe._DadosImposto,
                        Produto = paginas[i],
                        extras = dadosDanfe._DadosAdicionais,
                        paginaTotal = paginas.Keys.Count,
                        duplicatas = dadosDanfe._Duplicatas
                    };
                }
                else
                {
                    dadosOutrasPaginas.Add(new DadosOutrasPaginas
                    {
                        nfe = dadosDanfe._DadosNFe,
                        cliente = dadosDanfe._DadosCliente,
                        extras = dadosDanfe._DadosAdicionais,
                        paginaAtual = i + 1,
                        paginaTotal = paginas.Keys.Count,
                        Produto = paginas[i]
                    });
                }
            }
        }

        public async Task ExibirUmaPágina(int index)
        {
            await conec.ApagarCorpo();
            if (index == 0) await conec.AddPrimeiraPage(dadosPrimeiraPagina);
            else await conec.AddOutraPage(dadosOutrasPaginas[index - 1]);
        }

        internal static Geral Converter(Processo proc)
        {
            var dadosAdicionais = GetExtras(proc.NFe.Informações.infAdic);
            var dadosCabecalho = GetCabecalho(proc.NFe.Informações.identificação, proc.NFe.Informações.emitente);
            var dadosCliente = GetCliente(proc.NFe.Informações.identificação, proc.NFe.Informações.destinatário);
            var dadosImposto = GetImposto(proc.NFe.Informações.total);
            var dadosMotorista = GetMotorista(proc.NFe.Informações.transp);
            var dadosNFe = GetNFe(proc.NFe.Informações, proc.ProtNFe);
            var dadosProdutos = (from p in proc.NFe.Informações.produtos
                                 select GetProd(p)).ToArray();
            var duplicatas = proc.NFe.Informações.cobr?.Dup;
            var dadosDuplicatas = duplicatas != null ? (from d in duplicatas
                                                        select GetDuplicata(d)).ToArray() : new DadosDuplicata[0];
            var dadosISSQN = GetISSQN(proc.NFe.Informações.emitente, proc.NFe.Informações.total.ISSQNtot);
            return new Geral
            {
                _DadosAdicionais = dadosAdicionais,
                _DadosCabecalho = dadosCabecalho,
                _DadosCliente = dadosCliente,
                _DadosImposto = dadosImposto,
                _DadosMotorista = dadosMotorista,
                _DadosNFe = dadosNFe,
                _DadosProdutos = dadosProdutos,
                _Duplicatas = dadosDuplicatas
            };

            DadosISSQN GetISSQN(Emitente emit, ISSQNtot issqn)
            {
                return issqn != null ? new DadosISSQN
                {
                    BC = issqn.vBC.ToString(),
                    IM = emit.IM,
                    TotalServiços = issqn.vServ.ToString(),
                    ValorISSQN = issqn.vISS.ToString()
                } : new DadosISSQN();
            }

            DadosAdicionais GetExtras(InformacoesAdicionais extras)
            {
                return new DadosAdicionais
                {
                    Dados = Analisar(extras?.infCpl),
                    Fisco = Analisar(extras?.infAdFisco),
                };
            }

            DadosCabecalho GetCabecalho(Identificacao ident, Emitente emit)
            {
                return new DadosCabecalho
                {
                    NomeEmitente = emit.nome,
                    SerieNota = ident.Serie.ToString(),
                    NumeroNota = ident.Numero.ToString("000,000,000")
                };
            }

            DadosCliente GetCliente(Identificacao ident, Destinatario dest)
            {
                return new DadosCliente
                {
                    DocCliente = dest.Documento,
                    DataEmissao = Convert.ToDateTime(ident.DataHoraEmissão).ToString("dd-MM-yyyy"),
                    DataEntradaSaida = !string.IsNullOrEmpty(ident.DataHoraSaídaEntrada) ? Analisar(Convert.ToDateTime(ident.DataHoraSaídaEntrada).ToString("dd-MM-yyyy")) : string.Empty,
                    HoraEntradaSaida = !string.IsNullOrEmpty(ident.DataHoraSaídaEntrada) ? Analisar(Convert.ToDateTime(ident.DataHoraSaídaEntrada).ToString("hh:mm:ss")) : string.Empty,
                    Endereco = dest.endereco,
                    IECliente = Analisar(dest.inscricaoEstadual),
                    NomeCliente = dest.nome
                };
            }

            DadosImposto GetImposto(Total tot)
            {
                return new DadosImposto
                {
                    BaseCalculoICMS = tot.ICMSTot.vBC.ToString("0.00"),
                    BaseCalculoICMSST = tot.ICMSTot.vBCST.ToString("0.00"),
                    Desconto = tot.ICMSTot.vDesc.ToString("0.00"),
                    DespesasAcessorias = tot.ICMSTot.vOutro.ToString("0.00"),
                    TotalNota = tot.ICMSTot.vNF.ToString("0.00"),
                    ValorFrete = tot.ICMSTot.vFrete.ToString("0.00"),
                    ValorICMS = tot.ICMSTot.vICMS.ToString("0.00"),
                    ValorICMSST = tot.ICMSTot.vST.ToString("0.00"),
                    ValorIPI = tot.ICMSTot.vIPI.ToString("0.00"),
                    ValorSeguro = tot.ICMSTot.vSeg.ToString("0.00"),
                    ValorTotalProdutos = tot.ICMSTot.vProd.ToString("0.00")
                };
            }

            DadosMotorista GetMotorista(Transporte transp)
            {
                return new DadosMotorista
                {
                    CodigoANTT = Analisar(transp.veicTransp?.RNTC),
                    DocumentoMotorista = Analisar(transp.transporta?.Documento),
                    EnderecoMotorista = Analisar(transp.transporta?.XEnder),
                    EspecieVolume = Analisar(transp.vol.FirstOrDefault()?.esp),
                    IEMotorista = Analisar(transp.transporta?.InscricaoEstadual),
                    MarcaVolume = Analisar(transp.vol.FirstOrDefault()?.marca),
                    ModalidadeFrete = transp.modFrete.ToString(),
                    MunicipioMotorista = Analisar(transp.transporta?.XMun),
                    NomeMotorista = Analisar(transp.transporta?.Nome),
                    NumeroVolume = Analisar(transp.vol.FirstOrDefault()?.nVol),
                    PesoBrutoVolume = Analisar(transp.vol.FirstOrDefault()?.pesoB),
                    PesoLiquidoVolume = Analisar(transp.vol.FirstOrDefault()?.pesoL),
                    Placa = Analisar(transp.veicTransp?.Placa),
                    QuantidadeVolume = Analisar(transp.vol.FirstOrDefault()?.qVol),
                    UfMotorista = Analisar(transp.transporta?.UF),
                    UfPlaca = Analisar(transp.veicTransp?.UF)
                };
            }

            DadosNFe GetNFe(Detalhes detalhes, ProtocoloNFe prot)
            {
                var codigoBarras = detalhes.Id.Substring(detalhes.Id.IndexOf('e') + 1);
                return new DadosNFe
                {
                    Chave = codigoBarras,
                    ChaveComMascara = AplicarMascaraChave(codigoBarras),
                    CNPJEmit = detalhes.emitente.CNPJ,
                    DataHoraRecibo = prot.InfProt.dhRecbto.Replace('T', ' '),
                    Endereco = detalhes.emitente.endereco,
                    IE = detalhes.emitente.inscricaoEstadual,
                    IEST = detalhes.emitente.IEST,
                    NatOp = detalhes.identificação.NaturezaDaOperação,
                    NomeEmitente = detalhes.emitente.nome,
                    NumeroNota = detalhes.identificação.Numero.ToString(),
                    NumeroProtocolo = prot.InfProt.nProt.ToString(),
                    SerieNota = detalhes.identificação.Serie.ToString(),
                    TipoEmissao = detalhes.identificação.TipoEmissão.ToString(),
                };

                string AplicarMascaraChave(string original)
                {
                    var novaChave = "";
                    for (var i = 0; i < 44; i += 4)
                    {
                        novaChave += original.Substring(i, 4) + " ";
                    }
                    return novaChave;
                }
            }

            DadosProduto GetProd(DetalhesProdutos prod)
            {
                var consult = new ConsultarImpostos(prod.impostos.ToXElement<Impostos>());
                return new DadosProduto
                {
                    CFOP = prod.Produto.CFOP,
                    CProd = prod.Produto.CodigoProduto,
                    CSTICMS = prod.impostos.GetCSTICMS(),
                    NCM = prod.Produto.NCM,
                    QCom = prod.Produto.QuantidadeComercializada.ToString(),
                    UCom = prod.Produto.UnidadeComercializacao,
                    VUnCom = prod.Produto.ValorUnitario.ToString(),
                    BCICMS = consult.AgregarValor("vBC", 0).ToString(),
                    VProd = prod.Produto.ValorTotal.ToString(),
                    XProd = prod.Produto.Descricao,
                    PICMS = consult.AgregarValor("pICMS", 0).ToString(),
                    VICMS = consult.AgregarValor("vICMS", 0).ToString(),
                    PIPI = consult.AgregarValor("pIPI", 0).ToString(),
                    VIPI = consult.AgregarValor("vIPI", 0).ToString()
                };
            }

            DadosDuplicata GetDuplicata(Duplicata dup)
            {
                return new DadosDuplicata
                {
                    data = dup.DVenc,
                    n = dup.NDup,
                    valor = dup.DDup
                };
            }

            string Analisar(object str) => str != null ? (string)str : string.Empty;
        }
    }
}
