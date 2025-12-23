using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using HtmlAgilityPack;
namespace WindowsFormsApp9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string url = "https://www.gutenberg.org/cache/epub/1524/pg1524.txt";
            using (var client = new HttpClient())
            {
                try
                {
                    string text = await client.GetStringAsync(url);
                    listBox1.Items.Clear();

                    listBox1.Items.AddRange(text.Split(new[] { "\r\n\r\n" }, StringSplitOptions.None));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var books = new List<(string Title, string Url)>
            {
                 ("Гордость и предубеждение", "https://www.gutenberg.org/cache/epub/1342/pg1342.txt"),
            ("Робинзон Крузо", "https://www.gutenberg.org/cache/epub/822/pg822.txt"),
            ("Джейн Эйр", "https://www.gutenberg.org/cache/epub/1027/pg1027.txt"),
            ("Моби Дик", "https://www.gutenberg.org/cache/epub/2701/pg2701.txt"),
            ("Улисс", "https://www.gutenberg.org/cache/epub/author_id/5200.txt"), 
            ("Война и мир", "https://www.gutenberg.org/cache/epub/2600/pg2600.txt"),
            ("Анна Каренина", "https://www.gutenberg.org/cache/epub/13909/pg13909.txt"),
            ("Преступление и наказание", "https://www.gutenberg.org/cache/epub/2554/pg2554.txt"),
            ("Маленький принц", "https://www.gutenberg.org/cache/epub/19988/pg19988.txt"),
            ("Отверженные", "https://www.gutenberg.org/cache/epub/135/pg135.txt"),
            ("Гамлет", "https://www.gutenberg.org/cache/epub/1524/pg1524.txt"),
            ("Братья Карамазовы", "https://www.gutenberg.org/cache/epub/28054/pg28054.txt"),
            ("Автобиография", "https://www.gutenberg.org/cache/epub/7700/pg7700.txt"),
            ("Три мушкетёра", "https://www.gutenberg.org/cache/epub/1257/pg1257.txt"),
            ("Пена дней", "https://www.gutenberg.org/cache/epub/17080/pg17080.txt"),
            ("Сто лет одиночества", "https://www.gutenberg.org/cache/epub/58359/pg58359.txt"),
            ("Мастер и Маргарита", "https://www.gutenberg.org/cache/epub/42930/pg42930.txt"),
            ("Невинность", "https://www.gutenberg.org/cache/epub/33400/pg33400.txt"),
            ("О дивный новый мир", "https://www.gutenberg.org/cache/epub/51236/pg51236.txt"),
            ("Женщина в белом", "https://www.gutenberg.org/cache/epub/738/pg738.txt"),
            ("Пути фантазии", "https://www.gutenberg.org/cache/epub/62758/pg62758.txt"),
            ("Двадцать тысяч лье под водой", "https://www.gutenberg.org/cache/epub/16428/pg16428.txt"),
                
            };

            foreach (var book in books)
            {
                var item = new ListViewItem { Text = book.Title, Tag = book.Url };
                listBox1.Items.Add(item);
            }
        }
        //писать название на английском
        private async void button3_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите текст для поиска.");
                return;
            }

            listBox1.Items.Clear();

            string searchUrl = $"https://www.gutenberg.org/ebooks/search/?query={Uri.EscapeDataString(query)}&submit_search=Go%21";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string html = await client.GetStringAsync(searchUrl);

                    var results = ParseSearchResults(html);

                    if (results.Count == 0)
                    {
                        listBox1.Items.Add("Результаты не найдены.");
                    }
                    else
                    {
                        foreach (var result in results)
                        {
                            listBox1.Items.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }
        private List<string> ParseSearchResults(string html)
        {
            var results = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'booklink')]");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var titleNode = node.SelectSingleNode(".//span[@class='title']");
                    if (titleNode != null)
                    {
                        string title = titleNode.InnerText.Trim();
                        results.Add(title);
                    }
                }
            }
            return results;
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            string authorFirstName = textBox2.Text.Trim();
            string authorLastName = textBox3.Text.Trim();
            if (string.IsNullOrEmpty(authorFirstName) || string.IsNullOrEmpty(authorLastName))
            {
                MessageBox.Show("Пожалуйста, введите имя и фамилию автора.");
                return;
            }

            string fullAuthor = $"{authorFirstName} {authorLastName}";
            string searchUrl = $"https://www.gutenberg.org/ebooks/search/?query={Uri.EscapeDataString(fullAuthor)}&submit_search=Go%21";

            using (var client = new HttpClient())
            {
                try
                {
                    string html = await client.GetStringAsync(searchUrl);
                    var links = ParseAuthorBookLinks(html, fullAuthor);

                    if (links.Count == 0)
                    {
                        MessageBox.Show("Книги автора не найдены.");
                        return;
                    }
                    using (var fbd = new FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            string folderPath = fbd.SelectedPath;

                            foreach (var link in links)
                            {
                                await DownloadBookAsync(link, folderPath);
                            }
                            MessageBox.Show("Скачивание завершено.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
        private List<string> ParseAuthorBookLinks(string html, string authorFullName)
        {
            var links = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'booklink')]");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var authorNodes = node.SelectNodes(".//div[contains(@class, 'subtitle') or contains(text(),'" + authorFullName + "')]");
                    if (authorNodes != null)
                    {
                        string authorText = authorNodes[0].InnerText.ToLower();
                        if (authorText.Contains(authorFullName.ToLower()))
                        {
                            var linkNode = node.SelectSingleNode(".//a");
                            if (linkNode != null)
                            {
                                string href = linkNode.GetAttributeValue("href", "");
                                if (!string.IsNullOrEmpty(href))
                                {
                                    links.Add("https://www.gutenberg.org" + href);
                                }
                            }
                        }
                    }
                }
            }
            return links;
        }
        private async Task DownloadBookAsync(string url, string folderPath)
        {
            using (var client = new HttpClient())
            {
                string pageHtml = await client.GetStringAsync(url);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(pageHtml);

                var downloadLinkNode = doc.DocumentNode.SelectSingleNode("//a[contains(text(), 'Read this book online') or contains(@href, '.txt')]");

                if (downloadLinkNode != null)
                {
                    string downloadHref = downloadLinkNode.GetAttributeValue("href", "");
                    string downloadUrl = downloadHref.StartsWith("http") ? downloadHref : "https://www.gutenberg.org" + downloadHref;

                    string fileName = Path.Combine(folderPath, Path.GetFileName(downloadHref));
                    var bookContent = await client.GetStringAsync(downloadUrl);

                    File.WriteAllText(fileName, bookContent);
                }
            }
        }
    }
}
