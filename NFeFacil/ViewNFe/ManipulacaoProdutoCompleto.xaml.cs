﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using System;
using NFeFacil.Controles;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesProdutoOuServico;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using System.Collections.Generic;
using NFeFacil.View;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe
{
    [DetalhePagina(Symbol.Shop, "Produto")]
    public sealed partial class ManipulacaoProdutoCompleto : Page, IHambuguer, IValida
    {
        public DetalhesProdutos ProdutoCompleto { get; private set; }

        public ObservableCollection<DeclaracaoImportacao> ListaDI { get; } = new ObservableCollection<DeclaracaoImportacao>();
        public ObservableCollection<GrupoExportacao> ListaGE { get; } = new ObservableCollection<GrupoExportacao>();

        public ImpostoDevol ContextoImpostoDevol
        {
            get
            {
                if (ProdutoCompleto.ImpostoDevol == null)
                {
                    ProdutoCompleto.ImpostoDevol = new ImpostoDevol();
                }
                return ProdutoCompleto.ImpostoDevol;
            }
        }

        int TipoEspecialEscolhido
        {
            get
            {
                var prod = ProdutoCompleto.Produto;
                if (prod.veicProd != null)
                {
                    return 1;
                }
                else if (prod.medicamentos != null && prod.medicamentos.Count > 0)
                {
                    return 2;
                }
                else if (prod.armas != null && prod.armas.Count > 0)
                {
                    return 3;
                }
                else if (prod.comb != null)
                {
                    return 4;
                }
                else if (prod.NRECOPI != null)
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                var prod = ProdutoCompleto.Produto;
                switch (value)
                {
                    case 0:
                        prod.veicProd = null;
                        prod.medicamentos = null;
                        prod.armas = null;
                        prod.comb = null;
                        prod.NRECOPI = null;
                        break;
                    case 1:
                        MainPage.Current.Navegar<ProdutoEspecial.DefinirVeiculo>();
                        break;
                    case 2:
                        MainPage.Current.Navegar<ProdutoEspecial.DefinirMedicamentos>();
                        break;
                    case 3:
                        MainPage.Current.Navegar<ProdutoEspecial.DefinirArmamentos>();
                        break;
                    case 4:
                        MainPage.Current.Navegar<ProdutoEspecial.DefinirCombustivel>();
                        break;
                    case 5:
                        DefinirPapel();
                        break;
                    default:
                        break;
                }

                async void DefinirPapel()
                {
                    var caixa = new ProdutoEspecial.DefinirPapel(prod);
                    if (await caixa.ShowAsync() == ContentDialogResult.Primary)
                    {
                        prod.veicProd = null;
                        prod.medicamentos = null;
                        prod.armas = null;
                        prod.comb = null;
                        prod.NRECOPI = caixa.NRECOPI;
                    }
                }
            }
        }

        public ManipulacaoProdutoCompleto()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var produto = (DetalhesProdutos)e.Parameter;
            ProdutoCompleto = produto;
        }

        public ObservableCollection<ItemHambuguer> ConteudoMenu => new ObservableCollection<ItemHambuguer>
        {
            new ItemHambuguer(Symbol.Tag, "Dados"),
            new ItemHambuguer("\uE825", "Imposto devolvido"),
            new ItemHambuguer(Symbol.Comment, "Info adicional"),
            new ItemHambuguer(Symbol.World, "Importação"),
            new ItemHambuguer(Symbol.World, "Exportação"),
        };

        public int SelectedIndex { set => main.SelectedIndex = value; }

        public bool Concluido => false;

        private void Concluir_Click(object sender, RoutedEventArgs e)
        {
            ProdutoCompleto.Produto.DI = new List<DeclaracaoImportacao>(ListaDI);
            ProdutoCompleto.Produto.GrupoExportação = new List<GrupoExportacao>(ListaGE);

            var porcentDevolv = ProdutoCompleto.ImpostoDevol.pDevol;
            if (string.IsNullOrEmpty(porcentDevolv) || int.Parse(porcentDevolv) == 0)
            {
                ProdutoCompleto.ImpostoDevol = null;
            }
            MainPage.Current.Navegar<Impostos.EscolhaImpostos>(ProdutoCompleto);
        }

        async void AdicionarDeclaracaoImportacao(object sender, RoutedEventArgs e)
        {
            var caixa = new CaixasDialogoProduto.AdicionarDeclaracaoImportacao();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                ListaDI.Add(caixa.Declaracao);
            }
        }

        void RemoverDeclaracaoImportacao(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            ListaDI.Remove((DeclaracaoImportacao)contexto);
        }

        async void AdicionarDeclaracaoExportacao(object sender, RoutedEventArgs e)
        {
            var caixa = new CaixasDialogoProduto.EscolherTipoDeclaracaoExportacao();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                if (caixa.Direta)
                {
                    var caixa2 = new CaixasDialogoProduto.AddDeclaracaoExportacaoDireta();
                    if (await caixa2.ShowAsync() == ContentDialogResult.Primary)
                    {
                        ListaGE.Add(caixa2.Declaracao);
                    }
                }
                else
                {
                    var caixa2 = new CaixasDialogoProduto.AddDeclaracaoExportacaoIndireta();
                    if (await caixa2.ShowAsync() == ContentDialogResult.Primary)
                    {
                        ListaGE.Add(caixa2.Declaracao);
                    }
                }
            }
        }

        void RemoverDeclaracaoExportacao(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            ListaGE.Remove((GrupoExportacao)contexto);
        }
    }
}
