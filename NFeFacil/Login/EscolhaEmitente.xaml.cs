﻿using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.Login
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class EscolhaEmitente : Page
    {
        public EscolhaEmitente()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            using (var repo = new Repositorio.MEGACLASSE())
            {
                var dados = repo.ObterEmitentes();
                var conjuntos = new ObservableCollection<ConjuntoBasicoExibicao>();
                foreach (var atual in dados)
                {
                    var novoConjunto = new ConjuntoBasicoExibicao
                    {
                        Objeto = atual,
                        Principal = atual.Item1.NomeFantasia,
                        Secundario = atual.Item1.Nome,
                        Imagem = atual.Item2?.GetSource()
                    };
                    conjuntos.Add(novoConjunto);
                }
                grdEmitentes.ItemsSource = conjuntos;
            }
        }

        private void EmitenteEscolhido(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0];
                MainPage.Current.Navegar<GeralEmitente>(item);
            }
        }

        private void Cadastrar(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarEmitente>();
        }
    }

}
