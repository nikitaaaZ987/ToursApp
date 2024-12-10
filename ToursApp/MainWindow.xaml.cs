using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ToursApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HotelsPage());
            Manager.MainFrame = MainFrame;

            ImportTours();
        }

        private void ImportTours()
        {
            string filePath = @"C:\Users\Nikita\Desktop\Туры2.txt";
            string imagesPath = @"C:\Users\Nikita\Desktop\import до\Туры фото";

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Файл {filePath} не найден.");
                return;
            }

            if (!Directory.Exists(imagesPath))
            {
                MessageBox.Show($"Директория {imagesPath} не найдена.");
                return;
            }

            var fileData = File.ReadAllLines(filePath);
            var images = Directory.GetFiles(imagesPath);

            foreach (var line in fileData)
            {
                var data = line.Split('\t');

                if (data.Length < 6)
                {
                    MessageBox.Show($"Неверный формат строки: {line}");
                    continue;
                }

                var tempTour = new Tour
                {
                    Name = data[0].Replace("\"", ""),
                    TicketCount = int.Parse(data[2]),
                    Price = decimal.Parse(data[3]),
                    isActual = (data[4] == "0") ? false : true
                };

                foreach (var tourType in data[5].Replace("\"", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var currentType = ToursikEntities.GetContext().Type.ToList().FirstOrDefault(p => p.Name == tourType);
                    if (currentType != null)
                        tempTour.Type.Add(currentType);
                }

                try
                {
                    var imagePath = images.FirstOrDefault(p => p.Contains(tempTour.Name));
                    if (imagePath != null)
                    {
                        tempTour.ImagePreview = File.ReadAllBytes(imagePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении изображения для тура {tempTour.Name}: {ex.Message}");
                }

                ToursikEntities.GetContext().Tour.Add(tempTour);
                ToursikEntities.GetContext().SaveChanges();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}
